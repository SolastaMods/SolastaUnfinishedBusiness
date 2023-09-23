using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Properties;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionConditionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;

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
            .SetCustomSubFeatures(ValidatorsRestrictedContext.IsWeaponAttack)
            .SetDamageDice(DieType.D6, 4)
            .SetSpecificDamageType(DamageTypePsychic)
            .SetSavingThrowData(
                EffectDifficultyClassComputation.SpellCastingFeature,
                EffectSavingThrowType.None,
                AttributeDefinitions.Wisdom)
            .SetConditionOperations(
                new ConditionOperationDescription
                {
                    hasSavingThrow = true,
                    canSaveToCancel = false,
                    saveAffinity = EffectSavingThrowType.Negates,
                    conditionDefinition = conditionStaggeringSmiteEnemy,
                    operation = ConditionOperationDescription.ConditionOperation.Add
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
                    .Build())
            .AddToDB();

        spell.EffectDescription.EffectParticleParameters.effectParticleReference = new AssetReference();

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
            .SetCustomSubFeatures(PushesOrDragFromEffectPoint.Marker)
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

    #region Aura of Vitality

    internal static SpellDefinition BuildAuraOfVitality()
    {
        const string NAME = "AuraOfVitality";

        var conditionAffinityLifeDrained = FeatureDefinitionConditionAffinityBuilder
            .Create($"ConditionAffinity{NAME}LifeDrained")
            .SetGuiPresentationNoContent(true)
            .SetConditionAffinityType(ConditionAffinityType.Immunity)
            .SetConditionType(ConditionLifeDrained)
            .AddToDB();

        var conditionAuraOfVitality = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(Category.Condition, ConditionDivineFavor)
            .SetPossessive()
            .SetFeatures(
                conditionAffinityLifeDrained,
                DamageAffinityNecroticResistance)
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
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Sphere, 7)
                    .SetRecurrentEffect(
                        RecurrentEffect.OnActivation | RecurrentEffect.OnEnter | RecurrentEffect.OnTurnStart)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(conditionAuraOfVitality),
                        EffectFormBuilder.ConditionForm(ConditionLifeDrained, ConditionForm.ConditionOperation.Remove))
                    .SetParticleEffectParameters(FeatureDefinitionPowers.PowerPaladinAuraOfProtection)
                    .Build())
            .AddToDB();

        return spell;
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
            .SetGuiPresentation(Category.Condition, ConditionDivineFavor)
            .SetPossessive()
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
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Sphere, 7)
                    .SetRecurrentEffect(
                        RecurrentEffect.OnActivation | RecurrentEffect.OnEnter | RecurrentEffect.OnTurnStart)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionAuraOfPerseverance))
                    .SetParticleEffectParameters(FeatureDefinitionPowers.PowerPaladinAuraOfProtection)
                    .Build())
            .AddToDB();

        conditionAuraOfPerseverance.SetCustomSubFeatures(new ModifySavingThrowAuraOfPerseverance(spell));

        return spell;
    }

    private sealed class ModifySavingThrowAuraOfPerseverance : IModifySavingThrow
    {
        private readonly SpellDefinition _spellDefinition;

        public ModifySavingThrowAuraOfPerseverance(SpellDefinition spellDefinition)
        {
            _spellDefinition = spellDefinition;
        }

        public bool IsValid(
            RulesetActor rulesetActor,
            RulesetActor rulesetCaster,
            IEnumerable<EffectForm> effectForms,
            string attributeScore)
        {
            return effectForms.Any(x =>
                x.FormType == EffectForm.EffectFormType.Condition
                && (x.ConditionForm.ConditionDefinition ==
                    ConditionDefinitions.ConditionBlinded
                    || x.ConditionForm.ConditionDefinition ==
                    ConditionDefinitions.ConditionCharmed
                    || x.ConditionForm.ConditionDefinition ==
                    ConditionDefinitions.ConditionDeafened
                    || x.ConditionForm.ConditionDefinition ==
                    ConditionDefinitions.ConditionFrightened
                    || x.ConditionForm.ConditionDefinition ==
                    ConditionDefinitions.ConditionParalyzed
                    || x.ConditionForm.ConditionDefinition ==
                    ConditionDefinitions.ConditionPoisoned
                    || x.ConditionForm.ConditionDefinition ==
                    ConditionDefinitions.ConditionStunned));
        }

        public string AttributeAndActionModifier(
            RulesetActor rulesetActor,
            ActionModifier actionModifier,
            string attribute)
        {
            actionModifier.SavingThrowAdvantageTrends.Add(
                new TrendInfo(1, FeatureSourceType.Spell, _spellDefinition.Name, _spellDefinition));

            return attribute;
        }
    }

    #endregion
}
