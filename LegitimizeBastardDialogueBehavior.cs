using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;

namespace Bastards
{
    internal class LegitimizeBastardDialogueBehavior : CampaignBehaviorBase
    {
        public LegitimizeBastardDialogueBehavior(CampaignGameStarter starter)
        {
            this.AddDialog(starter);
        }
        public override void RegisterEvents()
        {
            // empty
        }

        public override void SyncData(IDataStore dataStore)
        {
            // EMpty
        }

        private void AddDialog(CampaignGameStarter starter)
        {
            // hero_main_options = self explanatory
            // lord_pretalk = make them ask "anything else?"
            // close_window = EXIT // seems to cause attack bug when done on map, so avoid.
            // lord_talk_speak_diplomacy_2 = "There is something I'd like to discuss."

            // Player is king and legitimize
            starter.AddPlayerLine("BastardsLegitimizeByPlayerStart", "hero_main_options", "BastardsLegitimizeByPlayerStartOutput",
                "I believe it's time you were made a true member to your family.",
            () => this.Conversation_PlayerCanLegitimizeBastard(), null, 100, null, null);

            starter.AddDialogLine("BastardsLegitimizeByPlayerConfirmationStart", "BastardsLegitimizeByPlayerStartOutput", "BastardsLegitimizeByPlayerConfirmation",
                "That would be an honor! However, are you sure that would be wise at the moment? It could cost you quite a lot of influence among the others.[rf:very_positive, rb:very_positive]",
            null, null, 100, null);

            starter.AddPlayerLine("BastardsLegitimizeByPlayerConfirmationOptionYes", "BastardsLegitimizeByPlayerConfirmation", "BastardsLegitimizeByPlayerConfirmationYes",
                "Let it be so.",
            null, null, 100, (out TextObject explain) => this.Conversation_HeroHasInfluenceToLegitimize(Hero.MainHero, out explain), null);
            starter.AddPlayerLine("BastardsLegitimizeByPlayerConfirmationOptionNo", "BastardsLegitimizeByPlayerConfirmation", "BastardsLegitimizeByPlayerConfirmationNo",
                "Perhaps you are right. The time isn't right.",
            null, null, 100, null, null);

            starter.AddDialogLine("BastardsLegitimizeByPlayerConfirmationEndYes", "BastardsLegitimizeByPlayerConfirmationYes", "lord_pretalk",
                "This is a great honor you bestow upon me! I will do my best to live up to your expectations.[rf:very_positive_ag, rb:very_positive]",
            null, () => this.Conversation_LegitimizeBastard(Hero.MainHero), 100, null);
            starter.AddDialogLine("BastardsLegitimizeByPlayerConfirmationEndNo", "BastardsLegitimizeByPlayerConfirmationNo", "lord_pretalk",
                "Alright... I will try to live up to your expectations.[rf:negative, rb:trivial]",
            null, null, 100, null);
        }

        private void SubtractHeroInfluence(Hero h)
        {
            float infNeeded = SubModule.configInfluenceCostForLegitimization;
            if (!Utils.IsHeroKing(h)) { infNeeded *= 2; }

            h.Clan.Influence -= infNeeded;
        }

        private bool Conversation_PlayerCanLegitimizeBastard()
        {
            Hero otherHero = Hero.OneToOneConversationHero;

            // If other hero is not a bastard
            if (!Utils.IsHeroBastard(otherHero)) { return false; }

            // If player is not king
            if (!Utils.IsHeroKing(Hero.MainHero)) { return false; }

            return true;
        }

        private bool Conversation_HeroHasInfluenceToLegitimize(Hero h, out TextObject explain)
        {
            float infNeeded = SubModule.configInfluenceCostForLegitimization;
            if (!Utils.IsHeroKing(h)) { infNeeded *= 2; }

            explain = new TextObject("You need to have " + SubModule.configInfluenceCostForLegitimization.ToString() + " influence and you must be a ruler.");

            return h.Clan.Influence >= infNeeded;
        }

        private void Conversation_LegitimizeBastard(Hero king)
        {
            Hero bastard = Hero.OneToOneConversationHero;

            this.SubtractHeroInfluence(king);

            Utils.PrintToMessages(bastard.Name.ToString() + " has been legitimized!", 255, 255, 133);
            Utils.PrintToMessages("You spent " + SubModule.configInfluenceCostForLegitimization.ToString() + " influence.");

            Utils.LegitimizeBastard(bastard);
        }
    }
}
