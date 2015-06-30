namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core.GraphDesigner;
    
    
    public class MatchAnyNodeDrawer : GenericNodeDrawer<MatchAnyNode,MatchAnyNodeViewModel> {
        
        public MatchAnyNodeDrawer(MatchAnyNodeViewModel viewModel) : 
                base(viewModel) {
        }
    }
}
