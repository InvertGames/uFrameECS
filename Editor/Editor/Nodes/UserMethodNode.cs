using System.CodeDom;

namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;
    
    
    public class UserMethodNode : UserMethodNodeBase {

        public override void WriteCode(TemplateContext ctx)
        {
            //base.WriteCode(ctx);
            var handlerMethod = ctx.CurrentDeclaration.protected_virtual_func(typeof(void), Name, Name.ToLower());
            var handlerInvoke = new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), Name);

            foreach (CodeParameterDeclarationExpression item in ctx.CurrentMethod.Parameters)
            {
                handlerMethod.Parameters.Add(item);
                handlerInvoke.Parameters.Add(new CodeVariableReferenceExpression(item.Name));
            }

            //foreach (var item in AllContextVariables)
            //{
            //    if (item.IsSubVariable) continue;
            //    var type = item.SourceVariable == null ? item.Name : item.SourceVariable.RelatedTypeName;
                
                
            //    handlerMethod.Parameters.Add(new CodeParameterDeclarationExpression(
            //       type , item.AsParameter));
            //    handlerInvoke.Parameters.Add(new CodeVariableReferenceExpression(item.ToString()));
            //}
            ctx.CurrentStatements.Add(handlerInvoke);
            //ctx.PushStatements(handlerMethod.Statements);

            //ctx.PopStatements();
        }
    }
    
    public partial interface IUserMethodConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
