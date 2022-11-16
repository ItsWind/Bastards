using BastardChildren.StaticUtils;
using BastardChildren.Models;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace BastardChildren
{
    public class ConsoleCommands
    {
        [CommandLineFunctionality.CommandLineArgumentFunction("reloadconfig", "bastardchildren")]
        private static string CommandReloadConfig(List<string> args)
        {
            SubModule.Config.LoadConfig();
            return "Config reloaded!";
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("debug_force_birth", "bastardchildren")]
        private static string DebugForceBirth(List<string> args) {
            if (args.Count <= 0) return "Mother not specified. Use bastardchildren.debug_force_birth MotherNameHere";

            foreach (Bastard bastard in SubModule.Bastards) {
                if (bastard.hero != null) continue;
                string motherNameNoSpaces = bastard.mother.ToString().Replace(" ", "");
                if (args[0] == motherNameNoSpaces) {
                    bastard.Birth();
                    return "Successful birth.";
                }
            }

            return "No pregnancy with mother name '" + args[0] + "' can be found in Bastard Children data.";
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("debug_print_bastards", "bastardchildren")]
        private static string DebugPrintBastards(List<string> args) {
            foreach (Bastard bastard in SubModule.Bastards) {
                Utils.PrintToMessages("READING BASTARD", 255, 0, 0);
                Utils.PrintToMessages("Father: " + bastard.father.ToString());
                Utils.PrintToMessages("Mother: " + bastard.mother.ToString());
                if (bastard.hero != null) {
                    Utils.PrintToMessages("Bastard: " + bastard.hero.ToString());
                    Utils.PrintToMessages("Bastard clan: " + bastard.hero.Clan.ToString());
                } else {
                    Utils.PrintToMessages("Still cooking.");
                    Utils.PrintToMessages("Ding time: " + CampaignTime.Milliseconds((long)bastard.birthTimeInMilliseconds).ToString());
                }
            }
            return "Bastards printed in information text window!";
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("debug_make_main_hero_ill", "bastardchildren")]
        private static string DebugMainHeroIll(List<string> args) {
            Campaign.Current.MainHeroIllDays = 1;
            return "Main hero is now ill!";
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("debug_set_lord_occupation", "bastardchildren")]
        private static string DebugSetLord(List<string> args) {
            if (args.Count == 1) {
                foreach (Bastard bastard in SubModule.Bastards) {
                    Hero? bastardHero = bastard.hero;
                    if (bastardHero == null) continue;

                    string noSpaceName = bastardHero.Name.ToString().Replace(" ", "");
                    if (noSpaceName == args[0]) {
                        bastardHero.SetNewOccupation(Occupation.Lord);
                        return bastardHero.Name + " has been set to a lord in occupation!";
                    }
                }
                return "No bastard found with that name.";
            }
            return "You used it wrong.";
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("debug_legitimize", "bastardchildren")]
        private static string DebugLegitimize(List<string> args) {
            if (args.Count == 1) {
                foreach (Bastard bastard in SubModule.Bastards) {
                    Hero? bastardHero = bastard.hero;
                    if (bastardHero == null) continue;

                    string noSpaceName = bastardHero.Name.ToString().Replace(" ", "");
                    if (noSpaceName == args[0]) {
                        bastard.Legitimize();
                        return bastardHero.Name + " has been legitimized!";
                    }
                }
                return "No bastard found with that name.";
            }
            return "You used it wrong.";
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("debug_come_of_age", "bastardchildren")]
        private static string DebugComeOfAge(List<string> args) {
            if (args.Count == 1) {
                foreach (Bastard bastard in SubModule.Bastards) {
                    Hero? bastardHero = bastard.hero;
                    if (bastardHero == null) continue;

                    string noSpaceName = bastardHero.Name.ToString().Replace(" ", "");
                    if (noSpaceName == args[0]) {
                        float yearsNeeded = 18f - bastardHero.Age;
                        bastardHero.SetBirthDay(bastardHero.BirthDay - CampaignTime.Years(yearsNeeded));
                        return bastardHero.Name + " has been set to 18 years old!";
                    }
                }
                return "No bastard found with that name.";
            }
            return "You used it wrong.";
        }
    }
}