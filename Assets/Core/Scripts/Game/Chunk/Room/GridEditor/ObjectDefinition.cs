using System;
using Core.Game.Chunk.Room.Grid;
using UnityEngine;

namespace Core.Game.Chunk.Room.GridEditor
{
    /// <summary>
    /// 物品定义（临时，后续需要完善）
    /// </summary>
    [Serializable]
    public class ObjectDefinition
    {
        public string Id;
        public string Name;
        public Sprite Icon;
        public ObjectSize Size;
        public ObjectCategory Category;
    }
}