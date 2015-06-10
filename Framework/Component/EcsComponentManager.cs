namespace uFrame.ECS
{
    public abstract class EcsComponentManager : IEcsComponentManager
    {
        public void RegisterComponent(IEcsComponent item)
        {
            AddItem(item);
        }

        public void UnRegisterComponent(IEcsComponent item)
        {
            RemoveItem(item);
        }

        protected abstract void AddItem(IEcsComponent component);
        protected abstract void RemoveItem(IEcsComponent component);
    }
}