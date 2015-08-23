using System.Collections.Generic;
using System.Linq;
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
        
        public void Update()
        {
            if (uFrameKernel.IsKernelLoaded)
            {
                foreach (var item in uFrameKernel.Instance.Services.OfType<ISystemUpdate>())
                {
                    item.SystemUpdate();
                }
            }
        }
    }
}