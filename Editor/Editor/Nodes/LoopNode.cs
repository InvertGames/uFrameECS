using System.CodeDom;

namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;
    
    
    public class LoopNode : LoopNodeBase {
        public virtual string ListVariableName
        {
            get { return "new object[] {}"; }
        } 
        public virtual string IndexVariableName
        {
            get { return this.Name.ToLower() + "Index"; }
        }

        public override IEnumerable<IContextVariable> ContextVariables
        {
            get
            {
                yield return new ContextVariable(IndexVariableName);
                yield return new ContextVariable(ListVariableName);
                //return base.ContextVariables;
            }
        }

        public override void WriteCode(TemplateContext ctx)
        {
            
            
            var loop = new CodeIterationStatement(
                new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(int)), IndexVariableName),
                new CodeSnippetExpression(string.Format("{0} < {1}.Count", IndexVariableName, ListVariableName)),
                new CodeSnippetStatement(string.Format("{0}++", IndexVariableName))
                );
            ctx.CurrentStatements.Add(loop);
            ctx.PushStatements(loop.Statements);
            WriteLoopCode(ctx);
            ctx.PopStatements();
            base.WriteCode(ctx);
        }

        public virtual void WriteLoopCode(TemplateContext ctx)
        {
            foreach (var right in EachOutputSlot.OutputsTo<ActionNode>())
            {
                if (right != null)
                {
                    right.WriteCode(ctx);
                }
            }
        }
    }
    
    public partial interface ILoopConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
