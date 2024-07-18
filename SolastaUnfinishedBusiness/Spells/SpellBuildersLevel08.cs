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
using SolastaUnfinishedBusiness.Subclasses;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionConditionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Spells;

internal static partial class SpellBuilders
{
    #region Maddening Darkness

    internal static SpellDefinition BuildMaddeningDarkness()
    {
        const string NAME = "MaddeningDarkness";

        return SpellDefinitionBuilder
            .Create(Darkness, NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.MaddeningDarkness, 128))
            .SetSpellLevel(8)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(Darkness)
                    .SetTargetingData(Side.All, RangeType.Distance, 24, TargetType.Sphere, 12)
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetRecurrentEffect(RecurrentEffect.OnActivation | RecurrentEffect.OnTurnEnd)
                    .AddEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypePsychic, 6, DieType.D8)
                            .Build())
                    .Build())
            .AddToDB();
    }

    #endregion

    #region Mind Blank

    internal static SpellDefinition BuildMindBlank()
    {
        const string NAME = "MindBlank";

        return SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.MindBlank, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(8)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Hour, 24)
                    .SetTargetingData(Side.Ally, RangeType.Touch, 0, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(
                            ConditionDefinitionBuilder
                                .Create(ConditionBearsEndurance, "ConditionMindBlank")
                                .SetOrUpdateGuiPresentation(Category.Condition)
                                .SetFeatures(
                                    ConditionAffinityCharmImmunity,
                                    ConditionAffinityCharmImmunityHypnoticPattern,
                                    ConditionAffinityCalmEmotionCharmedImmunity,
                                    DamageAffinityPsychicImmunity)
                                .AddToDB()))
                    .SetParticleEffectParameters(DispelMagic)
                    .Build())
            .AddToDB();
    }

    #endregion

    #region Abi-Dalzim's Horrid Wilting

    internal static SpellDefinition BuildAbiDalzimHorridWilting()
    {
        const string NAME = "AbiDalzimHorridWilting";

        var condition = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(
                FeatureDefinitionSavingThrowAffinityBuilder
                    .Create($"SavingThrowAffinity{NAME}")
                    .SetGuiPresentation(NAME, Category.Spell, Gui.NoLocalization)
                    .SetAffinities(CharacterSavingThrowAffinity.Disadvantage, false, AttributeDefinitions.Constitution)
                    .AddToDB())
            .SetSpecialInterruptions(ConditionInterruption.SavingThrow)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.AbiDalzimHorridWilting, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolNecromancy)
            .SetSpellLevel(8)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Distance, 24, TargetType.Cube, 6)
                    .SetSavingThrowData(false, AttributeDefinitions.Constitution, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .AddImmuneCreatureFamilies(CharacterFamilyDefinitions.Undead)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypeNecrotic, 12, DieType.D8)
                            .Build())
                    .SetCasterEffectParameters(FingerOfDeath)
                    .SetImpactEffectParameters(FingerOfDeath)
                    .Build())
            .AddCustomSubFeatures(new PowerOrSpellInitiatedByMeAbiDalzimHorridWilting(condition))
            .AddToDB();

        return spell;
    }

    private sealed class PowerOrSpellInitiatedByMeAbiDalzimHorridWilting(
        ConditionDefinition condition) : IPowerOrSpellInitiatedByMe
    {
        public IEnumerator OnPowerOrSpellInitiatedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var targets = action.ActionParams.TargetCharacters.ToList();
            var rulesetAttacker = action.ActingCharacter.RulesetCharacter;

            foreach (var target in targets)
            {
                if (target.RulesetCharacter is not RulesetCharacterMonster rulesetCharacterMonster)
                {
                    continue;
                }

                if (rulesetCharacterMonster.CharacterFamily == "Plant" ||
                    rulesetCharacterMonster.MonsterDefinition == MonsterDefinitions.Ice_Elemental ||
                    rulesetCharacterMonster.MonsterDefinition == CircleOfTheNight.WildShapeWaterElemental)
                {
                    rulesetCharacterMonster.InflictCondition(
                        condition.Name,
                        DurationType.Round,
                        0,
                        TurnOccurenceType.EndOfSourceTurn,
                        AttributeDefinitions.TagEffect,
                        rulesetAttacker.guid,
                        rulesetAttacker.CurrentFaction.Name,
                        1,
                        condition.Name,
                        0,
                        0,
                        0);
                }
            }

            yield break;
        }
    }

    #endregion

    #region Soul Expulsion

    internal static SpellDefinition BuildSoulExpulsion()
    {
        const string NAME = "SoulExpulsion";

        var conditionSavingThrowAffinity = ConditionDefinitionBuilder
            .Create($"Condition{NAME}SavingThrowAffinity")
            .SetGuiPresentation(NAME, Category.Spell, Gui.NoLocalization)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(
                FeatureDefinitionSavingThrowAffinityBuilder
                    .Create($"SavingThrowAffinity{NAME}")
                    .SetGuiPresentation(NAME, Category.Spell, Gui.NoLocalization)
                    .SetAffinities(CharacterSavingThrowAffinity.Disadvantage, false, AttributeDefinitions.Charisma)
                    .AddToDB())
            .SetSpecialInterruptions(ConditionInterruption.Attacked)
            .AddCustomSubFeatures(new ModifyDamageAffinitySoulExpulsion())
            .AddToDB();

        var conditionCombatAffinity = ConditionDefinitionBuilder
            .Create($"Condition{NAME}CombatAffinity")
            .SetGuiPresentation(Category.Condition, Gui.NoLocalization, ConditionDoomLaughter)
            .SetConditionType(ConditionType.Detrimental)
            .SetFeatures(
                FeatureDefinitionCombatAffinityBuilder
                    .Create($"CombatAffinity{NAME}")
                    .SetGuiPresentation(NAME, Category.Spell, Gui.NoLocalization)
                    .SetMyAttackAdvantage(AdvantageType.Disadvantage)
                    .AddToDB())
            .SetConditionParticleReference(ConditionEyebitePanicked)
            .AddToDB();

        var power = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}")
            .SetGuiPresentation(NAME, Category.Spell)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.All, RangeType.Distance, 24, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 2)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypeRadiant, 7, DieType.D8)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(conditionCombatAffinity, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .SetImpactEffectParameters(GuidingBolt)
                    .Build())
            .AddToDB();

        power.AddCustomSubFeatures(new ModifyEffectDescriptionSoulExpulsion(power));

        return SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.SoulExpulsion, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolNecromancy)
            .SetSpellLevel(8)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, (TurnOccurenceType)ExtraTurnOccurenceType.StartOfSourceTurn)
                    .SetTargetingData(Side.All, RangeType.Distance, 24, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Charisma, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 2)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypeNecrotic, 11, DieType.D8)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(ConditionDefinitions.ConditionStunned,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .SetParticleEffectParameters(Sunburst)
                    .SetCasterEffectParameters(Blindness)
                    .SetImpactEffectParameters(PowerPatronFiendDarkOnesOwnLuck
                        .EffectDescription.EffectParticleParameters.effectParticleReference)
                    .Build())
            .AddCustomSubFeatures(new CustomBehaviorSoulExpulsion(power, conditionSavingThrowAffinity))
            .AddToDB();
    }

    private sealed class ModifyEffectDescriptionSoulExpulsion(FeatureDefinitionPower power)
        : IModifyEffectDescription, IModifyDamageAffinity
    {
        public void ModifyDamageAffinity(RulesetActor defender, RulesetActor attacker, List<FeatureDefinition> features)
        {
            if (defender is RulesetCharacterMonster rulesetCharacterMonster &&
                rulesetCharacterMonster.CharacterFamily == CharacterFamilyDefinitions.Undead.Name)
            {
                features.RemoveAll(x =>
                    x is IDamageAffinityProvider
                    {
                        DamageAffinityType: DamageAffinityType.Resistance, DamageType: DamageTypeNecrotic
                    } or IDamageAffinityProvider
                    {
                        DamageAffinityType: DamageAffinityType.Immunity, DamageType: DamageTypeNecrotic
                    });
            }
        }

        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == power;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var glc = GameLocationCharacter.GetFromActor(character);

            if (glc != null &&
                glc.UsedSpecialFeatures.TryGetValue("SoulExpulsion", out var effectLevel))
            {
                effectDescription.EffectForms[0].DamageForm.DiceNumber = 7 + (2 * (effectLevel - 8));
            }

            return effectDescription;
        }
    }

    private sealed class CustomBehaviorSoulExpulsion(
        FeatureDefinitionPower power,
        ConditionDefinition conditionSavingThrowAffinity) : IPowerOrSpellInitiatedByMe, IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            if (Gui.Battle == null || action.Countered)
            {
                yield break;
            }

            var actingCharacter = action.ActingCharacter;
            var rulesetCharacter = actingCharacter.RulesetCharacter;
            var target = action.actionParams.TargetCharacters[0];
            var targets = Gui.Battle.GetContenders(
                target, actingCharacter, isOppositeSide: false, hasToPerceiveTarget: true, withinRange: 12);
            var actionModifiers = new List<ActionModifier>();

            actingCharacter.UsedSpecialFeatures.TryAdd("SoulExpulsion", action.ActionParams.RulesetEffect.EffectLevel);

            for (var i = 0; i < targets.Count; i++)
            {
                actionModifiers.Add(new ActionModifier());
            }

            var implementationManager =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(power, rulesetCharacter);
            var actionParams = new CharacterActionParams(actingCharacter, ActionDefinitions.Id.PowerNoCost)
            {
                ActionModifiers = actionModifiers,
                RulesetEffect = implementationManager
                    .MyInstantiateEffectPower(rulesetCharacter, usablePower, false),
                UsablePower = usablePower,
                targetCharacters = targets
            };

            ServiceRepository.GetService<ICommandService>()
                ?.ExecuteAction(actionParams, null, true);
        }

        public IEnumerator OnPowerOrSpellInitiatedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var targets = action.ActionParams.TargetCharacters.ToList();
            var rulesetAttacker = action.ActingCharacter.RulesetCharacter;

            foreach (var target in targets)
            {
                if (target.RulesetCharacter is RulesetCharacterMonster rulesetCharacterMonster &&
                    rulesetCharacterMonster.CharacterFamily == CharacterFamilyDefinitions.Undead.Name)
                {
                    rulesetCharacterMonster.InflictCondition(
                        conditionSavingThrowAffinity.Name,
                        DurationType.Round,
                        0,
                        TurnOccurenceType.EndOfSourceTurn,
                        AttributeDefinitions.TagEffect,
                        rulesetAttacker.guid,
                        rulesetAttacker.CurrentFaction.Name,
                        1,
                        conditionSavingThrowAffinity.Name,
                        0,
                        0,
                        0);
                }
            }

            yield break;
        }
    }

    private sealed class ModifyDamageAffinitySoulExpulsion : IModifyDamageAffinity
    {
        public void ModifyDamageAffinity(RulesetActor defender, RulesetActor attacker, List<FeatureDefinition> features)
        {
            if (defender is RulesetCharacterMonster rulesetCharacterMonster &&
                rulesetCharacterMonster.CharacterFamily == CharacterFamilyDefinitions.Undead.Name)
            {
                features.RemoveAll(x =>
                    x is IDamageAffinityProvider
                    {
                        DamageAffinityType: DamageAffinityType.Resistance or DamageAffinityType.Immunity,
                        DamageType: DamageTypeNecrotic
                    });
            }
        }
    }

    #endregion
}
