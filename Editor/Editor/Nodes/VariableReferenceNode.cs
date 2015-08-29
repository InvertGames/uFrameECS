using Invert.Json;

namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;
    
    
    public class VariableReferenceNode : VariableReferenceNodeBase, IContextVariable {
        [JsonProperty]
        public string HandlerId { get; set; }
        
        [JsonProperty]
        public string VariableId { get; set; }

        public HandlerNode HandlerNode
        {
            get { return Repository.GetSingle<HandlerNode>(HandlerId); }
        }

        public string ShortName
        {
            get { return VariableName.Split('.').LastOrDefault(); }
        }

        public string ValueExpression { get; private set; }

        public string Value
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public ITypedItem Source
        {
            get
            {

                var handler = HandlerNode;
                if (handler == null) return null;

                var variable = HandlerNode.GetAllContextVariables().FirstOrDefault(p => p.VariableName == VariableName);
                if (variable != null)
                    return variable.Source;

                return null;
            }
            set
            {

            }
        }

        [JsonProperty,InspectorProperty]
        public string VariableName {
            get { return Name; }
            set { Name = value; }
        }

        public string AsParameter
        {
            get { return Name; }
        }

        public bool IsSubVariable { get; set; }

        public string VariableType
        {
            get { return Source.RelatedTypeName; }
        }

        public IEnumerable<IContextVariable> GetPropertyDescriptions()
        {
            yield break;
        }
    }
    
    public partial interface IVariableReferenceConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
