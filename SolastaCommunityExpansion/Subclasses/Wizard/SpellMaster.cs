using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Features;
using SolastaModApi;
using SolastaModApi.Extensions;
using UnityEngine;

namespace SolastaCommunityExpansion.Subclasses.Wizard
{
    internal class SpellMaster : AbstractSubclass
    {
        private static readonly Guid SubclassNamespace = new("9f322734-1498-4f65-ace5-e6072b1d99be");
        private readonly CharacterSubclassDefinition Subclass;

        #region Spell recovery gui
        private static GuiPresentation _spellRecoveryGui;
        internal static GuiPresentation SpellRecoveryGui
        {
            get
            {
                return _spellRecoveryGui ??= Build();

                static GuiPresentation Build()
                {
                    var spellRecoveryGui = new GuiPresentationBuilder(
                        "Subclass/&MagicAffinitySpellMasterRecoveryTitle",
                        "Subclass/&MagicAffinitySpellMasterRecoveryDescription");
                    spellRecoveryGui.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerWizardArcaneRecovery.GuiPresentation.SpriteReference);

                    return spellRecoveryGui.Build();
                }
            }
        }
        #endregion

        #region Bonus recovery
        private static FeatureDefinitionPower _bonusRecovery;
        internal static FeatureDefinitionPower BonusRecovery => _bonusRecovery ??= BuildSpellFormPower(
                        1 /* usePerRecharge */, RuleDefinitions.UsesDetermination.Fixed, RuleDefinitions.ActivationTime.Rest,
                        1 /* cost */, RuleDefinitions.RechargeRate.LongRest, "PowerSpellMasterBonusRecovery", SpellRecoveryGui);
        #endregion

        internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
        {
            return DatabaseHelper.FeatureDefinitionSubclassChoices.SubclassChoiceWizardArcaneTraditions;
        }
        internal override CharacterSubclassDefinition GetSubclass()
        {
            return Subclass;
        }

        internal SpellMaster()
        {
            // Make Spell Master subclass
            CharacterSubclassDefinitionBuilder spellMaster = new CharacterSubclassDefinitionBuilder("SpellMaster", GuidHelper.Create(SubclassNamespace, "SpellMaster").ToString());
            GuiPresentationBuilder spellPresentation = new GuiPresentationBuilder(
                "Subclass/&TraditionSpellMasterTitle",
                "Subclass/&TraditionSpellMasterDescription");
            spellPresentation.SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.DomainInsight.GuiPresentation.SpriteReference);
            spellMaster.SetGuiPresentation(spellPresentation.Build());

            GuiPresentationBuilder preparedGui = new GuiPresentationBuilder(
                "Subclass/&TraditionSpellMasterPreparedTitle",
                "Subclass/&TraditionSpellMasterPreparedDescription");
            FeatureDefinitionMagicAffinity prepared = PreparedSpellModifier(RuleDefinitions.PreparedSpellsModifier.ProficiencyBonus,
                "MagicAffinitySpellMasterPrepared", preparedGui.Build());
            spellMaster.AddFeatureAtLevel(prepared, 2);

            GuiPresentationBuilder extraKnownGui = new GuiPresentationBuilder(
                "Subclass/&MagicAffinitySpellMasterBonusScribingTitle",
                "Subclass/&MagicAffinitySpellMasterBonusScribingDescription");
            FeatureDefinitionMagicAffinity extraKnown = BuildMagicAffinityScribing(1f, 1f, 1,
                RuleDefinitions.AdvantageType.None, "MagicAffinitySpellMasterKnowledge", extraKnownGui.Build());
            spellMaster.AddFeatureAtLevel(extraKnown, 2);

            spellMaster.AddFeatureAtLevel(BonusRecovery, 2);
            UpdateRecoveryLimited();

            BuildRestActivity(RestDefinitions.RestStage.AfterRest, RuleDefinitions.RestType.ShortRest,
                RestActivityDefinition.ActivityCondition.CanUsePower, "UsePower", BonusRecovery.Name, "ArcaneDepth", SpellRecoveryGui);

            GuiPresentationBuilder spellKnowledgeAffinity = new GuiPresentationBuilder(
                "Subclass/&MagicAffinitySpellMasterScribingTitle",
                "Subclass/&MagicAffinitySpellMasterScribingDescription");
            FeatureDefinitionMagicAffinity knowledgeAffinity = BuildMagicAffinityScribing(0.25f, 0.25f, 0,
                RuleDefinitions.AdvantageType.Advantage, "MagicAffinitySpellMasterScriber", spellKnowledgeAffinity.Build());
            spellMaster.AddFeatureAtLevel(knowledgeAffinity, 6);

            GuiPresentationBuilder bonusCantripsGui = new GuiPresentationBuilder(
                "Subclass/&TraditionSpellMasterBonusCantripsTitle",
                "Subclass/&TraditionSpellMasterBonusCantripsDescription");
            FeatureDefinitionPointPool bonusCantrips = new FeatureDefinitionPointPoolBuilder("TraditionSpellMasterBonusCantrips",
                GuidHelper.Create(SubclassNamespace, "TraditionSpellMasterBonusCantrips").ToString(),
                HeroDefinitions.PointsPoolType.Cantrip, 2, bonusCantripsGui.Build()).OnlyUniqueChoices().AddToDB();
            spellMaster.AddFeatureAtLevel(bonusCantrips, 6);

            GuiPresentationBuilder extraPreparedGui = new GuiPresentationBuilder(
                "Subclass/&TraditionSpellMasterExtraPreparedTitle",
                "Subclass/&TraditionSpellMasterExtraPreparedDescription");
            FeatureDefinitionMagicAffinity extraPrepared = PreparedSpellModifier(RuleDefinitions.PreparedSpellsModifier.SpellcastingAbilityBonus,
                "MagicAffinitySpellMasterExtraPrepared", extraPreparedGui.Build());
            spellMaster.AddFeatureAtLevel(extraPrepared, 10);

            GuiPresentationBuilder spellResistanceGui = new GuiPresentationBuilder(
                "Subclass/&TraditionSpellMasterSpellResistanceTitle",
                "Subclass/&TraditionSpellMasterSpellResistanceDescription");
            FeatureDefinitionSavingThrowAffinity spellResistance = new FeatureDefinitionSavingThrowAffinityBuilder("TraditionSpellMasterSpellResistance",
                GuidHelper.Create(SubclassNamespace, "TraditionSpellMasterSpellResistance").ToString(),
                new List<string>()
                {
                    AttributeDefinitions.Strength,
                    AttributeDefinitions.Dexterity,
                    AttributeDefinitions.Constitution,
                    AttributeDefinitions.Wisdom,
                    AttributeDefinitions.Intelligence,
                    AttributeDefinitions.Charisma,
                }, RuleDefinitions.CharacterSavingThrowAffinity.Advantage, true, spellResistanceGui.Build()).AddToDB();
            spellMaster.AddFeatureAtLevel(spellResistance, 14);

            Subclass = spellMaster.AddToDB();
        }

        public static FeatureDefinitionMagicAffinity PreparedSpellModifier(RuleDefinitions.PreparedSpellsModifier preparedModifier, string name, GuiPresentation guiPresentation)
        {
            FeatureDefinitionMagicAffinityBuilder builder = new FeatureDefinitionMagicAffinityBuilder(name, GuidHelper.Create(SubclassNamespace, name).ToString(),
                guiPresentation).SetSpellLearnAndPrepModifiers(1f, 1f, 0, RuleDefinitions.AdvantageType.None, preparedModifier);
            return builder.AddToDB();
        }

        public static FeatureDefinitionMagicAffinity BuildMagicAffinityScribing(float scribeDurationMultiplier, float scribeCostMultiplier,
            int additionalScribedSpells, RuleDefinitions.AdvantageType scribeAdvantage, string name, GuiPresentation guiPresentation)
        {
            FeatureDefinitionMagicAffinityBuilder builder = new FeatureDefinitionMagicAffinityBuilder(name, GuidHelper.Create(SubclassNamespace, name).ToString(),
                guiPresentation).SetSpellLearnAndPrepModifiers(scribeDurationMultiplier, scribeCostMultiplier, additionalScribedSpells, scribeAdvantage, RuleDefinitions.PreparedSpellsModifier.None);
            return builder.AddToDB();
        }

        public static FeatureDefinitionPower BuildSpellFormPower(int usesPerRecharge, RuleDefinitions.UsesDetermination usesDetermination,
            RuleDefinitions.ActivationTime activationTime, int costPerUse, RuleDefinitions.RechargeRate recharge, string name, GuiPresentation guiPresentation)
        {
            EffectDescriptionBuilder effectDescriptionBuilder = new EffectDescriptionBuilder();
            effectDescriptionBuilder.SetTargetingData(RuleDefinitions.Side.All, RuleDefinitions.RangeType.Self, 0, 0, 0, 0, ActionDefinitions.ItemSelectionType.None);
            effectDescriptionBuilder.SetCreatedByCharacter();

            EffectFormBuilder effectFormBuilder = new EffectFormBuilder();
            effectFormBuilder.SetSpellForm(9);
            effectDescriptionBuilder.AddEffectForm(effectFormBuilder.Build());
            effectDescriptionBuilder.SetEffectAdvancement(RuleDefinitions.EffectIncrementMethod.None, 1, 0, 0, 0, 0, 0, 0, 0, 0, RuleDefinitions.AdvancementDuration.None);

            EffectParticleParameters particleParams = new EffectParticleParameters();
            particleParams.Copy(DatabaseHelper.FeatureDefinitionPowers.PowerWizardArcaneRecovery.EffectDescription.EffectParticleParameters);
            effectDescriptionBuilder.SetParticleEffectParameters(particleParams);

            FeatureDefinitionPowerBuilder builder = new FeatureDefinitionPowerBuilder(name, GuidHelper.Create(SubclassNamespace, name).ToString(),
                usesPerRecharge, usesDetermination, AttributeDefinitions.Intelligence, activationTime, costPerUse, recharge, false, false, AttributeDefinitions.Intelligence, effectDescriptionBuilder.Build(), guiPresentation, false);
            return builder.AddToDB();
        }

        public static RestActivityDefinition BuildRestActivity(RestDefinitions.RestStage restStage, RuleDefinitions.RestType restType, RestActivityDefinition.ActivityCondition condition,
            string functor, string stringParameter, string name, GuiPresentation guiPresentation)
        {
            RestActivityDefinition restActivity = ScriptableObject.CreateInstance<RestActivityDefinition>();
            restActivity.SetRestStage(restStage);
            restActivity.SetRestType(restType);
            restActivity.SetCondition(condition);
            restActivity.SetFunctor(functor);
            restActivity.SetStringParameter(stringParameter);

            restActivity.name = name;
            restActivity.SetGuiPresentation(guiPresentation);
            restActivity.SetGuid(GuidHelper.Create(SubclassNamespace, name).ToString());
            DatabaseRepository.GetDatabase<RestActivityDefinition>().Add(restActivity);
            return restActivity;
        }

        public static void UpdateRecoveryLimited()
        {
            if (Main.Settings.EnableUnlimitedArcaneRecoveryOnWizardSpellMaster)
            {
                GuiPresentationBuilder spellRecoveryGui = new GuiPresentationBuilder(
                    "Subclass/&MagicAffinitySpellMasterRecoveryUnlimitedTitle",
                    "Subclass/&MagicAffinitySpellMasterRecoveryUnlimitedDescription");
                spellRecoveryGui.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerWizardArcaneRecovery.GuiPresentation.SpriteReference);

                BonusRecovery.SetGuiPresentation(spellRecoveryGui.Build());
                BonusRecovery.SetCostPerUse(0);
                BonusRecovery.SetRechargeRate(RuleDefinitions.RechargeRate.AtWill);
            }
            else
            {
                GuiPresentationBuilder spellRecoveryGui = new GuiPresentationBuilder(
                    "Subclass/&MagicAffinitySpellMasterRecoveryTitle",
                    "Subclass/&MagicAffinitySpellMasterRecoveryDescription");
                spellRecoveryGui.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerWizardArcaneRecovery.GuiPresentation.SpriteReference);
                BonusRecovery.SetGuiPresentation(spellRecoveryGui.Build());
                BonusRecovery.SetCostPerUse(1);
                BonusRecovery.SetRechargeRate(RuleDefinitions.RechargeRate.LongRest);
            }
        }
    }
}
