using BastardChildren.Models;
using BastardChildren.StaticUtils;
using HarmonyLib;
using TaleWorlds.CampaignSystem;

namespace BastardChildren.Patches {
    [HarmonyPatch(typeof(Clan), nameof(Clan.SetLeader))]
    internal class BastardClanHeirLegitimizationPatch {
        [HarmonyPostfix]
        private static void Postfix(Hero leader) {
            if (!SubModule.Config.GetValueBool("bastardsClanHeirLegitimization")) return;

            if (leader == null) return;

            Bastard? bastard = Utils.GetBastardFromHero(leader);
            if (bastard == null) return;

            bastard.Legitimize();
        }
    }
}
