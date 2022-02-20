using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Bastards
{
    internal class BirthBastardTickBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(() => this.CheckAllBastardBirths()));
        }

        public override void SyncData(IDataStore dataStore)
        {
            // empty
        }

        private void CheckAllBastardBirths()
        {
            // Daily tick bastard birth checks
            foreach (BastardsToBeBornDataHolder bastardData in SubModule.BastardsManagerInstance.bastardsToBeBorn.ToList())
            {
                if (CampaignTime.Now.ToDays >= bastardData.birthTime.ToDays)
                {
                    BirthBastard(bastardData);
                }
            }
        }

        private void BirthBastard(BastardsToBeBornDataHolder data)
        {
            if (data.mother.IsAlive)
            {
                // Mother no longer pregnant
                data.mother.IsPregnant = false;

                // Set mother and father occupations to avoid bug.
                Occupation oldMotherOccupation = data.mother.Occupation;
                Occupation oldFatherOccupation = data.father.Occupation;
                if (oldMotherOccupation != Occupation.Lord) { data.mother.SetNewOccupation(Occupation.Lord); }
                if (oldFatherOccupation != Occupation.Lord) { data.father.SetNewOccupation(Occupation.Lord); }

                // Get bastard hero
                Hero babyBastard = HeroCreator.DeliverOffSpring(data.mother, data.father, Convert.ToBoolean(SubModule.rand.Next(1, 101) % 2), null);//Utils.GetNewBastardHero(data.father, data.mother);
                
                // Set mother and father occupations back to original.
                data.mother.SetNewOccupation(oldMotherOccupation);
                data.father.SetNewOccupation(oldFatherOccupation);

                // Check if married after bastard conceived
                if (!(babyBastard.Father.Spouse != null && babyBastard.Father.Spouse.Equals(babyBastard.Mother)))
                {
                    // Get other hero clan, if no clan then set to player clan
                    Clan theirClan = Hero.MainHero.Clan;
                    if (data.mother.Clan != null && data.father.Clan != null)
                    {
                        if (Hero.MainHero.Clan.Equals(data.father.Clan))
                            theirClan = data.mother.Clan;
                        else if (Hero.MainHero.Clan.Equals(data.mother.Clan))
                            theirClan = data.father.Clan;
                    }

                    // Notify player with inquiry box
                    TextObject textObject;
                    if (Hero.MainHero.Equals(data.mother))
                    {
                        textObject = new TextObject("{=*}You have given birth to {BASTARDNAME}. Which clan did you want to send them off to?", null);
                    }
                    else
                    {
                        textObject = new TextObject("{=*}{BASTARDMOTHERNAME} has given birth to {BASTARDNAME}. Which clan did you want to send them off to?", null);
                        textObject.SetTextVariable("BASTARDMOTHERNAME", data.mother.Name);
                    }
                    textObject.SetTextVariable("BASTARDNAME", babyBastard.Name);
                    InformationManager.ShowInquiry(new InquiryData(new TextObject("{=*}Bastard Born", null).ToString(), textObject.ToString(), true, true, "My Clan", "Their Clan",
                        //My Clan
                        () => { babyBastard.Clan = Hero.MainHero.Clan; },
                        //Their Clan
                        () => { babyBastard.Clan = theirClan; },
                    ""), true);

                    if (SubModule.configEnableSurnames)
                    {
                        string[] names = Utils.GetBastardName(babyBastard);
                        babyBastard.SetName(new TextObject(names[1]), new TextObject(names[0]));
                    }

                    babyBastard.IsNoble = false;

                    SubModule.BastardsManagerInstance.AddBastard(babyBastard);
                }
            }
            SubModule.BastardsManagerInstance.RemoveBastardToBeBorn(data);
        }
    }
}
