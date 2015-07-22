using Invert.Core.GraphDesigner;

namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    
    
    public class ComponentNodeViewModel : ComponentNodeViewModelBase {
        
        public ComponentNodeViewModel(ComponentNode graphItemObject, Invert.Core.GraphDesigner.DiagramViewModel diagramViewModel) : 
                base(graphItemObject, diagramViewModel) {
        }

        public override NodeColor Color
        {
            get
            {
                if (GraphItem.Blackboard)
                {
                    return NodeColor.Black;
                }
                return base.Color;
            }
        }
    }
}
