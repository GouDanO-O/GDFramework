using System;
using System.Collections.Generic;
using System.IO;
using Core.Game.Chunk.Data;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Universe.Data
{
    [Serializable,JsonObject]
    public class UniverseDtoDef : ChunkDtoDef
    {
        public override string GetTypePrefix()
        {
            return "Universe";
        }

        public void ChangeWorldName(string newName)
        {
            this.DefName = newName;
        }
    }
}