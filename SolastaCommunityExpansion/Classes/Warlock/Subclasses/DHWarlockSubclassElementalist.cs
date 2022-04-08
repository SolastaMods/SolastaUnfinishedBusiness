using System;
using System.Collections.Generic;
using SolastaModApi;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.CustomFeatureDefinitions;
using UnityEngine.AddressableAssets;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using static RuleDefinitions;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Classes.Warlock.Subclasses
{
    internal static class DHWarlockSubclassElementalPatron
    {
        public const string Name = "DHWarlockSubclassElementalPatron";

        private static readonly Dictionary<string, string> Dictionaryof_Elemental_Damage = new()
        {
            { "Shadow", DamageTypeNecrotic },
            { "Astral", DamageTypePsychic },
            { "Ethereal", DamageTypeForce },
            { "Fire", DamageTypeFire },
            { "Earth", DamageTypeBludgeoning },
            { "Ice", DamageTypeCold },
            { "Air", DamageTypeThunder }
        };

        private static FeatureDefinitionPower ElementalFormPool { get; set; }

        private static readonly Dictionary<string, FeatureDefinitionPower> DictionaryOfElementalFormPowers = new();
        private static readonly Dictionary<string, FeatureDefinitionPower> DictionaryOfEnhancedElementalFormPowers = new();
        private static FeatureDefinitionBonusCantrips MinorElementalBonusCantrip { get; set; }
        private static readonly Dictionary<string, FeatureDefinitionAdditionalDamage> DictionaryOfElementaladditionalDamage = new();
        private static readonly Dictionary<string, ConditionDefinition> DictionaryOfElementalConditions = new();
        private static FeatureDefinitionMagicAffinity ElementalistMagicAffinity { get; set; }


        public static CharacterSubclassDefinition Build()
        {
            ElementalFormPool = FeatureDefinitionPowerPoolBuilder
                .Create("DH_ElementalFormPool")
                .SetGuiPresentationNoContent()
                .SetUsesProficiency()
                .SetUsesAbility(1, AttributeDefinitions.Charisma)
                .SetRechargeRate(RechargeRate.LongRest)
                .AddToDB();

            ElementalForms();
            ElementalistSpells();


            GuiPresentation guiFeatureSet_Level01 = new GuiPresentationBuilder(
                "Feature/&ElementalPatronFeatureSet_Level01Title",
                "Feature/&ElementalPatronFeatureSet_Level01Description")
                .Build();

            var FeatureSet_Level01Builder = FeatureDefinitionFeatureSetBuilder
                .Create(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetGreenmageWardenOfTheForest, "ElementalPatronFeatureSet_Level01")
                .SetGuiPresentation(guiFeatureSet_Level01);

            FeatureDefinitionFeatureSet FeatureSet_Level01 = FeatureSet_Level01Builder
                .ClearFeatureSet()
                .AddFeatureSet(ElementalFormPool)
                .AddFeatureSet(DictionaryOfElementalFormPowers["Shadow"])
                .AddFeatureSet(DictionaryOfElementalFormPowers["Astral"])
                .AddFeatureSet(DictionaryOfElementalFormPowers["Ethereal"])
                .AddFeatureSet(DictionaryOfElementalFormPowers["Fire"])
                .AddFeatureSet(DictionaryOfElementalFormPowers["Earth"])
                .AddFeatureSet(DictionaryOfElementalFormPowers["Ice"])
                .AddFeatureSet(DictionaryOfElementalFormPowers["Air"])
                .AddFeatureSet(ElementalistMagicAffinity)
                //.AddFeature(DatabaseHelper.FeatureDefinitionMagicAffinitys.MagicAffinityShockArcanistArcaneWarfare)
                // bonus cantrip granted/selection for cantrips that deal the above damage?
                .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                .SetUniqueChoices(false)
                .AddToDB();


            GuiPresentation guiFeatureSet_Level06 = new GuiPresentationBuilder(
                "Feature/&ElementalPatronFeatureSet_Level06Title",
                "Feature/&ElementalPatronFeatureSet_Level06Description")
                .Build();
            var FeatureSet_Level06Builder = FeatureDefinitionFeatureSetBuilder
                .Create(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetGreenmageWardenOfTheForest, "ElementalPatronFeatureSet_Level06")
                .SetGuiPresentation(guiFeatureSet_Level06);

            FeatureDefinitionFeatureSet FeatureSet_Level06 = FeatureSet_Level06Builder
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
            EnhancedElementalForms();

            GuiPresentation guiFeatureSet_Level10 = new GuiPresentationBuilder(
                "Feature/&ElementalPatronFeatureSet_Level10Title",
                "Feature/&ElementalPatronFeatureSet_Level10Description")
                .Build();
            var FeatureSet_Level10Builder = FeatureDefinitionFeatureSetBuilder.Create(
                DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetGreenmageWardenOfTheForest,
                "ElementalPatronFeatureSet_Level10")
                .SetGuiPresentation(guiFeatureSet_Level10);

            FeatureDefinitionFeatureSet FeatureSet_Level10 = FeatureSet_Level10Builder
                .ClearFeatureSet()
                .AddFeatureSet(DictionaryOfEnhancedElementalFormPowers["Shadow"])
                .AddFeatureSet(DictionaryOfEnhancedElementalFormPowers["Astral"])
                .AddFeatureSet(DictionaryOfEnhancedElementalFormPowers["Ethereal"])
                .AddFeatureSet(DictionaryOfEnhancedElementalFormPowers["Fire"])
                .AddFeatureSet(DictionaryOfEnhancedElementalFormPowers["Earth"])
                .AddFeatureSet(DictionaryOfEnhancedElementalFormPowers["Ice"])
                .AddFeatureSet(DictionaryOfEnhancedElementalFormPowers["Air"])
                .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                .SetUniqueChoices(false)
                .AddToDB();

            // cantrip at will version of Protection from Energy or conjure minor elementals


            AtWillConjureMinorElementals();


            var subclassGuiPresentation = new GuiPresentationBuilder(
                    "Subclass/&DHWarlockSubclassElementalPatronTitle",
                    "Subclass/&DHWarlockSubclassElementalPatronDescription")
                    .SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.TraditionLoremaster.GuiPresentation.SpriteReference)
                    .Build();

            return CharacterSubclassDefinitionBuilder.Create(Name)
                        .SetGuiPresentation(subclassGuiPresentation)
                        .AddFeatureAtLevel(FeatureSet_Level01, 1)
                        .AddFeatureAtLevel(FeatureSet_Level06, 6)
                        .AddFeatureAtLevel(FeatureSet_Level10, 10)
                        .AddFeatureAtLevel(MinorElementalBonusCantrip, 14)
                        .AddToDB();


        }


        public static void ElementalForms()
        {


            Dictionary<string, FeatureDefinitionDamageAffinity> Dictionaryof_Elemental_damageResistances = new Dictionary<string, FeatureDefinitionDamageAffinity>
            {
                { "Shadow",  DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityNecroticResistance },
                { "Astral",  DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPsychicResistance },
                { "Ethereal",DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityForceDamageResistance },
                { "Fire",    DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityFireResistance },
                { "Earth",   DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityBludgeoningResistance},
                { "Ice",     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance },
                { "Air",     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityThunderResistance },
            };

            Dictionary<string, AssetReference> Dictionaryof_Elemental_Forms_EffectsParticles = new Dictionary<string, AssetReference>
            {
                { "Shadow",  DatabaseHelper.MonsterDefinitions.Sorr_Akkath_Shikkath.MonsterPresentation.GetField<AssetReference>("attachedParticlesReference") },
                { "Astral",  DatabaseHelper.MonsterDefinitions.WightLord.MonsterPresentation.GetField<AssetReference>("attachedParticlesReference")},// backup kindredspirits bear - spectral spider
                { "Ethereal",DatabaseHelper.MonsterDefinitions.Ghost.MonsterPresentation.GetField<AssetReference>("attachedParticlesReference")},      // backup fey bear - sorr boss
                { "Fire",    DatabaseHelper.MonsterDefinitions.Fire_Jester.MonsterPresentation.GetField<AssetReference>("attachedParticlesReference")}, // fire jester
                { "Earth",   DatabaseHelper.MonsterDefinitions.Earth_Elemental.MonsterPresentation.GetField<AssetReference>("attachedParticlesReference")}, // or stone barbarian's ConditionStoneResilience.conditionParticleReference
                { "Ice",     DatabaseHelper.MonsterDefinitions.WindSnake.MonsterPresentation.GetField<AssetReference>("attachedParticlesReference")},// skarn ghoul
                { "Air",     DatabaseHelper.MonsterDefinitions.Air_Elemental.MonsterPresentation.GetField<AssetReference>("attachedParticlesReference")}
            };

            Dictionary<string, AssetReferenceSprite> Dictionaryof_Elemental_Forms_Sprites = new Dictionary<string, AssetReferenceSprite>
            {
                { "Shadow",  DatabaseHelper.FeatureDefinitionPowers.PowerDomainSunHeraldOfTheSun.GuiPresentation.SpriteReference},
                { "Astral",  DatabaseHelper.FeatureDefinitionPowers.PowerDomainInsightForeknowledge.GuiPresentation.SpriteReference},
                { "Ethereal",DatabaseHelper.FeatureDefinitionPowers.PowerOathOfDevotionAuraDevotion.GuiPresentation.SpriteReference},
                { "Fire",    DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalHeraldOfTheElementsFire.GuiPresentation.SpriteReference},
                { "Earth",   DatabaseHelper.FeatureDefinitionPowers.PowerDomainBattleDivineWrath.GuiPresentation.SpriteReference},
                { "Ice",     DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalHeraldOfTheElementsCold.GuiPresentation.SpriteReference},
                { "Air",     DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalHeraldOfTheElementsThunder.GuiPresentation.SpriteReference},
            };

            foreach (KeyValuePair<string, string> entry in Dictionaryof_Elemental_Damage)
            {

                string text = entry.Key;
                string damagetype = entry.Value.Substring(6);

                var guiPresentationElementalFormDamage = new GuiPresentationBuilder(
                     "Elemental Damage : " + text + " Plane",
                     "When channeling the elemental " + text + " Plane, additional " + damagetype + " damage, equal to your Proficiency Bonus, is added to the total"
                   )
                    .Build();


                FeatureDefinitionAdditionalDamage additionalDamage = FeatureDefinitionAdditionalDamageBuilder
                    .Create(
                        "DH_ElementalForm_" + text + "additionalDamage")
                    .Configure(
                        "ElementalDamage",
                        FeatureLimitedUsage.OncePerTurn, AdditionalDamageValueDetermination.ProficiencyBonus,
                        AdditionalDamageTriggerCondition.AlwaysActive, AdditionalDamageRequiredProperty.MeleeWeapon,
                        false /* attack only */, DieType.D4, 1 /* dice number */, AdditionalDamageType.Specific, entry.Value,
                        AdditionalDamageAdvancement.None, new List<DiceByRank>())
                    .SetGuiPresentation(guiPresentationElementalFormDamage)
                    .AddToDB();

                var guiPresentationElementalFormCondition = new GuiPresentationBuilder(
                     "Elemental Form : " + text + " Plane" + " Condition",
                     "When channeling the elemental  " + text + " Plane, you gain resistence to " + damagetype + " damage and once per turn, apply damage of the same type to your attack"
                   )
                    .Build();

                ConditionDefinition ElementalFormCondtion = new Tinkerer.FeatureHelpers.ConditionDefinitionBuilder(
                    "DH_ElementalForm_" + text + "Condition", DefinitionBuilder.CENamespaceGuid,
                    DurationType.Minute, 1, false, guiPresentationElementalFormCondition,
                    Dictionaryof_Elemental_damageResistances[entry.Key],
                        additionalDamage
                    ).AddToDB();



                ElementalFormCondtion.SetConditionParticleReference(Dictionaryof_Elemental_Forms_EffectsParticles[entry.Key]);


                EffectDescriptionBuilder effectDescription = new EffectDescriptionBuilder();
                effectDescription.SetDurationData(DurationType.Minute, 1, TurnOccurenceType.EndOfTurn);
                effectDescription.SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self, 1, 1, ActionDefinitions.ItemSelectionType.Equiped);
                effectDescription.AddEffectForm(new EffectFormBuilder().SetConditionForm(ElementalFormCondtion, ConditionForm.ConditionOperation.Add, true, true, new List<ConditionDefinition>()).Build());


                var guiPresentationElementalForm = new GuiPresentationBuilder(
                     "Elemental Form : " + text + " Plane",
                     "When channeling the elemental  " + text + " Plane, you gain resistence to " + damagetype + " damage and once per turn, apply damage of the same type to your attack"
                     )
                     .SetSpriteReference(Dictionaryof_Elemental_Forms_Sprites[entry.Key])
                     .Build();



                var ElementalFormBuilder = new FeatureDefinitionPowerSharedPoolBuilder(
                       "DH_ElementalForm_" + text,
                       ElementalFormPool,
                       RechargeRate.LongRest,
                       ActivationTime.BonusAction,
                       1,
                       false,
                       false,
                       AttributeDefinitions.Charisma,
                       effectDescription.Build(),
                       guiPresentationElementalForm,
                       true
                       );
                FeatureDefinitionPowerSharedPool ElementalFormPower = ElementalFormBuilder.AddToDB();



                DictionaryOfElementalFormPowers.Add(entry.Key, ElementalFormPower);

                DictionaryOfElementaladditionalDamage.Add(entry.Key, additionalDamage);

                DictionaryOfElementalConditions.Add(entry.Key, ElementalFormCondtion);

            }

            foreach (KeyValuePair<string, ConditionDefinition> entry in DictionaryOfElementalConditions)
            {

                entry.Value.CancellingConditions.AddRange(DictionaryOfElementalConditions.Values);

                entry.Value.CancellingConditions.Remove(entry.Value);



            }


        }


        public static void EnhancedElementalForms()
        {


            Dictionary<string, FeatureDefinitionDamageAffinity> Dictionaryof_Elemental_damageImmunitiess = new Dictionary<string, FeatureDefinitionDamageAffinity>
            {
                { "Shadow",  DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityNecroticImmunity},
                { "Astral",  DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPsychicImmunity},
                { "Ethereal",DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityForceImmunity},
                { "Fire",    DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityFireImmunity},
                { "Earth",   DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityBludgeoningResistance},
                { "Ice",     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityColdImmunity},
                { "Air",     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityThunderImmunity},
            };

            Dictionary<string, AssetReference> Dictionaryof_Elemental_Forms_EffectsParticles = new Dictionary<string, AssetReference>
            {
                { "Shadow",  DatabaseHelper.ConditionDefinitions.ConditionSorcererChildRiftDeflection.GetField<AssetReference>("characterShaderReference")},
                { "Astral",  DatabaseHelper.MonsterDefinitions.WightLord.MonsterPresentation.GetField<AssetReference>("attachedParticlesReference") },// backup kindredspirits bear - spectral spider
                { "Ethereal",DatabaseHelper.MonsterDefinitions.Ghost.MonsterPresentation.GetField<AssetReference>("attachedParticlesReference") },      // backup fey bear - sorr boss
                { "Fire",    DatabaseHelper.MonsterDefinitions.Fire_Jester.MonsterPresentation.GetField<AssetReference>("attachedParticlesReference")}, // fire jester
                { "Earth",   DatabaseHelper.MonsterDefinitions.Earth_Elemental.MonsterPresentation.GetField<AssetReference>("attachedParticlesReference")}, // or stone barbarian's ConditionStoneResilience.conditionParticleReference
                { "Ice",     DatabaseHelper.MonsterDefinitions.WindSnake.MonsterPresentation.GetField<AssetReference>("attachedParticlesReference")},// skarn ghoul
                { "Air",     DatabaseHelper.MonsterDefinitions.Air_Elemental.MonsterPresentation.GetField<AssetReference>("attachedParticlesReference")}
            };

            Dictionary<string, AssetReference> Dictionaryof_Elemental_Forms_EffectsShaders = new Dictionary<string, AssetReference>
            {
                { "Shadow",  DatabaseHelper.MonsterDefinitions.Wraith.MonsterPresentation.CustomShaderReference },
                { "Astral",  DatabaseHelper.MonsterDefinitions.SpectralSpider.MonsterPresentation.CustomShaderReference},// backup kindredspirits bear - spectral spider
                { "Ethereal",DatabaseHelper.MonsterDefinitions.PhaseSpider.MonsterPresentation.CustomShaderReference},      // backup fey bear - sorr boss
                { "Fire",    DatabaseHelper.MonsterDefinitions.Fire_Elemental.MonsterPresentation.CustomShaderReference}, // fire jester
                { "Earth",   DatabaseHelper.MonsterDefinitions.FeyBear.MonsterPresentation.CustomShaderReference}, // or stone barbarian's ConditionStoneResilience.conditionParticleReference
                { "Ice",     DatabaseHelper.MonsterDefinitions.SkarnGhoul.MonsterPresentation.CustomShaderReference},// skarn ghoul
                { "Air",     DatabaseHelper.MonsterDefinitions.Air_Elemental.MonsterPresentation.CustomShaderReference}
            };

            Dictionary<string, AssetReferenceSprite> Dictionaryof_Elemental_Forms_Sprites = new Dictionary<string, AssetReferenceSprite>
            {
                { "Shadow",  DatabaseHelper.FeatureDefinitionPowers.PowerDomainSunHeraldOfTheSun.GuiPresentation.SpriteReference},
                { "Astral",  DatabaseHelper.FeatureDefinitionPowers.PowerDomainInsightForeknowledge.GuiPresentation.SpriteReference},
                { "Ethereal",DatabaseHelper.FeatureDefinitionPowers.PowerOathOfDevotionAuraDevotion.GuiPresentation.SpriteReference},
                { "Fire",    DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalHeraldOfTheElementsFire.GuiPresentation.SpriteReference},
                { "Earth",   DatabaseHelper.FeatureDefinitionPowers.PowerDomainBattleDivineWrath.GuiPresentation.SpriteReference},
                { "Ice",     DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalHeraldOfTheElementsCold.GuiPresentation.SpriteReference},
                { "Air",     DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalHeraldOfTheElementsThunder.GuiPresentation.SpriteReference},
            };

            foreach (KeyValuePair<string, string> entry in Dictionaryof_Elemental_Damage)
            {

                string text = entry.Key;
                string damagetype = entry.Value.Substring(6);

                var guiPresentationEnhancedElementalFormCondition = new GuiPresentationBuilder(
                    "Elemental Form : " + text + " Plane" + " Condition",
                    "When channeling the elemental  " + text + " Plane, you gain Immunity to " + damagetype + " damage and once per turn, apply damage of the same type to your attack")
                    .Build();

                ConditionDefinition EnhancedElementalFormCondtion = ConditionDefinitionBuilder.Create(
                    "DH_EnhancedElementalForm_" + text + "Condition")
                   .SetDuration(DurationType.Minute, 1)
                    .SetSilent(Silent.None)
                    .SetGuiPresentation(guiPresentationEnhancedElementalFormCondition)
                    .AddFeatures(Dictionaryof_Elemental_damageImmunitiess[entry.Key], DictionaryOfElementaladditionalDamage[entry.Key])
                    .AddToDB();


                // particles for first form / shader for upgrade
                EnhancedElementalFormCondtion.SetConditionParticleReference(Dictionaryof_Elemental_Forms_EffectsParticles[entry.Key]);
                EnhancedElementalFormCondtion.SetCharacterShaderReference(Dictionaryof_Elemental_Forms_EffectsShaders[entry.Key]);


                EffectDescriptionBuilder effectDescription = new EffectDescriptionBuilder();
                effectDescription.SetDurationData(DurationType.Minute, 1, TurnOccurenceType.EndOfTurn);
                effectDescription.SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self, 1, 1, ActionDefinitions.ItemSelectionType.Equiped);
                effectDescription.AddEffectForm(new EffectFormBuilder().SetConditionForm(EnhancedElementalFormCondtion, ConditionForm.ConditionOperation.Add, true, true, new List<ConditionDefinition>()).Build());


                var guiPresentationEnhancedElementalForm = new GuiPresentationBuilder(
                     "Elemental Form : " + text + " Plane",
                     "When channeling the elemental  " + text + " Plane, you gain Immunity to " + damagetype + " damage and once per turn, apply damage of the same type to your attack"
                     )
                     .SetSpriteReference(Dictionaryof_Elemental_Forms_Sprites[entry.Key])
                     .Build();



                var EnhancedElementalFormBuilder = new FeatureDefinitionPowerSharedPoolBuilder(
                       "DH_EnhancedElementalForm_" + text,
                       ElementalFormPool,
                       RechargeRate.LongRest,
                       ActivationTime.BonusAction,
                       1,
                       false,
                       false,
                       AttributeDefinitions.Charisma,
                       effectDescription.Build(),
                       guiPresentationEnhancedElementalForm,
                       true
                       );
                FeatureDefinitionPowerSharedPool EnhancedElementalFormPower = EnhancedElementalFormBuilder.AddToDB();
                EnhancedElementalFormPower.SetOverriddenPower(DictionaryOfElementalFormPowers[entry.Key]);


                DictionaryOfEnhancedElementalFormPowers.Add(entry.Key, EnhancedElementalFormPower);






            }

        }

        public static void AtWillConjureMinorElementals()
        {
            SpellDefinitionBuilder AtWillConjureMinorElementalsBuilder = SpellDefinitionBuilder
                    .Create(ConjureMinorElementals, "DHAtWillConjureMinorElementals");
            AtWillConjureMinorElementalsBuilder.SetSpellLevel(0);

            FeatureDefinitionBonusCantripsBuilder MinorElementalBonusCantripBuilder = FeatureDefinitionBonusCantripsBuilder.Create(
                DatabaseHelper.FeatureDefinitionBonusCantripss.BonusCantripsDomainOblivion,
                 "DHConjureMinorElementalsBonusCantrip")
                .SetGuiPresentation(new GuiPresentationBuilder(
                    "Feature/&DHConjureMinorElementalsBonusCantripTitle",
                    "Feature/&DHConjureMinorElementalsBonusCantripDescription")
                .Build());

            MinorElementalBonusCantripBuilder.ClearBonusCantrips();
            MinorElementalBonusCantripBuilder.AddBonusCantrip(AtWillConjureMinorElementalsBuilder.AddToDB());
            MinorElementalBonusCantrip = MinorElementalBonusCantripBuilder.AddToDB();
        }


        public static void ElementalistSpells()
        {
            SpellListDefinition ElementalistSpellList = SpellListDefinitionBuilder
                .Create(
                    DatabaseHelper.SpellListDefinitions.SpellListPaladin,
                    "ElementalistSpellsList")
                .SetGuiPresentation("ElementalistSpellsList", Category.SpellList)
                .ClearSpells()
                .SetSpellsAtLevel(1, Thunderwave, FogCloud)
                .SetSpellsAtLevel(2, SpikeGrowth, ScorchingRay)
                .SetSpellsAtLevel(3, Fireball, LightningBolt)
                .SetSpellsAtLevel(4, Stoneskin, IceStorm, ConjureMinorElementals)
                .SetSpellsAtLevel(5, ConeOfCold, FlameStrike, ConjureElemental)
                .FinalizeSpells()
                .AddToDB();

            var ElementalistMagicAffintyBuilder = FeatureDefinitionMagicAffinityBuilder
                .Create("ElementalistSpellsMagicAffinity")
                .SetGuiPresentation(
                         new GuiPresentationBuilder(
                         "Feature/&ElementalistSpellsMagicAffinityTitle",
                         "Feature/&ElementalistSpellsMagicAffinityDescription"
                         ).Build()

                     );

            ElementalistMagicAffinity = ElementalistMagicAffintyBuilder.AddToDB();
            ElementalistMagicAffinity.SetExtendedSpellList(ElementalistSpellList);
        }
    }
}
