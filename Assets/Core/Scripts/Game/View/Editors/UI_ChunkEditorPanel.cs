using UnityEngine;
using UnityEngine.UI;
using GDFrameworkExtend.UIKit;
using GDFrameworkCore;
using Core.Game.Chunk.Universe.Data;
using Core.Game.Chunk.World.Data;
using Core.Game.Procedure.Models.Resource;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using Core.Game.Procedure.Resource;

namespace Core.Game.View
{
    public class UI_ChunkEditorPanelData : UIPanelData
    {
    }

    public partial class UI_ChunkEditorPanel : UIPanel, ICanGetModel
    {
        #region UI Components

        protected Transform ChunkEditorRoot;

        protected Transform HeaderRoot;
        protected Button UniverseEditorButton;
        protected Button WorldEditorButton;
        protected Button ExitButton;

        // 左侧列表区域
        protected Transform ListRoot;
        protected ScrollRect UniverseListScrollView;
        protected Transform UniverseListContent;
        protected ScrollRect WorldListScrollView;
        protected Transform WorldListContent;

        // 右侧编辑区域
        protected Transform EditorRoot;
        protected Transform UniverseEditorRoot;
        protected Transform WorldEditorRoot;

        // 宇宙编辑器组件
        protected TMP_InputField UniverseDefIdInput;
        protected TMP_InputField UniverseDefNameInput;
        protected TMP_InputField UniverseDefDescInput;
        protected TMP_InputField InitialPlayerWorldIdInput;
        protected Transform InitialShowingWorldListContent;
        protected Transform WorldIdListContent;
        protected Button AddInitialShowingWorldButton;
        protected Button AddWorldIdButton;
        protected Button SaveUniverseButton;
        protected Button CreateNewUniverseButton;
        protected Button DeleteUniverseButton;

        protected Transform UniverseVisualEditorRoot;
        protected Transform UniverseMapCanvas;
        protected Transform UniverseWorldNodesContainer;
        protected Button SaveUniverseMapButton;
        protected Button AddWorldToMapButton;
        protected Button ClearAllWorldsButton;
        protected TMP_InputField GridSizeInput;
        protected Toggle GridSnapToggle;
        protected TextMeshProUGUI CoordinateDisplay;

        // 世界节点预制体
        private GameObject _worldNodePrefab;

        // 当前显示的世界节点
        private List<UI_EditableWorldNode> _currentWorldNodes = new List<UI_EditableWorldNode>();

        // 编辑状态
        private bool _isEditMode = false;
        private float _gridSize = 50f;
        private bool _enableGridSnap = true;

        // 世界编辑器组件
        protected TMP_InputField WorldDefIdInput;
        protected TMP_InputField WorldDefNameInput;
        protected TMP_InputField WorldDefDescInput;
        protected TMP_Dropdown WorldUniverseDropdown;
        protected TMP_InputField InitialPlayerRegionIdInput;
        protected Transform InitialShowingRegionListContent;
        protected Transform RegionIdListContent;
        protected Button AddInitialShowingRegionButton;
        protected Button AddRegionIdButton;
        protected Button SaveWorldButton;
        protected Button CreateNewWorldButton;
        protected Button DeleteWorldButton;

        // 当前编辑状态
        private EditorMode _currentMode = EditorMode.Universe;
        private UniverseDtoDef _currentEditingUniverseDef;
        private WorldDtoDef _currentEditingWorldDef;
        private UniverseDataModel _universeDataModel;
        private WorldDataModel _worldDataModel;

        // 预制体
        private GameObject _universeListItemPrefab;
        private GameObject _worldListItemPrefab;
        private GameObject _stringListItemPrefab;

        private enum EditorMode
        {
            Universe,
            UniverseVisual, // 宇宙可视化编辑
            World,
            WorldVisual, // 世界可视化编辑
            Region,
            Dungeon,
            Room
        }

        #endregion

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as UI_ChunkEditorPanelData ?? new UI_ChunkEditorPanelData();

            _universeDataModel = this.GetModel<UniverseDataModel>();
            _worldDataModel = this.GetModel<WorldDataModel>();

            GetRelyComponent();
            InitPrefabs();
            RegisterEvent();
        }

        protected override void GetRelyComponent()
        {
            base.GetRelyComponent();

            ChunkEditorRoot = Common.Find("ChunkEditorRoot");

            // Header
            HeaderRoot = ChunkEditorRoot.Find("HeaderRoot");
            UniverseEditorButton = HeaderRoot.Find("UniverseEditorButton").GetComponent<Button>();
            WorldEditorButton = HeaderRoot.Find("WorldEditorButton").GetComponent<Button>();
            ExitButton = HeaderRoot.Find("ExitButton").GetComponent<Button>();

            // 左侧列表
            ListRoot = ChunkEditorRoot.Find("ListRoot");
            UniverseListScrollView = ListRoot.Find("UniverseListScrollView").GetComponent<ScrollRect>();
            UniverseListContent = UniverseListScrollView.content;
            WorldListScrollView = ListRoot.Find("WorldListScrollView").GetComponent<ScrollRect>();
            WorldListContent = WorldListScrollView.content;

            // 右侧编辑器
            EditorRoot = ChunkEditorRoot.Find("EditorRoot");

            // 宇宙编辑器
            UniverseEditorRoot = EditorRoot.Find("UniverseEditorRoot");
            GetUniverseEditorComponents();

            // 世界编辑器
            WorldEditorRoot = EditorRoot.Find("WorldEditorRoot");
            GetWorldEditorComponents();
        }
        
        protected void GetUniverseEditorComponents()
        {
            // 基础编辑器组件
            var basicInfoRoot = UniverseEditorRoot.Find("Viewport/Content/BasicInfo");
            UniverseDefIdInput = basicInfoRoot.Find("DefIdInput").GetComponent<TMP_InputField>();
            UniverseDefNameInput = basicInfoRoot.Find("DefNameInput").GetComponent<TMP_InputField>();
            UniverseDefDescInput = basicInfoRoot.Find("DefDescInput").GetComponent<TMP_InputField>();

            var worldConfigRoot = UniverseEditorRoot.Find("Viewport/Content/WorldConfig");
            InitialPlayerWorldIdInput =
                worldConfigRoot.Find("InitialPlayerWorldIdInput").GetComponent<TMP_InputField>();

            var initialShowingRoot = worldConfigRoot.Find("InitialShowingWorldList");
            InitialShowingWorldListContent = initialShowingRoot.Find("ScrollView/Viewport/Content");
            AddInitialShowingWorldButton = initialShowingRoot.Find("AddButton").GetComponent<Button>();

            var worldIdListRoot = worldConfigRoot.Find("WorldIdList");
            WorldIdListContent = worldIdListRoot.Find("ScrollView/Viewport/Content");
            AddWorldIdButton = worldIdListRoot.Find("AddButton").GetComponent<Button>();

            var buttonRoot = UniverseEditorRoot.Find("Viewport/Content/ButtonRoot");
            SaveUniverseButton = buttonRoot.Find("SaveButton").GetComponent<Button>();
            CreateNewUniverseButton = buttonRoot.Find("CreateNewButton").GetComponent<Button>();
            DeleteUniverseButton = buttonRoot.Find("DeleteButton").GetComponent<Button>();

            // ===== 新增：获取可视化编辑器组件 =====
            UniverseVisualEditorRoot = EditorRoot.Find("UniverseVisualEditorRoot");

            if (UniverseVisualEditorRoot != null)
            {
                var controlPanel = UniverseVisualEditorRoot.Find("ControlPanel");
                if (controlPanel != null)
                {
                    SaveUniverseMapButton = controlPanel.Find("SaveButton").GetComponent<Button>();
                    AddWorldToMapButton = controlPanel.Find("AddWorldButton").GetComponent<Button>();
                    ClearAllWorldsButton = controlPanel.Find("ClearButton").GetComponent<Button>();
                    GridSizeInput = controlPanel.Find("GridSizeGroup/GridSizeInput").GetComponent<TMP_InputField>();
                    GridSnapToggle = controlPanel.Find("GridSnapGroup/GridSnapToggle").GetComponent<Toggle>();
                    CoordinateDisplay = controlPanel.Find("CoordinateDisplay").GetComponent<TextMeshProUGUI>();
                }

                UniverseMapCanvas = UniverseVisualEditorRoot.Find("UniverseMapCanvas");
                if (UniverseMapCanvas != null)
                {
                    UniverseWorldNodesContainer = UniverseMapCanvas.Find("WorldNodesContainer");
                }
            }
            else
            {
                Debug.LogWarning("UniverseVisualEditorRoot 未找到！请运行 GameObject/UI/Chunk Editor Content 创建UI结构");
            }
        }

        protected void GetWorldEditorComponents()
        {
            var basicInfoRoot = WorldEditorRoot.Find("BasicInfo");
            WorldDefIdInput = basicInfoRoot.Find("DefIdInput").GetComponent<TMP_InputField>();
            WorldDefNameInput = basicInfoRoot.Find("DefNameInput").GetComponent<TMP_InputField>();
            WorldDefDescInput = basicInfoRoot.Find("DefDescInput").GetComponent<TMP_InputField>();
            WorldUniverseDropdown = basicInfoRoot.Find("UniverseDropdown").GetComponent<TMP_Dropdown>();

            var regionConfigRoot = WorldEditorRoot.Find("RegionConfig");
            InitialPlayerRegionIdInput =
                regionConfigRoot.Find("InitialPlayerRegionIdInput").GetComponent<TMP_InputField>();

            var initialShowingRoot = regionConfigRoot.Find("InitialShowingRegionList");
            InitialShowingRegionListContent = initialShowingRoot.Find("ScrollView/Viewport/Content");
            AddInitialShowingRegionButton = initialShowingRoot.Find("AddButton").GetComponent<Button>();

            var regionIdListRoot = regionConfigRoot.Find("RegionIdList");
            RegionIdListContent = regionIdListRoot.Find("ScrollView/Viewport/Content");
            AddRegionIdButton = regionIdListRoot.Find("AddButton").GetComponent<Button>();

            var buttonRoot = WorldEditorRoot.Find("ButtonRoot");
            SaveWorldButton = buttonRoot.Find("SaveButton").GetComponent<Button>();
            CreateNewWorldButton = buttonRoot.Find("CreateNewButton").GetComponent<Button>();
            DeleteWorldButton = buttonRoot.Find("DeleteButton").GetComponent<Button>();
        }

        protected void InitPrefabs()
        {
            // 从Resources加载预制体
            _universeListItemPrefab = Resources.Load<GameObject>("UI/Prefabs/UniverseListItem");
            _worldListItemPrefab = Resources.Load<GameObject>("UI/Prefabs/WorldListItem");
            _stringListItemPrefab = Resources.Load<GameObject>("UI/Prefabs/StringListItem");

            // 如果没有预制体,创建默认预制体
            if (_universeListItemPrefab == null)
                _universeListItemPrefab = CreateDefaultListItemPrefab("宇宙");
            if (_worldListItemPrefab == null)
                _worldListItemPrefab = CreateDefaultListItemPrefab("世界");
            if (_stringListItemPrefab == null)
                _stringListItemPrefab = CreateDefaultStringListItemPrefab();
        }

        protected override void RegisterEvent()
        {
            base.RegisterEvent();

            ExitButton.onClick.AddListener(ExitThisPanel);
            UniverseEditorButton.onClick.AddListener(() => SwitchEditorMode(EditorMode.Universe));
            WorldEditorButton.onClick.AddListener(() => SwitchEditorMode(EditorMode.World));

            // 宇宙编辑器事件
            SaveUniverseButton.onClick.AddListener(SaveCurrentUniverse);
            CreateNewUniverseButton.onClick.AddListener(CreateNewUniverse);
            DeleteUniverseButton.onClick.AddListener(DeleteCurrentUniverse);
            AddInitialShowingWorldButton.onClick.AddListener(() => AddStringToList(InitialShowingWorldListContent, ""));
            AddWorldIdButton.onClick.AddListener(() => AddStringToList(WorldIdListContent, ""));

            SaveUniverseMapButton.onClick.AddListener(SaveUniverseMapPositions);
            AddWorldToMapButton.onClick.AddListener(OpenAddWorldDialog);
            ClearAllWorldsButton.onClick.AddListener(ClearAllWorldNodes);
            GridSizeInput.onValueChanged.AddListener(OnGridSizeChanged);
            GridSnapToggle.onValueChanged.AddListener(OnGridSnapToggled);

            // 世界编辑器事件
            SaveWorldButton.onClick.AddListener(SaveCurrentWorld);
            CreateNewWorldButton.onClick.AddListener(CreateNewWorld);
            DeleteWorldButton.onClick.AddListener(DeleteCurrentWorld);
            AddInitialShowingRegionButton.onClick.AddListener(() =>
                AddStringToList(InitialShowingRegionListContent, ""));
            AddRegionIdButton.onClick.AddListener(() => AddStringToList(RegionIdListContent, ""));
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            SwitchEditorMode(EditorMode.Universe);
        }

        protected override void OnShow()
        {
        }

        protected override void OnHide()
        {
        }

        protected override void OnClose()
        {
            _currentEditingUniverseDef = null;
            _currentEditingWorldDef = null;
        }

        protected void ExitThisPanel()
        {
            this.CloseSelf();
        }

        #region Editor Mode Switching

        private void SwitchEditorMode(EditorMode mode)
        {
            _currentMode = mode;

            // 隐藏所有列表
            UniverseListScrollView.gameObject.SetActive(false);
            WorldListScrollView.gameObject.SetActive(false);

            // 隐藏所有编辑器
            UniverseEditorRoot.gameObject.SetActive(false);
            WorldEditorRoot.gameObject.SetActive(false);

            // 显示对应编辑器
            switch (mode)
            {
                case EditorMode.Universe:
                    ShowUniverseEditor();
                    break;
                case EditorMode.UniverseVisual:
                    ShowUniverseVisualEditor();
                    break;
                case EditorMode.World:
                    ShowWorldEditor();
                    break;
                case EditorMode.Region:
                    Debug.Log("区域编辑器待实现");
                    break;
            }
        }

        #endregion

        #region Universe Editor

        private void ShowUniverseEditor()
        {
            UniverseListScrollView.gameObject.SetActive(true);
            UniverseEditorRoot.gameObject.SetActive(true);
            RefreshUniverseList();
        }

        private void RefreshUniverseList()
        {
            ClearChildren(UniverseListContent);

            var allUniverses = _universeDataModel.GetAllUniverseDefs();

            if (allUniverses == null || allUniverses.Count == 0)
            {
                Debug.Log("没有可用的宇宙配置");
                return;
            }

            foreach (var universeDef in allUniverses)
            {
                CreateUniverseListItem(universeDef);
            }

            if (_currentEditingUniverseDef == null && allUniverses.Count > 0)
            {
                SelectUniverse(allUniverses[0]);
            }
        }

        private void CreateUniverseListItem(UniverseDtoDef universeDef)
        {
            var itemObj = Instantiate(_universeListItemPrefab, UniverseListContent);

            var nameText = itemObj.transform.Find("NameText")?.GetComponent<TMP_Text>();
            if (nameText != null)
            {
                nameText.text = $"{universeDef.DefName}\n<size=12><color=#888888>{universeDef.DefId}</color></size>";
            }

            var bgImage = itemObj.GetComponent<Image>();
            if (bgImage != null)
            {
                bgImage.color = _currentEditingUniverseDef == universeDef
                    ? new Color(0.3f, 0.5f, 0.8f, 0.5f)
                    : new Color(0.2f, 0.2f, 0.2f, 0.5f);
            }

            var button = itemObj.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => SelectUniverse(universeDef));
            }
        }

        private void SelectUniverse(UniverseDtoDef universeDef)
        {
            _currentEditingUniverseDef = universeDef;
            RefreshUniverseList();
            LoadUniverseToEditor(universeDef);
        }

        private void LoadUniverseToEditor(UniverseDtoDef universeDef)
        {
            if (universeDef == null) return;

            UniverseDefIdInput.text = universeDef.DefId;
            UniverseDefNameInput.text = universeDef.DefName;
            UniverseDefDescInput.text = universeDef.DefDescription;
            InitialPlayerWorldIdInput.text = universeDef.InitialPlayerLocateWorldId ?? "";

            UniverseDefIdInput.interactable = false;

            ClearChildren(InitialShowingWorldListContent);
            if (universeDef.InitialShowingWorldIdList != null)
            {
                foreach (var worldId in universeDef.InitialShowingWorldIdList)
                {
                    AddStringToList(InitialShowingWorldListContent, worldId);
                }
            }

            ClearChildren(WorldIdListContent);
            if (universeDef.WorldIdList != null)
            {
                foreach (var worldId in universeDef.WorldIdList)
                {
                    AddStringToList(WorldIdListContent, worldId);
                }
            }
        }

        private void SaveCurrentUniverse()
        {
            if (_currentEditingUniverseDef == null)
            {
                Debug.LogWarning("没有选中要保存的宇宙");
                return;
            }

            _currentEditingUniverseDef.DefName = UniverseDefNameInput.text;
            _currentEditingUniverseDef.DefDescription = UniverseDefDescInput.text;
            _currentEditingUniverseDef.InitialPlayerLocateWorldId = InitialPlayerWorldIdInput.text;
            _currentEditingUniverseDef.InitialShowingWorldIdList =
                GetStringListFromContent(InitialShowingWorldListContent);
            _currentEditingUniverseDef.WorldIdList = GetStringListFromContent(WorldIdListContent);

            _currentEditingUniverseDef.SaveThisDef();

            Debug.Log($"<color=green>✓ 保存宇宙配置: {_currentEditingUniverseDef.DefName}</color>");

            RefreshUniverseList();
        }

        private void CreateNewUniverse()
        {
            var newUniverse = new UniverseDtoDef
            {
                DefName = "新宇宙",
                DefDescription = "这是一个新的宇宙",
                InitialPlayerLocateWorldId = "",
                InitialShowingWorldIdList = new List<string>(),
                WorldIdList = new List<string>()
            };

            _universeDataModel.AddDtoDef(newUniverse);
            newUniverse.SaveThisDef();

            Debug.Log($"<color=green>✓ 创建新宇宙: {newUniverse.DefName} ({newUniverse.DefId})</color>");

            RefreshUniverseList();
            SelectUniverse(newUniverse);
        }

        private void DeleteCurrentUniverse()
        {
            if (_currentEditingUniverseDef == null)
            {
                Debug.LogWarning("没有选中要删除的宇宙");
                return;
            }

            var defName = _currentEditingUniverseDef.DefName;
            _currentEditingUniverseDef.DeleteThisDef();

            Debug.Log($"<color=yellow>✗ 删除宇宙配置: {defName}</color>");

            _currentEditingUniverseDef = null;
            RefreshUniverseList();
        }

        #endregion

        #region Visual Editor Mode

        private void ShowUniverseVisualEditor()
        {
            // 隐藏基础编辑器
            UniverseEditorRoot.gameObject.SetActive(false);

            // 显示可视化编辑器
            UniverseVisualEditorRoot.gameObject.SetActive(true);

            _isEditMode = true;

            // 刷新世界节点显示
            RefreshUniverseMapNodes();
        }

        private void HideUniverseVisualEditor()
        {
            UniverseVisualEditorRoot.gameObject.SetActive(false);
            _isEditMode = false;
        }

        #endregion

        #region World Nodes Management

        /// <summary>
        /// 刷新宇宙地图上的世界节点
        /// </summary>
        private void RefreshUniverseMapNodes()
        {
            if (_currentEditingUniverseDef == null)
            {
                Debug.LogWarning("没有选中要编辑的宇宙");
                return;
            }

            // 清空现有节点
            ClearAllWorldNodes();

            // 加载世界节点预制体
            if (_worldNodePrefab == null)
            {
                _worldNodePrefab = Resources.Load<GameObject>("UI/Prefabs/EditableWorldNode");
                if (_worldNodePrefab == null)
                {
                    _worldNodePrefab = CreateDefaultWorldNodePrefab();
                }
            }

            // 获取宇宙中的所有世界ID
            var worldIdList = _currentEditingUniverseDef.WorldIdList;
            if (worldIdList == null || worldIdList.Count == 0)
            {
                Debug.Log("当前宇宙没有世界");
                return;
            }

            // 为每个世界创建节点
            foreach (var worldDefId in worldIdList)
            {
                var worldDef = _worldDataModel.GetDefById(worldDefId);
                if (worldDef != null)
                {
                    CreateWorldNode(worldDef);
                }
            }

            Debug.Log($"刷新宇宙地图: 显示 {_currentWorldNodes.Count} 个世界节点");
        }

        /// <summary>
        /// 创建世界节点
        /// </summary>
        private UI_EditableWorldNode CreateWorldNode(WorldDtoDef worldDef)
        {
            var nodeObj = Instantiate(_worldNodePrefab, UniverseWorldNodesContainer);
            var worldNode = nodeObj.GetComponent<UI_EditableWorldNode>();

            if (worldNode == null)
            {
                worldNode = nodeObj.AddComponent<UI_EditableWorldNode>();
            }

            worldNode.Initialize(worldDef, this);
            _currentWorldNodes.Add(worldNode);

            return worldNode;
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
        }

        /// <summary>
        /// 保存宇宙地图上所有世界的位置
        /// </summary>
        private void SaveUniverseMapPositions()
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

        #region Dialog

        private void OpenAddWorldDialog()
        {
            // TODO: 打开添加世界对话框
            // 可以从可用的世界配置列表中选择添加到当前宇宙
            Debug.Log("打开添加世界对话框 (待实现)");
        }

        #endregion

        #region Prefab Creation

        private GameObject CreateDefaultWorldNodePrefab()
        {
            GameObject obj = new GameObject("EditableWorldNode");

            // RectTransform
            RectTransform rect = obj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(100, 100);

            // Background
            Image bg = obj.AddComponent<Image>();
            bg.color = new Color(0.2f, 0.4f, 0.8f, 0.8f);

            // World Name Text
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

            // Position Text
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

        #region World Editor

        private void ShowWorldEditor()
        {
            WorldListScrollView.gameObject.SetActive(true);
            WorldEditorRoot.gameObject.SetActive(true);
            RefreshWorldList();
            RefreshUniverseDropdown();
        }

        private void RefreshWorldList()
        {
            ClearChildren(WorldListContent);

            var allWorlds = _worldDataModel.GetAllWorldDefs();

            if (allWorlds == null || allWorlds.Count == 0)
            {
                Debug.Log("没有可用的世界配置");
                return;
            }

            foreach (var worldDef in allWorlds)
            {
                CreateWorldListItem(worldDef);
            }

            if (_currentEditingWorldDef == null && allWorlds.Count > 0)
            {
                SelectWorld(allWorlds[0]);
            }
        }

        private void CreateWorldListItem(WorldDtoDef worldDef)
        {
            var itemObj = Instantiate(_worldListItemPrefab, WorldListContent);

            var nameText = itemObj.transform.Find("NameText")?.GetComponent<TMP_Text>();
            if (nameText != null)
            {
                // 获取所属宇宙名称
                var context = _worldDataModel.GetContextByDefId(worldDef.DefId);
                var universeName = context?.UniverseName ?? "未知宇宙";

                nameText.text = $"{worldDef.DefName}\n" +
                                $"<size=12><color=#888888>{worldDef.DefId}</color></size>\n" +
                                $"<size=10><color=#666666>所属: {universeName}</color></size>";
            }

            var bgImage = itemObj.GetComponent<Image>();
            if (bgImage != null)
            {
                bgImage.color = _currentEditingWorldDef == worldDef
                    ? new Color(0.3f, 0.5f, 0.8f, 0.5f)
                    : new Color(0.2f, 0.2f, 0.2f, 0.5f);
            }

            var button = itemObj.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => SelectWorld(worldDef));
            }
        }

        private void SelectWorld(WorldDtoDef worldDef)
        {
            _currentEditingWorldDef = worldDef;
            RefreshWorldList();
            LoadWorldToEditor(worldDef);
        }

        private void LoadWorldToEditor(WorldDtoDef worldDef)
        {
            if (worldDef == null) return;

            WorldDefIdInput.text = worldDef.DefId;
            WorldDefNameInput.text = worldDef.DefName;
            WorldDefDescInput.text = worldDef.DefDescription;
            InitialPlayerRegionIdInput.text = worldDef.InitialPlayerLocateRegionId ?? "";

            WorldDefIdInput.interactable = false;

            // 设置所属宇宙下拉框
            var context = _worldDataModel.GetContextByDefId(worldDef.DefId);
            if (context != null && !string.IsNullOrEmpty(context.UniverseName))
            {
                SetUniverseDropdownValue(context.UniverseName);
            }

            ClearChildren(InitialShowingRegionListContent);
            if (worldDef.InitialShowingRegionIdList != null)
            {
                foreach (var regionId in worldDef.InitialShowingRegionIdList)
                {
                    AddStringToList(InitialShowingRegionListContent, regionId);
                }
            }

            ClearChildren(RegionIdListContent);
            if (worldDef.RegionIdList != null)
            {
                foreach (var regionId in worldDef.RegionIdList)
                {
                    AddStringToList(RegionIdListContent, regionId);
                }
            }
        }

        private void RefreshUniverseDropdown()
        {
            WorldUniverseDropdown.ClearOptions();

            var allUniverses = _universeDataModel.GetAllUniverseDefs();
            if (allUniverses == null || allUniverses.Count == 0)
            {
                WorldUniverseDropdown.options.Add(new TMP_Dropdown.OptionData("无可用宇宙"));
                WorldUniverseDropdown.interactable = false;
                return;
            }

            var options = allUniverses.Select(u => new TMP_Dropdown.OptionData(u.DefName)).ToList();
            WorldUniverseDropdown.AddOptions(options);
            WorldUniverseDropdown.interactable = true;
        }

        private void SetUniverseDropdownValue(string universeName)
        {
            for (int i = 0; i < WorldUniverseDropdown.options.Count; i++)
            {
                if (WorldUniverseDropdown.options[i].text == universeName)
                {
                    WorldUniverseDropdown.value = i;
                    break;
                }
            }
        }

        private void SaveCurrentWorld()
        {
            if (_currentEditingWorldDef == null)
            {
                Debug.LogWarning("没有选中要保存的世界");
                return;
            }

            _currentEditingWorldDef.DefName = WorldDefNameInput.text;
            _currentEditingWorldDef.DefDescription = WorldDefDescInput.text;
            _currentEditingWorldDef.InitialPlayerLocateRegionId = InitialPlayerRegionIdInput.text;
            _currentEditingWorldDef.InitialShowingRegionIdList =
                GetStringListFromContent(InitialShowingRegionListContent);
            _currentEditingWorldDef.RegionIdList = GetStringListFromContent(RegionIdListContent);

            // 更新所属宇宙
            var selectedUniverseName = WorldUniverseDropdown.options[WorldUniverseDropdown.value].text;
            var context = _worldDataModel.GetContextByDefId(_currentEditingWorldDef.DefId);
            if (context != null)
            {
                context.UniverseName = selectedUniverseName;
            }

            _currentEditingWorldDef.SaveThisDef();

            Debug.Log($"<color=green>✓ 保存世界配置: {_currentEditingWorldDef.DefName}</color>");

            RefreshWorldList();
        }

        private void CreateNewWorld()
        {
            if (_universeDataModel.GetAllUniverseDefs().Count == 0)
            {
                Debug.LogError("请先创建宇宙配置!");
                return;
            }

            var newWorld = new WorldDtoDef
            {
                DefName = "新世界",
                DefDescription = "这是一个新的世界",
                InitialPlayerLocateRegionId = "",
                InitialShowingRegionIdList = new List<string>(),
                RegionIdList = new List<string>()
            };

            // 创建上下文,使用下拉框选中的宇宙
            var selectedUniverseName = WorldUniverseDropdown.options[WorldUniverseDropdown.value].text;
            var context = new LaunchResourcesLoader.HierarchyContext
            {
                UniverseName = selectedUniverseName,
                WorldName = newWorld.DefName
            };

            _worldDataModel.AddDtoDef(context, newWorld);
            newWorld.SaveThisDef();

            Debug.Log($"<color=green>✓ 创建新世界: {newWorld.DefName} ({newWorld.DefId})</color>");

            RefreshWorldList();
            SelectWorld(newWorld);
        }

        private void DeleteCurrentWorld()
        {
            if (_currentEditingWorldDef == null)
            {
                Debug.LogWarning("没有选中要删除的世界");
                return;
            }

            var defName = _currentEditingWorldDef.DefName;
            _currentEditingWorldDef.DeleteThisDef();

            Debug.Log($"<color=yellow>✗ 删除世界配置: {defName}</color>");

            _currentEditingWorldDef = null;
            RefreshWorldList();
        }

        #endregion

        #region String List Helpers

        private void AddStringToList(Transform listContent, string value)
        {
            var itemObj = Instantiate(_stringListItemPrefab, listContent);

            var inputField = itemObj.transform.Find("InputField")?.GetComponent<TMP_InputField>();
            if (inputField != null)
            {
                inputField.text = value;
            }

            var deleteButton = itemObj.transform.Find("DeleteButton")?.GetComponent<Button>();
            if (deleteButton != null)
            {
                deleteButton.onClick.AddListener(() => Destroy(itemObj));
            }
        }

        private List<string> GetStringListFromContent(Transform listContent)
        {
            var result = new List<string>();

            for (int i = 0; i < listContent.childCount; i++)
            {
                var child = listContent.GetChild(i);
                var inputField = child.Find("InputField")?.GetComponent<TMP_InputField>();
                if (inputField != null && !string.IsNullOrEmpty(inputField.text))
                {
                    result.Add(inputField.text);
                }
            }

            return result;
        }

        #endregion

        #region Utility

        private void ClearChildren(Transform parent)
        {
            if (parent == null) return;

            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                Destroy(parent.GetChild(i).gameObject);
            }
        }

        private GameObject CreateDefaultListItemPrefab(string prefix = "")
        {
            var obj = new GameObject($"{prefix}ListItem");

            var layout = obj.AddComponent<LayoutElement>();
            layout.minHeight = 80;

            var button = obj.AddComponent<Button>();
            var image = obj.AddComponent<Image>();
            image.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);

            var textObj = new GameObject("NameText");
            textObj.transform.SetParent(obj.transform);
            var text = textObj.AddComponent<TMP_Text>();
            text.alignment = TextAlignmentOptions.Left;
            text.fontSize = 16;
            text.margin = new Vector4(10, 5, 10, 5);

            var rectTransform = textObj.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = new Vector2(10, 5);
            rectTransform.offsetMax = new Vector2(-10, -5);

            return obj;
        }

        private GameObject CreateDefaultStringListItemPrefab()
        {
            var obj = new GameObject("StringListItem");

            var layout = obj.AddComponent<LayoutElement>();
            layout.minHeight = 40;
            layout.preferredHeight = 40;

            var horizontalLayout = obj.AddComponent<HorizontalLayoutGroup>();
            horizontalLayout.childControlWidth = true;
            horizontalLayout.childControlHeight = true;
            horizontalLayout.childForceExpandWidth = true;
            horizontalLayout.childForceExpandHeight = false;
            horizontalLayout.spacing = 5;
            horizontalLayout.padding = new RectOffset(5, 5, 5, 5);

            // InputField
            var inputObj = new GameObject("InputField");
            inputObj.transform.SetParent(obj.transform);
            var inputField = inputObj.AddComponent<TMP_InputField>();
            var inputText = new GameObject("Text").AddComponent<TMP_Text>();
            inputText.transform.SetParent(inputObj.transform);
            inputField.textComponent = inputText;

            var inputBg = inputObj.AddComponent<Image>();
            inputBg.color = new Color(0.1f, 0.1f, 0.1f, 0.5f);

            // Delete Button
            var deleteObj = new GameObject("DeleteButton");
            deleteObj.transform.SetParent(obj.transform);
            var deleteButton = deleteObj.AddComponent<Button>();
            var deleteBg = deleteObj.AddComponent<Image>();
            deleteBg.color = new Color(0.8f, 0.2f, 0.2f, 0.8f);

            var deleteText = new GameObject("Text").AddComponent<TMP_Text>();
            deleteText.transform.SetParent(deleteObj.transform);
            deleteText.text = "X";
            deleteText.alignment = TextAlignmentOptions.Center;

            var deleteLayout = deleteObj.AddComponent<LayoutElement>();
            deleteLayout.preferredWidth = 40;

            return obj;
        }

        #endregion
    }
}