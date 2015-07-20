using uFrame.Attributes;
using UnityEngine;

namespace uFrame.ECS
{
    [UFrameEventDispatcher("On Collision Enter 2D")]
    public class OnCollisionEnter2DDispatcher : EcsDispatcher
    {
        public int ColliderId { get; set; }
        public void OnCollisionEnter2D(Collision2D coll)
        {

            var colliderEntity = coll.collider.gameObject.GetComponent<Entity>();
            if (colliderEntity == null) return;
            ColliderId = colliderEntity.EntityId;
            EntityId = gameObject.GetComponent<Entity>().EntityId;
            Publish(this);
        }
    }
}