using System.CodeDom;

namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;
    
    
    public class StringNode : StringNodeBase {
        [NodeProperty, JsonProperty]
        public string Value { get; set; }

        public override string VariableType
        {
            get { return typeof (string).FullName; }
        }

        public override CodeExpression GetCreateExpression()
        {
            return new CodePrimitiveExpression(Value);
        }
    }
    
    public partial interface IStringConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
