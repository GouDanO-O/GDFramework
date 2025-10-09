using Core.Game.Chunk.World;
using Core.Game.Chunk.World.Data;
using GDFrameworkCore;

namespace Core.Game.Chunk.Universe.Data
{
    public class UniverseDataUtility : IUtility
    {
        private UniverseDataModel _universeData;

        /// <summary>
        /// 验证是否能够进行切换世界
        /// </summary>
        public bool VerifyCanChangeWorld(WorldData willChangeWorld)
        {
            return true;
        }

        /// <summary>
        /// 获取上次登录的焦点世界
        /// </summary>
        /// <returns></returns>
        public WorldData GetLastFocusWorld()
        {
            string lastWorldId = "";
            if (_universeData.UniverseDtoTemporary == null)
            {
                lastWorldId = _universeData.UniverseDto.universeDtoDef.initialPlayerLocateWorldId;
            }
            else
            {
                lastWorldId = _universeData.UniverseDtoTemporary.lastFocusWorldId;
            }

            return FindWorldFromId(lastWorldId);
        }

        /// <summary>
        /// 根据世界ID去查找世界
        /// </summary>
        /// <param name="worldId"></param>
        /// <returns></returns>
        public WorldData FindWorldFromId(string worldId)
        {
            return _universeData.UniverseWorldDataDict.ContainsKey(worldId) ? _universeData.UniverseWorldDataDict[worldId] : null;
        }
    }
}