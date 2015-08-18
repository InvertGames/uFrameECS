using System.Collections.Generic;
using System.Linq;
using Invert.Core.GraphDesigner;

namespace Invert.uFrame.ECS
{
    public class HandlerIn : SingleInputSlot<IMappingsConnectable>, IFilterInput
    {
        public IMappingsConnectable FilterNode
        {
            get { return this.InputFrom<IMappingsConnectable>(); }
        }

        public string MappingId
        {
            get { return EventFieldInfo.Name; }
        }

        public EventFieldInfo EventFieldInfo { get; set; }

        public override string Name
        {
            get { return EventFieldInfo.Name; }
            set { base.Name = value; }
        }

        public string HandlerPropertyName
        {
            get { return Name; }
        }

        public override IEnumerable<IGraphItem> GetAllowed()
        {
            return Repository.AllOf<IMappingsConnectable>().OfType<IGraphItem>();
        }
    }
}