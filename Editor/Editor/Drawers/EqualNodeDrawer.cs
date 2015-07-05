namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core.GraphDesigner;
    
    
    public class EqualNodeDrawer : GenericNodeDrawer<EqualNode,EqualNodeViewModel> {
        
        public EqualNodeDrawer(EqualNodeViewModel viewModel) : 
                base(viewModel) {
        }
    }
}
