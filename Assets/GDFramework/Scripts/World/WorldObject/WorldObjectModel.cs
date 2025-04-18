using System.Collections.Generic;
using GDFramework.LubanKit.Cfg;
using GDFramework.LubanKit.Cfg.worldObj;
using GDFramework.Utility;
using GDFrameworkCore;
using NUnit.Framework;

namespace GDFramework.World.Object
{
    public class WorldObjectModel : AbstractModel,ICanGetSystem
    {
        private LubanTablesData _lubanTablesData => this.GetSystem<LubanKit.LubanKit>().LubanTablesData;
        
        //当前世界上存活的物体
        private List<IObj>  _curLivingWorldObjList = new List<IObj>();

        //根据物体的ID去存储当前物体存活列表
        private Dictionary<int, List<IObj>> _curLivingWorldObjDict = new Dictionary<int, List<IObj>>();
        
        protected override void OnInit()
        {
            
        }

        /// <summary>
        /// 获取世界物体数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public WorldObj GetWorldObjData(int id)
        {
            return _lubanTablesData.TbObj.GetOrDefault(id);
        }

        /// <summary>
        /// 根据ID来生成世界物体
        /// </summary>
        /// <param name="obj"></param>
        public void SpawnObj(IObj obj)
        {
            _curLivingWorldObjList.Add(obj);
            
            if (_curLivingWorldObjDict.ContainsKey(obj.UniqueObjId))
            {
                _curLivingWorldObjDict[obj.UniqueObjId].Add(obj);
            }
            else
            {
                List<IObj> livingObjList = new List<IObj>();
                livingObjList.Add(obj);
                _curLivingWorldObjDict.Add(obj.UniqueObjId,livingObjList);
            }
        }

        /// <summary>
        /// 移除世界物体
        /// </summary>
        /// <param name="obj"></param>
        public void DestroyObj(IObj obj)
        {
            _curLivingWorldObjList.Remove(obj);
            
            if (_curLivingWorldObjDict.ContainsKey(obj.UniqueObjId))
            {
                _curLivingWorldObjDict[obj.UniqueObjId].Remove(obj);
            }
            else
            {
                LogMonoUtility.AddErrorLog("当前物体并未存储在字典中,请检查数据");
            }
        }
    }
}