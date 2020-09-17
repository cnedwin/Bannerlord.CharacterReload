using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace FaceDetailsCreator.Utils
{
    class InformationUtils
    {

    

        public static  void ShowComfirInformation(TextObject title, TextObject message, Action affirmativeAction = null, Action negativeAction = null)
        {
            if(null == negativeAction)
            {
                negativeAction = () => { };
            }
            InformationManager.ShowInquiry(new InquiryData(title.ToString(), message==null? string.Empty: message.ToString(), true, true, GameTexts.FindText("str_done", null).ToString(), GameTexts.FindText("str_cancel", null).ToString(), affirmativeAction, negativeAction), false);

        }
    }
}
