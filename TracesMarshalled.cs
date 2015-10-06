using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using org.kevoree.pmodeling.api.trace;
using Org.Kevoree.Core.Api;
using Org.Kevoree.Core.Marshalled;

namespace Org.Kevoree.Core
{
    /**
     * Remplacer par une interfaces dans API et faire des redirections vers ce qui est utile dans TRACEsSequence tout en marshallant/serialisant
     * l'idée c'est que tout ce qui est passé ici doit être uniquement RO(donc "non mutable" dans le context du node)
     */
    class TracesMarshalled : MarshalByRefObject, ITracesSequence //: TraceSequence
    {
        private readonly TraceSequence _traces;

        public TracesMarshalled(TraceSequence traces)
        {
            this._traces = traces;
        }

        public List<IModelTraceMarshalled> GetTraces()
        {
        var afd = new System.Collections.Generic.List<IModelTraceMarshalled>();
            var aa = (_traces.getTraces().iterator());
            while (aa.hasNext())
            {
                var tmp = aa.next();
                afd.Add(new ModelTraceMarshalled((ModelTrace) tmp));
            }

            return afd.ToList();
        }

        
    }
}
