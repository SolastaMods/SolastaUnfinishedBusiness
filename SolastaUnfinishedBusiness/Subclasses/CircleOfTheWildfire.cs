#if false
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;
using static ActionDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Models.SpellsContext;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class CircleOfTheWildfire : AbstractSubclass
{
    private const string Name = "CircleOfTheWildfire";
    private const string ConditionCommandCannon = $"Condition{Name}Command";

    public CircleOfTheWildfire()
    {
        var autoPreparedSpellsWildfire = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{Name}")
            .SetGuiPresentation("ExpandedSpells", Category.Feature)
            .SetAutoTag("Circle")
            .SetPreparedSpellGroups(
                BuildSpellGroup(2, BurningHands, CureWounds),
                BuildSpellGroup(3, FlamingSphere, ScorchingRay),
                BuildSpellGroup(5, AshardalonStride, Revivify),
                BuildSpellGroup(7, AuraOfLife, FireShield),
                BuildSpellGroup(9, FlameStrike, MassCureWounds))
            .SetSpellcastingClass(CharacterClassDefinitions.Druid)
            .AddToDB();

        //
        // Summon Spirit
        //

        const string SpiritName = "WildfireSpirit";

        var powerSpiritTeleport = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}SpiritTeleport")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Action)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Distance, 3, TargetType.Position)
                    .InviteOptionalAlly()
                    .SetSavingThrowData(true, AttributeDefinitions.Wisdom, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetMotionForm(MotionForm.MotionType.TeleportToDestination, 1)
                            .Build())
                    .SetParticleEffectParameters(DimensionDoor)
                    .Build())
            .AddCustomSubFeatures(new ModifyTeleportEffectBehaviorSpiritTeleport())
            .AddToDB();

        powerSpiritTeleport.EffectDescription.EffectParticleParameters.targetParticleReference = new AssetReference();

        var actionAffinityEldritchCannon =
            FeatureDefinitionActionAffinityBuilder
                .Create($"ActionAffinity{Name}Spirit")
                .SetGuiPresentationNoContent(true)
                .SetForbiddenActions(
                    Id.AttackMain, Id.AttackOff, Id.AttackFree, Id.AttackReadied, Id.AttackOpportunity, Id.Ready,
                    Id.PowerMain, Id.PowerBonus, Id.PowerReaction, Id.SpendPower, Id.Shove, Id.ShoveBonus, Id.ShoveFree)
                .AddCustomSubFeatures(new SummonerHasConditionOrKOd())
                .AddToDB();

        var acBonus = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}ArmorClass")
            .SetGuiPresentation("Feedback/&BeastCompanionBonusTitle", Gui.NoLocalization)
            .SetAddConditionAmount(AttributeDefinitions.ArmorClass)
            .AddToDB();

        var toHit = FeatureDefinitionAttackModifierBuilder
            .Create($"AttackModifier{Name}AttackRoll")
            .SetGuiPresentation("Feedback/&BeastCompanionBonusTitle", Gui.NoLocalization)
            .SetAttackRollModifier(1, AttackModifierMethod.SourceConditionAmount)
            .AddToDB();

        var toDamage = FeatureDefinitionAttackModifierBuilder
            .Create($"AttackModifier{Name}DamageRoll")
            .SetGuiPresentation("Feedback/&BeastCompanionBonusTitle", Gui.NoLocalization)
            .SetDamageRollModifier(1, AttackModifierMethod.SourceConditionAmount)
            .AddToDB();

        var hpBonus = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}HitPoints")
            .SetGuiPresentation("Feedback/&BeastCompanionBonusTitle", Gui.NoLocalization)
            .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.AddConditionAmount,
                AttributeDefinitions.HitPoints)
            .AddToDB();

        var summoningAffinityBeastCompanion = FeatureDefinitionSummoningAffinityBuilder
            .Create($"SummoningAffinity{Name}BeastCompanion")
            .SetGuiPresentationNoContent(true)
            .SetRequiredMonsterTag(SpiritName)
            .SetAddedConditions(
                ConditionDefinitionBuilder
                    .Create($"Condition{Name}BeastCompanionArmorClass")
                    .SetGuiPresentation("Feedback/&BeastCompanionBonusTitle", Gui.NoLocalization)
                    .SetPossessive()
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetAmountOrigin(ExtraOriginOfAmount.SourceProficiencyAndAbilityBonus, AttributeDefinitions.Wisdom)
                    .SetFeatures(acBonus)
                    .AddToDB(),
                ConditionDefinitionBuilder
                    .Create($"Condition{Name}BeastCompanionAttackRoll")
                    .SetGuiPresentation("Feedback/&BeastCompanionBonusTitle", Gui.NoLocalization)
                    .SetPossessive()
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetAmountOrigin(ConditionDefinition.OriginOfAmount.SourceSpellAttack)
                    .SetFeatures(toHit)
                    .AddToDB(),
                ConditionDefinitionBuilder
                    .Create($"Condition{Name}BeastCompanionDamageRoll")
                    .SetGuiPresentation("Feedback/&BeastCompanionBonusTitle", Gui.NoLocalization)
                    .SetPossessive()
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetAmountOrigin(ExtraOriginOfAmount.SourceProficiencyBonus)
                    .SetFeatures(toDamage)
                    .AddToDB(),
                ConditionDefinitionBuilder
                    .Create($"Condition{Name}BeastCompanionHitPoints")
                    .SetGuiPresentation("Feedback/&BeastCompanionBonusTitle", Gui.NoLocalization)
                    .SetPossessive()
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetAmountOrigin(ExtraOriginOfAmount.SourceClassLevel, DruidClass)
                    .SetFeatures(hpBonus, hpBonus, hpBonus, hpBonus, hpBonus)
                    .AddToDB())
            .AddToDB();

        var monsterDefinitionSpirit = MonsterDefinitionBuilder
            .Create(MonsterDefinitions.Fire_Elemental, "WildfireSpirit")
            .SetOrUpdateGuiPresentation(Category.Monster)
            .SetSizeDefinition(CharacterSizeDefinitions.Small)
            .SetMonsterPresentation(
                MonsterPresentationBuilder
                    .Create()
                    .SetAllPrefab(MonsterDefinitions.Fire_Elemental.MonsterPresentation)
                    .SetPhantom()
                    .SetModelScale(0.5f)
                    .SetHasMonsterPortraitBackground(true)
                    .SetCanGeneratePortrait(true)
                    .Build())
            .SetCreatureTags(SpiritName)
            .SetStandardHitPoints(1)
            .SetHeight(4)
            .NoExperienceGain()
            .SetArmorClass(13)
            .SetChallengeRating(0)
            .SetHitDice(DieType.D8, 1)
            .SetAbilityScores(10, 14, 14, 13, 15, 11)
            .SetDefaultFaction(FactionDefinitions.Party)
            .SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.None)
            .SetFullyControlledWhenAllied(true)
            .SetDungeonMakerPresence(MonsterDefinition.DungeonMaker.None)
            .ClearAttackIterations()
            .SetFeatures(
                actionAffinityEldritchCannon,
                powerSpiritTeleport,
                FeatureDefinitionMoveModes.MoveModeMove6,
                FeatureDefinitionMoveModes.MoveModeFly6,
                FeatureDefinitionDamageAffinitys.DamageAffinityFireImmunity,
                FeatureDefinitionConditionAffinitys.ConditionAffinityCharmImmunity,
                FeatureDefinitionConditionAffinitys.ConditionAffinityFrightenedImmunity,
                FeatureDefinitionConditionAffinitys.ConditionAffinityProneImmunity,
                FeatureDefinitionConditionAffinitys.ConditionAffinityRestrainedmmunity,
                FeatureDefinitionSenses.SenseDarkvision)
            .AddToDB();

        // Command Spirit

        var conditionCommandSpirit = ConditionDefinitionBuilder
            .Create(ConditionCommandCannon)
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        var powerCommandSpirit = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}CommandSpirit")
            .SetGuiPresentation(Category.Feature, Command)
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionCommandSpirit, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(ValidatorsValidatePowerUse.InCombat, new ValidatorsValidatePowerUse(HasSpirit))
            .AddToDB();

        powerCommandSpirit.AddCustomSubFeatures(
            new CharacterBeforeTurnEndListenerCommandSpirit(
                conditionCommandSpirit,
                powerCommandSpirit));

        // Summon Spirit

        var powerSummonSpirit = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"PowerSharedPool{Name}SummonSpirit")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.Action, PowerDruidWildShape)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.Position)
                    .SetDurationData(DurationType.HalfClassLevelHours)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetSummonCreatureForm(1, monsterDefinitionSpirit.Name)
                            .Build())
                    .SetParticleEffectParameters(PowerDruidWildShape)
                    .Build())
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.PatronElementalist, 256))
            .AddFeaturesAtLevel(2, autoPreparedSpellsWildfire, powerSummonSpirit, summoningAffinityBeastCompanion)
            .AddFeaturesAtLevel(6)
            .AddFeaturesAtLevel(10)
            .AddFeaturesAtLevel(14)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Druid;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceDruidCircle;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private static bool HasSpirit(RulesetCharacter character)
    {
        return ValidatorsCharacter.HasAnyOfConditions($"Condition{Name}SpiritSelf")(character);
    }

    private sealed class SummonerHasConditionOrKOd : IValidateDefinitionApplication, ICharacterTurnStartListener
    {
        public void OnCharacterTurnStarted(GameLocationCharacter locationCharacter)
        {
            // if commanded allow anything
            if (IsCommanded(locationCharacter.RulesetCharacter))
            {
                return;
            }

            // if not commanded it cannot move
            locationCharacter.usedTacticalMoves = locationCharacter.MaxTacticalMoves;

            // or use powers so force the dodge action
            ServiceRepository.GetService<ICommandService>()?
                .ExecuteAction(new CharacterActionParams(locationCharacter, Id.Dodge), null, false);
        }

        public bool IsValid(BaseDefinition definition, RulesetCharacter character)
        {
            //Apply limits if not commanded
            return !IsCommanded(character);
        }

        private static bool IsCommanded(RulesetCharacter character)
        {
            //can act freely outside of battle
            if (Gui.Battle == null)
            {
                return true;
            }

            var summoner = character.GetMySummoner()?.RulesetCharacter;

            //shouldn't happen, but consider being commanded in this case
            if (summoner == null)
            {
                return true;
            }

            //can act if summoner is KO
            return summoner.IsUnconscious ||
                   //can act if summoner commanded
                   summoner.HasConditionOfType(ConditionCommandCannon);
        }
    }

    // Command Spirit

    private sealed class CharacterBeforeTurnEndListenerCommandSpirit(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionEldritchCannonCommand,
        FeatureDefinitionPower power) : ICharacterBeforeTurnEndListener
    {
        public void OnCharacterBeforeTurnEnded(GameLocationCharacter locationCharacter)
        {
            var status = locationCharacter.GetActionStatus(Id.PowerBonus, ActionScope.Battle);

            if (status != ActionStatus.Available ||
                !HasSpirit(locationCharacter.RulesetCharacter))
            {
                return;
            }

            var rulesetCharacter = locationCharacter.RulesetCharacter;

            rulesetCharacter.LogCharacterUsedPower(power);
            rulesetCharacter.InflictCondition(
                conditionEldritchCannonCommand.Name,
                DurationType.Round,
                1,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                conditionEldritchCannonCommand.Name,
                0,
                0,
                0);
        }
    }

    private sealed class ModifyTeleportEffectBehaviorSpiritTeleport : IModifyTeleportEffectBehavior
    {
        public bool AllyOnly => true;

        public bool TeleportSelf => true;

        public int MaxTargets => 8;
    }
}
#endif
