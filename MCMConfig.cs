using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;

namespace BastardChildren {
    internal sealed class MCMConfig : AttributeGlobalSettings<MCMConfig> {
        public override string Id => "BastardChildren";
        public override string DisplayName => "Bastard Children";
        public override string FolderName => "BastardChildren";
        public override string FormatType => "xml";

        // AI

        [SettingPropertyBool("Toggle AI Bastards", Order = 1, HintText = "Enable/disable AI being able to birth bastards with each other.", RequireRestart = false)]
        [SettingPropertyGroup("AI")]
        public bool AIBastardsEnabled { get; set; } = true;

        [SettingPropertyInteger("AI Conception Base Chance", -100, 100, Order = 2, HintText = "This is for the AI to CONSIDER making a bastard. This is trait affected if trait affected relations is turned on.", RequireRestart = false)]
        [SettingPropertyGroup("AI")]
        public int AIBaseConceptionChance { get; set; } = 5;

        // BIRTH

        [SettingPropertyFloatingInteger("Minimum Years Until Birth", 0f, 1f, Order = 1, HintText = "", RequireRestart = false)]
        [SettingPropertyGroup("Birth")]
        public float MinimumYearsUntilBirth { get; set; } = 0.7f;

        [SettingPropertyFloatingInteger("Maximum Years Until Birth", 0f, 1f, Order = 2, HintText = "", RequireRestart = false)]
        [SettingPropertyGroup("Birth")]
        public float MaximumYearsUntilBirth { get; set; } = 0.82f;

        [SettingPropertyInteger("Conception Percent Chance", 0, 100, Order = 3, HintText = "", RequireRestart = false)]
        [SettingPropertyGroup("Birth")]
        public int ConceptionChance { get; set; } = 25;

        [SettingPropertyInteger("Stillbirth Percent Chance", 0, 100, Order = 4, HintText = "", RequireRestart = false)]
        [SettingPropertyGroup("Birth")]
        public int StillbirthChance { get; set; } = 10;

        [SettingPropertyInteger("Labor Death Percent Chance", 0, 100, Order = 5, HintText = "", RequireRestart = false)]
        [SettingPropertyGroup("Birth")]
        public int LaborDeathChance { get; set; } = 15;

        // REQUIREMENTS

        [SettingPropertyBool("Enable Incest", Order = 1, HintText = "uhhhhhhhh", RequireRestart = false)]
        [SettingPropertyGroup("Requirements")]
        public bool IncestEnabled { get; set; } = false;

        [SettingPropertyInteger("Base Relation Needed", -100, 100, Order = 2, HintText = "", RequireRestart = false)]
        [SettingPropertyGroup("Requirements")]
        public int BaseRelationNeeded { get; set; } = 10;

        [SettingPropertyBool("Enable Trait Affected Relation Needed", Order = 3, HintText = "uhhhhhhhh", RequireRestart = false)]
        [SettingPropertyGroup("Requirements")]
        public bool TraitAffectedRelationEnabled { get; set; } = true;

        [SettingPropertyFloatingInteger("Asked Timer In Days", 0f, 14f, Order = 4, HintText = "", RequireRestart = false)]
        [SettingPropertyGroup("Requirements")]
        public float AskedTimerInDays { get; set; } = 1f;

        // LEGITIMIZATION

        [SettingPropertyFloatingInteger("Influence Cost", 0f, 5000f, Order = 1, HintText = "", RequireRestart = false)]
        [SettingPropertyGroup("Legitimisation")]
        public float LegitimizeInfluenceCost { get; set; } = 150f;

        [SettingPropertyBool("Enable Bastard Heirs Legitimized", Order = 2, HintText = "uhhhhhhhh", RequireRestart = false)]
        [SettingPropertyGroup("Legitimisation")]
        public bool LegitimizeBastardHeirsEnabled { get; set; } = true;

        [SettingPropertyFloatingInteger("Bastard Marriage Value Multiplier", 0f, 1f, Order = 3, HintText = "", RequireRestart = false)]
        [SettingPropertyGroup("Legitimisation")]
        public float BastardMarriageValueMult { get; set; } = 0.75f;

        [SettingPropertyBool("Enable Married Bastard Legitimized", Order = 4, HintText = "uhhhhhhhh", RequireRestart = false)]
        [SettingPropertyGroup("Legitimisation")]
        public bool LegitimizeMarriedBastardsEnabled { get; set; } = true;

        // CONSEQUENCES

        [SettingPropertyBool("Enable Consequences", Order = 1, HintText = "uhhhhhhhh", RequireRestart = false)]
        [SettingPropertyGroup("Consequences")]
        public bool ConsequencesEnabled { get; set; } = true;

        [SettingPropertyInteger("Percent Chance Kept Secret", 0, 100, Order = 2, HintText = "", RequireRestart = false)]
        [SettingPropertyGroup("Consequences")]
        public int PercentChanceKeptSecret { get; set; } = 70;

        [SettingPropertyInteger("Spouse Relation Loss", -100, 100, Order = 3, HintText = "", RequireRestart = false)]
        [SettingPropertyGroup("Consequences")]
        public int SpouseRelationLoss { get; set; } = -60;

        [SettingPropertyInteger("Clan Leader Relation Loss", -100, 100, Order = 4, HintText = "", RequireRestart = false)]
        [SettingPropertyGroup("Consequences")]
        public int ClanLeaderRelationLoss { get; set; } = -40;

        // MISC

        [SettingPropertyBool("Enable Surnames", Order = 1, HintText = "duh duh dUH DUH DUH DUhhhhh DUDUDU DUUUUH", RequireRestart = false)]
        [SettingPropertyGroup("Misc")]
        public bool SurnamesEnabled { get; set; } = true;
    }
}
