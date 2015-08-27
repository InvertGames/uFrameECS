using System.Collections.Generic;
using System.Linq;
using uFrame.ECS;
using uFrame.Kernel;
using UniRx;
using UnityEngine;

namespace uFrame.ECS
{
    public class EcsSystemLoader : SystemLoader
    {
        public List<EntityPrefabPool> Pools = new List<EntityPrefabPool>();
        private ISystemUpdate[] _items;

        public override void Load()
        {
            base.Load();
            Container.RegisterInstance<IComponentSystem>(this.AddSystem<EcsComponentService>());
            this.AddSystem<EntityService>().Pools = Pools;
        }

        public void Update()
        {
            if (uFrameKernel.IsKernelLoaded)
            {
                if (_items == null)
                {
                    _items = uFrameKernel.Instance.Services.OfType<ISystemUpdate>().ToArray();
                }

                foreach (var item in _items)
                {
                    item.SystemUpdate();
                }
            }
        }
    }

    public class DebugInfo
    {
        public string ActionId;
        public object[] Variables;

        public int Result { get; set; }
    }

    public static class DebugService
    {
        private static Subject<DebugInfo> _debugInfo;


        public static IObservable<DebugInfo> DebugInfo
        {
            get { return _debugInfo ?? (_debugInfo = new Subject<DebugInfo>()); }
        }

        public static int NotifyDebug(string actionId, object[] variables)
        {
#if UNITY_EDITOR
            if (_debugInfo != null)
            {
                var debugInfo = new DebugInfo()
                {
                    ActionId = actionId,
                    Variables = variables
                };
                _debugInfo.OnNext(debugInfo);
                return debugInfo.Result;
            }
#endif
            return 0;
        }
    }

}
public static class DebugExtensions
{
    public static int DebugInfo(this object obj, string actionId, params object[] variables)
    {

        return DebugService.NotifyDebug(actionId, variables);

    }
}