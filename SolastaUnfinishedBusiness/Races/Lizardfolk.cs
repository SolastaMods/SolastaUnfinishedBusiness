using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using TA;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterRaceDefinitions;
using SolastaUnfinishedBusiness.Subclasses;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Interfaces;
using System.Linq;
using static ActionDefinitions;
using System.Collections;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using System;

namespace SolastaUnfinishedBusiness.Races;

internal static class RaceLizardfolkBuilder
{
    private const string Name = "Lizardfolk";
    private const string TagHungryJaws = "TagHungryJaws";
    internal static CharacterRaceDefinition RaceLizardfolk { get; } = BuildLizardfolk();
    internal static WeaponDescription hungryJawsWeaponDescription;

    [NotNull]
    private static CharacterRaceDefinition BuildLizardfolk()
    {
        // custom effects on hit
        hungryJawsWeaponDescription = CustomWeaponsContext.UnarmedStrikeClaws.WeaponDescription.DeepCopy();
        hungryJawsWeaponDescription.WeaponTypeDefinition.meleeImpactParticleReference =
            SpellDefinitions.InflictWounds.EffectDescription.EffectParticleParameters.effectParticleReference;

        var attributeModifierLizardfolkNaturalArmor = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}NaturalArmor")
            .SetModifier(AttributeModifierOperation.Set, AttributeDefinitions.ArmorClass, 13)
            .SetSituationalContext(SituationalContext.NotWearingArmor)
            .AddToDB();

        var pointPoolLizardfolkNaturesIntuition = FeatureDefinitionPointPoolBuilder
            .Create($"PointPool{Name}NaturesIntuition")
            .SetGuiPresentation(Category.Feature)
            .SetPool(HeroDefinitions.PointsPoolType.Skill, 2)
            .RestrictChoices(
                SkillDefinitions.AnimalHandling,
                SkillDefinitions.Medecine,
                SkillDefinitions.Survival,
                SkillDefinitions.Stealth,
                SkillDefinitions.Nature,
                SkillDefinitions.Perception)
            .AddToDB();

        var featureLizardfolkClaws = FeatureDefinitionBuilder
            .Create($"Feature{Name}Claws")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new CommonBuilders.AddExtraUnarmedStrikeClawAttack())
            .AddToDB();

        var powerLizardfolkHungryJaws = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}HungryJaws")
            .SetUsesProficiencyBonus(ActivationTime.BonusAction, RechargeRate.LongRest)
            .SetGuiPresentation(Category.Feature, Sprites.GetSprite(Name, Resources.PowerHungryJaws, 256, 128))
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Enemy, RangeType.Touch, 0, TargetType.IndividualsUnique)
                    .Build())
            .AddToDB();

        powerLizardfolkHungryJaws.AddCustomSubFeatures(
            new AttackAfterMagicEffectHungryJaws(),
            new PhysicalAttackFinishedByMeHungryJaws(powerLizardfolkHungryJaws));

        var racePresentation = Dragonborn.RacePresentation.DeepCopy();
        var raceLizardfolk = CharacterRaceDefinitionBuilder
            .Create(Human, $"Race{Name}")
            .SetGuiPresentation(Category.Race, Sprites.GetSprite(Name, Resources.Lizardfolk, 1024, 512))
            .SetSizeDefinition(CharacterSizeDefinitions.Medium)
            .SetRacePresentation(racePresentation)
            .SetBaseHeight(96)
            .SetBaseWeight(260)
            .SetMinimalAge(20)
            .SetMaximalAge(200)
            .SetFeaturesAtLevel(1,
                FeatureDefinitionSenses.SenseNormalVision,
                FeatureDefinitionSenses.SenseDarkvision,
                FlexibleRacesContext.AttributeChoiceThree.FeatureDefinition,
                FlexibleRacesContext.FeatureSetLanguageCommonPlusOne,
                FeatureDefinitionMoveModes.MoveModeMove6,
                attributeModifierLizardfolkNaturalArmor,
                pointPoolLizardfolkNaturesIntuition,
                featureLizardfolkClaws,
                powerLizardfolkHungryJaws
                )
            .AddToDB();

        RacesContext.RaceScaleMap[raceLizardfolk] = 5.4f / 6.4f;

        return raceLizardfolk;
    }


    private sealed class AttackAfterMagicEffectHungryJaws : IAttackAfterMagicEffect
    {
        public IAttackAfterMagicEffect.CanAttackHandler CanAttack { get; } =
            CanMeleeAttack;

        public IAttackAfterMagicEffect.GetAttackAfterUseHandler PerformAttackAfterUse { get; } =
            DefaultAttackHandler;

        public IAttackAfterMagicEffect.CanUseHandler CanBeUsedToAttack { get; } =
            DefaultCanUseHandler;

        private static bool CanMeleeAttack([NotNull] GameLocationCharacter caster, GameLocationCharacter target)
        {
            return true;
        }

        [CanBeNull]
        private static IEnumerable<CharacterActionParams> DefaultAttackHandler(
            [CanBeNull] CharacterActionMagicEffect actionMagicEffect)
        {
            var attacks = new List<CharacterActionParams>();
            var actionParams = actionMagicEffect?.ActionParams;

            if (actionParams == null)
            {
                return attacks;
            }

            var caster = actionParams.ActingCharacter;
            var targets = actionParams.TargetCharacters;
            var rulesetCharacter = caster.RulesetCharacter;

            if (targets.Count == 0)
            {
                return attacks;
            }
            RulesetAttackMode attackMode = null;
            var attackModifiers = rulesetCharacter is RulesetCharacterHero hero ?
                hero.attackModifiers : rulesetCharacter is RulesetCharacterMonster monster 
                ? monster.attackModifiers : new List<IAttackModificationProvider>();

            attackMode = rulesetCharacter.TryRefreshAttackMode(
                ActionType.NoCost,
                CustomWeaponsContext.UnarmedStrikeClaws,
                hungryJawsWeaponDescription,
                ValidatorsCharacter.IsFreeOffhandVanilla(rulesetCharacter),
                true,
                EquipmentDefinitions.SlotTypeOffHand,
                attackModifiers,
                rulesetCharacter.FeaturesOrigin
            );

            attackMode.HasPriority = true;

            
            //get copy to be sure we don't break existing mode
            var rulesetAttackModeCopy = RulesetAttackMode.AttackModesPool.Get();
            rulesetAttackModeCopy.Copy(attackMode);

            attackMode = rulesetAttackModeCopy;
            
            attackMode.AddAttackTagAsNeeded(TagHungryJaws);
            ApplyAttackModeModifiers(caster, attackMode);
            var maxAttacks = 1;

            var attackModifier = new ActionModifier();

            foreach (var target in targets.Where(t => CanMeleeAttack(caster, t)))
            {
                var attackActionParams =
                    new CharacterActionParams(caster, ActionDefinitions.Id.AttackFree) { AttackMode = attackMode };

                attackActionParams.TargetCharacters.Add(target);
                attackActionParams.ActionModifiers.Add(attackModifier);
                attacks.Add(attackActionParams);

                if (attackActionParams.TargetCharacters.Count >= maxAttacks)
                {
                    break;
                }
            }

            return attacks;
        }

        private static void ApplyAttackModeModifiers(GameLocationCharacter caster, RulesetAttackMode attackMode)
        {
            // for handling Pugilist bonus damage specifically
            if (caster.RulesetCharacter is not RulesetCharacterHero hero)
            {
                return;
            }

            var modifiers = hero.GetSubFeaturesByType<IModifyWeaponAttackMode>();

            var mods = modifiers;

            if (attackMode.sourceObject is RulesetItem item)
            {
                mods = item.GetSubFeaturesByType<IModifyWeaponAttackMode>();
                mods.AddRange(modifiers);
            }

            foreach (var modifier in mods)
            {
                modifier.ModifyAttackMode(hero, attackMode);
            }
        }

        private static bool DefaultCanUseHandler(
            [NotNull] CursorLocationSelectTarget targeting,
            GameLocationCharacter caster,
            GameLocationCharacter target, [NotNull] out string failure)
        {
            failure = string.Empty;

            return true;
        }
    }
    internal class PhysicalAttackFinishedByMeHungryJaws : IPhysicalAttackFinishedByMe
    {
        private FeatureDefinitionPower power;

        public PhysicalAttackFinishedByMeHungryJaws(FeatureDefinitionPower power)
        {
            this.power = power;
        }

        public IEnumerator OnPhysicalAttackFinishedByMe(GameLocationBattleManager battleManager, 
            CharacterAction action, 
            GameLocationCharacter attacker, 
            GameLocationCharacter defender, 
            RulesetAttackMode attackMode, 
            RollOutcome rollOutcome, 
            int damageAmount)
        {
            if (!attackMode.AttackTags.Contains(TagHungryJaws))
            {
                yield break;
            }

            if (action.AttackRollOutcome != RollOutcome.Success
                && action.AttackRollOutcome != RollOutcome.CriticalSuccess)
            {
                // regain charge if missed
                attacker.RulesetCharacter.UpdateUsageForPower(power, -1);
                yield break;
            }

            var bonus = attacker.RulesetCharacter.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
            attacker.RulesetCharacter.ReceiveTemporaryHitPoints(
                bonus, DurationType.UntilAnyRest, 0, TurnOccurenceType.StartOfTurn, attacker.RulesetCharacter.Guid);
            yield break;
        }
    }
}

