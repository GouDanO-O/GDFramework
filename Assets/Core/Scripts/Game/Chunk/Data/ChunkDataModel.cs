using System;
using System.Collections.Generic;
using Core.Game.Chunk.Data.Interface;
using GDFrameworkCore;
using GDFrameworkExtend.Data;

namespace Core.Game.Chunk.Data
{
    public abstract class ChunkDataModel : AbstractModel
    {
        protected override void OnInit()
        {
            
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public abstract void InitializeDataModel();
    }
}