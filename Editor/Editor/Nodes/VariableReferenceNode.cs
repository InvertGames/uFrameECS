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
            get { return Graph.NodeItems.OfType<HandlerNode>().FirstOrDefault(p => p.Identifier == HandlerId); }
        }

        public string ShortName
        {
            get { return VariableName.Split('.').LastOrDefault(); }
        }
        
        public ITypedItem SourceVariable
        {
            get
            {
                var handler = HandlerNode;
                if (handler == null) return null;

                var variable = HandlerNode.AllContextVariables.FirstOrDefault(p => p.Identifier == VariableId);
                if (variable != null)
                    return variable.SourceVariable;

                return null;
            }
            set
            {

            }
        }
        [JsonProperty]
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
            get { return SourceVariable.RelatedTypeName; }
        }
    }
    
    public partial interface IVariableReferenceConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
