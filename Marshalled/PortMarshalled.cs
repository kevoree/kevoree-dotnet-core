using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using java.util;
using org.kevoree;
using org.kevoree.pmodeling.api;
using org.kevoree.pmodeling.api.events;
using org.kevoree.pmodeling.api.util;
using Org.Kevoree.Core.Api;
using Org.Kevoree.Core.Api.IMarshalled;
//using PortMarshalled = org.kevoree.Port;

namespace Org.Kevoree.Core.Marshalled
{
    class PortMarshalled: MarshalByRefObject, IPortMarshalled
    {
        private org.kevoree.Port deleg;

        public bool isOfType(Type t)
        {
            return this.deleg.GetType().IsAssignableFrom(t);
        }

        public IPortMarshalled getPort()
        {
            throw new InvalidCastException("Port does not have a getPort method !");
        }

        public IMBindingMarshalled getMBinding()
        {
            throw new InvalidCastException("Port does not have a getMBinding method !");
        }

        public IInstanceMarshalled getInstance()
        {
            throw new InvalidCastException("Port does not have a getInstance method !");
        }

        public IFragmentDictionaryMarshalled getFragmentDictionary()
        {
            throw new InvalidCastException("Port does not have a getFragmentDictionary method !");
        }

        IKMFContainerMarshalled IKMFContainerMarshalled.eContainer()
        {
            return eContainer();
        }

        string IKMFContainerMarshalled.getRefInParent()
        {
            return getRefInParent();
        }

        string IKMFContainerMarshalled.path()
        {
            return path();
        }

        public PortMarshalled(org.kevoree.Port port)
        {
            this.deleg = port;
        }

        public void setRecursiveReadOnly()
        {
            deleg.setRecursiveReadOnly();
        }

        public IKMFContainerMarshalled eContainer()
        {
            return new KMFContainerMarshalled(deleg.eContainer());
        }

        public bool isReadOnly()
        {
            return deleg.isReadOnly();
        }

        public bool isRecursiveReadOnly()
        {
            return deleg.isRecursiveReadOnly();
        }

        public void setInternalReadOnly()
        {
            deleg.setInternalReadOnly();
        }

        public void delete()
        {
            deleg.delete();
        }

        public bool isDeleted()
        {
            return deleg.isDeleted();
        }

        public bool modelEquals(KMFContainer kmfc)
        {
            return deleg.modelEquals(kmfc);
        }

        public bool deepModelEquals(KMFContainer kmfc)
        {
            return deleg.deepModelEquals(kmfc);
        }

        public string getRefInParent()
        {
            return deleg.getRefInParent();
        }

        public KMFContainer findByPath(string str)
        {
            return deleg.findByPath(str);
        }

        public KMFContainer findByID(string str1, string str2)
        {
            return deleg.findByID(str1, str2);
        }

        public string path()
        {
            return deleg.path();
        }

        public string metaClassName()
        {
            return deleg.metaClassName();
        }

        public void reflexiveMutator(ActionType at, string str, object obj, bool b1, bool b2)
        {
            deleg.reflexiveMutator(at, str, obj, b1, b2);
        }

        public List @select(string str)
        {
            return deleg.@select(str);
        }

        public void addModelElementListener(ModelElementListener mel)
        {
            deleg.addModelElementListener(mel);
        }

        public void removeModelElementListener(ModelElementListener mel)
        {
            deleg.removeModelElementListener(mel);
        }

        public void removeAllModelElementListeners()
        {
            deleg.removeAllModelElementListeners();
        }

        public void addModelTreeListener(ModelElementListener mel)
        {
            deleg.addModelTreeListener(mel);
        }

        public void removeModelTreeListener(ModelElementListener mel)
        {
            deleg.removeModelTreeListener(mel);
        }

        public void removeAllModelTreeListeners()
        {
            deleg.removeAllModelTreeListeners();
        }

        public void visit(ModelVisitor mv, bool b1, bool b2, bool b3)
        {
            deleg.visit(mv, b1, b2, b3);
        }

        public void visitNotContained(ModelVisitor mv)
        {
            deleg.visitNotContained(mv);
        }

        public void visitContained(ModelVisitor mv)
        {
            deleg.visitContained(mv);
        }

        public void visitReferences(ModelVisitor mv)
        {
            deleg.visitReferences(mv);
        }

        public void deepVisitNotContained(ModelVisitor mv)
        {
            deleg.deepVisitNotContained(mv);
        }

        public void deepVisitContained(ModelVisitor mv)
        {
            deleg.deepVisitContained(mv);
        }

        public void deepVisitReferences(ModelVisitor mv)
        {
            deleg.deepVisitReferences(mv);
        }

        public void visitAttributes(ModelAttributeVisitor mav)
        {
            deleg.visitAttributes(mav);
        }

        public List createTraces(KMFContainer kmfc, bool b1, bool b2, bool b3, bool b4)
        {
            return deleg.createTraces(kmfc, b1, b2, b3, b4);
        }

        public List toTraces(bool b1, bool b2)
        {
            return deleg.toTraces(b1, b2);
        }

        public string internalGetKey()
        {
            return deleg.internalGetKey();
        }

        public bool isRoot()
        {
            return deleg.isRoot();
        }

        public string getName()
        {
            return deleg.getName();
        }

        public void setName(string str)
        {
            deleg.setName(str);
        }

        public NamedElement withName(string str)
        {
            return deleg.withName(str);
        }

        public List getBindings()
        {
            return deleg.getBindings();
        }

        public void setBindings(List l)
        {
            deleg.setBindings(l);
        }

        public PortMarshalled addBindings(MBinding mb)
        {
            return new PortMarshalled(deleg.addBindings(mb));
        }

        public PortMarshalled addAllBindings(List l)
        {
            return new PortMarshalled(deleg.addAllBindings(l));
        }

        public PortMarshalled removeBindings(MBinding mb)
        {
            return new PortMarshalled(deleg.removeBindings(mb));
        }

        public PortMarshalled removeAllBindings()
        {
            return new PortMarshalled(deleg.removeAllBindings());
        }

        public MBinding findBindingsByID(string str)
        {
            return deleg.findBindingsByID(str);
        }

        public PortTypeRef getPortTypeRef()
        {
            return deleg.getPortTypeRef();
        }

        public void setPortTypeRef(PortTypeRef ptr)
        {
            deleg.setPortTypeRef(ptr);
        }

        public PortMarshalled withPortTypeRef(PortTypeRef ptr)
        {
            return new PortMarshalled(deleg.withPortTypeRef(ptr));
        }
    }
}
