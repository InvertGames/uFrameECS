using System;
using System.CodeDom;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Invert.Core;
using Invert.Core.GraphDesigner;
using Invert.IOC;
using Invert.uFrame.ECS;
using uFrame.Actions;
using uFrame.Attributes;
using uFrame.ECS;
using uFrame.Kernel;
using UnityEditor;

namespace Invert.uFrame.ECS.Templates
{
    [InitializeOnLoad]
    public class EcsTemplates : DiagramPlugin
    {
        static EcsTemplates()
        {
            InvertApplication.CachedAssemblies.Add(typeof(EcsTemplates).Assembly);
            InvertApplication.CachedAssemblies.Add(typeof(UFAction).Assembly);
            InvertApplication.TypeAssemblies.Add(typeof(UFAction).Assembly);
        }
        public override void Initialize(UFrameContainer container)
        {
            RegisteredTemplateGeneratorsFactory.RegisterTemplate<SystemNode,SystemTemplate>();
            RegisteredTemplateGeneratorsFactory.RegisterTemplate<ComponentNode,ComponentTemplate>();
//            RegisteredTemplateGeneratorsFactory.RegisterTemplate<ComponentGroupNode,ComponentGroupTemplate>();
  //          RegisteredTemplateGeneratorsFactory.RegisterTemplate<ComponentGroupNode,ComponentGroupManagerTemplate>();
            RegisteredTemplateGeneratorsFactory.RegisterTemplate<EventNode,EventTemplate>();
            RegisteredTemplateGeneratorsFactory.RegisterTemplate<ContextNode,ContextTemplate>();
            RegisteredTemplateGeneratorsFactory.RegisterTemplate<ContextNode, ContextItemTemplate>();
            RegisteredTemplateGeneratorsFactory.RegisterTemplate<HandlerNode, HandlerTemplate>();
//            RegisteredTemplateGeneratorsFactory.RegisterTemplate<EntityNode, EntityTemplate>();

            RegisteredTemplateGeneratorsFactory.RegisterTemplate<CustomActionNode, CustomActionEditableTemplate>();
            RegisteredTemplateGeneratorsFactory.RegisterTemplate<CustomActionNode, CustomActionDesignerTemplate>();
            RegisteredTemplateGeneratorsFactory.RegisterTemplate<ModuleNode, ModuleTemplate>();
        }
    }

    [TemplateClass(TemplateLocation.DesignerFile), AsPartial]
    [RequiresNamespace("uFrame.Kernel")]
    [RequiresNamespace("uFrame.ECS")]
    [ForceBaseType(typeof(SystemLoader))]
    public partial class ModuleTemplate : IClassTemplate<ModuleNode>, ITemplateCustomFilename
    {
        public string OutputPath
        {
            get { return Path2.Combine("Modules", Ctx.Data.Name, Ctx.Data.Name); }
        }

        public string Filename
        {
            get
            {
                return Path2.Combine("Modules", Ctx.Data.Name, Ctx.Data.Name + ".cs"); 
            }
        }

        public bool CanGenerate
        {
            get { return true; }
        }

        public void TemplateSetup()
        {

        }

        public TemplateContext<ModuleNode> Ctx { get; set; }

        [GenerateMethod,AsOverride]
        public void Load()
        {
            foreach (var system in Ctx.Data.Graph.NodeItems.OfType<SystemNode>())
            {
                Ctx._("this.AddSystem<{0}>()",system.Name);
            }
        }

   
    }

    [TemplateClass(TemplateLocation.DesignerFile)]
    [RequiresNamespace("uFrame.Kernel")]
    [RequiresNamespace("UnityEngine")]
    public partial class HandlerTemplate : IClassTemplate<HandlerNode>
    {
        public string OutputPath
        {
            get { return Path2.Combine("Modules", Ctx.Data.Graph.Name, "Handlers"); }
        }

        public bool CanGenerate
        {
            get { return Ctx.Data.Meta != null; }
        }

        public void TemplateSetup()
        {

        }

        public TemplateContext<HandlerNode> Ctx { get; set; }
    }
    //[TemplateClass(TemplateLocation.Both,"{0}PrefabPool")]
    //[RequiresNamespace("uFrame.Kernel")]
    //[RequiresNamespace("UnityEngine")]
    //[RequiresNamespace("uFrame.ECS")]
    //[ForceBaseType(typeof(EntityPrefabPool)), AsPartial]
    //public partial class EntityTemplate : IClassTemplate<EntityNode>
    //{

    //    public string OutputPath
    //    {
    //        get { return Path2.Combine(Ctx.Data.Graph.Name, "Entities"); }
    //    }

    //    public bool CanGenerate
    //    {
    //        get { return true; }
    //    }

    //    public void TemplateSetup()
    //    {
    //        Ctx.CurrentDeclaration.Name = Ctx.Data.Name + "PrefabPool";
    //        if (!Ctx.IsDesignerFile)
    //        {
    //            Ctx.CurrentDeclaration.BaseTypes.Clear();
    //        }
    //        else
    //        {
    //            foreach (var item in Ctx.Data.Components)
    //            {
    //                Ctx.CurrentDeclaration.CustomAttributes.Add(
    //                    new CodeAttributeDeclaration(typeof(RequireComponent).ToCodeReference(),
    //                        new CodeAttributeArgument(new CodeSnippetExpression(string.Format("typeof({0})", item.Name)))));
    //            }
    //        }

            
    //    }

    //    public TemplateContext<EntityNode> Ctx { get; set; }
    //}


    [TemplateClass(TemplateLocation.DesignerFile), AsPartial]
    [RequiresNamespace("uFrame.Kernel")]
    public partial class SystemTemplate : IClassTemplate<SystemNode>, ITemplateCustomFilename
    {
        public string Filename
        {
            get { return Path2.Combine("Modules", Ctx.Data.Graph.Name, Ctx.Data.Name + ".cs"); }
        }
        public string OutputPath
        {
            get { return Path2.Combine(Ctx.Data.Graph.Name, "Systems"); }
        }

        public IEnumerable<ComponentNode> Components
        {
            get
            {
                return Ctx.Data.Components.Select(p => p.SourceItem).OfType<ComponentNode>()
                    .Concat(Ctx.Data.Graph.NodeItems.OfType<ComponentNode>()).Distinct();
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
        public IEnumerable<HandlerNode> EventHandlers
        {
            get
            {
                return Ctx.Data.EventHandlers;
            }
        }

        public IEnumerable<ContextNode> FilterNodes
        {
            get
            {
                foreach (var item in Ctx.Data.Repository.All<ContextNode>())
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
    
        public bool CanGenerate
        {
            get { return true; }
        }

        public void TemplateSetup()
        {

        }

        public TemplateContext<SystemNode> Ctx { get; set; }
    }

    [TemplateClass(TemplateLocation.DesignerFile)]
    [ForceBaseType(typeof(EcsComponent)),AsPartial]
    [RequiresNamespace("uFrame.ECS")]
    [RequiresNamespace("UnityEngine")]
    public partial class ComponentTemplate : IClassTemplate<ComponentNode>, ITemplateCustomFilename
    {
        public string Filename
        {
            get
            {
                return Path2.Combine("Library", Ctx.Data.Graph.Name, "Components", Ctx.Data.Name + ".cs");
            }
        }
        // Not used now
        public string OutputPath
        {
            get { return Path2.Combine("Extensions", Ctx.Data.Graph.Name, "Components"); }
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

    [TemplateClass(TemplateLocation.DesignerFile, "{0}Context"), AsPartial]
    [RequiresNamespace("uFrame.ECS")]
    [RequiresNamespace("uFrame.Kernel")]
    [RequiresNamespace("UniRx")]
    public partial class ContextTemplate : IClassTemplate<ContextNode>
    {

        public IEnumerable<ComponentNode> SelectComponents
        {
            get
            {
                return Ctx.Data.Select.Select(p => p.SourceItem).OfType<ComponentNode>();
            }
        }
        public string OutputPath
        {
            get { return Path2.Combine("Modules", Ctx.Data.Graph.Name, "Groups"); }
        }

        public bool CanGenerate
        {
            get { return true; }
        }

        public void TemplateSetup()
        {

            this.Ctx.SetBaseType("ReactiveContext<{0}>",Ctx.Data.Name);
        }

        public TemplateContext<ContextNode> Ctx { get; set; }
    }

    [TemplateClass(TemplateLocation.DesignerFile, "{0}"), AsPartial]
    [RequiresNamespace("uFrame.ECS")]
    [RequiresNamespace("uFrame.Kernel")]
    [RequiresNamespace("UniRx")]
    public partial class ContextItemTemplate : ContextItem, IClassTemplate<ContextNode>
    {

        public IEnumerable<ComponentNode> SelectComponents
        {
            get
            {
                return Ctx.Data.Select.Select(p => p.SourceItem).OfType<ComponentNode>();
            }
        }
        public string OutputPath
        {
            get { return Path2.Combine("Modules", Ctx.Data.Graph.Name, "Groups"); }
        }

        public bool CanGenerate
        {
            get { return true; }
        }

        public void TemplateSetup()
        {
    
        }

        public TemplateContext<ContextNode> Ctx { get; set; }
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
        public string Filename
        {
            get
            {
                return Path2.Combine("Library",Ctx.Data.Graph.Name, "Events.cs");
            }
        }
        public string OutputPath
        {
            get { return Path2.Combine("Library", Ctx.Data.Graph.Name, "Events"); }
        }

        public bool CanGenerate
        {
            get { return true; }
        }
        
        public void TemplateSetup()
        {
            this.Ctx.CurrentDeclaration.CustomAttributes.Add(
                new CodeAttributeDeclaration(
                    typeof (uFrameEvent).ToCodeReference(),new CodeAttributeArgument(new CodePrimitiveExpression(Ctx.Data.Name)) 
                    ));
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

    [TemplateClass(TemplateLocation.DesignerFile), ForceBaseType(typeof(UFAction)), AsPartial]
    [RequiresNamespace("uFrame.ECS")]
    [RequiresNamespace("UnityEngine")]
    public partial class CustomActionDesignerTemplate : IClassTemplate<CustomActionNode>
    {
        
        public string OutputPath
        {
            get { return Path2.Combine("Library", Ctx.Data.Graph.Name, "CustomActions"); }
        }

        public bool CanGenerate
        {
            get { return true; }
        }

        public void TemplateSetup()
        {
            Ctx.CurrentDeclaration.CustomAttributes.Add(new CodeAttributeDeclaration(typeof(ActionTitle).ToCodeReference(),
                        new CodeAttributeArgument(new CodePrimitiveExpression(string.IsNullOrEmpty(Ctx.Data.ActionTitle) ? Ctx.Data.Name : Ctx.Data.ActionTitle))));
            foreach (var item in Ctx.Data.Inputs)
            {
                var field = Ctx.CurrentDeclaration._public_(item.RelatedTypeName, item.Name);
                field.CustomAttributes.Add(new CodeAttributeDeclaration(typeof(In).ToCodeReference(),
                    new CodeAttributeArgument(new CodePrimitiveExpression(item.Name))));
            }
            foreach (var item in Ctx.Data.Outputs)
            {
                var field = Ctx.CurrentDeclaration._public_(item.RelatedTypeName, item.Name);
                field.CustomAttributes.Add(new CodeAttributeDeclaration(typeof(Out).ToCodeReference(),
                    new CodeAttributeArgument(new CodePrimitiveExpression(item.Name))));
            }
            foreach (var item in Ctx.Data.Branches)
            {
                var field = Ctx.CurrentDeclaration._public_(typeof(Action), item.Name);
                field.CustomAttributes.Add(new CodeAttributeDeclaration(typeof(Out).ToCodeReference(),
                    new CodeAttributeArgument(new CodePrimitiveExpression(item.Name))));
            }
        }

        public TemplateContext<CustomActionNode> Ctx { get; set; }
    }

    [TemplateClass(TemplateLocation.EditableFile), ForceBaseType(typeof(UFAction)), AsPartial]
    [RequiresNamespace("uFrame.ECS")]
    public partial class CustomActionEditableTemplate : IClassTemplate<CustomActionNode>
    {

        public string OutputPath
        {
            get { return Path2.Combine("Library", Ctx.Data.Graph.Name, "CustomActions"); }
        }

        public bool CanGenerate
        {
            get { return true; }
        }

        public void TemplateSetup()
        {
            this.Ctx.CurrentDeclaration.BaseTypes.Clear();
            var method = Ctx.CurrentDeclaration.public_override_func(typeof (bool), "Execute");
            method.Statements.Add(new CodeSnippetExpression("return base.Execute()"));
        }

        public TemplateContext<CustomActionNode> Ctx { get; set; }
    }

    public class _CONTEXTITEM_ : _ITEMTYPE_
    {
        public override string TheType(TemplateContext context)
        {
            return base.TheType(context);
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