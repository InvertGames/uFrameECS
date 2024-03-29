namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core.GraphDesigner;
    
    
    public class ComponentsReference : ComponentsReferenceBase {
        [InspectorProperty]
        public bool Multiple
        {
            get { return Flags["Multiple"]; }
            set { Flags["Multiple"] = value; }
        }
    }
    
    public partial interface IComponentsConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
