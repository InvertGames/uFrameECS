using System;
using uFrame.Actions.Attributes;
using uFrame.Attributes;
using uFrame.ECS;
using UnityEngine;

namespace uFrame.Actions
{
    [ActionLibrary, uFrameCategory("Condition")]
    public static class Comparisons
    {
        [ActionTitle("Is True")]
        public static void IsTrue(bool value, Action yes, Action no)
        {
            if (value)
                if (yes != null) yes();
                else
                {
                    if (no != null) no();
                }
        }
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

    [ActionLibrary, uFrameCategory("Components")]
    public static class UnityLibrary
    {
  
        [ActionTitle("Get Unity Component")]
        public static Type GetUnityComponent<Type>(GameObject go, MonoBehaviour component)
        {
            if (component == null)
                return go.GetComponent<Type>();
            return component.GetComponent<Type>();
        }
        [ActionTitle("Get Rigidbody")]
        public static Rigidbody GetRigidbody(GameObject go, MonoBehaviour component)
        {
            return GetUnityComponent<Rigidbody>(go, component);
        }
        [ActionTitle("Get Rigidbody2D")]
        public static Rigidbody2D GetRigidbody2D(GameObject go, MonoBehaviour component)
        {
            return GetUnityComponent<Rigidbody2D>(go, component);
        }
        [ActionTitle("Get Collider 2D")]
        public static Collider2D GetCollider2D(GameObject go, MonoBehaviour component)
        {
            return GetUnityComponent<Collider2D>(go, component);
        }
        [ActionTitle("Get Collider")]
        public static Collider GetCollider(GameObject go, MonoBehaviour component)
        {
            return GetUnityComponent<Collider>(go, component);
        }
        [ActionTitle("Get Camera")]
        public static Camera GetCamera(GameObject go, MonoBehaviour component)
        {
            return GetUnityComponent<Camera>(go, component);
        }
        [ActionTitle("Get Main Camera")]
        public static Camera GetMainCamera()
        {
            return Camera.main;
        }
    }
}

namespace uFrame.Actions.Attributes
{



}