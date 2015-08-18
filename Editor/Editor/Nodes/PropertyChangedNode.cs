using System.CodeDom;

namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;
    
    
    public class PropertyChangedNode : PropertyChangedNodeBase, ISequenceNode, ISetupCodeWriter {
        public override bool CanGenerate { get { return true; } }

        //public override IEnumerable<IFilterInput> FilterInputs
        //{
        //    get
        //    {
        //        yield return SourceInputSlot;
        //    }
        //}

        public override string Name
        {
            get
            {
                
                return "PropertyChanged"; 
            }
            set { base.Name = value; }
        }

        public IPropertyConnectable SourceProperty
        {
            get { return  PropertyInputSlot.Item; }
        }

        public string DisplayName
        {
            get
            {
                if (Repository != null && !string.IsNullOrEmpty(this.PropertyInputSlotId) && PropertyInputSlot != null && SourceProperty != null)
                    return string.Format("{0}PropertyChanged", SourceProperty.Name);
                return "PropertyChanged";
            }
        }
        public override string HandlerMethodName
        {
            get
            {
                if (Repository != null && !string.IsNullOrEmpty(this.PropertyInputSlotId) && PropertyInputSlot != null && SourceProperty != null)
                    return string.Format("{0}{1}PropertyChanged",Graph.CurrentFilter.Name , SourceProperty.Name);
                return Graph.CurrentFilter.Name + "PropertyChanged";
            }
        }
        public override string HandlerFilterMethodName
        {
            get
            {
                if (Repository != null && !string.IsNullOrEmpty(this.PropertyInputSlotId) && PropertyInputSlot != null && SourceProperty != null)
                    return string.Format("{0}{1}PropertyChangedFilter", Graph.CurrentFilter.Name, SourceProperty.Name);
                return Graph.CurrentFilter.Name + "PropertyChangedFilter";
            }
        }

        public override string EventType
        {
            get { return SourceInputSlot.InputFrom<IMappingsConnectable>().Name; }
            set
            {
                
            }
        }

        public override void WriteCode(TemplateContext ctx)
        {
            base.WriteCode(ctx);
        }
        public override CodeMemberMethod WriteHandlerFilter(TemplateContext ctx, CodeMemberMethod handlerMethod)
        {
            return base.WriteHandlerFilter(ctx, handlerMethod);
        }

        protected override void WriteHandlerInvoker(CodeMethodInvokeExpression handlerInvoker, CodeMemberMethod handlerFilterMethod)
        {
            base.WriteHandlerInvoker(handlerInvoker, handlerFilterMethod);
            handlerInvoker.Parameters.Add(new CodeSnippetExpression("value"));
        }

        public override void WriteEventSubscription(TemplateContext ctx, CodeMemberMethod filterMethod, CodeMemberMethod handlerMethod)
        {
            //base.WriteEventSubscription(ctx, filterMethod, handlerMethod);
            var relatedTypeProperty = SourceProperty as GenericTypedChildItem;
            filterMethod.Parameters.Add(new CodeParameterDeclarationExpression(relatedTypeProperty.RelatedTypeName, "value"));
            handlerMethod.Parameters.Add(new CodeParameterDeclarationExpression(relatedTypeProperty.RelatedTypeName, "value"));

            ctx._("this.PropertyChanged<{0},{1}>(component=>component.{2}Observable, {3})", EventType, relatedTypeProperty.RelatedTypeName, SourceProperty.Name, filterMethod.Name);
        }

        public override IEnumerable<IMappingsConnectable> GetSystemGroups()
        { 
            //foreach (var item in Scope)
            //{
            //    yield return item.SourceItem as IMappingsConnectable;
            //}
            yield return SourceInputSlot.InputFrom<IMappingsConnectable>();
        }

        public IEnumerable GetObservableProperties()
        {
            foreach (var item in FilterInputs)
            {
                foreach (var p in item.InputFrom<IMappingsConnectable>().GetObservableProperties())
                {
                    yield return p;
                }
            }
        }
    }
    
    public partial interface IPropertyChangedConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
