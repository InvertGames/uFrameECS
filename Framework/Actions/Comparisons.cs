using System;
using uFrame.Actions.Attributes;
using uFrame.Attributes;
using UnityEngine;

namespace uFrame.Actions
{
    [ActionLibrary, uFrameCategory("Condition")]
    public static class Comparisons
    {
        public static void InputTest(KeyCode code)
        {
            
        }
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
        [ActionTitle("GetRigidBody")]
        public static Rigidbody GetRigidBody(MonoBehaviour behaviour)
        {
            return behaviour.GetComponent<Rigidbody>();
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
}

namespace uFrame.Actions.Attributes
{



}