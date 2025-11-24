using Core.Game.RoomEditor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Game.View.Room
{
    /// <summary>
    /// 物体按钮组件
    /// </summary>
    public class ObjectPaletteButton : MonoBehaviour
    {
        private Button _button;
        private Image _icon;
        private Image _background;
        private TextMeshProUGUI _label;
        private TextMeshProUGUI _sizeLabel;
        
        public ObjectTemplate Template { get; private set; }
        
        [SerializeField] private Color selectedColor = new Color(1f, 0.8f, 0.3f);
        [SerializeField] private Color normalColor = Color.white;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
            _icon = transform.Find("Icon")?.GetComponent<Image>();
            _background = GetComponent<Image>();
            _label = transform.Find("Label")?.GetComponent<TextMeshProUGUI>();
            _sizeLabel = transform.Find("SizeLabel")?.GetComponent<TextMeshProUGUI>();
        }
        
        public void Initialize(ObjectTemplate template, System.Action onClick)
        {
            Template = template;
            
            if (_icon != null)
                _icon.sprite = template.Icon;
            
            if (_label != null)
                _label.text = template.TemplateName;
            
            if (_sizeLabel != null)
                _sizeLabel.text = $"{template.Size.x}x{template.Size.y}";
            
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(() => onClick?.Invoke());
        }
        
        public void SetSelected(bool selected)
        {
            if (_background != null)
            {
                _background.color = selected ? selectedColor : normalColor;
            }
        }
    }
}