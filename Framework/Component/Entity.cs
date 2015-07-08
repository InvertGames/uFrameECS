using uFrame.Kernel;
using UnityEngine;

namespace uFrame.ECS
{
    public partial class Entity : uFrameComponent, IEcsComponent
    {
        [SerializeField]
        private int _entityId;

        private static int _lastUsedId = 0;

        public int EntityId
        {
            get
            {
                return _entityId;
            }
            set { _entityId = value; }
        }

        public static int LastUsedId
        {
            get { return _lastUsedId; }
            set { _lastUsedId = value; }
        }

        public override void KernelLoading()
        {
            base.KernelLoading();
            if (_entityId == 0)
            {
                _lastUsedId--;
                _entityId = _lastUsedId;
            }
    

        }
    }
}