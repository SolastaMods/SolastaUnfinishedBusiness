using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.Models;
using SolastaCommunityExpansion.Properties;
using SolastaCommunityExpansion.Utils;

namespace SolastaCommunityExpansion.Classes.Warlock.Features;

internal sealed class WarlockClassPactBoonSetBuilder : FeatureDefinitionFeatureSetBuilder
{
    private const string WarlockClassPactBoonSetName = "WarlockClassPactBoonSet";

    internal static readonly FeatureDefinitionFeatureSet WarlockClassPactBoonSet =
        CreateAndAddToDB(WarlockClassPactBoonSetName);

    private WarlockClassPactBoonSetBuilder(string name) : base(
        DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetHunterHuntersPrey, name, CENamespaceGuid)
    {
        Definition.GuiPresentation.Title = "Feature/&AHWarlockClassPactBoonSetTitle";
        Definition.GuiPresentation.Description = "Feature/&AHWarlockClassPactBoonSetDescription";

        Definition.FeatureSet.Clear();
        Definition.FeatureSet.Add(WarlockClassPactOfTheBladeSetBuilder.WarlockClassPactOfTheBladeSet);
        Definition.FeatureSet.Add(WarlockClassPactOfTheChainFeatureSetBuilder
            .WarlockClassPactOfTheChainFeatureSet);
        Definition.FeatureSet.Add(PactOfTheTomeFeatureSetBuilder.PactOfTheTomeFeatureSet);
        Definition.uniqueChoices = true;
        Definition.enumerateInDescription = true;
    }

    private static FeatureDefinitionFeatureSet CreateAndAddToDB(string name)
    {
        return new WarlockClassPactBoonSetBuilder(name).AddToDB();
    }
}

internal sealed class WarlockClassPactOfTheBladeSetBuilder : FeatureDefinitionFeatureSetBuilder
{
    private const string AhWarlockClassPactOfTheBladeSetName = "AHWarlockClassPactOfTheBladeSet";

    internal static readonly FeatureDefinitionFeatureSet WarlockClassPactOfTheBladeSet =
        CreateAndAddToDB(AhWarlockClassPactOfTheBladeSetName);

    private WarlockClassPactOfTheBladeSetBuilder(string name) : base(
        DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetHunterHuntersPrey, name, CENamespaceGuid)
    {
        Definition.GuiPresentation.Title = "Feature/&AHWarlockClassPactOfTheBladeSetTitle";
        Definition.GuiPresentation.Description = "Feature/&AHWarlockClassPactOfTheBladeSetDescription";

        Definition.FeatureSet.Clear();
        Definition.FeatureSet.Add(DatabaseHelper.FeatureDefinitionProficiencys.ProficiencyFighterWeapon);
        Definition.FeatureSet.Add(DatabaseHelper.FeatureDefinitionAttackModifiers
            .AttackModifierMartialSpellBladeMagicWeapon);
        Definition.mode = FeatureDefinitionFeatureSet.FeatureSetMode.Union;
        Definition.uniqueChoices = false;
    }

    private static FeatureDefinitionFeatureSet CreateAndAddToDB(string name)
    {
        return new WarlockClassPactOfTheBladeSetBuilder(name).AddToDB();
    }
}

internal sealed class WarlockClassPactOfTheChainFeatureSetBuilder : FeatureDefinitionFeatureSetBuilder
{
    private const string WarlockClassPactOfTheChainFeatureSetName = "DHWarlockClassPactOfTheChainFeatureSet";

    internal static readonly FeatureDefinitionFeatureSet WarlockClassPactOfTheChainFeatureSet =
        CreateAndAddToDB(WarlockClassPactOfTheChainFeatureSetName);

    private WarlockClassPactOfTheChainFeatureSetBuilder(string name) : base(
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
        var sprite = WarlockPactOfTheChainSummons.BuildCustomSprite();
        var imp = WarlockPactOfTheChainSummons.BuildCustomImp();
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

        var findFamiliarSpriteGui = new GuiPresentationBuilder(
            "Spell/&FindFamiliarSpriteTitle",
            "Spell/&FindFamiliarSpriteDescription");
        findFamiliarSpriteGui.SetSpriteReference(sprite.GuiPresentation.SpriteReference);


        var effectDescriptionImp = new EffectDescriptionBuilder()
            .SetDurationData(RuleDefinitions.DurationType.UntilLongRest, 1,
                RuleDefinitions.TurnOccurenceType.EndOfTurn)
            .SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Distance, 2,
                RuleDefinitions.TargetType.Position, 1, 1, ActionDefinitions.ItemSelectionType.Equiped)
            .AddEffectForm(new EffectFormBuilder().SetSummonCreatureForm(1, imp.name, false,
                DatabaseHelper.ConditionDefinitions.ConditionFlyingBootsWinged).Build())
            .SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.ConjureElementalAir.EffectDescription
                .EffectParticleParameters);


        var findFamiliarImpGui = new GuiPresentationBuilder(
            "Spell/&FindFamiliarImpTitle",
            "Spell/&FindFamiliarImpDescription");
        findFamiliarImpGui.SetSpriteReference(imp.GuiPresentation.SpriteReference);

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


        var findFamiliarImpPowerBuilder = FeatureDefinitionPowerBuilder
            .Create("FindFamiliarImpPower", CENamespaceGuid)
            .SetGuiPresentation(findFamiliarImpGui.Build())
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
        var findFamiliarImpPower = findFamiliarImpPowerBuilder.AddToDB();

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

        var findFamiliarSpritePowerBuilder = FeatureDefinitionPowerBuilder
            .Create("FindFamiliarSpritePower", CENamespaceGuid)
            .SetGuiPresentation(findFamiliarSpriteGui.Build())
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
        var findFamiliarSpritePower = findFamiliarSpritePowerBuilder.AddToDB();

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
            findFamiliarSpritePower,
            findFamiliarImpPower,
            FindFamiliarQuasitPower
        );
        Definition.FeatureSet.Add(findFamiliarPowerBundle);

        GlobalUniqueEffects.AddToGroup(GlobalUniqueEffects.Group.Familiar,
            FindFamiliarPseudodragonPower,
            findFamiliarSpritePower,
            findFamiliarImpPower,
            FindFamiliarQuasitPower
        );

        Definition.mode = FeatureDefinitionFeatureSet.FeatureSetMode.Union;
    }

    private static FeatureDefinitionFeatureSet CreateAndAddToDB(string name)
    {
        return new WarlockClassPactOfTheChainFeatureSetBuilder(name).AddToDB();
    }
}

internal sealed class PactOfTheTomeFeatureSetBuilder : FeatureDefinitionFeatureSetBuilder
{
    private const string PactOfTheTomeFeatureSetName = "DHPactOfTheTomeFeatureSet";

    private static readonly FeatureDefinitionPointPool DHPactOfTheTomeBonusCantrips =
        FeatureDefinitionPointPoolBuilder
            .Create("DHPactOfTheTomeBonusCantrips", CENamespaceGuid)
            .SetGuiPresentationNoContent()
            .SetPool(HeroDefinitions.PointsPoolType.Cantrip, 3)
            .OnlyUniqueChoices()
            .AddToDB();

    internal static readonly FeatureDefinitionFeatureSet PactOfTheTomeFeatureSet =
        CreateAndAddToDB(PactOfTheTomeFeatureSetName);

    private PactOfTheTomeFeatureSetBuilder(string name) : base(
        DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetGreenmageWardenOfTheForest, name, CENamespaceGuid)
    {
        Definition.GuiPresentation.Title = "Feature/&DHPactOfTheTomeFeatureSetTitle";
        Definition.GuiPresentation.Description = "Feature/&DHPactOfTheTomeFeatureSetDescription";

        Definition.FeatureSet.Clear();
        Definition.FeatureSet.Add(DHPactOfTheTomeBonusCantrips);
        Definition.FeatureSet.Add(DHPactOfTheTomeMagicAffinityBuilder.PactOfTheTomeMagicAffinity);
    }

    private static FeatureDefinitionFeatureSet CreateAndAddToDB(string name)
    {
        return new PactOfTheTomeFeatureSetBuilder(name).AddToDB();
    }
}

internal sealed class PactOfTheTomeSpellListBuilder : SpellListDefinitionBuilder
{
    private const string PactOfTheTomeSpellListName = "DHPactOfTheTomeSpellList";

    internal static readonly SpellListDefinition PactOfTheTomeSpellList =
        CreateAndAddToDB(PactOfTheTomeSpellListName);

    private PactOfTheTomeSpellListBuilder(string name) : base(
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

    private static SpellListDefinition CreateAndAddToDB(string name)
    {
        return new PactOfTheTomeSpellListBuilder(name).AddToDB();
    }
}

internal sealed class DHPactOfTheTomeMagicAffinityBuilder : FeatureDefinitionMagicAffinityBuilder
{
    private const string PactOfTheTomeMagicAffinityName = "DHPactOfTheTomeMagicAffinity";

    internal static readonly FeatureDefinitionMagicAffinity PactOfTheTomeMagicAffinity =
        CreateAndAddToDB(PactOfTheTomeMagicAffinityName);

    private DHPactOfTheTomeMagicAffinityBuilder(string name) : base(
        DatabaseHelper.FeatureDefinitionMagicAffinitys.MagicAffinityGreenmageGreenMagicList, name, CENamespaceGuid)
    {
        Definition.GuiPresentation.Title = "Feature/&NoContentTitle";
        Definition.GuiPresentation.Description = "Feature/&NoContentTitle";

        Definition.extendedSpellList = PactOfTheTomeSpellListBuilder.PactOfTheTomeSpellList;
    }

    private static FeatureDefinitionMagicAffinity CreateAndAddToDB(string name)
    {
        return new DHPactOfTheTomeMagicAffinityBuilder(name).AddToDB();
    }
}
