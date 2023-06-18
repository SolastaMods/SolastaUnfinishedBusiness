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
    internal static FeatureDefinitionAdditionalDamage AdditionalDamageTrialMark { get; } = BuildAdditionalDamageTrialMark();

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

        var lightEnergyCrossbowBolt = FeatureDefinitionBuilder
            .Create($"Feature{Name}LightEnergyCrossbowBolt")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(
                new RangedAttackInMeleeDisadvantageRemover(IsCrossbowWeapon),
                new CustomReactionLightEnergyCrossbowBolt())
            .AddToDB();
        
        //
        // LEVEL 15
        //
        
        // Divine Crossbow
        // Extra Attack (3)
        
        var divineCrossbow = FeatureDefinitionAttributeModifierBuilder
            .Create($"Feature{Name}DivineCrossbow")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(
                new ModifyCrossbowAttackModeDivineCrossbow())
            .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                AttributeDefinitions.CriticalThreshold, -1)
            .AddToDB();
        
        var extraAttackForce3 = FeatureDefinitionAttributeModifierBuilder
            .Create($"Feature{Name}ExtraAttack")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.ForceIfBetter,
                AttributeDefinitions.AttacksNumber, 3)
            .AddToDB();
        
        //
        // LEVEL 20
        //
        
        // Demon Slayer
        var demonSlayerGuiPresentation = new GuiPresentationBuilder(
            $"Feature/&Feature{Name}DemonSlayerTitle",
            $"Feature/&Feature{Name}DemonSlayerTitleDescription")
            .Build();

        var demonSlayerPhysics = FeatureDefinitionDieRollModifierDamageTypeDependentBuilder
            .Create($"Feature{Name}DemonSlayerPhysics")
            .SetGuiPresentation(demonSlayerGuiPresentation)
            .SetModifiers(RollContext.AttackDamageValueRoll,50, 3, 2,
                $"Feature/&DieRollModifier{Name}DemonSlayer", DamageTypeRadiant)
            .AddToDB();
        
        var demonSlayerMagic = FeatureDefinitionDieRollModifierDamageTypeDependentBuilder
            .Create($"Feature{Name}DemonSlayerMagic")
            .SetGuiPresentation(demonSlayerGuiPresentation)
            .SetModifiers(RollContext.MagicDamageValueRoll,50, 3, 2,
                $"Feature/&DieRollModifier{Name}DemonSlayer", DamageTypeRadiant)
            .AddToDB();
        
        var demonSlayerSet = FeatureDefinitionFeatureSetBuilder
            .Create($"Feature{Name}DemonSlayer")
            .SetGuiPresentation(demonSlayerGuiPresentation)
            .SetCustomSubFeatures(
                new ModifyAdditionalDamageFormDivineSmite())
            .AddFeatureSet(
                demonSlayerPhysics,
                demonSlayerMagic
            )
            .AddToDB();
        
        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.OathOfDemonHunter, 256))
            .AddFeaturesAtLevel(3,
                autoPreparedSpells,
                DemonHunter,
                PowerTrialMark)
            .AddFeaturesAtLevel(7, lightEnergyCrossbowBolt)
            .AddFeaturesAtLevel(15, 
                divineCrossbow,
                extraAttackForce3)
            .AddFeaturesAtLevel(20, demonSlayerSet)
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
            .SetGuiPresentation(Category.Condition)
            .SetSpecialDuration(DurationType.Round, 10)
            .SetConditionType(ConditionType.Detrimental)
            .SetPossessive()
            .AddToDB();
    }
    
    private static FeatureDefinitionAdditionalDamage BuildAdditionalDamageTrialMark()
    {
        return FeatureDefinitionAdditionalDamageBuilder
            .Create("DamageAffinityTrialMark")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag("TrialMark")
            .SetDamageValueDetermination(AdditionalDamageValueDetermination.Die)
            .SetTriggerCondition(AdditionalDamageTriggerCondition.AlwaysActive)
            .SetDamageDice(DieType.D6, 1)
            .SetAdditionalDamageType(AdditionalDamageType.Specific)
            .SetSpecificDamageType(DamageTypeRadiant)
            .AddToDB();
    }
    
    private static FeatureDefinitionPower BuildPowerTrialMark()
    {
        return FeatureDefinitionPowerBuilder
            .Create($"Power{Name}TrialMark")
            .SetGuiPresentation(Category.Feature, Sprites.GetSprite("PowerTrialMark", Resources.PowerTrialMark, 256, 128))
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
                new CustomAdditionalDamagePowerTrialMark(AdditionalDamageTrialMark),
                new ModifyAdditionalDamageFormTrialMark())
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
            
            var rulesetDefender = defender.RulesetCharacter;
            if (rulesetDefender.HasConditionOfType(TrialMark))
            {
                yield break;
            }
            
            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetUsablePower = rulesetAttacker.UsablePowers.Find(x => x.PowerDefinition == PowerTrialMark);
            
            if (rulesetUsablePower == null || rulesetAttacker.GetRemainingUsesOfPower(rulesetUsablePower) <= 0)
            {
                yield break;
            }
            
            var usablePower = UsablePowersProvider.Get(PowerTrialMark, rulesetAttacker);
            var reactionParams = new CharacterActionParams(attacker, (ActionDefinitions.Id)ExtraActionId.DoNothingReaction)
            {
                StringParameter = $"Reaction/&CustomReactionLightEnergyCrossbowBoltDescription",
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
    
    private sealed class ModifyCrossbowAttackModeDivineCrossbow : IModifyWeaponAttackMode
    {
        public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode)
        {
            if (!IsCrossbowWeapon(attackMode, null, null))
            {
                return;
            }
            
            attackMode.maxRange += 6;
        }
    }
    
    private sealed class ModifyAdditionalDamageFormDivineSmite : IModifyAdditionalDamageForm
    {
        public void OnModifyAdditionalDamageForm(FeatureDefinition featureDefinition, DamageForm additionalDamageForm,
            RulesetAttackMode attackMode, GameLocationCharacter attacker, GameLocationCharacter defender)
        {
            if (featureDefinition != AdditionalDamagePaladinDivineSmite)
            {
                return;
            }

            additionalDamageForm.diceNumber += 1;
        }
    }
    
    // Because of the CustomAdditionalDamage not in the character's feature,
    // I need to add level growth separately
    private sealed class ModifyAdditionalDamageFormTrialMark : IModifyAdditionalDamageForm
    {
        public void OnModifyAdditionalDamageForm(FeatureDefinition featureDefinition, DamageForm additionalDamageForm,
            RulesetAttackMode attackMode, GameLocationCharacter attacker, GameLocationCharacter defender)
        {
            if (featureDefinition != AdditionalDamageTrialMark)
            {
                return;
            }

            var classLevel = attacker.RulesetCharacter.GetClassLevel(PaladinClass);
            if (classLevel <= 3)
            {
                return;
            }

            additionalDamageForm.diceNumber += (classLevel - 3) / 4;
        }
    }
}
