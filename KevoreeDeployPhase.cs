using Org.Kevoree.Core.Api;
using Org.Kevoree.Core.Api.Command;

namespace Org.Kevoree.Core
{
    public interface KevoreeDeployPhase
    {
        void rollBack();
        bool runPhase();
        void populate(ICommand cmd);
        KevoreeDeployPhase getSucessor();
        void setSucessor(KevoreeDeployPhase kevoreeDeployPhase);
    }
}
