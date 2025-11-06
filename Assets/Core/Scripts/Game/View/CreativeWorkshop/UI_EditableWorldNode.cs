using Core.Game.Chunk.World.Data;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

namespace Core.Game.View
{
    /// <summary>
    /// 可编辑的世界节点
    /// 支持拖拽移动和点击选择
    /// </summary>
    public class UI_EditableWorldNode : MonoBehaviour, 
        IBeginDragHandler, IDragHandler, IEndDragHandler, 
        IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public WorldDtoDef WorldDef { get; private set; }
        
        private UI_UniverseEditorPanel _editorPanel;
        private RectTransform _rectTransform;
        private Image _backgroundImage;
        private TextMeshProUGUI _nameText;
        private TextMeshProUGUI _positionText;
        
        private Vector2 _dragOffset;
        private bool _isDragging = false;
        
        private Color _normalColor = new Color(0.2f, 0.4f, 0.8f, 0.8f);
        private Color _hoverColor = new Color(0.3f, 0.5f, 1f, 1f);
        private Color _dragColor = new Color(0.4f, 0.6f, 1f, 1f);
        private Color _selectedColor = new Color(0.2f, 0.8f, 0.2f, 1f);
        
        private bool _isSelected = false;

        // 点击事件
        public event Action<WorldDtoDef> OnWorldNodeClicked;

        public void Initialize(WorldDtoDef worldDef, UI_UniverseEditorPanel editorPanel)
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

        /// <summary>
        /// 设置选中状态
        /// </summary>
        public void SetSelected(bool selected)
        {
            _isSelected = selected;
            if (_backgroundImage != null && !_isDragging)
            {
                _backgroundImage.color = _isSelected ? _selectedColor : _normalColor;
            }
        }

        #region Drag Handlers

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
                _backgroundImage.color = _isSelected ? _selectedColor : _hoverColor;
            }
            
            Debug.Log($"<color=cyan>世界 {WorldDef.DefName} 移动到: {_rectTransform.anchoredPosition}</color>");
        }

        #endregion

        #region Pointer Handlers

        public void OnPointerClick(PointerEventData eventData)
        {
            // 触发点击事件
            OnWorldNodeClicked?.Invoke(WorldDef);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_backgroundImage != null && !_isDragging && !_isSelected)
            {
                _backgroundImage.color = _hoverColor;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_backgroundImage != null && !_isDragging && !_isSelected)
            {
                _backgroundImage.color = _normalColor;
            }
        }

        #endregion

        /// <summary>
        /// 保存位置到配置
        /// </summary>
        public void SavePosition()
        {
            if (WorldDef != null && _rectTransform != null)
            {
                WorldDef.UpdatePosition(_rectTransform.anchoredPosition);
                WorldDef.SaveThisDef();
                Debug.Log($"保存世界 {WorldDef.DefName} 位置: {_rectTransform.anchoredPosition}");
            }
        }
    }
}