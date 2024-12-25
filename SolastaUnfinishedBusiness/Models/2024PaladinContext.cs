using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;

namespace SolastaUnfinishedBusiness.Models;

internal static partial class Tabletop2024Context
{
    private static readonly FeatureDefinitionAttributeModifier AttributeModifierPaladinChannelDivinity11 =
        FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierPaladinChannelDivinity11")
            .SetGuiPresentationNoContent(true)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.ChannelDivinityNumber, 1)
            .AddToDB();

    private static readonly ConditionDefinition ConditionFrightenedByAbjureFoes = ConditionDefinitionBuilder
        .Create(ConditionDefinitions.ConditionFrightened, "ConditionFrightenedByAbjureFoes")
        .SetParentCondition(ConditionDefinitions.ConditionFrightened)
        .SetSpecialInterruptions(ConditionInterruption.Damaged)
        .SetFeatures()
        .AddCustomSubFeatures(new ActionFinishedByMeCheckBonusOrMainOrMove())
        .AddToDB();

    private static readonly FeatureDefinitionPower PowerPaladinAbjureFoes = FeatureDefinitionPowerBuilder
        .Create("PowerPaladinAbjureFoes")
        .SetGuiPresentation(Category.Feature,
            Sprites.GetSprite("PowerPaladinAbjureFoes", Resources.PowerPaladinAbjureFoes, 256, 128))
        .SetUsesFixed(ActivationTime.Action, RechargeRate.ChannelDivinity)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetDurationData(DurationType.Minute, 1)
                .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
                .SetSavingThrowData(false, AttributeDefinitions.Wisdom, true,
                    EffectDifficultyClassComputation.SpellCastingFeature)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .HasSavingThrow(EffectSavingThrowType.Negates)
                        .SetConditionForm(ConditionFrightenedByAbjureFoes, ConditionForm.ConditionOperation.Add)
                        .Build())
                .SetCasterEffectParameters(PowerClericTurnUndead)
                .Build())
        .AddCustomSubFeatures(new ModifyEffectDescriptionPowerPaladinAbjureFoes())
        .AddToDB();

    private static readonly FeatureDefinitionPower PowerPaladinRestoringTouch = FeatureDefinitionPowerBuilder
        .Create("PowerPaladinRestoringTouch")
        .SetGuiPresentation(Category.Feature)
        .SetUsesFixed(ActivationTime.NoCost, RechargeRate.HealingPool, 5)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.IndividualsUnique)
                .Build())
        .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
        .AddToDB();

    private static readonly ConditionDefinition[] RestoringTouchConditions =
    [
        ConditionDefinitions.ConditionFrightened,
        ConditionDefinitions.ConditionBlinded,
        ConditionDefinitions.ConditionCharmed,
        ConditionDefinitions.ConditionParalyzed,
        ConditionDefinitions.ConditionStunned
    ];

    private static void LoadPaladinRestoringTouch()
    {
        PowerPaladinLayOnHands.AddCustomSubFeatures(new PowerOrSpellFinishedByMeRestoringTouch());

        var powers = new List<FeatureDefinitionPower>();

        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var condition in RestoringTouchConditions)
        {
            var conditionTitle = condition.FormatTitle();
            var title = "PowerPaladinRestoringTouchSubPowerTitle".Formatted(Category.Feature, conditionTitle);
            var description =
                "PowerPaladinRestoringTouchSubPowerDescription".Formatted(Category.Feature, conditionTitle);
            var power = FeatureDefinitionPowerSharedPoolBuilder
                .Create($"PowerPaladinRestoringTouch{condition.Name}")
                .SetGuiPresentation(title, description)
                .SetSharedPool(ActivationTime.NoCost, PowerPaladinRestoringTouch, 5)
                .SetEffectDescription(
                    EffectDescriptionBuilder
                        .Create()
                        .SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.IndividualsUnique)
                        .SetEffectForms(
                            EffectFormBuilder
                                .Create()
                                .SetConditionForm(condition, ConditionForm.ConditionOperation.Remove)
                                .Build())
                        .Build())
                .AddToDB();

            powers.Add(power);
        }

        PowerBundle.RegisterPowerBundle(PowerPaladinRestoringTouch, false, [.. powers]);
    }

    internal static void SwitchPaladinSpellCastingAtOne()
    {
        var level = Main.Settings.EnablePaladinSpellCastingAtLevel1 ? 1 : 2;

        foreach (var featureUnlock in Paladin.FeatureUnlocks
                     .Where(x => x.FeatureDefinition == FeatureDefinitionCastSpells.CastSpellPaladin))
        {
            featureUnlock.level = level;
        }

        // allows back and forth compatibility with EnableRitualOnAllCasters2024
        foreach (var featureUnlock in Paladin.FeatureUnlocks
                     .Where(x => x.FeatureDefinition == FeatureSetClericRitualCasting))
        {
            featureUnlock.level = level;
        }

        Paladin.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);

        if (Main.Settings.EnablePaladinSpellCastingAtLevel1)
        {
            FeatureDefinitionCastSpells.CastSpellPaladin.slotsPerLevels = SharedSpellsContext.HalfRoundUpCastingSlots;
            SharedSpellsContext.ClassCasterType[PaladinClass] =
                FeatureDefinitionCastSpellBuilder.CasterProgression.HalfRoundUp;
        }
        else
        {
            FeatureDefinitionCastSpells.CastSpellPaladin.slotsPerLevels = SharedSpellsContext.HalfCastingSlots;
            SharedSpellsContext.ClassCasterType[PaladinClass] =
                FeatureDefinitionCastSpellBuilder.CasterProgression.Half;
        }
    }

    internal static void SwitchPaladinAbjureFoes()
    {
        Paladin.FeatureUnlocks
            .RemoveAll(x => x.FeatureDefinition == PowerPaladinAbjureFoes);

        if (Main.Settings.EnablePaladinAbjureFoes2024)
        {
            Paladin.FeatureUnlocks.Add(new FeatureUnlockByLevel(PowerPaladinAbjureFoes, 9));
        }

        Paladin.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchPaladinChannelDivinity()
    {
        if (Main.Settings.EnablePaladinChannelDivinity2024)
        {
            AttributeModifierPaladinChannelDivinity.modifierValue = 2;
            AttributeModifierPaladinChannelDivinity.GuiPresentation.description =
                "Feature/&PaladinChannelDivinityDescription";
            Paladin.FeatureUnlocks.Add(new FeatureUnlockByLevel(AttributeModifierPaladinChannelDivinity11, 11));
        }
        else
        {
            AttributeModifierPaladinChannelDivinity.modifierValue = 1;
            AttributeModifierPaladinChannelDivinity.GuiPresentation.description =
                "Feature/&ClericChannelDivinityDescription";
            Paladin.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == AttributeModifierPaladinChannelDivinity11);
        }

        Paladin.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchPaladinLayOnHand()
    {
        PowerPaladinLayOnHands.activationTime = Main.Settings.EnablePaladinLayOnHands2024
            ? ActivationTime.BonusAction
            : ActivationTime.Action;
        PowerPaladinNeutralizePoison.activationTime = Main.Settings.EnablePaladinLayOnHands2024
            ? ActivationTime.BonusAction
            : ActivationTime.Action;
    }

    internal static void SwitchPaladinRestoringTouch()
    {
        Paladin.FeatureUnlocks
            .RemoveAll(x =>
                x.FeatureDefinition == PowerPaladinRestoringTouch ||
                x.FeatureDefinition == PowerPaladinCleansingTouch);

        Paladin.FeatureUnlocks.Add(Main.Settings.EnablePaladinRestoringTouch2024
            ? new FeatureUnlockByLevel(PowerPaladinRestoringTouch, 14)
            : new FeatureUnlockByLevel(PowerPaladinCleansingTouch, 14));

        Paladin.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    private sealed class ModifyEffectDescriptionPowerPaladinAbjureFoes : IModifyEffectDescription
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == PowerPaladinAbjureFoes;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var charisma = character.TryGetAttributeValue(AttributeDefinitions.Charisma);
            var charismaModifier = AttributeDefinitions.ComputeAbilityScoreModifier(charisma);
            var targets = Math.Max(1, charismaModifier);

            effectDescription.targetParameter = targets;

            return effectDescription;
        }
    }

    private sealed class PowerOrSpellFinishedByMeRestoringTouch : IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            if (!Main.Settings.EnablePaladinRestoringTouch2024)
            {
                yield break;
            }

            var aborted = false;
            var caster = action.ActingCharacter;
            var rulesetCaster = caster.RulesetCharacter;
            var target = action.ActionParams.TargetCharacters[0];
            var rulesetTarget = target.RulesetCharacter;
            var usablePowerPool = PowerProvider.Get(PowerPaladinRestoringTouch, rulesetCaster);

            while (!aborted && rulesetCaster.GetRemainingUsesOfPower(usablePowerPool) > 0)
            {
                var usablePowers = RestoringTouchConditions
                    .Where(x => rulesetTarget.HasConditionOfTypeOrSubType(x.Name))
                    .Select(x =>
                        PowerProvider.Get(
                            GetDefinition<FeatureDefinitionPower>($"PowerPaladinRestoringTouch{x.Name}"),
                            rulesetCaster))
                    .ToArray();

                if (usablePowers.Length == 0)
                {
                    yield break;
                }

                rulesetCaster.UsablePowers.AddRange(usablePowers);

                yield return caster.MyReactToSpendPowerBundle(
                    usablePowerPool,
                    [target],
                    caster,
                    "RestoringTouch",
                    reactionNotValidated: _ => aborted = true);

                foreach (var usablePower in usablePowers)
                {
                    rulesetCaster.UsablePowers.Remove(usablePower);
                }
            }
        }
    }
}
