using System;
using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomFeatureDefinitions;
using SolastaModApi.Infrastructure;
using static FeatureDefinitionAttributeModifier;
using static RuleDefinitions.RollContext;

namespace SolastaCommunityExpansion.Feats
{
    internal static class OtherFeats
    {
        public static readonly Guid OtherFeatNamespace = new("655e8588-4d6e-42f3-9564-69e7345d5620");

        public static void CreateFeats(List<FeatDefinition> feats)
        {
            // Savage Attacker
            var savageAttacker = FeatDefinitionBuilder
                .Create("FeatSavageAttacker", OtherFeatNamespace)
                .SetFeatures(
                    BuildDieRollModifier("DieRollModifierFeatSavageAttacker",
                        AttackDamageValueRoll, 1 /* reroll count */, 1 /* reroll min value */ ),
                    BuildDieRollModifier("DieRollModifierFeatSavageMagicAttacker",
                        MagicDamageValueRoll, 1 /* reroll count */, 1 /* reroll min value */ ))
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            // Improved critical
            var improvedCritical = FeatDefinitionBuilder
                .Create("FeatImprovedCritical", OtherFeatNamespace)
                .SetGuiPresentation(Category.Feat)
                .SetFeatures(
                    FeatureDefinitionAttributeModifierBuilder
                        .Create("AttributeModifierImprovedCriticalFeat", OtherFeatNamespace)
                        .SetGuiPresentation("FeatImprovedCritical", Category.Feat)
                        .SetModifier(AttributeModifierOperation.Set, AttributeDefinitions.CriticalThreshold, 19)
                        .AddToDB())
                .AddToDB();

            // Tough
            var tough = FeatDefinitionBuilder
                .Create("FeatTough", OtherFeatNamespace)
                .SetFeatures(
                    FeatureDefinitionAttributeModifierBuilder
                        .Create("AttributeModifierToughFeat", OtherFeatNamespace)
                        .SetGuiPresentation("FeatTough", Category.Feat)
                        .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.HitPointBonusPerLevel, 2)
                        .AddToDB())
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            // War Caster
            var warCaster = FeatDefinitionBuilder
                .Create("FeatWarCaster", OtherFeatNamespace)
                .SetFeatures(
                    FeatureDefinitionMagicAffinityBuilder
                        .Create("MagicAffinityWarCasterFeat", OtherFeatNamespace)
                        .SetGuiPresentation("FeatWarCaster", Category.Feat)
                        .SetCastingModifiers(2, 0, true, false, false)
                        .SetConcentrationModifiers(RuleDefinitions.ConcentrationAffinity.Advantage, 0)
                        .SetHandsFullCastingModifiers(true, true, true)
                        .AddToDB())
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            // Elemental Adept Acid
            var elementalAdeptAcid = FeatDefinitionBuilder
                .Create("FeatElementalAdeptAcid", OtherFeatNamespace)
                .SetFeatures(
                    FeatureDefinitionIgnoreDamageResistanceBuilder.IgnoreDamageResistanceAcid
                )
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            // Elemental Adept Cold
            var elementalAdeptCold = FeatDefinitionBuilder
                .Create("FeatElementalAdeptCold", OtherFeatNamespace)
                .SetFeatures(
                    FeatureDefinitionIgnoreDamageResistanceBuilder.IgnoreDamageResistanceCold
                )
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            // Elemental Adept Fire
            var elementalAdeptFire = FeatDefinitionBuilder
                .Create("FeatElementalAdeptFire", OtherFeatNamespace)
                .SetFeatures(
                    FeatureDefinitionIgnoreDamageResistanceBuilder.IgnoreDamageResistanceFire
                )
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            // Elemental Adept Lightning
            var elementalAdeptLightning = FeatDefinitionBuilder
                .Create("FeatElementalAdeptLightning", OtherFeatNamespace)
                .SetFeatures(
                    FeatureDefinitionIgnoreDamageResistanceBuilder.IgnoreDamageResistanceAcid
                )
                .SetGuiPresentation(Category.Feat)
                .AddToDB();

            feats.AddRange(
                savageAttacker, 
                tough, 
                warCaster, 
                improvedCritical, 
                elementalAdeptAcid, 
                elementalAdeptCold, 
                elementalAdeptFire, 
                elementalAdeptLightning);
        }

        private static FeatureDefinitionDieRollModifier BuildDieRollModifier(string name,
            RuleDefinitions.RollContext context, int rerollCount, int minRerollValue)
        {
            return FeatureDefinitionDieRollModifierBuilder
                .Create(name, OtherFeatNamespace)
                .SetModifiers(context, rerollCount, minRerollValue, "Feat/&FeatSavageAttackerReroll")
                .SetGuiPresentation("FeatSavageAttacker", Category.Feat)
                .AddToDB();
        }
    }

    internal sealed class FeatureDefinitionIgnoreDamageResistanceBuilder : FeatureDefinitionBuilder<FeatureDefinitionIgnoreDamageResistance, FeatureDefinitionIgnoreDamageResistanceBuilder>
    {
        private const string IgnoreDamageResistanceAcidName = "IgnoreDamageResistanceAcid";
        private const string IgnoreDamageResistanceAcidGuid = "a324917bd3d3402bbd8b5b616b183242";

        private const string IgnoreDamageResistanceColdName = "IgnoreDamageResistanceAcid";
        private const string IgnoreDamageResistanceColdGuid = "3872825980854ec28c7cef6dddc4c669";

        private const string IgnoreDamageResistanceFireName = "IgnoreDamageResistanceAcid";
        private const string IgnoreDamageResistanceFireGuid = "b778c242d70c42439a68d13832a7413f";

        private const string IgnoreDamageResistanceLightningName = "IgnoreDamageResistanceLightning";
        private const string IgnoreDamageResistanceLightningGuid = "a9dd2c782b474d83bb8e38921d270ee3";

        private FeatureDefinitionIgnoreDamageResistanceBuilder(string name, string guid, params string[] damageTypes) : base(name, guid)
        {
            Definition.SetGuiPresentation("FeatElementalAdept" + String.Join("_", damageTypes), Category.Feat);
            Definition.DamageTypes = damageTypes.ToList();
        }

        private static FeatureDefinition CreateAndAddToDB(string name, string guid, params string[] damageTypes)
        {
            return new FeatureDefinitionIgnoreDamageResistanceBuilder(name, guid, damageTypes).AddToDB();
        }

        internal static readonly FeatureDefinition IgnoreDamageResistanceAcid =
            CreateAndAddToDB(IgnoreDamageResistanceAcidName, IgnoreDamageResistanceAcidGuid, RuleDefinitions.DamageTypeAcid);

        internal static readonly FeatureDefinition IgnoreDamageResistanceCold =
            CreateAndAddToDB(IgnoreDamageResistanceColdName, IgnoreDamageResistanceColdGuid, RuleDefinitions.DamageTypeCold);

        internal static readonly FeatureDefinition IgnoreDamageResistanceFire =
            CreateAndAddToDB(IgnoreDamageResistanceFireName, IgnoreDamageResistanceFireGuid, RuleDefinitions.DamageTypeFire);

        internal static readonly FeatureDefinition IgnoreDamageResistanceLightning =
            CreateAndAddToDB(IgnoreDamageResistanceLightningName, IgnoreDamageResistanceLightningGuid, RuleDefinitions.DamageTypeLightning);
    }
}
