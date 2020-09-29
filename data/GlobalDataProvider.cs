using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace CharacterReload.Data
{
    class GlobalDataProvider
    {

        private static GlobalDataProvider _instance;

        private List<FacGenRecordData> _facGenRecordData;

        public static GlobalDataProvider Instance
        {
            get
            {
                if (null == _instance)
                {
                    _instance = new GlobalDataProvider();
                }

                return _instance;
            }
        }

        public List<FacGenRecordData> FacGenRecordData()
        {
            if(null == _facGenRecordData)
            {
                _facGenRecordData = new List<FacGenRecordData>();
            }

            return this._facGenRecordData;
        }

        private GlobalDataProvider()
        {
            this.LoadFacGenData();
        }

        public void LoadFacGenData()
        {
            string path = System.IO.Path.Combine(Utilities.GetConfigsPath(), "CharacterReload", "CharacterReload.json");
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
                            this._facGenRecordData = (List<FacGenRecordData>)JsonConvert.DeserializeObject(json, typeof(List<FacGenRecordData>));
                        }

                    }
                }
                catch (JsonException e)
                {
                    InformationManager.DisplayMessage(new InformationMessage("FaceDetailsCreator load FacGenRecordData failed" + e.Message));
                }
            }
        }

        public void SaveFacGenData()
        {
            try
            {
                string dic = System.IO.Path.Combine(Utilities.GetConfigsPath(), "CharacterReload");
                string path = System.IO.Path.Combine(dic, "CharacterReload.json");

                System.IO.Directory.CreateDirectory(dic);

                string json = JsonConvert.SerializeObject(this._facGenRecordData , Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(path, json);
               // StreamWriter streamWriter = new StreamWriter(path, false);
               // streamWriter.Write(json);
               // streamWriter.Flush();// 清空缓存
               // streamWriter.Close();
            }
            catch (JsonException e)
            {
                InformationManager.DisplayMessage(new InformationMessage("FaceDetailsCreator save FacGenRecordData failed" + e.Message));
            }
        }
    }
}
