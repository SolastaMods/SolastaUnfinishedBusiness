using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using UnityEngine.AddressableAssets;
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

        #region Telekinetic

        // telekinetic general
        const string TELEKINETIC = "Telekinetic";

        // telekinetic int

        var featTelekineticInt = FeatDefinitionBuilder
            .Create("FeatTelekineticInt")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                BuildTelekinesis(AttributeDefinitions.Intelligence, MotionForm.MotionType.DragToOrigin),
                BuildTelekinesis(AttributeDefinitions.Intelligence, MotionForm.MotionType.PushFromOrigin),
                AttributeModifierCreed_Of_Pakri)
            .SetFeatFamily(TELEKINETIC)
            .AddToDB();

        // telekinetic cha


        var featTelekineticCha = FeatDefinitionBuilder
            .Create("FeatTelekineticCha")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                BuildTelekinesis(AttributeDefinitions.Charisma, MotionForm.MotionType.DragToOrigin),
                BuildTelekinesis(AttributeDefinitions.Charisma, MotionForm.MotionType.PushFromOrigin),
                AttributeModifierCreed_Of_Solasta)
            .SetFeatFamily(TELEKINETIC)
            .AddToDB();

        // telekinetic wis

        var featTelekineticWis = FeatDefinitionBuilder
            .Create("FeatTelekineticWis")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                BuildTelekinesis(AttributeDefinitions.Wisdom, MotionForm.MotionType.DragToOrigin),
                BuildTelekinesis(AttributeDefinitions.Wisdom, MotionForm.MotionType.PushFromOrigin),
                AttributeModifierCreed_Of_Maraike)
            .SetFeatFamily(TELEKINETIC)
            .AddToDB();

        groupFeats.SetRange(featTelekineticInt, featTelekineticCha, featTelekineticWis);
        groups.Add(GroupFeats.MakeGroup("FeatGroupTelekinetic", TELEKINETIC, groupFeats));
        feats.AddRange(groupFeats);

        #endregion

        #region Fey Teleportation

        const string FEY_TELEPORT = "FeyTeleport";

        var spells = BuildSpellGroup(0, MistyStep);

        var autoPreparedSpells = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsFeyTeleportation")
            .SetGuiPresentationNoContent(true)
            .AddCustomSubFeatures(ValidateRepertoireForAutoprepared.AnyClassOrSubclass)
            .SetPreparedSpellGroups(spells)
            .SetSpellcastingClass(null)
            .SetAutoTag(FEY_TELEPORT)
            .AddToDB();

        var learnTirmarian = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyFeatFeyTeleportationTirmarian")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.Language, "Language_Tirmarian")
            .AddToDB();

        groupFeats.SetRange(
            // fey teleportation int
            FeatDefinitionBuilder
                .Create("FeatFeyTeleportationInt")
                .SetFeatures(autoPreparedSpells, AttributeModifierCreed_Of_Pakri, learnTirmarian)
                .AddFeatures(MakeSpellFeatureAndInvocations(spells, FEY_TELEPORT, AttributeDefinitions.Intelligence,
                    false))
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(FEY_TELEPORT)
                .AddToDB(),
            // fey teleportation cha
            FeatDefinitionBuilder
                .Create("FeatFeyTeleportationCha")
                .SetFeatures(autoPreparedSpells, AttributeModifierCreed_Of_Solasta, learnTirmarian)
                .AddFeatures(MakeSpellFeatureAndInvocations(spells, FEY_TELEPORT, AttributeDefinitions.Charisma, false))
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(FEY_TELEPORT)
                .AddToDB(),
            // fey teleportation wis
            FeatDefinitionBuilder
                .Create("FeatFeyTeleportationWis")
                .SetFeatures(autoPreparedSpells, AttributeModifierCreed_Of_Maraike, learnTirmarian)
                .AddFeatures(MakeSpellFeatureAndInvocations(spells, FEY_TELEPORT, AttributeDefinitions.Wisdom, false))
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(FEY_TELEPORT)
                .AddToDB()
        );

        groups.Add(GroupFeats.MakeGroup("FeatGroupTeleportation", FEY_TELEPORT, groupFeats));
        feats.AddRange(groupFeats);

        #endregion

        #region Celestial Touched

        const string CELESTIAL = "CelestialTouched";

        spells = BuildSpellGroup(0, HealingWord, CureWounds, LesserRestoration);

        autoPreparedSpells = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsFeatCelestialTouched")
            .SetGuiPresentationNoContent(true)
            .AddCustomSubFeatures(ValidateRepertoireForAutoprepared.AnyClassOrSubclass)
            .SetPreparedSpellGroups(spells)
            .SetSpellcastingClass(null)
            .SetAutoTag(CELESTIAL)
            .AddToDB();

        groupFeats.SetRange(
            // celestial touched int
            FeatDefinitionBuilder
                .Create("FeatCelestialTouchedInt")
                .SetFeatures(autoPreparedSpells, AttributeModifierCreed_Of_Pakri)
                .AddFeatures(MakeSpellFeatureAndInvocations(spells, CELESTIAL, AttributeDefinitions.Intelligence))
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(CELESTIAL)
                .AddToDB(),
            // celestial touched wis
            FeatDefinitionBuilder
                .Create("FeatCelestialTouchedWis")
                .SetFeatures(autoPreparedSpells, AttributeModifierCreed_Of_Maraike)
                .AddFeatures(MakeSpellFeatureAndInvocations(spells, CELESTIAL, AttributeDefinitions.Wisdom))
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(CELESTIAL)
                .AddToDB(),
            // celestial touched cha
            FeatDefinitionBuilder
                .Create("FeatCelestialTouchedCha")
                .SetFeatures(autoPreparedSpells, AttributeModifierCreed_Of_Solasta)
                .AddFeatures(MakeSpellFeatureAndInvocations(spells, CELESTIAL, AttributeDefinitions.Charisma))
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(CELESTIAL)
                .AddToDB()
        );

        groups.Add(GroupFeats.MakeGroup("FeatGroupCelestialTouched", CELESTIAL, groupFeats));
        feats.AddRange(groupFeats);

        #endregion

        #region Flame Touched

        const string FLAME_TOUCHED = "FlameTouched";

        spells = BuildSpellGroup(0, BurningHands, HellishRebuke, ScorchingRay);

        autoPreparedSpells = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsFeatFlameTouched")
            .SetGuiPresentationNoContent(true)
            .AddCustomSubFeatures(ValidateRepertoireForAutoprepared.AnyClassOrSubclass)
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

        #region Shadow Touched

        const string SHADOW = "ShadowTouched";

        spells = BuildSpellGroup(0, Invisibility, FalseLife, InflictWounds);

        autoPreparedSpells = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsFeatShadowTouched")
            .SetGuiPresentationNoContent(true)
            .AddCustomSubFeatures(ValidateRepertoireForAutoprepared.AnyClassOrSubclass)
            .SetPreparedSpellGroups(spells)
            .SetSpellcastingClass(null)
            .SetAutoTag(SHADOW)
            .AddToDB();

        groupFeats.SetRange(
            // shadow touched int
            FeatDefinitionBuilder
                .Create("FeatShadowTouchedInt")
                .SetFeatures(autoPreparedSpells, AttributeModifierCreed_Of_Pakri)
                .AddFeatures(MakeSpellFeatureAndInvocations(spells, SHADOW, AttributeDefinitions.Intelligence))
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(SHADOW)
                .AddToDB(),
            // shadow touched wis
            FeatDefinitionBuilder
                .Create("FeatShadowTouchedWis")
                .SetFeatures(autoPreparedSpells, AttributeModifierCreed_Of_Maraike)
                .AddFeatures(MakeSpellFeatureAndInvocations(spells, SHADOW, AttributeDefinitions.Wisdom))
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(SHADOW)
                .AddToDB(),
            // shadow touched cha
            FeatDefinitionBuilder
                .Create("FeatShadowTouchedCha")
                .SetFeatures(autoPreparedSpells, AttributeModifierCreed_Of_Solasta)
                .AddFeatures(MakeSpellFeatureAndInvocations(spells, SHADOW, AttributeDefinitions.Charisma))
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(SHADOW)
                .AddToDB()
        );

        groups.Add(GroupFeats.MakeGroup("FeatGroupShadowTouched", SHADOW, groupFeats));
        feats.AddRange(groupFeats);

        #endregion

        #region Verdant Touched

        const string VERDANT = "VerdantTouched";

        spells = BuildSpellGroup(0, Barkskin, Entangle, Goodberry);

        autoPreparedSpells = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsFeatVerdantTouched")
            .SetGuiPresentationNoContent(true)
            .AddCustomSubFeatures(ValidateRepertoireForAutoprepared.AnyClassOrSubclass)
            .SetPreparedSpellGroups(spells)
            .SetSpellcastingClass(null)
            .SetAutoTag(VERDANT)
            .AddToDB();

        groupFeats.SetRange(
            // verdant touched int
            FeatDefinitionBuilder
                .Create("FeatVerdantTouchedInt")
                .SetFeatures(autoPreparedSpells, AttributeModifierCreed_Of_Pakri)
                .AddFeatures(MakeSpellFeatureAndInvocations(spells, VERDANT, AttributeDefinitions.Intelligence))
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(VERDANT)
                .AddToDB(),
            // verdant touched wis
            FeatDefinitionBuilder
                .Create("FeatVerdantTouchedWis")
                .SetFeatures(autoPreparedSpells, AttributeModifierCreed_Of_Maraike)
                .AddFeatures(MakeSpellFeatureAndInvocations(spells, VERDANT, AttributeDefinitions.Wisdom))
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(VERDANT)
                .AddToDB(),
            // verdant touched cha
            FeatDefinitionBuilder
                .Create("FeatVerdantTouchedCha")
                .SetFeatures(autoPreparedSpells, AttributeModifierCreed_Of_Solasta)
                .AddFeatures(MakeSpellFeatureAndInvocations(spells, VERDANT, AttributeDefinitions.Charisma))
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(VERDANT)
                .AddToDB()
        );

        groups.Add(GroupFeats.MakeGroup("FeatGroupVerdantTouched", VERDANT, groupFeats));
        feats.AddRange(groupFeats);

        #endregion

        #region Iridescent Touched

        const string IRIDESCENT = "IridescentTouched";

        spells = BuildSpellGroup(0, ColorSpray, FaerieFire, SpellsContext.ColorBurst);

        autoPreparedSpells = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsFeatIridescentTouched")
            .SetGuiPresentationNoContent(true)
            .AddCustomSubFeatures(ValidateRepertoireForAutoprepared.AnyClassOrSubclass)
            .SetPreparedSpellGroups(spells)
            .SetSpellcastingClass(null)
            .SetAutoTag(IRIDESCENT)
            .AddToDB();

        groupFeats.SetRange(
            // iridescent touched int
            FeatDefinitionBuilder
                .Create("FeatIridescentTouchedInt")
                .SetFeatures(autoPreparedSpells, AttributeModifierCreed_Of_Pakri)
                .AddFeatures(MakeSpellFeatureAndInvocations(spells, IRIDESCENT, AttributeDefinitions.Intelligence))
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(IRIDESCENT)
                .AddToDB(),
            // iridescent touched wis
            FeatDefinitionBuilder
                .Create("FeatIridescentTouchedWis")
                .SetFeatures(autoPreparedSpells, AttributeModifierCreed_Of_Maraike)
                .AddFeatures(MakeSpellFeatureAndInvocations(spells, IRIDESCENT, AttributeDefinitions.Wisdom))
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(IRIDESCENT)
                .AddToDB(),
            // iridescent touched cha
            FeatDefinitionBuilder
                .Create("FeatIridescentTouchedCha")
                .SetFeatures(autoPreparedSpells, AttributeModifierCreed_Of_Solasta)
                .AddFeatures(MakeSpellFeatureAndInvocations(spells, IRIDESCENT, AttributeDefinitions.Charisma))
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(IRIDESCENT)
                .AddToDB()
        );

        groups.Add(GroupFeats.MakeGroup("FeatGroupIridescentTouched", IRIDESCENT, groupFeats));
        feats.AddRange(groupFeats);

        #endregion

        #region Aegis Touched

        const string AEGIS = "AegisTouched";

        spells = BuildSpellGroup(0, ShieldOfFaith, ProtectionFromEvilGood, ProtectionFromPoison);

        autoPreparedSpells = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsFeatAegisTouched")
            .SetGuiPresentationNoContent(true)
            .AddCustomSubFeatures(ValidateRepertoireForAutoprepared.AnyClassOrSubclass)
            .SetPreparedSpellGroups(spells)
            .SetSpellcastingClass(null)
            .SetAutoTag(AEGIS)
            .AddToDB();

        groupFeats.SetRange(
            // aegis touched int
            FeatDefinitionBuilder
                .Create("FeatAegisTouchedInt")
                .SetFeatures(autoPreparedSpells, AttributeModifierCreed_Of_Pakri)
                .AddFeatures(MakeSpellFeatureAndInvocations(spells, AEGIS, AttributeDefinitions.Intelligence))
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(AEGIS)
                .AddToDB(),
            // aegis touched wis
            FeatDefinitionBuilder
                .Create("FeatAegisTouchedWis")
                .SetFeatures(autoPreparedSpells, AttributeModifierCreed_Of_Maraike)
                .AddFeatures(MakeSpellFeatureAndInvocations(spells, AEGIS, AttributeDefinitions.Wisdom))
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(AEGIS)
                .AddToDB(),
            // aegis touched cha
            FeatDefinitionBuilder
                .Create("FeatAegisTouchedCha")
                .SetFeatures(autoPreparedSpells, AttributeModifierCreed_Of_Solasta)
                .AddFeatures(MakeSpellFeatureAndInvocations(spells, AEGIS, AttributeDefinitions.Charisma))
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(AEGIS)
                .AddToDB()
        );

        groups.Add(GroupFeats.MakeGroup("FeatGroupAegisTouched", AEGIS, groupFeats));
        feats.AddRange(groupFeats);

        #endregion

        #region Peregrination Touched

        const string PEREGRINATION = "PeregrinationTouched";

        spells = BuildSpellGroup(0, Longstrider, ExpeditiousRetreat, SpiderClimb);

        autoPreparedSpells = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsFeatPeregrinationTouched")
            .SetGuiPresentationNoContent(true)
            .AddCustomSubFeatures(ValidateRepertoireForAutoprepared.AnyClassOrSubclass)
            .SetPreparedSpellGroups(spells)
            .SetSpellcastingClass(null)
            .SetAutoTag(PEREGRINATION)
            .AddToDB();

        groupFeats.SetRange(
            // peregrination touched int
            FeatDefinitionBuilder
                .Create("FeatPeregrinationTouchedInt")
                .SetFeatures(autoPreparedSpells, AttributeModifierCreed_Of_Pakri)
                .AddFeatures(MakeSpellFeatureAndInvocations(spells, PEREGRINATION, AttributeDefinitions.Intelligence))
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(PEREGRINATION)
                .AddToDB(),
            // peregrination touched wis
            FeatDefinitionBuilder
                .Create("FeatPeregrinationTouchedWis")
                .SetFeatures(autoPreparedSpells, AttributeModifierCreed_Of_Maraike)
                .AddFeatures(MakeSpellFeatureAndInvocations(spells, PEREGRINATION, AttributeDefinitions.Wisdom))
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(PEREGRINATION)
                .AddToDB(),
            // peregrination touched cha
            FeatDefinitionBuilder
                .Create("FeatPeregrinationTouchedCha")
                .SetFeatures(autoPreparedSpells, AttributeModifierCreed_Of_Solasta)
                .AddFeatures(MakeSpellFeatureAndInvocations(spells, PEREGRINATION, AttributeDefinitions.Charisma))
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(PEREGRINATION)
                .AddToDB()
        );

        groups.Add(GroupFeats.MakeGroup("FeatGroupPeregrinationTouched", PEREGRINATION, groupFeats));
        feats.AddRange(groupFeats);

        #endregion

        #region Retinue Touched

        const string RETINUE = "RetinueTouched";

        spells = BuildSpellGroup(0, Bless, Heroism, EnhanceAbility);

        autoPreparedSpells = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsFeatRetinueTouched")
            .SetGuiPresentationNoContent(true)
            .AddCustomSubFeatures(ValidateRepertoireForAutoprepared.AnyClassOrSubclass)
            .SetPreparedSpellGroups(spells)
            .SetSpellcastingClass(null)
            .SetAutoTag(RETINUE)
            .AddToDB();

        groupFeats.SetRange(
            // peregrination touched int
            FeatDefinitionBuilder
                .Create("FeatRetinueTouchedInt")
                .SetFeatures(autoPreparedSpells, AttributeModifierCreed_Of_Pakri)
                .AddFeatures(MakeSpellFeatureAndInvocations(spells, RETINUE, AttributeDefinitions.Intelligence))
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(RETINUE)
                .AddToDB(),
            // peregrination touched wis
            FeatDefinitionBuilder
                .Create("FeatRetinueTouchedWis")
                .SetFeatures(autoPreparedSpells, AttributeModifierCreed_Of_Maraike)
                .AddFeatures(MakeSpellFeatureAndInvocations(spells, RETINUE, AttributeDefinitions.Wisdom))
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(RETINUE)
                .AddToDB(),
            // peregrination touched cha
            FeatDefinitionBuilder
                .Create("FeatRetinueTouchedCha")
                .SetFeatures(autoPreparedSpells, AttributeModifierCreed_Of_Solasta)
                .AddFeatures(MakeSpellFeatureAndInvocations(spells, RETINUE, AttributeDefinitions.Charisma))
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(RETINUE)
                .AddToDB()
        );

        groups.Add(GroupFeats.MakeGroup("FeatGroupRetinueTouched", RETINUE, groupFeats));
        feats.AddRange(groupFeats);

        #endregion

        GroupFeats.MakeGroup("FeatGroupPlaneTouchedMagic", null, groups);
    }

    [NotNull]
    private static FeatureDefinition[] MakeSpellFeatureAndInvocations(
        FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup spellGroup,
        string name,
        string castingAttribute, bool longRest = true)
    {
        var featureName = $"CastSpell{name}{castingAttribute}";
        var spellFeature = FeatureDefinitionCastSpellBuilder
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

        foreach (var spell in spellGroup.SpellsList
                     .Where(x => x.castingTime is not ActivationTime.Reaction))
        {
            var invocation = CustomInvocationDefinitionBuilder
                .Create($"CustomInvocation{name}{spell.Name}{castingAttribute}")
                .SetGuiPresentation(spell.GuiPresentation) //TODO: auto-generate based on spell
                .AddCustomSubFeatures(ValidateRepertoireForAutoprepared.HasSpellCastingFeature(featureName))
                .SetPoolType(InvocationPoolTypeCustom.Pools.PlaneMagic)
                .SetGrantedSpell(spell, longRestRecharge: longRest)
                .AddToDB();

            if (!longRest)
            {
                invocation.AddCustomSubFeatures(InvocationShortRestRecharge.Marker);
            }

            invocations.Add(invocation);
        }

        var grant = FeatureDefinitionGrantInvocationsBuilder
            .Create($"GrantInvocations{name}{castingAttribute}")
            .SetGuiPresentationNoContent(true)
            .SetInvocations(invocations)
            .AddToDB();

        return new FeatureDefinition[] { spellFeature, grant };
    }

    [NotNull]
    private static FeatureDefinitionGrantInvocations BuildTelekinesis(
        string savingThrowDifficultyAbility, MotionForm.MotionType motionType)
    {
        const string NAME = "FeatTelekinetic";

        var motionTypeName = motionType.ToString();

        AssetReferenceSprite sprite;

        if (motionTypeName == "DragToOrigin")
        {
            sprite = Sprites.GetSprite(motionTypeName, Resources.TelekinesisPull, 128);

            // keep backward compatibility
            motionTypeName = string.Empty;
        }
        else
        {
            sprite = Sprites.GetSprite(motionTypeName, Resources.TelekinesisPush, 128);
        }

        var power = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}{savingThrowDifficultyAbility}{motionTypeName}")
            .SetGuiPresentation($"{NAME}{motionTypeName}", Category.Feature, sprite)
            .AddCustomSubFeatures(PowerFromInvocation.Marker)
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetSavingThrowData(
                        true,
                        AttributeDefinitions.Strength,
                        true,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                        savingThrowDifficultyAbility)
                    .ExcludeCaster()
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(motionType, 1)
                            .Build())
                    .SetParticleEffectParameters(PowerSpellBladeSpellTyrant)
                    .Build())
            .AddToDB();

        var invocation = CustomInvocationDefinitionBuilder
            .Create($"CustomInvocation{NAME}{savingThrowDifficultyAbility}{motionTypeName}")
            .SetGuiPresentation(power.GuiPresentation)
            .SetPoolType(InvocationPoolTypeCustom.Pools.PlaneMagic)
            .SetGrantedFeature(power)
            .AddToDB();

        return FeatureDefinitionGrantInvocationsBuilder
            .Create($"GrantInvocations{NAME}{savingThrowDifficultyAbility}{motionTypeName}")
            .SetGuiPresentationNoContent(true)
            .SetInvocations(invocation)
            .AddToDB();
    }
}
