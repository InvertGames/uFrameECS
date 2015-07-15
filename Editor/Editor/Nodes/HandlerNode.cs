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
        [ProxySection("User Methods", SectionVisibility.WhenNodeIsNotFilter)]
        public IEnumerable<IDiagramNodeItem> UserMethods
        {
            get { return this.Graph.NodeItems.Where(_ => this.Locations.Keys.Contains(_.Identifier)).OfType<UserMethodNode>().Cast<IDiagramNodeItem>(); }
        }
        public override string Name
        {
            get
            {
                if (EventNode == null)
                {
                    return "Event Not Found";
                }
                return EventNode.Name;
            }
            set { base.Name = value; }
        }

        public string HandlerMethodName
        {
            get { return Graph.Name + "_" + InputNames + "_" + Name + "Handler"; }
        }
        public string HandlerFilterMethodName
        {
            get { return InputNames + "_" + Name + "Filter"; }
        }

        public IEnumerable<ContextNode> Contexts
        {
            get
            {
                if (this.ContextNode != null)
                {
                    yield return this.ContextNode;
                }
                foreach (var item  in Mappings.Select(p => p.InputFrom<ContextNode>())
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

        [JsonProperty]
        public string EventIdentifier
        {
            get { return _eventIdentifier; }
            set
            {
                _eventIdentifier = value;
                _eventNode = null;

            }
        }

        public override IEnumerable<IItem> PossibleMappings
        {
            get
            {
                return EventNode.Properties.Cast<IItem>();
                //                return base.PossibleMappings;
            }
        }

        public EventNode EventNode
        {
            get { return _eventNode ?? (_eventNode = Project.NodeItems.FirstOrDefault(p => p.Identifier == EventIdentifier) as EventNode); }

        }
        public ContextNode ContextNode
        {
            get { return this.InputFrom<ContextNode>(); }
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
                foreach (var item in this.PersistedItems)
                {
                    yield return item;
                }
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
                var evtNode = EventNode;
                if (evtNode != null && !evtNode.SystemEvent)
                {


                    yield return new ContextVariable("Event")
                        {
                            Node = this,
                            //SourceVariable = select as GenericNode
                        };


                    foreach (var child in evtNode.PersistedItems.OfType<ITypedItem>())
                        {
                            yield return new ContextVariable("Event", child.Name)
                            {
                                Node = this,
                                IsSubVariable = true,
                                SourceVariable = child
                            };
                        }
                    
                }
                var defaultFilter = this.InputFrom<ContextNode>();
                if (defaultFilter != null)
                {
                    foreach (var select in defaultFilter.Select.Select(p => p.SourceItem).OfType<IDiagramNode>())
                    {

                        yield return new ContextVariable("EntityIdItem", select.Name)
                        {
                            Node = this,
                            //SourceVariable = select as GenericNode
                        };


                        foreach (var child in select.PersistedItems.OfType<ITypedItem>())
                        {
                            yield return new ContextVariable("EntityIdItem", select.Name, child.Name)
                            {
                                Node = this,
                                IsSubVariable = true,
                                SourceVariable = child
                            };
                        }
                    }
                }
                foreach (var item in Mappings)
                {
                    var filter = item.Context;
                    if (filter == null) continue;
                    foreach (var select in filter.Select.Select(p => p.SourceItem).OfType<IDiagramNode>())
                    {

                        yield return new ContextVariable(item.Name + "Item", select.Name)
                        {
                            Node = this,
                            //SourceVariable = select as GenericNode
                        };


                        foreach (var child in select.PersistedItems.OfType<ITypedItem>())
                        {
                            yield return new ContextVariable(item.Name + "Item", select.Name, child.Name)
                            {
                                Node = this,
                                IsSubVariable = true,
                                SourceVariable = child
                            };
                        }
                    }
                }
                if (EventNode == null)
                {
                    yield break;
                }

            }
        }


        public override void WriteCode(TemplateContext ctx)
        {
            var defaultFilter = ContextNode;
            //base.WriteCode(ctx);
            ctx._("var {0} = new {1}()", this.Name,HandlerMethodName);
            ctx._("{0}.System = this",this.Name);
            if (!EventNode.SystemEvent)
            {
                ctx._("{0}.Event = data", this.Name);
            }
            
            if (defaultFilter != null)
            {
                ctx._("{0}.EntityIdItem = {1}",this.Name, "entityIdItem");
            }
            foreach (var item in this.Mappings)
            {
                var filter = item.Context;
                if (filter == null) continue;
                ctx._("{0}.{1}Item = {1}Item", this.Name, item.Name.ToLower());
            }
            ctx._("{0}.Execute()", this.Name);

            //var seq = this.OutputTo<ActionNode>();
            //foreach (var item in this.Use)
            //{
                
            //}
        }

        public void WriteSetupCode(TemplateContext ctx)
        {
            if (EventNode.SystemEvent)
            {
                var sysMethodName = EventNode.SystemEventMethod;
                var systemMethod = ctx.CurrentDeclaration.Members.OfType<CodeMemberMethod>()
                    .FirstOrDefault(p => p.Name == sysMethodName) ?? ctx.CurrentDeclaration.public_func(null, EventNode.SystemEventMethod);
                // systemMethod.Statements.Add(new )
                ctx.PushStatements(systemMethod.Statements);
                LoopContextHandler(ctx);
        
                ctx.PopStatements();

            }
            var handlerMethod = ctx.CurrentDeclaration.protected_func(typeof(void), HandlerMethodName);
            var defaultFilter = ContextNode;
            if (!EventNode.SystemEvent)
            {
                var handlerFilterMethod = ctx.CurrentDeclaration.protected_func(typeof(void), HandlerFilterMethodName);

                handlerFilterMethod.Parameters.Add(new CodeParameterDeclarationExpression(
                    EventNode.ClassName,
                    "data"
                    ));

                handlerMethod.Parameters.Add(new CodeParameterDeclarationExpression(
                     EventNode.ClassName,
                     "data"
                 ));

                ctx.PushStatements(handlerFilterMethod.Statements);




                var invoker = new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), HandlerMethodName);
                invoker.Parameters.Add(new CodeSnippetExpression("data"));
                if (this.Mappings.Any())
                {
                    
                    if (defaultFilter != null)
                    {
                        ctx._("var entityIdItem = {0}Context.MatchAndSelect(data.EntityId)", defaultFilter.Name);
                        ctx._if("entityIdItem== null").TrueStatements._("return");
                        invoker.Parameters.Add(new CodeSnippetExpression("entityIdItem"));
                    }
                    foreach (var item in this.Mappings)
                    {
                        var filter = item.Context;
                        if (filter == null) continue;

                        ctx._("var {0}Item = {1}Context.MatchAndSelect(data.{2})", item.Name, filter.Name,
                            item.SourceItem.Name);
                        ctx._if("{0}Item == null", item.Name).TrueStatements._("return");
                        invoker.Parameters.Add(new CodeSnippetExpression(string.Format("{0}Item", item.Name)));
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

                if (EventNode.Dispatcher)
                {
                    ctx._("EnsureDispatcherOnComponents<{0}Dispatcher>( {1}Context.WithAnyTypes )", EventNode.Name,
                        defaultFilter.Name);

                }
                handlerMethod.Parameters.Add(new CodeParameterDeclarationExpression(defaultFilter.Name + "ContextItem",  "entityIdItem"));
            }
            else
            {
                foreach (var item in Mappings)
                {
                    var filter = item.Context;
                    if (filter == null) continue;
                    if (EventNode.Dispatcher)
                    {
                        ctx._("EnsureDispatcherOnComponents<{0}Dispatcher>( {1}Context.WithAnyTypes )", EventNode.Name, filter.Name);
                    }
                    handlerMethod.Parameters.Add(new CodeParameterDeclarationExpression(filter.Name + "ContextItem", item.Name.ToLower() + "Item"));
                }

            }

            if (!EventNode.SystemEvent)
            {
                ctx._("this.OnEvent<{0}>().Subscribe(_=>{{ {1}(_); }}).DisposeWith(this)", EventNode.ClassName, HandlerFilterMethodName);
            }

            var prevMethod = ctx.CurrentMethod;
            ctx.CurrentMember = handlerMethod;
            ctx.PushStatements(handlerMethod.Statements);
            WriteCode(ctx);
            ctx.PopStatements();
            ctx.CurrentMember = prevMethod;
        }

        private void LoopContextHandler(TemplateContext ctx, bool isAggregatorEvent= false)
        {
            ctx.PushStatements(ctx._if("{0}Context != null", ContextNode.Name).TrueStatements);

            ctx._("var e = {0}Context.Items.GetEnumerator()", ContextNode.Name);

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


        public IEnumerable<ConnectionData> Connections { get; set; }
    }

    public partial interface IOnEventConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable
    {
    }
}
