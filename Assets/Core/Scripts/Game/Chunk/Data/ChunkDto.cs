using System;
using Core.Game.Chunk.Interface;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Core.Game.Chunk.Data
{
    [Serializable]
    public abstract class ChunkDto : ScriptableObject,IChunkDto
    {
        [LabelText("昵称")]
        public string dtoName;

        [LabelText("ID")]
        public string dtoId;

        [LabelText("描述")]
        public string dtoDescription;
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            ChangingDtoData();
        }
#endif
        
        /// <summary>
        /// 当数据改变时
        /// </summary>
        protected virtual void ChangingDtoData()
        {
            
        }
    }
}