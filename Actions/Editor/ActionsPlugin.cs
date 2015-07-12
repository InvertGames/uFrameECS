using System;
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
using uFrame.Actions.Attributes;

public class ActionsPlugin : DiagramPlugin, IContextMenuQuery {
    private static Dictionary<string, ActionMetaInfo> _actions;

    public override void Initialize(UFrameContainer container)
    {
        if (Actions.Count > 0) return;
        ListenFor<IContextMenuQuery>();

        // Query for the available actions
        ActionTypes = InvertApplication.GetDerivedTypes<UFAction>(false, false).ToArray();
        foreach (var actionType in ActionTypes)
        {
            var actionInfo = new ActionMetaInfo()
            {
                Type = actionType
            };
            actionInfo.MetaAttributes =  actionType.GetCustomAttributes(typeof (ActionMetaAttribute), true).OfType<ActionMetaAttribute>().ToArray();
            var fields = actionType.GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (var field in fields)
            {
                var fieldMetaInfo = new ActionFieldInfo()
                {
                    Field = field
                };
                fieldMetaInfo.MetaAttributes =
                    field.GetCustomAttributes(typeof (ActionAttribute), true).OfType<ActionAttribute>().ToArray();
                if (fieldMetaInfo.DisplayType == null) 
                    continue;
                
                actionInfo.ActionFields.Add(fieldMetaInfo);
            }
            Actions.Add(actionType.FullName, actionInfo);

        }

    }

    public Type[] ActionTypes { get; set; }

    public static Dictionary<string, ActionMetaInfo> Actions
    {
        get { return _actions ?? (_actions = new Dictionary<string, ActionMetaInfo>()); }
        set { _actions = value; }
    }

    public void QueryCommands(ICommandUI ui, List<IEditorCommand> commands, Type contextType)
    {
        if (contextType == typeof (IDiagramContextCommand))
        {
   

            var diagramViewModel = ui.Handler.ContextObjects.OfType<DiagramViewModel>().FirstOrDefault();
            if (diagramViewModel != null)
            {
                var graph = diagramViewModel.GraphData;
                var currentFilter = graph.CurrentFilter as HandlerNode;
                if (currentFilter != null)
                {
                    foreach (var action in Actions.Values)
                    {
                        commands.Add(new AddActionCommand(action));
                    }

                    foreach (var item in currentFilter.AllContextVariables)
                    {
                        commands.Add(new AddVariableReferenceCommand()
                        {
                            Variable = item,
                            Handler = currentFilter
                        });
                    }
                }
            }
        }
        

    }
}

public class ActionMetaInfo
{
    private ActionDescription _description;
    private ActionTitle _title;
    private List<ActionFieldInfo> _actionFields;
    public Type Type { get; set; }

    public ActionTitle Title
    {
        get { return _title ?? (_title = MetaAttributes.OfType<ActionTitle>().FirstOrDefault()); }
        set { _title = value; }
    }

    public string TitleText
    {
        get
        {
            if (Title == null)
                return Type.Name;

            return Title.Title;
        }
    }

    public string DescriptionText
    {
        get
        {
            if (Description == null)
            {
                return "No Description Specified";
            }
            return Description.Description;
        }
    }

    public ActionDescription Description
    {
        get { return _description ?? (_description = MetaAttributes.OfType<ActionDescription>().FirstOrDefault()); }
        set { _description = value; }
    }

    public List<ActionFieldInfo> ActionFields
    {
        get { return _actionFields ?? (_actionFields = new List<ActionFieldInfo>()); }
        set { _actionFields = value; }
    }

    public ActionMetaAttribute[] MetaAttributes { get; set; }
}

public class ActionFieldInfo
{
    public string Name
    {
        get
        {
            
            return DisplayType.Name ?? Field.Name;
        }
    }
    private FieldDisplayTypeAttribute _displayType;
    public FieldInfo Field { get; set; }
    public ActionAttribute[] MetaAttributes { get; set; }

    public FieldDisplayTypeAttribute DisplayType
    {
        get { return _displayType ?? (_displayType = MetaAttributes.OfType<FieldDisplayTypeAttribute>().FirstOrDefault()); }
        set { _displayType = value; }
    }
}
public class AddActionCommand : EditorCommand<DiagramViewModel>
{
    public ActionMetaInfo ActionMetaInfo { get; set; }
    public override string Group
    {
        get { return "Actions"; }
    }

    public AddActionCommand(ActionMetaInfo actionMetaInfo)
    {
        ActionMetaInfo = actionMetaInfo;
    }

    public override string Name
    {
        get { return base.Name; }
    }

    public override string Title
    {
        get { return ActionMetaInfo.TitleText; }
        set { base.Title = value; }
    }

    public override string Path
    {
        get { return ActionMetaInfo.TitleText; }
    }

    public override void Perform(DiagramViewModel node)
    {
        var eventNode = new ActionNode()
        {
            Meta = ActionMetaInfo
        };

        node.AddNode(eventNode, node.LastMouseEvent.LastMousePosition);
    }

    public override string CanPerform(DiagramViewModel node)
    {
        return null;
    }
}

public class AddVariableReferenceCommand : EditorCommand<DiagramViewModel>
{
    public IContextVariable Variable { get; set; }
    public HandlerNode Handler { get; set; }

    public override string Name
    {
        get { return Variable.ShortName; }
    }

    public override string Group
    {
        get { return "Variables"; }
    }

    public override void Perform(DiagramViewModel node)
    {
        var referenceNode = new VariableReferenceNode()
        {
            Name = Variable.VariableName,
            VariableId = Variable.Identifier,
            HandlerId = Handler.Identifier
        };

        node.AddNode(referenceNode, node.LastMouseEvent.LastMousePosition);
    }

    public override string CanPerform(DiagramViewModel node)
    {
        return null;
    }
}
