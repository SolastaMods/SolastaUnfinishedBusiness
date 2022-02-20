using System;
using System.Collections.Generic;
using SolastaModApi;
using SolastaCommunityExpansion.Builders;


using UnityEngine;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Classes.Warlock.Features
{
    internal class AHWarlockClassPactBoonSetBuilder : BaseDefinitionBuilder<FeatureDefinitionFeatureSet>
    {
        const string AHWarlockClassPactBoonSetName = "AHWarlockClassPactBoonSet";
        private static readonly string AHWarlockClassPactBoonSetGuid = GuidHelper.Create(new Guid(Settings.GUID), AHWarlockClassPactBoonSetName).ToString();

        [Obsolete]
        protected AHWarlockClassPactBoonSetBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetHunterHuntersPrey, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&AHWarlockClassPactBoonSetTitle";
            Definition.GuiPresentation.Description = "Feature/&AHWarlockClassPactBoonSetDescription";

            Definition.FeatureSet.Clear();
            Definition.FeatureSet.Add(AHWarlockClassPactOfTheBladeSetBuilder.AHWarlockClassPactOfTheBladeSet);
            Definition.FeatureSet.Add(DHWarlockClassPactOfTheChainFeatureSetBuilder.DHWarlockClassPactOfTheChainFeatureSet);
            Definition.FeatureSet.Add(DHPactOfTheTomeFeatureSetBuilder.DHPactOfTheTomeFeatureSet);
            Definition.SetUniqueChoices(true);
            Definition.SetEnumerateInDescription(true);
        }

        [Obsolete]
        public static FeatureDefinitionFeatureSet CreateAndAddToDB(string name, string guid)
            => new AHWarlockClassPactBoonSetBuilder(name, guid).AddToDB();

        [Obsolete]
        public static FeatureDefinitionFeatureSet AHWarlockClassPactBoonSet = CreateAndAddToDB(AHWarlockClassPactBoonSetName, AHWarlockClassPactBoonSetGuid);
    }

    internal class AHWarlockClassPactOfTheBladeSetBuilder : BaseDefinitionBuilder<FeatureDefinitionFeatureSet>
    {
        const string AHWarlockClassPactOfTheBladeSetName = "AHWarlockClassPactOfTheBladeSet";
        private static readonly string AHWarlockClassPactOfTheBladeSetGuid = GuidHelper.Create(new Guid(Settings.GUID), AHWarlockClassPactOfTheBladeSetName).ToString();

        [Obsolete]
        protected AHWarlockClassPactOfTheBladeSetBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetHunterHuntersPrey, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&AHWarlockClassPactOfTheBladeSetTitle";
            Definition.GuiPresentation.Description = "Feature/&AHWarlockClassPactOfTheBladeSetDescription";

            Definition.FeatureSet.Clear();
            Definition.FeatureSet.Add(DatabaseHelper.FeatureDefinitionProficiencys.ProficiencyFighterWeapon);
            Definition.FeatureSet.Add(DatabaseHelper.FeatureDefinitionAttackModifiers.AttackModifierMartialSpellBladeMagicWeapon);
            Definition.SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union);
            Definition.SetUniqueChoices(false);
        }

        [Obsolete]
        public static FeatureDefinitionFeatureSet CreateAndAddToDB(string name, string guid)
            => new AHWarlockClassPactOfTheBladeSetBuilder(name, guid).AddToDB();

        [Obsolete]
        public static FeatureDefinitionFeatureSet AHWarlockClassPactOfTheBladeSet = CreateAndAddToDB(AHWarlockClassPactOfTheBladeSetName, AHWarlockClassPactOfTheBladeSetGuid);
    }

    internal class DHWarlockClassPactOfTheChainFeatureSetBuilder : BaseDefinitionBuilder<FeatureDefinitionFeatureSet>
    {
        const string DHWarlockClassPactOfTheChainFeatureSetName = "DHWarlockClassPactOfTheChainFeatureSet";
        private static readonly string DHWarlockClassPactOfTheChainFeatureSetGuid = GuidHelper.Create(new Guid(Settings.GUID), DHWarlockClassPactOfTheChainFeatureSetName).ToString();

        [Obsolete]
        protected DHWarlockClassPactOfTheChainFeatureSetBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetGreenmageWardenOfTheForest, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&DHWarlockClassPactOfTheChainFeatureSetTitle";
            Definition.GuiPresentation.Description = "Feature/&DHWarlockClassPactOfTheChainFeatureSetDescription";

            Definition.featureSet.Clear();

            // maybe could be merged with the witchs find familiar

            WarlockPactOfTheChainSummons.PactofChainFamiliarAuraOfSpellResistence();
            WarlockPactOfTheChainSummons.buildPactofChainFamiliarInvisibilityPower();
            WarlockPactOfTheChainSummons.buildCustomPseudodragon();
            WarlockPactOfTheChainSummons.buildCustomSprite();
            WarlockPactOfTheChainSummons.buildCustomImp();
            WarlockPactOfTheChainSummons.buildCustomQuasit();


            EffectDescriptionBuilder effectDescriptionPseudodragon = new EffectDescriptionBuilder();
            effectDescriptionPseudodragon.SetDurationData(RuleDefinitions.DurationType.UntilLongRest, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            effectDescriptionPseudodragon.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Distance, 1, RuleDefinitions.TargetType.Position, 1, 1, ActionDefinitions.ItemSelectionType.Equiped);
            effectDescriptionPseudodragon.AddEffectForm(new EffectFormBuilder().SetSummonForm(SummonForm.Type.Creature, ScriptableObject.CreateInstance<ItemDefinition>(), 1, WarlockPactOfTheChainSummons.PactChainPseudodragon.name, DatabaseHelper.ConditionDefinitions.ConditionFlyingBootsWinged, true, null, ScriptableObject.CreateInstance<EffectProxyDefinition>()).Build());
            effectDescriptionPseudodragon.SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.ConjureElementalAir.EffectDescription.EffectParticleParameters);



            GuiPresentationBuilder FindFamiliarPsuedodragonGui = new GuiPresentationBuilder(
                "Spell/&FindFamiliarPsuedodragonDescription",
                "Spell/&FindFamiliarPsuedodragonTitle");
            FindFamiliarPsuedodragonGui.SetSpriteReference(WarlockPactOfTheChainSummons.PactChainPseudodragon.GuiPresentation.SpriteReference);


            EffectDescriptionBuilder effectDescriptionSprite = new EffectDescriptionBuilder();
            effectDescriptionSprite.SetDurationData(RuleDefinitions.DurationType.UntilLongRest, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            effectDescriptionSprite.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Distance, 1, RuleDefinitions.TargetType.Position, 1, 1, ActionDefinitions.ItemSelectionType.Equiped);
            effectDescriptionSprite.AddEffectForm(new EffectFormBuilder().SetSummonForm(SummonForm.Type.Creature, ScriptableObject.CreateInstance<ItemDefinition>(), 1, WarlockPactOfTheChainSummons.PactChainSprite.name, DatabaseHelper.ConditionDefinitions.ConditionFlyingBootsWinged, true, null, ScriptableObject.CreateInstance<EffectProxyDefinition>()).Build());
            effectDescriptionSprite.SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.ConjureElementalAir.EffectDescription.EffectParticleParameters);

            GuiPresentationBuilder FindFamiliarSpriteGui = new GuiPresentationBuilder(
                "Spell/&FindFamiliarSpriteDescription",
                "Spell/&FindFamiliarSpriteTitle");
            FindFamiliarSpriteGui.SetSpriteReference(WarlockPactOfTheChainSummons.PactChainSprite.GuiPresentation.SpriteReference);


            EffectDescriptionBuilder effectDescriptionImp = new EffectDescriptionBuilder();
            effectDescriptionImp.SetDurationData(RuleDefinitions.DurationType.UntilLongRest, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            effectDescriptionImp.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Distance, 1, RuleDefinitions.TargetType.Position, 1, 1, ActionDefinitions.ItemSelectionType.Equiped);
            effectDescriptionImp.AddEffectForm(new EffectFormBuilder().SetSummonForm(SummonForm.Type.Creature, ScriptableObject.CreateInstance<ItemDefinition>(), 1, WarlockPactOfTheChainSummons.PactChainImp.name, DatabaseHelper.ConditionDefinitions.ConditionFlyingBootsWinged, true, null, ScriptableObject.CreateInstance<EffectProxyDefinition>()).Build());
            effectDescriptionImp.SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.ConjureElementalAir.EffectDescription.EffectParticleParameters);


            GuiPresentationBuilder FindFamiliarImpGui = new GuiPresentationBuilder(
                "Spell/&FindFamiliarImpDescription",
                "Spell/&FindFamiliarImpTitle");
            FindFamiliarImpGui.SetSpriteReference(WarlockPactOfTheChainSummons.PactChainImp.GuiPresentation.spriteReference);

            EffectDescriptionBuilder effectDescriptionQuasit = new EffectDescriptionBuilder();
            effectDescriptionQuasit.SetDurationData(RuleDefinitions.DurationType.UntilLongRest, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            effectDescriptionQuasit.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Distance, 1, RuleDefinitions.TargetType.Position, 1, 1, ActionDefinitions.ItemSelectionType.Equiped);
            effectDescriptionQuasit.AddEffectForm(new EffectFormBuilder().SetSummonForm(SummonForm.Type.Creature, ScriptableObject.CreateInstance<ItemDefinition>(), 1, WarlockPactOfTheChainSummons.PactChainQuasit.name, DatabaseHelper.ConditionDefinitions.ConditionFlyingBootsWinged, true, null, ScriptableObject.CreateInstance<EffectProxyDefinition>()).Build());
            effectDescriptionQuasit.SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.ConjureElementalAir.EffectDescription.EffectParticleParameters);


            GuiPresentationBuilder FindFamiliarQuasitGui = new GuiPresentationBuilder(
                "Spell/&FindFamiliarQuasitDescription",
                "Spell/&FindFamiliarQuasitTitle");
            FindFamiliarQuasitGui.SetSpriteReference(WarlockPactOfTheChainSummons.PactChainQuasit.GuiPresentation.SpriteReference);


            var FindFamiliarImpPowerBuilder = new FeatureDefinitionPowerBuilder(
                 "FindFamiliarImpPower",
                 GuidHelper.Create(new Guid(Settings.GUID), "FindFamiliarImpPower").ToString(),
                 1,
                 RuleDefinitions.UsesDetermination.Fixed,
                 AttributeDefinitions.Charisma,
                 RuleDefinitions.ActivationTime.Hours1,
                 1,
                 RuleDefinitions.RechargeRate.AtWill,
                 false,
                 false,
                 AttributeDefinitions.Charisma,
                 effectDescriptionImp.Build(),
                 FindFamiliarImpGui.Build(),
                 true);
            FeatureDefinitionPower FindFamiliarImpPower = FindFamiliarImpPowerBuilder.AddToDB();



            var FindFamiliarPseudodragonPowerBuilder = new FeatureDefinitionPowerBuilder(
                 "FindFamiliarPseudodragonPower",
                 GuidHelper.Create(new Guid(Settings.GUID), "FindFamiliarPseudodragonPower").ToString(),
                 1,
                 RuleDefinitions.UsesDetermination.Fixed,
                 AttributeDefinitions.Charisma,
                 RuleDefinitions.ActivationTime.Hours1,
                 1,
                 RuleDefinitions.RechargeRate.AtWill,
                 false,
                 false,
                 AttributeDefinitions.Charisma,
                 effectDescriptionPseudodragon.Build(),
                 FindFamiliarPsuedodragonGui.Build(),
                 true);
            FeatureDefinitionPower FindFamiliarPseudodragonPower = FindFamiliarPseudodragonPowerBuilder.AddToDB();



            var FindFamiliarSpritePowerBuilder = new FeatureDefinitionPowerBuilder(
                 "FindFamiliarSpritePower",
                 GuidHelper.Create(new Guid(Settings.GUID), "FindFamiliarSpritePower").ToString(),
                 1,
                 RuleDefinitions.UsesDetermination.Fixed,
                 AttributeDefinitions.Charisma,
                 RuleDefinitions.ActivationTime.Hours1,
                 1,
                 RuleDefinitions.RechargeRate.AtWill,
                 false,
                 false,
                 AttributeDefinitions.Charisma,
                 effectDescriptionSprite.Build(),
                 FindFamiliarSpriteGui.Build(),
                 true);
            FeatureDefinitionPower FindFamiliarSpritePower = FindFamiliarSpritePowerBuilder.AddToDB();


            var FindFamiliarQuasitPowerBuilder = new FeatureDefinitionPowerBuilder(
                 "FindFamiliarQuasitPower",
                 GuidHelper.Create(new Guid(Settings.GUID), "FindFamiliarQuasitPower").ToString(),
                 1,
                 RuleDefinitions.UsesDetermination.Fixed,
                 AttributeDefinitions.Charisma,
                 RuleDefinitions.ActivationTime.Hours1,
                 1,
                 RuleDefinitions.RechargeRate.AtWill,
                 false,
                 false,
                 AttributeDefinitions.Charisma,
                 effectDescriptionQuasit.Build(),
                 FindFamiliarQuasitGui.Build(),
                 true);
             FeatureDefinitionPower FindFamiliarQuasitPower = FindFamiliarQuasitPowerBuilder.AddToDB();


            Definition.featureSet.Add(FindFamiliarPseudodragonPower);
            Definition.featureSet.Add(FindFamiliarSpritePower);
            Definition.featureSet.Add(FindFamiliarImpPower);
            Definition.featureSet.Add(FindFamiliarQuasitPower);

            Definition.mode = FeatureDefinitionFeatureSet.FeatureSetMode.Union;



        }

        public static FeatureDefinitionFeatureSet CreateAndAddToDB(string name, string guid)
            => new DHWarlockClassPactOfTheChainFeatureSetBuilder(name, guid).AddToDB();

        public static FeatureDefinitionFeatureSet DHWarlockClassPactOfTheChainFeatureSet = CreateAndAddToDB(DHWarlockClassPactOfTheChainFeatureSetName, DHWarlockClassPactOfTheChainFeatureSetGuid);
    }


    internal class DHPactOfTheTomeFeatureSetBuilder : BaseDefinitionBuilder<FeatureDefinitionFeatureSet>
    {
        private const string DHPactOfTheTomeFeatureSetName = "DHPactOfTheTomeFeatureSet";
        private static readonly string DHPactOfTheTomeFeatureSetGuid = GuidHelper.Create(new Guid(Settings.GUID), DHPactOfTheTomeFeatureSetName).ToString();

        protected DHPactOfTheTomeFeatureSetBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetGreenmageWardenOfTheForest, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&DHPactOfTheTomeFeatureSetTitle";
            Definition.GuiPresentation.Description = "Feature/&DHPactOfTheTomeFeatureSetDescription";


            Definition.featureSet.Clear();
            Definition.featureSet.Add(DHPactOfTheTomeBonusCantripsBuilder.DHPactOfTheTomeBonusCantrips);
            Definition.featureSet.Add(DHPactOfTheTomeMagicAffinityBuilder.DHPactOfTheTomeMagicAffinity);

        }

        public static FeatureDefinitionFeatureSet CreateAndAddToDB(string name, string guid)
        {
            return new DHPactOfTheTomeFeatureSetBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionFeatureSet DHPactOfTheTomeFeatureSet = CreateAndAddToDB(DHPactOfTheTomeFeatureSetName, DHPactOfTheTomeFeatureSetGuid);
    }
    internal class DHPactOfTheTomeBonusCantripsBuilder : BaseDefinitionBuilder<FeatureDefinitionPointPool>
    {
        private const string PactOfTheTomeBonusCantripsName = "DHPactOfTheTomeBonusCantrips";
        private static readonly string PactOfTheTomeBonusCantripsGuid = GuidHelper.Create(new Guid(Settings.GUID), PactOfTheTomeBonusCantripsName).ToString();

        protected DHPactOfTheTomeBonusCantripsBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionPointPools.PointPoolCircleLandBonusCantrip, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&NoContentTitle";
            Definition.GuiPresentation.Description = "Feature/&NoContentTitle";
            Definition.restrictedChoices.Clear();

            Definition.poolType = HeroDefinitions.PointsPoolType.Cantrip;
            Definition.poolAmount = 4;
        }

        public static FeatureDefinitionPointPool CreateAndAddToDB(string name, string guid)
        {
            return new DHPactOfTheTomeBonusCantripsBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionPointPool DHPactOfTheTomeBonusCantrips = CreateAndAddToDB(PactOfTheTomeBonusCantripsName, PactOfTheTomeBonusCantripsGuid);
    }


    internal class DHPactOfTheTomeSpellListBuilder : BaseDefinitionBuilder<SpellListDefinition>
    {
        private const string PactOfTheTomeSpellListName = "DHPactOfTheTomeSpellList";
        private static readonly string PactOfTheTomeSpellListGuid = GuidHelper.Create(new Guid(Settings.GUID), PactOfTheTomeSpellListName).ToString();

        protected DHPactOfTheTomeSpellListBuilder(string name, string guid) : base(DatabaseHelper.SpellListDefinitions.SpellListKythaela_Cantrips, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&NoContentTitle";
            Definition.GuiPresentation.Description = "Feature/&NoContentTitle";

            Definition.SpellsByLevel[0].Spells.Clear();

            Definition.SpellsByLevel[0].Spells.AddRange(DatabaseHelper.SpellListDefinitions.SpellListCleric.SpellsByLevel[0].Spells);
            Definition.SpellsByLevel[0].Spells.AddRange(DatabaseHelper.SpellListDefinitions.SpellListDruid.SpellsByLevel[0].Spells);
            Definition.SpellsByLevel[0].Spells.AddRange(DatabaseHelper.SpellListDefinitions.SpellListDruid.SpellsByLevel[0].Spells);
            Definition.SpellsByLevel[0].Spells.AddRange(DatabaseHelper.SpellListDefinitions.SpellListSorcerer.SpellsByLevel[0].Spells);

        }

        public static SpellListDefinition CreateAndAddToDB(string name, string guid)
        {
            return new DHPactOfTheTomeSpellListBuilder(name, guid).AddToDB();
        }

        public static SpellListDefinition DHPactOfTheTomeSpellList = CreateAndAddToDB(PactOfTheTomeSpellListName, PactOfTheTomeSpellListGuid);
    }


    internal class DHPactOfTheTomeMagicAffinityBuilder : BaseDefinitionBuilder<FeatureDefinitionMagicAffinity>
    {
        private const string PactOfTheTomeMagicAffinityName = "DHPactOfTheTomeMagicAffinity";
        private static readonly string PactOfTheTomeMagicAffinityGuid = GuidHelper.Create(new Guid(Settings.GUID), PactOfTheTomeMagicAffinityName).ToString();

        protected DHPactOfTheTomeMagicAffinityBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionMagicAffinitys.MagicAffinityGreenmageGreenMagicList, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&NoContentTitle";
            Definition.GuiPresentation.Description = "Feature/&NoContentTitle";

            Definition.extendedSpellList = DHPactOfTheTomeSpellListBuilder.DHPactOfTheTomeSpellList;
        }

        public static FeatureDefinitionMagicAffinity CreateAndAddToDB(string name, string guid)
        {
            return new DHPactOfTheTomeMagicAffinityBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionMagicAffinity DHPactOfTheTomeMagicAffinity = CreateAndAddToDB(PactOfTheTomeMagicAffinityName, PactOfTheTomeMagicAffinityGuid);
    }



}
