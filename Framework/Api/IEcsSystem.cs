using uFrame.Kernel;

namespace uFrame.ECS
{
    public interface IEcsSystem : ISystemService
    {
        IComponentSystem ComponentSystem { get; }
    }
}