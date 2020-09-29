using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharacterReload.Data
{
    class FacGenRecordData
    {

        public string Name { get; set; }

        public string DateString { get; set; }

        public bool IsFemale { get; set; }

        public string BodyPropertiesString { get; set; }

        public FacGenRecordData(string name, string bodyPropertiesString)
        {
            Name = name;
            BodyPropertiesString = bodyPropertiesString;
            DateString = DateTime.Now.ToString(); 
        }
    }
}
