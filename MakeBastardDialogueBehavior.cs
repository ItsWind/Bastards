using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;

namespace Bastards
{
    internal class MakeBastardDialogueBehavior : CampaignBehaviorBase
    {
        private Dictionary<Hero, CampaignTime> peopleAsked = new();

        public MakeBastardDialogueBehavior(CampaignGameStarter starter)
        {
            this.AddDialog(starter);
        }

        public override void RegisterEvents()
        {
            // empty
        }

        public override void SyncData(IDataStore dataStore)
        {
            // empty ?
        }

        private bool GetHeroAlreadyAsked(Hero h)
        {
            if (peopleAsked.ContainsKey(h))
            {
                if (CampaignTime.Now >= peopleAsked[h])
                {
                    peopleAsked.Remove(h);
                    return false;
                }
                return true;
            }
            return false;
        }

        private void SetHeroAlreadyAsked(Hero h)
        {
            peopleAsked[h] = CampaignTime.DaysFromNow(SubModule.configAskedTimerInDays);
        }

        private int GetTraitAffectedRelationNeeded(Hero hero)
        {
            bool isPlayerKing = Utils.IsHeroKing(Hero.MainHero);
            if (SubModule.configEnableCannotRefuseIfKing && isPlayerKing)
            {
                return -100;
            }
            else if (SubModule.configEnableTraitAffectedRelationNeeded)
            {
                CharacterObject obj = hero.CharacterObject;
                int moddedRelationNeeded = SubModule.configMinimumRelationNeeded;

                // is noble lady affect
                if (hero.IsNoble)
                {
                    moddedRelationNeeded += 25;
                    if (hero.Spouse != null)
                    {
                        moddedRelationNeeded += 25;
                    }
                }

                // is player king affect
                if (isPlayerKing)
                {
                    moddedRelationNeeded -= 50;
                }

                // honor trait affect
                int honorLevel = obj.GetTraitLevel(DefaultTraits.Honor);
                moddedRelationNeeded += honorLevel * 15;

                // calculating/impulsive trait affect
                int calculatingLevel = obj.GetTraitLevel(DefaultTraits.Calculating);
                moddedRelationNeeded += calculatingLevel * 15;

                // daring trait affect
                int valorLevel = obj.GetTraitLevel(DefaultTraits.Valor);
                moddedRelationNeeded -= valorLevel * 15;

                return moddedRelationNeeded;
            }
            return SubModule.configMinimumRelationNeeded;
        }

        private void AddDialog(CampaignGameStarter starter)
        {
            // hero_main_options = self explanatory
            // lord_pretalk = make them ask "anything else?"
            // close_window = EXIT // seems to cause attack bug when done on map, so avoid.
            // lord_talk_speak_diplomacy_2 = "There is something I'd like to discuss."

            // BASE MOD DIALOG

            // Bastard event engage
            starter.AddPlayerLine("BastardsEventStart", "hero_main_options", "BastardsEventStartOutput", "I was thinking that you and I...",
                () => this.Conversation_BastardTopicsAllowed(), null, 100, null, null);

            // WHEN other hero has already been asked
            starter.AddDialogLine("BastardsEventStartQuestionedAlreadyAsked", "BastardsEventStartOutput", "lord_pretalk", "I'm not in the mood for this.",
                () => this.Conversation_BastardTopicsAlreadyAskedCooldown(), null, 100, null);
            // WHEN Other hero is interested
            starter.AddDialogLine("BastardsEventStartQuestionedInterested", "BastardsEventStartOutput", "BastardsEventConfirmBastard", "...? You and I...?",
                () => this.Conversation_CanMakeBastard(false), null, 100, null);
            // WHEN NOT interested
            starter.AddDialogLine("BastardsEventStartQuestionedNotInterested", "BastardsEventStartOutput", "lord_pretalk", "...? I don't know what picture you're trying to paint here, but this conversation is boring me.",
                () => this.Conversation_CanMakeBastard(true), () => this.SetHeroAlreadyAsked(Hero.OneToOneConversationHero), 100, null);

            // mad game
            starter.AddPlayerLine("BastardsEventConfirmation", "BastardsEventConfirmBastard", "BastardsEventMakeBastard", "Will you ride with me for a while?",
                null, null, 100, null);
            starter.AddPlayerLine("BastardsEventConfirmation", "BastardsEventConfirmBastard", "lord_pretalk", "Uh, nevermind.",
                null, null, 90, null);

            // OOOOOOO THREE POINT SHOT
            starter.AddDialogLine("BastardsEventConfirmationReceived", "BastardsEventMakeBastard", "lord_pretalk", "Yes.. I think I would like that very much.",
                null, () => this.Conversation_ConceiveBastard(), 100, null);
        }
        
        private bool Conversation_BastardTopicsAlreadyAskedCooldown()
        {
            Hero otherHero = Hero.OneToOneConversationHero;

            Dictionary<string, Hero> sortedMaleFemale = Utils.SortMaleFemaleHero(Hero.MainHero, otherHero);
            if (sortedMaleFemale.Count == 0) { return false; }
            if (sortedMaleFemale["female"].IsPregnant) { return false; }

            if (this.GetHeroAlreadyAsked(otherHero))
            {
                return true;
            }
            return false;
        }
        private bool Conversation_BastardTopicsAllowed()
        {
            Hero otherHero = Hero.OneToOneConversationHero;
            if (Hero.MainHero.IsChild || otherHero.IsChild) { return false; }

            Dictionary<string, Hero> sortedMaleFemale = Utils.SortMaleFemaleHero(Hero.MainHero, otherHero);
            if (sortedMaleFemale.Count == 0) { return false; }

            if (sortedMaleFemale["female"].IsPregnant) { return false; }

            if (!SubModule.configEnableIncest && Utils.HerosRelated(Hero.MainHero, otherHero)) { return false; }

            return true;
        }

        private bool Conversation_CanMakeBastard(bool checkFail)
        {
            bool returnVal = false;
            Hero otherHero = Hero.OneToOneConversationHero;

            Dictionary<string, Hero> sortedMaleFemale = Utils.SortMaleFemaleHero(Hero.MainHero, otherHero);

            if (!sortedMaleFemale["female"].IsPregnant && otherHero.GetRelationWithPlayer() >= this.GetTraitAffectedRelationNeeded(otherHero))
            {
                returnVal = true;
            }

            if (checkFail) { returnVal = !returnVal; }
            return returnVal;
        }

        private void Conversation_ConceiveBastard()
        {
            Hero otherHero = Hero.OneToOneConversationHero;
            // Conception chance
            if (SubModule.rand.Next(1, 101) <= SubModule.configPercentChanceOfConception)
            {
                Dictionary<string, Hero> sortedMaleFemale = Utils.SortMaleFemaleHero(Hero.MainHero, otherHero);

                sortedMaleFemale["female"].IsPregnant = true;
                Utils.PrintToMessages(sortedMaleFemale["female"].Name + " has gotten pregnant!", 255, 153, 204);
                SubModule.BastardsManagerInstance.AddBastardToBeBorn(sortedMaleFemale["male"], sortedMaleFemale["female"], CampaignTime.DaysFromNow(SubModule.rand.Next(SubModule.MIN_DAYS_PREGNANT_WITH_BASTARD, SubModule.MAX_DAYS_PREGNANT_WITH_BASTARD + 1))); // 58, 69
            }

            this.SetHeroAlreadyAsked(otherHero);

            int charmMod = 200;
            if (otherHero.IsNoble)
            {
                charmMod += 500;
            }
            Hero.MainHero.AddSkillXp(DefaultSkills.Charm, charmMod);
        }
    }
}
