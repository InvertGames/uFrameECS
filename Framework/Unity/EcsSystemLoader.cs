using System.Collections.Generic;
using System.Linq;
using uFrame.Kernel;

namespace uFrame.ECS
{
    public class EcsSystemLoader : SystemLoader
    {
        public List<EntityPrefabPool> Pools = new List<EntityPrefabPool>();
        private ISystemUpdate[] _items;

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
                if (_items == null)
                {
                    _items = uFrameKernel.Instance.Services.OfType<ISystemUpdate>().ToArray();
                }
                
                foreach (var item in _items)
                {
                    item.SystemUpdate();
                }
            }
        }
    }
}