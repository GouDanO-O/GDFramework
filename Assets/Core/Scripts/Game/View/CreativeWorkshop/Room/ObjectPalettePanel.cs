using System.Collections.Generic;
using Core.Game.Chunk.Room;
using Core.Game.RoomEditor;
using GDFrameworkCore;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Game.View.Room
{
    /// <summary>
    /// 物体调色板面板
    /// </summary>
    public class ObjectPalettePanel : MonoBehaviour, ICanGetSystem
    {
        [Header("面板引用")]
        [SerializeField] private Transform objectButtonContainer;
        [SerializeField] private GameObject objectButtonPrefab;
        
        [Header("分类标签")]
        [SerializeField] private TMP_Dropdown categoryDropdown;
        
        [Header("当前选择显示")]
        [SerializeField] private Image imgSelectedObject;
        [SerializeField] private TextMeshProUGUI txtSelectedObjectName;
        [SerializeField] private TextMeshProUGUI txtSelectedObjectInfo;
        [SerializeField] private TextMeshProUGUI txtObjectSize;
        
        [Header("属性面板")]
        [SerializeField] private GameObject propertiesPanel;
        [SerializeField] private Toggle toggleRotatable;
        [SerializeField] private Toggle toggleBlockMovement;
        [SerializeField] private TMP_InputField inputRotation;
        
        private RoomEditorSystem _editorSystem;
        private List<ObjectPaletteButton> _objectButtons = new List<ObjectPaletteButton>();
        private ObjectTemplate _currentSelection;
        
        private void Start()
        {
            _editorSystem = this.GetSystem<RoomEditorSystem>();
            InitializePalette();
            InitializeCategory();
        }
        
        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }
        
        private void InitializePalette()
        {

        }
        
        private void InitializeCategory()
        {
            categoryDropdown.ClearOptions();
            
            var options = new List<string>
            {
                "家具",
                "容器",
                "装饰物",
                "交互物",
                "光源"
            };
            
            categoryDropdown.AddOptions(options);
            categoryDropdown.onValueChanged.AddListener(OnCategoryChanged);
        }
        
        private void OnCategoryChanged(int index)
        {
            EPlaceableObjectType type = (EPlaceableObjectType)index;
            RefreshObjectList(type);
        }
        
        private void RefreshObjectList(EPlaceableObjectType type)
        {
            // 清空现有按钮
            foreach (var btn in _objectButtons)
            {
                Destroy(btn.gameObject);
            }
            _objectButtons.Clear();
        }
        
        private void CreateObjectButton(ObjectTemplate template)
        {
            GameObject btnObj = Instantiate(objectButtonPrefab, objectButtonContainer);
            var paletteBtn = btnObj.GetComponent<ObjectPaletteButton>();
            
            if (paletteBtn == null)
            {
                paletteBtn = btnObj.AddComponent<ObjectPaletteButton>();
            }
            
            paletteBtn.Initialize(template, () => OnObjectSelected(template));
            _objectButtons.Add(paletteBtn);
        }
        
        private void OnObjectSelected(ObjectTemplate template)
        {
            SelectObject(template);
            _editorSystem.SelectObjectTemplate(template.ToPlaceableData());
        }
        
        private void SelectObject(ObjectTemplate template)
        {
            _currentSelection = template;
            
            // 更新所有按钮的选中状态
            foreach (var btn in _objectButtons)
            {
                btn.SetSelected(btn.Template == template);
            }
            
            // 更新显示
            UpdateSelectionDisplay(template);
        }
        
        private void UpdateSelectionDisplay(ObjectTemplate template)
        {
            imgSelectedObject.sprite = template.Icon;
            txtSelectedObjectName.text = template.TemplateName;
            txtSelectedObjectInfo.text = template.Description;
            txtObjectSize.text = $"尺寸: {template.Size.x}x{template.Size.y}";
            
            // 更新属性面板
            if (propertiesPanel != null)
            {
                propertiesPanel.SetActive(true);
                toggleRotatable.isOn = template.Rotatable;
                toggleBlockMovement.isOn = template.BlocksMovement;
                inputRotation.text = "0";
            }
        }
        
        public void OnRotationChanged(string value)
        {
            if (int.TryParse(value, out int rotation))
            {
                rotation = Mathf.Clamp(rotation, 0, 270);
                rotation = (rotation / 90) * 90; // 限制为90度倍数
                inputRotation.text = rotation.ToString();
            }
        }
    }
}