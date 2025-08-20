using System;
using System.IO;
using GDFramework.Utility;
using GDFrameworkExtend.JsonKit;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GDFrameworkExtend.Data
{
    [Serializable]
    public abstract class ConfigData : PersistentData
    {
        [LabelText("配置名称")] public string configName;

        [LabelText("配置唯一ID")] public string configId;

        [LabelText("配置描述")] public string configDes;

        public virtual void LoadConfigData()
        {
        }

        public virtual void SaveConfigData()
        {
        }

        public virtual void SaveConfigData(string directory) => SaveConfigData(directory, JsonSettings.Make());

        public virtual void SaveConfigData(string directory, JsonSerializerSettings settings)
        {
            if (string.IsNullOrEmpty(configId))
                configId = "default";

            // 正确：创建“传入的目录本身”
            Directory.CreateDirectory(directory);

            string filePath = Path.Combine(directory, $"{configId}.json");
            string json = JsonConvert.SerializeObject(this, settings ?? JsonSettings.Make());
            File.WriteAllText(filePath, json);
            LogMonoUtility.AddLog($"保存 {filePath} 数据成功");
        }
    }
}