using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.BehaviorsSpecific;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Subclasses.CommonBuilders;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class CollegeOfLife : AbstractSubclass
{
    private const string Name = "CollegeOfLife";

    public CollegeOfLife()
    {
        // LEVEL 03

        MagicAffinityCollegeOfLifeHeightened = FeatureDefinitionMagicAffinityBuilder
            .Create($"MagicAffinity{Name}Heightened")
            .SetGuiPresentation(Category.Feature)
            .SetWarList(2) // we set spells on later load
            .AddToDB();

        // LEVEL 06

        var damageAffinityCollegeOfLifeNecroticResistance = FeatureDefinitionDamageAffinityBuilder
            .Create(DamageAffinityNecroticResistance, $"DamageAffinity{Name}NecroticResistance")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var powerSharedPoolCollegeOfLifeHealingPool = FeatureDefinitionPowerBuilder
            .Create($"PowerSharedPool{Name}HealingPool")
            .SetGuiPresentation(Category.Feature, FeatureDefinitionPowers.PowerPaladinLayOnHands)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .AddToDB();

        var conditionCollegeOfLifeDarkvision = ConditionDefinitionBuilder
            .Create($"Condition{Name}Darkvision")
            .SetGuiPresentation($"PowerSharedPool{Name}Darkvision", Category.Feature, ConditionDarkvision)
            .SetFeatures(FeatureDefinitionSenses.SenseSuperiorDarkvision)
            .AddToDB();

        var powerSharedPoolCollegeOfLifeDarkvision = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"PowerSharedPool{Name}Darkvision")
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
                            .SetConditionForm(conditionCollegeOfLifeDarkvision, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        var conditionCollegeOfLifePoison = ConditionDefinitionBuilder
            .Create($"Condition{Name}ElementalResistance")
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
            .Create($"PowerSharedPool{Name}ElementalResistance")
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
                            .SetConditionForm(conditionCollegeOfLifePoison, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        var conditionCollegeOfLifeConstitution = ConditionDefinitionBuilder
            .Create($"Condition{Name}Constitution")
            .SetGuiPresentation(Category.Condition, ConditionBearsEndurance)
            .SetFeatures(FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityCreedOfArun)
            .AddToDB();

        var powerSharedPoolCollegeOfLifeConstitution = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"PowerSharedPool{Name}Constitution")
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
                            .SetConditionForm(conditionCollegeOfLifeConstitution, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        var powerSharedPoolCollegeOfLifeFly = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"PowerSharedPool{Name}Fly")
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
                            .SetConditionForm(ConditionFlying12, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        var powerSharedPoolCollegeOfLifeHeal = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"PowerSharedPool{Name}Heal")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.BonusAction, powerSharedPoolCollegeOfLifeHealingPool)
            .SetEffectDescription(MassHealingWord.EffectDescription)
            .SetExplicitAbilityScore(AttributeDefinitions.Charisma)
            .AddToDB();

        var powerSharedPoolCollegeOfLifeRevive = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"PowerSharedPool{Name}Revive")
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
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.CollegeOfLife, 256))
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

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Bard;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceBardColleges;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    internal static void LateLoad()
    {
        foreach (var spellDefinition in SpellListDefinitions.SpellListAllSpells
                     .SpellsByLevel
                     .SelectMany(x => x.Spells)
                     .Where(x => x.SchoolOfMagic is SchoolNecromancy or SchoolTransmutation))
        {
            if (spellDefinition.SpellsBundle)
            {
                foreach (var spellInBundle in spellDefinition.SubspellsList)
                {
                    MagicAffinityCollegeOfLifeHeightened.WarListSpells.Add(spellInBundle.Name);
                }
            }
            else
            {
                MagicAffinityCollegeOfLifeHeightened.WarListSpells.Add(spellDefinition.Name);
            }
        }
    }
}
