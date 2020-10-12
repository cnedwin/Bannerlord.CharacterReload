using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharacterReload.Data
{
    class HeroAdminCharacterTrait
    {

        public string StringId { set; get; }


        public int Level { set; get; }

        public HeroAdminCharacterTrait(string stringId, int level)
        {
            StringId = stringId;
            Level = level;
        }
    }
}
