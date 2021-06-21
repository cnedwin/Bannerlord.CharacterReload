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
        private CharacterAttribute _attributesEnum;

		private string _nameText;
		private int _attributeValue;
		private Action<CharacterAttribute, int> _onAttributeChange;

		public HeroAdminAttributeItemVM(CharacterAttribute attributesEnum, int value,  Action<CharacterAttribute, int> onAttributeChange)
        {
            this._attributesEnum = attributesEnum;
			this._attributeValue = value;
			this._onAttributeChange = onAttributeChange;
			this._nameText = attributesEnum.Name.ToString();

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

	}
}
