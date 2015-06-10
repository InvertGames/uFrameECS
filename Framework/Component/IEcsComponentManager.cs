namespace uFrame.ECS
{
    public interface IEcsComponentManager
    {
        void RegisterComponent(IEcsComponent item);
        void UnRegisterComponent(IEcsComponent item);
    }
}