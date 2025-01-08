using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static ActionDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ActionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.MonsterDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionConditionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ItemDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionRegenerations;

namespace SolastaUnfinishedBusiness.Models;

internal static class Tabletop2014Context
{
    internal static readonly HashSet<MonsterDefinition> ConjuredMonsters =
    [
        ConjuredOneBeastTiger_Drake,
        ConjuredTwoBeast_Direwolf,
        ConjuredFourBeast_BadlandsSpider,
        ConjuredEightBeast_Wolf,

        // Conjure minor elemental (4)
        SkarnGhoul, // CR 2
        WindSnake, // CR 2
        Fire_Jester, // CR 1

        // Conjure woodland beings (4) - not implemented

        // Conjure elemental (5)
        Air_Elemental, // CR 5
        Fire_Elemental, // CR 5
        Earth_Elemental, // CR 5

        InvisibleStalker, // CR 6

        // Conjure fey (6)
        FeyGiantApe, // CR 6
        FeyGiant_Eagle, // CR 5
        FeyBear, // CR 4
        Green_Hag, // CR 3
        FeyWolf, // CR 2
        FeyDriad
    ];

    internal static readonly FeatureDefinitionActionAffinity ActionAffinityConditionBlind =
        FeatureDefinitionActionAffinityBuilder
            .Create("ActionAffinityConditionBlind")
            .SetGuiPresentationNoContent(true)
            .SetForbiddenActions(Id.AttackOpportunity)
            .AddToDB();

    internal static readonly FeatureDefinitionPower FeatureDefinitionPowerHelpAction = FeatureDefinitionPowerBuilder
        .Create("PowerHelp")
        .SetGuiPresentation(Category.Feature, Sprites.GetSprite("PowerHelp", Resources.PowerHelp, 256, 128))
        .SetUsesFixed(ActivationTime.Action)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                .SetTargetingData(Side.Enemy, RangeType.Touch, 0, TargetType.IndividualsUnique)
                .SetEffectForms(EffectFormBuilder.ConditionForm(CustomConditionsContext.Distracted))
                .Build())
        .SetUniqueInstance()
        .AddToDB();

    internal static void SwitchFullyControlConjurations()
    {
        foreach (var conjuredMonster in ConjuredMonsters)
        {
            conjuredMonster.fullyControlledWhenAllied = Main.Settings.FullyControlConjurations;
        }
    }

    internal static void LateLoad()
    {
        LoadGravitySlam();
        SwitchBardHealingBalladOnLongRest();
        SwitchColdResistanceAndImmunityAlsoGrantsWeatherImmunity();
        SwitchConditionBlindedShouldNotAllowOpportunityAttack();
        SwitchEldritchBlastRange();
        SwitchFullyControlConjurations();
        SwitchGrappleAction();
        SwitchGravitySlam();
        SwitchHelpPower();
        SwitchOfficialFoodRationsWeight();
        SwitchProneAction();
        SwitchRingOfRegenerationHealRate();
    }

    internal static void SwitchEldritchBlastRange()
    {
        EldritchBlast.effectDescription.rangeParameter = Main.Settings.FixEldritchBlastRange ? 24 : 16;
    }

    internal static void SwitchBardHealingBalladOnLongRest()
    {
        FeatureDefinitionRestHealingModifiers.RestHealingModifierBardHealingBallad.applyDuringLongRest =
            Main.Settings.EnableBardHealingBalladOnLongRest;
    }

    internal static void SwitchRingOfRegenerationHealRate()
    {
        var ringDefinition = RegenerationRing;

        if (Main.Settings.FixRingOfRegenerationHealRate)
        {
            // Heal by 1 hp per 3 minutes which is roughly the same as 
            // RAW of 1d6 (avg 3.5) every 10 minutes.
            ringDefinition.tickType = DurationType.Minute;
            ringDefinition.tickNumber = 3;
            ringDefinition.diceNumber = 1;
        }
        else
        {
            ringDefinition.tickType = DurationType.Round;
            ringDefinition.tickNumber = 1;
            ringDefinition.diceNumber = 2;
        }

        ringDefinition.dieType = DieType.D1;
    }

    internal static void SwitchOfficialFoodRationsWeight()
    {
        var foodSrdWeight = Food_Ration;
        var foodForagedSrdWeight = Food_Ration_Foraged;

        if (Main.Settings.UseOfficialFoodRationsWeight)
        {
            foodSrdWeight.weight = 2.0f;
            foodForagedSrdWeight.weight = 2.0f;
        }
        else
        {
            foodSrdWeight.weight = 3.0f;
            foodForagedSrdWeight.weight = 3.0f;
        }
    }

    internal static void SwitchColdResistanceAndImmunityAlsoGrantsWeatherImmunity()
    {
        foreach (var featureSet in DatabaseRepository.GetDatabase<FeatureDefinitionFeatureSet>())
        {
            if (Main.Settings.ColdResistanceAlsoGrantsImmunityToChilledCondition)
            {
                if (featureSet.FeatureSet.Contains(FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance))
                {
                    featureSet.FeatureSet.TryAdd(ConditionAffinityWeatherChilledImmunity);
                    featureSet.FeatureSet.TryAdd(ConditionAffinityWeatherFrozenImmunity);
                }
            }
            else
            {
                featureSet.FeatureSet.Remove(ConditionAffinityWeatherChilledImmunity);
                featureSet.FeatureSet.Remove(ConditionAffinityWeatherFrozenImmunity);
            }

            if (Main.Settings.ColdImmunityAlsoGrantsImmunityToChilledAndFrozenCondition)
            {
                // ReSharper disable once InvertIf
                if (featureSet.FeatureSet.Contains(FeatureDefinitionDamageAffinitys.DamageAffinityColdImmunity))
                {
                    featureSet.FeatureSet.TryAdd(ConditionAffinityWeatherChilledImmunity);
                    featureSet.FeatureSet.TryAdd(ConditionAffinityWeatherChilledInsteadOfFrozenImmunity);
                    featureSet.FeatureSet.TryAdd(ConditionAffinityWeatherFrozenImmunity);
                }
            }
            else
            {
                featureSet.FeatureSet.Remove(ConditionAffinityWeatherChilledImmunity);
                featureSet.FeatureSet.Remove(ConditionAffinityWeatherChilledInsteadOfFrozenImmunity);
                featureSet.FeatureSet.Remove(ConditionAffinityWeatherFrozenImmunity);
            }
        }

        foreach (var condition in DatabaseRepository.GetDatabase<ConditionDefinition>())
        {
            if (Main.Settings.ColdResistanceAlsoGrantsImmunityToChilledCondition)
            {
                if (condition.Features.Contains(FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance))
                {
                    condition.Features.TryAdd(ConditionAffinityWeatherChilledImmunity);
                    condition.Features.TryAdd(ConditionAffinityWeatherFrozenImmunity);
                }
            }
            else
            {
                condition.Features.Remove(ConditionAffinityWeatherChilledImmunity);
                condition.Features.Remove(ConditionAffinityWeatherFrozenImmunity);
            }

            if (Main.Settings.ColdImmunityAlsoGrantsImmunityToChilledAndFrozenCondition)
            {
                // ReSharper disable once InvertIf
                if (condition.Features.Contains(FeatureDefinitionDamageAffinitys.DamageAffinityColdImmunity))
                {
                    condition.Features.TryAdd(ConditionAffinityWeatherChilledImmunity);
                    condition.Features.TryAdd(ConditionAffinityWeatherChilledInsteadOfFrozenImmunity);
                    condition.Features.TryAdd(ConditionAffinityWeatherFrozenImmunity);
                }
            }
            else
            {
                condition.Features.Remove(ConditionAffinityWeatherChilledImmunity);
                condition.Features.Remove(ConditionAffinityWeatherChilledInsteadOfFrozenImmunity);
                condition.Features.Remove(ConditionAffinityWeatherFrozenImmunity);
            }
        }

        foreach (var item in DatabaseRepository.GetDatabase<ItemDefinition>())
        {
            if (Main.Settings.ColdResistanceAlsoGrantsImmunityToChilledCondition)
            {
                var itemProperty = item.staticProperties.FirstOrDefault(x =>
                    x.FeatureDefinition == FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance);

                if (itemProperty != null)
                {
                    item.staticProperties.TryAdd(new ItemPropertyDescription(itemProperty)
                    {
                        featureDefinition = ConditionAffinityWeatherChilledImmunity
                    });
                    item.staticProperties.TryAdd(new ItemPropertyDescription(itemProperty)
                    {
                        featureDefinition = ConditionAffinityWeatherFrozenImmunity
                    });
                }
            }
            else
            {
                item.staticProperties.RemoveAll(x => x.FeatureDefinition == ConditionAffinityWeatherChilledImmunity);
                item.staticProperties.RemoveAll(x => x.FeatureDefinition == ConditionAffinityWeatherFrozenImmunity);
            }

            if (Main.Settings.ColdImmunityAlsoGrantsImmunityToChilledAndFrozenCondition)
            {
                var itemProperty = item.staticProperties.FirstOrDefault(x =>
                    x.FeatureDefinition == FeatureDefinitionDamageAffinitys.DamageAffinityColdImmunity);

                // ReSharper disable once InvertIf
                if (itemProperty != null)
                {
                    item.staticProperties.TryAdd(new ItemPropertyDescription(itemProperty)
                    {
                        featureDefinition = ConditionAffinityWeatherChilledImmunity
                    });
                    item.staticProperties.TryAdd(new ItemPropertyDescription(itemProperty)
                    {
                        featureDefinition = ConditionAffinityWeatherChilledInsteadOfFrozenImmunity
                    });
                    item.staticProperties.TryAdd(new ItemPropertyDescription(itemProperty)
                    {
                        featureDefinition = ConditionAffinityWeatherFrozenImmunity
                    });
                }
            }
            else
            {
                item.staticProperties.RemoveAll(x => x.FeatureDefinition == ConditionAffinityWeatherChilledImmunity);
                item.staticProperties.RemoveAll(x =>
                    x.FeatureDefinition == ConditionAffinityWeatherChilledInsteadOfFrozenImmunity);
                item.staticProperties.RemoveAll(x => x.FeatureDefinition == ConditionAffinityWeatherFrozenImmunity);
            }
        }
    }

    internal static void SwitchConditionBlindedShouldNotAllowOpportunityAttack()
    {
        if (Main.Settings.BlindedConditionDontAllowAttackOfOpportunity)
        {
            ConditionDefinitions.ConditionInvisibleBase.Features.TryAdd(ActionAffinityConditionBlind);
            ConditionDefinitions.ConditionBlinded.Features.TryAdd(ActionAffinityConditionBlind);
            LightingAndObscurementContext.ConditionBlindedByDarkness.Features.TryAdd(ActionAffinityConditionBlind);
        }
        else
        {
            ConditionDefinitions.ConditionInvisibleBase.Features.Remove(ActionAffinityConditionBlind);
            ConditionDefinitions.ConditionBlinded.Features.Remove(ActionAffinityConditionBlind);
            LightingAndObscurementContext.ConditionBlindedByDarkness.Features.Remove(ActionAffinityConditionBlind);
        }
    }

    internal static void SwitchGrappleAction()
    {
        GrappleContext.ActionGrapple.formType = Main.Settings.EnableGrappleAction
            ? ActionFormType.Large
            : ActionFormType.Invisible;

        GrappleContext.ActionDisableGrapple.formType = Main.Settings.EnableGrappleAction
            ? ActionFormType.Large
            : ActionFormType.Invisible;
    }

    internal static void SwitchProneAction()
    {
        DropProne.actionType = ActionType.NoCost;
        DropProne.formType = Main.Settings.EnableProneAction
            ? ActionFormType.Small
            : ActionFormType.Invisible;
    }


    internal static void SwitchHelpPower()
    {
        var dbCharacterRaceDefinition = DatabaseRepository.GetDatabase<CharacterRaceDefinition>();
        var subRaces = dbCharacterRaceDefinition
            .SelectMany(x => x.SubRaces);
        var races = dbCharacterRaceDefinition
            .Where(x => !subRaces.Contains(x));

        if (Main.Settings.EnableHelpAction)
        {
            foreach (var characterRaceDefinition in races
                         .Where(a => !a.FeatureUnlocks.Exists(x =>
                             x.Level == 1 && x.FeatureDefinition == FeatureDefinitionPowerHelpAction)))
            {
                characterRaceDefinition.FeatureUnlocks.Add(
                    new FeatureUnlockByLevel(FeatureDefinitionPowerHelpAction, 1));
            }
        }
        else
        {
            foreach (var characterRaceDefinition in races
                         .Where(a => a.FeatureUnlocks.Exists(x =>
                             x.Level == 1 && x.FeatureDefinition == FeatureDefinitionPowerHelpAction)))
            {
                characterRaceDefinition.FeatureUnlocks.RemoveAll(x =>
                    x.Level == 1 && x.FeatureDefinition == FeatureDefinitionPowerHelpAction);
            }
        }
    }

    #region Gravity Slam

    private static EffectDescription _gravitySlamVanilla;
    private static EffectDescription _gravitySlamModified;

    private static void LoadGravitySlam()
    {
        _gravitySlamVanilla = GravitySlam.EffectDescription;
        _gravitySlamModified = EffectDescriptionBuilder
            .Create(_gravitySlamVanilla)
            .SetTargetingData(Side.All, RangeType.Distance, 20, TargetType.Cylinder, 4, 10)
            .AddEffectForms(EffectFormBuilder.MotionForm(ExtraMotionType.PushDown, 10))
            .Build();
    }

    internal static void SwitchGravitySlam()
    {
        if (Main.Settings.EnablePullPushOnVerticalDirection && Main.Settings.ModifyGravitySlam)
        {
            GravitySlam.effectDescription = _gravitySlamModified;
        }
        else
        {
            GravitySlam.effectDescription = _gravitySlamVanilla;
        }

        Global.RefreshControlledCharacter();
    }

    #endregion
}
