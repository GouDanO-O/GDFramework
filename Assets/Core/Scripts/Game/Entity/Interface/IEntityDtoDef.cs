namespace Core.Game.Chunk.Substance.Interface
{
    public interface IEntityDtoDef
    {
        string DefId { get; }
        string DefName { get; }
        string DefDescription { get; }

        void SaveThisDef();

        void DeleteThisDef();
        
        bool Validate(out string error);
        
        IEntityDtoDef Clone();
    }
}