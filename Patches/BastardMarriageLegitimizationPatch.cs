using BastardChildren.Models;
using BastardChildren.StaticUtils;
using HarmonyLib;
using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;

namespace BastardChildren.Patches {
    [HarmonyPatch(typeof(MarriageAction), nameof(MarriageAction.Apply))]
    internal class BastardMarriageLegitimizationPatch {
        [HarmonyPrefix]
        private static void Postfix(Hero firstHero, Hero secondHero) {
            if (!GlobalSettings<MCMConfig>.Instance.LegitimizeMarriedBastardsEnabled) return;

            Bastard? firstBastard = Utils.GetBastardFromHero(firstHero);
            if (firstBastard != null)
                firstBastard.Legitimize();

            Bastard? secondBastard = Utils.GetBastardFromHero(secondHero);
            if (secondBastard != null)
                secondBastard.Legitimize();
        }
    }
}
