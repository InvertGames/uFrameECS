using System;
using UnityEngine;
using System.Collections;
using uFrame.Actions.Attributes;
using uFrame.ECS;

namespace uFrame.Actions
{
    public abstract class UFAction
    {
        public Entity Entity;
        public abstract bool Execute();

    }

    [ActionTitle("Comparison/Are Equal")]
    public class Comparison : UFAction
    {
        [VarIn] public object A;

        [VarOut(IsNewLine = false)]
        public object Result;

        [VarIn] public object B;


        [VarOut] public Action True;
        [VarOut] public Action False;

        
        public override bool Execute()
        {
            if (A == B)
                True();
            else
                False();

            return false;
        }
    }

    [ActionTitle("Movement/Move Over Time")]
    public class MoveInDirectionOverTime : UFAction
    {
        [VarIn] public EcsComponent Component;
        [VarIn] public float Speed;
        [VarIn] public Vector3 Direction;

        public override bool Execute()
        {
            Component.CachedTransform.position += (Direction*Speed)*Time.deltaTime;
            return true;
        }
    }
    

    [ActionTitle("Math/Addition/Vector3")]
    public class AddVector3 : UFAction
    {
        [VarIn] public Vector3 In;
        [VarIn] public float X;
        [VarIn] public float Y;
        [VarIn] public float Z;

        [VarOut] public Vector3 Result;


        public Action Next;

        public override bool Execute()
        {
        
            Result = In + new Vector3(X, Y, Z);
            return true;
        }
    }

    [ActionTitle("Components/Add Component")]
    public class AddComponent : UFAction
    {
        [VarIn] public Type ComponentType;

        public override bool Execute()
        {
            Entity.gameObject.AddComponent(ComponentType);
            return true;
        }
    }

    [ActionTitle("Components/Remove Component")]
    public class RemoveComponent : UFAction
    {
        [VarIn] public EcsComponent ComponentType;
        [VarIn] public int Time;

        public override bool Execute()
        {
            UnityEngine.Object.Destroy(ComponentType,Time);
            return true;
        }
    }

    [ActionTitle("Timers/Timer")]
    public class Timer : UFAction
    {
        [VarIn] public int Minutes;
        [VarIn] public int Seconds;

        public override bool Execute()
        {
            return true;
        }
    }

    [ActionTitle("Timers/Timer")]
    public class Interval : UFAction
    {
        [VarIn]
        public int Minutes;
        [VarIn]
        public int Seconds;

        public override bool Execute()
        {
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
        private bool _isNewLine = true;
        public string Name { get; set; }

        public bool IsNewLine
        {
            get { return _isNewLine; }
            set { _isNewLine = value; }
        }
    }
    public class VarIn : FieldDisplayTypeAttribute
    {
        
    }
    public class VarOut : FieldDisplayTypeAttribute
    {

    }
}