using org.kevoree;
using Org.Kevoree.Core.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Org.Kevoree.Core
{
    class ContextAwareModelServiceDelegate: ContextAwareModelService
    {
        private UUIDModel model;
        private ContainerRoot pending;

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
