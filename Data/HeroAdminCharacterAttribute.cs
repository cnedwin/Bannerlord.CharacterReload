using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharacterReload.Data
{
    class HeroAdminCharacterAttribute 
    {

        public String AttributeName { set; get; }
       
        public int AttributeValue {  set; get; }

        public HeroAdminCharacterAttribute(string attributeName, int attributeValue)
        {
            AttributeName = attributeName;
            AttributeValue = attributeValue;
        }
    }
}
