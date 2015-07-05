namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core.GraphDesigner;
    
    
    public class FilterExpressionNodeDrawer : GenericNodeDrawer<FilterExpressionNode,FilterExpressionNodeViewModel> {
        
        public FilterExpressionNodeDrawer(FilterExpressionNodeViewModel viewModel) : 
                base(viewModel) {
        }
    }
}
