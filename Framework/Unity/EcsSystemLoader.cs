using uFrame.Kernel;

namespace uFrame.ECS
{
    public class EcsSystemLoader : SystemLoader
    {
        public override void Load()
        {
            base.Load();
            Container.RegisterInstance<IComponentSystem>(new UnityComponentSystem());
        }
    }
}