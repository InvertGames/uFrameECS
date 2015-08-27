using Invert.Core.GraphDesigner;
using uFrame.Attributes;

namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    
    
    public class ActionNodeViewModel : ActionNodeViewModelBase {
        public override NodeColor Color
        {
            get { return NodeColor.Black; }
        }
        public override INodeStyleSchema StyleSchema
        {
            get
            {
               return MinimalisticStyleSchema;
            }
        }
        public ActionNode Action
        {
            get { return GraphItem as ActionNode; }
        }

        protected override void DataObjectChanged()
        {
            base.DataObjectChanged();
            IsBreakpoint = Action.BreakPoint != null;
        }
        public bool IsBreakpoint { get; set; }

        
        public override IEnumerable<string> Tags
        {
            get
            {
                yield break;
                if (Action.Meta == null)
                {
                    yield return "Action Not Found";
                    yield break;
                }
                if (Action.Meta.Method != null)
                {
                    yield break;
                }
                yield return Action.Meta.TitleText;
            }
        }

        public override bool IsCollapsed
        {
            get { return false; }
            set { base.IsCollapsed = value; }
        }

        public override bool AllowCollapsing
        {
            get { return false; }
        }

        public override bool IsEditable
        {
            get
            {
                
                return base.IsEditable;
            }
        }

        public override string Name
        {
            get
            {
                if (Action.Meta != null)
                    return Action.Meta.TitleText;
                if (Action.Meta != null && Action.Meta.Method != null)
                    return Action.Meta.Method.Name;
                return base.Name;
            }
            set { base.Name = value; }
        }

        public ActionNodeViewModel(ActionNode graphItemObject, Invert.Core.GraphDesigner.DiagramViewModel diagramViewModel) : 
                base(graphItemObject, diagramViewModel) {
        }

        public virtual bool ShowContextVariables
        {
            get { return true; }
        }
        protected override void CreateContent()
        {

      
            var meta = Action.Meta;
            if (meta != null)
            {
                foreach (var item in Action.InputVars)
                {
                    var vm = new InputOutputViewModel()
                    {
                        Name = item.Name,
                        IsOutput = false,
                        IsInput = true,
                        DataObject = item,
                        IsNewLine = item.ActionFieldInfo.DisplayType.IsNewLine,
                        DiagramViewModel = DiagramViewModel
                    };
                    ContentItems.Add(vm);
                    if (vm.InputConnector != null)
                    {
                        vm.InputConnector.Style = ConnectorStyle.Circle;
                        vm.InputConnector.TintColor = UnityEngine.Color.green;
                    }
                    
                }
                foreach (var item in Action.OutputVars)
                {
                    var vm = new InputOutputViewModel()
                    {
                        Name = item.Name,
                        DataObject = item,
                        IsOutput = true,
                        IsNewLine = item.ActionFieldInfo.DisplayType.IsNewLine,
                        DiagramViewModel = DiagramViewModel
                    };
                    ContentItems.Add(vm);

                    if (item.ActionFieldInfo.Type != typeof (Action))
                    {
                        vm.OutputConnector.Style = ConnectorStyle.Circle;
                        vm.OutputConnector.TintColor = UnityEngine.Color.green;
                    }
                    

                }
               
            }
            base.CreateContent();

        }
    }


}
