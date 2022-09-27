#if false
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

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class PatronElementalist : AbstractSubclass
{
    private const string Name = "DHWarlockSubclassElementalPatron";

    //Think about making smaller base pool of elements, with ability to expand via eldritch Invocations or Mysterium Arcana
    private static readonly Dictionary<string, ElementalFormConfig> ElementalFormCfg = new()
    {
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

    // ReSharper disable once InconsistentNaming
    private readonly CharacterSubclassDefinition Subclass;

    internal PatronElementalist()
    {
        ElementalFormPool = FeatureDefinitionPowerPoolBuilder
            .Create("DH_ElementalFormPool")
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
                CustomIcons.CreateAssetReferenceSprite("ElementalForm", Resources.ElementalForm,
                    128, 64))
            .SetUsesProficiency()
            .SetRechargeRate(RechargeRate.LongRest)
            .AddToDB();

        EnhancedElementalFormPool = FeatureDefinitionPowerPoolBuilder
            .Create("DH_ElementalFormPoolEnhanced")
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
                CustomIcons.CreateAssetReferenceSprite("ElementalFormEnhanced",
                    Resources.ElementalFormEnhanced, 128, 64))
            .SetUsesProficiency()
            .SetOverriddenPower(ElementalFormPool)
            .AddToDB();

        BuildElementalForms();
        ElementalistSpells();

        var featureSetLevel06 = FeatureDefinitionFeatureSetBuilder
            .Create(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetGreenmageWardenOfTheForest,
                "ElementalPatronFeatureSet_Level06")
            .SetGuiPresentation(Category.Feature)
            // .ClearFeatureSet()
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

        Subclass = CharacterSubclassDefinitionBuilder.Create(Name)
            .SetGuiPresentation(Category.Subclass,
                DatabaseHelper.CharacterSubclassDefinitions.TraditionLoremaster.GuiPresentation.SpriteReference)
            // .AddFeatureAtLevel(FeatureSet_Level01, 1)
            .AddFeaturesAtLevel(1, ElementalistMagicAffinity, ElementalFormPool)
            .AddFeaturesAtLevel(6, featureSetLevel06)
            .AddFeaturesAtLevel(10, EnhancedElementalFormPool)
            .AddFeaturesAtLevel(14, MinorElementalBonusCantrip)
            .AddToDB();
    }

    private static FeatureDefinitionPower ElementalFormPool { get; set; }
    private static FeatureDefinitionPower EnhancedElementalFormPool { get; set; }
    private static FeatureDefinitionBonusCantrips MinorElementalBonusCantrip { get; set; }
    private static FeatureDefinitionMagicAffinity ElementalistMagicAffinity { get; set; }

    internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
    {
        return DatabaseHelper.FeatureDefinitionSubclassChoices.SubclassChoiceWarlockOtherworldlyPatrons;
    }

    internal override CharacterSubclassDefinition GetSubclass()
    {
        return Subclass;
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

        PowersBundleContext.RegisterPowerBundle(ElementalFormPool, true, regularPowers.ToArray());
        PowersBundleContext.RegisterPowerBundle(EnhancedElementalFormPool, true, enhancedPowers.ToArray());
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
                $"DH_EnhancedElementalForm_{text}Condition")
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
            .Create(ConjureMinorElementals, "DHAtWillConjureMinorElementals");
        AtWillConjureMinorElementalsBuilder.SetSpellLevel(0);

        MinorElementalBonusCantrip = FeatureDefinitionBonusCantripsBuilder
            .Create("DHConjureMinorElementalsBonusCantrip")
            .SetGuiPresentation(Category.Feature)
            .AddBonusCantrip(AtWillConjureMinorElementalsBuilder.AddToDB())
            .AddToDB();
    }


    private static void ElementalistSpells()
    {
        var ElementalistSpellList = SpellListDefinitionBuilder
            .Create(DatabaseHelper.SpellListDefinitions.SpellListPaladin, "ElementalistSpellsList")
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

        ElementalistMagicAffinity = FeatureDefinitionMagicAffinityBuilder
            .Create("ElementalistSpellsMagicAffinity")
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
#endif
