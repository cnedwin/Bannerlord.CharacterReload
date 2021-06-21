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


            //一个成长属性包含3个技能
            LoadCharacterAttributeAndSkills(adminCharacter, hero, DefaultCharacterAttributes.Vigor);
            LoadCharacterAttributeAndSkills(adminCharacter, hero, DefaultCharacterAttributes.Control);
            LoadCharacterAttributeAndSkills(adminCharacter, hero, DefaultCharacterAttributes.Endurance);

            LoadCharacterAttributeAndSkills(adminCharacter, hero, DefaultCharacterAttributes.Cunning);
            LoadCharacterAttributeAndSkills(adminCharacter, hero, DefaultCharacterAttributes.Social);
            LoadCharacterAttributeAndSkills(adminCharacter, hero, DefaultCharacterAttributes.Intelligence);


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


        private static void LoadCharacterAttributeAndSkills(HeroAdminCharacter adminCharacter, Hero hero,  CharacterAttribute characterAttribute)
        {
            HeroAdminCharacterAttribute attr;
            attr = new HeroAdminCharacterAttribute(characterAttribute.StringId, hero.GetAttributeValue(characterAttribute));
            adminCharacter.Attributes.Add(attr);
            foreach (SkillObject skill in characterAttribute.Skills)
            {
                adminCharacter.Skills.Add(new HeroAdminCharacterSkill(skill.StringId, hero.GetSkillValue(skill), hero.HeroDeveloper.GetFocus(skill)));
            }
        }

        public  void ToHero(Hero hero)
        {
            hero.HeroDeveloper.ClearHero();
            BodyProperties bodyProperties = BodyProperties.Default;
            hero.Level = this.Level;
            BodyProperties.FromString(this.BodyPropertiesString, out bodyProperties);
            HeroUtils.UpdateHeroCharacterBodyProperties(hero.CharacterObject, bodyProperties, this.IsFemale);
            HeroAdminCharacterAttribute attr;
            hero.ClearAttributes();
            hero.ClearSkills();
            SetCharacterAttributeAndSkills(hero, DefaultCharacterAttributes.Vigor);
            SetCharacterAttributeAndSkills(hero, DefaultCharacterAttributes.Control);
            SetCharacterAttributeAndSkills(hero, DefaultCharacterAttributes.Endurance);

            SetCharacterAttributeAndSkills(hero, DefaultCharacterAttributes.Cunning);
            SetCharacterAttributeAndSkills(hero, DefaultCharacterAttributes.Social);
            SetCharacterAttributeAndSkills(hero, DefaultCharacterAttributes.Intelligence);


            hero.ClearPerks();
            foreach (PerkObject perk in PerkObject.All)
            {
                HeroAdminCharacterPerk  result  = this.Perks.FirstOrDefault(cp => cp.StringId.Equals(perk.StringId));
                if (null != result)
                {
                    // hero.SetPerkValue(current, true); 
                    // 直接修改数据，因为上面方法会发送事件
                    CharacterPerks heroPerks = ReflectUtils.ReflectField<CharacterPerks>("_heroPerks", hero);
                    if (null != heroPerks)
                    {
                        heroPerks.SetPropertyValue(perk, result.Enable ? 1: 0);
                    }
                }
            }

            hero.ClearTraits();
            foreach (TraitObject trait in TraitObject.All)
            {
                HeroAdminCharacterTrait characterTrait = Traits.FirstOrDefault((obj) => obj.StringId.Equals(trait.StringId));
                if (null != characterTrait)
                {
                    hero.SetTraitLevelInternal(trait, characterTrait.Level) ;
                }

            }

        }

        private  void SetCharacterAttributeAndSkills( Hero hero, CharacterAttribute characterAttribute)
        {
            HeroAdminCharacterAttribute attr;
            HeroAdminCharacterSkill adminCharacterSkill;
            attr = this.Attributes.FirstOrDefault((obj) => obj.AttributeName == characterAttribute.StringId);
            ReflectUtils.ReflectMethodAndInvoke("SetAttributeValueInternal", hero, new object[] { characterAttribute , attr .AttributeValue});
            //hero.SetArr(characterAttribute, attr.AttributeValue);
            foreach (SkillObject skill in characterAttribute.Skills)
            {
                adminCharacterSkill = Skills.FirstOrDefault((obj) => obj.StringId.Equals(skill.StringId));
                if (null != adminCharacterSkill)
                {
                    // hero.SetSkillValue(current, adminCharacterSkill.SkillValue);
                    int xpRequiredForSkillLevel = Campaign.Current.Models.CharacterDevelopmentModel.GetXpRequiredForSkillLevel(adminCharacterSkill.SkillValue);
                    hero.HeroDeveloper.SetPropertyValue(skill, (float)xpRequiredForSkillLevel);
                    ReflectUtils.ReflectMethodAndInvoke("SetSkillValueInternal", hero, new object[] { skill, adminCharacterSkill.SkillValue });
                    // hero.SetSkill(skill, adminCharacterSkill.SkillValue);
                    ReflectUtils.ReflectMethodAndInvoke("SetFocus", hero.HeroDeveloper, new object[] { skill, adminCharacterSkill.SkillFocus });
                    ///hero.HeroDeveloper.AddFocus(current, adminCharacterSkill.SkillFocus, false);
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

        public int GetAttributeValue(CharacterAttribute attributesEnum)
        {
            HeroAdminCharacterAttribute result = Attributes.FirstOrDefault((obj) => obj.AttributeName.Equals(attributesEnum.StringId));
            if (null != result)
            {
                return result.AttributeValue;
            }
            else
            {
                return 0;
            }
        }

        public void SetAttributeValue(CharacterAttribute attributesEnum, int newValue)
        {
            HeroAdminCharacterAttribute result = Attributes.FirstOrDefault((obj) => obj.AttributeName.Equals(attributesEnum.StringId));
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

        public void SetTraitLevel(TraitObject traitObject, int newLevel) {
            HeroAdminCharacterTrait result = Traits.FirstOrDefault(tr => tr.StringId.Equals(traitObject.StringId));
            if (null != result)
            {
                result.Level = newLevel;
            }
        }
    }
}
