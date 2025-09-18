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

            // Dto.OnValidate 里增加
            configId = Slugify(configId);
            if (string.IsNullOrEmpty(configId))
                configId = "default";
            if (configId.Contains("/"))
                Debug.LogError($"configId 不能包含 '/': {name}");

            // 首次赋 stableUid = 资源 GUID
            if (string.IsNullOrEmpty(stableUid))
            {
                var path = AssetDatabase.GetAssetPath(this);
                if (!string.IsNullOrEmpty(path))
                    stableUid = AssetDatabase.AssetPathToGUID(path);
            }
        }
        
        static string Slugify(string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            value = value.Trim().ToLowerInvariant();
            value = value.Replace(' ', '-');
            // 去除连续的非法字符，只保留字母数字和 - _ .
            System.Text.StringBuilder sb = new System.Text.StringBuilder(value.Length);
            foreach (var ch in value)
            {
                if ((ch >= 'a' && ch <= 'z') || (ch >= '0' && ch <= '9') || ch == '-' || ch == '_' || ch == '.')
                {
                    sb.Append(ch);
                }
                else if (ch == '/')
                {
                    sb.Append('-');
                }
                else if (char.IsWhiteSpace(ch))
                {
                    sb.Append('-');
                }
            }
            var result = sb.ToString();
            while (result.Contains("--")) result = result.Replace("--", "-");
            return result.Trim('-');
        }
#endif
    }
}