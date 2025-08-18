using System.Collections.Generic;
using GDFrameworkCore;
using UnityEngine;


namespace Game.Models.Resource
{
    public class GameSceneResourcesDataModel : AbstractModel
    {
        public TextAsset WorldDataAsset;

        public readonly Dictionary<string, TextAsset> AreaBlocks = new();
        public readonly Dictionary<string, TextAsset> Rooms = new();
        public readonly Dictionary<string, TextAsset> Nodes = new();

        public void AddAreaBlock(string address, TextAsset ta) => 
            AreaBlocks[address] = ta;
        public void AddRoom(string address, TextAsset ta) => 
            Rooms[address] = ta;
        public void AddNode(string address, TextAsset ta) => 
            Nodes[address] = ta;

        protected override void OnInit()
        {
        }
    }
}