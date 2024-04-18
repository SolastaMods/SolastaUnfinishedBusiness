using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using UnityEngine.AddressableAssets;
using static ActionDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionConditionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaUnfinishedBusiness.Spells;

internal static partial class SpellBuilders
{
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
                    .BuildAndSetAffinityGroups(CharacterAbilityCheckAffinity.Disadvantage,
                        AttributeDefinitions.Strength,
                        AttributeDefinitions.Dexterity,
                        AttributeDefinitions.Constitution,
                        AttributeDefinitions.Intelligence,
                        AttributeDefinitions.Wisdom,
                        AttributeDefinitions.Charisma)
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
            .AddCustomSubFeatures(ValidatorsRestrictedContext.IsWeaponOrUnarmedAttack)
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
            .SetSpecialInterruptions(ConditionInterruption.AttacksAndDamages)
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
                            .SetMotionForm(MotionForm.MotionType.DragToOrigin, 4)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeForce, 5, DieType.D10)
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .Build())
                    .SetParticleEffectParameters(Shatter.EffectDescription.EffectParticleParameters)
                    .Build())
            .AddCustomSubFeatures(ForcePushOrDragFromEffectPoint.Marker)
            .AddToDB();

        return spell;
    }

    #endregion

    #region Brain Bulwark

    internal static SpellDefinition BuildFaithfulHound()
    {
        const string NAME = "FaithfulHound";

        var proxyFaithfulHound = EffectProxyDefinitionBuilder
            .Create(EffectProxyDefinitions.ProxyArcaneSword, $"EffectProxy{NAME}")
            .SetOrUpdateGuiPresentation(Category.Proxy)
            .AddToDB();

        proxyFaithfulHound.actionId = Id.AttackFree;
        proxyFaithfulHound.additionalFeatures.Clear();
        proxyFaithfulHound.attackParticle = new AssetReference();
        proxyFaithfulHound.canMove = false;
        proxyFaithfulHound.damageDie = DieType.D8;
        proxyFaithfulHound.damageDieNum = 4;
        proxyFaithfulHound.damageType = DamageTypePiercing;
        proxyFaithfulHound.prefabReference = MonsterDefinitions.FeyWolf.MonsterPresentation.malePrefabReference;

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.FaithfulHound, 128))
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
        : IActionFinishedByEnemy, IRollSavingThrowInitiated
    {
        public IEnumerator OnActionFinishedByEnemy(CharacterAction characterAction, GameLocationCharacter target)
        {
            var rulesetCharacter = target.RulesetCharacter;

            if (rulesetCharacter.TemporaryHitPoints == 0)
            {
                rulesetCharacter.RemoveAllConditionsOfCategoryAndType(
                    AttributeDefinitions.TagEffect, "ConditionBlessingOfRime");
            }

            yield break;
        }

        public void OnSavingThrowInitiated(
            RulesetCharacter caster,
            RulesetCharacter defender,
            ref string abilityScoreName,
            BaseDefinition sourceDefinition,
            List<TrendInfo> advantageTrends,
            int saveDC,
            bool hasHitVisual,
            List<EffectForm> effectForms)
        {
            if (abilityScoreName == AttributeDefinitions.Constitution)
            {
                advantageTrends.Add(
                    new TrendInfo(1, FeatureSourceType.Spell, spellDefinition.Name, spellDefinition));
            }
        }
    }

    #endregion

    #region Aura of Life

    internal static SpellDefinition BuildAuraOfLife()
    {
        // kept this name for backward compatibility reasons
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
                // need to keep this condition when hero is downed so it can stand up on it's next turn
                new ForceConditionCategory(AttributeDefinitions.TagCombat))
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.AuraOfVitality, 128))
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

    private sealed class ModifySavingThrowAuraOfPerseverance(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        SpellDefinition spellDefinition)
        : IRollSavingThrowInitiated
    {
        public void OnSavingThrowInitiated(
            RulesetCharacter caster,
            RulesetCharacter defender,
            ref string abilityScoreName,
            BaseDefinition sourceDefinition,
            List<TrendInfo> advantageTrends,
            int saveDC,
            bool hasHitVisual,
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
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionTree, ConditionForm.ConditionOperation.Add, true, true)
                            .Build(),
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
                    .SetDurationData(DurationType.Minute, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .Build())
            .AddToDB();

        return spell;
    }

    private sealed class ModifyAttackActionModifierBeast(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionBeast)
        : IModifyAttackActionModifier
    {
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
                attackModifier.attackAdvantageTrends.Add(
                    new TrendInfo(1, FeatureSourceType.Condition, conditionBeast.Name, conditionBeast));
            }
        }
    }

    private sealed class CustomBehaviorTree(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionTree)
        : IModifyAttackActionModifier, IRollSavingThrowInitiated
    {
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
                attackModifier.attackAdvantageTrends.Add(
                    new TrendInfo(1, FeatureSourceType.Condition, conditionTree.Name, conditionTree));
            }
        }

        public void OnSavingThrowInitiated(
            RulesetCharacter caster,
            RulesetCharacter defender,
            ref string abilityScoreName,
            BaseDefinition sourceDefinition,
            List<TrendInfo> advantageTrends,
            int saveDC,
            bool hasHitVisual,
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
                Id.CastBonus, Id.CastInvocation, Id.CastMain, Id.CastReaction, Id.CastReadied, Id.CastNoCost)
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
            RulesetCharacter caster,
            RulesetCharacter defender,
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
            if (caster == null || outcome is RollOutcome.Failure or RollOutcome.CriticalFailure)
            {
                return;
            }

            if (!defender.AllConditions.Any(x =>
                    x.ConditionDefinition.IsSubtypeOf(ConditionDefinitions.ConditionCharmed.Name) &&
                    x.SourceGuid == caster.Guid))
            {
                return;
            }

            outcome = RollOutcome.Failure;
            outcomeDelta = -1;
        }
    }

    #endregion
}
