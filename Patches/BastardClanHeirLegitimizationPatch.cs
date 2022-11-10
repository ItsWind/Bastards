using BastardChildren.Models;
using BastardChildren.StaticUtils;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
