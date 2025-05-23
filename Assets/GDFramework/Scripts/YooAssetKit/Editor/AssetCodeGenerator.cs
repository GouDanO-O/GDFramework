#if UNITY_EDITOR
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace YooAsset.Editor
{
    public class AssetCodeGenerator
    {
        [MenuItem("YooAsset/Generate Asset Code")]
        public static void GenerateAssetCode()
        {
            StringBuilder codeBuilder = new StringBuilder();
            
            // Add file header
            codeBuilder.AppendLine("// This file is auto-generated by YooAsset AssetCodeGenerator");
            codeBuilder.AppendLine("// Do not modify this file - your changes will be lost");
            codeBuilder.AppendLine();
            
            // Add namespace
            codeBuilder.AppendLine("namespace GDFramework.FrameData");
            codeBuilder.AppendLine("{");
            
            // Process each package
            foreach (var package in AssetBundleCollectorSettingData.Setting.Packages)
            {
                ProcessPackage(package, codeBuilder);
            }
            
            codeBuilder.AppendLine("}");
            
            // Write to file
            SaveToFile(codeBuilder.ToString());
            
            Debug.Log("YooAsset code generation completed!");
            AssetDatabase.Refresh();
        }
        
        private static void ProcessPackage(AssetBundleCollectorPackage package, StringBuilder codeBuilder)
        {
            codeBuilder.AppendLine();
            codeBuilder.AppendLine($"   public class {SanitizeIdentifier(package.PackageName)}");
            codeBuilder.AppendLine("    {");
            
            // Process each group within the package
            foreach (var group in package.Groups)
            {
                ProcessGroup(group, codeBuilder, package.PackageName);
            }
            
            codeBuilder.AppendLine("    }");
        }
        
        private static void ProcessGroup(AssetBundleCollectorGroup group, StringBuilder codeBuilder, string packageName)
        {
            if (!IsGroupActive(group))
                return;
                
            codeBuilder.AppendLine();
            codeBuilder.AppendLine($"       public class {SanitizeIdentifier(group.GroupName)}");
            codeBuilder.AppendLine("       {");
            
            // Process collectors within the group
            foreach (var collector in group.Collectors)
            {
                if (string.IsNullOrEmpty(collector.CollectPath))
                    continue;
                    
                ProcessCollector(collector, codeBuilder, packageName);
            }
            
            codeBuilder.AppendLine("       }");
        }
        
        private static void ProcessCollector(AssetBundleCollector collector, StringBuilder codeBuilder, string packageName)
        {
            try
            {
                if (collector.CollectorType == ECollectorType.MainAssetCollector || 
                    collector.CollectorType == ECollectorType.StaticAssetCollector)
                {
                    string collectPath = collector.CollectPath;
                    bool isFolder = AssetDatabase.IsValidFolder(collectPath);
                    
                    // Generate bundle name from collect path
                    string bundleName = GenerateBundleName(packageName, collectPath);
                    string className = SanitizeIdentifier(packageName);
                    
                    codeBuilder.AppendLine();
                    codeBuilder.AppendLine($"           public class {className}");
                    codeBuilder.AppendLine("           {");
                    codeBuilder.AppendLine($"               public const string BundleName = \"{bundleName}\";");
                    
                    // Collect all valid asset addresses
                    HashSet<string> addresses = new HashSet<string>();
                    if (isFolder)
                    {
                        string[] assetGuids = AssetDatabase.FindAssets("t:Object", new[] { collectPath });
                        foreach (string guid in assetGuids)
                        {
                            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                            if (ShouldSkipAsset(assetPath)) 
                                continue;
                            addresses.Add(Path.GetFileNameWithoutExtension(assetPath));
                        }
                    }
                    else
                    {
                        addresses.Add(Path.GetFileNameWithoutExtension(collectPath));
                    }
                    
                    // Generate address constants
                    foreach (var address in addresses)
                    {
                        string constantName = SanitizeIdentifier(address);
                        codeBuilder.AppendLine($"               public const string {constantName} = \"yoo:{address}\";");
                    }
                    
                    codeBuilder.AppendLine("           }");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error processing collector {collector.CollectPath}: {ex.Message}");
            }
        }
        
        private static bool ShouldSkipAsset(string assetPath)
        {
            return AssetDatabase.IsValidFolder(assetPath) || 
                   assetPath.EndsWith(".cs") || 
                   assetPath.EndsWith(".js") || 
                   assetPath.EndsWith(".dll");
        }
        
        // 生成与构建结果一致的BundleName
        private static string GenerateBundleName(string packageName, string collectPath)
        {
            // 统一转换为小写
            packageName = packageName.ToLower();
            
            // 处理路径规则（与YooAsset构建逻辑匹配）
            string normalizedPath = collectPath
                .Replace("Assets/", "")      // 移除Assets前缀
                .ToLower()                   // 全小写
                .Replace('/', '_')           // 替换路径分隔符
                .Replace(' ', '_');          // 替换空格
            
            // 如果是文件，取所在目录
            if (!AssetDatabase.IsValidFolder(collectPath))
            {
                string dirPath = Path.GetDirectoryName(normalizedPath);
                if (!string.IsNullOrEmpty(dirPath))
                    normalizedPath = dirPath;
            }
            
            // 合并包名与路径
            return $"{packageName}_{normalizedPath}";
        }
        
        private static bool IsGroupActive(AssetBundleCollectorGroup group)
        {
            IActiveRule activeRule = AssetBundleCollectorSettingData.GetActiveRuleInstance(group.ActiveRuleName);
            return activeRule.IsActiveGroup(new GroupData(group.GroupName));
        }
        
        private static string SanitizeIdentifier(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "Default";
                
            // 保留字母数字，其他字符转下划线
            StringBuilder sb = new StringBuilder();
            foreach (char c in input)
            {
                if (char.IsLetterOrDigit(c))
                    sb.Append(c);
                else
                    sb.Append('_');
            }
            
            string result = sb.ToString();
            if (char.IsDigit(result[0]))
                result = "_" + result;
                
            return result;
        }
        
        private static void SaveToFile(string content)
        {
            string directoryPath = "Assets/GDFramework/GDFrameworkData";
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
            
            string filePath = Path.Combine(directoryPath, "AssetPathConstants.cs");
            File.WriteAllText(filePath, content, Encoding.UTF8);
            Debug.Log($"Asset constants saved to {filePath}");
        }
    }
}
#endif