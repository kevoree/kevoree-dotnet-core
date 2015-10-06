using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using org.kevoree.pmodeling.api.trace;
using Org.Kevoree.Core.Api;

namespace Org.Kevoree.Core.Marshalled
{
    class ModelSetTraceMarshalled : MarshalByRefObject, IModelSetTraceMarshalled
    {
        private readonly org.kevoree.pmodeling.api.trace.ModelSetTrace _modelSetTrace;

        public ModelSetTraceMarshalled(org.kevoree.pmodeling.api.trace.ModelSetTrace modelSetTrace)
        {
            this._modelSetTrace = modelSetTrace;
        }
        
        public string getRefName()
        {
            return this._modelSetTrace.getRefName();
        }

        public string getSrcPath()
        {
            return this._modelSetTrace.getSrcPath();
        }

        public org.kevoree.pmodeling.api.util.ActionType getTraceType()
        {
            return this._modelSetTrace.getTraceType();
        }

        public string toCString(bool b1, bool b2)
        {
            return this._modelSetTrace.toCString(b1, b2);
        }

        public string toString()
        {
            return this._modelSetTrace.toString();
        }

        public string getContent()
        {
            return this._modelSetTrace.getContent();
        }
    }
}
