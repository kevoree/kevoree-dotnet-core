using Org.Kevoree.Core.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Org.Kevoree.Core
{
    class KevoreeSeqDeployPhase: KevoreeDeployPhase
    {
        private KevoreeCoreBean originCore;
        private KevoreeDeployPhase successor;

        private bool rollbackPerformed = false;

        private List<PrimitiveCommand> primitives = new List<PrimitiveCommand>();

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

        public void populate(PrimitiveCommand cmd) {
            primitives.Add(cmd);
            rollbackPerformed = false;
        }
    public bool runPhase() {

        // TODO a prio inutile ?
        if (primitives.Count == 0) {
            return true;
        }
        PrimitiveCommand lastPrimitive = null;
        try {
            var result = true;

            foreach(var primitive in primitives) {
                lastPrimitive = primitive;
                result = primitive.execute();
                if(!result){
                    /*if(originCore.isAnyTelemetryListener()){
                        originCore.broadcastTelemetry(TelemetryEvent.Type.LOG_ERROR,"Cmd:["+primitive.toString()+"]",null)
                    }*/
                    //originCore.broadcastTelemetry("warn","Error during execution of "+primitive, e.toString())
                    //Log.warn("Error during execution of {}",primitive)
                    break;
                }
            }
            return result;
        } catch (java.lang.Throwable e){
            /*if(originCore.isAnyTelemetryListener()){
                try {
                    originCore.broadcastTelemetry(TelemetryEvent.Type.LOG_ERROR,"Cmd:["+lastPrimitive.toString()+"]",e)
                } catch (e: Throwable){
                   e.printStackTrace()
                }
            }*/
            //e.printStackTrace()
            return false;
        }
    }

        public void rollBack() {
        //Log.trace("Rollback phase")
        if (successor != null) {
            //Log.trace("Rollback sucessor first")
            successor.rollBack();
        }
        if(!rollbackPerformed){
            // SEQUENCIAL ROOLBACK
            foreach(var c in primitives.AsEnumerable().Reverse()){
                try {
                    //Log.trace("Undo adaptation command {} ", c.javaClass.getName())
                    c.undo();
                } catch (java.lang.Exception e) {
                    //originCore.broadcastTelemetry(TelemetryEvent.Type.LOG_ERROR,"Exception during rollback", e)
                    //Log.warn("Exception during rollback", e)
                }
            }
            rollbackPerformed = true;
        }
    }
    }

}
