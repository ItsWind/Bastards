using HarmonyLib;
using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade;

namespace Bastards
{
    public class SubModule : MBSubModuleBase
    {
        public static Random rand = new();

        public static BastardsManager BastardsManagerInstance = new BastardsManager();

        private Config config = new();
        public static int configMinimumRelationNeeded { get; set; }
        public static int configPercentChanceOfConception { get; set; }
        public static int configAskedTimerInDays { get; set; }
        public static int configInfluenceCostForLegitimization { get; set; }

        public static bool configEnableSurnames { get; set; }
        public static bool configEnableIncest { get; set; }
        public static bool configEnableTraitAffectedRelationNeeded { get; set; }
        public static bool configEnableCannotRefuseIfKing { get; set; }

        public static int MIN_DAYS_PREGNANT_WITH_BASTARD = 55; //55
        public static int MAX_DAYS_PREGNANT_WITH_BASTARD = 65; //65

        /*protected override void OnApplicationTick(float dt)
        {
            if (Input.IsKeyPressed(InputKey.Comma))
            {
                int count = 0;
                foreach (BastardsToBeBornDataHolder data in BastardsManagerInstance.bastardsToBeBorn.ToList())
                {
                    count++;
                    Utils.PrintToMessages("READING BASTARD TO BE BORN #" + count.ToString(), 255, 0, 0);
                    Utils.PrintToMessages("FATHER: " + data.father.Name);
                    Utils.PrintToMessages("MOTHER: " + data.mother.Name);
                    Utils.PrintToMessages("BIRTHDAY: " + data.birthTime.ToString());
                }
            }
            else if (Input.IsKeyPressed(InputKey.Period))
            {
                int count = 0;
                foreach (Hero h in BastardsManagerInstance.bastards.ToList())
                {
                    count++;
                    Utils.PrintToMessages("READING BASTARD #" + count.ToString(), 255, 0, 0);
                    Utils.PrintToMessages("NAME: " + h.Name.ToString());
                    Utils.PrintToMessages("CLAN NAME: " + h.Clan.Name.ToString());
                }
            }
        }*/

        protected override void OnSubModuleLoad()
        {
            configMinimumRelationNeeded = this.config.GetConfigValues()["minimumRelationNeeded"];
            configPercentChanceOfConception = this.config.GetConfigValues()["percentChanceOfConception"];
            configAskedTimerInDays = this.config.GetConfigValues()["askedTimerInDays"];
            configInfluenceCostForLegitimization = this.config.GetConfigValues()["influenceCostForLegitimization"];

            configEnableSurnames = Convert.ToBoolean(this.config.GetConfigValues()["enableSurnames"]);
            configEnableIncest = Convert.ToBoolean(this.config.GetConfigValues()["enableIncest"]);
            configEnableTraitAffectedRelationNeeded = Convert.ToBoolean(this.config.GetConfigValues()["enableTraitAffectedRelationNeeded"]);
            configEnableCannotRefuseIfKing = Convert.ToBoolean(this.config.GetConfigValues()["enableCannotRefuseIfKing"]);

            new Harmony("Bastards").PatchAll();
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarter)
        {
            if (game.GameType is Campaign)
            {
                //The current game is a campaign, load campaign heartbeat behavior
                CampaignGameStarter campaignStarter = (CampaignGameStarter)gameStarter;

                campaignStarter.AddBehavior(new SavingBehavior());

                campaignStarter.AddBehavior(new BirthBastardTickBehavior());

                campaignStarter.AddBehavior(new MakeBastardDialogueBehavior(campaignStarter));
                campaignStarter.AddBehavior(new LegitimizeBastardDialogueBehavior(campaignStarter));
            }
        }
    }
}