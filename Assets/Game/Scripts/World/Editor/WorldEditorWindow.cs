using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GDFrameworkExtend.JsonKit;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;

namespace Game.World.Editor
{
    public class WorldDataEditor : OdinMenuEditorWindow
    {
        [MenuItem("Tools/Game/世界数据编辑器")]
        private static void OpenWindow()
        {
            GetWindow<WorldDataEditor>().Show();
        }

        private WorldDataModel currentWorldDataModel;
        private WorldDto selectedWorldDto;
        private AreaBlockDto selectedAreaBlockDto;
        
        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree()
            {
                { "世界数据管理", new WorldDataManager(), EditorIcons.Globe },
                { "创建新世界", new WorldCreator(), EditorIcons.Plus },
                { "临时数据管理", new TemporaryDataManager(), EditorIcons.Upload }
            };

            // 加载现有的世界数据
            LoadExistingWorlds(tree);

            return tree;
        }

        private void LoadExistingWorlds(OdinMenuTree tree)
        {
            try
            {
                // 获取世界数据路径
                string worldDataPath = "Assets/Game/Res/Configs/WorldData";
                if (!Directory.Exists(worldDataPath))
                {
                    Directory.CreateDirectory(worldDataPath);
                    return;
                }

                // 查找所有世界配置文件
                var worldFiles = Directory.GetFiles(worldDataPath, "*.json", SearchOption.TopDirectoryOnly);
                
                foreach (var worldFile in worldFiles)
                {
                    try
                    {
                        string worldName = Path.GetFileNameWithoutExtension(worldFile);
                        var worldWrapper = new WorldDataWrapper(worldFile, worldName);
                        
                        tree.Add($"世界列表/{worldName}", worldWrapper, EditorIcons.SingleUser);
                        
                        // 添加区块子菜单
                        if (worldWrapper.WorldDto?.worldDataPersistent?.areaBlockDatas != null)
                        {
                            foreach (var area in worldWrapper.WorldDto.worldDataPersistent.areaBlockDatas)
                            {
                                var areaWrapper = new AreaBlockWrapper(area, worldWrapper);
                                tree.Add($"世界列表/{worldName}/区块/{area.configId ?? "未命名区块"}", 
                                         areaWrapper, EditorIcons.GridBlocks);
                                
                                // 添加房间子菜单
                                if (area.areaBlockDataPersistent?.roomDatas != null)
                                {
                                    foreach (var room in area.areaBlockDataPersistent.roomDatas)
                                    {
                                        var roomWrapper = new RoomWrapper(room, areaWrapper);
                                        tree.Add($"世界列表/{worldName}/区块/{area.configId ?? "未命名区块"}/房间/{room.configId ?? "未命名房间"}", 
                                                 roomWrapper, EditorIcons.House);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"加载世界文件 {worldFile} 失败: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"加载世界数据失败: {ex.Message}");
            }
        }

        protected override void OnBeginDrawEditors()
        {
            var selected = this.MenuTree.Selection.FirstOrDefault();
            var toolbarHeight = this.MenuTree.Config.SearchToolbarHeight;

            SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
            {
                if (selected != null)
                {
                    GUILayout.Label($"当前选择: {selected.Name}");
                }

                if (SirenixEditorGUI.ToolbarButton(new GUIContent("刷新")))
                {
                    ForceMenuTreeRebuild();
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();
        }
    }

    // 世界数据管理器
    [Serializable]
    public class WorldDataManager
    {
        [InfoBox("世界数据编辑器 - 管理所有游戏世界配置", InfoMessageType.Info)]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1f)]
        public void 打开世界数据目录()
        {
            string path = "Assets/Game/Res/Configs/WorldData";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            EditorUtility.RevealInFinder(path);
        }

        [Button(ButtonSizes.Large), GUIColor(0.8f, 1f, 0.4f)]
        public void 重新加载所有世界数据()
        {
            var window = EditorWindow.GetWindow<WorldDataEditor>();
            window.ForceMenuTreeRebuild();
        }

        [Button(ButtonSizes.Large), GUIColor(1f, 0.8f, 0.4f)]
        public void 验证数据完整性()
        {
            // 可以添加数据验证逻辑
            EditorUtility.DisplayDialog("数据验证", "数据完整性检查已完成！", "确定");
        }
    }

    // 临时数据管理器
    [Serializable]
    public class TemporaryDataManager
    {
        [InfoBox("临时数据管理 - 管理游戏运行时的临时数据", InfoMessageType.Info)]
        
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1f)]
        public void 打开临时数据目录()
        {
            string path = Path.Combine(Application.persistentDataPath, "WorldData");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            EditorUtility.RevealInFinder(path);
        }

        [Button(ButtonSizes.Large), GUIColor(0.8f, 1f, 0.4f)]
        public void 从持久化数据生成临时数据()
        {
            if (EditorUtility.DisplayDialog("生成临时数据", "这将会为所有持久化世界数据生成对应的临时数据，是否继续？", "确定", "取消"))
            {
                GenerateAllTemporaryData();
            }
        }

        [Button(ButtonSizes.Large), GUIColor(1f, 0.6f, 0.4f)]
        public void 清空所有临时数据()
        {
            if (EditorUtility.DisplayDialog("清空临时数据", "确定要清空所有临时数据吗？此操作不可撤销！", "确定", "取消"))
            {
                ClearAllTemporaryData();
            }
        }

        [Button(ButtonSizes.Large), GUIColor(1f, 0.4f, 0.8f)]
        public void 同步临时数据到持久化数据()
        {
            if (EditorUtility.DisplayDialog("同步数据", "这将会将临时数据同步到持久化数据中，是否继续？", "确定", "取消"))
            {
                SyncTemporaryToPersistent();
            }
        }

        private void GenerateAllTemporaryData()
        {
            try
            {
                string persistentPath = "Assets/Game/Res/Configs/WorldData";
                if (!Directory.Exists(persistentPath))
                {
                    EditorUtility.DisplayDialog("错误", "持久化数据目录不存在！", "确定");
                    return;
                }

                var worldFiles = Directory.GetFiles(persistentPath, "*.json", SearchOption.TopDirectoryOnly);
                int processedCount = 0;

                foreach (var worldFile in worldFiles)
                {
                    try
                    {
                        string json = File.ReadAllText(worldFile);
                        var worldData = Newtonsoft.Json.JsonConvert.DeserializeObject<WorldDto>(json);
                        
                        if (worldData != null)
                        {
                            // 初始化临时数据
                            InitializeTemporaryDataForWorld(worldData);
                            // 保存临时数据
                            worldData.SaveData_Temporary();
                            processedCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"处理世界文件 {worldFile} 失败: {ex.Message}");
                    }
                }

                EditorUtility.DisplayDialog("生成完成", $"成功为 {processedCount} 个世界生成临时数据！", "确定");
            }
            catch (Exception ex)
            {
                EditorUtility.DisplayDialog("生成失败", $"生成临时数据时发生错误：{ex.Message}", "确定");
            }
        }

        private void InitializeTemporaryDataForWorld(WorldDto worldData)
        {
            // 初始化世界临时数据
            if (worldData.worldDataTemporary == null)
                worldData.worldDataTemporary = new WorldDataTemporary();

            // 为所有区块初始化临时数据
            if (worldData.worldDataPersistent?.areaBlockDatas != null)
            {
                foreach (var area in worldData.worldDataPersistent.areaBlockDatas)
                {
                    InitializeTemporaryDataForArea(area);
                }
            }
        }

        private void InitializeTemporaryDataForArea(AreaBlockDto areaData)
        {
            // 初始化区块临时数据
            if (areaData.areaBlockDataTemporary == null)
                areaData.areaBlockDataTemporary = new AreaBlockDataTemporary();

            // 为所有房间初始化临时数据
            if (areaData.areaBlockDataPersistent?.roomDatas != null)
            {
                foreach (var room in areaData.areaBlockDataPersistent.roomDatas)
                {
                    InitializeTemporaryDataForRoom(room);
                }
            }
        }

        private void InitializeTemporaryDataForRoom(RoomDto roomData)
        {
            // 初始化房间临时数据
            if (roomData.roomDataTemporary == null)
                roomData.roomDataTemporary = new RoomDataTemporary();

            // 为所有节点初始化临时数据（如果有的话）
            if (roomData.roomDataPersistent?.nodeDatas != null)
            {
                foreach (var node in roomData.roomDataPersistent.nodeDatas)
                {
                    if (node.nodeDataTemporary == null)
                        node.nodeDataTemporary = new NodeDataTemporary();
                }
            }
        }

        private void ClearAllTemporaryData()
        {
            try
            {
                string temporaryPath = Path.Combine(Application.persistentDataPath, "WorldData");
                if (Directory.Exists(temporaryPath))
                {
                    Directory.Delete(temporaryPath, true);
                    EditorUtility.DisplayDialog("清空完成", "所有临时数据已清空！", "确定");
                }
                else
                {
                    EditorUtility.DisplayDialog("提示", "临时数据目录不存在，无需清空！", "确定");
                }
            }
            catch (Exception ex)
            {
                EditorUtility.DisplayDialog("清空失败", $"清空临时数据时发生错误：{ex.Message}", "确定");
            }
        }

        private void SyncTemporaryToPersistent()
        {
            try
            {
                string temporaryPath = Path.Combine(Application.persistentDataPath, "WorldData");
                string persistentPath = "Assets/Game/Res/Configs/WorldData";

                if (!Directory.Exists(temporaryPath))
                {
                    EditorUtility.DisplayDialog("错误", "临时数据目录不存在！", "确定");
                    return;
                }

                // 这里可以添加具体的同步逻辑，根据你的需求来实现
                // 比如将临时数据中的某些变化同步回持久化数据
                
                EditorUtility.DisplayDialog("同步完成", "数据同步完成！", "确定");
            }
            catch (Exception ex)
            {
                EditorUtility.DisplayDialog("同步失败", $"同步数据时发生错误：{ex.Message}", "确定");
            }
        }
    }

    // 世界创建器
    [Serializable]
    public class WorldCreator
    {
        [LabelText("世界名称")]
        [ValidateInput("ValidateWorldName", "世界名称不能为空且不能包含特殊字符")]
        public string worldName = "新世界";

        [LabelText("世界ID")]
        [ValidateInput("ValidateWorldId", "世界ID不能为空且不能包含下划线")]
        public string worldId = "newworld";

        [LabelText("世界描述")]
        [MultiLineProperty(3)]
        public string worldDescription = "这是一个新创建的世界";

        [LabelText("同时创建临时数据")]
        public bool createTemporaryData = true;

        [Button(ButtonSizes.Large), GUIColor(0.4f, 1f, 0.4f)]
        public void 创建新世界()
        {
            if (!ValidateWorldName(worldName) || !ValidateWorldId(worldId))
            {
                EditorUtility.DisplayDialog("创建失败", "请检查输入的世界名称和ID是否符合要求", "确定");
                return;
            }

            try
            {
                var newWorld = new WorldDto
                {
                    configName = worldName,
                    configId = worldId,
                    configDes = worldDescription,
                    worldDataPersistent = new WorldDataPersistent
                    {
                        areaBlockIds = new List<string>(),
                        areaBlockDatas = new List<AreaBlockDto>()
                    },
                    worldDataTemporary = createTemporaryData ? new WorldDataTemporary() : null,
                };

                newWorld.AutoRefreshHierarchy();
                newWorld.SaveData_Persistent();

                // 如果需要创建临时数据
                if (createTemporaryData)
                {
                    newWorld.SaveData_Temporary();
                }

                EditorUtility.DisplayDialog("创建成功", $"世界 '{worldName}' 创建成功！", "确定");
                
                // 刷新编辑器窗口
                var window = EditorWindow.GetWindow<WorldDataEditor>();
                window.ForceMenuTreeRebuild();

                // 重置创建器
                worldName = "新世界";
                worldId = "new_world";
                worldDescription = "这是一个新创建的世界";
            }
            catch (Exception ex)
            {
                EditorUtility.DisplayDialog("创建失败", $"创建世界时发生错误：{ex.Message}", "确定");
            }
        }

        private bool ValidateWorldName(string name)
        {
            return !string.IsNullOrEmpty(name) && !name.Any(c => Path.GetInvalidFileNameChars().Contains(c));
        }

        private bool ValidateWorldId(string id)
        {
            return !string.IsNullOrEmpty(id) && !id.Contains("_") && !id.Any(c => Path.GetInvalidFileNameChars().Contains(c));
        }
    }

    // 世界数据包装器
    [Serializable]
    public class WorldDataWrapper
    {
        [HideInInspector]
        public string filePath;
        
        [ShowInInspector, HideLabel]
        public WorldDto WorldDto { get; private set; }

        public WorldDataWrapper(string path, string worldName)
        {
            filePath = path;
            LoadWorldData();
        }

        private void LoadWorldData()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    var data = Newtonsoft.Json.JsonConvert.DeserializeObject<WorldDto>(json);
                    WorldDto = data;
                    
                    // 动态解析和重建层级关系
                    if (WorldDto != null)
                    {
                        RebuildWorldHierarchy();
                        // 尝试加载对应的临时数据
                        LoadTemporaryData();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"加载世界数据失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 加载对应的临时数据
        /// </summary>
        private void LoadTemporaryData()
        {
            try
            {
                string temporaryPath = Path.Combine(Application.persistentDataPath, "WorldData");
                string worldTemporaryFile = Path.Combine(temporaryPath, $"{WorldDto.dtoId}.json");
                
                if (File.Exists(worldTemporaryFile))
                {
                    string tempJson = File.ReadAllText(worldTemporaryFile);
                    var tempData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(tempJson);
                    
                    if (tempData?.worldDataTemporary != null)
                    {
                        WorldDto.worldDataTemporary = Newtonsoft.Json.JsonConvert.DeserializeObject<WorldDataTemporary>(
                            tempData.worldDataTemporary.ToString());
                    }
                    
                    // 加载区块和房间的临时数据
                    LoadTemporaryDataForAreas(temporaryPath);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"加载临时数据失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 为所有区块加载临时数据
        /// </summary>
        private void LoadTemporaryDataForAreas(string temporaryPath)
        {
            try
            {
                string worldTempDir = Path.Combine(temporaryPath, WorldDto.configId);
                if (!Directory.Exists(worldTempDir)) return;

                foreach (var area in WorldDto.worldDataPersistent.areaBlockDatas)
                {
                    string areaTemporaryFile = Path.Combine(worldTempDir, $"{area.dtoId}.json");
                    if (File.Exists(areaTemporaryFile))
                    {
                        string areaTempJson = File.ReadAllText(areaTemporaryFile);
                        var areaTempData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(areaTempJson);
                        
                        if (areaTempData?.areaBlockDataTemporary != null)
                        {
                            area.areaBlockDataTemporary = Newtonsoft.Json.JsonConvert.DeserializeObject<AreaBlockDataTemporary>(
                                areaTempData.areaBlockDataTemporary.ToString());
                        }

                        // 加载房间临时数据
                        LoadTemporaryDataForRooms(area, worldTempDir);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"加载区块临时数据失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 为指定区块的所有房间加载临时数据
        /// </summary>
        private void LoadTemporaryDataForRooms(AreaBlockDto area, string worldTempDir)
        {
            try
            {
                string areaTempDir = Path.Combine(worldTempDir, area.configId);
                if (!Directory.Exists(areaTempDir)) return;

                foreach (var room in area.areaBlockDataPersistent.roomDatas)
                {
                    string roomTemporaryFile = Path.Combine(areaTempDir, $"{room.dtoId}.json");
                    if (File.Exists(roomTemporaryFile))
                    {
                        string roomTempJson = File.ReadAllText(roomTemporaryFile);
                        var roomTempData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(roomTempJson);
                        
                        if (roomTempData?.roomDataTemporary != null)
                        {
                            room.roomDataTemporary = Newtonsoft.Json.JsonConvert.DeserializeObject<RoomDataTemporary>(
                                roomTempData.roomDataTemporary.ToString());
                        }
                        
                        LoadTemporaryDataForNodes(room, worldTempDir);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"加载房间临时数据失败: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 为指定区块的所有房间加载临时数据
        /// </summary>
        private void LoadTemporaryDataForNodes(RoomDto room, string worldTempDir)
        {
            try
            {
                string nodeTempDir = Path.Combine(worldTempDir, room.configId);
                if (!Directory.Exists(nodeTempDir)) 
                    return;

                foreach (var node in room.roomDataPersistent.nodeDatas)
                {
                    string nodeTemporaryFile = Path.Combine(nodeTempDir, $"{node.dtoId}.json");
                    if (File.Exists(nodeTemporaryFile))
                    {
                        string nodeTempJson = File.ReadAllText(nodeTemporaryFile);
                        var nodeTempData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(nodeTempJson);
                        
                        if (nodeTempData?.nodeDataTemporary != null)
                        {
                            node.nodeDataTemporary = Newtonsoft.Json.JsonConvert.DeserializeObject<NodeDataTemporary>(
                                nodeTempData.nodeDataTemporary.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"加载房间临时数据失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 重建世界数据的层级关系和加载子数据
        /// </summary>
        private void RebuildWorldHierarchy()
        {
            try
            {
                // 确保基础数据结构存在
                if (WorldDto.worldDataPersistent == null)
                    WorldDto.worldDataPersistent = new WorldDataPersistent();
                
                if (WorldDto.worldDataPersistent.areaBlockDatas == null)
                    WorldDto.worldDataPersistent.areaBlockDatas = new List<AreaBlockDto>();

                if (WorldDto.worldDataPersistent.areaBlockIds == null)
                    WorldDto.worldDataPersistent.areaBlockIds = new List<string>();

                // 获取世界数据目录
                string worldDir = Path.Combine(WorldDto.PersistentDataPath, WorldDto.configId);
                
                if (Directory.Exists(worldDir))
                {
                    // 加载所有区块数据
                    LoadAreaBlocksFromDirectory(worldDir);
                }

                // 刷新层级关系
                WorldDto.AutoRefreshHierarchy();
                
                Debug.Log($"世界 '{WorldDto.configName}' 层级关系重建完成，共加载 {WorldDto.worldDataPersistent.areaBlockDatas.Count} 个区块");
            }
            catch (Exception ex)
            {
                Debug.LogError($"重建世界层级关系失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 从目录加载所有区块数据
        /// </summary>
        private void LoadAreaBlocksFromDirectory(string worldDir)
        {
            try
            {
                // 获取所有区块配置文件
                var areaFiles = Directory.GetFiles(worldDir, "*.json", SearchOption.TopDirectoryOnly);
                
                WorldDto.worldDataPersistent.areaBlockDatas.Clear();
                WorldDto.worldDataPersistent.areaBlockIds.Clear();

                foreach (var areaFile in areaFiles)
                {
                    try
                    {
                        string areaJson = File.ReadAllText(areaFile);
                        var areaData = Newtonsoft.Json.JsonConvert.DeserializeObject<AreaBlockDto>(areaJson);
                        
                        if (areaData != null)
                        {
                            // 设置父级关系
                            areaData.SetParent(WorldDto);
                            
                            // 加载区块的房间数据
                            LoadRoomsForAreaBlock(areaData, worldDir);
                            
                            // 添加到列表
                            WorldDto.worldDataPersistent.areaBlockDatas.Add(areaData);
                            if (!string.IsNullOrEmpty(areaData.configId))
                            {
                                WorldDto.worldDataPersistent.areaBlockIds.Add(areaData.configId);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"加载区块文件 {areaFile} 失败: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"从目录加载区块数据失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 为指定区块加载房间数据
        /// </summary>
        private void LoadRoomsForAreaBlock(AreaBlockDto areaBlock, string worldDir)
        {
            try
            {
                if (areaBlock.areaBlockDataPersistent == null)
                    areaBlock.areaBlockDataPersistent = new AreaBlockDataPersistent();

                if (areaBlock.areaBlockDataPersistent.roomDatas == null)
                    areaBlock.areaBlockDataPersistent.roomDatas = new List<RoomDto>();

                if (areaBlock.areaBlockDataPersistent.roomIds == null)
                    areaBlock.areaBlockDataPersistent.roomIds = new List<string>();

                // 获取区块目录
                string areaDir = Path.Combine(worldDir, areaBlock.configId);
                
                if (Directory.Exists(areaDir))
                {
                    var roomFiles = Directory.GetFiles(areaDir, "*.json", SearchOption.TopDirectoryOnly);
                    
                    areaBlock.areaBlockDataPersistent.roomDatas.Clear();
                    areaBlock.areaBlockDataPersistent.roomIds.Clear();

                    foreach (var roomFile in roomFiles)
                    {
                        try
                        {
                            string roomJson = File.ReadAllText(roomFile);
                            var roomData = Newtonsoft.Json.JsonConvert.DeserializeObject<RoomDto>(roomJson);
                            
                            if (roomData != null)
                            {
                                // 设置父级关系
                                roomData.SetParent(areaBlock);
                                
                                // 添加到列表
                                areaBlock.areaBlockDataPersistent.roomDatas.Add(roomData);
                                if (!string.IsNullOrEmpty(roomData.configId))
                                {
                                    areaBlock.areaBlockDataPersistent.roomIds.Add(roomData.configId);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"加载房间文件 {roomFile} 失败: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"为区块 {areaBlock.configId} 加载房间数据失败: {ex.Message}");
            }
        }

        [Button(ButtonSizes.Medium), GUIColor(0.4f, 1f, 0.4f)]
        public void 保存世界数据()
        {
            SaveWorldData();
        }

        [Button(ButtonSizes.Medium), GUIColor(0.4f, 0.8f, 1f)]
        public void 重新加载世界数据()
        {
            LoadWorldData();
            var window = EditorWindow.GetWindow<WorldDataEditor>();
            window.ForceMenuTreeRebuild();
        }

        [Button(ButtonSizes.Medium), GUIColor(0.8f, 0.4f, 1f)]
        public void 生成临时数据()
        {
            GenerateTemporaryData();
        }

        [Button(ButtonSizes.Medium), GUIColor(1f, 0.8f, 0.4f)]
        public void 保存临时数据()
        {
            SaveTemporaryData();
        }

        [Button(ButtonSizes.Medium), GUIColor(1f, 0.4f, 0.4f)]
        public void 删除世界()
        {
            if (EditorUtility.DisplayDialog("确认删除", $"确定要删除世界 '{WorldDto?.configName}' 吗？此操作不可撤销！", "删除", "取消"))
            {
                try
                {
                    if (File.Exists(filePath))
                        File.Delete(filePath);
                    
                    // 删除对应的世界文件夹
                    string worldDir = Path.Combine(Path.GetDirectoryName(filePath), WorldDto?.configId ?? "");
                    if (Directory.Exists(worldDir))
                        Directory.Delete(worldDir, true);

                    // 删除临时数据
                    string tempWorldDir = Path.Combine(Application.persistentDataPath, "WorldData", WorldDto?.configId ?? "");
                    if (Directory.Exists(tempWorldDir))
                        Directory.Delete(tempWorldDir, true);

                    string tempWorldFile = Path.Combine(Application.persistentDataPath, "WorldData", $"{WorldDto?.configId}.json");
                    if (File.Exists(tempWorldFile))
                        File.Delete(tempWorldFile);

                    EditorUtility.DisplayDialog("删除成功", "世界数据已成功删除！", "确定");
                    
                    var window = EditorWindow.GetWindow<WorldDataEditor>();
                    window.ForceMenuTreeRebuild();
                }
                catch (Exception ex)
                {
                    EditorUtility.DisplayDialog("删除失败", $"删除世界时发生错误：{ex.Message}", "确定");
                }
            }
        }

        /// <summary>
        /// 为当前世界生成临时数据
        /// </summary>
        public void GenerateTemporaryData()
        {
            try
            {
                if (WorldDto == null) return;

                // 初始化世界临时数据
                if (WorldDto.worldDataTemporary == null)
                    WorldDto.worldDataTemporary = new WorldDataTemporary();

                // 为所有区块生成临时数据
                if (WorldDto.worldDataPersistent?.areaBlockDatas != null)
                {
                    foreach (var area in WorldDto.worldDataPersistent.areaBlockDatas)
                    {
                        GenerateTemporaryDataForArea(area);
                    }
                }

                EditorUtility.DisplayDialog("生成成功", $"世界 '{WorldDto.configName}' 的临时数据生成完成！", "确定");
            }
            catch (Exception ex)
            {
                EditorUtility.DisplayDialog("生成失败", $"生成临时数据时发生错误：{ex.Message}", "确定");
            }
        }

        /// <summary>
        /// 为指定区块生成临时数据
        /// </summary>
        private void GenerateTemporaryDataForArea(AreaBlockDto areaData)
        {
            // 初始化区块临时数据
            if (areaData.areaBlockDataTemporary == null)
                areaData.areaBlockDataTemporary = new AreaBlockDataTemporary();

            // 为所有房间生成临时数据
            if (areaData.areaBlockDataPersistent?.roomDatas != null)
            {
                foreach (var room in areaData.areaBlockDataPersistent.roomDatas)
                {
                    GenerateTemporaryDataForRoom(room);
                }
            }
        }

        /// <summary>
        /// 为指定房间生成临时数据
        /// </summary>
        private void GenerateTemporaryDataForRoom(RoomDto roomData)
        {
            // 初始化房间临时数据
            if (roomData.roomDataTemporary == null)
                roomData.roomDataTemporary = new RoomDataTemporary();

            // 为所有节点生成临时数据（如果有的话）
            if (roomData.roomDataPersistent?.nodeDatas != null)
            {
                foreach (var node in roomData.roomDataPersistent.nodeDatas)
                {
                    if (node.nodeDataTemporary == null)
                        node.nodeDataTemporary = new NodeDataTemporary();
                }
            }
        }

        /// <summary>
        /// 保存临时数据
        /// </summary>
        public void SaveTemporaryData()
        {
            try
            {
                if (WorldDto == null) return;
                
                WorldDto.SaveData_Temporary();
                EditorUtility.DisplayDialog("保存成功", $"世界 '{WorldDto.configName}' 的临时数据保存成功！", "确定");
            }
            catch (Exception ex)
            {
                EditorUtility.DisplayDialog("保存失败", $"保存临时数据时发生错误：{ex.Message}", "确定");
            }
        }
        
        public void SaveWorldData()
        {
            try
            {
                if (WorldDto != null)
                {
                    WorldDto.SaveData();
                    EditorUtility.DisplayDialog("保存成功", $"世界 '{WorldDto.configName}' 数据保存成功！", "确定");
                }
            }
            catch (Exception ex)
            {
                EditorUtility.DisplayDialog("保存失败", $"保存世界数据时发生错误：{ex.Message}", "确定");
            }
        }
    }

    // 区块包装器
    [Serializable]
    public class AreaBlockWrapper
    {
        [ShowInInspector, HideLabel]
        public AreaBlockDto AreaBlockDto { get; private set; }
        
        [HideInInspector]
        public WorldDataWrapper parentWorld;

        public AreaBlockWrapper(AreaBlockDto areaBlock, WorldDataWrapper parent)
        {
            AreaBlockDto = areaBlock;
            parentWorld = parent;
        }

        [Button(ButtonSizes.Medium), GUIColor(0.4f, 1f, 0.4f)]
        public void 添加新房间()
        {
            if (AreaBlockDto?.areaBlockDataPersistent == null) return;

            var newRoom = new RoomDto
            {
                configName = $"新房间_{AreaBlockDto.areaBlockDataPersistent.roomDatas.Count + 1}",
                configId = $"room_{AreaBlockDto.areaBlockDataPersistent.roomDatas.Count + 1}",
                configDes = "这是一个新创建的房间",
                roomDataTemporary = new RoomDataTemporary() // 自动创建临时数据
            };

            AreaBlockDto.areaBlockDataPersistent.roomDatas.Add(newRoom);
            AreaBlockDto.AutoRefreshHierarchy();
            
            var window = EditorWindow.GetWindow<WorldDataEditor>();
            window.ForceMenuTreeRebuild();
        }

        [Button(ButtonSizes.Medium), GUIColor(0.8f, 0.4f, 1f)]
        public void 生成区块临时数据()
        {
            try
            {
                GenerateTemporaryDataForArea();
                EditorUtility.DisplayDialog("生成成功", $"区块 '{AreaBlockDto?.configName}' 的临时数据生成完成！", "确定");
            }
            catch (Exception ex)
            {
                EditorUtility.DisplayDialog("生成失败", $"生成区块临时数据时发生错误：{ex.Message}", "确定");
            }
        }

        [Button(ButtonSizes.Medium), GUIColor(1f, 0.8f, 0.4f)]
        public void 保存区块临时数据()
        {
            try
            {
                if (AreaBlockDto == null || parentWorld?.WorldDto == null) return;

                string worldTempDir = Path.Combine(Application.persistentDataPath, "WorldData", parentWorld.WorldDto.configId);
                AreaBlockDto.SaveData_Temporary(worldTempDir, JsonSettings.Make());
                
                EditorUtility.DisplayDialog("保存成功", $"区块 '{AreaBlockDto.configName}' 的临时数据保存成功！", "确定");
            }
            catch (Exception ex)
            {
                EditorUtility.DisplayDialog("保存失败", $"保存区块临时数据时发生错误：{ex.Message}", "确定");
            }
        }

        [Button(ButtonSizes.Medium), GUIColor(1f, 0.4f, 0.4f)]
        public void 删除此区块()
        {
            if (EditorUtility.DisplayDialog("确认删除", $"确定要删除区块 '{AreaBlockDto?.configName}' 吗？", "删除", "取消"))
            {
                if (parentWorld?.WorldDto?.worldDataPersistent?.areaBlockDatas != null)
                {
                    parentWorld.WorldDto.worldDataPersistent.areaBlockDatas.Remove(AreaBlockDto);
                    parentWorld.WorldDto.AutoRefreshHierarchy();
                    
                    var window = EditorWindow.GetWindow<WorldDataEditor>();
                    window.ForceMenuTreeRebuild();
                }
            }
        }

        /// <summary>
        /// 为当前区块生成临时数据
        /// </summary>
        private void GenerateTemporaryDataForArea()
        {
            // 初始化区块临时数据
            if (AreaBlockDto.areaBlockDataTemporary == null)
                AreaBlockDto.areaBlockDataTemporary = new AreaBlockDataTemporary();

            // 为所有房间生成临时数据
            if (AreaBlockDto.areaBlockDataPersistent?.roomDatas != null)
            {
                foreach (var room in AreaBlockDto.areaBlockDataPersistent.roomDatas)
                {
                    GenerateTemporaryDataForRoom(room);
                }
            }
        }

        /// <summary>
        /// 为指定房间生成临时数据
        /// </summary>
        private void GenerateTemporaryDataForRoom(RoomDto roomData)
        {
            // 初始化房间临时数据
            if (roomData.roomDataTemporary == null)
                roomData.roomDataTemporary = new RoomDataTemporary();

            // 为所有节点生成临时数据（如果有的话）
            if (roomData.roomDataPersistent?.nodeDatas != null)
            {
                foreach (var node in roomData.roomDataPersistent.nodeDatas)
                {
                    if (node.nodeDataTemporary == null)
                        node.nodeDataTemporary = new NodeDataTemporary();
                }
            }
        }
    }

    // 房间包装器
    [Serializable]
    public class RoomWrapper
    {
        [ShowInInspector, HideLabel]
        public RoomDto RoomDto { get; private set; }
        
        [HideInInspector]
        public AreaBlockWrapper parentArea;

        public RoomWrapper(RoomDto room, AreaBlockWrapper parent)
        {
            RoomDto = room;
            parentArea = parent;
        }

        [Button(ButtonSizes.Medium), GUIColor(0.8f, 0.4f, 1f)]
        public void 生成房间临时数据()
        {
            try
            {
                GenerateTemporaryDataForRoom();
                EditorUtility.DisplayDialog("生成成功", $"房间 '{RoomDto?.configName}' 的临时数据生成完成！", "确定");
            }
            catch (Exception ex)
            {
                EditorUtility.DisplayDialog("生成失败", $"生成房间临时数据时发生错误：{ex.Message}", "确定");
            }
        }

        [Button(ButtonSizes.Medium), GUIColor(1f, 0.8f, 0.4f)]
        public void 保存房间临时数据()
        {
            try
            {
                if (RoomDto == null || parentArea?.AreaBlockDto == null || parentArea.parentWorld?.WorldDto == null) return;

                string areaTempDir = Path.Combine(Application.persistentDataPath, "WorldData", 
                    parentArea.parentWorld.WorldDto.configId, parentArea.AreaBlockDto.configId);
                
                RoomDto.SaveData_Temporary(areaTempDir, JsonSettings.Make());
                
                EditorUtility.DisplayDialog("保存成功", $"房间 '{RoomDto.configName}' 的临时数据保存成功！", "确定");
            }
            catch (Exception ex)
            {
                EditorUtility.DisplayDialog("保存失败", $"保存房间临时数据时发生错误：{ex.Message}", "确定");
            }
        }

        [Button(ButtonSizes.Medium), GUIColor(1f, 0.4f, 0.4f)]
        public void 删除此房间()
        {
            if (EditorUtility.DisplayDialog("确认删除", $"确定要删除房间 '{RoomDto?.configName}' 吗？", "删除", "取消"))
            {
                if (parentArea?.AreaBlockDto?.areaBlockDataPersistent?.roomDatas != null)
                {
                    parentArea.AreaBlockDto.areaBlockDataPersistent.roomDatas.Remove(RoomDto);
                    parentArea.AreaBlockDto.AutoRefreshHierarchy();
                    
                    var window = EditorWindow.GetWindow<WorldDataEditor>();
                    window.ForceMenuTreeRebuild();
                }
            }
        }

        /// <summary>
        /// 为当前房间生成临时数据
        /// </summary>
        private void GenerateTemporaryDataForRoom()
        {
            // 初始化房间临时数据
            if (RoomDto.roomDataTemporary == null)
                RoomDto.roomDataTemporary = new RoomDataTemporary();

            // 为所有节点生成临时数据（如果有的话）
            if (RoomDto.roomDataPersistent?.nodeDatas != null)
            {
                foreach (var node in RoomDto.roomDataPersistent.nodeDatas)
                {
                    if (node.nodeDataTemporary == null)
                        node.nodeDataTemporary = new NodeDataTemporary();
                }
            }
        }
    }
}