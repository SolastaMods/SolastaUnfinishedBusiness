using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;
using Category = SolastaUnfinishedBusiness.Builders.Category;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class DomainTempest : AbstractSubclass
{
    public DomainTempest()
    {
        const string NAME = "DomainTempest";

        var autoPreparedSpellsDomainNature = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{NAME}")
            .SetGuiPresentation("ExpandedSpells", Category.Feature)
            .SetAutoTag("Domain")
            .SetPreparedSpellGroups(
                BuildSpellGroup(1, FogCloud, Thunderwave),
                BuildSpellGroup(3, Shatter),
                BuildSpellGroup(5, CallLightning, SleetStorm),
                BuildSpellGroup(7, IceStorm),
                BuildSpellGroup(9, InsectPlague))
            .SetSpellcastingClass(CharacterClassDefinitions.Cleric)
            .AddToDB();

        // LEVEL 01 - Wrath of The Storm

        var proficiencyHeavyArmor = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{NAME}HeavyArmor")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.Armor, EquipmentDefinitions.HeavyArmorCategory)
            .AddToDB();

        var proficiencyMartialWeapons = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{NAME}MartialWeapons")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.Weapon, EquipmentDefinitions.MartialWeaponCategory)
            .AddToDB();

        var featureSetAcolyteOfNature = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{NAME}WrathOfTheStorm")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(proficiencyHeavyArmor, proficiencyMartialWeapons)
            .AddToDB();

        //
        // Level 2 - Destructive Wrath
        //

        var divinePowerPrefix = Gui.Localize("Feature/&ClericChannelDivinityTitle") + ": ";

        var powerDestructiveWrath = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}DestructiveWrath")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("DestructiveWrath", Resources.PowerCharmAnimalsAndPlants, 256, 128))
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.ChannelDivinity)
            .AddToDB();

        powerDestructiveWrath.AddCustomSubFeatures(new CustomBehaviorDestructiveWrath(powerDestructiveWrath));

        var actionAffinityDestructiveWrathToggle = FeatureDefinitionActionAffinityBuilder
            .Create(FeatureDefinitionActionAffinitys.ActionAffinitySorcererMetamagicToggle,
                "ActionAffinityDestructiveWrathToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions((ActionDefinitions.Id)ExtraActionId.DestructiveWrathToggle)
            .AddCustomSubFeatures(
                new ValidateDefinitionApplication(ValidatorsCharacter.HasAvailablePowerUsage(powerDestructiveWrath)))
            .AddToDB();

        var featureSetDestructiveWrath = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{NAME}DestructiveWrath")
            .SetGuiPresentation(
                divinePowerPrefix + powerDestructiveWrath.FormatTitle(),
                powerDestructiveWrath.FormatDescription())
            .AddFeatureSet(actionAffinityDestructiveWrathToggle)
            .AddToDB();

        //
        // LEVEL 6 - Thunderous Strike
        //

        var actionAffinityThunderousStrike = FeatureDefinitionActionAffinityBuilder
            .Create("ActionAffinityThunderousStrikeToggle")
            .SetGuiPresentation(Category.Feature)
            .SetAuthorizedActions((ActionDefinitions.Id)ExtraActionId.ThunderousStrikeToggle)
            .AddCustomSubFeatures(new CustomBehaviorThunderousStrike())
            .AddToDB();

        //
        // LEVEL 08 - Divine Strike
        //

        var additionalDamageDivineStrike = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{NAME}DivineStrike")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag("DivineStrike")
            .SetDamageDice(DieType.D8, 1)
            .SetSpecificDamageType(DamageTypeThunder)
            .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 1, 8, 6)
            .SetFrequencyLimit(FeatureLimitedUsage.OnceInMyTurn)
            .SetAttackModeOnly()
            .AddToDB();


        // LEVEL 17 - Stormborn


        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Subclass, CharacterSubclassDefinitions.TraditionGreenmage)
            .AddFeaturesAtLevel(1, autoPreparedSpellsDomainNature, featureSetAcolyteOfNature)
            .AddFeaturesAtLevel(2, featureSetDestructiveWrath)
            .AddFeaturesAtLevel(6, actionAffinityThunderousStrike)
            .AddFeaturesAtLevel(8, additionalDamageDivineStrike)
            .AddFeaturesAtLevel(10, PowerClericDivineInterventionPaladin)
            .AddFeaturesAtLevel(17)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Cleric;

    internal override CharacterSubclassDefinition Subclass { get; }

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override FeatureDefinitionSubclassChoice SubclassChoice { get; }

    internal override DeityDefinition DeityDefinition => DeityDefinitions.Maraike;

    private sealed class CustomBehaviorDestructiveWrath(FeatureDefinitionPower powerDestructiveWrath)
        : IForceMaxDamageTypeDependent, IMagicEffectBeforeHitConfirmedOnEnemy, IActionFinishedByMe
    {
        private bool _isValid;

        public IEnumerator OnActionFinishedByMe(CharacterAction characterAction)
        {
            _isValid = false;

            yield break;
        }

        public bool IsValid(RulesetActor rulesetActor, DamageForm damageForm)
        {
            return damageForm.DamageType is DamageTypeLightning or DamageTypeThunder && _isValid;
        }

        public IEnumerator OnMagicEffectBeforeHitConfirmedOnEnemy(
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
            var usablePower = PowerProvider.Get(powerDestructiveWrath, rulesetAttacker);

            _isValid = actualEffectForms.Any(x =>
                           x.FormType == EffectForm.EffectFormType.Damage &&
                           x.DamageForm.DamageType is DamageTypeLightning or DamageTypeThunder) &&
                       rulesetAttacker.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.DestructiveWrathToggle) &&
                       rulesetAttacker.GetRemainingUsesOfPower(usablePower) > 0;

            if (!_isValid)
            {
                yield break;
            }

            rulesetAttacker.UsePower(usablePower);
            rulesetAttacker.LogCharacterUsedPower(powerDestructiveWrath);
        }
    }

    private sealed class CustomBehaviorThunderousStrike
        : IMagicEffectBeforeHitConfirmedOnEnemy, IPhysicalAttackBeforeHitConfirmedOnEnemy
    {
        private static readonly EffectForm PushForm = EffectFormBuilder
            .Create()
            .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 2)
            .Build();

        public IEnumerator OnMagicEffectBeforeHitConfirmedOnEnemy(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            if (actualEffectForms
                    .Any(x => x.FormType == EffectForm.EffectFormType.Damage &&
                              x.DamageForm.DamageType is DamageTypeLightning or DamageTypeThunder) &&
                defender.RulesetCharacter.SizeDefinition.Name is "Tiny" or "Small" or "Medium" or "Large" &&
                attacker.RulesetCharacter.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.ThunderousStrikeToggle))
            {
                actualEffectForms.Add(PushForm);
            }

            yield break;
        }

        public IEnumerator OnPhysicalAttackBeforeHitConfirmedOnEnemy(
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
            if (actualEffectForms
                    .Any(x => x.FormType == EffectForm.EffectFormType.Damage &&
                              x.DamageForm.DamageType is DamageTypeLightning or DamageTypeThunder) &&
                defender.RulesetCharacter.SizeDefinition.Name is "Tiny" or "Small" or "Medium" or "Large" &&
                attacker.RulesetCharacter.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.ThunderousStrikeToggle))
            {
                actualEffectForms.Add(PushForm);
            }

            {
                actualEffectForms.Add(PushForm);
            }

            yield break;
        }
    }
}
