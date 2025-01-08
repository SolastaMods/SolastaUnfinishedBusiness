using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static ActionDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionConditionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using Resources = SolastaUnfinishedBusiness.Properties.Resources;

namespace SolastaUnfinishedBusiness.Spells;

internal static partial class SpellBuilders
{
    #region Brain Bulwark

    internal static SpellDefinition BuildBrainBulwark()
    {
        const string NAME = "BrainBulwark";

        var conditionBrainBulwark = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(Category.Condition, ConditionBlessed)
            .SetPossessive()
            .SetFeatures(
                DamageAffinityPsychicResistance,
                ConditionAffinityFrightenedImmunity,
                ConditionAffinityFrightenedFearImmunity,
                ConditionAffinityMindControlledImmunity,
                ConditionAffinityMindDominatedImmunity,
                ConditionAffinityDemonicInfluenceImmunity,
                FeatureDefinitionConditionAffinityBuilder
                    .Create("ConditionAffinityInsaneImmunity")
                    .SetGuiPresentation($"Condition{NAME}", Category.Condition, Gui.NoLocalization)
                    .SetConditionAffinityType(ConditionAffinityType.Immunity)
                    .SetConditionType(ConditionInsane)
                    .AddToDB())
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.BrainBulwark, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolAbjuration)
            .SetSpellLevel(4)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(false)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Defense)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Hour, 1)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel,
                        additionalTargetsPerIncrement: 1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionBrainBulwark, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .SetParticleEffectParameters(DispelMagic)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Faithful Hound

    internal static SpellDefinition BuildFaithfulHound()
    {
        const string NAME = "FaithfulHound";

        var sprite = Sprites.GetSprite(NAME, Resources.FaithfulHound, 128);

        var proxyFaithfulHound = EffectProxyDefinitionBuilder
            .Create(EffectProxyDefinitions.ProxyArcaneSword, $"Proxy{NAME}")
            .SetGuiPresentation(Category.Proxy, Gui.NoLocalization, sprite)
            .SetPortrait(sprite)
            .SetActionId(ExtraActionId.ProxyHoundWeapon)
            .SetAttackMethod(ProxyAttackMethod.CasterSpellAbility, DamageTypePiercing, DieType.D8, 4)
            .SetAdditionalFeatures(FeatureDefinitionSenses.SenseDarkvision, FeatureDefinitionSenses.SenseTruesight16)
            .SetCanMove(false, false)
            .AddToDB();

        proxyFaithfulHound.attackParticle = new AssetReference();
        proxyFaithfulHound.prefabReference = MonsterDefinitions.FeyWolf.MonsterPresentation.malePrefabReference;

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, sprite)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
            .SetSpellLevel(4)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Hour, 8)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.Position)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetSummonEffectProxyForm(proxyFaithfulHound)
                            .Build())
                    .SetParticleEffectParameters(DispelMagic)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Gravity Sinkhole

    internal static SpellDefinition BuildGravitySinkhole()
    {
        const string NAME = "GravitySinkhole";

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.GravitySinkhole, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(4)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Debuff)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Distance, 24, TargetType.Sphere, 4)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                    .SetSavingThrowData(
                        false,
                        AttributeDefinitions.Constitution,
                        true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.DragToOrigin, 3)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeForce, 5, DieType.D10)
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .Build())
                    .SetParticleEffectParameters(Shatter)
                    .Build())
            .AddCustomSubFeatures(ForcePushOrDragFromEffectPoint.Marker)
            .AddToDB();

        return spell;
    }

    #endregion

    #region Psychic Lance

    internal static SpellDefinition BuildPsychicLance()
    {
        const string NAME = "PsychicLance";

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.PsychicLance, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEnchantment)
            .SetSpellLevel(4)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(false)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Defense)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 24, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                    .SetSavingThrowData(false, AttributeDefinitions.Intelligence, true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypePsychic, 7, DieType.D6)
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(ConditionDefinitions.ConditionIncapacitated,
                                ConditionForm.ConditionOperation.Add)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .SetParticleEffectParameters(PowerWordStun)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Staggering Smite

    internal static SpellDefinition BuildStaggeringSmite()
    {
        const string NAME = "StaggeringSmite";

        var conditionStaggeringSmiteEnemy = ConditionDefinitionBuilder
            .Create($"Condition{NAME}Enemy")
            .SetGuiPresentation(Category.Condition, ConditionDazzled)
            .SetSpecialDuration(DurationType.Round, 1)
            .AddFeatures(
                FeatureDefinitionCombatAffinityBuilder
                    .Create($"CombatAffinity{NAME}")
                    .SetGuiPresentation(NAME, Category.Spell, Gui.NoLocalization)
                    .SetMyAttackAdvantage(AdvantageType.Disadvantage)
                    .AddToDB(),
                FeatureDefinitionAbilityCheckAffinityBuilder
                    .Create($"AbilityCheckAffinity{NAME}")
                    .SetGuiPresentation(NAME, Category.Spell, Gui.NoLocalization)
                    .BuildAndSetAffinityGroups(
                        CharacterAbilityCheckAffinity.Disadvantage, AttributeDefinitions.AbilityScoreNames)
                    .AddToDB(),
                FeatureDefinitionActionAffinityBuilder
                    .Create($"ActionAffinity{NAME}")
                    .SetGuiPresentationNoContent(true)
                    .SetAllowedActionTypes(reaction: false)
                    .AddToDB())
            .AddToDB();

        var additionalDamageStaggeringSmite = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{NAME}")
            .SetGuiPresentation(NAME, Category.Spell)
            .SetNotificationTag(NAME)
            .SetAttackModeOnly()
            .SetRequiredProperty(RestrictedContextRequiredProperty.MeleeWeapon)
            .SetDamageDice(DieType.D6, 4)
            .SetSpecificDamageType(DamageTypePsychic)
            .SetSavingThrowData(
                EffectDifficultyClassComputation.SpellCastingFeature,
                EffectSavingThrowType.None,
                AttributeDefinitions.Wisdom)
            .AddConditionOperation(
                new ConditionOperationDescription
                {
                    operation = ConditionOperationDescription.ConditionOperation.Add,
                    conditionDefinition = conditionStaggeringSmiteEnemy,
                    hasSavingThrow = true,
                    saveAffinity = EffectSavingThrowType.Negates
                })
            .SetImpactParticleReference(Maze.EffectDescription.EffectParticleParameters.effectParticleReference)
            .AddToDB();

        var conditionStaggeringSmite = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(NAME, Category.Spell, ConditionBrandingSmite)
            .SetPossessive()
            .SetFeatures(additionalDamageStaggeringSmite)
            .SetSpecialInterruptions(ExtraConditionInterruption.AttacksWithMeleeAndDamages)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(BrandingSmite, NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.StaggeringSmite, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(4)
            .SetCastingTime(ActivationTime.BonusAction)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(false)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    // .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionStaggeringSmite))
                    .SetParticleEffectParameters(Maze)
                    .SetEffectEffectParameters(new AssetReference())
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Psionic Blast

    internal static SpellDefinition BuildPsionicBlast()
    {
        const string NAME = "PsionicBlast";

        var conditionDazed = ConditionDefinitionBuilder
            .Create(ConditionDazzled, $"Condition{NAME}")
            .SetOrUpdateGuiPresentation(NAME, Category.Spell)
            .SetPossessive()
            .SetParentCondition(ConditionDazzled)
            .SetFeatures(
                FeatureDefinitionMovementAffinityBuilder
                    .Create($"MovementAffinity{NAME}")
                    .SetGuiPresentationNoContent(true)
                    .SetBaseSpeedMultiplicativeModifier(0.5f)
                    .AddToDB())
            .AddToDB();

        conditionDazed.GuiPresentation.description = Gui.EmptyContent;

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.PsionicBlast, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(4)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(false)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.All, RangeType.Self, 6, TargetType.Cone, 6)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                    .SetSavingThrowData(false, AttributeDefinitions.Intelligence, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .ExcludeCaster()
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypePsychic, 5, DieType.D8)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(conditionDazed, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .SetParticleEffectParameters(Fear)
                    .SetCasterEffectParameters(ViciousMockery)
                    .SetImpactEffectParameters(PowerMagebaneWarcry)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Sickening Radiance

    internal static SpellDefinition BuildSickeningRadiance()
    {
        const string NAME = "SickeningRadiance";

        var proxy = EffectProxyDefinitionBuilder
            .Create(EffectProxyDefinitions.ProxyIndomitableLight, $"Proxy{NAME}")
            .SetGuiPresentation(NAME, Category.Spell, Gui.NoLocalization)
            .AddToDB();

        proxy.addLightSource = true;
        proxy.lightSourceForm.brightRange = 0;
        proxy.lightSourceForm.dimAdditionalRange = 6;
        proxy.lightSourceForm.color = new Color(0.5f, 0.7f, 0.3f, 1.0f);

        var conditionExhausted1 = ConditionDefinitionBuilder
            .Create($"Condition{NAME}Exhausted1")
            .SetGuiPresentation(Category.Condition, ConditionLethargic)
            .SetConditionType(ConditionType.Detrimental)
            .SetPossessive()
            .SetSilent(Silent.WhenRemoved)
            .SetSpecialDuration(DurationType.Minute, 10)
            .SetFeatures(FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionExhausted)
            .SetConditionParticleReference(ConditionHopeless)
            .AddToDB();

        var conditionExhausted2 = ConditionDefinitionBuilder
            .Create($"Condition{NAME}Exhausted2")
            .SetGuiPresentation(Category.Condition, ConditionLethargic)
            .SetConditionType(ConditionType.Detrimental)
            .SetPossessive()
            .SetSilent(Silent.WhenRemoved)
            .SetSpecialDuration(DurationType.Minute, 10)
            .SetFeatures(
                FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionExhausted,
                FeatureDefinitionMovementAffinitys.MovementAffinityConditionSlowed)
            .SetConditionParticleReference(ConditionHopeless)
            .AddToDB();

        var combatAffinityExhausted3 = FeatureDefinitionCombatAffinityBuilder
            .Create($"CombatAffinity{NAME}Exhausted3")
            .SetGuiPresentation($"Condition{NAME}Exhausted3", Category.Condition, Gui.NoLocalization)
            .SetMyAttackAdvantage(AdvantageType.Disadvantage)
            .AddToDB();

        var savingThrowAffinityExhausted3 = FeatureDefinitionSavingThrowAffinityBuilder
            .Create($"SavingThrowAffinity{NAME}Exhausted3")
            .SetGuiPresentation($"Condition{NAME}Exhausted3", Category.Condition, Gui.NoLocalization)
            .SetAffinities(CharacterSavingThrowAffinity.Disadvantage, false, AttributeDefinitions.AbilityScoreNames)
            .AddToDB();

        var conditionExhausted3 = ConditionDefinitionBuilder
            .Create($"Condition{NAME}Exhausted3")
            .SetGuiPresentation(Category.Condition, ConditionLethargic)
            .SetConditionType(ConditionType.Detrimental)
            .SetPossessive()
            .SetSilent(Silent.WhenRemoved)
            .SetSpecialDuration(DurationType.Minute, 10)
            .SetFeatures(
                FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionExhausted,
                FeatureDefinitionMovementAffinitys.MovementAffinityConditionSlowed,
                combatAffinityExhausted3,
                savingThrowAffinityExhausted3)
            .SetConditionParticleReference(ConditionHopeless)
            .AddToDB();

        var attributeModifierExhausted4 = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{NAME}Exhausted4")
            .SetGuiPresentationNoContent(true)
            .SetAddConditionAmount(AttributeDefinitions.HitPoints)
            .AddToDB();

        var conditionExhausted4 = ConditionDefinitionBuilder
            .Create($"Condition{NAME}Exhausted4")
            .SetGuiPresentation(Category.Condition, ConditionLethargic)
            .SetConditionType(ConditionType.Detrimental)
            .SetPossessive()
            .SetSilent(Silent.WhenRemoved)
            .SetSpecialDuration(DurationType.Minute, 10)
            .SetFixedAmount(1)
            .SetFeatures(
                FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionExhausted,
                FeatureDefinitionMovementAffinitys.MovementAffinityConditionSlowed,
                combatAffinityExhausted3,
                savingThrowAffinityExhausted3,
                attributeModifierExhausted4)
            .SetConditionParticleReference(ConditionHopeless)
            .AddToDB();

        var conditionExhausted5 = ConditionDefinitionBuilder
            .Create($"Condition{NAME}Exhausted5")
            .SetGuiPresentation(Category.Condition, ConditionLethargic)
            .SetConditionType(ConditionType.Detrimental)
            .SetPossessive()
            .SetSilent(Silent.WhenRemoved)
            .SetSpecialDuration(DurationType.Minute, 10)
            .SetFixedAmount(1)
            .SetFeatures(
                FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionExhausted,
                FeatureDefinitionMovementAffinitys.MovementAffinityConditionRestrained,
                combatAffinityExhausted3,
                savingThrowAffinityExhausted3,
                attributeModifierExhausted4)
            .SetConditionParticleReference(ConditionHopeless)
            .AddToDB();

        ConditionDefinition[] conditionsExhausted =
        [
            conditionExhausted1,
            conditionExhausted2,
            conditionExhausted3,
            conditionExhausted4,
            conditionExhausted5
        ];

        var conditionSickenedRadiance = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(Category.Condition, ConditionBaned)
            .SetConditionType(ConditionType.Detrimental)
            .SetPossessive()
            .SetSpecialDuration(DurationType.Minute, 10)
            .SetFeatures(
                FeatureDefinitionConditionAffinityBuilder
                    .Create($"ConditionAffinity{NAME}")
                    .SetGuiPresentation($"Condition{NAME}", Category.Condition, Gui.NoLocalization)
                    .SetConditionType(ConditionInvisibleBase)
                    .SetConditionAffinityType(ConditionAffinityType.Immunity)
                    .AddToDB())
            .AddCustomSubFeatures(new OnConditionAddedOrRemovedSickeningRadiance(conditionsExhausted))
            .AddToDB();

        conditionSickenedRadiance.silentWhenRefreshed = true;

        var conditionMark = ConditionDefinitionBuilder
            .Create($"Condition{NAME}Mark")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddCustomSubFeatures(
                new OnConditionAddedOrRemovedSickeningRadianceSelf(conditionSickenedRadiance, conditionsExhausted))
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.SickeningRadiance, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(4)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 10)
                    .SetTargetingData(Side.All, RangeType.Distance, 24, TargetType.Sphere, 6)
                    .SetSavingThrowData(
                        false,
                        AttributeDefinitions.Constitution,
                        true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetRecurrentEffect(RecurrentEffect.OnEnter | RecurrentEffect.OnTurnStart)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetDamageForm(DamageTypeRadiant, 4, DieType.D10)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(conditionSickenedRadiance, ConditionForm.ConditionOperation.Add)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetSummonEffectProxyForm(proxy)
                            .Build(),
                        EffectFormBuilder.ConditionForm(conditionMark, ConditionForm.ConditionOperation.Add, true,
                            true))
                    .SetCasterEffectParameters(GuardianOfFaith)
                    .SetImpactEffectParameters(PowerSymbolOfHopelessness)
                    .Build())
            .AddToDB();

        return spell;
    }

    private sealed class OnConditionAddedOrRemovedSickeningRadianceSelf(
        ConditionDefinition conditionSickenedRadiance,
        params ConditionDefinition[] conditionsExhausted) : IOnConditionAddedOrRemoved
    {
        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // empty
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            var locationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
            var contenders =
                Gui.Battle?.AllContenders ??
                locationCharacterService.PartyCharacters.Union(locationCharacterService.GuestCharacters);

            foreach (var contender in contenders)
            {
                var rulesetContender = contender.RulesetActor;

                if (contender.RulesetCharacter.TryGetConditionOfCategoryAndType(
                        AttributeDefinitions.TagEffect, conditionSickenedRadiance.Name, out var activeCondition) &&
                    activeCondition.SourceGuid == rulesetCondition.SourceGuid)
                {
                    rulesetContender.RemoveCondition(activeCondition);
                }

                foreach (var conditionExhausted in conditionsExhausted)
                {
                    if (contender.RulesetCharacter.TryGetConditionOfCategoryAndType(
                            AttributeDefinitions.TagEffect, conditionExhausted.Name, out var exhaustedCondition) &&
                        exhaustedCondition.SourceGuid == rulesetCondition.SourceGuid)
                    {
                        rulesetContender.RemoveCondition(exhaustedCondition);
                    }
                }
            }
        }
    }

    private sealed class OnConditionAddedOrRemovedSickeningRadiance(params ConditionDefinition[] conditionsExhausted)
        : IOnConditionAddedOrRemoved
    {
        public void OnConditionAdded(RulesetCharacter rulesetTarget, RulesetCondition rulesetCondition)
        {
            var rulesetCaster = EffectHelpers.GetCharacterByGuid(rulesetCondition.SourceGuid);

            if (rulesetCaster?.ConcentratedSpell == null)
            {
                return;
            }

            rulesetCondition.RemainingRounds = rulesetCaster.ConcentratedSpell.RemainingRounds;

            HandleExhaustedCondition(rulesetCaster, rulesetTarget, rulesetCondition.RemainingRounds);

            var effectGuid = rulesetCaster.ConcentratedSpell.Guid;

            if (rulesetTarget is not (RulesetCharacterHero or RulesetCharacterMonster) ||
                rulesetTarget.PersonalLightSource?.EffectGuid == effectGuid)
            {
                return;
            }

            var visibilityService = ServiceRepository.GetService<IGameLocationVisibilityService>();
            var lightSourceForm = FaerieFire.EffectDescription
                .GetFirstFormOfType(EffectForm.EffectFormType.LightSource).LightSourceForm;

            var rulesetLightSource = new RulesetLightSource(new Color(0, 0.6f, 0), 0, 1,
                lightSourceForm.GraphicsPrefabAssetGUID,
                LightSourceType.Basic,
                rulesetCondition.effectDefinitionName,
                rulesetTarget.Guid,
                effectGuid: effectGuid);

            var target = GameLocationCharacter.GetFromActor(rulesetTarget);

            visibilityService.AddCharacterLightSource(target, rulesetLightSource);
        }

        public void OnConditionRemoved(RulesetCharacter rulesetTarget, RulesetCondition rulesetCondition)
        {
            var rulesetCaster = EffectHelpers.GetCharacterByGuid(rulesetCondition.SourceGuid);

            if (rulesetCaster?.ConcentratedSpell == null)
            {
                return;
            }

            var effectGuid = rulesetCaster.ConcentratedSpell.Guid;

            if (rulesetTarget is not (RulesetCharacterHero or RulesetCharacterMonster) ||
                rulesetTarget.PersonalLightSource?.EffectGuid != effectGuid)
            {
                return;
            }

            var visibilityService = ServiceRepository.GetService<IGameLocationVisibilityService>();
            var target = GameLocationCharacter.GetFromActor(rulesetTarget);

            visibilityService.RemoveCharacterLightSource(target, rulesetTarget.PersonalLightSource);
        }

        private void HandleExhaustedCondition(
            RulesetCharacter rulesetCaster, RulesetCharacter rulesetTarget, int remainingRounds)
        {
            var amount = 0;
            var conditionName = string.Empty;

            if (rulesetTarget.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionsExhausted[4].Name, out var exhaustedCondition5) &&
                exhaustedCondition5.SourceGuid == rulesetCaster.Guid)
            {
                rulesetTarget.SustainDamage(1000, "DamagePure", false, rulesetCaster.Guid, null, out _);
            }
            else if (rulesetTarget.TryGetConditionOfCategoryAndType(
                         AttributeDefinitions.TagEffect, conditionsExhausted[3].Name, out var exhaustedCondition4) &&
                     exhaustedCondition4.SourceGuid == rulesetCaster.Guid)
            {
                conditionName = conditionsExhausted[4].Name;

                rulesetTarget.RemoveCondition(exhaustedCondition4);
            }
            else if (rulesetTarget.TryGetConditionOfCategoryAndType(
                         AttributeDefinitions.TagEffect, conditionsExhausted[2].Name, out var exhaustedCondition3) &&
                     exhaustedCondition3.SourceGuid == rulesetCaster.Guid)
            {
                amount = -rulesetTarget.GetAttribute(AttributeDefinitions.HitPoints).BaseValue / 2;
                conditionName = conditionsExhausted[3].Name;

                rulesetTarget.RemoveCondition(exhaustedCondition3);
            }
            else if (rulesetTarget.TryGetConditionOfCategoryAndType(
                         AttributeDefinitions.TagEffect, conditionsExhausted[1].Name, out var exhaustedCondition2) &&
                     exhaustedCondition2.SourceGuid == rulesetCaster.Guid)
            {
                conditionName = conditionsExhausted[2].Name;

                rulesetTarget.RemoveCondition(exhaustedCondition2);
            }
            else if (rulesetTarget.TryGetConditionOfCategoryAndType(
                         AttributeDefinitions.TagEffect, conditionsExhausted[0].Name, out var exhaustedCondition1) &&
                     exhaustedCondition1.SourceGuid == rulesetCaster.Guid)
            {
                conditionName = conditionsExhausted[1].Name;

                rulesetTarget.RemoveCondition(exhaustedCondition1);
            }
            else
            {
                conditionName = conditionsExhausted[0].Name;
            }

            if (rulesetTarget is RulesetCharacterMonster rulesetMonster &&
                (rulesetMonster.MonsterDefinition == MonsterDefinitions.ShamblingMound_MonsterDefinition ||
                 rulesetMonster.MonsterDefinition == MonsterDefinitions.ShamblingMound_MonsterDefinition_POI_ONLY ||
                 rulesetMonster.MonsterDefinition.CharacterFamily == CharacterFamilyDefinitions.Construct.Name ||
                 rulesetMonster.MonsterDefinition.CharacterFamily == CharacterFamilyDefinitions.Elemental.Name ||
                 rulesetMonster.MonsterDefinition.CharacterFamily == CharacterFamilyDefinitions.Undead.Name))
            {
                return;
            }

            if (conditionName != string.Empty)
            {
                rulesetTarget.InflictCondition(
                    conditionName,
                    DurationType.Round,
                    remainingRounds,
                    TurnOccurenceType.EndOfTurn,
                    AttributeDefinitions.TagEffect,
                    rulesetCaster.guid,
                    rulesetCaster.CurrentFaction.Name,
                    1,
                    conditionName,
                    amount,
                    0,
                    0);
            }
        }
    }

    #endregion

    #region Vitriolic Sphere

    internal static SpellDefinition BuildVitriolicSphere()
    {
        const string NAME = "VitriolicSphere";

        var power = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}")
            .SetGuiPresentation(NAME, Category.Spell, hidden: true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(EffectFormBuilder.DamageForm(DamageTypeAcid, 5, DieType.D4))
                    .SetImpactEffectParameters(VenomousSpike)
                    .Build())
            .AddToDB();

        var conditionVitriolicSphere = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(Category.Condition, ConditionAcidArrowed)
            .SetConditionType(ConditionType.Detrimental)
            // don't know why but setting end of turn on spell make it finish at end of source turn instead
            .SetSpecialDuration()
            .AddCustomSubFeatures(new OnConditionAddedOrRemovedVitriolicSphere(power))
            .SetConditionParticleReference(ConditionOnAcidPilgrim)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.VitriolicSphere, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(4)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Distance, 24, TargetType.Sphere, 4)
                    .SetSavingThrowData(false, AttributeDefinitions.Dexterity, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 2)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypeAcid, 10, DieType.D4)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(conditionVitriolicSphere, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .SetCasterEffectParameters(AcidSplash)
                    .SetImpactEffectParameters(AcidArrow)
                    .Build())
            .AddToDB();

        spell.EffectDescription.EffectParticleParameters.zoneParticleReference =
            Shatter.EffectDescription.EffectParticleParameters.zoneParticleReference;

        return spell;
    }

    private sealed class OnConditionAddedOrRemovedVitriolicSphere(FeatureDefinitionPower power)
        : IOnConditionAddedOrRemoved
    {
        public void OnConditionAdded(RulesetCharacter rulesetCharacter, RulesetCondition rulesetCondition)
        {
            // empty
        }

        public void OnConditionRemoved(RulesetCharacter rulesetCharacter, RulesetCondition rulesetCondition)
        {
            var rulesetCaster = EffectHelpers.GetCharacterByGuid(rulesetCondition.SourceGuid);
            var caster = GameLocationCharacter.GetFromActor(rulesetCaster);
            var character = GameLocationCharacter.GetFromActor(rulesetCharacter);
            var usablePower = PowerProvider.Get(power, rulesetCaster);

            caster.MyExecuteActionSpendPower(usablePower, character);
        }
    }

    #endregion

    #region Aura of Life

    internal static SpellDefinition BuildAuraOfLife()
    {
        // kept name for backward compatibility
        const string NAME = "AuraOfVitality";

        var conditionAffinityLifeDrained = FeatureDefinitionConditionAffinityBuilder
            .Create($"ConditionAffinity{NAME}LifeDrained")
            .SetGuiPresentationNoContent(true)
            .SetConditionAffinityType(ConditionAffinityType.Immunity)
            .SetConditionType(ConditionLifeDrained)
            .AddToDB();

        var conditionAuraOfVitality = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(Category.Condition, ConditionHeroism)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(
                conditionAffinityLifeDrained,
                DamageAffinityNecroticResistance)
            .AddCustomSubFeatures(
                new CharacterBeforeTurnStartListenerAuraOfVitality(),
                // need to keep this condition when hero is downed, so it can stand up on it's next turn
                new ForceConditionCategory(AttributeDefinitions.TagCombat))
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.AuraOfLife, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolAbjuration)
            .SetSpellLevel(4)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(false)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 10)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Sphere, 6)
                    .SetRecurrentEffect(
                        RecurrentEffect.OnActivation | RecurrentEffect.OnEnter | RecurrentEffect.OnTurnStart)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionAuraOfVitality))
                    .SetParticleEffectParameters(DivineWord)
                    .Build())
            .AddToDB();

        return spell;
    }

    private sealed class CharacterBeforeTurnStartListenerAuraOfVitality : ICharacterBeforeTurnStartListener
    {
        public void OnCharacterBeforeTurnStarted(GameLocationCharacter locationCharacter)
        {
            var rulesetCharacter = locationCharacter.RulesetCharacter;

            if (rulesetCharacter.CurrentHitPoints == 0)
            {
                rulesetCharacter.StabilizeAndGainHitPoints(1);
            }
        }
    }

    #endregion

    #region Aura of Perseverance

    internal static SpellDefinition BuildAuraOfPerseverance()
    {
        const string NAME = "AuraOfPerseverance";

        var conditionAffinityDiseased = FeatureDefinitionConditionAffinityBuilder
            .Create($"ConditionAffinity{NAME}Diseased")
            .SetGuiPresentationNoContent(true)
            .SetConditionAffinityType(ConditionAffinityType.Immunity)
            .SetConditionType(ConditionDefinitions.ConditionDiseased)
            .AddToDB();

        var conditionAuraOfPerseverance = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(Category.Condition, ConditionHeroism)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(conditionAffinityDiseased, DamageAffinityPoisonResistance)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.AuraOfPerseverance, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolAbjuration)
            .SetSpellLevel(4)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(false)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 10)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Sphere, 6)
                    .SetRecurrentEffect(
                        RecurrentEffect.OnActivation | RecurrentEffect.OnEnter | RecurrentEffect.OnTurnStart)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionAuraOfPerseverance))
                    .SetParticleEffectParameters(DivineWord)
                    .Build())
            .AddToDB();

        conditionAuraOfPerseverance.AddCustomSubFeatures(new ModifySavingThrowAuraOfPerseverance(spell));

        return spell;
    }

    private sealed class ModifySavingThrowAuraOfPerseverance(SpellDefinition spellDefinition)
        : IRollSavingThrowInitiated
    {
        public void OnSavingThrowInitiated(
            RulesetActor rulesetActorCaster,
            RulesetActor rulesetActorDefender,
            ref int saveBonus,
            ref string abilityScoreName,
            BaseDefinition sourceDefinition,
            List<TrendInfo> modifierTrends,
            List<TrendInfo> advantageTrends,
            ref int rollModifier,
            ref int saveDC,
            ref bool hasHitVisual,
            RollOutcome outcome,
            int outcomeDelta,
            List<EffectForm> effectForms)
        {
            if (effectForms.Any(x =>
                    x.FormType == EffectForm.EffectFormType.Condition
                    && (x.ConditionForm.ConditionDefinition.IsSubtypeOf(ConditionDefinitions.ConditionBlinded.Name)
                        || x.ConditionForm.ConditionDefinition.IsSubtypeOf(ConditionDefinitions.ConditionCharmed.Name)
                        || x.ConditionForm.ConditionDefinition.IsSubtypeOf(ConditionDefinitions.ConditionDeafened.Name)
                        || x.ConditionForm.ConditionDefinition.IsSubtypeOf(
                            ConditionDefinitions.ConditionFrightened.Name)
                        || x.ConditionForm.ConditionDefinition.IsSubtypeOf(ConditionDefinitions.ConditionParalyzed.Name)
                        || x.ConditionForm.ConditionDefinition.IsSubtypeOf(ConditionDefinitions.ConditionPoisoned.Name)
                        || x.ConditionForm.ConditionDefinition.IsSubtypeOf(ConditionDefinitions.ConditionStunned
                            .Name))))
            {
                advantageTrends.Add(
                    new TrendInfo(1, FeatureSourceType.Spell, spellDefinition.Name, spellDefinition));
            }
        }
    }

    #endregion

    #region Blessing of Rime

    internal static SpellDefinition BuildBlessingOfRime()
    {
        const string NAME = "BlessingOfRime";

        var conditionBlessingOfRime = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(Category.Condition, ConditionBlessed)
            .SetPossessive()
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.BlessingOfRime, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(4)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Hour, 1)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.IndividualsUnique, 3)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(conditionBlessingOfRime),
                        EffectFormBuilder
                            .Create()
                            .SetTempHpForm(0, DieType.D8, 3)
                            .Build())
                    .SetParticleEffectParameters(RayOfFrost)
                    .SetEffectEffectParameters(PowerDomainElementalIceLance)
                    .Build())
            .AddToDB();

        conditionBlessingOfRime.AddCustomSubFeatures(new CustomBehaviorBlessingOfRime(spell));

        return spell;
    }

    private sealed class CustomBehaviorBlessingOfRime(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        SpellDefinition spellDefinition)
        : IRollSavingThrowInitiated, IOnConditionAddedOrRemoved, ICharacterTurnStartListener
    {
        // required to ensure the behavior will still work after loading a save
        public void OnCharacterTurnStarted(GameLocationCharacter locationCharacter)
        {
            var rulesetCharacter = locationCharacter.RulesetCharacter;

            rulesetCharacter.DamageReceived -= DamageReceivedHandler;
            rulesetCharacter.DamageReceived += DamageReceivedHandler;
        }

        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            target.DamageReceived += DamageReceivedHandler;
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            target.DamageReceived -= DamageReceivedHandler;
        }

        public void OnSavingThrowInitiated(
            RulesetActor rulesetActorCaster,
            RulesetActor rulesetActorDefender,
            ref int saveBonus,
            ref string abilityScoreName,
            BaseDefinition sourceDefinition,
            List<TrendInfo> modifierTrends,
            List<TrendInfo> advantageTrends,
            ref int rollModifier,
            ref int saveDC,
            ref bool hasHitVisual,
            RollOutcome outcome,
            int outcomeDelta,
            List<EffectForm> effectForms)
        {
            if (abilityScoreName == AttributeDefinitions.Constitution)
            {
                advantageTrends.Add(
                    new TrendInfo(1, FeatureSourceType.Spell, spellDefinition.Name, spellDefinition));
            }
        }

        private static void DamageReceivedHandler(
            RulesetActor target,
            int damage,
            string damageType,
            ulong sourceGuid,
            RollInfo rollInfo)
        {
            if (target is RulesetCharacter rulesetCharacter &&
                rulesetCharacter.TemporaryHitPoints <= damage)
            {
                rulesetCharacter.RemoveAllConditionsOfCategoryAndType(
                    AttributeDefinitions.TagEffect, "ConditionBlessingOfRime");
            }
        }
    }

    #endregion

    #region Elemental Bane

    internal static SpellDefinition BuildElementalBane()
    {
        const string NAME = "ElementalBane";

        var conditionMark = ConditionDefinitionBuilder
            .Create($"Condition{NAME}Mark")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        var sprite = Sprites.GetSprite(NAME, Resources.ElementalBane, 128);
        var subSpells = new List<SpellDefinition>();
        var conditionEffects = new List<BaseDefinition>
        {
            ConditionOnAcidPilgrim,
            PowerDomainElementalHeraldOfTheElementsCold,
            ConditionOnFire,
            ConditionDefinitions.ConditionParalyzed,
            PowerDomainElementalHeraldOfTheElementsThunder
        };
        var current = 0;

        foreach (var (damageType, magicEffect) in DamagesAndEffects)
        {
            if (damageType == DamageTypePoison)
            {
                continue;
            }

            var damageTitle = Gui.Localize($"Tooltip/&Tag{damageType}Title");
            var title = Gui.Format("Condition/&ConditionElementalBaneTitle", damageTitle);
            var description = Gui.Format("Condition/&ConditionElementalBaneDescription", damageTitle);

            var power = FeatureDefinitionPowerBuilder
                .Create($"Power{NAME}{damageType}")
                .SetGuiPresentation(title, Gui.NoLocalization)
                .SetUsesFixed(ActivationTime.NoCost)
                .SetShowCasting(false)
                .SetEffectDescription(
                    EffectDescriptionBuilder
                        .Create()
                        .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                        .SetEffectForms(EffectFormBuilder.DamageForm(damageType, 2, DieType.D6))
                        .SetImpactEffectParameters(magicEffect)
                        .Build())
                .AddToDB();

            power.GuiPresentation.hidden = true;

            var condition = ConditionDefinitionBuilder
                .Create($"Condition{NAME}{damageType}")
                .SetGuiPresentation(title, description, ConditionRestrictedInsideMagicCircle)
                .SetPossessive()
                .SetConditionType(ConditionType.Detrimental)
                .AddCustomSubFeatures(new CustomBehaviorElementalBane(damageType, power, conditionMark))
                .SetConditionParticleReference(conditionEffects[current++])
                .AddToDB();

            var spell = SpellDefinitionBuilder
                .Create(NAME + damageType)
                .SetGuiPresentation(title, Gui.NoLocalization)
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(4)
                .SetCastingTime(ActivationTime.Action)
                .SetMaterialComponent(MaterialComponentType.Mundane)
                .SetVerboseComponent(true)
                .SetSomaticComponent(true)
                .SetVocalSpellSameType(VocalSpellSemeType.Attack)
                .SetRequiresConcentration(true)
                .SetEffectDescription(
                    EffectDescriptionBuilder
                        .Create()
                        .SetDurationData(DurationType.Minute, 1)
                        .SetTargetingData(Side.Enemy, RangeType.Distance, 18, TargetType.IndividualsUnique)
                        .SetSavingThrowData(false, AttributeDefinitions.Constitution, false,
                            EffectDifficultyClassComputation.SpellCastingFeature)
                        .SetEffectForms(
                            EffectFormBuilder
                                .Create()
                                .HasSavingThrow(EffectSavingThrowType.Negates)
                                .SetConditionForm(condition, ConditionForm.ConditionOperation.Add)
                                .Build())
                        .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel,
                            additionalTargetsPerIncrement: 1)
                        .SetCasterEffectParameters(magicEffect)
                        .SetEffectEffectParameters(magicEffect)
                        .Build())
                .AddToDB();

            subSpells.Add(spell);
        }

        return SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(NAME, Category.Spell, sprite)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(4)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetRequiresConcentration(true)
            .SetSubSpells([..subSpells])
            // UI Only
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 18, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Constitution, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel,
                        additionalTargetsPerIncrement: 1)
                    .Build())
            .AddToDB();
    }

    private sealed class CustomBehaviorElementalBane(
        string damageType,
        FeatureDefinitionPower powerElementalBane,
        ConditionDefinition conditionMark)
        : IModifyDamageAffinity, IOnConditionAddedOrRemoved, ICharacterTurnStartListener
    {
        // required to ensure the behavior will still work after loading a save
        public void OnCharacterTurnStarted(GameLocationCharacter locationCharacter)
        {
            var rulesetCharacter = locationCharacter.RulesetCharacter;

            rulesetCharacter.DamageReceived -= DamageReceivedHandler;
            rulesetCharacter.DamageReceived += DamageReceivedHandler;
        }

        public void ModifyDamageAffinity(RulesetActor defender, RulesetActor attacker, List<FeatureDefinition> features)
        {
            features.RemoveAll(x =>
                x is IDamageAffinityProvider damageAffinityProvider &&
                damageAffinityProvider.DamageType == damageType &&
                damageAffinityProvider.DamageAffinityType is DamageAffinityType.Resistance);
        }

        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            target.DamageReceived += DamageReceivedHandler;
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            target.DamageReceived -= DamageReceivedHandler;
        }

        private void DamageReceivedHandler(
            RulesetActor rulesetDefender,
            int damage,
            string receivedDamageType,
            ulong sourceGuid,
            RollInfo rollInfo)
        {
            if (receivedDamageType != damageType)
            {
                return;
            }

            var defender = GameLocationCharacter.GetFromActor(rulesetDefender);

            if (defender == null ||
                rulesetDefender.HasConditionOfCategoryAndType(AttributeDefinitions.TagEffect, conditionMark.Name))
            {
                return;
            }

            var rulesetAttacker = EffectHelpers.GetCharacterByGuid(sourceGuid);
            var attacker = GameLocationCharacter.GetFromActor(rulesetAttacker);

            rulesetAttacker.LogCharacterActivatesAbility(string.Empty, "Feedback/&AdditionalDamageElementalBaneLine");
            rulesetDefender.InflictCondition(
                conditionMark.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfSourceTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                conditionMark.Name,
                0,
                0,
                0);

            var usablePower = PowerProvider.Get(powerElementalBane, rulesetAttacker);

            attacker.MyExecuteActionSpendPower(usablePower, defender);
        }
    }

    #endregion

    #region Forest Guardian

    internal static SpellDefinition BuildForestGuardian()
    {
        const string NAME = "ForestGuardian";

        var additionalDamage = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamageBeast{NAME}")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag($"Beast{NAME}")
            .SetRequiredProperty(RestrictedContextRequiredProperty.MeleeWeapon)
            .SetDamageDice(DieType.D6, 1)
            .SetSpecificDamageType(DamageTypeForce)
            .AddToDB();

        var conditionBeast = ConditionDefinitionBuilder
            .Create($"ConditionBeast{NAME}")
            .SetGuiPresentation(Category.Condition, ConditionRangerHideInPlainSight)
            .SetPossessive()
            .SetFeatures(
                additionalDamage,
                FeatureDefinitionMovementAffinitys.MovementAffinityCarriedByWind,
                FeatureDefinitionSenses.SenseDarkvision24)
            .CopyParticleReferences(PowerRangerSwiftBladeBattleFocus)
            .AddToDB();

        conditionBeast.AddCustomSubFeatures(new ModifyAttackActionModifierBeast(conditionBeast));

        var spellBeast = SpellDefinitionBuilder
            .Create($"Beast{NAME}")
            .SetGuiPresentation(Category.Spell)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(4)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(false)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Defense)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionBeast))
                    .SetParticleEffectParameters(AnimalShapes)
                    .Build())
            .AddToDB();

        var conditionHindered = ConditionDefinitionBuilder
            .Create(ConditionRestrainedByMagicalArrow, $"ConditionHindered{NAME}")
            .SetOrUpdateGuiPresentation(ConditionHindered.Name, Category.Rules)
            .SetParentCondition(ConditionHindered)
            .AddToDB();

        var conditionTree = ConditionDefinitionBuilder
            .Create($"ConditionTree{NAME}")
            .SetGuiPresentation(Category.Condition, ConditionRangerHideInPlainSight)
            .SetPossessive()
            .CopyParticleReferences(PowerRangerSwiftBladeBattleFocus)
            .AddToDB();

        conditionTree.AddCustomSubFeatures(new CustomBehaviorTree(conditionTree));

        var spellTree = SpellDefinitionBuilder
            .Create($"Tree{NAME}")
            .SetGuiPresentation(Category.Spell)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(4)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(false)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Defense)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 6)
                    .SetSavingThrowData(false, AttributeDefinitions.Strength, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .ExcludeCaster()
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.EndOfTurn, true)
                            .SetConditionForm(conditionHindered, ConditionForm.ConditionOperation.Add)
                            .Build(),
                        EffectFormBuilder.ConditionForm(
                            conditionTree,
                            ConditionForm.ConditionOperation.Add, true, true),
                        EffectFormBuilder
                            .Create()
                            .SetTempHpForm(10, DieType.D1, 0, true)
                            .Build())
                    .SetParticleEffectParameters(AnimalShapes)
                    .Build())
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.ForestGuardian, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(4)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(false)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Defense)
            .SetRequiresConcentration(true)
            .SetSubSpells(spellBeast, spellTree)
            .SetEffectDescription(
                // UI Only
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .Build())
            .AddToDB();

        return spell;
    }

    private sealed class ModifyAttackActionModifierBeast(ConditionDefinition conditionBeast)
        : IModifyAttackActionModifier
    {
        private readonly TrendInfo _trendInfo =
            new(1, FeatureSourceType.Condition, conditionBeast.Name, conditionBeast);

        public void OnAttackComputeModifier(
            RulesetCharacter myself,
            RulesetCharacter defender,
            BattleDefinitions.AttackProximity attackProximity,
            RulesetAttackMode attackMode,
            string effectName,
            ref ActionModifier attackModifier)
        {
            if (attackMode?.AbilityScore == AttributeDefinitions.Strength)
            {
                attackModifier.AttackAdvantageTrends.Add(_trendInfo);
            }
        }
    }

    private sealed class CustomBehaviorTree(ConditionDefinition conditionTree)
        : IModifyAttackActionModifier, IRollSavingThrowInitiated
    {
        private readonly TrendInfo _trendInfo =
            new(1, FeatureSourceType.Condition, conditionTree.Name, conditionTree);

        public void OnAttackComputeModifier(RulesetCharacter myself,
            RulesetCharacter defender,
            BattleDefinitions.AttackProximity attackProximity,
            RulesetAttackMode attackMode,
            string effectName,
            ref ActionModifier attackModifier)
        {
            var abilityScore = attackMode?.abilityScore;

            if (abilityScore == AttributeDefinitions.Dexterity
                || abilityScore == AttributeDefinitions.Wisdom
                || attackProximity == BattleDefinitions.AttackProximity.MagicRange
                || attackProximity == BattleDefinitions.AttackProximity.MagicReach)
            {
                attackModifier.AttackAdvantageTrends.Add(_trendInfo);
            }
        }

        public void OnSavingThrowInitiated(
            RulesetActor rulesetActorCaster,
            RulesetActor rulesetActorDefender,
            ref int saveBonus,
            ref string abilityScoreName,
            BaseDefinition sourceDefinition,
            List<TrendInfo> modifierTrends,
            List<TrendInfo> advantageTrends,
            ref int rollModifier,
            ref int saveDC,
            ref bool hasHitVisual,
            RollOutcome outcome,
            int outcomeDelta,
            List<EffectForm> effectForms)
        {
            if (abilityScoreName == AttributeDefinitions.Constitution)
            {
                advantageTrends.Add(
                    new TrendInfo(1, FeatureSourceType.Condition, conditionTree.Name, conditionTree));
            }
        }
    }

    #endregion

    #region Irresistible Performance

    internal static SpellDefinition BuildIrresistiblePerformance()
    {
        const string NAME = "IrresistiblePerformance";

        var actionAffinityIrresistiblePerformance = FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{NAME}")
            .SetGuiPresentationNoContent(true)
            .SetForbiddenActions(
                Id.AttackFree, Id.AttackMain, Id.AttackOff, Id.AttackOpportunity, Id.AttackReadied,
                Id.CastBonus, Id.CastInvocation, Id.CastMain, Id.CastReaction, Id.CastReadied, Id.CastRitual,
                Id.CastNoCost)
            .AddToDB();

        var conditionIrresistiblePerformance = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionCharmed, $"Condition{NAME}")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionCharmed)
            .SetParentCondition(ConditionDefinitions.ConditionCharmed)
            .SetPossessive()
            .SetFeatures(actionAffinityIrresistiblePerformance)
            .AddToDB();

        var conditionAffinityIrresistiblePerformanceImmunity = FeatureDefinitionConditionAffinityBuilder
            .Create($"ConditionAffinity{NAME}Immunity")
            .SetGuiPresentationNoContent(true)
            .SetConditionAffinityType(ConditionAffinityType.Immunity)
            .SetConditionType(conditionIrresistiblePerformance)
            .AddToDB();

        foreach (var conditionDefinition in DatabaseRepository.GetDatabase<ConditionDefinition>()
                     .Where(x => x.Features.Any(f =>
                         f is FeatureDefinitionConditionAffinity
                         {
                             ConditionAffinityType: ConditionAffinityType.Immunity
                         } conditionAffinity
                         && conditionAffinity.conditionType == ConditionDefinitions.ConditionCharmed.Name)))

        {
            conditionDefinition.Features.Add(conditionAffinityIrresistiblePerformanceImmunity);
        }

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.IrresistiblePerformance, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEnchantment)
            .SetSpellLevel(4)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(false)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Debuff)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.Cube, 6)
                    .SetSavingThrowData(false, AttributeDefinitions.Charisma, true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(
                            ConditionDefinitionBuilder
                                .Create($"Condition{NAME}ForceFailOnCharmed")
                                .SetGuiPresentationNoContent(true)
                                .SetSilent(Silent.WhenAddedOrRemoved)
                                .AddCustomSubFeatures(new RollSavingThrowFinishedIrresistiblePerformance())
                                .AddToDB()),
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(conditionIrresistiblePerformance, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .SetParticleEffectParameters(ConjureFey)
                    .SetEffectEffectParameters(PowerBardTraditionManacalonsPerfection)
                    .Build())
            .AddToDB();

        return spell;
    }

    private sealed class RollSavingThrowFinishedIrresistiblePerformance : IRollSavingThrowFinished
    {
        public void OnSavingThrowFinished(
            RulesetActor rulesetActorCaster,
            RulesetActor rulesetActorDefender,
            int saveBonus,
            string abilityScoreName,
            BaseDefinition sourceDefinition,
            List<TrendInfo> modifierTrends,
            List<TrendInfo> advantageTrends,
            int rollModifier,
            int saveDC,
            bool hasHitVisual,
            ref RollOutcome outcome,
            ref int outcomeDelta,
            List<EffectForm> effectForms)
        {
            if (outcome == RollOutcome.Failure)
            {
                return;
            }

            if (!rulesetActorDefender.ConditionsByCategory
                    .SelectMany(x => x.Value)
                    .Any(x =>
                        x.ConditionDefinition.IsSubtypeOf(ConditionDefinitions.ConditionCharmed.Name) &&
                        x.SourceGuid == rulesetActorCaster?.Guid))
            {
                return;
            }

            outcome = RollOutcome.Failure;
            outcomeDelta = -1;
        }
    }

    #endregion
}
