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

        //public override string SubTitle
        //{
        //    get
        //    {
        //        if (Handler == null) return "EVENT NODE FOUND";
        //        if (Handler.Meta == null)
        //        {
        //             return "Event Not Found"; 
          
        //        }
        //         return Handler.Meta.Attribute.Title;
        //    }
        //}
         
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
           
            if (IsVisible(SectionVisibility.WhenNodeIsNotFilter))
            {
                var inputs = Handler.HandlerInputs;
                //if (inputs.Length > 0)
                //    ContentItems.Add(new GenericItemHeaderViewModel()
                //    {
                //        Name = "Mappings",
                //        DiagramViewModel = DiagramViewModel,
                //        IsNewLine = true,
                //    });
                foreach (var item in inputs)
                {
                    var vm = new InputOutputViewModel()
                    {
                        DataObject = item,
                        Name = item.Title,
                        IsInput = true,
                        IsOutput = false,
                        IsNewLine = true,
                        AllowSelection = true
                    };
                    ContentItems.Add(vm);
                }
            }
            base.CreateContent();
        }
    }
}
