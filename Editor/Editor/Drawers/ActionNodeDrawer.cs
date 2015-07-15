namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core.GraphDesigner;
    
    
    public class ActionNodeDrawer : GenericNodeDrawer<ActionNode,ActionNodeViewModel> {
        
        public ActionNodeDrawer(ActionNodeViewModel viewModel) : 
                base(viewModel) {
        }
        
        public override float HeaderPadding
        {
            get { return 3; }
        }

        public override float MinWidth
        {
            get { return 100f; }
        }

        //public override bool ShowHeader
        //{
        //    get
        //    {
        //        if (NodeViewModel.Action.Meta.Method == null)
        //            return false;
        //        return true;
        //    }
        //}
    }
}
