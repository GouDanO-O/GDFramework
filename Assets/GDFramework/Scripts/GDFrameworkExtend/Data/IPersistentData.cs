namespace GDFrameworkExtend.Data
{
    public interface IPersistentData
    {
        void LoadData(IPersistentData data);
        
        void SaveData();
    }
}