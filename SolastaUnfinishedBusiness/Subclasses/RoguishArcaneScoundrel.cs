using System.Collections;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class RoguishArcaneScoundrel : AbstractSubclass
{
    internal const string Name = "RoguishArcaneScoundrel";
    private const string DistractingAmbush = "DistractingAmbush";

    internal RoguishArcaneScoundrel()
    {
        //
        // LEVEL 3
        //

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
            .SetProficiencies(ProficiencyType.SkillOrExpertise, SkillDefinitions.Arcana)
            .AddToDB();

        //
        // LEVEL 9
        //

        var conditionDistractingAmbush = ConditionDefinitionBuilder
            .Create($"Condition{Name}{DistractingAmbush}")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionDistracted)
            .SetConditionType(ConditionType.Detrimental)
            .SetPossessive()
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
            .SetSpecialInterruptions(ConditionInterruption.AbilityCheck, ConditionInterruption.SavingThrow)
            .SetFeatures(
                FeatureDefinitionAbilityCheckAffinityBuilder
                    .Create($"AbilityCheckAffinity{Name}{DistractingAmbush}")
                    .SetGuiPresentation($"Condition{Name}{DistractingAmbush}", Category.Condition)
                    .BuildAndSetAffinityGroups(CharacterAbilityCheckAffinity.Disadvantage,
                        AttributeDefinitions.Strength,
                        AttributeDefinitions.Dexterity,
                        AttributeDefinitions.Constitution,
                        AttributeDefinitions.Intelligence,
                        AttributeDefinitions.Wisdom,
                        AttributeDefinitions.Charisma)
                    .AddToDB(),
                FeatureDefinitionSavingThrowAffinityBuilder
                    .Create($"SavingThrowAffinity{Name}{DistractingAmbush}")
                    .SetGuiPresentation($"Condition{Name}{DistractingAmbush}", Category.Condition)
                    .SetAffinities(CharacterSavingThrowAffinity.Disadvantage, false,
                        AttributeDefinitions.Strength,
                        AttributeDefinitions.Dexterity,
                        AttributeDefinitions.Constitution,
                        AttributeDefinitions.Intelligence,
                        AttributeDefinitions.Wisdom,
                        AttributeDefinitions.Charisma)
                    .AddToDB())
            .AddToDB();

        var additionalDamageDistractingAmbush = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}{DistractingAmbush}")
            .SetGuiPresentation(Category.Feature)
            .SetDamageValueDetermination(AdditionalDamageValueDetermination.FlatBonus)
            .SetRequiredProperty(RestrictedContextRequiredProperty.FinesseOrRangeWeapon)
            .SetTriggerCondition(AdditionalDamageTriggerCondition.AdvantageOrNearbyAlly)
            .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
            .SetConditionOperations(
                new ConditionOperationDescription
                {
                    operation = ConditionOperationDescription.ConditionOperation.Add,
                    conditionDefinition = conditionDistractingAmbush
                })
            .AddToDB();

        //
        // LEVEL 13
        //

        var autoPreparedSpellsArcaneBackslash = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{Name}ArcaneBackslash")
            .SetGuiPresentation("ExpandedSpells", Category.Feature)
            .SetAutoTag("Roguish")
            .SetPreparedSpellGroups(BuildSpellGroup(13, Counterspell))
            .SetSpellcastingClass(CharacterClassDefinitions.Rogue)
            .AddToDB();

        var powerArcaneBackslashCounterSpell = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ArcaneBackslashCounterSpell")
            .SetGuiPresentation(Counterspell.GuiPresentation)
            .SetUsesFixed(ActivationTime.Reaction, RechargeRate.ShortRest)
            .SetEffectDescription(Counterspell.EffectDescription)
            .SetCustomSubFeatures(new ModifyMagicEffectArcaneBackslashCounterSpell())
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
            new ActionFinishedCounterSpell(
                powerArcaneBacklash,
                powerArcaneBackslashCounterSpell,
                conditionDistractingAmbush));

        //
        // LEVEL 17
        //

        var powerGambit = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Gambit")
            .SetGuiPresentation(Category.Feature, SpellsContext.MirrorImage)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.ShortRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(SpellsContext.MirrorImage.EffectDescription)
                    .Build())
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass,
                Sprites.GetSprite("ArcaneScoundrel", Resources.RoguishArcaneScoundrel, 256))
            .AddFeaturesAtLevel(3,
                castSpell,
                proficiencyCraftyArcana)
            .AddFeaturesAtLevel(9,
                additionalDamageDistractingAmbush)
            .AddFeaturesAtLevel(13,
                autoPreparedSpellsArcaneBackslash,
                powerArcaneBacklash,
                powerArcaneBackslashCounterSpell)
            .AddFeaturesAtLevel(17,
                powerGambit)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceRogueRoguishArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private sealed class ModifyMagicEffectArcaneBackslashCounterSpell : IModifyMagicEffect
    {
        public EffectDescription ModifyEffect(
            BaseDefinition definition,
            EffectDescription effect,
            RulesetCharacter character)
        {
            var level = character.GetClassLevel(CharacterClassDefinitions.Rogue);

            if (level < 19)
            {
                return effect;
            }

            effect.effectForms[0].CounterForm.automaticSpellLevel = 4;

            return effect;
        }
    }

    private sealed class ActionFinishedCounterSpell : IActionFinished
    {
        private readonly ConditionDefinition _conditionDistractingAmbush;
        private readonly FeatureDefinitionPower _powerArcaneBackslash;
        private readonly FeatureDefinitionPower _powerCounterSpell;

        public ActionFinishedCounterSpell(
            FeatureDefinitionPower powerArcaneBackslash,
            FeatureDefinitionPower powerCounterSpell,
            ConditionDefinition conditionDistractingAmbush)
        {
            _powerArcaneBackslash = powerArcaneBackslash;
            _powerCounterSpell = powerCounterSpell;
            _conditionDistractingAmbush = conditionDistractingAmbush;
        }

        public IEnumerator OnActionFinished(CharacterAction action)
        {
            if ((action is not CharacterActionCastSpell characterActionCastSpell ||
                 characterActionCastSpell.ActiveSpell.SpellDefinition != Counterspell ||
                 !characterActionCastSpell.ActionParams.TargetAction.Countered) &&
                (action is not CharacterActionSpendPower characterActionSpendPower ||
                 characterActionSpendPower.activePower.PowerDefinition != _powerCounterSpell ||
                 !characterActionSpendPower.ActionParams.TargetAction.Countered))
            {
                yield break;
            }

            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;
            var usablePower = UsablePowersProvider.Get(_powerArcaneBackslash, rulesetCharacter);
            var effectPower = new RulesetEffectPower(rulesetCharacter, usablePower);

            foreach (var gameLocationCharacter in action.actionParams.TargetCharacters)
            {
                var rulesetDefender = gameLocationCharacter.RulesetCharacter;

                GameConsoleHelper.LogCharacterUsedPower(rulesetCharacter, _powerArcaneBackslash);
                effectPower.ApplyEffectOnCharacter(rulesetDefender, true, gameLocationCharacter.LocationPosition);
                rulesetDefender.InflictCondition(
                    _conditionDistractingAmbush.Name,
                    _conditionDistractingAmbush.DurationType,
                    _conditionDistractingAmbush.DurationParameter,
                    _conditionDistractingAmbush.TurnOccurence,
                    AttributeDefinitions.TagCombat,
                    rulesetCharacter.guid,
                    rulesetCharacter.CurrentFaction.Name,
                    1,
                    null,
                    0,
                    0,
                    0);
            }
        }
    }
}
