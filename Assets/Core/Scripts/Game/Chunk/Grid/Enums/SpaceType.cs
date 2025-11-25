using Sirenix.OdinInspector;

namespace Core.Game.Grid
{
    /// <summary>
    /// 空间类型
    /// </summary>
    public enum SpaceType
    {
        [LabelText("通用空间")]
        General,
        
        [LabelText("卧室")]
        Bedroom,
        
        [LabelText("客厅")]
        LivingRoom,
        
        [LabelText("厨房")]
        Kitchen,
        
        [LabelText("浴室")]
        Bathroom,
        
        [LabelText("书房")]
        Study,
        
        [LabelText("餐厅")]
        DiningRoom,
        
        [LabelText("储藏室")]
        Storage,
        
        [LabelText("走廊")]
        Corridor,
        
        [LabelText("阳台")]
        Balcony,
        
        [LabelText("户外")]
        Outdoor,
        
        [LabelText("地下室")]
        Basement,
        
        [LabelText("自定义")]
        Custom
    }

}