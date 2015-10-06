using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.kevoree;
using Org.Kevoree.Core.Api;
using Org.Kevoree.Core.Api.IMarshalled;

namespace Org.Kevoree.Core.Marshalled
{
    public class MBindingMarshalled: MarshalByRefObject, IMBindingMarshalled
    {
        private MBinding deleg;

        public IPortMarshalled getPort()
        {
            return new PortMarshalled(this.deleg.getPort());
        }

        public MBindingMarshalled(MBinding deleg)
        {
            this.deleg = deleg;
        }

        public IChannelMarshalled getHub()
        {
            return new ChannelMarshalled(this.deleg.getHub());
        }

        public bool isOfType(Type t)
        {
            return this.deleg.GetType().IsAssignableFrom(t);
        }

        public IKMFContainerMarshalled eContainer()
        {
            return new KMFContainerMarshalled(this.deleg.eContainer());
        }

        public string getRefInParent()
        {
            return this.deleg.getRefInParent();
        }

        public string path()
        {
            return this.deleg.path();
        }

        public IMBindingMarshalled getMBinding()
        {
            throw new InvalidCastException("MBinding does not have a getMBinding method !");

        }

        public IInstanceMarshalled getInstance()
        {
            throw new InvalidCastException("MBinding does not have a getInstance method !");
        }

        public IFragmentDictionaryMarshalled getFragmentDictionary()
        {
            throw new InvalidCastException("MBinding does not have a getFragmentDictionary method ! // OR TO IMPLEM");
        }
    }
}
