using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.Models;
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

        private static Dictionary<FeatureDefinition, string> InvocationsFilteredFeatureSet(FeatureDefinitionFeatureSet featureDefinitionFeatureSet)
        {
            var result = new Dictionary<FeatureDefinition, string>();

            foreach (var feature in featureDefinitionFeatureSet.FeatureSet
                .Where(x => x is not IFeatureDefinitionWithPrerequisites feature || feature.Validators.All(y => y.Invoke())))
            {
                result.Add(feature, string.Empty);
            }

            return result;
        }

        private static Dictionary<FeatureDefinition, string> InvocationsTakenFeatureSet(FeatureDefinitionFeatureSet featureDefinitionFeatureSet)
        {
            var result = new Dictionary<FeatureDefinition, string>();
            var hero = Global.ActiveLevelUpHero;
            var heroBuildingData = hero?.GetHeroBuildingData();

            if (hero == null || heroBuildingData == null)
            {
                return result;
            }

            foreach (var kvp in hero.ActiveFeatures)
            {
                var tag = kvp.Key;

                foreach (var feature in kvp.Value
                    .Where(y => DictionaryofEBInvocations.Values.Any(x => x == y)
                    || DictionaryofEIAttributeModifers.Values.Any(x => x == y)
                    || DictionaryofEIPowers.Values.Any(x => x == y)))
                {
                    result.TryAdd(feature, tag);
                }
            }

            return result;
        }

        #region WarlockEldritchInvocationSetRemoval
        private static FeatureDefinitionFeatureSetUniqueAcross warlockEldritchInvocationSetRemoval;
        public static FeatureDefinitionFeatureSetUniqueAcross WarlockEldritchInvocationSetRemoval => warlockEldritchInvocationSetRemoval ??= FeatureDefinitionFeatureSetUniqueAcrossBuilder
            .Create("ClassWarlockEldritchInvocationSetRemoval", CENamespaceGuid)
            .SetGuiPresentation("Feature/&ClassWarlockEldritchInvocationSetRemovalTitle", "Feature/&ClassWarlockEldritchInvocationSetRemovalDescription")
            .SetFeatureSet(WarlockEldritchInvocationSetLevel18.FeatureSet)
            .SetDynamicFeatureSetFunc(InvocationsTakenFeatureSet)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion)
            .SetUniqueChoices(false)
            .SetBehaviorTags(FeatureDefinitionFeatureSetUniqueAcross.REMOVE_BEHAVIOR)
            .AddToDB();
        #endregion

        #region WarlockEldritchInvocationSetLevel2
        private static FeatureDefinitionFeatureSetUniqueAcross warlockEldritchInvocationSetLevel2;
        public static FeatureDefinitionFeatureSetUniqueAcross WarlockEldritchInvocationSetLevel2 => warlockEldritchInvocationSetLevel2 ??= FeatureDefinitionFeatureSetUniqueAcrossBuilder
            .Create("ClassWarlockEldritchInvocationSetLevel2", CENamespaceGuid)
            .SetGuiPresentation("Feature/&ClassWarlockEldritchInvocationSetLevelTitle", "Feature/&ClassWarlockEldritchInvocationSetLevelDescription")
            /*
            EI that might need a bit more work
            Investment of the Chain Master - multiple features through summoning affinity
            Book of ancient secrets - similar to MagicAffinityWizardRitualCasting or ritual casting feat
            */
            .SetFeatureSet(
                AgonizingBlastFeatureSet,
                HinderingBlastFeatureSet,
                DictionaryofEBInvocations["RepellingBlast"],
                DictionaryofEBInvocations["GraspingHand"],
                DictionaryofEIPowers["ArmorofShadows"],
                DictionaryofEIPowers["EldritchSight"],
                DictionaryofEIPowers["FiendishVigor"],
                DictionaryofEIPowers["ThiefofFiveFates"],
                DictionaryofEIAttributeModifers["AspectoftheMoon"],
                DictionaryofEIAttributeModifers["BeguilingInfluence"],
                DictionaryofEIAttributeModifers["EldritchMind"],
                DictionaryofEIAttributeModifers["EyesoftheRuneKeeper"],
                DictionaryofEIAttributeModifers["GiftoftheEver-LivingOnes"]
            )
            .SetDynamicFeatureSetFunc(InvocationsFilteredFeatureSet)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion)
            .SetUniqueChoices(false)
            .AddToDB();
        #endregion

        #region WarlockEldritchInvocationSetLevel5
        private static FeatureDefinitionFeatureSetUniqueAcross warlockEldritchInvocationSetLevel5;
        public static FeatureDefinitionFeatureSetUniqueAcross WarlockEldritchInvocationSetLevel5 => warlockEldritchInvocationSetLevel5 ??= FeatureDefinitionFeatureSetUniqueAcrossBuilder
            .Create(WarlockEldritchInvocationSetLevel2, "ClassWarlockEldritchInvocationSetLevel5", CENamespaceGuid)
            /*
            EI that might need a bit more work
            Sign of Ill Omen - create a feature set that adds converted versions of the subspells
            Tomb of Levistus - add tempHP and then apply incapcitated or stunned status
            Undying Servitude - summon a a skeleton or zombie
            */
            .AddFeatureSet(
                DictionaryofEIPowers["MiretheMind"],
                DictionaryofEIAttributeModifers["EldritchSmite"],
                DictionaryofEIAttributeModifers["ThirstingBlade"],
                DictionaryofEIAttributeModifers["ImprovedPactWeapon"]
            )
            .SetDynamicFeatureSetFunc(InvocationsFilteredFeatureSet)
            .AddToDB();
        #endregion

        #region WarlockEldritchInvocationSetLevel7
        private static FeatureDefinitionFeatureSetUniqueAcross warlockEldritchInvocationSetLevel7;
        public static FeatureDefinitionFeatureSetUniqueAcross WarlockEldritchInvocationSetLevel7 => warlockEldritchInvocationSetLevel7 ??= FeatureDefinitionFeatureSetUniqueAcrossBuilder
            .Create(WarlockEldritchInvocationSetLevel5, "ClassWarlockEldritchInvocationSetLevel7", CENamespaceGuid)
            .AddFeatureSet(
                DictionaryofEIAttributeModifers["OneWithShadows"],
                DictionaryofEIPowers["DreadfulWord"],
                DictionaryofEIPowers["Trickster'sEscape"]
            )
            .SetDynamicFeatureSetFunc(InvocationsFilteredFeatureSet)
            .AddToDB();
        #endregion

        #region WarlockEldritchInvocationSetLevel9
        private static FeatureDefinitionFeatureSetUniqueAcross warlockEldritchInvocationSetLevel9;
        public static FeatureDefinitionFeatureSetUniqueAcross WarlockEldritchInvocationSetLevel9 => warlockEldritchInvocationSetLevel9 ??= FeatureDefinitionFeatureSetUniqueAcrossBuilder
            .Create(WarlockEldritchInvocationSetLevel7, "ClassWarlockEldritchInvocationSetLevel9", CENamespaceGuid)
            .AddFeatureSet(
                DictionaryofEIPowers["AscendantStep"],
                DictionaryofEIPowers["OtherworldlyLeap"],
                DictionaryofEIAttributeModifers["GiftoftheProtectors"]
            )
            .SetDynamicFeatureSetFunc(InvocationsFilteredFeatureSet)
            .AddToDB();
        #endregion

        #region WarlockEldritchInvocationSetLevel12
        private static FeatureDefinitionFeatureSetUniqueAcross warlockEldritchInvocationSetLevel12;
        public static FeatureDefinitionFeatureSetUniqueAcross WarlockEldritchInvocationSetLevel12 => warlockEldritchInvocationSetLevel12 ??= FeatureDefinitionFeatureSetUniqueAcrossBuilder
            .Create(WarlockEldritchInvocationSetLevel9, "ClassWarlockEldritchInvocationSetLevel12", CENamespaceGuid)
            .AddFeatureSet(
                DictionaryofEIAttributeModifers["BondoftheTalisman"]
            )
            .SetDynamicFeatureSetFunc(InvocationsFilteredFeatureSet)
            .AddToDB();
        #endregion

        #region WarlockEldritchInvocationSetLevel15
        private static FeatureDefinitionFeatureSetUniqueAcross warlockEldritchInvocationSetLevel15;
        public static FeatureDefinitionFeatureSetUniqueAcross WarlockEldritchInvocationSetLevel15 => warlockEldritchInvocationSetLevel15 ??= FeatureDefinitionFeatureSetUniqueAcrossBuilder
            .Create(WarlockEldritchInvocationSetLevel12, "ClassWarlockEldritchInvocationSetLevel15", CENamespaceGuid)
            .AddFeatureSet(
                /*
                *EI that  more work
                Master of Myriad Forms - would need to create the alter self spell then convert it
                */
                DictionaryofEIPowers["ChainsofCarceri"],
                DictionaryofEIPowers["ShroudofShadow"],
                DictionaryofEIAttributeModifers["WitchSight"]
            )
            .SetDynamicFeatureSetFunc(InvocationsFilteredFeatureSet)
            .AddToDB();
        #endregion

        #region WarlockEldritchInvocationSetLevel18
        private static FeatureDefinitionFeatureSetUniqueAcross warlockEldritchInvocationSetLevel18;
        public static FeatureDefinitionFeatureSetUniqueAcross WarlockEldritchInvocationSetLevel18 => warlockEldritchInvocationSetLevel18 ??= FeatureDefinitionFeatureSetUniqueAcrossBuilder
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
