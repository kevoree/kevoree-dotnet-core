using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using org.kevoree.pmodeling.api.trace;
using Org.Kevoree.Core.Api;

namespace Org.Kevoree.Core.Marshalled
{
    internal class ModelTraceMarshalled : MarshalByRefObject, IModelTraceMarshalled
    {
        private readonly ModelTrace _modelTrace;

        public ModelTraceMarshalled(ModelTrace modelTrace)
        {
            this._modelTrace = modelTrace;
        }

        public bool isOfType(Type a)
        {
            return this._modelTrace.GetType().IsAssignableFrom(a); // TODO : ou l'inverse ?
        }
        public string getRefName()
        {
            return this._modelTrace.getRefName();
        }

        public string getSrcPath()
        {
            return this._modelTrace.getSrcPath();
        }

        public org.kevoree.pmodeling.api.util.ActionType getTraceType()
        {
            return this._modelTrace.getTraceType();
        }

        public string toCString(bool b1, bool b2)
        {
            return this._modelTrace.toCString(b1, b2);
        }

        public string toString()
        {
            return this._modelTrace.toString();
        }

        public IModelAddTraceMarshalled getModelAddTrace()
        {
            return  new ModelAddTraceMarshalled((ModelAddTrace) this._modelTrace);
        }

        public IModelRemoveTraceMarshalled getModelRemoveTrace()
        {
            return new ModelRemoveTraceMarshalled((ModelRemoveTrace)this._modelTrace);
        }

        public IModelSetTraceMarshalled getModelSetTrace()
        {
            return new ModelSetTraceMarshalled((ModelSetTrace)this._modelTrace);
        }
    }
}