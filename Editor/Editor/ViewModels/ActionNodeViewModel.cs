using Invert.Core.GraphDesigner;
using uFrame.Actions.Attributes;

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

        public ActionNode Action
        {
            get { return GraphItem as ActionNode; }
        }

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
                    ContentItems.Add(new InputOutputViewModel()
                    {
                        Name = item.Name,
                        IsOutput = false,
                        IsInput = true,
                        DataObject = item,
                        IsNewLine = item.ActionFieldInfo.DisplayType.IsNewLine,
                        DiagramViewModel = DiagramViewModel
                    });
                }
                foreach (var item in Action.OutputVars)
                {
                    ContentItems.Add(new InputOutputViewModel()
                    {
                        Name = item.Name,
                        DataObject = item,
                        IsOutput = true,
                        IsNewLine = item.ActionFieldInfo.DisplayType.IsNewLine,
                        DiagramViewModel = DiagramViewModel
                    });
                }
               
            }
            base.CreateContent();

        }
    }


}
