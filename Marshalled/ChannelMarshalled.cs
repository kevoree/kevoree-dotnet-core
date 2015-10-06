using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.Kevoree.Core.Api;
using Org.Kevoree.Core.Api.IMarshalled;

namespace Org.Kevoree.Core.Marshalled
{
    class ChannelMarshalled : MarshalByRefObject, IChannelMarshalled
    {
        private org.kevoree.Channel channel;

        public ChannelMarshalled(org.kevoree.Channel channel)
        {
            this.channel = channel;
        }

        public bool isOfType(Type t)
        {
            return this.channel.GetType().IsAssignableFrom(t);
        }

        public IKMFContainerMarshalled eContainer()
        {
            
            return new KMFContainerMarshalled(this.channel.eContainer());
        }

        public string getRefInParent()
        {
            return this.channel.getRefInParent();
        }

        public string path()
        {
            return this.channel.path();
        }

        public IMBindingMarshalled getMBinding()
        {
            throw new InvalidCastException("Channel does not have a getMBinding method !");
        }

        public IInstanceMarshalled getInstance()
        {
            throw new InvalidCastException("Channel does not have a getInstance method !");
        }

        public IFragmentDictionaryMarshalled getFragmentDictionary()
        {
            throw new InvalidCastException("Channel does not have a getFragmentDictionary method ! // OR TO IMPLEM");
        }
    }
}
