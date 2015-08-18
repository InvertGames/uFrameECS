using System.Collections.Generic;
using uFrame.Kernel;

namespace uFrame.ECS
{
    public class EcsSystemLoader : SystemLoader
    {
        public List<EntityPrefabPool> Pools = new List<EntityPrefabPool>();
        public override void Load()
        {
            base.Load();
            Container.RegisterInstance<IComponentSystem>(this.AddSystem<EcsComponentService>());
            this.AddSystem<EntityService>().Pools = Pools;
        }
    }
}