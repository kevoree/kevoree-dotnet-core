using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using org.kevoree.pmodeling.api.trace;
using Org.Kevoree.Core.Api;

namespace Org.Kevoree.Core.Marshalled
{
    class ModelRemoveTraceMarshalled : MarshalByRefObject, IModelRemoveTraceMarshalled
    {
        private readonly org.kevoree.pmodeling.api.trace.ModelRemoveTrace _modelRemoveTrace;

        public ModelRemoveTraceMarshalled(org.kevoree.pmodeling.api.trace.ModelRemoveTrace modelAddTrace)
        {
            this._modelRemoveTrace = modelAddTrace;
        }
        
        public string getRefName()
        {
            return this._modelRemoveTrace.getRefName();
        }

        public string getSrcPath()
        {
            return this._modelRemoveTrace.getSrcPath();
        }

        public org.kevoree.pmodeling.api.util.ActionType getTraceType()
        {
            return this._modelRemoveTrace.getTraceType();
        }

        public string toCString(bool b1, bool b2)
        {
            return this._modelRemoveTrace.toCString(b1, b2);
        }

        public string toString()
        {
            return this._modelRemoveTrace.toString();
        }

        public string getObjPath()
        {
            return this._modelRemoveTrace.getObjPath();
        }


    }
}
