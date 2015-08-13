using uFrame.Attributes;
using UnityEngine;

namespace uFrame.ECS
{
    [UFrameEventDispatcher("On Collision Enter 2D"), uFrameCategory("Unity Messages")]
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
    [UFrameEventDispatcher("On Collision Enter"), uFrameCategory("Unity Messages")]
    public class OnCollisionEnterDispatcher : EcsDispatcher
    {
        public int ColliderId { get; set; }
        public void OnCollisionEnter(Collision coll)
        {

            var colliderEntity = coll.collider.gameObject.GetComponent<Entity>();
            if (colliderEntity == null) return;
            ColliderId = colliderEntity.EntityId;
            EntityId = gameObject.GetComponent<Entity>().EntityId;
            Publish(this);
        }
    }

    [UFrameEventDispatcher("On Collision Exit"), uFrameCategory("Unity Messages")]
    public class OnCollisionExitDispatcher : EcsDispatcher
    {
        public int ColliderId { get; set; }
        public void OnCollisionExit(Collision coll)
        {

            var colliderEntity = coll.collider.gameObject.GetComponent<Entity>();
            if (colliderEntity == null) return;
            ColliderId = colliderEntity.EntityId;
            EntityId = gameObject.GetComponent<Entity>().EntityId;
            Publish(this);
        }
    }

    [UFrameEventDispatcher("On Collision Stay"), uFrameCategory("Unity Messages")]
    public class OnCollisionStayDispatcher : EcsDispatcher
    {
        public int ColliderId { get; set; }
        public void OnCollisionStay(Collision coll)
        {

            var colliderEntity = coll.collider.gameObject.GetComponent<Entity>();
            if (colliderEntity == null) return;
            ColliderId = colliderEntity.EntityId;
            EntityId = gameObject.GetComponent<Entity>().EntityId;
            Publish(this);
        }
    }

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

    [UFrameEventDispatcher("On Trigger Exit"), uFrameCategory("Unity Messages")]
    public class OnTriggerExitDispatcher : EcsDispatcher
    {
        public int ColliderId { get; set; }
        public void OnTriggerExit(Collider coll)
        {

            var colliderEntity = coll.gameObject.GetComponent<Entity>();
            if (colliderEntity == null) return;
            ColliderId = colliderEntity.EntityId;
            EntityId = gameObject.GetComponent<Entity>().EntityId;
            Publish(this);
        }
    }

    [UFrameEventDispatcher("On Trigger Stay"), uFrameCategory("Unity Messages")]
    public class OnTriggerStayDispatcher : EcsDispatcher
    {
        public int ColliderId { get; set; }
        public void OnTriggerStay(Collider coll)
        {

            var colliderEntity = coll.gameObject.GetComponent<Entity>();
            if (colliderEntity == null) return;
            ColliderId = colliderEntity.EntityId;
            EntityId = gameObject.GetComponent<Entity>().EntityId;
            Publish(this);
        }
    }
}