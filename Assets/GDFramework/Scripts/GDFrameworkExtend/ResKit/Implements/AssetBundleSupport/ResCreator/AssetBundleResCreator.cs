using UnityEngine;

namespace GDFrameworkExtend.ResKit
{
    public class AssetBundleResCreator : IResCreator
    {
        public bool Match(ResSearchKeys resSearchKeys)
        {
            return resSearchKeys.AssetType == typeof(AssetBundle);
        }

        public IRes Create(ResSearchKeys resSearchKeys)
        {
            return AssetBundleRes.Allocate(resSearchKeys.AssetName);
        }
    }
}