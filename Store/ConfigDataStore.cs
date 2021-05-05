using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Engine;
using Newtonsoft.Json;
using TaleWorlds.Library;
using TaleWorlds.Core;

namespace CharacterReload.Data
{
    abstract class ConfigDataStore
    {

        private string _configRootPath = "CharacterReload";

        PlatformDirectoryPath directory = EngineFilePaths.ConfigsPath;
        IPlatformFileHelper platformFileHelper = Common.PlatformFileHelper;

        public object LoadDataFromConfig(String key, Type type)
        {
            string path = System.IO.Path.Combine(_configRootPath, key + ".json");
            PlatformFilePath filePath = new PlatformFilePath(EngineFilePaths.ConfigsPath, path);
            if (platformFileHelper.FileExists(filePath))
            {
                try
                {

                    string json = Common.PlatformFileHelper.GetFileContentString(filePath);
                    if (json != null)
                        {
                            return JsonConvert.DeserializeObject(json, type);
                        }
                }
                catch (JsonException e)
                {
                    InformationManager.DisplayMessage(new InformationMessage("CharacterReload load FacGenRecordData failed" + e.Message));
                    return null;
                }
            }
            return null;
        }

        public void SaveDataIntoConfig(String key, object objectValue)
        {
            try
            {
                string path = System.IO.Path.Combine(_configRootPath, key  + ".json");

                PlatformFilePath filePath = new PlatformFilePath(EngineFilePaths.ConfigsPath, path);
                string json = JsonConvert.SerializeObject(objectValue, Newtonsoft.Json.Formatting.Indented);
                Common.PlatformFileHelper.SaveFileString(filePath, json);
                // StreamWriter streamWriter = new StreamWriter(path, false);
                // streamWriter.Write(json);
                // streamWriter.Flush();// 清空缓存
                // streamWriter.Close();
            }
            catch (JsonException e)
            {
                //InformationManager.DisplayMessage(new InformationMessage("CharacterReload save FacGenRecordData failed" + e.Message));
            }
        }
    }
}
