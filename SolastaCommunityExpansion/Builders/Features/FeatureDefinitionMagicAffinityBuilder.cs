using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionMagicAffinityBuilder : FeatureDefinitionAffinityBuilder<FeatureDefinitionMagicAffinity, FeatureDefinitionMagicAffinityBuilder>
    {
        #region Constructors
        protected FeatureDefinitionMagicAffinityBuilder(FeatureDefinitionMagicAffinity original) : base(original)
        {
        }

        protected FeatureDefinitionMagicAffinityBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionMagicAffinityBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected FeatureDefinitionMagicAffinityBuilder(string name, bool createGuiPresentation = true) : base(name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionMagicAffinityBuilder(FeatureDefinitionMagicAffinity original, string name, bool createGuiPresentation = true) : base(original, name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionMagicAffinityBuilder(FeatureDefinitionMagicAffinity original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FeatureDefinitionMagicAffinityBuilder(FeatureDefinitionMagicAffinity original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion

        public FeatureDefinitionMagicAffinityBuilder SetConcentrationModifiers(RuleDefinitions.ConcentrationAffinity concentrationAffinity, int threshold)
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

        public FeatureDefinitionMagicAffinityBuilder SetWarList(int levelBonus, params SpellDefinition[] spells)
        {
            return SetWarList(levelBonus, spells.AsEnumerable());
        }

        public FeatureDefinitionMagicAffinityBuilder SetWarList(int levelBonus, IEnumerable<SpellDefinition> spells)
        {
            Definition.SetUsesWarList(true);
            Definition.SetWarListSlotBonus(levelBonus);
            Definition.WarListSpells.AddRange(spells.Select(s => s.Name));
            Definition.WarListSpells.Sort();
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


        public FeatureDefinitionMagicAffinityBuilder SetExtendedSpellList(SpellListDefinition spellListDefinition)
        {
            Definition.SetExtendedSpellList(spellListDefinition);
            return this;
        }
    }
}
