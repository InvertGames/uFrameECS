using uFrame.Attributes;
using UnityEngine;

namespace uFrame.Actions
{
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
}