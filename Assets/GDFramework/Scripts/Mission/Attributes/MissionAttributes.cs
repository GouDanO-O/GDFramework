using System;
using System.Linq;
using UnityEngine;

namespace GDFramework.Mission
{
    public static class MissionAttributes
    {
        /// <summary>
        /// 获取Action的默认名称
        /// </summary>
        /// <param name="action"></param>
        /// <param name="attr"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool FetchAttribute<T>(this object action, out T attr) where T : System.Attribute
        {
            var type = action.GetType();
            var attrs = type.GetCustomAttributes(typeof(T), true);
            attr = attrs.Length > 0 ? (T)attrs[0] : null;
            return attr != null;
        }

        /// <summary>
        /// 将所有[SeralizedField]字段重置为默认值
        /// </summary>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        public static void ResetObject<T>(T obj) where T : class
        {
            if(obj is null) return;

            /* get all fields */
            var type = obj.GetType();
            var fields = type.GetFields(
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic);

            /* reset all [SerializeField] field to default value */
            foreach (var field in fields.Where(x => x.IsDefined(typeof(SerializeField), true)))
                field.SetValue(obj, field.FieldType.IsValueType ? Activator.CreateInstance(field.FieldType) : null);
        }


        /// <summary>
        /// 复制对象的简单实现
        /// </summary>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CopyObject<T>(T obj) where T : class
        {
            if (obj is null) return null;
            if (obj is string) return obj;
            if (obj.GetType().IsAbstract) return null;

            /* get all fields */
            var type = obj.GetType();
            var fields = type.GetFields(
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic);

            /* create new instance */
            var newObj = Activator.CreateInstance(type);

            /* copy all [SerializeField] field to new object */
            foreach (var field in fields.Where(x => x.IsDefined(typeof(SerializeField), true)))
                field.SetValue(newObj, field.GetValue(obj));

            return newObj as T;
        }


        /// <summary>
        /// 从其他对象复制对象数据
        /// </summary>
        /// <param name="self"></param>
        /// <param name="other"></param>
        /// <typeparam name="T"></typeparam>
        public static void CopyObjectFrom<T>(T self, T other) where T : class
        {
            if (self is null || other is null) return;
            if (self.GetType() != other.GetType()) return;

            /* get all fields */
            var type = self.GetType();
            var fields = type.GetFields(
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic);

            /* copy all [SerializeField] field to new object */
            foreach (var field in fields.Where(x => x.IsDefined(typeof(SerializeField), true)))
                field.SetValue(self, field.GetValue(other));
        }
    }
}