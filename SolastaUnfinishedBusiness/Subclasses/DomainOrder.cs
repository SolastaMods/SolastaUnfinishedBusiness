using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Validators;

using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class DomainOrder : AbstractSubclass
{
    private static string NAME = "DomainOrder";

    private static FeatureDefinitionAdditionalDamage DivineStrike = FeatureDefinitionAdditionalDamageBuilder
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

    private static FeatureDefinitionPower EmbodimentOfLaw = FeatureDefinitionPowerBuilder
        .Create($"Power{NAME}EmbodimentOfLaw")
        .SetGuiPresentation(Category.Feature, FeatureDefinitionPowers.PowerDomainLawWordOfLaw)
        .SetUsesAbilityBonus(
            ActivationTime.NoCost, RechargeRate.LongRest, AttributeDefinitions.Wisdom)
        .SetShowCasting(false)
        .AddToDB();

    private static MetamagicOptionDefinition EmbodimentMetamagic = MetamagicOptionDefinitionBuilder
        .Create(MetamagicOptionDefinitions.MetamagicQuickenedSpell, "MetamagicEmbodimentOfTheLaw")
        .SetGuiPresentation(Category.Feature)
        .SetType(MetamagicType.QuickenedSpell)
        .SetCost(MetamagicCostMethod.FixedValue, 0)
        .AddCustomSubFeatures(
            new ValidateMetamagicApplication(
                (RulesetCharacter rulesetCharacter,
                    RulesetEffectSpell spell,
                    MetamagicOptionDefinition _,
                    ref bool result,
                    ref string failure) =>
                {
                    if (spell.ActionType == ActionDefinitions.ActionType.Main &&
                    spell.SchoolOfMagic == SchoolOfMagicDefinitions.SchoolEnchantment.Name &&
                    rulesetCharacter.CanUsePower(EmbodimentOfLaw))
                    {
                        return;
                    }

                    failure = spell.SchoolOfMagic == SchoolOfMagicDefinitions.SchoolEnchantment.Name ?
                    "Failure/&FailureFlagNoPowerUses" :
                        Gui.Format("Failure/&FailureFlagInvalidSpellSchool", spell.SchoolOfMagic.Remove(0,6));
                    result = false;
                }))
        .AddToDB();

    public DomainOrder()
    {
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
            .SetSpellcastingClass(Cleric)
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
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionFeatTakeAim)
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
                .SetTargetingData(Side.All, RangeType.Distance, 24, TargetType.IndividualsUnique)
                .SetNoSavingThrow()
                .SetIgnoreCover()
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
                    .SetTargetingData(Side.All, RangeType.Distance, 24, TargetType.IndividualsUnique, 2)
                    .Build())
            .AddToDB();

        powerVoiceOfAuthorityCompelAttack.AddCustomSubFeatures(
            new VoiceOfAuthorityCompelAttackHandler(conditionVoiceOfAuthority));


        // LEVEL 02
        // Channel Divinity: Order's Demand
        var divinePowerPrefix = Gui.Localize("Feature/&ClericChannelDivinityTitle") + ": ";

        var powerOrdersDemand = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}OrdersDemand")
            .SetGuiPresentation(Category.Feature,
                FeatureDefinitionPowers.PowerBerserkerIntimidatingPresence.GuiPresentation.SpriteReference)
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
        EmbodimentOfLaw.AddCustomSubFeatures(
            new EmbodimentOfLawCustomBehaviour(EmbodimentOfLaw, EmbodimentMetamagic));

        var metamagicToggle = FeatureDefinitionActionAffinitys.ActionAffinitySorcererMetamagicToggle;

        // Feature required to get the Metamagic panel to show up for EmbodimentMetamagic
        var sorceryPoints = FeatureDefinitionAttributeModifierBuilder
                    .Create(FeatureDefinitionAttributeModifiers.AttributeModifierSorcererSorceryPointsBase,
                        $"AttributeModifier{NAME}SorceryPointsMinimum")
                    .SetGuiPresentationNoContent(true)
                    .SetModifier(
                        FeatureDefinitionAttributeModifier.AttributeModifierOperation.ForceIfBetter,
                        AttributeDefinitions.SorceryPoints, 1)
                    .AddToDB();

        // LEVEL 08 Divine Strike
        // LEVEL 14 Divine Strike +1d8

        // LEVEL 17 Order's Wrath
        // if Divine Strike a creature, can curse until start of your next turn
        // first time it's hurt before then, +2d8 psychic damage
        var conditionCursedByOrdersWrath = ConditionDefinitionBuilder
            .Create("ConditionCursedByOrdersWrath")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionFrightened)
            .SetConditionType(ConditionType.Detrimental)
            .SetSpecialDuration(DurationType.Round, 1, (TurnOccurenceType)ExtraTurnOccurenceType.StartOfSourceTurn)
            .SetSilent(Silent.None)
            .SetPossessive()
            .SetConditionParticleReference(BestowCurse)
            .AddToDB();

        var additionalDamageOrdersWrath = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{NAME}OrdersWrath")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag($"{NAME}OrdersWrath")
            .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
            .SetDamageValueDetermination(AdditionalDamageValueDetermination.Die)
            .SetDamageDice(DieType.D8, 2)
            .SetSpecificDamageType(DamageTypePsychic)
            .SetTargetCondition(
                conditionCursedByOrdersWrath, AdditionalDamageTriggerCondition.TargetHasCondition)
            .AddConditionOperation(
                ConditionOperationDescription.ConditionOperation.Remove, conditionCursedByOrdersWrath)
            .SetAttackOnly()
            .AddToDB();

        var conditionApplyAdditionalDamage = ConditionDefinitionBuilder
            .Create("conditionApplyAdditionalDamageOrdersWrath")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialDuration(DurationType.Round, 0, TurnOccurenceType.EndOfTurn)
            .SetSpecialInterruptions(ConditionInterruption.AttacksAndDamages)
            .AddFeatures(additionalDamageOrdersWrath)
            .AddToDB();

        conditionCursedByOrdersWrath.AddCustomSubFeatures(
            new OrdersWrathAdditionalDamageHandler(conditionCursedByOrdersWrath, conditionApplyAdditionalDamage));

        
        var description = Gui.Format($"Feature/&Power{NAME}OrdersWrathDescription",
            Main.Settings.EnableClericBlessedStrikes2024 ? "Blessed Strikes" : "Divine Strike");
        var powerOrdersWrath = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}OrdersWrath")
            .SetGuiPresentation(Category.Feature, description)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.TurnStart)
            .AddToDB();

        powerOrdersWrath.AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden,
            ValidatorsRestrictedContext.IsWeaponOrUnarmedAttack,
            new OrdersWrathApplyOnDivineStrike(
                conditionCursedByOrdersWrath, powerOrdersWrath));


        Subclass = CharacterSubclassDefinitionBuilder
            .Create(NAME)
            //TODO: Get an image for the Order
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(NAME, Properties.Resources.DomainSmith, 256))
            .AddFeaturesAtLevel(1,
                autoPreparedSpellsDomainOrder,
                featureSetBonusProficiencies,
                powerVoiceOfAuthority,
                powerVoiceOfAuthorityCompelAttack)
            .AddFeaturesAtLevel(2, featureSetOrdersDemand)
            .AddFeaturesAtLevel(6, EmbodimentOfLaw, metamagicToggle, sorceryPoints)
            .AddFeaturesAtLevel(8, DivineStrike)
            .AddFeaturesAtLevel(17, powerOrdersWrath)
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
            var allies = targets.FindAll(t => t.Side == Side.Ally && t != attacker &&
                    t.RulesetCharacter is { isDeadOrDyingOrUnconscious: false });

            if (allies.Empty()) { yield break; }

            var usablePower = PowerProvider.Get(powerVoiceOfAuthority, rulesetAttacker);

            var battleManager = ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;
            if (!battleManager || battleManager is { IsBattleInProgress: false })
            {
                yield break;
            }

            attacker.MyExecuteActionPowerNoCost(usablePower, [.. targets]);
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

        public IEnumerator OnPowerOrSpellFinishedByMe(
            CharacterActionMagicEffect action, BaseDefinition baseDefinition)
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

    private sealed class EmbodimentOfLawCustomBehaviour(
        FeatureDefinitionPower power,
        MetamagicOptionDefinition metamagic
        ) : ICustomLevelUpLogic, IMagicEffectInitiatedByMe, IPowerOrSpellFinishedByMe
    {
        public void ApplyFeature(RulesetCharacterHero hero, [UsedImplicitly] string tag)
        {
            if (!hero.trainedMetamagicOptions.Contains(metamagic))
            {
                hero.trainedMetamagicOptions.Add(metamagic);
                hero.trainedMetamagicOptions.Sort((x, y) => x.Name.CompareTo(y.Name));
            }
            return;
        }
        public void RemoveFeature(RulesetCharacterHero hero, [UsedImplicitly] string tag)
        {
            hero.trainedMetamagicOptions.Remove(metamagic);
            return;
        }

        public IEnumerator OnMagicEffectInitiatedByMe(
            CharacterAction action,
            RulesetEffect activeEffect,
            GameLocationCharacter attacker,
            List<GameLocationCharacter> targets)
        {
            if (activeEffect.MetamagicOption == metamagic)
            {
                // Expend if you used it for a spell
                attacker.RulesetCharacter.UpdateUsageForPower(power, 1);
            }

            yield break;
        }

        public IEnumerator OnPowerOrSpellFinishedByMe(
            CharacterActionMagicEffect action,
            BaseDefinition baseDefinition)
        {
            if (baseDefinition.Name == power.Name)
            {
                // Refund if you activated it through the power menu
                action.ActingCharacter.RulesetCharacter.UpdateUsageForPower(power, -1);
            }

            yield break;
        }
    }

    private sealed class OrdersWrathApplyOnDivineStrike(
        ConditionDefinition conditionOrdersWrath,
        FeatureDefinitionPower powerOrdersWrath)
        : IMagicEffectFinishedByMe, IPhysicalAttackFinishedByMe
    {
        // Allow a character that took the "Blessed Strikes: Potent Spellcasting" option to curse, too
        public IEnumerator OnMagicEffectFinishedByMe(
            CharacterAction action,
            GameLocationCharacter attacker,
            List<GameLocationCharacter> targets)
        {
            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetTarget = targets.Find(t => t.RulesetCharacter is not null && t.Side != rulesetAttacker.Side)?
                                       .RulesetCharacter; // First enemy target

            if (action is CharacterActionCastSpell castSpell &&
                castSpell.ActiveSpell.SlotLevel == 0 &&
                castSpell.ActiveSpell.spellRepertoire?.SpellCastingClass == Cleric &&
                rulesetTarget is not null &&
                action is { SaveOutcome: RollOutcome.Failure or RollOutcome.CriticalFailure } &&
                rulesetAttacker.CanUsePower(powerOrdersWrath) &&
                rulesetAttacker.HasAnyFeature(
                    GetDefinition<FeatureDefinition>("FeatureClericBlessedStrikesPotentSpellcasting")))
            {
                var activeCondition = RulesetCondition.CreateCondition(
                    rulesetTarget.Guid,
                    conditionOrdersWrath);
                activeCondition.sourceGuid = attacker.Guid; // to track turns for ending the condition

                rulesetTarget.AddConditionOfCategory(AttributeDefinitions.TagEffect, activeCondition);

                rulesetAttacker.UpdateUsageForPower(powerOrdersWrath, 1); // Expend our use for this turn
            }

            yield break;
        }

        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetAttacker is null || rulesetDefender is null)
            {
                yield break;
            }

            // If the character has used their Divine Strike or "Blessed Strike: Primal Strike" on this hit
            bool canCurse = attacker.UsedSpecialFeatures.ContainsKey(DivineStrike.Name) ||
                            attacker.UsedSpecialFeatures.ContainsKey(Tabletop2024Context.BlessedStrikes);

            // If attack was with a weapon, we haven't used this power yet this turn, and meet the criteria above
            if (attackMode.SourceObject is RulesetItem &&
                rulesetAttacker.CanUsePower(powerOrdersWrath) &&
                canCurse)
            {
                var activeCondition = RulesetCondition.CreateCondition(defender.Guid, conditionOrdersWrath);
                activeCondition.sourceGuid = attacker.Guid; // to track turns for ending the condition

                rulesetDefender.AddConditionOfCategory(AttributeDefinitions.TagEffect, activeCondition);

                rulesetAttacker.UpdateUsageForPower(powerOrdersWrath, 1); // Expend our use for this turn
            }

            yield break;
        }
    }

    private sealed class OrdersWrathAdditionalDamageHandler(
        ConditionDefinition conditionOrdersWrath,
        ConditionDefinition conditionForAttacker)
        : IMagicEffectBeforeHitConfirmedOnMe,IPhysicalAttackBeforeHitConfirmedOnMe
    {
        public IEnumerator OnMagicEffectBeforeHitConfirmedOnMe(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetDefender = defender.RulesetCharacter;
            
            if (rulesetAttacker is null ||
                rulesetDefender is null ||
                !rulesetEffect.EffectDescription.NeedsToRollDie() || // Only triggered on attacks
                !rulesetDefender.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionOrdersWrath.Name, out var activeCondition) ||
                rulesetAttacker.Guid == activeCondition.SourceGuid || // Only triggered by allies
                rulesetAttacker.Side != EffectHelpers.GetCharacterByGuid(activeCondition.SourceGuid).Side)
            {
                yield break;
            }

            rulesetAttacker.AddConditionOfCategory(AttributeDefinitions.TagEffect,
                RulesetCondition.CreateCondition(rulesetDefender.Guid, conditionForAttacker));

            yield break;
        }

        public IEnumerator OnPhysicalAttackBeforeHitConfirmedOnMe(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetAttacker is null ||
                rulesetDefender is null ||
                !rulesetDefender.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionOrdersWrath.Name, out var activeCondition) ||
                rulesetAttacker.Guid == activeCondition.SourceGuid || // Only triggered by allies
                rulesetAttacker.Side != EffectHelpers.GetCharacterByGuid(activeCondition.SourceGuid).Side)
            {
                yield break;
            }

            rulesetAttacker.AddConditionOfCategory(AttributeDefinitions.TagEffect,
                RulesetCondition.CreateCondition(rulesetDefender.Guid, conditionForAttacker));

            yield break;
        }
    }
}
