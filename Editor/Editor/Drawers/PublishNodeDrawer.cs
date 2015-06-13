namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core.GraphDesigner;
    
    
    public class PublishNodeDrawer : GenericNodeDrawer<PublishNode,PublishNodeViewModel> {
        
        public PublishNodeDrawer(PublishNodeViewModel viewModel) : 
                base(viewModel) {
        }
    }
}
