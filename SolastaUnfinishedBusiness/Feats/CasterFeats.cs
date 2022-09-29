using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
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
    public static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        var newFeats = new List<FeatDefinition>();
        var groups = new List<FeatDefinition>();
        var classes = DatabaseRepository.GetDatabase<CharacterClassDefinition>();

        // Telekinetic general
        const string TELEKINETIC = "Telekinetic";

        var pushPresentation = GuiPresentationBuilder.Build("PowerFeatTelekineticPush", Category.Feature,
            PowerVampiricTouch.GuiPresentation.SpriteReference);

        var pullPresentation = GuiPresentationBuilder.Build("PowerFeatTelekineticPull", Category.Feature,
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
            "PowerFeatTelekineticIntPush", pushPresentation);

        var intPull = BuildMotionFormPower(
            1, RuleDefinitions.UsesDetermination.Fixed,
            RuleDefinitions.ActivationTime.BonusAction, 0, RuleDefinitions.RechargeRate.AtWill,
            RuleDefinitions.RangeType.Distance, 6, RuleDefinitions.TargetType.Individuals,
            RuleDefinitions.Side.All, true, true, AttributeDefinitions.Strength,
            RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
            AttributeDefinitions.Intelligence,
            MotionForm.MotionType.DragToOrigin, 1, 10,
            "PowerFeatTelekineticIntPull", pullPresentation);

        var intTelekineticFeat = FeatDefinitionBuilder
            .Create("FeatTelekineticInt")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(intPush, intPull,
                AttributeModifierCreed_Of_Pakri)
            .SetFeatFamily(TELEKINETIC)
            .AddToDB();

        // Telekinetic cha

        var chaPush = BuildMotionFormPower(
            1, RuleDefinitions.UsesDetermination.Fixed,
            RuleDefinitions.ActivationTime.BonusAction, 0, RuleDefinitions.RechargeRate.AtWill,
            RuleDefinitions.RangeType.Distance, 6, RuleDefinitions.TargetType.Individuals,
            RuleDefinitions.Side.All, true, true, AttributeDefinitions.Strength,
            RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Charisma,
            MotionForm.MotionType.PushFromOrigin, 1, 10,
            "PowerFeatTelekineticChaPush", pushPresentation);

        var chaPull = BuildMotionFormPower(
            1, RuleDefinitions.UsesDetermination.Fixed,
            RuleDefinitions.ActivationTime.BonusAction, 0, RuleDefinitions.RechargeRate.AtWill,
            RuleDefinitions.RangeType.Distance, 6, RuleDefinitions.TargetType.Individuals,
            RuleDefinitions.Side.All, true, true, AttributeDefinitions.Strength,
            RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Charisma,
            MotionForm.MotionType.DragToOrigin, 1, 10,
            "PowerFeatTelekineticChaPull", pullPresentation);

        var chaTelekineticFeat = FeatDefinitionBuilder
            .Create("FeatTelekineticCha")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(chaPush, chaPull,
                AttributeModifierCreed_Of_Solasta)
            .SetFeatFamily(TELEKINETIC)
            .AddToDB();

        // Telekinetic wis

        var wisPush = BuildMotionFormPower(
            1, RuleDefinitions.UsesDetermination.Fixed,
            RuleDefinitions.ActivationTime.BonusAction, 0, RuleDefinitions.RechargeRate.AtWill,
            RuleDefinitions.RangeType.Distance, 6, RuleDefinitions.TargetType.Individuals,
            RuleDefinitions.Side.All, true, true, AttributeDefinitions.Strength,
            RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Wisdom,
            MotionForm.MotionType.PushFromOrigin, 1, 10,
            "PowerFeatTelekineticWisPush", pushPresentation);

        var wisPull = BuildMotionFormPower(
            1, RuleDefinitions.UsesDetermination.Fixed,
            RuleDefinitions.ActivationTime.BonusAction, 0, RuleDefinitions.RechargeRate.AtWill,
            RuleDefinitions.RangeType.Distance, 6, RuleDefinitions.TargetType.Individuals,
            RuleDefinitions.Side.All, true, true, AttributeDefinitions.Strength,
            RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Wisdom,
            MotionForm.MotionType.DragToOrigin, 1, 10,
            "PowerFeatTelekineticWisPull", pullPresentation);

        var wisTelekineticFeat = FeatDefinitionBuilder
            .Create("FeatTelekineticWis")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(wisPush, wisPull,
                AttributeModifierCreed_Of_Maraike)
            .SetFeatFamily(TELEKINETIC)
            .AddToDB();

        newFeats.SetRange(intTelekineticFeat, chaTelekineticFeat, wisTelekineticFeat);
        groups.Add(GroupFeats.MakeGroup("FeatGroupTelekinetic", TELEKINETIC, newFeats));
        feats.AddRange(newFeats);

        // Fey Teleportation
        const string FEY_TELEPORT = "FeyTeleport";

        var mistyStepGroup = BuildSpellGroup(0, MistyStep);

        var mistyStepClassesPreparedSpells = AutoPreparedClassLists(classes,
            mistyStepGroup, MistyStep.GuiPresentation, "AutoPreparedSpellsFeyTeleportation", FEY_TELEPORT);

        var mistyStepPower = BuildPowerFromEffectDescription(
            1, RuleDefinitions.UsesDetermination.Fixed,
            RuleDefinitions.ActivationTime.BonusAction, 1, RuleDefinitions.RechargeRate.ShortRest,
            false, false, AttributeDefinitions.Intelligence,
            MistyStep.EffectDescription,
            "PowerFeatFeyTeleportationMistyStep", MistyStep.GuiPresentation);

        var feyTeleportationLanguage = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyFeatFeyTeleportationTirmarian")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(
                RuleDefinitions.ProficiencyType.Language,
                DatabaseHelper.LanguageDefinitions.Language_Tirmarian.Name)
            .AddToDB();

        newFeats.SetRange(
            // fey teleportation int
            FeatDefinitionBuilder
                .Create("FeatFeyTeleportationInt")
                .SetFeatures(AttributeModifierCreed_Of_Pakri,
                    feyTeleportationLanguage, mistyStepPower)
                .AddFeatures(mistyStepClassesPreparedSpells)
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(FEY_TELEPORT)
                .AddToDB(),
            // fey teleportation cha
            FeatDefinitionBuilder
                .Create("FeatFeyTeleportationCha")
                .SetFeatures(AttributeModifierCreed_Of_Solasta,
                    feyTeleportationLanguage, mistyStepPower)
                .AddFeatures(mistyStepClassesPreparedSpells)
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(FEY_TELEPORT)
                .AddToDB(),
            // fey teleportation wis
            FeatDefinitionBuilder
                .Create("FeatFeyTeleportationWis")
                .SetFeatures(AttributeModifierCreed_Of_Maraike,
                    feyTeleportationLanguage, mistyStepPower)
                .AddFeatures(mistyStepClassesPreparedSpells)
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(FEY_TELEPORT)
                .AddToDB()
        );

        groups.Add(GroupFeats.MakeGroup("FeatGroupTeleportation", FEY_TELEPORT, newFeats));
        feats.AddRange(newFeats);

        // celestial touched

        const string CELESTIAL_TOUCHED = "CelestialTouched";

        var celestialTouchedGroup =
            BuildSpellGroup(0, HealingWord, CureWounds, LesserRestoration);

        var learnCelestialTouchedPresentation =
            GuiPresentationBuilder.Build("PowerCelestialTouched", Category.Feature);

        var celestialTouchedClassesPreparedSpells = AutoPreparedClassLists(classes,
            celestialTouchedGroup, learnCelestialTouchedPresentation, "AutoPreparedSpellsFeatCelestialTouched",
            CELESTIAL_TOUCHED);

        var healingWordPower = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
            RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
            false, false, AttributeDefinitions.Intelligence,
            HealingWord.EffectDescription, "PowerFeatCelestialTouchedHealingWord", HealingWord.GuiPresentation);

        var cureWoundsPower = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
            RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
            false, false, AttributeDefinitions.Intelligence,
            CureWounds.EffectDescription, "PowerFeatCelestialTouchedCureWounds", CureWounds.GuiPresentation);

        var lesserRestorationPower = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
            RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
            false, false, AttributeDefinitions.Intelligence,
            LesserRestoration.EffectDescription, "PowerFeatCelestialTouchedLesserRestoration",
            LesserRestoration.GuiPresentation);

        newFeats.SetRange(
            // celestial touched int
            FeatDefinitionBuilder
                .Create("FeatCelestialTouchedInt")
                .SetFeatures(healingWordPower, cureWoundsPower, lesserRestorationPower,
                    AttributeModifierCreed_Of_Pakri)
                .AddFeatures(celestialTouchedClassesPreparedSpells)
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(CELESTIAL_TOUCHED)
                .AddToDB(),
            // celestial touched wis
            FeatDefinitionBuilder
                .Create("FeatCelestialTouchedWis")
                .SetFeatures(healingWordPower, cureWoundsPower, lesserRestorationPower,
                    AttributeModifierCreed_Of_Maraike)
                .AddFeatures(celestialTouchedClassesPreparedSpells)
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(CELESTIAL_TOUCHED)
                .AddToDB(),
            // celestial touched cha
            FeatDefinitionBuilder
                .Create("FeatCelestialTouchedCha")
                .SetFeatures(healingWordPower, cureWoundsPower, lesserRestorationPower,
                    AttributeModifierCreed_Of_Solasta)
                .AddFeatures(celestialTouchedClassesPreparedSpells)
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(CELESTIAL_TOUCHED)
                .AddToDB()
        );

        groups.Add(GroupFeats.MakeGroup("FeatGroupCelestialTouched", CELESTIAL_TOUCHED, newFeats));
        feats.AddRange(newFeats);

        // flame touched

        const string FLAME_TOUCHED = "FlameTouched";

        var flameTouchedGroup =
            BuildSpellGroup(0, BurningHands, HellishRebuke, ScorchingRay);

        var learnFlameTouchedPresentation =
            GuiPresentationBuilder.Build("PowerFlameTouched", Category.Feature);

        var flameTouchedClassesPreparedSpells = AutoPreparedClassLists(classes,
            flameTouchedGroup, learnFlameTouchedPresentation, "AutoPreparedSpellsFeatFlameTouched", FLAME_TOUCHED);

        var burningHandsPowerInt = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
            RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
            true, true, AttributeDefinitions.Intelligence,
            BurningHands.EffectDescription, "PowerFeatFlameTouchedBurningHandsInt", BurningHands.GuiPresentation);

        var burningHandsPowerWis = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
            RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
            true, true, AttributeDefinitions.Wisdom,
            BurningHands.EffectDescription, "PowerFeatFlameTouchedBurningHandsWis", BurningHands.GuiPresentation);

        var burningHandsPowerCha = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
            RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
            true, true, AttributeDefinitions.Charisma,
            BurningHands.EffectDescription, "PowerFeatFlameTouchedBurningHandsCha", BurningHands.GuiPresentation);

        var scorchingRayPowerInt = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
            RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
            true, true, AttributeDefinitions.Intelligence,
            ScorchingRay.EffectDescription, "PowerFeatFlameTouchedScorchingRayInt", ScorchingRay.GuiPresentation);

        var scorchingRayPowerWis = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
            RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
            true, true, AttributeDefinitions.Wisdom,
            ScorchingRay.EffectDescription, "PowerFeatFlameTouchedScorchingRayWis", ScorchingRay.GuiPresentation);

        var scorchingRayPowerCha = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
            RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
            true, true, AttributeDefinitions.Charisma,
            ScorchingRay.EffectDescription, "PowerFeatFlameTouchedScorchingRayCha", ScorchingRay.GuiPresentation);

        newFeats.SetRange(
            // flame touched int
            FeatDefinitionBuilder
                .Create("FeatFlameTouchedInt")
                .SetFeatures(burningHandsPowerInt, scorchingRayPowerInt,
                    AttributeModifierCreed_Of_Pakri)
                .AddFeatures(flameTouchedClassesPreparedSpells)
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(FLAME_TOUCHED)
                .AddToDB(),
            // flame touched wis
            FeatDefinitionBuilder
                .Create("FeatFlameTouchedWis")
                .SetFeatures(burningHandsPowerWis, scorchingRayPowerWis,
                    AttributeModifierCreed_Of_Maraike)
                .AddFeatures(flameTouchedClassesPreparedSpells)
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(FLAME_TOUCHED)
                .AddToDB(),
            // flame touched cha
            FeatDefinitionBuilder
                .Create("FeatFlameTouchedCha")
                .SetFeatures(burningHandsPowerCha, scorchingRayPowerCha,
                    AttributeModifierCreed_Of_Solasta)
                .AddFeatures(flameTouchedClassesPreparedSpells)
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(FLAME_TOUCHED)
                .AddToDB()
        );

        groups.Add(GroupFeats.MakeGroup("FeatGroupFlameTouched", FLAME_TOUCHED, newFeats));
        feats.AddRange(newFeats);

        // shadow touched

        var shadowTouchedGroup =
            BuildSpellGroup(0, Invisibility, FalseLife, InflictWounds);

        const string SHADOW_TOUCHED = "ShadowTouched";

        var learnShadowTouchedPresentation =
            GuiPresentationBuilder.Build("PowerShadowTouched", Category.Feature);

        var shadowTouchedClassesPreparedSpells = AutoPreparedClassLists(classes,
            shadowTouchedGroup, learnShadowTouchedPresentation, "AutoPreparedSpellsFeatShadowTouched", SHADOW_TOUCHED);

        var invisibilityPower = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
            RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
            false, false, AttributeDefinitions.Intelligence,
            Invisibility.EffectDescription, "PowerFeatShadowTouchedInvisibility", Invisibility.GuiPresentation);

        var falseLifePower = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
            RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
            false, false, AttributeDefinitions.Intelligence,
            FalseLife.EffectDescription, "PowerFeatShadowTouchedFalseLife", FalseLife.GuiPresentation);

        var inflictWoundsPowerInt = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
            RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
            true, true, AttributeDefinitions.Intelligence,
            InflictWounds.EffectDescription, "PowerFeatShadowTouchedInflictWoundsInt", InflictWounds.GuiPresentation);

        var inflictWoundsPowerWis = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
            RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
            true, true, AttributeDefinitions.Wisdom,
            InflictWounds.EffectDescription, "PowerFeatShadowTouchedInflictWoundsWis", InflictWounds.GuiPresentation);

        var inflictWoundsPowerCha = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
            RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
            true, true, AttributeDefinitions.Charisma,
            InflictWounds.EffectDescription, "PowerFeatShadowTouchedInflictWoundsCha", InflictWounds.GuiPresentation);

        newFeats.SetRange(
            // shadow touched int
            FeatDefinitionBuilder
                .Create("FeatShadowTouchedInt")
                .SetFeatures(invisibilityPower, inflictWoundsPowerInt, falseLifePower,
                    AttributeModifierCreed_Of_Pakri)
                .AddFeatures(shadowTouchedClassesPreparedSpells)
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(SHADOW_TOUCHED)
                .AddToDB(),
            // shadow touched wis
            FeatDefinitionBuilder
                .Create("FeatShadowTouchedWis")
                .SetFeatures(invisibilityPower, inflictWoundsPowerWis, falseLifePower,
                    AttributeModifierCreed_Of_Maraike)
                .AddFeatures(shadowTouchedClassesPreparedSpells)
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(SHADOW_TOUCHED)
                .AddToDB(),
            // shadow touched cha
            FeatDefinitionBuilder
                .Create("FeatShadowTouchedCha")
                .SetFeatures(invisibilityPower, inflictWoundsPowerCha, falseLifePower,
                    AttributeModifierCreed_Of_Solasta)
                .AddFeatures(shadowTouchedClassesPreparedSpells)
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(SHADOW_TOUCHED)
                .AddToDB()
        );

        groups.Add(GroupFeats.MakeGroup("FeatGroupShadowTouched", SHADOW_TOUCHED, newFeats));
        feats.AddRange(newFeats);

        GroupFeats.MakeGroup("FeatGroupPlaneTouchedMagic", null, groups);
    }

    [NotNull]
    private static FeatureDefinition[] AutoPreparedClassLists(
        [NotNull] IEnumerable<CharacterClassDefinition> classes,
        FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup spellGroup,
        GuiPresentation learnShadowTouchedPresentation,
        string namePrefix,
        string autoPrepTag)
    {
        return classes
            .Select(klass =>
                BuildAutoPreparedSpells(
                    new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup> { spellGroup },
                    klass,
                    namePrefix + klass.Name,
                    autoPrepTag,
                    learnShadowTouchedPresentation))
            .Cast<FeatureDefinition>()
            .ToArray();
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
            .Create(name)
            .SetGuiPresentation(guiPresentation)
            .Configure(
                usesPerRecharge, usesDetermination, abilityScore, activationTime, costPerUse, recharge,
                proficiencyBonusToAttack, abilityScoreBonusToAttack, abilityScore, effectDescription /* unique */)
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
            .Create(name)
            .SetGuiPresentation(guiPresentation)
            .SetPreparedSpellGroups(autoSpellLists)
            .SetCastingClass(characterClass)
            .SetAutoTag(tag)
            .AddToDB();
    }
}
