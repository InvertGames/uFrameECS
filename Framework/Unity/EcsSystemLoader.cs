using System.Collections.Generic;
using uFrame.Kernel;
using UnityEngine;

namespace uFrame.ECS
{
    public class EcsSystemLoader : SystemLoader
    {
        public List<EntityPrefabPool> Pools = new List<EntityPrefabPool>();
        public override void Load()
        {
            base.Load();

            var componentSystem = new UnityComponentSystem();
            Container.RegisterInstance<IComponentSystem>(componentSystem);
            var entityManager = componentSystem.RegisterComponent<Entity>();
            Container.RegisterInstance(entityManager);
            var parent = uFrameKernel.Instance.transform;
            var go = new GameObject("ComponentService", typeof (EcsComponentService));
            var es = new GameObject("EntityService", typeof (EntityService));
            es.GetComponent<EntityService>().Pools = Pools;
            es.transform.parent = parent;
            go.transform.parent = parent;

        }
    }

    public static class EcsSystemLoaderExtensions
    {
        public static ISystemService AddSystem<TSystemType>(this SystemLoader t) where TSystemType : Component, ISystemService
        {
            var parent = uFrameKernel.Instance.transform;
            var go = new GameObject(typeof(TSystemType).Name);
            go.transform.parent = parent;
            return go.AddComponent<TSystemType>();
        }
    }
}