using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.Properties;
using SolastaCommunityExpansion.Utils;
using static SolastaCommunityExpansion.Builders.DefinitionBuilder;
using static SolastaCommunityExpansion.Classes.Warlock.Features.EldritchInvocationsBuilder;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaCommunityExpansion.Classes.Warlock.Features;

internal static class WarlockFeatures
{
    internal static readonly FeatureDefinitionFeatureSetCustom WarlockMysticArcanumSet =
        CreateMysticArcanumSet((11, 6), (13, 7), (15, 8), (17, 9));

    private static FeatureDefinitionPower _warlockEldritchMasterPower;

    internal static FeatureDefinitionPower WarlockEldritchMasterPower => _warlockEldritchMasterPower ??=
        FeatureDefinitionPowerBuilder
            .Create(PowerWizardArcaneRecovery, "ClassWarlockEldritchMaster", CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .SetActivationTime(RuleDefinitions.ActivationTime.Minute1)
            .AddToDB();

    #region WarlockEldritchInvocationSet

    private static FeatureDefinitionFeatureSetCustom _warlockEldritchInvocationSet;

    public static FeatureDefinitionFeatureSetCustom WarlockEldritchInvocationSet => _warlockEldritchInvocationSet ??=
        FeatureDefinitionFeatureSetCustomBuilder
            .Create("ClassWarlockEldritchInvocationSetLevel", CENamespaceGuid)
            .SetGuiPresentation(Category.Feature,
                CustomIcons.CreateAssetReferenceSprite("EldritchInvocation",
                    Resources.EldritchInvocation, 128, 128)
            )
            .SetRequireClassLevels(true)
            .SetLevelFeatures(1,
                EldritchInvocations["AgonizingBlast"],
                EldritchInvocations["HinderingBlast"],
                EldritchInvocations["RepellingBlast"],
                EldritchInvocations["GraspingHand"],
                EldritchInvocations["ArmorofShadows"],
                EldritchInvocations["EldritchMind"],
                EldritchInvocations["EldritchSight"],
                EldritchInvocations["FiendishVigor"],
                EldritchInvocations["ThiefofFiveFates"],
                EldritchInvocations["BeguilingInfluence"],
                EldritchInvocations["DevilsSight"],
                EldritchInvocations["EyesoftheRuneKeeper"]
            )
            .SetLevelFeatures(3,
                EldritchInvocations["AspectoftheMoon"],
                EldritchInvocations["GiftoftheEverLivingOnes"],
                EldritchInvocations["ImprovedPactWeapon"]
            )
            .SetLevelFeatures(5,
                EldritchInvocations["OneWithShadows"],
                EldritchInvocations["MiretheMind"],
                EldritchInvocations["EldritchSmite"],
                EldritchInvocations["ThirstingBlade"]
            )
            .SetLevelFeatures(7,
                EldritchInvocations["OneWithShadowsStronger"],
                EldritchInvocations["DreadfulWord"],
                EldritchInvocations["TrickstersEscape"]
            )
            .SetLevelFeatures(9,
                EldritchInvocations["AscendantStep"],
                EldritchInvocations["OtherworldlyLeap"],
                EldritchInvocations["GiftoftheProtectors"]
            )
            .SetLevelFeatures(12,
                EldritchInvocations["BondoftheTalisman"]
            )
            .SetLevelFeatures(15,
                /*
                Master of Myriad Forms - would need to create the alter self spell then convert it
                */
                EldritchInvocations["ChainsofCarceri"],
                EldritchInvocations["ShroudofShadow"],
                EldritchInvocations["WitchSight"]
            )
            .AddToDB();

    #endregion

    #region WarlockEldritchInvocationReplacer

    private static FeatureDefinitionFeatureSetReplaceCustom _warlockEldritchInvocationReplacer;

    public static FeatureDefinitionFeatureSetReplaceCustom WarlockEldritchInvocationReplacer =>
        _warlockEldritchInvocationReplacer ??= FeatureDefinitionFeatureSetReplaceCustomBuilder
            .Create("ClassWarlockEldritchInvocationReplace", CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .SetReplacedFeatureSet(WarlockEldritchInvocationSet)
            .AddToDB();

    #endregion

    #region SupportCode

    private static FeatureDefinition CreateMysticArcanumPower(string baseName, SpellDefinition spell)
    {
        return FeatureDefinitionPowerBuilder
            .Create(baseName + spell.name, CENamespaceGuid)
            .SetGuiPresentation(spell.GuiPresentation)
            .Configure(
                1,
                RuleDefinitions.UsesDetermination.Fixed,
                AttributeDefinitions.Charisma,
                spell.ActivationTime,
                1,
                RuleDefinitions.RechargeRate.LongRest,
                false,
                false,
                AttributeDefinitions.Charisma,
                spell.EffectDescription,
                true)
            .AddToDB();
    }

    private static IEnumerable<SpellDefinition> GetSpells(params int[] levels)
    {
        return levels.SelectMany(level => WarlockSpells.WarlockSpellList.SpellsByLevel[level].Spells);
    }

    private static FeatureDefinitionFeatureSetCustom CreateMysticArcanumSet(params (int, int)[] levels)
    {
        var builder = FeatureDefinitionFeatureSetCustomBuilder
            .Create("ClassWarlockMysticArcanumSet", CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .SetRequireClassLevels(true);

        foreach (var (setLevel, spellLevel) in levels)
        {
            builder.SetLevelFeatures(setLevel, GetSpells(spellLevel)
                .Select(spell => CreateMysticArcanumPower($"DH_MysticArcanum{setLevel}_", spell)).ToList());
        }

        return builder.AddToDB();
    }

    #endregion
}
