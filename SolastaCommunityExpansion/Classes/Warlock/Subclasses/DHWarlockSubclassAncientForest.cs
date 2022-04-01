using System;
using System.Collections.Generic;
using UnityEngine;
using SolastaModApi;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi.Extensions;
using static RuleDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaModApi.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;
using static SolastaModApi.DatabaseHelper.MonsterDefinitions;
using static SolastaModApi.DatabaseHelper.SpellListDefinitions;
using static SolastaModApi.DatabaseHelper.ItemDefinitions;

namespace SolastaCommunityExpansion.Classes.Warlock.Subclasses
{
    public static class DHWarlockSubclassAncientForestPatron
    {
        public static CharacterSubclassDefinition Build()
        {
            var ancientForestSpelllist = SpellListDefinitionBuilder
                .Create(SpellListPaladin, "AncientForestExpandedSpelllist", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Feature)
                .SetGuiPresentationNoContent()
                .ClearSpells()
                .SetSpellsAtLevel(1, Goodberry, Entangle)
                .SetSpellsAtLevel(2, Barkskin, SpikeGrowth)
                .SetSpellsAtLevel(3, MassHealingWord, VampiricTouch)
                .SetSpellsAtLevel(4, Blight, GreaterRestoration)
                .SetSpellsAtLevel(5, Contagion, RaiseDead)
                .SetMaxSpellLevel()
                .AddToDB();

            //    necrotic and healing

            FeatureDefinitionMagicAffinity ancientForestExpandedSpellListAffinity = FeatureDefinitionMagicAffinityBuilder
                .Create("AncientForestExpandedSpelllistAfinity", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Feature)
                .SetExtendedSpellList(ancientForestSpelllist)
                .AddToDB();

            /*
             * Dyrad 
**OR**
Primal/Ancestral Spirits/Spirit Animals
 haunted wood, or a powerful hag 
Ancient Tree
Earthmother
God/s of the Wild Hunt
Ancient Druid (parallels Liches for Undying Patron)
Wild Fey
Different Archfey, e.g. Winter-themed

            https://www.reddit.com/r/DnD/comments/d67nun/5e_warlock_pact_of_thorns/
            https://www.reddit.com/r/DnDHomebrew/comments/cg1mnc/warlock_patron_world_tree_3rd_version_with/
            https://mfov.magehandpress.com/2017/11/the-great-tree.html
            https://rpg.stackexchange.com/questions/162444/is-this-homebrew-druid-themed-warlock-patron-balanced
            https://www.dndbeyond.com/subclasses/773271-the-plant
            https://preview.redd.it/uf6s9b3gmvr61.jpg?width=640&crop=smart&auto=webp&s=737b932a2dba1556c1962d99ec362f2b20be5314
            https://www.reddit.com/r/DnDHomebrew/comments/gji69r/warlock_patron_the_grovekeeper_because_being_a/
            https://www.reddit.com/r/UnearthedArcana/comments/iv8bhj/the_archdruid_patron_version_21_a_warlock_patron/
            */

            /*
             Dyrad
             1
            expanded spell list from primeval growth or plant patron
            PB necro damage once per turn
            6

            Witherbloom Brew rest activity

            10
            Photosynthetic Skin- While you are in direct light, you gain temporary hit points equal to your PB at the start of each of your turns.
            rooted (in direct sunlight, you regain hit points equal to your Charisma modifier at the start of each of your turns, zero movement, advantage on strentgh checks and use CHA for strength checks)
            14
            barkskin
             * 
             * */

            /*
                        FeatureDefinitionAdditionalDamage AdditionalDamageLifeSap = new FeatureDefinitionAdditionalDamageBuilder(
                                 "AdditionalDamageAncientForestLifeSap",
                                 GuidHelper.Create(new Guid(Settings.GUID), "AdditionalDamageAncientForestLifeSap").ToString(),
                                 "AncientForestLifeSap",
                                 RuleDefinitions.FeatureLimitedUsage.None,
                                 RuleDefinitions.AdditionalDamageValueDetermination.SpellcastingBonus,
                                 RuleDefinitions.AdditionalDamageTriggerCondition.SpellDamageMatchesSourceAncestry,
                                 RuleDefinitions.AdditionalDamageRequiredProperty.None,
                                 false,
                                 RuleDefinitions.DieType.D4,
                                 1,
                                 RuleDefinitions.AdditionalDamageType.AncestryDamageType,
                                 RuleDefinitions.DamageTypeForce,
                                 RuleDefinitions.AdditionalDamageAdvancement.None,
                                 new List<DiceByRank>(),
                                 new GuiPresentationBuilder("Feature/&AdditionalDamageAncientForestLifeSapDescription", "Feature/&AdditionalDamageAncientForestLifeSapTitle").Build()
                            ).AddToDB();

                     //   FeatureDefinitionFeatureSet AncientForestLifeSapFeatureSet = FeatureDefinitionFeatureSetBuilder
                     //       .Create(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetGreenmageWardenOfTheForest, "AncientForestLifeSapFeatureSet", GuidHelper.Create(new Guid(Settings.GUID), "AncientForestLifeSapFeatureSet").ToString())
                     //       .SetGuiPresentation(new GuiPresentationBuilder("Feature/&AncientForestLifeSapFeatureSetDescription", "Feature/&AncientForestLifeSapFeatureSetTitle").Build())
                     //      .ClearFeatureSet() 
                     //      .AddFeatureSet(AdditionalDamageAncientForestLifeSap)
                     //      .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                     //      .SetUniqueChoices(false)
                     //      .AddToDB();
            */


            FeatureDefinitionPower LifeSap = FeatureDefinitionPowerBuilder
                .Create("AncientForestLifeSap", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Power)
                .Configure(
                       1,
                       UsesDetermination.ProficiencyBonus,
                       AttributeDefinitions.Charisma,
                       ActivationTime.OnAttackSpellHitAutomatic,
                       1,
                       RechargeRate.AtWill,
                       false,
                       false,
                       AttributeDefinitions.Charisma,
                       new EffectDescriptionBuilder()
                            .AddEffectForm(
                                new EffectFormBuilder().SetDamageForm(
                                    false,
                                    DieType.D1,
                                    DamageTypeNecrotic,
                                    0,
                                    DieType.D1,
                                    0,
                                    HealFromInflictedDamage.Half,
                                    new List<RuleDefinitions.TrendInfo>())
                                .SetBonusMode(AddBonusMode.AbilityBonus)
                                .Build())
                            .SetTargetFiltering(TargetFilteringMethod.CharacterOnly, TargetFilteringTag.UnderHalfHitPoints, 0, DieType.D1)
                            .SetTargetingData(
                                    Side.Enemy,
                                    RangeType.Distance,
                                    1,
                                    TargetType.Individuals,
                                    1,
                                    1,
                                    ActionDefinitions.ItemSelectionType.None)
                            .Build()
                       ,
                       true)
                .AddToDB();

            FeatureDefinitionPower Regrowth = FeatureDefinitionPowerBuilder
                .Create(PowerPaladinLayOnHands, "AncientForestRegrowth", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Feature)
                .AddToDB();
            Regrowth.GuiPresentation.SetSpriteReference(PowerFunctionGoodberryHealing.GuiPresentation.SpriteReference);


            FeatureDefinitionBonusCantrips AncientForestBonusCantrip = FeatureDefinitionBonusCantripsBuilder
                .Create(DatabaseHelper.FeatureDefinitionBonusCantripss.BonusCantripsDomainOblivion, "DHAncientForestBonusCantrip", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Feature)
                .ClearBonusCantrips()
                .AddBonusCantrip(Shillelagh)
                .AddBonusCantrip(ChillTouch)
                .AddToDB();

            FeatureDefinitionPower HerbalBrewPool = FeatureDefinitionPowerPoolBuilder
                 .Create("DH_HerbalBrewPool", GuidHelper.Create(new Guid(Settings.GUID), "DH_HerbalBrewPool").ToString())
                .SetGuiPresentationNoContent()
                .SetUsesProficiency()
                .SetUsesAbility(1, AttributeDefinitions.Charisma)
                .SetRechargeRate(RechargeRate.LongRest)
                .AddToDB();

            Dictionary<string, FeatureDefinitionDamageAffinity> ResistentBrewsDamageAffinitys = new()
            {
                { "ResistentToRadiant", DamageAffinityRadiantResistance },
                { "ResistentToNecrotic", DamageAffinityNecroticResistance },
                { "ResistentToPoison", DamageAffinityPoisonResistance },
                { "ResistentToAcid", DamageAffinityAcidResistance },
                { "ResistentToLightning", DamageAffinityLightningResistance },
            };

            List<FeatureDefinitionPower> ResistentBrews = new();

            foreach (KeyValuePair<string, FeatureDefinitionDamageAffinity> entry in ResistentBrewsDamageAffinitys)
            {

                string text = entry.Key;
                string damagetype = entry.Value.DamageType.Substring(6);

                var guiPresentationResistentBrewsCondition = new GuiPresentationBuilder(
                "You gain resistence to " + damagetype + " damage for one hour",
                "Herbal Brew : Potion of " + damagetype + " Resistence")
                .Build();

                ConditionDefinition ResistentBrewsCondition = ConditionDefinitionBuilder.Create(
                    "DH_ResistentBrews_" + text + "Condition", GuidHelper.Create(new Guid(Settings.GUID), "DH_ResistentBrews_" + text + "Condition").ToString())
                    .SetDuration(DurationType.Hour, 1)
                    .SetSilent(Silent.None)
                    .SetGuiPresentation(guiPresentationResistentBrewsCondition)
                    .AddFeatures(ResistentBrewsDamageAffinitys[entry.Key])
                    .AddToDB();


                var guiPresentationResistentBrewsItem = new GuiPresentationBuilder(
               "This herbal brew grants resistence to " + damagetype + " damage for one hour",
               "Herbal Brew : Potion of " + damagetype + " resistence")
               .Build();


                FeatureDefinitionPower PotionFunction = FeatureDefinitionPowerBuilder
                    .Create("AncientForestPotionFunction" + entry.Key, DefinitionBuilder.CENamespaceGuid)
                    .SetGuiPresentation(guiPresentationResistentBrewsItem)
                    .Configure(
                           1,
                           UsesDetermination.Fixed,
                           AttributeDefinitions.Charisma,
                           ActivationTime.Action,
                           1,
                           RechargeRate.AtWill,
                           false,
                           false,
                           AttributeDefinitions.Charisma,
                           new EffectDescriptionBuilder()
                                .AddEffectForm(
                                    new EffectFormBuilder().SetConditionForm(
                                        ResistentBrewsCondition,
                                        ConditionForm.ConditionOperation.Add,
                                        true,
                                        true,
                                        new List<ConditionDefinition>()).Build())
                                .SetDurationData(DurationType.Hour, 1)
                                .SetTargetingData(
                                        Side.Ally,
                                        RangeType.Self,
                                        1,
                                        TargetType.Self,
                                        1,
                                        1,
                                        ActionDefinitions.ItemSelectionType.None)
                                .Build()
                                ,
                           true)
                    .AddToDB();

                ItemDefinition Potion = ItemDefinitionBuilder.Create(
                    PotionOfInvisibility, "AncientForestPotionOf" + entry.Key + "Resistance", DefinitionBuilder.CENamespaceGuid)
                    .SetGuiPresentation(guiPresentationResistentBrewsItem)
                    .MakeMagical()
                    .SetUsableDeviceDescription(PotionFunction)
                    .AddToDB();
                Potion.SetIsFood(true);
                Potion.SetFoodDescription(new FoodDescription().SetNutritiveCapacity(0).SetPerishable(true));

                /*               FeatureDefinitionPower HerbalBrewFortifying = FeatureDefinitionPowerBuilder
                                   .Create("AncientForestHerbalBrewFortifying"+ entry.Key + "Resistance", DefinitionBuilder.CENamespaceGuid)
                                   .SetGuiPresentation(guiPresentationResistentBrewsItem)
                                   .Configure(
                                          1,
                                          RuleDefinitions.UsesDetermination.ProficiencyBonus,
                                          AttributeDefinitions.Charisma,
                                          RuleDefinitions.ActivationTime.Rest,
                                          1,
                                          RuleDefinitions.RechargeRate.LongRest,
                                          false,
                                          false,
                                          AttributeDefinitions.Charisma,
                                          new EffectDescriptionBuilder()
                                               .AddEffectForm(
                                                   new EffectFormBuilder().SetSummonForm(
                                                       SummonForm.Type.InventoryItem,
                                                       Potion,
                                                       1,
                                                       Wolf.name,
                                                       null,
                                                       true,
                                                       null,
                                                       ScriptableObject.CreateInstance<EffectProxyDefinition>()
                                                       )
                                                   .SetBonusMode(AddBonusMode.DoubleProficiency)
                                                   .Build())
                                               .SetDurationData(DurationType.UntilLongRest, 1)
                                               .SetTargetingData(
                                                       RuleDefinitions.Side.Ally,
                                                       RuleDefinitions.RangeType.Self,
                                                       1,
                                                       RuleDefinitions.TargetType.Self,
                                                       1,
                                                       1,
                                                       ActionDefinitions.ItemSelectionType.Equiped)
                                               .Build()
                                          ,
                                          true)
                                   .AddToDB();
               */

                FeatureDefinitionPower HerbalBrewFortifying = new FeatureDefinitionPowerSharedPoolBuilder(
                       "AncientForestHerbalBrewFortifying" + entry.Key + "Resistance",
                       GuidHelper.Create(new Guid(Settings.GUID), "AncientForestHerbalBrewFortifying" + entry.Key + "Resistance").ToString(),
                       HerbalBrewPool,
                       RechargeRate.LongRest,
                       ActivationTime.Rest,
                       1,
                       false,
                       false,
                       AttributeDefinitions.Charisma,
                       new EffectDescriptionBuilder()
                                .AddEffectForm(
                                    new EffectFormBuilder().SetSummonForm(
                                        SummonForm.Type.InventoryItem,
                                        Potion,
                                        1,
                                        Wolf.name,
                                        null,
                                        true,
                                        null,
                                        ScriptableObject.CreateInstance<EffectProxyDefinition>()
                                        )
                                    .SetBonusMode(AddBonusMode.DoubleProficiency)
                                    .Build())
                                .SetDurationData(DurationType.UntilLongRest, 0)//, 1)
                                .SetTargetingData(
                                        Side.Ally,
                                        RangeType.Self,
                                        1,
                                        TargetType.Self,
                                        1,
                                        1,
                                        ActionDefinitions.ItemSelectionType.Equiped)
                                .Build(),
                       guiPresentationResistentBrewsItem,
                       false
                       ).AddToDB();

                // unused variable is set to default value rather than being forced to 0
                //HerbalBrewFortifying.EffectDescription.DurationParameter = 1;


                ResistentBrews.Add(HerbalBrewFortifying);

                RestActivityDefinitionBuilder
                    .Create("AncientForestRestActivityBrewingFortify" + entry.Key + "Resistance", DefinitionBuilder.CENamespaceGuid)
                    .SetRestData(
                        RestDefinitions.RestStage.AfterRest,
                        RestType.LongRest,
                        RestActivityDefinition.ActivityCondition.CanUsePower,
                        "UsePower",
                        HerbalBrewFortifying.name)
                    .SetGuiPresentation(HerbalBrewFortifying.GuiPresentation.Title, HerbalBrewFortifying.GuiPresentation.Description)
                    .AddToDB();
            }


            //   need to create shared pool for herbal brewing

            //         var HerbalBrewBuilder = new FeatureDefinitionPowerSharedPoolBuilder(
            //              "DHHerbalBrew" + text,
            //              GuidHelper.Create(new Guid(Settings.GUID), "DHHerbalBrew" + text).ToString(),
            //              HerbalBrewPool,
            //              RuleDefinitions.RechargeRate.LongRest,
            //              RuleDefinitions.ActivationTime.BonusAction,
            //              1,
            //              false,
            //              false,
            //              AttributeDefinitions.Charisma,
            //              effectDescription.Build(),
            //              guiPresentationHerbalBrew,
            //              true
            //              );
            //   FeatureDefinitionPowerSharedPool HerbalBrewPower = HerbalBrewBuilder.AddToDB();


            ItemDefinition HealingBrew = ItemDefinitionBuilder.Create(
                PotionOfHealing, "AncientForestPotionOfHealing", DefinitionBuilder.CENamespaceGuid)
                .AddToDB();
            HealingBrew.SetIsFood(true);
            HealingBrew.SetFoodDescription(new FoodDescription().SetNutritiveCapacity(0).SetPerishable(true));


            FeatureDefinitionPower HerbalBrewQuickening = new FeatureDefinitionPowerSharedPoolBuilder(
                   "AncientForestHerbalBrewQuickening",
                   GuidHelper.Create(new Guid(Settings.GUID), "AncientForestHerbalBrewQuickening").ToString(),
                   HerbalBrewPool,
                   RechargeRate.LongRest,
                   ActivationTime.Rest,
                   1,
                   false,
                   false,
                   AttributeDefinitions.Charisma,
                   new EffectDescriptionBuilder()
                            .AddEffectForm(
                                new EffectFormBuilder().SetSummonForm(
                                    SummonForm.Type.InventoryItem,
                                    HealingBrew,
                                    1,
                                    Wolf.name,
                                    null,
                                    true,
                                    null,
                                    ScriptableObject.CreateInstance<EffectProxyDefinition>()
                                    )
                                .SetBonusMode(AddBonusMode.DoubleProficiency)
                                .Build())
                            .SetDurationData(DurationType.UntilLongRest, 0)
                            .SetTargetingData(
                                    Side.Ally,
                                    RangeType.Self,
                                    1,
                                    TargetType.Self,
                                    1,
                                    1,
                                    ActionDefinitions.ItemSelectionType.Equiped)
                            .Build(),
                    new GuiPresentationBuilder(
               HealingBrew.GuiPresentation.Description,
               "Equipment/&HerbalBrewQuickeningTitle")
               .Build(),
                   false
                   ).AddToDB();

            // Herbal Brew (Quickening) : Potion of Healing

            /*
                        FeatureDefinitionPower HerbalBrewQuickening = FeatureDefinitionPowerBuilder
                            .Create("AncientForestHerbalBrewQuickening", DefinitionBuilder.CENamespaceGuid)
                            .SetGuiPresentation(Category.Power)
                            .Configure(
                                   1,
                                   RuleDefinitions.UsesDetermination.ProficiencyBonus,
                                   AttributeDefinitions.Charisma,
                                   RuleDefinitions.ActivationTime.Rest,
                                   1,
                                   RuleDefinitions.RechargeRate.LongRest,
                                   false,
                                   false,
                                   AttributeDefinitions.Charisma,
                                   new EffectDescriptionBuilder()
                                        .AddEffectForm(
                                            new EffectFormBuilder().SetSummonForm(
                                                SummonForm.Type.InventoryItem,
                                                HealingBrew,
                                                1,
                                                Wolf.name,
                                                null,
                                                true,
                                                null,
                                                ScriptableObject.CreateInstance<EffectProxyDefinition>()
                                                )
                                            .SetBonusMode(AddBonusMode.DoubleProficiency)
                                            .Build())
                                        .SetDurationData(DurationType.UntilLongRest,0)
                                        .SetTargetingData(
                                                RuleDefinitions.Side.Ally,
                                                RuleDefinitions.RangeType.Self,
                                                1,
                                                RuleDefinitions.TargetType.Self,
                                                1,
                                                1,
                                                ActionDefinitions.ItemSelectionType.Equiped)
                                        .Build()
                                   ,
                                   true)
                            .AddToDB();
            */

            RestActivityDefinitionBuilder
                .Create("AncientForestQuickeningRestActivity", DefinitionBuilder.CENamespaceGuid)
                .SetRestData(
                    RestDefinitions.RestStage.AfterRest,
                    RestType.LongRest,
                    RestActivityDefinition.ActivityCondition.CanUsePower,
                    "UsePower",
                    HerbalBrewQuickening.name)
                .SetGuiPresentation(HerbalBrewQuickening.GuiPresentation.Title, HerbalBrewQuickening.GuiPresentation.Description)
                .AddToDB();

            ItemDefinition ToxicBrew = ItemDefinitionBuilder.Create(
                Poison_Basic, "AncientForestToxicBrew", DefinitionBuilder.CENamespaceGuid)
                .AddToDB();
            ToxicBrew.SetIsFood(true);
            ToxicBrew.SetFoodDescription(new FoodDescription().SetNutritiveCapacity(0).SetPerishable(true));


            FeatureDefinitionPower HerbalBrewToxifying = new FeatureDefinitionPowerSharedPoolBuilder(
                 "AncientForestHerbalBrewToxifying",
                 GuidHelper.Create(new Guid(Settings.GUID), "AncientForestHerbalBrewToxifying").ToString(),
                 HerbalBrewPool,
                 RechargeRate.LongRest,
                 ActivationTime.Rest,
                 1,
                 false,
                 false,
                 AttributeDefinitions.Charisma,
                 new EffectDescriptionBuilder()
                          .AddEffectForm(
                              new EffectFormBuilder().SetSummonForm(
                                  SummonForm.Type.InventoryItem,
                                  ToxicBrew,
                                  1,
                                  Wolf.name,
                                  null,
                                  true,
                                  null,
                                  ScriptableObject.CreateInstance<EffectProxyDefinition>()
                                  )
                              .SetBonusMode(AddBonusMode.DoubleProficiency)
                              .Build())
                          .SetDurationData(DurationType.UntilLongRest, 0)
                          .SetTargetingData(
                                  Side.Ally,
                                  RangeType.Self,
                                  1,
                                  TargetType.Self,
                                  1,
                                  1,
                                  ActionDefinitions.ItemSelectionType.Equiped)
                          .Build(),
                 new GuiPresentationBuilder(
                 ToxicBrew.GuiPresentation.Description,
                 "Equipment/&HerbalBrewToxifyingTitle")
                 .Build(),
                 false
                 ).AddToDB();

            /*
                        FeatureDefinitionPower HerbalBrewToxifying = FeatureDefinitionPowerBuilder
                            .Create("AncientForestHerbalBrewToxifying", DefinitionBuilder.CENamespaceGuid)
                            .SetGuiPresentation(Category.Power)
                            .Configure(
                                   1,
                                   RuleDefinitions.UsesDetermination.ProficiencyBonus,
                                   AttributeDefinitions.Charisma,
                                   RuleDefinitions.ActivationTime.Rest,
                                   1,
                                   RuleDefinitions.RechargeRate.LongRest,
                                   false,
                                   false,
                                   AttributeDefinitions.Charisma,
                                   new EffectDescriptionBuilder()
                                        .AddEffectForm(
                                            new EffectFormBuilder().SetSummonForm(
                                                SummonForm.Type.InventoryItem,
                                                ToxicBrew,
                                                1,
                                                Wolf.name,
                                                null,
                                                true,
                                                null,
                                                ScriptableObject.CreateInstance<EffectProxyDefinition>()
                                                )
                                            .SetBonusMode(AddBonusMode.DoubleProficiency)
                                            .Build())
                                        .SetDurationData(DurationType.UntilLongRest, 0)
                                        .SetTargetingData(
                                                RuleDefinitions.Side.Ally,
                                                RuleDefinitions.RangeType.Self,
                                                1,
                                                RuleDefinitions.TargetType.Self,
                                                1,
                                                1,
                                                ActionDefinitions.ItemSelectionType.Equiped)
                                        .Build()
                                   ,
                                   true)
                            .AddToDB();
            */


            RestActivityDefinitionBuilder
                .Create("AncientForestToxifyingRestActivity", DefinitionBuilder.CENamespaceGuid)
                .SetRestData(
                    RestDefinitions.RestStage.AfterRest,
                    RestType.LongRest,
                    RestActivityDefinition.ActivityCondition.CanUsePower,
                    "UsePower",
                    HerbalBrewToxifying.name)
                .SetGuiPresentation(HerbalBrewToxifying.GuiPresentation.Title, HerbalBrewToxifying.GuiPresentation.Description)
                .AddToDB();

            FeatureDefinitionFeatureSet HerbalBrewFeatureSet = FeatureDefinitionFeatureSetBuilder.Create(
                "HerbalBrewFeatureSet", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Feature)
                .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                .SetUniqueChoices(false)
                .AddFeatureSet(HerbalBrewPool, HerbalBrewToxifying, HerbalBrewQuickening)
                .AddFeatureSet(ResistentBrews)
                .AddFeatureSet(DatabaseHelper.FeatureDefinitionPointPools.ProficiencyMarksmanToolChoice)
                .AddToDB();


            ConditionDefinition Photosynthesis = ConditionDefinitionBuilder
                .Create("AncientForestPhotosynthesis", DefinitionBuilder.CENamespaceGuid)
                .SetSilent(Silent.None)
                .SetGuiPresentation(Category.Condition)
                .AddFeatures(DatabaseHelper.FeatureDefinitionRegenerations.RegenerationRing)
                .AddToDB();

            FeatureDefinitionLightAffinity AncientForestLightAffinity = FeatureDefinitionLightAffinityBuilder
                .Create("AncientForestLightAffinity", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation("AncientForestLightAffinity", Category.Feature)
                .AddLightingEffectAndCondition(new FeatureDefinitionLightAffinity.LightingEffectAndCondition
                {
                    lightingState = LocationDefinitions.LightingState.Bright,
                    condition = Photosynthesis
                })
                .AddToDB();

            FeatureDefinitionPower AtWillEntanglePower = FeatureDefinitionPowerBuilder
            .Create("AncientForestAtWillEntangle", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Entangle.GuiPresentation)
            .Configure(
                   1,
                   UsesDetermination.Fixed,
                   AttributeDefinitions.Charisma,
                   ActivationTime.Action,
                   1,
                   RechargeRate.AtWill,
                   false,
                   false,
                   AttributeDefinitions.Charisma,
                   Entangle.EffectDescription,
                   true)
            .AddToDB();

            ConditionDefinition RootedCondition = ConditionDefinitionBuilder
                .Create("AncientForestRootedCondition", DefinitionBuilder.CENamespaceGuid)
                .SetSilent(Silent.None)
                .SetGuiPresentation(Category.Condition)
                .AddFeatures(DatabaseHelper.FeatureDefinitionMovementAffinitys.MovementAffinityConditionRestrained)
                .AddFeatures(DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityProneImmunity)
                .AddFeatures(DatabaseHelper.FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityConditionRaging)
                .AddFeatures(AtWillEntanglePower)
                .AddToDB();

            FeatureDefinitionPower RootedPower = FeatureDefinitionPowerBuilder
                .Create("AncientForestRootedPower", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Power)
                .Configure(
                       1,
                       UsesDetermination.Fixed,
                       AttributeDefinitions.Charisma,
                       ActivationTime.Action,
                       1,
                       RechargeRate.LongRest,
                       false,
                       false,
                       AttributeDefinitions.Charisma,
                           new EffectDescriptionBuilder()
                                .AddEffectForm(
                                    new EffectFormBuilder().SetConditionForm(
                                        RootedCondition,
                                        ConditionForm.ConditionOperation.Add,
                                        true,
                                        true,
                                        new List<ConditionDefinition>()).Build())
                                .SetDurationData(DurationType.Minute, 1)
                                .SetTargetingData(
                                        Side.Ally,
                                        RangeType.Self,
                                        1,
                                        TargetType.Self,
                                        1,
                                        1,
                                        ActionDefinitions.ItemSelectionType.None)
                                .Build()
                       ,
                       true)
                .AddToDB();

            RootedPower.GuiPresentation.SetSpriteReference(PowerRangerHideInPlainSight.GuiPresentation.SpriteReference);

            FeatureDefinitionPower AncientForestWallofThornsPool = FeatureDefinitionPowerPoolBuilder
                .Create("DHAncientForestWallofThornsPool", GuidHelper.Create(new Guid(Settings.GUID), "DHAncientForestWallofThornsPool").ToString())
                .SetGuiPresentationNoContent()
                .SetUsesProficiency()
                .SetUsesAbility(1, AttributeDefinitions.Charisma)
                .SetRechargeRate(RechargeRate.LongRest)
                .AddToDB();

            FeatureDefinitionFeatureSet WallofThornsFeatureSet = FeatureDefinitionFeatureSetBuilder.Create(
                "WallofThornsFeatureSet", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Feature)
                .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                .SetUniqueChoices(false)
                .AddToDB();

            List<SpellDefinition> ThornSpells = new List<SpellDefinition>
            {
                WallOfThornsWallLine,
                WallOfThornsWallRing
            };


            foreach (SpellDefinition spell in ThornSpells)
            {
                //   FeatureDefinitionPower WallofThornsPower = FeatureDefinitionPowerBuilder
                //       .Create("AncientForest" + spell.name, DefinitionBuilder.CENamespaceGuid)
                //       .SetGuiPresentation(spell.GuiPresentation)
                //       .Configure(
                //           1,
                //           RuleDefinitions.UsesDetermination.Fixed,
                //           AttributeDefinitions.Charisma,
                //           spell.ActivationTime,
                //           1,
                //           RuleDefinitions.RechargeRate.LongRest,
                //           false,
                //           false,
                //           AttributeDefinitions.Charisma,
                //           spell.EffectDescription,
                //           true)
                //       .AddToDB();

                FeatureDefinitionPower WallofThornsPower = new FeatureDefinitionPowerSharedPoolBuilder(
                     "AncientForest" + spell.name,
                     GuidHelper.Create(new Guid(Settings.GUID), "AncientForest" + spell.name).ToString(),
                     AncientForestWallofThornsPool,
                     RechargeRate.LongRest,
                     ActivationTime.Rest,
                     1,
                     false,
                     false,
                     AttributeDefinitions.Charisma,
                     spell.EffectDescription,
                     spell.GuiPresentation,
                     false
                     ).AddToDB();


                WallofThornsFeatureSet.AddFeatureSet(WallofThornsPower);
            }
            // should Use features sets so character saves don't break


            FeatureDefinitionAttributeModifier AncientForestAttributeModiferRegrowth = FeatureDefinitionAttributeModifierBuilder
                  .Create(AttributeModifierPaladinHealingPoolBase, "AncientForestAttributeModiferRegrowth", DefinitionBuilder.CENamespaceGuid)
                  .SetGuiPresentationNoContent(true)
                  .AddToDB();

            FeatureDefinitionAttributeModifier AncientForestAttributeModiferRegrowthMultiplier = FeatureDefinitionAttributeModifierBuilder
                .Create(AttributeModifierPaladinHealingPoolMultiplier, "AncientForestAttributeModiferRegrowthMultiplier", DefinitionBuilder.CENamespaceGuid)
                .AddToDB();

            return CharacterSubclassDefinitionBuilder
                .Create("AncientForest", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation("WarlockAncientForest", Category.Subclass, TraditionGreenmage.GuiPresentation.SpriteReference)
                .AddFeatureAtLevel(ancientForestExpandedSpellListAffinity, 1)
                .AddFeatureAtLevel(AncientForestAttributeModiferRegrowth, 1)
                .AddFeatureAtLevel(AncientForestAttributeModiferRegrowthMultiplier, 1)
                .AddFeatureAtLevel(Regrowth, 1)
                .AddFeatureAtLevel(AncientForestBonusCantrip, 1)
                .AddFeatureAtLevel(HerbalBrewFeatureSet, 6)
                .AddFeatureAtLevel(LifeSap, 6)
                .AddFeatureAtLevel(AncientForestLightAffinity, 10)
                .AddFeatureAtLevel(RootedPower, 10)
                .AddFeatureAtLevel(AttributeModifierBarkskin, 14)
                .AddFeatureAtLevel(WallofThornsFeatureSet, 14)
                .AddToDB();
        }
    }
}
