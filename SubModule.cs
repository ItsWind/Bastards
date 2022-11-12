using BastardChildren.Models;
using HarmonyLib;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace BastardChildren
{
    public class SubModule : MBSubModuleBase
    {
        public static Config Config = new();
        public static Random Random = new();

        public static List<Bastard> Bastards = new();

        protected override void OnSubModuleLoad() {
            new Harmony("BastardChildren").PatchAll();
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarter)
        {
            base.OnGameStart(game, gameStarter);

            if (game.GameType is Campaign)
            {
                CampaignGameStarter campaignStarter = (CampaignGameStarter)gameStarter;

                campaignStarter.AddBehavior(new BastardCampaignBehavior(campaignStarter));
                campaignStarter.AddBehavior(new AIBastardConceptionCampaignBehavior());
            }
        }
    }
}