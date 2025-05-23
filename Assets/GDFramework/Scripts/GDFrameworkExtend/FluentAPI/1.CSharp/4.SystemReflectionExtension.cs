/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 *
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/


using System;
using System.Linq;
using System.Reflection;

namespace GDFrameworkExtend.FluentAPI
{
    public static class SystemReflectionExtension
    {
        public static T CreateInstance<T>(this Type self) where T : class
        {
            // 获取构造函数
            var constructorInfos = self.GetConstructors(BindingFlags.Instance | BindingFlags.Public);

            // 获取无参构造函数
            var ctor = Array.Find(constructorInfos, c => c.GetParameters().Length == 0);

            return ctor.Invoke(null) as T;
        }
        
        public static object ReflectionCallPrivateMethod<T>(this T self, string methodName, params object[] args)
        {
            var type = typeof(T);
            var methodInfo = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);

            return methodInfo?.Invoke(self, args);
        }

        public static TReturnType ReflectionCallPrivateMethod<T, TReturnType>(this T self, string methodName,
            params object[] args)
        {
            return (TReturnType)self.ReflectionCallPrivateMethod(methodName, args);
        }
        
        public static bool HasAttribute<T>(this Type type, bool inherit = false) where T : Attribute
        {
            return type.GetCustomAttributes(typeof(T), inherit).Any();
        }

        public static bool HasAttribute(this Type type, Type attributeType, bool inherit = false)
        {
            return type.GetCustomAttributes(attributeType, inherit).Any();
        }

        public static bool HasAttribute<T>(this PropertyInfo prop, bool inherit = false) where T : Attribute
        {
            return prop.GetCustomAttributes(typeof(T), inherit).Any();
        }


        public static bool HasAttribute(this PropertyInfo prop, Type attributeType, bool inherit = false)
        {
            return prop.GetCustomAttributes(attributeType, inherit).Any();
        }

        public static bool HasAttribute<T>(this FieldInfo field, bool inherit = false) where T : Attribute
        {
            return field.GetCustomAttributes(typeof(T), inherit).Any();
        }
        
        public static bool HasAttribute(this FieldInfo field, Type attributeType, bool inherit)
        {
            return field.GetCustomAttributes(attributeType, inherit).Any();
        }
        
        public static bool HasAttribute<T>(this MethodInfo method, bool inherit = false) where T : Attribute
        {
            return method.GetCustomAttributes(typeof(T), inherit).Any();
        }

        public static bool HasAttribute(this MethodInfo method, Type attributeType, bool inherit = false)
        {
            return method.GetCustomAttributes(attributeType, inherit).Any();
        }
        
        public static T GetAttribute<T>(this Type type, bool inherit = false) where T : Attribute
        {
            return type.GetCustomAttributes<T>(inherit).FirstOrDefault();
        }


        public static object GetAttribute(this Type type, Type attributeType, bool inherit = false)
        {
            return type.GetCustomAttributes(attributeType, inherit).FirstOrDefault();
        }


        public static T GetAttribute<T>(this MethodInfo method, bool inherit = false) where T : Attribute
        {
            return method.GetCustomAttributes<T>(inherit).FirstOrDefault();
        }
        
        public static object GetAttribute(this MethodInfo method, Type attributeType, bool inherit = false)
        {
            return method.GetCustomAttributes(attributeType, inherit).FirstOrDefault();
        }

        public static T GetAttribute<T>(this FieldInfo field, bool inherit = false) where T : Attribute
        {
            return field.GetCustomAttributes<T>(inherit).FirstOrDefault();
        }

        public static object GetAttribute(this FieldInfo field, Type attributeType, bool inherit = false)
        {
            return field.GetCustomAttributes(attributeType, inherit).FirstOrDefault();
        }

        public static T GetAttribute<T>(this PropertyInfo prop, bool inherit = false) where T : Attribute
        {
            return prop.GetCustomAttributes<T>(inherit).FirstOrDefault();
        }
        
        public static object GetAttribute(this PropertyInfo prop, Type attributeType, bool inherit = false)
        {
            return prop.GetCustomAttributes(attributeType, inherit).FirstOrDefault();
        }
    }
}