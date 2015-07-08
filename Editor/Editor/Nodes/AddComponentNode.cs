namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;
    
    
    public class AddComponentNode : AddComponentNodeBase {
        public override void WriteCode(TemplateContext ctx)
        {
            base.WriteCode(ctx);
            var component = this.ComponentInputSlot.InputFrom<ComponentNode>();
            //if (component != null)
            //{
            //    ctx._("");
            //}
        }
    }
    
    public partial interface IAddComponentConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
