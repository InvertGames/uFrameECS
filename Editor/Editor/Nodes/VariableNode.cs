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
        public virtual object VariableType
        {
            get { return typeof(int).Name; }
        }

        public virtual string ValueExpression
        {
            get { return string.Empty; }
        }

        public IEnumerable<IContextVariable> GetPropertyDescriptions()
        {yield break;
        }

        public virtual CodeVariableDeclarationStatement GetDeclerationStatement()
        {
            return new CodeVariableDeclarationStatement(VariableType.ToCodeReference(), VariableName, GetCreateExpression());
        }
        public virtual CodeMemberField GetFieldStatement()
        {
            return new CodeMemberField(VariableType.ToCodeReference(), VariableName)
            {
                InitExpression = GetCreateExpression()
            };
        }
        public virtual CodeExpression GetCreateExpression()
        {
            return null;
        }

        public string ShortName
        {
            get { return Name; }
        }

     

        public ITypedItem Source
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
