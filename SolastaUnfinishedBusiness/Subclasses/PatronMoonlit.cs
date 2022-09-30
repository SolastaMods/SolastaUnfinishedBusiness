using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Models;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionMovementAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionMoveModes;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSenses;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellListDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class PatronMoonlit : AbstractSubclass
{
    internal PatronMoonlit()
    {
        var spellListMoonlit = SpellListDefinitionBuilder
            .Create(SpellListPaladin, "SpellListMoonlit")
            .SetGuiPresentationNoContent(true)
            .ClearSpells()
            .SetSpellsAtLevel(1, FaerieFire, Sleep)
            .SetSpellsAtLevel(2, MoonBeam, SeeInvisibility)
            .SetSpellsAtLevel(3, Daylight, Slow)
            .SetSpellsAtLevel(4, GreaterInvisibility, GuardianOfFaith)
            .SetSpellsAtLevel(5, DominatePerson, MindTwist)
            .FinalizeSpells()
            .AddToDB();

        var magicAffinityMoonlitExpandedSpells = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityMoonlitExpandedSpells")
            .SetGuiPresentation("MagicAffinityPatronExpandedSpells", Category.Feature)
            .SetExtendedSpellList(spellListMoonlit)
            .AddToDB();

        var conditionMoonlitInvisibility = ConditionDefinitionBuilder
            .Create("ConditionMoonlitInvisibility")
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetGuiPresentationNoContent()
            .SetFeatures(FeatureDefinitionMoonlitInvisibility.Build())
            .SetTurnOccurence(TurnOccurenceType.StartOfTurn)
            .AddToDB();

        // only reports condition on char panel
        Global.CharacterLabelEnabledConditions.Add(conditionMoonlitInvisibility);

        var unlit = new FeatureDefinitionLightAffinity.LightingEffectAndCondition
        {
            lightingState = LocationDefinitions.LightingState.Unlit, condition = conditionMoonlitInvisibility
        };
        var dim = new FeatureDefinitionLightAffinity.LightingEffectAndCondition
        {
            lightingState = LocationDefinitions.LightingState.Dim, condition = conditionMoonlitInvisibility
        };
        var darkness = new FeatureDefinitionLightAffinity.LightingEffectAndCondition
        {
            lightingState = LocationDefinitions.LightingState.Darkness, condition = conditionMoonlitInvisibility
        };

        var lightAffinityMoonlitWeak = FeatureDefinitionLightAffinityBuilder
            .Create("LightAffinityMoonlitWeak")
            .SetGuiPresentation(Category.Feature)
            .AddLightingEffectAndCondition(unlit)
            .AddToDB();

        var lightAffinityMoonlitStrong = FeatureDefinitionLightAffinityBuilder
            .Create("LightAffinityMoonlitStrong")
            .SetGuiPresentation(Category.Feature)
            .AddLightingEffectAndCondition(dim)
            .AddLightingEffectAndCondition(darkness)
            .AddToDB();

        // should probably be expanded to include a magic affinity that has immunity to darkness spells as well
        var conditionAffinityMoonlitDarknessImmunity = FeatureDefinitionConditionAffinityBuilder
            .Create("ConditionAffinityMoonlitDarknessImmunity")
            .SetGuiPresentation(Category.Feature)
            .SetConditionAffinityType(ConditionAffinityType.Immunity)
            .SetConditionType(DatabaseHelper.ConditionDefinitions.ConditionDarkness)
            .AddToDB();

        var powerMoonlitDarkMoon = FeatureDefinitionPowerBuilder
            .Create("PowerMoonlitDarkMoon")
            .SetGuiPresentation(Category.Feature, Darkness.GuiPresentation.SpriteReference)
            .Configure(
                1,
                UsesDetermination.ProficiencyBonus,
                AttributeDefinitions.Charisma,
                ActivationTime.Action,
                1,
                RechargeRate.LongRest,
                false,
                false,
                AttributeDefinitions.Charisma,
                Darkness.EffectDescription
                    .Copy()
                    .SetDuration(DurationType.Minute, 1),
                true)
            .AddToDB();

        var powerMoonlitFullMoon = FeatureDefinitionPowerBuilder
            .Create("PowerMoonlitFullMoon")
            .SetGuiPresentation(Category.Feature, Daylight.GuiPresentation.SpriteReference)
            .Configure(
                1,
                UsesDetermination.ProficiencyBonus,
                AttributeDefinitions.Charisma,
                ActivationTime.Action,
                1,
                RechargeRate.LongRest,
                false,
                false,
                AttributeDefinitions.Charisma,
                Daylight.EffectDescription
                    .Copy()
                    .SetDuration(DurationType.Minute, 1),
                true)
            .AddToDB();

        var powerMoonlitDanceOfTheNightSky = FeatureDefinitionPowerBuilder
            .Create("PowerMoonlitDanceOfTheNightSky")
            .SetGuiPresentation(Category.Feature)
            .Configure(
                1,
                UsesDetermination.Fixed,
                AttributeDefinitions.Charisma,
                ActivationTime.Action,
                1,
                RechargeRate.LongRest,
                false,
                false,
                AttributeDefinitions.Charisma,
                Fly.EffectDescription.Copy(),
                true)
            .AddToDB();

        powerMoonlitDanceOfTheNightSky.EffectDescription.SetTargetParameter(4);

        var conditionMoonlitMoonTouched = ConditionDefinitionBuilder
            .Create(DatabaseHelper.ConditionDefinitions.ConditionLevitate, "ConditionMoonlitMoonTouched")
            .SetGuiPresentation(Category.Condition)
            .SetConditionType(ConditionType.Neutral)
            .SetFeatures(MoveModeFly2)
            .SetFeatures(MovementAffinityConditionLevitate)
            .AddToDB();

        var powerMoonlitMoonTouched = FeatureDefinitionPowerBuilder
            .Create("PowerMoonlitMoonTouched")
            .SetGuiPresentation(Category.Feature)
            .Configure(
                1,
                UsesDetermination.Fixed,
                AttributeDefinitions.Charisma,
                ActivationTime.Action,
                1,
                RechargeRate.LongRest,
                false,
                false,
                AttributeDefinitions.Charisma,
                new EffectDescriptionBuilder()
                    .SetDurationData(
                        DurationType.Minute,
                        1,
                        TurnOccurenceType.EndOfTurn)
                    .SetTargetingData(
                        Side.All,
                        RangeType.Distance,
                        12,
                        TargetType.Cylinder,
                        10,
                        10)
                    .SetSavingThrowData(
                        true,
                        false,
                        AttributeDefinitions.Dexterity,
                        true,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                        AttributeDefinitions.Dexterity,
                        20,
                        false,
                        new List<SaveAffinityBySenseDescription>())
                    .AddEffectForm(new EffectFormBuilder()
                        .SetConditionForm(
                            conditionMoonlitMoonTouched,
                            ConditionForm.ConditionOperation.Add,
                            false,
                            false,
                            new List<ConditionDefinition>())
                        .HasSavingThrow(EffectSavingThrowType.Negates)
                        .Build())
                    .AddEffectForm(new EffectFormBuilder()
                        .SetMotionForm(
                            MotionForm.MotionType.Levitate,
                            10)
                        .HasSavingThrow(EffectSavingThrowType.Negates)
                        .Build())
                    .SetRecurrentEffect(Entangle.EffectDescription.RecurrentEffect)
                    .Build()
                ,
                true)
            .AddToDB();

        var moonbeamAtWill = SpellDefinitionBuilder
            .Create(MoonBeam, "MoonbeamAtWill")
            .SetSpellLevel(0)
            .AddToDB();

        var faerieFireAtWill = SpellDefinitionBuilder
            .Create(FaerieFire, "FaerieFireAtWill")
            .SetSpellLevel(0)
            .AddToDB();

        var bonusCantripsMoonlit = FeatureDefinitionBonusCantripsBuilder
            .Create("BonusCantripsMoonlit")
            .SetGuiPresentation(Category.Feature)
            .SetBonusCantrips(moonbeamAtWill, faerieFireAtWill)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("PatronMoonlit")
            .SetGuiPresentation(Category.Subclass, RangerShadowTamer.GuiPresentation.SpriteReference)
            .AddFeaturesAtLevel(1,
                magicAffinityMoonlitExpandedSpells,
                conditionAffinityMoonlitDarknessImmunity,
                SenseSuperiorDarkvision)
            //TODO: should this be indeed on 2?
            .AddFeaturesAtLevel(2, lightAffinityMoonlitWeak)
            .AddFeaturesAtLevel(6,
                lightAffinityMoonlitStrong
                , powerMoonlitDarkMoon,
                powerMoonlitFullMoon)
            .AddFeaturesAtLevel(10,
                powerMoonlitDanceOfTheNightSky,
                powerMoonlitMoonTouched)
            .AddFeaturesAtLevel(14,
                bonusCantripsMoonlit)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; set; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice => DatabaseHelper.FeatureDefinitionSubclassChoices
        .SubclassChoiceWarlockOtherworldlyPatrons;
}

internal sealed class FeatureDefinitionMoonlitInvisibility : FeatureDefinition, ICustomOnActionFeature,
    ICustomConditionFeature
{
    private const string CategoryRevealed = "MoonlitRevealed";
    private const string CategoryHidden = "MoonlitHidden";
    private static ConditionDefinition RevealedCondition { get; set; }
    private static ConditionDefinition InvisibilityCondition { get; set; }

    public void ApplyFeature(RulesetCharacter hero)
    {
        if (!hero.HasConditionOfType(RevealedCondition))
        {
            BecomeInvisible(hero);
        }
    }

    public void RemoveFeature(RulesetCharacter hero)
    {
        hero.RemoveAllConditionsOfCategory(CategoryHidden, false);
    }

    public void OnBeforeAction(CharacterAction characterAction)
    {
        // empty
    }

    public void OnAfterAction(CharacterAction characterAction)
    {
        var hero = characterAction.ActingCharacter.RulesetCharacter;
        var action = characterAction.ActionDefinition;

        if (!action.Name.StartsWith("Attack")
            && !action.Name.StartsWith("Cast")
            && !action.Name.StartsWith("Power"))
        {
            return;
        }

        var ruleEffect = characterAction.ActionParams.RulesetEffect;

        if (ruleEffect == null || !IsAllowedEffect(ruleEffect.EffectDescription))
        {
            BecomeRevealed(hero);
        }
    }

    public static FeatureDefinition Build()
    {
        RevealedCondition = BuildRevealedCondition();
        InvisibilityCondition = BuildInvisibilityCondition();

        return FeatureDefinitionMoonlitInvisibilityBuilder
            .Create("MoonlitInvisibility")
            .SetGuiPresentationNoContent()
            .AddToDB();
    }

    private static ConditionDefinition BuildRevealedCondition()
    {
        return ConditionDefinitionBuilder
            .Create("ConditionMoonlitRevealed")
            .SetGuiPresentationNoContent()
            .SetSilent(Silent.WhenAddedOrRemoved)
            .Configure(DurationType.Round, 1, false)
            .AddToDB();
    }

    private static ConditionDefinition BuildInvisibilityCondition()
    {
        var conditionMoonlitInvisible = ConditionDefinitionBuilder
            .Create(DatabaseHelper.ConditionDefinitions.ConditionInvisible, "ConditionMoonlitInvisible")
            .SetGuiPresentationNoContent()
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialInterruptions(
                ConditionInterruption.Attacks,
                ConditionInterruption.CastSpell,
                ConditionInterruption.UsePower
            )
            .SetTurnOccurence(TurnOccurenceType.StartOfTurn)
            .AddToDB();

        conditionMoonlitInvisible.CancellingConditions.SetRange(RevealedCondition);

        return conditionMoonlitInvisible;
    }

    // returns true if effect is self teleport or any self targeting spell that is self-buff
    private static bool IsAllowedEffect(EffectDescription effect)
    {
        switch (effect.TargetType)
        {
            case TargetType.Position:
            {
                foreach (var form in effect.EffectForms)
                {
                    if (form.FormType != EffectForm.EffectFormType.Motion) { return false; }

                    if (form.MotionForm.Type != MotionForm.MotionType.TeleportToDestination) { return false; }
                }

                break;
            }
            case TargetType.Self:
            {
                foreach (var form in effect.EffectForms)
                {
                    switch (form.FormType)
                    {
                        case EffectForm.EffectFormType.Damage:
                        case EffectForm.EffectFormType.Healing:
                        case EffectForm.EffectFormType.ShapeChange:
                        case EffectForm.EffectFormType.Summon:
                        case EffectForm.EffectFormType.Counter:
                        case EffectForm.EffectFormType.Motion:
                            return false;
                    }
                }

                break;
            }
            default:
                return false;
        }

        return true;
    }

    private static void BecomeRevealed(RulesetCharacter hero)
    {
        hero.AddConditionOfCategory(CategoryRevealed, RulesetCondition.CreateActiveCondition(hero.Guid,
            RevealedCondition, DurationType.Round,
            1,
            TurnOccurenceType.StartOfTurn,
            hero.Guid,
            hero.CurrentFaction.Name
        ));
    }

    private static void BecomeInvisible(RulesetCharacter hero)
    {
        hero.AddConditionOfCategory(CategoryHidden, RulesetCondition.CreateActiveCondition(hero.Guid,
            InvisibilityCondition,
            DurationType.Permanent,
            0,
            TurnOccurenceType.EndOfTurn,
            hero.Guid,
            hero.CurrentFaction.Name
        ), false);
    }

    [UsedImplicitly]
    private class FeatureDefinitionMoonlitInvisibilityBuilder : FeatureDefinitionBuilder<
        FeatureDefinitionMoonlitInvisibility,
        FeatureDefinitionMoonlitInvisibilityBuilder>
    {
        #region Constructors

        public FeatureDefinitionMoonlitInvisibilityBuilder(string name, Guid namespaceGuid) : base(name,
            namespaceGuid)
        {
        }

        public FeatureDefinitionMoonlitInvisibilityBuilder(string name, string definitionGuid) : base(name,
            definitionGuid)
        {
        }

        public FeatureDefinitionMoonlitInvisibilityBuilder(FeatureDefinitionMoonlitInvisibility original,
            string name,
            Guid namespaceGuid)
            : base(original, name, namespaceGuid)
        {
        }

        public FeatureDefinitionMoonlitInvisibilityBuilder(FeatureDefinitionMoonlitInvisibility original,
            string name,
            string definitionGuid) : base(original, name, definitionGuid)
        {
        }

        #endregion
    }
}
