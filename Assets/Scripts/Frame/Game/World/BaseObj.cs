namespace Frame.Game.World
{
    public abstract class BaseObj : IObj
    {
        public int UniqueObjId { get; set; }
        
        public string ObjName { get; set; }

        public abstract void InitObj();
        
        public abstract void DeInitObj();
    }
}