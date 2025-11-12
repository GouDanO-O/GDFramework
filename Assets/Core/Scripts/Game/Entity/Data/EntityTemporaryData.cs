using System;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Substance.Interface;
using Core.Game.Chunk.Tile;
using Core.Game.Storage;
using GDFrameworkCore;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Substance.Data
{
    [Serializable]
    public class EntityTemporaryData : TileEntityData,IEntityTemporaryData,ICanGetSystem
    {
        [LabelText("实例生成时唯一ID(如果是从池中取出,那么每次取出或回收都要置空)")]
        public string EntityInstanceId { get; set; }
        
        [LabelText("创建时间")]
        public DateTime CreateTime { get; set; }
        
        [LabelText("最后修改时间")]
        public DateTime LastModifyTime { get; set; }
        
        [LabelText("当前生命值")]
        public int CurrentHealth { get; set; }
        
        [LabelText("当前最大生命值")]
        public int CurrentMaxHealth { get; set; }
        
        private StorageSystem _storageSystem;

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }
        
        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="instanceId"></param>
        public virtual void CreateEntityTempData(string defId, string instanceId)
        {
            EntityDtoDefId = defId;
            CreateTime = DateTime.Now;
            LastModifyTime = DateTime.Now;
            if (instanceId == string.Empty && EntityInstanceId == string.Empty)
            {
                EntityInstanceId = GenerateInstanceId();
            }
        }

        public virtual void SaveTempData()
        {
            if (_storageSystem == null)
            {
                _storageSystem = this.GetSystem<StorageSystem>();
            }
            _storageSystem.SaveEntityTemporaryData(EntityInstanceId,this);
        }

        protected virtual string GenerateInstanceId()
        {
            return $"INST_{Guid.NewGuid().ToString("N").ToUpper()}";
        }


    }
}