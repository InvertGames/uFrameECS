// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 2.0.50727.1433
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using uFrame.Kernel;
using UnityEngine;
using uFrame.ECS;


public class MoveInDirectionHandler : object {
    
    public Scroller Item;
    
    private uFrame.ECS.ISystemUpdate _Event;
    
    private uFrame.ECS.EcsSystem _System;
    
    private object Comparisons_a = default( System.Object );
    
    private object Comparisons_b = default( System.Object );
    
    private bool NewBoolNode2 = true;
    
    private bool Comparisons_AreEqual_Result = default( System.Boolean );
    
    private uFrame.ECS.EcsComponent MoveInDirectionOverTime_Component = default( uFrame.ECS.EcsComponent );
    
    private float MoveInDirectionOverTime_Speed = default( System.Single );
    
    private UnityEngine.Vector3 MoveInDirectionOverTime_Direction = default( UnityEngine.Vector3 );
    
    private Vector3 Left = new UnityEngine.Vector3(-1f,0f,0f);
    
    private uFrame.Actions.MoveInDirectionOverTime ddbfeabed = new uFrame.Actions.MoveInDirectionOverTime();
    
    public uFrame.ECS.ISystemUpdate Event {
        get {
            return _Event;
        }
        set {
            _Event = value;
        }
    }
    
    public uFrame.ECS.EcsSystem System {
        get {
            return _System;
        }
        set {
            _System = value;
        }
    }
    
    public virtual void Execute() {
        // Before visit uFrame.Actions.Comparisons.AreEqual
        Comparisons_a = Item.IsScrolling;
        Comparisons_b = NewBoolNode2;
        // Visit uFrame.Actions.Comparisons.AreEqual
        Comparisons_AreEqual_Result = uFrame.Actions.Comparisons.AreEqual(Comparisons_a, Comparisons_b, Comparisons_AreEqual_yes, Comparisons_AreEqual_no);
        // HANDLER: MoveInDirection
    }
    
    private void Comparisons_AreEqual_yes() {
        // Before visit uFrame.Actions.MoveInDirectionOverTime
        MoveInDirectionOverTime_Component = Item;
        MoveInDirectionOverTime_Speed = Item.ScrollSpeed;
        MoveInDirectionOverTime_Direction = Left;
        // Visit uFrame.Actions.MoveInDirectionOverTime
        ddbfeabed.Component = Item;
        ddbfeabed.Speed = Item.ScrollSpeed;
        ddbfeabed.Direction = Left;
        ddbfeabed.System = System;
        if (!ddbfeabed.Execute()) {
            return;
        }
        // CALL EXECUTE ON MoveInDirectionOverTime CLASS
    }
    
    private void Comparisons_AreEqual_no() {
    }
}

public class ToggleScrollHandler : object {
    
    public Scroller Item;
    
    private ToggleScrolling _Event;
    
    private uFrame.ECS.EcsSystem _System;
    
    private object SetScrollingValue_Value = default( System.Object );
    
    public ToggleScrolling Event {
        get {
            return _Event;
        }
        set {
            _Event = value;
        }
    }
    
    public uFrame.ECS.EcsSystem System {
        get {
            return _System;
        }
        set {
            _System = value;
        }
    }
    
    public virtual void Execute() {
        SetScrollingValue_Value = Event.On;
        Item.IsScrolling = (Boolean)SetScrollingValue_Value;
        // HANDLER: ToggleScroll
    }
}

public class DestroyItemsHandler : object {
    
    public DestroyWhenInvisible EntityId;
    
    private uFrame.ECS.BecameInvisibleDispatcher _Event;
    
    private uFrame.ECS.EcsSystem _System;
    
    private int DestroyLibrary_entityId = default( System.Int32 );
    
    public uFrame.ECS.BecameInvisibleDispatcher Event {
        get {
            return _Event;
        }
        set {
            _Event = value;
        }
    }
    
    public uFrame.ECS.EcsSystem System {
        get {
            return _System;
        }
        set {
            _System = value;
        }
    }
    
    public virtual void Execute() {
        // Before visit uFrame.Actions.DestroyLibrary.DestroyEntity
        DestroyLibrary_entityId = Event.EntityId;
        // Visit uFrame.Actions.DestroyLibrary.DestroyEntity
        uFrame.Actions.DestroyLibrary.DestroyEntity(DestroyLibrary_entityId);
        // HANDLER: DestroyItems
    }
}

public class ResetItemHandler : object {
    
    public ResetWhenInvisible EntityId;
    
    private uFrame.ECS.BecameInvisibleDispatcher _Event;
    
    private uFrame.ECS.EcsSystem _System;
    
    private uFrame.ECS.Entity EntityTransform_entity = default( uFrame.ECS.Entity );
    
    private UnityEngine.Vector3 EntityTransform_position = default( UnityEngine.Vector3 );
    
    public uFrame.ECS.BecameInvisibleDispatcher Event {
        get {
            return _Event;
        }
        set {
            _Event = value;
        }
    }
    
    public uFrame.ECS.EcsSystem System {
        get {
            return _System;
        }
        set {
            _System = value;
        }
    }
    
    public virtual void Execute() {
        // Before visit uFrame.Actions.EntityTransform.SetPosition
        EntityTransform_entity = EntityId.Entity;
        EntityTransform_position = EntityId.StartPosition;
        // Visit uFrame.Actions.EntityTransform.SetPosition
        uFrame.Actions.EntityTransform.SetPosition(EntityTransform_entity, EntityTransform_position);
        // HANDLER: ResetItem
    }
}

public class TrackStartPositionHandler : object {
    
    public ResetWhenInvisible Item;
    
    private uFrame.Kernel.GameReadyEvent _Event;
    
    private uFrame.ECS.EcsSystem _System;
    
    public uFrame.Kernel.GameReadyEvent Event {
        get {
            return _Event;
        }
        set {
            _Event = value;
        }
    }
    
    public uFrame.ECS.EcsSystem System {
        get {
            return _System;
        }
        set {
            _System = value;
        }
    }
    
    public virtual void Execute() {
        // HANDLER: TrackStartPosition
    }
}
