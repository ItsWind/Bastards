using BastardChildren.Models;
using BastardChildren.StaticUtils;
using HarmonyLib;
using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;

namespace BastardChildren.Patches {
    [HarmonyPatch(typeof(MarriageAction), nameof(MarriageAction.Apply))]
    internal class MarriagePatch {
        [HarmonyPrefix]
        private static void Prefix(Hero firstHero, Hero secondHero) {
            // FEMALE TAKES BASTARDS
            Hero? femaleHero = Utils.GetFemaleHero(firstHero, secondHero);
            if (femaleHero != null) {
                foreach (Hero child in femaleHero.Children) {
                    Bastard? bastard = Utils.GetBastardFromHero(child);
                    if (bastard != null && child.Clan == femaleHero.Clan) {
                        Clan newClan = Campaign.Current.Models.MarriageModel.GetClanAfterMarriage(firstHero, secondHero);
                        child.Clan = newClan;
                    }
                }
            }

            // LEGITIZE BY MARRIAGE
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
