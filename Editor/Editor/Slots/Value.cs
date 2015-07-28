using System.Runtime.InteropServices;

namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core.GraphDesigner;
    
    
    public class Value : ValueBase, IActionIn {
        private ActionFieldInfo _fieldInfo;

        public ActionFieldInfo ActionFieldInfo
        {
            get
            {
                return _fieldInfo ?? (_fieldInfo = new ActionFieldInfo()
                {
                    Name = this.Name,
                    Type = typeof(object)
                });
            }
            set
            {
                
            }
        }

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
    
    public partial interface IValueConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
