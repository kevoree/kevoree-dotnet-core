using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.Kevoree.Core.Api;
using Org.Kevoree.Core.Api.IMarshalled;

namespace Org.Kevoree.Core.Marshalled
{
    class FragmentDictionaryMarshalled: MarshalByRefObject, IFragmentDictionaryMarshalled
    {
        private org.kevoree.FragmentDictionary fragmentDictionary;

        public FragmentDictionaryMarshalled(org.kevoree.FragmentDictionary fragmentDictionary)
        {
            this.fragmentDictionary = fragmentDictionary;
        }

        public string getName()
        {
            return this.fragmentDictionary.getName();
        }

        public bool isOfType(Type t)
        {
            return this.fragmentDictionary.GetType().IsAssignableFrom(t);
        }

        public IKMFContainerMarshalled eContainer()
        {
            return new KMFContainerMarshalled(this.fragmentDictionary.eContainer());
        }

        public string getRefInParent()
        {
            return this.fragmentDictionary.getRefInParent();
        }

        public string path()
        {
            return this.fragmentDictionary.path();
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
