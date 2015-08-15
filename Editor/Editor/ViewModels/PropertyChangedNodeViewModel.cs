namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    
    
    public class PropertyChangedNodeViewModel : PropertyChangedNodeViewModelBase {
        
        public PropertyChangedNodeViewModel(PropertyChangedNode graphItemObject, Invert.Core.GraphDesigner.DiagramViewModel diagramViewModel) : 
                base(graphItemObject, diagramViewModel) {
        }
    }
}
