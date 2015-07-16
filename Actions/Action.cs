using System;
using uFrame.Actions.Attributes;
using uFrame.ECS;
using uFrame.Kernel;
using UniRx;
using UnityEngine;

namespace uFrame.Actions
{
    [ActionLibrary]
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


    }



    [ActionLibrary]
    public static class Vector3Library
    {
        [ActionTitle("Vector3/Get X")]
        public static float GetX(Vector3 vector)
        {
            return vector.x;
        }
        [ActionTitle("Vector3/Get Y")]
        public static float GetY(Vector3 vector)
        {
            return vector.y;
        }
        [ActionTitle("Vector3/Get Z")]
        public static float GetZ(Vector3 vector)
        {
            return vector.z;
        }
    }
    [ActionLibrary]
    public static class Vector2Library
    {
        [ActionTitle("Vector3/Get X")]
        public static float GetX(Vector2 vector)
        {
            return vector.x;
        }
        [ActionTitle("Vector3/Get Y")]
        public static float GetY(Vector2 vector)
        {
            return vector.y;
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

    public class Log : UFAction
    {
        [In] public string Message;
        public override bool Execute()
        {
            Debug.Log(Message);
            return true;
        }
    }

    [ActionTitle("Conditions/Are Equal")]
    public class Comparison : UFAction
    {
        [In] public object A;
        [Out(IsNewLine = false)]
        public object Result;
        [In] public object B;
        [Out] public Action True;
        [Out] public Action False;

        public override bool Execute()
        {
            
            if (A != null && A.Equals(B))
                if (True != null) 
                    True();
            else
                if (False != null) 
                    False();

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

 

    [ActionTitle("Math/Addition/Vector3")]
    public class AddVector3 : UFAction
    {
        [In] public Vector3 In;
        [In] public float X;
        [In] public float Y;
        [In] public float Z;

        [Out] public Vector3 Result;


        public Action Next;

        public override bool Execute()
        {
        
            Result = In + new Vector3(X, Y, Z);
            return true;
        }
    }

    //[ActionTitle("Components/Add Component")]
    //public class AddComponent : UFAction
    //{
    //    [In] public Type ComponentType;

    //    public override bool Execute()
    //    {

    //        Entity.gameObject.AddComponent(ComponentType);
    //        return true;
    //    }
    //}

    //[ActionTitle("Components/Remove Component")]
    //public class RemoveComponent : UFAction
    //{
    //    [In] public EcsComponent ComponentType;
    //    [In] public int Time;

    //    public override bool Execute()
    //    {
    //        UnityEngine.Object.Destroy(ComponentType,Time);
    //        return true;
    //    }
    //}

    [ActionTitle("Timers/Timer")]
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

    [ActionTitle("Timers/Interval")]
    public class Interval : UFAction
    {
        [In]
        public int Minutes;
        [In]
        public int Seconds;
        [Out]
        public Action Tick;

        public override bool Execute()
        {
            Observable.Interval(new TimeSpan(0, 0, Minutes, Seconds, 0)).Subscribe(_ =>
            {
                Tick();
            }).DisposeWith(System);
            return true;
        }
    }
}

namespace uFrame.Actions.Attributes
{
    public class ActionMetaAttribute : Attribute
    {
        
    }
    public class ActionTitle : ActionMetaAttribute
    {
        public string Title { get; set; }

        public ActionTitle(string title)
        {
            Title = title;
        }

    }
    public class ActionDescription : ActionMetaAttribute
    {
        public string Description { get; set; }

        public ActionDescription(string description)
        {
            Description = description;
        }
    }
    public class ActionAttribute : Attribute
    {
        
    }

    public class FieldDisplayTypeAttribute : ActionAttribute
    {
        public FieldDisplayTypeAttribute(string displayName)
        {
            DisplayName = displayName;
        }

        public FieldDisplayTypeAttribute()
        {
        }

        private bool _isNewLine = true;
        public string DisplayName { get; set; }

        public string ParameterName { get; set; }

        //public int Row { get; set; }

        public FieldDisplayTypeAttribute(string parameterName, string displayName)
        {
            ParameterName = parameterName;
            DisplayName = displayName;
        }

        public FieldDisplayTypeAttribute(string parameterName, string displayName, bool isNewLine)
        {
            ParameterName = parameterName;
            DisplayName = displayName;
            _isNewLine = isNewLine;
        }

        public bool IsNewLine
        {
            get { return _isNewLine; }
            set { _isNewLine = value; }
        }
    }

    public class ActionLibrary : Attribute
    {
        
    }
    public class In : FieldDisplayTypeAttribute
    {
        public In()
        {
        }

        public In(string parameterName, string displayName) : base(parameterName, displayName)
        {
        }

        public In(string displayName) : base(displayName)
        {
        }

        public In(string parameterName, string displayName, bool isNewLine) : base(parameterName, displayName, isNewLine)
        {
        }
    }
    public class Out : FieldDisplayTypeAttribute
    {
        public Out()
        {
        }

        public Out(string parameterName, string displayName) : base(parameterName, displayName)
        {
        }

        public Out(string displayName) : base(displayName)
        {
        }

        public Out(string parameterName, string displayName, bool isNewLine) : base(parameterName, displayName, isNewLine)
        {
        }
    }



    public class EventAttribute : Attribute
    {
        public string Title { get; set; }

        public EventAttribute()
        {
        }

        public EventAttribute(string title)
        {
            Title = title;
        }

    }

    public class EventDispatcher : EventAttribute
    {
        public EventDispatcher()
        {
        }

        public EventDispatcher(string title) : base(title)
        {
        }
    }

    public class SystemEvent : EventAttribute
    {
        public SystemEvent(string systemMethodName)
        {
            SystemMethodName = systemMethodName;
        }

        public SystemEvent(string title, string systemMethodName) : base(title)
        {
            SystemMethodName = systemMethodName;
        }

        public string SystemMethodName { get; set; }
    }
}