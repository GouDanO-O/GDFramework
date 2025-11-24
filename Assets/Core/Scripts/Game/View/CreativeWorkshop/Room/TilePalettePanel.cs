using System.Collections.Generic;
using Core.Game.RoomEditor;
using GDFrameworkCore;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Game.View.Room
{
    /// <summary>
    /// 瓦片调色板面板
    /// </summary>
    public class TilePalettePanel : MonoBehaviour, ICanGetSystem
    {
        [Header("面板引用")]
        [SerializeField] private Transform tileButtonContainer;
        [SerializeField] private GameObject tileButtonPrefab;
        
        [Header("瓦片图标")]
        [SerializeField] private Sprite[] tileIcons; // 对应TileType的图标
        
        [Header("当前选择显示")]
        [SerializeField] private Image imgSelectedTile;
        [SerializeField] private TextMeshProUGUI txtSelectedTileName;
        [SerializeField] private TextMeshProUGUI txtSelectedTileInfo;
        
        private RoomEditorSystem _editorSystem;
        private List<TilePaletteButton> _tileButtons = new List<TilePaletteButton>();
        private Chunk.Room.ETileType _currentSelection = Chunk.Room.ETileType.Floor;
        
        private void Start()
        {
            _editorSystem = this.GetSystem<RoomEditorSystem>();
            InitializePalette();
        }
        
        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }
        
        private void InitializePalette()
        {
            // 为每种瓦片类型创建按钮
            var tileTypes = System.Enum.GetValues(typeof(Chunk.Room.ETileType));
            
            foreach (Chunk.Room.ETileType type in tileTypes)
            {
                if (type == Chunk.Room.ETileType.Empty) continue; // 跳过空瓦片
                
                CreateTileButton(type);
            }
            
            // 默认选择地板
            SelectTile(Chunk.Room.ETileType.Floor);
        }
        
        private void CreateTileButton(Chunk.Room.ETileType ETileType)
        {
            GameObject btnObj = Instantiate(tileButtonPrefab, tileButtonContainer);
            var paletteBtn = btnObj.GetComponent<TilePaletteButton>();
            
            if (paletteBtn == null)
            {
                paletteBtn = btnObj.AddComponent<TilePaletteButton>();
            }
            
            paletteBtn.Initialize(ETileType, GetTileIcon(ETileType), () => OnTileSelected(ETileType));
            _tileButtons.Add(paletteBtn);
        }
        
        private void OnTileSelected(Chunk.Room.ETileType ETileType)
        {
            SelectTile(ETileType);
            _editorSystem.SelectTileType(ETileType);
        }
        
        private void SelectTile(Chunk.Room.ETileType ETileType)
        {
            _currentSelection = ETileType;
            
            // 更新所有按钮的选中状态
            foreach (var btn in _tileButtons)
            {
                btn.SetSelected(btn.ETileType == ETileType);
            }
            
            // 更新显示
            UpdateSelectionDisplay(ETileType);
        }
        
        private void UpdateSelectionDisplay(Chunk.Room.ETileType ETileType)
        {
            imgSelectedTile.sprite = GetTileIcon(ETileType);
            txtSelectedTileName.text = GetTileName(ETileType);
            txtSelectedTileInfo.text = GetTileInfo(ETileType);
        }
        
        private Sprite GetTileIcon(Chunk.Room.ETileType ETileType)
        {
            int index = (int)ETileType;
            if (index >= 0 && index < tileIcons.Length)
                return tileIcons[index];
            return null;
        }
        
        private string GetTileName(Chunk.Room.ETileType ETileType)
        {
            return ETileType switch
            {
                Chunk.Room.ETileType.Floor => "地板",
                Chunk.Room.ETileType.Wall => "墙壁",
                Chunk.Room.ETileType.Door => "门",
                Chunk.Room.ETileType.Window => "窗户",
                Chunk.Room.ETileType.Stairs => "楼梯",
                Chunk.Room.ETileType.Water => "水面",
                Chunk.Room.ETileType.Grass => "草地",
                Chunk.Room.ETileType.Dirt => "泥土",
                Chunk.Room.ETileType.Stone => "石头",
                _ => "未知"
            };
        }
        
        private string GetTileInfo(Chunk.Room.ETileType ETileType)
        {
            return ETileType switch
            {
                Chunk.Room.ETileType.Floor => "室内地板 | 可行走",
                Chunk.Room.ETileType.Wall => "墙壁 | 不可行走",
                Chunk.Room.ETileType.Door => "门 | 可交互",
                Chunk.Room.ETileType.Window => "窗户 | 不可行走",
                Chunk.Room.ETileType.Stairs => "楼梯 | 可行走",
                Chunk.Room.ETileType.Water => "水面 | 特殊行走",
                Chunk.Room.ETileType.Grass => "草地 | 户外 | 可行走",
                Chunk.Room.ETileType.Dirt => "泥土 | 户外 | 可行走",
                Chunk.Room.ETileType.Stone => "石头 | 户外 | 可行走",
                _ => ""
            };
        }
    }
}