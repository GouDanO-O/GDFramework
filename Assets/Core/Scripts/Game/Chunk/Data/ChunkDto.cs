using Core.Game.Chunk.Interface;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Core.Game.Chunk.Data
{
    [ES3Serializable]
    public abstract class ChunkDto : ScriptableObject,IChunkDto
    {
        public string DtoName { get; set; }
        
        public int UniqueDtoId { get; set; }
        
        public string DtoId { get; set; }
        
        public string DtoDescription { get; set; }
    }
}