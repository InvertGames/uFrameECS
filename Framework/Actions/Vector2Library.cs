using uFrame.Attributes;
using UnityEngine;

namespace uFrame.Actions
{
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
}