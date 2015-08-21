using System.CodeDom;
using System.Linq;
using System.Runtime.InteropServices;
using Invert.Core.GraphDesigner;
using uFrame.ECS;

namespace Invert.uFrame.ECS.Templates
{
    public partial class HandlerTemplate
    {
        [GenerateProperty, WithField]
        public object Event
        {
            get
            {
                
                this.Ctx.SetType(Ctx.Data.EventType);
                return null;
            }
            set
            {
                
            }
        }


   
        [GenerateProperty, WithField]
        public EcsSystem System { get; set; }

        [TemplateSetup]
        public void SetName()
        {   
            foreach (var item in  Ctx.Data.FilterInputs)
            {
                var context = item.FilterNode;
                if (context == null) continue;
                CreateFilterProperty( item, context );
            }
            
        }

        private void CreateFilterProperty(IFilterInput input, IMappingsConnectable inputFilter)
        {
            var property = Ctx.CurrentDeclaration._public_(inputFilter.ContextTypeName, input.HandlerPropertyName);

        }

    }

    public class HandlerCsharpVisitor : HandlerNodeVisitor
    {
        public TemplateContext _ { get; set; }
        public int VariableCount;
        private CodeMethodInvokeExpression _currentActionInvoker;

        public string NewVariable
        {
            get { return string.Format("variable{0}", VariableCount++); }
        }

        public override void BeforeVisitAction(ActionNode actionNode)
        {
            if (actionNode.Meta == null) return;
            _._comment("Before visit {0}", actionNode.Meta.FullName);
            base.BeforeVisitAction(actionNode);

        }

        public override void VisitAction(ActionNode actionNode)
        {
            if (actionNode.Meta == null) return;
            base.VisitAction(actionNode);
            _._comment("Visit {0}", actionNode.Meta.FullName);
            var methodInfo = actionNode.Meta.Method;
            if (methodInfo != null)
            {

                _currentActionInvoker =
                    new CodeMethodInvokeExpression(
                        new CodeMethodReferenceExpression(new CodeSnippetExpression(actionNode.Meta.Type.FullName),
                            methodInfo.Name));

                foreach (var input in actionNode.InputVars)
                {
                    _currentActionInvoker.Parameters.Add(
                        new CodeSnippetExpression((input.ActionFieldInfo.Type.IsByRef ? "ref " : string.Empty) + string.Format("{0}", input.VariableName)));
                }
                ActionOut resultOut = null;
                // The outputs that should be assigned to by the method
                foreach (var @out in actionNode.OutputVars.OfType<ActionOut>())
                {
                    if (@out.Name == "Result")
                    {
                        resultOut = @out;
                        continue;
                    }
                    _currentActionInvoker.Parameters.Add(
                        new CodeSnippetExpression(string.Format("out {0}", @out.VariableName)));
                }
                foreach (var @out in actionNode.OutputVars.OfType<ActionBranch>())
                {
                    _currentActionInvoker.Parameters.Add(
                        new CodeSnippetExpression(string.Format("{0}", @out.VariableName)));
                }
                if (resultOut == null)
                {
                    _.CurrentStatements.Add(_currentActionInvoker);
                }
                else
                {
                    var assignResult = new CodeAssignStatement(
                        new CodeSnippetExpression(resultOut.VariableName), _currentActionInvoker);
                    _.CurrentStatements.Add(assignResult);
                }
               
            }
            else
            {
                var varStatement = _.CurrentDeclaration._private_(actionNode.Meta.Type, actionNode.VarName);
                varStatement.InitExpression = new CodeObjectCreateExpression(actionNode.Meta.Type);

                foreach (var item in actionNode.GraphItems.OfType<GenericSlot>())
                {
                    var contextVariable = item.InputFrom<IContextVariable>();
                    if (contextVariable == null) continue;
                    _._("{0}.{1} = {2}", varStatement.Name, item.Name, contextVariable.VariableName);
                }


                _._("{0}.System = System", varStatement.Name);


                foreach (var item in actionNode.OutputVars.OfType<ActionBranch>())
                {
                    var branchOutput = item.OutputTo<SequenceItemNode>();
                    if (branchOutput == null) continue;
                    _._("{0}.{1} = {2}", varStatement.Name, item.Name, item.VariableName);
                }

                _._if("!{0}.Execute()", varStatement.Name).TrueStatements._("return");

                foreach (var item in actionNode.OutputVars.OfType<ActionOut>())
                {
                    var contextVariable = item.OutputTo<IContextVariable>();
                    if (contextVariable == null) continue;
                    _._("{0} = {1}.{2}", item.VariableName, varStatement.Name, item.Name);
                }
                _._comment("CALL EXECUTE ON {0} CLASS",actionNode.Meta.Type.Name);
            }
        }

        public override void VisitOutput(IActionOut output)
        {
            base.VisitOutput(output);
            if (output.ActionFieldInfo.Type == typeof (System.Action)) return;
            //if (output.Name == "Result") return;
            _.TryAddNamespace(output.ActionFieldInfo.Type.Namespace);
            var varDecl = new CodeMemberField(
                output.VariableType.Replace("&", "").ToCodeReference(), 
                output.VariableName
                )
            {
                InitExpression = new CodeSnippetExpression(string.Format("default( {0} )", output.VariableType.Replace("&", "")))
            };
            _.CurrentDeclaration.Members.Add(varDecl);
            var variableReference = output.OutputTo<IContextVariable>();
            if (variableReference != null)
                _.CurrentStatements.Add(new CodeAssignStatement(new CodeSnippetExpression(variableReference.VariableName),
                    new CodeSnippetExpression(output.VariableName)));
            
        }

        public override void VisitSetVariable(SetVariableNode setVariableNode)
        {
            base.VisitSetVariable(setVariableNode);
            var ctxVariable = setVariableNode.VariableInputSlot.InputFrom<IContextVariable>();
            if (ctxVariable == null) return;

            _._("{0} = ({1}){2}", ctxVariable.VariableName, ctxVariable.VariableType,
                setVariableNode.ValueInputSlot.VariableName);
        }

        public override void VisitBranch(ActionBranch output)
        {
            var branchMethod = new CodeMemberMethod()
            {
                Name = output.VariableName
            };
            _.PushStatements(branchMethod.Statements);
            base.VisitBranch(output);
            _.PopStatements();
            _.CurrentDeclaration.Members.Add(branchMethod);

        }

        public override void VisitHandler(ISequenceNode handlerNode)
        {
            base.VisitHandler(handlerNode);
            _._comment("HANDLER: " + handlerNode.Name);
        }
        
        public override void VisitInput(IActionIn input)
        {
            base.VisitInput(input);
            if (input.ActionFieldInfo == null) return;
            _.TryAddNamespace(input.ActionFieldInfo.Type.Namespace);
            var varDecl = new CodeMemberField(
                input.ActionFieldInfo.Type.FullName.ToCodeReference(), 
                input.VariableName
               
                )
            {
                InitExpression = new CodeSnippetExpression(string.Format("default( {0} )", input.ActionFieldInfo.Type.FullName))
            };

            _.CurrentDeclaration.Members.Add(varDecl);

            var variableReference = input.Item;
            if (variableReference != null)
            _.CurrentStatements.Add(new CodeAssignStatement(new CodeSnippetExpression(input.VariableName),
                new CodeSnippetExpression(variableReference.VariableName)));

            var inputVariable = input.InputFrom<VariableNode>();
            if (inputVariable != null)
            {
        
                _.CurrentDeclaration.Members.Add(inputVariable.GetFieldStatement());
            }


        }
    }
}