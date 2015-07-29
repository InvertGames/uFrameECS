using System.Reflection;
using Invert.Core.GraphDesigner.Unity;
using Invert.IOC;
using Invert.Windows;
using uFrame.Attributes;
using UnityEngine;

namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;


    public class uFrameECS : uFrameECSBase, IPrefabNodeProvider, IContextMenuQuery, IQuickAccessEvents, IOnMouseDoubleClickEvent
    {
        private static Dictionary<string, ActionMetaInfo> _actions;
        private static Dictionary<string, EventMetaInfo> _events;
         
        public override void Initialize(UFrameContainer container)
        {
            base.Initialize(container);

            Context.Name = "Entity Group";
            CustomAction.Name = "Custom Action";
            System.Name = "System";
            System.HasSubNode<TypeReferenceNode>();
            Module.HasSubNode<TypeReferenceNode>();
            System.HasSubNode<ComponentNode>();
            System.HasSubNode<ContextNode>();

            Component.AddFlag("Blackboard"); 

            Module.HasSubNode<ComponentNode>();
            container.RegisterDrawer<ItemViewModel<IContextVariable>, ItemDrawer>();
            container.AddItemFlag<ComponentsReference>("Multiple", UnityEngine.Color.blue);
            container.AddNodeFlag<EventNode>("Dispatcher");
            System.HasSubNode<EnumNode>();
            container.Connectable<IContextVariable, IActionIn>();
            container.Connectable<IActionOut, IContextVariable>();
           // container.Connectable<ActionOut, ActionIn>(UnityEngine.Color.blue);
            container.Connectable<ActionBranch, SequenceItemNode>();
            container.Connectable<IMappingsConnectable, HandlerIn>();
            
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
            Actions.Clear();

            //// Query for the available actions
            //ActionTypes = InvertApplication.GetDerivedTypes<UFAction>(false, false).ToArray();

            foreach (var actionType in ActionTypes)
            {
                var actionInfo = new ActionMetaInfo()
                {
                    Type = actionType,
                    
                };
                actionInfo.MetaAttributes =
                    actionType.GetCustomAttributes(typeof (ActionMetaAttribute), true).OfType<ActionMetaAttribute>().ToArray();
                var fields = actionType.GetFields(BindingFlags.Instance | BindingFlags.Public );
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
                    var category = type.GetCustomAttributes(typeof (uFrameCategory), true).OfType<uFrameCategory>().FirstOrDefault();
                    var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
                    foreach (var method in methods)
                    {
                        var actionInfo = new ActionMetaInfo()
                        {
                            Type = type,
                            Category = category,
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
                                Type = method.ReturnType,
                                
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
        public IEnumerable<Type> ActionTypes
        {
            get
            {
                foreach (var assembly in InvertApplication.TypeAssemblies)
                {
                    foreach (var type in assembly.GetTypes())
                    {

                        if (type.IsDefined(typeof(ActionTitle), true))
                        {
                            yield return type;
                        }
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
                      
                        if (type.IsDefined(typeof(uFrameEvent), true))
                        {
                            yield return type;
                        }
                    }
                }
            }
        }

        private IHandlerCodeWriter[] _codeWriters;
        public IHandlerCodeWriter[] CodeWriters
        {
            get
            {
                return _codeWriters ??
                       (_codeWriters = EventCodeWriterTypes.Select(p => Activator.CreateInstance(p)).Cast<IHandlerCodeWriter>().ToArray());
            }
        }
        public IEnumerable<Type> EventCodeWriterTypes
        {
            get
            {
                foreach (var assembly in InvertApplication.CachedAssemblies)
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        if (type.IsClass && !type.IsAbstract && typeof(IHandlerCodeWriter).IsAssignableFrom(type))
                        {
                            yield return type;
                        }
                    }
                }
            }
        }
        private void LoadEvents()
        {
            
            Events.Clear();
            foreach (var eventType in EventTypes)
            {
                var eventInfo = new EventMetaInfo()
                {
                    Type = eventType,
                    CodeWriter = CodeWriters.FirstOrDefault(p => p.For == eventType)
                };

                eventInfo.Attribute =
                    eventType.GetCustomAttributes(typeof(uFrameEvent), true).OfType<uFrameEvent>().FirstOrDefault();

                var fields = eventType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                var properties = eventType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

                if (!eventInfo.SystemEvent && eventInfo.Dispatcher)
                {
                    eventInfo.Members.Add(new EventFieldInfo()
                    {
                        Type = typeof(int),
                        Name = "EntityId",
                        IsProperty = true
                    });
                }
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
                    
                    //var graph = diagramViewModel.GraphData;
                    //var currentFilter = graph.CurrentFilter as HandlerNode;
                    //if (currentFilter != null)
                    //{
                    //    foreach (var action in Actions.Values)
                    //    {
                    //        commands.Add(new AddActionCommand(action));
                    //    }

                    //    foreach (var item in currentFilter.AllContextVariables)
                    //    {
                    //        commands.Add(new AddVariableReferenceCommand()
                    //        {
                    //            Variable = item,
                    //            Handler = currentFilter
                    //        });
                    //    }

                    //}
                    //var systemNode = graph.CurrentFilter as SystemNode;
                    //if (systemNode != null)
                    //{
                    //    foreach (var item in Events.Values)
                    //    {
                    //        commands.Add(new AddHandlerCommand(item));
                    //    }
                    //}

                }
            }


        }

        public void QuickAccessItemsEvents(QuickAccessContext context, List<IEnumerable<QuickAccessItem>> items)
        {
            InvertApplication.Log(context.ContextType.ToString());
            if (context.ContextType == typeof (IInsertQuickAccessContext))
            {
                items.Clear();
                items.Add(QueryInsert(context));
            }
            if (context.ContextType == typeof (IConnectionQuickAccessContext))
            {
                if (InvertApplication.Container.Resolve<ProjectService>().CurrentProject.CurrentFilter is HandlerNode)
                {
             
                    items.Clear();
                    items.Add(QueryConntectionActions(context));
                }
                
            }
        }

        private IEnumerable<QuickAccessItem> QueryInsert(QuickAccessContext context)
        {
            var mousePosition = UnityEngine.Event.current.mousePosition;
            var currentGraph = InvertApplication.Container.Resolve<ProjectService>().CurrentProject.CurrentGraph;
            if (currentGraph.CurrentFilter is SystemNode)
            {
                foreach (var item in Events)
                {
                    var item1 = item;
                    var qa = new QuickAccessItem("Listen For", item.Value.Attribute.Title, _ =>
                    {
                        var eventNode = new HandlerNode()
                        {
                            Meta = _ as EventMetaInfo
                        };
                        InvertGraphEditor.CurrentDiagramViewModel.AddNode(eventNode, LastMouseEvent.MousePosition);
                    })
                    {
                        Item = item1.Value
                    };
                    yield return qa;
                }
            }
            if (currentGraph.CurrentFilter is HandlerNode)
            {
                var vm = InvertGraphEditor.CurrentDiagramViewModel;


                yield return new QuickAccessItem("Set", "Set Variable", _ => { vm.AddNode(new SetVariableNode(), vm.LastMouseEvent.LastMousePosition); });

                yield return new QuickAccessItem("Create", "Bool Variable", _ => { vm.AddNode(new BoolNode(), vm.LastMouseEvent.LastMousePosition); });
                yield return new QuickAccessItem("Create", "Vector2 Variable", _ => { vm.AddNode(new Vector2Node(), vm.LastMouseEvent.LastMousePosition); });
                yield return new QuickAccessItem("Create", "Vector3 Variable", _ => { vm.AddNode(new Vector3Node(), vm.LastMouseEvent.LastMousePosition); });
                yield return new QuickAccessItem("Create", "String Variable", _ => { vm.AddNode(new StringNode(), vm.LastMouseEvent.LastMousePosition); });
                yield return new QuickAccessItem("Create", "Float Variable", _ => { vm.AddNode(new FloatNode(), vm.LastMouseEvent.LastMousePosition); });
                yield return new QuickAccessItem("Create", "Integer Variable", _ => { vm.AddNode(new IntNode(), vm.LastMouseEvent.LastMousePosition); });
                yield return new QuickAccessItem("Create", "Literal", _ => { vm.AddNode(new LiteralNode(), vm.LastMouseEvent.LastMousePosition); });


                var currentFilter = currentGraph.CurrentFilter as HandlerNode;
                foreach (var item in currentFilter.GetAllContextVariables())
                {
                    var item1 = item;
                    var qa = new QuickAccessItem("Variables", item.VariableName ?? "Unknown", _ =>
                    {
                        var command = new AddVariableReferenceCommand()
                        {
                            Variable = _ as IContextVariable,
                            Handler = currentFilter,
                            Position = mousePosition
                        };
                        InvertGraphEditor.ExecuteCommand(command);
                    })
                    {
                        Item = item1
                    };
                    yield return qa;
                }
            }
            foreach (var item in QueryActions(context))
            {
                yield return item;
            }

          
        }
        private IEnumerable<QuickAccessItem> QueryActions(QuickAccessContext context)
        {
            var mousePosition = UnityEngine.Event.current.mousePosition;
            var diagramViewModel = InvertGraphEditor.CurrentDiagramViewModel;

            foreach (var item in Actions)
            {

                var qaItem = new QuickAccessItem(item.Value.CategoryPath.FirstOrDefault() ?? string.Empty, item.Value.TitleText, item.Value.TitleText, _ =>
                {
                    var actionInfo = _ as ActionMetaInfo;
                    var node = new ActionNode()
                    {
                        Meta = actionInfo
                    };
                    node.Graph = diagramViewModel.GraphData;


                    diagramViewModel.AddNode(node,mousePosition);
                 
                    node.IsSelected = true;
                    node.Name = "";
                })
                {
                    Item = item.Value
                };
                yield return qaItem;
            }
        }
        private IEnumerable<QuickAccessItem> QueryConntectionActions(QuickAccessContext context)
        {
            var connectionHandler = context.Data as ConnectionHandler;
            var diagramViewModel = connectionHandler.DiagramViewModel;

            foreach (var item in Actions)
            {
                
                var qaItem = new QuickAccessItem( item.Value.CategoryPath.FirstOrDefault() ?? string.Empty , item.Value.TitleText,item.Value.TitleText, _ =>
                {
                    var actionInfo = _ as ActionMetaInfo;
                    var node = new ActionNode()
                    {
                        Meta = actionInfo
                    };
                    node.Graph = diagramViewModel.GraphData;


                    diagramViewModel.AddNode(node, context.MouseData.MouseUpPosition);
                    diagramViewModel.GraphData.AddConnection(connectionHandler.StartConnector.ConnectorFor.DataObject as IConnectable, node);
                    node.IsSelected = true;
                    node.Name = "";
                })
                {
                    Item= item.Value
                };
                yield return qaItem;
            }
        }

        public void OnMouseDoubleClick(Drawer drawer, MouseEvent mouseEvent)
        {
            var d = drawer as DiagramDrawer;
            if (d != null)
            {
                // When we've clicked nothing
                if (d.DrawersAtMouse.Length < 1)
                {
                    LastMouseEvent = mouseEvent;
                    InvertApplication.SignalEvent<IWindowsEvents>(_ =>
                    {
                        _.ShowWindow("QuickAccessWindowFactory", "Add Node", null, mouseEvent.LastMousePosition,
                            new Vector2(225f, 300f));
                    });
                }
                else
                {
                    
                }
                
            }
            
        }

        public MouseEvent LastMouseEvent { get; set; }
    }

    public class EventMetaInfo
    {
        private List<EventFieldInfo> _members;

        public Type Type { get; set; }
        public uFrameEvent Attribute { get; set; }

        public bool Dispatcher
        {
            get { return Attribute is UFrameEventDispatcher; }
        }

        public bool SystemEvent
        {
            get { return Attribute is SystemUFrameEvent; }
        }

        public List<EventFieldInfo> Members
        {
            get { return _members ?? (_members = new List<EventFieldInfo>()); }
            set { _members = value; }
        }

        public string SystemEventMethod
        {
            get { return (Attribute as SystemUFrameEvent).SystemMethodName; }
        }

        public IHandlerCodeWriter CodeWriter { get; set; }
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
        private uFrameCategory _category;
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
        public uFrameCategory Category
        {
            get { return _category ?? (_category = Type.GetCustomAttributes(typeof(uFrameCategory),true).OfType<uFrameCategory>().FirstOrDefault()); }
            set { _category = value; }
        }

        public IEnumerable<string> CategoryPath
        {
            get
            {
                if (Category == null)
                {
                    yield break;
                }
                foreach (var item in Category.Title)
                {
                    yield return item;
                }
            }
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
        public EventMetaInfo ActionMetaInfo { get; set; }
        public override string Group
        {
            get { return "Handlers"; }
        }

        public AddHandlerCommand(EventMetaInfo actionMetaInfo)
        {
            ActionMetaInfo = actionMetaInfo;
        }

        public override string Name
        {
            get { return base.Name; }
        }

        public override string Title
        {
            get { return ActionMetaInfo.Attribute.Title; }
            set { base.Title = value; }
        }

        public override string Path
        {
            get { return "Listen For/" + ActionMetaInfo.Attribute.Title; }
        }

        public override void Perform(DiagramViewModel node)
        {
            var eventNode = new HandlerNode()
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
        public Vector2 Position { get; set; }
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

            node.AddNode(referenceNode ,Position);
        }

        public override string CanPerform(DiagramViewModel node)
        {
            return null;
        }
    }

}
