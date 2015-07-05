using System.Collections.Generic;

namespace uFrame.ECS
{
    public interface IEcsComponentManagerOf<TComponentType> : IEcsComponentManager
    {
        IEnumerable<TComponentType> Components { get; }
    }
}