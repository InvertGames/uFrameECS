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
        public static int VariableCount;
        private string _variableName;

        public static string GetNewVariable(string prefix)
        {
            return string.Format("{0}{1}",prefix, VariableCount++);
        }

        public override string Name
        {
            get { return VariableType + " Variable"; } 
            set { base.Name = value; }
        }

        public virtual ITypeInfo VariableType
        {
            get { return new SystemTypeInfo(typeof(object)); }
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
            return new CodeVariableDeclarationStatement(VariableType.FullName.ToCodeReference(), VariableName, GetCreateExpression());
        }
        public virtual CodeMemberField GetFieldStatement()
        {
            return new CodeMemberField(VariableType.FullName.ToCodeReference(), VariableName)
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
                if (string.IsNullOrEmpty(_variableName))
                {
                    _variableName = GetNewVariable(this.Name);
                }
                return _variableName;
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
