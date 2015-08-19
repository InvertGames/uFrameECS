namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core.GraphDesigner;
    
    
    public class GetPropertyNodeDrawer : GenericNodeDrawer<GetPropertyNode,GetPropertyNodeViewModel> {
        
        public GetPropertyNodeDrawer(GetPropertyNodeViewModel viewModel) : 
                base(viewModel) {
        }
    }
}
