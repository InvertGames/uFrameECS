namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core.GraphDesigner;
    
    
    public class MappingsReference : MappingsReferenceBase {
        public FilterNode Filter
        {
            get { return this.InputFrom<FilterNode>(); }
        }
    }
    
    public partial interface IMappingsConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
