using BastardChildren.StaticUtils;
using System;
using System.Collections.Generic;
using System.IO;

namespace BastardChildren
{
    public class Config
    {
        private string configFilePath = AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 26) + "Modules\\BastardChildren\\config.txt";

        private Dictionary<string, double> configValues = new();
        private string configFileString =
@"-- BIRTH CONFIG --


minDaysUntilBirth=57.0
> Set the minimum days after conception until birth. Default is 57.

maxDaysUntilBirth=68.0
> Set the maximum days after conception until birth. Default is 68.

percentChanceOfConception=25
> Percent chance of conceiving a bastard. 25 by default

percentChanceOfLaborDeath=10
> Percent chance of mother dying in labor. 10 by default

percentChanceOfStillbirth=15
> Percent chance of stillbirth. 15 by default


-- REQUIREMENTS CONFIG --


enableIncest=0
> Set enableIncest to 1 to enable... well... incest. 0 by default.

minimumRelationNeeded=10
> The minimum relation needed to engage in bastard conversations. 10 by default.

enableTraitAffectedRelationNeeded=1
> Set enableTraitAffectedRelationNeeded to 0 to disable traits affecting how much relation is needed. 1 by default
For example, with this enabled; honor level * 15 will be applied to the relation needed. Honorable heroes will need more relation and devious less.

askedTimerInDays=3.0
> After asking a hero for bastard conception, this is how long after you must wait to ask again. Default is 3 days


-- LEGITIMIZATION CONFIG --


legitimizationInfluenceCost=150.0
> Set influence cost for legitimizing bastard. This is DOUBLE when NOT a RULER. 150 by default.

bastardsClanHeirLegitimization=1
> Set to 0 to disable bastards becoming legitimized upon ascending to lead a clan. 1 by default.

bastardsMarriageValueMult=0.75
> Set the multiplier for marriage with bastards. It's hard to explain, just mess with it if you want.

bastardsMarriageLegitimization=1
> Set to 0 to disable bastards becoming legit upon a marriage. 1 by default.


-- CONSEQUENCES CONFIG --


enableConsequences=1
> Set to 0 to disable consequences for sending a bastard to live with another family. 1 by default.

percentChanceKeptSecret=50
> Set the percent chance that the bastard's parentage is kept secret when sent to the other clan. 50 by default.
Secret only works if the other hero is female, if the player is female sending a bastard the female spouse obviously knows it's not hers

spouseRelationLoss=-60
> Set the relation loss suffered with the other hero's spouse if they have one. -60 by default.

clanLeaderRelationLoss=-40
> Set the relation loss suffered with the clan leader of the other hero if it's not them. -40 by default.


-- MISC CONFIG --


enableSurnames=1
> Set enableSurnames to 0 to disable automatic tacking on of Game of Thrones like surnames. 1 by default

enableCruelty=0
> Set enableCruelty to 1 to enable cruel choices. 0 by default. Keep this at 0 if you are triggered easily.
";

        public Config() {
            if (!File.Exists(configFilePath))
                CreateConfigFile();
            LoadConfig();
        }

        private void CreateConfigFile() {
            StreamWriter sw = new(configFilePath);
            sw.WriteLine(configFileString);
            sw.Close();
        }

        private void WipeConfigFile() {
            Utils.PrintToMessages("! ! CONFIG VALUE ERROR ! ! Resetting config.txt for BastardChildren. Make sure to set back your values if you changed anything. " +
                "You can use bastardchildren.reloadconfig in console to reload your config settings.", 255, 20, 20);
            File.Delete(configFilePath);
            CreateConfigFile();
            LoadConfig();
        }

        public void LoadConfig() {
            configValues.Clear();

            StreamReader sr = new(configFilePath);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                int indexOfEqualSign = line.IndexOf('=');
                if (indexOfEqualSign != -1)
                {
                    string key = line.Substring(0, indexOfEqualSign);
                    string value = line.Substring(indexOfEqualSign + 1);
                    try {
                        configValues[key] = Convert.ToDouble(value);
                    }
                    catch (Exception ex) {
                        if (!(ex is FormatException) && !(ex is ArithmeticException)) throw;

                        WipeConfigFile();
                    }
                }
            }
            sr.Close();
        }

        private object GetValue(string key) {
            try {
                return configValues[key];
            }
            catch (KeyNotFoundException) {
                WipeConfigFile();
                return configValues[key];
            }
        }

        public int GetValueInt(string key) {
            object value = GetValue(key);
            return Convert.ToInt32(value);
        }

        public double GetValueDouble(string key) {
            object value = GetValue(key);
            return Convert.ToDouble(value);
        }

        public bool GetValueBool(string key) {
            object value = GetValue(key);
            return Convert.ToBoolean(value);
        }
    }
}
