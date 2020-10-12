using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Engine;
using Newtonsoft.Json;

namespace CharacterReload.Data
{
    abstract class ConfigDataStore
    {

        private string _configRootPath = "CharacterReload";

        public object LoadDataFromConfig(String key, Type type)
        {
            string path = System.IO.Path.Combine(Utilities.GetConfigsPath(), _configRootPath, key + ".json");
            FileInfo fileInfo = new FileInfo(path);
            if (fileInfo.Exists)
            {
                try
                {
                    using (StreamReader streamReader = fileInfo.OpenText())
                    {
                        string json = streamReader.ReadToEnd();
                        if (json != null)
                        {
                            return JsonConvert.DeserializeObject(json, type);
                        }
                    }
                }
                catch (JsonException e)
                {
                    //InformationManager.DisplayMessage(new InformationMessage("FaceDetailsCreator load FacGenRecordData failed" + e.Message));
                    return null;
                }
            }
            return null;
        }

        public void SaveDataIntoConfig(String key, object objectValue)
        {
            try
            {
                string dic = System.IO.Path.Combine(Utilities.GetConfigsPath(), _configRootPath);
                string path = System.IO.Path.Combine(dic, key  + ".json");

                System.IO.Directory.CreateDirectory(dic);
                string json = JsonConvert.SerializeObject(objectValue, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(path, json);
                // StreamWriter streamWriter = new StreamWriter(path, false);
                // streamWriter.Write(json);
                // streamWriter.Flush();// 清空缓存
                // streamWriter.Close();
            }
            catch (JsonException e)
            {
                //InformationManager.DisplayMessage(new InformationMessage("FaceDetailsCreator save FacGenRecordData failed" + e.Message));
            }
        }
    }
}
