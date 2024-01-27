using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.MonsterDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class PatronElementalist : AbstractSubclass
{
    private const string Name = "PatronElementalist";

    // think about making smaller base pool of elements, with ability to expand via eldritch Invocations
    private static readonly Dictionary<string, ElementalFormConfig> ElementalFormConfigs = new()
    {
        {
            "Fire", new ElementalFormConfig
            {
                DamageType = DamageTypeFire,
                Resistance = DamageAffinityFireResistance,
                Immunity = DamageAffinityFireImmunity,
                Particles = Fire_Jester.MonsterPresentation.attachedParticlesReference,
                Shaders = Fire_Elemental.MonsterPresentation.CustomShaderReference
                // Sprite = FeatureDefinitionPowers.PowerDomainElementalHeraldOfTheElementsFire,
            }
        },
        {
            "Earth", new ElementalFormConfig
            {
                DamageType = DamageTypeBludgeoning,
                Resistance = DamageAffinityBludgeoningResistance,
                Immunity = DamageAffinityBludgeoningImmunity,
                // or stone barbarian's ConditionStoneResilience.conditionParticleReference,
                Particles = Earth_Elemental.MonsterPresentation.attachedParticlesReference,
                // or stone barbarian's ConditionStoneResilience.conditionParticleReference,
                Shaders = FeyBear.MonsterPresentation.CustomShaderReference
                // Sprite = FeatureDefinitionPowers.PowerDomainBattleDivineWrath,
            }
        },
        {
            "Ice", new ElementalFormConfig
            {
                DamageType = DamageTypeCold,
                Resistance = DamageAffinityColdResistance,
                Immunity = DamageAffinityColdImmunity,
                Particles = WindSnake.MonsterPresentation.attachedParticlesReference,
                Shaders = SkarnGhoul.MonsterPresentation.CustomShaderReference
                // Sprite = FeatureDefinitionPowers.PowerDomainElementalHeraldOfTheElementsCold,
            }
        },
        {
            "Air", new ElementalFormConfig
            {
                DamageType = DamageTypeThunder,
                Resistance = DamageAffinityThunderResistance,
                Immunity = DamageAffinityThunderImmunity,
                Particles = Air_Elemental.MonsterPresentation.attachedParticlesReference,
                Shaders = Air_Elemental.MonsterPresentation.CustomShaderReference
                // Sprite = FeatureDefinitionPowers.PowerDomainElementalHeraldOfTheElementsThunder,
            }
        }
    };

    public PatronElementalist()
    {
        var spellListElementalist = SpellListDefinitionBuilder
            .Create(SpellListDefinitions.SpellListWizard, "SpellListElementalist")
            .SetGuiPresentationNoContent(true)
            .ClearSpells()
            .SetSpellsAtLevel(0, FireBolt, RayOfFrost)
            .SetSpellsAtLevel(1, BurningHands, Thunderwave)
            .SetSpellsAtLevel(2, FlamingSphere, ScorchingRay)
            .SetSpellsAtLevel(3, Fireball, LightningBolt)
            .SetSpellsAtLevel(4, IceStorm, WallOfFire)
            .SetSpellsAtLevel(5, ConeOfCold, FlameStrike)
            .FinalizeSpells(true, 9)
            .AddToDB();

        var magicAffinityElementalistExpandedSpells = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityElementalistExpandedSpells")
            .SetGuiPresentation("MagicAffinityPatronExpandedSpells", Category.Feature)
            .SetExtendedSpellList(spellListElementalist)
            .AddToDB();

        var iconRegular = Sprites.GetSprite(
            "ElementalFormIcon", Resources.ElementalFormIcon, 24, 24);
        var iconEnhanced = Sprites.GetSprite(
            "ElementalFormIconEnhanced", Resources.ElementalFormIconEnhanced, 24, 24);
        var formRegular = Sprites.GetSprite(
            "ElementalForm", Resources.ElementalForm, 128, 64);
        var formEnhanced = Sprites.GetSprite(
            "ElementalFormEnhanced", Resources.ElementalFormEnhanced, 128, 64);

        var powerElementalistElementalFormPool = FeatureDefinitionPowerBuilder
            .Create("PowerElementalistElementalFormPool")
            .SetGuiPresentation(Category.Feature, formRegular)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .SetBonusToAttack(true)
            .AddToDB();

        var powerElementalistElementalEnhancedFormPool = FeatureDefinitionPowerBuilder
            .Create("PowerElementalistElementalEnhancedFormPool")
            .SetGuiPresentation(Category.Feature, formEnhanced)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .SetBonusToAttack(true)
            .SetOverriddenPower(powerElementalistElementalFormPool)
            .AddToDB();

        var regularPowers = new FeatureDefinitionPower[ElementalFormConfigs.Count];
        var enhancedPowers = new FeatureDefinitionPower[ElementalFormConfigs.Count];

        for (var i = 0; i < ElementalFormConfigs.Count; i++)
        {
            var e = ElementalFormConfigs.ElementAt(i);
            var (regularPower, enhancedPower) = BuildElementalForm(
                e.Key,
                powerElementalistElementalFormPool,
                powerElementalistElementalEnhancedFormPool,
                e.Value,
                iconRegular,
                iconEnhanced);

            regularPowers[i] = regularPower;
            enhancedPowers[i] = enhancedPower;
        }

        PowerBundle.RegisterPowerBundle(powerElementalistElementalFormPool, true, regularPowers);
        PowerBundle.RegisterPowerBundle(powerElementalistElementalEnhancedFormPool, true, enhancedPowers);

        var featureSetElementalistKnowledge = FeatureDefinitionFeatureSetBuilder
            .Create(FeatureDefinitionFeatureSets.FeatureSetGreenmageWardenOfTheForest,
                "FeatureSetElementalistKnowledge")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                FeatureDefinitionAdditionalDamages.AdditionalDamageRangerFavoredEnemyElemental,
                FeatureDefinitionCombatAffinitys.CombatAffinityProtectedFromEvil,
                FeatureDefinitionConditionAffinitys.ConditionAffinityCircleLandNaturesWardCharmed,
                FeatureDefinitionConditionAffinitys.ConditionAffinityCircleLandNaturesWardFrightened)
            .AddToDB();

        var bonusCantripElementalistMinorElemental = FeatureDefinitionBonusCantripsBuilder
            .Create("BonusCantripElementalistMinorElemental")
            .SetGuiPresentation(Category.Feature)
            .SetBonusCantrips(
                SpellDefinitionBuilder
                    .Create(ConjureMinorElementals, "AtWillConjureMinorElemental")
                    .SetSpellLevel(0)
                    .AddToDB())
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder.Create(Name)
            .SetGuiPresentation(Category.Subclass,
                Sprites.GetSprite("PatronElementalist", Resources.PatronElementalist, 256))
            .AddFeaturesAtLevel(1,
                magicAffinityElementalistExpandedSpells,
                powerElementalistElementalFormPool)
            .AddFeaturesAtLevel(6,
                featureSetElementalistKnowledge)
            .AddFeaturesAtLevel(10,
                powerElementalistElementalEnhancedFormPool)
            .AddFeaturesAtLevel(14,
                bonusCantripElementalistMinorElemental)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Warlock;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice => FeatureDefinitionSubclassChoices
        .SubclassChoiceWarlockOtherworldlyPatrons;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private static GuiPresentation GuiPresentation(
        string type,
        string text,
        ElementalFormConfig elementalFormConfig,
        AssetReferenceSprite sprite = null)
    {
        var damageType = $"Rules/&{elementalFormConfig.DamageType}Title";
        var planeText = $"Feature/&ElementalPatron{text}Plane";

        return new GuiPresentationBuilder(
                Gui.Format($"Feature/&ElementalPatron{type}Title", planeText),
                Gui.Format($"Feature/&ElementalPatron{type}Description", planeText, damageType),
                sprite)
            .Build();
    }

    private static (FeatureDefinitionPower, FeatureDefinitionPower)
        BuildElementalForm(
            string text,
            FeatureDefinitionPower elementalFormPool,
            FeatureDefinitionPower enhancedElementalFormPool,
            ElementalFormConfig elementalFormConfig,
            AssetReferenceSprite iconRegular,
            AssetReferenceSprite iconEnhanced)
    {
        var additionalDamageElementalist = FeatureDefinitionAdditionalDamageBuilder
            .Create("AdditionalDamageElementalist" + text)
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("ElementalDamage")
            .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
            .SetTriggerCondition(AdditionalDamageTriggerCondition.SpellDamagesTarget)
            .SetDamageValueDetermination(AdditionalDamageValueDetermination.ProficiencyBonus)
            .SetSpecificDamageType(elementalFormConfig.DamageType)
            .AddToDB();

        var conditionElementalistNormal = ConditionDefinitionBuilder
            .Create("ConditionElementalistNormal" + text)
            .SetGuiPresentation(GuiPresentation("ElementalCondition", text, elementalFormConfig, iconRegular))
            .SetSilent(Silent.None)
            .AddFeatures(elementalFormConfig.Resistance, additionalDamageElementalist)
            .SetConditionParticleReference(elementalFormConfig.Particles)
            .AddToDB();

        var powerSharedPoolElementalistNormal = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerSharedPoolElementalistNormal" + text)
            .SetGuiPresentation(GuiPresentation("ElementalForm", text, elementalFormConfig))
            .SetSharedPool(ActivationTime.NoCost, elementalFormPool)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                conditionElementalistNormal,
                                ConditionForm.ConditionOperation.Add,
                                true,
                                true)
                            .Build())
                    .Build())
            .SetUniqueInstance()
            .AddToDB();

        var conditionElementalistEnhanced = ConditionDefinitionBuilder
            .Create("ConditionElementalistEnhanced" + text)
            .SetGuiPresentation(GuiPresentation("ElementalCondition", text, elementalFormConfig, iconEnhanced))
            .SetSilent(Silent.None)
            .AddFeatures(elementalFormConfig.Immunity, additionalDamageElementalist)
            .SetConditionParticleReference(elementalFormConfig.Particles)
            .SetCharacterShaderReference(elementalFormConfig.Shaders)
            .AddToDB();

        var powerSharedPoolElementalistEnhanced = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerSharedPoolElementalistEnhanced" + text)
            .SetGuiPresentation(GuiPresentation("ElementalFormEnhanced", text, elementalFormConfig))
            .SetSharedPool(ActivationTime.NoCost, enhancedElementalFormPool)
            .SetOverriddenPower(powerSharedPoolElementalistNormal)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionElementalistEnhanced,
                                ConditionForm.ConditionOperation.Add, true, true)
                            .Build())
                    .Build())
            .SetUniqueInstance()
            .AddToDB();

        return (powerSharedPoolElementalistNormal, powerSharedPoolElementalistEnhanced);
    }

    private sealed class ElementalFormConfig
    {
        internal string DamageType;
        internal FeatureDefinitionDamageAffinity Immunity;

        internal AssetReference Particles;
        internal FeatureDefinitionDamageAffinity Resistance;

        internal AssetReference Shaders;
    }
}
