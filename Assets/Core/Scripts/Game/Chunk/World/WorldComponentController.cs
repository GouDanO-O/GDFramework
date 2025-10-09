using GDFrameworkCore;
using UnityEngine;

namespace Core.Game.Chunk.World
{
    public class WorldComponentController : MonoBehaviour,IController
    {
        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }
    }
}