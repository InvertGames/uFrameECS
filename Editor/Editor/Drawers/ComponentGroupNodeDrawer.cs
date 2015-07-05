namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core.GraphDesigner;
    
    
    public class ComponentGroupNodeDrawer : GenericNodeDrawer<ComponentGroupNode,ComponentGroupNodeViewModel> {
        
        public ComponentGroupNodeDrawer(ComponentGroupNodeViewModel viewModel) : 
                base(viewModel) {
        }
    }
}
