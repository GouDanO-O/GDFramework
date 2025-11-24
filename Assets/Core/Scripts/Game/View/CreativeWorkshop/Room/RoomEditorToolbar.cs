using System.Collections.Generic;
using Core.Game.Chunk.Room;
using GDFrameworkCore;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Core.Game.View.Room
{
    /// <summary>
    /// 工具栏面板
    /// </summary>
    public class RoomEditorToolbar : MonoBehaviour, ICanGetSystem
    {
        [Header("按钮引用")]
        [SerializeField] private Button btnTileMode;
        [SerializeField] private Button btnObjectMode;
        [SerializeField] private Button btnEraseMode;
        [SerializeField] private Button btnSave;
        [SerializeField] private Button btnLoad;
        [SerializeField] private Button btnClear;
        [SerializeField] private Button btnFill;
        [SerializeField] private Button btnGrid;
        [SerializeField] private Button btnExit;
        
        [Header("状态显示")]
        [SerializeField] private TextMeshProUGUI txtCurrentMode;
        [SerializeField] private TextMeshProUGUI txtRoomInfo;
        [SerializeField] private TextMeshProUGUI txtCursorPos;
        
        [Header("颜色配置")]
        [SerializeField] private Color activeColor = Color.green;
        [SerializeField] private Color normalColor = Color.white;
        
        private RoomEditorSystem _editorSystem;
        private RoomEditorUIController _uiController;
        
        private void Start()
        {
            _editorSystem = this.GetSystem<RoomEditorSystem>();
            _uiController = GetComponentInParent<RoomEditorUIController>();
            
            InitializeButtons();
        }
        
        private void Update()
        {
            UpdateStatusDisplay();
        }
        
        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }
        
        private void InitializeButtons()
        {
            btnTileMode.onClick.AddListener(OnTileModeClicked);
            btnObjectMode.onClick.AddListener(OnObjectModeClicked);
            btnEraseMode.onClick.AddListener(OnEraseModeClicked);
            btnSave.onClick.AddListener(OnSaveClicked);
            btnLoad.onClick.AddListener(OnLoadClicked);
            btnClear.onClick.AddListener(OnClearClicked);
            btnFill.onClick.AddListener(OnFillClicked);
            btnGrid.onClick.AddListener(OnGridClicked);
            btnExit.onClick.AddListener(OnExitClicked);
        }
        
        private void UpdateStatusDisplay()
        {
            if (!_editorSystem.IsEditing) return;
            
            // 更新模式显示
            txtCurrentMode.text = $"模式: {GetModeText(_editorSystem.CurrentMode)}";
            
            // 更新房间信息
            var room = _editorSystem.CurrentRoom;
            if (room != null)
            {
                txtRoomInfo.text = $"房间: {room.DtoDef.DefName} ({room.DtoDef.Width}x{room.DtoDef.Height})";
            }
            
            // 更新光标位置
            var pos = _editorSystem.HoveredPosition;
            txtCursorPos.text = $"位置: ({pos.x}, {pos.y})";
            
            // 更新按钮状态
            UpdateButtonStates();
        }
        
        private void UpdateButtonStates()
        {
            var currentMode = _editorSystem.CurrentMode;
            
            SetButtonColor(btnTileMode, currentMode == EditorMode.Tile);
            SetButtonColor(btnObjectMode, currentMode == EditorMode.Object);
            SetButtonColor(btnEraseMode, currentMode == EditorMode.Erase);
        }
        
        private void SetButtonColor(Button button, bool isActive)
        {
            var colors = button.colors;
            colors.normalColor = isActive ? activeColor : normalColor;
            button.colors = colors;
        }
        
        private string GetModeText(EditorMode mode)
        {
            return mode switch
            {
                EditorMode.Tile => "瓦片编辑",
                EditorMode.Object => "物体放置",
                EditorMode.Erase => "擦除",
                _ => "未知"
            };
        }
        
        #region 按钮事件
        
        private void OnTileModeClicked()
        {
            _editorSystem.SetEditorMode(EditorMode.Tile);
            _uiController.ShowTilePalette();
        }
        
        private void OnObjectModeClicked()
        {
            _editorSystem.SetEditorMode(EditorMode.Object);
            _uiController.ShowObjectPalette();
        }
        
        private void OnEraseModeClicked()
        {
            _editorSystem.SetEditorMode(EditorMode.Erase);
        }
        
        private void OnSaveClicked()
        {
            _uiController.SaveRoom();
        }
        
        private void OnLoadClicked()
        {
            // TODO: 打开加载对话框
            Debug.Log("加载房间");
        }
        
        private void OnClearClicked()
        {
            if (ShowConfirmDialog("确定要清空所有内容吗?"))
            {
                _uiController.ClearAll();
            }
        }
        
        private void OnFillClicked()
        {
            _uiController.FillArea();
        }
        
        private void OnGridClicked()
        {
            _uiController.ToggleGrid();
        }
        
        private void OnExitClicked()
        {
            if (ShowConfirmDialog("确定要退出编辑器吗? 未保存的更改将丢失。"))
            {
                _editorSystem.StopEditRoom();
                // TODO: 返回上一个场景
            }
        }
        
        private bool ShowConfirmDialog(string message)
        {
            // TODO: 实现确认对话框
            return true;
        }
        
        #endregion
    }
}