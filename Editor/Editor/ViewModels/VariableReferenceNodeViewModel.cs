namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    
    
    public class VariableReferenceNodeViewModel : VariableReferenceNodeViewModelBase {
        
        public VariableReferenceNodeViewModel(VariableReferenceNode graphItemObject, Invert.Core.GraphDesigner.DiagramViewModel diagramViewModel) : 
                base(graphItemObject, diagramViewModel) {
        }

        public override string Name
        {
            get { return GraphItem.ShortName; }
            set { base.Name = value; }
        }

        public override IEnumerable<string> Tags
        {
            get { yield return GraphItem.Name; }
        }

        public override bool IsEditable
        {
            get { return false; }
        }
    }
}
