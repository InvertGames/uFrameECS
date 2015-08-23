using System.Collections.Generic;
using System.Linq;
using Invert.Core.GraphDesigner;

namespace Invert.uFrame.ECS
{
    public class EntityGroupIn : SelectionFor<IMappingsConnectable, HandlerInValue>, IFilterInput
    {
        public IMappingsConnectable FilterNode
        {
            get { return this.Item; }
        }

        public virtual string MappingId
        {
            get { return "EntityId"; }
        }

        public override string Name
        {
            get { return "Group"; }
            set { base.Name = value; }
        }

        public override string Title
        {
            get { return "Group"; }
        }

        public virtual string HandlerPropertyName
        {
            get { return Name; }
        }

        public override IEnumerable<IGraphItem> GetAllowed()
        {
            return Repository.AllOf<IMappingsConnectable>().OfType<IGraphItem>();
        }
        public override bool AllowInputs
        {
            get { return false; }
        }

    }

    public class HandlerIn : EntityGroupIn, IFilterInput
    {
    
        public override string MappingId
        {
            get { return EventFieldInfo.Name; }
        }

        public EventFieldInfo EventFieldInfo { get; set; }
        public override string Title
        {
            get
            {
                return EventFieldInfo.Title;
            }
        }
        public override string Name
        {
            get { return EventFieldInfo.Name; }
            set { base.Name = value; }
        }

        public override string HandlerPropertyName
        {
            get { return Name; }
        }

    }

    public class HandlerInValue : InputSelectionValue
    {
        
    }
}