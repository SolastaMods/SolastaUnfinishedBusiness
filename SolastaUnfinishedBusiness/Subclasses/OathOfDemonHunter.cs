using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.WeaponTypeDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAdditionalDamages;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class OathOfDemonHunter : AbstractSubclass
{
    private const string Name = "OathOfDemonHunter";
    
    internal override CharacterSubclassDefinition Subclass { get; }
    internal override FeatureDefinitionSubclassChoice SubclassChoice => FeatureDefinitionSubclassChoices
        .SubclassChoicePaladinSacredOaths;
    internal override DeityDefinition DeityDefinition { get; }

    internal static FeatureDefinition DemonHunter { get; } = BuildDemonHunter();
    internal static ConditionDefinition TrialMark { get; } = BuildTrialMark();
    internal static FeatureDefinitionPower PowerTrialMark { get; } = BuildPowerTrialMark();

    internal static IsWeaponValidHandler IsCrossbowWeapon { get; } = 
        ValidatorsWeapon
            .IsOfWeaponType(LightCrossbowType, HeavyCrossbowType, CustomWeaponsContext.HandXbowWeaponType);

    internal OathOfDemonHunter()
    {
        //
        // LEVEL 03
        //
        
        // auto prepared spells
        // demon hunter
        // trial mark
        
        var autoPreparedSpells = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{Name}")
            .SetGuiPresentation(Name, Category.Subclass, "Feature/&DomainSpellsDescription")
            .SetAutoTag("Oath")
            .SetPreparedSpellGroups(
                BuildSpellGroup(2, HuntersMark, ProtectionFromEvilGood),
                BuildSpellGroup(5, MagicWeapon, MistyStep),
                BuildSpellGroup(9, Haste, DispelMagic),
                BuildSpellGroup(13, GuardianOfFaith, GreaterInvisibility),
                BuildSpellGroup(17, HoldMonster, DispelEvilAndGood))
            .SetSpellcastingClass(CharacterClassDefinitions.Paladin)
            .AddToDB();

        //
        // LEVEL 07
        //
        
        // Light Energy Crossbow Bolt

        var lightEnergyCrossbowBolt = FeatureDefinitionFeatureSetBuilder
            .Create($"Feature{Name}LightEnergyCrossbowBolt")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(
                new RangedAttackInMeleeDisadvantageRemover(IsCrossbowWeapon),
                new CustomReactionLightEnergyCrossbowBolt())
            .AddFeatureSet()
            .AddToDB();
        
        
        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.OathOfDread, 256))
            .AddFeaturesAtLevel(3,
                autoPreparedSpells,
                DemonHunter,
                PowerTrialMark)
            .AddFeaturesAtLevel(7, lightEnergyCrossbowBolt)
            .AddToDB();
    }

    private static FeatureDefinition BuildDemonHunter()
    {
        return FeatureDefinitionBuilder
            .Create($"Feature{Name}DemonHunter")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();
    }
    
    private static ConditionDefinition BuildTrialMark()
    {
        return ConditionDefinitionBuilder
            .Create(ConditionMarkedByHunter, $"Condition{Name}TrialMark")
            .SetGuiPresentation(Category.Condition, ConditionMarkedByHunter)
            .SetSpecialDuration(DurationType.Round, 10)
            .SetConditionType(ConditionType.Detrimental)
            .SetPossessive()
            .AddToDB();
    }
    
    private static FeatureDefinitionPower BuildPowerTrialMark()
    {
        // Trial Mark
        var damageAffinityTrialMark = FeatureDefinitionAdditionalDamageBuilder
            .Create("DamageAffinityTrialMark")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag("TrialMark")
            .SetTargetSide(Side.Ally)
            .SetTriggerCondition(AdditionalDamageTriggerCondition.AlwaysActive)
            .SetDamageDice(DieType.D6, 1)
            .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 1, 4, 3)
            .SetAdditionalDamageType(AdditionalDamageType.Specific)
            .SetSpecificDamageType(DamageTypeRadiant)
            .SetRequiredProperty(RestrictedContextRequiredProperty.None)
            .SetFrequencyLimit(FeatureLimitedUsage.None)
            .AddToDB();
        
        return FeatureDefinitionPowerBuilder
            .Create($"Power{Name}TrialMark")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.ChannelDivinity)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 10, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .SetParticleEffectParameters(LightningBolt)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.None)
                            .SetConditionForm(TrialMark, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .SetCustomSubFeatures(
                new CustomAdditionalDamagePowerTrialMark(damageAffinityTrialMark))
            .AddToDB();
    }
    
    private sealed class CustomAdditionalDamagePowerTrialMark : CustomAdditionalDamage
    {
        public CustomAdditionalDamagePowerTrialMark(IAdditionalDamageProvider provider) : base(provider)
        {
            
        }

        internal override bool IsValid(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            RulesetEffect rulesetEffect,
            bool criticalHit,
            bool firstTarget,
            out CharacterActionParams reactionParams)
        {
            reactionParams = null;

            var rulesetDefender = defender.RulesetCharacter;

            return attackMode.sourceDefinition is ItemDefinition { IsWeapon: true } && rulesetDefender.HasConditionOfType(TrialMark);
        }
    }
    
    private sealed class CustomReactionLightEnergyCrossbowBolt : IPhysicalAttackFinished
    {
        public IEnumerator OnAttackFinished(GameLocationBattleManager battleManager, CharacterAction action,
            GameLocationCharacter attacker, GameLocationCharacter defender, RulesetAttackMode attackerAttackMode,
            RollOutcome attackRollOutcome, int damageAmount)
        {
            if (attackRollOutcome is RollOutcome.Failure or RollOutcome.CriticalFailure)
            {
                yield break;
            }
            
            if (!IsCrossbowWeapon(attackerAttackMode, null, null))
            {
                yield break;
            }
            
            var battleService = ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;
            if (battleService is not { IsBattleInProgress: true })
            {
                yield break;
            }
            
            var actionService =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
            if (actionService == null)
            {
                yield break;
            }
            
            var rulesetAttacker = attacker.RulesetCharacter;
            if (!rulesetAttacker.CanUsePower(PowerTrialMark))
            {
                yield break;
            }
            
            var usablePower = UsablePowersProvider.Get(PowerTrialMark, rulesetAttacker);
            var reactionParams = new CharacterActionParams(attacker, (ActionDefinitions.Id)ExtraActionId.DoNothingReaction)
            {
                StringParameter = $"CustomReactionLightEnergyCrossbowBoltDescription",
                UsablePower = usablePower
            };
            var previousReactionCount = actionService.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestCustom("LightEnergyCrossbowBolt", reactionParams);

            actionService.AddInterruptRequest(reactionRequest);

            yield return battleService.WaitForReactions(attacker, actionService, previousReactionCount);
            
            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }
            
            var rulesetDefender = defender.RulesetCharacter;
            rulesetAttacker.UsePower(usablePower);
            rulesetDefender.InflictCondition(
                TrialMark.Name,
                TrialMark.DurationType,
                TrialMark.DurationParameter,
                TrialMark.TurnOccurence,
                AttributeDefinitions.TagCombat,
                rulesetAttacker.Guid,
                rulesetDefender.CurrentFaction.Name,
                1,
                null,
                0,
                0,
                0);
            
        }
    }
}
