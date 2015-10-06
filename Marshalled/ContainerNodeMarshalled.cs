using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.Kevoree.Core.Api;
using Org.Kevoree.Core.Api.IMarshalled;

namespace Org.Kevoree.Core.Marshalled
{
    class ContainerNodeMarshalled : MarshalByRefObject, IContainerNodeMarshalled
    {
        private readonly org.kevoree.ContainerNode _containerNode;

        public ContainerNodeMarshalled(org.kevoree.ContainerNode containerNode)
        {
            this._containerNode = containerNode;
        }

        public string path()
        {
            return this._containerNode.path();
        }

        public bool isOfType(Type t)
        {
            return this._containerNode.GetType().IsAssignableFrom(t);
        }

        public IKMFContainerMarshalled eContainer()
        {
            return new KMFContainerMarshalled(this._containerNode.eContainer());
        }

        public string getRefInParent()
        {
            return this._containerNode.getRefInParent();
        }

        public IMBindingMarshalled getMBinding()
        {
            throw new InvalidCastException("ContainerNode does not have a getMBinding method !");

        }

        public IInstanceMarshalled getInstance()
        {
            throw new InvalidCastException("ContainerNode does not have a getInstance method !");
        }

        public IFragmentDictionaryMarshalled getFragmentDictionary()
        {
            throw new InvalidCastException("ContainerNode does not have a getFragmentDictionary method ! // OR TO IMPLEM");
        }
    }
}
