using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace uFrame.ECS
{
    public class EcsComponentManagerOf<TComponentType> : EcsComponentManager, IEcsComponentManagerOf<TComponentType> where TComponentType : IEcsComponent
    {
        private List<TComponentType> _items;

        public List<TComponentType> Components
        {
            get { return _items ?? (_items = new List<TComponentType>()); }
            set { _items = value; }
        }

        public IEnumerable<TComponentType> ForEntity(int entityId)
        {
            foreach (var item in Components)
            {
                if (item.EntityId == entityId)
                    yield return item;
            }
        }
        protected override void AddItem(IEcsComponent component)
        {
            Components.Add((TComponentType)component);
        }

        protected override void RemoveItem(IEcsComponent component)
        {
            Components.Remove((TComponentType)component);
        }
    }
}

