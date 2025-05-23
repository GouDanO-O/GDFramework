/****************************************************************************
 * Copyright (c) 2021.4 liangxie
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

using System.Collections.Generic;
using System.Linq;
using GDFrameworkExtend.FluentAPI;
using UnityEditor;

namespace GDFrameworkExtend.ResKit
{
    public class SubProjectData
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Folder { get; set; }

        public List<AssetBundleBuild> Builds = new List<AssetBundleBuild>();


        public static List<SubProjectData> SearchAllInProject()
        {
            return AssetDatabase.FindAssets("t:SubProject")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(assetPath => assetPath.EndsWith(".asset"))
                .Select(assetPath =>
                {
                    var subProject = AssetDatabase.LoadAssetAtPath<SubProject>(assetPath);

                    if (subProject)
                    {
                        return new SubProjectData()
                        {
                            Path = assetPath,
                            Folder = assetPath.RemoveString(subProject.name + ".asset"),
                            Name = subProject.name,
                        };
                    }

                    return null;
                })
                .Where(data => data != null)
                .ToList();
        }

        public static void SplitAssetBundles2DefaultAndSubProjectDatas(SubProjectData defaultSubProjectData,
            List<SubProjectData> subProjectDatas)
        {
            var assetBundleNames = AssetDatabase.GetAllAssetBundleNames();

            foreach (var assetBundleName in assetBundleNames)
            {
                var assetBundleBuild = new AssetBundleBuild
                {
                    assetBundleName = assetBundleName,
                    assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName)
                };
                var isDefault = true;


                foreach (var subProjectData in subProjectDatas)
                {
                    foreach (var assetName in assetBundleBuild.assetNames)
                    {
                        if (assetName.Contains(subProjectData.Folder))
                        {
                            subProjectData.Builds.Add(assetBundleBuild);
                            isDefault = false;
                            break;
                        }
                    }
                }

                if (isDefault)
                {
                    defaultSubProjectData.Builds.Add(assetBundleBuild);
                }
            }
        }
    }
}