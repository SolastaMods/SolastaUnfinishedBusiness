using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionProficiencys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class PatronSoulBlade : AbstractSubclass
{
    // ReSharper disable once InconsistentNaming
    private readonly CharacterSubclassDefinition Subclass;

    internal PatronSoulBlade()
    {
        var powerSoulBladeSummonPactWeapon = FeatureDefinitionPowerBuilder
            .Create(FeatureDefinitionPowers.PowerTraditionShockArcanistArcaneFury, "PowerSoulBladeSummonPactWeapon")
            .SetOrUpdateGuiPresentation(Category.Feature, SpiritualWeapon.GuiPresentation.SpriteReference)
            .SetEffectDescription(SpiritualWeapon.EffectDescription.Copy())
            .SetRechargeRate(RechargeRate.ShortRest)
            .SetActivationTime(ActivationTime.NoCost)
            .AddToDB();

        powerSoulBladeSummonPactWeapon.EffectDescription.savingThrowDifficultyAbility = AttributeDefinitions.Charisma;

        var attackModifierSoulBladeEmpowerWeapon = FeatureDefinitionAttackModifierBuilder
            .Create("AttackModifierSoulBladeEmpowerWeapon")
            .SetGuiPresentation(Category.Feature,
                FeatureDefinitionPowers.PowerOathOfDevotionSacredWeapon.GuiPresentation.SpriteReference)
            .SetAbilityScoreReplacement(AbilityScoreReplacement.SpellcastingAbility)
            .AddToDB();

        var powerSoulBladeEmpowerWeapon = FeatureDefinitionPowerBuilder
            .Create("PowerSoulBladeEmpowerWeapon")
            .SetGuiPresentation(Category.Feature,
                FeatureDefinitionPowers.PowerOathOfDevotionSacredWeapon.GuiPresentation.SpriteReference)
            .Configure(
                1,
                UsesDetermination.Fixed,
                AttributeDefinitions.Charisma,
                ActivationTime.Action,
                1,
                RechargeRate.LongRest,
                true,
                true,
                AttributeDefinitions.Charisma,
                new EffectDescriptionBuilder()
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
                            1, new FeatureUnlockByLevel(attackModifierSoulBladeEmpowerWeapon, 0)
                        )
                        .Build()
                    )
                    .Build()
            )
            .AddToDB();

        var powerSoulBladeSoulShield = FeatureDefinitionPowerBuilder
            .Create(FeatureDefinitionPowers.PowerFighterSecondWind, "PowerSoulBladeSoulShield")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .Configure(
                1,
                UsesDetermination.Fixed,
                AttributeDefinitions.Charisma,
                ActivationTime.BonusAction,
                1,
                RechargeRate.ShortRest,
                false,
                false,
                AttributeDefinitions.Charisma,
                FeatureDefinitionPowers.PowerFighterSecondWind.EffectDescription
                    .Copy()
                    .SetEffectForms(EffectFormBuilder
                        .Create()
                        .SetTempHPForm(-1, DieType.D1, 1)
                        .SetBonusMode(AddBonusMode.AbilityBonus)
                        .SetLevelAdvancement(EffectForm.LevelApplianceType.AddBonus, LevelSourceType.ClassLevel)
                        .Build())
                    .SetDurationType(DurationType.UntilLongRest))
            .AddToDB();

        var spellListSoulBlade = SpellListDefinitionBuilder
            .Create(SpellListDefinitions.SpellListPaladin, "SpellListSoulBlade")
            .SetGuiPresentationNoContent(true)
            .ClearSpells()
            .SetSpellsAtLevel(1, Shield, FalseLife)
            .SetSpellsAtLevel(2, Blur, BrandingSmite)
            .SetSpellsAtLevel(3, Haste, Slow)
            .SetSpellsAtLevel(4, PhantasmalKiller, BlackTentacles)
            .SetSpellsAtLevel(5, ConeOfCold, MindTwist)
            .FinalizeSpells()
            .AddToDB();

        var magicAffinitySoulBladeExpandedSpells = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinitySoulBladeExpandedSpells")
            .SetOrUpdateGuiPresentation("MagicAffinityPatronExpandedSpells", Category.Feature)
            .SetExtendedSpellList(spellListSoulBlade)
            .AddToDB();

        var proficiencySoulBladeArmor = FeatureDefinitionProficiencyBuilder
            .Create(ProficiencyClericArmor, "ProficiencySoulBladeArmor")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var proficiencySoulBladeWeapon = FeatureDefinitionProficiencyBuilder
            .Create(ProficiencyFighterWeapon, "ProficiencySoulBladeWeapon")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("PatronSoulBlade")
            .SetOrUpdateGuiPresentation(Category.Subclass,
                CharacterSubclassDefinitions.OathOfTheMotherland.GuiPresentation.SpriteReference)
            .AddFeaturesAtLevel(1,
                proficiencySoulBladeArmor,
                proficiencySoulBladeWeapon,
                magicAffinitySoulBladeExpandedSpells,
                powerSoulBladeEmpowerWeapon)
            .AddFeaturesAtLevel(6, powerSoulBladeSummonPactWeapon)
            .AddFeaturesAtLevel(10, powerSoulBladeSoulShield)
            .AddToDB();
    }

    internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
    {
        return FeatureDefinitionSubclassChoices.SubclassChoiceWarlockOtherworldlyPatrons;
    }

    internal override CharacterSubclassDefinition GetSubclass()
    {
        return Subclass;
    }
}
