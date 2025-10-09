using Core.Game.Chunk.Node.Conditions.Interface;

namespace Core.Game.Chunk.Node.Conditions
{
    public abstract class NodeCondition : INodeCondition
    {
        public abstract bool CheckCondition();
    }
}