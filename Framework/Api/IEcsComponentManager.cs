using System;
using System.Collections.Generic;

namespace uFrame.ECS
{
    public interface IEcsComponentManager
    {
        Type For { get; }
        IEnumerable<IEcsComponent> All { get; }
        void RegisterComponent(IEcsComponent item);
        void UnRegisterComponent(IEcsComponent item);
        IEnumerable<IEcsComponent> ForEntity(int entityId);
    }
}