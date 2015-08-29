using System.CodeDom;
using Invert.Json;

namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;
    using Invert.Data;
    
    public class BoolNode : BoolNodeBase {
        private bool _value;

        [NodeProperty, JsonProperty]
        public bool Value
        {
            get { return _value; }
            set { this.Changed("Value", ref _value, value); }
        }

        public override string VariableType
        {
            get { return typeof(bool).FullName; }
        }

        public override string ValueExpression
        {
            get { return Value ? "true" : "false"; }
        }

        public override CodeExpression GetCreateExpression()
        {
            return new CodePrimitiveExpression(Value);
        }
    }
    
    public partial interface IBoolConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
