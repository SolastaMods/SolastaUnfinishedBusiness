using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Classes;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Subclasses;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionProficiencys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttackModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionMovementAffinitys;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;

namespace SolastaUnfinishedBusiness.Models;

internal static class ClassesContext
{
    internal static void Load()
    {
        InventorClass.Build();

        // kept for backward compatibility
        CustomInvocationPoolDefinitionBuilder
            .Create("InvocationPoolMonkWeaponSpecialization")
            .SetGuiPresentation("InvocationPoolMonkWeaponSpecializationLearn", Category.Feature)
            .Setup(InvocationPoolTypeCustom.Pools.MonkWeaponSpecialization)
            .AddToDB();

        LoadMonkWeaponSpecialization();
        SwitchBarbarianFightingStyle();
        SwitchKatanaWeaponSpecialization();
        SwitchMonkAbundantKi();
        SwitchMonkFightingStyle();
        SwitchMonkImprovedUnarmoredMovementToMoveOnTheWall();
        SwitchRangerHumanoidFavoredEnemy();
        SwitchRogueFightingStyle();
        SwitchRogueStrSaving();
        SwitchScimitarWeaponSpecialization();
        SwitchSorcererMagicalGuidance();
        UpdateHandWrapsUseGauntletSlot();
    }

    #region Ranger

    internal static void SwitchRangerHumanoidFavoredEnemy()
    {
        if (Main.Settings.AddHumanoidFavoredEnemyToRanger)
        {
            AdditionalDamageRangerFavoredEnemyChoice.featureSet.Add(CommonBuilders
                .AdditionalDamageMarshalFavoredEnemyHumanoid);
        }
        else
        {
            AdditionalDamageRangerFavoredEnemyChoice.featureSet.Remove(CommonBuilders
                .AdditionalDamageMarshalFavoredEnemyHumanoid);
        }

        AdditionalDamageRangerFavoredEnemyChoice.FeatureSet.Sort(Sorting.CompareTitle);
    }

    #endregion

    #region _General

    internal static void SwitchKatanaWeaponSpecialization()
    {
        ProficiencyMonkWeapon.Proficiencies.Remove(CustomWeaponsContext.KatanaWeaponType.Name);

        if (Main.Settings.GrantKatanaSpecializationToMonk)
        {
            ProficiencyMonkWeapon.Proficiencies.Add(CustomWeaponsContext.KatanaWeaponType.Name);
        }
    }

    internal static void SwitchScimitarWeaponSpecialization()
    {
        var proficiencies = new List<FeatureDefinitionProficiency> { ProficiencyBardWeapon, ProficiencyRogueWeapon };

        foreach (var proficiency in proficiencies)
        {
            if (Main.Settings.GrantScimitarSpecializationToBardRogue)
            {
                proficiency.Proficiencies.TryAdd(WeaponTypeDefinitions.ScimitarType.Name);
            }
            else
            {
                proficiency.Proficiencies.Remove(WeaponTypeDefinitions.ScimitarType.Name);
            }
        }
    }

    #endregion

    #region Barbarian

    internal static readonly FeatureDefinitionFightingStyleChoice FightingStyleChoiceBarbarian =
        FeatureDefinitionFightingStyleChoiceBuilder
            .Create("FightingStyleChoiceBarbarian")
            .SetGuiPresentation("FighterFightingStyle", Category.Feature)
            .SetFightingStyles(
                // BlindFighting
                // Crippling
                // Defense
                "Dueling",
                // Executioner
                "GreatWeapon",
                // HandAndAHalf
                // Interception
                // Lunger
                // Merciless
                // Protection
                // Pugilist
                // RemarkableTechnique
                // RopeIpUp
                // ShieldExpert
                // Torchbearer
                "TwoWeapon")
            .AddToDB();

    internal static void SwitchBarbarianFightingStyle()
    {
        if (Main.Settings.EnableBarbarianFightingStyle)
        {
            Barbarian.FeatureUnlocks.TryAdd(
                new FeatureUnlockByLevel(FightingStyleChoiceBarbarian, 2));
        }
        else
        {
            Barbarian.FeatureUnlocks.RemoveAll(x =>
                x.level == 2 && x.FeatureDefinition == FightingStyleChoiceBarbarian);
        }

        Barbarian.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    #endregion

    #region Monk

    private static readonly FeatureDefinitionAttributeModifier AttributeModifierMonkAbundantKi =
        FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierMonkAbundantKi")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.AddHalfProficiencyBonus, AttributeDefinitions.KiPoints)
            .SetSituationalContext(SituationalContext.NotWearingArmorOrShield)
            .AddToDB();

    internal static readonly FeatureDefinitionFightingStyleChoice FightingStyleChoiceMonk =
        FeatureDefinitionFightingStyleChoiceBuilder
            .Create("FightingStyleChoiceMonk")
            .SetGuiPresentation("FighterFightingStyle", Category.Feature)
            .SetFightingStyles(
                "Archery",
                // BlindFighting
                // Crippling
                // Defense
                "Dueling",
                // Executioner
                // GreatWeapon
                // HandAndAHalf
                // Interception
                // Lunger
                // Merciless
                // Protection
                // Pugilist
                // RemarkableTechnique
                // RopeIpUp
                // ShieldExpert
                // Torchbearer
                "TwoWeapon")
            .AddToDB();

    private static void LoadMonkWeaponSpecialization()
    {
        var weaponTypeDefinitions = new List<WeaponTypeDefinition>
        {
            WeaponTypeDefinitions.BattleaxeType,
            WeaponTypeDefinitions.LightCrossbowType,
            WeaponTypeDefinitions.LongbowType,
            WeaponTypeDefinitions.LongswordType,
            WeaponTypeDefinitions.MorningstarType,
            WeaponTypeDefinitions.RapierType,
            WeaponTypeDefinitions.ScimitarType,
            WeaponTypeDefinitions.ShortbowType,
            WeaponTypeDefinitions.WarhammerType,
            CustomWeaponsContext.HandXbowWeaponType
        };

        foreach (var weaponTypeDefinition in weaponTypeDefinitions)
        {
            var weaponTypeName = weaponTypeDefinition.Name;

            var featureMonkWeaponSpecialization = FeatureDefinitionProficiencyBuilder
                .Create($"FeatureMonkWeaponSpecialization{weaponTypeName}")
                .SetGuiPresentationNoContent(true)
                .SetProficiencies(ProficiencyType.Weapon, weaponTypeName)
                .AddCustomSubFeatures(
                    new MonkWeaponSpecialization { WeaponType = weaponTypeDefinition })
                .AddToDB();

            if (!weaponTypeDefinition.IsBow && !weaponTypeDefinition.IsCrossbow)
            {
                featureMonkWeaponSpecialization.AddCustomSubFeatures(
                    new AddTagToWeapon(TagsDefinitions.WeaponTagFinesse, TagsDefinitions.Criticity.Important,
                        ValidatorsWeapon.IsOfWeaponType(weaponTypeDefinition))
                );
            }

            // ensure we get dice upgrade on these
            AttackModifierMonkMartialArtsImprovedDamage.AddCustomSubFeatures(
                new MonkWeaponSpecializationDiceUpgrade(weaponTypeDefinition));

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

    internal static void SwitchMonkAbundantKi()
    {
        if (Main.Settings.EnableMonkAbundantKi)
        {
            Monk.FeatureUnlocks.TryAdd(
                new FeatureUnlockByLevel(AttributeModifierMonkAbundantKi, 2));
        }
        else
        {
            Monk.FeatureUnlocks
                .RemoveAll(x => x.level == 2 &&
                                x.FeatureDefinition == AttributeModifierMonkAbundantKi);
        }

        Monk.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchMonkFightingStyle()
    {
        if (Main.Settings.EnableMonkFightingStyle)
        {
            Monk.FeatureUnlocks.TryAdd(
                new FeatureUnlockByLevel(FightingStyleChoiceMonk, 2));
        }
        else
        {
            Monk.FeatureUnlocks
                .RemoveAll(x => x.level == 2 &&
                                x.FeatureDefinition == FightingStyleChoiceMonk);
        }

        Monk.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchMonkImprovedUnarmoredMovementToMoveOnTheWall()
    {
        if (Main.Settings.EnableMonkImprovedUnarmoredMovementToMoveOnTheWall)
        {
            MovementAffinityMonkUnarmoredMovementImproved.GuiPresentation.description =
                "Feature/&MonkAlternateUnarmoredMovementImprovedDescription";
            MovementAffinityMonkUnarmoredMovementImproved.GuiPresentation.title =
                "Feature/&MonkAlternateUnarmoredMovementImprovedTitle";
            MovementAffinityMonkUnarmoredMovementImproved.canMoveOnWalls = true;
        }
        else
        {
            MovementAffinityMonkUnarmoredMovementImproved.GuiPresentation.description =
                "Feature/&MonkUnarmoredMovementImprovedDescription";
            MovementAffinityMonkUnarmoredMovementImproved.GuiPresentation.title =
                "Feature/&MonkUnarmoredMovementImprovedTitle";
            MovementAffinityMonkUnarmoredMovementImproved.canMoveOnWalls = false;
        }
    }

#if false
    internal static void SwitchMonkWeaponSpecialization()
    {
        var levels = new[] { 2, 11 };

        if (Main.Settings.EnableMonkWeaponSpecialization)
        {
            foreach (var level in levels)
            {
                Monk.FeatureUnlocks.TryAdd(
                    new FeatureUnlockByLevel(InvocationPoolMonkWeaponSpecialization, level));
            }
        }
        else
        {
            foreach (var level in levels)
            {
                Monk.FeatureUnlocks
                    .RemoveAll(x => x.level == level &&
                                    x.FeatureDefinition == InvocationPoolMonkWeaponSpecialization);
            }
        }

        Monk.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }
#endif

    internal static void UpdateHandWrapsUseGauntletSlot()
    {
        foreach (var item in DatabaseRepository.GetDatabase<ItemDefinition>())
        {
            if (item is not { WeaponDescription.weaponType: EquipmentDefinitions.WeaponTypeUnarmedStrike })
            {
                continue;
            }

            if (item == ItemDefinitions.UnarmedStrikeBase) { continue; }

            if (Main.Settings.EnableMonkHandwrapsUseGauntletSlot)
            {
                item.SlotTypes.Add(EquipmentDefinitions.SlotTypeGloves);
                item.SlotsWhereActive.Add(EquipmentDefinitions.SlotTypeGloves);
            }
            else
            {
                item.SlotTypes.Remove(EquipmentDefinitions.SlotTypeGloves);
                item.SlotsWhereActive.Remove(EquipmentDefinitions.SlotTypeGloves);
            }
        }
    }

    internal sealed class MonkWeaponSpecialization
    {
        internal WeaponTypeDefinition WeaponType { get; set; }
    }

    private sealed class MonkWeaponSpecializationDiceUpgrade : IValidateContextInsteadOfRestrictedProperty
    {
        private readonly WeaponTypeDefinition _weaponTypeDefinition;

        internal MonkWeaponSpecializationDiceUpgrade(WeaponTypeDefinition weaponTypeDefinition)
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
                character.GetSubFeaturesByType<MonkWeaponSpecializationDiceUpgrade>().Exists(
                    x => x._weaponTypeDefinition == attackModeWeaponType));
        }
    }

    #endregion

    #region Rogue

    internal static readonly FeatureDefinitionFightingStyleChoice FightingStyleChoiceRogue =
        FeatureDefinitionFightingStyleChoiceBuilder
            .Create("FightingStyleChoiceRogue")
            .SetGuiPresentation("FighterFightingStyle", Category.Feature)
            .SetFightingStyles(
                "Archery",
                // BlindFighting
                // Crippling
                "Defense",
                // Dueling
                // Executioner
                // GreatWeapon
                // HandAndAHalf
                // Interception
                // Lunger
                // Merciless
                // Protection
                // Pugilist
                // RemarkableTechnique
                // RopeIpUp
                // ShieldExpert
                // Torchbearer
                "TwoWeapon")
            .AddToDB();

    internal static void SwitchRogueFightingStyle()
    {
        if (Main.Settings.EnableRogueFightingStyle)
        {
            Rogue.FeatureUnlocks.TryAdd(
                new FeatureUnlockByLevel(FightingStyleChoiceRogue, 2));
        }
        else
        {
            Rogue.FeatureUnlocks.RemoveAll(x => x.level == 2 && x.FeatureDefinition == FightingStyleChoiceRogue);
        }

        Rogue.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    private static void SwitchRogueStrSaving()
    {
        var powerNames = new List<string>
        {
            "PowerRogueCunningStrikeDisarm",
            //"PowerRogueCunningStrikePoison",
            "PowerRogueCunningStrikeTrip",
            //"PowerRogueCunningStrikeWithdraw",
            //"PowerRogueDeviousStrikeDaze",
            //"PowerRogueDeviousStrikeKnockOut",
            "PowerRogueDeviousStrikeObscure",
            "PowerRoguishOpportunistDebilitatingStrike",
            "PowerRoguishOpportunistImprovedDebilitatingStrike",
            "PowerRoguishBladeCallerHailOfBlades"
        };

        foreach (var power in DatabaseRepository.GetDatabase<FeatureDefinitionPower>()
                     .Where(x => powerNames.Contains(x.Name)))
        {
            power.AddCustomSubFeatures(new ModifyEffectDescriptionSavingThrowRogue(power));
        }
    }

    #endregion

    #region Sorcerer

    private static readonly FeatureDefinition FeatureSorcererMagicalGuidance = FeatureDefinitionBuilder
        .Create("FeatureSorcererMagicalGuidance")
        .SetGuiPresentation(Category.Feature)
        .AddCustomSubFeatures(new TryAlterOutcomeAttributeCheckSorcererMagicalGuidance())
        .AddToDB();

    internal static void SwitchSorcererMagicalGuidance()
    {
        if (Main.Settings.EnableSorcererMagicalGuidance)
        {
            Sorcerer.FeatureUnlocks.TryAdd(new FeatureUnlockByLevel(FeatureSorcererMagicalGuidance, 5));
        }
        else
        {
            Sorcerer.FeatureUnlocks.RemoveAll(x =>
                x.level == 5 && x.FeatureDefinition == FeatureSorcererMagicalGuidance);
        }

        Sorcerer.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }


    private sealed class TryAlterOutcomeAttributeCheckSorcererMagicalGuidance : ITryAlterOutcomeAttributeCheck
    {
        public IEnumerator OnTryAlterAttributeCheck(
            GameLocationBattleManager battleManager,
            AbilityCheckData abilityCheckData,
            GameLocationCharacter defender,
            GameLocationCharacter helper)
        {
            var rulesetHelper = helper.RulesetCharacter;

            if (abilityCheckData.AbilityCheckRoll == 0 ||
                abilityCheckData.AbilityCheckRollOutcome != RollOutcome.Failure ||
                helper != defender ||
                rulesetHelper.RemainingSorceryPoints == 0)
            {
                yield break;
            }

            yield return helper.MyReactToDoNothing(
                ExtraActionId.DoNothingFree,
                defender,
                "MagicalGuidanceCheck",
                "CustomReactionMagicalGuidanceCheckDescription"
                    .Formatted(Category.Reaction, defender.Name, helper.Name),
                ReactionValidated,
                battleManager: battleManager,
                resource: ReactionResourceSorceryPoints.Instance);

            yield break;

            void ReactionValidated()
            {
                rulesetHelper.SpendSorceryPoints(1);

                var dieRoll = rulesetHelper.RollDie(DieType.D20, RollContext.None, false, AdvantageType.None, out _,
                    out _);
                var previousRoll = abilityCheckData.AbilityCheckRoll;

                abilityCheckData.AbilityCheckSuccessDelta += dieRoll - abilityCheckData.AbilityCheckRoll;
                abilityCheckData.AbilityCheckRoll = dieRoll;
                abilityCheckData.AbilityCheckRollOutcome = abilityCheckData.AbilityCheckSuccessDelta >= 0
                    ? RollOutcome.Success
                    : RollOutcome.Failure;

                rulesetHelper.LogCharacterActivatesAbility(
                    "Feature/&FeatureSorcererMagicalGuidanceTitle",
                    "Feedback/&MagicalGuidanceCheckToHitRoll",
                    extra:
                    [
                        (dieRoll > previousRoll ? ConsoleStyleDuplet.ParameterType.Positive : ConsoleStyleDuplet.ParameterType.Negative,
                            dieRoll.ToString()),
                        (previousRoll > dieRoll ? ConsoleStyleDuplet.ParameterType.Positive : ConsoleStyleDuplet.ParameterType.Negative,
                            previousRoll.ToString())
                    ]);
            }
        }
    }

    #endregion
}
