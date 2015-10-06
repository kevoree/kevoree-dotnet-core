using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using org.kevoree;
using java.util;
using org.kevoree.pmodeling.api.events;
using org.kevoree.pmodeling.api;
using org.kevoree.pmodeling.api.util;
using Org.Kevoree.Core.Api;
using Org.Kevoree.Core.Api.IMarshalled;

namespace Org.Kevoree.Core.Marshalled
{
    class ContainerRootMarshalled : MarshalByRefObject, IContainerRootMarshalled
    {
        private org.kevoree.ContainerRoot deleg;

        

        public ContainerRootMarshalled(org.kevoree.ContainerRoot deleg)
        {
            this.deleg = deleg;
        }

        public bool isOfType(Type t)
        {
            return this.deleg.GetType().IsAssignableFrom(t);
        }

        IKMFContainerMarshalled IKMFContainerMarshalled.eContainer()
        {
            return new KMFContainerMarshalled(this.deleg.eContainer());
        }

        public IMBindingMarshalled getMBinding()
        {
            throw new InvalidCastException("ContainerRoot does not have a getMBinding method ! // OR TO IMPLEM");
        }

        public IInstanceMarshalled getInstance()
        {
            throw new InvalidCastException("ContainerRoot does not have a getInstance method !");
        }

        public IFragmentDictionaryMarshalled getFragmentDictionary()
        {
            throw new InvalidCastException("ContainerRoot does not have a getFragmentDictionary method !");
        }


        public ContainerRoot addAllGroups(List l)
        {
            return this.deleg.addAllGroups(l);
        }

        public ContainerRoot addAllHubs(List l)
        {
            return this.deleg.addAllHubs(l);
        }

        public ContainerRoot addAllMBindings(List l) { return this.deleg.addAllMBindings(l); }

        public ContainerRoot addAllNodes(List l)
        {
            return this.deleg.addAllNodes(l);
        }

        public ContainerRoot addAllPackages(List l)
        {
            return this.deleg.addAllPackages(l);
        }

        public ContainerRoot addAllRepositories(List l) { return this.deleg.addAllRepositories(l); }
        public ContainerRoot addGroups(Group g) { return this.deleg.addGroups(g); }
        public ContainerRoot addHubs(Channel c) { return this.deleg.addHubs(c); }
        public ContainerRoot addMBindings(MBinding mb) { return this.deleg.addMBindings(mb); }
        public ContainerRoot addNodes(ContainerNode cn) { return this.deleg.addNodes(cn); }
        public ContainerRoot addPackages(Package p) { return this.deleg.addPackages(p); }
        public ContainerRoot addRepositories(Repository r) { return this.deleg.addRepositories(r); }
        public Group findGroupsByID(string str) { return this.deleg.findGroupsByID(str); }
        public Channel findHubsByID(string str) { return this.deleg.findHubsByID(str); }
        public MBinding findMBindingsByID(string str) { return this.deleg.findMBindingsByID(str); }
        public IContainerNodeMarshalled findNodesByID(string str) { return new ContainerNodeMarshalled(this.deleg.findNodesByID(str)); }
        public Package findPackagesByID(string str) { return this.deleg.findPackagesByID(str); }
        public Repository findRepositoriesByID(string str) { return this.deleg.findRepositoriesByID(str); }
        public string getGenerated_KMF_ID() { return this.deleg.getGenerated_KMF_ID(); }

        public List getGroups() { return this.deleg.getGroups(); }

        public List getHubs() { return this.deleg.getHubs(); }

        public List getmBindings() { return this.deleg.getmBindings(); }

        public List getNodes() { return this.deleg.getNodes(); }

        public List getPackages() { return this.deleg.getPackages(); }

        public List getRepositories() { return this.deleg.getRepositories(); }
        public ContainerRoot removeAllGroups() { return this.deleg.removeAllGroups(); }
        public ContainerRoot removeAllHubs() { return this.deleg.removeAllHubs(); }
        public ContainerRoot removeAllMBindings() { return this.deleg.removeAllMBindings(); }
        public ContainerRoot removeAllNodes() { return this.deleg.removeAllNodes(); }
        public ContainerRoot removeAllPackages() { return this.deleg.removeAllPackages(); }
        public ContainerRoot removeAllRepositories() { return this.deleg.removeAllRepositories(); }
        public ContainerRoot removeGroups(Group g) { return this.deleg.removeGroups(g); }
        public ContainerRoot removeHubs(Channel c) { return this.deleg.removeHubs(c); }
        public ContainerRoot removeMBindings(MBinding mb) { return this.deleg.removeMBindings(mb); }
        public ContainerRoot removeNodes(ContainerNode cn) { return this.deleg.removeNodes(cn); }
        public ContainerRoot removePackages(Package p) { return this.deleg.removePackages(p); }
        public ContainerRoot removeRepositories(Repository r) { return this.deleg.removeRepositories(r); }
        public void setGenerated_KMF_ID(string str) { this.deleg.setGenerated_KMF_ID(str); }
        public void setGroups(List l) { this.deleg.setGroups(l); }

        public void setHubs(List l) { this.deleg.setHubs(l); }

        public void setmBindings(List l) { this.deleg.setmBindings(l); }

        public void setNodes(List l) { this.deleg.setNodes(l); }

        public void setPackages(List l) { this.deleg.setPackages(l); }

        public void setRepositories(List l) { this.deleg.setRepositories(l); }
        public ContainerRoot withGenerated_KMF_ID(string str) { return this.deleg.withGenerated_KMF_ID(str); }





        public void addModelElementListener(ModelElementListener mel) { this.deleg.addModelElementListener(mel); }
        public void addModelTreeListener(ModelElementListener mel) { this.deleg.addModelTreeListener(mel); }

        public List createTraces(KMFContainer kmfc, bool b1, bool b2, bool b3, bool b4) { return this.deleg.createTraces(kmfc, b1, b2, b3, b4); }
        public bool deepModelEquals(KMFContainer kmfc) { return this.deleg.deepModelEquals(kmfc); }
        public void deepVisitContained(ModelVisitor mv) { this.deleg.deepVisitContained(mv); }
        public void deepVisitNotContained(ModelVisitor mv) { this.deleg.deepVisitNotContained(mv); }
        public void deepVisitReferences(ModelVisitor mv) { this.deleg.deepVisitReferences(mv); }
        public void delete() { this.deleg.delete(); }
        public KMFContainer eContainer() { return this.deleg.eContainer(); }
        public KMFContainer findByID(string str1, string str2) { return this.deleg.findByID(str1, str2); }
        public IKMFContainerMarshalled findByPath(string str) { return new KMFContainerMarshalled(this.deleg.findByPath(str)); }
        public string getRefInParent() { return this.deleg.getRefInParent(); }
        public string internalGetKey() { return this.deleg.internalGetKey(); }
        public bool isDeleted() { return this.deleg.isDeleted(); }
        public bool isReadOnly() { return this.deleg.isReadOnly(); }
        public bool isRecursiveReadOnly() { return this.deleg.isRecursiveReadOnly(); }
        public bool isRoot() { return this.deleg.isRoot(); }
        public string metaClassName() { return this.deleg.metaClassName(); }
        public bool modelEquals(KMFContainer kmfc) { return this.deleg.modelEquals(kmfc); }
        public string path() { return this.deleg.path(); }
        public void reflexiveMutator(ActionType at, string str, object obj, bool b1, bool b2) { this.deleg.reflexiveMutator(at, str, obj, b1, b2); }
        public void removeAllModelElementListeners() { this.deleg.removeAllModelElementListeners(); }
        public void removeAllModelTreeListeners() { this.deleg.removeAllModelTreeListeners(); }
        public void removeModelElementListener(ModelElementListener mel) { this.deleg.removeModelElementListener(mel); }
        public void removeModelTreeListener(ModelElementListener mel) { this.deleg.removeModelTreeListener(mel); }

        public List select(string str) { return this.deleg.select(str); }
        public void setInternalReadOnly() { this.deleg.setInternalReadOnly(); }
        public void setRecursiveReadOnly() { this.deleg.setRecursiveReadOnly(); }

        public List toTraces(bool b1, bool b2) { return this.deleg.toTraces(b1, b2); }
        public void visit(ModelVisitor mv, bool b1, bool b2, bool b3) { this.deleg.visit(mv, b1, b2, b3); }
        public void visitAttributes(ModelAttributeVisitor mav) { this.deleg.visitAttributes(mav); }
        public void visitContained(ModelVisitor mv) { this.deleg.visitContained(mv); }
        public void visitNotContained(ModelVisitor mv) { this.deleg.visitNotContained(mv); }
        public void visitReferences(ModelVisitor mv) { this.deleg.visitReferences(mv); }
    }
}
