using CharacterReload.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.EncyclopediaItems;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;

namespace CharacterReload.VM.HeroAdmin
{
    class HeroAdminCharacterVM : ViewModel
    {

        HeroAdminDevelopCharacterPreviewVM _heroModel;
        private string _heroName;
        private int _level = 0;

        public HeroAdminCharacterVM(string heroName, int level)
        {
            this._level = level;
            this._heroName = heroName;
            this.HeroModel = new HeroAdminDevelopCharacterPreviewVM();

        }

        public void RefreshHeroLevel(int level)
        {
            this._level = level;
            base.OnPropertyChanged("DisplayerHeroName");
        }

        public void FillFrom(BodyProperties bodyProperties, Equipment equipment, CultureObject culture, bool isFemale = false)
        {
            this.HeroModel.FillFrom(bodyProperties, equipment, culture, isFemale);
        }

        [DataSourceProperty]
        public string DisplayerHeroName
        {
            get
            {
                return this._heroName + "\n" + "(Level=" + this._level + ")"; ;
            }
            set
            {
                if (value != this._heroName)
                {
                    this._heroName = value;
                    base.OnPropertyChangedWithValue(value, "DisplayerHeroName");
                }
            }
        }


        [DataSourceProperty]
        public HeroAdminDevelopCharacterPreviewVM HeroModel
        {
            get
            {
                return this._heroModel;
            }
            set
            {
                if (value != this._heroModel)
                {
                    this._heroModel = value;
                    base.OnPropertyChangedWithValue(value, "HeroModel");
                }
            }
        }


    }
}
