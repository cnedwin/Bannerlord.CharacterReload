using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CharacterReload.Utils
{
    class ReflectUtils
    {

        public static BindingFlags GetBindingFlags()
        {
            return BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static;
        }

        public static T ReflectField<T>(String key, object instance)
        {
            T result = default;
            FieldInfo fieldInfo = instance.GetType().GetField(key, GetBindingFlags());
            if (null != fieldInfo && null != fieldInfo.GetValue(instance))
            {
                result = (T)fieldInfo.GetValue(instance);
            }

            return result;
        }

    

        public static object ReflectPropertyAndSetValue(String key, object value, object instance)
        {
            object result = null;
            PropertyInfo propertyInfo = instance.GetType().GetProperty(key, GetBindingFlags());
            if (null != propertyInfo)
            {
                propertyInfo.SetValue(instance, value);
            }
            return result;
        }

        public static T ReflectProperty<T>(String key, object instance)
        {
            T result = default;
            PropertyInfo propertyInfo = instance.GetType().GetProperty(key, GetBindingFlags());
            if (null != propertyInfo)
            {
                result = (T)propertyInfo.GetValue(instance);
            }
            return result;
        }

        public static void ReflectMethodAndInvoke(String mothodName, object instance, object[] paramObjects)
        {
            MethodInfo methodInfo = instance.GetType().GetMethod(mothodName, GetBindingFlags());
            if (null != methodInfo)
            {
                methodInfo.Invoke(instance, paramObjects);
            }
        }

        public static T DeepCopy<T>(T obj)
        {
            //如果是字符串或值类型则直接返回
            if (obj is string || obj.GetType().IsValueType) return obj;

            object retval = Activator.CreateInstance(obj.GetType());
            FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (FieldInfo field in fields)
            {
                try { field.SetValue(retval, DeepCopy(field.GetValue(obj))); }
                catch { }
            }
            return (T)retval;
        }
    }
}
