namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core.GraphDesigner;
    
    
    public class ComponentFilterNodeDrawer : GenericNodeDrawer<ComponentFilterNode,ComponentFilterNodeViewModel> {
        
        public ComponentFilterNodeDrawer(ComponentFilterNodeViewModel viewModel) : 
                base(viewModel) {
        }
    }
}
