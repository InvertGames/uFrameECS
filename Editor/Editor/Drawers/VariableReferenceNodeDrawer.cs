using UnityEngine;

namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core.GraphDesigner;
    
    
    public class VariableReferenceNodeDrawer : GenericNodeDrawer<VariableReferenceNode,VariableReferenceNodeViewModel> {
        
        public VariableReferenceNodeDrawer(VariableReferenceNodeViewModel viewModel) : 
                base(viewModel) {
        }

        public override Vector2 HeaderPadding
        {
            get { return Vector2.zero; }
        }
        public override float MinWidth
        {
            get { return 50f; }
        }

        public override object BackgroundStyle
        {
            get { return CachedStyles.BoxHighlighter6; }
        }

        //public override object HeaderTextStyle
        //{
        //    get { return CachedStyles.ItemTextEditingStyle; }
        //}

        protected override object HeaderStyle
        {
            get { return null; }
        }
    }
}
