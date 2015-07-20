namespace uFrame.ECS
{
    public interface IEcsComponent
    {
        int EntityId { get; set; }
        int ComponentId { get; }
    }

 
}