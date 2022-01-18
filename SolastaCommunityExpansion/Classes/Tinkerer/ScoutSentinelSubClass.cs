using SolastaCommunityExpansion.CustomFeatureDefinitions;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;



namespace SolastaArtificerMod
{
    public static class ScoutSentinelTinkererSubclassBuilder
    {
        public const string Name = "ScoutSentinel";
        public const string Guid = "fb2e5f73-d552-430f-b329-1f0a2ecdf6bd";

        public static FeatureDefinitionPowerSharedPool scoutmodepower;
        public static FeatureDefinitionPowerSharedPool sentinelmodepower;

        public static void BuildAndAddSubclass()
        {



            var subclassGuiPresentation = new GuiPresentationBuilder(
                "Subclass/&ScoutSentinelTinkererSubclassDescription",
                "Subclass/&ScoutSentinelTinkererSubclassTitle")
                .SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.MartialMountaineer.GuiPresentation.SpriteReference)
                .Build();

            var definition = new CharacterSubclassDefinitionBuilder(Name, Guid)
                .SetGuiPresentation(subclassGuiPresentation)

            // level 3
            .AddFeatureAtLevel(ScoutSentinelFeatureSet_level03Builder.ScoutSentinelFeatureSet_level03, 3)
            // level 5
            .AddFeatureAtLevel(ScoutSentinelFeatureSet_level05Builder.ScoutSentinelFeatureSet_level05, 5)
            // level 10
            .AddFeatureAtLevel(ScoutSentinelFeatureSet_level09Builder.ScoutSentinelFeatureSet_level09, 9)
            // level 14
            .AddFeatureAtLevel(ScoutSentinelFeatureSet_level15Builder.ScoutSentinelFeatureSet_level15, 15)
           .AddToDB(true);

        }
    }

    internal class ScoutSentinelFeatureSet_level03Builder : BaseDefinitionBuilder<FeatureDefinitionFeatureSet>
    {
        private const string ScoutSentinelFeatureSet_level03Name = "ScoutSentinelFeatureSet_level03";
        private const string ScoutSentinelFeatureSet_level03Guid = "a6560212-c665-49fd-94b7-378512e68edb";

        protected ScoutSentinelFeatureSet_level03Builder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetGreenmageWardenOfTheForest, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&ScoutSentinelFeatureSet_level03Title";
            Definition.GuiPresentation.Description = "Feat/&ScoutSentinelFeatureSet_level03Description";


            GuiPresentation guiPresentationArmorMode = new GuiPresentation();
            guiPresentationArmorMode.SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f));
            guiPresentationArmorMode.SetDescription("Feat/&ArmorModePoolDescription");
            guiPresentationArmorMode.SetTitle("Feat/&ArmorModePoolTitle");
            guiPresentationArmorMode.SetSpriteReference(null);
            guiPresentationArmorMode.SetSymbolChar("221E");

            FeatureDefinitionPower ArmorModePool = new FeatureDefinitionPowerPoolBuilder
                (
                    "ArmorModePool",
                    "fd0567d8-a728-4459-8569-273f3ead3f73",
                    1,
                    RuleDefinitions.UsesDetermination.Fixed,
                    AttributeDefinitions.Intelligence,
                    RuleDefinitions.RechargeRate.ShortRest,
                    guiPresentationArmorMode
                ).AddToDB();



            GuiPresentation guiPresentationSentinel = new GuiPresentation();
            guiPresentationSentinel.SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f));
            guiPresentationSentinel.SetDescription("Feature/&SentinelModePowerDescription");
            guiPresentationSentinel.SetTitle("Feature/&SentinelModePowerTitle");
            guiPresentationSentinel.SetSpriteReference(DatabaseHelper.SpellDefinitions.MageArmor.GuiPresentation.SpriteReference);
            guiPresentationSentinel.SetSymbolChar("221E");

            ItemPropertyForm itemPropertyForm = new ItemPropertyForm();
            itemPropertyForm.FeatureBySlotLevel.Add(new FeatureUnlockByLevel(IntToAttackAndDamageBuilder.IntToAttackAndDamage, 0));
            EffectForm effectItem = new EffectForm
            {
                FormType = EffectForm.EffectFormType.ItemProperty
            };
            effectItem.SetItemPropertyForm(itemPropertyForm);

            EffectDescription effectsentinelmode = new EffectDescription();
            effectsentinelmode.Copy(DatabaseHelper.SpellDefinitions.ProduceFlameHold.EffectDescription);
            effectsentinelmode.SlotTypes.Clear();
            effectsentinelmode.SlotTypes.AddRange("MainHandSlot", "OffHandSlot");
            effectsentinelmode.SetDurationType(RuleDefinitions.DurationType.UntilShortRest);
            effectsentinelmode.SetEffectParticleParameters(DatabaseHelper.SpellDefinitions.Shield.EffectDescription.EffectParticleParameters);
            effectsentinelmode.EffectForms[0].SummonForm.SetItemDefinition(SentinelSuitWeaponBuilder.SentinelSuitWeapon);
            effectsentinelmode.SetItemSelectionType(ActionDefinitions.ItemSelectionType.Weapon);
            effectsentinelmode.EffectForms[0].SummonForm.SetTrackItem(false);
            effectsentinelmode.EffectForms[0].SummonForm.SetNumber(1);
            effectsentinelmode.EffectForms.Add(effectItem);



            ScoutSentinelTinkererSubclassBuilder.sentinelmodepower = new FeatureDefinitionPowerSharedPoolBuilder
                (
                 "SentinelModePower"                                         // string name
                 , "410768a3-757f-48ee-8a2f-bffd963c0a5b"                    // string guid
                 , ArmorModePool                                             // FeatureDefinitionPower poolPower
                 , RuleDefinitions.RechargeRate.ShortRest                    // RuleDefinitions.RechargeRate recharge
                 , RuleDefinitions.ActivationTime.NoCost                     // RuleDefinitions.ActivationTime activationTime
                 , 1                                                         // int costPerUse
                 , false                                                     // bool proficiencyBonusToAttack
                 , false                                                     // bool abilityScoreBonusToAttack
                 , DatabaseHelper.SmartAttributeDefinitions.Intelligence.name// string abilityScore
                 , effectsentinelmode                                        // EffectDescription effectDescription
                 , guiPresentationSentinel                                   // GuiPresentation guiPresentation
                 , true                                                      // bool uniqueInstanc
                ).AddToDB();


            GuiPresentation guiPresentationScout = new GuiPresentation();
            guiPresentationScout.SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f));
            guiPresentationScout.SetDescription("Feature/&ScoutModePowerDescription");
            guiPresentationScout.SetTitle("Feature/&ScoutModePowerTitle");
            guiPresentationScout.SetSpriteReference(DatabaseHelper.SpellDefinitions.ShadowArmor.GuiPresentation.SpriteReference);
            guiPresentationScout.SetSymbolChar("221E");
            guiPresentationScout.SetSortOrder(1);

            EffectDescription effectScoutMode = new EffectDescription();
            effectScoutMode.Copy(DatabaseHelper.SpellDefinitions.ProduceFlameHold.EffectDescription);
            effectScoutMode.SlotTypes.Clear();
            effectScoutMode.SlotTypes.AddRange("MainHandSlot", "OffHandSlot");
            effectScoutMode.SetDurationType(RuleDefinitions.DurationType.UntilShortRest);
            effectScoutMode.SetEffectParticleParameters(DatabaseHelper.SpellDefinitions.Shield.EffectDescription.EffectParticleParameters);
            effectScoutMode.EffectForms[0].SummonForm.SetItemDefinition(ScoutSuitWeaponBuilder.ScoutSuitWeapon);
            effectScoutMode.SetItemSelectionType(ActionDefinitions.ItemSelectionType.Weapon);
            effectScoutMode.EffectForms[0].SummonForm.SetTrackItem(false);
            effectScoutMode.EffectForms[0].SummonForm.SetNumber(1);
            effectScoutMode.EffectForms.Add(effectItem);

            ScoutSentinelTinkererSubclassBuilder.scoutmodepower = new FeatureDefinitionPowerSharedPoolBuilder
                 (
                  "ScoutModePower"                                            // string name
                  , "ff6b9eb1-01ad-4100-ab12-7d6dc38ccc70"                    // string guid
                  , ArmorModePool                                             // FeatureDefinitionPower poolPower
                  , RuleDefinitions.RechargeRate.ShortRest                    // RuleDefinitions.RechargeRate recharge
                  , RuleDefinitions.ActivationTime.NoCost                     // RuleDefinitions.ActivationTime activationTime
                  , 1                                                         // int costPerUse
                  , false                                                     // bool proficiencyBonusToAttack
                  , false                                                     // bool abilityScoreBonusToAttack
                  , DatabaseHelper.SmartAttributeDefinitions.Intelligence.name// string abilityScore
                  , effectScoutMode                                           // EffectDescription effectDescription
                  , guiPresentationScout                                      // GuiPresentation guiPresentation
                  , true                                                      // bool uniqueInstanc
                 ).AddToDB();

            Definition.FeatureSet.Clear();
            Definition.FeatureSet.Add(ArmorModePool);
            Definition.FeatureSet.Add(ScoutSentinelTinkererSubclassBuilder.scoutmodepower);
            Definition.FeatureSet.Add(ScoutSentinelTinkererSubclassBuilder.sentinelmodepower);
            Definition.FeatureSet.Add(SubclassProficienciesBuilder.SubclassProficiencies);
            Definition.FeatureSet.Add(UseArmorWeaponsAsFocusBuilder.UseArmorWeaponsAsFocus);
            Definition.FeatureSet.Add(SubclassMovementAffinitiesBuilder.SubclassMovementAffinities);
            Definition.FeatureSet.Add(ScoutSentinelAutopreparedSpellsBuilder.SubclassAutopreparedSpells);
        }

        public static FeatureDefinitionFeatureSet CreateAndAddToDB(string name, string guid)
        {
            return new ScoutSentinelFeatureSet_level03Builder(name, guid).AddToDB();
        }

        public static FeatureDefinitionFeatureSet ScoutSentinelFeatureSet_level03 = CreateAndAddToDB(ScoutSentinelFeatureSet_level03Name, ScoutSentinelFeatureSet_level03Guid);
    }

    internal class ScoutSentinelFeatureSet_level05Builder : BaseDefinitionBuilder<FeatureDefinitionFeatureSet>
    {
        private const string ScoutSentinelFeatureSet_level05Name = "ScoutSentinelFeatureSet_level05";
        private const string ScoutSentinelFeatureSet_level05Guid = "a881c02a-7add-426b-a2d4-1f3994d12fa9";

        protected ScoutSentinelFeatureSet_level05Builder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetGreenmageWardenOfTheForest, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&ScoutSentinelFeatureSet_level05Title";
            Definition.GuiPresentation.Description = "Feat/&ScoutSentinelFeatureSet_level05Description";

            Definition.FeatureSet.Clear();
            Definition.FeatureSet.Add(DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierFighterExtraAttack);


        }

        public static FeatureDefinitionFeatureSet CreateAndAddToDB(string name, string guid)
        {
            return new ScoutSentinelFeatureSet_level05Builder(name, guid).AddToDB();
        }

        public static FeatureDefinitionFeatureSet ScoutSentinelFeatureSet_level05 = CreateAndAddToDB(ScoutSentinelFeatureSet_level05Name, ScoutSentinelFeatureSet_level05Guid);
    }

    internal class ScoutSentinelFeatureSet_level09Builder : BaseDefinitionBuilder<FeatureDefinitionFeatureSet>
    {
        private const string ScoutSentinelFeatureSet_level09Name = "ScoutSentinelFeatureSet_level09";
        private const string ScoutSentinelFeatureSet_level09Guid = "87e8b110-4590-4791-b31c-b8bba5f362b1";

        protected ScoutSentinelFeatureSet_level09Builder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetGreenmageWardenOfTheForest, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&ScoutSentinelFeatureSet_level09Title";
            Definition.GuiPresentation.Description = "Feat/&ScoutSentinelFeatureSet_level09Description";


            GuiPresentation guiPresentation = new GuiPresentation();
            guiPresentation.SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f));
            guiPresentation.SetDescription("Feat/&ExtraInfusionSlotsTitle");
            guiPresentation.SetTitle("Feat/&ExtraInfusionSlotsDescription");
            guiPresentation.SetSpriteReference(null);
            guiPresentation.SetSymbolChar("221E");
            guiPresentation.SetSortOrder(1);



            FeatureDefinitionPowerPoolModifier ExtraInfusionSlots = new FeatureDefinitionPowerPoolModifierBuilder(
                "ExtraInfusionSlots",
                "350902fd-cf99-48e4-8edc-115c82886bdb",
                2,
                RuleDefinitions.UsesDetermination.Fixed,
                AttributeDefinitions.Intelligence,
                TinkererClass.InfusionPool, guiPresentation
                ).AddToDB();



            Definition.FeatureSet.Clear();
            Definition.FeatureSet.Add(ExtraInfusionSlots);

        }

        public static FeatureDefinitionFeatureSet CreateAndAddToDB(string name, string guid)
        {
            return new ScoutSentinelFeatureSet_level09Builder(name, guid).AddToDB();
        }

        public static FeatureDefinitionFeatureSet ScoutSentinelFeatureSet_level09 = CreateAndAddToDB(ScoutSentinelFeatureSet_level09Name, ScoutSentinelFeatureSet_level09Guid);
    }

    internal class ScoutSentinelFeatureSet_level15Builder : BaseDefinitionBuilder<FeatureDefinitionFeatureSet>
    {
        private const string ScoutSentinelFeatureSet_level15Name = "ScoutSentinelFeatureSet_level15";
        private const string ScoutSentinelFeatureSet_level15Guid = "69a9ba53-4949-47ec-9693-467d053e4646";

        protected ScoutSentinelFeatureSet_level15Builder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetGreenmageWardenOfTheForest, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&ScoutSentinelFeatureSet_level15Title";
            Definition.GuiPresentation.Description = "Feat/&ScoutSentinelFeatureSet_level15Description";

            Definition.FeatureSet.Clear();
            Definition.FeatureSet.Add(ImprovedScoutModePowerBuilder.ImprovedScoutModePower);
            Definition.FeatureSet.Add(ImprovedSentinelModePowerBuilder.ImprovedSentinelModePower);
        }

        public static FeatureDefinitionFeatureSet CreateAndAddToDB(string name, string guid)
        {
            return new ScoutSentinelFeatureSet_level15Builder(name, guid).AddToDB();
        }

        public static FeatureDefinitionFeatureSet ScoutSentinelFeatureSet_level15 = CreateAndAddToDB(ScoutSentinelFeatureSet_level15Name, ScoutSentinelFeatureSet_level15Guid);
    }




    //*****************************************************************************************************************************************
    //***********************************		SubclassAutopreparedSpellsBuilder		*******************************************************
    //*****************************************************************************************************************************************




    public class ScoutSentinelAutopreparedSpellsBuilder : BaseDefinitionBuilder<FeatureDefinitionAutoPreparedSpells>
    {
        private const string SubclassAutopreparedSpellsName = "SubclassAutopreparedSpells";
        private const string SubclassAutopreparedSpellsGuid = "c5f03caa-7078-4da7-b166-00f292fcebfc";

        protected ScoutSentinelAutopreparedSpellsBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionAutoPreparedSpellss.AutoPreparedSpellsDomainBattle, name, guid)
        {


            Definition.GuiPresentation.Title = "Feat/&AutoPreparedSpellsTitle";
            Definition.GuiPresentation.Description = "Feat/&AutoPreparedSpellsDescription";


            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup autoPreparedSpellsGroup_Level_3 = new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup
            {
                ClassLevel = 3,
                SpellsList = (new List<SpellDefinition> {
                DatabaseHelper.SpellDefinitions.Thunderwave,
                DatabaseHelper.SpellDefinitions.MagicMissile
            })
            };

            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup autoPreparedSpellsGroup_Level_5 = new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup
            {
                ClassLevel = 5,
                SpellsList = (new List<SpellDefinition> {
                DatabaseHelper.SpellDefinitions.Shatter,
                DatabaseHelper.SpellDefinitions.Blur
            })
            };


            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup autoPreparedSpellsGroup_Level_9 = new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup
            {
                ClassLevel = 9,
                SpellsList = (new List<SpellDefinition> {
                DatabaseHelper.SpellDefinitions.LightningBolt
                ,DatabaseHelper.SpellDefinitions.HypnoticPattern
            })
            };

            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup autoPreparedSpellsGroup_Level_13 = new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup
            {
                ClassLevel = 13,
                SpellsList = (new List<SpellDefinition> {
                DatabaseHelper.SpellDefinitions.FireShield
                ,DatabaseHelper.SpellDefinitions.GreaterInvisibility
            })
            };


            //  added extra spells to balance spells withput "implemented"=true flag yet
            //blur for mirror image
            // dimension door for passwall
            // wall of fire (4th lvl) and wind wall (3th lvl) for wall of force (5th lvl)

            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup autoPreparedSpellsGroup_Level_17 = new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup
            {
                ClassLevel = 17,
                SpellsList = (new List<SpellDefinition> {
                DatabaseHelper.SpellDefinitions.DimensionDoor
            //    ,DatabaseHelper.SpellDefinitions.WallOfForce
                ,DatabaseHelper.SpellDefinitions.WallOfFire
                ,DatabaseHelper.SpellDefinitions.WindWall
            })
            };

            Definition.AutoPreparedSpellsGroups.Clear();
            Definition.AutoPreparedSpellsGroups.AddRange(new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>
            {
                autoPreparedSpellsGroup_Level_3,
                autoPreparedSpellsGroup_Level_5,
                autoPreparedSpellsGroup_Level_9,
                autoPreparedSpellsGroup_Level_13,
                autoPreparedSpellsGroup_Level_17
            });

        }




        public static FeatureDefinitionAutoPreparedSpells CreateAndAddToDB(string name, string guid)
        {
            return new ScoutSentinelAutopreparedSpellsBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionAutoPreparedSpells SubclassAutopreparedSpells = CreateAndAddToDB(SubclassAutopreparedSpellsName, SubclassAutopreparedSpellsGuid);
    }


    //*****************************************************************************************************************************************
    //***********************************		SubclassProficienciesBuilder		*******************************************************
    //*****************************************************************************************************************************************

    internal class SubclassProficienciesBuilder : BaseDefinitionBuilder<FeatureDefinitionProficiency>
    {
        private const string SubclassProficienciesName = "SubclassProficiencies";
        private const string SubclassProficienciesGuid = "1923caf6-672b-475a-bcdf-50d535ce65d1";

        protected SubclassProficienciesBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionProficiencys.ProficiencyDomainLifeArmor, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&SubclassProficienciesTitle"; //Feature/&NoContentTitle
            Definition.GuiPresentation.Description = "Feat/&SubclassProficienciesDescription";//Feature/&NoContentTitle
                                                                                              // Definition.Proficiencies.Add(DatabaseHelper.FeatureDefinitionProficiencys.ProficiencySmithTools.Name);
            Definition.Proficiencies.Add(DatabaseHelper.ArmorCategoryDefinitions.HeavyArmorCategory.Name);

        }

        public static FeatureDefinitionProficiency CreateAndAddToDB(string name, string guid)
        {
            return new SubclassProficienciesBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionProficiency SubclassProficiencies = CreateAndAddToDB(SubclassProficienciesName, SubclassProficienciesGuid);
    }

    //*****************************************************************************************************************************************
    //***********************************		SubclassMovementAffinitiesBuilder		*******************************************************
    //*****************************************************************************************************************************************

    internal class SubclassMovementAffinitiesBuilder : BaseDefinitionBuilder<FeatureDefinitionMovementAffinity>
    {
        private const string SubclassMovementAffinitiesName = "SubclassMovementAffinities";
        private const string SubclassMovementAffinitiesGuid = "14ef799a-edc6-4749-866b-9a6afc26d4fa";

        protected SubclassMovementAffinitiesBuilder(string name, string guid) : base(name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&SubclassMovementTitle";
            Definition.GuiPresentation.Description = "Feat/&SubclassMovementDescription";
            Definition.SetHeavyArmorImmunity(true);


        }

        public static FeatureDefinitionMovementAffinity CreateAndAddToDB(string name, string guid)
        {
            return new SubclassMovementAffinitiesBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionMovementAffinity SubclassMovementAffinities = CreateAndAddToDB(SubclassMovementAffinitiesName, SubclassMovementAffinitiesGuid);
    }




    //*****************************************************************************************************************************************
    //***********************************		UseArmorWeaponsAsFocusBuilder		*******************************************************
    //*****************************************************************************************************************************************

    internal class UseArmorWeaponsAsFocusBuilder : BaseDefinitionBuilder<FeatureDefinitionMagicAffinity>
    {
        private const string UseArmorWeaponsAsFocusName = "UseArmorWeaponsAsFocus";
        private const string UseArmorWeaponsAsFocusGuid = "55a4d71f-2b9d-4df6-abf1-c0cc6682eb9d";

        protected UseArmorWeaponsAsFocusBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionMagicAffinitys.MagicAffinitySpellBladeIntoTheFray, name, guid)
        {

            Definition.GuiPresentation.Title = "Feat/&UseArmorWeaponsAsFocusTitle";
            Definition.GuiPresentation.Description = "Feat/&UseArmorWeaponsAsFocusDescription";
            Definition.SetCanUseProficientWeaponAsFocus(true);
            Definition.SetSomaticWithWeapon(true);
            Definition.SetRangeSpellNoProximityPenalty(false);


        }

        public static FeatureDefinitionMagicAffinity CreateAndAddToDB(string name, string guid)
        {
            return new UseArmorWeaponsAsFocusBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionMagicAffinity UseArmorWeaponsAsFocus = CreateAndAddToDB(UseArmorWeaponsAsFocusName, UseArmorWeaponsAsFocusGuid);

    }



    //*****************************************************************************************************************************************
    //***********************************		IntToAttackAndDamageBuilder		*******************************************************
    //*****************************************************************************************************************************************

    internal class IntToAttackAndDamageBuilder : BaseDefinitionBuilder<FeatureDefinitionAttackModifier>
    {
        private const string IntToAttackAndDamageName = "IntToAttackAndDamage";
        private const string IntToAttackAndDamageGuid = "ebb243f7-382c-4caf-9d7f-40c80dab4623";

        protected IntToAttackAndDamageBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionAttackModifiers.AttackModifierShillelagh, name, guid)
        {

            Definition.GuiPresentation.Title = "Feat/&IntToAttackAndDamageTitle";
            Definition.GuiPresentation.Description = "Feat/&IntToAttackAndDamageDescription";

            Definition.SetDamageDieReplacement(RuleDefinitions.DamageDieReplacement.None);
            Definition.SetCanAddAbilityBonusToSecondary(true);
            Definition.SetAbilityScoreReplacement(RuleDefinitions.AbilityScoreReplacement.SpellcastingAbility);

            //    AssetReference assetReference = new AssetReference();
            //    assetReference.SetField("m_AssetGUID", "ad68a1be3193a314c911afd02ca8d360");
            //    Definition.SetImpactParticleReference(assetReference);



        }

        public static FeatureDefinitionAttackModifier CreateAndAddToDB(string name, string guid)
        {
            return new IntToAttackAndDamageBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionAttackModifier IntToAttackAndDamage = CreateAndAddToDB(IntToAttackAndDamageName, IntToAttackAndDamageGuid);
    }



    //*************************************************************************************************************************
    //***********************************		Sentinel Suit Weapon           		*******************************************
    //*************************************************************************************************************************

    internal class SentinelSuitWeaponBuilder : BaseDefinitionBuilder<ItemDefinition>
    {
        private const string SentinelSuitWeaponName = "SentinelSuitWeapon";
        private const string SentinelSuitWeaponGuid = "c86a1f25-3364-42bc-92b8-64f1358cbf15";

        protected SentinelSuitWeaponBuilder(string name, string guid) : base(DatabaseHelper.ItemDefinitions.UnarmedStrikeBase, name, guid)
        {


            // can only take 3 ( at game launch in may, havent checked since)
            Definition.IsFocusItem = true;
            Definition.IsUsableDevice = true;
            Definition.IsWeapon = true;
            //         Definition.IsFood = true;


            Definition.SetInDungeonEditor(true);


            EffectForm damageEffect = new EffectForm
            {
                DamageForm = new DamageForm
                {
                    DiceNumber = 1,
                    DieType = RuleDefinitions.DieType.D8,
                    BonusDamage = 0,
                    DamageType = "DamageThunder"
                },
                AddBonusMode = RuleDefinitions.AddBonusMode.AbilityBonus
            };

            EffectForm ThunderStruckEffect = new EffectForm
            {
                ConditionForm = new ConditionForm(),
                FormType = EffectForm.EffectFormType.Condition
            };
            ThunderStruckEffect.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
            ThunderStruckEffect.ConditionForm.ConditionDefinition = ThunderStruckConditionBuilder.ThunderStruck;

            EffectForm balancingeffect = new EffectForm
            {
                ConditionForm = new ConditionForm(),
                FormType = EffectForm.EffectFormType.Condition
            };
            balancingeffect.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
            balancingeffect.ConditionForm.ConditionDefinition = ThunderStruckBalancingAdvantageConditionBuilder.ThunderStruckBalancingAdvantage;





            //Add to our new effect
            EffectDescription newEffectDescription = new EffectDescription();
            newEffectDescription.Copy(Definition.WeaponDescription.EffectDescription);
            newEffectDescription.EffectForms.Clear();
            newEffectDescription.EffectForms.Add(damageEffect);
            newEffectDescription.EffectForms.Add(balancingeffect);
            newEffectDescription.EffectForms.Add(ThunderStruckEffect);

            newEffectDescription.HasSavingThrow = false;
            newEffectDescription.DurationType = RuleDefinitions.DurationType.Round;
            newEffectDescription.SetEndOfEffect(RuleDefinitions.TurnOccurenceType.EndOfTurn);
            newEffectDescription.SetTargetSide(RuleDefinitions.Side.Enemy);
            newEffectDescription.SetTargetType(RuleDefinitions.TargetType.Individuals);
            newEffectDescription.SetTargetProximityDistance(1);
            newEffectDescription.SetCanBePlacedOnCharacter(true);
            newEffectDescription.SetRangeType(RuleDefinitions.RangeType.MeleeHit);


            newEffectDescription.SetEffectParticleParameters(DatabaseHelper.SpellDefinitions.Shatter.EffectDescription.EffectParticleParameters);

            WeaponDescription ThunderPunch = new WeaponDescription();
            ThunderPunch.SetWeaponType("UnarmedStrikeType");
            ThunderPunch.SetReachRange(1);
            ThunderPunch.SetMaxRange(1);
            ThunderPunch.SetCloseRange(1);
            ThunderPunch.SetAmmunitionType("");
            ThunderPunch.SetEffectDescription(newEffectDescription);
            ThunderPunch.WeaponTags.Add("ScoutSentinelWeapon");



            ItemPropertyDescription UsingBonusActionItemPower = new ItemPropertyDescription(DatabaseHelper.ItemDefinitions.BeltOfDwarvenKind.StaticProperties[6]); ;
            UsingBonusActionItemPower.SetFeatureDefinition(UsingitemPowerBuilder.UsingitemPower);
            UsingBonusActionItemPower.SetType(ItemPropertyDescription.PropertyType.Feature);
            UsingBonusActionItemPower.SetKnowledgeAffinity(EquipmentDefinitions.KnowledgeAffinity.InactiveAndHidden);


            DeviceFunctionDescription deviceFunctionDescription = new DeviceFunctionDescription(DatabaseHelper.ItemDefinitions.PotionOfComprehendLanguages.UsableDeviceDescription.DeviceFunctions[0]);
            deviceFunctionDescription.SetCanOverchargeSpell(false);
            deviceFunctionDescription.SetDurationType(RuleDefinitions.DurationType.UntilLongRest);
            deviceFunctionDescription.SetFeatureDefinitionPower(ThunderShieldBuilder.ThunderShield);
            deviceFunctionDescription.SetParentUsage(EquipmentDefinitions.ItemUsage.ByFunction);
            deviceFunctionDescription.SetRechargeRate(RuleDefinitions.RechargeRate.LongRest);
            deviceFunctionDescription.SetType(DeviceFunctionDescription.FunctionType.Power);
            deviceFunctionDescription.SetUseAffinity(DeviceFunctionDescription.FunctionUseAffinity.IterationPerRecharge);
            deviceFunctionDescription.SetUseAmount(6);

            UsableDeviceDescription usableDeviceDescription = new UsableDeviceDescription();
            usableDeviceDescription.SetUsage(EquipmentDefinitions.ItemUsage.ByFunction);
            usableDeviceDescription.SetChargesCapitalNumber(5);
            usableDeviceDescription.SetRechargeRate(RuleDefinitions.RechargeRate.LongRest);
            usableDeviceDescription.SetRechargeNumber(0);
            usableDeviceDescription.SetRechargeDie(RuleDefinitions.DieType.D1);
            usableDeviceDescription.SetRechargeBonus(5);
            usableDeviceDescription.SetOutOfChargesConsequence(EquipmentDefinitions.ItemOutOfCharges.Persist);
            usableDeviceDescription.SetMagicAttackBonus(5);
            usableDeviceDescription.SetSaveDC(13);
            usableDeviceDescription.DeviceFunctions.Add(deviceFunctionDescription);


            Definition.SlotTypes.AddRange(new List<string> { "MainHandSlot", "OffHandSlot", "GlovesSlot", "UtilitySlot" });
            Definition.SlotsWhereActive.AddRange(new List<string> { "MainHandSlot", "OffHandSlot", "GlovesSlot", "UtilitySlot" });

            FocusItemDescription focusItemDescription = new FocusItemDescription();
            focusItemDescription.SetShownAsFocus(true);
            focusItemDescription.SetFocusType(EquipmentDefinitions.FocusType.Arcane);

            Definition.SetActiveOnGround(false);
            Definition.SetCanBeStacked(true);
            Definition.SetDefaultStackCount(2);
            Definition.SetFocusItemDescription(focusItemDescription);
            Definition.SetForceEquip(false);
            Definition.SetForceEquipSlot("");
            Definition.SetInDungeonEditor(true);
            Definition.SetItemRarity(RuleDefinitions.ItemRarity.VeryRare);
            Definition.SetMagical(true);             //
            Definition.SetMerchantCategory("Weapon");
            Definition.SetRequiresAttunement(false);

            Definition.SetRequiresIdentification(false);
            Definition.SetStackSize(2);
            Definition.SetUsableDeviceDescription(usableDeviceDescription);
            Definition.SetWeaponDescription(ThunderPunch);
            Definition.SetWeight(1);
            Definition.StaticProperties.Add(UsingBonusActionItemPower);

            Definition.GuiPresentation.Title = "Equipment/&ThunderPunchTitle";
            Definition.GuiPresentation.Description = "Equipment/&ThunderPunchDescription";
            Definition.GuiPresentation.SetSpriteReference(DatabaseHelper.ItemDefinitions.GauntletsOfOgrePower.GuiPresentation.SpriteReference);



        }

        public static ItemDefinition CreateAndAddToDB(string name, string guid)
        {
            return new SentinelSuitWeaponBuilder(name, guid).AddToDB();
        }

        public static ItemDefinition SentinelSuitWeapon = CreateAndAddToDB(SentinelSuitWeaponName, SentinelSuitWeaponGuid);
    }



    internal class ThunderShieldBuilder : BaseDefinitionBuilder<FeatureDefinitionPower>
    {
        private const string ThunderShieldName = "ThunderShield";
        private const string ThunderShieldGuid = "f5ca9b23-0326-4b26-86e7-33ebcc061faf";

        protected ThunderShieldBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionPowers.PowerDomainLifePreserveLife, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&ThunderShieldTitle";
            Definition.GuiPresentation.Description = "Feat/&ThunderShieldDescription";
            Definition.GuiPresentation.SetSpriteReference(DatabaseHelper.SpellDefinitions.Shield.GuiPresentation.SpriteReference);


            Definition.SetRechargeRate(RuleDefinitions.RechargeRate.LongRest);
            Definition.SetCostPerUse(1);
            Definition.SetFixedUsesPerRecharge(5);
            Definition.SetActivationTime(RuleDefinitions.ActivationTime.BonusAction);
            Definition.SetShortTitleOverride("Feat/&ThunderShieldTitle");

            EffectForm healingEffect = new EffectForm
            {
                FormType = EffectForm.EffectFormType.TemporaryHitPoints
            };

            var tempHPForm = new TemporaryHitPointsForm
            {
                DiceNumber = 1,
                DieType = RuleDefinitions.DieType.D1,
                BonusHitPoints = 0
            };

            healingEffect.SetTemporaryHitPointsForm(tempHPForm);
            healingEffect.SetApplyLevel(EffectForm.LevelApplianceType.Multiply);
            healingEffect.SetLevelType(RuleDefinitions.LevelSourceType.CharacterLevel);
            healingEffect.SetLevelMultiplier(1);


            //Add to our new effect
            EffectDescription newEffectDescription = new EffectDescription();
            newEffectDescription.Copy(Definition.EffectDescription);
            newEffectDescription.EffectForms.Clear();
            newEffectDescription.EffectForms.Add(healingEffect);
            newEffectDescription.HasSavingThrow = false;
            newEffectDescription.DurationType = RuleDefinitions.DurationType.Day;
            newEffectDescription.SetTargetSide(RuleDefinitions.Side.Ally);
            newEffectDescription.SetTargetType(RuleDefinitions.TargetType.Self);
            newEffectDescription.SetTargetProximityDistance(12);
            newEffectDescription.SetCanBePlacedOnCharacter(true);
            newEffectDescription.SetRangeType(RuleDefinitions.RangeType.Self);

            newEffectDescription.SetEffectParticleParameters(DatabaseHelper.SpellDefinitions.Shield.EffectDescription.EffectParticleParameters);

            Definition.SetEffectDescription(newEffectDescription);
        }

        public static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
        {
            return new ThunderShieldBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionPower ThunderShield = CreateAndAddToDB(ThunderShieldName, ThunderShieldGuid);
    }


    internal class ThunderStruckConditionBuilder : BaseDefinitionBuilder<ConditionDefinition>
    {
        private const string ThunderStruckName = "ThunderStruck";
        private const string ThunderStruckGuid = "63e2091c-4186-43d5-a099-7c8ca97224d6";

        protected ThunderStruckConditionBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionPoisoned, name, guid)
        {
            Definition.Features.Clear();
            Definition.GuiPresentation.Title = "Rules/&ThunderStruckTitle";
            Definition.GuiPresentation.Description = "Rules/&ThunderStruckDescription";

            Definition.SetAllowMultipleInstances(true);
            Definition.SetConditionType(RuleDefinitions.ConditionType.Neutral);

            Definition.Features.Add(ThunderStruckDisadvantageCombatAffintityBuilder.Disadvantage);
            Definition.SetSpecialDuration(true);
            Definition.SetDurationType(RuleDefinitions.DurationType.Round);
            Definition.SetDurationParameter(1);
            Definition.SetTurnOccurence(RuleDefinitions.TurnOccurenceType.EndOfTurn);

            // Fx
            AssetReference assetReference = new AssetReference();
            assetReference.SetField("m_AssetGUID", "3e25fca5d3585174f9b7e20aca6ef3d9");
            Definition.SetConditionStartParticleReference(assetReference);
            Definition.SetConditionParticleReference(assetReference);


        }

        public static ConditionDefinition CreateAndAddToDB(string name, string guid)
        {
            return new ThunderStruckConditionBuilder(name, guid).AddToDB();
        }

        public static ConditionDefinition ThunderStruck = CreateAndAddToDB(ThunderStruckName, ThunderStruckGuid);
    }

    internal class ThunderStruckDisadvantageCombatAffintityBuilder : BaseDefinitionBuilder<FeatureDefinitionCombatAffinity>
    {
        private const string ThunderStruckDisadvantageName = "ThunderStruckDisadvantage";
        private const string ThunderStruckDisadvantageGuid = "276daa3c-3ef2-4ea8-938f-f006d2721467";

        protected ThunderStruckDisadvantageCombatAffintityBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionCombatAffinitys.CombatAffinityPoisoned, name, guid)
        {

            Definition.SetMyAttackAdvantage(RuleDefinitions.AdvantageType.Disadvantage);

        }

        public static FeatureDefinitionCombatAffinity CreateAndAddToDB(string name, string guid)
        {
            return new ThunderStruckDisadvantageCombatAffintityBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionCombatAffinity Disadvantage = CreateAndAddToDB(ThunderStruckDisadvantageName, ThunderStruckDisadvantageGuid);
    }
    internal class ThunderStruckBalancingAdvantageConditionBuilder : BaseDefinitionBuilder<ConditionDefinition>
    {
        private const string ThunderStruckBalancingAdvantageName = "ThunderStruckBalancingAdvantage";
        private const string ThunderStruckBalancingAdvantageGuid = "0b2b4bee-21b0-46c7-9504-1374bd226cb0";

        protected ThunderStruckBalancingAdvantageConditionBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionDummy, name, guid)
        {
            Definition.Features.Clear();
            Definition.GuiPresentation.Title = "Rules/&ThunderStruckBalancingAdvantageTitle";
            Definition.GuiPresentation.Description = "Rules/&ThunderStruckBalancingAdvantageDescription";

            Definition.SetAllowMultipleInstances(true);
            Definition.SetConditionType(RuleDefinitions.ConditionType.Neutral);
            Definition.SetSpecialDuration(true);
            Definition.SetDurationType(RuleDefinitions.DurationType.Round);
            Definition.SetDurationParameter(1);
            Definition.SetTurnOccurence(RuleDefinitions.TurnOccurenceType.EndOfTurn);

            Definition.Features.Add(BalancingAdvantageCombatAffintityBuilder.BalancingAdvantage);

        }

        public static ConditionDefinition CreateAndAddToDB(string name, string guid)
        {
            return new ThunderStruckBalancingAdvantageConditionBuilder(name, guid).AddToDB();
        }

        public static ConditionDefinition ThunderStruckBalancingAdvantage = CreateAndAddToDB(ThunderStruckBalancingAdvantageName, ThunderStruckBalancingAdvantageGuid);
    }

    internal class BalancingAdvantageCombatAffintityBuilder : BaseDefinitionBuilder<FeatureDefinitionCombatAffinity>
    {
        private const string BalancingAdvantageName = "BalancingAdvantage";
        private const string BalancingAdvantageGuid = "9dc28618-3adb-497d-930d-2a01bcac42d5";

        protected BalancingAdvantageCombatAffintityBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionCombatAffinitys.CombatAffinityCursedByBestowCurseOnAttackRoll, name, guid)
        {

            Definition.SetMyAttackAdvantage(RuleDefinitions.AdvantageType.Advantage);
            Definition.SetSituationalContext(RuleDefinitions.SituationalContext.TargetIsEffectSource);
        }

        public static FeatureDefinitionCombatAffinity CreateAndAddToDB(string name, string guid)
        {
            return new BalancingAdvantageCombatAffintityBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionCombatAffinity BalancingAdvantage = CreateAndAddToDB(BalancingAdvantageName, BalancingAdvantageGuid);
    }
    internal class UsingitemPowerBuilder : BaseDefinitionBuilder<FeatureDefinitionActionAffinity>
    {
        private const string UsingitemPowerName = "UsingitemPower";
        private const string UsingitemPowerGuid = "39f8cb05-5475-456a-a9b2-022b6e07850b";

        protected UsingitemPowerBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionActionAffinitys.ActionAffinityThiefFastHands, name, guid)
        {

            Definition.AuthorizedActions.Clear();
            Definition.AuthorizedActions.Add(ActionDefinitions.Id.UseItemBonus);

            Definition.GuiPresentation.Title = "Feat/&UsingitemPowerTitle";
            Definition.GuiPresentation.Description = "Feat/&UsingitemPowerDescription";

        }

        public static FeatureDefinitionActionAffinity CreateAndAddToDB(string name, string guid)
        {
            return new UsingitemPowerBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionActionAffinity UsingitemPower = CreateAndAddToDB(UsingitemPowerName, UsingitemPowerGuid);
    }






    //**************************************************************************************************************************************
    //************************************************      Scout Suit Weapon        *******************************************************
    //**************************************************************************************************************************************


    internal class ScoutSuitWeaponBuilder : BaseDefinitionBuilder<ItemDefinition>
    {
        private const string ScoutSuitWeaponName = "ScoutSuitWeapon";
        private const string ScoutSuitWeaponGuid = "21f90fad-b039-4efe-bee8-5afd44453664";

        protected ScoutSuitWeaponBuilder(string name, string guid) : base(DatabaseHelper.ItemDefinitions.Dart, name, guid)
        {

            // can only take 3    (at game launch in may, havent checked since)
            Definition.IsFocusItem = true;
            Definition.IsWeapon = true;
            //     Definition.IsFood = true;
            //   Definition.IsUsableDevice = true;

            Definition.SetInDungeonEditor(true);

            EffectForm damageEffect = new EffectForm
            {
                DamageForm = new DamageForm
                {
                    DiceNumber = 1,
                    DieType = RuleDefinitions.DieType.D6,
                    BonusDamage = 0,
                    DamageType = "DamageLightning"
                },
                AddBonusMode = RuleDefinitions.AddBonusMode.AbilityBonus
            };

            EffectDescription newEffectDescription = new EffectDescription();
            newEffectDescription.Copy(Definition.WeaponDescription.EffectDescription);
            newEffectDescription.EffectForms.Clear();
            newEffectDescription.EffectForms.Add(damageEffect);

            newEffectDescription.HasSavingThrow = false;
            newEffectDescription.DurationType = RuleDefinitions.DurationType.Round;
            newEffectDescription.SetEndOfEffect(RuleDefinitions.TurnOccurenceType.EndOfTurn);
            newEffectDescription.SetTargetSide(RuleDefinitions.Side.Enemy);
            newEffectDescription.SetTargetType(RuleDefinitions.TargetType.Individuals);
            newEffectDescription.SetTargetProximityDistance(1);
            newEffectDescription.SetCanBePlacedOnCharacter(true);
            newEffectDescription.SetRangeType(RuleDefinitions.RangeType.RangeHit);
            newEffectDescription.SetRangeParameter(12);

            newEffectDescription.SetEffectParticleParameters(DatabaseHelper.SpellDefinitions.LightningBolt.EffectDescription.EffectParticleParameters);

            WeaponDescription LightningSpear = new WeaponDescription();
            LightningSpear.SetWeaponType("DartType");
            LightningSpear.SetReachRange(1);
            LightningSpear.SetMaxRange(60);
            LightningSpear.SetCloseRange(18);
            LightningSpear.SetAmmunitionType("");
            LightningSpear.SetEffectDescription(newEffectDescription);
            LightningSpear.WeaponTags.Add("ScoutSentinelWeapon");

            ItemPropertyDescription LightningSpearAdditionalDamage = new ItemPropertyDescription(DatabaseHelper.ItemDefinitions.BeltOfDwarvenKind.StaticProperties[6]);
            LightningSpearAdditionalDamage.SetFeatureDefinition(LightningSpearAdditionalDamageBuilder.LightningSpearAdditionalDamage);
            LightningSpearAdditionalDamage.SetType(ItemPropertyDescription.PropertyType.Feature);
            LightningSpearAdditionalDamage.SetKnowledgeAffinity(EquipmentDefinitions.KnowledgeAffinity.InactiveAndHidden);

            ItemPropertyDescription LightningCloakStealth = new ItemPropertyDescription(DatabaseHelper.ItemDefinitions.BeltOfDwarvenKind.StaticProperties[6]);
            LightningCloakStealth.SetFeatureDefinition(LightningCloakAbilityCheckAffinityBuilder.LightningCloakAbilityCheckAffinity);
            LightningCloakStealth.SetType(ItemPropertyDescription.PropertyType.Feature);
            LightningCloakStealth.SetKnowledgeAffinity(EquipmentDefinitions.KnowledgeAffinity.InactiveAndHidden);

            ItemPropertyDescription LightningCloakMovement = new ItemPropertyDescription(DatabaseHelper.ItemDefinitions.BeltOfDwarvenKind.StaticProperties[6]);
            LightningCloakMovement.SetFeatureDefinition(LightningCloakMovementAffinitiesBuilder.LightningCloakMovementAffinities);
            LightningCloakMovement.SetType(ItemPropertyDescription.PropertyType.Feature);
            LightningCloakMovement.SetKnowledgeAffinity(EquipmentDefinitions.KnowledgeAffinity.InactiveAndHidden);


            Definition.SlotTypes.AddRange(new List<string> { "MainHandSlot", "OffHandSlot", "GlovesSlot", "UtilitySlot" });
            Definition.SlotsWhereActive.AddRange(new List<string> { "MainHandSlot", "OffHandSlot", "GlovesSlot", "UtilitySlot" });

            FocusItemDescription focusItemDescription = new FocusItemDescription();
            focusItemDescription.SetShownAsFocus(true);
            focusItemDescription.SetFocusType(EquipmentDefinitions.FocusType.Arcane);

            Definition.SetActiveOnGround(false);
            Definition.SetCanBeStacked(true);
            Definition.SetDefaultStackCount(-1);
            Definition.SetFocusItemDescription(focusItemDescription);
            Definition.SetForceEquip(false);
            Definition.SetForceEquipSlot("MainHandSlot");
            Definition.SetInDungeonEditor(true);
            Definition.SetItemRarity(RuleDefinitions.ItemRarity.VeryRare);
            Definition.SetMagical(true);
            Definition.SetMerchantCategory("Weapon");
            Definition.SetRequiresAttunement(false);
            Definition.SetRequiresIdentification(false);
            Definition.SetStackSize(2);
            Definition.SetWeaponDescription(LightningSpear);
            Definition.SetWeight(1);
            Definition.StaticProperties.Add(LightningSpearAdditionalDamage);
            Definition.StaticProperties.Add(LightningCloakStealth);
            Definition.StaticProperties.Add(LightningCloakMovement);

            Definition.GuiPresentation.Title = "Equipment/&LightningSpearTitle";
            Definition.GuiPresentation.Description = "Equipment/&LightningSpearDescription";
            Definition.GuiPresentation.SetSpriteReference(DatabaseHelper.ItemDefinitions.GlovesOfMissileSnaring.GuiPresentation.SpriteReference);

            Definition.SetItemPresentation(DatabaseHelper.ItemDefinitions.UnarmedStrikeBase.ItemPresentation);



        }

        public static ItemDefinition CreateAndAddToDB(string name, string guid)
        {
            return new ScoutSuitWeaponBuilder(name, guid).AddToDB();
        }

        public static ItemDefinition ScoutSuitWeapon = CreateAndAddToDB(ScoutSuitWeaponName, ScoutSuitWeaponGuid);
    }


    internal class LightningSpearAdditionalDamageBuilder : BaseDefinitionBuilder<FeatureDefinitionAdditionalDamage>
    {
        private const string LightningSpearAdditionalDamageName = "LightningSpearAdditionalDamage";
        private const string LightningSpearAdditionalDamageGuid = "52d19882-ca63-422d-aece-d80f806859a8";

        protected LightningSpearAdditionalDamageBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageBracersOfArchery, name, guid)
        {
            Definition.GuiPresentation.Title = "Feedback/&LightningSpearAdditionalDamageTitle";
            Definition.GuiPresentation.Description = "Feedback/&LightningSpearAdditionalDamageDescription";
            Definition.SetNotificationTag("LightningSpear");
            Definition.SetTriggerCondition(RuleDefinitions.AdditionalDamageTriggerCondition.AlwaysActive);
            Definition.SetLimitedUsage(RuleDefinitions.FeatureLimitedUsage.OncePerTurn);
            Definition.SetRequiredProperty(RuleDefinitions.AdditionalDamageRequiredProperty.RangeWeapon);
            Definition.SetDamageDieType(RuleDefinitions.DieType.D6);
            Definition.SetDamageDiceNumber(1);
            Definition.SetSpecificDamageType(RuleDefinitions.DamageTypeLightning);
        }

        public static FeatureDefinitionAdditionalDamage CreateAndAddToDB(string name, string guid)
        {
            return new LightningSpearAdditionalDamageBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionAdditionalDamage LightningSpearAdditionalDamage = CreateAndAddToDB(LightningSpearAdditionalDamageName, LightningSpearAdditionalDamageGuid);
    }
    internal class LightningCloakMovementAffinitiesBuilder : BaseDefinitionBuilder<FeatureDefinitionMovementAffinity>
    {
        private const string LightningCloakMovementAffinitiesName = "LightningCloakMovementAffinities";
        private const string LightningCloakMovementAffinitiesGuid = "9a098f7c-0286-4a04-ba42-23b7dfc45e7b";

        protected LightningCloakMovementAffinitiesBuilder(string name, string guid) : base(name, guid)
        {
            Definition.SetBaseSpeedAdditiveModifier(1);
        }

        public static FeatureDefinitionMovementAffinity CreateAndAddToDB(string name, string guid)
        {
            return new LightningCloakMovementAffinitiesBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionMovementAffinity LightningCloakMovementAffinities = CreateAndAddToDB(LightningCloakMovementAffinitiesName, LightningCloakMovementAffinitiesGuid);
    }
    internal class LightningCloakAbilityCheckAffinityBuilder : BaseDefinitionBuilder<FeatureDefinitionAbilityCheckAffinity>
    {
        private const string LightningCloakAbilityCheckAffinityName = "LightningCloakAbilityCheckAffinity";
        private const string LightningCloakAbilityCheckAffinityGuid = "160e74ef-1596-4b68-8a5b-48b65b155b26";

        protected LightningCloakAbilityCheckAffinityBuilder(string name, string guid) : base(name, guid)
        {
            FeatureDefinitionAbilityCheckAffinity.AbilityCheckAffinityGroup DampeningField = new FeatureDefinitionAbilityCheckAffinity.AbilityCheckAffinityGroup
            {
                abilityScoreName = "Dexterity",
                proficiencyName = "Stealth",
                affinity = RuleDefinitions.CharacterAbilityCheckAffinity.Advantage
            };
            Definition.AffinityGroups.Add(DampeningField);
        }

        public static FeatureDefinitionAbilityCheckAffinity CreateAndAddToDB(string name, string guid)
        {
            return new LightningCloakAbilityCheckAffinityBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionAbilityCheckAffinity LightningCloakAbilityCheckAffinity = CreateAndAddToDB(LightningCloakAbilityCheckAffinityName, LightningCloakAbilityCheckAffinityGuid);
    }

    //*************************************************************************************************************************
    //***********************************		Improved Sentinel Suit Weapon   		****************************************
    //*************************************************************************************************************************

    internal class ImprovedSentinelModePowerBuilder : BaseDefinitionBuilder<FeatureDefinitionPowerSharedPool>
    {
        private const string ImprovedSentinelModePowerName = "ImprovedSentinelModePower";
        private const string ImprovedSentinelModePowerGuid = "b216988a-5905-47fd-9359-093cd77d41f9";

        protected ImprovedSentinelModePowerBuilder(string name, string guid) : base(ScoutSentinelTinkererSubclassBuilder.sentinelmodepower, name, guid)
        {

            //Definition.EffectDescription.EffectParticleParameters.Clear();
            Definition.GuiPresentation.Title = "Feature/&ImprovedSentinelModePowerTitle";
            Definition.GuiPresentation.Description = "Feature/&ImprovedSentinelModePowerDescription";
            Definition.EffectDescription.EffectForms[0].SummonForm.SetItemDefinition(ImprovedSentinelSuitWeaponBuilder.ImprovedSentinelSuitWeapon);
            Definition.SetOverriddenPower(ScoutSentinelTinkererSubclassBuilder.sentinelmodepower);



        }

        public static FeatureDefinitionPowerSharedPool CreateAndAddToDB(string name, string guid)
        {
            return new ImprovedSentinelModePowerBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionPowerSharedPool ImprovedSentinelModePower = CreateAndAddToDB(ImprovedSentinelModePowerName, ImprovedSentinelModePowerGuid);

    }
    internal class ImprovedSentinelSuitWeaponBuilder : BaseDefinitionBuilder<ItemDefinition>
    {
        private const string ImprovedSentinelSuitWeaponName = "ImprovedSentinelSuitWeapon";
        private const string ImprovedSentinelSuitWeaponGuid = "f65108aa-1f69-4b08-b170-5fffd8444606";

        protected ImprovedSentinelSuitWeaponBuilder(string name, string guid) : base(SentinelSuitWeaponBuilder.SentinelSuitWeapon, name, guid)
        {

            Definition.GuiPresentation.Title = "Equipment/&ImprovedThunderPunchTitle";
            Definition.GuiPresentation.Description = "Equipment/&ImprovedThunderPunchDescription";
            Definition.GuiPresentation.SetSpriteReference(DatabaseHelper.ItemDefinitions.GauntletsOfOgrePower.GuiPresentation.SpriteReference);

            DeviceFunctionDescription grapplefunction = new DeviceFunctionDescription(DatabaseHelper.ItemDefinitions.PotionOfComprehendLanguages.UsableDeviceDescription.DeviceFunctions[0]);
            grapplefunction.SetCanOverchargeSpell(false);
            grapplefunction.SetDurationType(RuleDefinitions.DurationType.UntilLongRest);
            grapplefunction.SetFeatureDefinitionPower(GauntletsGrappleBuilder.GauntletsGrapple);
            grapplefunction.SetParentUsage(EquipmentDefinitions.ItemUsage.ByFunction);
            grapplefunction.SetRechargeRate(RuleDefinitions.RechargeRate.LongRest);
            grapplefunction.SetType(DeviceFunctionDescription.FunctionType.Power);
            grapplefunction.SetUseAffinity(DeviceFunctionDescription.FunctionUseAffinity.IterationPerRecharge);
            grapplefunction.SetUseAmount(6);

            Definition.UsableDeviceDescription.DeviceFunctions.Add(grapplefunction);
            Definition.UsableDeviceDescription.SetChargesCapitalNumber(10);


        }

        public static ItemDefinition CreateAndAddToDB(string name, string guid)
        {
            return new ImprovedSentinelSuitWeaponBuilder(name, guid).AddToDB();
        }

        public static ItemDefinition ImprovedSentinelSuitWeapon = CreateAndAddToDB(ImprovedSentinelSuitWeaponName, ImprovedSentinelSuitWeaponGuid);
    }


    internal class GauntletsGrappleBuilder : BaseDefinitionBuilder<FeatureDefinitionPower>
    {
        private const string GauntletsGrappleName = "GauntletsGrapple";
        private const string GauntletsGrappleGuid = "71b309c2-1f8b-4df4-955e-3f8504bc381e";

        protected GauntletsGrappleBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionPowers.PowerDomainLawHolyRetribution, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&GauntletsGrappleTitle";
            Definition.GuiPresentation.Description = "Feat/&GauntletsGrappleDescription";
            Definition.GuiPresentation.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerShadowTamerRopeGrapple.GuiPresentation.SpriteReference);


            Definition.SetRechargeRate(RuleDefinitions.RechargeRate.LongRest);
            Definition.SetCostPerUse(1);
            Definition.SetFixedUsesPerRecharge(6);
            Definition.SetActivationTime(RuleDefinitions.ActivationTime.BonusAction);
            Definition.SetShortTitleOverride("Feat/&GauntletsGrappleTitle");
            Definition.SetReactionContext(RuleDefinitions.ReactionTriggerContext.HitByMelee);


            EffectForm motionEffect = new EffectForm
            {
                FormType = EffectForm.EffectFormType.Motion
            };

            var motion = new MotionForm();
            motion.SetDistance(6);
            motion.SetType(MotionForm.MotionType.DragToOrigin);

            motionEffect.SetMotionForm(motion);
            motionEffect.SetApplyLevel(EffectForm.LevelApplianceType.Multiply);
            motionEffect.SetLevelType(RuleDefinitions.LevelSourceType.CharacterLevel);
            motionEffect.SetLevelMultiplier(1);
            motionEffect.SavingThrowAffinity = RuleDefinitions.EffectSavingThrowType.Negates;

            EffectForm damageEffect = new EffectForm
            {
                DamageForm = new DamageForm
                {
                    DiceNumber = 1,
                    DieType = RuleDefinitions.DieType.D8,
                    BonusDamage = 0,
                    DamageType = "DamageThunder"
                },
                AddBonusMode = RuleDefinitions.AddBonusMode.AbilityBonus
            };

            EffectDescription newEffectDescription = new EffectDescription();
            newEffectDescription.Copy(Definition.EffectDescription);
            newEffectDescription.EffectForms.Clear();
            newEffectDescription.EffectForms.Add(motionEffect);
            newEffectDescription.EffectForms.Add(damageEffect);
            newEffectDescription.HasSavingThrow = true;
            newEffectDescription.SavingThrowAbility = DatabaseHelper.SmartAttributeDefinitions.Strength.Name;
            newEffectDescription.SetSavingThrowDifficultyAbility(DatabaseHelper.SmartAttributeDefinitions.Intelligence.Name);
            newEffectDescription.SetDifficultyClassComputation(RuleDefinitions.EffectDifficultyClassComputation.FixedValue);
            newEffectDescription.FixedSavingThrowDifficultyClass = 19;
            newEffectDescription.SetCreatedByCharacter(true);

            newEffectDescription.DurationType = RuleDefinitions.DurationType.UntilLongRest;
            newEffectDescription.SetTargetSide(RuleDefinitions.Side.Enemy);
            newEffectDescription.SetTargetType(RuleDefinitions.TargetType.IndividualsUnique);
            newEffectDescription.SetTargetProximityDistance(12);
            newEffectDescription.SetCanBePlacedOnCharacter(true);
            newEffectDescription.SetRangeType(RuleDefinitions.RangeType.Distance);
            newEffectDescription.SetRangeParameter(6);

            newEffectDescription.SetEffectParticleParameters(DatabaseHelper.FeatureDefinitionPowers.PowerShadowTamerRopeGrapple.EffectDescription.EffectParticleParameters);

            Definition.SetEffectDescription(newEffectDescription);
        }

        public static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
        {
            return new GauntletsGrappleBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionPower GauntletsGrapple = CreateAndAddToDB(GauntletsGrappleName, GauntletsGrappleGuid);
    }

    //**************************************************************************************************************************************
    //******************************************       Improved Scout Suit Weapon        ***************************************************
    //**************************************************************************************************************************************


    internal class ImprovedScoutModePowerBuilder : BaseDefinitionBuilder<FeatureDefinitionPowerSharedPool>
    {
        private const string ImprovedScoutModePowerName = "ImprovedScoutModePower";
        private const string ImprovedScoutModePowerGuid = "89ec2647-5560-4e82-aa7b-59a8489de492";

        protected ImprovedScoutModePowerBuilder(string name, string guid) : base(ScoutSentinelTinkererSubclassBuilder.scoutmodepower, name, guid)
        {
            Definition.EffectDescription.EffectParticleParameters.Clear();
            Definition.GuiPresentation.Title = "Feature/&ImprovedScoutModePowerTitle";
            Definition.GuiPresentation.Description = "Feature/&ImprovedScoutModePowerDescription";
            Definition.GuiPresentation.SetSpriteReference(DatabaseHelper.SpellDefinitions.ShadowArmor.GuiPresentation.SpriteReference);


            Definition.SetOverriddenPower(ScoutSentinelTinkererSubclassBuilder.scoutmodepower);
            Definition.EffectDescription.EffectForms[0].SummonForm.SetItemDefinition(ImprovedScoutSuitWeaponBuilder.ImprovedScoutSuitWeapon);


        }

        public static FeatureDefinitionPowerSharedPool CreateAndAddToDB(string name, string guid)
        {
            return new ImprovedScoutModePowerBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionPowerSharedPool ImprovedScoutModePower = CreateAndAddToDB(ImprovedScoutModePowerName, ImprovedScoutModePowerGuid);


    }


    internal class ImprovedScoutSuitWeaponBuilder : BaseDefinitionBuilder<ItemDefinition>
    {
        private const string ImprovedScoutSuitWeaponName = "ImprovedScoutSuitWeapon";
        private const string ImprovedScoutSuitWeaponGuid = "c2ebf9a1-5c45-4f70-ba6c-c791cd607ea7";

        protected ImprovedScoutSuitWeaponBuilder(string name, string guid) : base(ScoutSuitWeaponBuilder.ScoutSuitWeapon, name, guid)
        {



            Definition.GuiPresentation.Title = "Equipment/&ImprovedLightningSpearTitle";
            Definition.GuiPresentation.Description = "Equipment/&ImprovedLightningSpearDescription";
            Definition.GuiPresentation.SetSpriteReference(DatabaseHelper.ItemDefinitions.GlovesOfMissileSnaring.GuiPresentation.SpriteReference);


            Definition.SetInDungeonEditor(true);



            //next attack advantage // condition true strike or guiding bolt
            EffectForm NextAttackAdvantage = new EffectForm
            {
                ConditionForm = new ConditionForm(),
                FormType = EffectForm.EffectFormType.Condition
            };
            NextAttackAdvantage.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
            NextAttackAdvantage.ConditionForm.ConditionDefinition = AdvantageAttackOnEnemyBuilder.AdvantageAttackOnEnemy;


            // combat affinity cursed on attack roll
            EffectForm EnemyAttackDisadvantageEffect = new EffectForm
            {
                ConditionForm = new ConditionForm(),
                FormType = EffectForm.EffectFormType.Condition
            };
            EnemyAttackDisadvantageEffect.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
            EnemyAttackDisadvantageEffect.ConditionForm.ConditionDefinition = DisadvantageOnAttackByEnemyBuilder.DisadvantageOnAttackByEnemy;


            //extra damage on attack
            EffectForm ExtraAttackEffect = new EffectForm
            {
                ConditionForm = new ConditionForm(),
                FormType = EffectForm.EffectFormType.Condition
            };
            ExtraAttackEffect.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
            ExtraAttackEffect.ConditionForm.ConditionDefinition = ExtraDamageOnAttackConditionBuilder.ExtraDamageOnAttackCondition;

            // target illuminated
            LightSourceForm lightSourceForm = new LightSourceForm();
            lightSourceForm.Copy(DatabaseHelper.SpellDefinitions.Shine.EffectDescription.EffectForms[0].LightSourceForm);


            EffectForm MagicalLightSourceEffect = new EffectForm();
            MagicalLightSourceEffect.SetLevelMultiplier(1);
            MagicalLightSourceEffect.SetLevelType(RuleDefinitions.LevelSourceType.CharacterLevel);
            MagicalLightSourceEffect.HasSavingThrow = false;
            MagicalLightSourceEffect.SetCreatedByCharacter(true);
            MagicalLightSourceEffect.FormType = EffectForm.EffectFormType.LightSource;
            MagicalLightSourceEffect.SetLightSourceForm(lightSourceForm);


            Definition.WeaponDescription.EffectDescription.EffectForms.Add(EnemyAttackDisadvantageEffect);
            Definition.WeaponDescription.EffectDescription.EffectForms.Add(ExtraAttackEffect);
            Definition.WeaponDescription.EffectDescription.EffectForms.Add(NextAttackAdvantage);
            // game hangs when light effect is added, dont know why
            //Definition.WeaponDescription.EffectDescription.EffectForms.Add(MagicalLightSourceEffect);


        }

        public static ItemDefinition CreateAndAddToDB(string name, string guid)
        {
            return new ImprovedScoutSuitWeaponBuilder(name, guid).AddToDB();
        }

        public static ItemDefinition ImprovedScoutSuitWeapon = CreateAndAddToDB(ImprovedScoutSuitWeaponName, ImprovedScoutSuitWeaponGuid);
    }

    internal class DisadvantageOnAttackByEnemyBuilder : BaseDefinitionBuilder<ConditionDefinition>
    {
        private const string DisadvantageOnAttackByEnemyName = "DisadvantageOnAttackByEnemy";
        private const string DisadvantageOnAttackByEnemyGuid = "94bbcf4e-c376-4804-a157-2a5a5dd003e9";

        protected DisadvantageOnAttackByEnemyBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionCursedByBestowCurseAttackRoll, name, guid)
        {
            // Jolted - enemey has disadvantage on scout sentinel after weapon hits
            Definition.GuiPresentation.Title = "Rules/&DisadvantageOnAttackByEnemyTitle";
            Definition.GuiPresentation.Description = "Rules/&DisadvantageOnAttackByEnemyDescription";

        }

        public static ConditionDefinition CreateAndAddToDB(string name, string guid)
        {
            return new DisadvantageOnAttackByEnemyBuilder(name, guid).AddToDB();
        }

        public static ConditionDefinition DisadvantageOnAttackByEnemy = CreateAndAddToDB(DisadvantageOnAttackByEnemyName, DisadvantageOnAttackByEnemyGuid);
    }

    internal class AdvantageAttackOnEnemyBuilder : BaseDefinitionBuilder<ConditionDefinition>
    {
        private const string AdvantageAttackOnEnemyName = "AdvantageAttackOnEnemy";
        private const string AdvantageAttackOnEnemyGuid = "e10ff259-0294-4814-86de-327eaa1486a6";

        protected AdvantageAttackOnEnemyBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionTargetedByGuidingBolt, name, guid)
        {
            //Lit Up
            Definition.GuiPresentation.Title = "Rules/&AdvantageAttackOnEnemyTitle";
            Definition.GuiPresentation.Description = "Rules/&AdvantageAttackOnEnemyDescription";
        }

        public static ConditionDefinition CreateAndAddToDB(string name, string guid)
        {
            return new AdvantageAttackOnEnemyBuilder(name, guid).AddToDB();
        }

        public static ConditionDefinition AdvantageAttackOnEnemy = CreateAndAddToDB(AdvantageAttackOnEnemyName, AdvantageAttackOnEnemyGuid);
    }

    internal class ExtraDamageOnAttackConditionBuilder : BaseDefinitionBuilder<ConditionDefinition>
    {
        private const string ExtraDamageOnAttackConditionName = "ExtraDamageOnAttackCondition";
        private const string ExtraDamageOnAttackConditionGuid = "2a9bc931-adb9-4751-8770-3a1367920a57";

        protected ExtraDamageOnAttackConditionBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionMarkedByFate, name, guid)
        {
            Definition.Features.Clear();
            Definition.GuiPresentation.Title = "Rules/&ExtraDamageOnAttackConditionTitle";
            Definition.GuiPresentation.Description = "Rules/&ExtraDamageOnAttackConditionDescription";
            //Static shocked
            Definition.SetAllowMultipleInstances(true);
            Definition.SetConditionType(RuleDefinitions.ConditionType.Detrimental);
            Definition.SetDurationType(RuleDefinitions.DurationType.Round);
            Definition.SetDurationParameter(1);
            Definition.SetTurnOccurence(RuleDefinitions.TurnOccurenceType.EndOfTurn);
            Definition.HasSpecialInterruptionOfType(RuleDefinitions.ConditionInterruption.Damaged);



            Definition.SetAdditionalDamageWhenHit(true);
            Definition.SetAdditionalDamageDieNumber(1);
            Definition.SetAdditionalDamageDieType(RuleDefinitions.DieType.D6);
            Definition.SetAdditionalDamageType(RuleDefinitions.DamageTypeLightning);
            Definition.SetAdditionalDamageQuantity(ConditionDefinition.DamageQuantity.Dice);


        }

        public static ConditionDefinition CreateAndAddToDB(string name, string guid)
        {
            return new ExtraDamageOnAttackConditionBuilder(name, guid).AddToDB();
        }

        public static ConditionDefinition ExtraDamageOnAttackCondition = CreateAndAddToDB(ExtraDamageOnAttackConditionName, ExtraDamageOnAttackConditionGuid);
    }

}
