using BastardChildren.Models;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;

namespace BastardChildren.Patches {
    [HarmonyPatch(typeof(PregnancyCampaignBehavior), "DailyTickHero")]
    internal class PregnancyDailyTickHeroPatch {
        [HarmonyPrefix]
        private static bool Prefix(Hero hero) {
            if (!hero.IsPregnant)
                return true;

            Bastard bastard;

            try {
                bastard = SubModule.Bastards.Where(x => x.hero == null && x.mother == hero).First();
            }
            catch (Exception) {
                return true;
            }

            bastard.Tick();
            return false;
        }
    }
}
