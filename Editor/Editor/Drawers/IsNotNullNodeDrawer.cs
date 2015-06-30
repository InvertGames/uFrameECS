namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core.GraphDesigner;
    
    
    public class IsNotNullNodeDrawer : GenericNodeDrawer<IsNotNullNode,IsNotNullNodeViewModel> {
        
        public IsNotNullNodeDrawer(IsNotNullNodeViewModel viewModel) : 
                base(viewModel) {
        }
    }
}
