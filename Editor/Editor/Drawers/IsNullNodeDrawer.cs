namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core.GraphDesigner;
    
    
    public class IsNullNodeDrawer : GenericNodeDrawer<IsNullNode,IsNullNodeViewModel> {
        
        public IsNullNodeDrawer(IsNullNodeViewModel viewModel) : 
                base(viewModel) {
        }
    }
}
