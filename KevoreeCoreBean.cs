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


namespace Org.Kevoree.Core
{
    [Serializable]
    public class KevoreeCoreBean : ContextAwareModelService
    {
        private readonly KevoreeListeners modelListeners;
        private KevoreeFactory kevoreeFactory = new DefaultKevoreeFactory();
        private Org.Kevoree.Core.Api.NodeType nodeInstance;
        private BlockingCollection<Action> scheduler = new BlockingCollection<Action>(new ConcurrentQueue<Action>());
        private string nodeName;
        private ContainerRoot pending = null;
        private MethodAnnotationResolver resolver;
        private java.util.Date lastDate;
        private BootstrapService bootstrapService;
        private TupleLockCallBack currentLock;
        private volatile UUIDModel model;
        LinkedList<UUIDModel> models = new LinkedList<UUIDModel>();

        private ContextAwareModelServiceDelegate delegator = new ContextAwareModelServiceDelegate();

        public KevoreeCoreBean()
        {
            this.modelListeners = new KevoreeListeners(this);
        }

        public void setNodeName(string nodeName)
        {
            this.nodeName = nodeName;
        }

        private ContainerRoot cloneCurrentModel(ContainerRoot pmodel)
        {
            return (ContainerRoot)kevoreeFactory.createModelCloner().clone(pmodel, true);
        }

        public KevoreeFactory getFactory()
        {
            return this.kevoreeFactory;
        }

        private TupleLockCallBack getCurrentLock()
        {
            return this.currentLock;
        }

        private INodeRunner bootstrapNodeType(ContainerRoot model, String nodeName)
        {
            ContainerNode nodeInstance = model.findNodesByID(nodeName);
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


        private void checkBootstrapNode(ContainerRoot currentModel)
        {
            try
            {
                if (nodeInstance == null)
                {
                    ContainerNode foundNode = currentModel.findNodesByID(getNodeName());
                    if (foundNode != null)
                    {
                        nodeInstance = bootstrapNodeType(currentModel, getNodeName());
                        if (nodeInstance != null)
                        {
                            resolver = new MethodAnnotationResolver(nodeInstance.GetType());
                            //throw new NotImplementedException("ici faire le lancement de la méthode start     r la méthode trouvée par reflexion.");
                            nodeInstance.Start();

                            UUIDModelImpl uuidModel = new UUIDModelImpl(Guid.NewGuid(), kevoreeFactory.createContainerRoot());

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
                resolver = null;
            }
        }

        private void UpdateModelRunnable(ContainerRoot targetModel, Guid? uuid, UpdateCallback callback,
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


        private bool internalUpdateModel(ContainerRoot proposedNewModel, string callerPath)
        {
            if (proposedNewModel.findNodesByID(this.nodeName) == null)
            {
                return false;
            }
            try
            {
                ContainerRoot readOnlyNewModel = proposedNewModel;
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
                    currentModel = this.model.getModel();
                }
                else
                {
                    currentModel = null;
                }
                //Log.trace("Before listeners PreCheck !");
                UpdateContext updateContext = new UpdateContext(currentModel, readOnlyNewModel, callerPath);
                bool preCheckResult = modelListeners.preUpdate(updateContext);
                //Log.trace("PreCheck result = " + preCheckResult);
                //Log.trace("Before listeners InitUpdate !");
                bool initUpdateResult = modelListeners.initUpdate(updateContext);
                //Log.debug("InitUpdate result = " + initUpdateResult);
                if (preCheckResult && initUpdateResult)
                {
                    ContainerRoot newmodel = readOnlyNewModel;
                    // CHECK FOR HARA KIRI
                    ContainerRoot previousHaraKiriModel = null;
                    // Checks and bootstrap the node
                    checkBootstrapNode(newmodel);
                    if (this.model != null)
                    {
                        currentModel = this.model.getModel();
                    }
                    else
                    {
                        currentModel = null;
                    }
                    long milli = java.lang.System.currentTimeMillis();
                    /*if (Log.DEBUG) {
                        Log.debug("Begin update model {}", milli);
                    }*/
                    bool deployResult;
                    try
                    {
                        if (nodeInstance != null)
                        {
                            // Compare the two models and plan the adaptation
                            // Log.info("Comparing models and planning
                            // adaptation.")
                            AdaptationModel adaptationModel = nodeInstance.plan(currentModel, newmodel);
                            // Execution of the adaptation
                            // Log.info("Launching adaptation of the system.")
                            updateContext = new UpdateContext(currentModel, newmodel, callerPath);

                            UpdateContext final_updateContext = updateContext;
                            Func<bool> afterUpdateTest = () => { return modelListeners.afterUpdate(final_updateContext); };
                            Func<bool> postRollbackTest = () =>
                            {
                                modelListeners.postRollback(final_updateContext);
                                return true;
                            };
                            //PreCommand preCmd = new PreCommand(this, updateContext, modelListeners);

                            Func<bool> preCmdPreRollbackTest = getPreCmdPreRollbackTest(updateContext, modelListeners);

                            ContainerNode rootNode = newmodel.findNodesByID(getNodeName());
                            deployResult = PrimitiveCommandExecutionHelper.execute(this, rootNode,
                                    adaptationModel, nodeInstance, afterUpdateTest, preCmdPreRollbackTest,
                                    postRollbackTest);
                        }
                        else
                        {
                            deployResult = false;
                        }
                    }
                    catch (Exception)
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

        private void switchToNewModel(ContainerRoot c)
        {
            ContainerRoot cc = c;
            if (!c.isReadOnly())
            {
                //broadcastTelemetry(TelemetryEvent.Type.LOG_WARNING, "It is not safe to store ReadWrite model!", null);
                // Log.error("It is not safe to store ReadWrite model")
                cc = (ContainerRoot)kevoreeFactory.createModelCloner().clone(c, true);
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
                //Log.debug("Garbage old previous model");
            }
            // Changes the current model by the new model   
            if (cc != null)
            {
                UUIDModel uuidModel = new UUIDModelImpl(Guid.NewGuid(), cc);
                this.model = uuidModel;
                lastDate = new java.util.Date(java.lang.System.currentTimeMillis());
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



        /*private Action UpdateModelRunnable(ContainerRoot targetModel, Guid uuid, UpdateCallback callback,
				string callerPath)
        {
            throw new NotImplementedException();
        }*/





        public void start()
        {

            // TODO : 
            //modelListeners.start(getNodeName());
            //broadcastTelemetry(TelemetryEvent.Type.PLATFORM_START, "Kevoree Start event : node name = " + getNodeName(), null);

            Thread t = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    Action value = scheduler.Take();
                    Task.Run(value);
                }
            }));

            t.Start();

            //UUIDModelImpl uuidModel = new UUIDModelImpl(UUID.randomUUID(), kevoreeFactory.createContainerRoot());
            //model.set(uuidModel);
        }




        public void setBootstrapService(BootstrapService bootstrapService)
        {
            this.bootstrapService = bootstrapService;
        }


        private void UpdateSequenceRunnable(TraceSequence sequence, UpdateCallback callback, string callerPath)
        {
            try
            {
                ContainerRoot newModel = (ContainerRoot)kevoreeFactory.createModelCloner().clone(this.model.getModel(), false);
                sequence.applyOn(newModel);
                bool res = internalUpdateModel(cloneCurrentModel(newModel), callerPath);
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
                //Log.error("error while apply trace sequence", e)
                callback(false);
            }
        }

        private void UpdateScriptRunnable(string script, UpdateCallback callback, string callerPath)
        {
            try
            {
                ContainerRoot newModel = (ContainerRoot)kevoreeFactory.createModelCloner().clone(model.getModel(), false);
                new KevScriptEngine().execute(script, newModel);
                bool res = internalUpdateModel(cloneCurrentModel(newModel), callerPath);
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

                    // TODO ?
                    /*futurWatchDog.cancel(true);
                    futurWatchDog = null;
                    lockWatchDog.shutdownNow();
                    lockWatchDog = null;*/
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
                    //Log.error("Exception inside a LockCallback with argument {}, {}", t, null, true)
                }
            }
            else
            {
                Guid lockUUID = Guid.NewGuid();
                currentLock = new TupleLockCallBack(callBack, lockUUID);
                //lockWatchDog = java.util.concurrent.Executors.newSingleThreadScheduledExecutor();
                //futurWatchDog = lockWatchDog.schedule(new WatchDogCallable(), timeout, TimeUnit.MILLISECONDS);
                try
                {
                    callBack.run(lockUUID, false);
                }
                catch (Exception)
                {
                    //Log.error("Exception inside a LockCallback with argument {}, {}", t, lockUUID.toString(), false)
                }
            }
        }


        // START ContextAwareModelServiceImpl
        public UUIDModel getCurrentModel()
        {
            return model;
        }

        public ContainerRoot getPendingModel()
        {
            return pending;
        }

        public void compareAndSwap(ContainerRoot model, Guid uuid, UpdateCallback callback, String callerPath)
        {
            scheduler.Add(() => UpdateModelRunnable(cloneCurrentModel(model), uuid, callback, callerPath));
        }


        public void update(ContainerRoot model, UpdateCallback callback, string callerPath)
        {
            scheduler.Add(() => UpdateModelRunnable(cloneCurrentModel(model), null, callback, callerPath));
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
