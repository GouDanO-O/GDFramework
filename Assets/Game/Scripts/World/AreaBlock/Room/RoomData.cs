using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GDFramework.Utility;
using GDFrameworkExtend.Data;
using GDFrameworkExtend.JsonKit;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Game.World
{
    [Serializable]
    public class RoomData
    {
        public RoomDto roomDto;
    }
}