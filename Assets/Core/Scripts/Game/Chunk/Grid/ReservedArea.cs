using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Grid
{
    public class ReservedArea
    {
        [LabelText("起始位置")]
        public Vector3Int StartPosition;
        
        [LabelText("区域大小")]
        [MinValue(1)]
        public Vector3Int Size = Vector3Int.one;
        
        [LabelText("用途说明")]
        public string Purpose = "特殊区域";
    }
}