using uFrame.Kernel;
using UniRx;
using UnityEngine;

namespace uFrame.ECS
{
    public class EcsComponent : uFrameComponent, IEcsComponent, IDisposableContainer
    {
        [SerializeField]
        private int _entityId;

        private Transform _cachedTransform;

        public int EntityId
        {
            get { return _entityId; }
            set { _entityId = value; }
        }

        public override void KernelLoaded()
        {
            base.KernelLoaded();
            this.Publish(new ComponentCreatedEvent()
            {
                Component = this
            });
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            this.Publish(new ComponentDestroyedEvent()
            {
                Component = this
            });
        }

        /// <summary>
        /// A lazy loaded cached reference to the transform
        /// </summary>
        public Transform CachedTransform
        {
            get { return _cachedTransform ?? (_cachedTransform = transform); }
            set { _cachedTransform = value; }
        }
    }

    public class EcsDispatcher : uFrameComponent, IEcsComponent
    {
        public int EntityId { get; set; }
    }

   
}