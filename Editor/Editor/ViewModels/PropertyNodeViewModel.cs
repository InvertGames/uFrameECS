namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    
    
    public class PropertyNodeViewModel : PropertyNodeViewModelBase {
        
        public PropertyNodeViewModel(PropertyNode graphItemObject, Invert.Core.GraphDesigner.DiagramViewModel diagramViewModel) : 
                base(graphItemObject, diagramViewModel) {
        }

        public override string Name
        {
            get { return "Property"; }
            set { base.Name = value; }
        }
    }
}
