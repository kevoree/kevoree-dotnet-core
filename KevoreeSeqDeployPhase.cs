using Org.Kevoree.Core.Api.Command;
using System.Collections.Generic;
using System.Linq;

namespace Org.Kevoree.Core
{
    class KevoreeSeqDeployPhase : KevoreeDeployPhase
    {
        private KevoreeCoreBean originCore;
        private KevoreeDeployPhase successor;

        private bool rollbackPerformed = false;

        private List<ICommand> primitives = new List<ICommand>();

        public KevoreeSeqDeployPhase(KevoreeCoreBean originCore)
        {
            // TODO: Complete member initialization
            this.originCore = originCore;
        }

        public KevoreeDeployPhase getSucessor()
        {
            return this.successor;
        }

        public void setSucessor(KevoreeDeployPhase successor)
        {
            this.successor = successor;
        }

        public void populate(ICommand cmd)
        {
            primitives.Add(cmd);
            rollbackPerformed = false;
        }
        public bool runPhase()
        {

            // TODO a prio inutile ?
            if (primitives.Count == 0)
            {
                return true;
            }
            ICommand lastPrimitive = null;
            try
            {
                var result = true;

                foreach (var primitive in primitives)
                {
                    lastPrimitive = primitive;
                    result = primitive.Execute();
                    if (!result)
                    {
                        break;
                    }
                }
                return result;
            }
            catch (java.lang.Throwable)
            {
                return false;
            }
        }

        public void rollBack()
        {
            //Log.trace("Rollback phase")
            if (successor != null)
            {
                //Log.trace("Rollback sucessor first")
                successor.rollBack();
            }
            if (!rollbackPerformed)
            {
                // SEQUENCIAL ROOLBACK
                foreach (var c in primitives.AsEnumerable().Reverse())
                {
                    try
                    {
                        //Log.trace("Undo adaptation command {} ", c.javaClass.getName())
                        c.Undo();
                    }
                    catch (java.lang.Exception)
                    {
                        //Log.warn("Exception during rollback", e)
                    }
                }
                rollbackPerformed = true;
            }
        }
    }

}
