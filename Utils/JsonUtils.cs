using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharacterReload.Utils
{
    class JsonUtils
    {
        /*
         * 使用json 转换进行对象拷贝
         */
        public static T DeepCopyByJson<T>(T obj)
        {
            string json = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);//对象转成成json
            T t = (T)JsonConvert.DeserializeObject(json, obj.GetType()); //json 转成对象
            return t;
        }
    }
}