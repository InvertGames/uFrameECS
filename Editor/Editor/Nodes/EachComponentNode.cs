namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;
    
    
    public class EachComponentNode : EachComponentNodeBase {

        public override string ListVariableName
        {
            get { return string.Format("{0}List", this.Name); }
        }


        public virtual string ItemVariableName
        {
            get { return this.Name.ToLower() + "Item"; }
        }

        public override string Name
        {
            get
            {
                //if (ComponentInputNode != null)
                //{
                //    return string.Format("For Each {0}", ComponentInputNode.Name);
                //}
                return base.Name;
            }
            set { base.Name = value; }
        }

        private ComponentNode ComponentInputNode
        {
            get { return ComponentInputSlot.InputFrom<ComponentNode>(); }
        }

        public override IEnumerable<IContextVariable> ContextVariables
        {
            get
            {
                yield return new ContextVariable(IndexVariableName);
                yield return new ContextVariable(ListVariableName);
                yield return new ContextVariable(ItemVariableName);
            }
        }

        public override void WriteCode(TemplateContext ctx)
        {
            ctx._("var {0} = {1}Manager.Components", ListVariableName,ComponentInputSlot.InputFrom<ComponentNode>().Name);
            base.WriteCode(ctx);

        }

        public override void WriteLoopCode(TemplateContext ctx)
        {
            base.WriteLoopCode(ctx);
            //ctx._("var {0} = {1}[{2}]", ItemVariableName, IndexVariableName);
        }
    }
    
    public partial interface IEachComponentConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
