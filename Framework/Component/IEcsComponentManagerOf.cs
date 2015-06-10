using System.Collections.Generic;

namespace uFrame.ECS
{
    public interface IEcsComponentManagerOf<TComponentType>
    {
        List<TComponentType> Components { get; }
    }
}