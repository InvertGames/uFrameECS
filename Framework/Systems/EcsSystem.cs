using uFrame.Kernel;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace uFrame.ECS
{
    public abstract class EcsSystem : SystemServiceMonoBehavior, IEcsSystem
    {

    }

    public static class EcsSystemExtensions
    {
     
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