using Org.Kevoree.Core.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Org.Kevoree.Core
{
    public interface KevoreeDeployPhase
    {
        void rollBack();
    bool runPhase();
    void populate(PrimitiveCommand cmd);
    KevoreeDeployPhase getSucessor();
    void setSucessor(KevoreeDeployPhase kevoreeDeployPhase);
    }
}
