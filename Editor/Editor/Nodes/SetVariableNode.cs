namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;
    
    
    public class SetVariableNode : SetVariableNodeBase {
        private string _VariableInputSlotId;

        private string _ValueInputSlotId;

        private PropertyIn _Variable;

        private ValueIn _Value;
        
        [Invert.Json.JsonProperty()]
        public virtual string VariableInputSlotId
        {
            get
            {
                if (_VariableInputSlotId == null)
                {
                    _VariableInputSlotId = Guid.NewGuid().ToString();
                }
                return _VariableInputSlotId;
            }
            set
            {
                _VariableInputSlotId = value;
            }
        }

        [Invert.Json.JsonProperty()]
        public virtual string ValueInputSlotId
        {
            get
            {
                if (_ValueInputSlotId == null)
                {
                    _ValueInputSlotId = Guid.NewGuid().ToString();
                }
                return _ValueInputSlotId;
            }
            set
            {
                _ValueInputSlotId = value;
            }
        }

        [InputSlot("Variable")]
        public virtual PropertyIn VariableInputSlot
        {
            get
            {
                if (Repository == null)
                {
                    return null;
                }
                if (_Variable != null)
                {
                    return _Variable;
                }
                return _Variable ?? (_Variable = new PropertyIn()
                {
                    Repository = Repository, Node = this, Identifier = VariableInputSlotId,
                    Name="Variable",
                    DoesAllowInputs = true
                });
            }
        }

        [InputSlot("Value")]
        public virtual ValueIn ValueInputSlot
        {
            get
            {
                if (Repository == null)
                {
                    return null;
                }
                if (_Value != null)
                {
                    return _Value;
                }
                return _Value ?? (_Value = new ValueIn()
                {
                    Repository = Repository,
                    DoesAllowInputs = true, 
                    Node = this, 
                    Identifier = ValueInputSlotId, 
                    Name = "Value",
                    Variable = VariableInputSlot
                });
            }
        }

        public override void Validate(List<ErrorInfo> errors)
        {
            base.Validate(errors);
            if (VariableInputSlot.Item == null || ValueInputSlot.Item == null)
            {
                errors.AddError("Variable and Value must be set.");
                return;
            }
            if (VariableInputSlot.Item.VariableType == ValueInputSlot.Item.VariableType)
            {
                errors.AddError("Variable types do not match.");
            }
        }

        public override IEnumerable<IGraphItem> GraphItems
        {
            get
            {
                yield return VariableInputSlot;
                yield return ValueInputSlot;
            }
        }

        public override void WriteCode(TemplateContext ctx)
        {
            
            ctx._("{0} = {1}",
                VariableInputSlot.Item.VariableName,
                ValueInputSlot.Item.VariableName
                );
            base.WriteCode(ctx);
        }
    }

    public class ValueIn : PropertyIn
    {
        public PropertyIn Variable { get; set; }

        public override object VariableType
        {
            get
            {
                return Variable.VariableType;
                return base.VariableType;
            }
        }
    }
    public partial interface ISetVariableConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
