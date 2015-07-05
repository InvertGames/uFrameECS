namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;
    
    
    public class FilterNode : FilterNodeBase, IWithAnyConnectable, ISelectConnectable, ISetupCodeWriter {
        public void WriteSetupCode(TemplateContext ctx)
        {
            
        }
    }
    
    public partial interface IFilterConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
