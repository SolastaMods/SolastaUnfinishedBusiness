using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionMagicAffinityBuilder : DefinitionBuilder<FeatureDefinitionMagicAffinity>
    {
        // TODO this is not yet complete, also I'm unsure the current groupings are the best set.
        public FeatureDefinitionMagicAffinityBuilder(string name, string guid, GuiPresentation guiPresentation)
            : base(name, guid)
        {
            Definition.SetGuiPresentation(guiPresentation);
        }

        public FeatureDefinitionMagicAffinityBuilder(FeatureDefinitionMagicAffinity original, string name, string guid, GuiPresentation guiPresentation)
            : base(original, name, guid)
        {
            Definition.SetGuiPresentation(guiPresentation);
        }

        public FeatureDefinitionMagicAffinityBuilder(string name, string guid)
            : base(name, guid)
        {
        }

        public FeatureDefinitionMagicAffinityBuilder(string name, Guid namespaceGuid)
            : base(name, namespaceGuid)
        {
        }

        public FeatureDefinitionMagicAffinityBuilder(FeatureDefinitionMagicAffinity original, string name, string guid)
            : base(original, name, guid)
        {
        }

        public FeatureDefinitionMagicAffinityBuilder(FeatureDefinitionMagicAffinity original, string name, Guid namespaceGuid)
            : base(original, name, namespaceGuid)
        {
        }

        public static FeatureDefinitionMagicAffinityBuilder Create(string name, Guid namespaceGuid)
        {
            return new FeatureDefinitionMagicAffinityBuilder(name, namespaceGuid);
        }

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

        public FeatureDefinitionMagicAffinityBuilder SetWarList(IEnumerable<string> spellNames, int levelBonus)
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
