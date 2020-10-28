using CharacterReload.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.PlatformService;

namespace CharacterReload.Data
{
    class HeroAdminCharacter
    {

        public string SaveName { get; set; }

        public int Level { get; set; }

        public string Name { get; set; }

        public bool IsFemale { get; set; }

        public string BodyPropertiesString { get; set; }

        public List<HeroAdminCharacterAttribute> Attributes { set; get; }

        public List<HeroAdminCharacterSkill> Skills { set; get; }

        public List<HeroAdminCharacterPerk> Perks { set; get; }

        public List<HeroAdminCharacterTrait> Traits { set; get; }

        public HeroAdminCharacter()
        {
            init();
        }

        public void ReLevel()
        {
            this.Level = 1;
            init();

        }

        private void init()
        {
            Attributes = new List<HeroAdminCharacterAttribute>();
            Skills = new List<HeroAdminCharacterSkill>();
            Perks = new List<HeroAdminCharacterPerk>();
            Traits = new List<HeroAdminCharacterTrait>();
        }

        public static HeroAdminCharacter FromHero(Hero hero)
        {
            HeroAdminCharacter adminCharacter = new HeroAdminCharacter();
            adminCharacter.Level = hero.Level;
            adminCharacter.IsFemale = hero.IsFemale;
            adminCharacter.BodyPropertiesString = hero.BodyProperties.ToString();


            HeroAdminCharacterAttribute attr;
            for (int i = 0; i < 6; i++)
            {
                attr = new HeroAdminCharacterAttribute(Enum.GetName(typeof(CharacterAttributesEnum), i), hero.GetAttributeValue((CharacterAttributesEnum)i));
                adminCharacter.Attributes.Add(attr);
            }

            foreach (SkillObject current in SkillObject.All)
            {
                adminCharacter.Skills.Add(new HeroAdminCharacterSkill(current.StringId, hero.GetSkillValue(current), hero.HeroDeveloper.GetFocus(current)));
            }

            foreach (PerkObject current in PerkObject.All)
            {
                if (hero.GetPerkValue(current))
                {
                    adminCharacter.Perks.Add(new HeroAdminCharacterPerk(current.StringId, current.Skill.StringId, true));
                }
            }

            foreach (TraitObject trait in TraitObject.All)
            {
                int level = hero.GetTraitLevel(trait);
                adminCharacter.Traits.Add(new HeroAdminCharacterTrait(trait.StringId, level));

            }


            return adminCharacter;
        }

        public void ToHero(Hero hero)
        {
            /// hero.HeroDeveloper.ClearHero();
            BodyProperties bodyProperties = BodyProperties.Default;
            hero.Level = this.Level;
            BodyProperties.FromString(this.BodyPropertiesString, out bodyProperties);
            HeroUtils.UpdateHeroCharacterBodyProperties(hero.CharacterObject, bodyProperties, this.IsFemale);
            HeroAdminCharacterAttribute attr;
            hero.ClearAttributes();
            for (int i = 0; i < 6; i++)
            {
                attr = this.Attributes.FirstOrDefault((obj) => obj.AttributeName == Enum.GetName(typeof(CharacterAttributesEnum), i));
                if (null != attr)
                {
                    hero.SetAttributeValue((CharacterAttributesEnum)i, attr.AttributeValue);
                    // hero.HeroDeveloper.AddAttribute((CharacterAttributesEnum)i, attr.AttributeValue, false);
                }
            }
            HeroAdminCharacterSkill adminCharacterSkill;
            hero.ClearSkills();
            foreach (SkillObject current in SkillObject.All)
            {
                adminCharacterSkill = Skills.FirstOrDefault((obj) => obj.StringId.Equals(current.StringId));
                if (null != adminCharacterSkill)
                {
                    // hero.SetSkillValue(current, adminCharacterSkill.SkillValue);
                    int xpRequiredForSkillLevel = Campaign.Current.Models.CharacterDevelopmentModel.GetXpRequiredForSkillLevel(adminCharacterSkill.SkillValue);
                    hero.HeroDeveloper.SetPropertyValue(current, (float)xpRequiredForSkillLevel);
                    hero.SetSkillValue(current, adminCharacterSkill.SkillValue);
                    ReflectUtils.ReflectMethodAndInvoke("SetFocus", hero.HeroDeveloper, new object[] { current, adminCharacterSkill.SkillFocus });
                    ///hero.HeroDeveloper.AddFocus(current, adminCharacterSkill.SkillFocus, false);
                }
            }
            hero.ClearPerks();
            foreach (PerkObject perk in PerkObject.All)
            {
                HeroAdminCharacterPerk result = this.Perks.FirstOrDefault(cp => cp.StringId.Equals(perk.StringId));
                if (null != result)
                {
                    // hero.SetPerkValue(current, true); 
                    // 直接修改数据，因为上面方法会发送事件
                    CharacterPerks heroPerks = ReflectUtils.ReflectField<CharacterPerks>("_heroPerks", hero);
                    if (null != heroPerks)
                    {
                        heroPerks.SetPropertyValue(perk, result.Enable ? 1 : 0);
                    }
                }
            }

            hero.ClearTraits();
            foreach (TraitObject trait in TraitObject.All)
            {
                HeroAdminCharacterTrait characterTrait = Traits.FirstOrDefault((obj) => obj.StringId.Equals(trait.StringId));
                if (null != characterTrait)
                {
                    hero.SetTraitLevel(trait, characterTrait.Level);
                }

            }

        }

        public void SetPerkValue(PerkObject perk, bool enable)
        {
            HeroAdminCharacterPerk result = this.Perks.FirstOrDefault(cp => cp.StringId.Equals(perk.StringId));
            if (null != result)
            {
                result.Enable = enable;
            }
            else
            {
                this.Perks.Add(new HeroAdminCharacterPerk(perk.StringId, perk.Skill.StringId, enable));
            }
            /* CharacterPerks heroPerks = ReflectUtils.ReflectField<CharacterPerks>("_heroPerks", this._hero);
             if (null != heroPerks)
             {
                 heroPerks.SetPropertyValue(perk, selected ? 1 : 0);
             }*/
        }

        public bool GetPerkValue(PerkObject perk)
        {
            return this.Perks.Any(cp => cp.StringId.Equals(perk.StringId) && cp.Enable);
        }

        public void SetSkillValue(SkillObject skill, int level)
        {
            HeroAdminCharacterSkill result = Skills.FirstOrDefault((obj) => obj.StringId.Equals(skill.StringId));
            if (null != result)
            {
                result.SkillValue = level;
            }
        }

        public void ClearPerks()
        {
            this.Perks.Clear();
        }

        public void SetFocusValue(SkillObject skill, int newValue)
        {
            HeroAdminCharacterSkill result = Skills.FirstOrDefault((obj) => obj.StringId.Equals(skill.StringId));
            if (null != result)
            {
                result.SkillFocus = newValue;
            }
        }

        public void ClearFocuses()
        {
            foreach (HeroAdminCharacterSkill result in this.Skills)
            {
                result.SkillFocus = 0;
            }
        }

        public int GetSkillValue(SkillObject current)
        {
            HeroAdminCharacterSkill result = Skills.FirstOrDefault((obj) => obj.StringId.Equals(current.StringId));
            if (null != result)
            {
                return result.SkillValue;
            }
            else
            {
                return 0;
            }
        }

        public int GetFocusValue(SkillObject skill)
        {
            HeroAdminCharacterSkill result = Skills.FirstOrDefault((obj) => obj.StringId.Equals(skill.StringId));
            if (null != result)
            {
                return result.SkillFocus;
            }
            else
            {
                return 0;
            }
        }

        public int GetAttributeValue(CharacterAttributesEnum attributesEnum)
        {
            HeroAdminCharacterAttribute result = Attributes.FirstOrDefault((obj) => obj.AttributeName.Equals(Enum.GetName(typeof(CharacterAttributesEnum), attributesEnum)));
            if (null != result)
            {
                return result.AttributeValue;
            }
            else
            {
                return 0;
            }
        }

        public void SetAttributeValue(CharacterAttributesEnum attributesEnum, int newValue)
        {
            HeroAdminCharacterAttribute result = Attributes.FirstOrDefault((obj) => obj.AttributeName.Equals(Enum.GetName(typeof(CharacterAttributesEnum), attributesEnum)));
            if (null != result)
            {
                result.AttributeValue = newValue;
            }
        }

        public int GetTraitLevel(TraitObject traitObject)
        {
            HeroAdminCharacterTrait trait = Traits.FirstOrDefault(tr => tr.StringId.Equals(traitObject.StringId));
            if (null != trait)
            {
                return trait.Level;
            }

            return 0;
        }

        public void SetTraitLevel(TraitObject traitObject, int newLevel)
        {
            HeroAdminCharacterTrait result = Traits.FirstOrDefault(tr => tr.StringId.Equals(traitObject.StringId));
            if (null != result)
            {
                result.Level = newLevel;
            }
        }
    }
}