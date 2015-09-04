using UnityEngine;

namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;
    
    
    public class ComponentNode : ComponentNodeBase, IComponentsConnectable, IMappingsConnectable, ITypedItem {
 

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

        //public override IEnumerable<IMemberInfo> GetMembers()
        //{
            
        //}

        public IEnumerable<IContextVariable> GetVariables(IFilterInput input)
        {
            yield return new ContextVariable(input.HandlerPropertyName)
            {
                Repository = this.Repository,
                Node = this,
                Source = this,
                VariableType = this,
                //TypeInfo =  typeof(MonoBehaviour)
            };
            yield return new ContextVariable(input.HandlerPropertyName, "EntityId")
            {
                Repository = this.Repository,
                Node = this,
                VariableType = new SystemTypeInfo(typeof(int)),

            };
            yield return new ContextVariable(input.HandlerPropertyName, "Entity")
            {
                Repository = this.Repository,
                Node = this,
                VariableType = new SystemTypeInfo(typeof(MonoBehaviour)),
                //TypeInfo = typeof(MonoBehaviour)
            };

            foreach (var item in PersistedItems.OfType<IMemberInfo>())
            {
                yield return new ContextVariable(input.HandlerPropertyName,item.MemberName)
                {
                    Repository = this.Repository,
                    Node = this,
                    Source = item as ITypedItem,
                    VariableType = item.MemberType
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

        public IEnumerable<PropertiesChildItem> GetObservableProperties()
        {
            return Properties;
        }

       
    }
    
    public partial interface IComponentConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
