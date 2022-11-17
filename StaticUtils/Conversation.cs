using BastardChildren.Models;
using BastardChildren.AddonHelpers;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace BastardChildren.StaticUtils {
    public static class Conversation {
        private static Dictionary<Hero, CampaignTime> heroesAskedForConception = new();

        public static bool PlayerCanLegitimizeBastard() {
            Hero otherHero = Hero.OneToOneConversationHero;
            Bastard? bastard = Utils.GetBastardFromHero(otherHero);

            // If other hero is not a bastard
            if (bastard == null) return false;

            if (!(otherHero.Father == Hero.MainHero) && !(otherHero.Mother == Hero.MainHero)) return false;

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

        public static bool BastardConceptionAllowed() {
            Hero otherHero = Hero.OneToOneConversationHero;

            if (Hero.MainHero.IsChild || otherHero.IsChild) return false;

            Hero? femaleHero = Utils.GetFemaleHero(Hero.MainHero, otherHero);

            if (femaleHero == null || Utils.HeroIsPregnant(femaleHero)) return false;

            if (!SubModule.Config.GetValueBool("enableIncest") && Utils.HerosRelated(Hero.MainHero, otherHero)) return false;

            return true;
        }

        public static bool BastardConceptionAccepted() {
            bool returnVal = false;
            Hero otherHero = Hero.OneToOneConversationHero;
            Hero? femaleHero = Utils.GetFemaleHero(Hero.MainHero, otherHero);

            if (femaleHero != null && !Utils.HeroIsPregnant(femaleHero) && 
                otherHero.GetRelationWithPlayer() >= Utils.GetRelationNeededForConceptionAcceptance(Hero.MainHero, otherHero)) {
                returnVal = true;
            }

            //if (checkFail) { returnVal = !returnVal; }
            return returnVal;
        }

        public static void ConceiveBastard() {
            Hero otherHero = Hero.OneToOneConversationHero;
            Hero? femaleHero = Utils.GetFemaleHero(Hero.MainHero, otherHero);

            if (femaleHero != null && !Utils.HeroIsPregnant(femaleHero)) {
                // Conception chance
                if (Utils.PercentChanceCheck(SubModule.Config.GetValueInt("percentChanceOfConception"))) {
                    Hero father = femaleHero != Hero.MainHero ? Hero.MainHero : otherHero;

                    Bastard bastard = new Bastard(father, femaleHero);
                    SubModule.Bastards.Add(bastard);

                    femaleHero.IsPregnant = true;
                    Utils.PrintToMessages(femaleHero.Name + " has gotten pregnant!", 255, 153, 204);
                }
            }

            heroesAskedForConception[otherHero] = CampaignTime.DaysFromNow((float)SubModule.Config.GetValueDouble("askedTimerInDays"));

            int charmMod = 200;
            if (otherHero.Clan != null && otherHero.Clan.IsNoble) {
                charmMod += 500;
            }
            Hero.MainHero.AddSkillXp(DefaultSkills.Charm, charmMod);

            // Event fire
            BastardCampaignEvents.Fire_OnPlayerBastardConceptionAttempt(otherHero);
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
