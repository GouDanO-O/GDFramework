using System;
using System.Collections.Generic;
using System.IO;
using Core.Game.Chunk.Data;
using GDFramework.Utility;
using GDFrameworkCore;
using GDFrameworkExtend.Data;
using GDFrameworkExtend.EventKit;
using GDFrameworkExtend.JsonKit;
using GDFrameworkExtend.StorageKit;
using Newtonsoft.Json;
using NUnit.Framework;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Game.Chunk.Node.Data
{
    /// <summary>
    /// World->单个世界里面的所有区域
    /// Areas->单个区域里面的所有房间
    /// Rooms->单个房间里面的所有Nodes,这个房间里面会存储着如下所持有的所有节点数据
    /// 通过运行时或非运行时进行序列化存储
    /// 每次进入区域,首先,序列化所有房间,房间里面又存储
    /// 只存储当前节点的触发状态和位置
    /// </summary>
    [LabelText("节点数据")]
    public class NodeData : ChunkData
    {
        [LabelText("当前节点的固定数据")]
        private NodeDto nodeDto;

        [LabelText("当前节点的临时数据")]
        private NodeDtoTemporary nodeDtoTemporary;

        public void InitNodeData(Node node)
        {
        }

        /// <summary>
        /// 能否进行互动
        /// </summary>
        /// <returns></returns>
        public bool CanTrigger()
        {
            return true;
        }

        /// <summary>
        /// 能否进行移动
        /// </summary>
        /// <returns></returns>
        public bool CanMoveable()
        {
            return true;
        }

        /// <summary>
        /// 检查触发条件
        /// </summary>
        /// <returns></returns>
        public bool CheckCondition()
        {
            return true;
        }

        /// <summary>
        /// 重置节点状态
        /// </summary>
        public void ResetNodeState()
        {
        }

        /// <summary>
        /// 存储节点数据
        /// </summary>
        public void SaveNodeData()
        {
        }

        /// <summary>
        /// 销毁节点临时数据
        /// </summary>
        public void DestroyNodeData()
        {
        }

        public void ChangeTempPosition(Vector2 position)
        {
        }
    }
}