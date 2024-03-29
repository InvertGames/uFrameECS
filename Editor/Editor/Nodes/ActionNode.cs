using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using Invert.Core;
using Invert.Core.GraphDesigner;
using uFrame.Attributes;

namespace Invert.uFrame.ECS
{
    public interface IContextVariable : IDiagramNodeItem
    {
        //IEnumerable<IContextVariable> Members { get; }
        string ShortName { get; }
        ITypedItem SourceVariable { get; set; }
        string VariableName { get; set; }
        string AsParameter { get; }
        bool IsSubVariable { get; set; }
        string VariableType { get; }
    }

    public interface IVariableExpressionItem
    {
        string Expression { get; set; }
    }


    public class ContextVariable : GenericTypedChildItem, IContextVariable
    {

        private string _memberExpression;
        private string _variableType;
        private List<object> _items;

        public override string Identifier
        {
            get { return Node.Identifier + ":" + MemberExpression; }
            set { }
        }

        public ContextVariable(params object[] items)
        {
            Items = items.ToList();
        }

        public virtual List<object> Items
        {
            get { return _items ?? (_items = new List<object>()); }
            set { _items = value; }
        }

        public override string ToString()
        {
            return MemberExpression;
        }

        public virtual string MemberExpression
        {
            get
            {

                return _memberExpression ?? (_memberExpression =

                    string.Join(".", Items.Select(p =>
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
                if (Items.Count < 1)
                    return "Missing";

                var item = Items.Count > 1 ? Items[Items.Count - 2] : Items.Last();
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

        public bool IsSubVariable { get; set; }

        public string VariableType
        {
            get { return _variableType ?? (_variableType = SourceVariable.RelatedTypeName); }
            set { _variableType = value; }
        }

        public string ShortName
        {
            get { return Items.Last() as string; }
        }

        public ITypedItem SourceVariable { get; set; }
        public string[] FirstMembers { get; set; }
    }

    public class HandlerInVariable : ContextVariable
    {
        public HandlerInVariable(params object[] items)
            : base(items)
        {
        }

        public IEnumerable<string> MemberExpressionItems
        {
            get
            {
                if (Input != null)
                {
                    yield return Input.Name;
                }
                if (Component != null)
                {
                    yield return Component.Name;
                }
                else
                {
                    yield return "Item";
                }
                if (SourceVariable != null)
                {
                    yield return SourceVariable.Name;
                }
                else
                {
                    foreach (var item in Items)
                    {
                        yield return item.ToString();
                    }
                }
            }
        }

        public override string MemberExpression
        {
            get { return string.Join(".", MemberExpressionItems.ToArray()); }
        }

        public HandlerNode HandlerNode { get; set; }
        public HandlerIn Input { get; set; }
        public ComponentNode Component { get; set; }
    }

    public interface ICodeOutput
    {
        IEnumerable<IContextVariable> GetAllContextVariables();
        IEnumerable<IContextVariable> GetContextVariables();

        void WriteCode(TemplateContext ctx);
    }
    public class ActionNode : ActionNodeBase, ICodeOutput
    {
        public override IEnumerable<IGraphItem> GraphItems
        {
            get
            {
                foreach (var item in InputVars)
                    yield return item;
                foreach (var item in OutputVars)
                    yield return item;
            }
        }

        public string VarName
        {
            get
            {
                var result = string.Empty;
                for (int index = 0; index < Identifier.Length; index++)
                {
                    char c = Identifier[index];

                    if (Char.IsLetter(c))
                        result += c;
                }
                return result;
            }
        }

        public void WriteLeft(TemplateContext ctx)
        {
            foreach (var input in InputVars)
            {
                var actionOutput = input.InputFrom<ActionOut>();
                var actionNode = actionOutput.Node as ActionNode;
                if (actionNode != null)
                {
                    actionNode.WriteLeft(ctx);
                }
            }
        }

        public void WriteRight(TemplateContext ctx)
        {
            this.Right.WriteCode(ctx);
        }

        public override void WriteCode(TemplateContext ctx)
        {
            if (Meta == null) return;

            if (!string.IsNullOrEmpty(Meta.Type.Namespace))
                ctx.TryAddNamespace(Meta.Type.Namespace);

            var outputtedNodes = new List<ActionNode>();
            foreach (var input in InputVars)
            {
                var actionOutput = input.InputFrom<ActionOut>();
                if (actionOutput == null) continue;
                var actionNode = actionOutput.Node as ActionNode;
                
                if (actionNode != null)
                {
                    if (outputtedNodes.Contains(actionNode)) continue;

                    actionNode.WriteCode(ctx);
                    outputtedNodes.Add(actionNode);
                }
            }


            if (Meta.Method == null)
            {
                WriteActionClassExecute(ctx);
            }
            else
            {
                WriteMethodCall(ctx);
            }

            
            var hasInferredOutput = false;
            foreach (var output in OutputVars.OfType<ActionBranch>().Select(p=>p.OutputTo<ActionNode>()))
            {
                if (output == null) continue;
                //output.WriteCode(ctx);
                hasInferredOutput = true;
            }

            if (!hasInferredOutput)
            {
                if (Right != null)
                {
                    Right.WriteCode(ctx);
                }
            }
            

        }
        public virtual void WriteCode2(TemplateContext ctx)
        {
            if (Meta == null) return;
            if (Meta != null)
            {
                if (!string.IsNullOrEmpty(Meta.Type.Namespace))
                    ctx.TryAddNamespace(Meta.Type.Namespace);
            }

            if (Meta.Method == null)
            {
                WriteActionClassExecute(ctx);
            }
            else
            {
                WriteMethodCall(ctx);
            }

            base.WriteCode(ctx);
        }

        private void WriteActionClassExecute(TemplateContext ctx)
        {
            var varStatement = ctx.CurrentDeclaration._private_(Meta.Type, VarName);
            varStatement.InitExpression = new CodeObjectCreateExpression(Meta.Type);

            foreach (var item in GraphItems.OfType<GenericSlot>())
            {
                var contextVariable = item.InputFrom<IContextVariable>();
                if (contextVariable == null) continue;
                ctx._("{0}.{1} = {2}", varStatement.Name, item.Name, contextVariable.Name);
            }


            ctx._("{0}.System = System", varStatement.Name);


            foreach (var item in OutputVars.OfType<ActionBranch>())
            {
                var branchOutput = item.OutputTo<SequenceItemNode>();
                if (branchOutput == null) continue;
                ctx._("{0}.{1} = {2}", varStatement.Name, item.Name, branchOutput.Name);
                var method = ctx.CurrentDeclaration.protected_func(typeof (void).ToCodeReference(), branchOutput.Name);
                ctx.PushStatements(method.Statements);
                branchOutput.WriteCode(ctx);
                ctx.PopStatements();
            }
            ctx._if("!{0}.Execute()", varStatement.Name).TrueStatements._("return");


            foreach (var item in OutputVars.OfType<ActionOut>())
            {
                var contextVariable = item.OutputTo<IContextVariable>();
                if (contextVariable == null) continue;
                ctx._("{0} = {1}.{2}", contextVariable.Name, varStatement.Name, item.Name);
            }
        }

        private void WriteMethodCall(TemplateContext ctx)
        {
            foreach (var item in OutputVars.OfType<ActionOut>())
            {
                ctx._("{0} {1}", item.VariableType, item.VariableName);
            }
            var invoker = new CodeMethodInvokeExpression(new CodeSnippetExpression(Meta.Type.Name), Meta.Method.Name);
            var parameters = Meta.Method.GetParameters();
            foreach (var item in parameters)
            {
                // ALL OUTPUTS FIRST
                if (item.IsOut || item.ParameterType == typeof (Action))
                {
                    var item1 = item;
                    var output = OutputVars.FirstOrDefault(p => p.ActionFieldInfo.DisplayType.ParameterName == item1.Name);

                    // If its a BRANCH OUTPUT
                    if (item1.ParameterType == typeof (Action))
                    {
                        var branchOutput = output.OutputTo<SequenceItemNode>();
                        if (branchOutput != null)
                        {
                            invoker.Parameters.Add(new CodeSnippetExpression(branchOutput.Name));
                            var method = ctx.CurrentDeclaration.protected_func(typeof (void).ToCodeReference(),
                                branchOutput.Name);
                            ctx.PushStatements(method.Statements);
                            branchOutput.WriteCode(ctx);
                            ctx.PopStatements();
                        }
                        else
                        {
                            invoker.Parameters.Add(new CodeSnippetExpression("null"));
                        }
                    }
                        // IF ITS A REGULAR OUTPUT
                    else
                    {
                        var outputVariable = output.OutputTo<IContextVariable>();
                        if (outputVariable != null)
                        {
                            invoker.Parameters.Add(new CodeSnippetExpression("out " + outputVariable.Name));
                        }
                        else
                        {
                            var actionOut = output as ActionOut;
                            if (actionOut != null)
                            {
                                invoker.Parameters.Add(new CodeSnippetExpression("out " + actionOut.VariableName));
                            }
                            
                        }
                    }
                }
                else
                {
                    var item1 = item;
                    var input = InputVars.FirstOrDefault(p => p.ActionFieldInfo.DisplayType.ParameterName == item1.Name);
                    var inputVar = input.InputFrom<IContextVariable>();

                    if (inputVar != null)
                    {
                        invoker.Parameters.Add(new CodeSnippetExpression(inputVar.VariableName));
                    }
                    else
                    {
                        invoker.Parameters.Add(
                            new CodeSnippetExpression(string.Format("default({0})", item.ParameterType.Name)));
                    }
                }
            }

            if (Meta.Method.ReturnType != typeof (void))
            {
                var result = OutputVars.FirstOrDefault(p => p.Name == "Result");
                if (result != null)
                {
                    var outputVar = result.OutputTo<IContextVariable>();
                    if (outputVar != null)
                    {
                        var assign = new CodeAssignStatement(new CodeSnippetExpression(outputVar.VariableName),
                            invoker);
                        ctx.CurrentStatements.Add(assign);
                    }
                    else
                    {
                        ctx.CurrentStatements.Add(invoker);
                    }
                }
                else
                {
                    ctx.CurrentStatements.Add(invoker);
                }
            }
            else
            {
                ctx.CurrentStatements.Add(invoker);
            }
        }

        private ActionMetaInfo _meta;
        private string _metaType;
        private IActionIn[] _inputVars;
        private IActionOut[] _outputVars;

        public ActionMetaInfo Meta
        {
            get
            {
                if (string.IsNullOrEmpty(MetaType))
                    return null;
                if (!uFrameECS.Actions.ContainsKey(MetaType))
                {
                    InvertApplication.LogError(string.Format("{0} action was not found in graph {1}.", MetaType, this.Graph.Name));
                    return null;
                }
                return _meta ?? (_meta = uFrameECS.Actions[MetaType]);
            }
            set
            {
                _meta = value;
                _metaType = value.FullName;
            }
        }

        [JsonProperty]
        public string MetaType
        {
            get { return _metaType; }
            set
            {
                _metaType = value;


            }
        }


        public IActionIn[] InputVars
        {
            get { return _inputVars ?? (_inputVars = GetInputVars().ToArray()); }
            set { _inputVars = value; }
        }

        private IEnumerable<IActionIn> GetInputVars()
        {
            var meta = Meta;
            if (meta != null)
            {
                foreach (var item in Meta.ActionFields.Where(p => p.DisplayType is In))
                {
                    var variableIn = new ActionIn()
                    {
                        ActionFieldInfo = item,
                        Node = this,
                        Identifier = this.Identifier + ":" + meta.Type.Name + ":" + item.Name
                    };
                    yield return variableIn;
                }
            }
        }

        public IActionOut[] OutputVars
        {
            get { return _outputVars ?? (_outputVars = GetOutputVars().ToArray()); }
            set { _outputVars = value; }
        }

        public IEnumerable<IActionOut> GetOutsWithType<TType>()
        {
            return OutputVars.Where(p => p.ActionFieldInfo.Type.IsAssignableFrom(typeof(TType)));
        }

        public IEnumerable<IActionIn> GetInsWithType<TType>()
        {
            return InputVars.Where(p => p.ActionFieldInfo.Type.IsAssignableFrom(typeof(TType)));
        }

        private IEnumerable<IActionOut> GetOutputVars()
        {
            var meta = Meta;
            if (meta != null)
            {
                foreach (var item in Meta.ActionFields.Where(p => p.DisplayType is Out))
                {
                    if (item.Type == typeof(Action))
                    {
                        var variableOut = new ActionBranch()
                        {
                            ActionFieldInfo = item,
                            Node = this,
                            Identifier = this.Identifier + ":" + meta.Type.Name + ":" + item.Name
                        };
                        yield return variableOut;
                    }
                    else
                    {
                        var variableOut = new ActionOut()
                        {
                            ActionFieldInfo = item,
                            Node = this,
                            Identifier = this.Identifier + ":" + meta.Type.Name + ":" + item.Name
                        };
                        yield return variableOut;
                    }


                }
            }
        }




    }

    public partial interface IActionConnectable : IDiagramNodeItem, IConnectable
    {
    }

    public interface IActionItem : IDiagramNodeItem
    {
        ActionFieldInfo ActionFieldInfo { get; set; }
        string VariableName { get; }
    }
    public interface IActionIn : IActionItem
    {
     
    }

    public interface IActionOut : IActionItem
    {
        string VariableType { get; }
    }

    public class ActionIn : SingleInputSlot<IContextVariable>, IActionIn
    {
        public ActionFieldInfo ActionFieldInfo { get; set; }

        

        public string VariableName
        {
            get
            {
                var actionNode = Node as ActionNode;
                return actionNode.Meta.Type.Name + "_" + Name;
                //var str = string.Empty;
                //foreach (var c in Node.Identifier)
                //{
                //    if (Char.IsLetter(c))
                //    {
                //        str
                //    }
                //}
            }
        }

        public override string Name
        {
            get { return ActionFieldInfo.Name; }
            set { base.Name = value; }
        }
    }

    public class ActionOut : SingleOutputSlot<IContextVariable>, IActionOut, IContextVariable
    {
        public ActionFieldInfo ActionFieldInfo { get; set; }
        public override string Name
        {
            get { return ActionFieldInfo.Name; }
            set { base.Name = value; }
        }
        public string VariableName
        {
            get
            {
                var actionNode = Node as ActionNode;
                var str = actionNode.Meta.Type.Name + "_";
                if (actionNode.Meta.Method != null)
                {
                    str += actionNode.Meta.Method.Name + "_";
                }
                return str + Name;
                //var str = string.Empty;
                //foreach (var c in Node.Identifier)
                //{
                //    if (Char.IsLetter(c))
                //    {
                //        str
                //    }
                //}
            }
            set
            {
                
            }
        }
        public string ShortName
        {
            get { return ActionFieldInfo.Name; }
            set
            {
                
            }
        }

        public ITypedItem SourceVariable { get; set; }

        //public string VariableName
        //{
        //    get { return Node.Name + ActionFieldInfo.Name; }
        //    set
        //    {
                
        //    }
        //}

        public string AsParameter
        {
            get { return Name.ToLower(); }
        }

        public bool IsSubVariable { get; set; }

        public string VariableType
        {
            get
            {
                
                return ActionFieldInfo.Type.FullName;
            }
        }
    }

    public class ActionBranch : SingleOutputSlot<ActionNode>, IActionOut
    {
        public string VariableName
        {
            get
            {
                var actionNode = Node as ActionNode;
                var str = actionNode.Meta.Type.Name + "_";
                if (actionNode.Meta.Method != null)
                {
                    str += actionNode.Meta.Method.Name + "_";
                }
                return str + Name;
            }
        }
        public ActionFieldInfo ActionFieldInfo { get; set; }
        public override string Name
        {
            get { return ActionFieldInfo.Name; }
            set { base.Name = value; }
        }

        public string VariableType { get; set; }
    }
}
