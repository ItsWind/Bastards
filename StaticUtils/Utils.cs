using BastardChildren.Models;
using MCM.Abstractions.Base.Global;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Library;

namespace BastardChildren.StaticUtils {
    public static class Utils {
        public static void PrintToMessages(string str, float r=255, float g=255, float b=255)
        {
            float[] newValues = { r / 255.0f, g / 255.0f, b / 255.0f };
            Color col = new(newValues[0], newValues[1], newValues[2]);
            InformationManager.DisplayMessage(new InformationMessage(str, col));
        }
        public static void PrintToDisplayBox(string displayTitle, string displayText) {
            InformationManager.ShowInquiry(new InquiryData(displayTitle, displayText, true, false, "Ok", null, null, null), true);
        }

        public static bool HeroIsPregnant(Hero hero) {
            if (hero.IsPregnant) return true;

            foreach (Bastard bastard in SubModule.Bastards)
                if (bastard.hero == null && bastard.mother == hero) return true;

            return false;
        }

        public static void ModifyHeroRelations(Hero hero1, Hero hero2, int mod) {
            if (hero1 == null || hero2 == null) return;

            if (hero1 == hero2) return;

            int currRelation = CharacterRelationManager.GetHeroRelation(hero1, hero2);
            CharacterRelationManager.SetHeroRelation(hero1, hero2, currRelation + mod);

            if (hero1 == Hero.MainHero || hero2 == Hero.MainHero) {
                Hero otherHero = hero1 == Hero.MainHero ? hero2 : hero1;
                int r = 0;
                int g = 0;
                int b = 0;
                if (mod > 0)
                    g = 204;
                else
                    r = 204;
                PrintToMessages("Your relation with " + otherHero.Name + " has changed by " + mod + ".", r, g, b);
            }
        }

        public static void ModifyPlayerTraitLevel(TraitObject trait, int mod) {
            int currentLevel = Hero.MainHero.GetTraitLevel(trait);
            Hero.MainHero.SetTraitLevel(trait, currentLevel + mod);
        }

        public static Bastard? GetBastardFromHero(Hero hero) {
            foreach (Bastard bastard in SubModule.Bastards)
                if (hero != null && hero == bastard.hero)
                    return bastard;
            return null;
        }

        public static void LegitimizeBastardFromHero(Hero h) {
            Bastard? bastard = GetBastardFromHero(h);
            if (bastard != null)
                bastard.Legitimize();
        }

        public static bool IsHeroKing(Hero h) {
            return h.Clan != null && h.Clan.Kingdom != null && h.Clan.Kingdom.Leader.Equals(h);
        }

        public static Hero? GetFemaleHero(Hero h1, Hero h2) {
            if (h1.IsFemale) {
                if (!h2.IsFemale) return h1;
            } else {
                if (h2.IsFemale) return h2;
            }
            return null;
        }

        public static bool GetIfHeroWouldConceiveBastard(Hero hero) {
            CharacterObject charObj = hero.CharacterObject;
            int honorLevel = charObj.GetTraitLevel(DefaultTraits.Honor);
            int calculatingLevel = charObj.GetTraitLevel(DefaultTraits.Calculating);
            int valorLevel = charObj.GetTraitLevel(DefaultTraits.Valor);

            // If hero is honorable and is married
            if (honorLevel > 0 && hero.Spouse != null) return false;

            int chance = GlobalSettings<MCMConfig>.Instance.AIBaseConceptionChance;

            if (hero.Spouse != null) chance -= hero.GetRelation(hero.Spouse);

            if (GlobalSettings<MCMConfig>.Instance.TraitAffectedRelationEnabled) {
                chance -= honorLevel * 15;
                chance -= calculatingLevel * 10;
                chance += valorLevel * 12;
            }

            return PercentChanceCheck(chance);
        }

        public static int GetRelationNeededForConceptionAcceptance(Hero hero, Hero otherHero) {
            int relationNeeded = GlobalSettings<MCMConfig>.Instance.BaseRelationNeeded;

            if (GlobalSettings<MCMConfig>.Instance.TraitAffectedRelationEnabled) {
                CharacterObject otherHeroBaseObj = otherHero.CharacterObject;

                // other hero is noble
                if (otherHero.Clan != null && otherHero.Clan.IsNoble) {
                    relationNeeded += 25;

                    // other hero has a spouse
                    if (otherHero.Spouse != null) {
                        relationNeeded += 25;
                    }
                }

                // is hero a king
                if (IsHeroKing(hero)) {
                    relationNeeded -= 50;
                }

                // honor trait affect
                int honorLevel = otherHeroBaseObj.GetTraitLevel(DefaultTraits.Honor);
                relationNeeded += honorLevel * 15;

                // calculating/impulsive trait affect
                int calculatingLevel = otherHeroBaseObj.GetTraitLevel(DefaultTraits.Calculating);
                relationNeeded += calculatingLevel * 15;

                // daring trait affect
                int valorLevel = otherHeroBaseObj.GetTraitLevel(DefaultTraits.Valor);
                relationNeeded -= valorLevel * 15;
            }

            return relationNeeded;
        }

        public static bool PercentChanceCheck(int chanceOutOfHundred) {
            if (chanceOutOfHundred >= 100)
                return true;
            if (chanceOutOfHundred <= 0)
                return false;

            if (SubModule.Random.Next(1, 101) <= chanceOutOfHundred)
                return true;
            return false;
        }

        private static Dictionary<string, string> bastardSurnamesList = new Dictionary<string, string>
        {
            { "Sturgia", "Snow" },
            { "Empire", "Waters" },
            { "Battania", "Rivers" },
            { "Vlandia", "Hill" },
            { "Khuzait", "Grass" },
            { "Aserai", "Sand" }
        };
        public static string[] GetBastardName(Hero h) {
            string[] returnVals = { "", "" };

            string moddedName = h.FirstName.ToString();
            if (moddedName.Contains(" ")) {
                int indexOfSpace = moddedName.IndexOf(" ");
                moddedName = moddedName.Substring(0, indexOfSpace);
            }
            returnVals[0] = moddedName;

            string surname;
            try {
                surname = bastardSurnamesList[h.Culture.GetName().ToString()];
            } catch (KeyNotFoundException) {
                surname = "Waters";
            }

            moddedName = moddedName + " " + surname;
            returnVals[1] = moddedName;

            return returnVals;
        }

        public static bool HerosRelated(Hero h1, Hero h2) {
            if (h1.Father != null && h1.Father.Equals(h2) || h1.Mother != null && h1.Mother.Equals(h2)) { return true; }
            if (h2.Father != null && h2.Father.Equals(h1) || h2.Mother != null && h2.Mother.Equals(h1)) { return true; }

            if (h1.Children.Contains(h2)) { return true; }
            if (h2.Children.Contains(h1)) { return true; }

            if (h1.Father != null && h1.Father.Children.Contains(h2) || h1.Mother != null && h1.Mother.Children.Contains(h2)) { return true; }
            if (h2.Father != null && h2.Father.Children.Contains(h1) || h2.Mother != null && h2.Mother.Children.Contains(h1)) { return true; }

            return false;
        }
    }
}
