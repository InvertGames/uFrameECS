using System.CodeDom;
using Invert.ICSharpCode.NRefactory.CSharp.Statements;
using Invert.Json;
using uFrame.Actions.Attributes;

namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;

    public interface IContextVariable : IDiagramNodeItem
    {
        //IEnumerable<IContextVariable> Members { get; }
        string ShortName { get; }
        ITypedItem SourceVariable { get; set; }
        string VariableName { get; set; }
        string AsParameter { get; }
        bool IsSubVariable { get; set; }
        string VariableType { get; }
    }

    public interface IVariableExpressionItem
    {
        string Expression { get; set; }
    }

    
    public class ContextVariable : GenericTypedChildItem, IContextVariable
    {

        private string _memberExpression;
        
        public override string Identifier
        {
            get { return Node.Identifier + ":" + MemberExpression; }
            set {  }
        }

        public ContextVariable(params object[] items)
        {
            Items = items;
        } 

        public object[] Items { get; set; }
        public override string ToString()
        {
            return MemberExpression;
        }

        public string MemberExpression
        {
            get
            {
                return _memberExpression ?? (_memberExpression = string.Join(".", Items.Select(p =>
                {
                    var cv = p as IDiagramNodeItem;
                    if (cv != null) return cv.Name;
                    return (string)p;
                }).ToArray()));
            }
        }

        public override string Title
        {
            get { return MemberExpression; }
        }

        public override string Group
        {
            get
            {
                if (Items.Length < 1)
                    return "Missing";

                var item = Items.Length > 1 ? Items[Items.Length - 2] : Items.Last();
                var cv = item as IDiagramNodeItem;
                if (cv != null)
                {
                    return cv.Name;
                }
                return (string)item;
            }
        }

        public override string SearchTag
        {
            get { return MemberExpression; }
        }

        public override string Name
        {
            get { return VariableName; }
            set { base.Name = value; }
        }

        public string VariableName
        {
            get { return MemberExpression; }
            set { }
        }

        public string AsParameter
        {
            get { return Items.Last().ToString().ToLower(); }
        }

        public bool IsSubVariable { get; set; }

        public string VariableType
        {
            get { return SourceVariable.RelatedTypeName; }
        }

        public string ShortName
        {
            get { return Items.Last() as string; }
        }

        public ITypedItem SourceVariable { get; set; }
    }
    public interface ICodeOutput
    {
        IEnumerable<IContextVariable> AllContextVariables { get; }
        IEnumerable<IContextVariable> ContextVariables { get; }

        void WriteCode(TemplateContext ctx);
    }
    public class ActionNode : ActionNodeBase, ICodeOutput {
        public override IEnumerable<IGraphItem> GraphItems
        {
            get 
            {
                foreach (var item in InputVars)
                    yield return item;
                foreach (var item in OutputVars)
                    yield return item;
            }
        }

        public string VarName
        {
            get
            {
                var result = string.Empty;
                for (int index = 0; index < Identifier.Length; index++)
                {
                    char c = Identifier[index];

                    if (Char.IsLetter(c))
                        result += c;
                }
                return result;
            }
        }
        
        public override void WriteCode(TemplateContext ctx)
        {
            base.WriteCode(ctx);
            CodeVariableDeclarationStatement varStatement = new CodeVariableDeclarationStatement(Meta.Type, VarName, new CodeObjectCreateExpression(Meta.Type));
            ctx.CurrentStatements.Add(varStatement);

            foreach (var item in GraphItems.OfType<IActionItem>())
            {
                var decl = item.InputFrom<VariableNode>();
                if (decl == null) continue;

                ctx.CurrentStatements.Add(decl.GetDeclerationStatement());
            }

            foreach (var item in InputVars)
            {
                var contextVariable = item.InputFrom<IContextVariable>();
                if (contextVariable == null) continue;
                ctx._("{0}.{1} = {2}", varStatement.Name, item.Name,contextVariable.Name);
                
            }
            ctx._if("!{0}.Execute()", varStatement.Name).TrueStatements._("return");
            foreach (var item in OutputVars)
            {
                var contextVariable = item.OutputTo<IContextVariable>();
                if (contextVariable == null) continue;
                ctx._("{0} = {1}.{2}", contextVariable.Name, varStatement.Name, item.Name);

            }
        }

        private ActionMetaInfo _meta;
        private string _metaType;
        private IVariableInput[] _inputVars;
        private IVariableOutput[] _outputVars;

        public ActionMetaInfo Meta
        {
            get
            {
                if (string.IsNullOrEmpty(MetaType))
                    return null;
                return _meta ??(_meta = ActionsPlugin.Actions[MetaType]);
            }
            set
            {
                _meta = value;
                _metaType = value.Type.FullName;
            }
        }
         
        [JsonProperty]
        public string MetaType
        {
            get { return _metaType; }
            set
            {
                _metaType = value;
            
                
            }
        }


        public IVariableInput[] InputVars
        {
            get { return _inputVars ?? (_inputVars = GetInputVars().ToArray()); }
            set { _inputVars = value; }
        }

        private IEnumerable<IVariableInput> GetInputVars()
        {
            var meta = Meta;
            if (meta != null)
            {
                foreach (var item in Meta.ActionFields.Where(p => p.DisplayType is VarIn))
                {
                    var variableIn = new VariableIn()
                    {
                        ActionFieldInfo = item,
                        Node = this,
                        Identifier = this.Identifier + ":" + meta.Type.Name + ":" + item.Name
                    };
                    yield return variableIn;
                }
            }
        }

        public IVariableOutput[] OutputVars
        {
            get { return _outputVars ?? (_outputVars = GetOutputVars().ToArray()); }
            set { _outputVars = value; }
        }

        private IEnumerable<IVariableOutput> GetOutputVars()
        {
            var meta = Meta;
            if (meta != null)
            {
                foreach (var item in Meta.ActionFields.Where(p => p.DisplayType is VarOut))
                {
                    var variableOut = new VariableOut()
                    {
                        ActionFieldInfo = item,
                        Node = this,
                        Identifier = this.Identifier + ":" + meta.Type.Name + ":" + item.Name
                    };
                    yield return variableOut;
                }
            }
        }




    }
    
    public partial interface IActionConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }

    public interface IActionItem : IDiagramNodeItem
    {
        ActionFieldInfo ActionFieldInfo { get; set; }
    }
    public interface IVariableInput : IActionItem
    {
        
    }

    public interface IVariableOutput : IActionItem
    {
        
    }

    public class VariableIn : SingleInputSlot<IContextVariable>, IVariableInput
    {
        public ActionFieldInfo ActionFieldInfo { get; set; }

        public override string Name
        {
            get { return ActionFieldInfo.Name; }
            set { base.Name = value; }
        }
    }

    public class VariableOut : SingleOutputSlot<IContextVariable>, IVariableOutput
    {
        public ActionFieldInfo ActionFieldInfo { get; set; }
        public override string Name
        {
            get { return ActionFieldInfo.Name; }
            set { base.Name = value; }
        }
    }
}
