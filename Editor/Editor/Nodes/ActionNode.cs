namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;

    public interface IContextVariable : IDiagramNodeItem
    {
        //IEnumerable<IContextVariable> Members { get; }
        ITypedItem SourceVariable { get; set; }
        string VariableName { get; set; }
        string AsParameter { get; }
    }

    public interface IVariableExpressionItem
    {
        string Expression { get; set; }
    }
    public class ContextVariable : GenericTypedChildItem, IContextVariable
    {
        private string _memberExpression;

        public override string Identifier
        {
            get { return Node.Identifier + ":" + MemberExpression; }
            set {  }
        }

        public ContextVariable(params object[] items)
        {
            Items = items;
        } 

        public object[] Items { get; set; }
        public override string ToString()
        {
            return MemberExpression;
        }

        public string MemberExpression
        {
            get
            {
                return _memberExpression ?? (_memberExpression = string.Join(".", Items.Select(p =>
                {
                    var cv = p as IDiagramNodeItem;
                    if (cv != null) return cv.Name;
                    return (string)p;
                }).ToArray()));
            }
        }

        public override string Title
        {
            get { return MemberExpression; }
        }

        public override string Group
        {
            get
            {
                if (Items.Length < 1)
                    return "Missing";

                var item = Items.Length > 1 ? Items[Items.Length - 2] : Items.Last();
                var cv = item as IDiagramNodeItem;
                if (cv != null)
                {
                    return cv.Name;
                }
                return (string)item;
            }
        }

        public override string SearchTag
        {
            get { return MemberExpression; }
        }

        public override string Name
        {
            get { return VariableName; }
            set { base.Name = value; }
        }

        public string VariableName
        {
            get { return MemberExpression; }
            set { }
        }

        public string AsParameter
        {
            get { return Items.Last().ToString().ToLower(); }
        }

        public ITypedItem SourceVariable { get; set; }
    }
    public interface ICodeOutput
    {
        IEnumerable<IContextVariable> AllContextVariables { get; }
        IEnumerable<IContextVariable> ContextVariables { get; }

        void WriteCode(TemplateContext ctx);
    }
    public class ActionNode : ActionNodeBase, ICodeOutput {
        public ActionNode Left
        {
            get
            {
                var r = this.InputFrom<ActionNode>();
                if (r != null) return r;
                var leftInput = this.InputFrom<IDiagramNodeItem>();
                if (leftInput != null)
                {
                    return leftInput.Node as ActionNode;
                }
                return null;
            }
        }

        public IEnumerable<ActionNode> LeftNodes
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
        public IEnumerable<ActionNode> RightNodes
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
        public ActionNode Right
        {
            get { return this.OutputTo<ActionNode>(); }
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
    
    public partial interface IActionConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
