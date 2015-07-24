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
                if (Ctx.Data.Meta == null)
                    return null;
                this.Ctx.SetType(Ctx.Data.Meta.Type);
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
            var inputFilter = Ctx.Data.InputFrom<IMappingsConnectable>();
            if (inputFilter != null)
            {
                CreateFilterProperty("EntityId", inputFilter);
            }
            foreach (var item in  Ctx.Data.HandlerInputs)
            {
                var context = item.Context;
                if (context == null) continue;
                //var a = item.SourceItem as PropertiesChildItem;
                CreateFilterProperty( item.Name, context );
            }
            
        }

        private void CreateFilterProperty(string mappingId, IMappingsConnectable inputFilter)
        {
            var property = Ctx.CurrentDeclaration._public_(inputFilter.ContextTypeName, inputFilter.GetContextItemName(mappingId));

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