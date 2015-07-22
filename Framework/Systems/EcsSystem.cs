using System;
using System.Collections.Generic;
using System.Linq;
using uFrame.IOC;
using uFrame.Kernel;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace uFrame.ECS
{
    public abstract class EcsSystem : SystemServiceMonoBehavior, IEcsSystem
    {
        [Inject]
        public IComponentSystem ComponentSystem { get; set; }

        public override void Setup()
        {
            base.Setup();
            //ComponentSystem.RegisterGroup<PlayerGroup>();
        }

        public void EnsureDispatcherOnComponents<TDispatcher>(params Type[] forTypes) where TDispatcher : EcsDispatcher
        {
            this.OnEvent<ComponentCreatedEvent>().Where(p => forTypes.Contains(p.Component.GetType()))
                .Subscribe(_ =>
                {
                    var component = _.Component as EcsComponent;
                    if (component == null) return;

                    var d = component.gameObject.GetComponent<TDispatcher>();
                    if (d != null) return;
                    var entityId = component.EntityId;

                    component.gameObject
                        .AddComponent<TDispatcher>()
                        .EntityId = entityId
                        ;
                })
                .DisposeWith(this);
        }
    }

    public static class EcsSystemExtensions
    {
        public static IEnumerable<IEcsComponent> MergeByEntity(this EcsSystem system, params IEcsComponentManager[] managers)
        {
            var list = new HashSet<int>();
            foreach (var manager in managers)
            {
                foreach (var item in manager.All)
                {
                    if (list.Contains(item.EntityId)) continue;
                    yield return item;
                    list.Add(item.EntityId);
                }
            }
        }
        public static void FilterWithDispatcher<TDispatcher>(this EcsSystem system, Func<TDispatcher, int> getMatchId, Action<int> handler, params Type[] forTypes)
            where TDispatcher : EcsDispatcher
        {
            system.OnEvent<ComponentCreatedEvent>().Where(p => forTypes.Contains(p.Component.GetType()))
                .Subscribe(_ =>
                {
                    var component = _.Component as EcsComponent;
                    if (component == null) return;

                    var d = component.gameObject.GetComponent<TDispatcher>();
                    if (d != null) return;
                    var entityId = component.EntityId;
                    
                    component.gameObject
                        .AddComponent<TDispatcher>()
                        .EntityId = entityId
                        ;

                    system.OnEvent<TDispatcher>()
                         .Where(p =>getMatchId(p) == component.EntityId)
                         .Subscribe(x => handler(x.EntityId))
                         .DisposeWith(system);
                })
                .DisposeWith(system);
        ;
        }

        public static IObservable<TComponentType> OnComponentCreated<TComponentType>(this IEventAggregator system) where TComponentType : class
        {
            return system.GetEvent<ComponentCreatedEvent>().Where(p => p.Component is TComponentType).Select(p => p.Component as TComponentType);
        }
        public static IObservable<TComponentType> OnComponentDestroyed<TComponentType>(this IEventAggregator system) where TComponentType : class
        {
            return system.GetEvent<ComponentDestroyedEvent>().Where(p => p.Component is TComponentType).Select(p => p.Component as TComponentType);
        }
        public static IObservable<TComponentType> OnComponentCreated<TComponentType>(this IEcsSystem system) where TComponentType : class
        {
            return system.OnEvent<ComponentCreatedEvent>().Where(p => p.Component is TComponentType).Select(p => p.Component as TComponentType);
        }
        public static IObservable<TComponentType> OnComponentDestroyed<TComponentType>(this IEcsSystem system) where TComponentType : class
        {
            return system.OnEvent<ComponentDestroyedEvent>().Where(p => p.Component is TComponentType).Select(p => p.Component as TComponentType);
        }
    }
}