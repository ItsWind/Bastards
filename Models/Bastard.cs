using BastardChildren.StaticUtils;
using HarmonyLib;
using MCM.Abstractions.Base.Global;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace BastardChildren.Models
{
    public class Bastard
    {
        [SaveableField(1)]
        public Hero? hero = null;
        [SaveableField(2)]
        public Hero father;
        [SaveableField(3)]
        public Hero mother;
        [SaveableField(4)]
        public double birthTimeInMilliseconds;

        public Bastard(Hero heroFather, Hero heroMother)
        {
            this.father = heroFather;
            this.mother = heroMother;

            double minYears = GlobalSettings<MCMConfig>.Instance.MinimumYearsUntilBirth;
            double maxYears = GlobalSettings<MCMConfig>.Instance.MaximumYearsUntilBirth;

            float yearsUntilBirth = (float)(minYears + (maxYears - minYears) * SubModule.Random.NextDouble());
            birthTimeInMilliseconds = CampaignTime.YearsFromNow(yearsUntilBirth).ToMilliseconds;

            SubModule.Bastards.Add(this);
            heroMother.IsPregnant = true;
        }

        // Tick is done on hourly tick
        public void Tick()
        {
            // If bastard is unborn
            if (hero == null) {
                // Check if mother is dead
                if (!mother.IsAlive) {
                    mother.IsPregnant = false;
                    SubModule.Bastards.Remove(this);
                    return;
                }

                // Keep making the mother pregnant, resets on some kind of tick event
                mother.IsPregnant = true;

                // If it's time to pop
                if (CampaignTime.Now.ToMilliseconds >= birthTimeInMilliseconds)
                    Birth();
            }
        }

        public void Legitimize() {
            if (hero == null) return;

            // Set name without surname
            string moddedName = hero.FirstName.ToString();
            if (moddedName.Contains(" ")) {
                int indexOfSpace = moddedName.IndexOf(" ");
                moddedName = moddedName.Substring(0, indexOfSpace);
            }
            hero.SetName(new TextObject(moddedName), new TextObject(moddedName));

            // Remove from bastards list
            SubModule.Bastards.Remove(this);
        }

        private void SetBastardGuardian(Hero guardian, Hero? consequenceHero=null) {
            if (hero == null) return;

            Clan guardianClan = guardian.Clan;
            if (guardianClan != null) {
                hero.Clan = guardianClan;

                // See if caught or if child is considered legit
                if (guardian.Spouse != null) {
                    // Secret only works because guardian is female, if the player is female sending a bastard the spouse obviously knows
                    if (guardian.IsFemale && Utils.PercentChanceCheck(GlobalSettings<MCMConfig>.Instance.PercentChanceKeptSecret)) {
                        Legitimize();
                        return;
                    }
                }

                if (consequenceHero != null)
                    DoConsequence(guardian, consequenceHero);
            }
            else {
                // Baby disappears, sold to orphanage or whatever
                Disappear();
            }
        }

        private void DoConsequence(Hero guardian, Hero consequenceHero) {
            if (guardian.Clan == Hero.MainHero.Clan) return;

            if (!GlobalSettings<MCMConfig>.Instance.ConsequencesEnabled) return;

            Utils.ModifyHeroRelations(consequenceHero, guardian.Spouse, GlobalSettings<MCMConfig>.Instance.SpouseRelationLoss);

            if (guardian.Clan.Leader == guardian) return;

            Utils.ModifyHeroRelations(consequenceHero, guardian.Clan.Leader, GlobalSettings<MCMConfig>.Instance.ClanLeaderRelationLoss);
        }

        public void Disappear() {
            if (hero == null)
                return;

            hero.Clan = null;
            hero.Father = null;
            hero.Mother = null;

            List<Hero> fatherChildren = (List<Hero>)AccessTools.Field(typeof(Hero), "_children").GetValue(father);
            List<Hero> motherChildren = (List<Hero>)AccessTools.Field(typeof(Hero), "_children").GetValue(mother);
            List<Hero> aliveHeroes = (List<Hero>)AccessTools.Field(typeof(CampaignObjectManager), "_aliveHeroes").GetValue(Campaign.Current.CampaignObjectManager);
            aliveHeroes.Remove(hero);
            fatherChildren.Remove(hero);
            motherChildren.Remove(hero);
            SubModule.Bastards.Remove(this);
        }

        public void Birth() {
            // Set mother to not pregnant
            mother.IsPregnant = false;

            // Variables for campaign event
            List<Hero> aliveChildren = new();
            int stillbirthNum = 0;

            // Stillbirth chance
            if (Utils.PercentChanceCheck(GlobalSettings<MCMConfig>.Instance.StillbirthChance)) {
                Utils.PrintToMessages(mother.Name + " has delivered stillborn.", 255, 100, 100);
                SubModule.Bastards.Remove(this);
                stillbirthNum++;
            } else {
                // Birth hero
                hero = HeroCreator.DeliverOffSpring(mother, father, SubModule.Random.Next(0, 2) >= 1 ? true : false);
                aliveChildren.Add(hero);
            }

            // Dispatch campaign event
            CampaignEventDispatcher.Instance.OnGivenBirth(mother, aliveChildren, stillbirthNum);


            // Mother dying in labor chance
            if (Utils.PercentChanceCheck(GlobalSettings<MCMConfig>.Instance.LaborDeathChance)) {
                KillCharacterAction.ApplyInLabor(mother);
            }

            // If born when mother and father are married
            if (mother.Spouse == father) {
                Legitimize();
                return;
            }

            // If bastard was stillborn
            if (hero == null) return;

            // Set bastard as lord occupation
            hero.SetNewOccupation(Occupation.Lord);

            // Check if bastard is players child
            if (Hero.MainHero == father || Hero.MainHero == mother) {
                // Get other hero
                Hero otherHero = Hero.MainHero == father ? mother : father;

                // Get clan inquiry text
                TextObject textObject;
                if (Hero.MainHero == mother)
                    textObject = new TextObject("{=*}You have given birth to {BASTARDNAME}. Will you raise them as your own and take them into your clan?", null);
                else {
                    textObject = new TextObject("{=*}{BASTARDMOTHERNAME} has given birth to {BASTARDNAME}. Will you raise them as your own and take them into your clan?", null);
                    textObject.SetTextVariable("BASTARDMOTHERNAME", mother.Name);
                }
                textObject.SetTextVariable("BASTARDNAME", hero.Name);

                // Perform clan inquiry
                InformationManager.ShowInquiry(new InquiryData(new TextObject("{=*}Bastard Born", null).ToString(), textObject.ToString(), true, true, "Yes", "No",
                    // Yes, take them into my clan
                    () => {
                        SetBastardGuardian(Hero.MainHero);
                    },
                    // No, I will not take them into my clan
                    () => {
                        SetBastardGuardian(otherHero, Hero.MainHero);
                    },
                ""), true);
            }
            // If the bastard is not the current player character's child
            else {
                SetBastardGuardian(mother, father);
            }

            // Set GoT surnames
            if (GlobalSettings<MCMConfig>.Instance.SurnamesEnabled) {
                string[] names = Utils.GetBastardName(hero);
                hero.SetName(new TextObject(names[1]), new TextObject(names[0]));
            }
        }
    }
}
