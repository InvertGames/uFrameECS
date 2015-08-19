using System.CodeDom;
using Invert.Json;

namespace Invert.uFrame.ECS
{
    using Invert.Core;
    using Invert.Core.GraphDesigner;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface ISystemGroupProvider
    {
        IEnumerable<IMappingsConnectable> GetSystemGroups();
    }
    public class HandlerNode : HandlerNodeBase, ISetupCodeWriter, ICodeOutput, ISequenceNode, ISystemGroupProvider
    {
        private string _eventIdentifier;
        private EventNode _eventNode;
        private EventMetaInfo _meta;
        private string _metaType;
        private EntityGroupIn[] _contextInputs;

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


        public virtual string HandlerMethodName
        {
            get { return Name + "Handler"; }
        }

        public virtual string HandlerFilterMethodName
        {
            get { return Name + "Filter"; }
        }

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

        public virtual IEnumerable<IFilterInput> FilterInputs
        {
            get
            {
                foreach (var handlerIn in HandlerInputs)
                {
                    if (handlerIn.FilterNode != null)
                        yield return handlerIn;
                }
            }
        }

        public virtual string EventType
        {
            get { return Meta.Type.FullName; }
            set { throw new NotImplementedException(); }
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


        public IEnumerable<IContextVariable> Vars
        {
            get { return GetAllContextVariables(); }
        }
        public override void WriteCode(TemplateContext ctx)
        {
            var handlerMethod = IsSystemEvent ? WriteSystemHandler(ctx) : WriteHandler(ctx);
            var filterMethod = WriteHandlerFilter(ctx, handlerMethod);
            WriteEventSubscription(ctx, filterMethod, handlerMethod);
        }

        private CodeMemberMethod WriteSystemHandler(TemplateContext ctx)
        {
            var sysMethodName = Meta.SystemEventMethod;
            var systemMethod = ctx.CurrentDeclaration.Members.OfType<CodeMemberMethod>()
                .FirstOrDefault(p => p.Name == sysMethodName) ?? ctx.CurrentDeclaration.public_func(null, Meta.SystemEventMethod);
            // systemMethod.Statements.Add(new )
            ctx.PushStatements(systemMethod.Statements);
            LoopContextHandler(ctx);
            ctx.PopStatements();
            return systemMethod;
        }

        public bool IsSystemEvent
        {
            get
            {
                return Meta != null && Meta.SystemEvent;
            }
        }
        public virtual CodeMemberMethod WriteHandlerFilter(TemplateContext ctx, CodeMemberMethod handlerMethod)
        {
            var handlerFilterMethod = ctx.CurrentDeclaration.protected_func(typeof(void), HandlerFilterMethodName);

            handlerFilterMethod.Parameters.Add(new CodeParameterDeclarationExpression(EventType, "data"));

            var handlerInvoker = new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), HandlerMethodName);
     
            WriteHandlerInvoker(handlerInvoker, handlerFilterMethod);

            ctx.PushStatements(handlerFilterMethod.Statements);

            //foreach (var item in Scope)
            //{
            //    this.BeginWriteLoop(ctx, item.SourceItem as IMappingsConnectable);
            //    handlerMethod.Parameters.Add(new CodeParameterDeclarationExpression(item.SourceItem.Name, item.SourceItem.Name + "Item"));
            //}

            if (Meta != null && Meta.CodeWriter != null)
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
                    }
                }
                ctx.CurrentStatements.Add(handlerInvoker);
            }
            //foreach (var item in Scope)
            //{
            //    this.EndWriteLoop(ctx);
            //}

            ctx.PopStatements();
            return handlerFilterMethod;
        }

        public virtual CodeMemberMethod WriteHandler(TemplateContext ctx)
        {
            var handlerMethod = ctx.CurrentDeclaration.protected_func(typeof(void), HandlerMethodName);
            handlerMethod.Parameters.Add(new CodeParameterDeclarationExpression(
                 EventType,
                 "data"
             ));
            // Push the context on the code template
            var prevMethod = ctx.CurrentMethod;
            ctx.CurrentMember = handlerMethod;
            ctx.PushStatements(handlerMethod.Statements);
            // Now writing the handler method contents
            var name = "handler";
            ctx._("var {0} = new {1}()", name, HandlerMethodName);
            ctx._("{0}.System = this", name);
            
            WriteHandlerSetup(ctx, name, handlerMethod);

            ctx._("{0}.Execute()", name);
            // End handler method contents
            ctx.PopStatements();
            ctx.CurrentMember = prevMethod;
            return handlerMethod;
        }

        private void WriteHandlerSetup(TemplateContext ctx, string name, CodeMemberMethod handlerMethod)
        {
            if (!IsSystemEvent)
            {
                ctx._("{0}.Event = data", name);
            }
            foreach (var item in this.FilterInputs)
            {
                var filter = item.FilterNode;
                if (filter == null) continue;
                ctx._("{0}.{1} = {2}", name, item.HandlerPropertyName, item.HandlerPropertyName.ToLower());
                handlerMethod.Parameters.Add(new CodeParameterDeclarationExpression(filter.ContextTypeName,
                    item.HandlerPropertyName.ToLower()));
            }
        }

        protected virtual void WriteHandlerInvoker(CodeMethodInvokeExpression handlerInvoker, CodeMemberMethod handlerFilterMethod)
        {
            handlerInvoker.Parameters.Add(new CodeSnippetExpression("data"));
            foreach (var item in FilterInputs)
            {
                var filter = item.FilterNode;
                if (filter == null) continue;
                handlerInvoker.Parameters.Add(new CodeSnippetExpression(filter.GetContextItemName(item.Name)));
            } 
            //foreach (var item in Scope)
            //{
            //    handlerInvoker.Parameters.Add(new CodeSnippetExpression(item.SourceItem.Name + "Items.Current"));
            //}
        }


        public virtual void WriteEventSubscription(TemplateContext ctx, CodeMemberMethod filterMethod, CodeMemberMethod handlerMethod)
        {
            if (Meta != null && !Meta.SystemEvent && Meta.CodeWriter != null)
            {
                Meta.CodeWriter.WriteSetupMethod(this, ctx, handlerMethod);
            }
            else
            {
                ctx._("this.OnEvent<{0}>().Subscribe(_=>{{ {1}(_); }}).DisposeWith(this)", EventType, HandlerFilterMethodName);
            }
        }

        public virtual void WriteSetupCode(TemplateContext ctx)
        {
            WriteCode(ctx);
        }

        private void WriteEnsureDispatchers(TemplateContext ctx)
        {
            foreach (var item in FilterInputs)
            {
                var filter = item.FilterNode;
                if (filter == null) continue;
                if (Meta.Dispatcher)
                {
                    ctx._("EnsureDispatcherOnComponents<{0}>( {1} )", Meta.Type.Name, filter.DispatcherTypesExpression());
                }
            }
        }

        public virtual void BeginWriteLoop(TemplateContext ctx, IMappingsConnectable connectable)
        {

           // ctx.PushStatements(ctx._if("{0} != null", ContextNode.SystemPropertyName).TrueStatements);

            ctx._("var {0}Items = {1}.GetEnumerator()",connectable.Name, connectable.EnumeratorExpression);

            var iteration = new CodeIterationStatement(
                new CodeSnippetStatement(string.Empty),
                new CodeSnippetExpression(string.Format("{0}Items.MoveNext()", connectable.Name)),
                new CodeSnippetStatement(string.Empty)
                );

            ctx.CurrentStatements.Add(iteration);
            ctx.PushStatements(iteration.Statements);
        }

        public virtual void EndWriteLoop(TemplateContext ctx)
        {

            ctx.PopStatements();
        }
        private void LoopContextHandler(TemplateContext ctx, bool isAggregatorEvent = false)
        {
           
            if (isAggregatorEvent)
            {
                ctx._("{0}(data, e.Current)", HandlerMethodName);
            }
            else
            {
                ctx._("{0}(e.Current)", HandlerMethodName);
            }

        }

        public EntityGroupIn[] HandlerInputs
        {
            get { return _contextInputs ?? (_contextInputs = GetHandlerInputs().ToArray()); }
            set { _contextInputs = value; }
        }

        private IEnumerable<EntityGroupIn> GetHandlerInputs()
        {
            var meta = Meta;
            if (meta != null)
            {
                yield return new EntityGroupIn()
                {
                    Repository = Repository,
                    Node = this,
                    Identifier = this.Identifier + ":" + meta.Type.Name + ":" + "Group"
                };
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

        public virtual bool CanGenerate
        {
            get { return Meta != null; }
        }

        public virtual IEnumerable<IMappingsConnectable> GetSystemGroups()
        {
            //foreach (var item in Scope)
            //{
            //    yield return item.SourceItem as IMappingsConnectable;
            //}
            foreach (var input in FilterInputs)
            {
                var filter = input.FilterNode;
                if (filter != null)
                {
                    yield return filter;
                }
            }
        }
    }
}