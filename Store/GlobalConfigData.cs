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

    /**
     * 提供数据.
     */
    class GlobalConfigData : ConfigDataStore
    {

        private static GlobalConfigData _INSTANCE;

        private List<FacGenRecordData> _facGenRecordData;

        private List<HeroAdminCharacter> _heroCharacters;

    
        public static GlobalConfigData Instance
        {
            get
            {
                if (null == _INSTANCE)
                {
                    _INSTANCE = new GlobalConfigData();
                }

                return _INSTANCE;
            }
        }

        public List<HeroAdminCharacter> HeroAdminData()
        {
            if (null == this._heroCharacters)
            {
                this._heroCharacters = new List<HeroAdminCharacter>();
            }

            return this._heroCharacters;
        }

        public List<FacGenRecordData> FacGenRecordData()
        {
            if(null == this._facGenRecordData)
            {
                this._facGenRecordData = new List<FacGenRecordData>();
            }

            return this._facGenRecordData;
        }

        private GlobalConfigData()
        {
            this.InitData();
        }

        public void InitData()
        {
            object value1 = LoadDataFromConfig("FacGenRecordData", typeof(List<FacGenRecordData>));
            if (null != value1)
            {
                this._facGenRecordData = (List<FacGenRecordData>)value1;
            }
            object value2 = LoadDataFromConfig("HeroAdminCharacter", typeof(List<HeroAdminCharacter>));
            if (null != value2)
            {
                this._heroCharacters = (List<HeroAdminCharacter>)value2;
            }

        }

        public void SaveHeroAdminData()
        {
            SaveDataIntoConfig("HeroAdminCharacter", this._heroCharacters);
        }

        public void SaveFacGenData()
        {
            SaveDataIntoConfig("FacGenRecordData", this._facGenRecordData);
        }
    }
}
