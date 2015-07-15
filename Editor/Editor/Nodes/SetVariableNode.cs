namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;
    
    
    public class SetVariableNode : SetVariableNodeBase {
        public override void WriteCode(TemplateContext ctx)
        {
            
        
            ctx._("{0} = {1}",
                VariableInputSlot.InputFrom<IContextVariable>().VariableName, 
                ValueInputSlot.InputFrom<IContextVariable>().VariableName
                );
            base.WriteCode(ctx);
        }
    }
    
    public partial interface ISetVariableConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
