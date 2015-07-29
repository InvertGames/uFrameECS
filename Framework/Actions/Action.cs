using System;
using JetBrains.Annotations;
using uFrame.Actions.Attributes;
using uFrame.Attributes;
using uFrame.ECS;
using uFrame.Kernel;
using UniRx;
using UnityEngine;

namespace uFrame.Actions
{
    [ActionLibrary, uFrameCategory("Condition", "Floats")]
    public static class Comparisons
    {
        [ActionTitle("Compare Floats")]
        public static bool CompareFloats(float a, float b)
        {
            return a == b;
        }

        [ActionTitle("Less Than")]
        public static bool LessThan(float a, float b, Action yes, Action no)
        {
            if (a < b)
            {
                if (yes != null) yes();
                return true;
            }
            if (no != null) no();
            return false;
        }

        [ActionTitle("Less Than Or Equal")]
        public static bool LessThanOrEqual(float a, float b, Action yes, Action no)
        {
            if (a <= b)
            {
                if (yes != null) yes();
                return true;
            }
            if (no != null) no();
            return false;
        }

        [ActionTitle("Greater Than")]
        public static bool GreaterThan(float a, float b, Action yes, Action no)
        {
            if (a > b)
            {
                if (yes != null) yes();
                return true;
            }
            if (no != null) no();
            return false;
        }

        [ActionTitle("Greater Than Or Equal")]
        public static bool GreaterThanOrEqual(float a, float b, Action yes, Action no)
        {
            if (a >= b)
            {
                if (yes != null) yes();
                return true;
            }
            if (no != null) no();
            return false;
        }

        [ActionTitle("Equal")]
        public static bool AreEqual(object a, object b, Action yes, Action no)
        {
            var result = false;
            if ((a == null || b == null))
            {
                result = a == b;
                if (yes != null) yes();
            }
            else
            {
                if (a.Equals(b))
                {
                    if (yes != null) yes();
                }
                else
                {
                    if (no != null)
                    {
                        no();
                    }
                }
            }

            return result;
        }

    }
    [ActionLibrary, uFrameCategory("Condition", "Floats")]
    public static class DebugLibrary
    {
        [ActionTitle("Log Message")]
        public static void LogMessage(object message)
        {
            UnityEngine.Debug.Log(message);
        }


    }

    [ActionLibrary, uFrameCategory("Set")]
    public static class SetLibrary
    {
        [ActionTitle("Set Value")]
        public static void SetValue( ref object a, object value)
        {
            if (a == null) throw new ArgumentNullException("a");
            a = value;
        }
    }

    [ActionLibrary, uFrameCategory("Random")]
    public static class CreateRandoms
    {
         [ActionTitle("Random Vector3")]
        public static Vector3 RandomVector3(int minX, int maxX, int minY, int maxY, int minZ, int maxZ)
        {
            return new Vector3(
                UnityEngine.Random.Range(minX, maxX),
                UnityEngine.Random.Range(minY, maxY),
                UnityEngine.Random.Range(minZ, maxZ)
                );
        }
         [ActionTitle("Random Vector2")]
        public static Vector2 RandomVector2(int minX, int maxX, int minY, int maxY)
        {
            return new Vector2(
                UnityEngine.Random.Range(minX, maxX),
                UnityEngine.Random.Range(minY, maxY)
                );
        }
         [ActionTitle("Random Float")]
        public static float RandomFloat(float min, float max)
        {
            return UnityEngine.Random.Range(min, max);
        }
         [ActionTitle("Random Int")]
        public static int RandomInt(int min, int max)
        {
            return UnityEngine.Random.Range(min, max);
        }
         [ActionTitle("Random Bool")]
        public static bool RandomBool()
        {
            return UnityEngine.Random.Range(0, 2) == 1;
        }
    }

    [ActionLibrary, uFrameCategory("Destroy", "Component", "Entity")]
    public static class DestroyLibrary
    {
         [ActionTitle("Destroy Component")]
        public static void DestroyComponent(MonoBehaviour behaviour)
        {
            UnityEngine.Object.Destroy(behaviour);
        }
         [ActionTitle("Destroy Entity")]
        public static void DestroyEntity(int entityId)
        {
            UnityEngine.Object.Destroy(EntityService.GetEntityView(entityId).gameObject);
        }

         [ActionTitle("Destroy Timer")]
         public static void DestroyTimer(IDisposable timer)
         {
             timer.Dispose();
         }
    }

    [ActionLibrary, uFrameCategory("Vector3")]
    public static class Vector3Library
    {


        [ActionTitle("Get Indices")]
        public static void GetIndices(Vector3 vector,out float x, out float y, out float z)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }
        [ActionTitle("Get X")]
        public static float GetX(Vector3 vector)
        {
            return vector.x;
        }
        
        [ActionTitle("Get Y")]
        public static float GetY(Vector3 vector)
        {
            return vector.y;
        }
        
        [ActionTitle("Get Z")]
        public static float GetZ(Vector3 vector)
        {
            return vector.z;
        }

        [ActionTitle("Create Vector3")]
        public static Vector3 Create(float x, float y, float z)
        {
            return new Vector3(x,y,z);
        }
    }



    [ActionLibrary, uFrameCategory("Vector2")]
    public static class Vector2Library
    {
        [ActionTitle("Get X")]
        public static float GetX(Vector2 vector)
        {
            return vector.x;
        }
        [ActionTitle("Get Y")]
        public static float GetY(Vector2 vector)
        {
            return vector.y;
        }
    
    }

    [ActionLibrary, uFrameCategory("GameObject")]
    public static class GameObjects
    {
        [ActionTitle("Deactivate GameObject")]
        public static void DeactiateGameObject(GameObject gameObject, MonoBehaviour behaviour)
        {
            if (gameObject != null)
            {
                gameObject.SetActive(false);
                return;
            }
            if (behaviour != null)
            {
                behaviour.gameObject.SetActive(false);
            }
        }
        [ActionTitle("Activate GameObject")]
        public static void ActivateGameObject(GameObject gameObject, MonoBehaviour behaviour)
        {
            if (gameObject != null)
            {
                gameObject.SetActive(true);
                return;
            }
            if (behaviour != null)
            {
                behaviour.gameObject.SetActive(true);
            }
        }
    }

    [ActionLibrary, uFrameCategory("Transform","Entity")]
    public static class EntityTransform
    {
        [ActionTitle("Set Position")]
        public static void SetPosition(Entity entity, Vector3 position)
        {
            entity.transform.position = position;
        }

        [ActionTitle("Set Rotation")]
        public static void SetRotation(Entity entity, Vector3 rotation)
        {
            entity.transform.rotation = Quaternion.Euler(rotation);
        }
        [ActionTitle("Set Local Position")]
        public static void SetLocalPosition(Entity entity, Vector3 position)
        {
            entity.transform.localPosition = position;
        }

        [ActionTitle("Set Local Rotation")]
        public static void SetLocalRotation(Entity entity, Vector3 rotation)
        {
            entity.transform.localRotation = Quaternion.Euler(rotation);
        }

        [ActionTitle("Set Scale")]
        public static void SetScale(Entity entity, Vector3 scale)
        {
            entity.transform.localScale = scale;
        }


        [ActionTitle("Get Position")]
        public static Vector3 GetPosition(Entity entity)
        {
            return entity.transform.position;
        }

        [ActionTitle("Get Rotation")]
        public static Vector3 GetRotation(Entity entity)
        {
            return entity.transform.eulerAngles;
        }
        [ActionTitle("Get Local Position")]
        public static Vector3 GetLocalPosition(Entity entity)
        {
            return entity.transform.localPosition;
        }

        [ActionTitle("Get Local Rotation")]
        public static Vector3 GetLocalRotation(Entity entity)
        {
            return entity.transform.localEulerAngles;
        }

        [ActionTitle("Get Local Scale")]
        public static Vector3 GetLocalScale(Entity entity)
        {
            return entity.transform.localScale;
        }
    }

    public abstract class UFAction
    {
        public Entity EntityView;
        public EcsSystem System;

        public virtual bool Execute()
        {
            return false;
        }

    }

    [ActionLibrary, uFrameCategory("Debug")]
    public class Log : UFAction
    {
        [In] public string Message;
        public override bool Execute()
        {
            Debug.Log(Message);
            return true;
        }
    }

    

    [ActionTitle("Movement/Move Over Time")]
    public class MoveInDirectionOverTime : UFAction
    {
        [In] public EcsComponent Component;
        [In] public float Speed;
        [In] public Vector3 Direction;

        public override bool Execute()
        {
            Component.CachedTransform.position += (Direction*Speed)*Time.deltaTime;
            return true;
        }
    }

    
    [ActionTitle("Timer"),uFrameCategory("Timers")]
    public class Timer : UFAction
    {
        [In] public int Minutes;
        [In] public int Seconds;

        [Out] public Action Complete;
        public override bool Execute()
        {
            
            Observable.Timer(new TimeSpan(0, 0, Minutes, Seconds, 0)).Subscribe(_ =>
            {
                Complete();
            }).DisposeWith(System);
            return true;
        }
    }

    [ActionTitle("Interval"),uFrameCategory("Timers")]
    public class Interval : UFAction
    {
        [In]
        public int Minutes;
        [In]
        public int Seconds;
        [Out]
        public Action Tick;
        [Out]
        public IDisposable Result;

        public override bool Execute()
        {
            Result = Observable.Interval(new TimeSpan(0, 0, Minutes, Seconds, 0)).Subscribe(_ =>
            {
                Tick();
            }).DisposeWith(System);
            return true;
        }
    }
}

namespace uFrame.Actions.Attributes
{



}