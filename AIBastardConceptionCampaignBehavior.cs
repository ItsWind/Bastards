using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using BastardChildren.StaticUtils;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Party;
using BastardChildren.Models;

namespace BastardChildren {
    public class AIBastardConceptionCampaignBehavior : CampaignBehaviorBase {
        private static Dictionary<Hero, CampaignTime> heroesAskedForConception = new();

        public override void RegisterEvents() {
            CampaignEvents.DailyTickHeroEvent.AddNonSerializedListener(this, ConceptBastardDailyTickHero);
        }

        public override void SyncData(IDataStore dataStore) {
            //
        }

        private void ConceptBastardDailyTickHero(Hero hero) {
            if (!SubModule.Config.GetValueBool("aiAllowedToHaveBastards")) return;

            if (hero == Hero.MainHero) return;

            if (hero.IsChild) return;

            if (hero.Occupation != Occupation.Lord) return;

            // if the hero is a prisoner
            if (hero.IsPrisoner) {
                HandleCrueltyDailyTickHero(hero);
                return;
            }

            // if the hero is in a settlement
            if (hero.CurrentSettlement != null) {
                Settlement settlement = hero.CurrentSettlement;
                TryConceptionsFromPartyList(hero, settlement.Parties.ToList());
            }
            // if the hero is in an army
            else if (hero.PartyBelongedTo != null && hero.PartyBelongedTo.Army != null) {
                TryConceptionsFromPartyList(hero, hero.PartyBelongedTo.Army.Parties.ToList());
            }
        }

        private void TryConceptionsFromPartyList(Hero hero, List<MobileParty> parties) {
            foreach (MobileParty party in parties) {
                Hero? otherHero = party.LeaderHero;
                if (otherHero == null || otherHero == hero || otherHero.IsChild || otherHero == Hero.MainHero) continue;

                TryConception(hero, otherHero);
            }
        }

        private void TryConception(Hero hero1, Hero hero2) {
            // Get female hero
            Hero? femaleHero = Utils.GetFemaleHero(hero1, hero2);

            // If female hero not found (same gender) or female hero is pregnant
            if (femaleHero == null || femaleHero.IsPregnant) return;

            // If the heroes share a clan
            if (hero1.Clan != null && hero2.Clan != null && hero1.Clan == hero2.Clan) return;

            // If heroes are related
            if (Utils.HerosRelated(hero1, hero2)) return;

            // If either hero wouldn't try to conceive a bastard
            if (!Utils.GetIfHeroWouldConceiveBastard(hero1) || !Utils.GetIfHeroWouldConceiveBastard(hero2)) return;

            // Get relation needed and the current relation
            int relationNeeded = Utils.GetRelationNeededForConceptionAcceptance(hero1, hero2);
            int currRelation = CharacterRelationManager.GetHeroRelation(hero1, hero2);

            // If current relation is under needed relation
            if (currRelation < relationNeeded) return;

            // If hero has already been asked
            if (heroesAskedForConception.ContainsKey(hero2) && heroesAskedForConception[hero2].IsFuture) return;

            // attempt conception
            heroesAskedForConception[hero2] = CampaignTime.DaysFromNow((float)SubModule.Config.GetValueDouble("askedTimerInDays"));

            if (Utils.PercentChanceCheck(SubModule.Config.GetValueInt("percentChanceOfConception"))) {
                //Utils.PrintToMessages(hero1.ToString() + " : " + hero2.ToString() + " : CONCEPTION ACHIEVED");
                Hero maleHero = femaleHero == hero1 ? hero2 : hero1;

                Bastard bastard = new Bastard(maleHero, femaleHero);
                SubModule.Bastards.Add(bastard);

                femaleHero.IsPregnant = true;
            }
        }

        private void HandleCrueltyDailyTickHero(Hero hero) {
            if (!SubModule.Config.GetValueBool("enableCruelty")) return;


        }
    }
}
