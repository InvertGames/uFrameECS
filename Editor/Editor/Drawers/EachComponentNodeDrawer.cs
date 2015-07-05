namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core.GraphDesigner;
    
    
    public class EachComponentNodeDrawer : GenericNodeDrawer<EachComponentNode,EachComponentNodeViewModel> {
        
        public EachComponentNodeDrawer(EachComponentNodeViewModel viewModel) : 
                base(viewModel) {
        }
    }
}
