using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using uFrame.Kernel;
using UniRx;

namespace uFrame.ECS
{
    public class EcsComponentManagerOf<TComponentType> : EcsComponentManager, IEcsComponentManagerOf<TComponentType> where TComponentType : IEcsComponent
    {
        protected List<TComponentType> _items = new List<TComponentType>();

        public virtual IEnumerable<TComponentType> Components
        {
            get
            {
                return _items;
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

        public override IEnumerable<IEcsComponent> ForEntity(int entityId)
        {
            foreach (var item in Components)
            {
                if (item.EntityId == entityId)
                    yield return item;
            }
        }

        public HashSet<int> _entities = new HashSet<int>();
        protected override void AddItem(IEcsComponent component)
        {
            _items.Add((TComponentType)component);
            if (!_entities.Contains(component.EntityId))
                _entities.Add(component.EntityId);
        }

        protected override void RemoveItem(IEcsComponent component)
        {
            _items.Remove((TComponentType)component);
            _entities.Remove(component.EntityId);
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
        IEcsComponentManager[] WithAnyManagers { get; set; }
        IEcsComponentManager[] SelectManagers { get; set; }
        Type[] WithAnyTypes { get; set; }
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
            if (WithAnyManagers != null && WithAnyManagers.Length > 0)
            {
                WithAnyTypes = WithAnyManagers.Select(p => p.For).ToArray();
            }
            else
            {
                WithAnyTypes = SelectManagers.Select(p => p.For).ToArray();
            }
            
        }

        protected virtual void Initialize()
        {
            WithAnyManagers = GetWithAnyManagers().ToArray();
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

        public Context(EcsSystem system, IEcsComponentManager[] withAny, IEcsComponentManager[] selectManagers)
        {
            _system = system;
            ComponentSystem = _system.ComponentSystem;
     
            WithAnyManagers = withAny;
            SelectManagers = selectManagers;
            if (WithAnyManagers != null && WithAnyManagers.Length > 0)
            {
                WithAnyTypes = WithAnyManagers.Select(p => p.For).ToArray();
            }
            else
            {
                WithAnyTypes = SelectManagers.Select(p => p.For).ToArray();
            }      
        }

        public Type[] WithAnyTypes { get; set; }

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
            foreach (var manager in WithAnyManagers)
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

    public class ReactiveContext<TContextItem> : IContext where TContextItem : class, new()
    {
        private readonly EcsSystem _system;

        private Dictionary<int, TContextItem> _items = new Dictionary<int,TContextItem>();
        public IEcsComponentManager[] WithAnyManagers { get; set; }
        public IEcsComponentManager[] SelectManagers { get; set; }
        public TContextItem MatchAndSelect(int entityId)
        {
            if (Match(entityId))
            {
                return Select();
            }
            return null;
        }
        public IEnumerable<TContextItem> Items
        {
            get { return _items.Values; }
        }

        public Type[] WithAnyTypes { get; set; }

        public Type[] AllTypes
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
       
            WithAnyManagers = GetWithAnyManagers().ToArray();
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
            AllTypes = WithAnyManagers.Select(p => p.For).Concat(SelectManagers.Select(_ => _.For)).ToArray();
            if (WithAnyManagers != null && WithAnyManagers.Length > 0)
            {
                WithAnyTypes = WithAnyManagers.Select(p => p.For).ToArray();
            }
            else
            {
                WithAnyTypes = SelectManagers.Select(p => p.For).ToArray();
            }  
            system.OnEvent<ComponentCreatedEvent>()
                .Subscribe(_ =>
                {
                    var componentType = _.Component.GetType();
                    if (AllTypes.Contains(componentType))
                    {
                        UpdateItem(_.Component.EntityId);
                    }
                }).DisposeWith(system);

            system.OnEvent<ComponentDestroyedEvent>()
                .Subscribe(_ =>
                {
                    var componentType = _.Component.GetType();
                    if (AllTypes.Contains(componentType))
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

        public Entity EntityView
        {
            get { return _entityView ?? (_entityView = EntityService.GetEntityView(EntityId)); }
        }

    }

}

