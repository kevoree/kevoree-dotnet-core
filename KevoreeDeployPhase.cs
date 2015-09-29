using Org.Kevoree.Core.Api;

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
