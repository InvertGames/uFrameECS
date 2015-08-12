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
    [UFrameEventDispatcher("On Collision Enter")]
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

    [UFrameEventDispatcher("On Collision Exit")]
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

    [UFrameEventDispatcher("On Collision Stay")]
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

    [UFrameEventDispatcher("On Trigger Enter")]
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

    [UFrameEventDispatcher("On Trigger Exit")]
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

    [UFrameEventDispatcher("On Trigger Stay")]
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