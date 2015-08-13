// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 2.0.50727.1433
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core.GraphDesigner;
    
    
    public class VariableBase : SingleInputSlot<IVariableConnectable> {
        
        public override string Name {
            get {
                return "Variable";
            }
            set {
            }
        }
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
    }
    
    public partial interface IVariableConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class ValueBase : SingleInputSlot<IValueConnectable> {
        
        public override string Name {
            get {
                return "Value";
            }
            set {
            }
        }
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
    }
    
    public partial interface IValueConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class TimerBase : SingleInputSlot<ITimerConnectable> {
        
        public override string Name {
            get {
                return "Timer";
            }
            set {
            }
        }
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
    }
    
    public partial interface ITimerConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class FilterByBase : SingleInputSlot<IFilterByConnectable> {
        
        public override string Name {
            get {
                return "Filter By";
            }
            set {
            }
        }
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
    }
    
    public partial interface IFilterByConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class VariablesBase : SingleInputSlot<IVariablesConnectable> {
        
        public override string Name {
            get {
                return "Variables";
            }
            set {
            }
        }
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
    }
    
    public partial interface IVariablesConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class EventBase : SingleInputSlot<IEventConnectable> {
        
        public override string Name {
            get {
                return "Event";
            }
            set {
            }
        }
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
    }
    
    public partial interface IEventConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class ComponentBase : SingleInputSlot<IComponentConnectable> {
        
        public override string Name {
            get {
                return "Component";
            }
            set {
            }
        }
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
    }
    
    public partial interface IComponentConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class EachBase : SingleOutputSlot<IEachConnectable>, IActionConnectable {
        
        public override string Name {
            get {
                return "Each";
            }
            set {
            }
        }
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
    }
    
    public partial interface IEachConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
