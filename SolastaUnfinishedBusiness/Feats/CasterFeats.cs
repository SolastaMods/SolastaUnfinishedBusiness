using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;

namespace SolastaUnfinishedBusiness.Feats;

internal static class CasterFeats
{
    internal static readonly Dictionary<string, SpellDefinition[]> MagicTouchedData = new()
    {
        { "AegisTouched", [ShieldOfFaith, ProtectionFromEvilGood, ProtectionFromPoison] },
        { "CelestialTouched", [HealingWord, CureWounds, LesserRestoration] },
        { "FlameTouched", [BurningHands, HellishRebuke, ScorchingRay] },
        { "IridescentTouched", [ColorSpray, FaerieFire, SpellsContext.ColorBurst] },
        { "PeregrinationTouched", [Longstrider, ExpeditiousRetreat, SpiderClimb] },
        { "RetinueTouched", [Bless, Heroism, EnhanceAbility] },
        { "ShadowTouched", [Invisibility, FalseLife, InflictWounds] },
        { "VerdantTouched", [Barkskin, Entangle, Goodberry] }
    };

    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        var groups = new List<FeatDefinition>();
        var groupFeats = new List<FeatDefinition>();

        #region Telekinetic

        const string TELEKINETIC = "Telekinetic";

        groupFeats.SetRange(FeatDefinitionBuilder
                .Create($"Feat{TELEKINETIC}Int")
                .SetGuiPresentation(Category.Feat)
                .SetFeatures(
                    BuildTelekinesis(AttributeDefinitions.Intelligence, MotionForm.MotionType.DragToOrigin),
                    BuildTelekinesis(AttributeDefinitions.Intelligence, MotionForm.MotionType.PushFromOrigin),
                    AttributeModifierCreed_Of_Pakri)
                .SetFeatFamily(TELEKINETIC)
                .AddToDB(),
            FeatDefinitionBuilder
                .Create($"Feat{TELEKINETIC}Cha")
                .SetGuiPresentation(Category.Feat)
                .SetFeatures(
                    BuildTelekinesis(AttributeDefinitions.Charisma, MotionForm.MotionType.DragToOrigin),
                    BuildTelekinesis(AttributeDefinitions.Charisma, MotionForm.MotionType.PushFromOrigin),
                    AttributeModifierCreed_Of_Solasta)
                .SetFeatFamily(TELEKINETIC)
                .AddToDB(),
            FeatDefinitionBuilder
                .Create($"Feat{TELEKINETIC}Wis")
                .SetGuiPresentation(Category.Feat)
                .SetFeatures(
                    BuildTelekinesis(AttributeDefinitions.Wisdom, MotionForm.MotionType.DragToOrigin),
                    BuildTelekinesis(AttributeDefinitions.Wisdom, MotionForm.MotionType.PushFromOrigin),
                    AttributeModifierCreed_Of_Maraike)
                .SetFeatFamily(TELEKINETIC)
                .AddToDB());

        groups.Add(GroupFeats.MakeGroup($"FeatGroup{TELEKINETIC}", TELEKINETIC, groupFeats));
        feats.AddRange(groupFeats);

        #endregion

        #region Fey Teleportation

        const string FEY_TELEPORT = "FeyTeleport";

        var spells = BuildSpellGroup(0, MistyStep);

        var autoPreparedSpells = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{FEY_TELEPORT}ation")
            .SetGuiPresentationNoContent(true)
            .AddCustomSubFeatures(ValidateRepertoireForAutoprepared.AnyClassOrSubclass)
            .SetPreparedSpellGroups(spells)
            .SetSpellcastingClass(null)
            .SetAutoTag(FEY_TELEPORT)
            .AddToDB();

        var learnTirmarian = FeatureDefinitionProficiencyBuilder
            .Create($"ProficiencyFeat{FEY_TELEPORT}ationTirmarian")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.Language, "Language_Tirmarian")
            .AddToDB();

        groupFeats.SetRange(
            FeatDefinitionBuilder
                .Create($"Feat{FEY_TELEPORT}ationInt")
                .SetFeatures(autoPreparedSpells, AttributeModifierCreed_Of_Pakri, learnTirmarian)
                .AddFeatures(MakeSpellFeatureAndInvocations(spells, FEY_TELEPORT, AttributeDefinitions.Intelligence,
                    false))
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(FEY_TELEPORT)
                .AddToDB(),
            FeatDefinitionBuilder
                .Create($"Feat{FEY_TELEPORT}ationCha")
                .SetFeatures(autoPreparedSpells, AttributeModifierCreed_Of_Solasta, learnTirmarian)
                .AddFeatures(MakeSpellFeatureAndInvocations(spells, FEY_TELEPORT, AttributeDefinitions.Charisma, false))
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(FEY_TELEPORT)
                .AddToDB(),
            FeatDefinitionBuilder
                .Create($"Feat{FEY_TELEPORT}ationWis")
                .SetFeatures(autoPreparedSpells, AttributeModifierCreed_Of_Maraike, learnTirmarian)
                .AddFeatures(MakeSpellFeatureAndInvocations(spells, FEY_TELEPORT, AttributeDefinitions.Wisdom, false))
                .SetGuiPresentation(Category.Feat)
                .SetFeatFamily(FEY_TELEPORT)
                .AddToDB());

        groups.Add(GroupFeats.MakeGroup($"FeatGroup{FEY_TELEPORT}", FEY_TELEPORT, groupFeats));
        feats.AddRange(groupFeats);

        #endregion

        #region Touched Magic

        const string TOUCHED_MAGIC = "TouchedMagic";

        var featGroups = new List<FeatDefinition>();

        foreach (var kvp in MagicTouchedData)
        {
            var tag = kvp.Key;

            spells = BuildSpellGroup(0, kvp.Value);
            autoPreparedSpells = FeatureDefinitionAutoPreparedSpellsBuilder
                .Create($"AutoPreparedSpellsFeat{tag}")
                .SetGuiPresentationNoContent(true)
                .AddCustomSubFeatures(ValidateRepertoireForAutoprepared.AnyClassOrSubclass)
                .SetPreparedSpellGroups(spells)
                .SetSpellcastingClass(null)
                .SetAutoTag(tag)
                .AddToDB();

            groupFeats.SetRange(
                FeatDefinitionBuilder
                    .Create($"Feat{tag}Int")
                    .SetFeatures(autoPreparedSpells, AttributeModifierCreed_Of_Pakri)
                    .AddFeatures(MakeSpellFeatureAndInvocations(spells, tag, AttributeDefinitions.Intelligence))
                    .SetGuiPresentation(Category.Feat)
                    .SetFeatFamily(TOUCHED_MAGIC)
                    .SetMustCastSpellsPrerequisite()
                    .AddToDB(),
                FeatDefinitionBuilder
                    .Create($"Feat{tag}Wis")
                    .SetFeatures(autoPreparedSpells, AttributeModifierCreed_Of_Maraike)
                    .AddFeatures(MakeSpellFeatureAndInvocations(spells, tag, AttributeDefinitions.Wisdom))
                    .SetGuiPresentation(Category.Feat)
                    .SetFeatFamily(TOUCHED_MAGIC)
                    .SetMustCastSpellsPrerequisite()
                    .AddToDB(),
                FeatDefinitionBuilder
                    .Create($"Feat{tag}Cha")
                    .SetFeatures(autoPreparedSpells, AttributeModifierCreed_Of_Solasta)
                    .AddFeatures(MakeSpellFeatureAndInvocations(spells, tag, AttributeDefinitions.Charisma))
                    .SetGuiPresentation(Category.Feat)
                    .SetFeatFamily(TOUCHED_MAGIC)
                    .SetMustCastSpellsPrerequisite()
                    .AddToDB());

            featGroups.Add(GroupFeats.MakeGroup($"FeatGroup{tag}", TOUCHED_MAGIC, groupFeats));
        }

        groups.Add(GroupFeats.MakeGroup("FeatGroupTouchedMagic", TOUCHED_MAGIC, featGroups));

        #endregion

        GroupFeats.MakeGroup("FeatGroupPlaneMagic", null, groups);
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
                .SetGuiPresentation(spell.GuiPresentation)
                .AddCustomSubFeatures(ValidateRepertoireForAutoprepared.HasSpellCastingFeature(featureName))
                .SetPoolType(InvocationPoolTypeCustom.Pools.PlaneMagic)
                .SetGrantedSpell(spell, longRestRecharge: longRest)
                .AddToDB();

            if (!longRest)
            {
                invocation.AddCustomSubFeatures(RechargeInvocationOnShortRest.Marker);
            }

            invocations.Add(invocation);
        }

        var grant = FeatureDefinitionGrantInvocationsBuilder
            .Create($"GrantInvocations{name}{castingAttribute}")
            .SetGuiPresentationNoContent(true)
            .SetInvocations(invocations)
            .AddToDB();

        return [spellFeature, grant];
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

            motionTypeName = string.Empty;
        }
        else
        {
            sprite = Sprites.GetSprite(motionTypeName, Resources.TelekinesisPush, 128);
        }

        var power = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}{savingThrowDifficultyAbility}{motionTypeName}")
            .SetGuiPresentation($"{NAME}{motionTypeName}", Category.Feature, sprite)
            .AddCustomSubFeatures(ModifyPowerFromInvocation.Marker)
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
