using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAdditionalDamages;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionMagicAffinitys;

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
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(Counterspell.EffectDescription)
                    .AddEffectForms(EffectFormBuilder.ConditionForm(conditionDistractingAmbush))
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
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeForce, 1, DieType.D6)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 1, 1, 2, 2)
                            .Build())
                    .Build())
            .AddToDB();

        powerArcaneBacklash.AddCustomSubFeatures(
            PowerVisibilityModifier.Hidden,
            new ActionFinishedByMeArcaneBackslash(
                powerArcaneBacklash,
                powerArcaneBackslashCounterSpell));

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
            .AddFeatureSet(
                MagicAffinityAdditionalSpellSlot3,
                additionalDamagePossessed,
                powerEssenceTheft)
            .AddToDB();

        var featureSetPremeditationSlot = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}PremeditationSlot")
            .SetGuiPresentationNoContent(true)
            .AddFeatureSet(MagicAffinityAdditionalSpellSlot4)
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
            .AddFeaturesAtLevel(19,
                featureSetPremeditationSlot)
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

    private sealed class ModifyEffectDescriptionArcaneBackslashCounterSpell : IModifyEffectDescription
    {
        private readonly FeatureDefinitionPower _powerArcaneBackslashCounterSpell;

        public ModifyEffectDescriptionArcaneBackslashCounterSpell(
            FeatureDefinitionPower powerArcaneBackslashCounterSpell)
        {
            _powerArcaneBackslashCounterSpell = powerArcaneBackslashCounterSpell;
        }

        public bool IsValid(
            BaseDefinition definition,
            RulesetCharacter character,
            EffectDescription effectDescription)
        {
            return definition == _powerArcaneBackslashCounterSpell
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

    private sealed class ActionFinishedByMeArcaneBackslash : IActionFinishedByMe
    {
        private readonly FeatureDefinitionPower _powerArcaneBackslash;
        private readonly FeatureDefinitionPower _powerCounterSpell;

        public ActionFinishedByMeArcaneBackslash(
            FeatureDefinitionPower powerArcaneBackslash,
            FeatureDefinitionPower powerCounterSpell)
        {
            _powerArcaneBackslash = powerArcaneBackslash;
            _powerCounterSpell = powerCounterSpell;
        }

        public IEnumerator OnActionFinishedByMe(CharacterAction action)
        {
            if ((action is not CharacterActionCastSpell characterActionCastSpell ||
                 characterActionCastSpell.ActiveSpell.SpellDefinition != Counterspell ||
                 !characterActionCastSpell.ActionParams.TargetAction.Countered) &&
                (action is not CharacterActionUsePower characterActionUsePower ||
                 characterActionUsePower.activePower.PowerDefinition != _powerCounterSpell ||
                 !characterActionUsePower.ActionParams.TargetAction.Countered))
            {
                yield break;
            }

            var actingCharacter = action.ActingCharacter;

            actingCharacter.UsedSpecialFeatures.TryAdd(AdditionalDamageRogueSneakAttack.Name, 1);

            var actionParams = action.ActionParams.Clone();
            var rulesetAttacker = actingCharacter.RulesetCharacter;
            var usablePower = UsablePowersProvider.Get(_powerArcaneBackslash, rulesetAttacker);

            actionParams.ActionDefinition = DatabaseHelper.ActionDefinitions.SpendPower;
            actionParams.RulesetEffect = ServiceRepository.GetService<IRulesetImplementationService>()
                //CHECK: no need for AddAsActivePowerToSource
                .InstantiateEffectPower(rulesetAttacker, usablePower, false);

            // different follow up pattern [not adding to ResultingActions]
            ServiceRepository.GetService<ICommandService>()?.ExecuteAction(actionParams, null, false);
        }
    }

    private sealed class CustomBehaviorEssenceTheft : IMagicEffectInitiatedByMe, IFilterTargetingCharacter
    {
        private readonly ConditionDefinition _conditionPossessed;
        private readonly FeatureDefinitionPower _powerEssenceTheft;

        public CustomBehaviorEssenceTheft(
            FeatureDefinitionPower powerEssenceTheft,
            ConditionDefinition conditionPossessed)
        {
            _powerEssenceTheft = powerEssenceTheft;
            _conditionPossessed = conditionPossessed;
        }

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            if (__instance.actionParams.RulesetEffect is not RulesetEffectPower rulesetEffectPower ||
                rulesetEffectPower.PowerDefinition != _powerEssenceTheft)
            {
                return true;
            }

            if (target.RulesetCharacter == null)
            {
                return true;
            }

            var isValid = !target.RulesetCharacter.HasConditionOfType(_conditionPossessed.Name);

            if (!isValid)
            {
                __instance.actionModifier.FailureFlags.Add("Tooltip/&MustNotHavePossessedCondition");
            }

            return isValid;
        }

        public IEnumerator OnMagicEffectInitiatedByMe(CharacterActionMagicEffect action, BaseDefinition power)
        {
            var actingCharacter = action.ActingCharacter;

            actingCharacter.UsedSpecialFeatures.TryAdd(_powerEssenceTheft.Name, 1);

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
