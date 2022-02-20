using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomFeatureDefinitions;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using UnityEngine;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Classes.Tinkerer.Subclasses
{
    //*****************************************************************************************************************************************
    //***********************************		ArtilleryConstructlevel03FeatureSetBuilder		***********************************************************
    //*****************************************************************************************************************************************

    internal sealed class ArtilleryConstructlevel03FeatureSetBuilder : FeatureDefinitionFeatureSetBuilder
    {
        private const string Name = "ArtilleryConstructlevel03FeatureSet";
        private const string Guid = "59f857e6-7b06-4c2b-a241-b73c42d64c23";

        // TODO: convert to lazy loading properties
        public static readonly FeatureDefinitionPower ArtilleryModePool = CreateArtilleryModePool();
        public static readonly FeatureDefinitionPowerSharedPool FlameArtillery_03modepower = CreateFlameArtillery03ModePower();
        public static readonly FeatureDefinitionPowerSharedPool ForceArtillery_03modepower = CreateForceArtillery03ModePower();
        public static readonly FeatureDefinitionPowerSharedPool TempHPShield_03modepower = CreateTempHPShield03ModePower();

        private static FeatureDefinitionPowerSharedPool CreateTempHPShield03ModePower()
        {
            GuiPresentationBuilder guiPresentationTempHPShield03 = new GuiPresentationBuilder(
                "Feature/&TempHPShieldModePowerTitle",
                "Feature/&TempHPShieldModePowerDescription");
            guiPresentationTempHPShield03.SetSpriteReference(Aid.GuiPresentation.SpriteReference);

            EffectDescriptionBuilder effectTempHPShieldmode03 = new EffectDescriptionBuilder();
            effectTempHPShieldmode03.SetDurationData(RuleDefinitions.DurationType.Hour, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            effectTempHPShieldmode03.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Distance, 1, RuleDefinitions.TargetType.Position, 1, 1, ActionDefinitions.ItemSelectionType.Equiped);
            effectTempHPShieldmode03.AddEffectForm(new EffectFormBuilder().SetSummonForm(SummonForm.Type.Creature, ScriptableObject.CreateInstance<ItemDefinition>(), 1, TempHPShieldConstructBuilder.TempHPShieldConstruct.name, null, true, null, ScriptableObject.CreateInstance<EffectProxyDefinition>()).Build());

            return new FeatureDefinitionPowerSharedPoolBuilder
                (
                 "TempHPShieldModePower"                                         // string name
                 , "b104ec34-5489-43b7-a887-0ffbb604ca8d"                    // string guid
                 , ArtilleryModePool                                             // FeatureDefinitionPower poolPower
                 , RuleDefinitions.RechargeRate.ShortRest                    // RuleDefinitions.RechargeRate recharge
                 , RuleDefinitions.ActivationTime.NoCost                     // RuleDefinitions.ActivationTime activationTime
                 , 1                                                         // int costPerUse
                 , false                                                     // bool proficiencyBonusToAttack
                 , false                                                     // bool abilityScoreBonusToAttack
                 , DatabaseHelper.SmartAttributeDefinitions.Intelligence.name// string abilityScore
                 , effectTempHPShieldmode03.Build()                            // EffectDescription effectDescription
                 , guiPresentationTempHPShield03.Build()                       // GuiPresentation guiPresentation
                 , true                                                      // bool uniqueInstanc
                ).AddToDB();
        }

        private static FeatureDefinitionPowerSharedPool CreateFlameArtillery03ModePower()
        {
            GuiPresentationBuilder guiPresentationFlameArtillery03 = new GuiPresentationBuilder(
               "Feature/&FlameArtilleryModePowerTitle",
               "Feature/&FlameArtilleryModePowerDescription");
            guiPresentationFlameArtillery03.SetSpriteReference(BurningHands.GuiPresentation.SpriteReference);

            EffectDescriptionBuilder effectFlameArtillerymode03 = new EffectDescriptionBuilder();
            effectFlameArtillerymode03.SetDurationData(RuleDefinitions.DurationType.Hour, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            effectFlameArtillerymode03.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Distance, 1, RuleDefinitions.TargetType.Position, 1, 1, ActionDefinitions.ItemSelectionType.Equiped);
            effectFlameArtillerymode03.AddEffectForm(new EffectFormBuilder().SetSummonForm(SummonForm.Type.Creature, ScriptableObject.CreateInstance<ItemDefinition>(), 1, FlameArtilleryConstructBuilder.FlameArtilleryConstruct.name, null, true, null, ScriptableObject.CreateInstance<EffectProxyDefinition>()).Build());

            return new FeatureDefinitionPowerSharedPoolBuilder(
                 "FlameArtilleryModePower"                                   // string name
                 , "f1559469-cf41-4204-a8c1-53379d04df43"                    // string guid
                 , ArtilleryModePool                                         // FeatureDefinitionPower poolPower
                 , RuleDefinitions.RechargeRate.LongRest                     // RuleDefinitions.RechargeRate recharge
                 , RuleDefinitions.ActivationTime.NoCost                     // RuleDefinitions.ActivationTime activationTime
                 , 1                                                         // int costPerUse
                 , false                                                     // bool proficiencyBonusToAttack
                 , false                                                     // bool abilityScoreBonusToAttack
                 , DatabaseHelper.SmartAttributeDefinitions.Intelligence.name// string abilityScore
                 , effectFlameArtillerymode03.Build()                          // EffectDescription effectDescription
                 , guiPresentationFlameArtillery03.Build()                     // GuiPresentation guiPresentation
                 , true                                                      // bool uniqueInstanc
                ).AddToDB();
        }

        private static FeatureDefinitionPowerSharedPool CreateForceArtillery03ModePower()
        {
            GuiPresentationBuilder guiPresentationForceArtillery03 = new GuiPresentationBuilder(
                "Feature/&ForceArtilleryModePowerTitle",
                "Feature/&ForceArtilleryModePowerDescription");
            guiPresentationForceArtillery03.SetSpriteReference(MagicMissile.GuiPresentation.SpriteReference);

            EffectDescriptionBuilder effectForceArtillerymode03 = new EffectDescriptionBuilder();
            effectForceArtillerymode03.SetDurationData(RuleDefinitions.DurationType.Hour, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            effectForceArtillerymode03.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Distance, 1, RuleDefinitions.TargetType.Position, 1, 1, ActionDefinitions.ItemSelectionType.Equiped);
            effectForceArtillerymode03.AddEffectForm(new EffectFormBuilder().SetSummonForm(SummonForm.Type.Creature, ScriptableObject.CreateInstance<ItemDefinition>(), 1, ForceArtilleryConstructBuilder.ForceArtilleryConstruct.name, null, true, null, ScriptableObject.CreateInstance<EffectProxyDefinition>()).Build());

            return new FeatureDefinitionPowerSharedPoolBuilder
                (
                 "ForceArtilleryModePower"                                   // string name
                 , "b01c40ca-685a-4d5d-9ee4-ed47d39cb5b9"                    // string guid
                 , ArtilleryModePool                                         // FeatureDefinitionPower poolPower
                 , RuleDefinitions.RechargeRate.ShortRest                    // RuleDefinitions.RechargeRate recharge
                 , RuleDefinitions.ActivationTime.NoCost                     // RuleDefinitions.ActivationTime activationTime
                 , 1                                                         // int costPerUse
                 , false                                                     // bool proficiencyBonusToAttack
                 , false                                                     // bool abilityScoreBonusToAttack
                 , DatabaseHelper.SmartAttributeDefinitions.Intelligence.name// string abilityScore
                 , effectForceArtillerymode03.Build()                          // EffectDescription effectDescription
                 , guiPresentationForceArtillery03.Build()                     // GuiPresentation guiPresentation
                 , true                                                      // bool uniqueInstanc
                ).AddToDB();
        }

        private static FeatureDefinitionPower CreateArtilleryModePool()
        {
            GuiPresentation guiPresentationArtilleryMode = new GuiPresentation();
            guiPresentationArtilleryMode.SetDescription("Feat/&ArtilleryModePoolDescription");
            guiPresentationArtilleryMode.SetTitle("Feat/&ArtilleryModePoolTitle");

            return new FeatureDefinitionPowerPoolBuilder(
                "ArtilleryModePool",
                "89d9c1f5-75e9-4b25-b7e8-a24f30d1befb",
                1,
                RuleDefinitions.UsesDetermination.Fixed,
                AttributeDefinitions.Intelligence,
                RuleDefinitions.RechargeRate.ShortRest,
                guiPresentationArtilleryMode).AddToDB();
        }

        private ArtilleryConstructlevel03FeatureSetBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetGreenmageWardenOfTheForest, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&SummonArtilleryConstructTitle";
            Definition.GuiPresentation.Description = "Feat/&SummonArtilleryConstructDescription";

            Definition.FeatureSet.Clear();
            Definition.FeatureSet.Add(ArtilleryModePool);
            Definition.FeatureSet.Add(TempHPShield_03modepower);
            Definition.FeatureSet.Add(FlameArtillery_03modepower);
            Definition.FeatureSet.Add(ForceArtillery_03modepower);
            Definition.FeatureSet.Add(AddConstructCantripsBuilder.AddConstructCantrips);
            //Definition.FeatureSet.Add(SummoningAffinityTinkererConstructBuilder.SummoningAffinityTinkererConstruct);
            Definition.FeatureSet.Add(SummoningAffinityTinkererArtilleryConstructBuilder.SummoningAffinityTinkererArtilleryConstruct);
        }

        private static FeatureDefinitionFeatureSet CreateAndAddToDB(string name, string guid)
        {
            return new ArtilleryConstructlevel03FeatureSetBuilder(name, guid).AddToDB();
        }

        public static readonly FeatureDefinitionFeatureSet ArtilleryConstructlevel03FeatureSet = CreateAndAddToDB(Name, Guid);
    }

    //*****************************************************************************************************************************************
    //***********************************		ArtilleryConstructlevel09FeatureSetBuilder		***********************************************************
    //*****************************************************************************************************************************************

    internal sealed class ArtilleryConstructlevel09FeatureSetBuilder : FeatureDefinitionFeatureSetBuilder
    {
        private const string Name = "ArtilleryConstructlevel09FeatureSet";
        private const string Guid = "a1e9557c-c1a9-4912-9fea-1a16c4124331";

        // TODO: convert to lazy loading properties
        public static readonly FeatureDefinitionPowerSharedPool FlameArtillery_09modepower = CreateFlameArtillery09ModePower();
        public static readonly FeatureDefinitionPowerSharedPool ForceArtillery_09modepower = CreateForceArtillery09ModePower();
        public static readonly FeatureDefinitionPowerSharedPool TempHPShield_09modepower = CreateTempHPShield09ModePower();

        private static FeatureDefinitionPowerSharedPool CreateFlameArtillery09ModePower()
        {
            GuiPresentationBuilder guiPresentationFlameArtillery09 = new GuiPresentationBuilder(
                "Feature/&FlameArtillery_09ModePowerTitle",
                "Feature/&FlameArtillery_09ModePowerDescription");
            guiPresentationFlameArtillery09.SetSpriteReference(BurningHands.GuiPresentation.SpriteReference);

            EffectDescriptionBuilder effectFlameArtillerymode09 = new EffectDescriptionBuilder();
            effectFlameArtillerymode09.SetDurationData(RuleDefinitions.DurationType.Hour, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            effectFlameArtillerymode09.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Distance, 1, RuleDefinitions.TargetType.Position, 1, 1, ActionDefinitions.ItemSelectionType.Equiped);
            effectFlameArtillerymode09.AddEffectForm(new EffectFormBuilder().SetSummonForm(SummonForm.Type.Creature, ScriptableObject.CreateInstance<ItemDefinition>(), 1, FlameArtilleryConstruct9Builder.FlameArtilleryConstruct9.name, null, true, null, ScriptableObject.CreateInstance<EffectProxyDefinition>()).Build());

            var power = new FeatureDefinitionPowerSharedPoolBuilder(
                 "FlameArtillery_09ModePower"                                   // string name
                 , "d841a2aa-efc0-4ccc-b50a-ed0d8e4d68e1"                    // string guid
                 , ArtilleryConstructlevel03FeatureSetBuilder.ArtilleryModePool// FeatureDefinitionPower poolPower
                 , RuleDefinitions.RechargeRate.LongRest                     // RuleDefinitions.RechargeRate recharge
                 , RuleDefinitions.ActivationTime.NoCost                     // RuleDefinitions.ActivationTime activationTime
                 , 1                                                         // int costPerUse
                 , false                                                     // bool proficiencyBonusToAttack
                 , false                                                     // bool abilityScoreBonusToAttack
                 , DatabaseHelper.SmartAttributeDefinitions.Intelligence.name// string abilityScore
                 , effectFlameArtillerymode09.Build()                          // EffectDescription effectDescription
                 , guiPresentationFlameArtillery09.Build()                     // GuiPresentation guiPresentation
                 , true                                                      // bool uniqueInstanc
                ).AddToDB();

            power.SetOverriddenPower(ArtilleryConstructlevel03FeatureSetBuilder.FlameArtillery_03modepower);

            return power;
        }

        private static FeatureDefinitionPowerSharedPool CreateForceArtillery09ModePower()
        {
            GuiPresentationBuilder guiPresentationForceArtillery09 = new GuiPresentationBuilder(
                "Feature/&ForceArtillery_09ModePowerTitle",
                "Feature/&ForceArtillery_09ModePowerDescription");
            guiPresentationForceArtillery09.SetSpriteReference(MagicMissile.GuiPresentation.SpriteReference);

            EffectDescriptionBuilder effectForceArtillerymode09 = new EffectDescriptionBuilder();
            effectForceArtillerymode09.SetDurationData(RuleDefinitions.DurationType.Hour, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            effectForceArtillerymode09.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Distance, 1, RuleDefinitions.TargetType.Position, 1, 1, ActionDefinitions.ItemSelectionType.Equiped);
            effectForceArtillerymode09.AddEffectForm(new EffectFormBuilder().SetSummonForm(SummonForm.Type.Creature, ScriptableObject.CreateInstance<ItemDefinition>(), 1, ForceArtilleryConstruct9Builder.ForceArtilleryConstruct9.name, null, true, null, ScriptableObject.CreateInstance<EffectProxyDefinition>()).Build());

            var power = new FeatureDefinitionPowerSharedPoolBuilder
                (
                 "ForceArtillery_09ModePower"                                   // string name
                 , "367fe374-e902-4ce9-9dc8-78d43c277faf"                    // string guid
                 , ArtilleryConstructlevel03FeatureSetBuilder.ArtilleryModePool// FeatureDefinitionPower poolPower
                 , RuleDefinitions.RechargeRate.ShortRest                    // RuleDefinitions.RechargeRate recharge
                 , RuleDefinitions.ActivationTime.NoCost                     // RuleDefinitions.ActivationTime activationTime
                 , 1                                                         // int costPerUse
                 , false                                                     // bool proficiencyBonusToAttack
                 , false                                                     // bool abilityScoreBonusToAttack
                 , DatabaseHelper.SmartAttributeDefinitions.Intelligence.name// string abilityScore
                 , effectForceArtillerymode09.Build()                          // EffectDescription effectDescription
                 , guiPresentationForceArtillery09.Build()                     // GuiPresentation guiPresentation
                 , true                                                      // bool uniqueInstanc
                ).AddToDB();

            power.SetOverriddenPower(ArtilleryConstructlevel03FeatureSetBuilder.ForceArtillery_03modepower);

            return power;
        }

        private static FeatureDefinitionPowerSharedPool CreateTempHPShield09ModePower()
        {
            GuiPresentationBuilder guiPresentationTempHPShield09 = new GuiPresentationBuilder(
                "Feature/&TempHPShield_09ModePowerTitle",
                "Feature/&TempHPShield_09ModePowerDescription");
            guiPresentationTempHPShield09.SetSpriteReference(Aid.GuiPresentation.SpriteReference);

            EffectDescriptionBuilder effectTempHPShieldmode09 = new EffectDescriptionBuilder();
            effectTempHPShieldmode09.SetDurationData(RuleDefinitions.DurationType.Hour, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            effectTempHPShieldmode09.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Distance, 1, RuleDefinitions.TargetType.Position, 1, 1, ActionDefinitions.ItemSelectionType.Equiped);
            effectTempHPShieldmode09.AddEffectForm(new EffectFormBuilder().SetSummonForm(SummonForm.Type.Creature, ScriptableObject.CreateInstance<ItemDefinition>(), 1, TempHPShieldConstruct9Builder.TempHPShieldConstruct9.name, null, true, null, ScriptableObject.CreateInstance<EffectProxyDefinition>()).Build());

            var power = new FeatureDefinitionPowerSharedPoolBuilder(
                 "TempHPShield_09ModePower"                                     // string name
                 , "feac61d2-d2af-43a8-8136-dce6df168d73"                    // string guid
                 , ArtilleryConstructlevel03FeatureSetBuilder.ArtilleryModePool // FeatureDefinitionPower poolPower
                 , RuleDefinitions.RechargeRate.ShortRest                    // RuleDefinitions.RechargeRate recharge
                 , RuleDefinitions.ActivationTime.NoCost                     // RuleDefinitions.ActivationTime activationTime
                 , 1                                                         // int costPerUse
                 , false                                                     // bool proficiencyBonusToAttack
                 , false                                                     // bool abilityScoreBonusToAttack
                 , DatabaseHelper.SmartAttributeDefinitions.Intelligence.name// string abilityScore
                 , effectTempHPShieldmode09.Build()                            // EffectDescription effectDescription
                 , guiPresentationTempHPShield09.Build()                       // GuiPresentation guiPresentation
                 , true                                                      // bool uniqueInstanc
                ).AddToDB();

            power.SetOverriddenPower(ArtilleryConstructlevel03FeatureSetBuilder.TempHPShield_03modepower);

            return power;
        }

        private ArtilleryConstructlevel09FeatureSetBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetGreenmageWardenOfTheForest, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&SummonArtilleryConstructlevel09Title";
            Definition.GuiPresentation.Description = "Feat/&SummonArtilleryConstructlevel09Description";

            Definition.FeatureSet.Clear();
            Definition.FeatureSet.Add(TempHPShield_09modepower);
            Definition.FeatureSet.Add(FlameArtillery_09modepower);
            Definition.FeatureSet.Add(ForceArtillery_09modepower);
        }

        private static FeatureDefinitionFeatureSet CreateAndAddToDB(string name, string guid)
        {
            return new ArtilleryConstructlevel09FeatureSetBuilder(name, guid).AddToDB();
        }

        public static readonly FeatureDefinitionFeatureSet ArtilleryConstructlevel09FeatureSet = CreateAndAddToDB(Name, Guid);
    }

    //*****************************************************************************************************************************************
    //***********************************		ArtilleryConstructlevel15FeatureSetBuilder		***********************************************************
    //*****************************************************************************************************************************************

    internal sealed class ArtilleryConstructlevel15FeatureSetBuilder : FeatureDefinitionFeatureSetBuilder
    {
        private const string Name = "ArtilleryConstructlevel15FeatureSet";
        private const string Guid = "50c91d16-1a84-494a-ba72-dd4879955f2f";

        // TODO: convert to lazy loading properties
        public static readonly FeatureDefinitionPowerSharedPool FlameArtillery_15modepower = CreateFlameArtillery15ModePower();
        public static readonly FeatureDefinitionPowerSharedPool ForceArtillery_15modepower = CreateForceArtillery15ModePower();
        public static readonly FeatureDefinitionPowerSharedPool TempHPShield_15modepower = CreateTempHPShield15ModePower();

        private static FeatureDefinitionPowerSharedPool CreateFlameArtillery15ModePower()
        {
            GuiPresentationBuilder guiPresentationFlameArtillery15 = new GuiPresentationBuilder(
                "Feature/&FlameArtillery_15ModePowerTitle",
                "Feature/&FlameArtillery_15ModePowerDescription");
            guiPresentationFlameArtillery15.SetSpriteReference(BurningHands.GuiPresentation.SpriteReference);

            EffectDescriptionBuilder effectFlameArtillerymode15 = new EffectDescriptionBuilder();
            effectFlameArtillerymode15.SetDurationData(RuleDefinitions.DurationType.Hour, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            effectFlameArtillerymode15.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Distance, 1, RuleDefinitions.TargetType.Position, 1, 1, ActionDefinitions.ItemSelectionType.Equiped);
            effectFlameArtillerymode15.AddEffectForm(new EffectFormBuilder().SetSummonForm(SummonForm.Type.Creature, ScriptableObject.CreateInstance<ItemDefinition>(), 1, FlameArtilleryConstruct15Builder.FlameArtilleryConstruct15.name, null, true, null, ScriptableObject.CreateInstance<EffectProxyDefinition>()).Build());

            var power = new FeatureDefinitionPowerSharedPoolBuilder
                (
                 "FlameArtillery_15ModePower"                                   // string name
                 , "0c84cc3b-0730-4e25-b8a7-8a074696efe4"                    // string guid
                 , ArtilleryConstructlevel03FeatureSetBuilder.ArtilleryModePool// FeatureDefinitionPower poolPower
                 , RuleDefinitions.RechargeRate.LongRest                     // RuleDefinitions.RechargeRate recharge
                 , RuleDefinitions.ActivationTime.NoCost                     // RuleDefinitions.ActivationTime activationTime
                 , 1                                                         // int costPerUse
                 , false                                                     // bool proficiencyBonusToAttack
                 , false                                                     // bool abilityScoreBonusToAttack
                 , DatabaseHelper.SmartAttributeDefinitions.Intelligence.name// string abilityScore
                 , effectFlameArtillerymode15.Build()                          // EffectDescription effectDescription
                 , guiPresentationFlameArtillery15.Build()                     // GuiPresentation guiPresentation
                 , true                                                      // bool uniqueInstanc
                ).AddToDB();

            power.SetOverriddenPower(ArtilleryConstructlevel09FeatureSetBuilder.FlameArtillery_09modepower);
            return power;
        }

        private static FeatureDefinitionPowerSharedPool CreateForceArtillery15ModePower()
        {
            GuiPresentationBuilder guiPresentationForceArtillery15 = new GuiPresentationBuilder(
                "Feature/&ForceArtillery_15ModePowerTitle",
                "Feature/&ForceArtillery_15ModePowerDescription");
            guiPresentationForceArtillery15.SetSpriteReference(MagicMissile.GuiPresentation.SpriteReference);

            EffectDescriptionBuilder effectForceArtillerymode15 = new EffectDescriptionBuilder();
            effectForceArtillerymode15.SetDurationData(RuleDefinitions.DurationType.Hour, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            effectForceArtillerymode15.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Distance, 1, RuleDefinitions.TargetType.Position, 1, 1, ActionDefinitions.ItemSelectionType.Equiped);
            effectForceArtillerymode15.AddEffectForm(new EffectFormBuilder().SetSummonForm(SummonForm.Type.Creature, ScriptableObject.CreateInstance<ItemDefinition>(), 1, ForceArtilleryConstruct15Builder.ForceArtilleryConstruct15.name, null, true, null, ScriptableObject.CreateInstance<EffectProxyDefinition>()).Build());

            var power = new FeatureDefinitionPowerSharedPoolBuilder
                (
                 "ForceArtillery_15ModePower"                                   // string name
                 , "c0e479c8-6fac-4c1a-9e98-58929e42264a"                    // string guid
                 , ArtilleryConstructlevel03FeatureSetBuilder.ArtilleryModePool// FeatureDefinitionPower poolPower
                 , RuleDefinitions.RechargeRate.ShortRest                    // RuleDefinitions.RechargeRate recharge
                 , RuleDefinitions.ActivationTime.NoCost                     // RuleDefinitions.ActivationTime activationTime
                 , 1                                                         // int costPerUse
                 , false                                                     // bool proficiencyBonusToAttack
                 , false                                                     // bool abilityScoreBonusToAttack
                 , DatabaseHelper.SmartAttributeDefinitions.Intelligence.name// string abilityScore
                 , effectForceArtillerymode15.Build()                          // EffectDescription effectDescription
                 , guiPresentationForceArtillery15.Build()                     // GuiPresentation guiPresentation
                 , true                                                      // bool uniqueInstanc
                ).AddToDB();

            power.SetOverriddenPower(ArtilleryConstructlevel09FeatureSetBuilder.ForceArtillery_09modepower);

            return power;
        }

        private static FeatureDefinitionPowerSharedPool CreateTempHPShield15ModePower()
        {
            GuiPresentationBuilder guiPresentationTempHPShield15 = new GuiPresentationBuilder(
                "Feature/&TempHPShield_15ModePowerTitle",
                "Feature/&TempHPShield_15ModePowerDescription");
            guiPresentationTempHPShield15.SetSpriteReference(Aid.GuiPresentation.SpriteReference);

            EffectDescriptionBuilder effectTempHPShieldmode15 = new EffectDescriptionBuilder();
            effectTempHPShieldmode15.SetDurationData(RuleDefinitions.DurationType.Hour, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            effectTempHPShieldmode15.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Distance, 1, RuleDefinitions.TargetType.Position, 1, 1, ActionDefinitions.ItemSelectionType.Equiped);
            effectTempHPShieldmode15.AddEffectForm(new EffectFormBuilder().SetSummonForm(SummonForm.Type.Creature, ScriptableObject.CreateInstance<ItemDefinition>(), 1, TempHPShieldConstruct15Builder.TempHPShieldConstruct15.name, null, true, null, ScriptableObject.CreateInstance<EffectProxyDefinition>()).Build());

            var power = new FeatureDefinitionPowerSharedPoolBuilder
                (
                 "TempHPShield_15ModePower"                                     // string name
                 , "82a01793-c5a4-4b87-a079-de79b7638c4f"                    // string guid
                 , ArtilleryConstructlevel03FeatureSetBuilder.ArtilleryModePool // FeatureDefinitionPower poolPower
                 , RuleDefinitions.RechargeRate.ShortRest                    // RuleDefinitions.RechargeRate recharge
                 , RuleDefinitions.ActivationTime.NoCost                     // RuleDefinitions.ActivationTime activationTime
                 , 1                                                         // int costPerUse
                 , false                                                     // bool proficiencyBonusToAttack
                 , false                                                     // bool abilityScoreBonusToAttack
                 , DatabaseHelper.SmartAttributeDefinitions.Intelligence.name// string abilityScore
                 , effectTempHPShieldmode15.Build()                            // EffectDescription effectDescription
                 , guiPresentationTempHPShield15.Build()                       // GuiPresentation guiPresentation
                 , true                                                      // bool uniqueInstanc
                ).AddToDB();

            power.SetOverriddenPower(ArtilleryConstructlevel09FeatureSetBuilder.TempHPShield_09modepower);
            return power;
        }

        private ArtilleryConstructlevel15FeatureSetBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetGreenmageWardenOfTheForest, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&SummonArtilleryConstructlevel15Title";
            Definition.GuiPresentation.Description = "Feat/&SummonArtilleryConstructlevel15Description";

            GuiPresentationBuilder artilleryPoolIncreaseGui = new GuiPresentationBuilder(
                "Subclass/&HArtilleryIncreaseTitle",
                "Subclass/&HArtilleryIncreaseDescription");

            FeatureDefinitionPowerPoolModifier artilleryPoolIncrease = new FeatureDefinitionPowerPoolModifierBuilder(
                "AttributeModiferArtilleryPoolIncrease",
                GuidHelper.Create(TinkererClass.GuidNamespace, "AttributeModiferArtilleryPoolIncrease").ToString(),
                1,
                RuleDefinitions.UsesDetermination.Fixed,
                AttributeDefinitions.Intelligence,
                ArtilleryConstructlevel03FeatureSetBuilder.ArtilleryModePool,
                artilleryPoolIncreaseGui.Build()
                ).AddToDB();

            Definition.FeatureSet.Clear();
            Definition.FeatureSet.Add(artilleryPoolIncrease);
            Definition.FeatureSet.Add(TempHPShield_15modepower);
            Definition.FeatureSet.Add(FlameArtillery_15modepower);
            Definition.FeatureSet.Add(ForceArtillery_15modepower);
        }

        private static FeatureDefinitionFeatureSet CreateAndAddToDB(string name, string guid)
        {
            return new ArtilleryConstructlevel15FeatureSetBuilder(name, guid).AddToDB();
        }

        public static readonly FeatureDefinitionFeatureSet ArtilleryConstructlevel15FeatureSet = CreateAndAddToDB(Name, Guid);
    }

    //*****************************************************************************************************************************************
    //***********************************		SelfDestructBuilder		*******************************************************************
    //*****************************************************************************************************************************************

    internal sealed class SelfDestructBuilder : FeatureDefinitionPowerBuilder
    {
        private const string SelfDestructName = "SelfDestruct";
        private const string SelfDestructGuid = "68ecf3d9-f718-4c62-921c-f30df3708312";

        private SelfDestructBuilder(string name, string guid) : base(ThunderShieldBuilder.ThunderShield, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&SelfDestructTitle";
            Definition.GuiPresentation.Description = "Feat/&SelfDestructDescription";
            Definition.GuiPresentation.SetSpriteReference(FlamingSphere.GuiPresentation.SpriteReference);
            Definition.SetShortTitleOverride("Feat/&SelfDestructTitle");

            Definition.SetActivationTime(RuleDefinitions.ActivationTime.Action);
            Definition.SetRechargeRate(RuleDefinitions.RechargeRate.AtWill);

            // SelfDestructionConditionBuilder.SelfDestructionCondition

            EffectForm SelfDestructConditionEffect = new EffectForm
            {
                ConditionForm = new ConditionForm(),
                FormType = EffectForm.EffectFormType.Condition
            };
            SelfDestructConditionEffect.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
            SelfDestructConditionEffect.ConditionForm.ConditionDefinition = SelfDestructionConditionBuilder.SelfDestructionCondition;
            SelfDestructConditionEffect.HasSavingThrow = false;
            SelfDestructConditionEffect.ConditionForm.SetApplyToSelf(true);
            SelfDestructConditionEffect.ConditionForm.SetForceOnSelf(true);
            // CounterForm SelfDestruct = new CounterForm();
            //
            // SelfDestruct.SetType (CounterForm.CounterType.DismissCreature);

            DamageForm ExplosionDamage = new DamageForm
            {
                DieType = RuleDefinitions.DieType.D8,
                DiceNumber = 3,
                DamageType = RuleDefinitions.DamageTypeForce,
                BonusDamage = 0
            };

            EffectForm ExplosionEffect = new EffectForm
            {
                FormType = EffectForm.EffectFormType.Damage,
                DamageForm = ExplosionDamage
            };
            ExplosionEffect.SetCreatedByCharacter(true);
            ExplosionEffect.HasSavingThrow = true;
            ExplosionEffect.SavingThrowAffinity = RuleDefinitions.EffectSavingThrowType.HalfDamage;
            ExplosionEffect.AddBonusMode = RuleDefinitions.AddBonusMode.None;
            ExplosionEffect.SetLevelMultiplier(1);
            ExplosionEffect.SetLevelType(RuleDefinitions.LevelSourceType.EffectLevel);
            ExplosionEffect.SetApplyLevel(EffectForm.LevelApplianceType.No);

            EffectDescription newEffectDescription = new EffectDescription();
            newEffectDescription.Copy(Definition.EffectDescription);
            newEffectDescription.EffectForms.Clear();
            newEffectDescription.EffectAdvancement.Clear();

            newEffectDescription.EffectForms.Add(SelfDestructConditionEffect);
            newEffectDescription.EffectForms.Add(ExplosionEffect);

            newEffectDescription.DurationParameter = 1;
            newEffectDescription.DurationType = RuleDefinitions.DurationType.Instantaneous;
            newEffectDescription.SetTargetType(RuleDefinitions.TargetType.Sphere);
            newEffectDescription.SetTargetParameter(4);
            newEffectDescription.SetTargetSide(RuleDefinitions.Side.All);
            newEffectDescription.SetRangeParameter(0);

            newEffectDescription.SetRangeType(RuleDefinitions.RangeType.Self);
            newEffectDescription.SavingThrowAbility = DatabaseHelper.SmartAttributeDefinitions.Dexterity.Name;
            newEffectDescription.FixedSavingThrowDifficultyClass = 17;
            newEffectDescription.SetDifficultyClassComputation(RuleDefinitions.EffectDifficultyClassComputation.FixedValue);
            newEffectDescription.HasSavingThrow = true;
            newEffectDescription.SetCreatedByCharacter(true);
            newEffectDescription.SetCanBePlacedOnCharacter(true);
            newEffectDescription.SetEffectParticleParameters(Fireball.EffectDescription.EffectParticleParameters);

            Definition.SetUniqueInstance(true);
            Definition.SetEffectDescription(newEffectDescription);
        }

        private static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
        {
            return new SelfDestructBuilder(name, guid).AddToDB();
        }

        public static readonly FeatureDefinitionPower SelfDestruct = CreateAndAddToDB(SelfDestructName, SelfDestructGuid);
    }

    //*****************************************************************************************************************************************
    //***********************************		SelfDestructionConditionBuilder		*******************************************************************
    //*****************************************************************************************************************************************

    internal sealed class SelfDestructionConditionBuilder : ConditionDefinitionBuilder
    {
        private const string SelfDestructionConditionName = "SelfDestructionCondition";
        private const string SelfDestructionConditionGuid = "c96984f4-0370-4775-ab8b-3ee6db0c8806";

        private SelfDestructionConditionBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionDummy, name, guid)
        {
            Definition.GuiPresentation.Title = "Rules/&SelfDestructionConditionTitle";
            Definition.GuiPresentation.Description = "Rules/&SelfDestructionConditionDescription";

            Definition.Features.Clear();

            KillForm SelfDestruct = new KillForm();
            SelfDestruct.SetChallengeRating(10);
            SelfDestruct.SetHitPoints(200);
            SelfDestruct.SetKillCondition(RuleDefinitions.KillCondition.Always);

            EffectForm KillEffect = new EffectForm
            {
                FormType = EffectForm.EffectFormType.Kill
            };
            KillEffect.SetKillForm(SelfDestruct);
            KillEffect.SetCreatedByCharacter(true);
            KillEffect.AddBonusMode = RuleDefinitions.AddBonusMode.None;
            KillEffect.SetLevelMultiplier(1);
            KillEffect.SetLevelType(RuleDefinitions.LevelSourceType.EffectLevel);
            KillEffect.SetApplyLevel(EffectForm.LevelApplianceType.No);

            Definition.RecurrentEffectForms.Add(KillEffect);

            Definition.SetDurationType(RuleDefinitions.DurationType.Permanent);
            Definition.SetTurnOccurence(RuleDefinitions.TurnOccurenceType.EndOfTurn);
            Definition.SetSpecialDuration(true);
            Definition.SetDurationParameter(1);
            Definition.SetConditionType(RuleDefinitions.ConditionType.Detrimental);
        }

        private static ConditionDefinition CreateAndAddToDB(string name, string guid)
        {
            return new SelfDestructionConditionBuilder(name, guid).AddToDB();
        }

        public static readonly ConditionDefinition SelfDestructionCondition = CreateAndAddToDB(SelfDestructionConditionName, SelfDestructionConditionGuid);
    }

    //*****************************************************************************************************************************************
    //***********************************		HalfCoverShieldBuilder		*******************************************************************
    //*****************************************************************************************************************************************

    internal sealed class HalfCoverShieldBuilder : FeatureDefinitionPowerBuilder
    {
        private const string HalfCoverShieldName = "HalfCoverShield";
        private const string HalfCoverShieldGuid = "0fd3ccb3-0f15-4766-8eb8-32042838ce6d";

        private HalfCoverShieldBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionPowers.PowerDomainBattleHeraldOfBattle, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&HalfCoverShieldTitle";
            Definition.GuiPresentation.Description = "Feat/&HalfCoverShieldDescription";
            Definition.GuiPresentation.SetSpriteReference(Shield.GuiPresentation.SpriteReference);

            EffectForm halfCoverShield = new EffectForm
            {
                ConditionForm = new ConditionForm(),
                FormType = EffectForm.EffectFormType.Condition
            };
            halfCoverShield.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
            halfCoverShield.ConditionForm.SetConditionDefinitionName(HalfCoverShieldConditionBuilder.HalfCoverShieldCondition.Name);
            halfCoverShield.ConditionForm.ConditionDefinition = HalfCoverShieldConditionBuilder.HalfCoverShieldCondition;// DistractingPulseBuilder.DistractingPulse;

            halfCoverShield.SetCreatedByCharacter(true);

            halfCoverShield.AddBonusMode = RuleDefinitions.AddBonusMode.AbilityBonus;
            halfCoverShield.SetLevelMultiplier(1);
            halfCoverShield.SetLevelType(RuleDefinitions.LevelSourceType.EffectLevel);
            halfCoverShield.SetApplyLevel(EffectForm.LevelApplianceType.No);

            Definition.EffectDescription.EffectAdvancement.Clear();
            Definition.EffectDescription.EffectForms.Clear();
            Definition.EffectDescription.EffectForms.Add(halfCoverShield);
            Definition.EffectDescription.SetTargetType(RuleDefinitions.TargetType.Sphere);
            Definition.EffectDescription.SetTargetParameter(2);
            Definition.EffectDescription.SetTargetSide(RuleDefinitions.Side.Ally);
            Definition.EffectDescription.SetRangeParameter(2);
            Definition.EffectDescription.HasSavingThrow = false;
            Definition.EffectDescription.SavingThrowAbility = DatabaseHelper.SmartAttributeDefinitions.Dexterity.Name;

            Definition.EffectDescription.SetCreatedByCharacter(true);
            Definition.EffectDescription.SetCanBePlacedOnCharacter(true);
            Definition.EffectDescription.SetRangeType(RuleDefinitions.RangeType.Self);
            Definition.EffectDescription.SetEffectParticleParameters(Shield.EffectDescription.EffectParticleParameters);
        }

        private static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
        {
            return new HalfCoverShieldBuilder(name, guid).AddToDB();
        }

        public static readonly FeatureDefinitionPower HalfCoverShield = CreateAndAddToDB(HalfCoverShieldName, HalfCoverShieldGuid);
    }

    //*****************************************************************************************************************************************
    //***********************************		HalfCoverShieldConditionBuilder		*******************************************************************
    //*****************************************************************************************************************************************

    internal sealed class HalfCoverShieldConditionBuilder : ConditionDefinitionBuilder
    {
        private const string HalfCoverShieldConditionName = "HalfCoverShieldCondition";
        private const string HalfCoverShieldConditionGuid = "c160a20e-4714-478a-8bbb-6df5526728a9";

        private HalfCoverShieldConditionBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionHeraldOfBattle, name, guid)
        {
            Definition.GuiPresentation.Title = "Rules/&HalfCoverShieldConditionTitle";
            Definition.GuiPresentation.Description = "Rules/&HalfCoverShieldConditionDescription";

            Definition.Features.Clear();
            Definition.Features.Add(HalfCoverShieldAttributeBuilder.HalfCoverShieldAttribute);
        }

        private static ConditionDefinition CreateAndAddToDB(string name, string guid)
        {
            return new HalfCoverShieldConditionBuilder(name, guid).AddToDB();
        }

        public static readonly ConditionDefinition HalfCoverShieldCondition = CreateAndAddToDB(HalfCoverShieldConditionName, HalfCoverShieldConditionGuid);
    }

    //*****************************************************************************************************************************************
    //***********************************		HalfCoverShieldAttributeBuilder		*******************************************************************
    //*****************************************************************************************************************************************

    internal sealed class HalfCoverShieldAttributeBuilder : FeatureDefinitionAttributeModifierBuilder
    {
        private const string HalfCoverShieldAttributeName = "HalfCoverShieldAttribute";
        private const string HalfCoverShieldAttributeGuid = "4dd6548f-4d7f-4bbf-b120-70a17f79f8e0";

        private HalfCoverShieldAttributeBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierHeraldOfBattle, name, guid)
        {
            Definition.GuiPresentation.Title = "Rules/&HalfCoverShieldAttributeTitle";
            Definition.GuiPresentation.Description = "Rules/&HalfCoverShieldAttributeDescription";

            Definition.SetModifiedAttribute(DatabaseHelper.SmartAttributeDefinitions.ArmorClass.Name);
            Definition.SetModifierType2(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive);
            Definition.SetModifierValue(2);
        }

        private static FeatureDefinitionAttributeModifier CreateAndAddToDB(string name, string guid)
        {
            return new HalfCoverShieldAttributeBuilder(name, guid).AddToDB();
        }

        public static readonly FeatureDefinitionAttributeModifier HalfCoverShieldAttribute = CreateAndAddToDB(HalfCoverShieldAttributeName, HalfCoverShieldAttributeGuid);
    }

    //*****************************************************************************************************************************************
    //***********************************		SummonArtillerySpellConstructBuilder		*******************************************************************
    //*****************************************************************************************************************************************

    internal sealed class SummonArtillerySpellConstructBuilder : SpellDefinitionBuilder
    {
        private const string SummonArtillerySpellConstructName = "SummonArtillerySpellConstruct";
        private const string SummonArtillerySpellConstructGuid = "214dab9d-f40e-424f-8730-b41acdae26ec";

        private SummonArtillerySpellConstructBuilder(string name, string guid) : base(DancingLights, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&ResummonArtilleryConstruct_03Title";
            Definition.GuiPresentation.Description = "Feat/&ResummonArtilleryConstructDescription";
            Definition.GuiPresentation.SetSpriteReference(FaerieFire.GuiPresentation.SpriteReference);

            Definition.SetSpellLevel(1);
            Definition.SetRequiresConcentration(false);
            Definition.SetUniqueInstance(true);
            Definition.SetCastingTime(RuleDefinitions.ActivationTime.Action);

            Definition.SetSpellsBundle(true);
            Definition.SubspellsList.AddRange(
                SummonFlameArtillerySpellConstructBuilder.SummonFlameArtilleryConstruct,
                SummonForceArtillerySpellConstructBuilder.SummonForceArtilleryConstruct,
                SummonTempHPShieldSpellConstructBuilder.SummonTempHPShieldConstruct
            );

            Definition.EffectDescription.Clear();
        }

        private static SpellDefinition CreateAndAddToDB(string name, string guid)
        {
            return new SummonArtillerySpellConstructBuilder(name, guid).AddToDB();
        }

        public static readonly SpellDefinition SummonArtillerySpellConstruct = CreateAndAddToDB(SummonArtillerySpellConstructName, SummonArtillerySpellConstructGuid);
    }

    //*****************************************************************************************************************************************
    //***********************************		SummonArtillerySpellConstruct_9Builder		*******************************************************************
    //*****************************************************************************************************************************************

    internal sealed class SummonArtillerySpellConstruct9Builder : SpellDefinitionBuilder
    {
        private const string SummonArtillerySpellConstruct_9Name = "SummonArtillerySpellConstruct_9";
        private const string SummonArtillerySpellConstruct_9Guid = "9ad29b43-5207-46e8-bbc9-7b8138b2912f";

        private SummonArtillerySpellConstruct9Builder(string name, string guid) : base(SummonArtillerySpellConstructBuilder.SummonArtillerySpellConstruct, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&ResummonArtilleryConstruct_09Title";
            Definition.GuiPresentation.Description = "Feat/&ResummonArtilleryConstructDescription";

            Definition.SubspellsList.Clear();
            Definition.SubspellsList.AddRange(
                SummonFlameArtillerySpellConstruct9Builder.SummonFlameArtilleryConstruct9,
                SummonForceArtillerySpellConstruct9Builder.SummonForceArtilleryConstruct_9,
                SummonTempHPShieldSpellConstruct9Builder.SummonTempHPShieldConstruct_9
            );
        }

        private static SpellDefinition CreateAndAddToDB(string name, string guid)
        {
            return new SummonArtillerySpellConstruct9Builder(name, guid).AddToDB();
        }

        public static readonly SpellDefinition SummonArtillerySpellConstruct9 = CreateAndAddToDB(SummonArtillerySpellConstruct_9Name, SummonArtillerySpellConstruct_9Guid);
    }

    //*****************************************************************************************************************************************
    //***********************************		SummonArtillerySpellConstruct_15Builder		*******************************************************************
    //*****************************************************************************************************************************************

    internal sealed class SummonArtillerySpellConstruct15Builder : SpellDefinitionBuilder
    {
        private const string SummonArtillerySpellConstruct_15Name = "SummonArtillerySpellConstruct_15";
        private const string SummonArtillerySpellConstruct_15Guid = "f0038039-423b-4265-b0cf-2eab2450f982";

        private SummonArtillerySpellConstruct15Builder(string name, string guid) : base(SummonArtillerySpellConstructBuilder.SummonArtillerySpellConstruct, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&ResummonArtilleryConstruct_15Title";
            Definition.GuiPresentation.Description = "Feat/&ResummonArtilleryConstructDescription";

            Definition.SubspellsList.Clear();
            Definition.SubspellsList.AddRange(
                SummonFlameArtillerySpellConstruct15Builder.SummonFlameArtilleryConstruct15,
                SummonForceArtillerySpellConstruct15Builder.SummonForceArtilleryConstruct15,
                SummonTempHPShieldSpellConstruct15Builder.SummonTempHPShieldConstruct15
            );
        }

        private static SpellDefinition CreateAndAddToDB(string name, string guid)
        {
            return new SummonArtillerySpellConstruct15Builder(name, guid).AddToDB();
        }

        public static readonly SpellDefinition SummonArtillerySpellConstruct15 = CreateAndAddToDB(SummonArtillerySpellConstruct_15Name, SummonArtillerySpellConstruct_15Guid);
    }

    internal sealed class SummoningAffinityTinkererArtilleryConstructBuilder : FeatureDefinitionSummoningAffinityBuilder
    {
        private const string Name = "SummoningAffinityTinkererArtilleryConstruct";
        private const string Guid = "edae2ec0-f871-4918-855a-117bd428d51e";

        private SummoningAffinityTinkererArtilleryConstructBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionSummoningAffinitys.SummoningAffinityKindredSpiritBond, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&NoContentTitle";
            Definition.GuiPresentation.Description = "Feature/&NoContentTitle";
            Definition.GuiPresentation.SetSpriteReference(null);

            Definition.SetEffectOnConjuredDeath(false);
            Definition.AddedConditions.Clear();
            Definition.AddedConditions.Empty();
            Definition.EffectForms.Clear();
            Definition.EffectForms.Empty();

            // changed the tag here and in relevant constructs
            // so the scaling is only applied to the Protector and Artillry Constructs
            Definition.SetRequiredMonsterTag("ScalingTinkererArtilleryConstruct");
            Definition.AddedConditions.AddRange(
                // using kindred conditions for following reasons
                // 1- Didnt want to create custom conditions until custom ConditionDefintionBuilder and
                //    FeatureDefinitionAttributeModifierBuilder are available as it is likely a rewrite
                //    would be requested as soon as such builders were added.
                // 2- The tabletop scaling using the class level and proficiency bonus of the summoner
                //    is not possible using base game features/database manipulation. A patch would be
                //    required to add such scaling to the game.
                // 3- A new scaling set via new summoningAffinity, conditionDefinitions and attributeModifers
                //    could be added but custom conditions may not be worthwhile as without the above patch,
                //    meaning the any new scaling would not match the required scaling
                // 4- The default summons scaling used in the base game is similar in magnitude to the original
                //    concept for the protector construct, so it seemed acceptable for a first implementation.
                //
                //DatabaseHelper.ConditionDefinitions.ConditionKindredSpiritBondTotalControl,
                //DatabaseHelper.ConditionDefinitions.ConditionKindredSpiritBondAC,
                DatabaseHelper.ConditionDefinitions.ConditionKindredSpiritBondHP,
                DatabaseHelper.ConditionDefinitions.ConditionKindredSpiritBondMeleeAttack
            //DatabaseHelper.ConditionDefinitions.ConditionKindredSpiritBondMeleeDamage,
            //DatabaseHelper.ConditionDefinitions.ConditionKindredSpiritBondSkillProficiency,
            //DatabaseHelper.ConditionDefinitions.ConditionKindredSpiritBondSavingThrows
            );
        }

        private static FeatureDefinitionSummoningAffinity CreateAndAddToDB(string name, string guid)
        {
            return new SummoningAffinityTinkererArtilleryConstructBuilder(name, guid).AddToDB();
        }

        public static readonly FeatureDefinitionSummoningAffinity SummoningAffinityTinkererArtilleryConstruct = CreateAndAddToDB(Name, Guid);
    }
}
