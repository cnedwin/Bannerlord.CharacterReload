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
    class HeroAdminCharacterVM: ViewModel
    {
 
        HeroAdminDevelopCharacterPreviewVM _heroModel;
        private string _heroName;

        public HeroAdminCharacterVM( string heroName)
        {
            this._heroName = heroName;
            this.HeroModel = new HeroAdminDevelopCharacterPreviewVM();
 
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
                return this._heroName;
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
