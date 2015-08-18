namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;
    
    
    public class PropertyNode : PropertyNodeBase {
    }
    
    public partial interface IPropertyConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
