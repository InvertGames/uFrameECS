﻿using System;
using System.Collections.Generic;
using System.Linq;
using uFrame.Kernel;
using uFrame.MVVM;
using UniRx;
using UnityEngine;

namespace uFrame.ECS
{
    public class EcsComponentManagerOf<TComponentType> : EcsComponentManager, IEcsComponentManagerOf<TComponentType> where TComponentType : class,IEcsComponent
    {
        protected Dictionary<int, List<TComponentType>> _components = new Dictionary<int, List<TComponentType>>();

        private Subject<TComponentType> _CreatedObservable;

        public IObservable<TComponentType> CreatedObservable
        {
            get
            {
                if (_CreatedObservable == null)
                {
                    _CreatedObservable = new Subject<TComponentType>();
                }
                return _CreatedObservable;
            }
        }
        private Subject<TComponentType> _RemovedObservable;

        public IObservable<TComponentType> RemovedObservable
        {
            get
            {
                if (_RemovedObservable == null)
                {
                    _RemovedObservable = new Subject<TComponentType>();
                }
                return _RemovedObservable;
            }
        }

        public virtual IEnumerable<TComponentType> Components
        {
            get
            {
                return _components.Values.SelectMany(p=>p);
            }
        }

        public override Type For
        {
            get { return typeof (TComponentType); }
        }

        public override IEnumerable<IEcsComponent> All
        {
            get
            {
                foreach (TComponentType component in Components)
                    yield return component;
            }
        }

        public TComponentType this[int entityId]
        {
            get
            {
                if (_components.ContainsKey(entityId))
                {
                    return _components[entityId].FirstOrDefault();
                }
                return null;
            }
        }

        public override IEnumerable<IEcsComponent> ForEntity(int entityId)
        {
            if (!_components.ContainsKey(entityId)) yield break;

            foreach (var item in _components[entityId])
            {
                yield return item;
            }
        }

        protected override void AddItem(IEcsComponent component)
        {
            if (!_components.ContainsKey(component.EntityId))
            {
                _components.Add(component.EntityId,new List<TComponentType>());
            }
            _components[component.EntityId].Add((TComponentType)component);
            if (_CreatedObservable != null)
            {
                _CreatedObservable.OnNext((TComponentType) component);
            }
        }

        protected override void RemoveItem(IEcsComponent component)
        {
            if (_components == null || !_components.ContainsKey(component.EntityId)) return;
            _components[component.EntityId].Remove((TComponentType)component);
            if (_RemovedObservable != null)
            {
                _RemovedObservable.OnNext((TComponentType)component);
            }
        }

        public override bool Match(int entityId)
        {
            return _components.ContainsKey(entityId);
        }
    }
}

