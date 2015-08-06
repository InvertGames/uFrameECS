using System.CodeDom;
using Invert.Json;

namespace Invert.uFrame.ECS
{
    using Invert.Core;
    using Invert.Core.GraphDesigner;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface ISetupCodeWriter
    {
        void WriteSetupCode(TemplateContext ctx);
    }

    public interface IFilterInput : IDiagramNodeItem
    {
        string HandlerPropertyName { get;  }
        IMappingsConnectable FilterNode { get; }
        string MappingId { get; }
        // IEnumerable<IContextVariable> GetVariables();
    }

    public class HandlerNode : HandlerNodeBase, ISetupCodeWriter, ICodeOutput, IFilterInput
    {
        private string _eventIdentifier;
        private EventNode _eventNode;
        private EventMetaInfo _meta;
        private string _metaType;
        private HandlerIn[] _contextInputs;

        public void Accept(IHandlerNodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override bool AllowInputs
        {
            get
            {
                return !HandlerInputs.Any();
                
            }
        }

     
        public string HandlerMethodName
        {
            get { return Name + "Handler"; }
        }

        public string HandlerFilterMethodName
        {
            get { return Name + "Filter"; }
        }





        //[JsonProperty]
        //public string EventIdentifier
        //{
        //    get { return _eventIdentifier; }
        //    set
        //    {
        //        _eventIdentifier = value;
        //        _eventNode = null;

        //    }
        //}

        //public override IEnumerable<IItem> PossibleMappings
        //{
        //    get
        //    {
        //        // TODO Make this work or remove it one
        //        return Meta.Members.Cast<IItem>();
        //        //                return base.PossibleMappings;
        //    }
        //}

        //public EventNode EventNode
        //{
        //    get { return _eventNode ?? (_eventNode = Project.NodeItems.FirstOrDefault(p => p.Identifier == EventIdentifier) as EventNode); }

        //}

        public EventMetaInfo Meta
        {
            get
            {
                if (string.IsNullOrEmpty(MetaType))
                    return null;
                if (!uFrameECS.Events.ContainsKey(MetaType))
                {
                    InvertApplication.Log(string.Format("{0} type not found.", MetaType));
                    return null;
                }
                return _meta ?? (_meta = uFrameECS.Events[MetaType]);
            }
            set
            {
                _meta = value;
                _metaType = value.Type.FullName;
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

        public IMappingsConnectable ContextNode
        {
            get { return this.InputFrom<IMappingsConnectable>(); }
        }

        public IEnumerable<IFilterInput> FilterInputs
        {
            get
            {
                if (this.FilterNode != null)
                yield return this;
                foreach (var handlerIn in HandlerInputs)
                {
                    if (handlerIn.FilterNode != null)
                    yield return handlerIn;
                }
            }
        }

        public override IEnumerable<IGraphItem> GraphItems
        {
            get
            {
                foreach (var item in HandlerInputs)
                    yield return item;

                foreach (var item in this.PersistedItems)
                    yield return item;
                //foreach (var item in ContextVariables)
                //{
                //    yield return item;
                //}
            }
        }

        public override IEnumerable<IContextVariable> GetContextVariables()
        {
            var evtNode = Meta;
            if (evtNode != null && !evtNode.SystemEvent)
            {
                yield return new ContextVariable("Event")
                {
                    Repository = this.Repository,
                    Node = this,
                    VariableType = evtNode.Type.FullName
                    //SourceVariable = select as GenericNode
                };

                foreach (var child in evtNode.Members)
                {
                    yield return new ContextVariable("Event", child.Name)
                    {
                        Repository = this.Repository,
                        Node = this,
                        IsSubVariable = true,
                        VariableType = child.Type.FullName
                    };
                }
            }
            foreach (var input in FilterInputs)
            {
                var filter = input.FilterNode;
                foreach (var item in filter.GetVariables(input))
                {
                    yield return item;
                }
            }
            
        }

 
        public IEnumerable<IContextVariable> Vars {
            get { return GetAllContextVariables(); }
        }
        public override void WriteCode(TemplateContext ctx)
        {
           // var defaultFilter = ContextNode;
            //base.WriteCode(ctx);
            ctx._("var {0} = new {1}()", this.Meta.Type.Name, HandlerMethodName);
            ctx._("{0}.System = this", Meta.Type.Name);
            if (!Meta.SystemEvent)
            {
                ctx._("{0}.Event = data", Meta.Type.Name);
            }
            
            foreach (var item in this.FilterInputs)
            {
                var filter = item.FilterNode;
                if (filter == null) continue;
                ctx._("{0}.{1} = {2}", Meta.Type.Name, item.HandlerPropertyName, item.HandlerPropertyName.ToLower());
            }

            ctx._("{0}.Execute()", Meta.Type.Name);

            //var seq = this.OutputTo<ActionNode>();
            //foreach (var item in this.Use)
            //{
            //}
        }

        public void WriteSetupCode(TemplateContext ctx)
        {
            if (Meta == null) return;
            if (Meta.SystemEvent)
            {
                var sysMethodName = Meta.SystemEventMethod;
                var systemMethod = ctx.CurrentDeclaration.Members.OfType<CodeMemberMethod>()
                    .FirstOrDefault(p => p.Name == sysMethodName) ?? ctx.CurrentDeclaration.public_func(null, Meta.SystemEventMethod);
                // systemMethod.Statements.Add(new )
                ctx.PushStatements(systemMethod.Statements);
                LoopContextHandler(ctx);

                ctx.PopStatements();
            }
            var handlerMethod = ctx.CurrentDeclaration.protected_func(typeof(void), HandlerMethodName);
            var defaultFilter = ContextNode;
            if (!Meta.SystemEvent)
            {
                var handlerFilterMethod = ctx.CurrentDeclaration.protected_func(typeof(void), HandlerFilterMethodName);

                handlerFilterMethod.Parameters.Add(new CodeParameterDeclarationExpression(
                    Meta.Type.Name,
                    "data"
                    ));

                handlerMethod.Parameters.Add(new CodeParameterDeclarationExpression(
                     Meta.Type.Name,
                     "data"
                 ));

                ctx.PushStatements(handlerFilterMethod.Statements);

                var handlerInvoker = new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), HandlerMethodName);
                handlerInvoker.Parameters.Add(new CodeSnippetExpression("data"));
                if (Meta.CodeWriter != null)
                {
                    Meta.CodeWriter.WriteFilterMethod(this, ctx, handlerFilterMethod, handlerInvoker);
                }
                else
                {

                 

                    if (HandlerInputs.Any())
                    {
                        foreach (var item in FilterInputs)
                        {
                            var filter = item.FilterNode;
                            if (filter == null) continue;

                            ctx._("var {0} = {1}", filter.GetContextItemName(item.Name), filter.MatchAndSelect("data." + item.MappingId));
                            ctx._if("{0} == null", filter.GetContextItemName(item.Name)).TrueStatements._("return");
                            handlerInvoker.Parameters.Add(new CodeSnippetExpression(filter.GetContextItemName(item.Name)));
                        }
                        ctx.CurrentStatements.Add(handlerInvoker);
                    }
                    else if (FilterInputs.Any())
                    {
                        LoopContextHandler(ctx, true);
                    }
                    else
                    {
                        ctx.CurrentStatements.Add(handlerInvoker);
                    }
                }

                ctx.PopStatements();
            }

            WriteEnsureDispatchers(ctx, defaultFilter, handlerMethod);

            if (!Meta.SystemEvent)
            {
                if (Meta.CodeWriter != null)
                {
                    Meta.CodeWriter.WriteSetupMethod(this, ctx, handlerMethod);
                }
                else
                {
                    ctx._("this.OnEvent<{0}>().Subscribe(_=>{{ {1}(_); }}).DisposeWith(this)", Meta.Type.FullName, HandlerFilterMethodName);
                }
            }

            var prevMethod = ctx.CurrentMethod;
            ctx.CurrentMember = handlerMethod;
            ctx.PushStatements(handlerMethod.Statements);
            WriteCode(ctx);
            ctx.PopStatements();
            ctx.CurrentMember = prevMethod;
        }

        private void WriteEnsureDispatchers(TemplateContext ctx, IMappingsConnectable defaultFilter,
            CodeMemberMethod handlerMethod)
        {
            //if (defaultFilter != null)
            //{
            //    if (Meta.Dispatcher)
            //    {
            //        ctx._("EnsureDispatcherOnComponents<{0}>( {1} )", Meta.Type.Name,
            //            defaultFilter.DispatcherTypesExpression());
            //    }
            //    handlerMethod.Parameters.Add(new CodeParameterDeclarationExpression(defaultFilter.ContextTypeName,
            //        defaultFilter.GetContextItemName("entity")));
            //}
            //else
            //{
                foreach (var item in FilterInputs)
                {
                    var filter = item.FilterNode;
                    if (filter == null) continue;
                    if (Meta.Dispatcher)
                    {
                        ctx._("EnsureDispatcherOnComponents<{0}>( {1} )", Meta.Type.Name, filter.DispatcherTypesExpression());
                    }
                    handlerMethod.Parameters.Add(new CodeParameterDeclarationExpression(filter.ContextTypeName,
                        item.HandlerPropertyName.ToLower()));
                }
            //}
        }

        private void LoopContextHandler(TemplateContext ctx, bool isAggregatorEvent = false)
        {
            ctx.PushStatements(ctx._if("{0} != null", ContextNode.SystemPropertyName).TrueStatements);

            ctx._("var e = {0}.GetEnumerator()", ContextNode.EnumeratorExpression);

            var iteration = new CodeIterationStatement(
                new CodeSnippetStatement(string.Empty),
                new CodeSnippetExpression("e.MoveNext()"),
                new CodeSnippetStatement(string.Empty)
                );

            ctx.CurrentStatements.Add(iteration);
            ctx.PushStatements(iteration.Statements);
            if (isAggregatorEvent)
            {
                ctx._("{0}(data, e.Current)", HandlerMethodName);
            }
            else
            {
                ctx._("{0}(e.Current)", HandlerMethodName);
            }

            ctx.PopStatements();
            ctx.PopStatements();
        }

        public HandlerIn[] HandlerInputs
        {
            get { return _contextInputs ?? (_contextInputs = GetHandlerInputs().ToArray()); }
            set { _contextInputs = value; }
        }

        private IEnumerable<HandlerIn> GetHandlerInputs()
        {
            var meta = Meta;
            if (meta != null)
            {
                foreach (var item in Meta.Members)
                {
                    if (item.Type != typeof(int)) continue;
                    var variableIn = new HandlerIn()
                    {
                        Repository = Repository,
                        EventFieldInfo = item,
                        Node = this,
                        Identifier = this.Identifier + ":" + meta.Type.Name + ":" + item.Name
                    };
                    yield return variableIn;
                }
            }
        }

        public IEnumerable<ConnectionData> Connections { get; set; }

        public string HandlerPropertyName
        {
            get { return "Item"; }
        }

        public IMappingsConnectable FilterNode
        {
            get { return this.InputFrom<IMappingsConnectable>(); }
        }

        public string MappingId
        {
            get { return "EntityId"; }
        }
    }

    public interface IHandlerNodeVisitor
    {
        void Visit(IDiagramNodeItem item);
        
    }

    public class HandlerNodeVisitor : IHandlerNodeVisitor
    {
        private List<ActionNode> outputtedNodes = new List<ActionNode>();

        public void Visit(IDiagramNodeItem item)
        {
            var handlerNode = item as HandlerNode;
            var actionNode = item as ActionNode;
            var actionBranch = item as ActionBranch;
            var actionOut = item as IActionOut;
            var actionIn = item as IActionIn;
            var setVariableNode = item as SetVariableNode;
            var handlerIn = item as HandlerIn;
            if (handlerIn != null)
            {
                BeforeVisitHandlerIn(handlerIn);
                VisitHandlerIn(handlerIn);
                AfterVisitHandlerIn(handlerIn);
            }
            if (setVariableNode != null)
            {
                BeforeSetVariableHandler(setVariableNode);
                VisitSetVariable(setVariableNode);
                AfterVisitSetVariable(setVariableNode);
            }
            if (handlerNode != null)
            {
                BeforeVisitHandler(handlerNode);
                VisitHandler(handlerNode);
                AfterVisitHandler(handlerNode);
            }

            if (actionNode != null)
            {
                BeforeVisitAction(actionNode);
                VisitAction(actionNode);
                AfterVisitAction(actionNode);
            }
                
            if (actionBranch != null)
            {
                BeforeVisitBranch(actionBranch);
                VisitBranch(actionBranch);
                AfterVisitBranch(actionBranch);
            }
                
            if (actionOut != null)
            {
                BeforeVisitOutput(actionOut);
                VisitOutput(actionOut);
                AfterVisitOutput(actionOut);
            }
                
            if (actionIn != null)
            {
                BeforeVisitInput(actionIn);
                VisitInput(actionIn);
                AfterVisitInput(actionIn);
            }
                
        }

        public virtual void AfterVisitHandlerIn(HandlerIn handlerIn)
        {
            
        }

        public virtual void VisitHandlerIn(HandlerIn handlerIn)
        {
                
        }

        public virtual void BeforeVisitHandlerIn(HandlerIn handlerIn)
        {
                
        }

        public virtual void BeforeSetVariableHandler(SetVariableNode setVariableNode)
        {
            Visit(setVariableNode.VariableInputSlot);
            Visit(setVariableNode.ValueInputSlot);
        }

        public virtual void VisitSetVariable(SetVariableNode setVariableNode)
        {
   
        }

        private void AfterVisitSetVariable(SetVariableNode setVariableNode)
        {

        }

        public virtual void AfterVisitInput(IActionIn actionIn)
        {
            
        }

        public virtual void BeforeVisitHandler(HandlerNode handlerNode)
        {
            foreach (var item in handlerNode.HandlerInputs)
            {
                Visit(item);
            }
        }

        public virtual void AfterVisitHandler(HandlerNode handlerNode)
        {
            
        }


        public virtual void BeforeVisitBranch(ActionBranch actionBranch)
        {
                    
        }

        public virtual void AfterVisitBranch(ActionBranch actionBranch)
        {
                
        }

        public virtual void BeforeVisitOutput(IActionOut actionOut)
        {
                
        }

        public virtual void AfterVisitOutput(IActionOut actionIn)
        {
                
        }

        public virtual void BeforeVisitInput(IActionIn actionIn)
        {
            var actionOutput = actionIn.InputFrom<ActionOut>();
            if (actionOutput == null) return;
            var actionNode = actionOutput.Node as ActionNode;

            if (actionNode != null)
            {
                if (outputtedNodes.Contains(actionNode)) return;

                Visit(actionNode);
                outputtedNodes.Add(actionNode);
            }
        }

        public virtual void BeforeVisitAction(ActionNode actionNode)
        {
            var outputtedNodes = new List<ActionNode>();

            

            foreach (var input in actionNode.InputVars)
            {
                Visit(input);
            }
       
        }

        public virtual void AfterVisitAction(ActionNode actionNode)
        {

            
            var hasInferredOutput = false;
            foreach (var output in actionNode.OutputVars.OfType<ActionOut>())
            {
                Visit(output);
            }
            foreach (var output in actionNode.OutputVars.OfType<ActionBranch>())
            {
                Visit(output);
                if (output.OutputTo<ActionNode>() != null)
                {
                    hasInferredOutput = true;
                }

            }
            if (!hasInferredOutput & actionNode.Right != null)
            {
                Visit(actionNode.Right);
            }
        }
        public virtual void VisitAction(ActionNode actionNode)
        {

        }

        public virtual void VisitBranch(ActionBranch output)
        {
            var item = output.OutputTo<SequenceItemNode>();
            if (item != null)
            {
                Visit(item);
            }
        }

        public virtual void VisitOutput(IActionOut output)
        {
                
        }

        public virtual void VisitInput(IActionIn input)
        {
            
        }

        public virtual void VisitHandler(HandlerNode handlerNode)
        {
            Visit(handlerNode.Right);
        }
    }

    public class HandlerIn : SingleInputSlot<IMappingsConnectable>, IFilterInput
    {
        public IMappingsConnectable FilterNode
        {
            get { return this.InputFrom<IMappingsConnectable>(); }
        }

        public string MappingId
        {
            get { return EventFieldInfo.Name; }
        }

        public EventFieldInfo EventFieldInfo { get; set; }

        public override string Name
        {
            get { return EventFieldInfo.Name; }
            set { base.Name = value; }
        }

        public string HandlerPropertyName
        {
            get { return Name; }
        }
    }

    public partial interface IMappingsConnectable : Invert.Core.GraphDesigner.IDiagramNode, Invert.Core.GraphDesigner.IConnectable
    {
        System.Collections.Generic.IEnumerable<ComponentNode> SelectComponents { get; }

        string GetContextItemName(string mappingId);

        string ContextTypeName { get; }

        string SystemPropertyName { get; }

        string EnumeratorExpression { get; }

        IEnumerable<IContextVariable> GetVariables(IFilterInput filterInput);

        string MatchAndSelect(string mappingExpression);

        string DispatcherTypesExpression();
    }

    public partial interface IOnEventConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable
    {
    }

    public interface IHandlerCodeWriter
    {
        Type For { get; }

        void WriteFilterMethod(HandlerNode handlerNode, TemplateContext ctx, CodeMemberMethod handlerFilterMethod, CodeMethodInvokeExpression invoker);

        void WriteSetupMethod(HandlerNode handlerNode, TemplateContext ctx, CodeMemberMethod handlerMethod);
    }

    public interface IHandlerCodeWriterFor<TFor> : IHandlerCodeWriter
    {
    }

    public abstract class HandlerCodeWriter<TFor> : IHandlerCodeWriterFor<TFor>
    {
        public Type For
        {
            get { return typeof(TFor); }
        }

        public abstract void WriteFilterMethod(HandlerNode handlerNode, TemplateContext ctx, CodeMemberMethod handlerFilterMethod,
            CodeMethodInvokeExpression invoker);

        public abstract void WriteSetupMethod(HandlerNode handlerNode, TemplateContext ctx, CodeMemberMethod handlerMethod);

    }
}