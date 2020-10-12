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
    class HeroAdminDevelopCharacterPreviewVM: ViewModel
    {
		public enum StanceTypes
		{
			None,
			EmphasizeFace,
			SideView,
			CelebrateVictory,
			OnMount
		}
		private bool _isDead;
	
		private int _selectedIndex = 0;

		private string _mountCreationKey = "";

		private string _bodyProperties = "";

		private bool _isFemale;

		private int _stanceIndex;

		private uint _armorColor1;

		private uint _armorColor2;

		private string _equipmentCode;

		protected Equipment _equipment;

		private string _charStringId;

		protected string _bannerCode;

		[DataSourceProperty]
		public bool IsDead
		{
			get
			{
				return this._isDead;
			}
			set
			{
				if (value != this._isDead)
				{
					this._isDead = value;
					base.OnPropertyChanged("IsDead");
				}
			}
		}

		[DataSourceProperty]
		public bool IsBattledSelected
		{
			get
			{
				return this._selectedIndex == 1;
			}
		}

		[DataSourceProperty]
		public bool IsCivilizedSelected
		{
			get
			{
				return this._selectedIndex == 0;
			}
		}

		[DataSourceProperty]
		public bool IsUnderwearSelected
		{
			get
			{
				return this._selectedIndex == 2;
			}
		}

		[DataSourceProperty]
		public string BannerCodeText
		{
			get
			{
				return this._bannerCode;
			}
			set
			{
				if (value != this._bannerCode)
				{
					this._bannerCode = value;
					base.OnPropertyChangedWithValue(value, "BannerCodeText");
				}
			}
		}

		[DataSourceProperty]
		public string BodyProperties
		{
			get
			{
				return this._bodyProperties;
			}
			set
			{
				if (value != this._bodyProperties)
				{
					this._bodyProperties = value;
					base.OnPropertyChangedWithValue(value, "BodyProperties");
				}
			}
		}

		[DataSourceProperty]
		public string MountCreationKey
		{
			get
			{
				return this._mountCreationKey;
			}
			set
			{
				if (value != this._mountCreationKey)
				{
					this._mountCreationKey = value;
					base.OnPropertyChangedWithValue(value, "MountCreationKey");
				}
			}
		}

		[DataSourceProperty]
		public string CharStringId
		{
			get
			{
				return this._charStringId;
			}
			set
			{
				if (value != this._charStringId)
				{
					this._charStringId = value;
					base.OnPropertyChangedWithValue(value, "CharStringId");
				}
			}
		}

		[DataSourceProperty]
		public int StanceIndex
		{
			get
			{
				return this._stanceIndex;
			}
			private set
			{
				if (value != this._stanceIndex)
				{
					this._stanceIndex = value;
					base.OnPropertyChangedWithValue(value, "StanceIndex");
				}
			}
		}

		[DataSourceProperty]
		public bool IsFemale
		{
			get
			{
				return this._isFemale;
			}
			set
			{
				if (value != this._isFemale)
				{
					this._isFemale = value;
					base.OnPropertyChangedWithValue(value, "IsFemale");
				}
			}
		}

		[DataSourceProperty]
		public string EquipmentCode
		{
			get
			{
				return this._equipmentCode;
			}
			set
			{
				if (value != this._equipmentCode)
				{
					this._equipmentCode = value;
					base.OnPropertyChangedWithValue(value, "EquipmentCode");
				}
			}
		}

		[DataSourceProperty]
		public uint ArmorColor1
		{
			get
			{
				return this._armorColor1;
			}
			set
			{
				if (value != this._armorColor1)
				{
					this._armorColor1 = value;
					base.OnPropertyChangedWithValue(value, "ArmorColor1");
				}
			}
		}

		[DataSourceProperty]
		public uint ArmorColor2
		{
			get
			{
				return this._armorColor2;
			}
			set
			{
				if (value != this._armorColor2)
				{
					this._armorColor2 = value;
					base.OnPropertyChangedWithValue(value, "ArmorColor2");
				}
			}
		}

		

		public HeroAdminDevelopCharacterPreviewVM()
		{
			this._equipment = new Equipment(false);
			this.EquipmentCode = this._equipment.CalculateEquipmentCode();
			this.StanceIndex = (int)StanceTypes.None;
		}

		public void SetEquipment(EquipmentIndex index, EquipmentElement item)
		{
			this._equipment[(int)index] = item;
			this.EquipmentCode = this._equipment.CalculateEquipmentCode();
		}

	

		public void FillFrom(BodyProperties bodyProperties, Equipment equipment, CultureObject culture, bool isFemale = false)
		{
			if (FaceGen.GetMaturityTypeWithAge(bodyProperties.Age) > BodyMeshMaturityType.Child)
			{
				if (culture != null)
				{
					this.ArmorColor1 = culture.Color;
					this.ArmorColor2 = culture.Color2;
				}
				//this.CharStringId = character.StringId;
				this.IsFemale = isFemale;
				FillEquipment(bodyProperties, equipment);
			}
		}



		private void FillEquipment(BodyProperties bodyProperties, Equipment equipment)
		{
			this.BodyProperties = bodyProperties.ToString();
			//this.MountCreationKey = TaleWorlds.Core.MountCreationKey.GetRandomMountKey((equipment != null) ? equipment[10].Item : null, Common.GetDJB2(this._character.StringId));
			this._equipment = ((equipment != null) ? equipment.Clone(false) : null);
			Equipment expr_C6 = equipment;
			this.EquipmentCode = ((expr_C6 != null) ? expr_C6.CalculateEquipmentCode() : null);
		}
	}
}
