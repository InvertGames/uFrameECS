namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core.GraphDesigner;


    public class ContextNodeDrawer : GenericNodeDrawer<ContextNode, ContextNodeViewModel>
    {
        
        public ContextNodeDrawer(ContextNodeViewModel viewModel) : 
                base(viewModel) {
        }
   
    }
}
