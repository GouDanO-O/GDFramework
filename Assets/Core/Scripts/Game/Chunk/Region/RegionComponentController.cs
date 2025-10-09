using GDFrameworkCore;
using UnityEngine;

namespace Core.Game.Chunk.Region
{
    public class RegionComponentController : MonoBehaviour,IController
    {
        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }
    }
}