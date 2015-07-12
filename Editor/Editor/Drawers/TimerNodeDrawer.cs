namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core.GraphDesigner;


    public class TimerNodeDrawer : GenericNodeDrawer<TimerNode, TimerNodeViewModel>
    {

        public TimerNodeDrawer(TimerNodeViewModel viewModel) : 
                base(viewModel) {
        }
    }
}
