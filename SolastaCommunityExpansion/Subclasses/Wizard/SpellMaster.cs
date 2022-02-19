using System;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper;
using static SolastaModApi.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionPowers;

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
                return _spellRecoveryGui ??= GuiPresentationBuilder
                    .Build("MagicAffinitySpellMasterRecovery", Category.Subclass, PowerWizardArcaneRecovery.GuiPresentation.SpriteReference);
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
            return FeatureDefinitionSubclassChoices.SubclassChoiceWizardArcaneTraditions;
        }
        internal override CharacterSubclassDefinition GetSubclass()
        {
            return Subclass;
        }

        internal SpellMaster()
        {
            var prepared = FeatureDefinitionMagicAffinityBuilder
                .Create("MagicAffinitySpellMasterPrepared", SubclassNamespace)
                .SetGuiPresentation("TraditionSpellMasterPrepared", Category.Subclass)
                .SetSpellLearnAndPrepModifiers(1f, 1f, 0, RuleDefinitions.AdvantageType.None, RuleDefinitions.PreparedSpellsModifier.ProficiencyBonus)
                .AddToDB();

            var extraPrepared = FeatureDefinitionMagicAffinityBuilder
                .Create("MagicAffinitySpellMasterExtraPrepared", SubclassNamespace)
                .SetGuiPresentation("TraditionSpellMasterExtraPrepared", Category.Subclass)
                .SetSpellLearnAndPrepModifiers(1f, 1f, 0, RuleDefinitions.AdvantageType.None, RuleDefinitions.PreparedSpellsModifier.SpellcastingAbilityBonus)
                .AddToDB();

            var extraKnown = FeatureDefinitionMagicAffinityBuilder
                .Create("MagicAffinitySpellMasterKnowledge", SubclassNamespace)
                .SetGuiPresentation("MagicAffinitySpellMasterBonusScribing", Category.Subclass)
                .SetSpellLearnAndPrepModifiers(1f, 1f, 1, RuleDefinitions.AdvantageType.None, RuleDefinitions.PreparedSpellsModifier.None)
                .AddToDB();

            var knowledgeAffinity = FeatureDefinitionMagicAffinityBuilder
                .Create("MagicAffinitySpellMasterScriber", SubclassNamespace)
                .SetGuiPresentation("MagicAffinitySpellMasterScribing", Category.Subclass)
                .SetSpellLearnAndPrepModifiers(0.25f, 0.25f, 0, RuleDefinitions.AdvantageType.Advantage, RuleDefinitions.PreparedSpellsModifier.None)
                .AddToDB();

            var bonusCantrips = FeatureDefinitionPointPoolBuilder
                .Create("TraditionSpellMasterBonusCantrips", SubclassNamespace)
                .SetGuiPresentation(Category.Subclass)
                .SetPool(HeroDefinitions.PointsPoolType.Cantrip, 2)
                .OnlyUniqueChoices()
                .AddToDB();

            var spellResistance = FeatureDefinitionSavingThrowAffinityBuilder
                .Create("TraditionSpellMasterSpellResistance", SubclassNamespace)
                .SetGuiPresentation(Category.Spell)
                .SetAffinities(
                    RuleDefinitions.CharacterSavingThrowAffinity.Advantage, true,
                    AttributeDefinitions.Strength,
                    AttributeDefinitions.Dexterity,
                    AttributeDefinitions.Constitution,
                    AttributeDefinitions.Wisdom,
                    AttributeDefinitions.Intelligence,
                    AttributeDefinitions.Charisma
                )
                .AddToDB();

            // Make Spell Master subclass
            var spellMaster = CharacterSubclassDefinitionBuilder
                .Create("SpellMaster", SubclassNamespace)
                .SetGuiPresentation("TraditionSpellMaster", Category.Subclass, DomainInsight.GuiPresentation.SpriteReference)
                .AddFeatureAtLevel(prepared, 2)
                .AddFeatureAtLevel(extraKnown, 2)
                .AddFeatureAtLevel(BonusRecovery, 2)
                .AddFeatureAtLevel(knowledgeAffinity, 6)
                .AddFeatureAtLevel(bonusCantrips, 6)
                .AddFeatureAtLevel(extraPrepared, 10)
                .AddFeatureAtLevel(spellResistance, 14)
                .AddToDB();

            UpdateRecoveryLimited();

            RestActivityDefinitionBuilder
                .Create("ArcaneDepth", SubclassNamespace)
                .Configure(
                    RestDefinitions.RestStage.AfterRest, RuleDefinitions.RestType.ShortRest,
                    RestActivityDefinition.ActivityCondition.CanUsePower, "UsePower", BonusRecovery.Name)
                .SetGuiPresentation(SpellRecoveryGui)
                .AddToDB();

            Subclass = spellMaster;
        }

        private static FeatureDefinitionPower BuildSpellFormPower(int usesPerRecharge, RuleDefinitions.UsesDetermination usesDetermination,
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
            particleParams.Copy(PowerWizardArcaneRecovery.EffectDescription.EffectParticleParameters);
            effectDescriptionBuilder.SetParticleEffectParameters(particleParams);

            return FeatureDefinitionPowerBuilder.Create(name, SubclassNamespace)
                .SetGuiPresentation(guiPresentation)
                .Configure(usesPerRecharge, usesDetermination, AttributeDefinitions.Intelligence, activationTime, costPerUse,
                    recharge, false, false, AttributeDefinitions.Intelligence, effectDescriptionBuilder.Build(), false)
                .AddToDB();
        }

        internal static void UpdateRecoveryLimited()
        {
            if (Main.Settings.EnableUnlimitedArcaneRecoveryOnWizardSpellMaster)
            {
                BonusRecovery.SetGuiPresentation(
                    GuiPresentationBuilder.Build(
                        "MagicAffinitySpellMasterRecoveryUnlimited",
                        Category.Subclass, PowerWizardArcaneRecovery.GuiPresentation.SpriteReference));
                BonusRecovery.SetCostPerUse(0);
                BonusRecovery.SetRechargeRate(RuleDefinitions.RechargeRate.AtWill);
            }
            else
            {
                BonusRecovery.SetGuiPresentation(
                    GuiPresentationBuilder.Build(
                        "MagicAffinitySpellMasterRecovery",
                        Category.Subclass, PowerWizardArcaneRecovery.GuiPresentation.SpriteReference));
                BonusRecovery.SetCostPerUse(1);
                BonusRecovery.SetRechargeRate(RuleDefinitions.RechargeRate.LongRest);
            }
        }

        private sealed class RestActivityDefinitionBuilder : Builders.RestActivityDefinitionBuilder
        {
            private RestActivityDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
            {
            }

            internal static RestActivityDefinitionBuilder Create(string name, Guid namespaceGuid)
            {
                return new RestActivityDefinitionBuilder(name, namespaceGuid);
            }

            internal RestActivityDefinitionBuilder Configure(
                RestDefinitions.RestStage restStage, RuleDefinitions.RestType restType,
                RestActivityDefinition.ActivityCondition condition, string functor, string stringParameter)
            {
                Definition.SetRestStage(restStage);
                Definition.SetRestType(restType);
                Definition.SetCondition(condition);
                Definition.SetFunctor(functor);
                Definition.SetStringParameter(stringParameter);

                return this;
            }
        }
    }
}
