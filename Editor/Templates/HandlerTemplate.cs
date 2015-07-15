using System.Linq;
using System.Runtime.InteropServices;
using Invert.Core.GraphDesigner;
using uFrame.ECS;

namespace Invert.uFrame.ECS.Templates
{
    public partial class HandlerTemplate
    {
        [GenerateProperty, WithField]
        public object Event
        {
            get
            {
                this.Ctx.SetType(Ctx.Data.EventNode.Name);
                return null;
            }
            set
            {
                
            }
        }

        [GenerateProperty, WithField]
        public EcsSystem System { get; set; }

        [TemplateSetup]
        public void SetName()
        {
            this.Ctx.CurrentDeclaration.Name = Ctx.Data.HandlerMethodName;
            var inputFilter = Ctx.Data.InputFrom<ContextNode>();
            if (inputFilter != null)
            {
                CreateFilterProperty("EntityId", inputFilter);
            }
            foreach (var item in  Ctx.Data.Mappings)
            {
                //var a = item.SourceItem as PropertiesChildItem;
                CreateFilterProperty(item.SourceItem.Name, inputFilter);
            }
            
        }

        private void CreateFilterProperty(string mappingId, ContextNode inputFilter)
        {
            var property = Ctx.CurrentDeclaration._public_(inputFilter.Name + "ContextItem", mappingId + "Item");

        }

        [GenerateMethod]
        public void Execute()
        {
            if (Ctx.Data.Right != null)
            {
                Ctx.Data.Right.WriteCode(Ctx);
            }
            
        }
    }
}