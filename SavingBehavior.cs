using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace Bastards
{
    internal class SavingBehavior : CampaignBehaviorBase
    {
        public BastardsManager BastardsManager;

        public override void RegisterEvents()
        {
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(
            () =>
                {
                    this.SetPregnancies();
                }
            ));
        }

        public override void SyncData(IDataStore dataStore)
        {
            if (dataStore.IsSaving)
            {
                this.BastardsManager = SubModule.BastardsManagerInstance;
                dataStore.SyncData("BastardsManager", ref BastardsManager);
            }
            if (dataStore.IsLoading)
            {
                dataStore.SyncData("BastardsManager", ref BastardsManager);
                SubModule.BastardsManagerInstance = this.BastardsManager;
                this.SetPregnancies();
                // Bastards list save fix
                try
                {
                    if (SubModule.BastardsManagerInstance.bastards.Any()) { }
                }
                catch (Exception ex)
                {
                    List<BastardsToBeBornDataHolder> currBastardsToBeBorn = SubModule.BastardsManagerInstance.bastardsToBeBorn.ToList();
                    SubModule.BastardsManagerInstance = new BastardsManager(currBastardsToBeBorn);
                }
            }
        }

        private void SetPregnancies()
        {
            foreach (BastardsToBeBornDataHolder data in SubModule.BastardsManagerInstance.bastardsToBeBorn.ToList())
            {
                data.mother.IsPregnant = true;
            }
        }
    }
    public class CustomSaveDefiner : SaveableTypeDefiner
    {
        // use a big number and ensure that no other mod is using a close range
        public CustomSaveDefiner() : base(822997416) { }

        protected override void DefineClassTypes()
        {
            // The Id's here are local and will be related to the Id passed to the constructor
            //AddClassDefinition(typeof(CustomMapNotification), 1);
            //AddStructDefinition(typeof(ExampleStruct), 2);
            //AddClassDefinition(typeof(ExampleNested), 3);
            //AddStructDefinition(typeof(NestedStruct), 4);
            AddClassDefinition(typeof(BastardsManager), 1);
            AddClassDefinition(typeof(BastardsToBeBornDataHolder), 2);
        }

        protected override void DefineContainerDefinitions()
        {
            //ConstructContainerDefinition(typeof(List<CustomMapNotification>));
            // Both of these are necessary: order isn't important
            //ConstructContainerDefinition(typeof(List<NestedStruct>));
            //ConstructContainerDefinition(typeof(Dictionary<string, List<NestedStruct>>));
            ConstructContainerDefinition(typeof(List<BastardsToBeBornDataHolder>));
            //ConstructContainerDefinition(typeof(List<Hero>));
        }
    }
}
