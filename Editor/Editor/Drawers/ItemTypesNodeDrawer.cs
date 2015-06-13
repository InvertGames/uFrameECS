namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core.GraphDesigner;
    
    
    public class ItemTypesNodeDrawer : GenericNodeDrawer<ItemTypesNode,ItemTypesNodeViewModel> {
        
        public ItemTypesNodeDrawer(ItemTypesNodeViewModel viewModel) : 
                base(viewModel) {
        }
    }
}
