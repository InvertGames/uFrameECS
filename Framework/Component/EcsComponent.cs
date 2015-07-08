using uFrame.Kernel;
using UniRx;
using UnityEngine;

namespace uFrame.ECS
{
   
    public class EcsComponent : uFrameComponent, IEcsComponent, IDisposableContainer
    {
        //[SerializeField]
        private int _entityId;

        private Transform _cachedTransform;

        public virtual int EntityId
        {
            get
            {
                return _entityId;
            }
            set { 
                _entityId = value;
            }
        }
        public Entity _Entity;
        private Subject<Unit> _changed;

        public void Reset()
        {
            var entityComponent = GetComponent<Entity>();
            if (entityComponent == null)
                entityComponent = gameObject.AddComponent<Entity>();
            _Entity = entityComponent;
        }



        public override void KernelLoading()
        {
  
            
         
            base.KernelLoading();
        }

        public override void KernelLoaded()
        {
            base.KernelLoaded();
            if (_Entity != null)
            {
                _entityId = _Entity.EntityId;
            }
            else
            {
                _entityId = GetComponent<Entity>().EntityId;
            }
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

        public IObservable<Unit> Changed
        {
            get { return _changed ?? (_changed = new Subject<Unit>()); }
        }
    }
}