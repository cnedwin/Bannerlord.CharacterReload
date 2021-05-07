﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace CharacterReload.Data
{
    class GlobalDataProvider
    {

        private static GlobalDataProvider _instance;

        private List<FacGenRecordData> _facGenRecordData;

        PlatformDirectoryPath directory = EngineFilePaths.ConfigsPath;
        IPlatformFileHelper platformFileHelper = Common.PlatformFileHelper;

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
            string path = System.IO.Path.Combine("CharacterReload", "CharacterReload.json");
            PlatformFilePath filePath = new PlatformFilePath(EngineFilePaths.ConfigsPath,path);
            if (platformFileHelper.FileExists(filePath))
            {
                try
                {
                    
                        string json = Common.PlatformFileHelper.GetFileContentString(filePath);
                        if (json != null)
                        {
                            this._facGenRecordData = (List<FacGenRecordData>)JsonConvert.DeserializeObject(json, typeof(List<FacGenRecordData>));
                        }

                    
                }
                catch (JsonException e)
                {
                    InformationManager.DisplayMessage(new InformationMessage("CharacterReload load FacGenRecordData failed" + e.Message));
                }
            }
        }

        public void SaveFacGenData()
        {
            try
            {

                string path = System.IO.Path.Combine("CharacterReload", "CharacterReload.json");
                PlatformFilePath filePath = new PlatformFilePath(EngineFilePaths.ConfigsPath, path);

                string json = JsonConvert.SerializeObject(this._facGenRecordData , Newtonsoft.Json.Formatting.Indented);
                Common.PlatformFileHelper.SaveFileString(filePath,json);
               // StreamWriter streamWriter = new StreamWriter(path, false);
               // streamWriter.Write(json);
               // streamWriter.Flush();// 清空缓存
               // streamWriter.Close();
            }
            catch (JsonException e)
            {
                InformationManager.DisplayMessage(new InformationMessage("CharacterReload save FacGenRecordData failed" + e.Message));
            }
        }
    }
}
