using Core.Game.Chunk.World.Data;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Core.Game.View
{
    /// <summary>
    /// 可编辑的世界节点
    /// 支持拖拽移动
    /// </summary>
    public class UI_EditableWorldNode : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public WorldDtoDef WorldDef { get; private set; }
        
        private UI_ChunkEditorPanel _editorPanel;
        private RectTransform _rectTransform;
        private Image _backgroundImage;
        private TextMeshProUGUI _nameText;
        private TextMeshProUGUI _positionText;
        
        private Vector2 _dragOffset;
        private bool _isDragging = false;
        
        private Color _normalColor = new Color(0.2f, 0.4f, 0.8f, 0.8f);
        private Color _hoverColor = new Color(0.3f, 0.5f, 1f, 1f);
        private Color _dragColor = new Color(0.4f, 0.6f, 1f, 1f);

        public void Initialize(WorldDtoDef worldDef, UI_ChunkEditorPanel editorPanel)
        {
            WorldDef = worldDef;
            _editorPanel = editorPanel;
            
            _rectTransform = GetComponent<RectTransform>();
            _backgroundImage = GetComponent<Image>();
            _nameText = transform.Find("WorldName")?.GetComponent<TextMeshProUGUI>();
            _positionText = transform.Find("Position")?.GetComponent<TextMeshProUGUI>();
            
            // 设置初始位置
            if (_rectTransform != null)
            {
                _rectTransform.anchoredPosition = worldDef.InitialSpawnedPosition;
            }
            
            // 更新显示
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            if (_nameText != null)
            {
                _nameText.text = WorldDef.DefName;
            }
            
            UpdatePositionDisplay();
        }

        private void UpdatePositionDisplay()
        {
            if (_positionText != null && _rectTransform != null)
            {
                var pos = _rectTransform.anchoredPosition;
                _positionText.text = $"({pos.x:F0}, {pos.y:F0})";
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _isDragging = true;
            
            // 计算拖拽偏移
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _rectTransform.parent as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint);
            
            _dragOffset = _rectTransform.anchoredPosition - localPoint;
            
            if (_backgroundImage != null)
            {
                _backgroundImage.color = _dragColor;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isDragging) return;
            
            // 计算新位置
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _rectTransform.parent as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint);
            
            Vector2 newPosition = localPoint + _dragOffset;
            
            // 网格吸附
            if (_editorPanel != null)
            {
                newPosition = _editorPanel.SnapToGrid(newPosition);
                _editorPanel.UpdateCoordinateDisplay(newPosition);
            }
            
            _rectTransform.anchoredPosition = newPosition;
            UpdatePositionDisplay();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _isDragging = false;
            
            if (_backgroundImage != null)
            {
                _backgroundImage.color = _hoverColor;
            }
            
            Debug.Log($"<color=cyan>世界 {WorldDef.DefName} 移动到: {_rectTransform.anchoredPosition}</color>");
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_backgroundImage != null && !_isDragging)
            {
                _backgroundImage.color = _hoverColor;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_backgroundImage != null && !_isDragging)
            {
                _backgroundImage.color = _normalColor;
            }
        }

        /// <summary>
        /// 保存位置到配置
        /// </summary>
        public void SavePosition()
        {
            if (WorldDef != null && _rectTransform != null)
            {
                WorldDef.UpdatePosition(_rectTransform.anchoredPosition);
                Debug.Log($"保存世界 {WorldDef.DefName} 位置: {_rectTransform.anchoredPosition}");
            }
        }
    }
}