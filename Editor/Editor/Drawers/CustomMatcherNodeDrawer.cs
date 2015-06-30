namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core.GraphDesigner;
    
    
    public class CustomMatcherNodeDrawer : GenericNodeDrawer<CustomMatcherNode,CustomMatcherNodeViewModel> {
        
        public CustomMatcherNodeDrawer(CustomMatcherNodeViewModel viewModel) : 
                base(viewModel) {
        }
    }
}
