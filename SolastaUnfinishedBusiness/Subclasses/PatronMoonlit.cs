using SolastaUnfinishedBusiness.Api;
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
            .Create(SpellListWizard, "SpellListMoonlit")
            .SetGuiPresentationNoContent(true)
            .ClearSpells()
            .SetSpellsAtLevel(1, FaerieFire, Sleep)
            .SetSpellsAtLevel(2, MoonBeam, SeeInvisibility)
            .SetSpellsAtLevel(3, Daylight, Slow)
            .SetSpellsAtLevel(4, GreaterInvisibility, GuardianOfFaith)
            .SetSpellsAtLevel(5, DominatePerson, MindTwist)
            .FinalizeSpells(true, 9)
            .AddToDB();

        var magicAffinityMoonlitExpandedSpells = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityMoonlitExpandedSpells")
            .SetGuiPresentation("MagicAffinityPatronExpandedSpells", Category.Feature)
            .SetExtendedSpellList(spellListMoonlit)
            .AddToDB();

        var conditionMoonlitInvisibility = ConditionDefinitionBuilder
            .Create("ConditionMoonlitInvisible")
            .SetGuiPresentationNoContent()
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(MoonlitInvisibility.Build())
            .SetTurnOccurence(TurnOccurenceType.StartOfTurn)
            .AddToDB();

        // only reports condition on char panel
        Global.CharacterLabelEnabledConditions.Add(conditionMoonlitInvisibility);

        var lightAffinityMoonlitWeak = FeatureDefinitionLightAffinityBuilder
            .Create("LightAffinityMoonlitWeak")
            .SetGuiPresentation(Category.Feature)
            .AddLightingEffectAndCondition(new FeatureDefinitionLightAffinity.LightingEffectAndCondition
            {
                lightingState = LocationDefinitions.LightingState.Unlit, condition = conditionMoonlitInvisibility
            })
            .AddToDB();

        var lightAffinityMoonlitStrong = FeatureDefinitionLightAffinityBuilder
            .Create("LightAffinityMoonlitStrong")
            .SetGuiPresentation(Category.Feature)
            .AddLightingEffectAndCondition(new FeatureDefinitionLightAffinity.LightingEffectAndCondition
            {
                lightingState = LocationDefinitions.LightingState.Dim, condition = conditionMoonlitInvisibility
            })
            .AddLightingEffectAndCondition(new FeatureDefinitionLightAffinity.LightingEffectAndCondition
            {
                lightingState = LocationDefinitions.LightingState.Darkness, condition = conditionMoonlitInvisibility
            })
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
            .SetGuiPresentation(Category.Feature, Darkness)
            .SetUsesProficiencyBonus(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create(Darkness.EffectDescription)
                .SetDurationData(DurationType.Minute, 1)
                .Build())
            .SetUniqueInstance()
            .AddToDB();

        var powerMoonlitFullMoon = FeatureDefinitionPowerBuilder
            .Create("PowerMoonlitFullMoon")
            .SetGuiPresentation(Category.Feature, Daylight)
            .SetUsesProficiencyBonus(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create(Daylight.EffectDescription)
                .SetDurationData(DurationType.Minute, 1)
                .Build())
            .SetUniqueInstance()
            .AddToDB();

        var powerMoonlitDanceOfTheNightSky = FeatureDefinitionPowerBuilder
            .Create("PowerMoonlitDanceOfTheNightSky")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create(Fly.EffectDescription)
                .SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.IndividualsUnique, 4)
                .Build())
            .SetUniqueInstance()
            .AddToDB();

        var powerMoonlitMoonTouched = FeatureDefinitionPowerBuilder
            .Create("PowerMoonlitMoonTouched")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetDurationData(DurationType.Minute, 1)
                .SetTargetingData(Side.All, RangeType.Distance, 12, TargetType.Cylinder, 10, 10)
                .SetSavingThrowData(
                    false,
                    AttributeDefinitions.Dexterity,
                    true,
                    EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                    AttributeDefinitions.Dexterity,
                    20)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetConditionForm(
                            ConditionDefinitionBuilder
                                .Create(DatabaseHelper.ConditionDefinitions.ConditionLevitate,
                                    "ConditionMoonlitMoonTouched")
                                .SetGuiPresentation(Category.Condition)
                                .SetConditionType(ConditionType.Neutral)
                                .SetFeatures(MoveModeFly2, MovementAffinityConditionLevitate)
                                .AddToDB(),
                            ConditionForm.ConditionOperation.Add,
                            false,
                            false)
                        .HasSavingThrow(EffectSavingThrowType.Negates)
                        .Build(),
                    EffectFormBuilder
                        .Create()
                        .SetMotionForm(MotionForm.MotionType.Levitate, 10)
                        .HasSavingThrow(EffectSavingThrowType.Negates)
                        .Build())
                .SetRecurrentEffect(Entangle.EffectDescription.RecurrentEffect)
                .Build())
            .SetUniqueInstance()
            .AddToDB();

        var bonusCantripsMoonlit = FeatureDefinitionBonusCantripsBuilder
            .Create("BonusCantripsMoonlit")
            .SetGuiPresentation(Category.Feature)
            .SetBonusCantrips(
                SpellDefinitionBuilder
                    .Create(MoonBeam, "AtWillMoonbeam")
                    .SetSpellLevel(0)
                    .AddToDB(),
                SpellDefinitionBuilder
                    .Create(FaerieFire, "AtWillFaerieFire")
                    .SetSpellLevel(0)
                    .AddToDB())
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("PatronMoonlit")
            .SetGuiPresentation(Category.Subclass, RangerShadowTamer)
            .AddFeaturesAtLevel(1,
                magicAffinityMoonlitExpandedSpells,
                conditionAffinityMoonlitDarknessImmunity,
                SenseSuperiorDarkvision)
            .AddFeaturesAtLevel(2, lightAffinityMoonlitWeak)
            .AddFeaturesAtLevel(6,
                lightAffinityMoonlitStrong,
                powerMoonlitDarkMoon,
                powerMoonlitFullMoon)
            .AddFeaturesAtLevel(10,
                powerMoonlitDanceOfTheNightSky,
                powerMoonlitMoonTouched)
            .AddFeaturesAtLevel(14,
                bonusCantripsMoonlit)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice => DatabaseHelper.FeatureDefinitionSubclassChoices
        .SubclassChoiceWarlockOtherworldlyPatrons;
}

internal sealed class MoonlitInvisibility : ICustomOnActionFeature, ICustomConditionFeature
{
    private const string CategoryRevealed = "MoonlitRevealed";
    private const string CategoryHidden = "MoonlitHidden";
    private static ConditionDefinition ConditionMoonlitRevealed { get; set; }
    private static ConditionDefinition ConditionMoonlitInvisibility { get; set; }

    public void ApplyFeature(RulesetCharacter hero)
    {
        if (!hero.HasConditionOfType(ConditionMoonlitRevealed))
        {
            BecomeInvisible(hero);
        }
    }

    public void RemoveFeature(RulesetCharacter hero)
    {
        hero.RemoveAllConditionsOfCategory(CategoryHidden, false);
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

    internal static FeatureDefinition Build()
    {
        ConditionMoonlitRevealed = ConditionDefinitionBuilder
            .Create("ConditionMoonlitRevealed")
            .SetGuiPresentationNoContent()
            .SetDuration(DurationType.Round, 1)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        ConditionMoonlitInvisibility = ConditionDefinitionBuilder
            .Create(DatabaseHelper.ConditionDefinitions.ConditionInvisible, "ConditionMoonlitInvisibility")
            .SetCancellingConditions(ConditionMoonlitRevealed)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialInterruptions(
                ConditionInterruption.Attacks,
                ConditionInterruption.CastSpell,
                ConditionInterruption.UsePower)
            .SetTurnOccurence(TurnOccurenceType.StartOfTurn)
            .AddToDB();

        return FeatureDefinitionBuilder
            .Create("FeatureMoonlitInvisibility")
            .SetGuiPresentationNoContent()
            .SetCustomSubFeatures(new MoonlitInvisibility())
            .AddToDB();
    }

    // returns true if effect is self teleport or any self targeting spell that is self-buff
    private static bool IsAllowedEffect(EffectDescription effect)
    {
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
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
                    // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
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
        hero.AddConditionOfCategory(CategoryRevealed,
            RulesetCondition.CreateActiveCondition(
                hero.Guid,
                ConditionMoonlitRevealed,
                DurationType.Round,
                1,
                TurnOccurenceType.StartOfTurn,
                hero.Guid,
                hero.CurrentFaction.Name
            ));
    }

    private static void BecomeInvisible(RulesetCharacter hero)
    {
        hero.AddConditionOfCategory(CategoryHidden,
            RulesetCondition.CreateActiveCondition(
                hero.Guid,
                ConditionMoonlitInvisibility,
                DurationType.Permanent,
                0,
                TurnOccurenceType.EndOfTurn,
                hero.Guid,
                hero.CurrentFaction.Name),
            false);
    }
}
