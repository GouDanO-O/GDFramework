namespace Game.World.Object.Interface
{
    public interface IComponent
    {
        IWorldObject Owner { get; set; }
        
        void Initialize(IWorldObject owner);
    }
}