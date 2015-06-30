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
    
    
    public class ComponentNodeViewModelBase : Invert.Core.GraphDesigner.GenericNodeViewModel<ComponentNode> {
        
        public ComponentNodeViewModelBase(ComponentNode graphItemObject, Invert.Core.GraphDesigner.DiagramViewModel diagramViewModel) : 
                base(graphItemObject, diagramViewModel) {
        }
    }
    
    public class SystemNodeViewModelBase : Invert.Core.GraphDesigner.GenericNodeViewModel<SystemNode> {
        
        public SystemNodeViewModelBase(SystemNode graphItemObject, Invert.Core.GraphDesigner.DiagramViewModel diagramViewModel) : 
                base(graphItemObject, diagramViewModel) {
        }
    }
    
    public class ItemTypesNodeViewModelBase : Invert.Core.GraphDesigner.GenericNodeViewModel<ItemTypesNode> {
        
        public ItemTypesNodeViewModelBase(ItemTypesNode graphItemObject, Invert.Core.GraphDesigner.DiagramViewModel diagramViewModel) : 
                base(graphItemObject, diagramViewModel) {
        }
    }
    
    public class EventNodeViewModelBase : Invert.Core.GraphDesigner.GenericNodeViewModel<EventNode> {
        
        public EventNodeViewModelBase(EventNode graphItemObject, Invert.Core.GraphDesigner.DiagramViewModel diagramViewModel) : 
                base(graphItemObject, diagramViewModel) {
        }
    }
    
    public class PublishNodeViewModelBase : EventNodeViewModel {
        
        public PublishNodeViewModelBase(PublishNode graphItemObject, Invert.Core.GraphDesigner.DiagramViewModel diagramViewModel) : 
                base(graphItemObject, diagramViewModel) {
        }
    }
    
    public class UserMethodNodeViewModelBase : ActionNodeViewModel {
        
        public UserMethodNodeViewModelBase(UserMethodNode graphItemObject, Invert.Core.GraphDesigner.DiagramViewModel diagramViewModel) : 
                base(graphItemObject, diagramViewModel) {
        }
    }
    
    public class ConditionNodeViewModelBase : ActionNodeViewModel {
        
        public ConditionNodeViewModelBase(ConditionNode graphItemObject, Invert.Core.GraphDesigner.DiagramViewModel diagramViewModel) : 
                base(graphItemObject, diagramViewModel) {
        }
    }
    
    public class LoopNodeViewModelBase : Invert.Core.GraphDesigner.GenericNodeViewModel<LoopNode> {
        
        public LoopNodeViewModelBase(LoopNode graphItemObject, Invert.Core.GraphDesigner.DiagramViewModel diagramViewModel) : 
                base(graphItemObject, diagramViewModel) {
        }
    }
    
    public class IfNodeViewModelBase : Invert.Core.GraphDesigner.GenericNodeViewModel<IfNode> {
        
        public IfNodeViewModelBase(IfNode graphItemObject, Invert.Core.GraphDesigner.DiagramViewModel diagramViewModel) : 
                base(graphItemObject, diagramViewModel) {
        }
    }
    
    public class IfNotNodeViewModelBase : Invert.Core.GraphDesigner.GenericNodeViewModel<IfNotNode> {
        
        public IfNotNodeViewModelBase(IfNotNode graphItemObject, Invert.Core.GraphDesigner.DiagramViewModel diagramViewModel) : 
                base(graphItemObject, diagramViewModel) {
        }
    }
    
    public class IsNullNodeViewModelBase : Invert.Core.GraphDesigner.GenericNodeViewModel<IsNullNode> {
        
        public IsNullNodeViewModelBase(IsNullNode graphItemObject, Invert.Core.GraphDesigner.DiagramViewModel diagramViewModel) : 
                base(graphItemObject, diagramViewModel) {
        }
    }
    
    public class IsNotNullNodeViewModelBase : Invert.Core.GraphDesigner.GenericNodeViewModel<IsNotNullNode> {
        
        public IsNotNullNodeViewModelBase(IsNotNullNode graphItemObject, Invert.Core.GraphDesigner.DiagramViewModel diagramViewModel) : 
                base(graphItemObject, diagramViewModel) {
        }
    }
    
    public class ActionNodeViewModelBase : Invert.Core.GraphDesigner.GenericNodeViewModel<ActionNode> {
        
        public ActionNodeViewModelBase(ActionNode graphItemObject, Invert.Core.GraphDesigner.DiagramViewModel diagramViewModel) : 
                base(graphItemObject, diagramViewModel) {
        }
    }
    
    public class MatcherNodeViewModelBase : ConditionNodeViewModel {
        
        public MatcherNodeViewModelBase(MatcherNode graphItemObject, Invert.Core.GraphDesigner.DiagramViewModel diagramViewModel) : 
                base(graphItemObject, diagramViewModel) {
        }
    }
    
    public class MatchAllNodeViewModelBase : MatcherNodeViewModel {
        
        public MatchAllNodeViewModelBase(MatchAllNode graphItemObject, Invert.Core.GraphDesigner.DiagramViewModel diagramViewModel) : 
                base(graphItemObject, diagramViewModel) {
        }
    }
    
    public class MatchAnyNodeViewModelBase : MatcherNodeViewModel {
        
        public MatchAnyNodeViewModelBase(MatchAnyNode graphItemObject, Invert.Core.GraphDesigner.DiagramViewModel diagramViewModel) : 
                base(graphItemObject, diagramViewModel) {
        }
    }
    
    public class CustomMatcherNodeViewModelBase : MatcherNodeViewModel {
        
        public CustomMatcherNodeViewModelBase(CustomMatcherNode graphItemObject, Invert.Core.GraphDesigner.DiagramViewModel diagramViewModel) : 
                base(graphItemObject, diagramViewModel) {
        }
    }
    
    public class OnEventNodeViewModelBase : EventNodeViewModel {
        
        public OnEventNodeViewModelBase(OnEventNode graphItemObject, Invert.Core.GraphDesigner.DiagramViewModel diagramViewModel) : 
                base(graphItemObject, diagramViewModel) {
        }
    }
}
