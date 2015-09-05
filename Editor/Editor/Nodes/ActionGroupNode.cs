namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;
    
    
    public class ActionGroupNode : ActionGroupNodeBase, IVariableContextProvider {
        
        
        public override void WriteCode(TemplateContext ctx)
        {
            base.WriteCode(ctx);

        }
    }
    
    public partial interface IActionGroupConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
