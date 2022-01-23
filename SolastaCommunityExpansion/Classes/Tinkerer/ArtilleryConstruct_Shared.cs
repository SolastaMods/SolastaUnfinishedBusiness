using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.CustomFeatureDefinitions;
using SolastaModApi;
using SolastaModApi.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace SolastaCommunityExpansion.Classes.Tinkerer
{
    //*****************************************************************************************************************************************
    //***********************************		ArtilleryConstructlevel03FeatureSetBuilder		***********************************************************
    //*****************************************************************************************************************************************

    internal class ArtilleryConstructlevel03FeatureSetBuilder : BaseDefinitionBuilder<FeatureDefinitionFeatureSet>
    {
        private const string Name = "ArtilleryConstructlevel03FeatureSet";
        private const string Guid = "59f857e6-7b06-4c2b-a241-b73c42d64c23";

        public static FeatureDefinitionPower ArtilleryModePool;
        public static FeatureDefinitionPowerSharedPool FlameArtillery_03modepower;
        public static FeatureDefinitionPowerSharedPool ForceArtillery_03modepower;
        public static FeatureDefinitionPowerSharedPool TempHPShield_03modepower;

        protected ArtilleryConstructlevel03FeatureSetBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetGreenmageWardenOfTheForest, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&SummonArtilleryConstructTitle";
            Definition.GuiPresentation.Description = "Feat/&SummonArtilleryConstructDescription";

            GuiPresentation guiPresentationArtilleryMode = new GuiPresentation();
            guiPresentationArtilleryMode.SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f));
            guiPresentationArtilleryMode.SetDescription("Feat/&ArtilleryModePoolDescription");
            guiPresentationArtilleryMode.SetTitle("Feat/&ArtilleryModePoolTitle");
            guiPresentationArtilleryMode.SetSpriteReference(null);
            guiPresentationArtilleryMode.SetSymbolChar("221E");

            ArtilleryModePool = new FeatureDefinitionPowerPoolBuilder
               (
                   "ArtilleryModePool",
                   "89d9c1f5-75e9-4b25-b7e8-a24f30d1befb",
                   1,
                   RuleDefinitions.UsesDetermination.Fixed,
                   AttributeDefinitions.Intelligence,
                   RuleDefinitions.RechargeRate.ShortRest,
                   guiPresentationArtilleryMode
               ).AddToDB();

            GuiPresentationBuilder guiPresentationFlameArtillery03 = new GuiPresentationBuilder(
                "Feature/&FlameArtilleryModePowerDescription",
                "Feature/&FlameArtilleryModePowerTitle");
            guiPresentationFlameArtillery03.SetSpriteReference(DatabaseHelper.SpellDefinitions.BurningHands.GuiPresentation.SpriteReference);

            EffectDescriptionBuilder effectFlameArtillerymode03 = new EffectDescriptionBuilder();
            effectFlameArtillerymode03.SetDurationData(RuleDefinitions.DurationType.Hour, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            effectFlameArtillerymode03.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Distance, 1, RuleDefinitions.TargetType.Position, 1, 1, ActionDefinitions.ItemSelectionType.Equiped);
            effectFlameArtillerymode03.AddEffectForm(new EffectFormBuilder().SetSummonForm(SummonForm.Type.Creature, ScriptableObject.CreateInstance<ItemDefinition>(), 1, FlameArtilleryConstructBuilder.FlameArtilleryConstruct.name, null, true, null, ScriptableObject.CreateInstance<EffectProxyDefinition>()).Build());

            FlameArtillery_03modepower = new FeatureDefinitionPowerSharedPoolBuilder
                (
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

            GuiPresentationBuilder guiPresentationForceArtillery03 = new GuiPresentationBuilder(
                "Feature/&ForceArtilleryModePowerDescription",
                "Feature/&ForceArtilleryModePowerTitle");
            guiPresentationForceArtillery03.SetSpriteReference(DatabaseHelper.SpellDefinitions.MagicMissile.GuiPresentation.SpriteReference);

            EffectDescriptionBuilder effectForceArtillerymode03 = new EffectDescriptionBuilder();
            effectForceArtillerymode03.SetDurationData(RuleDefinitions.DurationType.Hour, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            effectForceArtillerymode03.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Distance, 1, RuleDefinitions.TargetType.Position, 1, 1, ActionDefinitions.ItemSelectionType.Equiped);
            effectForceArtillerymode03.AddEffectForm(new EffectFormBuilder().SetSummonForm(SummonForm.Type.Creature, ScriptableObject.CreateInstance<ItemDefinition>(), 1, ForceArtilleryConstructBuilder.ForceArtilleryConstruct.name, null, true, null, ScriptableObject.CreateInstance<EffectProxyDefinition>()).Build());

            ForceArtillery_03modepower = new FeatureDefinitionPowerSharedPoolBuilder
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

            GuiPresentationBuilder guiPresentationTempHPShield03 = new GuiPresentationBuilder(
                "Feature/&TempHPShieldModePowerDescription",
                "Feature/&TempHPShieldModePowerTitle");
            guiPresentationTempHPShield03.SetSpriteReference(DatabaseHelper.SpellDefinitions.Aid.GuiPresentation.SpriteReference);

            EffectDescriptionBuilder effectTempHPShieldmode03 = new EffectDescriptionBuilder();
            effectTempHPShieldmode03.SetDurationData(RuleDefinitions.DurationType.Hour, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            effectTempHPShieldmode03.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Distance, 1, RuleDefinitions.TargetType.Position, 1, 1, ActionDefinitions.ItemSelectionType.Equiped);
            effectTempHPShieldmode03.AddEffectForm(new EffectFormBuilder().SetSummonForm(SummonForm.Type.Creature, ScriptableObject.CreateInstance<ItemDefinition>(), 1, TempHPShieldConstructBuilder.TempHPShieldConstruct.name, null, true, null, ScriptableObject.CreateInstance<EffectProxyDefinition>()).Build());

            TempHPShield_03modepower = new FeatureDefinitionPowerSharedPoolBuilder
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

            Definition.FeatureSet.Clear();
            Definition.FeatureSet.Add(ArtilleryModePool);
            Definition.FeatureSet.Add(TempHPShield_03modepower);
            Definition.FeatureSet.Add(FlameArtillery_03modepower);
            Definition.FeatureSet.Add(ForceArtillery_03modepower);
            Definition.FeatureSet.Add(AddConstructCantripsBuilder.AddConstructCantrips);
            //Definition.FeatureSet.Add(SummoningAffinityTinkererConstructBuilder.SummoningAffinityTinkererConstruct);
            Definition.FeatureSet.Add(SummoningAffinityTinkererArtilleryConstructBuilder.SummoningAffinityTinkererArtilleryConstruct);
        }

        public static FeatureDefinitionFeatureSet CreateAndAddToDB(string name, string guid)
        {
            return new ArtilleryConstructlevel03FeatureSetBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionFeatureSet ArtilleryConstructlevel03FeatureSet = CreateAndAddToDB(Name, Guid);
    }

    //*****************************************************************************************************************************************
    //***********************************		ArtilleryConstructlevel09FeatureSetBuilder		***********************************************************
    //*****************************************************************************************************************************************

    internal class ArtilleryConstructlevel09FeatureSetBuilder : BaseDefinitionBuilder<FeatureDefinitionFeatureSet>
    {
        private const string Name = "ArtilleryConstructlevel09FeatureSet";
        private const string Guid = "a1e9557c-c1a9-4912-9fea-1a16c4124331";

        public static FeatureDefinitionPowerSharedPool FlameArtillery_09modepower;
        public static FeatureDefinitionPowerSharedPool ForceArtillery_09modepower;
        public static FeatureDefinitionPowerSharedPool TempHPShield_09modepower;

        protected ArtilleryConstructlevel09FeatureSetBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetGreenmageWardenOfTheForest, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&SummonArtilleryConstructlevel09Title";
            Definition.GuiPresentation.Description = "Feat/&SummonArtilleryConstructlevel09Description";

            GuiPresentationBuilder guiPresentationFlameArtillery09 = new GuiPresentationBuilder(
                "Feature/&FlameArtillery_09ModePowerDescription",
                "Feature/&FlameArtillery_09ModePowerTitle");
            guiPresentationFlameArtillery09.SetSpriteReference(DatabaseHelper.SpellDefinitions.BurningHands.GuiPresentation.SpriteReference);

            EffectDescriptionBuilder effectFlameArtillerymode09 = new EffectDescriptionBuilder();
            effectFlameArtillerymode09.SetDurationData(RuleDefinitions.DurationType.Hour, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            effectFlameArtillerymode09.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Distance, 1, RuleDefinitions.TargetType.Position, 1, 1, ActionDefinitions.ItemSelectionType.Equiped);
            effectFlameArtillerymode09.AddEffectForm(new EffectFormBuilder().SetSummonForm(SummonForm.Type.Creature, ScriptableObject.CreateInstance<ItemDefinition>(), 1, FlameArtilleryConstruct_9Builder.FlameArtilleryConstruct_9.name, null, true, null, ScriptableObject.CreateInstance<EffectProxyDefinition>()).Build());

            FlameArtillery_09modepower = new FeatureDefinitionPowerSharedPoolBuilder
                (
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
            FlameArtillery_09modepower.SetOverriddenPower(ArtilleryConstructlevel03FeatureSetBuilder.FlameArtillery_03modepower);

            GuiPresentationBuilder guiPresentationForceArtillery09 = new GuiPresentationBuilder(
                "Feature/&ForceArtillery_09ModePowerDescription",
                "Feature/&ForceArtillery_09ModePowerTitle");
            guiPresentationForceArtillery09.SetSpriteReference(DatabaseHelper.SpellDefinitions.MagicMissile.GuiPresentation.SpriteReference);

            EffectDescriptionBuilder effectForceArtillerymode09 = new EffectDescriptionBuilder();
            effectForceArtillerymode09.SetDurationData(RuleDefinitions.DurationType.Hour, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            effectForceArtillerymode09.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Distance, 1, RuleDefinitions.TargetType.Position, 1, 1, ActionDefinitions.ItemSelectionType.Equiped);
            effectForceArtillerymode09.AddEffectForm(new EffectFormBuilder().SetSummonForm(SummonForm.Type.Creature, ScriptableObject.CreateInstance<ItemDefinition>(), 1, ForceArtilleryConstruct_9Builder.ForceArtilleryConstruct_9.name, null, true, null, ScriptableObject.CreateInstance<EffectProxyDefinition>()).Build());

            ForceArtillery_09modepower = new FeatureDefinitionPowerSharedPoolBuilder
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
            ForceArtillery_09modepower.SetOverriddenPower(ArtilleryConstructlevel03FeatureSetBuilder.ForceArtillery_03modepower);

            GuiPresentationBuilder guiPresentationTempHPShield09 = new GuiPresentationBuilder(
                "Feature/&TempHPShield_09ModePowerDescription",
                "Feature/&TempHPShield_09ModePowerTitle");
            guiPresentationTempHPShield09.SetSpriteReference(DatabaseHelper.SpellDefinitions.Aid.GuiPresentation.SpriteReference);

            EffectDescriptionBuilder effectTempHPShieldmode09 = new EffectDescriptionBuilder();
            effectTempHPShieldmode09.SetDurationData(RuleDefinitions.DurationType.Hour, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            effectTempHPShieldmode09.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Distance, 1, RuleDefinitions.TargetType.Position, 1, 1, ActionDefinitions.ItemSelectionType.Equiped);
            effectTempHPShieldmode09.AddEffectForm(new EffectFormBuilder().SetSummonForm(SummonForm.Type.Creature, ScriptableObject.CreateInstance<ItemDefinition>(), 1, TempHPShieldConstruct_9Builder.TempHPShieldConstruct_9.name, null, true, null, ScriptableObject.CreateInstance<EffectProxyDefinition>()).Build());

            TempHPShield_09modepower = new FeatureDefinitionPowerSharedPoolBuilder
                (
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
            TempHPShield_09modepower.SetOverriddenPower(ArtilleryConstructlevel03FeatureSetBuilder.TempHPShield_03modepower);

            Definition.FeatureSet.Clear();
            Definition.FeatureSet.Add(TempHPShield_09modepower);
            Definition.FeatureSet.Add(FlameArtillery_09modepower);
            Definition.FeatureSet.Add(ForceArtillery_09modepower);
        }

        public static FeatureDefinitionFeatureSet CreateAndAddToDB(string name, string guid)
        {
            return new ArtilleryConstructlevel09FeatureSetBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionFeatureSet ArtilleryConstructlevel09FeatureSet = CreateAndAddToDB(Name, Guid);
    }

    //*****************************************************************************************************************************************
    //***********************************		ArtilleryConstructlevel15FeatureSetBuilder		***********************************************************
    //*****************************************************************************************************************************************

    internal class ArtilleryConstructlevel15FeatureSetBuilder : BaseDefinitionBuilder<FeatureDefinitionFeatureSet>
    {
        private const string Name = "ArtilleryConstructlevel15FeatureSet";
        private const string Guid = "50c91d16-1a84-494a-ba72-dd4879955f2f";

        public static FeatureDefinitionPowerSharedPool FlameArtillery_15modepower;
        public static FeatureDefinitionPowerSharedPool ForceArtillery_15modepower;
        public static FeatureDefinitionPowerSharedPool TempHPShield_15modepower;

        protected ArtilleryConstructlevel15FeatureSetBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetGreenmageWardenOfTheForest, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&SummonArtilleryConstructlevel15Title";
            Definition.GuiPresentation.Description = "Feat/&SummonArtilleryConstructlevel15Description";

            GuiPresentationBuilder ArtilleryPoolIncreaseGui = new GuiPresentationBuilder(
                "Subclass/&HArtilleryIncreaseDescription",
                "Subclass/&HArtilleryIncreaseTitle");

            FeatureDefinitionPowerPoolModifier ArtilleryPoolIncrease = new FeatureDefinitionPowerPoolModifierBuilder(
                "AttributeModiferArtilleryPoolIncrease",
                GuidHelper.Create(TinkererClass.GuidNamespace, "AttributeModiferArtilleryPoolIncrease").ToString(),
                1,
                RuleDefinitions.UsesDetermination.Fixed,
                AttributeDefinitions.Intelligence,
                ArtilleryConstructlevel03FeatureSetBuilder.ArtilleryModePool,
                ArtilleryPoolIncreaseGui.Build()
                ).AddToDB();

            GuiPresentationBuilder guiPresentationFlameArtillery15 = new GuiPresentationBuilder(
                "Feature/&FlameArtillery_15ModePowerDescription",
                "Feature/&FlameArtillery_15ModePowerTitle");
            guiPresentationFlameArtillery15.SetSpriteReference(DatabaseHelper.SpellDefinitions.BurningHands.GuiPresentation.SpriteReference);

            EffectDescriptionBuilder effectFlameArtillerymode15 = new EffectDescriptionBuilder();
            effectFlameArtillerymode15.SetDurationData(RuleDefinitions.DurationType.Hour, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            effectFlameArtillerymode15.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Distance, 1, RuleDefinitions.TargetType.Position, 1, 1, ActionDefinitions.ItemSelectionType.Equiped);
            effectFlameArtillerymode15.AddEffectForm(new EffectFormBuilder().SetSummonForm(SummonForm.Type.Creature, ScriptableObject.CreateInstance<ItemDefinition>(), 1, FlameArtilleryConstruct_15Builder.FlameArtilleryConstruct_15.name, null, true, null, ScriptableObject.CreateInstance<EffectProxyDefinition>()).Build());

            FlameArtillery_15modepower = new FeatureDefinitionPowerSharedPoolBuilder
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
            FlameArtillery_15modepower.SetOverriddenPower(ArtilleryConstructlevel09FeatureSetBuilder.FlameArtillery_09modepower);

            GuiPresentationBuilder guiPresentationForceArtillery15 = new GuiPresentationBuilder(
                "Feature/&ForceArtillery_15ModePowerDescription",
                "Feature/&ForceArtillery_15ModePowerTitle");
            guiPresentationForceArtillery15.SetSpriteReference(DatabaseHelper.SpellDefinitions.MagicMissile.GuiPresentation.SpriteReference);

            EffectDescriptionBuilder effectForceArtillerymode15 = new EffectDescriptionBuilder();
            effectForceArtillerymode15.SetDurationData(RuleDefinitions.DurationType.Hour, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            effectForceArtillerymode15.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Distance, 1, RuleDefinitions.TargetType.Position, 1, 1, ActionDefinitions.ItemSelectionType.Equiped);
            effectForceArtillerymode15.AddEffectForm(new EffectFormBuilder().SetSummonForm(SummonForm.Type.Creature, ScriptableObject.CreateInstance<ItemDefinition>(), 1, ForceArtilleryConstruct_15Builder.ForceArtilleryConstruct_15.name, null, true, null, ScriptableObject.CreateInstance<EffectProxyDefinition>()).Build());

            ForceArtillery_15modepower = new FeatureDefinitionPowerSharedPoolBuilder
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
            ForceArtillery_15modepower.SetOverriddenPower(ArtilleryConstructlevel09FeatureSetBuilder.ForceArtillery_09modepower);

            GuiPresentationBuilder guiPresentationTempHPShield15 = new GuiPresentationBuilder(
                "Feature/&TempHPShield_15ModePowerDescription",
                "Feature/&TempHPShield_15ModePowerTitle");
            guiPresentationTempHPShield15.SetSpriteReference(DatabaseHelper.SpellDefinitions.Aid.GuiPresentation.SpriteReference);

            EffectDescriptionBuilder effectTempHPShieldmode15 = new EffectDescriptionBuilder();
            effectTempHPShieldmode15.SetDurationData(RuleDefinitions.DurationType.Hour, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            effectTempHPShieldmode15.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Distance, 1, RuleDefinitions.TargetType.Position, 1, 1, ActionDefinitions.ItemSelectionType.Equiped);
            effectTempHPShieldmode15.AddEffectForm(new EffectFormBuilder().SetSummonForm(SummonForm.Type.Creature, ScriptableObject.CreateInstance<ItemDefinition>(), 1, TempHPShieldConstruct_15Builder.TempHPShieldConstruct_15.name, null, true, null, ScriptableObject.CreateInstance<EffectProxyDefinition>()).Build());

            TempHPShield_15modepower = new FeatureDefinitionPowerSharedPoolBuilder
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
            TempHPShield_15modepower.SetOverriddenPower(ArtilleryConstructlevel09FeatureSetBuilder.TempHPShield_09modepower);

            Definition.FeatureSet.Clear();
            Definition.FeatureSet.Add(ArtilleryPoolIncrease);
            Definition.FeatureSet.Add(TempHPShield_15modepower);
            Definition.FeatureSet.Add(FlameArtillery_15modepower);
            Definition.FeatureSet.Add(ForceArtillery_15modepower);
        }

        public static FeatureDefinitionFeatureSet CreateAndAddToDB(string name, string guid)
        {
            return new ArtilleryConstructlevel15FeatureSetBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionFeatureSet ArtilleryConstructlevel15FeatureSet = CreateAndAddToDB(Name, Guid);
    }

    //*****************************************************************************************************************************************
    //***********************************		SelfDestructBuilder		*******************************************************************
    //*****************************************************************************************************************************************

    internal class SelfDestructBuilder : BaseDefinitionBuilder<FeatureDefinitionPower>
    {
        private const string SelfDestructName = "SelfDestruct";
        private const string SelfDestructGuid = "68ecf3d9-f718-4c62-921c-f30df3708312";

        protected SelfDestructBuilder(string name, string guid) : base(ThunderShieldBuilder.ThunderShield, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&SelfDestructTitle";
            Definition.GuiPresentation.Description = "Feat/&SelfDestructDescription";
            Definition.GuiPresentation.SetSpriteReference(DatabaseHelper.SpellDefinitions.FlamingSphere.GuiPresentation.SpriteReference);
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
                DamageForm = (ExplosionDamage)
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
            newEffectDescription.SetEffectParticleParameters(DatabaseHelper.SpellDefinitions.Fireball.EffectDescription.EffectParticleParameters);

            Definition.SetUniqueInstance(true);
            Definition.SetEffectDescription(newEffectDescription);
        }

        public static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
        {
            return new SelfDestructBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionPower SelfDestruct = CreateAndAddToDB(SelfDestructName, SelfDestructGuid);
    }

    //*****************************************************************************************************************************************
    //***********************************		SelfDestructionConditionBuilder		*******************************************************************
    //*****************************************************************************************************************************************

    internal class SelfDestructionConditionBuilder : BaseDefinitionBuilder<ConditionDefinition>
    {
        private const string SelfDestructionConditionName = "SelfDestructionCondition";
        private const string SelfDestructionConditionGuid = "c96984f4-0370-4775-ab8b-3ee6db0c8806";

        protected SelfDestructionConditionBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionDummy, name, guid)
        //protected SelfDestructionConditionBuilder(string name, string guid) : base( name, guid)
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

        public static ConditionDefinition CreateAndAddToDB(string name, string guid)
        {
            return new SelfDestructionConditionBuilder(name, guid).AddToDB();
        }

        public static ConditionDefinition SelfDestructionCondition = CreateAndAddToDB(SelfDestructionConditionName, SelfDestructionConditionGuid);
    }

    //*****************************************************************************************************************************************
    //***********************************		HalfCoverShieldBuilder		*******************************************************************
    //*****************************************************************************************************************************************

    internal class HalfCoverShieldBuilder : BaseDefinitionBuilder<FeatureDefinitionPower>
    {
        private const string HalfCoverShieldName = "HalfCoverShield";
        private const string HalfCoverShieldGuid = "0fd3ccb3-0f15-4766-8eb8-32042838ce6d";

        protected HalfCoverShieldBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionPowers.PowerDomainBattleHeraldOfBattle, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&HalfCoverShieldTitle";
            Definition.GuiPresentation.Description = "Feat/&HalfCoverShieldDescription";
            Definition.GuiPresentation.SetSpriteReference(DatabaseHelper.SpellDefinitions.Shield.GuiPresentation.SpriteReference);

            EffectForm HalfCoverShield = new EffectForm
            {
                ConditionForm = new ConditionForm(),
                FormType = EffectForm.EffectFormType.Condition
            };
            HalfCoverShield.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
            HalfCoverShield.ConditionForm.SetConditionDefinitionName(HalfCoverShieldConditionBuilder.HalfCoverShieldCondition.Name);
            HalfCoverShield.ConditionForm.ConditionDefinition = HalfCoverShieldConditionBuilder.HalfCoverShieldCondition;// DistractingPulseBuilder.DistractingPulse;

            HalfCoverShield.SetCreatedByCharacter(true);

            HalfCoverShield.AddBonusMode = RuleDefinitions.AddBonusMode.AbilityBonus;
            HalfCoverShield.SetLevelMultiplier(1);
            HalfCoverShield.SetLevelType(RuleDefinitions.LevelSourceType.EffectLevel);
            HalfCoverShield.SetApplyLevel(EffectForm.LevelApplianceType.No);

            Definition.EffectDescription.EffectAdvancement.Clear();
            Definition.EffectDescription.EffectForms.Clear();
            Definition.EffectDescription.EffectForms.Add(HalfCoverShield);
            Definition.EffectDescription.SetTargetType(RuleDefinitions.TargetType.Sphere);
            Definition.EffectDescription.SetTargetParameter(2);
            Definition.EffectDescription.SetTargetSide(RuleDefinitions.Side.Ally);
            Definition.EffectDescription.SetRangeParameter(2);
            Definition.EffectDescription.HasSavingThrow = false;
            Definition.EffectDescription.SavingThrowAbility = DatabaseHelper.SmartAttributeDefinitions.Dexterity.Name;

            Definition.EffectDescription.SetCreatedByCharacter(true);
            Definition.EffectDescription.SetCanBePlacedOnCharacter(true);
            Definition.EffectDescription.SetRangeType(RuleDefinitions.RangeType.Self);
            Definition.EffectDescription.SetEffectParticleParameters(DatabaseHelper.SpellDefinitions.Shield.EffectDescription.EffectParticleParameters);
        }

        public static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
        {
            return new HalfCoverShieldBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionPower HalfCoverShield = CreateAndAddToDB(HalfCoverShieldName, HalfCoverShieldGuid);
    }

    //*****************************************************************************************************************************************
    //***********************************		HalfCoverShieldConditionBuilder		*******************************************************************
    //*****************************************************************************************************************************************

    internal class HalfCoverShieldConditionBuilder : BaseDefinitionBuilder<ConditionDefinition>
    {
        private const string HalfCoverShieldConditionName = "HalfCoverShieldCondition";
        private const string HalfCoverShieldConditionGuid = "c160a20e-4714-478a-8bbb-6df5526728a9";

        protected HalfCoverShieldConditionBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionHeraldOfBattle, name, guid)
        {
            Definition.GuiPresentation.Title = "Rules/&HalfCoverShieldConditionTitle";
            Definition.GuiPresentation.Description = "Rules/&HalfCoverShieldConditionDescription";

            Definition.Features.Clear();
            Definition.Features.Add(HalfCoverShieldAttributeBuilder.HalfCoverShieldAttribute);
        }

        public static ConditionDefinition CreateAndAddToDB(string name, string guid)
        {
            return new HalfCoverShieldConditionBuilder(name, guid).AddToDB();
        }

        public static ConditionDefinition HalfCoverShieldCondition = CreateAndAddToDB(HalfCoverShieldConditionName, HalfCoverShieldConditionGuid);
    }

    //*****************************************************************************************************************************************
    //***********************************		HalfCoverShieldAttributeBuilder		*******************************************************************
    //*****************************************************************************************************************************************

    internal class HalfCoverShieldAttributeBuilder : BaseDefinitionBuilder<FeatureDefinitionAttributeModifier>
    {
        private const string HalfCoverShieldAttributeName = "HalfCoverShieldAttribute";
        private const string HalfCoverShieldAttributeGuid = "4dd6548f-4d7f-4bbf-b120-70a17f79f8e0";

        protected HalfCoverShieldAttributeBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierHeraldOfBattle, name, guid)
        {
            Definition.GuiPresentation.Title = "Rules/&HalfCoverShieldAttributeTitle";
            Definition.GuiPresentation.Description = "Rules/&HalfCoverShieldAttributeDescription";

            Definition.SetModifiedAttribute(DatabaseHelper.SmartAttributeDefinitions.ArmorClass.Name);
            Definition.SetModifierType2(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive);
            Definition.SetModifierValue(2);
        }

        public static FeatureDefinitionAttributeModifier CreateAndAddToDB(string name, string guid)
        {
            return new HalfCoverShieldAttributeBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionAttributeModifier HalfCoverShieldAttribute = CreateAndAddToDB(HalfCoverShieldAttributeName, HalfCoverShieldAttributeGuid);
    }

    //*****************************************************************************************************************************************
    //***********************************		SummonArtillerySpellConstructBuilder		*******************************************************************
    //*****************************************************************************************************************************************

    internal class SummonArtillerySpellConstructBuilder : BaseDefinitionBuilder<SpellDefinition>
    {
        private const string SummonArtillerySpellConstructName = "SummonArtillerySpellConstruct";
        private const string SummonArtillerySpellConstructGuid = "214dab9d-f40e-424f-8730-b41acdae26ec";

        protected SummonArtillerySpellConstructBuilder(string name, string guid) : base(DatabaseHelper.SpellDefinitions.DancingLights, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&ResummonArtilleryConstruct_03Title";
            Definition.GuiPresentation.Description = "Feat/&ResummonArtilleryConstructDescription";
            Definition.GuiPresentation.SetSpriteReference(DatabaseHelper.SpellDefinitions.FaerieFire.GuiPresentation.SpriteReference);

            Definition.SetSpellLevel(1);
            Definition.SetRequiresConcentration(false);
            Definition.SetUniqueInstance(true);
            Definition.SetCastingTime(RuleDefinitions.ActivationTime.Action);

            Definition.SetSpellsBundle(true);
            Definition.SubspellsList.AddRange(new List<SpellDefinition>
            {
                SummonFlameArtillerySpellConstructBuilder.SummonFlameArtilleryConstruct ,
               SummonForceArtillerySpellConstructBuilder.SummonForceArtilleryConstruct,
                SummonTempHPShieldSpellConstructBuilder.SummonTempHPShieldConstruct
            });

            Definition.EffectDescription.Clear();
        }

        public static SpellDefinition CreateAndAddToDB(string name, string guid)
        {
            return new SummonArtillerySpellConstructBuilder(name, guid).AddToDB();
        }

        public static SpellDefinition SummonArtillerySpellConstruct = CreateAndAddToDB(SummonArtillerySpellConstructName, SummonArtillerySpellConstructGuid);
    }

    //*****************************************************************************************************************************************
    //***********************************		SummonArtillerySpellConstruct_9Builder		*******************************************************************
    //*****************************************************************************************************************************************

    internal class SummonArtillerySpellConstruct_9Builder : BaseDefinitionBuilder<SpellDefinition>
    {
        private const string SummonArtillerySpellConstruct_9Name = "SummonArtillerySpellConstruct_9";
        private const string SummonArtillerySpellConstruct_9Guid = "9ad29b43-5207-46e8-bbc9-7b8138b2912f";

        protected SummonArtillerySpellConstruct_9Builder(string name, string guid) : base(SummonArtillerySpellConstructBuilder.SummonArtillerySpellConstruct, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&ResummonArtilleryConstruct_09Title";
            Definition.GuiPresentation.Description = "Feat/&ResummonArtilleryConstructDescription";

            Definition.SubspellsList.Clear();
            Definition.SubspellsList.AddRange(new List<SpellDefinition>
            {
                SummonFlameArtillerySpellConstruct_9Builder.SummonFlameArtilleryConstruct_9 ,
               SummonForceArtillerySpellConstruct_9Builder.SummonForceArtilleryConstruct_9,
                SummonTempHPShieldSpellConstruct_9Builder.SummonTempHPShieldConstruct_9
            });
        }

        public static SpellDefinition CreateAndAddToDB(string name, string guid)
        {
            return new SummonArtillerySpellConstruct_9Builder(name, guid).AddToDB();
        }

        public static SpellDefinition SummonArtillerySpellConstruct_9 = CreateAndAddToDB(SummonArtillerySpellConstruct_9Name, SummonArtillerySpellConstruct_9Guid);
    }

    //*****************************************************************************************************************************************
    //***********************************		SummonArtillerySpellConstruct_15Builder		*******************************************************************
    //*****************************************************************************************************************************************

    internal class SummonArtillerySpellConstruct_15Builder : BaseDefinitionBuilder<SpellDefinition>
    {
        private const string SummonArtillerySpellConstruct_15Name = "SummonArtillerySpellConstruct_15";
        private const string SummonArtillerySpellConstruct_15Guid = "f0038039-423b-4265-b0cf-2eab2450f982";

        protected SummonArtillerySpellConstruct_15Builder(string name, string guid) : base(SummonArtillerySpellConstructBuilder.SummonArtillerySpellConstruct, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&ResummonArtilleryConstruct_15Title";
            Definition.GuiPresentation.Description = "Feat/&ResummonArtilleryConstructDescription";

            Definition.SubspellsList.Clear();
            Definition.SubspellsList.AddRange(new List<SpellDefinition>
            {
                SummonFlameArtillerySpellConstruct_15Builder.SummonFlameArtilleryConstruct_15 ,
               SummonForceArtillerySpellConstruct_15Builder.SummonForceArtilleryConstruct_15,
                SummonTempHPShieldSpellConstruct_15Builder.SummonTempHPShieldConstruct_15
            });
        }

        public static SpellDefinition CreateAndAddToDB(string name, string guid)
        {
            return new SummonArtillerySpellConstruct_15Builder(name, guid).AddToDB();
        }

        public static SpellDefinition SummonArtillerySpellConstruct_15 = CreateAndAddToDB(SummonArtillerySpellConstruct_15Name, SummonArtillerySpellConstruct_15Guid);
    }

    internal class SummoningAffinityTinkererArtilleryConstructBuilder : BaseDefinitionBuilder<FeatureDefinitionSummoningAffinity>
    {
        private const string Name = "SummoningAffinityTinkererArtilleryConstruct";
        private const string Guid = "edae2ec0-f871-4918-855a-117bd428d51e";

        protected SummoningAffinityTinkererArtilleryConstructBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionSummoningAffinitys.SummoningAffinityKindredSpiritBond, name, guid)
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
            Definition.AddedConditions.AddRange(new List<ConditionDefinition>
            {
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
            DatabaseHelper.ConditionDefinitions.ConditionKindredSpiritBondMeleeAttack,
            //DatabaseHelper.ConditionDefinitions.ConditionKindredSpiritBondMeleeDamage,
            //DatabaseHelper.ConditionDefinitions.ConditionKindredSpiritBondSkillProficiency,
            //DatabaseHelper.ConditionDefinitions.ConditionKindredSpiritBondSavingThrows
            });
        }

        public static FeatureDefinitionSummoningAffinity CreateAndAddToDB(string name, string guid)
        {
            return new SummoningAffinityTinkererArtilleryConstructBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionSummoningAffinity SummoningAffinityTinkererArtilleryConstruct = CreateAndAddToDB(Name, Guid);
    }
}
