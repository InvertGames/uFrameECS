namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core.GraphDesigner;
    
    
    public class SetPropertyNodeDrawer : GenericNodeDrawer<SetPropertyNode,SetPropertyNodeViewModel> {
        
        public SetPropertyNodeDrawer(SetPropertyNodeViewModel viewModel) : 
                base(viewModel) {
        }
    }
}
