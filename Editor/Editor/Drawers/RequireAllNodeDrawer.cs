namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core.GraphDesigner;
    
    
    public class RequireAllNodeDrawer : GenericNodeDrawer<RequireAllNode,RequireAllNodeViewModel> {
        
        public RequireAllNodeDrawer(RequireAllNodeViewModel viewModel) : 
                base(viewModel) {
        }
    }
}
