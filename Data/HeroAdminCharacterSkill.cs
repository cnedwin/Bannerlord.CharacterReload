using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharacterReload.Data
{
    class HeroAdminCharacterSkill
    {
        public string StringId { set; get; }
        public int SkillValue { set; get; }
        public int SkillFocus { set; get; }

        public HeroAdminCharacterSkill(string stringId, int skillValue, int skillFocus)
        {
            StringId = stringId;
            SkillValue = skillValue;
            SkillFocus = skillFocus;
        }
    }
}
