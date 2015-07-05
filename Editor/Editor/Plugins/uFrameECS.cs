using Invert.IOC;
using UnityEditor.Graphs;

namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;


    public class uFrameECS : uFrameECSBase, IPrefabNodeProvider
    {
        public override void Initialize(UFrameContainer container)
        {
            base.Initialize(container);
            ListenFor<IPrefabNodeProvider>();
            container.RegisterDrawer<ItemViewModel<IContextVariable>, ItemDrawer>();
            container.AddItemFlag<ComponentsReference>("Multiple", UnityEngine.Color.blue);
            container.AddNodeFlag<EventNode>("Dispatcher");
        }

        public IEnumerable<QuickAddItem> PrefabNodes(INodeRepository nodeRepository)
        {
            foreach (var item in nodeRepository.NodeItems.OfType<EventNode>())
            {
               
                var qa = new QuickAddItem(item.Group, item.Name, _ =>
                {
                  
                    var eventNode = new OnEventNode()
                    {
                        EventIdentifier = _.Item.Identifier
                    };
                    _.Diagram.AddNode(eventNode, _.MousePosition);
                })
                {
                    Item = item
                };
                yield return qa;
            }
            yield break;
        }
    }
}
