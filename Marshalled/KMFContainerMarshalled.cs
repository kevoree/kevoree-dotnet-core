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
    class KMFContainerMarshalled : MarshalByRefObject,  IKMFContainerMarshalled
    {
        private readonly  org.kevoree.pmodeling.api.KMFContainer _kMFContainer;

        public IMBindingMarshalled getMBinding()
        {
            return new MBindingMarshalled((MBinding)_kMFContainer);
        }

        public IInstanceMarshalled getInstance()
        {
            return new InstanceMarshalled((Instance)_kMFContainer);
        }

        public IFragmentDictionaryMarshalled getFragmentDictionary()
        {
            return new FragmentDictionaryMarshalled((FragmentDictionary) this._kMFContainer);
        }

        public KMFContainerMarshalled(org.kevoree.pmodeling.api.KMFContainer kMFContainer)
        {
            this._kMFContainer = kMFContainer;
        }

        public bool isOfType(Type t)
        {
            return this._kMFContainer.GetType().IsAssignableFrom(t); // TODO : ou l'inverse
        }

        public IKMFContainerMarshalled eContainer()
        {
            return new KMFContainerMarshalled(this._kMFContainer.eContainer());
        }

        public string getRefInParent()
        {
            return this._kMFContainer.getRefInParent();
        }

        public string path()
        {
            return this._kMFContainer.path();
        }
    }
}
