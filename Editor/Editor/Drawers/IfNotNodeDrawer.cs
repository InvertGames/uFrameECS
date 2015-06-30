namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core.GraphDesigner;
    
    
    public class IfNotNodeDrawer : GenericNodeDrawer<IfNotNode,IfNotNodeViewModel> {
        
        public IfNotNodeDrawer(IfNotNodeViewModel viewModel) : 
                base(viewModel) {
        }
    }
}
