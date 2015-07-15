namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core.GraphDesigner;
    
    
    public class ActionLibraryNodeDrawer : GenericNodeDrawer<ActionLibraryNode,ActionLibraryNodeViewModel> {
        
        public ActionLibraryNodeDrawer(ActionLibraryNodeViewModel viewModel) : 
                base(viewModel) {
        }
    }
}
