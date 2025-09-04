using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Game.World
{
    public abstract class Dto : ScriptableObject
    {
        [LabelText("配置名称")]
        public string configName;
        
        [LabelText("配置ID(同一层级必须唯一)")]
        public string configId;

        [LabelText("配置描述"), TextArea]
        public string configDes;
        
        [LabelText("层级拼接后的全局ID(dtoId)"), ReadOnly, DisableInEditorMode]
        public string dtoId;
        
        [HideInInspector] // 稳定不变的 UID（用资源 GUID）
        public string stableUid;
        
#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            // 没填 configId 时默认用资产名
            if (string.IsNullOrEmpty(configId))
                configId = "default";

            // 首次赋 stableUid = 资源 GUID
            if (string.IsNullOrEmpty(stableUid))
            {
                var path = AssetDatabase.GetAssetPath(this);
                if (!string.IsNullOrEmpty(path))
                    stableUid = AssetDatabase.AssetPathToGUID(path);
            }
        }
#endif
    }
}