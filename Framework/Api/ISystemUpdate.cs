using uFrame.Attributes;

namespace uFrame.ECS
{
    [SystemUFrameEvent("Update", "SystemUpdate")]
    public interface ISystemUpdate
    {
        void SystemUpdate();
    }
}