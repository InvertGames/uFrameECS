using uFrame.Attributes;

namespace uFrame.ECS
{
    [UFrameEventDispatcher("On Mouse Down"), uFrameCategory("Unity Messages")]
    public class MouseDownDispatcher : EcsDispatcher
    {
        public void OnMouseDown()
        {
            Publish(this);
        }
    }
    [UFrameEventDispatcher("On Mouse Drag"), uFrameCategory("Unity Messages")]
    public class MouseDragDispatcher : EcsDispatcher
    {
        public void OnMouseDrag()
        {
            Publish(this);
        }
    }
    [UFrameEventDispatcher("On Mouse Enter"), uFrameCategory("Unity Messages")]
    public class MouseEnterDispatcher : EcsDispatcher
    {
        public void OnMouseEnter()
        {
            Publish(this);
        }
    }
    [UFrameEventDispatcher("On Mouse Exit"), uFrameCategory("Unity Messages")]
    public class MouseExitDispatcher : EcsDispatcher
    {
        public void OnMouseExit()
        {
            Publish(this);
        }
    }

    [UFrameEventDispatcher("On Became Invisible"), uFrameCategory("Unity Messages")]
    public class BecameInvisibleDispatcher : EcsDispatcher
    {
        public void OnBecameInvisible()
        {
            Publish(this);
        }
    }

    [UFrameEventDispatcher("On Became Visible"), uFrameCategory("Unity Messages")]
    public class BecameVisibleDispatcher : EcsDispatcher
    {
        public void OnBecameVisible()
        {
            Publish(this);
        }
    }
}