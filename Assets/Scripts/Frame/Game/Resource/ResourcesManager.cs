using Frame.Models;
using Frame.Utility;
using QFramework;
using UnityEngine;
using UnityEngine.Events;

namespace Frame.Game.Resource
{
    public class ResourcesManager : AbstractSystem
    {
        public UnityAction onFirstLoadComplete;
        public int loadedCount { get; private set; }

        private int maxLoadCount = 1;

        Resouces_Utility m_loader;

        ResourcesData_Model m_resourcesDataModel;

        protected override void OnInit()
        {
            m_loader = this.GetUtility<Resouces_Utility>();
            m_loader.InitLoader();
            m_resourcesDataModel=this.GetModel<ResourcesData_Model>();
        }

        /// <summary>
        /// 加载资源数据
        /// </summary>
        public void InitialLoad()
        {
            if (m_loader != null && m_resourcesDataModel!=null)
            {
                m_loader.LoadJsonAsync(QAssetBundle.Tbmultilingual_json.tbmultilingual, (data) =>
                {
                    this.GetModel<MultilingualData_Model>().languageTextAsset = data;
                    InitialLoadCheck();
                });
            }
        }
        
        /// <summary>
        /// 每加载一个就进行检测
        /// </summary>
        private void InitialLoadCheck()
        {
            loadedCount++;
            if (loadedCount == maxLoadCount)
            {
                onFirstLoadComplete?.Invoke();
                Debug.Log("加载完成");
            }
            Debug.Log("加载数据成功");
        }
    }
}

