using CharacterReload.Data;
using CharacterReload.Utils;
using CharacterReload.VM.Facgen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.GauntletUI;

namespace CharacterReload.VM.HeroAdmin
{
    class HeroAdminRecordVM : ViewModel
    {
        List<HeroAdminCharacter> _data;
        MBBindingList<HeroAdminRecordItemVM> _genRecordItemVMs;

        private Action<HeroAdminCharacter> _onToLoadHeroCharacter;

        int MaxSaveCount = 50;

        private HeroAdminCharacter _editHero;


        private HeroAdminRecordItemVM _lastSelectedItem;

        public HeroAdminRecordVM(HeroAdminCharacter editHero, Action<HeroAdminCharacter> onToLoadHeroCharacter)
        {
            this._editHero = editHero;
            this._onToLoadHeroCharacter = onToLoadHeroCharacter;
            this._data = GlobalConfigData.Instance.HeroAdminData(); ;
            if (null == this._genRecordItemVMs)
            {
                this._genRecordItemVMs = new MBBindingList<HeroAdminRecordItemVM>();
            }
            else
            {
                this._genRecordItemVMs.Clear();
            }


            if(null != this._data && this._data.Count > 0)
            {
                this._data.ForEach(obj => {
                    this._genRecordItemVMs.Add(new HeroAdminRecordItemVM(obj, OnSelectedItem));
                });
            }

        }

        [DataSourceProperty]
        public bool HasSelectedItem
        {
            get
            {
                return null != this._lastSelectedItem;
            }
        }

        [DataSourceProperty]
        public string HeroTemplateText
        {
            get
            {
                return new TextObject("{=bottom_HeroTemplate}Hero Template", null).ToString();
            }

        }

        [DataSourceProperty]
        public string SaveCurrentText
        {
            get
            {
                return new TextObject("{=bottom_SaveCurrent}Save Current", null).ToString();
            }

        }

        [DataSourceProperty]
        public string LoadCurrentText
        {
            get
            {
                return new TextObject("{=bottom_LoadCurrent}Load Current", null).ToString();
            }

        }

        [DataSourceProperty]
        public string DeleteCurrentText
        {
            get
            {
                return new TextObject("{=bottom_DeleteCurrent}Delete Current", null).ToString();
            }

        }

        [DataSourceProperty]
        public MBBindingList<HeroAdminRecordItemVM> RecordItems
        {
            get
            {
                return this._genRecordItemVMs;
            }
            set
            {
                if (value != this._genRecordItemVMs)
                {
                    this._genRecordItemVMs = value;
                    base.OnPropertyChanged("RecordItems");
                }
            }
        }

        public void OnSelectedItem(HeroAdminRecordItemVM item)
        {
            if(null != _lastSelectedItem)
            {
               this._lastSelectedItem.IsSelected = false;
            }
            this._lastSelectedItem = item;
            base.OnPropertyChanged("HasSelectedItem");
        }


        public void ExecuteSaveSelected()
        {
            InformationManager.ShowTextInquiry(new TextInquiryData(new TextObject("{=tips_cr_InputSaveName}Enter the saved name").ToString(), new TextObject("{=tips_cr_MaxSaved}Can only save up to " + MaxSaveCount).ToString(),
             true, true, GameTexts.FindText("str_done", null).ToString(), GameTexts.FindText("str_cancel", null).ToString(), new Action<string>(OnEnterNameAfter), InformationManager.HideInquiry, false));
        }

        public void OnEnterNameAfter(string saveName)
        {

            HeroAdminCharacter data = this._editHero;
            data.SaveName = saveName;
            this._data.Insert(0, data);
            this._genRecordItemVMs.Insert(0, new HeroAdminRecordItemVM(data, OnSelectedItem));
            base.OnPropertyChanged("RecordItems");
            if(null != this._lastSelectedItem)
            {
                this._lastSelectedItem.IsSelected = false;
                this._lastSelectedItem = null;
            }

            GlobalConfigData.Instance.SaveHeroAdminData();
            base.OnPropertyChanged("HasSelectedItem");
        }

        public void ExecuteLoadSelected()
        {
            if (null != this._lastSelectedItem)
            {
                InformationUtils.ShowComfirInformation(new TextObject("{=tips_cr_ConfirmLoad}Confirm to load"), null, () => {
                    HeroAdminCharacter data = this._lastSelectedItem.GetItemData();
                   // data.ToHero(this._editHero);
                    this._onToLoadHeroCharacter(data);


                });
            }
        }

    

        public void ExecuteDeleteSelected()
        {
            if (null != this._lastSelectedItem)
            {
                InformationUtils.ShowComfirInformation(new TextObject("{=tips_cr_WarningToDelete}Warning: Confirm to Delete"), new TextObject("{=tips_cr_WarningToDelete2}This operation will delete the data and cannot be undone"), () => {
                    this._lastSelectedItem.IsSelected = false;
                   
                    this._genRecordItemVMs.Remove(this._lastSelectedItem);
                    this._data.Remove(this._lastSelectedItem.GetItemData());
                    GlobalConfigData.Instance.SaveHeroAdminData();
                    base.OnPropertyChanged("RecordItems");
                    base.OnPropertyChanged("HasSelectedItem");
                    this._lastSelectedItem = null;
                });

              
              
            }
        }

    }
}
