using System.Reflection;
using Invert.IOC;
using uFrame.Actions;
using uFrame.Actions.Attributes;
using UnityEditor.Graphs;
using UnityEngine;

namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;


    public class uFrameECS : uFrameECSBase, IPrefabNodeProvider, IContextMenuQuery
    {
        private static Dictionary<string, ActionMetaInfo> _actions;
        private static Dictionary<string, EventMetaInfo> _events;

        public override void Initialize(UFrameContainer container)
        {
            base.Initialize(container);
            

            ListenFor<IPrefabNodeProvider>();
            ListenFor<IContextMenuQuery>();

            container.RegisterDrawer<ItemViewModel<IContextVariable>, ItemDrawer>();
            container.AddItemFlag<ComponentsReference>("Multiple", UnityEngine.Color.blue);
            container.AddNodeFlag<EventNode>("Dispatcher");
            System.HasSubNode<EnumNode>();
            container.Connectable<IContextVariable, IActionIn>();
            container.Connectable<IActionOut, IContextVariable>();
            container.Connectable<ActionBranch, SequenceItemNode>();
   
            VariableReference.Name = "Var";

            LoadActions();
            LoadEvents();
        }

        private void LoadActions()
        {
            LoadActionTypes();

            LoadActionLibrary();
        }

        private void LoadActionTypes()
        {
            // Query for the available actions
            ActionTypes = InvertApplication.GetDerivedTypes<UFAction>(false, false).ToArray();

            foreach (var actionType in ActionTypes)
            {
                var actionInfo = new ActionMetaInfo()
                {
                    Type = actionType
                };
                actionInfo.MetaAttributes =
                    actionType.GetCustomAttributes(typeof (ActionMetaAttribute), true).OfType<ActionMetaAttribute>().ToArray();
                var fields = actionType.GetFields(BindingFlags.Instance | BindingFlags.Public);
                foreach (var field in fields)
                {
                    var fieldMetaInfo = new ActionFieldInfo()
                    {
                        Type = field.FieldType,
                        Name = field.Name
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

        private static void LoadActionLibrary()
        {
            foreach (var assembly in InvertApplication.CachedAssemblies)
            {
                foreach (
                    var type in
                        assembly.GetTypes()
                            .Where(p => p.IsSealed && p.IsSealed && p.IsDefined(typeof (ActionLibrary), true)))
                {
                    var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
                    foreach (var method in methods)
                    {
                        var actionInfo = new ActionMetaInfo()
                        {
                            Type = type,
                            Method = method,
                        };
                        actionInfo.MetaAttributes =
                            method.GetCustomAttributes(typeof (ActionMetaAttribute), true)
                                .OfType<ActionMetaAttribute>()
                                .ToArray();


                        var vars = method.GetParameters();
                        foreach (var parameter in vars)
                        {
                            var fieldMetaInfo = new ActionFieldInfo()
                            {
                                Type = parameter.ParameterType,
                                Name = parameter.Name
                            };

                            fieldMetaInfo.MetaAttributes =
                                method.GetCustomAttributes(typeof (FieldDisplayTypeAttribute), true)
                                    .Cast<FieldDisplayTypeAttribute>()
                                    .Where(p => p.ParameterName == parameter.Name).ToArray();
                            if (!fieldMetaInfo.MetaAttributes.Any())
                            {
                                if (parameter.IsOut || parameter.ParameterType == typeof (Action))
                                {
                                    fieldMetaInfo.DisplayType = new Out(parameter.Name, parameter.Name);
                                }
                                else
                                {
                                    fieldMetaInfo.DisplayType = new In(parameter.Name, parameter.Name);
                                }
                            }
                            actionInfo.ActionFields.Add(fieldMetaInfo);
                        }
                        if (method.ReturnType != typeof (void))
                        {
                            var result = new ActionFieldInfo()
                            {
                                Type = actionInfo.Type,
                                Name = "Result"
                            };
                            result.MetaAttributes =
                                method.GetCustomAttributes(typeof (FieldDisplayTypeAttribute), true)
                                    .OfType<FieldDisplayTypeAttribute>()
                                    .Where(p => p.ParameterName == "Result").ToArray();

                            result.DisplayType = new Out("Result", "Result");
                            actionInfo.ActionFields.Add(result);
                        }
                        Actions.Add(actionInfo.FullName, actionInfo);
                    }
                }
            }
        }

        public IEnumerable<Type> EventTypes
        {
            get
            {
                foreach (var assembly in InvertApplication.TypeAssemblies)
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        if (type.IsDefined(typeof(EventAttribute), true))
                        {
                            yield return type;
                        }
                    }
                }
            }
        }

        private void LoadEvents()
        {
            
  
            foreach (var eventType in EventTypes)
            {
                var eventInfo = new EventMetaInfo()
                {
                    Type = eventType
                };

                eventInfo.Attribute =
                    eventType.GetCustomAttributes(typeof(EventAttribute), true).OfType<EventAttribute>().FirstOrDefault();

                var fields = eventType.GetFields(BindingFlags.Instance | BindingFlags.Public);
                var properties = eventType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

                foreach (var field in fields)
                {
                    var fieldMetaInfo = new EventFieldInfo()
                    {
                        Type = field.FieldType,
                        Name = field.Name
                    };
            

                    eventInfo.Members.Add(fieldMetaInfo);
                }
                foreach (var field in properties)
                {
                    var fieldMetaInfo = new EventFieldInfo()
                    {
                        Type = field.PropertyType,
                        Name = field.Name,
                        IsProperty = true
                    };


                    eventInfo.Members.Add(fieldMetaInfo);
                }
                Events.Add(eventType.FullName, eventInfo);
            }
        }

        public IEnumerable<QuickAddItem> PrefabNodes(INodeRepository nodeRepository)
        {
            foreach (var item in Events)
            {
                var item1 = item;
                var qa = new QuickAddItem(item.Value.Type.Namespace, item.Value.Attribute.Title, _ =>
                {
                    var eventNode = new HandlerNode()
                    {
                        Meta = item1.Value
                        
                    };
                    _.Diagram.AddNode(eventNode, _.MousePosition);
                })
                {
                    
                };
                yield return qa;
            }
            yield break;
        }

        public Type[] ActionTypes { get; set; }


        public static Dictionary<string, EventMetaInfo> Events
        {
            get { return _events ?? (_events = new Dictionary<string, EventMetaInfo>()); }
            set { _events = value; }
        }
        public static Dictionary<string, ActionMetaInfo> Actions
        {
            get { return _actions ?? (_actions = new Dictionary<string, ActionMetaInfo>()); }
            set { _actions = value; }
        }

        public void QueryCommands(ICommandUI ui, List<IEditorCommand> commands, Type contextType)
        {
            if (contextType == typeof(IDiagramContextCommand))
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

    public class EventMetaInfo
    {
        private List<EventFieldInfo> _members;

        public Type Type { get; set; }
        public EventAttribute Attribute { get; set; }

        public bool Dispatcher
        {
            get { return Attribute is EventDispatcher; }
        }

        public bool SystemEvent
        {
            get { return Attribute is SystemEvent; }
        }

        public List<EventFieldInfo> Members
        {
            get { return _members ?? (_members = new List<EventFieldInfo>()); }
            set { _members = value; }
        }

        public string SystemEventMethod
        {
            get { return (Attribute as SystemEvent).SystemMethodName; }
        }
    }

    public class EventFieldInfo
    {
        public Type Type { get; set; }
        public string Name { get; set; }
        public bool IsProperty { get; set; }
    }


    public class ActionMetaInfo
    {
        private ActionDescription _description;
        private ActionTitle _title;
        private List<ActionFieldInfo> _actionFields;
        public Type Type { get; set; }
        public MethodInfo Method { get; set; }
        public ActionTitle Title
        {
            get { return _title ?? (_title = MetaAttributes.OfType<ActionTitle>().FirstOrDefault()); }
            set { _title = value; }
        }

        public string FullName
        {
            get
            {
                if (Method == null)
                    return Type.FullName;
                return Type.FullName + "." + Method.Name;
            }
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
                if (DisplayType == null) return _name;
                return DisplayType.DisplayName ?? _name;
            }
            set { _name = value; }
        }
        private FieldDisplayTypeAttribute _displayType;
        private string _name;
        public Type Type { get; set; }
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
    public class AddHandlerCommand : EditorCommand<DiagramViewModel>
    {
        public ActionMetaInfo ActionMetaInfo { get; set; }
        public override string Group
        {
            get { return "Handlers"; }
        }

        public AddHandlerCommand(ActionMetaInfo actionMetaInfo)
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

}
