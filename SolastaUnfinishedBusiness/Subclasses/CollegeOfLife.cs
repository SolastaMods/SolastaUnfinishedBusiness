using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Subclasses.CommonBuilders;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class CollegeOfLife : AbstractSubclass
{
    internal CollegeOfLife()
    {
        // LEVEL 03

        var magicAffinityCollegeOfLifeHeightened = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityCollegeOfLifeHeightened")
            .SetGuiPresentation(Category.Feature)
            .SetWarList(2,
                // Necromancy
                BestowCurse,
                Blindness,
                Eyebite,
                RaiseDead,
                // Transmutation
                EnhanceAbility,
                FeatherFall,
                HeatMetal,
                Knock,
                Longstrider)
            .AddToDB();

        // LEVEL 06

        var damageAffinityCollegeOfLifeNecroticResistance = FeatureDefinitionDamageAffinityBuilder
            .Create(DamageAffinityNecroticResistance, "DamageAffinityCollegeOfLifeNecroticResistance")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var powerSharedPoolCollegeOfLifeHealingPool = FeatureDefinitionPowerBuilder
            .Create("PowerSharedPoolCollegeOfLifeHealingPool")
            .SetGuiPresentation(Category.Feature, hidden: true)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .AddToDB();

        var conditionCollegeOfLifeDarkvision = ConditionDefinitionBuilder
            .Create("ConditionCollegeOfLifeDarkvision")
            .SetGuiPresentation("PowerSharedPoolCollegeOfLifeDarkvision", Category.Feature, ConditionDarkvision)
            .SetFeatures(FeatureDefinitionSenses.SenseSuperiorDarkvision)
            .AddToDB();

        var powerSharedPoolCollegeOfLifeDarkvision = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerSharedPoolCollegeOfLifeDarkvision")
            .SetGuiPresentation(Category.Feature, PowerDomainBattleDivineWrath)
            .SetSharedPool(ActivationTime.BonusAction, powerSharedPoolCollegeOfLifeHealingPool)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Touch, 2, TargetType.IndividualsUnique)
                    .SetDurationData(DurationType.UntilLongRest)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .CreatedByCharacter()
                            .SetConditionForm(conditionCollegeOfLifeDarkvision, ConditionForm.ConditionOperation.Add,
                                false, false)
                            .Build())
                    .Build())
            .AddToDB();

        var conditionCollegeOfLifePoison = ConditionDefinitionBuilder
            .Create("ConditionCollegeOfLifeElementalResistance")
            .SetGuiPresentation(Category.Condition, ConditionProtectedFromPoison)
            .SetFeatures(
                DamageAffinityAcidResistance,
                DamageAffinityColdResistance,
                DamageAffinityFireResistance,
                DamageAffinityLightningResistance,
                DamageAffinityNecroticResistance,
                DamageAffinityPoisonResistance,
                DamageAffinityThunderResistance)
            .AddToDB();

        var powerSharedPoolCollegeOfLifePoison = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerSharedPoolCollegeOfLifeElementalResistance")
            .SetGuiPresentation(Category.Feature, PowerDomainElementalFireBurst)
            .SetSharedPool(ActivationTime.BonusAction, powerSharedPoolCollegeOfLifeHealingPool)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Touch, 2, TargetType.IndividualsUnique)
                    .SetDurationData(DurationType.UntilLongRest)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .CreatedByCharacter()
                            .SetConditionForm(conditionCollegeOfLifePoison, ConditionForm.ConditionOperation.Add,
                                false, false)
                            .Build())
                    .Build())
            .AddToDB();

        var conditionCollegeOfLifeConstitution = ConditionDefinitionBuilder
            .Create("ConditionCollegeOfLifeConstitution")
            .SetGuiPresentation(Category.Condition, ConditionBearsEndurance)
            .SetFeatures(FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityCreedOfArun)
            .AddToDB();

        var powerSharedPoolCollegeOfLifeConstitution = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerSharedPoolCollegeOfLifeConstitution")
            .SetGuiPresentation(Category.Feature, PowerPaladinAuraOfCourage)
            .SetSharedPool(ActivationTime.BonusAction, powerSharedPoolCollegeOfLifeHealingPool)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Touch, 2, TargetType.IndividualsUnique)
                    .SetDurationData(DurationType.UntilLongRest)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .CreatedByCharacter()
                            .SetConditionForm(conditionCollegeOfLifeConstitution, ConditionForm.ConditionOperation.Add,
                                false, false)
                            .Build())
                    .Build())
            .AddToDB();

        var powerSharedPoolCollegeOfLifeFly = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerSharedPoolCollegeOfLifeFly")
            .SetGuiPresentation(Category.Feature, Fly)
            .SetSharedPool(ActivationTime.BonusAction, powerSharedPoolCollegeOfLifeHealingPool)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Touch, 2, TargetType.IndividualsUnique)
                    .SetDurationData(DurationType.UntilLongRest)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .CreatedByCharacter()
                            .SetConditionForm(ConditionFlying12, ConditionForm.ConditionOperation.Add, false, false)
                            .Build())
                    .Build())
            .AddToDB();

        var powerSharedPoolCollegeOfLifeHeal = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerSharedPoolCollegeOfLifeHeal")
            .SetGuiPresentation(Category.Feature, MassHealingWord)
            .SetSharedPool(ActivationTime.BonusAction, powerSharedPoolCollegeOfLifeHealingPool)
            .SetEffectDescription(MassHealingWord.EffectDescription)
            .AddToDB();

        var powerSharedPoolCollegeOfLifeRevive = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerSharedPoolCollegeOfLifeRevive")
            .SetGuiPresentation(Category.Feature, Revivify)
            .SetSharedPool(ActivationTime.BonusAction, powerSharedPoolCollegeOfLifeHealingPool)
            .SetEffectDescription(Revivify.EffectDescription)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("CollegeOfLife")
            .SetGuiPresentation(Category.Subclass, RoguishDarkweaver)
            .AddFeaturesAtLevel(3,
                magicAffinityCollegeOfLifeHeightened)
            .AddFeaturesAtLevel(6,
                damageAffinityCollegeOfLifeNecroticResistance,
                powerSharedPoolCollegeOfLifeHealingPool,
                powerSharedPoolCollegeOfLifeDarkvision,
                powerSharedPoolCollegeOfLifePoison,
                powerSharedPoolCollegeOfLifeConstitution,
                powerSharedPoolCollegeOfLifeFly,
                powerSharedPoolCollegeOfLifeHeal,
                powerSharedPoolCollegeOfLifeRevive)
            .AddFeaturesAtLevel(14,
                DamageAffinityGenericHardenToNecrotic,
                PowerCasterCommandUndead)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceBardColleges;
}
