using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using UnityEngine;

using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class DomainOrder : AbstractSubclass
{
    public DomainOrder()
    {
        const string NAME = "DomainOrder";

        // LEVEL 01

        // Auto Prepared Spells
        var autoPreparedSpellsDomainOrder = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{NAME}")
            .SetGuiPresentation("ExpandedSpells", Category.Feature)
            .SetAutoTag("Domain")
            .SetPreparedSpellGroups(
                BuildSpellGroup(1, SpellsContext.Command, Heroism),
                BuildSpellGroup(3, HoldPerson, SpellsContext.PsychicWhip),
                BuildSpellGroup(5, MassHealingWord, Slow),
                BuildSpellGroup(7, Confusion, SpellsContext.PsychicLance),
                BuildSpellGroup(9, HoldMonster, DominatePerson))
            .SetSpellcastingClass(CharacterClassDefinitions.Cleric)
            .AddToDB();

        // Heavy Armor Proficiency
        var bonusProficiencyArmorDomainOrder = FeatureDefinitionProficiencyBuilder
            .Create($"BonusProficiency{NAME}")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.Armor, EquipmentDefinitions.HeavyArmorCategory)
            .AddToDB();

        var pointPoolSkills = FeatureDefinitionPointPoolBuilder
            .Create($"PointPool{NAME}Skills")
            .SetGuiPresentationNoContent(true)
            .SetPool(HeroDefinitions.PointsPoolType.Skill, 1)
            .RestrictChoices(
                SkillDefinitions.Intimidation,
                SkillDefinitions.Persuasion)
            .AddToDB();

        var featureSetBonusProficiencies = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{NAME}BonusProficiencies")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(bonusProficiencyArmorDomainOrder, pointPoolSkills)
            .AddToDB();

        // Voice of Authority
        // mark ally targets of your spells that use spell slots until the end of your turn
        // then add a NoCost power to cause a marked creature to make a weapon attack vs a target it can see
        var conditionVoiceOfAuthority = ConditionDefinitionBuilder
            .Create($"Condition{NAME}VoiceOfAuthority")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionDeafened)
            .SetConditionType(ConditionType.Beneficial)
            .SetSpecialDuration(DurationType.Round)
            .SetSilent(Silent.WhenRemoved)
            .AddToDB();

        var powerVoiceOfAuthority = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}VoiceOfAuthority")
            .SetGuiPresentation(Category.Feature)
            .SetShowCasting(false)
            .SetUsesFixed(ActivationTime.OnPowerActivatedAuto)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetDurationData(DurationType.Round)
                .SetTargetingData(Side.All, RangeType.Distance, 48, TargetType.IndividualsUnique)
                .SetNoSavingThrow()
                .SetIgnoreCover()
                .SetCasterEffectParameters(SpellsContext.DissonantWhispers)
                .SetEffectEffectParameters(SpellsContext.DissonantWhispers)
                .SetEffectForms(EffectFormBuilder.ConditionForm(conditionVoiceOfAuthority))
                .Build())
            .AddToDB();

        powerVoiceOfAuthority.AddCustomSubFeatures(
            new VoiceOfAuthorityHandler(powerVoiceOfAuthority));

        var powerVoiceOfAuthorityCompelAttack = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}VoiceOfAuthorityCompelAttack")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite(
                    "PowerInfectiousFuryCompelledStrike",
                    Properties.Resources.PowerInfectiousFuryCompelledStrike,
                    256,
                    128))
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.TurnStart)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Distance, 48, TargetType.IndividualsUnique, 2)
                    .Build())
            .AddToDB();

        powerVoiceOfAuthorityCompelAttack.AddCustomSubFeatures(
            new VoiceOfAuthorityCompelAttackHandler(conditionVoiceOfAuthority));


        // LEVEL 02
        var divinePowerPrefix = Gui.Localize("Feature/&ClericChannelDivinityTitle") + ": ";

        var powerOrdersDemand = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}OrdersDemand")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ChannelDivinity)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 6)
                    .SetSavingThrowData(
                        false,
                        AttributeDefinitions.Wisdom,
                        true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(ConditionDefinitions.ConditionCharmed,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .SetCasterEffectParameters(Fear)
                    .ExcludeCaster()
                    .Build())
            .AddToDB();

        var featureSetOrdersDemand = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{NAME}OrdersDemand")
            .SetGuiPresentation(
                divinePowerPrefix + powerOrdersDemand.FormatTitle(), powerOrdersDemand.FormatDescription())
            .AddFeatureSet(powerOrdersDemand)
            .AddToDB();


        // LEVEL 06 Embodiment of the Law
        // P.B./day cast Enchantment spell as a B.A.



        // LEVEL 08 Divine Strike
        // LEVEL 14 Divine Strike +1d8
        var additionalDamageDivineStrike = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{NAME}DivineStrike")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag("DivineStrike")
            .SetDamageDice(DieType.D8, 1)
            .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 1, 8, 6)
            .SetSpecificDamageType(DamageTypePsychic)
            .SetFrequencyLimit(FeatureLimitedUsage.OnceInMyTurn)
            .SetAttackModeOnly()
            .SetImpactParticleReference(ViciousMockery)
            .AddToDB();


        // LEVEL 17 Order's Wrath
        // if Divine Strike a creature, can curse until start of your next turn
        // first time it's hurt before then, +2d8 psychic damage


        Subclass = CharacterSubclassDefinitionBuilder
            .Create(NAME)
            //TODO: Get an image for the Order
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(NAME, Properties.Resources.DomainSmith, 256))
            .AddFeaturesAtLevel(1,
                autoPreparedSpellsDomainOrder,
                featureSetBonusProficiencies,
                powerVoiceOfAuthority,
                powerVoiceOfAuthorityCompelAttack)
            .AddFeaturesAtLevel(2,
                featureSetOrdersDemand)
            //.AddFeaturesAtLevel(6, null)
            .AddFeaturesAtLevel(8, additionalDamageDivineStrike)
            //.AddFeaturesAtLevel(17, null)
            .AddFeaturesAtLevel(20, Level20SubclassesContext.PowerClericDivineInterventionImprovementPaladin)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => Cleric;

    internal override CharacterSubclassDefinition Subclass { get; }

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override FeatureDefinitionSubclassChoice SubclassChoice { get; }

    internal override DeityDefinition DeityDefinition => DeityDefinitions.Pakri;

    private sealed class VoiceOfAuthorityHandler(FeatureDefinitionPower powerVoiceOfAuthority)
        : IMagicEffectFinishedByMe
    {

        public IEnumerator OnMagicEffectFinishedByMe(CharacterAction action,
            GameLocationCharacter attacker,
            List<GameLocationCharacter> targets)
        {
            if (action is not CharacterActionCastSpell actionCastSpell ||
                actionCastSpell.Countered ||
                actionCastSpell.ExecutionFailed ||
                actionCastSpell.ActiveSpell.SlotLevel == 0 ||
                !attacker.IsMyTurn())
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;
            var allies = targets.FindAll(t => t.Side == Side.Ally && 
                    t.RulesetCharacter is { isDeadOrDyingOrUnconscious: false });

            if (allies.Empty()) { yield break; }

            var usablePower = PowerProvider.Get(powerVoiceOfAuthority, rulesetAttacker);

            var battleManager = ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;
            if (!battleManager || battleManager is { IsBattleInProgress: false })
            {
                yield break;
            }

            attacker.MyExecuteActionPowerNoCost(usablePower, [.. targets]);
            /*
            yield return attacker.MyReactToUsePower(
                ActionDefinitions.Id.PowerNoCost,
                usablePower,
                targets,
                attacker,
                "PowerVoiceOfAuthority",
                battleManager: battleManager);
            */
        }
    }

    private class VoiceOfAuthorityCompelAttackHandler(ConditionDefinition condition)
        : IFilterTargetingCharacter, IPowerOrSpellFinishedByMe, IValidatePowerUse
    {
        public bool EnforceFullSelection => true;

        public bool CanUsePower(RulesetCharacter character, FeatureDefinitionPower power)
        {
            var glcService = ServiceRepository.GetService<IGameLocationCharacterService>();
            var battleManager = ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;
            if (!battleManager)
            {
                return false;
            }

            return PowerProvider.Get(power, character).RemainingUses > 0 &&
                battleManager.IsBattleInProgress &&
                glcService.PartyCharacters.Any(c =>
                    c.RulesetCharacter.HasConditionOfCategoryAndType(AttributeDefinitions.TagEffect, condition.Name));
        }

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            var selectedTargets = __instance.SelectionService.SelectedTargets;

            if (selectedTargets.Count == 0)
            {
                if (!target.RulesetCharacter.HasConditionOfType(condition))
                {
                    __instance.actionModifier.FailureFlags.Add("Failure/&TargetMustHaveVoiceOfAuthority");

                    return false;
                }

                // ReSharper disable once InvertIf
                if (!target.CanReact())
                {
                    __instance.actionModifier.FailureFlags.Add("Failure/&AllyMustBeAbleToReact");

                    return false;
                }

                return true;
            }

            var selectedTarget = selectedTargets[0];
            var attackMode = selectedTarget.FindActionAttackMode(ActionDefinitions.Id.AttackMain);

            // ReSharper disable once InvertIf
            if (attackMode == null ||
                !IsValidAttack(__instance, attackMode, selectedTarget, target))
            {
                __instance.actionModifier.FailureFlags.Add("Failure/&MustBeAbleToAttackTarget");

                return false;
            }

            return true;
        }

        private static bool IsValidAttack(
            CursorLocationSelectTarget __instance,
            RulesetAttackMode attackMode,
            GameLocationCharacter selectedCharacter,
            GameLocationCharacter targetedCharacter)
        {
            __instance.predictivePosition = selectedCharacter.LocationPosition;

            var attackParams1 = new BattleDefinitions.AttackEvaluationParams();

            attackParams1.FillForPhysicalReachAttack(selectedCharacter, __instance.predictivePosition, attackMode,
                targetedCharacter, targetedCharacter.LocationPosition, __instance.actionModifier);

            if (__instance.BattleService.CanAttack(attackParams1))
            {
                return true;
            }

            var attackParams2 = new BattleDefinitions.AttackEvaluationParams();

            attackParams2.FillForPhysicalRangeAttack(selectedCharacter, __instance.predictivePosition, attackMode,
                targetedCharacter, targetedCharacter.LocationPosition, __instance.actionModifier);

            return __instance.BattleService.CanAttack(attackParams2);
        }

        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var targetCharacters = action.ActionParams.TargetCharacters;
            var attacker = targetCharacters[0];
            var defender = targetCharacters[1];

            attacker.RulesetCharacter.RemoveAllConditionsOfCategoryAndType(
                AttributeDefinitions.TagEffect, condition.name);

            var attackMode = attacker.FindActionAttackMode(ActionDefinitions.Id.AttackMain);

            attacker.MyExecuteActionAttack(
                ActionDefinitions.Id.AttackOpportunity,
                defender,
                attackMode,
                new ActionModifier());

            yield break;
        }
    }
}
