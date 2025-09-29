using GDFrameworkExtend.Data;

namespace Game.World.Interface
{
    public interface IData
    {
        /// <summary>
        /// 独一无二的ID
        /// </summary>
        public string UniqueId { get; set; }

        /// <summary>
        /// 拼接ID
        /// 根据父节点_当前节点的ID进行拼接
        /// </summary>
        /// <param name="fatherDataId"></param>
        /// <param name="thisDataId"></param>
        public void CombineUniqueId(string fatherDataId, string thisDataId)
        {
            this.UniqueId = fatherDataId + thisDataId;
        }

        /// <summary>
        /// 获得固定数据
        /// </summary>
        /// <param name="dtoData"></param>
        public void GetPersistentData(Dto dtoData)
        {
            
        }

        /// <summary>
        /// 获得临时数据
        /// </summary>
        /// <param name="data"></param>
        public void GetTemporaryData(TemporaryData data)
        {
            
        }

        /// <summary>
        /// 保存临时数据
        /// </summary>
        public void SaveTemporaryData()
        {
            
        }
    }
}