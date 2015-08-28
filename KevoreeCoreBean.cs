using org.kevoree;
using org.kevoree.factory;
using Org.Kevoree.Core.Api;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Org.Kevoree.Core.Api.Handler;
using Org.Kevoree.Core.Api.Adaptation;


namespace Org.Kevoree.Core
{
    public class KevoreeCoreBean
    {

        private readonly KevoreeListeners modelListeners;

        public KevoreeCoreBean()
        {
            this.modelListeners = new KevoreeListeners(this);
        }
        private KevoreeFactory kevoreeFactory = new DefaultKevoreeFactory();

        private Org.Kevoree.Core.Api.NodeType nodeInstance;


        private BlockingCollection<Action> scheduler = new BlockingCollection<Action>(new ConcurrentQueue<Action>());


        private string nodeName;

        private ContainerRoot pending = null;

        public string getNodeName()
        {
            return nodeName;
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

        public void update(ContainerRoot model, UpdateCallback callback, string callerPath)
        {

            scheduler.Add(() => UpdateModelRunnable(cloneCurrentModel(model), null, callback, callerPath));
        }

        private TupleLockCallBack currentLock;
        private volatile UUIDModel model;
        LinkedList<UUIDModel> models = new LinkedList<UUIDModel>();

        private TupleLockCallBack getCurrentLock()
        {
            return this.currentLock;
        }

        private Object bootstrapNodeType(ContainerRoot model, String nodeName)
        {
            ContainerNode nodeInstance = model.findNodesByID(nodeName);
            if (nodeInstance != null)
            {
                // TODO : ici charger le component
                //FlexyClassLoader kcl = bootstrapService.installTypeDefinition(nodeInstance.getTypeDefinition());
                //Object newInstance = bootstrapService.createInstance(nodeInstance, kcl);
                //bootstrapService.injectDictionary(nodeInstance, newInstance, false);
                throw new NotImplementedException("TODO : ici faire le chargement dynamique via NuGet (je crois)");
                //return null;
            }
            else
            {
                //broadcastTelemetry(TelemetryEvent.Type.LOG_ERROR, "Node not found using name " + nodeName, null);
                // Log.error("Node not found using name " + nodeName);
                return null;
            }
        }

        private MethodAnnotationResolver resolver;
        private java.util.Date lastDate;

        private void checkBootstrapNode(ContainerRoot currentModel)
        {
            try
            {
                if (nodeInstance == null)
                {
                    ContainerNode foundNode = currentModel.findNodesByID(getNodeName());
                    if (foundNode != null)
                    {
                        nodeInstance = (Org.Kevoree.Core.Api.NodeType)bootstrapNodeType(currentModel, getNodeName());
                        if (nodeInstance != null)
                        {
                            resolver = new MethodAnnotationResolver(nodeInstance.GetType());
                            throw new NotImplementedException("ici faire le lancement de la méthode start sur la méthode trouvée par reflexion.");
                            /*Method met = resolver.resolve(org.kevoree.annotation.Start.class);
                            met.invoke(nodeInstance);
                            UUIDModelImpl uuidModel = new UUIDModelImpl(UUID.randomUUID(),
                                    kevoreeFactory.createContainerRoot());
                            model.set(uuidModel);
                             * */
                        }
                        else
                        {
                            /*broadcastTelemetry(TelemetryEvent.Type.LOG_ERROR,
                                    "TypeDef installation fail. Node not found using name " + getNodeName(), null);*/
                            // Log.error("TypeDef installation fail !")
                        }
                    }
                    else
                    {
                        /*broadcastTelemetry(TelemetryEvent.Type.LOG_ERROR,
                                "Node instance name " + getNodeName() + " not found in bootstrap model !", null);*/
                        // Log.error("Node instance name {} not found in bootstrap
                        // model !", getNodeName())
                    }
                }
            }
            catch (java.lang.Throwable e)
            {
                /*broadcastTelemetry(TelemetryEvent.Type.LOG_ERROR, "Error while bootstraping node instance", e);*/
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
                catch (java.lang.Throwable ee)
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

        /*private UUIDModel model
        {
            return this.model;
        }*/

        private bool internalUpdateModel(ContainerRoot proposedNewModel, string callerPath)
        {
            if (proposedNewModel.findNodesByID(this.nodeName) == null)
            {
                /*broadcastTelemetry(TelemetryEvent.Type.LOG_ERROR, "Asking for update with a NULL model or node name ("
                    + getNodeName() + ") was not found in target model !", null);*/
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
                    /*if (/ *
                            * hkh.detectNodeHaraKiri(currentModel,
                            * readOnlyNewModel, getNodeName())
                            * /false) {
                        //broadcastTelemetry(TelemetryEvent.Type.LOG_WARNING, "HaraKiri detected , flush platform", null);
                        // Log.warn("HaraKiri detected , flush platform")
                        previousHaraKiriModel = currentModel;
                        // Creates an empty model, removes the current node
                        // (harakiri)
                        newmodel = kevoreeFactory.createContainerRoot();
                        try {
                            // Compare the two models and plan the adaptation
                            AdaptationModel adaptationModel = nodeInstance.plan(currentModel, newmodel);
                            / *if (Log.DEBUG) {
                                // Avoid the loop if the debug is not activated
                                Log.debug("Adaptation model size {}", adaptationModel.getAdaptations().size());
                            }* /
                            // Executes the adaptation
                            ContainerNode rootNode = currentModel.findNodesByID(getNodeName());
                            Func<bool> afterUpdateTest = () => { return true; };
                            PrimitiveCommandExecutionHelper.instance$.execute(this, rootNode, adaptationModel,
                                    nodeInstance, afterUpdateTest, afterUpdateTest, afterUpdateTest);
                            if (nodeInstance != null) {
                                Method met = resolver.resolve(org.kevoree.annotation.Stop.class);
                                met.invoke(nodeInstance);
                            }
                            // end of harakiri
                            nodeInstance = null;
                            resolver = null;
                            // place the current model as an empty model (for
                            // backup)

                            ContainerRoot backupEmptyModel = kevoreeFactory.createContainerRoot();
                            backupEmptyModel.setInternalReadOnly();
                            switchToNewModel(backupEmptyModel);

                            // prepares for deployment of the new system
                            newmodel = readOnlyNewModel;
                        } catch (Exception e) {
                            broadcastTelemetry(TelemetryEvent.Type.LOG_ERROR, "Error while updating!", e);
                            // Log.error("Error while update ", e);return false
                        }
                        Log.debug("End HaraKiri");
                    }*/
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
                            /*broadcastTelemetry(TelemetryEvent.Type.MODEL_COMPARE_AND_PLAN,
                                    "Comparing models and planning adaptation.", null);*/
                            AdaptationModel adaptationModel = nodeInstance.plan(currentModel, newmodel);
                            // Execution of the adaptation
                            // Log.info("Launching adaptation of the system.")
                            /*broadcastTelemetry(TelemetryEvent.Type.PLATFORM_UPDATE_START,
                                    "Launching adaptation of the system.", null);*/
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
                            //broadcastTelemetry(TelemetryEvent.Type.LOG_ERROR, "Node is not initialized", null);
                            // Log.error("Node is not initialized")
                            deployResult = false;
                        }
                    }
                    catch (Exception e)
                    {
                        //broadcastTelemetry(TelemetryEvent.Type.LOG_ERROR, "Error while updating", e);
                        // Log.error("Error while updating", e)
                        deployResult = false;
                    }
                    if (deployResult)
                    {
                        switchToNewModel(newmodel);
                        /*broadcastTelemetry(TelemetryEvent.Type.PLATFORM_UPDATE_SUCCESS, "Update sucessfully completed.", null); */
                        // Log.info("Update sucessfully completed.")
                    }
                    else
                    {
                        // KEEP FAIL MODEL, TODO
                        // Log.warn("Update failed")
                        /* broadcastTelemetry(TelemetryEvent.Type.PLATFORM_UPDATE_FAIL, "Update failed !", null); */
                        // IF HARAKIRI
                        if (previousHaraKiriModel != null)
                        {
                            internalUpdateModel(previousHaraKiriModel, callerPath);
                            previousHaraKiriModel = null; // CLEAR
                        }
                    }
                    long milliEnd = java.lang.System.currentTimeMillis() - milli;
                    //Log.info("End deploy result={}-{}", deployResult, milliEnd);
                    pending = null;
                    return deployResult;

                }
                else
                {
                    //Log.warn("PreCheck or InitUpdate Step was refused, update aborded !");
                    return false;
                }

            }
            catch (java.lang.Throwable e)
            {
                //broadcastTelemetry(TelemetryEvent.Type.LOG_ERROR, "Error while updating.", e);
                // Log.error("Error while update", e)
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



    }
}
