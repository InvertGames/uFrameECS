namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;
    
    
    public class PublishNode : PublishNodeBase {
        public EventNode EventNode
        {
            get { return EventInputSlot.InputFrom<EventNode>(); }
        }

        public override void WriteCode(TemplateContext ctx)
        {
            base.WriteCode(ctx);
            if (EventNode != null)
            {
                ctx._("var {0}Event = new {0}()", EventNode.Name);
                ctx._("this.Publish({0}Event)", EventNode.Name);
            }
        }
    }
    
    public partial interface IPublishConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
