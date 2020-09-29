using CharacterReload.VM;
using CharacterReload.Data;
using CharacterReload.VM.Facgen;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.GauntletUI;
using TaleWorlds.TwoDimension;

namespace CharacterReload.Pathes
{
    [HarmonyPatch(typeof(BodyGeneratorView), MethodType.Constructor, new Type[] {
     typeof(ControlCharacterCreationStage),
     typeof(TextObject),
     typeof(ControlCharacterCreationStage),
     typeof(TextObject),
     typeof( BasicCharacterObject ),
     typeof( bool ),
     typeof( IFaceGeneratorCustomFilter ),
     typeof( ControlCharacterCreationStageReturnInt ),
     typeof( ControlCharacterCreationStageReturnInt  ),
     typeof(  ControlCharacterCreationStageReturnInt  ),
     typeof(  ControlCharacterCreationStageWithInt  )
    })]
    class BodyGeneratorViewPath
    {
        public static SpriteCategory clanCategory;

        public static void Postfix(ref BodyGeneratorView __instance) {

            SpriteData spriteData = UIResourceManager.SpriteData;
            TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
            ResourceDepot uIResourceDepot = UIResourceManager.UIResourceDepot;
            clanCategory = spriteData.SpriteCategories["ui_clan"];
            clanCategory.Load(resourceContext, uIResourceDepot);

            FacGenRecordVM facGenRecord = new FacGenRecordVM(__instance, GlobalDataProvider.Instance.FacGenRecordData());
            GauntletMovie movie  = __instance.GauntletLayer.LoadMovie("FacGenRecord", facGenRecord);
            //movie.BrushFactory.LoadBrushFile("FacGenRecord");
            IEnumerable<Brush> brushes =  movie.BrushFactory.Brushes.Where(obj => obj.Name.Contains("Clan"));
        }
    }

    [HarmonyPatch(typeof(BodyGeneratorView), "OnFinalize")]
    class BodyGeneratorViewOnFinalizePath {

        public static void Postfix()
        {
            if (null != BodyGeneratorViewPath.clanCategory)
            {
                GlobalDataProvider.Instance.SaveFacGenData();
                BodyGeneratorViewPath.clanCategory = null;
            }
        }
    }

}
