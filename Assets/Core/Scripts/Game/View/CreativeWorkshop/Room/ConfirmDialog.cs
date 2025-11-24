using System;
using Core.Game.Chunk.Room;
using Core.Game.Chunk.Room.Data;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Core.Game.View.Room
{
    /// <summary>
    /// 确认对话框
    /// </summary>
    public class ConfirmDialog : MonoBehaviour
    {
        [Header("UI引用")]
        [SerializeField] private TextMeshProUGUI txtMessage;
        [SerializeField] private Button btnConfirm;
        [SerializeField] private Button btnCancel;
        
        private System.Action _onConfirm;
        private System.Action _onCancel;
        
        private void Start()
        {
            btnConfirm.onClick.AddListener(OnConfirmClicked);
            btnCancel.onClick.AddListener(OnCancelClicked);
        }
        
        /// <summary>
        /// 显示对话框
        /// </summary>
        public void Show(string message, System.Action onConfirm, System.Action onCancel = null)
        {
            txtMessage.text = message;
            _onConfirm = onConfirm;
            _onCancel = onCancel;
            gameObject.SetActive(true);
        }
        
        /// <summary>
        /// 隐藏对话框
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        
        private void OnConfirmClicked()
        {
            _onConfirm?.Invoke();
            Hide();
        }
        
        private void OnCancelClicked()
        {
            _onCancel?.Invoke();
            Hide();
        }
    }
    
    /// <summary>
    /// 填充区域对话框
    /// </summary>
    public class FillAreaDialog : MonoBehaviour
    {
        [Header("UI引用")]
        [SerializeField] private TextMeshProUGUI txtInstructions;
        [SerializeField] private TextMeshProUGUI txtStartPos;
        [SerializeField] private TextMeshProUGUI txtEndPos;
        [SerializeField] private Button btnConfirm;
        [SerializeField] private Button btnCancel;
        
        private Vector2Int? _startPosition;
        private Vector2Int? _endPosition;
        private System.Action<Vector2Int, Vector2Int> _onConfirm;
        
        private bool _isSelecting;
        
        private void Start()
        {
            btnConfirm.onClick.AddListener(OnConfirmClicked);
            btnCancel.onClick.AddListener(OnCancelClicked);
            btnConfirm.interactable = false;
        }
        
        /// <summary>
        /// 开始选择区域
        /// </summary>
        public void StartSelection(System.Action<Vector2Int, Vector2Int> onConfirm)
        {
            _onConfirm = onConfirm;
            _startPosition = null;
            _endPosition = null;
            _isSelecting = true;
            
            gameObject.SetActive(true);
            UpdateDisplay();
        }
        
        /// <summary>
        /// 设置起点
        /// </summary>
        public void SetStartPosition(Vector2Int pos)
        {
            if (!_isSelecting) return;
            
            _startPosition = pos;
            UpdateDisplay();
        }
        
        /// <summary>
        /// 设置终点
        /// </summary>
        public void SetEndPosition(Vector2Int pos)
        {
            if (!_isSelecting || !_startPosition.HasValue) return;
            
            _endPosition = pos;
            btnConfirm.interactable = true;
            UpdateDisplay();
        }
        
        private void UpdateDisplay()
        {
            if (_startPosition.HasValue)
            {
                txtStartPos.text = $"起点: ({_startPosition.Value.x}, {_startPosition.Value.y})";
            }
            else
            {
                txtStartPos.text = "起点: 未选择";
            }
            
            if (_endPosition.HasValue)
            {
                txtEndPos.text = $"终点: ({_endPosition.Value.x}, {_endPosition.Value.y})";
                
                int width = Mathf.Abs(_endPosition.Value.x - _startPosition.Value.x) + 1;
                int height = Mathf.Abs(_endPosition.Value.y - _startPosition.Value.y) + 1;
                txtInstructions.text = $"将填充 {width}x{height} 个瓦片";
            }
            else
            {
                txtEndPos.text = "终点: 未选择";
                txtInstructions.text = _startPosition.HasValue 
                    ? "请点击第二个位置作为终点" 
                    : "请点击第一个位置作为起点";
            }
        }
        
        private void OnConfirmClicked()
        {
            if (_startPosition.HasValue && _endPosition.HasValue)
            {
                _onConfirm?.Invoke(_startPosition.Value, _endPosition.Value);
            }
            Hide();
        }
        
        private void OnCancelClicked()
        {
            Hide();
        }
        
        private void Hide()
        {
            _isSelecting = false;
            gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// 房间加载对话框
    /// </summary>
    public class RoomLoadDialog : MonoBehaviour
    {
        [Header("UI引用")]
        [SerializeField] private Transform roomListContainer;
        [SerializeField] private GameObject roomListItemPrefab;
        [SerializeField] private Button btnLoad;
        [SerializeField] private Button btnCancel;
        [SerializeField] private TMP_InputField searchInput;
        
        private System.Action<RoomDtoDef> _onRoomSelected;
        private RoomDtoDef _selectedRoom;
        
        private void Start()
        {
            btnLoad.onClick.AddListener(OnLoadClicked);
            btnCancel.onClick.AddListener(OnCancelClicked);
            searchInput.onValueChanged.AddListener(OnSearchChanged);
            
            btnLoad.interactable = false;
        }
        
        /// <summary>
        /// 显示对话框
        /// </summary>
        public void Show(System.Action<RoomDtoDef> onRoomSelected)
        {
            _onRoomSelected = onRoomSelected;
            gameObject.SetActive(true);
            RefreshRoomList();
        }
        
        /// <summary>
        /// 刷新房间列表
        /// </summary>
        private void RefreshRoomList(string searchText = "")
        {
            // 清空现有列表
            foreach (Transform child in roomListContainer)
            {
                Destroy(child.gameObject);
            }
            
            // TODO: 从StorageSystem加载所有房间配置
            // var rooms = storageSystem.LoadAllRoomDefs();
            
            // 示例代码
            var exampleRooms = new[]
            {
                new RoomDtoDef { DefName = "主卧室", Width = 20, Height = 15 },
                new RoomDtoDef { DefName = "客厅", Width = 30, Height = 25 },
                new RoomDtoDef { DefName = "厨房", Width = 15, Height = 12 }
            };
            
            foreach (var room in exampleRooms)
            {
                if (!string.IsNullOrEmpty(searchText) && 
                    !room.DefName.Contains(searchText))
                    continue;
                
                CreateRoomListItem(room);
            }
        }
        
        private void CreateRoomListItem(RoomDtoDef room)
        {
            GameObject itemObj = Instantiate(roomListItemPrefab, roomListContainer);
            var listItem = itemObj.GetComponent<RoomListItem>();
            
            if (listItem != null)
            {
                listItem.Initialize(room, () => OnRoomItemClicked(room));
            }
        }
        
        private void OnRoomItemClicked(RoomDtoDef room)
        {
            _selectedRoom = room;
            btnLoad.interactable = true;
            
            // 更新所有列表项的选中状态
            foreach (Transform child in roomListContainer)
            {
                var item = child.GetComponent<RoomListItem>();
                if (item != null)
                {
                    item.SetSelected(item.Room == room);
                }
            }
        }
        
        private void OnSearchChanged(string searchText)
        {
            RefreshRoomList(searchText);
        }
        
        private void OnLoadClicked()
        {
            if (_selectedRoom != null)
            {
                _onRoomSelected?.Invoke(_selectedRoom);
            }
            Hide();
        }
        
        private void OnCancelClicked()
        {
            Hide();
        }
        
        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// 房间列表项
    /// </summary>
    public class RoomListItem : MonoBehaviour
    {
        [Header("UI引用")]
        [SerializeField] private TextMeshProUGUI txtRoomName;
        [SerializeField] private TextMeshProUGUI txtRoomSize;
        [SerializeField] private TextMeshProUGUI txtModifyTime;
        [SerializeField] private Image background;
        [SerializeField] private Button button;
        
        [Header("颜色")]
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color selectedColor = new Color(0.3f, 0.6f, 1f);
        
        public RoomDtoDef Room { get; private set; }
        
        public void Initialize(RoomDtoDef room, System.Action onClick)
        {
            Room = room;
            
            txtRoomName.text = room.DefName;
            txtRoomSize.text = $"{room.Width}x{room.Height}";
            txtModifyTime.text = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClick?.Invoke());
        }
        
        public void SetSelected(bool selected)
        {
            background.color = selected ? selectedColor : normalColor;
        }
    }
    
    /// <summary>
    /// 房间属性面板
    /// </summary>
    public class RoomPropertiesPanel : MonoBehaviour
    {
        [Header("UI引用")]
        [SerializeField] private TMP_InputField inputRoomName;
        [SerializeField] private TMP_InputField inputWidth;
        [SerializeField] private TMP_InputField inputHeight;
        [SerializeField] private Toggle toggleHasOutdoor;
        [SerializeField] private TMP_Dropdown dropdownDefaultFloor;
        [SerializeField] private Button btnApply;
        [SerializeField] private Button btnCancel;
        
        private RoomDtoDef _currentRoom;
        private System.Action<RoomDtoDef> _onApply;
        
        private void Start()
        {
            btnApply.onClick.AddListener(OnApplyClicked);
            btnCancel.onClick.AddListener(OnCancelClicked);
        }
        
        /// <summary>
        /// 显示面板
        /// </summary>
        public void Show(RoomDtoDef room, System.Action<RoomDtoDef> onApply)
        {
            _currentRoom = room;
            _onApply = onApply;
            
            LoadRoomData();
            gameObject.SetActive(true);
        }
        
        private void LoadRoomData()
        {
            if (_currentRoom == null) return;
            
            inputRoomName.text = _currentRoom.DefName;
            inputWidth.text = _currentRoom.Width.ToString();
            inputHeight.text = _currentRoom.Height.ToString();
            toggleHasOutdoor.isOn = _currentRoom.HasOutdoorArea;
            dropdownDefaultFloor.value = (int)_currentRoom.DefaultFloorType;
        }
        
        private void OnApplyClicked()
        {
            if (_currentRoom == null) return;
            
            // 验证输入
            if (!int.TryParse(inputWidth.text, out int width) || width < 5 || width > 100)
            {
                Debug.LogWarning("宽度必须在5-100之间");
                return;
            }
            
            if (!int.TryParse(inputHeight.text, out int height) || height < 5 || height > 100)
            {
                Debug.LogWarning("高度必须在5-100之间");
                return;
            }
            
            // 应用修改
            _currentRoom.DefName = inputRoomName.text;
            _currentRoom.Width = width;
            _currentRoom.Height = height;
            _currentRoom.HasOutdoorArea = toggleHasOutdoor.isOn;
            _currentRoom.DefaultFloorType = (ETileType)dropdownDefaultFloor.value;
            
            _onApply?.Invoke(_currentRoom);
            Hide();
        }
        
        private void OnCancelClicked()
        {
            Hide();
        }
        
        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// 快捷键提示面板
    /// </summary>
    public class KeyboardShortcutsPanel : MonoBehaviour
    {
        [Header("UI引用")]
        [SerializeField] private TextMeshProUGUI txtShortcuts;
        [SerializeField] private Button btnClose;
        
        private void Start()
        {
            btnClose.onClick.AddListener(Hide);
            
            // 显示快捷键列表
            txtShortcuts.text = @"
<b>快捷键说明</b>

<b>通用操作:</b>
Ctrl + S - 保存房间
Ctrl + Z - 撤销 (TODO)
Ctrl + Y - 重做 (TODO)
G - 切换网格显示
ESC - 退出编辑器

<b>编辑模式:</b>
1 - 切换到瓦片模式
2 - 切换到物体模式
3 - 切换到擦除模式

<b>瓦片编辑:</b>
鼠标左键 - 放置瓦片
鼠标右键 - 删除瓦片
按住左键拖拽 - 连续放置

<b>物体编辑:</b>
鼠标左键 - 放置物体
鼠标右键 - 删除物体
R - 旋转选中物体 (TODO)

<b>视图控制:</b>
鼠标滚轮 - 缩放视图
鼠标中键拖拽 - 平移视图
            ";
        }
        
        public void Show()
        {
            gameObject.SetActive(true);
        }
        
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}