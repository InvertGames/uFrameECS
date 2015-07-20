using uFrame.Attributes;

namespace uFrame.ECS
{
    [SystemUFrameEvent("Update", "Update")]
    public interface ISystemUpdate
    {
        void Update();
    }
}