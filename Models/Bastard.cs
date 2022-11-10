using BastardChildren.StaticUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
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

            double minDays = SubModule.Config.GetValueDouble("minDaysUntilBirth");
            double maxDays = SubModule.Config.GetValueDouble("maxDaysUntilBirth");

            float daysUntilBirth = (float)(minDays + (maxDays - minDays) * SubModule.Random.NextDouble());
            this.birthTimeInMilliseconds = CampaignTime.DaysFromNow(daysUntilBirth).ToMilliseconds;
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

        private void SetBastardGuardian(Hero guardian, bool doConsequence=false) {
            if (hero == null) return;

            Clan guardianClan = guardian.Clan;
            if (guardianClan != null) {
                hero.Clan = guardianClan;
                if (doConsequence)
                    DoConsequence(guardian);
            }
            else {
                // Baby disappears, sold to orphanage or whatever
                KillCharacterAction.ApplyByRemove(hero);
                SubModule.Bastards.Remove(this);
            }
        }

        private void DoConsequence(Hero guardian) {
            if (hero == null) return;

            if (!SubModule.Config.GetValueBool("enableConsequences")) return;

            Hero guardianSpouse = guardian.Spouse;

            if (guardianSpouse != null) {
                // Secret only works because guardian is female, if the player is female sending a bastard the spouse obviously knows
                if (guardian.IsFemale &&
                    SubModule.Random.Next(1, 100) <= SubModule.Config.GetValueInt("percentChanceKeptSecret")) {
                    Legitimize();
                    Utils.PrintToDisplayBox("Not Caught", hero.Name + " is thought to be legitimate!");
                    return;
                }
                else {
                    Utils.ModifyPlayerRelations(guardianSpouse, SubModule.Config.GetValueInt("spouseRelationLoss"));
                    Utils.PrintToDisplayBox("Caught!", guardianSpouse.Name + " is not happy with your actions.");
                }
            }

            Hero guardianClanLeader = guardian.Clan.Leader;

            if (guardianClanLeader == guardian) return;

            Utils.ModifyPlayerRelations(guardianClanLeader, SubModule.Config.GetValueInt("clanLeaderRelationLoss"));
            Utils.PrintToDisplayBox("Caught!", guardianClanLeader.Name + " is not happy with your actions.");
        }

        private void Birth() {
            // Set mother to not pregnant
            mother.IsPregnant = false;

            // Variables for campaign event
            List<Hero> aliveChildren = new();
            int stillbirthNum = 0;

            // Stillbirth chance
            if (SubModule.Random.Next(1, 100) <= SubModule.Config.GetValueInt("percentChanceOfStillbirth")) {
                Utils.PrintToMessages(mother.Name + " has delivered stillborn.", 255, 100, 100);
                SubModule.Bastards.Remove(this);
                stillbirthNum++;
            } else {
                // Birth hero
                try {
                    hero = HeroCreator.DeliverOffSpring(mother, father, SubModule.Random.Next(0, 2) >= 1 ? true : false);
                    aliveChildren.Add(hero);
                }
                catch (Exception thisDamnBug) {
                    Utils.PrintToDisplayBox("BASTARD CHILDREN ERROR", mother.Name + " HAS DELIVERED DEBUG STILLBIRTH. PLEASE SEND A SCREENSHOT OF THIS TO WINDWHISTLE ON THE NEXUS PAGE. " +
                        "FEMALE: " + Hero.MainHero.IsFemale.ToString() + ", FEMALE CULTURE: " + mother.Culture.ToString() + ", " +
                        "MALE CULTURE: " + father.Culture.ToString() + ", *!* EXCEPTION *!*: " + thisDamnBug.Message);
                    SubModule.Bastards.Remove(this);
                    stillbirthNum++;
                }
            }

            // Dispatch campaign event
            CampaignEventDispatcher.Instance.OnGivenBirth(mother, aliveChildren, stillbirthNum);


            // Mother dying in labor chance
            if (SubModule.Random.Next(1, 100) <= SubModule.Config.GetValueInt("percentChanceOfLaborDeath")) {
                KillCharacterAction.ApplyInLabor(mother);
            }

            // End function if birthed after marriage between mother and father OR if stillborn
            if (mother.Spouse == father || hero == null) return;

            // Set bastard as lord occupation
            hero.SetNewOccupation(Occupation.Lord);

            // Bastard clan to send off to
            Hero bastardGuardian = mother;

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
                        bastardGuardian = Hero.MainHero;
                        SetBastardGuardian(bastardGuardian);
                    },
                    // No, I will not take them into my clan
                    () => {
                        bastardGuardian = otherHero;
                        SetBastardGuardian(bastardGuardian, true);
                    },
                ""), true);
            }
            // If the bastard is not the current player character's child
            else {
                SetBastardGuardian(bastardGuardian);
            }

            // Set GoT surnames
            if (SubModule.Config.GetValueBool("enableSurnames")) {
                string[] names = Utils.GetBastardName(hero);
                hero.SetName(new TextObject(names[1]), new TextObject(names[0]));
            }
        }
    }
}
