using System.Collections;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.MonsterDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAbilityCheckAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionCombatAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSavingThrowAffinitys;

namespace SolastaUnfinishedBusiness.Spells;

internal static partial class SpellBuilders
{
    #region Foresight

    internal static SpellDefinition BuildForesight()
    {
        const string NAME = "Foresight";

        return SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.Foresight, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(9)
            .SetCastingTime(ActivationTime.Minute1)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Divination)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Hour, 8)
                    .SetTargetingData(Side.Ally, RangeType.Touch, 0, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(
                            ConditionDefinitionBuilder
                                .Create(ConditionDefinitions.ConditionBearsEndurance, "ConditionForesight")
                                .SetOrUpdateGuiPresentation(Category.Condition)
                                .SetFeatures(
                                    AbilityCheckAffinityConditionBearsEndurance,
                                    AbilityCheckAffinityConditionBullsStrength,
                                    AbilityCheckAffinityConditionCatsGrace,
                                    AbilityCheckAffinityConditionEaglesSplendor,
                                    AbilityCheckAffinityConditionFoxsCunning,
                                    AbilityCheckAffinityConditionOwlsWisdom,
                                    CombatAffinityStealthy,
                                    SavingThrowAffinityShelteringBreeze)
                                .AddToDB()))
                    .SetParticleEffectParameters(DispelMagic)
                    .Build())
            .AddToDB();
    }

    #endregion

    #region Invulnerability

    internal static SpellDefinition BuildInvulnerability()
    {
        const string NAME = "Invulnerability";

        var conditionInvulnerability = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(NAME, Category.Spell, ConditionDefinitions.ConditionShielded)
            .SetFeatures(
                DatabaseRepository.GetDatabase<DamageDefinition>()
                    .Select(damageType =>
                        FeatureDefinitionDamageAffinityBuilder
                            .Create($"DamageAffinity{NAME}{damageType.Name}")
                            .SetGuiPresentationNoContent(true)
                            .SetDamageType(damageType.Name)
                            .SetDamageAffinityType(DamageAffinityType.Immunity)
                            .AddToDB()))
            .CopyParticleReferences(DispelEvilAndGood)
            .AddToDB();

        conditionInvulnerability.GuiPresentation.description = Gui.EmptyContent;

        return SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.Invulnerability, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolAbjuration)
            .SetSpellLevel(9)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Specific)
            .SetSpecificMaterialComponent(TagsDefinitions.ItemTagDiamond, 500, true)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 10)
                    .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionInvulnerability))
                    .SetParticleEffectParameters(DispelMagic)
                    .SetCasterEffectParameters(HolyAura)
                    .Build())
            .AddToDB();
    }

    #endregion

    #region Mass Heal

    internal static SpellDefinition BuildMassHeal()
    {
        return SpellDefinitionBuilder
            .Create("MassHeal")
            .SetGuiPresentation(Category.Spell, Heal)
            .SetCastingTime(ActivationTime.Action)
            .SetSpellLevel(9)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Healing)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Distance, 12, TargetType.IndividualsUnique, 6)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetHealingForm(
                                HealingComputation.Dice,
                                120,
                                DieType.D1,
                                0,
                                false,
                                HealingCap.MaximumHitPoints)
                            .Build())
                    .SetParticleEffectParameters(Heal)
                    .Build())
            .AddToDB();
    }

    #endregion

    #region Meteor Swarm

    internal static SpellDefinition BuildMeteorSwarmSingleTarget()
    {
        const string NAME = "MeteorSwarmSingleTarget";

        return SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.MeteorSwarm, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(9)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Distance, 18, TargetType.Sphere, 8)
                    // 20 dice number because hits dont stack even on single target
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeFire, 20, DieType.D6)
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeBludgeoning, 20, DieType.D6)
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .Build())
                    .SetSavingThrowData(
                        false,
                        AttributeDefinitions.Dexterity,
                        true,
                        EffectDifficultyClassComputation.SpellCastingFeature,
                        AttributeDefinitions.Dexterity,
                        13)
                    .SetParticleEffectParameters(FlameStrike)
                    .Build())
            .AddToDB();
    }

    #endregion

    #region Weird

    internal static SpellDefinition BuildWeird()
    {
        return SpellDefinitionBuilder
            .Create("Weird")
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite("Weird", Resources.Weird, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolIllusion)
            .SetSpellLevel(9)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 24, TargetType.Sphere, 6)
                    .SetSavingThrowData(
                        false,
                        AttributeDefinitions.Wisdom,
                        true,
                        EffectDifficultyClassComputation.SpellCastingFeature,
                        AttributeDefinitions.Constitution,
                        13)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypePsychic, 10, DieType.D10)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                ConditionDefinitionBuilder
                                    .Create(ConditionDefinitions.ConditionFrightenedPhantasmalKiller, "ConditionWeird")
                                    .SetOrUpdateGuiPresentation(Category.Condition)
                                    .SetRecurrentEffectForms(
                                        EffectFormBuilder.DamageForm(DamageTypePsychic, 5, DieType.D10))
                                    .AddToDB(),
                                ConditionForm.ConditionOperation.Add)
                            .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.EndOfTurn, true)
                            .Build())
                    .SetCasterEffectParameters(PhantasmalKiller)
                    .SetImpactEffectParameters(
                        PhantasmalKiller.EffectDescription.EffectParticleParameters.effectParticleReference)
                    .Build())
            .SetRequiresConcentration(true)
            .AddToDB();
    }

    #endregion

    #region Psychic Scream

    internal static SpellDefinition BuildPsychicScream()
    {
        const string NAME = "PsychicScream";

        return SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.PsychicScream, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEnchantment)
            .SetSpellLevel(9)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetSomaticComponent(true)
            .SetVerboseComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.All, RangeType.Distance, 18, TargetType.IndividualsUnique, 10)
                    .SetSavingThrowData(false, AttributeDefinitions.Intelligence, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.EndOfTurn, true)
                            .SetConditionForm(ConditionDefinitions.ConditionStunned,
                                ConditionForm.ConditionOperation.Add)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypePsychic, 14, DieType.D6)
                            .Build())
                    .SetParticleEffectParameters(PowerWordStun)
                    .Build())
            .AddToDB();
    }

    #endregion

    #region Power Word Kill

    internal static SpellDefinition BuildPowerWordKill()
    {
        return SpellDefinitionBuilder
            .Create("PowerWordKill")
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite("PowerWordKill", Resources.PowerWordKill, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(9)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .SetHitPointsFilter(0, 100, 10000)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetFilterId(0)
                            .SetDamageForm(DamageTypePsychic, 12, DieType.D12)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetKillForm(KillCondition.UnderHitPoints, 0F, 100)
                            .Build())
                    .SetParticleEffectParameters(FingerOfDeath)
                    .Build())
            .AddCustomSubFeatures(new FilterTargetingCharacterPowerWordKillOrHeal())
            .AddToDB();
    }

    #endregion

    #region Power Word Heal

    internal static SpellDefinition BuildPowerWordHeal()
    {
        return SpellDefinitionBuilder
            .Create("PowerWordHeal")
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite("PowerWordHeal", Resources.PowerWordHeal, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEnchantment)
            .SetSpellLevel(9)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(false)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Healing)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetHealingForm(
                                HealingComputation.Dice,
                                700,
                                DieType.D1,
                                0,
                                false,
                                HealingCap.MaximumHitPoints)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                ConditionDefinitions.ConditionParalyzed,
                                ConditionForm.ConditionOperation.RemoveDetrimentalAll,
                                false,
                                false,
                                ConditionDefinitions.ConditionCharmed,
                                ConditionDefinitions.ConditionFrightened,
                                ConditionDefinitions.ConditionParalyzed,
                                ConditionDefinitions.ConditionPoisoned,
                                ConditionDefinitions.ConditionProne)
                            .Build())
                    .SetParticleEffectParameters(Regenerate)
                    .Build())
            .AddCustomSubFeatures(new FilterTargetingCharacterPowerWordKillOrHeal())
            .AddToDB();
    }

    // required to support Bard level 20 feature Words of Creations (only scenario where these spells have a 2nd target)
    private sealed class FilterTargetingCharacterPowerWordKillOrHeal : IFilterTargetingCharacter
    {
        public bool EnforceFullSelection => false;

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            if (__instance.SelectionService.SelectedTargets.Count == 0 ||
                __instance.SelectionService.SelectedTargets[0].IsWithinRange(target, 2))
            {
                return true;
            }

            __instance.actionModifier.FailureFlags.Add("Failure/&SecondTargetNotWithinRange");

            return false;
        }
    }

    #endregion

    #region Shapechange

    internal const string ShapechangeName = "Shapechange";

    internal static SpellDefinition BuildShapechange()
    {
        return SpellDefinitionBuilder
            .Create(ShapechangeName)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(ShapechangeName, Resources.ShapeChange, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(9)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Specific)
            .SetSpecificMaterialComponent("Diamond", 1500, false)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetParticleEffectParameters(PowerDruidWildShape)
                    .SetDurationData(DurationType.Hour, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetShapeChangeForm(
                                ShapeChangeForm.Type.FreeListSelection,
                                true,
                                ConditionDefinitions.ConditionWildShapeSubstituteForm,
                                [
                                    new ShapeOptionDescription
                                    {
                                        requiredLevel = 1, substituteMonster = BlackDragon_MasterOfNecromancy
                                    },
                                    new ShapeOptionDescription { requiredLevel = 1, substituteMonster = Divine_Avatar },
                                    new ShapeOptionDescription
                                    {
                                        requiredLevel = 1, substituteMonster = Emperor_Laethar
                                    },
                                    new ShapeOptionDescription { requiredLevel = 1, substituteMonster = Giant_Ape },
                                    new ShapeOptionDescription
                                    {
                                        requiredLevel = 1, substituteMonster = GoldDragon_AerElai
                                    },
                                    new ShapeOptionDescription
                                    {
                                        requiredLevel = 1, substituteMonster = GreenDragon_MasterOfConjuration
                                    },
                                    new ShapeOptionDescription { requiredLevel = 1, substituteMonster = Remorhaz },
                                    new ShapeOptionDescription { requiredLevel = 1, substituteMonster = Spider_Queen },
                                    new ShapeOptionDescription
                                    {
                                        requiredLevel = 1, substituteMonster = Sorr_Akkath_Shikkath
                                    },
                                    new ShapeOptionDescription
                                    {
                                        requiredLevel = 1, substituteMonster = Sorr_Akkath_Tshar_Boss
                                    }
                                ])
                            .Build())
                    .Build())
            .SetRequiresConcentration(true)
            .AddToDB();
    }

    #endregion

    #region Time Stop

    internal static SpellDefinition BuildTimeStop()
    {
        const string NAME = "TimeStop";

        var conditionTimeStop = ConditionDefinitionBuilder
            .Create("ConditionTimeStop")
            .SetGuiPresentation(Category.Condition, Sprites.GetSprite(NAME, Resources.ConditionTimeStop, 27, 32))
            .SetPossessive()
            .AddToDB();

        var conditionTimeStopEnemy = ConditionDefinitionBuilder
            .Create("ConditionTimeStopEnemy")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionPatronTimekeeperCurseOfTime)
            .SetConditionType(ConditionType.Detrimental)
            .SetPossessive()
            .SetFeatures(
                FeatureDefinitionActionAffinityBuilder
                    .Create($"ActionAffinity{NAME}")
                    .SetGuiPresentationNoContent(true)
                    .SetAllowedActionTypes(false, false, false, false, false, false)
                    .AddToDB())
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.TimeStop, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(9)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(false)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Divination)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Permanent)
                    .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Sphere, 12)
                    .ExcludeCaster()
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(conditionTimeStopEnemy),
                        EffectFormBuilder.ConditionForm(
                            conditionTimeStop,
                            ConditionForm.ConditionOperation.Add, true, true))
                    .SetParticleEffectParameters(DispelMagic)
                    .SetCasterEffectParameters(GravitySlam)
                    .Build())
            .AddCustomSubFeatures(new CustomBehaviorTimeStop())
            .AddToDB();

        conditionTimeStop.AddCustomSubFeatures(new ActionFinishedByMeTimeStop(spell));

        return spell;
    }

    private sealed class ActionFinishedByMeTimeStop(SpellDefinition spellDefinition)
        : IActionFinishedByMe, ICharacterBeforeTurnEndListener
    {
        // remove time stop if any action has non self target
        public IEnumerator OnActionFinishedByMe(CharacterAction action)
        {
            var character = action.ActingCharacter;
            var targets = action.ActionParams.TargetCharacters;
            var hasNonSelfTarget = targets == null || targets.Any(x => x != character);

            if (!hasNonSelfTarget ||
                (action is CharacterActionCastSpell actionCastSpell &&
                 actionCastSpell.ActiveSpell.SpellDefinition == spellDefinition))
            {
                yield break;
            }

            RemoveTimeStop(character);
        }

        // remove time stop after last instance turn
        public void OnCharacterBeforeTurnEnded(GameLocationCharacter locationCharacter)
        {
            var battle = Gui.Battle;

            if (battle == null)
            {
                return;
            }

            var index = battle.InitiativeSortedContenders.FindLastIndex(x => x.Guid == locationCharacter.Guid);

            if (battle.activeContenderIndex != index)
            {
                return;
            }

            RemoveTimeStop(locationCharacter);
        }

        private static void RemoveTimeStop(GameLocationCharacter locationCharacter)
        {
            var rulesetCharacter = locationCharacter.RulesetCharacter;
            var battle = Gui.Battle;

            // remove duplicates
            if (Gui.Battle != null)
            {
                while (battle.InitiativeSortedContenders.Count(x => x == locationCharacter) > 1)
                {
                    var index = battle.InitiativeSortedContenders.FindLastIndex(x => x.Guid == locationCharacter.Guid);

                    battle.InitiativeSortedContenders.RemoveAt(index);
                }

                Gui.Battle.ContenderModified(
                    locationCharacter, GameLocationBattle.ContenderModificationMode.Remove, false, false);
            }

            // remove time stop condition from others
            var locationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
            var contenders =
                (Gui.Battle?.AllContenders ??
                 locationCharacterService.PartyCharacters.Union(locationCharacterService.GuestCharacters))
                .ToArray();

            foreach (var rulesetContender in contenders
                         .Select(contender => contender.RulesetCharacter))
            {
                if (rulesetContender.TryGetConditionOfCategoryAndType(
                        AttributeDefinitions.TagEffect, "ConditionTimeStopEnemy", out var activeConditionEnemy) &&
                    activeConditionEnemy.SourceGuid == rulesetCharacter.Guid)
                {
                    rulesetContender.RemoveCondition(activeConditionEnemy);
                }
            }

            // remove time stop from self

            if (rulesetCharacter.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, "ConditionTimeStop", out var activeCondition))
            {
                rulesetCharacter.RemoveCondition(activeCondition);
            }
        }
    }

    private sealed class CustomBehaviorTimeStop : IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            if (Gui.Battle == null)
            {
                yield break;
            }

            var locationCharacter = action.ActingCharacter;
            var rulesetCharacter = locationCharacter.RulesetCharacter;
            var initiativeSortedContenders = Gui.Battle.InitiativeSortedContenders;
            var positionCharacter = initiativeSortedContenders.FirstOrDefault(x => x == locationCharacter);
            var positionCharacterIndex = initiativeSortedContenders.IndexOf(positionCharacter);
            var rounds = RollDie(DieType.D4, AdvantageType.None, out _, out _) + 1;

            rulesetCharacter.LogCharacterActivatesAbility(
                string.Empty,
                "Feedback/&TimeStop",
                extra:
                [
                    (ConsoleStyleDuplet.ParameterType.Base, rounds.ToString())
                ]);

            // add additional contenders to initiative
            for (var i = 0; i < rounds; i++)
            {
                initiativeSortedContenders.Insert(positionCharacterIndex + 1, locationCharacter);
            }

            Gui.Battle.ContenderModified(
                locationCharacter, GameLocationBattle.ContenderModificationMode.Add, false, false);
        }
    }

    #endregion
}
