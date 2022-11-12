using BastardChildren.Models;
using BastardChildren.StaticUtils;
using HarmonyLib;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.BarterSystem.Barterables;

namespace BastardChildren.Patches {
    [HarmonyPatch(typeof(MarriageBarterable), nameof(MarriageBarterable.GetUnitValueForFaction))]
    internal class BastardMarriageBarterGetUnitValuePatch {
        [HarmonyPostfix]
        private static void Postfix(ref int __result, MarriageBarterable __instance) {
            Hero? heroBeingProposedTo = Traverse.Create(__instance).Field("HeroBeingProposedTo").GetValue() as Hero;
            Hero? proposingHero = Traverse.Create(__instance).Field("ProposingHero").GetValue() as Hero;

            if (heroBeingProposedTo == null || proposingHero == null)
                return;

            int modifierValue = (int)Math.Round((double)Math.Abs(__result) * SubModule.Config.GetValueDouble("bastardsMarriageValueMult"));

            Bastard? bastardBeingProposedTo = Utils.GetBastardFromHero(heroBeingProposedTo);
            if (bastardBeingProposedTo != null)
                __result -= modifierValue;

            Bastard? proposingBastard = Utils.GetBastardFromHero(proposingHero);
            if (proposingBastard != null)
                __result += modifierValue;
        }
    }
}
