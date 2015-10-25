using model = org.kevoree;
using Org.Kevoree.Core.Api;
using Org.Kevoree.Core.Api.Adaptation;
using System;
using System.Collections.Generic;
using System.Linq;
using Org.Kevoree.Core.Api.IMarshalled;

namespace Org.Kevoree.Core
{
    class PrimitiveCommandExecutionHelper
    {
        public static bool execute(KevoreeCoreBean originCore, IContainerNodeMarshalled rootNode, AdaptationModel adaptionModel, NodeType nodeInstance, Func<bool> afterUpdateFunc, Func<bool> preRollBack, Func<bool> postRollback)
        {
            var processedActions = new HashSet<AdaptationPrimitive>();
            bool success = true;
            foreach (AdaptationPrimitive action in adaptionModel.ToArray())
            {
                processedActions.Add(action);
                var resultAction = processAction(action, nodeInstance);
                if (!resultAction)
                {
                    success = false;
                    break;
                }   
            }

            originCore.getLogger().Error("Adaptation failed");

            if (!success)
            {
                foreach (var act in processedActions.Reverse())
                {
                    processUndoAction(act, nodeInstance);
                }
            }
            return success;
        }

        private static bool processAction(AdaptationPrimitive action, NodeType nodeInstance)
        {
            var primitive = nodeInstance.getPrimitive(action);
            return primitive.Execute();
        }

        private static void processUndoAction(AdaptationPrimitive action, NodeType nodeInstance)
        {
            var primitive = nodeInstance.getPrimitive(action);
            primitive.Undo();
        }

    }
}
