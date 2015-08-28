using Invert.Data;

namespace Invert.uFrame.ECS
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;


    public class PropertyNode : PropertyNodeBase, IContextVariable
    {
        private PropertyIn _o;
        private PropertySelection _propertySelection;

        [InputSlot("Object")]
        public PropertyIn Object
        {
            get
            {
                return _o ?? (_o = new PropertyIn()
            {
                Repository = this.Repository,
                Node = this.Node,
                Name = "Object",
                DoesAllowInputs = true,
                Identifier = Identifier + ":" + "Object"

            });
            }
        }
        [InputSlot("Property")]
        public PropertySelection PropertySelection
        {
            get
            {
                return _propertySelection ?? (_propertySelection = new PropertySelection()
                {
                    Repository = this.Repository,
                    Node = this.Node,
                    Name = "Property",
                    ObjectSelector = Object,
                    Identifier = Identifier + ":" + "Property",
                   
                });
            }

        }

        public override IEnumerable<IGraphItem> GraphItems
        {
            get
            {
                yield return Object;
                yield return PropertySelection;
            }
        }

        public override string Name
        {
            get { return base.Name; }
            set { base.Name = value; }
        }

        public ITypedItem Source
        {
            get
            {
                if (PropertySelection.Item == null) return null;
                return PropertySelection.Item.Source;
            }
        }

        public string VariableName
        {
            get
            {
                if (PropertySelection.Item == null) return "--Select--";
                return PropertySelection.Item.VariableName;
            }
           
        }

        public string VariableType
        {
            get
            {
                if (PropertySelection.Item == null) return null;
                return PropertySelection.Item.VariableType;
            }
        }

        public IEnumerable<IContextVariable> GetPropertyDescriptions()
        {
            if (PropertySelection.Item == null) yield break;
            foreach (var item in  PropertySelection.Item.GetPropertyDescriptions()) yield return item;
        }
    }


    public class PropertySelection : SelectionFor<IContextVariable, PropertySelectionValue>
    {
        public override bool AllowInputs
        {
            get { return false; }
        }

      
        public PropertyIn ObjectSelector { get; set; }

        public override IEnumerable<IGraphItem> GetAllowed()
        {
            var item = ObjectSelector.Item;
            if (item == null) yield break;
            foreach (var property in item.GetPropertyDescriptions())
            {
                yield return property;
            }
        }
    }

    public class PropertySelectionValue : InputSelectionValue
    {

    }
    public partial interface IPropertyConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable
    {
    }
}
