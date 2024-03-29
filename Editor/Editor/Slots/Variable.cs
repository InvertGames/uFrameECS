namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core.GraphDesigner;
    
    
    public class Variable : VariableBase {
        public ActionFieldInfo ActionFieldInfo { get; set; }
        public string VariableName
        {
            get
            {
                var actionNode = Node as SetVariableNode;
                var str = actionNode.Name + "_";
         
                return str + Name;
            }
            set
            {

            }
        }
    }
    
    public partial interface IVariableConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
