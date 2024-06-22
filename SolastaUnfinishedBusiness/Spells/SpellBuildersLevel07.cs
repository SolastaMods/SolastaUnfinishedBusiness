using System.Collections;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaUnfinishedBusiness.Spells;

internal static partial class SpellBuilders
{
    #region Reverse Gravity

    internal static SpellDefinition BuildReverseGravity()
    {
        const string NAME = "ReverseGravity";

        return SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.ReverseGravity, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(7)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.All, RangeType.Distance, 12, TargetType.Cylinder, 10, 10)
                    .SetSavingThrowData(false, AttributeDefinitions.Dexterity, true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                ConditionDefinitionBuilder
                                    .Create(ConditionDefinitions.ConditionLevitate, "ConditionReverseGravity")
                                    .SetOrUpdateGuiPresentation(Category.Condition)
                                    .SetConditionType(ConditionType.Neutral)
                                    .SetParentCondition(ConditionDefinitions.ConditionFlying)
                                    .SetFeatures(FeatureDefinitionMoveModes.MoveModeFly2)
                                    .AddToDB(),
                                ConditionForm.ConditionOperation.Add)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(
                                MotionForm.MotionType.Levitate,
                                10)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .SetRecurrentEffect(Entangle.EffectDescription.RecurrentEffect)
                    .Build())
            .AddToDB();
    }

    #endregion

    #region Draconic Transformation

    internal static SpellDefinition BuildDraconicTransformation()
    {
        const string NAME = "DraconicTransformation";

        var sprite = Sprites.GetSprite(NAME, Resources.DraconicTransformation, 128);

        var conditionMark = ConditionDefinitionBuilder
            .Create($"Condition{NAME}Mark")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddToDB();

        var power = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}")
            .SetGuiPresentation(Category.Feature, sprite)
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Cone, 12)
                    .SetSavingThrowData(false,
                        AttributeDefinitions.Dexterity,
                        false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypeForce, 6, DieType.D8)
                            .Build())
                    .SetParticleEffectParameters(ConeOfCold)
                    .SetCasterEffectParameters(GravitySlam)
                    .SetImpactEffectParameters(EldritchBlast)
                    .Build())
            .AddToDB();

        power.disableIfConditionIsOwned = conditionMark;

        var condition = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionFlyingAdaptive, $"Condition{NAME}")
            .SetGuiPresentation(NAME, Category.Spell, ConditionDefinitions.ConditionFlying)
            .SetPossessive()
            .SetParentCondition(ConditionDefinitions.ConditionFlying)
            .SetFeatures(
                power,
                FeatureDefinitionMoveModes.MoveModeFly12,
                FeatureDefinitionSenses.SenseBlindSight6)
            .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
            .AddToDB();

        condition.GuiPresentation.description = Gui.NoLocalization;

        return SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, sprite)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(7)
            .SetCastingTime(ActivationTime.BonusAction)
            .SetMaterialComponent(MaterialComponentType.Specific)
            .SetSpecificMaterialComponent(TagsDefinitions.ItemTagDiamond, 500, false)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Cone, 12)
                    .SetSavingThrowData(false,
                        AttributeDefinitions.Dexterity,
                        false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(
                            conditionMark,
                            ConditionForm.ConditionOperation.Add, true),
                        EffectFormBuilder.ConditionForm(
                            condition,
                            ConditionForm.ConditionOperation.Add, true),
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypeForce, 6, DieType.D8)
                            .Build())
                    .SetParticleEffectParameters(ConeOfCold)
                    .SetCasterEffectParameters(GravitySlam)
                    .SetImpactEffectParameters(EldritchBlast)
                    .Build())
            .AddToDB();
    }

    #endregion

    #region Crown of Stars

    internal static SpellDefinition BuildCrownOfStars()
    {
        const string NAME = "CrownOfStars";

        var sprite = Sprites.GetSprite($"Power{NAME}", Resources.CrownOfStars, 128);
        var powerCrownOfStars = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}")
            .SetGuiPresentation(Category.Feature, sprite)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.None, 1, 7)
            .SetUseSpellAttack()
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.RangeHit, 24, TargetType.IndividualsUnique)
                    .SetEffectForms(EffectFormBuilder.DamageForm(DamageTypeRadiant, 4, DieType.D12))
                    .SetParticleEffectParameters(ShadowDagger)
                    .SetParticleEffectParameters(GuidingBolt)
                    .SetCasterEffectParameters(PowerPaladinAuraOfCourage)
                    .Build())
            .AddToDB();

        var conditionCrownOfStars = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation($"Power{NAME}", Category.Feature, ConditionGuided)
            .SetPossessive()
            .SetConditionType(ConditionType.Beneficial)
            .SetFeatures(powerCrownOfStars)
            .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
            .CopyParticleReferences(DeathWard)
            .AddToDB();

        conditionCrownOfStars.GuiPresentation.description = Gui.NoLocalization;

        var lightSourceForm = FaerieFire.EffectDescription
            .GetFirstFormOfType(EffectForm.EffectFormType.LightSource).LightSourceForm;

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, sprite)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(7)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Hour, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel,
                        additionalSummonsPerIncrement: 2)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(conditionCrownOfStars),
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetLightSourceForm(
                                LightSourceType.Basic, 6, 6, lightSourceForm.Color,
                                lightSourceForm.graphicsPrefabReference)
                            .Build())
                    .SetCasterEffectParameters(Sparkle)
                    .SetEffectEffectParameters(PowerOathOfJugementPurgeCorruption)
                    .Build())
            .AddCustomSubFeatures(new PowerOrSpellFinishedByMeSpellCrownOfStars(conditionCrownOfStars))
            .AddToDB();

        powerCrownOfStars.AddCustomSubFeatures(
            HasModifiedUses.Marker,
            new ModifyPowerPoolAmount
            {
                PowerPool = powerCrownOfStars,
                Type = PowerPoolBonusCalculationType.Fixed,
                Attribute = conditionCrownOfStars.Name,
                Value = 2
            },
            new CustomBehaviorPowerCrownOfStars(spell, conditionCrownOfStars));

        return spell;
    }

    private sealed class CustomBehaviorPowerCrownOfStars(
        SpellDefinition spellCrownOfStars,
        ConditionDefinition conditionCrownOfStars) : IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;

            if (baseDefinition is not FeatureDefinitionPower powerMotes)
            {
                yield break;
            }

            var remainingUses = action.ActingCharacter.RulesetCharacter.GetRemainingPowerUses(powerMotes);

            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (remainingUses == 0 &&
                action.ActingCharacter.RulesetCharacter.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionCrownOfStars.Name, out var activeCondition))
            {
                rulesetCharacter.RemoveCondition(activeCondition);
            }
            else if (remainingUses < 4)
            {
                var usablePower =
                    rulesetCharacter.SpellsCastByMe.FirstOrDefault(x => x.SpellDefinition == spellCrownOfStars);

                if (usablePower != null &&
                    usablePower.TrackedLightSourceGuids.Count > 0 &&
                    usablePower.TrackedLightSourceGuids[0] == rulesetCharacter.PersonalLightSource.Guid)
                {
                    rulesetCharacter.PersonalLightSource.brightRange = 0;
                }
            }
        }
    }

    // ReSharper disable once SuggestBaseTypeForParameterInConstructor
    private sealed class PowerOrSpellFinishedByMeSpellCrownOfStars(ConditionDefinition conditionCrownOfStars)
        : IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            if (action is not CharacterActionCastSpell actionCastSpell)
            {
                yield break;
            }

            foreach (var rulesetTarget in action.ActionParams.TargetCharacters
                         .Select(targetCharacter => targetCharacter.RulesetCharacter))
            {
                if (rulesetTarget.TryGetConditionOfCategoryAndType(
                        AttributeDefinitions.TagEffect,
                        conditionCrownOfStars.Name,
                        out var activeCondition))
                {
                    activeCondition.Amount = actionCastSpell.ActiveSpell.EffectLevel;
                }
            }
        }
    }

    #endregion
}
