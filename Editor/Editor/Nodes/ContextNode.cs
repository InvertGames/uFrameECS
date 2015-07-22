namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;


    public class ContextNode : ContextNodeBase,ISelectConnectable, IMappingsConnectable, IHandlerConnectable, ICodeOutput {

        public IEnumerable<IContextVariable> AllContextVariables
        {
            get { return ContextVariables; }
        }

        public virtual IEnumerable<IContextVariable> ContextVariables
        {
            get {
                foreach (var item in SelectComponents)
                {
                    foreach (var property in item.PersistedItems.OfType<ITypedItem>())
                    {
                        yield return new ContextVariable(string.Format("{0}", property.Name))
                        {
                            SourceVariable = property,
                            Node = this
                        };
                    }
                } 
            }
        }
        public void WriteCode(TemplateContext ctx)
        {
            
        }


        public IEnumerable<ComponentNode> SelectComponents { get { return Select.Select(p => p.SourceItem).OfType<ComponentNode>(); } }


        public string GetContextItemName(string mappingId)
        {
            return mappingId + "Item";
        }

        public string ContextTypeName
        {
            get { return Name + "ContextItem"; }
        }

        public string SystemPropertyName
        {
            get { return Name + "Context"; }
        }

        public string EnumeratorExpression
        {
            get { return string.Format("{0}.Items", SystemPropertyName); }
        }

        public IEnumerable<IContextVariable> GetVariables(string prefix)
        {
            foreach (var select in SelectComponents)
            {

                yield return new ContextVariable(prefix, select.Name)
                {
                    Node = this,
                    //SourceVariable = select as GenericNode
                };
                yield return new ContextVariable(prefix, select.Name, "EntityId")
                {
                    Node = this,
                    IsSubVariable = true,
                    
                };
                yield return new ContextVariable(prefix, select.Name, "Entity")
                {
                    Node = this,
                    IsSubVariable = true,

                };
                foreach (var child in select.PersistedItems.OfType<ITypedItem>())
                {
                    yield return new ContextVariable(prefix , select.Name, child.Name)
                    {
                        Node = this,
                        IsSubVariable = true,
                        SourceVariable = child
                    };
                }
            }
        }

        public string MatchAndSelect(string mappingExpression)
        {
            return string.Format("{0}Context.MatchAndSelect({1})",Name,mappingExpression);
        }

        public string DispatcherTypesExpression()
        {
            return SystemPropertyName + ".SelectTypes";
        }
    }
    
    public partial interface IContextConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
