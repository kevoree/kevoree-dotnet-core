using org.kevoree;
using org.kevoree.factory;
using Org.Kevoree.Core.Api;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Org.Kevoree.Core.Api.Handler;
using Org.Kevoree.Core.Api.Adaptation;
using org.kevoree.pmodeling.api.trace;
using org.kevoree.kevscript;
using org.kevoree.api.telemetry;
using System.Runtime.Remoting;
using Org.Kevoree.Core.Api.IMarshalled;
using Org.Kevoree.Core.Marshalled;
using org.kevoree.pmodeling.api.json;
using org.kevoree.pmodeling.api;


namespace Org.Kevoree.Core
{
    [Serializable]
    public class KevoreeCoreBean : MarshalByRefObject, ContextAwareModelService
    {
        private readonly KevoreeListeners modelListeners;
        private KevoreeFactory kevoreeFactory = new DefaultKevoreeFactory();
        private Org.Kevoree.Core.Api.NodeType nodeInstance;
        private BlockingCollection<Action> scheduler = new BlockingCollection<Action>(new ConcurrentQueue<Action>());
        private string nodeName;
        private IContainerRootMarshalled pending = null;
        //private MethodAnnotationResolver resolver;
        //private java.util.Date lastDate;
        private BootstrapService bootstrapService;
        private TupleLockCallBack currentLock;
        private volatile UUIDModel model;
        private readonly LinkedList<UUIDModel> models = new LinkedList<UUIDModel>();

        public KevoreeCoreBean()
        {
            this.modelListeners = new KevoreeListeners(this);
        }

        public void setNodeName(string nodeName)
        {
            this.nodeName = nodeName;
        }

        public KevoreeFactory getFactory()
        {
            return this.kevoreeFactory;
        }

        private TupleLockCallBack getCurrentLock()
        {
            return this.currentLock;
        }

        private INodeRunner bootstrapNodeType(IContainerRootMarshalled model, String nodeName)
        {
            var containerNode = model.findNodesByID(nodeName);
            ContainerNode nodeInstance = CloneContainerNode(containerNode);
            if (nodeInstance != null)
            {
                // TODO : ici charger le component
                //FlexyClassLoader kcl = bootstrapService.installTypeDefinition(nodeInstance.getTypeDefinition());
                //Object newInstance = bootstrapService.createInstance(nodeInstance, kcl);
                var newInstance = bootstrapService.createInstance(nodeInstance);
                bootstrapService.injectDictionary(nodeInstance, newInstance, false);
                //throw new NotImplementedException("TODO : ici faire le chargement dynamique via NuGet (je crois)");

                // scan pour une classe d'un type ou d'un autre, on garde toujours la première trouvée

                return newInstance;
            }
            else
            {
                return null;
            }
        }


        private void checkBootstrapNode(IContainerRootMarshalled currentModel)
        {
            try
            {
                if (nodeInstance == null)
                {
                    IContainerNodeMarshalled foundNode = currentModel.findNodesByID(getNodeName());
                    if (foundNode != null)
                    {
                        nodeInstance = bootstrapNodeType(currentModel, getNodeName());
                        if (nodeInstance != null)
                        {
                            nodeInstance.Start();


                            UUIDModelImpl uuidModel = new UUIDModelImpl(Guid.NewGuid(), new ContainerRootMarshalled(kevoreeFactory.createContainerRoot()));

                            // TODO : check for concurrency problems here.
                            this.model = uuidModel;
                        }
                    }
                }
            }
            catch (java.lang.Throwable)
            {
                // TODO is it possible to display the following log ?
                try
                {
                    if (nodeInstance != null)
                    {
                        // TODO : Mieux gérer les erreurs
                        /*Method met = resolver.resolve(org.kevoree.annotation.Stop.class);
                        met.invoke(nodeInstance);
                         */
                    }
                }
                catch (java.lang.Throwable)
                {
                }
                finally
                {
                }
                nodeInstance = null;
               // resolver = null;
            }
        }

        private void UpdateModelRunnable(IContainerRootMarshalled targetModel, Guid? uuid, UpdateCallback callback,
                string callerPath)
        {
            bool res = false;
            if (this.getCurrentLock() != null)
            {

                if (uuid.Equals(this.getCurrentLock().getGuid()))
                {
                    res = this.internalUpdateModel(targetModel, callerPath);
                }
                else
                {
                    //Log.debug("Core Locked , bad UUID {}", uuid);
                    res = false; // LOCK REFUSED !
                }
            }
            else
            {
                // COMMON CHECK
                if (uuid != null)
                {
                    if (this.model != null && uuid.Equals(this.model.getUUID()))
                    {
                        res = this.internalUpdateModel(targetModel, callerPath);
                    }
                    else
                    {
                        res = false;
                    }
                }
                else
                {
                    res = this.internalUpdateModel(targetModel, callerPath);
                }
            }
            bool finalRes = res;
            new Thread(new ThreadStart(() =>
            {
                if (callback != null)
                {
                    callback(finalRes);
                }
            })).Start();
        }


        private bool internalUpdateModel(IContainerRootMarshalled proposedNewModel, string callerPath)
        {
            if (proposedNewModel.findNodesByID(this.nodeName) == null)
            {
                return false;
            }
            try
            {
                var readOnlyNewModel = CloneContainerRoot(proposedNewModel);
                if (readOnlyNewModel.isReadOnly())
                {
                    readOnlyNewModel = (ContainerRoot)kevoreeFactory.createModelCloner().clone(readOnlyNewModel, false);
                    readOnlyNewModel.setGenerated_KMF_ID(nodeName + "@" + callerPath + "#" + java.lang.System.nanoTime());
                    readOnlyNewModel = (ContainerRoot)kevoreeFactory.createModelCloner().clone(readOnlyNewModel, true);
                }
                else
                {
                    readOnlyNewModel.setGenerated_KMF_ID(nodeName + "@" + callerPath + "#" + java.lang.System.nanoTime());
                }
                pending = proposedNewModel;
                // Model check is OK.
                ContainerRoot currentModel;
                if (this.model != null)
                {

                    var serialized = this.model.getModel().serialize();
                    var kf = new org.kevoree.factory.DefaultKevoreeFactory();
                    currentModel = (ContainerRoot)kf.createJSONLoader().loadModelFromString(serialized).get(0);
                }
                else
                {
                    currentModel = null;
                }
                UpdateContext updateContext = new UpdateContext(new ContainerRootMarshalled(currentModel), new ContainerRootMarshalled(readOnlyNewModel), callerPath);
                bool preCheckResult = modelListeners.preUpdate(updateContext);
                bool initUpdateResult = modelListeners.initUpdate(updateContext);
                if (preCheckResult && initUpdateResult)
                {
                    IContainerRootMarshalled newmodel = new ContainerRootMarshalled(readOnlyNewModel);
                    // CHECK FOR HARA KIRI
                    IContainerRootMarshalled previousHaraKiriModel = null;
                    // Checks and bootstrap the node
                    checkBootstrapNode(newmodel);
                    if (this.model != null)
                    {
                        var serialized = this.model.getModel().serialize();
                        var kf = new org.kevoree.factory.DefaultKevoreeFactory();
                        currentModel = (ContainerRoot)kf.createJSONLoader().loadModelFromString(serialized).get(0);
                    }
                    else
                    {
                        currentModel = null;
                    }
                    long milli = java.lang.System.currentTimeMillis();
                    
                    bool deployResult;
                    try
                    {
                        if (nodeInstance != null)
                        {
                            // Compare the two models and plan the adaptation
                            // Log.info("Comparing models and planning
                            // adaptation.")

                            var dkf = new DefaultKevoreeFactory();
                            var modelCompare = dkf.createModelCompare();

                            // TODO : clean up -> cloned
                            var newmodel2 = CloneContainerRoot(newmodel);

                            /*  start serialize model */
                            /*var kf = new org.kevoree.factory.DefaultKevoreeFactory();
                            var serialized = kf.createJSONSerializer().serialize(newmodel2);
                            Console.WriteLine(serialized);*/
                            /*  end serialize model */

                            var traces = modelCompare.diff(currentModel, newmodel2);
                            AdaptationModel adaptationModel = nodeInstance.plan(new ContainerRootMarshalled(currentModel), newmodel, new TracesMarshalled(traces));
                            // Execution of the adaptation
                            updateContext = new UpdateContext(new ContainerRootMarshalled(currentModel), new ContainerRootMarshalled(newmodel2), callerPath);

                            UpdateContext final_updateContext = updateContext;
                            Func<bool> afterUpdateTest = () => { return modelListeners.afterUpdate(final_updateContext); };
                            Func<bool> postRollbackTest = () =>
                            {
                                modelListeners.postRollback(final_updateContext);
                                return true;
                            };

                            Func<bool> preCmdPreRollbackTest = getPreCmdPreRollbackTest(updateContext, modelListeners);

                            IContainerNodeMarshalled rootNode = newmodel.findNodesByID(getNodeName());
                            deployResult = PrimitiveCommandExecutionHelper.execute(this, rootNode,
                                    adaptationModel, nodeInstance, afterUpdateTest, preCmdPreRollbackTest,
                                    postRollbackTest);

                            if (deployResult)
                            {
                                this.model = new UUIDModelImpl(Guid.NewGuid(), newmodel);
                            }
                        }
                        else
                        {
                            deployResult = false;
                        }
                    }
                    catch (Exception e)
                    {
                        deployResult = false;
                    }
                    if (deployResult)
                    {
                        switchToNewModel(newmodel);
                    }
                    else
                    {
                        // KEEP FAIL MODEL, TODO
                        // IF HARAKIRI
                        if (previousHaraKiriModel != null)
                        {
                            internalUpdateModel(previousHaraKiriModel, callerPath);
                            previousHaraKiriModel = null; // CLEAR
                        }
                    }
                    long milliEnd = java.lang.System.currentTimeMillis() - milli;
                    pending = null;
                    return deployResult;

                }
                else
                {
                    return false;
                }

            }
            catch (java.lang.Throwable)
            {
                return false;
            }
        }

        private ContainerRoot CloneContainerRoot(IContainerRootMarshalled newmodel)
        {
            var kf = new DefaultKevoreeFactory();
            JSONModelLoader loader = new JSONModelLoader(kf);
            var serialized = newmodel.serialize();
            Console.WriteLine("ContainerRoot >>>> " + serialized);
            return (ContainerRoot)loader.loadModelFromString(serialized).get(0);
        }

        private ContainerNode CloneContainerNode(IContainerNodeMarshalled newmodel)
        {


            return newmodel.getDelegate();
            /*
            var fac = new DefaultKevoreeFactory();
            var loader = fac.createJSONLoader();
            var serialized = newmodel.serialize();
            Console.WriteLine("ContainerNode >>>> " + serialized);
            var parsed = loader.loadModelFromString(serialized);
            return (ContainerNode)parsed.get(0);*/
        }

        private void switchToNewModel(IContainerRootMarshalled c)
        {
            ContainerRoot cc = CloneContainerRoot(c);
            if (!c.isReadOnly())
            {
                cc = (ContainerRoot)kevoreeFactory.createModelCloner().clone(cc, true);
            }
            // current model is backed-up
            UUIDModel previousModel = model;
            if (previousModel != null)
            {
                models.AddLast(previousModel);
            }
            // TODO : MAGIC NUMBER ;-) , ONLY KEEP 10 PREVIOUS MODEL
            if (models.Count > 15)
            {
                models.RemoveFirst();
            }
            // Changes the current model by the new model   
            if (cc != null)
            {
                UUIDModel uuidModel = new UUIDModelImpl(Guid.NewGuid(), new ContainerRootMarshalled(cc));
                this.model = uuidModel;
                // Fires the update to listeners
                modelListeners.notifyAllListener();
            }
        }

        private Func<bool> getPreCmdPreRollbackTest(UpdateContext updateContext, KevoreeListeners modelListeners)
        {
            var alreadyCalled = false;
            return () =>
            {
                if (!alreadyCalled)
                {
                    modelListeners.preRollback(updateContext);
                    alreadyCalled = true;
                }
                return true;
            };
        }
        public void start()
        {

            Thread t = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    Action value = scheduler.Take();
                    Task.Run(value);
                }
            }));

            t.Start();
        }




        public void setBootstrapService(BootstrapService bootstrapService)
        {
            this.bootstrapService = bootstrapService;
        }

        public BootstrapService getBootstrapService()
        {
            return this.bootstrapService;
        }


        private void UpdateSequenceRunnable(TraceSequence sequence, UpdateCallback callback, string callerPath)
        {
            try
            {
                string serialized = this.model.getModel().serialize();
                var kf = new org.kevoree.factory.DefaultKevoreeFactory();
                var newModel = (ContainerRoot)kf.createJSONLoader().loadModelFromString(serialized).get(0);
                sequence.applyOn(newModel);
                bool res = internalUpdateModel(new ContainerRootMarshalled(newModel), callerPath);
                new Thread(new ThreadStart(() =>
                {
                    if (callback != null)
                    {
                        callback(res);
                    }
                })).Start();
            }
            catch (Exception)
            {
                callback(false);
            }
        }

        private void UpdateScriptRunnable(string script, UpdateCallback callback, string callerPath)
        {
            try
            {
                var serialized = model.getModel().serialize();
                var kf = new org.kevoree.factory.DefaultKevoreeFactory();
                var newModel = (ContainerRoot)kf.createJSONLoader().loadModelFromString(serialized).get(0);
                new KevScriptEngine().execute(script, newModel);
                bool res = internalUpdateModel(new ContainerRootMarshalled(newModel), callerPath);
                new Thread(new ThreadStart(() =>
                {
                    if (callback != null)
                    {
                        callback(res);
                    }
                })).Start();
            }
            catch (Exception)
            {
                callback(false);
            }
        }


        private void ReleaseLockCallable(Guid uuid)
        {
            if (currentLock != null)
            {
                if (currentLock.getGuid() == uuid)
                {
                    currentLock = null;
                }
            }

        }

        private void AcquireLock(LockCallBack callBack, long timeout)
        {
            if (currentLock != null)
            {
                try
                {
                    callBack.run(null, true);
                }
                catch (Exception)
                {
                }
            }
            else
            {
                Guid lockUUID = Guid.NewGuid();
                currentLock = new TupleLockCallBack(callBack, lockUUID);

                try
                {
                    callBack.run(lockUUID, false);
                }
                catch (Exception)
                {
                    
                }
            }
        }


        // START ContextAwareModelServiceImpl
        public UUIDModel getCurrentModel()
        {
            return model;
        }

        public IContainerRootMarshalled getPendingModel()
        {
            return pending;
        }

        public void compareAndSwap(IContainerRootMarshalled model, Guid uuid, UpdateCallback callback, String callerPath)
        {
            scheduler.Add(() => UpdateModelRunnable(model, uuid, callback, callerPath));
        }


        public void update(IContainerRootMarshalled model, UpdateCallback callback, string callerPath)
        {
            scheduler.Add(() => UpdateModelRunnable(model, null, callback, callerPath));
        }

        public void registerModelListener(ModelListener listener, String callerPath)
        {
            modelListeners.addListener(listener);
        }

        public void unregisterModelListener(ModelListener listener, String callerPath)
        {
            modelListeners.removeListener(listener);
        }

        public void acquireLock(LockCallBack callBack, long timeout, String callerPath)
        {
            scheduler.Add(() => AcquireLock(callBack, timeout));
        }

        public void releaseLock(Guid uuid, String callerPath)
        {
            if (uuid != null)
            {
                if (scheduler != null)
                {
                    scheduler.Add(() => ReleaseLockCallable(uuid));
                }
            }
        }

        public string getNodeName()
        {
            return nodeName;
        }

        public void submitScript(String script, UpdateCallback callback, String callerPath)
        {
            if (script != null && currentLock == null)
            {
                scheduler.Add(() => UpdateScriptRunnable(script, callback, callerPath));
            }
            else
            {
                callback(false);
            }
        }

        public void submitSequence(TraceSequence sequence, UpdateCallback callback, String callerPath)
        {
            if (sequence != null && currentLock == null)
            {
                scheduler.Add(() => UpdateSequenceRunnable(sequence, callback, callerPath));
            }
            else
            {
                callback(false);
            }
        }
    }
}
