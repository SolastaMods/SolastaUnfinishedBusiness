using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Api.Infrastructure;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.Spells;
using static SolastaCommunityExpansion.Builders.Features.AutoPreparedSpellsGroupBuilder;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaCommunityExpansion.Api.DatabaseHelper.SpellDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;

namespace SolastaCommunityExpansion.Feats;

internal static class CasterFeats
{
    private static readonly Guid CasterFeatsNamespace = new("bf70984d-e7b9-446a-9ae3-0f2039de833d");

    public static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        var classes = DatabaseRepository.GetDatabase<CharacterClassDefinition>();

        // Telekinetic general

        var pushPresentation = GuiPresentationBuilder.Build("FeatTelekineticPush", Category.Feat,
            PowerVampiricTouch.GuiPresentation.SpriteReference);

        var pullPresentation = GuiPresentationBuilder.Build("FeatTelekineticPull", Category.Feat,
            PowerVampiricTouch.GuiPresentation.SpriteReference);

        // Telekinetic int

        var intPush = BuildMotionFormPower(
            1, RuleDefinitions.UsesDetermination.Fixed,
            RuleDefinitions.ActivationTime.BonusAction, 0, RuleDefinitions.RechargeRate.AtWill,
            RuleDefinitions.RangeType.Distance, 6, RuleDefinitions.TargetType.Individuals,
            RuleDefinitions.Side.All, true, true, AttributeDefinitions.Strength,
            RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
            AttributeDefinitions.Intelligence,
            MotionForm.MotionType.PushFromOrigin, 1, 10,
            "PowerTelekineticIntPush", pushPresentation);

        var intPull = BuildMotionFormPower(
            1, RuleDefinitions.UsesDetermination.Fixed,
            RuleDefinitions.ActivationTime.BonusAction, 0, RuleDefinitions.RechargeRate.AtWill,
            RuleDefinitions.RangeType.Distance, 6, RuleDefinitions.TargetType.Individuals,
            RuleDefinitions.Side.All, true, true, AttributeDefinitions.Strength,
            RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
            AttributeDefinitions.Intelligence,
            MotionForm.MotionType.DragToOrigin, 1, 10,
            "PowerTelekineticIntPull", pullPresentation);

        var intTelekineticFeat = FeatDefinitionBuilder
            .Create("FeatTelekineticInt", CasterFeatsNamespace)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(intPush, intPull,
                AttributeModifierCreed_Of_Pakri)
            .AddToDB();

        feats.Add(intTelekineticFeat);

        // Telekinetic cha

        var chaPush = BuildMotionFormPower(
            1, RuleDefinitions.UsesDetermination.Fixed,
            RuleDefinitions.ActivationTime.BonusAction, 0, RuleDefinitions.RechargeRate.AtWill,
            RuleDefinitions.RangeType.Distance, 6, RuleDefinitions.TargetType.Individuals,
            RuleDefinitions.Side.All, true, true, AttributeDefinitions.Strength,
            RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Charisma,
            MotionForm.MotionType.PushFromOrigin, 1, 10,
            "PowerTelekineticChaPush", pushPresentation);

        var chaPull = BuildMotionFormPower(
            1, RuleDefinitions.UsesDetermination.Fixed,
            RuleDefinitions.ActivationTime.BonusAction, 0, RuleDefinitions.RechargeRate.AtWill,
            RuleDefinitions.RangeType.Distance, 6, RuleDefinitions.TargetType.Individuals,
            RuleDefinitions.Side.All, true, true, AttributeDefinitions.Strength,
            RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Charisma,
            MotionForm.MotionType.DragToOrigin, 1, 10,
            "PowerTelekineticChaPull", pullPresentation);

        var chaTelekineticFeat = FeatDefinitionBuilder
            .Create("FeatTelekineticCha", CasterFeatsNamespace)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(chaPush, chaPull,
                AttributeModifierCreed_Of_Solasta)
            .AddToDB();

        feats.Add(chaTelekineticFeat);

        // Telekinetic wis

        var wisPush = BuildMotionFormPower(
            1, RuleDefinitions.UsesDetermination.Fixed,
            RuleDefinitions.ActivationTime.BonusAction, 0, RuleDefinitions.RechargeRate.AtWill,
            RuleDefinitions.RangeType.Distance, 6, RuleDefinitions.TargetType.Individuals,
            RuleDefinitions.Side.All, true, true, AttributeDefinitions.Strength,
            RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Wisdom,
            MotionForm.MotionType.PushFromOrigin, 1, 10,
            "PowerTelekineticWisPush", pushPresentation);

        var wisPull = BuildMotionFormPower(
            1, RuleDefinitions.UsesDetermination.Fixed,
            RuleDefinitions.ActivationTime.BonusAction, 0, RuleDefinitions.RechargeRate.AtWill,
            RuleDefinitions.RangeType.Distance, 6, RuleDefinitions.TargetType.Individuals,
            RuleDefinitions.Side.All, true, true, AttributeDefinitions.Strength,
            RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Wisdom,
            MotionForm.MotionType.DragToOrigin, 1, 10,
            "PowerTelekineticWisPull", pullPresentation);

        var wisTelekineticFeat = FeatDefinitionBuilder
            .Create("FeatTelekineticWis", CasterFeatsNamespace)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(wisPush, wisPull,
                AttributeModifierCreed_Of_Maraike)
            .AddToDB();

        feats.Add(wisTelekineticFeat);

        // Fey Teleportation

        var mistyStepGroup = BuildSpellGroup(0, MistyStep);

        var mistyStepClassesPreparedSpells = AutoPreparedClassLists(classes,
            mistyStepGroup, MistyStep.GuiPresentation, "FeyTeleportationAutoPrepMisty", "FeyTeleport");

        var mistyStepPower = BuildPowerFromEffectDescription(
            1, RuleDefinitions.UsesDetermination.Fixed,
            RuleDefinitions.ActivationTime.BonusAction, 1, RuleDefinitions.RechargeRate.ShortRest,
            false, false, AttributeDefinitions.Intelligence,
            MistyStep.EffectDescription,
            "PowerMistyStepFromFeat", MistyStep.GuiPresentation);

        var feyTeleportationLanguage = FeatureDefinitionProficiencyBuilder
            .Create("FeyTeleportationLanguageTirmarian", CasterFeatsNamespace)
            .SetGuiPresentation(Category.Feat)
            .SetProficiencies(
                RuleDefinitions.ProficiencyType.Language,
                DatabaseHelper.LanguageDefinitions.Language_Tirmarian.Name)
            .AddToDB();

        feats.AddRange(
            // fey teleportation int
            FeatDefinitionBuilder
                .Create("FeatFeyTeleportationInt", CasterFeatsNamespace)
                .SetFeatures(AttributeModifierCreed_Of_Pakri,
                    feyTeleportationLanguage, mistyStepPower)
                .AddFeatures(mistyStepClassesPreparedSpells)
                .SetGuiPresentation(Category.Feat)
                .AddToDB(),
            // fey teleportation cha
            FeatDefinitionBuilder
                .Create("FeatFeyTeleportationCha", CasterFeatsNamespace)
                .SetFeatures(AttributeModifierCreed_Of_Solasta,
                    feyTeleportationLanguage, mistyStepPower)
                .AddFeatures(mistyStepClassesPreparedSpells)
                .SetGuiPresentation(Category.Feat)
                .AddToDB(),
            // fey teleportation wis
            FeatDefinitionBuilder
                .Create("FeatFeyTeleportationWis", CasterFeatsNamespace)
                .SetFeatures(AttributeModifierCreed_Of_Maraike,
                    feyTeleportationLanguage, mistyStepPower)
                .AddFeatures(mistyStepClassesPreparedSpells)
                .SetGuiPresentation(Category.Feat)
                .AddToDB()
        );

        // celestial touched

        var celestialTouchedGroup =
            BuildSpellGroup(0, HealingWord, CureWounds, LesserRestoration);

        var learnCelestialTouchedPresentation =
            GuiPresentationBuilder.Build("PowerCelestialTouchedFromFeat", Category.Feat);

        var celestialTouchedClassesPreparedSpells = AutoPreparedClassLists(classes,
            celestialTouchedGroup, learnCelestialTouchedPresentation, "CelestialTouchedAutoPrep", "CelestialTouched");

        var healingWordPower = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
            RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
            false, false, AttributeDefinitions.Intelligence,
            HealingWord.EffectDescription, "PowerHealingWordFromFeat", HealingWord.GuiPresentation);

        var cureWoundsPower = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
            RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
            false, false, AttributeDefinitions.Intelligence,
            CureWounds.EffectDescription, "PowerCureWoundsFromFeat", CureWounds.GuiPresentation);

        var lesserRestorationPower = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
            RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
            false, false, AttributeDefinitions.Intelligence,
            LesserRestoration.EffectDescription, "PowerLesserRestorationFromFeat", LesserRestoration.GuiPresentation);

        feats.AddRange(
            // celestial touched int
            FeatDefinitionBuilder
                .Create("FeatCelestialTouchedInt", CasterFeatsNamespace)
                .SetFeatures(healingWordPower, cureWoundsPower, lesserRestorationPower,
                    AttributeModifierCreed_Of_Pakri)
                .AddFeatures(celestialTouchedClassesPreparedSpells)
                .SetGuiPresentation(Category.Feat)
                .AddToDB(),
            // celestial touched wis
            FeatDefinitionBuilder
                .Create("FeatCelestialTouchedWis", CasterFeatsNamespace)
                .SetFeatures(healingWordPower, cureWoundsPower, lesserRestorationPower,
                    AttributeModifierCreed_Of_Maraike)
                .AddFeatures(celestialTouchedClassesPreparedSpells)
                .SetGuiPresentation(Category.Feat)
                .AddToDB(),
            // celestial touched cha
            FeatDefinitionBuilder
                .Create("FeatCelestialTouchedCha", CasterFeatsNamespace)
                .SetFeatures(healingWordPower, cureWoundsPower, lesserRestorationPower,
                    AttributeModifierCreed_Of_Solasta)
                .AddFeatures(celestialTouchedClassesPreparedSpells)
                .SetGuiPresentation(Category.Feat)
                .AddToDB()
        );

        // flame touched

        var flameTouchedGroup =
            BuildSpellGroup(0, BurningHands, HellishRebuke, ScorchingRay);

        var learnFlameTouchedPresentation =
            GuiPresentationBuilder.Build("PowerFlameTouchedFromFeat", Category.Feat);

        var flameTouchedClassesPreparedSpells = AutoPreparedClassLists(classes,
            flameTouchedGroup, learnFlameTouchedPresentation, "FlameTouchedAutoPrep", "FlameTouched");

        var burningHandsPowerInt = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
            RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
            true, true, AttributeDefinitions.Intelligence,
            BurningHands.EffectDescription, "PowerBurningHandsIntFromFeat", BurningHands.GuiPresentation);

        var burningHandsPowerWis = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
            RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
            true, true, AttributeDefinitions.Wisdom,
            BurningHands.EffectDescription, "PowerBurningHandsWisFromFeat", BurningHands.GuiPresentation);

        var burningHandsPowerCha = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
            RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
            true, true, AttributeDefinitions.Charisma,
            BurningHands.EffectDescription, "PowerBurningHandsChaFromFeat", BurningHands.GuiPresentation);

        var scorchingRayPowerInt = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
            RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
            true, true, AttributeDefinitions.Intelligence,
            ScorchingRay.EffectDescription, "PowerScorchingRayIntFromFeat", ScorchingRay.GuiPresentation);

        var scorchingRayPowerWis = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
            RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
            true, true, AttributeDefinitions.Wisdom,
            ScorchingRay.EffectDescription, "PowerScorchingRayWisFromFeat", ScorchingRay.GuiPresentation);

        var scorchingRayPowerCha = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
            RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
            true, true, AttributeDefinitions.Charisma,
            ScorchingRay.EffectDescription, "PowerScorchingRayChaFromFeat", ScorchingRay.GuiPresentation);

        feats.AddRange(
            // flame touched int
            FeatDefinitionBuilder
                .Create("FeatFlameTouchedInt", CasterFeatsNamespace)
                .SetFeatures(burningHandsPowerInt, scorchingRayPowerInt,
                    AttributeModifierCreed_Of_Pakri)
                .AddFeatures(flameTouchedClassesPreparedSpells)
                .SetGuiPresentation(Category.Feat)
                .AddToDB(),
            // flame touched wis
            FeatDefinitionBuilder
                .Create("FeatFlameTouchedWis", CasterFeatsNamespace)
                .SetFeatures(burningHandsPowerWis, scorchingRayPowerWis,
                    AttributeModifierCreed_Of_Maraike)
                .AddFeatures(flameTouchedClassesPreparedSpells)
                .SetGuiPresentation(Category.Feat)
                .AddToDB(),
            // flame touched cha
            FeatDefinitionBuilder
                .Create("FeatFlameTouchedCha", CasterFeatsNamespace)
                .SetFeatures(burningHandsPowerCha, scorchingRayPowerCha,
                    AttributeModifierCreed_Of_Solasta)
                .AddFeatures(flameTouchedClassesPreparedSpells)
                .SetGuiPresentation(Category.Feat)
                .AddToDB()
        );

        // shadow touched

        var shadowTouchedGroup =
            BuildSpellGroup(0, Invisibility, FalseLife, InflictWounds);

        var learnShadowTouchedPresentation =
            GuiPresentationBuilder.Build("PowerShadowTouchedFromFeat", Category.Feat);

        var shadowTouchedClassesPreparedSpells = AutoPreparedClassLists(classes,
            shadowTouchedGroup, learnShadowTouchedPresentation, "ShadowTouchedAutoPrep", "ShadowTouched");

        var invisibilityPower = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
            RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
            false, false, AttributeDefinitions.Intelligence,
            Invisibility.EffectDescription, "PowerInvisibilityFromFeat", Invisibility.GuiPresentation);

        var falseLifePower = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
            RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
            false, false, AttributeDefinitions.Intelligence,
            FalseLife.EffectDescription, "PowerFalseLifeFromFeat", FalseLife.GuiPresentation);

        var inflictWoundsPowerInt = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
            RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
            true, true, AttributeDefinitions.Intelligence,
            InflictWounds.EffectDescription, "PowerInflictWoundsIntFromFeat", InflictWounds.GuiPresentation);

        var inflictWoundsPowerWis = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
            RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
            true, true, AttributeDefinitions.Wisdom,
            InflictWounds.EffectDescription, "PowerInflictWoundsWisFromFeat", InflictWounds.GuiPresentation);

        var inflictWoundsPowerCha = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
            RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
            true, true, AttributeDefinitions.Charisma,
            InflictWounds.EffectDescription, "PowerInflictWoundsChaFromFeat", InflictWounds.GuiPresentation);

        feats.AddRange(
            // shadow touched int
            FeatDefinitionBuilder
                .Create("FeatShadowTouchedInt", CasterFeatsNamespace)
                .SetFeatures(invisibilityPower, inflictWoundsPowerInt, falseLifePower,
                    AttributeModifierCreed_Of_Pakri)
                .AddFeatures(shadowTouchedClassesPreparedSpells)
                .SetGuiPresentation(Category.Feat)
                .AddToDB(),
            // shadow touched wis
            FeatDefinitionBuilder
                .Create("FeatShadowTouchedWis", CasterFeatsNamespace)
                .SetFeatures(invisibilityPower, inflictWoundsPowerWis, falseLifePower,
                    AttributeModifierCreed_Of_Maraike)
                .AddFeatures(shadowTouchedClassesPreparedSpells)
                .SetGuiPresentation(Category.Feat)
                .AddToDB(),
            // shadow touched cha
            FeatDefinitionBuilder
                .Create("FeatShadowTouchedCha", CasterFeatsNamespace)
                .SetFeatures(invisibilityPower, inflictWoundsPowerCha, falseLifePower,
                    AttributeModifierCreed_Of_Solasta)
                .AddFeatures(shadowTouchedClassesPreparedSpells)
                .SetGuiPresentation(Category.Feat)
                .AddToDB()
        );
    }

    [NotNull]
    private static List<FeatureDefinition> AutoPreparedClassLists(
        [NotNull] IEnumerable<CharacterClassDefinition> classes,
        FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup spellGroup,
        GuiPresentation learnShadowTouchedPresentation,
        string namePrefix,
        string autoPrepTag)
    {
        return classes
            .Select(klass =>
                BuildAutoPreparedSpells(
                    new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup> {spellGroup},
                    klass,
                    namePrefix + klass.Name,
                    autoPrepTag,
                    learnShadowTouchedPresentation))
            .Cast<FeatureDefinition>()
            .ToList();
    }

    private static FeatureDefinitionPower BuildMotionFormPower(
        int usesPerRecharge,
        RuleDefinitions.UsesDetermination usesDetermination,
        RuleDefinitions.ActivationTime activationTime,
        int costPerUse,
        RuleDefinitions.RechargeRate recharge,
        RuleDefinitions.RangeType rangeType,
        int rangeParameter,
        RuleDefinitions.TargetType targetType,
        RuleDefinitions.Side target,
        bool hasSavingThrow,
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
        var effectDescriptionBuilder = new EffectDescriptionBuilder();

        effectDescriptionBuilder.SetTargetingData(target, rangeType, rangeParameter, targetType, 1, 0);
        effectDescriptionBuilder.SetCreatedByCharacter();
        effectDescriptionBuilder.SetSavingThrowData(
            hasSavingThrow, disableSavingThrowOnAllies, savingThrowAbility, true, difficultyClassComputation,
            savingThrowDifficultyAbility, fixedSavingThrowDifficultyClass);

        var particleParams = new EffectParticleParameters();

        particleParams.Copy(PowerWizardArcaneRecovery.EffectDescription.EffectParticleParameters);

        var effectFormBuilder = new EffectFormBuilder();

        effectFormBuilder.SetMotionForm(motionType, motionDistance);
        effectDescriptionBuilder.AddEffectForm(effectFormBuilder.Build());
        effectDescriptionBuilder.SetEffectAdvancement(RuleDefinitions.EffectIncrementMethod.None);
        effectDescriptionBuilder.SetParticleEffectParameters(particleParams);

        return BuildPowerFromEffectDescription(usesPerRecharge, usesDetermination, activationTime, costPerUse,
            recharge,
            false, false, savingThrowDifficultyAbility, effectDescriptionBuilder.Build(), name, guiPresentation);
    }

    private static FeatureDefinitionPower BuildPowerFromEffectDescription(
        int usesPerRecharge,
        RuleDefinitions.UsesDetermination usesDetermination,
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
            .Create(name, CasterFeatsNamespace)
            .SetGuiPresentation(guiPresentation)
            .Configure(
                usesPerRecharge, usesDetermination, abilityScore, activationTime, costPerUse, recharge,
                proficiencyBonusToAttack, abilityScoreBonusToAttack, abilityScore, effectDescription,
                false /* unique */)
            .AddToDB();
    }

    private static FeatureDefinitionAutoPreparedSpells BuildAutoPreparedSpells(
        IEnumerable<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup> autoSpellLists,
        CharacterClassDefinition characterClass,
        string name,
        string tag,
        GuiPresentation guiPresentation)
    {
        return FeatureDefinitionAutoPreparedSpellsBuilder
            .Create(name, CasterFeatsNamespace)
            .SetGuiPresentation(guiPresentation)
            .SetPreparedSpellGroups(autoSpellLists)
            .SetCastingClass(characterClass)
            .SetAutoTag(tag)
            .AddToDB();
    }
}
