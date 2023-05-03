using BastardChildren.Models;
using BastardChildren.StaticUtils;
using MCM.Abstractions.Base.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace BastardChildren {
    public class BastardCampaignBehavior : CampaignBehaviorBase {
        public static BastardCampaignBehavior Instance;

        public List<Bastard> Bastards = new();

        public BastardCampaignBehavior(CampaignGameStarter game) {
            Instance = this;

            AddBaseModDialogs(game);
            AddBecomeBastardDialogs(game);
            AddLegitimizeBastardDialogs(game);
        }

        public override void RegisterEvents() {
            CampaignEvents.BeforeHeroKilledEvent.AddNonSerializedListener(this, new Action<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>( (hero1, hero2, killActionDetail, someBool) => {
                if (hero1 != null) {
                    // If hero killed is a bastard
                    /*Bastard? bastard = Utils.GetBastardFromHero(hero1);
                    if (bastard != null)
                        Bastards.Remove(bastard);*/
                    
                    // If hero killed is pregnant with a bastard
                    if (hero1.IsPregnant) {
                        Bastard? possibleBastardInBelly = null;

                        try {
                            possibleBastardInBelly = Bastards.Where(x => x.hero == null && x.mother == hero1).First();
                        } catch (Exception) { }

                        if (possibleBastardInBelly != null) {
                            hero1.IsPregnant = false;
                            Bastards.Remove(possibleBastardInBelly);
                        }
                    }
                }
            } ));
        }

        public override void SyncData(IDataStore dataStore) {
            dataStore.SyncData("Bastards", ref Bastards);

            if (dataStore.IsLoading) {
                foreach (Bastard bastard in Bastards.ToList()) {
                    Hero? bastardHero = bastard.hero;
                    if (bastardHero == null && bastard.mother.IsPregnant)
                        continue;
                    else if (Campaign.Current.AliveHeroes.Contains(bastardHero) || Campaign.Current.DeadOrDisabledHeroes.Contains(bastardHero))
                        continue;

                    Bastards.Remove(bastard);
                }
            }
        }

        private void AddBaseModDialogs(CampaignGameStarter game) {
            // hero_main_options = self explanatory
            // lord_pretalk = make them ask "anything else?"
            // close_window = EXIT // seems to cause attack bug when done on map, so avoid.
            // lord_talk_speak_diplomacy_2 = "There is something I'd like to discuss."

            // BASE MOD DIALOG

            // Bastard event engage
            if (!GlobalSettings<MCMConfig>.Instance.DisableConversationOption)
                game.AddPlayerLine("BastardsEventStart", "hero_main_options", "BastardsEventStartOutput",
                    "{=BastardsEventStart}I was thinking that you and I...",
                    () => Conversation.BastardConceptionAllowed(), null, 100, null, null);

            // WHEN other hero has already been asked
            game.AddDialogLine("BastardsEventStartQuestionedAlreadyAsked", "BastardsEventStartOutput", "lord_pretalk",
                "{=BastardsEventStartQuestionedAlreadyAsked}I'm not in the mood for this.[rf:negative, rb:negative]",
                () => Conversation.BastardConceptionAlreadyAskedCooldown(), null, 110, null);
            // WHEN Other hero is interested
            game.AddDialogLine("BastardsEventStartQuestionedInterested", "BastardsEventStartOutput", "BastardsEventConfirmBastard",
                "{=BastardsEventStartQuestionedInterested}...? You and I...?[rf:unsure, rb:unsure]",
                () => Conversation.BastardConceptionAccepted(), null, 100, null);
            // WHEN NOT interested
            game.AddDialogLine("BastardsEventStartQuestionedNotInterested", "BastardsEventStartOutput", "lord_pretalk",
                "{=BastardsEventStartQuestionedNotInterested}...? I don't know what picture you're trying to paint here, but this conversation is boring me.[rf:very_negative_ag, rb:negative]",
                null, () => ChangeRelationAction.ApplyPlayerRelation(Hero.OneToOneConversationHero, -2, false), 90, null);

            // mad game
            game.AddPlayerLine("BastardsEventConfirmationYes", "BastardsEventConfirmBastard", "BastardsEventMakeBastard",
                "{=BastardsEventConfirmationYes}Will you ride with me for a while?",
                null, null, 100, null);
            game.AddPlayerLine("BastardsEventConfirmationNo", "BastardsEventConfirmBastard", "lord_pretalk",
                "{=BastardsEventConfirmationNo}Uh, nevermind.",
                null, null, 90, null);

            // OOOOOOO THREE POINT SHOT
            game.AddDialogLine("BastardsEventConfirmationReceived", "BastardsEventMakeBastard", "lord_pretalk",
                "{=BastardsEventConfirmationReceived}Yes.. I think I would like that very much.[rf:positive, rb:positive]",
                null, () => Conversation.ConceiveBastard(), 100, null);
        }

        private void AddBecomeBastardDialogs(CampaignGameStarter game) {
            game.AddPlayerLine("BastardsBecomeHerosBastard", "lord_talk_speak_diplomacy_2", "BastardsBecomeHerosBastardOutput",
                "{=BastardsBecomeHerosBastard}I believe I'm your bastard child.",
            null, null, 100, null, null);

            game.AddDialogLine("BastardsBecomeHerosBastardConfirmationStart", "BastardsBecomeHerosBastardOutput", "BastardsBecomeHerosBastardConfirmation",
                "{=BastardsBecomeHerosBastardConfirmationStart}Eh, are you sure about that...?[rf:unsure, rb:unsure]",
            null, null, 100, null);

            game.AddPlayerLine("BastardsBecomeHerosBastardConfirmationOptionYes", "BastardsBecomeHerosBastardConfirmation", "BastardsBecomeHerosBastardConfirmationYes",
                "{=BastardsBecomeHerosBastardConfirmationOptionYes}Yea, just look at me and try to deny it.",
            null, null, 100, null, null);
            game.AddPlayerLine("BastardsBecomeHerosBastardConfirmationOptionNo", "BastardsBecomeHerosBastardConfirmation", "BastardsBecomeHerosBastardConfirmationNo",
                "{=BastardsBecomeHerosBastardConfirmationOptionNo}Perhaps you are right. Apologies, been drinking too much lately.",
            null, null, 90, null, null);

            game.AddDialogLine("BastardsBecomeHerosBastardConfirmationEndYes", "BastardsBecomeHerosBastardConfirmationYes", "lord_pretalk",
                "{=BastardsBecomeHerosBastardConfirmationEndYes}Yea, I suppose so! You seem to have my features well enough![rf:very_positive_ag, rb:very_positive]",
            null, () => Conversation.BecomeBastardOfHero(), 100, null);
            game.AddDialogLine("BastardsBecomeHerosBastardConfirmationEndNo", "BastardsBecomeHerosBastardConfirmationNo", "lord_pretalk",
                "{=BastardsBecomeHerosBastardConfirmationEndNo}Ehm...[rf:negative_ag]",
            null, null, 100, null);
        }

        private void AddLegitimizeBastardDialogs(CampaignGameStarter game) {
            game.AddPlayerLine("BastardsLegitimizeByPlayerStart", "hero_main_options", "BastardsLegitimizeByPlayerStartOutput",
                "{=BastardsLegitimizeByPlayerStart}I believe it's time you were made a true member to your family.",
            () => Conversation.PlayerCanLegitimizeBastard(), null, 100, null, null);

            game.AddDialogLine("BastardsLegitimizeByPlayerConfirmationStart", "BastardsLegitimizeByPlayerStartOutput", "BastardsLegitimizeByPlayerConfirmation",
                "{=BastardsLegitimizeByPlayerConfirmationStart}That would be an honor! However, are you sure that would be wise at the moment? It could cost you quite a lot of influence among the others.[rf:very_positive, rb:very_positive]",
            null, null, 100, null);

            game.AddPlayerLine("BastardsLegitimizeByPlayerConfirmationOptionYes", "BastardsLegitimizeByPlayerConfirmation", "BastardsLegitimizeByPlayerConfirmationYes",
                "{=BastardsLegitimizeByPlayerConfirmationOptionYes}Let it be so.",
            null, null, 100, (out TextObject explain) => Conversation.HasInfluenceToLegitimize(out explain), null);
            game.AddPlayerLine("BastardsLegitimizeByPlayerConfirmationOptionNo", "BastardsLegitimizeByPlayerConfirmation", "BastardsLegitimizeByPlayerConfirmationNo",
                "{=BastardsLegitimizeByPlayerConfirmationOptionNo}Perhaps you are right. The time isn't right.",
            null, null, 90, null, null);

            game.AddDialogLine("BastardsLegitimizeByPlayerConfirmationEndYes", "BastardsLegitimizeByPlayerConfirmationYes", "lord_pretalk",
                "{=BastardsLegitimizeByPlayerConfirmationEndYes}This is a great honor you bestow upon me! I will do my best to live up to your expectations.[rf:very_positive, rb:very_positive]",
            null, () => Conversation.LegitimizeBastard(), 100, null);

            game.AddDialogLine("BastardsLegitimizeByPlayerConfirmationEndNo", "BastardsLegitimizeByPlayerConfirmationNo", "lord_pretalk",
                "{=BastardsLegitimizeByPlayerConfirmationEndNo}Alright... I will try to live up to your expectations.[rf:negative, rb:unsure]",
            null, null, 100, null);
        }
    }
    public class CustomSaveDefiner : SaveableTypeDefiner {
        public CustomSaveDefiner() : base(823997416) { }

        protected override void DefineClassTypes() {
            AddClassDefinition(typeof(Bastard), 1);
        }

        protected override void DefineContainerDefinitions() {
            ConstructContainerDefinition(typeof(List<Bastard>));
        }
    }
}
