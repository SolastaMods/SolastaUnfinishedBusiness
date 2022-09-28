using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Utils;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.DamageDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.MonsterDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class PatronElementalist : AbstractSubclass
{
    private const string Name = "PatronElementalist";

    // think about making smaller base pool of elements, with ability to expand via eldritch Invocations
    private static readonly Dictionary<string, ElementalFormConfig> ElementalFormConfigs = new()
    {
        {
            "Fire", new ElementalFormConfig
            {
                DamageType = DamageFire,
                Resistance = DamageAffinityFireResistance,
                Immunity = DamageAffinityFireImmunity,
                Particles = Fire_Jester.MonsterPresentation.attachedParticlesReference,
                Shaders = Fire_Elemental.MonsterPresentation.CustomShaderReference
                // Sprite = DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalHeraldOfTheElementsFire.GuiPresentation.SpriteReference,
            }
        },
        {
            "Earth", new ElementalFormConfig
            {
                DamageType = DamageBludgeoning,
                Resistance = DamageAffinityBludgeoningResistance,
                Immunity = DamageAffinityBludgeoningImmunity,
                // or stone barbarian's ConditionStoneResilience.conditionParticleReference,
                Particles = Earth_Elemental.MonsterPresentation.attachedParticlesReference,
                // or stone barbarian's ConditionStoneResilience.conditionParticleReference,
                Shaders = FeyBear.MonsterPresentation.CustomShaderReference
                // Sprite = DatabaseHelper.FeatureDefinitionPowers.PowerDomainBattleDivineWrath.GuiPresentation.SpriteReference,
            }
        },
        {
            "Ice", new ElementalFormConfig
            {
                DamageType = DamageCold,
                Resistance = DamageAffinityColdResistance,
                Immunity = DamageAffinityColdImmunity,
                Particles = WindSnake.MonsterPresentation.attachedParticlesReference,
                Shaders = SkarnGhoul.MonsterPresentation.CustomShaderReference
                // Sprite = DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalHeraldOfTheElementsCold.GuiPresentation.SpriteReference,
            }
        },
        {
            "Air", new ElementalFormConfig
            {
                DamageType = DamageThunder,
                Resistance = DamageAffinityThunderResistance,
                Immunity = DamageAffinityThunderImmunity,
                Particles = Air_Elemental.MonsterPresentation.attachedParticlesReference,
                Shaders = Air_Elemental.MonsterPresentation.CustomShaderReference
                // Sprite = DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalHeraldOfTheElementsThunder.GuiPresentation.SpriteReference,
            }
        }
    };

    // ReSharper disable once InconsistentNaming
    private readonly CharacterSubclassDefinition Subclass;

    internal PatronElementalist()
    {
        var iconRegular = CustomIcons.CreateAssetReferenceSprite("ElementalFormIcon",
            Resources.ElementalFormIcon, 24, 24);
        var iconEnhanced = CustomIcons.CreateAssetReferenceSprite("ElementalFormIconEnhanced",
            Resources.ElementalFormIconEnhanced, 24, 24);
        var formRegular = CustomIcons.CreateAssetReferenceSprite("ElementalForm", Resources.ElementalForm, 128, 64);
        var formEnhanced =
            CustomIcons.CreateAssetReferenceSprite("ElementalFormEnhanced", Resources.ElementalForm, 128, 64);

        var powerElementalistElementalFormPool = FeatureDefinitionPowerPoolBuilder
            .Create("PowerElementalistElementalFormPool")
            .Configure(
                1,
                UsesDetermination.Fixed,
                AttributeDefinitions.Charisma,
                ActivationTime.BonusAction,
                1,
                RechargeRate.LongRest,
                false,
                false,
                AttributeDefinitions.Charisma,
                new EffectDescription())
            .SetGuiPresentation(Category.Feature, formRegular)
            .SetUsesProficiency()
            .SetRechargeRate(RechargeRate.LongRest)
            .AddToDB();

        var powerElementalistElementalEnhancedFormPool = FeatureDefinitionPowerPoolBuilder
            .Create("PowerElementalistElementalEnhancedFormPool")
            .Configure(
                1,
                UsesDetermination.Fixed,
                AttributeDefinitions.Charisma,
                ActivationTime.BonusAction,
                1,
                RechargeRate.LongRest,
                false,
                false,
                AttributeDefinitions.Charisma,
                new EffectDescription())
            .SetGuiPresentation(Category.Feature, formEnhanced)
            .SetUsesProficiency()
            .SetOverriddenPower(powerElementalistElementalFormPool)
            .AddToDB();

        var regularPowers = new List<FeatureDefinitionPower>();
        var enhancedPowers = new List<FeatureDefinitionPower>();

        foreach (var e in ElementalFormConfigs)
        {
            var (regularPower, enhancedPower) = BuildElementalForm(
                e.Key,
                powerElementalistElementalFormPool,
                powerElementalistElementalEnhancedFormPool,
                e.Value,
                iconRegular,
                iconEnhanced);

            regularPowers.Add(regularPower);
            enhancedPowers.Add(enhancedPower);
        }

        PowersBundleContext.RegisterPowerBundle(powerElementalistElementalFormPool, true, regularPowers.ToArray());
        PowersBundleContext.RegisterPowerBundle(powerElementalistElementalEnhancedFormPool, true,
            enhancedPowers.ToArray());

        var spellListElementalist = SpellListDefinitionBuilder
            .Create(DatabaseHelper.SpellListDefinitions.SpellListPaladin, "SpellListElementalist")
            .SetGuiPresentationNoContent(true)
            .ClearSpells()
            .SetSpellsAtLevel(0, FireBolt, RayOfFrost, ShockingGrasp)
            .SetSpellsAtLevel(1, BurningHands, Thunderwave, FogCloud)
            .SetSpellsAtLevel(2, FlamingSphere, ScorchingRay, HeatMetal)
            .SetSpellsAtLevel(3, Fireball, LightningBolt, SleetStorm)
            .SetSpellsAtLevel(4, Stoneskin, IceStorm, WallOfFire)
            .SetSpellsAtLevel(5, ConeOfCold, FlameStrike, ConjureElemental)
            .FinalizeSpells()
            .AddToDB();

        var magicAffinityElementalistExpandedSpells = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityElementalistExpandedSpells")
            .SetGuiPresentation("MagicAffinityPatronExpandedSpells", Category.Feature)
            .SetExtendedSpellList(spellListElementalist)
            .AddToDB();

        var featureSetElementalistKnowledge = FeatureDefinitionFeatureSetBuilder
            .Create(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetGreenmageWardenOfTheForest,
                "FeatureSetElementalistKnowledge")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageRangerFavoredEnemyElemental)
            .AddFeatureSet(DatabaseHelper.FeatureDefinitionCombatAffinitys.CombatAffinityProtectedFromEvil)
            .AddFeatureSet(DatabaseHelper.FeatureDefinitionConditionAffinitys
                .ConditionAffinityCircleLandNaturesWardCharmed)
            .AddFeatureSet(DatabaseHelper.FeatureDefinitionConditionAffinitys
                .ConditionAffinityCircleLandNaturesWardFrightened)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .SetUniqueChoices(false)
            .AddToDB();

        var conjureMinorElementalsAtWill = SpellDefinitionBuilder
            .Create(ConjureMinorElementals, "ConjureMinorElementalAtWill")
            .SetSpellLevel(0)
            .AddToDB();

        var minorElementalBonusCantrip = FeatureDefinitionBonusCantripsBuilder
            .Create("BonusCantripElementalistMinorElemental")
            .SetGuiPresentation(Category.Feature)
            .AddBonusCantrip(conjureMinorElementalsAtWill)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder.Create(Name)
            .SetGuiPresentation(Category.Subclass,
                DatabaseHelper.CharacterSubclassDefinitions.TraditionLoremaster.GuiPresentation.SpriteReference)
            .AddFeaturesAtLevel(1, magicAffinityElementalistExpandedSpells, powerElementalistElementalFormPool)
            .AddFeaturesAtLevel(6, featureSetElementalistKnowledge)
            .AddFeaturesAtLevel(10, powerElementalistElementalEnhancedFormPool)
            .AddFeaturesAtLevel(14, minorElementalBonusCantrip)
            .AddToDB();
    }

    internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
    {
        return DatabaseHelper.FeatureDefinitionSubclassChoices.SubclassChoiceWarlockOtherworldlyPatrons;
    }

    internal override CharacterSubclassDefinition GetSubclass()
    {
        return Subclass;
    }

    private static GuiPresentation GuiPresentation(
        string type,
        string text,
        ElementalFormConfig elementalFormConfig,
        AssetReferenceSprite sprite = null)
    {
        var damageType = elementalFormConfig.DamageType.GuiPresentation.Title;
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
            .SetGuiPresentation(GuiPresentation("ElementalDamage", text, elementalFormConfig))
            .Configure(
                "ElementalDamage",
                FeatureLimitedUsage.OncePerTurn,
                AdditionalDamageValueDetermination.ProficiencyBonus,
                AdditionalDamageTriggerCondition.SpellDamagesTarget,
                RestrictedContextRequiredProperty.None,
                false,
                DieType.D4,
                1,
                AdditionalDamageType.Specific,
                elementalFormConfig.DamageType.Name,
                AdditionalDamageAdvancement.None,
                new List<DiceByRank>()
            )
            .AddToDB();

        var conditionElementalistNormal = ConditionDefinitionBuilder
            .Create("ConditionElementalistNormal" + text)
            .SetGuiPresentation(GuiPresentation("ElementalCondition", text, elementalFormConfig, iconRegular))
            .SetDuration(DurationType.Minute, 1)
            .SetSilent(Silent.None)
            .AddFeatures(elementalFormConfig.Resistance, additionalDamageElementalist)
            .SetConditionParticleReference(elementalFormConfig.Particles)
            .AddToDB();

        var powerSharedPoolElementalistNormal = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerSharedPoolElementalistNormal" + text)
            .SetGuiPresentation(GuiPresentation("ElementalForm", text, elementalFormConfig))
            .Configure(
                elementalFormPool,
                RechargeRate.LongRest,
                ActivationTime.NoCost,
                1,
                false,
                false,
                AttributeDefinitions.Charisma,
                new EffectDescriptionBuilder()
                    .SetDurationData(DurationType.Minute, 1, TurnOccurenceType.EndOfTurn)
                    .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self)
                    .AddEffectForm(new EffectFormBuilder()
                        .SetConditionForm(
                            conditionElementalistNormal,
                            ConditionForm.ConditionOperation.Add,
                            true,
                            true
                        )
                        .Build()
                    )
                    .Build(),
                true)
            .AddToDB();

        var conditionElementalistEnhanced = ConditionDefinitionBuilder
            .Create("ConditionElementalistEnhanced" + text)
            .SetGuiPresentation(GuiPresentation("ElementalCondition", text, elementalFormConfig, iconEnhanced))
            .SetDuration(DurationType.Minute, 1)
            .SetSilent(Silent.None)
            .AddFeatures(elementalFormConfig.Immunity, additionalDamageElementalist)
            .SetConditionParticleReference(elementalFormConfig.Particles)
            .SetCharacterShaderReference(elementalFormConfig.Shaders)
            .AddToDB();

        var powerSharedPoolElementalistEnhanced = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerSharedPoolElementalistEnhanced" + text)
            .SetGuiPresentation(GuiPresentation("ElementalFormEnhanced", text, elementalFormConfig))
            .SetOverriddenPower(powerSharedPoolElementalistNormal)
            .Configure(
                enhancedElementalFormPool,
                RechargeRate.LongRest,
                ActivationTime.NoCost,
                1,
                false,
                false,
                AttributeDefinitions.Charisma,
                new EffectDescriptionBuilder()
                    .SetDurationData(DurationType.Minute, 1, TurnOccurenceType.EndOfTurn)
                    .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self)
                    .AddEffectForm(new EffectFormBuilder()
                        .SetConditionForm(conditionElementalistEnhanced,
                            ConditionForm.ConditionOperation.Add, true, true)
                        .Build()
                    ).Build(),
                true)
            .AddToDB();

        return (powerSharedPoolElementalistNormal, powerSharedPoolElementalistEnhanced);
    }

    private sealed class ElementalFormConfig
    {
        internal DamageDefinition DamageType;
        internal FeatureDefinitionDamageAffinity Immunity;

        internal AssetReference Particles;
        internal FeatureDefinitionDamageAffinity Resistance;

        internal AssetReference Shaders;
        // internal AssetReferenceSprite Sprite;
    }
}
