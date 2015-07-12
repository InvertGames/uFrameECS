namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;
    
    
    public class Vector2Node : Vector2NodeBase {
        [NodeProperty, JsonProperty]
        public float X { get; set; }
        [NodeProperty, JsonProperty]
        public float Y { get; set; }
    }
    
    public partial interface IVector2Connectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
