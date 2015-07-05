namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;
    
    
    public class MatchAllNode : MatchAllNodeBase {
        public override void WriteCode(TemplateContext ctx)
        {
            base.WriteCode(ctx);
            //foreach (var component in Components)
            //{
            //    //ctx._("var {0} = ComponentSystem.TryGetComponent<{1}>()");
            //}
        }
    }
    
    public partial interface IMatchAllConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
