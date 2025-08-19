using System;
using System.IO;
using GDFramework.Utility;
using GDFrameworkExtend.StorageKit;
using Newtonsoft.Json;

namespace GDFrameworkExtend.Data
{
    /// <summary>
    /// 临时游戏数据
    /// 仅当前存档中持续存在,会被玩家的行为影响而产生影响
    /// </summary>
    [Serializable]
    public abstract class TemporalityData
    {
        
        public string temporalityDataId;
        
        public virtual void LoadTemporalityData()
        {
            
        }
        
        public virtual void SaveTemporalityData()
        {

        }

        public virtual void SaveTemporalityData(string path)
        {
            string willSavePath = path;
            
            string dirPath = Path.GetDirectoryName(willSavePath);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
            
            willSavePath += temporalityDataId+".json";
            
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(willSavePath, json);
            LogMonoUtility.AddLog("保存世界临时数据成功");
        }
    }
}