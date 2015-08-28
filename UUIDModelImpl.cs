using Org.Kevoree.Core.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using org.kevoree;

namespace Org.Kevoree.Core
{
    class UUIDModelImpl : UUIDModel
    {
        private Guid guid;
        private ContainerRoot cc;

        public UUIDModelImpl(Guid guid, org.kevoree.ContainerRoot cc)
        {
            // TODO: Complete member initialization
            this.guid = guid;
            this.cc = cc;
        }

        public Guid getUUID() { return this.guid; }

        public ContainerRoot getModel() { return this.cc; }

    }
}
