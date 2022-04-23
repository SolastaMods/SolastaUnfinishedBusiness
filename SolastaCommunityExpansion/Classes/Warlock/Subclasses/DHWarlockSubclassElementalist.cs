using System.Collections.Generic;
using SolastaModApi;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.CustomDefinitions;
using UnityEngine.AddressableAssets;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.Models;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using static RuleDefinitions;
using static SolastaModApi.DatabaseHelper.DamageDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Classes.Warlock.Subclasses
{
    internal static class DHWarlockSubclassElementalPatron
    {
        public const string Name = "DHWarlockSubclassElementalPatron";

        private static FeatureDefinitionPower ElementalFormPool { get; set; }
        private static FeatureDefinitionPower EnhancedElementalFormPool { get; set; }
        private static FeatureDefinitionBonusCantrips MinorElementalBonusCantrip { get; set; }
        private static FeatureDefinitionMagicAffinity ElementalistMagicAffinity { get; set; }

        public static CharacterSubclassDefinition Build()
        {
            ElementalFormPool = FeatureDefinitionPowerPoolBuilder
                .Create("DH_ElementalFormPool", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Power)
                .SetUsesProficiency()
                .SetRechargeRate(RechargeRate.LongRest)
                .SetActivation(ActivationTime.BonusAction, 1)
                .AddToDB();
            
            EnhancedElementalFormPool = FeatureDefinitionPowerPoolBuilder
                .Create("DH_ElementalFormPoolEnhanced", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Power)
                .SetUsesProficiency()
                .SetRechargeRate(RechargeRate.LongRest)
                .SetActivation(ActivationTime.BonusAction, 1)
                .SetOverriddenPower(ElementalFormPool)
                .AddToDB();
            

            BuildElementalForms();
            ElementalistSpells();

            var FeatureSet_Level01 = FeatureDefinitionFeatureSetBuilder
                .Create(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetGreenmageWardenOfTheForest, "ElementalPatronFeatureSet_Level01", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Feature)
                .ClearFeatureSet()
                .AddFeatureSet(ElementalFormPool)
                .AddFeatureSet(ElementalistMagicAffinity)
                //.AddFeature(DatabaseHelper.FeatureDefinitionMagicAffinitys.MagicAffinityShockArcanistArcaneWarfare)
                // bonus cantrip granted/selection for cantrips that deal the above damage?
                .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                .SetUniqueChoices(false)
                .AddToDB();

            var FeatureSet_Level06 = FeatureDefinitionFeatureSetBuilder
                .Create(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetGreenmageWardenOfTheForest, "ElementalPatronFeatureSet_Level06", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Feature)
                .ClearFeatureSet()
                .AddFeatureSet(DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageRangerFavoredEnemyElemental)
                .AddFeatureSet(DatabaseHelper.FeatureDefinitionCombatAffinitys.CombatAffinityProtectedFromEvil)
                .AddFeatureSet(DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityCircleLandNaturesWardCharmed)
                .AddFeatureSet(DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityCircleLandNaturesWardFrightened)
                .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                .SetUniqueChoices(false)
                .AddToDB();

            /* enhanced Elemental form (1 power , 2 effect forms aura sphere target and condition applied to self)
        * = immune ( heals from damage)
        * and
        * creates damage aura or damage retaliation
            something like =
        * "damageType": "DamageCold",
           "damageAffinityType": "Immunity",
           "tagsIgnoringAffinity": [],
           "healsBack": true,
           "healBackCap": 10,
       */

            var FeatureSet_Level10 = FeatureDefinitionFeatureSetBuilder.Create(
                DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetGreenmageWardenOfTheForest,
                "ElementalPatronFeatureSet_Level10", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Feature)
                .ClearFeatureSet()
                .AddFeatureSet(EnhancedElementalFormPool)
                .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                .SetUniqueChoices(false)
                .AddToDB();

            // cantrip at will version of Protection from Energy or conjure minor elementals
            AtWillConjureMinorElementals();

            return CharacterSubclassDefinitionBuilder.Create(Name, DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Subclass, DatabaseHelper.CharacterSubclassDefinitions.TraditionLoremaster.GuiPresentation.SpriteReference)
                .AddFeatureAtLevel(FeatureSet_Level01, 1)
                .AddFeatureAtLevel(FeatureSet_Level06, 6)
                .AddFeatureAtLevel(FeatureSet_Level10, 10)
                .AddFeatureAtLevel(MinorElementalBonusCantrip, 14)
                .AddToDB();
        }

        private static void BuildElementalForms()
        {
            var regularPowers = new List<FeatureDefinitionPower>();
            var refularConditions = new List<ConditionDefinition>();

            var enhancedPowers = new List<FeatureDefinitionPower>();
            var enhancedConditions = new List<ConditionDefinition>();

            foreach (var e in ElementalFormCfg)
            {
                var ((RPower, RCondition), (EPower, ECondition)) = BuildElementalForm(e.Key, e.Value);

                regularPowers.Add(RPower);
                refularConditions.Add(RCondition);

                enhancedPowers.Add(EPower);
                enhancedConditions.Add(ECondition);
            }

            PowerBundleContext.RegisterPowerBundle(ElementalFormPool, regularPowers);
            PowerBundleContext.RegisterPowerBundle(EnhancedElementalFormPool, enhancedPowers);
            
            foreach (var condition in refularConditions)
            {
                condition.CancellingConditions.AddRange(refularConditions);
                condition.CancellingConditions.Remove(condition);
            }
            foreach (var condition in enhancedConditions)
            {
                condition.CancellingConditions.AddRange(enhancedConditions);
                condition.CancellingConditions.Remove(condition);
            }
        }

        internal class ElementalFormConfig
        {
            internal DamageDefinition DamageType;
            internal FeatureDefinitionDamageAffinity Resistance;
            internal FeatureDefinitionDamageAffinity Immunity;

            internal AssetReference Particles;
            internal AssetReference Shaders;
            internal AssetReferenceSprite Sprite;
        }

        private static readonly Dictionary<string, ElementalFormConfig> ElementalFormCfg = new()
        {
            {
                "Shadow", new ElementalFormConfig
                {
                    // DamageName = "Necrotic",
                    DamageType = DamageNecrotic,
                    Resistance = DamageAffinityNecroticResistance,
                    Immunity = DamageAffinityNecroticImmunity,
                    Particles = DatabaseHelper.MonsterDefinitions.Sorr_Akkath_Shikkath.MonsterPresentation.GetField<AssetReference>("attachedParticlesReference"),
                    Shaders = DatabaseHelper.MonsterDefinitions.Wraith.MonsterPresentation.CustomShaderReference ,
                    Sprite = DatabaseHelper.FeatureDefinitionPowers.PowerDomainSunHeraldOfTheSun.GuiPresentation.SpriteReference,
                }
            },
            {
                "Astral", new ElementalFormConfig
                {
                    // DamageName = "Psychic",
                    DamageType = DamagePsychic,
                    Resistance = DamageAffinityPsychicResistance,
                    Immunity = DamageAffinityPsychicImmunity,
                    Particles = DatabaseHelper.MonsterDefinitions.WightLord.MonsterPresentation.GetField<AssetReference>("attachedParticlesReference"),// backup kindredspirits bear - spectral spider
                    Shaders = DatabaseHelper.MonsterDefinitions.SpectralSpider.MonsterPresentation.CustomShaderReference,// backup kindredspirits bear - spectral spider,
                    Sprite = DatabaseHelper.FeatureDefinitionPowers.PowerDomainInsightForeknowledge.GuiPresentation.SpriteReference,
                }
            },
            // {
            //     "Ethereal", new ElementalFormConfig
            //     {
            //         DamageName = "ForceDamage",
            //         Particles = DatabaseHelper.MonsterDefinitions.Ghost.MonsterPresentation.GetField<AssetReference>("attachedParticlesReference") ,  // backup fey bear - sorr boss,
            //         Shaders = DatabaseHelper.MonsterDefinitions.PhaseSpider.MonsterPresentation.CustomShaderReference,      // backup fey bear - sorr boss,
            //         Sprite = DatabaseHelper.FeatureDefinitionPowers.PowerOathOfDevotionAuraDevotion.GuiPresentation.SpriteReference,
            //     }
            // },
            // {
            //     "Fire", new ElementalFormConfig
            //     {
            //         DamageName = "Fire",
            //         Particles = DatabaseHelper.MonsterDefinitions.Fire_Jester.MonsterPresentation.GetField<AssetReference>("attachedParticlesReference"), // fire jester,
            //         Shaders = DatabaseHelper.MonsterDefinitions.Fire_Elemental.MonsterPresentation.CustomShaderReference, // fire jester,
            //         Sprite = DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalHeraldOfTheElementsFire.GuiPresentation.SpriteReference,
            //     }
            // },
            // {
            //     "Earth", new ElementalFormConfig
            //     {
            //         DamageName = "Bludgeoning",
            //         Particles = DatabaseHelper.MonsterDefinitions.Earth_Elemental.MonsterPresentation.GetField<AssetReference>("attachedParticlesReference"), // or stone barbarian's ConditionStoneResilience.conditionParticleReference,
            //         Shaders =  DatabaseHelper.MonsterDefinitions.FeyBear.MonsterPresentation.CustomShaderReference, // or stone barbarian's ConditionStoneResilience.conditionParticleReference,
            //         Sprite = DatabaseHelper.FeatureDefinitionPowers.PowerDomainBattleDivineWrath.GuiPresentation.SpriteReference,
            //     }
            // },
            // {
            //     "Ice", new ElementalFormConfig
            //     {
            //         DamageName = "Cold",
            //         Particles = DatabaseHelper.MonsterDefinitions.WindSnake.MonsterPresentation.GetField<AssetReference>("attachedParticlesReference"),// skarn ghoul,
            //         Shaders = DatabaseHelper.MonsterDefinitions.SkarnGhoul.MonsterPresentation.CustomShaderReference,// skarn ghoul
            //         Sprite = DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalHeraldOfTheElementsCold.GuiPresentation.SpriteReference,
            //     }
            // },
            // {
            //     "Air", new ElementalFormConfig
            //     {
            //         DamageName = "Thunder",
            //         Particles = DatabaseHelper.MonsterDefinitions.Air_Elemental.MonsterPresentation.GetField<AssetReference>("attachedParticlesReference"),
            //         Shaders = DatabaseHelper.MonsterDefinitions.Air_Elemental.MonsterPresentation.CustomShaderReference,
            //         Sprite = DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalHeraldOfTheElementsThunder.GuiPresentation.SpriteReference,
            //     }
            // }
        };

        private static GuiPresentation GuiPresentation(string type, string text, ElementalFormConfig cfg, AssetReferenceSprite sprite = null)
        {
            var damageType = cfg.DamageType.GuiPresentation.Title;
            var planeText = $"Feature/&ElementalPact{text}Plane";

            return new GuiPresentationBuilder(
                Gui.Format($"Feature/&ElementalPatron{type}FormatTitle", planeText),
                Gui.Format($"Feature/&ElementalPatron{type}FormatDescription", planeText, damageType),
                sprite
            ).Build();
        }

        public static ((FeatureDefinitionPower, ConditionDefinition), (FeatureDefinitionPower, ConditionDefinition)) BuildElementalForm(string text, ElementalFormConfig cfg)
        {
            //Regular form
            
            FeatureDefinitionAdditionalDamage additionalDamage = FeatureDefinitionAdditionalDamageBuilder
                .Create($"DH_ElementalForm_{text}AdditionalDamage", DefinitionBuilder.CENamespaceGuid)
                .Configure(
                    "ElementalDamage",
                    FeatureLimitedUsage.OncePerTurn,
                    AdditionalDamageValueDetermination.ProficiencyBonus,
                    AdditionalDamageTriggerCondition.AlwaysActive,
                    AdditionalDamageRequiredProperty.MeleeWeapon,
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

                ConditionDefinition ElementalFormCondtion = ConditionDefinitionBuilder
                    .Create($"DH_ElementalForm_{text}Condition", DefinitionBuilder.CENamespaceGuid)
                    .SetGuiPresentation(GuiPresentation("ElementalForm", text, cfg, cfg.Sprite))//TODO: add icon for effect
                    .SetSilent(Silent.None)
                    .SetDuration(DurationType.Minute, 1)
                    .AddFeatures(cfg.Resistance, additionalDamage)
                    .SetConditionParticleReference(cfg.Particles)
                    .AddToDB();

                FeatureDefinitionPowerSharedPool ElementalFormPower = new FeatureDefinitionPowerSharedPoolBuilder(
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
                        GuiPresentation("ElementalForm", text, cfg, cfg.Sprite),
                        true
                    )
                    .AddToDB();
            
            //Enhanced form
            
            ConditionDefinition EnhancedElementalFormCondtion = ConditionDefinitionBuilder.Create(
                    $"DH_EnhancedElementalForm_{text}Condition", DefinitionBuilder.CENamespaceGuid)
                .SetDuration(DurationType.Minute, 1)
                .SetGuiPresentation(GuiPresentation("ElementalFormEnhanced", text, cfg, cfg.Sprite))//TODO: add icon for effect
                .SetSilent(Silent.None)
                .SetDuration(DurationType.Minute, 1)
                .AddFeatures(cfg.Immunity, additionalDamage)
                .SetConditionParticleReference(cfg.Particles)
                .SetCharacterShaderReference(cfg.Shaders)
                .AddToDB();

            var EnhancedElementalFormPower = new FeatureDefinitionPowerSharedPoolBuilder(
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
                    GuiPresentation("ElementalFormEnhanced", text, cfg, cfg.Sprite),
                    true
                )
                .SetOverriddenPower(ElementalFormPower)
                .AddToDB();

            return ((ElementalFormPower, ElementalFormCondtion), (EnhancedElementalFormPower, EnhancedElementalFormCondtion));
        }

        public static void AtWillConjureMinorElementals()
        {
            SpellDefinitionBuilder AtWillConjureMinorElementalsBuilder = SpellDefinitionBuilder
                    .Create(ConjureMinorElementals, "DHAtWillConjureMinorElementals", DefinitionBuilder.CENamespaceGuid);
            AtWillConjureMinorElementalsBuilder.SetSpellLevel(0);

            MinorElementalBonusCantrip = FeatureDefinitionFreeBonusCantripsBuilder
                .Create("DHConjureMinorElementalsBonusCantrip", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Feature)
                .AddBonusCantrip(AtWillConjureMinorElementalsBuilder.AddToDB())
                .AddToDB();
        }


        public static void ElementalistSpells()
        {
            SpellListDefinition ElementalistSpellList = SpellListDefinitionBuilder
                .Create(DatabaseHelper.SpellListDefinitions.SpellListPaladin, "ElementalistSpellsList", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.SpellList)
                .ClearSpells()
                .SetSpellsAtLevel(1, Thunderwave, FogCloud)
                .SetSpellsAtLevel(2, SpikeGrowth, ScorchingRay)
                .SetSpellsAtLevel(3, Fireball, LightningBolt)
                .SetSpellsAtLevel(4, Stoneskin, IceStorm, ConjureMinorElementals)
                .SetSpellsAtLevel(5, ConeOfCold, FlameStrike, ConjureElemental)
                .FinalizeSpells()
                .AddToDB();

            ElementalistMagicAffinity = FeatureDefinitionMagicAffinityBuilder
                .Create("ElementalistSpellsMagicAffinity", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Feature)
                .SetExtendedSpellList(ElementalistSpellList)
                .AddToDB();
        }
    }
}
