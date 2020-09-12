using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FaceDetailsCreator.utils
{
    class ReflectUtils
    {

        public static BindingFlags GetBindingFlags()
        {
            return BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static;
        }

        public static object ReflectField(String key, object instance)
        {
            object result = null;
            FieldInfo fieldInfo = instance.GetType().GetField(key, GetBindingFlags());
            if (null != fieldInfo)
            {
                result = fieldInfo.GetValue(instance);
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

        public static void ReflectMethodAndInvoke(String mothodName, object instance, object[] paramObjects)
        {
            MethodInfo methodInfo = instance.GetType().GetMethod(mothodName, GetBindingFlags());
            if (null != methodInfo)
            {
                methodInfo.Invoke(instance, paramObjects);
            }
        }
    }
}
