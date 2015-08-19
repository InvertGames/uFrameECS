using System.CodeDom;

namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;


    public class SequenceItemNode : SequenceItemNodeBase, ICodeOutput
    {
        public IVariableContextProvider Left
        {
            get
            {
                var r = this.InputFrom<IVariableContextProvider>();
                return r;
                //var leftInput = this.InputFrom<IDiagramNodeItem>();
                //if (leftInput != null)
                //{
                //    return leftInput.Node as SequenceItemNode;
                //}
                return null;
            }
        }
        public IEnumerable<IVariableContextProvider> LeftNodes
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
        public IEnumerable<IVariableContextProvider> RightNodes
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
       

        public IEnumerable<IContextVariable> GetAllContextVariables()
        {
            var left = Left;
            if (left != null)
            {
                foreach (var contextVar in left.GetAllContextVariables())
                {
                    yield return contextVar;
                }
            }
            foreach (var item in GetContextVariables())
            {
                yield return item;
            }
        }

        public virtual IEnumerable<IContextVariable> GetContextVariables()
        {
            yield break;
        }

        public virtual void WriteCode(TemplateContext ctx)
        {
            OutputVariables(ctx);
            ctx._comment(Name);
            foreach (var right in this.OutputsTo<SequenceItemNode>())
            {
                if (right != null)
                {
                    right.WriteCode(ctx);
                }
            }
            
        }

        protected void OutputVariables(TemplateContext ctx)
        {
            foreach (var item in GraphItems.OfType<IConnectable>())
            {
                var decl = item.InputFrom<VariableNode>();
                if (decl == null) continue;

                ctx.CurrentDeclaration.Members.Add(decl.GetFieldStatement());
            }
     
        }
    }
    
    public partial interface ISequenceItemConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
