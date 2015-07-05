using Invert.Core.GraphDesigner;

namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    
    
    public class OnEventNodeViewModel : OnEventNodeViewModelBase {
        
        public OnEventNodeViewModel(OnEventNode graphItemObject, Invert.Core.GraphDesigner.DiagramViewModel diagramViewModel) : 
                base(graphItemObject, diagramViewModel) {
        }

        public override bool IsEditable
        {
            get { return false; }
        }

        public override bool ShowContextVariables
        {
            get { return IsVisible(SectionVisibility.WhenNodeIsFilter); }
        }

        protected override void CreateContent()
        {
            base.CreateContent();
            //if (this.IsVisible(SectionVisibility.WhenNodeIsNotFilter))
            //{
            //    foreach (var item in GraphItem.AllContextVariables.OfType<ITypedItem>())
            //    {
            //        ContentItems.Add(new InputOutputViewModel()
            //        {
            //            DataObject = item,
            //            Name = item.Name,
                        
            //        });
            //    }
            //}
            //else
            //{
                
            //}
        }
    }
}
