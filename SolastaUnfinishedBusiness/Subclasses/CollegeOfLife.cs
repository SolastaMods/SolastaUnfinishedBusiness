using System.Linq;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Subclasses.CommonBuilders;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class CollegeOfLife : AbstractSubclass
{
    internal CollegeOfLife()
    {
        // LEVEL 03

        MagicAffinityCollegeOfLifeHeightened = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityCollegeOfLifeHeightened")
            .SetGuiPresentation(Category.Feature)
            .SetWarList(2) // we set spells on later load
            .AddToDB();

        // LEVEL 06

        var damageAffinityCollegeOfLifeNecroticResistance = FeatureDefinitionDamageAffinityBuilder
            .Create(DamageAffinityNecroticResistance, "DamageAffinityCollegeOfLifeNecroticResistance")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var powerSharedPoolCollegeOfLifeHealingPool = FeatureDefinitionPowerBuilder
            .Create("PowerSharedPoolCollegeOfLifeHealingPool")
            .SetGuiPresentation(Category.Feature)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .AddToDB();

        var conditionCollegeOfLifeDarkvision = ConditionDefinitionBuilder
            .Create("ConditionCollegeOfLifeDarkvision")
            .SetGuiPresentation("PowerSharedPoolCollegeOfLifeDarkvision", Category.Feature, ConditionDarkvision)
            .SetFeatures(FeatureDefinitionSenses.SenseSuperiorDarkvision)
            .AddToDB();

        var powerSharedPoolCollegeOfLifeDarkvision = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerSharedPoolCollegeOfLifeDarkvision")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.BonusAction, powerSharedPoolCollegeOfLifeHealingPool)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Touch, 2, TargetType.IndividualsUnique)
                    .SetDurationData(DurationType.UntilLongRest)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
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
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.BonusAction, powerSharedPoolCollegeOfLifeHealingPool)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Touch, 2, TargetType.IndividualsUnique)
                    .SetDurationData(DurationType.UntilLongRest)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
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
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.BonusAction, powerSharedPoolCollegeOfLifeHealingPool)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Touch, 2, TargetType.IndividualsUnique)
                    .SetDurationData(DurationType.UntilLongRest)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionCollegeOfLifeConstitution, ConditionForm.ConditionOperation.Add,
                                false, false)
                            .Build())
                    .Build())
            .AddToDB();

        var powerSharedPoolCollegeOfLifeFly = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerSharedPoolCollegeOfLifeFly")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.BonusAction, powerSharedPoolCollegeOfLifeHealingPool)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Touch, 2, TargetType.IndividualsUnique)
                    .SetDurationData(DurationType.UntilAnyRest)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(ConditionFlying12, ConditionForm.ConditionOperation.Add, false, false)
                            .Build())
                    .Build())
            .AddToDB();

        var powerSharedPoolCollegeOfLifeHeal = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerSharedPoolCollegeOfLifeHeal")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.BonusAction, powerSharedPoolCollegeOfLifeHealingPool)
            .SetEffectDescription(MassHealingWord.EffectDescription)
            .AddToDB();

        var powerSharedPoolCollegeOfLifeRevive = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerSharedPoolCollegeOfLifeRevive")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.BonusAction, powerSharedPoolCollegeOfLifeHealingPool)
            .SetEffectDescription(Revivify.EffectDescription)
            .AddToDB();

        PowerBundle.RegisterPowerBundle(powerSharedPoolCollegeOfLifeHealingPool, false,
            powerSharedPoolCollegeOfLifeDarkvision,
            powerSharedPoolCollegeOfLifePoison,
            powerSharedPoolCollegeOfLifeConstitution,
            powerSharedPoolCollegeOfLifeFly,
            powerSharedPoolCollegeOfLifeHeal,
            powerSharedPoolCollegeOfLifeRevive);

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("CollegeOfLife")
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite("CollegeOfLife", Resources.CollegeOfLife, 256))
            .AddFeaturesAtLevel(3,
                MagicAffinityCollegeOfLifeHeightened)
            .AddFeaturesAtLevel(6,
                damageAffinityCollegeOfLifeNecroticResistance,
                powerSharedPoolCollegeOfLifeHealingPool)
            .AddFeaturesAtLevel(14,
                DamageAffinityGenericHardenToNecrotic,
                PowerCasterCommandUndead)
            .AddToDB();
    }

    private static FeatureDefinitionMagicAffinity MagicAffinityCollegeOfLifeHeightened { get; set; }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceBardColleges;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    internal static void LateLoad()
    {
        MagicAffinityCollegeOfLifeHeightened.WarListSpells.SetRange(SpellListDefinitions.SpellListAllSpells
            .SpellsByLevel
            .SelectMany(x => x.Spells)
            .Where(x => x.SchoolOfMagic is SchoolNecromancy or SchoolTransmutation)
            .Select(x => x.Name));
    }
}
