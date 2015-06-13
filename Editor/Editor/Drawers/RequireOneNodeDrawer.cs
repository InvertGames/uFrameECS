namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core.GraphDesigner;
    
    
    public class RequireOneNodeDrawer : GenericNodeDrawer<RequireOneNode,RequireOneNodeViewModel> {
        
        public RequireOneNodeDrawer(RequireOneNodeViewModel viewModel) : 
                base(viewModel) {
        }
    }
}
