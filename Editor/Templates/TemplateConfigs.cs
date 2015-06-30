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
            
            RegisteredTemplateGeneratorsFactory.RegisterTemplate<OnEventNode,OnEventTemplate>();
            RegisteredTemplateGeneratorsFactory.RegisterTemplate<PublishNode,PublishTemplate>();
        }
    }
    [TemplateClass(TemplateLocation.Both)]
   
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


   
    public partial class EventTemplate
    {

    }

    [TemplateClass(TemplateLocation.Both)]
    [RequiresNamespace("uFrame.ECS")]
    public partial class OnEventTemplate : EventTemplate, IClassTemplate<OnEventNode>
    {

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

        }

        public TemplateContext<OnEventNode> Ctx { get; set; }
    }

    [TemplateClass(TemplateLocation.Both)]
    [RequiresNamespace("uFrame.ECS")]
    public partial class PublishTemplate : EventTemplate, IClassTemplate<PublishNode>
    {

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

        }

        public TemplateContext<PublishNode> Ctx { get; set; }
    }


}