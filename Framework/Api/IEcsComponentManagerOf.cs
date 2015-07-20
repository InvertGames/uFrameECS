using System.Collections.Generic;

namespace uFrame.ECS
{
    public interface IEcsComponentManagerOf<TComponentType> : IEcsComponentManager
    {
        TComponentType this[int entityId] { get; }
        IEnumerable<TComponentType> Components { get; }
    }
}