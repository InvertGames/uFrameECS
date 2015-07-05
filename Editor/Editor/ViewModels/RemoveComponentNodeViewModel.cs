namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    
    
    public class RemoveComponentNodeViewModel : RemoveComponentNodeViewModelBase {
        
        public RemoveComponentNodeViewModel(RemoveComponentNode graphItemObject, Invert.Core.GraphDesigner.DiagramViewModel diagramViewModel) : 
                base(graphItemObject, diagramViewModel) {
        }
    }
}
