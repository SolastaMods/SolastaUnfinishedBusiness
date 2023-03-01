using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAdditionalDamages;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class RoguishArcaneScoundrel : AbstractSubclass
{
    internal const string Name = "RoguishArcaneScoundrel";
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
                    .SetGuiPresentationNoContent(true)
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
                    .SetGuiPresentationNoContent(true)
                    .SetAffinities(CharacterSavingThrowAffinity.Disadvantage, true,
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
            .SetNotificationTag(TagsDefinitions.AdditionalDamageSneakAttackTag)
            .SetDamageDice(DieType.D6, 1)
            .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 1, 2)
            .SetRequiredProperty(RestrictedContextRequiredProperty.FinesseOrRangeWeapon)
            .SetTriggerCondition(AdditionalDamageTriggerCondition.AdvantageOrNearbyAlly)
            .SetFirstTargetOnly(true)
            .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
            .SetCustomSubFeatures(new FeatureDefinitionCustomCodeDistractingAmbush())
            .SetConditionOperations(
                new ConditionOperationDescription
                {
                    hasSavingThrow = false,
                    operation = ConditionOperationDescription.ConditionOperation.Add,
                    conditionDefinition = conditionDistractingAmbush
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
            new ModifyMagicEffectCounterSpell(powerArcaneBacklash, conditionDistractingAmbush));

        var autoPreparedSpellsArcaneBackslash = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{Name}ArcaneBackslash")
            .SetGuiPresentation("ExpandedSpells", Category.Feature)
            .SetAutoTag("Roguish")
            .SetPreparedSpellGroups(BuildSpellGroup(13, Counterspell))
            .SetSpellcastingClass(CharacterClassDefinitions.Rogue)
            .AddToDB();

        /*
        Level 17 - Scoundrel's Gambit
        Starting at 17th level, you can use your action to cast Mirror Image as 2nd level spell, without expanding a spell slot, once per short rest.
        */

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

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private sealed class ModifyMagicEffectCounterSpell : IOnAfterActionFeature
    {
        private readonly ConditionDefinition _conditionDefinition;
        private readonly FeatureDefinitionPower _featureDefinitionPower;

        public ModifyMagicEffectCounterSpell(
            FeatureDefinitionPower featureDefinitionPower,
            ConditionDefinition conditionDefinition)
        {
            _featureDefinitionPower = featureDefinitionPower;
            _conditionDefinition = conditionDefinition;
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
                var defenderRulesetCharacter = gameLocationCharacter.RulesetCharacter;

                GameConsoleHelper.LogCharacterUsedPower(rulesetCharacter, _featureDefinitionPower);
                effectPower.ApplyEffectOnCharacter(defenderRulesetCharacter, true,
                    gameLocationCharacter.LocationPosition);

                var rulesetCondition = RulesetCondition.CreateActiveCondition(
                    defenderRulesetCharacter.Guid,
                    _conditionDefinition,
                    DurationType.Round,
                    1,
                    TurnOccurenceType.EndOfTurn,
                    rulesetCharacter.Guid,
                    rulesetCharacter.CurrentFaction.Name);

                defenderRulesetCharacter.AddConditionOfCategory(AttributeDefinitions.TagCombat, rulesetCondition);
            }
        }
    }

    private sealed class FeatureDefinitionCustomCodeDistractingAmbush : IFeatureDefinitionCustomCode
    {
        // remove original sneak attack as we've added a conditional one otherwise ours will never trigger
        public void ApplyFeature(RulesetCharacterHero hero, string tag)
        {
            foreach (var featureDefinitions in hero.ActiveFeatures.Values)
            {
                featureDefinitions.RemoveAll(x => x == AdditionalDamageRogueSneakAttack);
            }
        }
    }
}
