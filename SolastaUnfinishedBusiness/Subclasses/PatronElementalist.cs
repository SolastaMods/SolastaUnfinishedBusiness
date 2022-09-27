using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomDefinitions;
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
    private static readonly Dictionary<string, ElementalFormConfig> ElementalFormCfg = new()
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
        var elementalFormPool = FeatureDefinitionPowerPoolBuilder
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
            .SetGuiPresentation(Category.Feature,
                CustomIcons.CreateAssetReferenceSprite("ElementalForm", Resources.ElementalForm, 128, 64))
            .SetUsesProficiency()
            .SetRechargeRate(RechargeRate.LongRest)
            .AddToDB();

        var enhancedElementalFormPool = FeatureDefinitionPowerPoolBuilder
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
            .SetGuiPresentation(Category.Feature,
                CustomIcons.CreateAssetReferenceSprite("ElementalFormEnhanced", Resources.ElementalFormEnhanced, 128, 64))
            .SetUsesProficiency()
            .SetOverriddenPower(elementalFormPool)
            .AddToDB();

        var iconRegular = CustomIcons.CreateAssetReferenceSprite("ElementalFormIcon",
            Resources.ElementalFormIcon, 24, 24);
        var iconEnhanced = CustomIcons.CreateAssetReferenceSprite("ElementalFormIconEnhanced",
            Resources.ElementalFormIconEnhanced, 24, 24);
        
        var regularPowers = new List<FeatureDefinitionPower>();
        var enhancedPowers = new List<FeatureDefinitionPower>();

        foreach (var e in ElementalFormCfg)
        {
            var (rPower, ePower) = BuildElementalForm(elementalFormPool, enhancedElementalFormPool, e.Key, e.Value, iconRegular, iconEnhanced);
            
            regularPowers.Add(rPower);
            enhancedPowers.Add(ePower);
        }

        PowersBundleContext.RegisterPowerBundle(elementalFormPool, true, regularPowers.ToArray());
        PowersBundleContext.RegisterPowerBundle(enhancedElementalFormPool, true, enhancedPowers.ToArray());
        
        var elementalistSpellList = SpellListDefinitionBuilder
            .Create(DatabaseHelper.SpellListDefinitions.SpellListPaladin, "ElementalistSpellList")
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

        var elementalistMagicAffinityExpandedSpells = FeatureDefinitionMagicAffinityBuilder
            .Create("ElementalistMagicAffinityExpandedSpells")
            .SetGuiPresentation("MagicAffinityPatronExpandedSpells", Category.Feature)
            .SetExtendedSpellList(elementalistSpellList)
            .AddToDB();

        var featureSetElementalistKnowledge = FeatureDefinitionFeatureSetBuilder
            .Create(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetGreenmageWardenOfTheForest,
                "FeatureSetElementalistKnowledge")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageRangerFavoredEnemyElemental)
            .AddFeatureSet(DatabaseHelper.FeatureDefinitionCombatAffinitys.CombatAffinityProtectedFromEvil)
            .AddFeatureSet(DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityCircleLandNaturesWardCharmed)
            .AddFeatureSet(DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityCircleLandNaturesWardFrightened)
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
            // .AddFeatureAtLevel(FeatureSet_Level01, 1)
            .AddFeaturesAtLevel(1, elementalistMagicAffinityExpandedSpells, elementalFormPool)
            .AddFeaturesAtLevel(6, featureSetElementalistKnowledge)
            .AddFeaturesAtLevel(10, enhancedElementalFormPool)
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
        ElementalFormConfig cfg,
        AssetReferenceSprite sprite = null)
    {
        var damageType = cfg.DamageType.GuiPresentation.Title;
        var planeText = $"Feature/&ElementalPact{text}Plane";

        return new GuiPresentationBuilder(
            Gui.Format($"Feature/&ElementalPatron{type}FormatTitle", planeText),
            Gui.Format($"Feature/&ElementalPatron{type}FormatDescription", planeText, damageType),
            sprite
        ).Build();
    }

    private static (FeatureDefinitionPower, FeatureDefinitionPower)
        BuildElementalForm(
            FeatureDefinitionPower elementalFormPool,
            FeatureDefinitionPower enhancedElementalFormPool,
            string text,
            ElementalFormConfig cfg,
            AssetReferenceSprite iconRegular,
            AssetReferenceSprite iconEnhanced)
    {
        var additionalDamage = FeatureDefinitionAdditionalDamageBuilder
            .Create($"DH_ElementalForm_{text}AdditionalDamage")
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
                cfg.DamageType.Name,
                AdditionalDamageAdvancement.None,
                new List<DiceByRank>()
            )
            .SetGuiPresentation(GuiPresentation("ElementalDamage", text, cfg))
            .AddToDB();

        var ElementalFormCondtion = ConditionDefinitionBuilder
            .Create($"DH_ElementalForm_{text}Condition")
            .SetGuiPresentation(GuiPresentation("ElementalCondition", text, cfg, iconRegular))
            .SetSilent(Silent.None)
            .SetDuration(DurationType.Minute, 1)
            .AddFeatures(cfg.Resistance, additionalDamage)
            .SetConditionParticleReference(cfg.Particles)
            .AddToDB();

        var elementalFormPower = FeatureDefinitionPowerSharedPoolBuilder
            .Create("DH_ElementalForm_" + text)
            .SetGuiPresentation(GuiPresentation("ElementalForm", text, cfg))
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
                            ElementalFormCondtion,
                            ConditionForm.ConditionOperation.Add,
                            true,
                            true
                        )
                        .Build()
                    )
                    .Build(),
                true)
            .AddToDB();

        //Enhanced form

        var EnhancedElementalFormCondtion = ConditionDefinitionBuilder.Create(
                $"DH_EnhancedElementalForm_{text}Condition")
            .SetDuration(DurationType.Minute, 1)
            .SetGuiPresentation(GuiPresentation("ElementalCondition", text, cfg, iconEnhanced))
            .SetSilent(Silent.None)
            .SetDuration(DurationType.Minute, 1)
            .AddFeatures(cfg.Immunity, additionalDamage)
            .SetConditionParticleReference(cfg.Particles)
            .SetCharacterShaderReference(cfg.Shaders)
            .AddToDB();

        var enhancedElementalFormPower = FeatureDefinitionPowerSharedPoolBuilder
            .Create("DH_EnhancedElementalForm_" + text)
            .SetGuiPresentation(GuiPresentation("ElementalFormEnhanced", text, cfg))
            .SetOverriddenPower(elementalFormPower)
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
                        .SetConditionForm(EnhancedElementalFormCondtion,
                            ConditionForm.ConditionOperation.Add, true, true)
                        .Build()
                    ).Build(),
                true)
            .AddToDB();

        return (elementalFormPower, enhancedElementalFormPower);
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
