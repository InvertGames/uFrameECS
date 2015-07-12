namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;


    public class ContextNode : ContextNodeBase, IWithAnyConnectable, ISelectConnectable, IMappingsConnectable, IHandlerConnectable, ICodeOutput {

        public IEnumerable<IContextVariable> AllContextVariables
        {
            get { return ContextVariables; }
        }

        public virtual IEnumerable<IContextVariable> ContextVariables
        {
            get {
                foreach (var item in Select.Select(p=>p.SourceItem).OfType<ComponentNode>())
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
    }
    
    public partial interface IContextConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
