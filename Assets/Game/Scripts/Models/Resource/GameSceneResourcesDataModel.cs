using System.Collections.Generic;
using Game.World;
using GDFramework.YooAssetKit;
using GDFrameworkCore;
using NUnit.Framework;
using UnityEngine;
using YooAsset;


namespace Game.Models.Resource
{
    public class GameSceneResourcesDataModel : AbstractModel,ICanGetSystem
    {
        public Dictionary<string, Dto> DtoRegistry = new Dictionary<string, Dto>();

        public List<WorldDto> AllWorlds
        {
            get; 
            private set;
        } = new List<WorldDto>();

        public List<AreaBlockDto> AllAreaBlocks
        {
            get; 
            private set;
        } = new List<AreaBlockDto>();

        public List<RoomDto> AllRooms
        {
            get; 
            private set;
        } = new List<RoomDto>();

        public List<NodeDto> AllNodes
        {
            get;
            private set;
        } = new List<NodeDto>();
        
        protected override void OnInit()
        {
            
        }
    }
}