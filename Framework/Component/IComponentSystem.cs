using System;
using System.Collections.Generic;

namespace uFrame.ECS
{
    public interface IComponentSystem
    {
        bool HasAny(int entityId, params Type[] type);
        bool TryGetComponent<TComponent>(int entityId, out TComponent component) where TComponent : class, IEcsComponent;
        bool TryGetComponent<TComponent>(int[] entityIds, out TComponent[] component) where TComponent : class, IEcsComponent;
        bool TryGetComponent<TComponent>(List<int> entityIds, out TComponent[] component) where TComponent : class, IEcsComponent;
        IEnumerable<TComponent> GetAllComponents<TComponent>() where TComponent : IEcsComponent;
        IEcsComponentManagerOf<TComponent> RegisterComponent<TComponent>() where TComponent : IEcsComponent;
        IEcsComponentManager RegisterComponent(Type componentType);
        void RegisterComponentInstance(Type componentType, IEcsComponent instance);
        void DestroyComponentInstance(Type componentType, IEcsComponent instance);

    }
}