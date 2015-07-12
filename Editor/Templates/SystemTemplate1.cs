using System;
using System.CodeDom;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Invert.Core.GraphDesigner;
using Invert.uFrame.ECS;
using uFrame.ECS;
using uFrame.Kernel;
using UniRx;


namespace Invert.uFrame.ECS.Templates
{
    [ForceBaseType(typeof(EcsSystem))]
    [RequiresNamespace("uFrame.ECS")]
    [RequiresNamespace("UniRx")]
    public partial class SystemTemplate
    {

        [ForEach("FilterNodes"), GenerateProperty, WithField]
        public _CONTEXT_ _Name_Context { get; set; }

        [GenerateMethod(TemplateLocation.Both), AsOverride, InsideAll]
        public void Setup()
        {
            Ctx.CurrentMethod.invoke_base();
            if (!Ctx.IsDesignerFile) return;

            foreach (var item in Components)
            {
                Ctx._("{0}Manager = ComponentSystem.RegisterComponent<{0}>()", item.Name);
            }
            foreach (var item in ComponentGroups)
            {
                Ctx._("{0}Manager = ComponentSystem.RegisterComponent<{0}>()", item.Name);
            }
            foreach (var item in FilterNodes)
            {
                Ctx._("{0}Context = new {0}Context(this)", item.Name);
                
            }
            foreach (var item in Ctx.Data.Graph.NodeItems.OfType<ISetupCodeWriter>())
            {
                item.WriteSetupCode(Ctx);
            }


        }

        [ForEach("Components"), GenerateProperty, WithField]
        public IEcsComponentManagerOf<_ITEMTYPE_> _Name_Manager { get; set; }

        [ForEach("ComponentGroups"), GenerateProperty, WithField]
        public IEcsComponentManagerOf<_ITEMTYPE_> _GroupName_Manager { get; set; }
    }

    [RequiresNamespace("uFrame.ECS")]
    [RequiresNamespace("UniRx")]
    [NamespacesFromItems]
    public partial class ComponentTemplate
    {

        public static int _ComponentIds = 1;
        [GenerateProperty]
        public int ComponentID
        {
            get
            {
                Ctx._("return {0}", _ComponentIds++);
                return 0;
            }
        }

        [ForEach("Properties"), GenerateProperty, WithLazyField(typeof(Subject<_ITEMTYPE_>), true)]
        public IObservable<_ITEMTYPE_> _Name_Observable { get { return null; } }

        [ForEach("Properties"), GenerateProperty, WithName, WithField(null, typeof(SerializeField))]
        public _ITEMTYPE_ Property
        {
            get { return null; }
            set
            {
                Ctx._if("_{0}Observable != null", Ctx.Item.Name).TrueStatements
                    ._("_{0}Observable.OnNext(value)", Ctx.Item.Name);
            }
        }

        [ForEach("Collections"), GenerateProperty, WithName, WithLazyField]
        public List<_ITEMTYPE_> Collection { get; set; }
    }

    [RequiresNamespace("uFrame.ECS")]
    [RequiresNamespace("UniRx")]
    [NamespacesFromItems]
    public partial class EventTemplate
    {
        [ForEach("Properties"), GenerateProperty,WithName, WithField(null, typeof(SerializeField))]
        public _ITEMTYPE_ Property { get; set; }

        [ForEach("Collections"), GenerateProperty, WithName, WithLazyField]
        public List<_ITEMTYPE_> Collection { get; set; }
    }

    //public IObservable<_ITEMTYPE_> _Name_Observable
    //{
    //    get
    //    {
    //        // return _MaxNavigatorsObservable ?? (_MaxNavigatorsObservable = new Subject<int>());
    //    }
    //}
    //public virtual Int32 MaxNavigators
    //{
    //    get
    //    {
    //        return _MaxNavigators;
    //    }
    //    set
    //    {
    //        _MaxNavigators = value;
    //        if (_MaxNavigatorsObservable != null)
    //        {
    //            _MaxNavigatorsObservable.OnNext(value);
    //        }
    //    }
    //}

    //[RequiresNamespace("uFrame.ECS")]
    //public partial class OnEventTemplate
    //{


    //}

    //[RequiresNamespace("uFrame.ECS")]
    //public partial class PublishTemplate
    //{


    //}

    public partial class ComponentGroupTemplate
    {
        [ForEach("Components"), GenerateProperty, WithField(typeof(_ITEMTYPE_), typeof(SerializeField))]
        public _ITEMTYPE_ _Name_ { get; set; }

    }

    public partial class ComponentGroupManagerTemplate
    {
        [GenerateMethod]
        public virtual bool Filter(_ITEMTYPE_ item)
        {

            var filterBegin = Ctx.Data.FilterOutputSlot.OutputTo<ActionNode>();

            if (filterBegin != null)
            {
                filterBegin.WriteCode(Ctx);
            }

            //Ctx.CurrentMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), "entityId"));


            Ctx._("return true");
            return false;
        }

        [GenerateMethod]
        public virtual _ITEMTYPE_ CreateGroup()
        {
            //var validateInvoke = new CodeMethodInvokeExpression(
            //    new CodeThisReferenceExpression(),
            //    "Validate");
            Ctx.CurrentMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), "entityId"));

            foreach (var component in Components)
            {
                Ctx.CurrentMethod.Parameters.Add(new CodeParameterDeclarationExpression(component.Name,
                    component.Name.ToLower()));

                //validateInvoke.Parameters.Add(new CodeVariableReferenceExpression(component.Name.ToLower()));
            }
            //var condition = new CodeConditionStatement(
            //    validateInvoke
            //    );

            Ctx._("var group = this.gameObject.AddComponent<{0}>()", Ctx.Data.Name);
            Ctx._("group.EntityId = entityId");
            foreach (var component in Components)
            {
                Ctx._("group.{0} = {1}", component.Name, component.Name.ToLower());
            }
            Ctx._("return group");
            //Ctx.CurrentStatements.Add(condition);
            //Ctx._("return null");
            return null;
        }

        [GenerateMethod, AsOverride]
        public void ComponentCreated(IEcsComponent ecsComponent)
        {

            Ctx._if("Ids.Contains(ecsComponent.EntityId)").TrueStatements._("return");
            //Ctx._if("ecsComponent is ComponentGroup").TrueStatements._("return");
            foreach (var component in Components)
            {
                Ctx._("{0} {1}", component.Name, component.Name.ToLower());
            }

            foreach (var component in Components)
            {
                Ctx._if("!ComponentSystem.TryGetComponent(ecsComponent.EntityId, out {0})", component.Name.ToLower())
                    .TrueStatements
                        ._("return");
            }

            Ctx._("Ids.Add(ecsComponent.EntityId)");

            CodeMethodInvokeExpression invokeAdd = new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                "CreateGroup"
                );

            var groupVar = new CodeVariableDeclarationStatement(Ctx.Data.Name, "group", invokeAdd);

            invokeAdd.Parameters.Add(new CodeVariableReferenceExpression("ecsComponent.EntityId"));
            invokeAdd.Parameters.AddRange(
                    Components.Select(_ => new CodeVariableReferenceExpression(_.Name.ToLower()))
                    .Cast<CodeExpression>().ToArray()
                );
            Ctx.CurrentStatements.Add(groupVar);

            Ctx._("this.OnEvent<ComponentDestroyedEvent>().First(p=> p.Component == {0}).Subscribe(_=>{{ Ids.Remove(group.EntityId); DestroyImmediate(group); }}).DisposeWith(this);",
                    String.Join("|| p.Component == ", Components.Select(p => p.Name.ToLower()).ToArray())
                );
            // Put any observables here EXAMPLE:
            //damageable.HealthObservable.Subscribe(health =>
            //{
            //    UpdateGroup(group);
            //}).DisposeWith(damageable);


        }
    }


    public partial class ContextTemplate
    {
        [GenerateConstructor("system")]
        public void Constructor(EcsSystem system)
        {

        }

        [GenerateMethod, AsOverride]
        protected virtual IEnumerable<IEcsComponentManager> GetWithAnyManagers()
        {
            foreach (var item in WithAnyComponents)
            {
                Ctx._("yield return ComponentSystem.RegisterComponent<{0}>()", item.Name);
            }
            Ctx._("yield break");
            yield break;
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
            if (WithAnyComponents.Any())
            {
                Ctx._if("!ComponentSystem.HasAny(entityId, WithAnyTypes)")
                    .TrueStatements._("return false");
            }

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

    public partial class ContextItemTemplate
    {
        [ForEach("SelectComponents"),GenerateProperty, WithField]
        public _ITEMTYPE_ _Name_ { get; set; }
    }

}

