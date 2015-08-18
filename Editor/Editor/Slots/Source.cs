namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core.GraphDesigner;
    
    
    public class Source : SourceBase, IFilterInput
    {
        public string HandlerPropertyName { get { return "Source"; } }

        public IMappingsConnectable FilterNode
        {
            get { return this.InputFrom<IMappingsConnectable>(); }
        }

        public string MappingId { get { return "EntityId"; } }
    }
    
    public partial interface ISourceConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
