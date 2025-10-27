using System;
using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Tile
{
    /// <summary>
    /// 瓦片共分为如下层级:(由基础层依次去渲染,越上层,渲染层级越高)
    /// 基础层-->瓦片本身贴图
    /// 底部装饰层-->瓦片上面的装饰(可以是花,草,碎石等),根据装饰渲染优先级,依次去渲染(如果同级则按照先后顺序去渲染)
    /// 物体层-->瓦片上面的物体,根据渲染优先级,依次去渲染(如果同级则按照先后顺序去渲染)
    /// </summary>
    public class TileData : ChunkData
    {
        [LabelText("瓦片固定数据")]
        private TileDto _tileDto;
        
        [LabelText("瓦片临时数据")]
        private TileDtoTemporary _tileDtoTemporary;
        
        [LabelText("瓦片上放置的物体(首先读取存档,如果没有,那么就取固定配置中的物体)")]
        private List<string> _tilePlacedNodeIdList = new List<string>();
        
        [LabelText("瓦片上放置的装饰物(首先读取存档,如果没有,那么就取固定配置中的物体)")]
        private List<string> _tilePlacedDecorationIdList = new List<string>();

        private void SetTileData()
        {
            if (_tileDtoTemporary.curTilePlacedDecorationIdList.Count == 0)
            {
                for (int i = 0; i < _tileDto.tileDtoDef.decorationIdList.Count; i++)
                {
                    _tilePlacedDecorationIdList.Add(_tileDto.tileDtoDef.decorationIdList[i]);
                }
            }
            else
            {
                for (int i = 0; i < _tileDtoTemporary.curTilePlacedDecorationIdList.Count; i++)
                {
                    _tilePlacedDecorationIdList.Add(_tileDtoTemporary.curTilePlacedDecorationIdList[i]);
                }
            }
            
            if (_tileDtoTemporary.curTilePlacedDecorationIdList.Count == 0)
            {
                for (int i = 0; i < _tileDto.tileDtoDef.decorationIdList.Count; i++)
                {
                    _tilePlacedNodeIdList.Add(_tileDto.tileDtoDef.decorationIdList[i]);
                }
            }
            else
            {
                for (int i = 0; i < _tileDtoTemporary.curTilePlacedDecorationIdList.Count; i++)
                {
                    _tilePlacedNodeIdList.Add(_tileDtoTemporary.curTilePlacedDecorationIdList[i]);
                }
            }
        }
    }
}