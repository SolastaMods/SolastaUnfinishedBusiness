using System.Collections.Generic;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.Models;
using SolastaCommunityExpansion.Properties;
using SolastaCommunityExpansion.Utils;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.DamageDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaCommunityExpansion.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Classes.Warlock.Subclasses;

internal static class WarlockSubclassElementalPatron
{
    private const string Name = "DHWarlockSubclassElementalPatron";

    //Think about making smaller base pool of elements, with ability to expand via eldritch Invocations or Mysterium Arcana
    private static readonly Dictionary<string, ElementalFormConfig> ElementalFormCfg = new()
    {
        // {
        //     "Shadow", new ElementalFormConfig
        //     {
        //         // DamageName = "Necrotic",
        //         DamageType = DamageNecrotic,
        //         Resistance = DamageAffinityNecroticResistance,
        //         Immunity = DamageAffinityNecroticImmunity,
        //         Particles = DatabaseHelper.MonsterDefinitions.Sorr_Akkath_Shikkath.MonsterPresentation.attachedParticlesReference,
        //         Shaders = DatabaseHelper.MonsterDefinitions.Wraith.MonsterPresentation.CustomShaderReference ,
        //         // Sprite = DatabaseHelper.FeatureDefinitionPowers.PowerDomainSunHeraldOfTheSun.GuiPresentation.SpriteReference,
        //     }
        // },
        // {
        //     "Astral", new ElementalFormConfig
        //     {
        //         // DamageName = "Psychic",
        //         DamageType = DamagePsychic,
        //         Resistance = DamageAffinityPsychicResistance,
        //         Immunity = DamageAffinityPsychicImmunity,
        //         Particles = DatabaseHelper.MonsterDefinitions.WightLord.MonsterPresentation.attachedParticlesReference,// backup kindredspirits bear - spectral spider
        //         Shaders = DatabaseHelper.MonsterDefinitions.SpectralSpider.MonsterPresentation.CustomShaderReference,// backup kindredspirits bear - spectral spider,
        //         // Sprite = DatabaseHelper.FeatureDefinitionPowers.PowerDomainInsightForeknowledge.GuiPresentation.SpriteReference,
        //     }
        // },
        // {
        //     "Ethereal", new ElementalFormConfig
        //     {
        //         // DamageName = "ForceDamage",
        //         DamageType = DamageForce,
        //         Resistance = DamageAffinityForceDamageResistance,
        //         Immunity = DamageAffinityForceImmunity,
        //         Particles = DatabaseHelper.MonsterDefinitions.Ghost.MonsterPresentation.attachedParticlesReference ,  // backup fey bear - sorr boss,
        //         Shaders = DatabaseHelper.MonsterDefinitions.PhaseSpider.MonsterPresentation.CustomShaderReference,      // backup fey bear - sorr boss,
        //         // Sprite = DatabaseHelper.FeatureDefinitionPowers.PowerOathOfDevotionAuraDevotion.GuiPresentation.SpriteReference,
        //     }
        // },
        {
            "Fire", new ElementalFormConfig
            {
                // DamageName = "Fire",
                DamageType = DamageFire,
                Resistance = DamageAffinityFireResistance,
                Immunity = DamageAffinityFireImmunity,
                Particles = DatabaseHelper.MonsterDefinitions.Fire_Jester.MonsterPresentation
                    .attachedParticlesReference, // fire jester,
                Shaders = DatabaseHelper.MonsterDefinitions.Fire_Elemental.MonsterPresentation
                    .CustomShaderReference // fire jester,
                // Sprite = DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalHeraldOfTheElementsFire.GuiPresentation.SpriteReference,
            }
        },
        {
            "Earth", new ElementalFormConfig
            {
                // DamageName = "Bludgeoning",
                DamageType = DamageBludgeoning,
                Resistance = DamageAffinityBludgeoningResistance,
                Immunity = DamageAffinityBludgeoningImmunity,
                Particles =
                    DatabaseHelper.MonsterDefinitions.Earth_Elemental.MonsterPresentation
                        .attachedParticlesReference, // or stone barbarian's ConditionStoneResilience.conditionParticleReference,
                Shaders = DatabaseHelper.MonsterDefinitions.FeyBear.MonsterPresentation
                    .CustomShaderReference // or stone barbarian's ConditionStoneResilience.conditionParticleReference,
                // Sprite = DatabaseHelper.FeatureDefinitionPowers.PowerDomainBattleDivineWrath.GuiPresentation.SpriteReference,
            }
        },
        {
            "Ice", new ElementalFormConfig
            {
                // DamageName = "Cold",
                DamageType = DamageCold,
                Resistance = DamageAffinityColdResistance,
                Immunity = DamageAffinityColdImmunity,
                Particles =
                    DatabaseHelper.MonsterDefinitions.WindSnake.MonsterPresentation
                        .attachedParticlesReference, // skarn ghoul,
                Shaders = DatabaseHelper.MonsterDefinitions.SkarnGhoul.MonsterPresentation
                    .CustomShaderReference // skarn ghoul
                // Sprite = DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalHeraldOfTheElementsCold.GuiPresentation.SpriteReference,
            }
        },
        {
            "Air", new ElementalFormConfig
            {
                // DamageName = "Thunder",
                DamageType = DamageThunder,
                Resistance = DamageAffinityThunderResistance,
                Immunity = DamageAffinityThunderImmunity,
                Particles =
                    DatabaseHelper.MonsterDefinitions.Air_Elemental.MonsterPresentation.attachedParticlesReference,
                Shaders = DatabaseHelper.MonsterDefinitions.Air_Elemental.MonsterPresentation.CustomShaderReference
                // Sprite = DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalHeraldOfTheElementsThunder.GuiPresentation.SpriteReference,
            }
        }
    };

    private static FeatureDefinitionPower ElementalFormPool { get; set; }
    private static FeatureDefinitionPower EnhancedElementalFormPool { get; set; }
    private static FeatureDefinitionBonusCantrips MinorElementalBonusCantrip { get; set; }
    private static FeatureDefinitionMagicAffinity ElementalistMagicAffinity { get; set; }

    public static CharacterSubclassDefinition Build()
    {
        ElementalFormPool = FeatureDefinitionPowerPoolBuilder
            .Create("DH_ElementalFormPool", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Power,
                CustomIcons.CreateAssetReferenceSprite("ElementalForm", Resources.ElementalForm,
                    128, 64))
            .SetUsesProficiency()
            .SetRechargeRate(RechargeRate.LongRest)
            .SetActivation(ActivationTime.BonusAction, 1)
            .AddToDB();

        EnhancedElementalFormPool = FeatureDefinitionPowerPoolBuilder
            .Create("DH_ElementalFormPoolEnhanced", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Power,
                CustomIcons.CreateAssetReferenceSprite("ElementalFormEnhanced",
                    Resources.ElementalFormEnhanced, 128, 64))
            .SetShortTitle("Power/&DH_ElementalFormPoolEnhancedTitleShort")
            .SetUsesProficiency()
            .SetRechargeRate(RechargeRate.LongRest)
            .SetActivation(ActivationTime.BonusAction, 1)
            .SetOverriddenPower(ElementalFormPool)
            .AddToDB();

        BuildElementalForms();
        ElementalistSpells();

        var featureSetLevel06 = FeatureDefinitionFeatureSetBuilder
            .Create(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetGreenmageWardenOfTheForest,
                "ElementalPatronFeatureSet_Level06", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .ClearFeatureSet()
            .AddFeatureSet(DatabaseHelper.FeatureDefinitionAdditionalDamages
                .AdditionalDamageRangerFavoredEnemyElemental)
            .AddFeatureSet(DatabaseHelper.FeatureDefinitionCombatAffinitys.CombatAffinityProtectedFromEvil)
            .AddFeatureSet(DatabaseHelper.FeatureDefinitionConditionAffinitys
                .ConditionAffinityCircleLandNaturesWardCharmed)
            .AddFeatureSet(DatabaseHelper.FeatureDefinitionConditionAffinitys
                .ConditionAffinityCircleLandNaturesWardFrightened)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .SetUniqueChoices(false)
            .AddToDB();

        // cantrip at will version of Protection from Energy or conjure minor elementals
        AtWillConjureMinorElementals();

        return CharacterSubclassDefinitionBuilder.Create(Name, DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Subclass,
                DatabaseHelper.CharacterSubclassDefinitions.TraditionLoremaster.GuiPresentation.SpriteReference)
            // .AddFeatureAtLevel(FeatureSet_Level01, 1)
            .AddFeaturesAtLevel(1, ElementalistMagicAffinity, ElementalFormPool)
            .AddFeatureAtLevel(featureSetLevel06, 6)
            .AddFeatureAtLevel(EnhancedElementalFormPool, 10)
            .AddFeatureAtLevel(MinorElementalBonusCantrip, 14)
            .AddToDB();
    }

    private static void BuildElementalForms()
    {
        var iconRegular = CustomIcons.CreateAssetReferenceSprite("ElementalFormIcon",
            Resources.ElementalFormIcon, 24, 24);
        var iconEnhanced = CustomIcons.CreateAssetReferenceSprite("ElementalFormIconEnhanced",
            Resources.ElementalFormIconEnhanced, 24, 24);

        var regularPowers = new List<FeatureDefinitionPower>();
        var enhancedPowers = new List<FeatureDefinitionPower>();

        foreach (var e in ElementalFormCfg)
        {
            var (rPower, ePower) = BuildElementalForm(e.Key, e.Value, iconRegular, iconEnhanced);
            regularPowers.Add(rPower);
            enhancedPowers.Add(ePower);
        }

        PowerBundleContext.RegisterPowerBundle(ElementalFormPool, true, regularPowers);
        PowerBundleContext.RegisterPowerBundle(EnhancedElementalFormPool, true, enhancedPowers);
    }

    private static GuiPresentation GuiPresentation(string type, string text, ElementalFormConfig cfg,
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
        BuildElementalForm(string text, ElementalFormConfig cfg, AssetReferenceSprite iconRegular,
            AssetReferenceSprite iconEnhanced)
    {
        //Regular form

        var additionalDamage = FeatureDefinitionAdditionalDamageBuilder
            .Create($"DH_ElementalForm_{text}AdditionalDamage", DefinitionBuilder.CENamespaceGuid)
            .Configure(
                "ElementalDamage",
                FeatureLimitedUsage.OncePerTurn,
                AdditionalDamageValueDetermination.ProficiencyBonus,
                AdditionalDamageTriggerCondition.SpellDamagesTarget,
                AdditionalDamageRequiredProperty.None,
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
            .Create($"DH_ElementalForm_{text}Condition", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(GuiPresentation("ElementalCondition", text, cfg, iconRegular))
            .SetSilent(Silent.None)
            .SetDuration(DurationType.Minute, 1)
            .AddFeatures(cfg.Resistance, additionalDamage)
            .SetConditionParticleReference(cfg.Particles)
            .AddToDB();

        var elementalFormPower = new FeatureDefinitionPowerSharedPoolBuilder(
                "DH_ElementalForm_" + text,
                ElementalFormPool,
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
                GuiPresentation("ElementalForm", text, cfg),
                true
            )
            .AddToDB();

        //Enhanced form

        var EnhancedElementalFormCondtion = ConditionDefinitionBuilder.Create(
                $"DH_EnhancedElementalForm_{text}Condition", DefinitionBuilder.CENamespaceGuid)
            .SetDuration(DurationType.Minute, 1)
            .SetGuiPresentation(GuiPresentation("ElementalCondition", text, cfg, iconEnhanced))
            .SetSilent(Silent.None)
            .SetDuration(DurationType.Minute, 1)
            .AddFeatures(cfg.Immunity, additionalDamage)
            .SetConditionParticleReference(cfg.Particles)
            .SetCharacterShaderReference(cfg.Shaders)
            .AddToDB();

        var enhancedElementalFormPower = new FeatureDefinitionPowerSharedPoolBuilder(
                "DH_EnhancedElementalForm_" + text,
                EnhancedElementalFormPool,
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
                GuiPresentation("ElementalFormEnhanced", text, cfg),
                true
            )
            .SetOverriddenPower(elementalFormPower)
            .AddToDB();

        return (elementalFormPower, enhancedElementalFormPower);
    }

    private static void AtWillConjureMinorElementals()
    {
        var AtWillConjureMinorElementalsBuilder = SpellDefinitionBuilder
            .Create(ConjureMinorElementals, "DHAtWillConjureMinorElementals", DefinitionBuilder.CENamespaceGuid);
        AtWillConjureMinorElementalsBuilder.SetSpellLevel(0);

        MinorElementalBonusCantrip = FeatureDefinitionBonusCantripsBuilder
            .Create("DHConjureMinorElementalsBonusCantrip", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .AddBonusCantrip(AtWillConjureMinorElementalsBuilder.AddToDB())
            .AddToDB();
    }


    private static void ElementalistSpells()
    {
        var ElementalistSpellList = SpellListDefinitionBuilder
            .Create(DatabaseHelper.SpellListDefinitions.SpellListPaladin, "ElementalistSpellsList",
                DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.SpellList)
            .ClearSpells()
            .SetSpellsAtLevel(0, FireBolt, RayOfFrost, ShockingGrasp)
            .SetSpellsAtLevel(1, BurningHands, Thunderwave, FogCloud)
            .SetSpellsAtLevel(2, FlamingSphere, ScorchingRay, HeatMetal)
            .SetSpellsAtLevel(3, Fireball, LightningBolt, SleetStorm)
            .SetSpellsAtLevel(4, Stoneskin, IceStorm, WallOfFire)
            .SetSpellsAtLevel(5, ConeOfCold, FlameStrike, ConjureElemental)
            .FinalizeSpells()
            .AddToDB();

        ElementalistMagicAffinity = FeatureDefinitionMagicAffinityBuilder
            .Create("ElementalistSpellsMagicAffinity", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .SetExtendedSpellList(ElementalistSpellList)
            .AddToDB();
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
