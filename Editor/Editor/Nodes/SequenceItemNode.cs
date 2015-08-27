using System.CodeDom;
using Invert.Json;
using UnityEngine;

namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;
    using Invert.Data;

    public class SequenceItemNode : SequenceItemNodeBase, ICodeOutput
    {
        public override bool AllowMultipleInputs
        {
            get { return false; }
        }

        public override bool AllowMultipleOutputs
        {
            get { return false; }
        }

        public override void RecordRemoved(IDataRecord record)
        {
            base.RecordRemoved(record);
            var container = this.Container();
            if (container == null || container.Identifier == record.Identifier)
            {
                Repository.Remove(this);
            }
          
        }

        public IVariableContextProvider Left
        {
            get
            {
                var r = this.InputFrom<IVariableContextProvider>();
                return r;
            }
        }
        public IEnumerable<IVariableContextProvider> LeftNodes
        {
            get
            {
                var left = Left;
                while (left != null)
                {
                    yield return left;
                    left = left.Left;
                }
            }
        }
        public IEnumerable<IVariableContextProvider> RightNodes
        {
            get
            {
                var right = Right;
                while (right != null)
                {
                    yield return right;
                    right = right.Right;
                }
            }
        } 
        public SequenceItemNode Right
        {
            get { return this.OutputTo<SequenceItemNode>(); }
        }

        public IEnumerable<IContextVariable> GetAllContextVariables()
        {
            var left = Left;
            if (left != null)
            {
                foreach (var contextVar in left.GetAllContextVariables())
                {
                    yield return contextVar;
                }
            }
            foreach (var item in GetContextVariables())
            {
                yield return item;
            }
        }

        public virtual IEnumerable<IContextVariable> GetContextVariables()
        {
            yield break;
        }
        
        public virtual void WriteCode(TemplateContext ctx)
        {
            OutputVariables(ctx);
            ctx._comment(Name);
            foreach (var right in this.OutputsTo<SequenceItemNode>())
            {
                if (right != null)
                {
                    right.WriteCode(ctx);
                }
            }
            
        }

        protected void OutputVariables(TemplateContext ctx)
        {
            foreach (var item in GraphItems.OfType<IConnectable>())
            {
                var decl = item.InputFrom<VariableNode>();
                if (decl == null) continue;

                ctx.CurrentDeclaration.Members.Add(decl.GetFieldStatement());
            }
     
        }

        //public override IEnumerable<IDiagramNode> FilterNodes
        //{
        //    get
        //    {
        //        yield return this;
        //        foreach (
        //            var item in
        //                Repository.AllOf<SequenceItemNode>()
        //                    .Where(p => p.FilterId == this.Identifier)
        //                    .OfType<IDiagramNode>())
        //        {
        //            yield return item; 
        //        } 

        //    }
        //}

        //public override IEnumerable<IFilterItem> FilterItems
        //{
        //    get { return FilterNodes.OfType<IFilterItem>(); }
        //}
    }
    
    public partial interface ISequenceItemConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
