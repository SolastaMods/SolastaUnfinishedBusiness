using SolastaModApi.Extensions;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionProficiencys;
using static SolastaModApi.DatabaseHelper;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;
using static RuleDefinitions;

namespace SolastaCommunityExpansion.Classes.Warlock.Subclasses
{
    public static class AHWarlockSubclassSoulBladePact
    {
        internal static CharacterSubclassDefinition Build()
        {
            var summonPactWeaponPower = FeatureDefinitionPowerBuilder
                .Create(FeatureDefinitionPowers.PowerTraditionShockArcanistArcaneFury, "AHSoulBladeSummonPactWeaponPower", DefinitionBuilder.CENamespaceGuid)
                .SetOrUpdateGuiPresentation(Category.Feature, SpiritualWeapon.GuiPresentation.SpriteReference)
                .SetEffectDescription(SpiritualWeapon.EffectDescription)
                .SetRechargeRate(RechargeRate.ShortRest)
                .SetActivationTime(ActivationTime.NoCost)
                .AddToDB();

            var additionalDamageBonus = FeatureDefinitionAdditionalDamageBuilder
                .Create(FeatureDefinitionAdditionalDamages.AdditionalDamageBracersOfArchery, "AHSoulBladeEmpowerWeaponDamageBonus", DefinitionBuilder.CENamespaceGuid)
                .SetOrUpdateGuiPresentation(Category.Feature)
                .SetRequiredProperty(AdditionalDamageRequiredProperty.None)
                .SetAdditionalDamageType(AdditionalDamageType.SameAsBaseDamage)
                .SetDamageValueDetermination(AdditionalDamageValueDetermination.ProficiencyBonus)
                .SetNotificationTag("SoulEmpowered")
                .AddToDB();

            var weaponCondition = ConditionDefinitionBuilder
                .Create(ConditionDefinitions.ConditionHeraldOfBattle, "AHSoulBladeEmpowerWeaponCondition", DefinitionBuilder.CENamespaceGuid)
                .SetOrUpdateGuiPresentation(Category.Feature)
                .SetAllowMultipleInstances(false)
                .SetFeatures(additionalDamageBonus)
                .SetDuration(DurationType.Minute, 1)
                .AddToDB();

            //TODO: convert to separate power that adds damage and small dim glow for a minute PB times a day
            // var empowerWeaponPower = FeatureDefinitionPowerBuilder
            //     .Create(FeatureDefinitionPowers.PowerOathOfDevotionSacredWeapon, "AHWarlockSoulBladePactEmpowerWeaponPower", DefinitionBuilder.CENamespaceGuid)
            //     .SetOrUpdateGuiPresentation(Category.Feature)
            //     .SetShortTitleOverride("Feature/&AHWarlockSoulBladePactEmpowerWeaponPowerTitle")
            //     .SetRechargeRate(RechargeRate.ShortRest)
            //     .SetFixedUsesPerRecharge(1)
            //     .SetCostPerUse(1)
            //     .SetActivationTime(ActivationTime.BonusAction)
            //     .SetEffectDescription(FeatureDefinitionPowers.PowerOathOfDevotionSacredWeapon.EffectDescription.Copy()
            //         .SetDuration(DurationType.Minute, 1)
            //         .SetEffectForms(EffectFormBuilder
            //             .Create()
            //             .SetConditionForm(weaponCondition, ConditionForm.ConditionOperation.Add, false, true)
            //             .Build())
            //         .AddEffectForms(FeatureDefinitionPowers.PowerOathOfDevotionSacredWeapon.EffectDescription.EffectForms)
            //     )
            //     .AddToDB();

            var empowerWeaponPower = FeatureDefinitionPowerBuilder
                .Create("AHWarlockSoulBladePactEmpowerWeaponPower", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Feature, FeatureDefinitionPowers.PowerOathOfDevotionSacredWeapon.GuiPresentation.SpriteReference)
                .SetShortTitleOverride("Feature/&AHWarlockSoulBladePactEmpowerWeaponPowerTitle")
                .SetRechargeRate(RechargeRate.LongRest)
                .SetFixedUsesPerRecharge(1)
                .SetCostPerUse(1)
                .SetActivationTime(ActivationTime.Action)
                .SetAttackModifierAbility(true,true, AttributeDefinitions.Charisma)
                .SetEffectDescription(new EffectDescriptionBuilder()
                    .SetDurationData(DurationType.UntilLongRest)
                    .SetTargetingData(Side.Ally, 
                        RangeType.Self, 
                        1, 
                        TargetType.Item, 
                        1, 
                        1,
                        ActionDefinitions.ItemSelectionType.Weapon
                    )
                    .AddEffectForms(new EffectFormBuilder()
                        .SetItemPropertyForm(
                            ItemPropertyUsage.Unlimited,
                            1, new FeatureUnlockByLevel(FeatureDefinitionAttackModifiers.AttackModifierShillelagh, 0)
                        )
                        .Build()
                    )
                    .Build()
                )
                .AddToDB();

            var shieldPower = FeatureDefinitionPowerBuilder
                .Create(FeatureDefinitionPowers.PowerFighterSecondWind, "AHWarlockSoulBladePactSoulShieldPower", DefinitionBuilder.CENamespaceGuid)
                .SetOrUpdateGuiPresentation(Category.Feature)
                .SetRechargeRate(RechargeRate.ShortRest)
                .SetFixedUsesPerRecharge(1)
                .SetCostPerUse(1)
                .SetActivationTime(ActivationTime.BonusAction)
                .SetAbilityScore(AttributeDefinitions.Charisma)
                .SetEffectDescription(FeatureDefinitionPowers.PowerFighterSecondWind.EffectDescription
                    .Copy()
                    .SetEffectForms(EffectFormBuilder
                        .Create()
                        .SetTempHPForm(-1, DieType.D1, 1)
                        .SetBonusMode(AddBonusMode.AbilityBonus)
                        .SetLevelAdvancement(EffectForm.LevelApplianceType.AddBonus, LevelSourceType.ClassLevel, 1)
                        .Build())
                    .SetDurationType(DurationType.UntilLongRest))
                .AddToDB();

            var spellList = SpellListDefinitionBuilder
                .Create(SpellListDefinitions.SpellListPaladin, "AHWarlockSoulBladePactSpellList", DefinitionBuilder.CENamespaceGuid)
                .SetOrUpdateGuiPresentation(Category.Feature)
                .ClearSpells()
                .SetSpellsAtLevel(1, Shield, FalseLife)
                .SetSpellsAtLevel(2, Blur, BrandingSmite)
                .SetSpellsAtLevel(3, Haste, Slow)
                .SetSpellsAtLevel(4, PhantasmalKiller, BlackTentacles)
                .SetSpellsAtLevel(5, ConeOfCold, MindTwist)
                .FinalizeSpells()
                .AddToDB();

            var extendedSpellList = FeatureDefinitionMagicAffinityBuilder
                .Create(FeatureDefinitionMagicAffinitys.MagicAffinityGreenmageGreenMagicList, "AHWarlockSoulBladePactExtendedSpellList", DefinitionBuilder.CENamespaceGuid)
                .SetOrUpdateGuiPresentation(Category.Feature)
                .SetExtendedSpellList(spellList)
                .AddToDB();

            var proficiencySoulBladeArmor = FeatureDefinitionProficiencyBuilder
                .Create(ProficiencyClericArmor, "ProficiencySoulBladeArmor", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Feature)
                .AddToDB();

            var proficiencySoulBladeWeapon = FeatureDefinitionProficiencyBuilder
                .Create(ProficiencyFighterWeapon, "ProficiencySoulBladeWeapon", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Feature)
                .AddToDB();

            return CharacterSubclassDefinitionBuilder
                .Create("AHWarlockSubclassSoulBladePact", DefinitionBuilder.CENamespaceGuid)
                .SetOrUpdateGuiPresentation(Category.Subclass, CharacterSubclassDefinitions.OathOfTheMotherland.GuiPresentation.SpriteReference)
                .AddFeatureAtLevel(extendedSpellList, 1) // Extra Soulblade spells
                .AddFeatureAtLevel(proficiencySoulBladeArmor, 1) // Martial weapons
                .AddFeatureAtLevel(proficiencySoulBladeWeapon, 1) // Medium armor and shield
                .AddFeatureAtLevel(empowerWeaponPower, 1) //Feature to rival hexblade curse
                .AddFeatureAtLevel(summonPactWeaponPower, 6)
                .AddFeatureAtLevel(shieldPower, 10)
                .AddToDB();
        }
    }
}
