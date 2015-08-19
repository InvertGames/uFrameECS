using Invert.Data;

namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;
    
    
    public class GetPropertyNode : GetPropertyNodeBase {
        
        
        public ActionIn Object { get; set; }

        public override IEnumerable<IGraphItem> GraphItems
        {
            get { return base.GraphItems; }
        }
    }

    public class VariableReferenceItem : IDataRecord
    {
        public IRepository Repository { get; set; }
        public string Identifier { get; set; }
        public bool Changed { get; set; }
    }
    
    public partial interface IGetPropertyConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
