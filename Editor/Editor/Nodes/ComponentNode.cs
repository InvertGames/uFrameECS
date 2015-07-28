namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;
    
    
    public class ComponentNode : ComponentNodeBase, IComponentsConnectable, IMappingsConnectable {
 

        public IEnumerable<ComponentNode> WithAnyComponents
        {
            get { yield break; }
        }

        public IEnumerable<ComponentNode> SelectComponents
        {
            get { yield return this; }
        }

        public string GetContextItemName(string mappingId)
        {
            return mappingId + Name;
        }

        public string ContextTypeName
        {
            get { return Name; }
        }

        [InspectorProperty]
        public bool Blackboard
        {
            get { return this["Blackboard"]; }
            set { this["Blackboard"] = value; }
        }

        public IEnumerable<IContextVariable> GetVariables(IFilterInput input)
        {
            yield return new ContextVariable(input.HandlerPropertyName)
            {
                Node = this,
                VariableType = this.Name
            };

            yield return new ContextVariable(input.HandlerPropertyName, "Entity")
            {
                Node = this,
                VariableType = "uFrameECS.Entity",
                
            };

            foreach (var item in PersistedItems.OfType<ITypedItem>())
            {
                yield return new ContextVariable(input.HandlerPropertyName,item.Name)
                {
                    Node = this,
                    SourceVariable = item,
                    VariableType = item.RelatedTypeName
                };
            }
        }

        public string SystemPropertyName
        {
            get { return this.Name + "Manager"; }
        }

        public string EnumeratorExpression
        {
            get { return string.Format("{0}.Components", SystemPropertyName); }
        }

        public string MatchAndSelect(string mappingExpression)
        {
            return string.Format("{0}[{1}]",SystemPropertyName,mappingExpression);
        }

        public string DispatcherTypesExpression()
        {
            return string.Format("typeof({0})", this.Name);
        }
    }
    
    public partial interface IComponentConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
