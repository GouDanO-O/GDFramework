using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Game.World.Interface;
using GDFramework.Utility;
using GDFrameworkExtend.Data;
using GDFrameworkExtend.JsonKit;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Game.World
{
    public class RoomData : IData
    {
        public string UniqueId { get; set; }     
        
        public void CombineUniqueId(string fatherDataId, string thisDataId)
        {
            
        }

        [LabelText("当前房间的固定数据")]
        private RoomDto roomDto;

        [LabelText("当前房间的临时数据")]
        private RoomDtoTemporary roomDtoTemporary;
        
        [LabelText("当前房间所持有的节点数据")]
        private Dictionary<string,NodeData> curHoldingNodeDtoDict = new Dictionary<string, NodeData>();

        
    }
}