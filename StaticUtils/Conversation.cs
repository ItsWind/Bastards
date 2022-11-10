using BastardChildren.Models;
using BastardChildren.AddonHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace BastardChildren.StaticUtils {
    public static class Conversation {
        private static Dictionary<Hero, CampaignTime> heroesAskedForConception = new();
        private static Dictionary<Hero, CampaignTime> heroesCrueltyReceived = new();

        public static bool PlayerCanLegitimizeBastard() {
            Hero otherHero = Hero.OneToOneConversationHero;
            Bastard? bastard = Utils.GetBastardFromHero(otherHero);

            // If other hero is not a bastard
            if (bastard == null) { return false; }

            return true;
        }

        public static void LegitimizeBastard() {
            Hero bastardHero = Hero.OneToOneConversationHero;
            float infNeeded = (float)SubModule.Config.GetValueDouble("legitimizationInfluenceCost");
            if (!Utils.IsHeroKing(Hero.MainHero)) { infNeeded *= 2; }

            Hero.MainHero.Clan.Influence -= infNeeded;

            Utils.PrintToMessages(bastardHero.Name.ToString() + " has been legitimized!", 255, 255, 133);
            Utils.PrintToMessages("You spent " + infNeeded + " influence.");

            Utils.LegitimizeBastardFromHero(bastardHero);
        }

        public static bool HasInfluenceToLegitimize(out TextObject explain) {
            float infNeeded = (float)SubModule.Config.GetValueDouble("legitimizationInfluenceCost");
            if (!Utils.IsHeroKing(Hero.MainHero)) { infNeeded *= 2; }

            explain = new TextObject("You need to have " + infNeeded + " influence.");

            return Hero.MainHero.Clan.Influence >= infNeeded;
        }

        public static bool BastardConceptionAlreadyAskedCooldown() {
            Hero otherHero = Hero.OneToOneConversationHero;

            if (heroesAskedForConception.ContainsKey(otherHero)) {
                if (!heroesAskedForConception[otherHero].IsFuture)
                    heroesAskedForConception.Remove(otherHero);
                else
                    return true;
            }
            return false;
        }

        public static bool CrueltyReceivedCooldown() {
            Hero otherHero = Hero.OneToOneConversationHero;

            if (heroesCrueltyReceived.ContainsKey(otherHero)) {
                if (!heroesCrueltyReceived[otherHero].IsFuture)
                    heroesCrueltyReceived.Remove(otherHero);
                else
                    return true;
            }
            return false;
        }

        public static bool BastardConceptionAllowed() {
            Hero otherHero = Hero.OneToOneConversationHero;

            if (Hero.MainHero.IsChild || otherHero.IsChild) return false;

            Hero? femaleHero = Utils.GetFemaleHero(Hero.MainHero, otherHero);

            if (femaleHero == null || femaleHero.IsPregnant) return false;

            if (!SubModule.Config.GetValueBool("enableIncest") && Utils.HerosRelated(Hero.MainHero, otherHero)) return false;

            return true;
        }

        public static bool BastardConceptionAccepted() {
            bool returnVal = false;
            Hero otherHero = Hero.OneToOneConversationHero;
            Hero? femaleHero = Utils.GetFemaleHero(Hero.MainHero, otherHero);

            if (femaleHero != null && !femaleHero.IsPregnant && 
                otherHero.GetRelationWithPlayer() >= Utils.GetRelationNeededForConceptionAcceptance(otherHero)) {
                returnVal = true;
            }

            //if (checkFail) { returnVal = !returnVal; }
            return returnVal;
        }

        public static void ConceiveBastard(bool cruel=false) {
            Hero otherHero = Hero.OneToOneConversationHero;
            Hero? femaleHero = Utils.GetFemaleHero(Hero.MainHero, otherHero);

            if (femaleHero != null && !femaleHero.IsPregnant) {
                // Conception chance
                if (SubModule.Random.Next(1, 100) <= SubModule.Config.GetValueInt("percentChanceOfConception")) {
                    Hero father = femaleHero != Hero.MainHero ? Hero.MainHero : otherHero;

                    Bastard bastard = new Bastard(father, femaleHero);
                    SubModule.Bastards.Add(bastard);

                    femaleHero.IsPregnant = true;
                    Utils.PrintToMessages(femaleHero.Name + " has gotten pregnant!", 255, 153, 204);
                }
            }

            if (cruel == false) {
                heroesAskedForConception[otherHero] = CampaignTime.DaysFromNow((float)SubModule.Config.GetValueDouble("askedTimerInDays"));

                int charmMod = 200;
                if (otherHero.Clan != null && otherHero.Clan.IsNoble) {
                    charmMod += 500;
                }
                Hero.MainHero.AddSkillXp(DefaultSkills.Charm, charmMod);
            }
            else {
                heroesCrueltyReceived[otherHero] = CampaignTime.DaysFromNow(15.0f);

                Utils.ModifyPlayerRelations(otherHero, -100);
                if (otherHero.Spouse != null)
                    Utils.ModifyPlayerRelations(otherHero.Spouse, -75);
                if (otherHero.Father != null && otherHero.Father.IsAlive)
                    Utils.ModifyPlayerRelations(otherHero.Father, -75);
                if (otherHero.Mother != null && otherHero.Mother.IsAlive)
                    Utils.ModifyPlayerRelations(otherHero.Mother, -75);
                if (!otherHero.Siblings.IsEmpty())
                    foreach (Hero hero in otherHero.Siblings)
                        Utils.ModifyPlayerRelations(hero, -75);

                Utils.ModifyPlayerTraitLevel(DefaultTraits.Mercy, -1);
                Utils.ModifyPlayerTraitLevel(DefaultTraits.Honor, -1);
                Utils.ModifyPlayerTraitLevel(DefaultTraits.Generosity, -1);
                Utils.ModifyPlayerTraitLevel(DefaultTraits.Calculating, -1);
                Utils.ModifyPlayerTraitLevel(DefaultTraits.Valor, 1);
            }

            // Event fire
            BastardCampaignEvents.Fire_OnBastardConceptionAttempt(otherHero, cruel);
        }

        public static void BecomeBastardOfHero() {
            Hero h = Hero.OneToOneConversationHero;

            if (h.IsFemale) {
                Hero.MainHero.Mother.Children.Remove(Hero.MainHero);
                Hero.MainHero.Mother = h;
            } else {
                Hero.MainHero.Father.Children.Remove(Hero.MainHero);
                Hero.MainHero.Father = h;
            }

            if (SubModule.Config.GetValueBool("enableSurnames")) {
                string[] moddedNames = Utils.GetBastardName(Hero.MainHero);
                Hero.MainHero.SetName(new TextObject(moddedNames[1]), new TextObject(moddedNames[0]));
            }

            // Event fire
            BastardCampaignEvents.Fire_OnPlayerBecomeBastard(h);
        }
    }
}
