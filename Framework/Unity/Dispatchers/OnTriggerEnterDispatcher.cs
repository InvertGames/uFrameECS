using uFrame.Attributes;
using UnityEngine;

namespace uFrame.ECS
{
    [UFrameEventDispatcher("On Trigger Enter"), uFrameCategory("Unity Messages")]
    public class OnTriggerEnterDispatcher : EcsDispatcher
    {
        public int ColliderId { get; set; }
        public void OnTriggerEnter(Collider coll)
        {

            var colliderEntity = coll.gameObject.GetComponent<Entity>();
            if (colliderEntity == null) return;
            ColliderId = colliderEntity.EntityId;
            EntityId = gameObject.GetComponent<Entity>().EntityId;
            Publish(this);
        }
    }
}