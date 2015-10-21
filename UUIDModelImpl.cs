using Org.Kevoree.Core.Api;
using System;
using org.kevoree;
using Org.Kevoree.Core.Api.IMarshalled;

namespace Org.Kevoree.Core
{
    class UUIDModelImpl : MarshalByRefObject, UUIDModel
    {
        private Guid guid;
        private IContainerRootMarshalled cc;

        public UUIDModelImpl(Guid guid, IContainerRootMarshalled cc)
        {
            this.guid = guid;
            this.cc = cc;
        }

        public Guid getUUID() { return this.guid; }

        public IContainerRootMarshalled getModel() { return this.cc; }

    }
}
