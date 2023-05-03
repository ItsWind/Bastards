using BastardChildren.AddonHelpers;
using BastardChildren.Models;
using HarmonyLib;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace BastardChildren
{
    public class SubModule : MBSubModuleBase
    {
        public static Random Random = new();

        protected override void OnSubModuleLoad() {
            new Harmony("BastardChildren").PatchAll();

            SetupBastardEvents();
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarter)
        {
            if (game.GameType is Campaign)
            {
                CampaignGameStarter campaignStarter = (CampaignGameStarter)gameStarter;

                campaignStarter.AddBehavior(new BastardCampaignBehavior(campaignStarter));
                campaignStarter.AddBehavior(new AIBastardConceptionCampaignBehavior());
            }
        }

        private void SetupBastardEvents() {
            BastardCampaignEvents.AddAction_OnPlayerBastardConceptionAttempt((hero) => ChangeRelationOnConceptionAttempt(hero));
            BastardCampaignEvents.AddAction_OnAIBastardConceptionAttempt((hero1, hero2) => ChangeRelationOnConceptionAttempt(hero1, hero2));
        }

        private void ChangeRelationOnConceptionAttempt(Hero hero1, Hero? hero2 = null) {
            if (hero2 == null)
                ChangeRelationAction.ApplyPlayerRelation(hero1, 2, false);
            else
                ChangeRelationAction.ApplyRelationChangeBetweenHeroes(hero1, hero2, 2);
        }
    }
}