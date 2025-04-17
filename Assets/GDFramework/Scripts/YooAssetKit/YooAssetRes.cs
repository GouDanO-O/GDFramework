using GDFrameworkCore;
using GDFrameworkExtend.PoolKit;
using GDFrameworkExtend.ResKit;
using UnityEngine;
using YooAsset;

namespace GDFramework.YooAssetKit
{
    public class YooAssetRes : Res,IController
    {
        private string _location;

        private YooAssetManager _yooAssetManager => this.GetSystem<YooAssetManager>();
        
        public override bool LoadSync()
        {
            var package = YooAssets.GetPackage("DefaultPackage");
            var _syncOperationHandle = YooAssets.LoadAssetSync<Object>(_location);
            mAsset = _syncOperationHandle.AssetObject;
            State = ResState.Ready;
            _yooAssetManager.AddAssetHandle(_syncOperationHandle,package);
            return true;
        }
        

        public override async void LoadAsync()
        {
            var package = YooAssets.GetPackage("DefaultPackage");
            var  _asyncOperationHandle =YooAssets.LoadAssetSync<Object>(_location);
            await _asyncOperationHandle.Task;
            mAsset = _asyncOperationHandle.AssetObject;
            State = ResState.Ready;
            _yooAssetManager.AddAssetHandle(_asyncOperationHandle,package);
        }

        protected override void OnReleaseRes()
        {
            mAsset = null;
            State = ResState.Waiting;
            _yooAssetManager.Release();
        }

        public static YooAssetRes Allocate(string name,string originalAssetName)
        {
            var res = SafeObjectPool<YooAssetRes>.Instance.Allocate();
            if (res != null)
            {
                if (originalAssetName.StartsWith("yoo://"))
                {
                    originalAssetName = originalAssetName.Substring("yoo://".Length);
                }
                if(originalAssetName.StartsWith("yoo:"))
                {
                    originalAssetName = originalAssetName.Substring("yoo:".Length);
                }
                
                res.AssetName = name;
                res._location = originalAssetName;
            }
            return res;
        }

        public IArchitecture GetArchitecture()
        {
            return Main.Interface;
        }
    }
    
    public class YooAssetResCreator : IResCreator
    {
        public IRes Create(ResSearchKeys resSearchKeys)
        {
            YooAssetRes res = YooAssetRes.Allocate(resSearchKeys.AssetName,resSearchKeys.OriginalAssetName);
            res.AssetType = resSearchKeys.AssetType;
            return res;
        }

        public bool Match(ResSearchKeys resSearchKeys)
        {
            return resSearchKeys.OriginalAssetName.StartsWith("yoo:") || resSearchKeys.OriginalAssetName.StartsWith("yoo:");
        }
    }
}