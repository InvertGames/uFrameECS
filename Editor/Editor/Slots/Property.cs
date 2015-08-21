namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core.GraphDesigner;
    
    
    public class Property : PropertyBase {
        public override IEnumerable<IGraphItem> GetAllowed()
        {
            return Repository.AllOf<ComponentNode>().SelectMany(p=>p.PersistedItems)
                .OfType<PropertiesChildItem>().Cast<IGraphItem>();
            //var pcn = this.Node as PropertyChangedNode;
            //if (pcn != null)
            //{
            //    foreach (var item in pcn.GetObservableProperties())
            //    {
            //        yield return item as IGraphItem;
            //    }
            //}
            
        }
    }
    
    public partial interface IPropertyConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
