using System;
using System.CodeDom;
using System.Linq;
using Invert.Core.GraphDesigner;
using Invert.IOC;
using Invert.uFrame.ECS;
using uFrame.Actions;
using uFrame.Attributes;
using uFrame.ECS;

public class EcsDyanmicActionTemplates : DiagramPlugin
{
    public override bool Required
    {
        get { return true; }
    }
    public override void Initialize(UFrameContainer container)
    {
        RegisteredTemplateGeneratorsFactory.RegisterTemplate<EnumNode, EnumSwitchActionTemplate>();
        RegisteredTemplateGeneratorsFactory.RegisterTemplate<ComponentNode, AddComponentTemplate>();
        RegisteredTemplateGeneratorsFactory.RegisterTemplate<EventNode, PublishActionTemplate>();
        RegisteredTemplateGeneratorsFactory.RegisterTemplate<EntityNode, SpawnEntityTemplate>();
        RegisteredTemplateGeneratorsFactory.RegisterTemplate<IMappingsConnectable, LoopComponentsTemplate>();
    }
}

[TemplateClass(TemplateLocation.DesignerFile)]
public partial class EnumSwitchActionTemplate : UFAction, IClassTemplate<EnumNode>
{
    public string OutputPath { get { return Path2.Combine(Ctx.Data.Graph.Name, "Actions"); } }
    public bool CanGenerate { get { return true; } }

    [GenerateMethod]
    public override void Execute()
    {
        Ctx._("return true");
    }

    public void TemplateSetup()
    {
        Ctx.CurrentDeclaration.CustomAttributes.Add(
            new CodeAttributeDeclaration(typeof(ActionTitle).ToCodeReference(),
                new CodeAttributeArgument(
                    new CodePrimitiveExpression(string.Format("Switch/{0}/{1}", Ctx.Data.Graph.Name, Ctx.Data.Name)))));
        Ctx.CurrentDeclaration.Name = Ctx.Data.Name + "Switch";
        foreach (var item in Ctx.Data.Items)
        {
            var field = Ctx.CurrentDeclaration._public_(typeof(Action), item.Name);
            field.CustomAttributes.Add(new CodeAttributeDeclaration(typeof(Out).ToCodeReference()));
        }
    }

    public TemplateContext<EnumNode> Ctx { get; set; }
}


[TemplateClass(TemplateLocation.DesignerFile)]

[RequiresNamespace("UnityEngine")]
[RequiresNamespace("uFrame.Actions")]
public class ActionTemplate<TNodeType> :  IClassTemplate<TNodeType> where TNodeType : IDiagramNodeItem {

    public string OutputPath
    {
        get { return Path2.Combine("Library", Ctx.Data.Graph.Name, Ctx.Data.Name + "Actions"); }
    }

    public virtual bool CanGenerate
    {
        get { return true; }
    }

    public virtual void TemplateSetup()
    {
        Ctx.SetBaseType(typeof(UFAction));
        Ctx.CurrentDeclaration.CustomAttributes.Add(
          new CodeAttributeDeclaration(typeof(ActionTitle).ToCodeReference(),
              new CodeAttributeArgument(
                  new CodePrimitiveExpression(ActionTitle))));
        Ctx.CurrentDeclaration.Name = ClassName;
    }

    protected virtual string ClassName
    {
        get { return "Add" + Ctx.Data.Name + "Action"; }
    }

    protected virtual string ActionTitle
    {
        get { return string.Format("{0}/{1}", Ctx.Data.Graph.Name, Ctx.Data.Name); }
    }

    public TemplateContext<TNodeType> Ctx { get; set; }
    public CodeMemberField AddIn(object type, string name)
    {
        var result = Ctx.CurrentDeclaration._public_(type, name);
        result.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(In))));
        return result;
    }
    public CodeMemberField AddOut(object type, string name)
    {
        var result = Ctx.CurrentDeclaration._public_(type, name);
        result.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(Out))));
        return result;
    }

    public CodeMemberField AddBranch(string name)
    {
        var result = Ctx.CurrentDeclaration._public_(typeof(Action), name);
        result.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(Out))));
        return result;
    }

}


public class LoopComponentsTemplate : ActionTemplate<IMappingsConnectable>
{
    protected override string ClassName
    {
        get { return string.Format("{0}LoopAction", Ctx.Data.Name); }
    }

    protected override string ActionTitle
    {
        get { return string.Format("{0} Loop", Ctx.Data.Name); }
    }
    public override void TemplateSetup()
    {
        base.TemplateSetup();
        this.Ctx.SetBaseType("LoopEntities<{0}>",Ctx.Data.Name);
    }
}
public class AddComponentTemplate : ActionTemplate<ComponentNode>
{
    protected override string ClassName
    {
        get { return string.Format("Add{0}Action", Ctx.Data.Name); }
    }

    protected override string ActionTitle
    {
        get { return string.Format("Add {0} Component",  Ctx.Data.Name); }
    }

    [GenerateMethod(CallBase = true), AsOverride]
    public void Execute()
    {
        AddIn(typeof(EcsComponent), "Beside");

        foreach (var item in Ctx.Data.Properties)
        {
            AddIn(item.RelatedTypeName, item.Name);
        }
        
        Ctx._("Beside.gameObject.AddComponent<{0}>()",Ctx.Data.Name);
    }
}
//public class RemoveComponentTemplate : ActionTemplate<ComponentNode>
//{
//    protected override string ClassName
//    {
//        get { return string.Format("Remove{0}Action", Ctx.Data.Name); }
//    }

//    protected override string ActionTitle
//    {
//        get { return string.Format("Remove {0}", Ctx.Data.Name); }
//    }

//    [GenerateMethod(CallBase = true), AsOverride]
//    public bool Execute()
//    {
        
//        Ctx._("return base.Execute()");
//        return false;
//    }
//}


public class PublishActionTemplate : ActionTemplate<EventNode>
{
    protected override string ClassName
    {
        get { return string.Format("Publish{0}Action", Ctx.Data.Name); }
    }

    protected override string ActionTitle
    {
        get { return string.Format("Publish {0}", Ctx.Data.Name); }
    }

    public override bool CanGenerate
    {
        get { return !Ctx.Data.Dispatcher && !Ctx.Data.SystemEvent; }
    }

    [GenerateMethod(CallBase = true), AsOverride]
    public void Execute()
    {

        Ctx._("var evt = new {0}()",Ctx.Data.Name);
        
        foreach (var item in Ctx.Data.PersistedItems.OfType<ITypedItem>())
        {
            AddIn(item.RelatedTypeName, item.Name);
            Ctx._("evt.{0} = {0}", item.Name);
        }

        Ctx._("System.Publish(evt)");

        
        


    }
}

public class SpawnEntityTemplate : ActionTemplate<EntityNode>
{

    protected override string ClassName
    {
        get { return "Spawn" + Ctx.Data.Name + "Entity"; }
    }

    protected override string ActionTitle
    {
        get { return string.Format("Entities/Spawn {0}", Ctx.Data.Name); }
    }
    [GenerateMethod(CallBase = true), AsOverride]
    public void Execute()
    {
        AddIn(typeof(string), "Pool");

        foreach (var item in Ctx.Data.Components.Select(p=>p.SourceItem).OfType<ComponentNode>().SelectMany(x=>x.PersistedItems.OfType<ITypedItem>()))
        {
            AddIn(item.RelatedTypeName, item.Name);
        }
        foreach (var item in Ctx.Data.Components.Select(p => p.SourceItem).OfType<ComponentNode>())
        {
            AddOut(item.Name, item.Name);
        }
        AddOut(typeof (Entity), "Entity");

    }
}

