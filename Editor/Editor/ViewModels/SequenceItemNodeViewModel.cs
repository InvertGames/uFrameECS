using Invert.Core.GraphDesigner;

namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    
    
    public class SequenceItemNodeViewModel : SequenceItemNodeViewModelBase {
        
        public SequenceItemNodeViewModel(SequenceItemNode graphItemObject, Invert.Core.GraphDesigner.DiagramViewModel diagramViewModel) : 
                base(graphItemObject, diagramViewModel) {
        }
        public SequenceItemNode SequenceNode
        {
            get { return GraphItem as SequenceItemNode; }
        }
        protected override void CreateContent()
        {
            //HashSet<string> usedIds = new HashSet<string>();
            //foreach (var item in SequenceNode.PersistedItems.OfType<UsedVariable>())
            //{
            //    ContentItems.Add(new ItemViewModel<IContextVariable>(this)
            //    {
                    
            //        DataObject = item,
                    
            //        IsNewLine = true,
            //        OutputConnectorType = typeof(IContextVariable),
            //        InputConnectorType = typeof(IContextVariable)
            //    });
            //    //ContentItems.Add(new InputOutputViewModel()
            //    //{
            //    //    IsOutput = true,
            //    //    DataObject = item,
            //    //    IsNewLine = true,
            //    //    OutputConnectorType = typeof(IContextVariable),
            //    //    Name = item.Name
            //    //});
            //    usedIds.Add(item.Identifier);
            //}
            //foreach (var item in SequenceNode.AllContextVariables)
            //{
                
            //    if (usedIds.Contains(item.Identifier)) continue;
            //    ContentItems.Add(new ItemViewModel<IContextVariable>(this)
            //    {
            //        DataObject = item,
            //        IsNewLine = true,
            //        OutputConnectorType = typeof(IContextVariable),
            //        InputConnectorType = typeof(IContextVariable)
            //    });
            //    //ContentItems.Add(new InputOutputViewModel()
            //    //{
            //    //    IsOutput = true,
            //    //    DataObject = item,
            //    //    IsNewLine = true,
            //    //    OutputConnectorType = typeof(IContextVariable),
            //    //    Name = item.Name
            //    //});
            //}

            //if (IsVisible(SectionVisibility.WhenNodeIsNotFilter) && HandlerNode.EventNode != null && !HandlerNode.EventNode.NeedsMappings) return;
            base.CreateContent();


        }
    }
}
