using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Invert.Core.GraphDesigner;
using Invert.IOC;
using Invert.uFrame.ECS;
using uFrame.ECS;

namespace Invert.uFrame.ECS.Templates
{
    public class EcsTemplates : DiagramPlugin
    {
        public override void Initialize(UFrameContainer container)
        {
            RegisteredTemplateGeneratorsFactory.RegisterTemplate<SystemNode,SystemTemplate>();
            RegisteredTemplateGeneratorsFactory.RegisterTemplate<ComponentNode,ComponentTemplate>();
            RegisteredTemplateGeneratorsFactory.RegisterTemplate<ComponentGroupNode,ComponentGroupTemplate>();
            RegisteredTemplateGeneratorsFactory.RegisterTemplate<ComponentGroupNode,ComponentGroupManagerTemplate>();
            RegisteredTemplateGeneratorsFactory.RegisterTemplate<EventNode,EventTemplate>();
            RegisteredTemplateGeneratorsFactory.RegisterTemplate<FilterNode,ContextTemplate>();
            RegisteredTemplateGeneratorsFactory.RegisterTemplate<FilterNode, ContextItemTemplate>();
            
            //RegisteredTemplateGeneratorsFactory.RegisterTemplate<OnEventNode,OnEventTemplate>();
            //RegisteredTemplateGeneratorsFactory.RegisterTemplate<PublishNode,PublishTemplate>();
        }
    }

    [TemplateClass(TemplateLocation.Both)]
    [RequiresNamespace("uFrame.Kernel")]
    public partial class SystemTemplate : IClassTemplate<SystemNode>
    {
        public IEnumerable<ComponentNode> Components
        {
            get
            {
                return Ctx.Data.Components.Select(p => p.SourceItem).OfType<ComponentNode>()
                    .Concat(Ctx.Data.Graph.NodeItems.OfType<ComponentNode>());
            }
        }

        public IEnumerable<ComponentGroupNode> ComponentGroups
        {
            get
            {
                return Ctx.Data.Components.Select(p => p.SourceItem).OfType<ComponentGroupNode>()
                    .Concat(Ctx.Data.Graph.NodeItems.OfType<ComponentGroupNode>()).Distinct();
            }
        }
        public IEnumerable<OnEventNode> EventHandlers
        {
            get
            {
                return Ctx.Data.EventHandlers;
            }
        }

        public IEnumerable<FilterNode> FilterNodes
        {
            get
            {
                foreach (var item in Ctx.Data.Project.NodeItems.OfType<FilterNode>())
                {

                    if (item.Graph.Identifier == Ctx.Data.Graph.Identifier)
                    {
                        yield return item;
                        continue;
                    }
                    if (Ctx.Data.Graph.PositionData.HasPosition(Ctx.Data.Graph.RootFilter,item))
                    {
                        yield return item;
                    }
                }
            }
        }
        public string OutputPath
        {
            get { return Path2.Combine(Ctx.Data.Graph.Name, "Systems"); }
        }

        public bool CanGenerate
        {
            get { return true; }
        }

        public void TemplateSetup()
        {

        }

        public TemplateContext<SystemNode> Ctx { get; set; }
    }

    [TemplateClass(TemplateLocation.Both)]
    [ForceBaseType(typeof(EcsComponent))]
    [RequiresNamespace("uFrame.ECS")]
    public partial class ComponentTemplate : IClassTemplate<ComponentNode>
    {
   
        public string OutputPath
        {
            get { return Path2.Combine(Ctx.Data.Graph.Name, "Components"); }
        }

        public bool CanGenerate
        {
            get { return true; }
        }

        public void TemplateSetup()
        {

        }

        public TemplateContext<ComponentNode> Ctx { get; set; }
    }

    [TemplateClass(TemplateLocation.DesignerFile)]
    [ForceBaseType(typeof(ComponentGroup))]
    [RequiresNamespace("uFrame.ECS")]
    [AsPartial]
    public partial class ComponentGroupTemplate : IClassTemplate<ComponentGroupNode>
    {
        public IEnumerable<ComponentNode> Components
        {
            get
            {
                return Ctx.Data.Components.Select(p => p.SourceItem).OfType<ComponentNode>();
            }
        }
        public string OutputPath
        {
            get { return Path2.Combine(Ctx.Data.Graph.Name, "Components"); }
        }

        public bool CanGenerate
        {
            get { return true; }
        }

        public void TemplateSetup()
        {

        }

        public TemplateContext<ComponentGroupNode> Ctx { get; set; }
    }


    [TemplateClass(TemplateLocation.Both,"{0}Manager")]
    [RequiresNamespace("uFrame.ECS")]
    [RequiresNamespace("uFrame.Kernel")]
    [RequiresNamespace("UniRx")]
    public partial class ComponentGroupManagerTemplate : IClassTemplate<ComponentGroupNode>
    {
        public IEnumerable<ComponentNode> Components
        {
            get
            {
                return Ctx.Data.Components.Select(p => p.SourceItem).OfType<ComponentNode>();
            }
        }
        public string OutputPath
        {
            get { return Path2.Combine(Ctx.Data.Graph.Name, "ComponentGroups"); }
        }

        public bool CanGenerate
        {
            get { return true; }
        }

        public void TemplateSetup()
        {
            if (Ctx.IsDesignerFile)
                Ctx.SetBaseType("FilterSystem<{0}>", Ctx.Data.Name);
        }

        public TemplateContext<ComponentGroupNode> Ctx { get; set; }
    }

    [TemplateClass(TemplateLocation.DesignerFile, "{0}Context"), AsPartial]
    [RequiresNamespace("uFrame.ECS")]
    [RequiresNamespace("uFrame.Kernel")]
    [RequiresNamespace("UniRx")]
    public partial class ContextTemplate : IClassTemplate<FilterNode>
    {
        public IEnumerable<ComponentNode> WithAnyComponents
        {
            get
            {
                return Ctx.Data.WithAny.Select(p => p.SourceItem).OfType<ComponentNode>();
            }
        }
        public IEnumerable<ComponentNode> SelectComponents
        {
            get
            {
                return Ctx.Data.Select.Select(p => p.SourceItem).OfType<ComponentNode>();
            }
        }
        public string OutputPath
        {
            get { return Path2.Combine(Ctx.Data.Graph.Name, "ComponentGroups"); }
        }

        public bool CanGenerate
        {
            get { return true; }
        }

        public void TemplateSetup()
        {

            this.Ctx.SetBaseType("ReactiveContext<{0}ContextItem>",Ctx.Data.Name);
        }

        public TemplateContext<FilterNode> Ctx { get; set; }
    }

    [TemplateClass(TemplateLocation.DesignerFile, "{0}ContextItem"), AsPartial]
    [RequiresNamespace("uFrame.ECS")]
    [RequiresNamespace("uFrame.Kernel")]
    [RequiresNamespace("UniRx")]
    public partial class ContextItemTemplate : ContextItem, IClassTemplate<FilterNode>
    {
        public IEnumerable<ComponentNode> WithAnyComponents
        {
            get
            {
                return Ctx.Data.WithAny.Select(p => p.SourceItem).OfType<ComponentNode>();
            }
        }
        public IEnumerable<ComponentNode> SelectComponents
        {
            get
            {
                return Ctx.Data.Select.Select(p => p.SourceItem).OfType<ComponentNode>();
            }
        }
        public string OutputPath
        {
            get { return Path2.Combine(Ctx.Data.Graph.Name, "ComponentGroups"); }
        }

        public bool CanGenerate
        {
            get { return true; }
        }

        public void TemplateSetup()
        {
    
        }

        public TemplateContext<FilterNode> Ctx { get; set; }
    }



    [TemplateClass(TemplateLocation.DesignerFile), AsPartial]
    [RequiresNamespace("uFrame.ECS")]
    public partial class EventTemplate : IClassTemplate<EventNode>
    {
        public IEnumerable<PropertiesChildItem> Properties
        {
            get
            {
                foreach (var item in Ctx.Data.Properties)
                {
                    if (item.Name == "EntityId") continue;
                    yield return item;
                }
            }
        }
        public string OutputPath
        {
            get { return Path2.Combine(Ctx.Data.Graph.Name, "Events"); }
        }

        public bool CanGenerate
        {
            get { return true; }
        }
        
        public void TemplateSetup()
        {
            if (Ctx.Data.Dispatcher)
            {
                this.Ctx.CurrentDeclaration.Name += "Dispatcher";
                this.Ctx.SetBaseType(typeof (EcsDispatcher));
            }
            else
            {
                this.Ctx.CurrentDeclaration.Name = Ctx.Data.Name;
                if (!Ctx.IsDesignerFile)
                {
                    this.Ctx.CurrentDeclaration.BaseTypes.Clear();
                }
            }
        }

        public TemplateContext<EventNode> Ctx { get; set; }
    }


    public class _CONTEXTITEM_ : _ITEMTYPE_
    {
        public override string TheType(TemplateContext context)
        {
            return base.TheType(context) + "ContextItem";
        }
    }

    public class _CONTEXT_ : _ITEMTYPE_
    {
        public override string TheType(TemplateContext context)
        {
            return base.TheType(context) + "Context";
        }
    }
}