using HarmonyLib;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;

namespace Bastards.Patches
{
    [HarmonyPatch(typeof(Clan), nameof(Clan.GetHeirApparents))]
    internal class GetHeirApparentsPatch
    {
        [HarmonyPostfix]
        public static Dictionary<Hero, int> Postfix(Dictionary<Hero, int> heirApparents)
        {
            Dictionary<Hero, int> newHeirs = new Dictionary<Hero, int>(heirApparents);

            foreach (KeyValuePair<Hero, int> entry in heirApparents)
            {
                Hero h = entry.Key;
                if (Utils.IsHeroBastard(h))
                {
                    newHeirs.Remove(h);
                }
            }

            return newHeirs;
        }
    }
}
