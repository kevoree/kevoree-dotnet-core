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
            //var cpt = adaptionModel.Count();
            //Console.WriteLine("DEBUG : " +cpt + " adaptations planned");
            foreach (AdaptationPrimitive action in adaptionModel.ToArray())
            {
                processedActions.Add(action);
                if (!processAction(action, nodeInstance))
                {
                    success = false;
                    break;
                }
                
            }

            if (!success)
            {
                // TODO : process all undo actions in reverse order !
                foreach (var act in processedActions.Reverse())
                {
                    processAction(act, nodeInstance);
                }
            }
            return success;
        }

        private static bool processAction(AdaptationPrimitive action, NodeType nodeInstance)
        {
            var primitive = nodeInstance.getPrimitive(action);
            Console.WriteLine(primitive.ToString() + " " + primitive.Name());
            return primitive.Execute();
        }

    }
}
