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

        public override bool IsEditable
        {
            get { return false; }
        }
         


        public HandlerNode HandlerNode
        {
            get { return GraphItem as HandlerNode; }
        }
       
    }
}
