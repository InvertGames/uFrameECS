namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;
    
    
    public class SystemNode : SystemNodeBase {
        public override bool AllowInputs
        {
            get { return false; }
        }

        public override bool AllowOutputs
        {
            get { return false; }
        }

        public IEnumerable<HandlerNode> EventHandlers
        {
            get
            {
                return this.GetContainingNodes(Graph).OfType<HandlerNode>();
            }
        }
    }
    
    public partial interface ISystemConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
