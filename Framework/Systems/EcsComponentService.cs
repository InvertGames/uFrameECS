using uFrame.IOC;
using uFrame.Kernel;
using UniRx;

namespace uFrame.ECS
{
    public class EcsComponentService : EcsSystem
    {
        [Inject]
        public IComponentSystem ComponentSystem { get; set; }


        public override void Setup()
        {
            base.Setup();
            this.OnEvent<ComponentCreatedEvent>().Subscribe(_ =>
            {
                ComponentSystem.RegisterComponentInstance(_.Component.GetType(), _.Component);
            }).DisposeWith(this);

            this.OnEvent<ComponentDestroyedEvent>().Subscribe(_ =>
            {
                ComponentSystem.DestroyComponentInstance(_.Component.GetType(), _.Component);
            }).DisposeWith(this);
        }

    }
    public class ComponentCreatedEvent
    {
        public IEcsComponent Component { get; set; }
    }

    public class ComponentDestroyedEvent
    {
        public IEcsComponent Component { get; set; }
    }

}