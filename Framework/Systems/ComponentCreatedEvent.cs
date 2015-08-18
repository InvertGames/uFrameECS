using uFrame.Attributes;

namespace uFrame.ECS
{
    [uFrameEvent("Component Created")]
    public class ComponentCreatedEvent
    {
        public IEcsComponent Component { get; set; }
    }
}