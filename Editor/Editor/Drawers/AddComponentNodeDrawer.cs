namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core.GraphDesigner;
    
    
    public class AddComponentNodeDrawer : GenericNodeDrawer<AddComponentNode,AddComponentNodeViewModel> {
        
        public AddComponentNodeDrawer(AddComponentNodeViewModel viewModel) : 
                base(viewModel) {
        }
    }
}
