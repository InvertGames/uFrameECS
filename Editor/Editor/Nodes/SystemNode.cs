namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;
    
    
    public class SystemNode : SystemNodeBase {
        public IEnumerable<HandlerNode> EventHandlers
        {
            get { return this.Graph.NodeItems.OfType<HandlerNode>(); }
        }
    }
    
    public partial interface ISystemConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
