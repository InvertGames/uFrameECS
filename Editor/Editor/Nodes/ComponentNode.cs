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
            return Name;
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

        public IEnumerable<IContextVariable> GetVariables(string prefix)
        {
            yield return new ContextVariable(prefix + this.Name)
            {
                Node = this,
                //SourceVariable = select as GenericNode
            };

            yield return new ContextVariable(prefix + Name, "EntityId")
            {
                Node = this,
                IsSubVariable = true,
            };
            yield return new ContextVariable(prefix + Name, "Entity")
            {
                Node = this,
                IsSubVariable = true,

            };
            foreach (var child in PersistedItems.OfType<ITypedItem>())
            {
                yield return new ContextVariable(prefix + Name, child.Name)
                {
                    Node = this,
                    IsSubVariable = true,
                    SourceVariable = child
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
