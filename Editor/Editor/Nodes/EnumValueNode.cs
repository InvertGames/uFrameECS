using System.CodeDom;
using Invert.Data;
using Invert.Json;
using UnityEngine;

namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;
    
    
    public class EnumValueNode : EnumValueNodeBase {
        private int _value;

        public override object VariableType
        {
            get { return EnumType; }
        }

        public IActionIn OutputItem
        {
            get { return this.OutputTo<IActionIn>(); }
        }

        public Type EnumType
        {
            get
            {
                var outputItem = OutputItem;
                if (outputItem == null) return null;
                     return outputItem.VariableType as Type;
                return null;
            }
        }

        public override string ValueExpression
        {
            get
            {
                return string.Format("({0}){1})", EnumType.Name, Value);
            }
        }

        public override CodeExpression GetCreateExpression()
        {
            return new CodeSnippetExpression(ValueExpression);
        }

        [JsonProperty]
        public int Value
        {
            get { return _value; }
            set { this.Changed("Value",ref _value,value); }
        }

        public override void Validate(List<ErrorInfo> errors)
        {
            base.Validate(errors);
            if (EnumType == null)
            {
                errors.AddError("Connect this to an input to select a value.",this);
            }
        }
    }
    
    public partial interface IEnumValueConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
