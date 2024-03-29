using Invert.Core.GraphDesigner;

namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    
    
    public class HandlerNodeViewModel : HandlerNodeViewModelBase {
        
        public HandlerNodeViewModel(HandlerNode graphItemObject, Invert.Core.GraphDesigner.DiagramViewModel diagramViewModel) : 
                base(graphItemObject, diagramViewModel) {
        }

        public HandlerNode Handler
        {
            get { return GraphItem as HandlerNode; }
        }

        //public override bool IsEditable
        //{
        //    get { return false; }
        //}

        public override IEnumerable<string> Tags
        {
            get
            {
                if (Handler == null) yield break;
                if (Handler.Meta == null)
                {
                    yield return "Event Not Found";
                    yield break;
                }
                yield return Handler.Meta.Attribute.Title;
            }
        }

        public HandlerNode HandlerNode
        {
            get { return GraphItem as HandlerNode; }
        }

        protected override void CreateContent()
        {
            base.CreateContent();
            if (IsVisible(SectionVisibility.WhenNodeIsNotFilter))
            {
                foreach (var item in Handler.HandlerInputs)
                {
                    var vm = new InputOutputViewModel()
                    {
                        DataObject = item,
                        Name = item.EventFieldInfo.Name,
                        IsInput = true,
                        IsOutput = false,
                        IsNewLine = true
                    };
                    ContentItems.Add(vm);
                }
            }
            
        }
    }
}
