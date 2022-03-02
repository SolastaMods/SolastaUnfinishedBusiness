using System.Collections.Generic;

namespace SolastaCommunityExpansion.CustomFeatureDefinitions
{
    public class FeatureDefinitionModifyDamageRollOnDamageType : FeatureDefinition, IDieRollModificationProvider
    {
        private readonly List<string> DamageTypes = new();

        public FeatureDefinitionModifyDamageRollOnDamageType(params string[] damageTypes)
        {
            DamageTypes.AddRange(damageTypes);
        }

        public static DamageForm DamageFormContext { get; set; }

        public bool IsValidContext => DamageFormContext != null && !DamageTypes.Contains(DamageFormContext.DamageType);

        public int MinRollValue => 0;
        public int MaxRollValue => 0;
        public int MinRerollValue => IsValidContext ? 2 : 1;
        public int RerollCount => IsValidContext ? 1 : 0;
        public string RerollLocalizationKey => "Feature/&FeatElementalAdeptReroll";
        public bool ForcedMinimalHalfDamageOnDice => false;
        public RuleDefinitions.RollContext ValidityContext => RuleDefinitions.RollContext.MagicDamageValueRoll;
    }
}
