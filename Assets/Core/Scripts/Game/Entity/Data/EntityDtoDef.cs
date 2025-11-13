using System;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Substance.Actions.Interface;
using Core.Game.Chunk.Substance.Interface;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Substance.Data
{
    [Serializable]
    public abstract class EntityDtoDef : IEntityDtoDef,IEntityPlaceableDtoDef,IEntityLifeCycleAction
    {
        [LabelText("配置ID")]
        [InfoBox("这是配置的唯一标识(在编辑器保存时,只会生成一次,")]
        public string DefId { get; protected set; }

        [LabelText("配置名称")]
        public string DefName { get; set; }

        [LabelText("配置描述")]
        public string DefDescription { get; set; }

        [LabelText("贴图ID")]
        public string SpriteId { get; set; }
        
        [LabelText("尺寸,默认为1x1个瓦片大小")]
        public Vector2Int EntitySize { get; set; }
        
        [LabelText("是否阻碍移动")]
        public bool IsBlockingMovement { get; set; }
        

        private void GenerateDefId()
        {
            string typePrefix = GetTypePrefix();
            string uniqueId = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
            DefId = $"{typePrefix}_DEF_{uniqueId}";
        }

        public virtual string GetTypePrefix()
        {
            return "Entity";
        }
        
        public void SaveThisDef()
        {
            
        }

        public void DeleteThisDef()
        {
            
        }

        public virtual bool Validate(out string error)
        {
            if (string.IsNullOrEmpty(DefId))
            {
                error = "配置ID不能为空";
                return false;
            }

            error = string.Empty;
            return true;
        }
        
        public IEntityDtoDef Clone()
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
        
            string json = JsonConvert.SerializeObject(this, settings);
            return JsonConvert.DeserializeObject<EntityDtoDef>(json, settings) as IEntityDtoDef;
        }


    }
}