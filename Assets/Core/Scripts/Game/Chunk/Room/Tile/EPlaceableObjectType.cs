using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Room
{
    public enum EPlaceableObjectType
    {
        [LabelText("家具")]
        Furniture = 0,

        [LabelText("收纳容器")]
        Container = 1,

        [LabelText("装饰物")]
        Decoration = 2,

        [LabelText("交互物")]
        Interactive = 3,

        [LabelText("光源")]
        LightSource = 4
    }
}