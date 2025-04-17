namespace GDFramework.World
{
    public interface IObj
    { 
        int UniqueObjId { get; set; }
        
        string ObjName { get; set; }

        void InitObj();
        
        void DeInitObj();

        void UpdateObjLogic();
    }
}