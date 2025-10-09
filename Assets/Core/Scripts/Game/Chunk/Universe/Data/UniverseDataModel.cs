using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.World;
using Core.Game.Chunk.World.Data;
using GDFrameworkCore;
using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Universe.Data
{
    public class UniverseDataModel : ChunkDataModel
    {
        [LabelText("当前宇宙固定数据")]
        private UniverseDto _universeDto;
        
        [LabelText("当前宇宙临时数据")]
        private UniverseDtoTemporary _universeDtoTemporary;
        
        private UniverseDataUtility _universeDataUtility;
        
        /// <summary>
        /// 上一次离开的世界数据
        /// </summary>
        private WorldData _lastWorldData;
        
        /// <summary>
        /// 当前的世界数据
        /// </summary>
        private WorldData _curHoldingWorldData;
        
        /// <summary>
        /// 当前宇宙所拥有的所有世界数据
        /// </summary>
        private List<WorldData> _universeWorldDataList = new List<WorldData>();

        protected override void OnInit()
        {
            base.OnInit();
            this._universeDataUtility = this.GetUtility<UniverseDataUtility>();
        }

        /// <summary>
        /// 改变世界
        /// </summary>
        /// <param name="willChangeWorld"></param>
        /// <param name="lastWorld"></param>
        public void ChangeWorld(WorldData willChangeWorld, WorldData lastWorld)
        {
            if (_universeDataUtility.VerifyCanChangeWorld(willChangeWorld))
            {
                
            }
            
        }
    }
}