using System.CodeDom;

namespace Invert.uFrame.ECS
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;


    public class VariableNode : VariableNodeBase, IContextVariable , ITypedItem
    {
        public virtual string VariableType
        {
            get { return typeof(int).Name; }
        }

        public virtual CodeVariableDeclarationStatement GetDeclerationStatement()
        {
            return new CodeVariableDeclarationStatement(VariableType, VariableName, GetCreateExpression());
        }

        public virtual CodeExpression GetCreateExpression()
        {
            return null;
        }

        public string ShortName
        {
            get { throw new NotImplementedException(); }
        }

        public ITypedItem SourceVariable
        {
            get { return this; }
            set
            {

            }
        }

        public string VariableName
        {
            get
            {
                return Name;
            }
            set { Name = value; }
        }

        public string AsParameter
        {
            get { return Name; }
        }

        public bool IsSubVariable { get; set; }
    }

    public partial interface IVariableConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable
    {
    }
}
