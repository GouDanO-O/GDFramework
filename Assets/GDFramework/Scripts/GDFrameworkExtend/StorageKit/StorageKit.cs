using System;
using System.Collections.Generic;
using GDFrameworkCore;
using GDFrameworkExtend.Data;
using UnityEngine;

namespace GDFrameworkExtend.StorageKit
{
    public class StorageKit : AbstractSystem
    {
        private Dictionary<Type, string> _persistentDataKeys = new Dictionary<Type, string>();
        private Dictionary<Type, string> _temporalityDataKeys = new Dictionary<Type, string>();
        private Dictionary<Type, string> _modelKeys = new Dictionary<Type, string>();

        private string _currentSaveSlot = "DefaultSave";

        protected override void OnInit()
        {
            // 初始化默认存档槽
            SetCurrentSaveSlot("DefaultSave");
        }

        /// <summary>
        /// 设置当前存档槽
        /// </summary>
        /// <param name="saveSlot">存档槽名称</param>
        public void SetCurrentSaveSlot(string saveSlot)
        {
            _currentSaveSlot = saveSlot;
        }

        /// <summary>
        /// 获取当前存档槽
        /// </summary>
        /// <returns></returns>
        public string GetCurrentSaveSlot()
        {
            return _currentSaveSlot;
        }

        #region Model存储相关方法

        /// <summary>
        /// 保存AbstractModel数据
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="model">要保存的Model实例</param>
        public void SaveModel<T>(T model) where T : AbstractModel
        {
            string key = GetModelKey<T>();
            try
            {
                ES3.Save(key, model);
                Debug.Log($"Model {typeof(T).Name} saved successfully with key: {key}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save model {typeof(T).Name}: {e.Message}");
            }
        }

        /// <summary>
        /// 加载AbstractModel数据
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="defaultValue">默认值</param>
        /// <returns>加载的Model实例</returns>
        public T LoadModel<T>(T defaultValue = null) where T : AbstractModel
        {
            string key = GetModelKey<T>();
            try
            {
                if (ES3.KeyExists(key))
                {
                    T loadedModel = ES3.Load<T>(key);
                    Debug.Log($"Model {typeof(T).Name} loaded successfully with key: {key}");
                    return loadedModel;
                }
                else
                {
                    Debug.Log($"No saved data found for model {typeof(T).Name}, returning default value");
                    return defaultValue;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load model {typeof(T).Name}: {e.Message}");
                return defaultValue;
            }
        }

        /// <summary>
        /// 检查Model是否存在存档
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <returns>是否存在存档</returns>
        public bool HasModelSave<T>() where T : AbstractModel
        {
            string key = GetModelKey<T>();
            return ES3.KeyExists(key);
        }

        /// <summary>
        /// 删除Model存档
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        public void DeleteModel<T>() where T : AbstractModel
        {
            string key = GetModelKey<T>();
            if (ES3.KeyExists(key))
            {
                ES3.DeleteKey(key);
                Debug.Log($"Model {typeof(T).Name} save data deleted");
            }
        }

        private string GetModelKey<T>() where T : AbstractModel
        {
            Type type = typeof(T);
            if (!_modelKeys.ContainsKey(type))
            {
                _modelKeys[type] = $"Model_{type.Name}_{_currentSaveSlot}";
            }

            return _modelKeys[type];
        }

        #endregion

        #region PersistentData存储相关方法

        /// <summary>
        /// 保存PersistentData数据（全局数据，不受存档影响）
        /// </summary>
        /// <typeparam name="T">PersistentData类型</typeparam>
        /// <param name="data">要保存的数据实例</param>
        public void SavePersistentData<T>(T data) where T : PersistentData
        {
            string key = GetPersistentDataKey<T>();
            try
            {
                ES3.Save(key, data);
                Debug.Log($"PersistentData {typeof(T).Name} saved successfully with key: {key}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save PersistentData {typeof(T).Name}: {e.Message}");
            }
        }

        /// <summary>
        /// 加载PersistentData数据
        /// </summary>
        /// <typeparam name="T">PersistentData类型</typeparam>
        /// <param name="defaultValue">默认值</param>
        /// <returns>加载的数据实例</returns>
        public T LoadPersistentData<T>(T defaultValue = null) where T : PersistentData
        {
            string key = GetPersistentDataKey<T>();
            try
            {
                if (ES3.KeyExists(key))
                {
                    T loadedData = ES3.Load<T>(key);
                    Debug.Log($"PersistentData {typeof(T).Name} loaded successfully with key: {key}");
                    return loadedData;
                }
                else
                {
                    Debug.Log($"No saved data found for PersistentData {typeof(T).Name}, returning default value");
                    return defaultValue;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load PersistentData {typeof(T).Name}: {e.Message}");
                return defaultValue;
            }
        }

        /// <summary>
        /// 检查PersistentData是否存在存档
        /// </summary>
        /// <typeparam name="T">PersistentData类型</typeparam>
        /// <returns>是否存在存档</returns>
        public bool HasPersistentDataSave<T>() where T : PersistentData
        {
            string key = GetPersistentDataKey<T>();
            return ES3.KeyExists(key);
        }

        /// <summary>
        /// 删除PersistentData存档
        /// </summary>
        /// <typeparam name="T">PersistentData类型</typeparam>
        public void DeletePersistentData<T>() where T : PersistentData
        {
            string key = GetPersistentDataKey<T>();
            if (ES3.KeyExists(key))
            {
                ES3.DeleteKey(key);
                Debug.Log($"PersistentData {typeof(T).Name} save data deleted");
            }
        }

        private string GetPersistentDataKey<T>() where T : PersistentData
        {
            Type type = typeof(T);
            if (!_persistentDataKeys.ContainsKey(type))
            {
                // PersistentData不受存档槽影响，全局唯一
                _persistentDataKeys[type] = $"PersistentData_{type.Name}";
            }

            return _persistentDataKeys[type];
        }

        #endregion

        #region TemporalityData存储相关方法

        /// <summary>
        /// 保存TemporalityData数据（临时数据，受存档影响）
        /// </summary>
        /// <typeparam name="T">TemporalityData类型</typeparam>
        /// <param name="data">要保存的数据实例</param>
        public void SaveTemporalityData<T>(T data) where T : TemporalityData
        {
            string key = GetTemporalityDataKey<T>();
            try
            {
                ES3.Save(key, data);
                Debug.Log($"TemporalityData {typeof(T).Name} saved successfully with key: {key}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save TemporalityData {typeof(T).Name}: {e.Message}");
            }
        }

        /// <summary>
        /// 加载TemporalityData数据
        /// </summary>
        /// <typeparam name="T">TemporalityData类型</typeparam>
        /// <param name="defaultValue">默认值</param>
        /// <returns>加载的数据实例</returns>
        public T LoadTemporalityData<T>(T defaultValue = null) where T : TemporalityData
        {
            string key = GetTemporalityDataKey<T>();
            try
            {
                if (ES3.KeyExists(key))
                {
                    T loadedData = ES3.Load<T>(key);
                    Debug.Log($"TemporalityData {typeof(T).Name} loaded successfully with key: {key}");
                    return loadedData;
                }
                else
                {
                    Debug.Log($"No saved data found for TemporalityData {typeof(T).Name}, returning default value");
                    return defaultValue;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load TemporalityData {typeof(T).Name}: {e.Message}");
                return defaultValue;
            }
        }

        /// <summary>
        /// 检查TemporalityData是否存在存档
        /// </summary>
        /// <typeparam name="T">TemporalityData类型</typeparam>
        /// <returns>是否存在存档</returns>
        public bool HasTemporalityDataSave<T>() where T : TemporalityData
        {
            string key = GetTemporalityDataKey<T>();
            return ES3.KeyExists(key);
        }

        /// <summary>
        /// 删除TemporalityData存档
        /// </summary>
        /// <typeparam name="T">TemporalityData类型</typeparam>
        public void DeleteTemporalityData<T>() where T : TemporalityData
        {
            string key = GetTemporalityDataKey<T>();
            if (ES3.KeyExists(key))
            {
                ES3.DeleteKey(key);
                Debug.Log($"TemporalityData {typeof(T).Name} save data deleted");
            }
        }

        private string GetTemporalityDataKey<T>() where T : TemporalityData
        {
            Type type = typeof(T);
            if (!_temporalityDataKeys.ContainsKey(type))
            {
                // TemporalityData受存档槽影响
                _temporalityDataKeys[type] = $"TemporalityData_{type.Name}_{_currentSaveSlot}";
            }

            return _temporalityDataKeys[type];
        }

        #endregion

        #region 存档管理方法

        /// <summary>
        /// 删除整个存档槽的所有数据
        /// </summary>
        /// <param name="saveSlot">存档槽名称</param>
        public void DeleteSaveSlot(string saveSlot)
        {
            string oldSaveSlot = _currentSaveSlot;
            SetCurrentSaveSlot(saveSlot);

            // 删除所有Model和TemporalityData
            _modelKeys.Clear();
            _temporalityDataKeys.Clear();

            // 这里需要根据你的具体需求来删除相关键值
            ES3.DeleteDirectory($"SaveSlot_{saveSlot}");

            Debug.Log($"Save slot {saveSlot} deleted");

            SetCurrentSaveSlot(oldSaveSlot);
        }

        /// <summary>
        /// 检查存档槽是否存在
        /// </summary>
        /// <param name="saveSlot">存档槽名称</param>
        /// <returns>是否存在</returns>
        public bool SaveSlotExists(string saveSlot)
        {
            return ES3.DirectoryExists($"SaveSlot_{saveSlot}");
        }

        /// <summary>
        /// 获取所有存档槽名称
        /// </summary>
        /// <returns>存档槽名称数组</returns>
        public string[] GetAllSaveSlots()
        {
            return ES3.GetDirectories();
        }

        #endregion

        #region 便捷方法

        /// <summary>
        /// 保存所有数据
        /// </summary>
        public void SaveAll()
        {
            // 这里你可以根据需要保存所有已注册的数据
            Debug.Log("SaveAll called - implement based on your specific models");
        }

        /// <summary>
        /// 加载所有数据
        /// </summary>
        public void LoadAll()
        {
            // 这里你可以根据需要加载所有已注册的数据
            Debug.Log("LoadAll called - implement based on your specific models");
        }

        #endregion
    }
}