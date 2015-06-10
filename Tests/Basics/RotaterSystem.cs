using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using uFrame.ECS;
using uFrame.IOC;
using uFrame.Kernel;
using UniRx;
using UniRx.Triggers;

using UnityEngine;

namespace Assets.uFrameECS.Tests.Basics
{
    public class RotaterSystem : EcsSystem
    {
        [Inject]
        public IComponentSystem ComponentSystem { get; set; }

        public override void Setup()
        {
            base.Setup();
            
            ConstantRotationManager = ComponentSystem.RegisterComponent<ConstantRotation>();
            this.OnComponentCreated<ConstantRotation>()
                .Subscribe(c =>
                {
                    
                    //c.UpdateAsObservable()
                    //    .Subscribe(_ => c.CachedTransform.Rotate(Vector3.right*(c.Speed*Time.deltaTime)))
                    //    .DisposeWith(c); // Disposed when the component is destroyed via UnityEngine.Object.Destroy(c)

                    Observable.Timer(TimeSpan.FromSeconds(4f))
                        .Subscribe(_ => Destroy(c.gameObject))
                        .DisposeWith(c);
                })
                .DisposeWith(this); // Dispose with this systemUnityEngine.Object.Destroy(system)
        }

        public IEcsComponentManagerOf<ConstantRotation> ConstantRotationManager { get; set; }

  
        public void Update()
        {
            foreach (var item in ConstantRotationManager.Components)
            {
                item.CachedTransform.Rotate(Vector3.right*(item.Speed*Time.deltaTime));
            }
        }
    }

}
