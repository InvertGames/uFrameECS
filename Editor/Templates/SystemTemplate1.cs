using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Invert.Core.GraphDesigner;
using Invert.uFrame.ECS;
using uFrame.ECS;

namespace Invert.uFrame.ECS.Templates
{
    [ForceBaseType(typeof(EcsSystem))]
    [RequiresNamespace("uFrame.ECS")]
    public partial class SystemTemplate
    {
        [GenerateMethod(TemplateLocation.Both),AsOverride, InsideAll]
        public void Setup()
        {
            Ctx.CurrentMethod.invoke_base();
            if (!Ctx.IsDesignerFile) return;

            foreach (var item in Components)
            {
                Ctx._("{0}Manager = ComponentSystem.RegisterComponent<{0}>()",item.Name);
            }
            
        }
        [ForEach("Components"), GenerateProperty, WithField]
        public IEcsComponentManagerOf<_ITEMTYPE_> _Name_Manager { get; set; }
    }
     [RequiresNamespace("uFrame.ECS")]
    public partial class ComponentTemplate
    {
         [ForEach("Properties"), GenerateProperty, WithField, WithName]
         public _ITEMTYPE_ Property { get; set; }

         [ForEach("Collections"), GenerateProperty, WithField, WithName]
         public List<_ITEMTYPE_> Collection { get; set; }
    }

    [RequiresNamespace("uFrame.ECS")]
    public partial class OnEventTemplate 
    {


    }
    
    [RequiresNamespace("uFrame.ECS")]
    public partial class PublishTemplate 
    {

    
    }
}

