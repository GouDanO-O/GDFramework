using Core.Game.Chunk.Data.Interface;

namespace Core.Game.View.Details.Interface
{
    public interface IUI_EditorDetail_MapNode
    {
        void SetDestroy();

        IChunkDtoDef GetThisNodeDtoDef();

        bool GetThisNodeIsLocking();
        
        
        void ChangeSelecting(bool isSelecting);

        void ChangeInitialNode(bool isInitialNode);
    }
}