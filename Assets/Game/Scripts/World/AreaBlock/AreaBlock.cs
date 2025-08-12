using System.Collections.Generic;
using GDFrameworkCore;
using GDFrameworkExtend.Data;
using GDFrameworkExtend.SingletonKit;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.World
{
    /// <summary>
    /// 每个区块里面包含有多个房间
    /// 区块都必定会有入口,但是不一定会有出口
    /// 同时,也可能一个区块具有多个入口或者多个出口
    /// </summary>
    public class AreaBlock : MonoSingleton<AreaBlock>,IController
    {
        [ReadOnly,LabelText("当前区块的数据")]
        public AreaBlockData curAreaBlockData;
        
        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        public void InitAreaBlock()
        {
            WorldDataModel curWorldData = this.GetModel<WorldDataModel>();
            if (curWorldData==null || curWorldData.worldDataPersistent==null||curWorldData.worldDataTemporary == null)
            {
                return;
            }
            string curAreaBlockId = this.GetModel<WorldDataModel>().worldDataTemporary.curPlayerLocateAreaBlockId;
            this.curAreaBlockData = this.GetModel<WorldDataModel>().GetCurrentAreaBlockData(curAreaBlockId);
            InitRoomData(this.curAreaBlockData);
        }

        private void InitRoomData(AreaBlockData  curAreaBlockData)
        {
            Room.Instance.InitRoom(curAreaBlockData);
        }
        
        
    }
}