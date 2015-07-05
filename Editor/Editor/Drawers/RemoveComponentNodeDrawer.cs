namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core.GraphDesigner;
    
    
    public class RemoveComponentNodeDrawer : GenericNodeDrawer<RemoveComponentNode,RemoveComponentNodeViewModel> {
        
        public RemoveComponentNodeDrawer(RemoveComponentNodeViewModel viewModel) : 
                base(viewModel) {
        }
    }
}
