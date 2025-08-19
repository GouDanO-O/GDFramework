using System;
using System.IO;
using GDFramework.Utility;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GDFrameworkExtend.Data
{
    [Serializable]
    public abstract class ConfigData : PersistentData
    {
        [LabelText("配置名称")]
        public string configName;
        
        [LabelText("配置唯一ID")]
        public string configId;

        [LabelText("配置描述")]
        public string configDes;

        public virtual void LoadConfigData()
        {
            
        }
        
        public virtual void SaveConfigData()
        {
            
        }

        public virtual void SaveConfigData(string path)
        {
            string willSavePath = path;
            
            string dirPath = Path.GetDirectoryName(willSavePath);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
            
            willSavePath += this.configId+".json";
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(willSavePath, json);
            LogMonoUtility.AddLog($"保存{willSavePath}数据成功");
        }
    }
}