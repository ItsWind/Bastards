using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Bastards
{
    public static class Utils
    {
        public static bool IsHeroKing(Hero h)
        {
            return h.Clan != null && h.Clan.Kingdom != null && h.Clan.Kingdom.Leader.Equals(h);
        }

        public static bool IsHeroBastard(Hero h)
        {
            return SubModule.BastardsManagerInstance.bastards.Contains(h);
        }

        public static bool HerosRelated(Hero h1, Hero h2)
        {
            if (h1.Father != null && h1.Father.Equals(h2) || h1.Mother != null && h1.Mother.Equals(h2)) { return true; }
            if (h2.Father != null && h2.Father.Equals(h1) || h2.Mother != null && h2.Mother.Equals(h1)) { return true; }

            if (h1.Children.Contains(h2)) { return true; }
            if (h2.Children.Contains(h1)) { return true; }

            if (h1.Father != null && h1.Father.Children.Contains(h2) || h1.Mother != null && h1.Mother.Children.Contains(h2)) { return true; }
            if (h2.Father != null && h2.Father.Children.Contains(h1) || h2.Mother != null && h2.Mother.Children.Contains(h1)) { return true; }

            return false;
        }

        public static void LegitimizeBastard(Hero h)
        {
            if (Utils.IsHeroBastard(h))
            {
                // Set name without surname
                string moddedName = h.FirstName.ToString();
                if (moddedName.Contains(" "))
                {
                    int indexOfSpace = moddedName.IndexOf(" ");
                    moddedName = moddedName.Substring(0, indexOfSpace);
                }
                h.SetName(new TextObject(moddedName), new TextObject(moddedName));

                // Make hero a noble
                h.IsNoble = true;

                // Remove from bastards list
                SubModule.BastardsManagerInstance.RemoveBastard(h);
            }
        }

        public static Dictionary<string, string> BastardSurnamesList = new Dictionary<string, string>
        {
            { "Sturgia", "Snow" },
            { "Empire", "Waters" },
            { "Battania", "Rivers" },
            { "Vlandia", "Hill" },
            { "Khuzait", "Grass" },
            { "Aserai", "Sand" }
        };
        public static string[] GetBastardName(Hero h)
        {
            Dictionary<string, string> bastardSurnames = Utils.BastardSurnamesList;
            string[] returnVals = { "", "" };

            string moddedName = h.FirstName.ToString();
            if (moddedName.Contains(" "))
            {
                int indexOfSpace = moddedName.IndexOf(" ");
                moddedName = moddedName.Substring(0, indexOfSpace);
            }
            returnVals[0] = moddedName;

            string surname;
            try
            {
                surname = bastardSurnames[h.Culture.GetName().ToString()];
            }
            catch (KeyNotFoundException e)
            {
                surname = "Waters";
            }

            moddedName = moddedName + " " + surname;
            returnVals[1] = moddedName;

            return returnVals;
        }

        public static Dictionary<string, Hero> SortMaleFemaleHero(Hero h1, Hero h2)
        {
            Dictionary<string, Hero> returnTable = new();
            if (!((h1.IsFemale && h2.IsFemale) || (!h1.IsFemale && !h2.IsFemale)))
            {
                if (h1.IsFemale)
                {
                    returnTable["female"] = h1;
                    returnTable["male"] = h2;
                }
                else
                {
                    returnTable["female"] = h2;
                    returnTable["male"] = h1;
                }
            }
            return returnTable;
        }
        public static void PrintToMessages(string str, int r, int g, int b)
        {
            float[] newValues = { (float)r / 255.0f, (float)g / 255.0f, (float)b / 255.0f };
            Color col = new(newValues[0], newValues[1], newValues[2]);
            InformationManager.DisplayMessage(new InformationMessage(str, col));
        }
        public static void PrintToMessages(string str)
        {
            InformationManager.DisplayMessage(new InformationMessage(str));
        }
    }
}
