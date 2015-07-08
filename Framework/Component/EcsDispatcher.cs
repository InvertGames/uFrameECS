using uFrame.Kernel;

namespace uFrame.ECS
{
    public class EcsDispatcher : uFrameComponent, IEcsComponent
    {
        public int EntityId { get; set; }
    }
}