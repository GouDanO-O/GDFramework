using Core.Game.Chunk.Room;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Game.View.Room
{
    /// <summary>
    /// 瓦片按钮组件
    /// </summary>
    public class TilePaletteButton : MonoBehaviour
    {
        private Button _button;
        private Image _icon;
        private Image _background;
        private TextMeshProUGUI _label;
        
        public ETileType ETileType { get; private set; }
        
        [SerializeField] private Color selectedColor = new Color(0.3f, 0.8f, 1f);
        [SerializeField] private Color normalColor = Color.white;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
            _icon = transform.Find("Icon")?.GetComponent<Image>();
            _background = GetComponent<Image>();
            _label = transform.Find("Label")?.GetComponent<TextMeshProUGUI>();
        }
        
        public void Initialize(ETileType ETileType, Sprite icon, System.Action onClick)
        {
            ETileType = ETileType;
            
            if (_icon != null)
                _icon.sprite = icon;
            
            if (_label != null)
                _label.text = ETileType.ToString();
            
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