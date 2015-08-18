namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    
    
    public class PropertyChangedNodeViewModel : PropertyChangedNodeViewModelBase {
        
        public PropertyChangedNodeViewModel(PropertyChangedNode graphItemObject, Invert.Core.GraphDesigner.DiagramViewModel diagramViewModel) : 
                base(graphItemObject, diagramViewModel) {
        }

        public override IEnumerable<string> Tags
        {
            get { yield break; }
        }

        public PropertyChangedNode ChangedNode
        {
            get { return GraphItem as PropertyChangedNode; }
        }
        public override string Name
        {
            get { return ChangedNode.DisplayName; }
            set { base.Name = value; }
        }

        public override bool IsEditable
        {
            get { return false; }
        }
    }
}
