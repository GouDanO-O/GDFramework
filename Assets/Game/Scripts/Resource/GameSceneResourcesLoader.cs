using System.Linq;
using System.Collections.Generic;
using Game.Models.Resource;
using GDFramework.FrameData;
using GDFramework.Resource;
using GDFrameworkCore;
using UnityEngine;
using YooAsset;

public class GameSceneResourcesLoader : BaseResourcesLoader
{
    private GameSceneResourcesDataModel _model;

    protected override void AddLoadingResource()
    {
        _model = this.GetModel<GameSceneResourcesDataModel>();
        
        WillLoadResourcesList.Add(new SResourcesLoaderNode
        {
            dataName = DefaultPackage.GameConfig.WorldDataAssetGroup.WorldData, 
            loaderCallback = data =>
            {
                _model.WorldDataAsset = data as TextAsset;
                LoadingCheck();
            }
        });

        // // 1) 批量添加 AreaBlocks
        // AddJsonFolderByLabel("area_blocks",
        //     onOneJson: (addr, ta) => { _model.AddAreaBlock(addr, ta); });
        //
        // // 2) 批量添加 Rooms
        // AddJsonFolderByLabel("rooms",
        //     onOneJson: (addr, ta) => { _model.AddRoom(addr, ta); });
        //
        // // 3) 批量添加 Nodes
        // AddJsonFolderByLabel("nodes",
        //     onOneJson: (addr, ta) => { _model.AddNode(addr, ta); });
    }

    /// <summary>
    /// 将 label 下所有 .json 资源转成一个个加载节点加入队列
    /// </summary>
    private void AddJsonFolderByLabel(string label, System.Action<string, TextAsset> onOneJson)
    {
        var pkg = YooAssets.GetPackage("DefaultPackage");
        var infos = pkg.GetAssetInfos(label);

        // 只取 .json，并按地址排序，保证顺序稳定
        foreach (var info in infos
            .Where(i => i.AssetPath.EndsWith(".json", System.StringComparison.OrdinalIgnoreCase))
            .OrderBy(i => i.Address))
        {
            var yooAddress = $"yoo:{info.Address}"; // 让你的 YooAssetResCreator 匹配到

            WillLoadResourcesList.Add(new SResourcesLoaderNode
            {
                dataName = yooAddress,
                loaderCallback = obj =>
                {
                    var ta = obj as TextAsset;
                    if (ta == null)
                    {
                        Debug.LogError($"期望 TextAsset，但得到 {obj?.GetType().Name} ：{yooAddress}");
                    }
                    else
                    {
                        onOneJson?.Invoke(info.Address, ta);
                    }
                    LoadingCheck();
                }
            });
        }
    }
}
