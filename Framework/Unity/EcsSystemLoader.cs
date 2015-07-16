using System.Collections.Generic;
using uFrame.Actions.Attributes;
using uFrame.Kernel;

namespace uFrame.ECS
{
    public class EcsSystemLoader : SystemLoader
    {
        public override void Load()
        {
            base.Load();
            var componentSystem = new UnityComponentSystem();
            Container.RegisterInstance<IComponentSystem>(componentSystem);
            var entityManager = componentSystem.RegisterComponent<Entity>();
            Container.RegisterInstance(entityManager);

        }
    }

    public class EntityService : SystemServiceMonoBehavior
    {
        public static int LastUsedId
        {
            get { return _lastUsedId; }
            set { _lastUsedId = value; }
        }
        private static int _lastUsedId = 0;

        private static Dictionary<int, Entity> _entities = new Dictionary<int, Entity>();

        public static Dictionary<int, Entity> Entities
        {
            get { return _entities; }
            set { _entities = value; }
        }

        public static int NewId()
        {
            _lastUsedId--;
            return _lastUsedId;
        }

        public static Entity GetEntityView(int entityId)
        {
            if (Entities.ContainsKey(entityId))
                return Entities[entityId];
            return null;
        }

        public static void DestroyEntity(int entityId)
        {
            if (Entities.ContainsKey(entityId))
                Destroy(Entities[entityId]);
        }
        public static void RegisterEntityView(Entity entity)
        {
            if (!Entities.ContainsKey(entity.EntityId))
            {
                Entities.Add(entity.EntityId, entity);
            }
        }

        public static void UnRegisterEntityView(Entity entity)
        {
            Entities.Remove(entity.EntityId);
        }
    }

    [EventDispatcher("On Mouse Down")]
    public class MouseDownDispatcher : EcsDispatcher
    {
        public void OnMouseDown()
        {
            Publish(this);
        }
    }
    [EventDispatcher("On Mouse Up")]
    public class MouseUpDispatcher : EcsDispatcher
    {
        public void OnMouseUp()
        {
            Publish(this);
        }
    }
}