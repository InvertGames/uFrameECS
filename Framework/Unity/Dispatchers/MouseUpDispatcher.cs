using uFrame.Attributes;

namespace uFrame.ECS
{
    [UFrameEventDispatcher("On Mouse Up")]
    public class MouseUpDispatcher : EcsDispatcher
    {
        public void OnMouseUp()
        {
            Publish(this);
        }
    }
}