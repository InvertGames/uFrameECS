using Invert.Core.GraphDesigner;

namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    
    
    public class SetVariableNodeViewModel : SetVariableNodeViewModelBase {
        public override INodeStyleSchema StyleSchema
        {
            get
            {
                return MinimalisticStyleSchema;
            }
        }
        public SetVariableNodeViewModel(SetVariableNode graphItemObject, Invert.Core.GraphDesigner.DiagramViewModel diagramViewModel) : 
                base(graphItemObject, diagramViewModel) {
        }
    }
}
