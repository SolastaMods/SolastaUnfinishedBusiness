using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Feats;
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
    internal const int DistractingAmbushLevel = 9;
    internal const string CastSpellName = $"CastSpell{Name}";

    private const string DistractingAmbush = "DistractingAmbush";
    private const string ConditionDistractingAmbushName = $"Condition{Name}{DistractingAmbush}";

    public RoguishArcaneScoundrel()
    {
        // LEVEL 3

        // Spell Casting

        var castSpell = FeatureDefinitionCastSpellBuilder
            .Create(CastSpellName)
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

        // Arcane Affinity

        var proficiencyCraftyArcana = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}Arcana")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Skill, SkillDefinitions.Arcana)
            .AddToDB();

        // Guileful Casting

        var magicAffinityGuilefulCasting = FeatureDefinitionMagicAffinityBuilder
            .Create($"MagicAffinity{Name}GuilefulCasting")
            .SetGuiPresentation(Category.Feature)
            .SetHandsFullCastingModifiers(true, false, true)
            .AddToDB();

        // LEVEL 9

        // Distracting Ambush

        var conditionDistractingAmbush = ConditionDefinitionBuilder
            .Create(ConditionDistractingAmbushName)
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionDistracted)
            .SetConditionType(ConditionType.Detrimental)
            .SetPossessive()
            .SetSpecialInterruptions(ConditionInterruption.AbilityCheck, ConditionInterruption.SavingThrow)
            .SetFeatures(
                FeatureDefinitionAbilityCheckAffinityBuilder
                    .Create($"AbilityCheckAffinity{Name}{DistractingAmbush}")
                    .SetGuiPresentation(ConditionDistractingAmbushName, Category.Condition, Gui.NoLocalization)
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
                    .SetGuiPresentation(ConditionDistractingAmbushName, Category.Condition, Gui.NoLocalization)
                    .SetAffinities(CharacterSavingThrowAffinity.Disadvantage, false,
                        AttributeDefinitions.Strength,
                        AttributeDefinitions.Dexterity,
                        AttributeDefinitions.Constitution,
                        AttributeDefinitions.Intelligence,
                        AttributeDefinitions.Wisdom,
                        AttributeDefinitions.Charisma)
                    .AddToDB())
            .AddToDB();

        // kept name for backward compatibility
        var additionalDamageDistractingAmbush = FeatureDefinitionBuilder
            .Create($"AdditionalDamage{Name}{DistractingAmbush}")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        // LEVEL 13

        // Arcane Backslash

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

        var powerArcaneBacklashSneakDamage = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ArcaneBackslash")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeForce, 1, DieType.D6)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 1, 1, 2, 2)
                            .Build(),
                        EffectFormBuilder.ConditionForm(conditionDistractingAmbush))
                    .Build())
            .AddToDB();

        powerArcaneBacklashSneakDamage.AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden,
            new MagicEffectFinishedByMeArcaneBackslash(
                powerArcaneBacklashSneakDamage, powerArcaneBackslashCounterSpell));

        // LEVEL 17

        // Essence Theft

        var conditionPossessed = ConditionDefinitionBuilder
            .Create($"Condition{Name}Possessed")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionPossessed)
            .SetConditionType(ConditionType.Detrimental)
            .SetPossessive()
            .AddToDB();

        // kept name for backward compatibility
        var powerPossessed = FeatureDefinitionPowerBuilder
            .Create($"AdditionalDamage{Name}Possessed")
            .SetGuiPresentation($"Condition{Name}Possessed", Category.Condition)
            .SetUsesFixed(ActivationTime.OnSneakAttackHitAuto)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 0, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 24, TargetType.IndividualsUnique)
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
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDiceAdvancement(LevelSourceType.CharacterLevel, 1, 1, 2, 19)
                            .SetDamageForm(DamageTypeForce, 4, DieType.D6)
                            .Build(),
                        EffectFormBuilder.ConditionForm(conditionDistractingAmbush))
                    .SetParticleEffectParameters(FeatureDefinitionPowers.PowerRoguishHoodlumDirtyFighting)
                    .Build())
            .AddToDB();

        powerEssenceTheft.AddCustomSubFeatures(
            new ValidatorsValidatePowerUse(CanUseEssenceTheft),
            new CustomBehaviorEssenceTheft(powerEssenceTheft, conditionPossessed));

        var featureSetTricksOfTheTrade = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}TricksOfTheTrade")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(powerPossessed, powerEssenceTheft)
            .AddToDB();

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.RoguishArcaneScoundrel, 256))
            .AddFeaturesAtLevel(3,
                castSpell, magicAffinityGuilefulCasting, proficiencyCraftyArcana)
            .AddFeaturesAtLevel(9,
                additionalDamageDistractingAmbush)
            .AddFeaturesAtLevel(13,
                autoPreparedSpellsArcaneBackslash, powerArcaneBacklashSneakDamage, powerArcaneBackslashCounterSpell)
            .AddFeaturesAtLevel(17,
                featureSetTricksOfTheTrade)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Rogue;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceRogueRoguishArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    internal static void InflictConditionDistractingAmbush(RulesetCharacter rulesetAttacker,
        RulesetCharacter rulesetDefender)
    {
        rulesetDefender.InflictCondition(
            ConditionDistractingAmbushName,
            DurationType.Round,
            1,
            TurnOccurenceType.EndOfSourceTurn,
            AttributeDefinitions.TagEffect,
            rulesetAttacker.guid,
            rulesetAttacker.CurrentFaction.Name,
            1,
            ConditionDistractingAmbushName,
            0,
            0,
            0);
    }

    private static bool CanUseEssenceTheft(RulesetCharacter character)
    {
        var gameLocationCharacter = GameLocationCharacter.GetFromActor(character);

        return Gui.Battle != null &&
               gameLocationCharacter != null &&
               gameLocationCharacter.UsedSpecialFeatures.ContainsKey(AdditionalDamageRogueSneakAttack.Name);
    }

    private sealed class ModifyEffectDescriptionArcaneBackslashCounterSpell(
        FeatureDefinitionPower powerArcaneBackslashCounterSpell) : IModifyEffectDescription
    {
        public bool IsValid(
            BaseDefinition definition,
            RulesetCharacter character,
            EffectDescription effectDescription)
        {
            return definition == powerArcaneBackslashCounterSpell &&
                   character.GetClassLevel(CharacterClassDefinitions.Rogue) >= 19;
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

    private sealed class MagicEffectFinishedByMeArcaneBackslash(
        FeatureDefinitionPower powerArcaneBackslash,
        FeatureDefinitionPower powerCounterSpell) : IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(
            CharacterAction action,
            GameLocationCharacter attacker,
            List<GameLocationCharacter> targets)
        {
            if (!action.ActionParams.TargetAction.Countered ||
                (action.ActionParams.RulesetEffect.SourceDefinition != Counterspell &&
                 action.ActionParams.RulesetEffect.SourceDefinition != powerCounterSpell))
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerArcaneBackslash, rulesetAttacker);

            attacker.UsedSpecialFeatures.TryAdd(AdditionalDamageRogueSneakAttack.Name, 1);

            //TODO: check if MyExecuteActionSpendPower works here
            attacker.MyExecuteActionPowerNoCost(usablePower, [.. targets]);
        }
    }

    private sealed class CustomBehaviorEssenceTheft(
        FeatureDefinitionPower powerEssenceTheft,
        ConditionDefinition conditionPossessed) : IFilterTargetingCharacter, IModifyEffectDescription
    {
        public bool EnforceFullSelection => false;

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            if (target.RulesetCharacter == null)
            {
                return false;
            }

            var isValid = !target.RulesetCharacter.HasConditionOfType(conditionPossessed.Name);

            if (!isValid)
            {
                __instance.actionModifier.FailureFlags.Add("Tooltip/&MustNotHavePossessedCondition");
            }

            return isValid;
        }

        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == powerEssenceTheft;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var hero = character.GetOriginalHero();

            if (hero != null &&
                (hero.TrainedFeats.Contains(ClassFeats.CloseQuartersDex) ||
                 hero.TrainedFeats.Contains(ClassFeats.CloseQuartersInt)))
            {
                effectDescription.EffectForms[0].DamageForm.DieType = DieType.D8;
            }

            return effectDescription;
        }
    }
}
