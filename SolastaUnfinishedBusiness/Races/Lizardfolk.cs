using System.Collections;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Subclasses;
using SolastaUnfinishedBusiness.Validators;
using static ActionDefinitions;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterRaceDefinitions;

namespace SolastaUnfinishedBusiness.Races;

internal static class RaceLizardfolkBuilder
{
    private const string Name = "Lizardfolk";
    private const string TagHungryJaws = "TagHungryJaws";

    internal static CharacterRaceDefinition RaceLizardfolk { get; } = BuildLizardfolk();

    [NotNull]
    private static CharacterRaceDefinition BuildLizardfolk()
    {
        var attributeModifierLizardfolkNaturalArmor = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}NaturalArmor")
            .SetGuiPresentation(Category.Feature)
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
            .SetGuiPresentation(Category.Feature, Sprites.GetSprite(Name, Resources.PowerHungryJaws, 256, 128))
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Touch, 0, TargetType.IndividualsUnique)
                    .Build())
            .AddToDB();

        powerLizardfolkHungryJaws.AddCustomSubFeatures(
            new PowerOrSpellFinishedByMeHungryJaws(),
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


    private sealed class PowerOrSpellFinishedByMeHungryJaws : IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            if (Gui.Battle == null)
            {
                yield break;
            }

            var actingCharacter = action.ActingCharacter;
            var targets = action.ActionParams.TargetCharacters
                .Where(x =>
                    x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false })
                .ToArray();

            if (targets.Length == 0)
            {
                yield break;
            }

            var rulesetCharacter = actingCharacter.RulesetCharacter;
            var attackModifiers = rulesetCharacter switch
            {
                RulesetCharacterHero hero => hero.attackModifiers,
                RulesetCharacterMonster monster => monster.attackModifiers,
                _ => []
            };

            var attackMode = rulesetCharacter.TryRefreshAttackMode(
                ActionType.NoCost,
                CustomWeaponsContext.UnarmedStrikeClaws,
                CustomWeaponsContext.UnarmedStrikeClaws.WeaponDescription,
                ValidatorsCharacter.IsFreeOffhandVanilla(rulesetCharacter),
                true,
                EquipmentDefinitions.SlotTypeOffHand,
                attackModifiers,
                rulesetCharacter.FeaturesOrigin
            );

            attackMode.HasPriority = true;
            attackMode.AddAttackTagAsNeeded(TagHungryJaws);

            foreach (var target in targets)
            {
                actingCharacter.MyExecuteActionAttack(
                    Id.AttackFree,
                    target,
                    attackMode,
                    new ActionModifier());
            }
        }
    }

    private class PhysicalAttackFinishedByMeHungryJaws(FeatureDefinitionPower powerHungryJaws)
        : IPhysicalAttackFinishedByMe
    {
        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
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

            // regain charge if missed
            if (action.AttackRollOutcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess))
            {
                attacker.RulesetCharacter.UpdateUsageForPower(powerHungryJaws, -1);

                yield break;
            }

            var bonus = attacker.RulesetCharacter.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);

            attacker.RulesetCharacter.ReceiveTemporaryHitPoints(
                bonus, DurationType.UntilAnyRest, 0, TurnOccurenceType.StartOfTurn, attacker.RulesetCharacter.Guid);
            EffectHelpers.StartVisualEffect(
                attacker, attacker, SpellDefinitions.CureWounds, EffectHelpers.EffectType.Effect);
            EffectHelpers.StartVisualEffect(
                defender, defender, SpellDefinitions.InflictWounds, EffectHelpers.EffectType.Effect);
        }
    }
}
