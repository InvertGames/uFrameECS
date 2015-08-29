using Invert.Json;

namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;
    using Data;
    
    public class ConditionNode : ConditionNodeBase {
        private PropertyIn _ainput;
        private PropertyIn _binput;
        private ConditionComparer _comparer;

        [InputSlot("A")]
        public PropertyIn AInput
        {
            get
            {
                return _ainput ?? (_ainput = new PropertyIn()
                {
                    Repository = this.Repository,
                    Node = this.Node,
                    Name = "A Input",
                    DoesAllowInputs = true,
                    Identifier = Identifier + ":" + "A"

                });
            }
        }

        [NodeProperty,JsonProperty]
        public ConditionComparer Comparer
        {
            get { return _comparer; }
            set { this.Changed("Comparer", ref _comparer, value); }
        }

        [InputSlot("B")]
        public PropertyIn BInput
        {
            get
            {
                return _binput ?? (_binput = new PropertyIn()
                {
                    Repository = this.Repository,
                    Node = this.Node,
                    Name = "B Input",
                    DoesAllowInputs = true,
                    Identifier = Identifier + ":" + "B"

                });
            }
        }
        
        public override string GetExpression()
        {
            return AInput.Item.ValueExpression + Sign + BInput.Item.ValueExpression;
        }

        public string Sign
        {
            get
            {
                switch (Comparer)
                {
                    case ConditionComparer.Equal:
                        return "==";
                        case ConditionComparer.GreaterThen:
                        return ">";
                        case ConditionComparer.GreaterThenOrEqual:
                        return ">=";
                        case ConditionComparer.LessThen:
                        return "<";
                        case ConditionComparer.LessThenOrEqual:
                        return "<=";
                        case ConditionComparer.NotEqual:
                        return "!=";
                }
                return "==";
            }
        }

        public override IEnumerable<IGraphItem> GraphItems
        {
            get
            {
                yield return AInput;
                yield return BInput;
            }
        }
    }

    public enum ConditionComparer
    {
        Equal,
        NotEqual,
        GreaterThen,
        GreaterThenOrEqual,
        LessThen,
        LessThenOrEqual
        
    }
    
    public partial interface IConditionConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
