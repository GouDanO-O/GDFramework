using UnityEngine;
using UnityEngine.UI;
using GDFrameworkExtend.UIKit;
using GDFrameworkCore;
using Core.Game.Chunk.Universe.Data;
using Core.Game.Chunk.World.Data;
using System.Collections.Generic;
using Core.Game.Procedure.Resource;
using TMPro;

namespace Core.Game.View
{
    /// <summary>
    /// 宇宙编辑面板 - 二级编辑面板
    /// 复用 UI_UniversePanel 的布局，提供可编辑的世界星图
    /// </summary>
    public class UI_UniverseEditorPanelData : UIPanelData
    {
        public UniverseDtoDef EditingUniverse;
    }

    public partial class UI_UniverseEditorPanel : UIPanel, ICanGetModel
    {
        #region UI Components (复用 UI_UniversePanel 的结构)

        protected Transform UniverseMap;
        protected Transform UniverseMapCenter;
        protected Transform UniverseMapCenterContentRoot; // 世界节点的容器
        protected Transform UniverseMapHeader;
        protected Transform UniverseMapDowner;

        // Header 组件
        protected TextMeshProUGUI UniverseMapNameText;
        protected TextMeshProUGUI FocusWorldName;

        // Downer 组件（改为编辑操作）
        protected Button SaveMapButton; // 保存地图布局
        protected Button AddWorldButton; // 添加世界到地图
        protected Button BackToListButton; // 返回列表
        protected Button EditWorldDetailButton; // 编辑世界详情

        // 控制面板
        protected Transform EditorControlPanel;
        protected TMP_InputField GridSizeInput;
        protected Toggle GridSnapToggle;
        protected TextMeshProUGUI CoordinateDisplay;
        protected Button ClearAllWorldsButton;

        #endregion

        #region Private Fields

        private UniverseDtoDef _editingUniverse;
        private WorldDataModel _worldDataModel;
        private UniverseDataModel _universeDataModel;

        private GameObject _worldNodePrefab;
        private List<UI_EditableWorldNode> _currentWorldNodes = new List<UI_EditableWorldNode>();

        private WorldDtoDef _currentSelectedWorld;
        private float _gridSize = 50f;
        private bool _enableGridSnap = true;

        #endregion

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as UI_UniverseEditorPanelData ?? new UI_UniverseEditorPanelData();

            _editingUniverse = (mData as UI_UniverseEditorPanelData)?.EditingUniverse;

            _worldDataModel = this.GetModel<WorldDataModel>();
            _universeDataModel = this.GetModel<UniverseDataModel>();

            GetRelyComponent();
            InitPrefabs();
            RegisterEvent();
        }

        protected override void GetRelyComponent()
        {
            base.GetRelyComponent();

            // 复用 UI_UniversePanel 的结构
            UniverseMap = Common.Find("UniverseMap");

            // 中心区域（世界星图）
            UniverseMapCenter = UniverseMap.Find("UniverseMapCenter");
            UniverseMapCenterContentRoot = UniverseMapCenter.Find("Contents");

            // 头部区域
            UniverseMapHeader = UniverseMap.Find("UniverseMapHeader");
            UniverseMapNameText = UniverseMapHeader.Find("UniverseMapName/NameText").GetComponent<TextMeshProUGUI>();
            FocusWorldName = UniverseMapHeader.Find("FocusWorld/WorldName").GetComponent<TextMeshProUGUI>();

            // 底部操作区域（需要替换按钮）
            UniverseMapDowner = UniverseMap.Find("UniverseMapDowner");

            // 这里假设原来的按钮被替换为编辑器专用按钮
            // 如果没有，需要在 Unity 中手动调整，或者通过代码动态创建
            SaveMapButton = UniverseMapDowner.Find("SaveMapButton").GetComponent<Button>();
            AddWorldButton = UniverseMapDowner.Find("AddWorldButton").GetComponent<Button>();
            BackToListButton = UniverseMapDowner.Find("BackToListButton").GetComponent<Button>();
            EditWorldDetailButton = UniverseMapDowner.Find("EditWorldDetailButton").GetComponent<Button>();

            // 编辑器控制面板（如果有）
            EditorControlPanel = UniverseMapDowner.Find("EditorControlPanel");
            if (EditorControlPanel != null)
            {
                GridSizeInput = EditorControlPanel.Find("GridSizeInput")?.GetComponent<TMP_InputField>();
                GridSnapToggle = EditorControlPanel.Find("GridSnapToggle")?.GetComponent<Toggle>();
                CoordinateDisplay = EditorControlPanel.Find("CoordinateDisplay")?.GetComponent<TextMeshProUGUI>();
                ClearAllWorldsButton = EditorControlPanel.Find("ClearButton")?.GetComponent<Button>();
            }
        }

        protected void InitPrefabs()
        {
            _worldNodePrefab = Resources.Load<GameObject>("UI/Prefabs/EditableWorldNode");
            if (_worldNodePrefab == null)
            {
                _worldNodePrefab = CreateDefaultWorldNodePrefab();
            }
        }

        protected override void RegisterEvent()
        {
            base.RegisterEvent();

            SaveMapButton.onClick.AddListener(SaveWorldMapPositions);
            AddWorldButton.onClick.AddListener(OpenAddWorldDialog);
            BackToListButton.onClick.AddListener(BackToUniverseList);
            EditWorldDetailButton.onClick.AddListener(EditSelectedWorldDetail);

            if (GridSizeInput != null)
                GridSizeInput.onValueChanged.AddListener(OnGridSizeChanged);
            if (GridSnapToggle != null)
                GridSnapToggle.onValueChanged.AddListener(OnGridSnapToggled);
            if (ClearAllWorldsButton != null)
                ClearAllWorldsButton.onClick.AddListener(ClearAllWorldNodes);
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            if (_editingUniverse == null)
            {
                Debug.LogError("没有指定要编辑的宇宙！");
                this.CloseSelf();
                return;
            }

            // 显示宇宙名称
            if (UniverseMapNameText != null)
            {
                UniverseMapNameText.text = _editingUniverse.DefName;
            }

            // 加载世界星图
            RefreshWorldMap();
        }

        protected override void OnShow()
        {
        }

        protected override void OnHide()
        {
        }

        protected override void OnClose()
        {
            ClearAllWorldNodes();
            _currentSelectedWorld = null;
        }

        #region World Map Management

        /// <summary>
        /// 刷新世界地图
        /// </summary>
        private void RefreshWorldMap()
        {
            ClearAllWorldNodes();

            if (_editingUniverse == null || _editingUniverse.WorldIdList == null)
            {
                Debug.Log("当前宇宙没有世界");
                return;
            }

            // 为每个世界创建节点
            foreach (var worldId in _editingUniverse.WorldIdList)
            {
                var worldDef = _worldDataModel.GetDefById(worldId);
                if (worldDef != null)
                {
                    CreateWorldNode(worldDef);
                }
            }

            Debug.Log($"加载宇宙地图: {_editingUniverse.DefName}, 共 {_currentWorldNodes.Count} 个世界");
        }

        /// <summary>
        /// 创建世界节点
        /// </summary>
        private UI_EditableWorldNode CreateWorldNode(WorldDtoDef worldDef)
        {
            var nodeObj = Instantiate(_worldNodePrefab, UniverseMapCenterContentRoot);
            var worldNode = nodeObj.GetComponent<UI_EditableWorldNode>();

            if (worldNode == null)
            {
                worldNode = nodeObj.AddComponent<UI_EditableWorldNode>();
            }

            worldNode.Initialize(worldDef, this);
            worldNode.OnWorldNodeClicked += OnWorldNodeSelected;

            _currentWorldNodes.Add(worldNode);

            return worldNode;
        }

        /// <summary>
        /// 世界节点被选中
        /// </summary>
        private void OnWorldNodeSelected(WorldDtoDef worldDef)
        {
            _currentSelectedWorld = worldDef;

            if (FocusWorldName != null)
            {
                FocusWorldName.text = worldDef.DefName;
            }

            Debug.Log($"<color=cyan>选中世界: {worldDef.DefName}</color>");
        }

        /// <summary>
        /// 清空所有世界节点
        /// </summary>
        private void ClearAllWorldNodes()
        {
            foreach (var node in _currentWorldNodes)
            {
                if (node != null)
                {
                    Destroy(node.gameObject);
                }
            }

            _currentWorldNodes.Clear();

            _currentSelectedWorld = null;
            if (FocusWorldName != null)
            {
                FocusWorldName.text = "未选择";
            }
        }

        /// <summary>
        /// 保存世界地图位置
        /// </summary>
        private void SaveWorldMapPositions()
        {
            int savedCount = 0;

            foreach (var worldNode in _currentWorldNodes)
            {
                if (worldNode != null && worldNode.WorldDef != null)
                {
                    worldNode.SavePosition();
                    savedCount++;
                }
            }

            Debug.Log($"<color=green>✓ 保存宇宙地图: {savedCount} 个世界位置已保存</color>");
        }

        #endregion

        #region World Operations

        /// <summary>
        /// 打开添加世界对话框
        /// </summary>
        private void OpenAddWorldDialog()
        {
            // TODO: 打开世界选择对话框
            // 从所有可用的世界中选择，添加到当前宇宙
            Debug.Log("打开添加世界对话框（待实现）");

            // 临时实现：直接创建一个新世界
            CreateNewWorldForUniverse();
        }

        /// <summary>
        /// 为当前宇宙创建新世界
        /// </summary>
        private void CreateNewWorldForUniverse()
        {
            var newWorld = new WorldDtoDef
            {
                DefName = $"新世界_{_editingUniverse.WorldIdList.Count + 1}",
                DefDescription = "新创建的世界",
                InitialPlayerLocateRegionId = "",
                InitialShowingRegionIdList = new List<string>(),
                RegionIdList = new List<string>(),
                InitialSpawnedPosition = Vector2.zero // 默认位置
            };

            // 添加到世界数据模型
            var context = new LaunchResourcesLoader.HierarchyContext
            {
                UniverseName = _editingUniverse.DefName,
                WorldName = newWorld.DefName
            };
            _worldDataModel.AddDtoDef(context, newWorld);
            newWorld.SaveThisDef();

            // 添加到当前宇宙的世界列表
            if (_editingUniverse.WorldIdList == null)
            {
                _editingUniverse.WorldIdList = new List<string>();
            }

            _editingUniverse.WorldIdList.Add(newWorld.DefId);
            _editingUniverse.SaveThisDef();

            Debug.Log($"<color=green>✓ 创建新世界: {newWorld.DefName} ({newWorld.DefId})</color>");

            // 刷新地图
            RefreshWorldMap();
        }

        /// <summary>
        /// 编辑选中的世界详情
        /// </summary>
        private void EditSelectedWorldDetail()
        {
            if (_currentSelectedWorld == null)
            {
                Debug.LogWarning("请先选择一个世界");
                return;
            }

            // TODO: 打开世界详情编辑面板
            Debug.Log($"编辑世界详情: {_currentSelectedWorld.DefName}（待实现）");
        }

        /// <summary>
        /// 返回宇宙列表
        /// </summary>
        private void BackToUniverseList()
        {
            this.CloseSelf();
        }

        #endregion

        #region Grid & Snap

        private void OnGridSizeChanged(string value)
        {
            if (float.TryParse(value, out float newSize))
            {
                _gridSize = Mathf.Max(10f, newSize);
                Debug.Log($"网格大小: {_gridSize}");
            }
        }

        private void OnGridSnapToggled(bool enabled)
        {
            _enableGridSnap = enabled;
            Debug.Log($"网格吸附: {(_enableGridSnap ? "开启" : "关闭")}");
        }

        /// <summary>
        /// 对齐到网格
        /// </summary>
        public Vector2 SnapToGrid(Vector2 position)
        {
            if (!_enableGridSnap)
            {
                return position;
            }

            float snappedX = Mathf.Round(position.x / _gridSize) * _gridSize;
            float snappedY = Mathf.Round(position.y / _gridSize) * _gridSize;

            return new Vector2(snappedX, snappedY);
        }

        /// <summary>
        /// 更新坐标显示
        /// </summary>
        public void UpdateCoordinateDisplay(Vector2 position)
        {
            if (CoordinateDisplay != null)
            {
                CoordinateDisplay.text = $"X: {position.x:F0}, Y: {position.y:F0}";
            }
        }

        #endregion

        #region Prefab Creation

        private GameObject CreateDefaultWorldNodePrefab()
        {
            GameObject obj = new GameObject("EditableWorldNode");

            RectTransform rect = obj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(100, 100);

            Image bg = obj.AddComponent<Image>();
            bg.color = new Color(0.2f, 0.4f, 0.8f, 0.8f);

            // World Name
            GameObject textObj = new GameObject("WorldName");
            textObj.transform.SetParent(obj.transform, false);

            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
            text.text = "World";
            text.alignment = TextAlignmentOptions.Center;
            text.fontSize = 14;
            text.color = Color.white;

            RectTransform textRect = text.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(5, 5);
            textRect.offsetMax = new Vector2(-5, -5);

            // Position
            GameObject posObj = new GameObject("Position");
            posObj.transform.SetParent(obj.transform, false);

            TextMeshProUGUI posText = posObj.AddComponent<TextMeshProUGUI>();
            posText.text = "(0, 0)";
            posText.alignment = TextAlignmentOptions.Center;
            posText.fontSize = 10;
            posText.color = new Color(0.8f, 0.8f, 0.8f);

            RectTransform posRect = posText.GetComponent<RectTransform>();
            posRect.anchorMin = new Vector2(0, 0);
            posRect.anchorMax = new Vector2(1, 0);
            posRect.pivot = new Vector2(0.5f, 0);
            posRect.sizeDelta = new Vector2(0, 20);
            posRect.anchoredPosition = new Vector2(0, -25);

            return obj;
        }

        #endregion
    }
}