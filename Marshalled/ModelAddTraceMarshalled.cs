using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using org.kevoree.pmodeling.api.trace;
using Org.Kevoree.Core.Api;

namespace Org.Kevoree.Core.Marshalled
{
    class ModelAddTraceMarshalled : MarshalByRefObject, IModelAddTraceMarshalled
    {
        private readonly org.kevoree.pmodeling.api.trace.ModelAddTrace _modelAddTrace;

        public ModelAddTraceMarshalled(org.kevoree.pmodeling.api.trace.ModelAddTrace modelAddTrace)
        {
            this._modelAddTrace = modelAddTrace;
        }
        
        public string getRefName()
        {
            return this._modelAddTrace.getRefName();
        }

        public string getSrcPath()
        {
            return this._modelAddTrace.getSrcPath();
        }

        public org.kevoree.pmodeling.api.util.ActionType getTraceType()
        {
            return this._modelAddTrace.getTraceType();
        }

        public string toCString(bool b1, bool b2)
        {
            return this._modelAddTrace.toCString(b1, b2);
        }

        public string toString()
        {
            return this._modelAddTrace.toString();
        }

        public string getPreviousPath()
        {
            return this._modelAddTrace.getPreviousPath();
        }

    }
}
