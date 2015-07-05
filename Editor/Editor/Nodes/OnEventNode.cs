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
    public class OnEventNode : OnEventNodeBase, IActionConnectable, ISetupCodeWriter
    {
        private string _eventIdentifier;
        private EventNode _eventNode;

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
            get { return Name + "Handler"; }
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

        public override IEnumerable<IContextVariable> ContextVariables
        {
            get
            {
                foreach (var item in Mappings)
                {
                    var filter = item.Filter;
                    if (filter == null) continue;
                    foreach (var select in filter.Select.Select(p => p.SourceItem).OfType<IDiagramNode>())
                    {
                        yield return new ContextVariable(select.Name)
                        {
                            //SourceVariable = select
                        };
                        foreach (var child in select.PersistedItems.OfType<ITypedItem>())
                        {
                            yield return new ContextVariable(select.Name, child.Name)
                            {
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
            base.WriteCode(ctx);
            if (EventNode.SystemEvent)
            {
                var systemMethod = ctx.CurrentDeclaration.public_func(null, EventNode.SystemEventMethod);
                // systemMethod.Statements.Add(new )
                ctx.PushStatements(systemMethod.Statements);
                ctx._("var e = {0}Context.Items.GetEnumerator()", FilterNode.Name);

                var iteration = new CodeIterationStatement(
                    new CodeSnippetStatement(string.Empty),
                    new CodeSnippetExpression("e.MoveNext()"),
                    new CodeSnippetStatement(string.Empty)
                    );

                ctx.CurrentStatements.Add(iteration);
                ctx.PushStatements(iteration.Statements);
                ctx._("{0}(e.Current)", HandlerMethodName);

                ctx.PopStatements();
                ctx.PopStatements();

            }
            //foreach (var component in FilterNode.Select)
            //{
            //    ctx._if("!ComponentSystem.TryGetComponent(ecsComponent.EntityId, out {0})", component.Name.ToLower())
            //        .TrueStatements
            //            ._("return");
            //}
        }

        public FilterNode FilterNode
        {
            get { return this.InputFrom<FilterNode>(); }
        }

        public void WriteSetupCode(TemplateContext ctx)
        {
            var handlerMethod = ctx.CurrentDeclaration.protected_func(typeof(void), HandlerMethodName);


            var defaultFilter = FilterNode;
            if (defaultFilter != null)
            {

                if (EventNode.Dispatcher)
                {
                    ctx._("EnsureDispatcherOnComponents<{0}Dispatcher>( {1}Context.WithAnyTypes )", EventNode.Name,
                        defaultFilter.Name);

                }
                handlerMethod.Parameters.Add(new CodeParameterDeclarationExpression(defaultFilter.Name + "ContextItem", "item"));
            }
            else
            {
                foreach (var item in Mappings)
                {
                    var filter = item.Filter;
                    if (filter == null) continue;
                    if (EventNode.Dispatcher)
                    {
                        ctx._("EnsureDispatcherOnComponents<{0}Dispatcher>( {1}Context.WithAnyTypes )", EventNode.Name, filter.Name);
                    }
                    handlerMethod.Parameters.Add(new CodeParameterDeclarationExpression(filter.Name + "ContextItem", item.Name.ToLower() + "Item"));
                }

            }

            if (!EventNode.SystemEvent && !EventNode.Dispatcher)
            {
                ctx._("this.OnEvent<{0}>().Subscribe(_=>{{ {1}(_); }}).DisposeWith(this)", Name, HandlerMethodName);
            }

            if (EventNode.Dispatcher)
            {
                ctx._("this.OnEvent<{0}Dispatcher>().Subscribe(_=>{{ {1}(_); }}).DisposeWith(this)", Name, HandlerMethodName);
            }

            ctx.PushStatements(handlerMethod.Statements);
            WriteCode(ctx);
            ctx.PopStatements();
        }


    }

    public partial interface IOnEventConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable
    {
    }
}
