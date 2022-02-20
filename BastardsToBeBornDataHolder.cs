using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace Bastards
{
    public class BastardsToBeBornDataHolder
    {
        [SaveableField(1)]
        public Hero father;
        [SaveableField(2)]
        public Hero mother;
        [SaveableField(3)]
        public CampaignTime birthTime;
        public BastardsToBeBornDataHolder(Hero father, Hero mother, CampaignTime birthTime)
        {
            this.father = father;
            this.mother = mother;
            this.birthTime = birthTime;
        }
    }
}
