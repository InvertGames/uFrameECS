namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;


    public class GroupNode : GroupNodeBase,IRequireConnectable, IMappingsConnectable, IHandlerConnectable, ICodeOutput {
        public IEnumerable<IContextVariable> GetAllContextVariables()
        {
            return GetContextVariables();
        }

        public virtual IEnumerable<IContextVariable> GetContextVariables()
        {
            //foreach (var item in SelectComponents)
            //{
            //    foreach (var property in item.PersistedItems.OfType<ITypedItem>())
            //    {
            //        yield return new ContextVariable(string.Format("{0}", property.Name))
            //        {
            //            SourceVariable = property,
            //            Node = this
            //        };
            //    }
            //}
            yield break;
        }

        public void WriteCode(TemplateContext ctx)
        {
            
        }


        public IEnumerable<ComponentNode> SelectComponents { get { return Require.Select(p => p.SourceItem).OfType<ComponentNode>(); } }


        public string GetContextItemName(string mappingId)
        {
            return mappingId + "Item";
        }

        public string ContextTypeName
        {
            get { return Name; }
        }

        public string SystemPropertyName
        {
            get { return Name + "Context"; }
        }

        public string EnumeratorExpression
        {
            get { return string.Format("{0}.Items", SystemPropertyName); }
        }

        public IEnumerable<IContextVariable> GetVariables(IFilterInput input)
        {
            foreach (var select in SelectComponents)
            {

                yield return new ContextVariable(input.HandlerPropertyName, select.Name) { Repository = this.Repository, Node = this, VariableType = this.Name };
                yield return new ContextVariable(input.HandlerPropertyName, select.Name, "EntityId") { Repository = this.Repository, Node = this, VariableType = "int" };
                yield return new ContextVariable(input.HandlerPropertyName, select.Name, "Entity") { Repository = this.Repository, Node = this, VariableType = "uFrame.ECS.Entity" };

                foreach (var item in select.PersistedItems.OfType<ITypedItem>())
                {
                    yield return new ContextVariable(input.HandlerPropertyName, select.Name, item.Name)
                    {
                        Repository = this.Repository,
                        SourceVariable = item,
                        VariableType = item.RelatedTypeName,
                        Node = this
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
