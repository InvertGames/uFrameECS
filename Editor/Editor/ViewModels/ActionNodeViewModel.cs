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
                if (Action.Meta == null)
                {
                    yield return "Action Not Found";
                    yield break;
                }
                yield return Action.Meta.TitleText;
            }
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
