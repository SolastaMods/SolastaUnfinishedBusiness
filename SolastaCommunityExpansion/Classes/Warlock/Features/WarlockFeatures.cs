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
                .Where(x => x is not IFeatureDefinitionWithPrerequisites feature || feature.Validators.All(y => y.Invoke()))
                .ToList();
        }

        #region WarlockEldritchInvocationSetLevel2
        private static FeatureDefinitionFeatureSetDynamic warlockEldritchInvocationSetLevel2;
        public static FeatureDefinitionFeatureSetDynamic WarlockEldritchInvocationSetLevel2 => warlockEldritchInvocationSetLevel2 ??= FeatureDefinitionFeatureSetDynamicBuilder
            .Create("ClassWarlockEldritchInvocationSetLevel2", CENamespaceGuid)
            .SetGuiPresentation("Feature/&ClassWarlockEldritchInvocationSetLevelTitle", "Feature/&ClassWarlockEldritchInvocationSetLevelDescription")
            /*
            EI that might need a bit more work
            Investment of the Chain Master - multiple features through summoning affinity
            Book of ancient secrets - similar to MagicAffinityWizardRitualCasting or ritual casting feat
            */
            .SetFeatureSet(
                EldritchInvocationsBlasts["AgonizingBlast"],
                EldritchInvocationsBlasts["HinderingBlast"],
                EldritchInvocationsBlasts["RepellingBlast"],
                EldritchInvocationsBlasts["GraspingHand"],
                EldritchInvocationsPowers["ArmorofShadows"],
                EldritchInvocationsPowers["EldritchSight"],
                EldritchInvocationsPowers["FiendishVigor"],
                EldritchInvocationsPowers["ThiefofFiveFates"],
                EldritchInvocationsAttributeModifiers["AspectoftheMoon"],
                EldritchInvocationsAttributeModifiers["BeguilingInfluence"],
                EldritchInvocationsAttributeModifiers["EldritchMind"],
                EldritchInvocationsAttributeModifiers["DevilsSight"],
                EldritchInvocationsAttributeModifiers["EyesoftheRuneKeeper"],
                EldritchInvocationsAttributeModifiers["GiftoftheEver-LivingOnes"]
            )
            .SetDynamicFeatureSetFunc(InvocationsFilteredFeatureSet)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion)
            .SetUniqueChoices(true)
            .AddToDB();
        #endregion

        #region WarlockEldritchInvocationSetLevel5
        private static FeatureDefinitionFeatureSetDynamic warlockEldritchInvocationSetLevel5;
        public static FeatureDefinitionFeatureSetDynamic WarlockEldritchInvocationSetLevel5 => warlockEldritchInvocationSetLevel5 ??= FeatureDefinitionFeatureSetDynamicBuilder
            .Create(WarlockEldritchInvocationSetLevel2, "ClassWarlockEldritchInvocationSetLevel5", CENamespaceGuid)
            /*
            EI that might need a bit more work
            Sign of Ill Omen - create a feature set that adds converted versions of the subspells
            Tomb of Levistus - add tempHP and then apply incapcitated or stunned status
            Undying Servitude - summon a a skeleton or zombie
            */
            .AddFeatureSet(
                EldritchInvocationsAttributeModifiers["OneWithShadows"],
                EldritchInvocationsPowers["MiretheMind"],
                EldritchInvocationsAttributeModifiers["EldritchSmite"],
                EldritchInvocationsAttributeModifiers["ThirstingBlade"],
                EldritchInvocationsAttributeModifiers["ImprovedPactWeapon"]
            )
            .SetDynamicFeatureSetFunc(InvocationsFilteredFeatureSet)
            .AddToDB();
        #endregion

        #region WarlockEldritchInvocationSetLevel7
        private static FeatureDefinitionFeatureSetDynamic warlockEldritchInvocationSetLevel7;
        public static FeatureDefinitionFeatureSetDynamic WarlockEldritchInvocationSetLevel7 => warlockEldritchInvocationSetLevel7 ??= FeatureDefinitionFeatureSetDynamicBuilder
            .Create(WarlockEldritchInvocationSetLevel5, "ClassWarlockEldritchInvocationSetLevel7", CENamespaceGuid)
            .AddFeatureSet(
                EldritchInvocationsAttributeModifiers["OneWithShadowsStronger"],
                EldritchInvocationsPowers["DreadfulWord"],
                EldritchInvocationsPowers["TrickstersEscape"]
            )
            .SetDynamicFeatureSetFunc(InvocationsFilteredFeatureSet)
            .AddToDB();
        #endregion

        #region WarlockEldritchInvocationSetLevel9
        private static FeatureDefinitionFeatureSetDynamic warlockEldritchInvocationSetLevel9;
        public static FeatureDefinitionFeatureSetDynamic WarlockEldritchInvocationSetLevel9 => warlockEldritchInvocationSetLevel9 ??= FeatureDefinitionFeatureSetDynamicBuilder
            .Create(WarlockEldritchInvocationSetLevel7, "ClassWarlockEldritchInvocationSetLevel9", CENamespaceGuid)
            .AddFeatureSet(
                EldritchInvocationsPowers["AscendantStep"],
                EldritchInvocationsPowers["OtherworldlyLeap"],
                EldritchInvocationsAttributeModifiers["GiftoftheProtectors"]
            )
            .SetDynamicFeatureSetFunc(InvocationsFilteredFeatureSet)
            .AddToDB();
        #endregion

        #region WarlockEldritchInvocationSetLevel12
        private static FeatureDefinitionFeatureSetDynamic warlockEldritchInvocationSetLevel12;
        public static FeatureDefinitionFeatureSetDynamic WarlockEldritchInvocationSetLevel12 => warlockEldritchInvocationSetLevel12 ??= FeatureDefinitionFeatureSetDynamicBuilder
            .Create(WarlockEldritchInvocationSetLevel9, "ClassWarlockEldritchInvocationSetLevel12", CENamespaceGuid)
            .AddFeatureSet(
                EldritchInvocationsAttributeModifiers["BondoftheTalisman"]
            )
            .SetDynamicFeatureSetFunc(InvocationsFilteredFeatureSet)
            .AddToDB();
        #endregion

        #region WarlockEldritchInvocationSetLevel15
        private static FeatureDefinitionFeatureSetDynamic warlockEldritchInvocationSetLevel15;
        public static FeatureDefinitionFeatureSetDynamic WarlockEldritchInvocationSetLevel15 => warlockEldritchInvocationSetLevel15 ??= FeatureDefinitionFeatureSetDynamicBuilder
            .Create(WarlockEldritchInvocationSetLevel12, "ClassWarlockEldritchInvocationSetLevel15", CENamespaceGuid)
            .AddFeatureSet(
                /*
                *EI that  more work
                Master of Myriad Forms - would need to create the alter self spell then convert it
                */
                EldritchInvocationsPowers["ChainsofCarceri"],
                EldritchInvocationsPowers["ShroudofShadow"],
                EldritchInvocationsAttributeModifiers["WitchSight"]
            )
            .SetDynamicFeatureSetFunc(InvocationsFilteredFeatureSet)
            .AddToDB();
        #endregion

        #region WarlockEldritchInvocationSetLevel18
        private static FeatureDefinitionFeatureSetDynamic warlockEldritchInvocationSetLevel18;
        public static FeatureDefinitionFeatureSetDynamic WarlockEldritchInvocationSetLevel18 => warlockEldritchInvocationSetLevel18 ??= FeatureDefinitionFeatureSetDynamicBuilder
            .Create(WarlockEldritchInvocationSetLevel15, "ClassWarlockEldritchInvocationSetLevel18", CENamespaceGuid)
            .SetDynamicFeatureSetFunc(InvocationsFilteredFeatureSet)
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
