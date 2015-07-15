namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    
    
    public class ActionLibraryNodeViewModel : ActionLibraryNodeViewModelBase {
        
        public ActionLibraryNodeViewModel(ActionLibraryNode graphItemObject, Invert.Core.GraphDesigner.DiagramViewModel diagramViewModel) : 
                base(graphItemObject, diagramViewModel) {
        }
    }
}
