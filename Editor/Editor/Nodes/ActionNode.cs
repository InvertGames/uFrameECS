using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Invert.Core;
using Invert.Core.GraphDesigner;
using Invert.Data;
using Invert.Json;
using JetBrains.Annotations;
using uFrame.Attributes;
using UnityEngine;

namespace Invert.uFrame.ECS
{
    public interface IContextVariable : IDiagramNodeItem
    {
        ITypedItem Source { get; }
        string VariableName { get; }
        ITypeInfo VariableType { get; }
        string ShortName { get; }
        string ValueExpression { get; }
        IEnumerable<IContextVariable> GetPropertyDescriptions();
    }


    public interface IVariableExpressionItem
    {
        string Expression { get; set; }
    }


    public class ContextVariable : GenericTypedChildItem, IContextVariable
    {

        private string _memberExpression;
        private ITypeInfo _variableType;
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
            get { return ShortName; }
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

        public ITypeInfo VariableType
        {
            get { return _variableType ?? (_variableType = (Source as ITypeInfo)); }
            set { _variableType = value; }
        }


        public IEnumerable<IContextVariable> GetPropertyDescriptions()
        {
            var sourceNode = VariableType;
            if (sourceNode != null)
            {
                foreach (var item in sourceNode.GetMembers())
                {
                    yield return new ContextVariable(VariableName, item.MemberName)
                    {
                        Source = item as ITypedItem,
                        Repository = Repository,
                        Name = item.MemberName,
                        Node = this.Node,
                        VariableType = item.MemberType
                    };
                }
            }

        }

        public string ShortName
        {
            get { return Items.Last() as string; }
        }

        public string ValueExpression
        {
            get { return VariableName; }

        }

        public ITypedItem Source { get; set; }
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
                if (Source != null)
                {
                    yield return Source.Name;
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
    public interface IVariableContextProvider : IDiagramNodeItem
    {
        IEnumerable<IContextVariable> GetAllContextVariables();
        IEnumerable<IContextVariable> GetContextVariables();
        IVariableContextProvider Left { get; }
    }
    public interface ICodeOutput : IVariableContextProvider
    {
        void WriteCode(TemplateContext ctx);
    }
    public class ActionNode : ActionNodeBase, ICodeOutput, IConnectableProvider
    {
        public override bool AllowMultipleOutputs
        {
            get { return false; }
        }
        public override bool AllowMultipleInputs
        {
            get { return false; }
        }

        public override Color Color
        {
            get { return Color.blue; }
        }

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
                var actionOutput = input.Item;
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
            foreach (var output in OutputVars.OfType<ActionBranch>().Select(p => p.OutputTo<ActionNode>()))
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

            foreach (var item in GraphItems.OfType<IActionIn>())
            {
                var contextVariable = item.Item;
                if (contextVariable == null) continue;
                ctx._("{0}.{1} = {2}", varStatement.Name, item.Name, contextVariable.Name);
            }


            ctx._("{0}.System = System", varStatement.Name);


            foreach (var item in OutputVars.OfType<ActionBranch>())
            {
                var branchOutput = item.OutputTo<SequenceItemNode>();
                if (branchOutput == null) continue;
                ctx._("{0}.{1} = {2}", varStatement.Name, item.Name, branchOutput.Name);
                var method = ctx.CurrentDeclaration.protected_func(typeof(void).ToCodeReference(), branchOutput.Name);
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
                if (item.IsOut || item.ParameterType == typeof(Action))
                {
                    var item1 = item;
                    var output = OutputVars.FirstOrDefault(p => p.ActionFieldInfo.DisplayType.ParameterName == item1.Name);

                    // If its a BRANCH OUTPUT
                    if (item1.ParameterType == typeof(Action))
                    {
                        var branchOutput = output.OutputTo<SequenceItemNode>();
                        if (branchOutput != null)
                        {
                            invoker.Parameters.Add(new CodeSnippetExpression(branchOutput.Name));
                            var method = ctx.CurrentDeclaration.protected_func(typeof(void).ToCodeReference(),
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
                    var inputVar = input.Item;

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

            if (Meta.Method.ReturnType != typeof(void))
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

                //// Get the generic contraints
                //foreach (var item in meta.Type.GetGenericArguments())
                //{
                //    //var typeConstraints = item.GetGenericParameterConstraints().Select(p => p.Name);
                //    var variableIn = new TypeSelection();
                //    variableIn.Node = this;
                //    variableIn.Repository = Repository;
                //    variableIn.Identifier = this.Identifier + ":" + item.Name;
                //    variableIn.ActionFieldInfo = new ActionFieldInfo()
                //    {
                //        DisplayType = new FieldDisplayTypeAttribute(item.Name,item.Name,true),
                //        Name = item.Name,
                //        Type = typeof(Type),
                //        MetaAttributes = new ActionAttribute[] { }
                //    };
                //    yield return variableIn;
                //}

                foreach (var item in Meta.ActionFields.Where(p => p.DisplayType is In))
                {
                    IActionIn variableIn;

                    variableIn = item.IsGenericArgument ? (IActionIn)new TypeSelection() : new ActionIn();
                    variableIn.Node = this;
                    variableIn.Repository = Repository;
                    variableIn.ActionFieldInfo = item;
                    variableIn.Identifier = this.Identifier + ":" + meta.Type.Name + ":" + item.Name;
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
                            Repository = Repository,
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
                            Repository = Repository,
                            ActionFieldInfo = item,
                            Node = this,
                            Identifier = this.Identifier + ":" + meta.Type.Name + ":" + item.Name
                        };
                        yield return variableOut;
                    }


                }
            }
        }


        public IEnumerable<IConnectable> Connectables
        {
            get
            {
                foreach (var item in InputVars) yield return item;
                foreach (var item in OutputVars) yield return item;
            }
        }

        public Breakpoint BreakPoint
        {
            get { return Repository.All<Breakpoint>().FirstOrDefault(p => p.ForIdentifier == this.Identifier); }
        }
    }
    public class Breakpoint : IDataRecord
    {
        public IRepository Repository { get; set; }
        public string Identifier { get; set; }
        public bool Changed { get; set; }

        [JsonProperty]
        public string ForIdentifier { get; set; }

        public ActionNode Action
        {
            get { return Repository.GetSingle<ActionNode>(ForIdentifier); }
        }
    }
    public partial interface IActionConnectable : IDiagramNodeItem, IConnectable
    {
    }

    public interface IActionItem : IDiagramNodeItem
    {
        ActionFieldInfo ActionFieldInfo { get; set; }
        string VariableName { get; }
        ITypeInfo VariableType { get; }

    }
    public interface IActionIn : IActionItem
    {
        IContextVariable Item { get; }

    }

    public interface IActionOut : IActionItem
    {

    }

    //public class GroupIn : SelectionFor<IMappingsConnectable, GroupSelection>, IActionIn
    //{
    //    public override bool AllowInputs
    //    {
    //        get { return false; }
    //    }

    //    public override IEnumerable<IDataRecord> GetAllowed()
    //    {
    //        foreach (var item in Repository.AllOf<IMappingsConnectable>())
    //        {
    //            yield return item;
    //        }
    //    }

    //    public ActionFieldInfo ActionFieldInfo { get; set; }
    //    public string VariableName
    //    {
    //        get
    //        {
    //            var actionNode = Node as ActionNode;
    //            return actionNode.Meta.Type.Name + "_" + Name;
    //        }
    //    }

    //    public object VariableType { get { return typeof(Type).Name; } }

    //    public override string Name
    //    {
    //        get { return ActionFieldInfo.Name; }
    //        set { base.Name = value; }
    //    }



    //    IContextVariable IActionIn.Item
    //    {
    //        get { return null; }
    //    }
    //}

    public class ActionIn : SelectionFor<IContextVariable, VariableSelection>, IActionIn
    {
        private string _variableName;

        public override bool CanInputFrom(IConnectable output)
        {
            var outputVariable = output as IContextVariable;
            if (outputVariable != null)
            {
                if (!outputVariable.VariableType.IsAssignableTo(VariableType))
                {
                    return false;
                }
            }
            return true;
        }

        public ActionFieldInfo ActionFieldInfo { get; set; }

        public string VariableName
        {
            get
            {

                return _variableName ?? (_variableName = VariableNode.NewVariable);
            }
        }

        public ITypeInfo VariableType
        {
            get
            {
                if (ActionFieldInfo != null)
                {
                    return new SystemTypeInfo(ActionFieldInfo.Type);
                }
                return new SystemTypeInfo(typeof(object));
                //var item = Item;
                //if (item == null)
                //    return "object";
                //return Item.VariableType;
            }
        }

        public override string Name
        {
            get { return ActionFieldInfo.Name; }
            set { base.Name = value; }
        }

        public override IEnumerable<IDataRecord> GetAllowed()
        {
            var action = this.Node as IVariableContextProvider;
            if (action != null)
            {

                foreach (var item in action.GetAllContextVariables().Where(p=>p.VariableType.IsAssignableTo(VariableType)))
                    yield return item;
            }
            else
            {
                InvertApplication.Log("BS");
            }
        }
    }
    public class PropertyIn : SelectionFor<IContextVariable, VariableSelection>, IActionIn
    {
        public bool DoesAllowInputs;
        public override bool AllowInputs
        {
            get { return DoesAllowInputs; }

        }
        public IVariableContextProvider Handler
        {
            get { return Node.Filter as IVariableContextProvider; }
        }

        public ActionFieldInfo ActionFieldInfo { get; set; }

        public string VariableName
        {
            get
            {
                var item = Item;
                if (item == null)
                {
                    return "...";
                }
                return item.VariableName;
            }
        }

        public virtual ITypeInfo VariableType
        {
            get
            {
                var item = Item;
                if (item == null)
                    return new SystemTypeInfo(typeof(object));
                return Item.VariableType;
            }
        }

        //public override string Name
        //{
        //    get { return "Property"; }
        //    set { base.Name = value; }
        //}

        public override IEnumerable<IDataRecord> GetAllowed()
        {
            var action = this.Node as IVariableContextProvider;
            if (action != null)
            {
                foreach (var item in action.GetContextVariables())
                {
                    if (item.Source is PropertiesChildItem)
                    {
                        yield return item;
                    }
                }
            }
            else
            {
                var hn = Handler;
                if (hn != null)
                {
                    foreach (var item in hn.GetContextVariables())
                    {
                        yield return item;
                    }
                }
            }

        }
    }
    public class VariableIn : SelectionFor<IContextVariable, VariableSelection>, IActionIn
    {
        public bool DoesAllowInputs;
        public override bool AllowInputs
        {
            get { return DoesAllowInputs; }

        }
        public IVariableContextProvider Handler
        {
            get { return Node.Filter as IVariableContextProvider; }
        }

        public ActionFieldInfo ActionFieldInfo { get; set; }

        public string VariableName
        {
            get
            {
                var item = Item;
                if (item == null)
                {
                    return "...";
                }
                return item.VariableName;
            }
        }

        public virtual ITypeInfo VariableType
        {
            get
            {
                var item = Item;
                if (item == null)
                    return new SystemTypeInfo(typeof(object));
                return Item.VariableType;
            }
        }

        //public override string Name
        //{
        //    get { return "Property"; }
        //    set { base.Name = value; }
        //}

        public override IEnumerable<IDataRecord> GetAllowed()
        {
            //var action = this.Node as IVariableContextProvider;
            //if (action != null)
            //{
            //    foreach (var item in action.GetContextVariables())
            //    {
            //        yield return item;
            //    }
            //}
            //else
            //{
            var hn = Handler;
            if (hn != null)
            {
                foreach (var item in hn.GetContextVariables())
                {
                    yield return item;
                }
            }
            //}

        }
    }
    public class GroupSelection : InputSelectionValue
    {

    }
    public class VariableSelection : InputSelectionValue
    {

    }
    public class ActionOut : SingleOutputSlot<IContextVariable>, IActionOut, IContextVariable
    {
        private string _variableName;

        public override bool CanOutputTo(IConnectable input)
        {

            return true;
            return base.CanOutputTo(input);
        }

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
                return _variableName ?? (_variableName = VariableNode.NewVariable);
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

        public string ValueExpression
        {
            get { return VariableName; }
        }

        public ITypedItem Source { get; set; }

        public string AsParameter
        {
            get { return Name.ToLower(); }
        }

        public bool IsSubVariable { get; set; }

        public ITypeInfo VariableType
        {
            get
            {
                if (ActionFieldInfo.Type.IsGenericParameter)
                {
                    var typeSelection = this.Node.GraphItems.OfType<TypeSelection>()
                        .FirstOrDefault(
                            p =>
                                p.ActionFieldInfo.IsGenericArgument &&
                                p.ActionFieldInfo.Name == ActionFieldInfo.Type.Name);
                    if (typeSelection != null)
                    {
                        var item = typeSelection.Item as ITypeInfo;
                        if (item != null)
                        {
                            return item;
                        }
                    }
                }
                return new SystemTypeInfo(ActionFieldInfo.Type);
            }
        }

        public ActionNode ActionNode
        {
            get { return this.Node as ActionNode; }
        }
        public IEnumerable<IContextVariable> GetPropertyDescriptions()
        {
            
            return VariableType.GetPropertyDescriptions(this);
        }
    }

    public static class EcsReflectionExtensions
    {
        public static IEnumerable<IContextVariable> GetPropertyDescriptions(this ITypeInfo type, IContextVariable parent)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            foreach (var item in type.GetMembers())
            {
                yield return new ContextVariable(parent.VariableName, item.MemberName)
                {
                    Repository = parent.Repository,
                    Node = parent.Node,
                    VariableType = item.MemberType
                };
            }
        }
    }
    public class ActionBranch : SingleOutputSlot<ActionNode>, IActionOut, IVariableContextProvider
    {
        public override Color Color
        {
            get { return Color.blue; }
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
            }
        }
        public ActionFieldInfo ActionFieldInfo { get; set; }
        public override string Name
        {
            get { return ActionFieldInfo.Name; }
            set { base.Name = value; }
        }

        public ITypeInfo VariableType { get; set; }


        public IEnumerable<IContextVariable> GetAllContextVariables()
        {
            if (Left == null)
            {
                yield break;
            }
            foreach (var item in Left.GetAllContextVariables())
                yield return item;
        }

        public IEnumerable<IContextVariable> GetContextVariables()
        {
            yield break;
        }

        public IVariableContextProvider Left
        {
            get { return this.Node as SequenceItemNode; }
        }
    }




}
