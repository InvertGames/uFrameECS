using uFrame.Actions;
using uFrame.Attributes;

namespace uFrame.ECS
{
    [ActionTitle("Spawn")]
    public class Despawn : UFAction
    {
        [In] public int EntityId;
        [In] public Entity Entity;

        public override bool Execute()
        {
            System.Publish(new DespawnEntity()
            {
                Entity = Entity,
                EntityId = EntityId
            });
            return base.Execute();
        }
    }
}