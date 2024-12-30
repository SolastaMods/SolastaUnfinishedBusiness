using System.Collections;
using System.Collections.Generic;
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
using SolastaUnfinishedBusiness.Patches;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static ActionDefinitions;
using static FeatureDefinitionAttributeModifier;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttackModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class WayOfBlade : AbstractSubclass
{
    private const string Name = "WayOfBlade";

    internal static readonly FeatureDefinitionPower PowerAgileParry = FeatureDefinitionPowerBuilder
        .Create($"Power{Name}AgileParry")
        .SetGuiPresentationNoContent(true)
        .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints)
        .SetShowCasting(false)
        .AddToDB();

    internal static readonly FeatureDefinitionPowerSharedPool PowerAgileParryAttack =
        FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}AgileParryAttack")
            .SetGuiPresentation($"FeatureSet{Name}AgileParry", Category.Feature, hidden: true)
            .SetSharedPool(ActivationTime.NoCost, PowerAgileParry)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .Build())
            .AddCustomSubFeatures(new MagicEffectFinishedByMeAgileParryAttack())
            .AddToDB();

    internal static readonly FeatureDefinitionPowerSharedPool PowerAgileParrySave =
        FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}AgileParrySave")
            .SetGuiPresentation("PowerMonkReturnAttacks", Category.Feature, hidden: true)
            .SetSharedPool(ActivationTime.NoCost, PowerAgileParry)
            .SetShowCasting(false)
            .SetExplicitAbilityScore(AttributeDefinitions.Dexterity)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Dexterity, false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Wisdom, 8)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetBonusMode(AddBonusMode.AbilityBonus)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetDamageForm(diceNumber: 2)
                            .Build())
                    .SetImpactEffectParameters(PowerRoguishHoodlumDirtyFighting)
                    .Build())
            .AddToDB();

    private static readonly EffectForm ConditionFormSwiftBlade = EffectFormBuilder.ConditionForm(
        ConditionDefinitionBuilder
            .Create($"Condition{Name}SwiftBlade")
            .SetGuiPresentation($"Feature{Name}SwiftStrike", Category.Feature)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(
                FeatureDefinitionAdditionalActionBuilder
                    .Create($"AdditionalAction{Name}SwiftBlade")
                    .SetGuiPresentationNoContent(true)
                    .SetActionType(ActionType.Main)
                    .SetRestrictedActions(Id.AttackMain)
                    .SetMaxAttacksNumber(1)
                    .AddToDB())
            .AddToDB());

    public WayOfBlade()
    {
        //
        // LEVEL 03
        //

        // One With The Blade

        var attributeModifierOneWithTheBlade = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}OneWithTheBlade")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, 2)
            .SetSituationalContext(ExtraSituationalContext.HasMeleeWeaponInMainHandWithFreeOffhand)
            .AddCustomSubFeatures(
                new ModifyWeaponAttackModeOneWithTheBlade(),
                new AddExtraMainHandAttack(
                    ActionType.Bonus,
                    ValidatorsCharacter.HasMeleeWeaponInMainHandAndFreeOffhand,
                    ValidatorsCharacter.HasAnyOfConditions(ConditionMonkMartialArtsUnarmedStrikeBonus)))
            .AddToDB();

        // Path of the Blade

        var invocationPoolPathOfTheBlade = CustomInvocationPoolDefinitionBuilder
            .Create($"InvocationPool{Name}PathOfTheBlade")
            .SetGuiPresentation(Category.Feature)
            .Setup(InvocationPoolTypeCustom.Pools.MonkWeaponSpecialization, 2)
            .AddToDB();

        var invocationPoolMonkWeaponSpecializationAddOne = CustomInvocationPoolDefinitionBuilder
            .Create($"InvocationPool{Name}PathOfTheBladeAddOne")
            .SetGuiPresentationNoContent(true)
            .Setup(InvocationPoolTypeCustom.Pools.MonkWeaponSpecialization)
            .AddToDB();

        LoadMonkWeaponSpecialization();

        //
        // LEVEL 06
        //

        // Agile Parry

        PowerBundle.RegisterPowerBundle(PowerAgileParry, false, PowerAgileParryAttack, PowerAgileParrySave);

        var featureSetAgileParry = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}AgileParry")
            .SetGuiPresentation(Category.Feature)
            // most of the logic is already implemented at Tabletop2024Context.PowerMonkReturnAttacks
            // reason why re-added here in case Monk 2024 Deflect Attacks isn't checked
            .SetFeatureSet(
                PowerAgileParry, PowerAgileParryAttack, PowerAgileParrySave, Tabletop2024Context.PowerMonkReturnAttacks)
            .AddToDB();

        //
        // LEVEL 11
        //

        // Swift Strike

        var featureSwiftStrike = FeatureDefinitionBuilder
            .Create($"Feature{Name}SwiftStrike")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        // Monk 2024 powers are handled directly on 2024MonkContext during their building
        PowerMonkPatientDefense.AddCustomSubFeatures(ModifyEffectDescriptionSwiftStrike.Marker);
        PowerMonkStepOfTheWindDash.AddCustomSubFeatures(ModifyEffectDescriptionSwiftStrike.Marker);
        PowerMonkStepOftheWindDisengage.AddCustomSubFeatures(ModifyEffectDescriptionSwiftStrike.Marker);

        //
        // LEVEL 17
        //

        //  Master of the Blade 

        var featureMasterOfTheBlade = FeatureDefinitionBuilder
            .Create($"Feature{Name}MasterOfTheBlade")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        featureMasterOfTheBlade.AddCustomSubFeatures(
            new PhysicalAttackFinishedByMeMasterOfTheBlade(featureMasterOfTheBlade));

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.WayOfTheBlade, 256))
            .AddFeaturesAtLevel(3, attributeModifierOneWithTheBlade, invocationPoolPathOfTheBlade)
            .AddFeaturesAtLevel(6, featureSetAgileParry, invocationPoolMonkWeaponSpecializationAddOne)
            .AddFeaturesAtLevel(11, featureSwiftStrike, invocationPoolMonkWeaponSpecializationAddOne)
            .AddFeaturesAtLevel(17, featureMasterOfTheBlade, invocationPoolMonkWeaponSpecializationAddOne)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => Monk;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceMonkMonasticTraditions;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    internal static bool IsAgileParryValid(
        GameLocationCharacter defender, GameLocationCharacter attacker)
    {
        var rulesetDefender = defender.RulesetCharacter;
        var wayOfBladeLevel = rulesetDefender.GetSubclassLevel(Monk, Name);

        if (wayOfBladeLevel < 6 ||
            !ValidatorsCharacter.HasMeleeWeaponInMainHandAndFreeOffhand(rulesetDefender))
        {
            return false;
        }

        var defenderAttackMode = defender.FindActionAttackMode(Id.AttackMain);

        return defender.IsWithinRange(attacker, defenderAttackMode?.ReachRange ?? 1);
    }

    private static void LoadMonkWeaponSpecialization()
    {
        var weaponTypeDefinitions = new List<WeaponTypeDefinition>
        {
            CustomWeaponsContext.KatanaWeaponType,
            WeaponTypeDefinitions.BattleaxeType,
            WeaponTypeDefinitions.LongswordType,
            WeaponTypeDefinitions.MorningstarType,
            WeaponTypeDefinitions.RapierType,
            WeaponTypeDefinitions.ScimitarType,
            WeaponTypeDefinitions.WarhammerType
        };

        foreach (var weaponTypeDefinition in weaponTypeDefinitions)
        {
            var weaponTypeName = weaponTypeDefinition.Name;

            var featureMonkWeaponSpecialization = FeatureDefinitionProficiencyBuilder
                .Create($"FeatureMonkWeaponSpecialization{weaponTypeName}")
                .SetGuiPresentationNoContent(true)
                .SetProficiencies(ProficiencyType.Weapon, weaponTypeName)
                .AddCustomSubFeatures(new WeaponSpecialization { WeaponType = weaponTypeDefinition })
                .AddToDB();

            featureMonkWeaponSpecialization.AddCustomSubFeatures(
                new AddTagToWeapon(
                    TagsDefinitions.WeaponTagFinesse,
                    TagsDefinitions.Criticity.Important,
                    ValidatorsWeapon.IsOfWeaponType(weaponTypeDefinition))
            );

            // ensure we get dice upgrade on these
            AttackModifierMonkMartialArtsImprovedDamage.AddCustomSubFeatures(
                new WeaponSpecializationDiceUpgrade(weaponTypeDefinition));

            _ = CustomInvocationDefinitionBuilder
                .Create($"CustomInvocationMonkWeaponSpecialization{weaponTypeName}")
                .SetGuiPresentation(
                    weaponTypeDefinition.GuiPresentation.Title,
                    weaponTypeDefinition.GuiPresentation.Description,
                    CustomWeaponsContext.GetStandardWeaponOfType(weaponTypeDefinition.Name))
                .SetPoolType(InvocationPoolTypeCustom.Pools.MonkWeaponSpecialization)
                .SetGrantedFeature(featureMonkWeaponSpecialization)
                .AddCustomSubFeatures(ModifyInvocationVisibility.Marker)
                .AddToDB();
        }
    }

    //
    // One With The Blade
    //

    // set attacks number to 2 to allow a mix of unarmed / melee weapon attacks otherwise game engine will consume bonus action
    // the patch on CharacterActionItemForm.Refresh finishes the trick by hiding the number of attacks with a proper hide tag
    private sealed class ModifyWeaponAttackModeOneWithTheBlade : IModifyWeaponAttackMode
    {
        public void ModifyWeaponAttackMode(
            RulesetCharacter rulesetCharacter,
            RulesetAttackMode attackMode,
            RulesetItem weapon,
            bool canAddAbilityDamageBonus)
        {
            var character = GameLocationCharacter.GetFromActor(rulesetCharacter);

            if (character is not { UsedBonusAttacks: 0 } ||
                attackMode.ActionType != ActionType.Bonus ||
                !rulesetCharacter.HasConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, ConditionFlurryOfBlows) ||
                !rulesetCharacter.IsMonkWeapon(attackMode.SourceDefinition as ItemDefinition))
            {
                return;
            }

            attackMode.AttacksNumber = 2;
            attackMode.AddAttackTagAsNeeded(
                CharacterActionItemFormPatcher.Refresh_Patch.HideAttacksNumberOnActionPanel);
        }
    }

    //
    // Path of the Blade
    //

    internal sealed class WeaponSpecialization
    {
        internal WeaponTypeDefinition WeaponType { get; set; }
    }

    private sealed class WeaponSpecializationDiceUpgrade : IValidateContextInsteadOfRestrictedProperty
    {
        private readonly WeaponTypeDefinition _weaponTypeDefinition;

        internal WeaponSpecializationDiceUpgrade(WeaponTypeDefinition weaponTypeDefinition)
        {
            _weaponTypeDefinition = weaponTypeDefinition;
        }

        public (OperationType, bool) ValidateContext(
            BaseDefinition definition,
            IRestrictedContextProvider provider,
            RulesetCharacter character,
            ItemDefinition itemDefinition,
            bool rangedAttack, RulesetAttackMode attackMode,
            RulesetEffect rulesetEffect)
        {
            var attackModeWeaponType =
                (attackMode?.SourceDefinition as ItemDefinition)?.WeaponDescription.WeaponTypeDefinition;

            return (OperationType.Or,
                character.GetSubFeaturesByType<WeaponSpecializationDiceUpgrade>().Exists(
                    x => x._weaponTypeDefinition == attackModeWeaponType));
        }
    }

    //
    // Master of the Blade
    //

    private sealed class PhysicalAttackFinishedByMeMasterOfTheBlade(FeatureDefinition featureMasterOfTheBlade)
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
            var rulesetAttacker = attacker.RulesetCharacter;

            if (rollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess ||
                !attacker.OncePerTurnIsValid(featureMasterOfTheBlade.Name) ||
                ValidatorsWeapon.IsUnarmed(attackMode) ||
                !rulesetAttacker.IsMonkWeapon(attackMode.SourceDefinition as ItemDefinition))
            {
                yield break;
            }

            attacker.SetSpecialFeatureUses(featureMasterOfTheBlade.Name, 0);
            rulesetAttacker.LogCharacterUsedFeature(featureMasterOfTheBlade);

            attacker.MyExecuteActionAttack(
                Id.AttackFree,
                defender,
                attackMode,
                action.ActionParams.ActionModifiers[0]);
        }
    }

    //
    // Swift Strike
    //

    internal sealed class ModifyEffectDescriptionSwiftStrike : IModifyEffectDescription
    {
        internal static readonly ModifyEffectDescriptionSwiftStrike Marker = new();

        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            var subclassLevel = character.GetSubclassLevel(Monk, Name);

            return subclassLevel >= 11 &&
                   (definition.Name.StartsWith("PowerMonkPatientDefense") ||
                    definition.Name.StartsWith("PowerMonkStepOfTheWind"));
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            effectDescription.EffectForms.Add(ConditionFormSwiftBlade);

            return effectDescription;
        }
    }

    //
    // Agile Parry Attack
    //

    private sealed class MagicEffectFinishedByMeAgileParryAttack : IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(
            CharacterAction action, GameLocationCharacter attacker, List<GameLocationCharacter> targets)
        {
            if (action.ActionParams.RulesetEffect.SourceDefinition != PowerAgileParryAttack)
            {
                yield break;
            }

            var defender = targets[0];
            var attackMode = attacker.FindActionAttackMode(Id.AttackMain);
            var attackModeCopy = RulesetAttackMode.AttackModesPool.Get();

            attackModeCopy.Copy(attackMode);
            attackModeCopy.ActionType = ActionType.NoCost;

            attacker.MyExecuteActionAttack(
                Id.AttackOpportunity,
                defender,
                attackModeCopy,
                new ActionModifier());
        }
    }
}
