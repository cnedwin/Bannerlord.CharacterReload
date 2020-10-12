using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace CharacterReload.VM.HeroAdmin
{
    class HeroAdminAttributeItemVM: ViewModel
    {
        private CharacterAttributesEnum _attributesEnum;

		private string _nameText;
		private int _attributeValue;
		private Action<CharacterAttributesEnum,int> _onAttributeChange;

		public HeroAdminAttributeItemVM(CharacterAttributesEnum attributesEnum, int value,  Action<CharacterAttributesEnum, int> onAttributeChange)
        {
            this._attributesEnum = attributesEnum;
			this._attributeValue = value;
			this._onAttributeChange = onAttributeChange;
			CharacterAttribute characterAttribute = CharacterAttributes.GetCharacterAttribute(attributesEnum);
			this._nameText = characterAttribute.Abbreviation.ToString();



		}

		[DataSourceProperty]
		public string DisplayName
		{
			get
			{
				return this._nameText;
			}
			set
			{
				if (value != this._nameText)
				{
					this._nameText = value;
					base.OnPropertyChangedWithValue(value, "DisplayName");
				}
			}
		}

		[DataSourceProperty]
		public int AttributeValue
		{
			get
			{
				return this._attributeValue;
			}
			set
			{
				if (value != this._attributeValue)
				{
					this._attributeValue = value;
					if (null != _onAttributeChange) this._onAttributeChange(this._attributesEnum, value);
					base.OnPropertyChangedWithValue(value, "AttributeValue");
					base.OnPropertyChanged("AttributeValueText");
				}
			}
		}


		[DataSourceProperty]
		public string AttributeValueText
		{
			get
			{
				return this._attributeValue.ToString();
			}
			
		}

		[DataSourceProperty]
		public string ResetLevelText
		{
			get
			{
				return new TextObject("{=bottom_ReLevelHero}Reset Level", null).ToString();
			}
		}

	}
}
