using uFrame.ECS;

namespace uFrame.Actions
{
    public abstract class UFAction
    {
        public Entity EntityView;
        public EcsSystem System;

        public virtual bool Execute()
        {
            return false;
        }

    }
}