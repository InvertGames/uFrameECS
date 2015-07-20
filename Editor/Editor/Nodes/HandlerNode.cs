using System.CodeDom;
using System.Runtime.InteropServices.ComTypes;

namespace Invert.uFrame.ECS
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;

    public interface ISetupCodeWriter
    {
        void WriteSetupCode(TemplateContext ctx);
    }
    public class HandlerNode : HandlerNodeBase, ISetupCodeWriter, ICodeOutput
    {

        private string _eventIdentifier;
        private EventNode _eventNode;
        private EventMetaInfo _meta;
        private string _metaType;
        private HandlerIn[] _contextInputs;

        [ProxySection("User Methods", SectionVisibility.WhenNodeIsNotFilter)]
        public IEnumerable<IDiagramNodeItem> UserMethods
        {
            get { return this.Graph.NodeItems.Where(_ => this.Locations.Keys.Contains(_.Identifier)).OfType<UserMethodNode>().Cast<IDiagramNodeItem>(); }
        }

        public string HandlerMethodName 
        {
            get { return Name + "Handler"; }
        }
        public string HandlerFilterMethodName
        {
            get { return Name + "Filter"; }
        }

        public IEnumerable<IMappingsConnectable> Contexts
        {
            get
            {
                if (this.ContextNode != null)
                {
                    yield return this.ContextNode;
                }
                foreach (var item in HandlerInputs.Select(p => p.Context)
                    .Where(p => p != null))
                {
                    yield return item;
                }
            }
        }
        public string InputNames
        {
            get
            {

                return
                    string.Join("_",
                            Contexts.Select(p => p.Name)
                            .ToArray());
            }
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

        
        public IEnumerable<IContextVariable> AllContextVariables
        {
            get
            {
                return ContextVariables;
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
        public override IEnumerable<IContextVariable> ContextVariables
        {
            get
            {
                var evtNode = Meta;
                if (evtNode != null && !evtNode.SystemEvent)
                {


                    yield return new ContextVariable("Event")
                        {
                            Node = this,
                            //SourceVariable = select as GenericNode
                        };


                    foreach (var child in evtNode.Members)
                    {
                        yield return new ContextVariable("Event", child.Name)
                        {
                            Node = this,
                            IsSubVariable = true,
                            VariableType = child.Type.FullName
                        };
                    }

                }
                var defaultFilter = this.InputFrom<IMappingsConnectable>();
                if (defaultFilter != null)
                {
                    foreach (var v in defaultFilter.GetVariables(""))
                    {
                        yield return v;
                    }

                }
                foreach (var item in HandlerInputs)
                {
                    var filter = item.Context;
                    if (filter == null) continue;
                    foreach (var v in filter.GetVariables(item.Name + "Item"))
                    {
                        yield return v;
                    }
                    
                }
        

            }
        }


        public override void WriteCode(TemplateContext ctx)
        {
            var defaultFilter = ContextNode;
            //base.WriteCode(ctx);
            ctx._("var {0} = new {1}()", this.Meta.Type.Name, HandlerMethodName);
            ctx._("{0}.System = this", Meta.Type.Name);
            if (!Meta.SystemEvent)
            {
                ctx._("{0}.Event = data", Meta.Type.Name);
            }

            if (HandlerInputs.Any())
            {
                if (defaultFilter != null)
                {
                    ctx._("{0}.EntityIdItem = {1}", Meta.Type.Name, "entityIdItem");
                }
                foreach (var item in this.HandlerInputs)
                {
                    var filter = item.Context;
                    if (filter == null) continue;
                    ctx._("{0}.{1} = {2}", Meta.Type.Name, filter.GetContextItemName(item.Name), filter.GetContextItemName(item.Name.ToLower()));
                }
            }
            else if (defaultFilter != null)
            {
                ctx._("{0}.{1} = {2}", Meta.Type.Name,defaultFilter.GetContextItemName("EntityId"), defaultFilter.GetContextItemName("entityId"));
            }

            
            ctx._("{0}.Execute()", Meta.Type.Name);

            //var seq = this.OutputTo<ActionNode>();
            //foreach (var item in this.Use)
            //{

            //}
        }

        public void WriteSetupCode(TemplateContext ctx)
        {
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
                var invoker = new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), HandlerMethodName);
                invoker.Parameters.Add(new CodeSnippetExpression("data"));
               
                if (HandlerInputs.Any())
                {
                    if (defaultFilter != null)
                    {
                        ctx._("var entityIdItem = {0}", defaultFilter.MatchAndSelect("data.EntityId"));
                        ctx._if("entityIdItem== null").TrueStatements._("return");
                        invoker.Parameters.Add(new CodeSnippetExpression("entityIdItem"));
                    }
                    foreach (var item in this.HandlerInputs)
                    {
                        var filter = item.Context;
                        if (filter == null) continue;

                        ctx._("var {0} = {1}", filter.GetContextItemName(item.Name), filter.MatchAndSelect("data." + item.Name));
                        ctx._if("{0} == null", filter.GetContextItemName(item.Name)).TrueStatements._("return");
                        invoker.Parameters.Add(new CodeSnippetExpression(filter.GetContextItemName(item.Name)));
                    }
                    ctx.CurrentStatements.Add(invoker);
                }
                else if (defaultFilter != null)
                {
                    LoopContextHandler(ctx, true);
                }
                else
                {
                    ctx.CurrentStatements.Add(invoker);
                }


                ctx.PopStatements();
            }

            if (defaultFilter != null)
            {

                if (Meta.Dispatcher)
                {
                    ctx._("EnsureDispatcherOnComponents<{0}>( {1} )", Meta.Type.Name,
                        defaultFilter.DispatcherTypesExpression());

                }
                handlerMethod.Parameters.Add(new CodeParameterDeclarationExpression(defaultFilter.ContextTypeName, defaultFilter.GetContextItemName("entityId")));
            }
            else
            {
                foreach (var item in HandlerInputs)
                {
                    var filter = item.Context;
                    if (filter == null) continue;
                    if (Meta.Dispatcher)
                    {
                        ctx._("EnsureDispatcherOnComponents<{0}>( {1} )", Meta.Type.Name, filter.DispatcherTypesExpression());
                    }
                    handlerMethod.Parameters.Add(new CodeParameterDeclarationExpression(filter.ContextTypeName, filter.GetContextItemName(item.Name.ToLower())));
                }

            }

            if (!Meta.SystemEvent)
            {
                ctx._("this.OnEvent<{0}>().Subscribe(_=>{{ {1}(_); }}).DisposeWith(this)", Meta.Type.FullName, HandlerFilterMethodName);
            }

            var prevMethod = ctx.CurrentMethod;
            ctx.CurrentMember = handlerMethod;
            ctx.PushStatements(handlerMethod.Statements);
            WriteCode(ctx);
            ctx.PopStatements();
            ctx.CurrentMember = prevMethod;
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
                    if (item.Type != typeof (int)) continue;
                    var variableIn = new HandlerIn()
                    {
                        EventFieldInfo = item,
                        Node = this,

                        Identifier = this.Identifier + ":" + meta.Type.Name + ":" + item.Name
                    };
                    yield return variableIn;
                }
            }
        }
        public IEnumerable<ConnectionData> Connections { get; set; }
    }
    public class HandlerIn : SingleInputSlot<IMappingsConnectable>
    {
        public IMappingsConnectable Context
        {
            get { return this.InputFrom<IMappingsConnectable>(); }
        }

        public EventFieldInfo EventFieldInfo { get; set; }

        public override string Name
        {
            get { return EventFieldInfo.Name; }
            set { base.Name = value; }
        }
    }

    public partial interface IMappingsConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable
    {
        System.Collections.Generic.IEnumerable<ComponentNode> WithAnyComponents { get; }
        System.Collections.Generic.IEnumerable<ComponentNode> SelectComponents { get; }
        string GetContextItemName(string mappingId);
        string ContextTypeName { get; }
        string SystemPropertyName { get; }
        string EnumeratorExpression { get; }
        IEnumerable<IContextVariable> GetVariables(string prefix);
        
        string MatchAndSelect(string mappingExpression);
        string DispatcherTypesExpression();
    }

    public partial interface IOnEventConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable
    {
    }
}
