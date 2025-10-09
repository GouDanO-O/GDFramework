using Core.Game.Chunk.Region.Data;
using GDFrameworkCore;
using UnityEngine;

namespace Core.Game.Chunk.Region
{
    public class RegionComponentController : MonoBehaviour,IController
    {
        private RegionData _curRegionData;
        
        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }
    }
}