using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class RoguishArcaneScoundrel : AbstractSubclass
{
    private const string Name = "RoguishArcaneScoundrel";
    private const string DistractingAmbush = "DistractingAmbush";

    internal RoguishArcaneScoundrel()
    {
        var castSpell = FeatureDefinitionCastSpellBuilder
            .Create($"CastSpell{Name}")
            .SetGuiPresentation(Category.Feature)
            .SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Subclass)
            .SetSpellCastingAbility(AttributeDefinitions.Intelligence)
            .SetSpellList(SpellListDefinitions.SpellListWizard)
            .SetSpellKnowledge(SpellKnowledge.Selection)
            .SetSpellReadyness(SpellReadyness.AllKnown)
            .SetSlotsRecharge(RechargeRate.LongRest)
            .SetFocusType(EquipmentDefinitions.FocusType.Arcane)
            .SetReplacedSpells(4, 1)
            .SetKnownCantrips(3, 3, FeatureDefinitionCastSpellBuilder.CasterProgression.OneThird)
            .SetKnownSpells(4, FeatureDefinitionCastSpellBuilder.CasterProgression.OneThird)
            .SetSlotsPerLevel(FeatureDefinitionCastSpellBuilder.CasterProgression.OneThird)
            .AddToDB();

        var proficiencyCraftyArcana = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}Arcana")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.SkillOrExpertise, DatabaseHelper.SkillDefinitions.Arcana.Name)
            .AddToDB();

        var additionalDamageDistractingAmbush = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}{DistractingAmbush}")
            .SetGuiPresentation(Category.Feature)
            .SetDamageValueDetermination(AdditionalDamageValueDetermination.None)
            .SetRequiredProperty(RestrictedContextRequiredProperty.FinesseOrRangeWeapon)
            .SetTriggerCondition(AdditionalDamageTriggerCondition.AdvantageOrNearbyAlly)
            .SetFirstTargetOnly(true)
            .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
            .SetConditionOperations(
                new ConditionOperationDescription
                {
                    hasSavingThrow = false,
                    operation = ConditionOperationDescription.ConditionOperation.Add,
                    conditionDefinition = ConditionDefinitionBuilder
                        .Create($"Condition{Name}{DistractingAmbush}")
                        .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionDistracted)
                        .SetConditionType(ConditionType.Detrimental)
                        .SetPossessive()
                        .SetSpecialDuration(DurationType.Round, 1)
                        .SetFeatures(
                            FeatureDefinitionActionAffinityBuilder
                                .Create($"ActionAffinity{Name}{DistractingAmbush}")
                                .SetGuiPresentationNoContent(true)
                                .SetAllowedActionTypes()
                                .SetForbiddenActions(
                                    ActionDefinitions.Id.AttackOpportunity,
                                    ActionDefinitions.Id.CastReaction,
                                    ActionDefinitions.Id.PowerReaction,
                                    ActionDefinitions.Id.UncannyDodge)
                                .AddToDB(),
                            FeatureDefinitionSavingThrowAffinityBuilder
                                .Create($"SavingThrowAffinity{Name}{DistractingAmbush}")
                                .SetGuiPresentationNoContent(true)
                                .SetModifiers(FeatureDefinitionSavingThrowAffinity.ModifierType.RemoveDice,
                                    DieType.D1, 2, false,
                                    AttributeDefinitions.Charisma,
                                    AttributeDefinitions.Constitution,
                                    AttributeDefinitions.Dexterity,
                                    AttributeDefinitions.Intelligence,
                                    AttributeDefinitions.Strength,
                                    AttributeDefinitions.Wisdom)
                                .AddToDB())
                        .AddToDB()
                })
            .AddToDB();

        var powerArcaneBacklash = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ArcaneBackslash")
            .SetGuiPresentation(Category.Feature)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Instantaneous)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeForce, 1, DieType.D6)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 1, 1, 2, 2)
                            .Build())
                    .Build())
            .SetCustomSubFeatures(PowerVisibilityModifier.Hidden)
            .AddToDB();

        powerArcaneBacklash.SetCustomSubFeatures(
            PowerVisibilityModifier.Hidden,
            new ModifyMagicEffectCounterSpell(powerArcaneBacklash));

        var autoPreparedSpellsArcaneBackslash = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{Name}ArcaneBackslash")
            .SetGuiPresentation("ExpandedSpells", Category.Feature)
            .SetAutoTag("Roguish")
            .SetPreparedSpellGroups(BuildSpellGroup(13, Counterspell))
            .SetSpellcastingClass(CharacterClassDefinitions.Rogue)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, RangerSwiftBlade)
            .AddFeaturesAtLevel(3,
                castSpell,
                proficiencyCraftyArcana)
            .AddFeaturesAtLevel(9,
                additionalDamageDistractingAmbush)
            .AddFeaturesAtLevel(13,
                autoPreparedSpellsArcaneBackslash,
                powerArcaneBacklash)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceRogueRoguishArchetypes;

    internal override DeityDefinition DeityDefinition { get; }

    private sealed class ModifyMagicEffectCounterSpell : IOnAfterActionFeature
    {
        private readonly FeatureDefinitionPower _featureDefinitionPower;

        public ModifyMagicEffectCounterSpell(FeatureDefinitionPower featureDefinitionPower)
        {
            _featureDefinitionPower = featureDefinitionPower;
        }

        public void OnAfterAction(CharacterAction action)
        {
            if (action is not CharacterActionCastSpell characterActionCastSpell ||
                characterActionCastSpell.ActiveSpell.SpellDefinition != Counterspell ||
                !characterActionCastSpell.ActionParams.TargetAction.Countered)
            {
                return;
            }

            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;
            var usablePower = new RulesetUsablePower(_featureDefinitionPower, null, CharacterClassDefinitions.Rogue);
            var effectPower = new RulesetEffectPower(rulesetCharacter, usablePower);

            foreach (var gameLocationCharacter in action.actionParams.TargetCharacters)
            {
                effectPower.ApplyEffectOnCharacter(gameLocationCharacter.RulesetCharacter, true,
                    gameLocationCharacter.LocationPosition);
            }
        }
    }
}
