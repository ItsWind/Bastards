using BastardChildren.Models;
using BastardChildren.StaticUtils;
using HarmonyLib;
using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem;

namespace BastardChildren.Patches {
    [HarmonyPatch(typeof(Clan), nameof(Clan.SetLeader))]
    internal class SetClanLeaderPatch {
        [HarmonyPostfix]
        private static void Postfix(Hero leader) {
            if (!GlobalSettings<MCMConfig>.Instance.LegitimizeBastardHeirsEnabled) return;

            if (leader == null) return;

            Bastard? bastard = Utils.GetBastardFromHero(leader);
            if (bastard == null) return;

            bastard.Legitimize();
        }
    }
}
