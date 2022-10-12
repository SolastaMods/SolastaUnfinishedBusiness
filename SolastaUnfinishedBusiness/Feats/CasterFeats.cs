using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;

namespace SolastaUnfinishedBusiness.Feats;

internal static class CasterFeats
{
    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        var groups = new List<FeatDefinition>();
        var groupFeats = new List<FeatDefinition>();

        // telekinetic general
        const string TELEKINETIC = "Telekinetic";

        var pushPresentation = GuiPresentationBuilder.Build("PowerFeatTelekineticPush", Category.Feature,
            PowerVampiricTouch.GuiPresentation.SpriteReference);

        var pullPresentation = GuiPresentationBuilder.Build("PowerFeatTelekineticPull", Category.Feature,
            PowerVampiricTouch.GuiPresentation.SpriteReference);

        // telekinetic int

        var powerFeatTelekineticIntPush = BuildMotionFormPower(
            1,
            RuleDefinitions.ActivationTime.BonusAction, 0, RuleDefinitions.RechargeRate.AtWill,
            RuleDefinitions.RangeType.Distance, 6, RuleDefinitions.TargetType.Individuals,
            RuleDefinitions.Side.All, true, AttributeDefinitions.Strength,
            RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
            AttributeDefinitions.Intelligence,
            MotionForm.MotionType.PushFromOrigin, 1, 10,
            "PowerFeatTelekineticIntPush", pushPresentation);

        var powerFeatTelekineticIntPull = BuildMotionFormPower(
            1,
            RuleDefinitions.ActivationTime.BonusAction, 0, RuleDefinitions.RechargeRate.AtWill,
            RuleDefinitions.RangeType.Distance, 6, RuleDefinitions.TargetType.Individuals,
            RuleDefinitions.Side.All, true, AttributeDefinitions.Strength,
            RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
            AttributeDefinitions.Intelligence,
            MotionForm.MotionType.DragToOrigin, 1, 10,
            "PowerFeatTelekineticIntPull", pullPresentation);

        var featTelekineticInt = FeatDefinitionBuilder
            .Create("FeatTelekineticInt")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                powerFeatTelekineticIntPush,
                powerFeatTelekineticIntPull,
                AttributeModifierCreed_Of_Pakri)
            .SetFeatFamily(TELEKINETIC)
            .AddToDB();

        // telekinetic cha

        var powerFeatTelekineticChaPush = BuildMotionFormPower(
            1,
            RuleDefinitions.ActivationTime.BonusAction, 0, RuleDefinitions.RechargeRate.AtWill,
            RuleDefinitions.RangeType.Distance, 6, RuleDefinitions.TargetType.Individuals,
            RuleDefinitions.Side.All, true, AttributeDefinitions.Strength,
            RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Charisma,
            MotionForm.MotionType.PushFromOrigin, 1, 10,
            "PowerFeatTelekineticChaPush", pushPresentation);

        var powerFeatTelekineticChaPull = BuildMotionFormPower(
            1,
            RuleDefinitions.ActivationTime.BonusAction, 0, RuleDefinitions.RechargeRate.AtWill,
            RuleDefinitions.RangeType.Distance, 6, RuleDefinitions.TargetType.Individuals,
            RuleDefinitions.Side.All, true, AttributeDefinitions.Strength,
            RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Charisma,
            MotionForm.MotionType.DragToOrigin, 1, 10,
            "PowerFeatTelekineticChaPull", pullPresentation);

        var featTelekineticCha = FeatDefinitionBuilder
            .Create("FeatTelekineticCha")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                powerFeatTelekineticChaPush,
                powerFeatTelekineticChaPull,
                AttributeModifierCreed_Of_Solasta)
            .SetFeatFamily(TELEKINETIC)
            .AddToDB();

        // telekinetic wis

        var powerFeatTelekineticWisPush = BuildMotionFormPower(
            1,
            RuleDefinitions.ActivationTime.BonusAction, 0, RuleDefinitions.RechargeRate.AtWill,
            RuleDefinitions.RangeType.Distance, 6, RuleDefinitions.TargetType.Individuals,
            RuleDefinitions.Side.All, true, AttributeDefinitions.Strength,
            RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Wisdom,
            MotionForm.MotionType.PushFromOrigin, 1, 10,
            "PowerFeatTelekineticWisPush", pushPresentation);

        var powerFeatTelekineticWisPull = BuildMotionFormPower(
            1,
            RuleDefinitions.ActivationTime.BonusAction, 0, RuleDefinitions.RechargeRate.AtWill,
            RuleDefinitions.RangeType.Distance, 6, RuleDefinitions.TargetType.Individuals,
            RuleDefinitions.Side.All, true, AttributeDefinitions.Strength,
            RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Wisdom,
            MotionForm.MotionType.DragToOrigin, 1, 10,
            "PowerFeatTelekineticWisPull", pullPresentation);

        var featTelekineticWis = FeatDefinitionBuilder
            .Create("FeatTelekineticWis")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                powerFeatTelekineticWisPush,
                powerFeatTelekineticWisPull,
                AttributeModifierCreed_Of_Maraike)
            .SetFeatFamily(TELEKINETIC)
            .AddToDB();

        groupFeats.SetRange(featTelekineticInt, featTelekineticCha, featTelekineticWis);
        groups.Add(GroupFeats.MakeGroup("FeatGroupTelekinetic", TELEKINETIC, groupFeats));
        feats.AddRange(groupFeats);

        // fey teleportation

        const string FEY_TELEPORT = "FeyTeleport";

        var autoPreparedSpellsFeyTeleportation = AutoPreparedClassLists(
            BuildSpellGroup(0, MistyStep),
            MistyStep.GuiPresentation,
            "AutoPreparedSpellsFeyTeleportation", FEY_TELEPORT);

        var powerFeatFeyTeleportationMistyStep = BuildPowerFromEffectDescription(
            1,
            RuleDefinitions.ActivationTime.BonusAction, 1, RuleDefinitions.RechargeRate.ShortRest,
            false, false, AttributeDefinitions.Intelligence,
            MistyStep.EffectDescription,
            "PowerFeatFeyTeleportationMistyStep", MistyStep.GuiPresentation);

        var proficiencyFeatFeyTeleportationTirmarian = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyFeatFeyTeleportationTirmarian")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(
                RuleDefinitions.ProficiencyType.Language, "Language_Tirmarian")
            .AddToDB();

        groupFeats.SetRange(
            // fey teleportation int
            FeatDefinitionBuilder
                .Create("FeatFeyTeleportationInt")
                .SetFeatures(
                    AttributeModifierCreed_Of_Pakri,
                    proficiencyFeatFeyTeleportationTirmarian,
                    powerFeatFeyTeleportationMistyStep)
                .AddFeatures(autoPreparedSpellsFeyTeleportation)
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(FEY_TELEPORT)
                .AddToDB(),
            // fey teleportation cha
            FeatDefinitionBuilder
                .Create("FeatFeyTeleportationCha")
                .SetFeatures(
                    AttributeModifierCreed_Of_Solasta,
                    proficiencyFeatFeyTeleportationTirmarian,
                    powerFeatFeyTeleportationMistyStep)
                .AddFeatures(autoPreparedSpellsFeyTeleportation)
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(FEY_TELEPORT)
                .AddToDB(),
            // fey teleportation wis
            FeatDefinitionBuilder
                .Create("FeatFeyTeleportationWis")
                .SetFeatures(
                    AttributeModifierCreed_Of_Maraike,
                    proficiencyFeatFeyTeleportationTirmarian,
                    powerFeatFeyTeleportationMistyStep)
                .AddFeatures(autoPreparedSpellsFeyTeleportation)
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(FEY_TELEPORT)
                .AddToDB()
        );

        groups.Add(GroupFeats.MakeGroup("FeatGroupTeleportation", FEY_TELEPORT, groupFeats));
        feats.AddRange(groupFeats);

        // celestial touched

        const string CELESTIAL_TOUCHED = "CelestialTouched";

        var autoPreparedSpellsFeatCelestialTouched = AutoPreparedClassLists(
            BuildSpellGroup(0, HealingWord, CureWounds, LesserRestoration),
            GuiPresentationBuilder
                .Build("AutoPreparedSpellsFeatCelestialTouched", Category.Feature),
            "AutoPreparedSpellsFeatCelestialTouched", CELESTIAL_TOUCHED);

        var powerFeatCelestialTouchedHealingWord = BuildPowerFromEffectDescription(1,
            RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
            false, false, AttributeDefinitions.Intelligence,
            HealingWord.EffectDescription, "PowerFeatCelestialTouchedHealingWord", HealingWord.GuiPresentation);

        var powerFeatCelestialTouchedCureWounds = BuildPowerFromEffectDescription(1,
            RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
            false, false, AttributeDefinitions.Intelligence,
            CureWounds.EffectDescription, "PowerFeatCelestialTouchedCureWounds", CureWounds.GuiPresentation);

        var powerFeatCelestialTouchedLesserRestoration = BuildPowerFromEffectDescription(1,
            RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
            false, false, AttributeDefinitions.Intelligence,
            LesserRestoration.EffectDescription, "PowerFeatCelestialTouchedLesserRestoration",
            LesserRestoration.GuiPresentation);

        groupFeats.SetRange(
            // celestial touched int
            FeatDefinitionBuilder
                .Create("FeatCelestialTouchedInt")
                .SetFeatures(
                    powerFeatCelestialTouchedHealingWord,
                    powerFeatCelestialTouchedCureWounds,
                    powerFeatCelestialTouchedLesserRestoration,
                    AttributeModifierCreed_Of_Pakri)
                .AddFeatures(autoPreparedSpellsFeatCelestialTouched)
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(CELESTIAL_TOUCHED)
                .AddToDB(),
            // celestial touched wis
            FeatDefinitionBuilder
                .Create("FeatCelestialTouchedWis")
                .SetFeatures(
                    powerFeatCelestialTouchedHealingWord,
                    powerFeatCelestialTouchedCureWounds,
                    powerFeatCelestialTouchedLesserRestoration,
                    AttributeModifierCreed_Of_Maraike)
                .AddFeatures(autoPreparedSpellsFeatCelestialTouched)
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(CELESTIAL_TOUCHED)
                .AddToDB(),
            // celestial touched cha
            FeatDefinitionBuilder
                .Create("FeatCelestialTouchedCha")
                .SetFeatures(
                    powerFeatCelestialTouchedHealingWord,
                    powerFeatCelestialTouchedCureWounds,
                    powerFeatCelestialTouchedLesserRestoration,
                    AttributeModifierCreed_Of_Solasta)
                .AddFeatures(autoPreparedSpellsFeatCelestialTouched)
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(CELESTIAL_TOUCHED)
                .AddToDB()
        );

        groups.Add(GroupFeats.MakeGroup("FeatGroupCelestialTouched", CELESTIAL_TOUCHED, groupFeats));
        feats.AddRange(groupFeats);

        // flame touched

        const string FLAME_TOUCHED = "FlameTouched";

        var autoPreparedSpellsFeatFlameTouched = AutoPreparedClassLists(
            BuildSpellGroup(0, BurningHands, HellishRebuke, ScorchingRay),
            GuiPresentationBuilder
                .Build("AutoPreparedSpellsFeatFlameTouched", Category.Feature),
            "AutoPreparedSpellsFeatFlameTouched", FLAME_TOUCHED);

        var powerFeatFlameTouchedBurningHandsInt = BuildPowerFromEffectDescription(1,
            RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
            true, true, AttributeDefinitions.Intelligence,
            BurningHands.EffectDescription, "PowerFeatFlameTouchedBurningHandsInt", BurningHands.GuiPresentation);

        var powerFeatFlameTouchedBurningHandsWis = BuildPowerFromEffectDescription(1,
            RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
            true, true, AttributeDefinitions.Wisdom,
            BurningHands.EffectDescription, "PowerFeatFlameTouchedBurningHandsWis", BurningHands.GuiPresentation);

        var powerFeatFlameTouchedBurningHandsCha = BuildPowerFromEffectDescription(1,
            RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
            true, true, AttributeDefinitions.Charisma,
            BurningHands.EffectDescription, "PowerFeatFlameTouchedBurningHandsCha", BurningHands.GuiPresentation);

        var powerFeatFlameTouchedScorchingRayInt = BuildPowerFromEffectDescription(1,
            RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
            true, true, AttributeDefinitions.Intelligence,
            ScorchingRay.EffectDescription, "PowerFeatFlameTouchedScorchingRayInt", ScorchingRay.GuiPresentation);

        var powerFeatFlameTouchedScorchingRayWis = BuildPowerFromEffectDescription(1,
            RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
            true, true, AttributeDefinitions.Wisdom,
            ScorchingRay.EffectDescription, "PowerFeatFlameTouchedScorchingRayWis", ScorchingRay.GuiPresentation);

        var powerFeatFlameTouchedScorchingRayCha = BuildPowerFromEffectDescription(1,
            RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
            true, true, AttributeDefinitions.Charisma,
            ScorchingRay.EffectDescription, "PowerFeatFlameTouchedScorchingRayCha", ScorchingRay.GuiPresentation);

        groupFeats.SetRange(
            // flame touched int
            FeatDefinitionBuilder
                .Create("FeatFlameTouchedInt")
                .SetFeatures(
                    powerFeatFlameTouchedBurningHandsInt,
                    powerFeatFlameTouchedScorchingRayInt,
                    AttributeModifierCreed_Of_Pakri)
                .AddFeatures(autoPreparedSpellsFeatFlameTouched)
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(FLAME_TOUCHED)
                .AddToDB(),
            // flame touched wis
            FeatDefinitionBuilder
                .Create("FeatFlameTouchedWis")
                .SetFeatures(
                    powerFeatFlameTouchedBurningHandsWis,
                    powerFeatFlameTouchedScorchingRayWis,
                    AttributeModifierCreed_Of_Maraike)
                .AddFeatures(autoPreparedSpellsFeatFlameTouched)
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(FLAME_TOUCHED)
                .AddToDB(),
            // flame touched cha
            FeatDefinitionBuilder
                .Create("FeatFlameTouchedCha")
                .SetFeatures(
                    powerFeatFlameTouchedBurningHandsCha,
                    powerFeatFlameTouchedScorchingRayCha,
                    AttributeModifierCreed_Of_Solasta)
                .AddFeatures(autoPreparedSpellsFeatFlameTouched)
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(FLAME_TOUCHED)
                .AddToDB()
        );

        groups.Add(GroupFeats.MakeGroup("FeatGroupFlameTouched", FLAME_TOUCHED, groupFeats));
        feats.AddRange(groupFeats);

        // shadow touched

        const string SHADOW_TOUCHED = "ShadowTouched";

        var autoPreparedSpellsFeatShadowTouched = AutoPreparedClassLists(
            BuildSpellGroup(0, Invisibility, FalseLife, InflictWounds),
            GuiPresentationBuilder
                .Build("AutoPreparedSpellsFeatShadowTouched", Category.Feature),
            "AutoPreparedSpellsFeatShadowTouched", SHADOW_TOUCHED);

        var powerFeatShadowTouchedInvisibility = BuildPowerFromEffectDescription(1,
            RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
            false, false, AttributeDefinitions.Intelligence,
            Invisibility.EffectDescription, "PowerFeatShadowTouchedInvisibility", Invisibility.GuiPresentation);

        var powerFeatShadowTouchedFalseLife = BuildPowerFromEffectDescription(1,
            RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
            false, false, AttributeDefinitions.Intelligence,
            FalseLife.EffectDescription, "PowerFeatShadowTouchedFalseLife", FalseLife.GuiPresentation);

        var powerFeatShadowTouchedInflictWoundsInt = BuildPowerFromEffectDescription(1,
            RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
            true, true, AttributeDefinitions.Intelligence,
            InflictWounds.EffectDescription, "PowerFeatShadowTouchedInflictWoundsInt", InflictWounds.GuiPresentation);

        var powerFeatShadowTouchedInflictWoundsWis = BuildPowerFromEffectDescription(1,
            RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
            true, true, AttributeDefinitions.Wisdom,
            InflictWounds.EffectDescription, "PowerFeatShadowTouchedInflictWoundsWis", InflictWounds.GuiPresentation);

        var powerFeatShadowTouchedInflictWoundsCha = BuildPowerFromEffectDescription(1,
            RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
            true, true, AttributeDefinitions.Charisma,
            InflictWounds.EffectDescription, "PowerFeatShadowTouchedInflictWoundsCha", InflictWounds.GuiPresentation);

        groupFeats.SetRange(
            // shadow touched int
            FeatDefinitionBuilder
                .Create("FeatShadowTouchedInt")
                .SetFeatures(
                    powerFeatShadowTouchedInvisibility,
                    powerFeatShadowTouchedInflictWoundsInt,
                    powerFeatShadowTouchedFalseLife,
                    AttributeModifierCreed_Of_Pakri)
                .AddFeatures(autoPreparedSpellsFeatShadowTouched)
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(SHADOW_TOUCHED)
                .AddToDB(),
            // shadow touched wis
            FeatDefinitionBuilder
                .Create("FeatShadowTouchedWis")
                .SetFeatures(
                    powerFeatShadowTouchedInvisibility,
                    powerFeatShadowTouchedInflictWoundsWis,
                    powerFeatShadowTouchedFalseLife,
                    AttributeModifierCreed_Of_Maraike)
                .AddFeatures(autoPreparedSpellsFeatShadowTouched)
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(SHADOW_TOUCHED)
                .AddToDB(),
            // shadow touched cha
            FeatDefinitionBuilder
                .Create("FeatShadowTouchedCha")
                .SetFeatures(
                    powerFeatShadowTouchedInvisibility,
                    powerFeatShadowTouchedInflictWoundsCha,
                    powerFeatShadowTouchedFalseLife,
                    AttributeModifierCreed_Of_Solasta)
                .AddFeatures(autoPreparedSpellsFeatShadowTouched)
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(SHADOW_TOUCHED)
                .AddToDB()
        );

        groups.Add(GroupFeats.MakeGroup("FeatGroupShadowTouched", SHADOW_TOUCHED, groupFeats));
        feats.AddRange(groupFeats);

        GroupFeats.MakeGroup("FeatGroupPlaneTouchedMagic", null, groups);
    }

    [NotNull]
    private static FeatureDefinition[] AutoPreparedClassLists(
        FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup spellGroup,
        GuiPresentation learnShadowTouchedPresentation,
        string namePrefix,
        string autoPrepTag)
    {
        return DatabaseRepository.GetDatabase<CharacterClassDefinition>()
            .Select(x =>
                FeatureDefinitionAutoPreparedSpellsBuilder
                    .Create(namePrefix + x.Name)
                    .SetGuiPresentation(learnShadowTouchedPresentation)
                    .SetPreparedSpellGroups(spellGroup)
                    .SetSpellcastingClass(x)
                    .SetAutoTag(autoPrepTag)
                    .AddToDB())
            .Cast<FeatureDefinition>()
            .ToArray();
    }

    [NotNull]
    private static FeatureDefinitionPower BuildMotionFormPower(
        int usesPerRecharge,
        RuleDefinitions.ActivationTime activationTime,
        int costPerUse,
        RuleDefinitions.RechargeRate recharge,
        RuleDefinitions.RangeType rangeType,
        int rangeParameter,
        RuleDefinitions.TargetType targetType,
        RuleDefinitions.Side target,
        bool disableSavingThrowOnAllies,
        string savingThrowAbility,
        RuleDefinitions.EffectDifficultyClassComputation difficultyClassComputation,
        string savingThrowDifficultyAbility,
        MotionForm.MotionType motionType,
        int motionDistance,
        int fixedSavingThrowDifficultyClass,
        string name,
        GuiPresentation guiPresentation)
    {
        return BuildPowerFromEffectDescription(usesPerRecharge, activationTime, costPerUse,
            recharge,
            false,
            false,
            savingThrowDifficultyAbility,
            EffectDescriptionBuilder
                .Create()
                .SetTargetingData(target, rangeType, rangeParameter, targetType, 1, 0)
                .SetCreatedByCharacter()
                .SetSavingThrowData(
                    disableSavingThrowOnAllies,
                    savingThrowAbility,
                    true,
                    difficultyClassComputation,
                    savingThrowDifficultyAbility,
                    fixedSavingThrowDifficultyClass)
                .SetEffectForms(EffectFormBuilder
                    .Create()
                    .SetMotionForm(motionType, motionDistance)
                    .Build())
                .SetEffectAdvancement(RuleDefinitions.EffectIncrementMethod.None)
                .SetParticleEffectParameters(PowerWizardArcaneRecovery)
                .Build(),
            name,
            guiPresentation);
    }

    [NotNull]
    private static FeatureDefinitionPower BuildPowerFromEffectDescription(
        int usesPerRecharge,
        RuleDefinitions.ActivationTime activationTime,
        int costPerUse,
        RuleDefinitions.RechargeRate recharge,
        bool proficiencyBonusToAttack,
        bool abilityScoreBonusToAttack,
        string abilityScore,
        EffectDescription effectDescription,
        string name,
        GuiPresentation guiPresentation)
    {
        return FeatureDefinitionPowerBuilder
            .Create(name)
            .SetGuiPresentation(guiPresentation)
            .SetUsesFixed(
                activationTime,
                recharge,
                effectDescription,
                costPerUse,
                usesPerRecharge)
            .SetBonusToAttack(proficiencyBonusToAttack, abilityScoreBonusToAttack, abilityScore)
            .AddToDB();
    }
}
