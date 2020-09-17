using FaceDetailsCreator.Data;
using FaceDetailsCreator.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.GauntletUI;
using TaleWorlds.ObjectSystem;

namespace FaceDetailsCreator.VM.Facgen
{
    class FacGenRecordItemVM: ViewModel
    {
        private FacGenRecordData _item;

        private bool _isSelected;

        private ImageIdentifierVM _visual;

        private readonly Action<FacGenRecordItemVM> _onRecordSelected;


        public FacGenRecordItemVM(FacGenRecordData item, Action<FacGenRecordItemVM> onRecordSelected)
        {
            this._item = item;
          
            //characterCode.Code = characterCode.CreateNewCodeString();
            CharacterCode characterCode = CreateFrom(item);
            this.Visual = new ImageIdentifierVM(characterCode);
            this._onRecordSelected = onRecordSelected;
        }

        public  CharacterCode CreateFrom(FacGenRecordData item)
        {
            CharacterCode characterCode = CharacterCode.CreateEmpty();
            BodyProperties bodyProperties = BodyProperties.Default;
            BodyProperties.FromString(item.BodyPropertiesString, out bodyProperties);
            characterCode.BodyProperties = bodyProperties;
            string text = new Equipment().CalculateEquipmentCode();
            ReflectUtils.ReflectPropertyAndSetValue("EquipmentCode", text, characterCode);
            ReflectUtils.ReflectPropertyAndSetValue("IsHero", true, characterCode);
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

        public FacGenRecordData GetFacGenRecordData() {

            return _item;
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
				return this._item.Name;
			}
			
		}

		[DataSourceProperty]
		public string DateString
		{
			get
			{
				return this._item.DateString;
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

        public void OnHistoryRecordSelected()
        {
            if (!this.IsSelected)
            {
                this.IsSelected = true;
                this._onRecordSelected(this);
            }

        }


    }
}
