using Invert.Core.GraphDesigner;

namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    
    
    public class ActionNodeViewModel : ActionNodeViewModelBase {
        
        public ActionNodeViewModel(ActionNode graphItemObject, Invert.Core.GraphDesigner.DiagramViewModel diagramViewModel) : 
                base(graphItemObject, diagramViewModel) {
        }

        public virtual bool ShowContextVariables
        {
            get { return true; }
        }
        protected override void CreateContent()
        {
            if (ShowContextVariables )
            {
                foreach (var item in GraphItem.AllContextVariables)
                {
                    ContentItems.Add(new ItemViewModel<IContextVariable>(item, this));
                }
            }
          
            base.CreateContent();
          
        }
    }


}
