using System.CodeDom;

namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;
    
    
    public class PropertyChangedNode : PropertyChangedNodeBase, ISequenceNode, ISetupCodeWriter {
        public bool CanGenerate { get { return true; } }

        public override string Name
        {
            get
            {
                if (SourceProperty != null)
                    return string.Format("{0}PropertyChanged",SourceProperty.Name);
                return "PropertyChanged";
            }
            set { base.Name = value; }
        }

        public IPropertyConnectable SourceProperty
        {
            get { return  PropertyInputSlot.Item; }
        }

        public string HandlerMethodName
        {
            get { return Graph.CurrentFilter.Name + this.Name; }
        }

        public IEnumerable<IFilterInput> FilterInputs
        {
            get { yield break; }
        }

        public string EventType
        {
            get { return "uFrame.ECS.ComponentCreatedEvent"; }
            set
            {
                
            }
        }

        public void Accept(IHandlerNodeVisitor csharpVisitor)
        {
            
        }

        public override void WriteCode(TemplateContext ctx)
        {
            base.WriteCode(ctx);
        }

        public void WriteSetupCode(TemplateContext ctx)
        {
            if (SourceProperty == null) return;
            var relatedTypeProperty = SourceProperty as GenericTypedChildItem;
             
            var handlerMethod = ctx.CurrentDeclaration.protected_func(typeof(void), HandlerMethodName);
            handlerMethod.Parameters.Add(new CodeParameterDeclarationExpression(SourceProperty.Node.Name, "data"));
            handlerMethod.Parameters.Add(new CodeParameterDeclarationExpression(relatedTypeProperty.RelatedTypeName, "value"));

            ctx._("this.OnComponentCreated<{0}>().Subscribe(_=>{{ " +
                    "_.{1}Observable.Subscribe(v=>{{ " +
                        "{2}(_, v)" +
                    " }}).DisposeWith(_).DisposeWith(this)" +
                  " }}).DisposeWith(this)",SourceProperty.Node.Name, SourceProperty.Name);

        }
    }
    
    public partial interface IPropertyChangedConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
