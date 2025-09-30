using System.Collections.Generic;
using GDFrameworkCore;
using GDFrameworkExtend.Data;
using GDFrameworkExtend.SingletonKit;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Region
{
    /// <summary>
    /// 每个区块里面包含有多个房间
    /// 区块都必定会有入口,但是不一定会有出口
    /// 同时,也可能一个区块具有多个入口或者多个出口
    /// </summary>
    public class RegionManager : MonoSingleton<RegionManager>,IController
    {
        [ReadOnly,LabelText("当前区块的数据")]
        public RegionData CurRegionData;
        
        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        public void InitAreaBlock()
        {

        }

        private void InitRoomData(RegionData  curRegionData)
        {
            Room.RoomManager.Instance.InitRoom(curRegionData);
        }
        
        
    }
}