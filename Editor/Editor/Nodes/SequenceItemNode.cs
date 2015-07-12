namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;


    public class SequenceItemNode : SequenceItemNodeBase, ICodeOutput
    {
        public SequenceItemNode Left
        {
            get
            {
                var r = this.InputFrom<SequenceItemNode>();
                if (r != null) return r;
                var leftInput = this.InputFrom<IDiagramNodeItem>();
                if (leftInput != null)
                {
                    return leftInput.Node as SequenceItemNode;
                }
                return null;
            }
        }
        public IEnumerable<SequenceItemNode> LeftNodes
        {
            get
            {
                var left = Left;
                while (left != null)
                {
                    yield return left;
                    left = left.Left;
                }
            }
        }
        public IEnumerable<SequenceItemNode> RightNodes
        {
            get
            {
                var right = Right;
                while (right != null)
                {
                    yield return right;
                    right = right.Right;
                }
            }
        }
        public SequenceItemNode Right
        {
            get { return this.OutputTo<SequenceItemNode>(); }
        }


        public IEnumerable<IContextVariable> AllContextVariables
        {
            get
            {
                var left = Left;
                if (left != null)
                {
                    foreach (var contextVar in left.AllContextVariables)
                    {
                        yield return contextVar;
                    }
                }
                foreach (var item in ContextVariables)
                {
                    yield return item;
                }
            }
        }
        public virtual IEnumerable<IContextVariable> ContextVariables
        {
            get { yield break; }
        }

        public virtual void WriteCode(TemplateContext ctx)
        {
            ctx._comment(Name);
            foreach (var right in this.OutputsTo<ActionNode>())
            {
                if (right != null)
                {
                    right.WriteCode(ctx);
                }
            }
            
        }
    }
    
    public partial interface ISequenceItemConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
