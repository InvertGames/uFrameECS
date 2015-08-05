using System.CodeDom;
using Invert.Json;

namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;
    
    
    public class Vector3Node : Vector3NodeBase {
        [NodeProperty,JsonProperty]
        public float X { get; set; }
        [NodeProperty, JsonProperty]
        public float Y { get; set; }
        [NodeProperty, JsonProperty]
        public float Z { get; set; }

        public override string VariableType
        {
            get { return typeof(UnityEngine.Vector3).Name; }
        }

        public override CodeExpression GetCreateExpression()
        {
            return new CodeSnippetExpression(string.Format("new UnityEngine.Vector3({0}f,{1}f,{2}f)", X, Y, Z));
        }
    }
    
    public partial interface IVector3Connectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
