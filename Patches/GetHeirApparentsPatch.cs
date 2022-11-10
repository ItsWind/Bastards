/*
using BastardChildren.StaticUtils;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;

namespace BastardChildren.Patches {
    [HarmonyPatch(typeof(Clan), nameof(Clan.GetHeirApparents))]
    internal class GetHeirApparentsPatch {
        [HarmonyPostfix]
        private static void Postfix(ref Dictionary<Hero, int> __result) {
            if (SubModule.Config.GetValueBool("bastardsCanBecomeHeirs")) return;

            Dictionary<Hero, int> newHeirs = new Dictionary<Hero, int>(__result);
            foreach (KeyValuePair<Hero, int> entry in __result) {
                Hero h = entry.Key;
                if (Utils.GetBastardFromHero(h) != null)
                    newHeirs.Remove(h);
            }

            __result = newHeirs;
        }
    }
}
*/