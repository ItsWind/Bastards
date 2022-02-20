using System;
using System.Collections.Generic;
using System.IO;

namespace Bastards
{
    internal class Config
    {
        private readonly string configFileString =
            "Set enableSurnames to 0 to disable automatic tacking on of Game of Thrones like surnames. 1 by default\n" +
            "enableSurnames=1\n\n" +

            "Set enableIncest to 1 to enable... well... incest. 0 by default\n" +
            "enableIncest=0\n\n" +

            "The minimum relation needed to engage in bastard conversations. 10 by default\n" +
            "minimumRelationNeeded=10\n\n" +

            "Set enableTraitAffectedRelationNeeded to 0 to disable traits affecting how much relation is needed. 1 by default\n" +
            "For example, with this enabled; honor level * 15 will be applied to the relation needed. Honorable women will need more relation and devious less.\n" +
            "enableTraitAffectedRelationNeeded=1\n\n" +

            "Set enableCannotRefuseIfKing to 1 if you want to never be refused if you are king. This bypasses relation/modded relation checks. 0 by default\n" +
            "enableCannotRefuseIfKing=0\n\n" +

            "If you would like to set a percent chance of conceiving a bastard, feel free here. 100 by default\n" +
            "percentChanceOfConception=100\n\n" +

            "When asking a lady if they want to ride with you and they reject OR they don't get pregnant, this is the amount of days to wait until you can try again. 7 by default\n" +
            "askedTimerInDays=7\n\n" +

            "Set influenceCostForLegitimization to the amount of influence needed to legitimize a bastard. 150 by default\n" +
            "influenceCostForLegitimization=150\n\n";

        private Dictionary<string, int> configValues = new();

        private void CreateConfigFile(string filePath)
        {
            StreamWriter sw = new(filePath);
            sw.WriteLine(this.configFileString);
            sw.Close();
        }

        public Config()
        {
            string configFilePath = AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 26) + "Modules\\Bastards\\config.txt";

            if (!File.Exists(configFilePath))
            {
                CreateConfigFile(configFilePath);
            }

            StreamReader sr = new(configFilePath);
            string line;
            // Read and display lines from the file until the end of
            // the file is reached.
            while ((line = sr.ReadLine()) != null)
            {
                int indexOfEqualSign = line.IndexOf('=');
                if (indexOfEqualSign != -1)
                {
                    string key = line.Substring(0, indexOfEqualSign);
                    string value = line.Substring(indexOfEqualSign + 1);
                    configValues.Add(key, Convert.ToInt32(value));
                }
            }
            sr.Close();
        }
        public Dictionary<string, int> GetConfigValues()
        {
            return configValues;
        }
    }
}
