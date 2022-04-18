using System;
using System.Collections.Generic;
using SolastaModApi;
using SolastaCommunityExpansion.Builders;
using UnityEngine;
using SolastaModApi.Extensions;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Classes.Warlock.Features
{
    internal class WarlockClassPactBoonSetBuilder : FeatureDefinitionFeatureSetBuilder
    {
        private const string WarlockClassPactBoonSetName = "WarlockClassPactBoonSet";

        protected WarlockClassPactBoonSetBuilder(string name) : base(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetHunterHuntersPrey, name, DefinitionBuilder.CENamespaceGuid)
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

        internal static FeatureDefinitionFeatureSet CreateAndAddToDB(string name)
            => new WarlockClassPactBoonSetBuilder(name).AddToDB();

        internal static readonly FeatureDefinitionFeatureSet WarlockClassPactBoonSet = CreateAndAddToDB(WarlockClassPactBoonSetName);
    }

    internal class AHWarlockClassPactOfTheBladeSetBuilder : FeatureDefinitionFeatureSetBuilder
    {
        private const string AHWarlockClassPactOfTheBladeSetName = "AHWarlockClassPactOfTheBladeSet";

        protected AHWarlockClassPactOfTheBladeSetBuilder(string name) : base(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetHunterHuntersPrey, name, DefinitionBuilder.CENamespaceGuid)
        {
            Definition.GuiPresentation.Title = "Feature/&AHWarlockClassPactOfTheBladeSetTitle";
            Definition.GuiPresentation.Description = "Feature/&AHWarlockClassPactOfTheBladeSetDescription";

            Definition.FeatureSet.Clear();
            Definition.FeatureSet.Add(DatabaseHelper.FeatureDefinitionProficiencys.ProficiencyFighterWeapon);
            Definition.FeatureSet.Add(DatabaseHelper.FeatureDefinitionAttackModifiers.AttackModifierMartialSpellBladeMagicWeapon);
            Definition.SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union);
            Definition.SetUniqueChoices(false);
        }

        internal static FeatureDefinitionFeatureSet CreateAndAddToDB(string name)
            => new AHWarlockClassPactOfTheBladeSetBuilder(name).AddToDB();

        internal static readonly FeatureDefinitionFeatureSet AHWarlockClassPactOfTheBladeSet = CreateAndAddToDB(AHWarlockClassPactOfTheBladeSetName);
    }

    internal class DHWarlockClassPactOfTheChainFeatureSetBuilder : FeatureDefinitionFeatureSetBuilder
    {
        private const string DHWarlockClassPactOfTheChainFeatureSetName = "DHWarlockClassPactOfTheChainFeatureSet";

        protected DHWarlockClassPactOfTheChainFeatureSetBuilder(string name) : base(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetGreenmageWardenOfTheForest, name, DefinitionBuilder.CENamespaceGuid)
        {
            Definition.GuiPresentation.Title = "Feature/&DHWarlockClassPactOfTheChainFeatureSetTitle";
            Definition.GuiPresentation.Description = "Feature/&DHWarlockClassPactOfTheChainFeatureSetDescription";

            Definition.FeatureSet.Clear();

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
            FindFamiliarImpGui.SetSpriteReference(WarlockPactOfTheChainSummons.PactChainImp.GuiPresentation.SpriteReference);

            EffectDescriptionBuilder effectDescriptionQuasit = new EffectDescriptionBuilder();
            effectDescriptionQuasit.SetDurationData(RuleDefinitions.DurationType.UntilLongRest, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            effectDescriptionQuasit.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Distance, 1, RuleDefinitions.TargetType.Position, 1, 1, ActionDefinitions.ItemSelectionType.Equiped);
            effectDescriptionQuasit.AddEffectForm(new EffectFormBuilder().SetSummonForm(SummonForm.Type.Creature, ScriptableObject.CreateInstance<ItemDefinition>(), 1, WarlockPactOfTheChainSummons.PactChainQuasit.name, DatabaseHelper.ConditionDefinitions.ConditionFlyingBootsWinged, true, null, ScriptableObject.CreateInstance<EffectProxyDefinition>()).Build());
            effectDescriptionQuasit.SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.ConjureElementalAir.EffectDescription.EffectParticleParameters);


            GuiPresentationBuilder FindFamiliarQuasitGui = new GuiPresentationBuilder(
                "Spell/&FindFamiliarQuasitDescription",
                "Spell/&FindFamiliarQuasitTitle");
            FindFamiliarQuasitGui.SetSpriteReference(WarlockPactOfTheChainSummons.PactChainQuasit.GuiPresentation.SpriteReference);


            var FindFamiliarImpPowerBuilder = FeatureDefinitionPowerBuilder
                .Create("FindFamiliarImpPower", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(FindFamiliarImpGui.Build())
                .Configure(
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
                 true);
            FeatureDefinitionPower FindFamiliarImpPower = FindFamiliarImpPowerBuilder.AddToDB();

            var FindFamiliarPseudodragonPowerBuilder = FeatureDefinitionPowerBuilder
                .Create("FindFamiliarPseudodragonPower", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(FindFamiliarPsuedodragonGui.Build())
                .Configure(
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
                 true);
            FeatureDefinitionPower FindFamiliarPseudodragonPower = FindFamiliarPseudodragonPowerBuilder.AddToDB();

            var FindFamiliarSpritePowerBuilder = FeatureDefinitionPowerBuilder
                .Create("FindFamiliarSpritePower", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(FindFamiliarSpriteGui.Build())
                .Configure(
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
                 true);
            FeatureDefinitionPower FindFamiliarSpritePower = FindFamiliarSpritePowerBuilder.AddToDB();

            var FindFamiliarQuasitPowerBuilder = FeatureDefinitionPowerBuilder
                .Create("FindFamiliarQuasitPower", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(FindFamiliarQuasitGui.Build())
                .Configure(
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
                 true);
            FeatureDefinitionPower FindFamiliarQuasitPower = FindFamiliarQuasitPowerBuilder.AddToDB();

            // Definition.FeatureSet.Add(FindFamiliarPseudodragonPower);
            // Definition.FeatureSet.Add(FindFamiliarSpritePower);
            // Definition.FeatureSet.Add(FindFamiliarImpPower);
            // Definition.FeatureSet.Add(FindFamiliarQuasitPower);

            FeatureDefinitionPower findFamiliarPowerBundle = FeatureDefinitionPowerPoolBuilder
                .Create("FindFamiliarBundlePower", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(new GuiPresentationBuilder(
                    "Find familiar title",
                    "Summons a familiar from the list"
                ).Build())
                .SetActivation(RuleDefinitions.ActivationTime.NoCost, 1)
                .AddToDB();


            PowerBundleContext.RegisterPowerBundle(findFamiliarPowerBundle,
                new List<FeatureDefinitionPower>()
                {
                    FindFamiliarPseudodragonPower,
                    FindFamiliarSpritePower,
                    FindFamiliarImpPower,
                    FindFamiliarQuasitPower
                });
            Definition.FeatureSet.Add(findFamiliarPowerBundle);

            Definition.SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union);
        }

        internal static FeatureDefinitionFeatureSet CreateAndAddToDB(string name)
        {
            return new DHWarlockClassPactOfTheChainFeatureSetBuilder(name).AddToDB();
        }

        internal static readonly FeatureDefinitionFeatureSet DHWarlockClassPactOfTheChainFeatureSet = CreateAndAddToDB(DHWarlockClassPactOfTheChainFeatureSetName);
    }


    internal class DHPactOfTheTomeFeatureSetBuilder : FeatureDefinitionFeatureSetBuilder
    {
        private const string DHPactOfTheTomeFeatureSetName = "DHPactOfTheTomeFeatureSet";

        protected DHPactOfTheTomeFeatureSetBuilder(string name) : base(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetGreenmageWardenOfTheForest, name, DefinitionBuilder.CENamespaceGuid)
        {
            Definition.GuiPresentation.Title = "Feature/&DHPactOfTheTomeFeatureSetTitle";
            Definition.GuiPresentation.Description = "Feature/&DHPactOfTheTomeFeatureSetDescription";

            Definition.FeatureSet.Clear();
            Definition.FeatureSet.Add(DHPactOfTheTomeBonusCantripsBuilder.DHPactOfTheTomeBonusCantrips);
            Definition.FeatureSet.Add(DHPactOfTheTomeMagicAffinityBuilder.DHPactOfTheTomeMagicAffinity);
        }

        internal static FeatureDefinitionFeatureSet CreateAndAddToDB(string name)
        {
            return new DHPactOfTheTomeFeatureSetBuilder(name).AddToDB();
        }

        internal static readonly FeatureDefinitionFeatureSet DHPactOfTheTomeFeatureSet = CreateAndAddToDB(DHPactOfTheTomeFeatureSetName);
    }

    internal class DHPactOfTheTomeBonusCantripsBuilder : FeatureDefinitionPointPoolBuilder
    {
        private const string PactOfTheTomeBonusCantripsName = "DHPactOfTheTomeBonusCantrips";

        protected DHPactOfTheTomeBonusCantripsBuilder(string name) : base(DatabaseHelper.FeatureDefinitionPointPools.PointPoolCircleLandBonusCantrip, name, DefinitionBuilder.CENamespaceGuid)
        {
            Definition.GuiPresentation.Title = "Feature/&NoContentTitle";
            Definition.GuiPresentation.Description = "Feature/&NoContentTitle";
            Definition.RestrictedChoices.Clear();

            Definition.SetPoolType(HeroDefinitions.PointsPoolType.Cantrip);
            Definition.SetPoolAmount(4);
        }

        internal static FeatureDefinitionPointPool CreateAndAddToDB(string name)
        {
            return new DHPactOfTheTomeBonusCantripsBuilder(name).AddToDB();
        }

        internal static readonly FeatureDefinitionPointPool DHPactOfTheTomeBonusCantrips = CreateAndAddToDB(PactOfTheTomeBonusCantripsName);
    }


    internal class DHPactOfTheTomeSpellListBuilder : SpellListDefinitionBuilder
    {
        private const string PactOfTheTomeSpellListName = "DHPactOfTheTomeSpellList";

        protected DHPactOfTheTomeSpellListBuilder(string name) : base(DatabaseHelper.SpellListDefinitions.SpellListKythaela_Cantrips, name, DefinitionBuilder.CENamespaceGuid)
        {
            Definition.GuiPresentation.Title = "Feature/&NoContentTitle";
            Definition.GuiPresentation.Description = "Feature/&NoContentTitle";

            Definition.SpellsByLevel[0].Spells.Clear();

            Definition.SpellsByLevel[0].Spells.AddRange(DatabaseHelper.SpellListDefinitions.SpellListCleric.SpellsByLevel[0].Spells);
            Definition.SpellsByLevel[0].Spells.AddRange(DatabaseHelper.SpellListDefinitions.SpellListDruid.SpellsByLevel[0].Spells);
            Definition.SpellsByLevel[0].Spells.AddRange(DatabaseHelper.SpellListDefinitions.SpellListDruid.SpellsByLevel[0].Spells);
            Definition.SpellsByLevel[0].Spells.AddRange(DatabaseHelper.SpellListDefinitions.SpellListSorcerer.SpellsByLevel[0].Spells);

        }

        internal static SpellListDefinition CreateAndAddToDB(string name)
        {
            return new DHPactOfTheTomeSpellListBuilder(name).AddToDB();
        }

        internal static readonly SpellListDefinition DHPactOfTheTomeSpellList = CreateAndAddToDB(PactOfTheTomeSpellListName);
    }


    internal class DHPactOfTheTomeMagicAffinityBuilder : FeatureDefinitionMagicAffinityBuilder
    {
        private const string PactOfTheTomeMagicAffinityName = "DHPactOfTheTomeMagicAffinity";

        protected DHPactOfTheTomeMagicAffinityBuilder(string name) : base(DatabaseHelper.FeatureDefinitionMagicAffinitys.MagicAffinityGreenmageGreenMagicList, name, DefinitionBuilder.CENamespaceGuid)
        {
            Definition.GuiPresentation.Title = "Feature/&NoContentTitle";
            Definition.GuiPresentation.Description = "Feature/&NoContentTitle";

            Definition.SetExtendedSpellList(DHPactOfTheTomeSpellListBuilder.DHPactOfTheTomeSpellList);
        }

        internal static FeatureDefinitionMagicAffinity CreateAndAddToDB(string name)
        {
            return new DHPactOfTheTomeMagicAffinityBuilder(name).AddToDB();
        }

        internal static readonly FeatureDefinitionMagicAffinity DHPactOfTheTomeMagicAffinity = CreateAndAddToDB(PactOfTheTomeMagicAffinityName);
    }
}
