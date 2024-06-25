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
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionConditionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;

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
            .SetSpecialInterruptions(ExtraConditionInterruption.AfterWasAttacked)
            .AddToDB();

        return SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.MindBlank, 128))
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
                    .SetRestrictedCreatureFamilies(
                        DatabaseRepository.GetDatabase<CharacterFamilyDefinition>()
                            .Select(x => x.Name)
                            .Where(x =>
                                x != CharacterFamilyDefinitions.Construct.Name &&
                                x != CharacterFamilyDefinitions.Undead.Name)
                            .ToArray())
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypeNecrotic, 10, DieType.D8)
                            .Build())
                    .SetParticleEffectParameters(CircleOfDeath)
                    .Build())
            .AddCustomSubFeatures(new MagicEffectBeforeHitConfirmedOnEnemy(condition))
            .AddToDB();
    }

    private sealed class MagicEffectBeforeHitConfirmedOnEnemy(
        ConditionDefinition condition) : IMagicEffectBeforeHitConfirmedOnEnemy
    {
        public IEnumerator OnMagicEffectBeforeHitConfirmedOnEnemy(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetDefender is not RulesetCharacterMonster rulesetCharacterMonster ||
                (rulesetCharacterMonster.MonsterDefinition != MonsterDefinitions.Ice_Elemental &&
                 rulesetCharacterMonster.MonsterDefinition.Name != "WildShapeWaterElemental" &&
                 rulesetCharacterMonster.CharacterFamily != "Plant"))
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;

            rulesetDefender.InflictCondition(
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

    #endregion

    #region Soul Expulsion

    internal static SpellDefinition BuildSoulExpulsion()
    {
        const string NAME = "SoulExpulsion";

        var conditionSavingThrowAffinity = ConditionDefinitionBuilder
            .Create($"Condition{NAME}SavingThrowAffinity")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(
                FeatureDefinitionSavingThrowAffinityBuilder
                    .Create($"SavingThrowAffinity{NAME}")
                    .SetGuiPresentation(NAME, Category.Spell, Gui.NoLocalization)
                    .SetAffinities(CharacterSavingThrowAffinity.Disadvantage, false, AttributeDefinitions.Charisma)
                    .AddToDB())
            .SetSpecialInterruptions(ConditionInterruption.SavingThrow)
            .AddToDB();

        var conditionCombatAffinity = ConditionDefinitionBuilder
            .Create($"Condition{NAME}CombatAffinity")
            .SetGuiPresentation(NAME, Category.Spell, ConditionDistracted)
            .SetFeatures(
                FeatureDefinitionCombatAffinityBuilder
                    .Create($"CombatAffinity{NAME}")
                    .SetGuiPresentation(NAME, Category.Spell, Gui.NoLocalization)
                    .SetMyAttackAdvantage(AdvantageType.Disadvantage)
                    .AddToDB())
            .SetPossessive()
            .AddToDB();

        conditionCombatAffinity.GuiPresentation.description = Gui.NoLocalization;

        var power = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}")
            .SetGuiPresentation(NAME, Category.Spell)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, (TurnOccurenceType)ExtraTurnOccurenceType.StartOfSourceTurn)
                    .SetTargetingData(Side.All, RangeType.Distance, 24, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Charisma, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
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
                    .SetCasterEffectParameters(Sunburst)
                    .SetImpactEffectParameters(Sunburst)
                    .Build())
            .AddToDB();

        return SpellDefinitionBuilder
            .Create(Darkness, NAME)
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
                    .SetParticleEffectParameters(CircleOfDeath)
                    .Build())
            .AddCustomSubFeatures(new PowerOrSpellFinishedByMe(power, conditionSavingThrowAffinity))
            .AddToDB();
    }

    private sealed class PowerOrSpellFinishedByMe(
        FeatureDefinitionPower power,
        ConditionDefinition conditionSavingThrowAffinity) : IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            if (Gui.Battle == null)
            {
                yield break;
            }

            var actingCharacter = action.ActingCharacter;
            var rulesetCharacter = actingCharacter.RulesetCharacter;
            var target = action.actionParams.TargetCharacters[0];
            var targets = Gui.Battle.GetContenders(
                target, actingCharacter, isOppositeSide: false, hasToPerceiveTarget: true, withinRange: 12);
            var actionModifiers = new List<ActionModifier>();

            foreach (var glc in targets
                         .Where(x => x.RulesetCharacter is RulesetCharacterMonster rulesetCharacterMonster &&
                                     rulesetCharacterMonster.CharacterFamily == CharacterFamilyDefinitions.Undead.Name))
            {
                var rulesetDefender = glc.RulesetCharacter;

                rulesetDefender.InflictCondition(
                    conditionSavingThrowAffinity.Name,
                    DurationType.Round,
                    0,
                    TurnOccurenceType.EndOfSourceTurn,
                    AttributeDefinitions.TagEffect,
                    rulesetCharacter.guid,
                    rulesetCharacter.CurrentFaction.Name,
                    1,
                    conditionSavingThrowAffinity.Name,
                    0,
                    0,
                    0);
            }

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
    }

    #endregion
}
