namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core.GraphDesigner;
    
    
    public class ComponentsNodeDrawer : GenericNodeDrawer<ComponentsNode,ComponentsNodeViewModel> {
        
        public ComponentsNodeDrawer(ComponentsNodeViewModel viewModel) : 
                base(viewModel) {
        }
    }
}
