using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.Api.Infrastructure;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaCommunityExpansion.Models;
using static RuleDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionMovementAffinitys;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionMoveModes;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionSenses;
using static SolastaCommunityExpansion.Api.DatabaseHelper.SpellDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.SpellListDefinitions;

namespace SolastaCommunityExpansion.Classes.Warlock.Subclasses;

public static class DHWarlockSubclassMoonLitPatron
{
    private static FeatureDefinition _invisFeature;

    public static FeatureDefinition InvisibilityFeature =>
        _invisFeature ??= FeatureDefinitionMoonlitInvisibility.Build();

    public static CharacterSubclassDefinition Build()
    {
        var MoonLitExpandedSpelllist = SpellListDefinitionBuilder
            .Create(SpellListPaladin, "MoonLitExpandedSpelllist", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentationNoContent()
            .ClearSpells()
            .SetSpellsAtLevel(1, FaerieFire, Sleep)
            .SetSpellsAtLevel(2, MoonBeam, SeeInvisibility)
            .SetSpellsAtLevel(3, Daylight, Slow)
            .SetSpellsAtLevel(4, GreaterInvisibility, GuardianOfFaith)
            .SetSpellsAtLevel(5, DominatePerson, MindTwist)
            .FinalizeSpells()
            .AddToDB();


        var MoonLitExpandedSpelllistAfinity = FeatureDefinitionMagicAffinityBuilder
            .Create("MoonLitExpandedSpelllistAfinity", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation("MoonLitExpandedSpelllistAfinity", Category.Feature)
            .SetExtendedSpellList(MoonLitExpandedSpelllist)
            .AddToDB();

        var moonlitInvisibleCondition = ConditionDefinitionBuilder
            .Create("ConditionMoonlitSpecial", DefinitionBuilder.CENamespaceGuid)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetGuiPresentationNoContent()
            .SetFeatures(InvisibilityFeature)
            .SetTurnOccurence(TurnOccurenceType.StartOfTurn)
            .AddToDB();

        // don't get the annoying message on log
        Global.CharacterLabelEnabledConditions.Add(moonlitInvisibleCondition);

        var unlit = new FeatureDefinitionLightAffinity.LightingEffectAndCondition
        {
            lightingState = LocationDefinitions.LightingState.Unlit, condition = moonlitInvisibleCondition
        };
        var dim = new FeatureDefinitionLightAffinity.LightingEffectAndCondition
        {
            lightingState = LocationDefinitions.LightingState.Dim, condition = moonlitInvisibleCondition
        };
        var darkness = new FeatureDefinitionLightAffinity.LightingEffectAndCondition
        {
            lightingState = LocationDefinitions.LightingState.Darkness, condition = moonlitInvisibleCondition
        };

        var MoonLitLightAffinityWeak = FeatureDefinitionLightAffinityBuilder
            .Create("MoonLitLightAffinity", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation("MoonLitLightAffinity", Category.Feature)
            .AddLightingEffectAndCondition(unlit)
            .AddToDB();

        var MoonLitLightAffinityStrong = FeatureDefinitionLightAffinityBuilder
            .Create("MoonLitLightAffinityStrong", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation("MoonLitLightAffinityStrong", Category.Feature)
            .AddLightingEffectAndCondition(dim)
            .AddLightingEffectAndCondition(darkness)
            .AddToDB();

        // should probably be expanded to include a magicaffinty that has immunity to darkness spells as well
        var MoonLitDarknessImmunity = FeatureDefinitionConditionAffinityBuilder
            .Create("MoonLitDarknessImmunity", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation("MoonLitDarknessImmunity", Category.Feature)
            .SetConditionAffinityType(ConditionAffinityType.Immunity)
            .SetConditionType(DatabaseHelper.ConditionDefinitions.ConditionDarkness)
            .AddToDB();

        var DarkMoon = FeatureDefinitionPowerBuilder
            .Create("MoonlitDarkMoon", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Power, Darkness.GuiPresentation.SpriteReference)
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
                Darkness.EffectDescription.Copy().SetDuration(DurationType.Minute, 1),
                true)
            .AddToDB();

        var FullMoon = FeatureDefinitionPowerBuilder
            .Create("MoonlitFullMoon", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Power, Daylight.GuiPresentation.SpriteReference)
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
                Daylight.EffectDescription.Copy().SetDuration(DurationType.Minute, 1),
                true)
            .AddToDB();


        var DanceoftheNightSky = FeatureDefinitionPowerBuilder
            .Create("MoonlitDanceoftheNightSky", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Power)
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
        DanceoftheNightSky.EffectDescription.SetTargetParameter(4);

        var MoonTouchedCondition = ConditionDefinitionBuilder
            .Create(DatabaseHelper.ConditionDefinitions.ConditionLevitate, "MoonTouchedCondition",
                DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Condition)
            .SetConditionType(ConditionType.Neutral)
            .SetFeatures(MoveModeFly2)
            .SetFeatures(MovementAffinityConditionLevitate)
            .AddToDB();

        var MoonTouched = FeatureDefinitionPowerBuilder
            .Create("MoonlitMoonTouched", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Power)
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
                            MoonTouchedCondition,
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


        var AtWillMoonbeam = SpellDefinitionBuilder
            .Create(MoonBeam, "MoonlitAtWillMoonbeam", DefinitionBuilder.CENamespaceGuid)
            .SetSpellLevel(0)
            .AddToDB();

        var AtWillFaerieFire = SpellDefinitionBuilder
            .Create(FaerieFire, "MoonlitAtWillFaerieFire", DefinitionBuilder.CENamespaceGuid)
            .SetSpellLevel(0)
            .AddToDB();

        var MoonlitBonusCantrips = FeatureDefinitionBonusCantripsBuilder
            .Create("MoonlitBonusCantrips", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feat)
            .ClearBonusCantrips()
            .AddBonusCantrip(AtWillMoonbeam)
            .AddBonusCantrip(AtWillFaerieFire)
            .AddToDB();
        /*
         Moonlit
        1
        Lunar Guide Expanded Spells or Moonlit Expanded Spells
        emit light (maybe indomitable light) + tempHP aura
        light affinity invisible in darkness/dim light
        6
        Cycles of the Moon
        Full Moon (cast daylight)
        Dark New (cast darkness)
        10
        Dance of the Night Sky (casting fly on 4 people)
        Moon touched (cast reverse gravity without expending a spell slot)
        14
        moonbeam , faerie fire- at will
         */

        return CharacterSubclassDefinitionBuilder
            .Create("MoonLit", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation("WarlockMoonLit", Category.Subclass,
                RangerShadowTamer.GuiPresentation.SpriteReference)
            .AddFeatureAtLevel(MoonLitExpandedSpelllistAfinity, 1)
            .AddFeatureAtLevel(SenseSuperiorDarkvision, 1)
            //  .AddFeatureAtLevel(ConditionAffinityBlindnessImmunity, 1)
            .AddFeatureAtLevel(MoonLitDarknessImmunity, 1)
            .AddFeatureAtLevel(MoonLitLightAffinityWeak, 2)
            .AddFeatureAtLevel(MoonLitLightAffinityStrong, 6)
            .AddFeatureAtLevel(FullMoon, 6)
            .AddFeatureAtLevel(DarkMoon, 6)
            // .AddFeatureAtLevel(, 6)
            .AddFeatureAtLevel(DanceoftheNightSky, 10)
            .AddFeatureAtLevel(MoonTouched, 10)
            .AddFeatureAtLevel(MoonlitBonusCantrips, 14)
            // .AddFeatureAtLevel(, 14)
            .AddToDB();
    }
}

internal class FeatureDefinitionMoonlitInvisibility : FeatureDefinition, ICustomOnActionFeature,
    ICustomConditionFeature
{
    private static readonly string CATEGORY_REVEALED = "MoonlitRevealed";
    private static readonly string CATEGORY_HIDDEN = "MoonlitHidden";
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
        hero.RemoveAllConditionsOfCategory(CATEGORY_HIDDEN, false);
    }

    public void OnBeforeAction(CharacterAction characterAction)
    {
        // Let's try this invis without movement breaking it
        // var hero = characterAction.ActingCharacter.RulesetCharacter;
        // var action = characterAction.ActionDefinition;
        //
        // if (action == ExplorationMove || action == TacticalMove)
        //     BecomeRevealed(hero);
    }

    public void OnAfterAction(CharacterAction characterAction)
    {
        var hero = characterAction.ActingCharacter.RulesetCharacter;
        var action = characterAction.ActionDefinition;

        if (action.Name.StartsWith("Attack") || action.Name.StartsWith("Cast") ||
            action.Name.StartsWith(TagsDefinitions.Power))
        {
            var ruleEffect = characterAction.ActionParams.RulesetEffect;
            if (ruleEffect == null || !IsAllowedEffect(ruleEffect.EffectDescription))
            {
                BecomeRevealed(hero);
            }
        }
    }

    public static FeatureDefinition Build()
    {
        RevealedCondition = BuildRevealedCondition();
        InvisibilityCondition = BuildInvisibilityCondition();
        return FeatureDefinitionMoonlitInvisibilityBuilder
            .Create("MoonlitCustomInvisibilityFeature", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentationNoContent()
            .AddToDB();
    }

    private static ConditionDefinition BuildRevealedCondition()
    {
        return ConditionDefinitionBuilder
            .Create("ConditionMoonlitRevealed", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentationNoContent()
            .Configure(DurationType.Round, 1, false)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();
    }

    private static ConditionDefinition BuildInvisibilityCondition()
    {
        var condition = ConditionDefinitionBuilder
            .Create(DatabaseHelper.ConditionDefinitions.ConditionInvisible, "ConditionMoonlitInvisible",
                DefinitionBuilder.CENamespaceGuid)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialInterruptions(
                ConditionInterruption.Attacks,
                ConditionInterruption.CastSpell,
                ConditionInterruption.UsePower
            )
            .SetTurnOccurence(TurnOccurenceType.StartOfTurn)
            .AddToDB();

        condition.CancellingConditions.SetRange(RevealedCondition);

        return condition;
    }

    /**Returns true if effect is self teleport or any self targeting spell that is self-buff*/
    private static bool IsAllowedEffect(EffectDescription effect)
    {
        if (effect.TargetType == TargetType.Position)
        {
            foreach (var form in effect.EffectForms)
            {
                if (form.FormType != EffectForm.EffectFormType.Motion) { return false; }

                if (form.MotionForm.Type != MotionForm.MotionType.TeleportToDestination) { return false; }
            }
        }
        else if (effect.TargetType == TargetType.Self)
        {
            foreach (var form in effect.EffectForms)
            {
                if (form.FormType == EffectForm.EffectFormType.Damage) { return false; }

                if (form.FormType == EffectForm.EffectFormType.Healing) { return false; }

                if (form.FormType == EffectForm.EffectFormType.ShapeChange) { return false; }

                if (form.FormType == EffectForm.EffectFormType.Summon) { return false; }

                if (form.FormType == EffectForm.EffectFormType.Counter) { return false; }

                if (form.FormType == EffectForm.EffectFormType.Motion) { return false; }
            }
        }
        else
        {
            return false;
        }

        return true;
    }

    private static void BecomeRevealed(RulesetCharacter hero)
    {
        hero.AddConditionOfCategory(CATEGORY_REVEALED, RulesetCondition.CreateActiveCondition(hero.Guid,
            RevealedCondition, DurationType.Round,
            1,
            TurnOccurenceType.StartOfTurn,
            hero.Guid,
            hero.CurrentFaction.Name
        ));
    }

    private static void BecomeInvisible(RulesetCharacter hero)
    {
        hero.AddConditionOfCategory(CATEGORY_HIDDEN, RulesetCondition.CreateActiveCondition(hero.Guid,
            InvisibilityCondition,
            DurationType.Permanent,
            0,
            TurnOccurenceType.EndOfTurn,
            hero.Guid,
            hero.CurrentFaction.Name
        ), false);
    }

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
