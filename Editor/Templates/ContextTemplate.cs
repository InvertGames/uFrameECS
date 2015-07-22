using System.Collections.Generic;
using System.Linq;
using Invert.Core.GraphDesigner;
using uFrame.ECS;

namespace Invert.uFrame.ECS.Templates
{
    public partial class ContextTemplate
    {
        [GenerateConstructor("system")]
        public void Constructor(EcsSystem system)
        {

        }



        [GenerateMethod, AsOverride]
        protected virtual IEnumerable<IEcsComponentManager> GetSelectManagers()
        {
            foreach (var item in SelectComponents)
            {
                Ctx._("yield return ComponentSystem.RegisterComponent<{0}>()", item.Name);
            }
            Ctx._("yield break");
            yield break;
        }

        [GenerateMethod(CallBase = false), AsOverride]
        public bool Match(int entityId)
        {
            foreach (var item in SelectComponents)
            {
                Ctx.CurrentDeclaration._private_(item.Name, "_" + item.Name.ToLower());
                Ctx._if("!ComponentSystem.TryGetComponent(entityId, out _{0})", item.Name.ToLower())
                    .TrueStatements._("return false");
            }
            //return base.Match(entityId);
            Ctx._("return true");
            return true;
        }

        [GenerateMethod, AsOverride]
        public _CONTEXTITEM_ Select()
        {
            Ctx._("var item = new {0}()", new _CONTEXTITEM_().TheType(Ctx));
            foreach (var item in SelectComponents)
            {
                Ctx._("item.{0} = _{1}", item.Name, item.Name.ToLower());
            }
            Ctx._("return item");
            return null;
        }
    }
}