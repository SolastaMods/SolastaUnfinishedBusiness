using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.Models;
using SolastaCommunityExpansion.Properties;
using SolastaCommunityExpansion.Utils;
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Classes.Warlock.Features
{
    internal class WarlockClassPactBoonSetBuilder : FeatureDefinitionFeatureSetBuilder
    {
        private const string WarlockClassPactBoonSetName = "WarlockClassPactBoonSet";

        internal static readonly FeatureDefinitionFeatureSet WarlockClassPactBoonSet =
            CreateAndAddToDB(WarlockClassPactBoonSetName);

        protected WarlockClassPactBoonSetBuilder(string name) : base(
            DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetHunterHuntersPrey, name, CENamespaceGuid)
        {
            Definition.GuiPresentation.Title = "Feature/&AHWarlockClassPactBoonSetTitle";
            Definition.GuiPresentation.Description = "Feature/&AHWarlockClassPactBoonSetDescription";

            Definition.FeatureSet.Clear();
            Definition.FeatureSet.Add(AHWarlockClassPactOfTheBladeSetBuilder.AHWarlockClassPactOfTheBladeSet);
            Definition.FeatureSet.Add(DHWarlockClassPactOfTheChainFeatureSetBuilder
                .DHWarlockClassPactOfTheChainFeatureSet);
            Definition.FeatureSet.Add(DHPactOfTheTomeFeatureSetBuilder.DHPactOfTheTomeFeatureSet);
            Definition.SetUniqueChoices(true);
            Definition.SetEnumerateInDescription(true);
        }

        internal static FeatureDefinitionFeatureSet CreateAndAddToDB(string name)
        {
            return new WarlockClassPactBoonSetBuilder(name).AddToDB();
        }
    }

    internal class AHWarlockClassPactOfTheBladeSetBuilder : FeatureDefinitionFeatureSetBuilder
    {
        private const string AHWarlockClassPactOfTheBladeSetName = "AHWarlockClassPactOfTheBladeSet";

        internal static readonly FeatureDefinitionFeatureSet AHWarlockClassPactOfTheBladeSet =
            CreateAndAddToDB(AHWarlockClassPactOfTheBladeSetName);

        protected AHWarlockClassPactOfTheBladeSetBuilder(string name) : base(
            DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetHunterHuntersPrey, name, CENamespaceGuid)
        {
            Definition.GuiPresentation.Title = "Feature/&AHWarlockClassPactOfTheBladeSetTitle";
            Definition.GuiPresentation.Description = "Feature/&AHWarlockClassPactOfTheBladeSetDescription";

            Definition.FeatureSet.Clear();
            Definition.FeatureSet.Add(DatabaseHelper.FeatureDefinitionProficiencys.ProficiencyFighterWeapon);
            Definition.FeatureSet.Add(DatabaseHelper.FeatureDefinitionAttackModifiers
                .AttackModifierMartialSpellBladeMagicWeapon);
            Definition.SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union);
            Definition.SetUniqueChoices(false);
        }

        internal static FeatureDefinitionFeatureSet CreateAndAddToDB(string name)
        {
            return new AHWarlockClassPactOfTheBladeSetBuilder(name).AddToDB();
        }
    }

    internal class DHWarlockClassPactOfTheChainFeatureSetBuilder : FeatureDefinitionFeatureSetBuilder
    {
        private const string DHWarlockClassPactOfTheChainFeatureSetName = "DHWarlockClassPactOfTheChainFeatureSet";

        internal static readonly FeatureDefinitionFeatureSet DHWarlockClassPactOfTheChainFeatureSet =
            CreateAndAddToDB(DHWarlockClassPactOfTheChainFeatureSetName);

        protected DHWarlockClassPactOfTheChainFeatureSetBuilder(string name) : base(
            DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetGreenmageWardenOfTheForest, name, CENamespaceGuid)
        {
            Definition.GuiPresentation.Title = "Feature/&DHWarlockClassPactOfTheChainFeatureSetTitle";
            Definition.GuiPresentation.Description = "Feature/&DHWarlockClassPactOfTheChainFeatureSetDescription";

            Definition.FeatureSet.Clear();

            // maybe could be merged with the witchs find familiar

            //Warlock's familiar's don't scale but get one-time bonuses when Chain Master invocation is selected
            // Definition.FeatureSet.Add(WarlockPactOfTheChainSummons.buildSummoningAffinity());

            WarlockPactOfTheChainSummons.buildPactofChainFamiliarInvisibilityPower();
            WarlockPactOfTheChainSummons.buildPactofChainFamiliarScarePower();
            var pseudodragon = WarlockPactOfTheChainSummons.buildCustomPseudodragon();
            var sprite = WarlockPactOfTheChainSummons.buildCustomSprite();
            var imp = WarlockPactOfTheChainSummons.buildCustomImp();
            var quasit = WarlockPactOfTheChainSummons.buildCustomQuasit();


            var effectDescriptionPseudodragon = new EffectDescriptionBuilder()
                .SetDurationData(RuleDefinitions.DurationType.UntilLongRest, 1,
                    RuleDefinitions.TurnOccurenceType.EndOfTurn)
                .SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Distance, 2,
                    RuleDefinitions.TargetType.Position)
                .AddEffectForm(new EffectFormBuilder()
                    .SetSummonCreatureForm(1, pseudodragon.name, false,
                        DatabaseHelper.ConditionDefinitions.ConditionFlyingBootsWinged)
                    .Build()
                )
                .SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.ConjureElementalAir.EffectDescription
                    .EffectParticleParameters);

            var FindFamiliarPsuedodragonGui = new GuiPresentationBuilder(
                "Spell/&FindFamiliarPsuedodragonTitle",
                "Spell/&FindFamiliarPsuedodragonDescription");
            FindFamiliarPsuedodragonGui.SetSpriteReference(pseudodragon.GuiPresentation.SpriteReference);


            var effectDescriptionSprite = new EffectDescriptionBuilder();
            effectDescriptionSprite.SetDurationData(RuleDefinitions.DurationType.UntilLongRest, 1,
                RuleDefinitions.TurnOccurenceType.EndOfTurn);
            effectDescriptionSprite.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Distance, 2,
                RuleDefinitions.TargetType.Position, 1, 1, ActionDefinitions.ItemSelectionType.Equiped);
            effectDescriptionSprite.AddEffectForm(new EffectFormBuilder().SetSummonCreatureForm(1, sprite.name, false,
                DatabaseHelper.ConditionDefinitions.ConditionFlyingBootsWinged).Build());
            effectDescriptionSprite.SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.ConjureElementalAir
                .EffectDescription.EffectParticleParameters);

            var FindFamiliarSpriteGui = new GuiPresentationBuilder(
                "Spell/&FindFamiliarSpriteTitle",
                "Spell/&FindFamiliarSpriteDescription");
            FindFamiliarSpriteGui.SetSpriteReference(sprite.GuiPresentation.SpriteReference);


            var effectDescriptionImp = new EffectDescriptionBuilder()
                .SetDurationData(RuleDefinitions.DurationType.UntilLongRest, 1,
                    RuleDefinitions.TurnOccurenceType.EndOfTurn)
                .SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Distance, 2,
                    RuleDefinitions.TargetType.Position, 1, 1, ActionDefinitions.ItemSelectionType.Equiped)
                .AddEffectForm(new EffectFormBuilder().SetSummonCreatureForm(1, imp.name, false,
                    DatabaseHelper.ConditionDefinitions.ConditionFlyingBootsWinged).Build())
                .SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.ConjureElementalAir.EffectDescription
                    .EffectParticleParameters);


            var FindFamiliarImpGui = new GuiPresentationBuilder(
                "Spell/&FindFamiliarImpTitle",
                "Spell/&FindFamiliarImpDescription");
            FindFamiliarImpGui.SetSpriteReference(imp.GuiPresentation.SpriteReference);

            var effectDescriptionQuasit = new EffectDescriptionBuilder();
            effectDescriptionQuasit.SetDurationData(RuleDefinitions.DurationType.UntilLongRest, 1,
                RuleDefinitions.TurnOccurenceType.EndOfTurn);
            effectDescriptionQuasit.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Distance, 2,
                RuleDefinitions.TargetType.Position, 1, 1, ActionDefinitions.ItemSelectionType.Equiped);
            effectDescriptionQuasit.AddEffectForm(new EffectFormBuilder().SetSummonCreatureForm(1, quasit.name)
                .Build());
            effectDescriptionQuasit.SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.ConjureElementalAir
                .EffectDescription.EffectParticleParameters);


            var FindFamiliarQuasitGui = new GuiPresentationBuilder(
                "Spell/&FindFamiliarQuasitTitle",
                "Spell/&FindFamiliarQuasitDescription");
            FindFamiliarQuasitGui.SetSpriteReference(quasit.GuiPresentation.SpriteReference);


            var FindFamiliarImpPowerBuilder = FeatureDefinitionPowerBuilder
                .Create("FindFamiliarImpPower", CENamespaceGuid)
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
            var FindFamiliarImpPower = FindFamiliarImpPowerBuilder.AddToDB();

            var FindFamiliarPseudodragonPowerBuilder = FeatureDefinitionPowerBuilder
                .Create("FindFamiliarPseudodragonPower", CENamespaceGuid)
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
            var FindFamiliarPseudodragonPower = FindFamiliarPseudodragonPowerBuilder.AddToDB();

            var FindFamiliarSpritePowerBuilder = FeatureDefinitionPowerBuilder
                .Create("FindFamiliarSpritePower", CENamespaceGuid)
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
            var FindFamiliarSpritePower = FindFamiliarSpritePowerBuilder.AddToDB();

            var FindFamiliarQuasitPowerBuilder = FeatureDefinitionPowerBuilder
                .Create("FindFamiliarQuasitPower", CENamespaceGuid)
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
            var FindFamiliarQuasitPower = FindFamiliarQuasitPowerBuilder.AddToDB();

            var findFamiliarPowerBundle = FeatureDefinitionPowerPoolBuilder
                .Create("FindFamiliarBundlePower", CENamespaceGuid)
                .SetGuiPresentation(Category.Power,
                    CustomIcons.CreateAssetReferenceSprite("WarlockChainSummon",
                        Resources.WarlockChainSummon, 128, 64))
                .SetActivation(RuleDefinitions.ActivationTime.Hours1, 1)
                .AddToDB();


            PowerBundleContext.RegisterPowerBundle(findFamiliarPowerBundle, false,
                FindFamiliarPseudodragonPower,
                FindFamiliarSpritePower,
                FindFamiliarImpPower,
                FindFamiliarQuasitPower
            );
            Definition.FeatureSet.Add(findFamiliarPowerBundle);

            GlobalUniqueEffects.AddToGroup(GlobalUniqueEffects.Group.Familiar,
                FindFamiliarPseudodragonPower,
                FindFamiliarSpritePower,
                FindFamiliarImpPower,
                FindFamiliarQuasitPower
            );

            Definition.SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union);
        }

        internal static FeatureDefinitionFeatureSet CreateAndAddToDB(string name)
        {
            return new DHWarlockClassPactOfTheChainFeatureSetBuilder(name).AddToDB();
        }
    }


    internal class DHPactOfTheTomeFeatureSetBuilder : FeatureDefinitionFeatureSetBuilder
    {
        private const string DHPactOfTheTomeFeatureSetName = "DHPactOfTheTomeFeatureSet";

        private static readonly FeatureDefinitionPointPool DHPactOfTheTomeBonusCantrips =
            FeatureDefinitionPointPoolWithBonusBuilder
                .Create("DHPactOfTheTomeBonusCantrips", CENamespaceGuid)
                .SetGuiPresentationNoContent()
                .SetPool(HeroDefinitions.PointsPoolType.Cantrip, 3)
                .OnlyUniqueChoices()
                .AddToDB();

        internal static readonly FeatureDefinitionFeatureSet DHPactOfTheTomeFeatureSet =
            CreateAndAddToDB(DHPactOfTheTomeFeatureSetName);

        protected DHPactOfTheTomeFeatureSetBuilder(string name) : base(
            DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetGreenmageWardenOfTheForest, name, CENamespaceGuid)
        {
            Definition.GuiPresentation.Title = "Feature/&DHPactOfTheTomeFeatureSetTitle";
            Definition.GuiPresentation.Description = "Feature/&DHPactOfTheTomeFeatureSetDescription";

            Definition.FeatureSet.Clear();
            Definition.FeatureSet.Add(DHPactOfTheTomeBonusCantrips);
            Definition.FeatureSet.Add(DHPactOfTheTomeMagicAffinityBuilder.DHPactOfTheTomeMagicAffinity);
        }

        internal static FeatureDefinitionFeatureSet CreateAndAddToDB(string name)
        {
            return new DHPactOfTheTomeFeatureSetBuilder(name).AddToDB();
        }
    }


    internal class DHPactOfTheTomeSpellListBuilder : SpellListDefinitionBuilder
    {
        private const string PactOfTheTomeSpellListName = "DHPactOfTheTomeSpellList";

        internal static readonly SpellListDefinition DHPactOfTheTomeSpellList =
            CreateAndAddToDB(PactOfTheTomeSpellListName);

        protected DHPactOfTheTomeSpellListBuilder(string name) : base(
            DatabaseHelper.SpellListDefinitions.SpellListKythaela_Cantrips, name, CENamespaceGuid)
        {
            Definition.GuiPresentation.Title = "Feature/&NoContentTitle";
            Definition.GuiPresentation.Description = "Feature/&NoContentTitle";

            Definition.SpellsByLevel[0].Spells.Clear();

            Definition.SpellsByLevel[0].Spells
                .AddRange(DatabaseHelper.SpellListDefinitions.SpellListCleric.SpellsByLevel[0].Spells);
            Definition.SpellsByLevel[0].Spells
                .AddRange(DatabaseHelper.SpellListDefinitions.SpellListDruid.SpellsByLevel[0].Spells);
            Definition.SpellsByLevel[0].Spells
                .AddRange(DatabaseHelper.SpellListDefinitions.SpellListDruid.SpellsByLevel[0].Spells);
            Definition.SpellsByLevel[0].Spells
                .AddRange(DatabaseHelper.SpellListDefinitions.SpellListSorcerer.SpellsByLevel[0].Spells);
        }

        internal static SpellListDefinition CreateAndAddToDB(string name)
        {
            return new DHPactOfTheTomeSpellListBuilder(name).AddToDB();
        }
    }


    internal class DHPactOfTheTomeMagicAffinityBuilder : FeatureDefinitionMagicAffinityBuilder
    {
        private const string PactOfTheTomeMagicAffinityName = "DHPactOfTheTomeMagicAffinity";

        internal static readonly FeatureDefinitionMagicAffinity DHPactOfTheTomeMagicAffinity =
            CreateAndAddToDB(PactOfTheTomeMagicAffinityName);

        protected DHPactOfTheTomeMagicAffinityBuilder(string name) : base(
            DatabaseHelper.FeatureDefinitionMagicAffinitys.MagicAffinityGreenmageGreenMagicList, name, CENamespaceGuid)
        {
            Definition.GuiPresentation.Title = "Feature/&NoContentTitle";
            Definition.GuiPresentation.Description = "Feature/&NoContentTitle";

            Definition.SetExtendedSpellList(DHPactOfTheTomeSpellListBuilder.DHPactOfTheTomeSpellList);
        }

        internal static FeatureDefinitionMagicAffinity CreateAndAddToDB(string name)
        {
            return new DHPactOfTheTomeMagicAffinityBuilder(name).AddToDB();
        }
    }
}
