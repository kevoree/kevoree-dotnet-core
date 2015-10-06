using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.Kevoree.Core.Api;
using Org.Kevoree.Core.Api.IMarshalled;

namespace Org.Kevoree.Core.Marshalled
{
    class InstanceMarshalled : MarshalByRefObject, IInstanceMarshalled
    {
        private org.kevoree.Instance instance;

        public InstanceMarshalled(org.kevoree.Instance instance)
        {
            this.instance = instance;
        }

        public bool isOfType(Type t)
        {
            return this.instance.GetType().IsAssignableFrom(t); // TODO : ou l'inverse
        }

        public string getName()
        {
            return this.instance.getName();
        }

        public string path()
        {
            return this.path();
        }

        public IKMFContainerMarshalled eContainer()
        {
            return  new KMFContainerMarshalled(this.instance.eContainer());
        }

        public string getRefInParent()
        {
            return this.instance.getRefInParent();
        }

        public IMBindingMarshalled getMBinding()
        {
            throw  new InvalidCastException("Instance does not have a getMBinding method !");
        }

        public IInstanceMarshalled getInstance()
        {
            throw new InvalidCastException("Instance does not have a getInstance method !");
        }

        public IFragmentDictionaryMarshalled getFragmentDictionary()
        {
            throw new InvalidCastException("Instance does not have a getFragmentDictionary method ! // OR TO IMPLEM");
        }
    }
}
