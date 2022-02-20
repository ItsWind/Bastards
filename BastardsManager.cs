using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace Bastards
{
    public class BastardsManager
    {
        [SaveableField(1)]
        public List<BastardsToBeBornDataHolder> bastardsToBeBorn = new();
        [SaveableField(2)]
        public List<Hero> bastards = new();

        // Constructor for save/load old version compat
        public BastardsManager()
        {
        }
        public BastardsManager(List<BastardsToBeBornDataHolder> bastardsToBeBorn)
        {
            this.bastardsToBeBorn = bastardsToBeBorn;
        }

        public void AddBastardToBeBorn(Hero father, Hero mother, CampaignTime birthTime)
        {
            bastardsToBeBorn.Add(new BastardsToBeBornDataHolder(father, mother, birthTime));
        }
        public void RemoveBastardToBeBorn(BastardsToBeBornDataHolder data)
        {
            bastardsToBeBorn.Remove(data);
        }

        public void AddBastard(Hero bastard)
        {
            if (!bastards.Contains(bastard))
            {
                bastards.Add(bastard);
            }
        }
        public void RemoveBastard(Hero bastard)
        {
            if (bastards.Contains(bastard))
            {
                bastards.Remove(bastard);
            }
        }
    }
}
