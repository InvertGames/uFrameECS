namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core.GraphDesigner;
    
    
    public class IntervalNodeDrawer : GenericNodeDrawer<IntervalNode,IntervalNodeViewModel> {
        
        public IntervalNodeDrawer(IntervalNodeViewModel viewModel) : 
                base(viewModel) {
        }
    }
}
