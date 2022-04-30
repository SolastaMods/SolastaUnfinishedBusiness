using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaModApi.Extensions;
using static SolastaCommunityExpansion.Builders.DefinitionBuilder;
using static SolastaCommunityExpansion.Classes.Warlock.Features.EldritchInvocationsBuilder;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaCommunityExpansion.Classes.Warlock.Features
{
    internal static class WarlockFeatures
    {
        internal static readonly FeatureDefinitionFeatureSet WarlockMysticArcanumSetLevel11 = CreateMysticArcanumSet(11, 6);
        internal static readonly FeatureDefinitionFeatureSet WarlockMysticArcanumSetLevel13 = CreateMysticArcanumSet(13, 7, 6);
        internal static readonly FeatureDefinitionFeatureSet WarlockMysticArcanumSetLevel15 = CreateMysticArcanumSet(15, 8, 7, 6);
        internal static readonly FeatureDefinitionFeatureSet WarlockMysticArcanumSetLevel17 = CreateMysticArcanumSet(17, 9, 8, 7, 6);

        private static FeatureDefinitionPower warlockEldritchMasterPower;

        internal static FeatureDefinitionPower WarlockEldritchMasterPower => warlockEldritchMasterPower ??= FeatureDefinitionPowerBuilder
            .Create(PowerWizardArcaneRecovery, "ClassWarlockEldritchMaster", CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .SetActivationTime(RuleDefinitions.ActivationTime.Minute1)
            .AddToDB();

        private static List<FeatureDefinition> InvocationsFilteredFeatureSet(FeatureDefinitionFeatureSet featureDefinitionFeatureSet)
        {
            return featureDefinitionFeatureSet.FeatureSet
                .Where(x => x is not IFeatureDefinitionWithPrerequisites feature || feature.Validators.All(y => y()==null))
                .ToList();
        }

        #region WarlockEldritchInvocationSet
        private static CustomFeatureDefinitionSet warlockEldritchInvocationSet;
        public static CustomFeatureDefinitionSet WarlockEldritchInvocationSet => warlockEldritchInvocationSet ??= CustomFeatureDefinitionSetBuilder
            .Create("WarlockEldritchInvocationSet", CENamespaceGuid)
            .SetGuiPresentation(
                "Feature/&ClassWarlockEldritchInvocationSetLevelTitle", 
                "Feature/&ClassWarlockEldritchInvocationSetLevelDescription",
                Utils.CustomIcons.CreateAssetReferenceSprite("EldritchInvocation", Properties.Resources.EldritchInvocation, 128, 128)
            )
            .SetRequireClassLevels(true)
            .SetLevelFeatures(1,
                EldritchInvocations["AgonizingBlast"],
                EldritchInvocations["HinderingBlast"],
                EldritchInvocations["RepellingBlast"],
                EldritchInvocations["GraspingHand"],
                EldritchInvocations["ArmorofShadows"],
                EldritchInvocations["EldritchSight"],
                EldritchInvocations["FiendishVigor"],
                EldritchInvocations["ThiefofFiveFates"],
                EldritchInvocations["BeguilingInfluence"],
                EldritchInvocations["DevilsSight"],
                EldritchInvocations["EyesoftheRuneKeeper"]
            )
            .SetLevelFeatures(3,
                EldritchInvocations["EldritchMind"],
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

        #region SupportCode
        private static FeatureDefinitionPower CreateMysticArcanumPower(string baseName, SpellDefinition spell)
        {
            return FeatureDefinitionPowerBuilder
                .Create(baseName + spell.name, DefinitionBuilder.CENamespaceGuid)
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

        private static FeatureDefinitionFeatureSet CreateMysticArcanumSet(int setLevel, params int[] spellLevels)
        {
            return FeatureDefinitionFeatureSetBuilder
                .Create(TerrainTypeAffinityRangerNaturalExplorerChoice, $"ClassWarlockMysticArcanumSetLevel{setLevel}", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation("ClassWarlockMysticArcanumSet", Category.Feature)
                .SetFeatureSet(GetSpells(spellLevels).Select(spell => CreateMysticArcanumPower($"DH_MysticArcanum{setLevel}_", spell)))
                .SetUniqueChoices(true)
                .AddToDB();
        }
        #endregion
    }
}
