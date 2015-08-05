using System.CodeDom;
using Invert.Json;

namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;
    
    
    public class BoolNode : BoolNodeBase {
        [NodeProperty, JsonProperty]
        public bool Value { get; set; }
        public override string VariableType
        {
            get { return typeof(bool).FullName; }
        }
        
        public override CodeExpression GetCreateExpression()
        {
            return new CodePrimitiveExpression(Value);
        }
    }
    
    public partial interface IBoolConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
