using Invert.Core.GraphDesigner;

namespace Invert.uFrame.ECS.Templates
{
    public partial class ContextItemTemplate
    {
        [ForEach("SelectComponents"),GenerateProperty, WithField]
        public _ITEMTYPE_ _Name_ { get; set; }
    }
}