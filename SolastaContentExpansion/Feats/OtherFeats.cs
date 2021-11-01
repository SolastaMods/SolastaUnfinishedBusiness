using SolastaContentExpansion.Features;
using SolastaModApi;
using System;
using System.Collections.Generic;

namespace SolastaContentExpansion.Feats
{
    class OtherFeats
    {
        public static Guid OtherFeatNamespace = new Guid("655e8588-4d6e-42f3-9564-69e7345d5620");

        public static void CreateFeats(List<FeatDefinition> feats)
        {
            // Savage Attacker
            GuiPresentationBuilder savageAttackerPresentation = new GuiPresentationBuilder(
                "Feat/&FeatSavageAttackerDescription",
                "Feat/&FeatSavageAttackerTitle");

            string rerollKey = "Feat/&FeatSavageAttackerReroll";
            FeatureDefinitionDieRollModifier savageAttackDieRoll = BuildDieRollModifier(RuleDefinitions.RollContext.AttackDamageValueRoll,
                1 /* reroll count */, 1 /* reroll min value */, rerollKey, "DieRollModifierFeatSavageAttacker",
                savageAttackerPresentation.Build());
            FeatureDefinitionDieRollModifier savageMagicDieRoll = BuildDieRollModifier(RuleDefinitions.RollContext.MagicDamageValueRoll,
                1 /* reroll count */, 1 /* reroll min value */, rerollKey, "DieRollModifierFeatSavageMagicAttacker",
                savageAttackerPresentation.Build());

            FeatDefinitionBuilder savageAttacker = new FeatDefinitionBuilder("FeatSavageAttacker", GuidHelper.Create(OtherFeatNamespace, "FeatSavageAttacker").ToString(), new List<FeatureDefinition>()
            {
                savageAttackDieRoll,
                savageMagicDieRoll,
            }, savageAttackerPresentation.Build());
            feats.Add(savageAttacker.AddToDB());

            // Tough
            GuiPresentationBuilder toughPresentation = new GuiPresentationBuilder(
                "Feat/&FeatToughDescription",
                "Feat/&FeatToughTitle");

            FeatureDefinitionAttributeModifier toughModifier = BuildAttributeModifier(
                FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive, AttributeDefinitions.HitPointBonusPerLevel,
                2, "AttributeModifierToughFeat", toughPresentation.Build());

            FeatDefinitionBuilder tough = new FeatDefinitionBuilder("FeatTough", GuidHelper.Create(OtherFeatNamespace, "FeatTough").ToString(),
                new List<FeatureDefinition>()
            {
                toughModifier,
            }, toughPresentation.Build());
            feats.Add(tough.AddToDB());

            // War Caster
            GuiPresentationBuilder warCasterPresentation = new GuiPresentationBuilder(
                "Feat/&FeatWarCasterDescription",
                "Feat/&FeatWarCasterTitle");

            FeatureDefinitionMagicAffinity warCasterModifier = BuildMagicAffinityWarCaster("MagicAffinityWarCasterFeat",
                warCasterPresentation.Build());

            FeatDefinitionBuilder warCaster = new FeatDefinitionBuilder("FeatWarCaster", GuidHelper.Create(OtherFeatNamespace, "FeatWarCaster").ToString(),
                new List<FeatureDefinition>()
            {
                warCasterModifier,
            }, warCasterPresentation.Build());
            feats.Add(warCaster.AddToDB());
        }

        private static FeatureDefinitionMagicAffinity BuildMagicAffinityWarCaster(string name, GuiPresentation guiPresentation)
        {
            FeatureDefinitionMagicAffinityBuilder builder = new FeatureDefinitionMagicAffinityBuilder(name, GuidHelper.Create(OtherFeatNamespace, name).ToString(),
                guiPresentation).SetCastingModifiers(2, 0, true, false, false).SetConcentrationModifiers(RuleDefinitions.ConcentrationAffinity.Advantage, 0).SetHandsFullCastingModifiers(true, true, true);
            return builder.AddToDB();
        }

        private static FeatureDefinitionDieRollModifier BuildDieRollModifier(RuleDefinitions.RollContext context, int rerollCount, int minRerollValue, string consoleLocalizationKey, string name, GuiPresentation guiPresentation)
        {
            FeatureDefinitionDieRollModifierBuilder builder = new FeatureDefinitionDieRollModifierBuilder(name, GuidHelper.Create(OtherFeatNamespace, name).ToString(),
                context, rerollCount, minRerollValue, consoleLocalizationKey, guiPresentation);
            return builder.AddToDB();
        }

        public static FeatureDefinitionAttributeModifier BuildAttributeModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation modifierType,
            string attribute, int amount, string name, GuiPresentation guiPresentation)
        {
            FeatureDefinitionAttributeModifierBuilder builder = new FeatureDefinitionAttributeModifierBuilder(name, GuidHelper.Create(OtherFeatNamespace, name).ToString(),
                modifierType, attribute, amount, guiPresentation);
            return builder.AddToDB();
        }
    }
}
