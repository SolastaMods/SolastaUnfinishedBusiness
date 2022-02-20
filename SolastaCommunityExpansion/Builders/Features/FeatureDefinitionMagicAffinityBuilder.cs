using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionMagicAffinityBuilder : FeatureDefinitionAffinityBuilder<FeatureDefinitionMagicAffinity, FeatureDefinitionMagicAffinityBuilder>
    {
        #region Constructors
        protected FeatureDefinitionMagicAffinityBuilder(string name, string guid)
            : base(name, guid)
        {
        }

        protected FeatureDefinitionMagicAffinityBuilder(string name, Guid namespaceGuid)
            : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionMagicAffinityBuilder(FeatureDefinitionMagicAffinity original, string name, string guid)
            : base(original, name, guid)
        {
        }

        protected FeatureDefinitionMagicAffinityBuilder(FeatureDefinitionMagicAffinity original, string name, Guid namespaceGuid)
            : base(original, name, namespaceGuid)
        {
        }
        #endregion

        #region Factory methods
        public static FeatureDefinitionMagicAffinityBuilder Create(string name, string guid)
        {
            return new FeatureDefinitionMagicAffinityBuilder(name, guid);
        }

        public static FeatureDefinitionMagicAffinityBuilder Create(string name, Guid namespaceGuid)
        {
            return new FeatureDefinitionMagicAffinityBuilder(name, namespaceGuid);
        }

        public static FeatureDefinitionMagicAffinityBuilder Create(FeatureDefinitionMagicAffinity original, string name, string guid)
        {
            return new FeatureDefinitionMagicAffinityBuilder(original, name, guid);
        }

        public static FeatureDefinitionMagicAffinityBuilder Create(FeatureDefinitionMagicAffinity original, string name, Guid namespaceGuid)
        {
            return new FeatureDefinitionMagicAffinityBuilder(original, name, namespaceGuid);
        }
        #endregion

        public FeatureDefinitionMagicAffinityBuilder SetConcentrationModifiers(RuleDefinitions.ConcentrationAffinity concentrationAffinity,
               int threshold)
        {
            Definition.SetConcentrationAffinity(concentrationAffinity);
            if (threshold > 0)
            {
                Definition.SetOverConcentrationThreshold(threshold);
            }
            return this;
        }

        public FeatureDefinitionMagicAffinityBuilder SetHandsFullCastingModifiers(bool weapon, bool weaponOrShield, bool weaponAsFocus)
        {
            Definition.SetSomaticWithWeaponOrShield(weaponOrShield);
            Definition.SetSomaticWithWeapon(weapon);
            Definition.SetCanUseProficientWeaponAsFocus(weaponAsFocus);
            return this;
        }

        public FeatureDefinitionMagicAffinityBuilder SetCastingModifiers(int attackModifier, int dcModifier, bool noProximityPenalty, bool cantripRetribution, bool halfDamageCantrips)
        {
            Definition.SetRangeSpellNoProximityPenalty(noProximityPenalty);
            Definition.SetSpellAttackModifier(attackModifier);
            Definition.SetSaveDCModifier(dcModifier);
            Definition.SetCantripRetribution(cantripRetribution);
            Definition.SetForceHalfDamageOnCantrips(halfDamageCantrips);
            return this;
        }

        public FeatureDefinitionMagicAffinityBuilder SetWarList(int levelBonus, params string[] spellNames)
        {
            return SetWarList(levelBonus, spellNames.AsEnumerable());
        }

        public FeatureDefinitionMagicAffinityBuilder SetWarList(int levelBonus, IEnumerable<string> spellNames)
        {
            Definition.SetUsesWarList(true);
            Definition.SetWarListSlotBonus(levelBonus);
            Definition.WarListSpells.AddRange(spellNames);
            return this;
        }

        public FeatureDefinitionMagicAffinityBuilder SetSpellLearnAndPrepModifiers(
            float scribeDurationMultiplier, float scribeCostMultiplier,
            int additionalScribedSpells, RuleDefinitions.AdvantageType scribeAdvantage, RuleDefinitions.PreparedSpellsModifier preparedModifier)
        {
            Definition.SetScribeCostMultiplier(scribeCostMultiplier);
            Definition.SetScribeDurationMultiplier(scribeDurationMultiplier);
            Definition.SetAdditionalScribedSpells(additionalScribedSpells);
            Definition.SetScribeAdvantageType(scribeAdvantage);
            Definition.SetPreparedSpellModifier(preparedModifier);
            return this;
        }

        public FeatureDefinitionMagicAffinityBuilder SetRitualCasting(RuleDefinitions.RitualCasting ritualCasting)
        {
            Definition.SetRitualCasting(ritualCasting);
            return this;
        }
    }
}
