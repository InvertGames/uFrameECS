using uFrame.Kernel;

namespace uFrame.ECS
{
    public class EcsSystemLoader : SystemLoader
    {
        public override void Load()
        {
            base.Load();
            var componentSystem = new UnityComponentSystem();
            Container.RegisterInstance<IComponentSystem>(componentSystem);
            var entityManager = componentSystem.RegisterComponent<Entity>();
            Container.RegisterInstance(entityManager);

        }
    }
}