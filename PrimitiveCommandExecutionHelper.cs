using model = org.kevoree;
using Org.Kevoree.Core.Api;
using Org.Kevoree.Core.Api.Adaptation;
using System;

namespace Org.Kevoree.Core
{
    class PrimitiveCommandExecutionHelper
    {
        public static bool execute(KevoreeCoreBean originCore, model.ContainerNode rootNode, AdaptationModel adaptionModel, NodeType nodeInstance, Func<bool> afterUpdateFunc, Func<bool> preRollBack, Func<bool> postRollback)
        {
            //Step orderedPrimitiveSet = adaptionModel;
            //if (orderedPrimitiveSet != null)
            //{

                /*
                val phase = if (orderedPrimitiveSet is ParallelStep) {
                    KevoreeParDeployPhase(originCore)
                } else {
                }
                */
                KevoreeDeployPhase phase = new KevoreeSeqDeployPhase(originCore);
                var res = executeStep(originCore, rootNode, adaptionModel, nodeInstance, phase, preRollBack);
                if (res)
                {
                    if (!afterUpdateFunc())
                    {
                        //CASE REFUSE BY LISTENERS
                        preRollBack();
                        phase.rollBack();
                        postRollback();
                    }
                }
                else
                {
                    postRollback();
                }
                return res;
            /*}
            /*else
            {
                return afterUpdateFunc();
            }*/
        }

        private static bool executeStep(KevoreeCoreBean originCore, model.ContainerNode rootNode, AdaptationModel step, NodeType nodeInstance, KevoreeDeployPhase phase, Func<bool> preRollBack)
        {
            if (step == null)
            {
                return true;
            }


            foreach (var abc in step)
            {
                var primitive = nodeInstance.getPrimitive((AdaptationPrimitive)abc);
                phase.populate(primitive);
            }

            return false;
            //originCore.broadcastTelemetry(TelemetryEvent.Type.DEPLOYMENT_STEP,step.getAdaptationType()!!.name(), null);



            /*Predicate<AdaptationPrimitive> pp = (p) => { return true; };*/
            /*var populateResult = step.TrueForAll((adapt) =>
            {
                var primitive = nodeInstance.getPrimitive(adapt);
                if (primitive != null)
                {
                    //Log.trace("Populate primitive => {} ", primitive)
                    phase.populate(primitive);
                    return true;
                }
                else
                {
                    //Log.warn("Error while searching primitive => {} ", adapt)
                    return false;
                }
            });

            */
            /*var phaseResult = phase.runPhase();
            if (phaseResult)
            {
                var nextStep = step.getNextStep();
                var subResult = false;
                if (nextStep != null)
                {
                    /*val nextPhase = if (nextStep is ParallelStep) {
                        KevoreeParDeployPhase(originCore)
                    } else {
                        KevoreeSeqDeployPhase(originCore)
                    }* /
                    var nextPhase = new KevoreeSeqDeployPhase(originCore);
                    phase.setSucessor(nextPhase);
                    subResult = executeStep(originCore, rootNode, nextStep, nodeInstance, nextPhase, preRollBack);
                }
                else
                {
                    subResult = true;
                }
                if (!subResult)
                {
                    preRollBack();
                    phase.rollBack();
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                preRollBack();
                phase.rollBack();
                return false;
            }*/
           
        }
    }
}
