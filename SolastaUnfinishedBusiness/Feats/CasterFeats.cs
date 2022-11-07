using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.Models;
using static RuleDefinitions;
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
        FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup spells;
        FeatureDefinitionAutoPreparedSpells autoPreparedSpells;

        // telekinetic general
        const string TELEKINETIC = "Telekinetic";

        var pushPresentation = GuiPresentationBuilder.Build("PowerFeatTelekineticPush", Category.Feature,
            PowerVampiricTouch.GuiPresentation.SpriteReference);

        var pullPresentation = GuiPresentationBuilder.Build("PowerFeatTelekineticPull", Category.Feature,
            PowerVampiricTouch.GuiPresentation.SpriteReference);

        // telekinetic int

        var powerFeatTelekineticIntPush = BuildMotionFormPower("PowerFeatTelekineticIntPush",
            AttributeDefinitions.Intelligence, MotionForm.MotionType.PushFromOrigin, pushPresentation);

        var powerFeatTelekineticIntPull = BuildMotionFormPower("PowerFeatTelekineticIntPull",
            AttributeDefinitions.Intelligence, MotionForm.MotionType.DragToOrigin, pullPresentation);

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

        var powerFeatTelekineticChaPush = BuildMotionFormPower("PowerFeatTelekineticChaPush",
            AttributeDefinitions.Charisma, MotionForm.MotionType.PushFromOrigin, pushPresentation);

        var powerFeatTelekineticChaPull = BuildMotionFormPower("PowerFeatTelekineticChaPull",
            AttributeDefinitions.Charisma, MotionForm.MotionType.DragToOrigin, pullPresentation);

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

        var powerFeatTelekineticWisPush = BuildMotionFormPower("PowerFeatTelekineticWisPush",
            AttributeDefinitions.Wisdom, MotionForm.MotionType.PushFromOrigin, pushPresentation);

        var powerFeatTelekineticWisPull = BuildMotionFormPower("PowerFeatTelekineticWisPull",
            AttributeDefinitions.Wisdom, MotionForm.MotionType.DragToOrigin, pullPresentation);

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

        var powerFeatFeyTeleportationMistyStep = FeatureDefinitionPowerBuilder
            .Create("PowerFeatFeyTeleportationMistyStep")
            .SetGuiPresentation(MistyStep.GuiPresentation)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.ShortRest)
            .SetEffectDescription(MistyStep.EffectDescription)
            .AddToDB();

        var proficiencyFeatFeyTeleportationTirmarian = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyFeatFeyTeleportationTirmarian")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(
                ProficiencyType.Language, "Language_Tirmarian")
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

        var powerFeatCelestialTouchedHealingWord = FeatureDefinitionPowerBuilder
            .Create("PowerFeatCelestialTouchedHealingWord")
            .SetGuiPresentation(HealingWord.GuiPresentation)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(HealingWord.EffectDescription)
            .AddToDB();

        var powerFeatCelestialTouchedCureWounds = FeatureDefinitionPowerBuilder
            .Create("PowerFeatCelestialTouchedCureWounds")
            .SetGuiPresentation(CureWounds.GuiPresentation)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(CureWounds.EffectDescription)
            .AddToDB();

        var powerFeatCelestialTouchedLesserRestoration = FeatureDefinitionPowerBuilder
            .Create("PowerFeatCelestialTouchedLesserRestoration")
            .SetGuiPresentation(LesserRestoration.GuiPresentation)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(LesserRestoration.EffectDescription)
            .AddToDB();

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

        #region Flame Touched

        const string FLAME_TOUCHED = "FlameTouched";

        spells = BuildSpellGroup(0, BurningHands, HellishRebuke, ScorchingRay);

        autoPreparedSpells = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsFeatFlameTouched")
            .SetGuiPresentationNoContent(true)
            .SetCustomSubFeatures(ValidateReperoireForAutoprep.AnyClassOrSubclass)
            .SetPreparedSpellGroups(spells)
            .SetSpellcastingClass(null)
            .SetAutoTag(FLAME_TOUCHED)
            .AddToDB();

        groupFeats.SetRange(
            // flame touched int
            FeatDefinitionBuilder
                .Create("FeatFlameTouchedInt")
                .SetFeatures(autoPreparedSpells, AttributeModifierCreed_Of_Pakri)
                .AddFeatures(MakeSpellFeatureAndInvocations(spells, FLAME_TOUCHED, AttributeDefinitions.Intelligence))
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(FLAME_TOUCHED)
                .AddToDB(),
            // flame touched wis
            FeatDefinitionBuilder
                .Create("FeatFlameTouchedWis")
                .SetFeatures(autoPreparedSpells, AttributeModifierCreed_Of_Maraike)
                .AddFeatures(MakeSpellFeatureAndInvocations(spells, FLAME_TOUCHED, AttributeDefinitions.Wisdom))
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(FLAME_TOUCHED)
                .AddToDB(),
            // flame touched cha
            FeatDefinitionBuilder
                .Create("FeatFlameTouchedCha")
                .SetFeatures(autoPreparedSpells, AttributeModifierCreed_Of_Solasta)
                .AddFeatures(MakeSpellFeatureAndInvocations(spells, FLAME_TOUCHED, AttributeDefinitions.Charisma))
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(FLAME_TOUCHED)
                .AddToDB()
        );

        groups.Add(GroupFeats.MakeGroup("FeatGroupFlameTouched", FLAME_TOUCHED, groupFeats));
        feats.AddRange(groupFeats);

        #endregion

        // shadow touched

        const string SHADOW_TOUCHED = "ShadowTouched";

        var autoPreparedSpellsFeatShadowTouched = AutoPreparedClassLists(
            BuildSpellGroup(0, Invisibility, FalseLife, InflictWounds),
            GuiPresentationBuilder
                .Build("AutoPreparedSpellsFeatShadowTouched", Category.Feature),
            "AutoPreparedSpellsFeatShadowTouched", SHADOW_TOUCHED);

        var powerFeatShadowTouchedInvisibility = FeatureDefinitionPowerBuilder
            .Create("PowerFeatShadowTouchedInvisibility")
            .SetGuiPresentation(Invisibility.GuiPresentation)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(Invisibility.EffectDescription)
            .AddToDB();

        var powerFeatShadowTouchedFalseLife = FeatureDefinitionPowerBuilder
            .Create("PowerFeatShadowTouchedFalseLife")
            .SetGuiPresentation(FalseLife.GuiPresentation)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(FalseLife.EffectDescription)
            .AddToDB();

        var powerFeatShadowTouchedInflictWoundsInt = FeatureDefinitionPowerBuilder
            .Create("PowerFeatShadowTouchedInflictWoundsInt")
            .SetGuiPresentation(InflictWounds.GuiPresentation)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(InflictWounds.EffectDescription)
            .SetBonusToAttack(true, true)
            .AddToDB();

        var powerFeatShadowTouchedInflictWoundsWis = FeatureDefinitionPowerBuilder
            .Create("PowerFeatShadowTouchedInflictWoundsWis")
            .SetGuiPresentation(InflictWounds.GuiPresentation)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(InflictWounds.EffectDescription)
            .SetBonusToAttack(true, true, AttributeDefinitions.Wisdom)
            .AddToDB();

        var powerFeatShadowTouchedInflictWoundsCha = FeatureDefinitionPowerBuilder
            .Create("PowerFeatShadowTouchedInflictWoundsCha")
            .SetGuiPresentation(InflictWounds.GuiPresentation)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(InflictWounds.EffectDescription)
            .SetBonusToAttack(true, true, AttributeDefinitions.Charisma)
            .AddToDB();

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
    private static FeatureDefinition[] MakeSpellFeatureAndInvocations(
        FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup spellGroup,
        string name,
        string castingAttribute)
    {
        var featureName = $"CastSpell{name}{castingAttribute}";
        var spellfeature = FeatureDefinitionCastSpellBuilder
            .Create(featureName)
            .SetGuiPresentationNoContent(true)
            .SetFocusType(EquipmentDefinitions.FocusType.None)
            .SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Race)
            .SetSpellReadyness(SpellReadyness.AllKnown)
            .SetSlotsRecharge(RechargeRate.None)
            .SetSlotsPerLevel(FeatureDefinitionCastSpellBuilder.CasterProgression.None)
            .SetSpellList(SpellsContext.EmptySpellList)
            .SetSpellCastingAbility(castingAttribute)
            .AddToDB();

        var invocations = new List<InvocationDefinition>();

        foreach (var spell in spellGroup.SpellsList)
        {
            if (spell.castingTime is ActivationTime.Reaction) { continue; }

            invocations.Add(CustomInvocationDefinitionBuilder
                .Create($"CustomInvocation{name}{spell.Name}{castingAttribute}")
                .SetGuiPresentation(spell.GuiPresentation) //TODO: auto-generate based on spell
                .SetCustomSubFeatures(ValidateReperoireForAutoprep.HasSpellCastingFeature(featureName))
                .SetPoolType(null)
                .SetGrantedSpell(spell, longRestRecharge: true)
                .AddToDB());
        }

        var grant = FeatureDefinitionGrantInvocationsBuilder
            .Create($"GrantInvocations{name}{castingAttribute}")
            .SetInvocations(invocations)
            .AddToDB();


        return new FeatureDefinition[] {spellfeature, grant};
    }

    [NotNull]
    private static FeatureDefinitionPower BuildMotionFormPower(
        string name,
        string savingThrowDifficultyAbility,
        MotionForm.MotionType motionType,
        GuiPresentation guiPresentation)
    {
        return FeatureDefinitionPowerBuilder
            .Create(name)
            .SetGuiPresentation(guiPresentation)
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(
                    Side.All, RangeType.Distance, 6,
                    TargetType.Individuals)
                .SetCreatedByCharacter()
                .SetSavingThrowData(
                    true,
                    AttributeDefinitions.Strength,
                    true,
                    EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                    savingThrowDifficultyAbility)
                .SetEffectForms(EffectFormBuilder
                    .Create()
                    .SetMotionForm(motionType, 1)
                    .Build())
                .SetEffectAdvancement(EffectIncrementMethod.None)
                .SetParticleEffectParameters(PowerWizardArcaneRecovery)
                .Build())
            .AddToDB();
    }
}
