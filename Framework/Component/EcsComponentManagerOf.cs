using System;
using System.Collections.Generic;
using System.Linq;
using uFrame.Kernel;
using UniRx;
using UnityEngine;

namespace uFrame.ECS
{
    public class EcsComponentManagerOf<TComponentType> : EcsComponentManager, IEcsComponentManagerOf<TComponentType> where TComponentType : class,IEcsComponent
    {
        Dictionary<int, List<TComponentType>> _components = new Dictionary<int, List<TComponentType>>();

        public virtual IEnumerable<TComponentType> Components
        {
            get
            {
                return _components.Values.SelectMany(p=>p);
            }
        }

        public override Type For
        {
            get { return typeof (TComponentType); }
        }

        public override IEnumerable<IEcsComponent> All
        {
            get
            {
                foreach (TComponentType component in Components)
                    yield return component;
            }
        }

        public TComponentType this[int entityId]
        {
            get
            {
                if (_components.ContainsKey(entityId))
                {
                    return _components[entityId].FirstOrDefault();
                }
                return null;
            }
        }

        public override IEnumerable<IEcsComponent> ForEntity(int entityId)
        {
            if (!_components.ContainsKey(entityId)) yield break;

            foreach (var item in _components[entityId])
            {
                yield return item;
            }
        }

        protected override void AddItem(IEcsComponent component)
        {
            if (!_components.ContainsKey(component.EntityId))
            {
                _components.Add(component.EntityId,new List<TComponentType>());
            }
            _components[component.EntityId].Add((TComponentType)component);
        }

        protected override void RemoveItem(IEcsComponent component)
        {
            if (!_components.ContainsKey(component.EntityId)) return;
            _components[component.EntityId].Remove((TComponentType)component);
        }
    }

    //public class FilteredComponentManagerOf<TComponentType> : EcsComponentManagerOf<TComponentType> where TComponentType : IEcsComponent
    //{
    //    public FilteredComponentManagerOf(Predicate<TComponentType> filter)
    //    {
    //        if (filter == null) throw new ArgumentNullException("filter","filter can't be null");
    //        Filter = filter;
    //    }

    //    public override IEnumerable<TComponentType> Components
    //    {
    //        get
    //        {
    //            for (int index = 0; index < _items.Count; index++)
    //            {
    //                var item = _items[index];
    //                if (Filter(item))
    //                {
    //                    yield return item;
    //                }
    //            }
    //        }
    //    }

    //    public Predicate<TComponentType> Filter { get; set; }
    //}

    public class ContextItemAdded<TContextItem>
    {
        public ContextItem ContextItem { get; set; }
    }
    public class ContextItemRemoved<TContextItem>
    {
        public ContextItem ContextItem { get; set; }
    }
    public interface IContext
    {
        IEcsComponentManager[] SelectManagers { get; set; }
        Type[] SelectTypes { get; set; }
        bool Match(int entityId);
    }

    public class Context<TContextItem> : IContext where TContextItem : class, new()
    {
        public Context()
        {
        }

        private readonly EcsSystem _system;

        public IEcsComponentManager[] WithAnyManagers { get; set; }
        public IEcsComponentManager[] SelectManagers { get; set; }

        public Context(EcsSystem system)
        {
            _system = system;
            ComponentSystem = _system.ComponentSystem;
            InitializeInternal();
        }

        private  void InitializeInternal()
        {
            
            Initialize();
            SelectTypes = SelectManagers.Select(p => p.For).ToArray();
        }

        public Type[] SelectTypes { get; set; }

        protected virtual void Initialize()
        {
            SelectManagers = GetSelectManagers().ToArray();
        }



        protected virtual IEnumerable<IEcsComponentManager> GetSelectManagers()
        {
            yield break;
        }

        public Context(EcsSystem system, IEcsComponentManager[] withAny, IEcsComponentManager[] selectManagers)
        {
            _system = system;
            ComponentSystem = _system.ComponentSystem;
     
            WithAnyManagers = withAny;
            SelectManagers = selectManagers;
        }



        public IComponentSystem ComponentSystem { get; set; }

        public IEnumerable<TContextItem> Items
        {
            get {
                if (WithAnyManagers == null || WithAnyManagers.Length < 1)
                {
                    // If we only want to work with components that have this type specifically
                    var first = SelectManagers.First();
                    foreach (var item in first.All)
                    {
                        if (Match(item.EntityId))
                        {
                            yield return Select();
                        }
                    }
                }
                else
                {
                    foreach (var ecsComponent in AnyItems())
                    {
                        if (Match(ecsComponent.EntityId))
                        {
                            yield return Select();
                        }
                    }
                }
                
                    
            }
        }

        private IEnumerable<IEcsComponent> AnyItems()
        {
            var list = new HashSet<int>();
            foreach (var manager in SelectManagers)
            {
                foreach (var item in manager.All)
                {
                    if (list.Contains(item.EntityId)) continue;
                    yield return item;
                    list.Add(item.EntityId);
                }
            }
        }

        public TContextItem MatchAndSelect(int entityId)
        {
            if (Match(entityId))
            {
                return Select();
            }
            return null;
        }

        public virtual bool Match(int entityId)
        {
            return true;
        

        }

        public virtual TContextItem Select()
        {
            return new TContextItem();
        }

    }

    public class ReactiveContext<TContextItem> : IContext where TContextItem : ContextItem, new()
    {
        protected readonly EcsSystem _system;

        private Dictionary<int, TContextItem> _items = new Dictionary<int,TContextItem>();
        public IEcsComponentManager[] SelectManagers { get; set; }
        public TContextItem MatchAndSelect(int entityId)
        {
            if (_items.ContainsKey(entityId))
            {
                return _items[entityId];
            }
            return null;
        }
        public IEnumerable<TContextItem> Items
        {
            get { return _items.Values; }
        }



        public Type[] SelectTypes
        {
            get; set; 
            
        }
        public Dictionary<int, TContextItem> ContextItems
        {
            get { return _items; }
            set { _items = value; }
        }

        private void Initialize()
        {
            SelectManagers = GetSelectManagers().ToArray();
        }

        protected virtual IEnumerable<IEcsComponentManager> GetWithAnyManagers()
        {
            yield break;
        }

        protected virtual IEnumerable<IEcsComponentManager> GetSelectManagers()
        {
            yield break;
        }
        public ReactiveContext(EcsSystem system)
        {
            _system = system;
            ComponentSystem = system.ComponentSystem;
            Initialize();
            SelectTypes = SelectManagers.Select(p => p.For).Concat(SelectManagers.Select(_ => _.For)).ToArray();

            system.OnEvent<ComponentCreatedEvent>()
                .Subscribe(_ =>
                {
                    var componentType = _.Component.GetType();
                    if (SelectTypes.Contains(componentType))
                    {
                        UpdateItem(_.Component.EntityId);
                    }
                }).DisposeWith(system);

            system.OnEvent<ComponentDestroyedEvent>()
                .Subscribe(_ =>
                {
                    var componentType = _.Component.GetType();
                    if (SelectTypes.Contains(componentType))
                    {
                        UpdateItem(_.Component.EntityId);
                    }
                    
                }).DisposeWith(system);

        }

        public IComponentSystem ComponentSystem { get; set; }

        public void UpdateItem(int entityId)
        {
            if (Match(entityId))
            {
                if (ContextItems.ContainsKey(entityId)) return;
                var item = Select();
                
                ContextItems.Add(entityId, item);
            }
            else
            {
                if (ContextItems.ContainsKey(entityId))
                {
                    ContextItems.Remove(entityId);

                }
            }
        }

        public virtual bool Match(int entityId)
        {
            return true;
        }
        public virtual TContextItem Select()
        {
            return new TContextItem();
        }
    }

    public class ContextItem : IEcsComponent
    {
        private Entity _entityView;

        public int EntityId { get; set; }

        public int ComponentId
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Entity EntityView
        {
            get { return _entityView ?? (_entityView = EntityService.GetEntityView(EntityId)); }
        }

    }

}

