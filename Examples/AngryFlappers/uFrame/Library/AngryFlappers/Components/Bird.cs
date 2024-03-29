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
using uFrame.ECS;
using UnityEngine;
using UniRx;


public partial class Bird : uFrame.ECS.EcsComponent {
    
    private Subject<Vector3> _VelocityObservable;
    
    private Subject<Boolean> _DidFlapObservable;
    
    private Subject<Int32> _MaxSpeedObservable;
    
    private Subject<Single> _FlapVelocityObservable;
    
    [UnityEngine.SerializeField()]
    private Vector3 _Velocity;
    
    [UnityEngine.SerializeField()]
    private Boolean _DidFlap;
    
    [UnityEngine.SerializeField()]
    private Int32 _MaxSpeed;
    
    [UnityEngine.SerializeField()]
    private Single _FlapVelocity;
    
    public int ComponentID {
        get {
            return 4;
        }
    }
    
    public IObservable<Vector3> VelocityObservable {
        get {
            if (_VelocityObservable == null) {
                _VelocityObservable = new Subject<Vector3>();
            }
            return _VelocityObservable;
        }
    }
    
    public IObservable<Boolean> DidFlapObservable {
        get {
            if (_DidFlapObservable == null) {
                _DidFlapObservable = new Subject<Boolean>();
            }
            return _DidFlapObservable;
        }
    }
    
    public IObservable<Int32> MaxSpeedObservable {
        get {
            if (_MaxSpeedObservable == null) {
                _MaxSpeedObservable = new Subject<Int32>();
            }
            return _MaxSpeedObservable;
        }
    }
    
    public IObservable<Single> FlapVelocityObservable {
        get {
            if (_FlapVelocityObservable == null) {
                _FlapVelocityObservable = new Subject<Single>();
            }
            return _FlapVelocityObservable;
        }
    }
    
    public Vector3 Velocity {
        get {
            return _Velocity;
        }
        set {
            if (_VelocityObservable != null) {
                _VelocityObservable.OnNext(value);
            }
            _Velocity = value;
        }
    }
    
    public Boolean DidFlap {
        get {
            return _DidFlap;
        }
        set {
            if (_DidFlapObservable != null) {
                _DidFlapObservable.OnNext(value);
            }
            _DidFlap = value;
        }
    }
    
    public Int32 MaxSpeed {
        get {
            return _MaxSpeed;
        }
        set {
            if (_MaxSpeedObservable != null) {
                _MaxSpeedObservable.OnNext(value);
            }
            _MaxSpeed = value;
        }
    }
    
    public Single FlapVelocity {
        get {
            return _FlapVelocity;
        }
        set {
            if (_FlapVelocityObservable != null) {
                _FlapVelocityObservable.OnNext(value);
            }
            _FlapVelocity = value;
        }
    }
}
