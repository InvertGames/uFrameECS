namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;
    
    
    public class ComponentsNode : ComponentsNodeBase {
    }
    
    public partial interface IComponentsConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
