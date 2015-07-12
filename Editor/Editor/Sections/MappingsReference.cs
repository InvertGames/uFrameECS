namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core.GraphDesigner;
    
    
    public class MappingsReference : MappingsReferenceBase {
        public ContextNode Context
        {
            get { return this.InputFrom<ContextNode>(); }
        }

        public override bool AllowOutputs
        {
            get { return false; }
        }

        public override string Name
        {
            get
            {
                var sourceItem = SourceItem as PropertiesChildItem;
                if (sourceItem != null)
                    return sourceItem.FriendlyName;
                return base.Name;
            }
            set { base.Name = value; }
        }
    }
    
    public partial interface IMappingsConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
