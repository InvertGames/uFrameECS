using System.CodeDom;
using Invert.Json;

namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;
    
    
    public class FloatNode : FloatNodeBase {
        public override string VariableType
        {
            get { return typeof (float).Name; }
        }
        [NodeProperty, JsonProperty]
        public float Value { get; set; }

        public override CodeExpression GetCreateExpression()
        {
            return new CodePrimitiveExpression(Value);
        }

    }
    
    public partial interface IFloatConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
