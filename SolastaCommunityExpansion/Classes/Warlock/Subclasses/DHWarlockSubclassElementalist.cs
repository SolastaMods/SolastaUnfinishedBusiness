using System;
using System.Collections.Generic;
using SolastaModApi;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.CustomFeatureDefinitions;
using UnityEngine.AddressableAssets;
using UnityEngine;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Classes.Warlock.Subclasses
{
    public static class DHWarlockSubclassElementalPatron
    {
        public const string Name = "DHWarlockSubclassElementalPatron";
        private static readonly string Guid = GuidHelper.Create(new Guid(Settings.GUID), Name).ToString();

        public static Dictionary<string, string> Dictionaryof_Elemental_Damage = new Dictionary<string, string>()
              {
                  { "Shadow",   RuleDefinitions.DamageTypeNecrotic  },
                  { "Astral",   RuleDefinitions.DamageTypePsychic   },
                  { "Ethereal", RuleDefinitions.DamageTypeForce     },
                  { "Fire",     RuleDefinitions.DamageTypeFire      },
                  { "Earth",    RuleDefinitions.DamageTypeBludgeoning},
                  { "Ice",      RuleDefinitions.DamageTypeCold      },
                  { "Air",      RuleDefinitions.DamageTypeThunder   }
              };
        public static FeatureDefinitionPower ElementalFormPool;
        public static Dictionary<string, FeatureDefinitionPower> DictionaryOfElementalFormPowers = new Dictionary<string, FeatureDefinitionPower>();
        public static Dictionary<string, FeatureDefinitionPower> DictionaryOfEnhancedElementalFormPowers = new Dictionary<string, FeatureDefinitionPower>();
        public static SpellDefinition AtWillCantripConjureMinorElementals;
        public static FeatureDefinitionBonusCantrips MinorElementalBonusCantrip;
        public static Dictionary<string, FeatureDefinitionAdditionalDamage> DictionaryOfElementaladditionalDamage = new Dictionary<string, FeatureDefinitionAdditionalDamage>();
        public static Dictionary<string, ConditionDefinition> DictionaryOfElementalConditions = new Dictionary<string, ConditionDefinition>();
        public static FeatureDefinitionMagicAffinity ElementalistMagicAffinity;


        public static void Build()
        {

            GuiPresentation blank = new GuiPresentationBuilder("Feature/&NoContentTitle", "Feature/&NoContentTitle").Build();
          //  blank.hidden = true;

            ElementalFormPool = new FeatureDefinitionPowerPoolBuilder
            (
                "DH_ElementalFormPool",
                GuidHelper.Create(new Guid(Settings.GUID), "DH_ElementalFormPool").ToString(),
                1,
                RuleDefinitions.UsesDetermination.ProficiencyBonus,
                AttributeDefinitions.Charisma,
                RuleDefinitions.RechargeRate.LongRest,
                blank
            ).AddToDB();


            ElementalForms();
            ElementalistSpells();


            GuiPresentation guiFeatureSet_Level01 = new GuiPresentationBuilder(
                "Feature/&ElementalPatronFeatureSet_Level01Description",
                "Feature/&ElementalPatronFeatureSet_Level01Title")
                .Build();
            var FeatureSet_Level01Builder = new FeatureDefinitionFeatureSetBuilder(
                DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetGreenmageWardenOfTheForest,
                "ElementalPatronFeatureSet_Level01",
                GuidHelper.Create(new Guid(Settings.GUID), "ElementalPatronFeatureSet_Level01").ToString(),
                guiFeatureSet_Level01
                );
            FeatureDefinitionFeatureSet FeatureSet_Level01 = FeatureSet_Level01Builder
                .ClearFeatures()
                .AddFeature(ElementalFormPool)
                .AddFeature(DictionaryOfElementalFormPowers["Shadow"  ])
                .AddFeature(DictionaryOfElementalFormPowers["Astral"  ])
                .AddFeature(DictionaryOfElementalFormPowers["Ethereal"])
                .AddFeature(DictionaryOfElementalFormPowers["Fire"    ])
                .AddFeature(DictionaryOfElementalFormPowers["Earth"   ])
                .AddFeature(DictionaryOfElementalFormPowers["Ice"     ])
                .AddFeature(DictionaryOfElementalFormPowers["Air"     ])
                .AddFeature(ElementalistMagicAffinity)
                //.AddFeature(DatabaseHelper.FeatureDefinitionMagicAffinitys.MagicAffinityShockArcanistArcaneWarfare)
                // bonus cantrip granted/selection for cantrips that deal the above damage?
                .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                .SetUniqueChoices(false)
                .AddToDB() ;


            GuiPresentation guiFeatureSet_Level06 = new GuiPresentationBuilder(
                "Feature/&ElementalPatronFeatureSet_Level06Description",
                "Feature/&ElementalPatronFeatureSet_Level06Title")
                .Build();
            var FeatureSet_Level06Builder = new FeatureDefinitionFeatureSetBuilder(
                DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetGreenmageWardenOfTheForest,
                "ElementalPatronFeatureSet_Level06",
                GuidHelper.Create(new Guid(Settings.GUID), "ElementalPatronFeatureSet_Level06").ToString(),
                guiFeatureSet_Level06
                );
            FeatureDefinitionFeatureSet FeatureSet_Level06 = FeatureSet_Level06Builder
                .ClearFeatures()
                .AddFeature(DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageRangerFavoredEnemyElemental)
                .AddFeature(DatabaseHelper.FeatureDefinitionCombatAffinitys.CombatAffinityProtectedFromEvil)
                .AddFeature(DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityCircleLandNaturesWardCharmed)
                .AddFeature(DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityCircleLandNaturesWardFrightened)
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
                "Feature/&ElementalPatronFeatureSet_Level10Description",
                "Feature/&ElementalPatronFeatureSet_Level10Title")
                .Build();
            var FeatureSet_Level10Builder = new FeatureDefinitionFeatureSetBuilder(
                DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetGreenmageWardenOfTheForest,
                "ElementalPatronFeatureSet_Level10",
                GuidHelper.Create(new Guid(Settings.GUID), "ElementalPatronFeatureSet_Level10").ToString(),
                guiFeatureSet_Level10
                );
            FeatureDefinitionFeatureSet FeatureSet_Level10 = FeatureSet_Level10Builder
                .ClearFeatures()
                .AddFeature(DictionaryOfEnhancedElementalFormPowers["Shadow"])
                .AddFeature(DictionaryOfEnhancedElementalFormPowers["Astral"])
                .AddFeature(DictionaryOfEnhancedElementalFormPowers["Ethereal"])
                .AddFeature(DictionaryOfEnhancedElementalFormPowers["Fire"])
                .AddFeature(DictionaryOfEnhancedElementalFormPowers["Earth"])
                .AddFeature(DictionaryOfEnhancedElementalFormPowers["Ice"])
                .AddFeature(DictionaryOfEnhancedElementalFormPowers["Air"])
                .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                .SetUniqueChoices(false)
                .AddToDB();

            // cantrip at will version of Protection from Energy or conjure minor elementals


            AtWillConjureMinorElementals();


            var subclassGuiPresentation = new GuiPresentationBuilder(
                    "Subclass/&DHWarlockSubclassElementalPatronDescription",
                    "Subclass/&DHWarlockSubclassElementalPatronTitle")
                    .SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.TraditionLoremaster.GuiPresentation.SpriteReference)
                    .Build();

            var Elementalist = new CharacterSubclassDefinitionBuilder(Name, Guid);
            Elementalist.SetGuiPresentation(subclassGuiPresentation)
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
                { "Shadow",  DatabaseHelper.MonsterDefinitions.Sorr_Akkath_Shikkath.MonsterPresentation.attachedParticlesReference },
                { "Astral",  DatabaseHelper.MonsterDefinitions.WightLord.MonsterPresentation.attachedParticlesReference},// backup kindredspirits bear - spectral spider
                { "Ethereal",DatabaseHelper.MonsterDefinitions.Ghost.MonsterPresentation.attachedParticlesReference},      // backup fey bear - sorr boss
                { "Fire",    DatabaseHelper.MonsterDefinitions.Fire_Jester.MonsterPresentation.attachedParticlesReference}, // fire jester
                { "Earth",   DatabaseHelper.MonsterDefinitions.Earth_Elemental.MonsterPresentation.attachedParticlesReference}, // or stone barbarian's ConditionStoneResilience.conditionParticleReference
                { "Ice",     DatabaseHelper.MonsterDefinitions.WindSnake.MonsterPresentation.attachedParticlesReference},// skarn ghoul
                { "Air",     DatabaseHelper.MonsterDefinitions.Air_Elemental.MonsterPresentation.attachedParticlesReference}
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

            GuiPresentation blank = new GuiPresentationBuilder("Feature/&NoContentTitle", "Feature/&NoContentTitle").Build();



            foreach (KeyValuePair<string, string> entry in Dictionaryof_Elemental_Damage)
            {

                string text = entry.Key;
                string damagetype = entry.Value.Substring(6);

                var guiPresentationElementalFormDamage = new GuiPresentationBuilder(
                    "When channeling the elemental " + text + " Plane, additional " + damagetype + " damage, equal to your Proficiency Bonus, is added to the total",
                    "Elemental Damage : " + text + " Plane")
                    .Build();

                FeatureDefinitionAdditionalDamage additionalDamage = new FeatureDefinitionAdditionalDamageBuilder(
                     "DH_ElementalForm_" + text + "additionalDamage",
                     GuidHelper.Create(new Guid(Settings.GUID), "DH_ElementalForm_" + text + "additionalDamage").ToString(),
                     "ElementalDamage",
                     RuleDefinitions.FeatureLimitedUsage.OncePerTurn,
                     RuleDefinitions.AdditionalDamageValueDetermination.ProficiencyBonus,
                     RuleDefinitions.AdditionalDamageTriggerCondition.AlwaysActive,
                     RuleDefinitions.AdditionalDamageRequiredProperty.MeleeWeapon,
                     false,
                     RuleDefinitions.DieType.D4,
                     1,
                     RuleDefinitions.AdditionalDamageType.Specific,
                     entry.Value,
                     RuleDefinitions.AdditionalDamageAdvancement.None,
                     new List<DiceByRank>(),
                     guiPresentationElementalFormDamage
                    ).AddToDB();


                var guiPresentationElementalFormCondition = new GuiPresentationBuilder(
                    "When channeling the elemental  " + text + " Plane, you gain resistence to " + damagetype + " damage and once per turn, apply damage of the same type to your attack",
                    "Elemental Form : " + text + " Plane" + " Condition")
                    .Build();

                ConditionDefinition ElementalFormCondtion = new ConditionDefinitionBuilder(
                    "DH_ElementalForm_" + text + "Condition", GuidHelper.Create(new Guid(Settings.GUID), "DH_ElementalForm_" + text + "Condition").ToString(),
                    new List<FeatureDefinition>() {
                        Dictionaryof_Elemental_damageResistances[entry.Key],
                        additionalDamage },
                    RuleDefinitions.DurationType.Minute, 1, false, guiPresentationElementalFormCondition
                    ).AddToDB();



                ElementalFormCondtion.conditionParticleReference = Dictionaryof_Elemental_Forms_EffectsParticles[entry.Key];


                EffectDescriptionBuilder effectDescription = new EffectDescriptionBuilder();
                effectDescription.SetDurationData(RuleDefinitions.DurationType.Minute, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn);
                effectDescription.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Self, 1, RuleDefinitions.TargetType.Self, 1, 1, ActionDefinitions.ItemSelectionType.Equiped);
                effectDescription.AddEffectForm(new EffectFormBuilder().SetConditionForm(ElementalFormCondtion, ConditionForm.ConditionOperation.Add, true, true, new List<ConditionDefinition>()).Build());


                var guiPresentationElementalForm = new GuiPresentationBuilder(
                     "When channeling the elemental  " + text + " Plane, you gain resistence to " + damagetype + " damage and once per turn, apply damage of the same type to your attack",
                     "Elemental Form : " + text + " Plane" )
                     .SetSpriteReference(Dictionaryof_Elemental_Forms_Sprites[entry.Key])
                     .Build();



                var ElementalFormBuilder = new FeatureDefinitionPowerSharedPoolBuilder(
                       "DH_ElementalForm_" + text,
                       GuidHelper.Create(new Guid(Settings.GUID), "DH_ElementalForm_" + text).ToString(),
                       ElementalFormPool,
                       RuleDefinitions.RechargeRate.LongRest,
                       RuleDefinitions.ActivationTime.BonusAction,
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
                { "Shadow",  DatabaseHelper.ConditionDefinitions.ConditionSorcererChildRiftDeflection.characterShaderReference},
                { "Astral",  DatabaseHelper.MonsterDefinitions.WightLord.MonsterPresentation.attachedParticlesReference},// backup kindredspirits bear - spectral spider
                { "Ethereal",DatabaseHelper.MonsterDefinitions.Ghost.MonsterPresentation.attachedParticlesReference},      // backup fey bear - sorr boss
                { "Fire",    DatabaseHelper.MonsterDefinitions.Fire_Jester.MonsterPresentation.attachedParticlesReference}, // fire jester
                { "Earth",   DatabaseHelper.MonsterDefinitions.Earth_Elemental.MonsterPresentation.attachedParticlesReference}, // or stone barbarian's ConditionStoneResilience.conditionParticleReference
                { "Ice",     DatabaseHelper.MonsterDefinitions.WindSnake.MonsterPresentation.attachedParticlesReference},// skarn ghoul
                { "Air",     DatabaseHelper.MonsterDefinitions.Air_Elemental.MonsterPresentation.attachedParticlesReference}
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

            GuiPresentation blank = new GuiPresentationBuilder("Feature/&NoContentTitle", "Feature/&NoContentTitle").Build();
          //  blank.hidden = true;



            foreach (KeyValuePair<string, string> entry in Dictionaryof_Elemental_Damage)
            {

                string text = entry.Key;
                string damagetype = entry.Value.Substring(6);

                var guiPresentationEnhancedElementalFormCondition = new GuiPresentationBuilder(
                    "When channeling the elemental  " + text + " Plane, you gain Immunity to " + damagetype + " damage and once per turn, apply damage of the same type to your attack",
                    "Elemental Form : " + text + " Plane" + " Condition")
                    .Build();

                ConditionDefinition EnhancedElementalFormCondtion = new ConditionDefinitionBuilder(
                    "DH_EnhancedElementalForm_" + text + "Condition", GuidHelper.Create(new Guid(Settings.GUID), "DH_EnhancedElementalForm_" + text + "Condition").ToString(),
                    new List<FeatureDefinition>() {
                        Dictionaryof_Elemental_damageImmunitiess[entry.Key],
                        DictionaryOfElementaladditionalDamage[entry.Key] },
                    RuleDefinitions.DurationType.Minute, 1, false, guiPresentationEnhancedElementalFormCondition
                    ).AddToDB();


                // particles for first form / shader for upgrade
                EnhancedElementalFormCondtion.conditionParticleReference = Dictionaryof_Elemental_Forms_EffectsParticles[entry.Key];

                EnhancedElementalFormCondtion.characterShaderReference = Dictionaryof_Elemental_Forms_EffectsShaders[entry.Key];


                EffectDescriptionBuilder effectDescription = new EffectDescriptionBuilder();
                effectDescription.SetDurationData(RuleDefinitions.DurationType.Minute, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn);
                effectDescription.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Self, 1, RuleDefinitions.TargetType.Self, 1, 1, ActionDefinitions.ItemSelectionType.Equiped);
                effectDescription.AddEffectForm(new EffectFormBuilder().SetConditionForm(EnhancedElementalFormCondtion, ConditionForm.ConditionOperation.Add, true, true, new List<ConditionDefinition>()).Build());


                var guiPresentationEnhancedElementalForm = new GuiPresentationBuilder(
                     "When channeling the elemental  " + text + " Plane, you gain Immunity to " + damagetype + " damage and once per turn, apply damage of the same type to your attack",
                     "Elemental Form : " + text + " Plane")
                     .SetSpriteReference(Dictionaryof_Elemental_Forms_Sprites[entry.Key])
                     .Build();



                var EnhancedElementalFormBuilder = new FeatureDefinitionPowerSharedPoolBuilder(
                       "DH_EnhancedElementalForm_" + text,
                       GuidHelper.Create(new Guid(Settings.GUID), "DH_EnhancedElementalForm_" + text).ToString(),
                       ElementalFormPool,
                       RuleDefinitions.RechargeRate.LongRest,
                       RuleDefinitions.ActivationTime.BonusAction,
                       1,
                       false,
                       false,
                       AttributeDefinitions.Charisma,
                       effectDescription.Build(),
                       guiPresentationEnhancedElementalForm,
                       true
                       );
                FeatureDefinitionPowerSharedPool EnhancedElementalFormPower = EnhancedElementalFormBuilder.AddToDB();
                EnhancedElementalFormPower.overriddenPower = DictionaryOfElementalFormPowers[entry.Key];


                DictionaryOfEnhancedElementalFormPowers.Add(entry.Key, EnhancedElementalFormPower);






            }

        }

        public static void AtWillConjureMinorElementals()
        {

            SpellBuilder AtWillConjureMinorElementalsBuilder = new SpellBuilder(
                 DatabaseHelper.SpellDefinitions.ConjureMinorElementals,
                 "DHAtWillConjureMinorElementals",
                 GuidHelper.Create(new System.Guid(Settings.GUID), "DHAtWillConjureMinorElementals").ToString());
            AtWillConjureMinorElementalsBuilder.SetSpellLevel(0);
            AtWillCantripConjureMinorElementals = AtWillConjureMinorElementalsBuilder.AddToDB();

            FeatureDefinitionBonusCantripsBuilder MinorElementalBonusCantripBuilder = new FeatureDefinitionBonusCantripsBuilder(
                DatabaseHelper.FeatureDefinitionBonusCantripss.BonusCantripsDomainOblivion,
                 "DHConjureMinorElementalsBonusCantrip",
                 GuidHelper.Create(new System.Guid(Settings.GUID), "DHAtWillConjureMinorElementalsBonusCantrip").ToString(),
                 new GuiPresentationBuilder("Feature/&DHConjureMinorElementalsBonusCantripDescription", "Feature/&DHConjureMinorElementalsBonusCantripTitle").Build());
            MinorElementalBonusCantripBuilder.ClearBonusCantrips();
            MinorElementalBonusCantripBuilder.AddBonusCantrip(AtWillCantripConjureMinorElementals);
             MinorElementalBonusCantrip = MinorElementalBonusCantripBuilder.AddToDB();



        }


        public static void ElementalistSpells()
        {
            SpellListDefinition ElementalistSpellList = new SpellListBuilder //CopyAndCreateNewBlueprint<SpellListDefinition>.CreateCopy
            (
                "ElementalistSpellsList",
                GuidHelper.Create(new Guid(Settings.GUID), "ElementalistSpellsList").ToString(),
                "SpellList/&ElementalistSpellsListTitle",
                // "SpellList/&ElementalistSpellsListDescription",
                DatabaseHelper.SpellListDefinitions.SpellListSkeletonKnight,
                new List<SpellDefinition>
                { }
            ).AddToDB();

            ElementalistSpellList.SpellsByLevel.Clear();
            ElementalistSpellList.SpellsByLevel.AddRange(new List<SpellListDefinition.SpellsByLevelDuplet>()
            {
                new SpellListDefinition.SpellsByLevelDuplet
                {
                    Level =0,
                    Spells = new List<SpellDefinition>
                    {
                    }
                },
                new SpellListDefinition.SpellsByLevelDuplet
                {
                    Level =1,
                    Spells = new List<SpellDefinition>
                    {
                        DatabaseHelper.SpellDefinitions.Thunderwave,
                        DatabaseHelper.SpellDefinitions.FogCloud
                    }
                },
                new SpellListDefinition.SpellsByLevelDuplet
                {
                    Level =2,
                    Spells = new List<SpellDefinition>
                    {
                        DatabaseHelper.SpellDefinitions.SpikeGrowth,
                        DatabaseHelper.SpellDefinitions.ScorchingRay
                    }
                },
                new SpellListDefinition.SpellsByLevelDuplet
                {
                    Level =3,
                    Spells = new List<SpellDefinition>
                    {
                        DatabaseHelper.SpellDefinitions.Fireball,
                        DatabaseHelper.SpellDefinitions.LightningBolt
                    }
                },
                new SpellListDefinition.SpellsByLevelDuplet
                {
                    Level =4,
                    Spells = new List<SpellDefinition>
                    {
                        DatabaseHelper.SpellDefinitions.Stoneskin,
                        DatabaseHelper.SpellDefinitions.IceStorm,
                        DatabaseHelper.SpellDefinitions.ConjureMinorElementals
                    }
                },
                new SpellListDefinition.SpellsByLevelDuplet
                {
                    Level =5,
                    Spells = new List<SpellDefinition>
                    {
                        DatabaseHelper.SpellDefinitions.ConeOfCold,
                        DatabaseHelper.SpellDefinitions.FlameStrike,
                        DatabaseHelper.SpellDefinitions.ConjureElemental
                    }
                },

            });


            var ElementalistMagicAffintyBuilder = new FeatureDefinitionMagicAffinityBuilder
                     (
                         "ElementalistSpellsMagicAffinity",
                         GuidHelper.Create(new Guid(Settings.GUID), "ElementalistSpellsMagicAffinity").ToString(),
                         new GuiPresentationBuilder(
                         "Feature/&ElementalistSpellsMagicAffinityDescription",
                         "Feature/&ElementalistSpellsMagicAffinityTitle"
                         ).Build()

                     );
            ElementalistMagicAffinity = ElementalistMagicAffintyBuilder.AddToDB();

            ElementalistMagicAffinity.SetExtendedSpellList(ElementalistSpellList);

        }

    }



}
