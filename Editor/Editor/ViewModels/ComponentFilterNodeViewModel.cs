namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    
    
    public class ComponentFilterNodeViewModel : ComponentFilterNodeViewModelBase {
        
        public ComponentFilterNodeViewModel(ComponentFilterNode graphItemObject, Invert.Core.GraphDesigner.DiagramViewModel diagramViewModel) : 
                base(graphItemObject, diagramViewModel) {
        }
    }
}
