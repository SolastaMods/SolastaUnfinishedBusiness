using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAdditionalDamages;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class RoguishArcaneScoundrel : AbstractSubclass
{
    internal const string Name = "RoguishArcaneScoundrel";
    private const string DistractingAmbush = "DistractingAmbush";

    public RoguishArcaneScoundrel()
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
            .SetProficiencies(ProficiencyType.Skill, SkillDefinitions.Arcana)
            .AddToDB();

        var magicAffinityGuilefulCasting = FeatureDefinitionMagicAffinityBuilder
            .Create($"MagicAffinity{Name}GuilefulCasting")
            .SetGuiPresentation(Category.Feature)
            .SetHandsFullCastingModifiers(true, false, true)
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
                    .SetGuiPresentation($"Condition{Name}{DistractingAmbush}", Category.Condition, Gui.NoLocalization)
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
                    .SetGuiPresentation($"Condition{Name}{DistractingAmbush}", Category.Condition, Gui.NoLocalization)
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
            .SetDamageValueDetermination(AdditionalDamageValueDetermination.None)
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
            .SetUsesFixed(ActivationTime.Reaction, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(
                        EffectDescriptionBuilder
                            .Create(Counterspell)
                            .Build())
                    .Build())
            .AddToDB();

        powerArcaneBackslashCounterSpell.AddCustomSubFeatures(
            new ModifyEffectDescriptionArcaneBackslashCounterSpell(powerArcaneBackslashCounterSpell));

        var powerArcaneBacklash = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ArcaneBackslash")
            .SetUsesFixed(ActivationTime.NoCost)
            .SetGuiPresentation(Category.Feature)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeForce, 1, DieType.D6)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 1, 1, 2, 2)
                            .Build())
                    .Build())
            .AddToDB();

        powerArcaneBacklash.AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden,
            new ActionFinishedByMeArcaneBackslash(
                powerArcaneBacklash,
                powerArcaneBackslashCounterSpell,
                conditionDistractingAmbush));

        //
        // LEVEL 17
        //
        var conditionPossessed = ConditionDefinitionBuilder
            .Create($"Condition{Name}Possessed")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionPossessed)
            .SetConditionType(ConditionType.Detrimental)
            .SetPossessive()
            .SetSpecialDuration(DurationType.Round, 0, TurnOccurenceType.EndOfSourceTurn)
            .AddToDB();

        // kept name for backward compatibility
        var additionalDamagePossessed = FeatureDefinitionPowerBuilder
            .Create($"AdditionalDamage{Name}Possessed")
            .SetGuiPresentation($"Condition{Name}Possessed", Category.Condition)
            .SetUsesFixed(ActivationTime.OnSneakAttackHitAuto)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionPossessed))
                    .Build())
            .AddToDB();

        var powerEssenceTheft = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}EssenceTheft")
            .SetGuiPresentation(Category.Feature, FeatureDefinitionPowers.PowerRoguishHoodlumDirtyFighting)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.TurnStart)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetParticleEffectParameters(FeatureDefinitionPowers.PowerRoguishHoodlumDirtyFighting)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDiceAdvancement(LevelSourceType.CharacterLevel, 1, 1, 2, 19)
                            .SetDamageForm(DamageTypeForce, 4, DieType.D6)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionDistractingAmbush, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        powerEssenceTheft.AddCustomSubFeatures(
            new ValidatorsValidatePowerUse(CanUseEssenceTheft),
            new CustomBehaviorEssenceTheft(powerEssenceTheft, conditionPossessed));

        var featureSetTricksOfTheTrade = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}TricksOfTheTrade")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(additionalDamagePossessed, powerEssenceTheft)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass,
                Sprites.GetSprite("ArcaneScoundrel", Resources.RoguishArcaneScoundrel, 256))
            .AddFeaturesAtLevel(3,
                castSpell,
                magicAffinityGuilefulCasting,
                proficiencyCraftyArcana)
            .AddFeaturesAtLevel(9,
                additionalDamageDistractingAmbush)
            .AddFeaturesAtLevel(13,
                autoPreparedSpellsArcaneBackslash,
                powerArcaneBacklash,
                powerArcaneBackslashCounterSpell)
            .AddFeaturesAtLevel(17,
                featureSetTricksOfTheTrade)
            .AddToDB();

        return;

        static bool CanUseEssenceTheft(RulesetCharacter character)
        {
            var gameLocationCharacter = GameLocationCharacter.GetFromActor(character);

            return gameLocationCharacter != null &&
                   gameLocationCharacter.UsedSpecialFeatures.ContainsKey(AdditionalDamageRogueSneakAttack.Name);
        }
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Rogue;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceRogueRoguishArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private sealed class ModifyEffectDescriptionArcaneBackslashCounterSpell(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionPower powerArcaneBackslashCounterSpell) : IModifyEffectDescription
    {
        public bool IsValid(
            BaseDefinition definition,
            RulesetCharacter character,
            EffectDescription effectDescription)
        {
            return definition == powerArcaneBackslashCounterSpell
                   && character.GetClassLevel(CharacterClassDefinitions.Rogue) >= 19;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            effectDescription.effectForms[0].CounterForm.automaticSpellLevel = 4;

            return effectDescription;
        }
    }

    private sealed class ActionFinishedByMeArcaneBackslash(
        FeatureDefinitionPower powerArcaneBackslash,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionPower powerCounterSpell,
        ConditionDefinition conditionArcaneAmbush)
        : IActionFinishedByMe
    {
        public IEnumerator OnActionFinishedByMe(CharacterAction action)
        {
            if ((action is not CharacterActionCastSpell characterActionCastSpell ||
                 characterActionCastSpell.ActiveSpell.SpellDefinition != Counterspell ||
                 !characterActionCastSpell.ActionParams.TargetAction.Countered) &&
                (action is not CharacterActionUsePower characterActionUsePower ||
                 characterActionUsePower.activePower.PowerDefinition != powerCounterSpell ||
                 !characterActionUsePower.ActionParams.TargetAction.Countered))
            {
                yield break;
            }

            var actingCharacter = action.ActingCharacter;
            var rulesetAttacker = actingCharacter.RulesetCharacter;
            var targetCharacter = action.ActionParams.TargetCharacters[0];

            targetCharacter.RulesetCharacter.InflictCondition(
                conditionArcaneAmbush.Name,
                conditionArcaneAmbush.DurationType,
                conditionArcaneAmbush.DurationParameter,
                conditionArcaneAmbush.TurnOccurence,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                conditionArcaneAmbush.Name,
                0,
                0,
                0);

            actingCharacter.UsedSpecialFeatures.TryAdd(AdditionalDamageRogueSneakAttack.Name, 1);

            var implementationManagerService =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(powerArcaneBackslash, rulesetAttacker);
            var targets = action.ActionParams.TargetCharacters.ToList();
            var actionParams = new CharacterActionParams(actingCharacter, ActionDefinitions.Id.PowerNoCost)
            {
                ActionModifiers = Enumerable.Repeat(new ActionModifier(), targets.Count).ToList(),
                RulesetEffect = implementationManagerService
                    //CHECK: no need for AddAsActivePowerToSource
                    .MyInstantiateEffectPower(rulesetAttacker, usablePower, false),
                UsablePower = usablePower,
                targetCharacters = targets
            };

            // different follow up pattern [not adding to ResultingActions]
            ServiceRepository.GetService<ICommandService>()?
                .ExecuteAction(actionParams, null, false);
        }
    }

    private sealed class CustomBehaviorEssenceTheft(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionPower powerEssenceTheft,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionPossessed)
        : IMagicEffectInitiatedByMe, IFilterTargetingCharacter
    {
        public bool EnforceFullSelection => false;

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            if (__instance.actionParams.RulesetEffect is not RulesetEffectPower rulesetEffectPower ||
                rulesetEffectPower.PowerDefinition != powerEssenceTheft)
            {
                return true;
            }

            if (target.RulesetCharacter == null)
            {
                return true;
            }

            var isValid = !target.RulesetCharacter.HasConditionOfType(conditionPossessed.Name);

            if (!isValid)
            {
                __instance.actionModifier.FailureFlags.Add("Tooltip/&MustNotHavePossessedCondition");
            }

            return isValid;
        }

        public IEnumerator OnMagicEffectInitiatedByMe(CharacterActionMagicEffect action, BaseDefinition power)
        {
            var actingCharacter = action.ActingCharacter;

            actingCharacter.UsedSpecialFeatures.TryAdd(powerEssenceTheft.Name, 1);

            var damage = action.ActionParams.RulesetEffect.EffectDescription.FindFirstDamageForm();

            // this currently works as there is only one feature in game using DamageDieProviderFromCharacter
            // we might need to change this to a proper interface if others start using it
            var hasSneakAttackDieTypeChange = actingCharacter.RulesetCharacter
                .GetSubFeaturesByType<DamageDieProviderFromCharacter>()
                .Count != 0;

            if (!hasSneakAttackDieTypeChange)
            {
                damage.dieType = DieType.D6;
                yield break;
            }

            damage.dieType = DieType.D8;
        }
    }
}
