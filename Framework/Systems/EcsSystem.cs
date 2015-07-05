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

    public class FilterSystem<TGroupType> : EcsSystem where TGroupType : ComponentGroup
    {
        private List<int> _ids = new List<int>();

        public List<int> Ids
        {
            get { return _ids; }
            set { _ids = value; }
        }

        public IEnumerable<TGroupType> Components
        {
            get
            {
                return Manager.Components.Where(item => Filter(item));
            }
        }

        public bool Filter(TGroupType group)
        {
            return true;
        }

        public override void Setup()
        {
            base.Setup();

            this.OnComponentCreated<IEcsComponent>()
                .Where(p => !(p is ComponentGroup))// && !Ids.Contains(p.EntityId))
                .Subscribe(ComponentCreated)
                .DisposeWith(this);

            //this.OnComponentCreated<TGroupType>()
            //    .Subscribe(_ => Ids.Add(_.EntityId))
            //    .DisposeWith(this)
            //    ;

            //this.OnComponentDestroyed<TGroupType>()
            //    .Subscribe(_ => Ids.Remove(_.EntityId))
            //    .DisposeWith(this)
            //    ;


            //EventAggregator.OnComponentDestroyed<IEcsComponent>().Subscribe(ComponentDestroyed);
            Manager = this.ComponentSystem.RegisterComponent<TGroupType>();
        }

        public TGroupType CreateGroup(int entityId)
        {
            var c = this.gameObject.AddComponent<TGroupType>();
            return c;
        }

        public IEcsComponentManagerOf<TGroupType> Manager { get; set; }

        public virtual void ComponentCreated(IEcsComponent ecsComponent)
        {

        }

        public virtual void ComponentDestroyed(IEcsComponent ecsComponent)
        {

        }

        public virtual void GroupCreated(TGroupType groupType)
        {

        }

    }

    public class ComponentFilterSystem : EcsSystem
    {
        public Type[] WithAnyTypes { get; set; }


        public override void Setup()
        {
            base.Setup();


            //this.OnEvent<OnMouseDown>().Where(p=>p.SourceId == component.EntityId)
        }

        public virtual void Install(EcsComponent component)
        {


        }

        public void Handle(int entityId)
        {
            var item = entityId;

        }


    }

    public abstract class ComponentGroup : EcsComponent
    {

    }



}