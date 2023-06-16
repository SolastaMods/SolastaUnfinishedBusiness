using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
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
            new ActionFinishedArcaneBackslash(
                powerArcaneBacklash,
                powerArcaneBackslashCounterSpell,
                conditionDistractingAmbush));

        //
        // LEVEL 17
        //

        const string ADDITIONAL_DAMAGE_POSSESSED = $"AdditionalDamage{Name}Possessed";
        const string POWER_ESSENCE_THEFT = $"Power{Name}EssenceTheft";

        var conditionPossessed = ConditionDefinitionBuilder
            .Create($"Condition{Name}Possessed")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionPossessed)
            .SetConditionType(ConditionType.Detrimental)
            .SetPossessive()
            .SetSpecialDuration(DurationType.Round, 0, TurnOccurenceType.EndOfSourceTurn)
            .AddToDB();

        var additionalDamagePossessed = FeatureDefinitionAdditionalDamageBuilder
            .Create(AdditionalDamageRogueSneakAttack, ADDITIONAL_DAMAGE_POSSESSED)
            .SetNotificationTag(TagsDefinitions.AdditionalDamageSneakAttackTag)
            .SetDamageDice(DieType.D6, 1)
            .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 1, 2)
            .SetRequiredProperty(RestrictedContextRequiredProperty.FinesseOrRangeWeapon)
            .SetTriggerCondition(AdditionalDamageTriggerCondition.AdvantageOrNearbyAlly)
            .SetFrequencyLimit(FeatureLimitedUsage.OnceInMyTurn)
            .SetConditionOperations(
                new ConditionOperationDescription
                {
                    operation = ConditionOperationDescription.ConditionOperation.Add,
                    conditionDefinition = conditionPossessed
                })
            .SetCustomSubFeatures(new CustomCodeAdditionalDamagePossessed())
            .AddToDB();


        static bool CanUseEssenceTheft(RulesetCharacter character)
        {
            var gameLocationCharacter = GameLocationCharacter.GetFromActor(character);

            return gameLocationCharacter != null &&
                   gameLocationCharacter.UsedSpecialFeatures.ContainsKey(ADDITIONAL_DAMAGE_POSSESSED);
        }

        var powerEssenceTheft = FeatureDefinitionPowerBuilder
            .Create(POWER_ESSENCE_THEFT)
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

        powerEssenceTheft.SetCustomSubFeatures(
            new ValidatorsPowerUse(CanUseEssenceTheft),
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
            .SetCustomSubFeatures(new CustomCodePremeditationSlot4())
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
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var level = character.GetClassLevel(CharacterClassDefinitions.Rogue);

            if (level < 19)
            {
                return effectDescription;
            }

            effectDescription.effectForms[0].CounterForm.automaticSpellLevel = 4;

            return effectDescription;
        }
    }

    private sealed class ActionFinishedArcaneBackslash : IActionFinished
    {
        private readonly ConditionDefinition _conditionDistractingAmbush;
        private readonly FeatureDefinitionPower _powerArcaneBackslash;
        private readonly FeatureDefinitionPower _powerCounterSpell;

        public ActionFinishedArcaneBackslash(
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
                (action is not CharacterActionUsePower characterActionUsePower ||
                 characterActionUsePower.activePower.PowerDefinition != _powerCounterSpell ||
                 !characterActionUsePower.ActionParams.TargetAction.Countered))
            {
                yield break;
            }

            var actingCharacter = action.ActingCharacter;
            var rulesetCharacter = actingCharacter.RulesetCharacter;
            var usablePower = UsablePowersProvider.Get(_powerArcaneBackslash, rulesetCharacter);
            var effectPower = new RulesetEffectPower(rulesetCharacter, usablePower);

            actingCharacter.UsedSpecialFeatures.TryAdd(AdditionalDamageRogueSneakAttack.Name, 1);

            foreach (var gameLocationCharacter in action.actionParams.TargetCharacters)
            {
                var rulesetDefender = gameLocationCharacter.RulesetCharacter;

                rulesetCharacter.LogCharacterUsedPower(_powerArcaneBackslash);
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

    private sealed class CustomCodeAdditionalDamagePossessed : IFeatureDefinitionCustomCode, IClassHoldingFeature
    {
        public CharacterClassDefinition Class => CharacterClassDefinitions.Rogue;

        public void ApplyFeature(RulesetCharacterHero hero, string tag)
        {
            foreach (var featureDefinitions in hero.ActiveFeatures.Values)
            {
                featureDefinitions.RemoveAll(x => x == AdditionalDamageRogueSneakAttack);
            }
        }

        public void RemoveFeature(RulesetCharacterHero hero, string tag)
        {
            // Empty
        }
    }

    private sealed class CustomBehaviorEssenceTheft : IActionInitiated, IFilterTargetingMagicEffect
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

        public IEnumerator OnActionInitiated(CharacterAction characterAction)
        {
            if (characterAction is not CharacterActionUsePower characterActionUsePower ||
                characterActionUsePower.activePower.PowerDefinition != _powerEssenceTheft)
            {
                yield break;
            }

            var actingCharacter = characterAction.ActingCharacter;

            actingCharacter.UsedSpecialFeatures.TryAdd(_powerEssenceTheft.Name, 1);

            var damage = characterAction.ActionParams.RulesetEffect.EffectDescription.FindFirstDamageForm();

            // this currently works as there is only one feature in game using DamageDieProviderFromCharacter
            // we might need to change this to a proper interface if others start using it
            var hasSneakAttackDieTypeChange = actingCharacter.RulesetCharacter
                .GetSubFeaturesByType<DamageDieProviderFromCharacter>()
                .Any();

            if (!hasSneakAttackDieTypeChange)
            {
                damage.dieType = DieType.D6;
                yield break;
            }

            damage.dieType = DieType.D8;
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
    }

    private sealed class CustomCodePremeditationSlot4 : IFeatureDefinitionCustomCode
    {
        public void ApplyFeature(RulesetCharacterHero hero, string tag)
        {
            foreach (var featureDefinitions in hero.ActiveFeatures.Values)
            {
                featureDefinitions.RemoveAll(
                    x => x == MagicAffinityAdditionalSpellSlot4);
            }
        }

        public void RemoveFeature(RulesetCharacterHero hero, string tag)
        {
            // Empty
        }
    }
}
