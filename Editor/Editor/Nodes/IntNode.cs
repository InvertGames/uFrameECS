using System.CodeDom;

namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;
    
    
    public class IntNode : IntNodeBase {
        public override string VariableType
        {
            get { return typeof(int).Name; }
        }
        [NodeProperty, JsonProperty]
        public int Value { get; set; }

        public override CodeExpression GetCreateExpression()
        {
            return new CodePrimitiveExpression(Value);
        }
    }
    
    public partial interface IIntConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
