using org.kevoree;
using org.kevoree.pmodeling.api.trace;
using Org.Kevoree.Core.Api;
using Org.Kevoree.Core.Api.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Org.Kevoree.Core
{
    public interface ContextAwareModelService
    {
        UUIDModel getCurrentModel();

        ContainerRoot getPendingModel();

        void compareAndSwap(ContainerRoot model, Guid uuid, UpdateCallback callback, String callerPath);

        void update(ContainerRoot model, UpdateCallback callback, String callerPath);

        void registerModelListener(ModelListener listener, String callerPath);

        void unregisterModelListener(ModelListener listener, String callerPath);

        void acquireLock(LockCallBack callBack, long timeout, String callerPath);

        void releaseLock(Guid uuid, String callerPath);

        String getNodeName();

        void submitScript(String script, Org.Kevoree.Core.Api.UpdateCallback callback, String callerPath);

        void submitSequence(TraceSequence sequence, Org.Kevoree.Core.Api.UpdateCallback callback, String callerPath);
    }
}
