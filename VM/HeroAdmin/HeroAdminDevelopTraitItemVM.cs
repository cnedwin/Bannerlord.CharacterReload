using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace CharacterReload.VM.HeroAdmin
{
    class HeroAdminDevelopTraitItemVM : ViewModel
    {
        private TraitObject _traitObject;

		private string _nameText;
		private int _attributeValue;
		private Action<TraitObject, int> _onTraitChange;

		public HeroAdminDevelopTraitItemVM(TraitObject traitObject, int value,  Action<TraitObject, int> onTraitChange)
        {
            this._traitObject = traitObject;
			this._attributeValue = value;
			this._onTraitChange = onTraitChange;
			this._nameText = traitObject.Name.ToString();
		}

		[DataSourceProperty]
		public int MaxValue
		{
			get
			{
				return this._traitObject.MaxValue;
			}
			
		}


		[DataSourceProperty]
		public int MinValue
		{
			get
			{
				return this._traitObject.MinValue;
			}

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
		public int TraitValue
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
					if (null != _onTraitChange) this._onTraitChange(this._traitObject, value);
					base.OnPropertyChangedWithValue(value, "TraitValue");
					base.OnPropertyChanged("TraitValueText");
				}
			}
		}


		[DataSourceProperty]
		public string TraitValueText
		{
			get
			{
				return this._attributeValue.ToString();
			}
			
		}
		
	}
}
