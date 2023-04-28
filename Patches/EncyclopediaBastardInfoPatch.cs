using BastardChildren.Models;
using BastardChildren.StaticUtils;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;

namespace BastardChildren.Patches {
    [HarmonyPatch(typeof(ConversationHelper), nameof(ConversationHelper.GetHeroRelationToHeroTextShort))]
    internal class EncyclopediaBastardInfoPatch {
        [HarmonyPostfix]
        private static void Postfix(ref string __result, Hero queriedHero, Hero baseHero) {
            if ((queriedHero.Father != null && queriedHero.Father == baseHero) || (queriedHero.Mother != null && queriedHero.Mother == baseHero)) {
                Bastard? bastard = Utils.GetBastardFromHero(queriedHero);
                if (bastard != null)
                    __result = "Bastard";
            }
        }
    }
}
