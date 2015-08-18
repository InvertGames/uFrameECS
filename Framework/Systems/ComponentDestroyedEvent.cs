using uFrame.Attributes;

namespace uFrame.ECS
{
    [uFrameEvent("Component Destroyed")]
    public class ComponentDestroyedEvent
    {
        public IEcsComponent Component { get; set; }
    }
}