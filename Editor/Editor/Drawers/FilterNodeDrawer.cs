namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core.GraphDesigner;
    
    
    public class FilterNodeDrawer : GenericNodeDrawer<FilterNode,FilterNodeViewModel> {
        
        public FilterNodeDrawer(FilterNodeViewModel viewModel) : 
                base(viewModel) {
        }
        //public override bool ShowHeader
        //{
        //    get { return false; }
        //}   
    }
}
