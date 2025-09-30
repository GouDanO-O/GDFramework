using System;
using Core.Game.Chunk.Node.Action.Interface;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Node.Action
{
    [Serializable]
    public abstract class NodeAction : INodeAction
    {
        [TextArea(1, 2), LabelText("动作描述")] 
        public string description;
        
        public abstract void Execute();
    }
}