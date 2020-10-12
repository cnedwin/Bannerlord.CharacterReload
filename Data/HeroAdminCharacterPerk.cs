using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharacterReload.Data
{
    class HeroAdminCharacterPerk
    {

        public string StringId { set; get; }

        public string BelongToSkillStringId { set; get; }

        public bool Enable { set; get; }

        public HeroAdminCharacterPerk(string perkStringId, string belongToSkillStringId,  bool enable)
        {
            StringId = perkStringId;
            BelongToSkillStringId = belongToSkillStringId;
            Enable = enable;
        }
    }
}
