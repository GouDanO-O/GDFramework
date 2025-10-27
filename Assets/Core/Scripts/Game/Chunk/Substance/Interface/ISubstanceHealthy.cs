namespace Core.Game.Chunk.Substance.Interface
{
    public interface ISubstanceHealthy
    {
        void BeRepair(float added);
        
        void BeHarmed(float decreased);
        
        void BeDestroy();
    }
}