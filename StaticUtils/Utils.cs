using BastardChildren.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Library;
using TaleWorlds.Localization;

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

        private static void ModifyHeroRelations(Hero hero1, Hero hero2, int mod) {
            int currRelation = CharacterRelationManager.GetHeroRelation(hero1, hero2);
            CharacterRelationManager.SetHeroRelation(hero1, hero2, currRelation + mod);
        }
        public static void ModifyPlayerRelations(Hero hero, int mod) {
            ModifyHeroRelations(Hero.MainHero, hero, mod);
            int r = 0;
            int g = 0;
            int b = 0;
            if (mod > 0)
                g = 204;
            else
                r = 204;
            PrintToMessages("Your relation with " + hero.Name + " of " + hero.Clan.Name + " has changed by " + mod + ".", r, g, b);
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

        public static int GetRelationNeededForConceptionAcceptance(Hero otherHero) {
            int relationNeeded = SubModule.Config.GetValueInt("minimumRelationNeeded");

            if (SubModule.Config.GetValueBool("enableTraitAffectedRelationNeeded")) {
                CharacterObject otherHeroBaseObj = otherHero.CharacterObject;

                // other hero is noble
                if (otherHero.Clan != null && otherHero.Clan.IsNoble) {
                    relationNeeded += 25;

                    // other hero has a spouse
                    if (otherHero.Spouse != null) {
                        relationNeeded += 25;
                    }
                }

                // is player king affect
                if (IsHeroKing(Hero.MainHero)) {
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
