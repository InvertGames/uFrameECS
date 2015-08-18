using System;
using uFrame.Attributes;
using uFrame.ECS;

namespace uFrame.Actions
{
    [uFrame.Attributes.ActionTitle("Loop Entities"), uFrameCategory("Loops", "Entities")]
    public class LoopEntities : UFAction
    {
        [In] 
        public IEcsComponentManager Group;

        [Out] 
        public IEcsComponent Item;

        [Out]
        public Action Continue { get; set; }

        public override bool Execute()
        {
            foreach (var item in Group.All)
            {
                Item = item;
                Continue();
            }
            return base.Execute();
        }
    }
}