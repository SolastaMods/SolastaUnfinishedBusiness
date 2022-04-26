using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.Models;
using SolastaModApi;
using SolastaModApi.Extensions;
using static RuleDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaModApi.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;
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
                .SetGuiPresentationNoContent()
                .ClearSpells()
                .SetSpellsAtLevel(1, Goodberry, Entangle)
                .SetSpellsAtLevel(2, Barkskin, SpikeGrowth)
                .SetSpellsAtLevel(3, MassHealingWord, VampiricTouch)
                .SetSpellsAtLevel(4, Blight, IdentifyCreatures)
                .SetSpellsAtLevel(5, Contagion, RaiseDead)
                .FinalizeSpells()
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

            var lifeSapDamageAndHeal = FeatureDefinitionPowerBuilder
                .Create("AncientForestLifeSapDamageAndHeal", DefinitionBuilder.CENamespaceGuid)
                .SetEffectDescription(new EffectDescriptionBuilder()
                    .AddEffectForm(
                        new EffectFormBuilder()
                            .SetDamageForm(
                                false,
                                DieType.D1,
                                DamageTypeNecrotic,
                                0,
                                DieType.D1,
                                0,
                                HealFromInflictedDamage.Half,
                                new List<TrendInfo>()
                            )
                            .SetBonusMode(AddBonusMode.DoubleProficiency)
                            .Build())
                    .Build()
                )
                .AddToDB();
            
            var lifeSapFeature = FeatureDefinitionOnMagicalAttackDamageEffectBuilder
                .Create("AncientForestLifeSap", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Power)
                .SetOnMagicalAttackDamageDelegates(null, (attacker, defender, _, effect, _, _, _) =>
                {
                    var caster = attacker.RulesetCharacter;
                    if (caster.MissingHitPoints >= caster.CurrentHitPoints &&
                        effect.EffectDescription.HasFormOfType(EffectForm.EffectFormType.Damage))
                    {
                        PowersContext.ApplyPowerEffectForms(
                            lifeSapDamageAndHeal,
                            caster,
                            defender.RulesetCharacter,
                            "AncientForestLifeSap"
                        );
                    }
                })
                .AddToDB();

            FeatureDefinitionPower Regrowth = FeatureDefinitionPowerBuilder
                .Create(PowerPaladinLayOnHands, "AncientForestRegrowth", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Feature, PowerFunctionGoodberryHealing.GuiPresentation.SpriteReference)
                .AddToDB();

            var AncientForestBonusCantrip = FeatureDefinitionFreeBonusCantripsBuilder
                .Create( "DHAncientForestBonusCantrip", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Feature)
                .ClearBonusCantrips()
                .AddBonusCantrip(Shillelagh)
                .AddBonusCantrip(ChillTouch)
                .AddToDB();

            FeatureDefinitionPower herbalBrewPool = FeatureDefinitionPowerPoolBuilder
                .Create("DH_HerbalBrewPool", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(new GuiPresentationBuilder(
                    "Feature/&HerbalBrewFeatureSetTitle",
                    "Feature/&HerbalBrewDescription",
                    PotionRemedy.GuiPresentation.SpriteReference //TODO: find better icon
                ).Build())
                .SetUsesProficiency()
                .SetRechargeRate(RechargeRate.LongRest)
                .SetActivation(ActivationTime.Rest, 1)
                .AddToDB();

            PowerBundleContext.RegisterPowerBundle(herbalBrewPool, true, 
                BuildHerbalBrew(herbalBrewPool, "Toxifying", Poison_Basic),
                BuildHerbalBrew(herbalBrewPool, "Healing", PotionOfHealing),
                BuildHerbalBrew(herbalBrewPool, DamageAffinityAcidResistance, PotionOfSpeed),
                BuildHerbalBrew(herbalBrewPool, DamageAffinityLightningResistance, Ingredient_RefinedOil),
                BuildHerbalBrew(herbalBrewPool, DamageAffinityNecroticResistance, PotionOfClimbing),
                BuildHerbalBrew(herbalBrewPool, DamageAffinityPoisonResistance, PotionOfHeroism),
                BuildHerbalBrew(herbalBrewPool, DamageAffinityRadiantResistance, PotionOfInvisibility)
            );

            RestActivityDefinitionBuilder
                .Create("AncientForestToxifyingRestActivity", DefinitionBuilder.CENamespaceGuid)
                .SetRestData(
                    RestDefinitions.RestStage.AfterRest,
                    RestType.LongRest,
                    RestActivityDefinition.ActivityCondition.CanUsePower,
                    PowerBundleContext.UseCustomRestPowerFunctorName,
                    herbalBrewPool.name)
                .SetGuiPresentation(herbalBrewPool.GuiPresentation.Title,
                    herbalBrewPool.GuiPresentation.Description)
                .AddToDB();

            FeatureDefinitionFeatureSet herbalBrewFeatureSet = FeatureDefinitionFeatureSetBuilder.Create(
                "HerbalBrewFeatureSet", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Feature)
                .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                .SetUniqueChoices(false)
                .AddFeatureSet(herbalBrewPool)
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
                .SetGuiPresentation(Category.Power, PowerRangerHideInPlainSight.GuiPresentation.SpriteReference)
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
                                        TargetType.Self)
                                .Build()
                       ,
                       true)
                .AddToDB();

            FeatureDefinitionPower AncientForestWallofThornsPool = FeatureDefinitionPowerPoolBuilder
                .Create("DHAncientForestWallofThornsPool", DefinitionBuilder.CENamespaceGuid)
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

            var thornSpells = new List<SpellDefinition>
            {
                WallOfThornsWallLine,
                WallOfThornsWallRing
            };

            foreach (SpellDefinition spell in thornSpells)
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
                .AddFeatureAtLevel(herbalBrewFeatureSet, 6)
                .AddFeatureAtLevel(lifeSapFeature, 6)
                .AddFeatureAtLevel(AncientForestLightAffinity, 10)
                .AddFeatureAtLevel(RootedPower, 10)
                .AddFeatureAtLevel(AttributeModifierBarkskin, 14)
                .AddFeatureAtLevel(WallofThornsFeatureSet, 14)
                .AddToDB();
        }
        private static FeatureDefinitionPower BuildHerbalBrew(FeatureDefinitionPower pool,
            string type,
            ItemDefinition baseItem)
        {
            var itemTitle = $"Equipment/&HerbalBrew{type}Title";
            var itemName = $"AncientForestHerbalBrew{type}Item";
            var powerName = $"AncientForestHerbalBrew{type}Power";

            GuiPresentation guiPresentation = new GuiPresentationBuilder(
                itemTitle,
                baseItem.GuiPresentation.Description,
                baseItem.GuiPresentation.SpriteReference
            ).Build();

            var brewItem = ItemDefinitionBuilder.Create(baseItem, itemName, DefinitionBuilder.CENamespaceGuid)
                .SetGold(0)
                .AddToDB();
            brewItem.SetGuiPresentation(guiPresentation);
            brewItem.SetIsFood(true);
            brewItem.SetFoodDescription(new FoodDescription().SetNutritiveCapacity(0).SetPerishable(true));
            brewItem.SetIsUsableDevice(true);
            brewItem.SetUsableDeviceDescription(baseItem.UsableDeviceDescription);

            EffectForm brewForm = new EffectFormBuilder()
                .SetSummonItemForm(brewItem, 1)
                .SetBonusMode(AddBonusMode.DoubleProficiency)
                .Build();

            EffectDescription brewEffect = new EffectDescriptionBuilder()
                .AddEffectForm(brewForm)
                .SetDurationData(DurationType.UntilLongRest)
                .SetTargetingData(
                    Side.Ally,
                    RangeType.Self,
                    1,
                    TargetType.Self,
                    1,
                    1,
                    ActionDefinitions.ItemSelectionType.Equiped)
                .Build();

            return new FeatureDefinitionPowerSharedPoolBuilder(
                powerName,
                pool,
                RechargeRate.ShortRest,
                ActivationTime.NoCost,
                1,
                false,
                false,
                AttributeDefinitions.Charisma,
                brewEffect,
                guiPresentation,
                false
            ).AddToDB();
        }    
        private static FeatureDefinitionPower BuildHerbalBrew(FeatureDefinitionPower pool,
            FeatureDefinitionDamageAffinity resType,
            ItemDefinition baseItem)
        {
            var resTypeName = resType.Name;

            var itemName = $"AncientForestHerbalBrew{resTypeName}Item";
            var powerName = $"AncientForestHerbalBrew{resTypeName}Power";

            GuiPresentation guiPresentation = new GuiPresentationBuilder(
                $"Equipment/&HerbalBrew{resTypeName}Title",
                $"Equipment/&HerbalBrew{resTypeName}Description",
                baseItem.GuiPresentation.SpriteReference
            ).Build();
            
            ConditionDefinition resistanceCondition = ConditionDefinitionBuilder.Create(
                    $"AncientForestHerbalBrew{resTypeName}Condition", DefinitionBuilder.CENamespaceGuid)
                .SetDuration(DurationType.Hour, 1)
                .SetSilent(Silent.None)
                .SetGuiPresentation(guiPresentation)
                .AddFeatures(resType)
                .AddToDB();
            
            FeatureDefinitionPower potionFunction = FeatureDefinitionPowerBuilder
                .Create($"AncientForestPotion{resTypeName}Function", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(new GuiPresentationBuilder(guiPresentation).SetTitle("Equipment/&FunctionPotionDrinkTitle").Build())
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
                                resistanceCondition,
                                ConditionForm.ConditionOperation.Add,
                                true,
                                true
                            ).Build())
                        .SetDurationData(DurationType.Hour, 1)
                        .SetTargetingData(
                            Side.Ally,
                            RangeType.Self,
                            1,
                            TargetType.Self)
                        .Build()
                    ,
                    true)
                .AddToDB();

            var brewItem = ItemDefinitionBuilder.Create(baseItem, itemName, DefinitionBuilder.CENamespaceGuid)
                .SetGold(0)
                .SetGuiPresentation(guiPresentation)
                .MakeMagical()
                .SetUsableDeviceDescription(potionFunction)
                .AddToDB();
            var description = brewItem.UsableDeviceDescription;
            brewItem.SetGuiPresentation(guiPresentation);
            brewItem.SetIsFood(true);
            brewItem.SetItemRarity(ItemRarity.Common);
            brewItem.SetFoodDescription(new FoodDescription().SetNutritiveCapacity(0).SetPerishable(true));
            brewItem.SetIsUsableDevice(true);
            brewItem.SetRequiresIdentification(false);
            brewItem.SetUsableDeviceDescription(description);

            EffectForm brewForm = new EffectFormBuilder()
                .SetSummonItemForm(brewItem, 1)
                .SetBonusMode(AddBonusMode.DoubleProficiency)
                .Build();


            EffectDescription brewEffect = new EffectDescriptionBuilder()
                .AddEffectForm(brewForm)
                .SetDurationData(DurationType.UntilLongRest)
                .SetTargetingData(
                    Side.Ally,
                    RangeType.Self,
                    1,
                    TargetType.Self,
                    1,
                    1,
                    ActionDefinitions.ItemSelectionType.Equiped)
                .Build();


            return new FeatureDefinitionPowerSharedPoolBuilder(
                powerName,
                pool,
                RechargeRate.ShortRest,
                ActivationTime.NoCost,
                1,
                false,
                false,
                AttributeDefinitions.Charisma,
                brewEffect,
                guiPresentation,
                false
            ).AddToDB();
        }
    }
}
