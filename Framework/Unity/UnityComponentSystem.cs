using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace uFrame.ECS
{
    public class UnityComponentSystem : IComponentSystem
    {

        private Dictionary<Type, IEcsComponentManager> _componentManager;

        public LinkedList<Component> Components { get; set; }

        public Dictionary<Type, IEcsComponentManager> ComponentManagers
        {
            get { return _componentManager ?? (_componentManager = new Dictionary<Type, IEcsComponentManager>()); }
            set { _componentManager = value; }
        }

        public IEcsComponentManager RegisterComponent(Type componentType)
        {
            IEcsComponentManager existing;
            if (!ComponentManagers.TryGetValue(componentType, out existing))
            {
                throw new Exception(string.Format("Component {0} not registered correctly.",componentType.Name));
             
            }
            return existing;
        }

        public void RegisterComponentInstance(Type componentType, IEcsComponent instance)
        {
            IEcsComponentManager existing;
            if (!ComponentManagers.TryGetValue(componentType, out existing))
            {
                var type = typeof(EcsComponentManagerOf<>).MakeGenericType(componentType);
                existing = Activator.CreateInstance(type) as EcsComponentManager;
                ComponentManagers.Add(componentType, existing);
            }
            existing.RegisterComponent(instance);
        }
        public void DestroyComponentInstance(Type componentType, IEcsComponent instance)
        {
            IEcsComponentManager existing;
            if (!ComponentManagers.TryGetValue(componentType, out existing))
            {
                return;
            }
            existing.UnRegisterComponent(instance);

        }

        public void AddComponent(int entityId, Type componentType)
        {
            var entityManager = RegisterComponent<Entity>();
            var entity = entityManager.Components.FirstOrDefault(p => p.EntityId == entityId);


        }

        public void AddComponent<TComponentType>(int entityId) where TComponentType : IEcsComponent
        {
            throw new NotImplementedException();
        }

        public IEcsComponentManagerOf<TComponent> RegisterComponent<TComponent>() where TComponent : IEcsComponent
        {
            IEcsComponentManager existing;
            if (!ComponentManagers.TryGetValue(typeof(TComponent), out existing))
            {
                existing = new EcsComponentManagerOf<TComponent>();
                ComponentManagers.Add(typeof(TComponent), existing);
                return (IEcsComponentManagerOf<TComponent>)existing;
            }
            else
            {
                return (IEcsComponentManagerOf<TComponent>)existing;
            }

        }

        public bool TryGetComponent<TComponent>(int[] entityIds, out TComponent[] components) where TComponent : class, IEcsComponent
        {
            var list = new List<TComponent>();
            foreach (var entityid in entityIds)
            {
                TComponent component;
                if (!TryGetComponent(entityid, out component))
                {
                    components = null;
                    return false;
                }
                list.Add(component);
            }
            components = list.ToArray();
            return true;
        }

        public bool TryGetComponent<TComponent>(List<int> entityIds, out TComponent[] components) where TComponent : class, IEcsComponent
        {
            var list = new List<TComponent>();
            foreach (var entityid in entityIds)
            {
                TComponent component;
                if (!TryGetComponent(entityid, out component))
                {
                    components = null;
                    return false;
                }
                list.Add(component);
            }
            components = list.ToArray();
            return true;
        }

        public IEnumerable<T> GetAllComponents<T>() where T : IEcsComponent
        {
            IEcsComponentManager existing;
            if (ComponentManagers.TryGetValue(typeof(T), out existing))
            {
                var manager = (EcsComponentManagerOf<T>)existing;
                foreach (var item in manager.Components)
                    yield return (T)item;

            }
           
        }

        public bool HasAny(int entityId, params Type[] types)
        {
            foreach (var type in types)
            {
                if (ComponentManagers[type].ForEntity(entityId).Any())
                {
                    return true;
                }
            }
            return false;
        }

        public bool TryGetComponent<TComponent>(int entityId, out TComponent component) where TComponent : class, IEcsComponent
        {
            IEcsComponentManager existing;
            if (!ComponentManagers.TryGetValue(typeof(TComponent), out existing))
            {
                component = null;
                return false;
            }
            var manager = (IEcsComponentManagerOf<TComponent>)existing;
            var result = manager.Components.FirstOrDefault(p => p.EntityId == entityId);
            if (result != null)
            {
                component = result;
                return true;
            }
            component = null;
            return false;
        }
    }
}