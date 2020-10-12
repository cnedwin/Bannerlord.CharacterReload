using CharacterReload.Data;
using CharacterReload.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.EncyclopediaItems;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace CharacterReload.VM.HeroAdmin
{
    class HeroAdminRecordItemVM: ViewModel
    {

        private HeroAdminCharacter _heroAdminCharacter;
        private readonly Action<HeroAdminRecordItemVM> _onRecordSelected;
        private bool _isSelected;
        MBBindingList<EncyclopediaSkillVM> _skills;

        private ImageIdentifierVM _visual;

        public HeroAdminRecordItemVM(HeroAdminCharacter heroAdminCharacter, Action<HeroAdminRecordItemVM> selectAction)
        {
            _heroAdminCharacter = heroAdminCharacter;
            this._onRecordSelected = selectAction;
            CharacterCode characterCode = CreateFrom(heroAdminCharacter);
            this.Visual = new ImageIdentifierVM(characterCode);
            this._skills = new MBBindingList<EncyclopediaSkillVM>();
            RefreshHeroSkill();
        }

        public HeroAdminCharacter GetItemData()
        {

            return this._heroAdminCharacter;
        }

        [DataSourceProperty]
        public ImageIdentifierVM Visual
        {
            get
            {
                return this._visual;
            }
            set
            {
                if (value != this._visual)
                {
                    this._visual = value;
                    base.OnPropertyChanged("Visual");
                }
            }
        }


        [DataSourceProperty]
        public string DisplayName
        {
            get
            {
                return this._heroAdminCharacter.SaveName;
            }

        }

     
        [DataSourceProperty]
        public bool IsSelected
        {
            get
            {
                return this._isSelected;
            }
            set
            {
                if (value != this._isSelected)
                {
                    this._isSelected = value;
                    base.OnPropertyChanged("IsSelected");
                }
            }
        }

        [DataSourceProperty]
        public MBBindingList<EncyclopediaSkillVM> Skills
        {
            get
            {
                return this._skills;
            }
            set
            {
                if (value != this._skills)
                {
                    this._skills = value;
                    base.OnPropertyChangedWithValue(value, "Skills");
                }
            }
        }

        public void RefreshHeroSkill()
        {
            this.Skills.Clear();
            foreach (SkillObject current in SkillObject.All)
            {
                this.Skills.Add(new EncyclopediaSkillVM(current, this._heroAdminCharacter.GetSkillValue(current)));
            }
        }

        public void OnRecordSelected()
        {
            if (!this.IsSelected)
            {
                this.IsSelected = true;
                this._onRecordSelected(this);
            }

        }


        public CharacterCode CreateFrom(HeroAdminCharacter item)
        {
            CharacterCode characterCode = CharacterCode.CreateEmpty();
            BodyProperties bodyProperties = BodyProperties.Default;
            BodyProperties.FromString(item.BodyPropertiesString, out bodyProperties);
            characterCode.BodyProperties = bodyProperties;
            string text = new Equipment().CalculateEquipmentCode();
            ReflectUtils.ReflectPropertyAndSetValue("EquipmentCode", text, characterCode);
            ReflectUtils.ReflectPropertyAndSetValue("IsHero", true, characterCode); //这个属性影响，如是英雄只会显示头， 不是则显示半身
            MBStringBuilder mBStringBuilder = default(MBStringBuilder);
            mBStringBuilder.Initialize(16, "CreateFrom");
            mBStringBuilder.Append<string>("@---@");
            mBStringBuilder.Append<string>(text);
            mBStringBuilder.Append<string>("@---@");
            mBStringBuilder.Append<string>(characterCode.BodyProperties.ToString());
            mBStringBuilder.Append<string>("@---@");
            mBStringBuilder.Append<string>(item.IsFemale ? "1" : "0");
            mBStringBuilder.Append<string>("@---@");
            mBStringBuilder.Append<string>(characterCode.IsHero ? "1" : "0");
            mBStringBuilder.Append<string>("@---@");
            mBStringBuilder.Append<string>(((int)characterCode.FormationClass).ToString());
            mBStringBuilder.Append<string>("@---@");
            ReflectUtils.ReflectPropertyAndSetValue("Code", mBStringBuilder.ToStringAndRelease(), characterCode);
            return characterCode;
        }
    }
}
