using BastardChildren.Models;
using BastardChildren.StaticUtils;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;

namespace BastardChildren.Patches {
    [HarmonyPatch(typeof(MarriageAction), nameof(MarriageAction.Apply))]
    internal class BastardMarriageLegitimizationPatch {
        [HarmonyPrefix]
        private static void Postfix(Hero firstHero, Hero secondHero) {
            if (!SubModule.Config.GetValueBool("bastardsMarriageLegitimization")) return;

            Bastard? firstBastard = Utils.GetBastardFromHero(firstHero);
            if (firstBastard != null)
                firstBastard.Legitimize();

            Bastard? secondBastard = Utils.GetBastardFromHero(secondHero);
            if (secondBastard != null)
                secondBastard.Legitimize();
        }
    }
}
