using System;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi;
using SolastaModApi.Extensions;
using static SolastaCommunityExpansion.Builders.DefinitionBuilder;
using static SolastaCommunityExpansion.Classes.Warlock.Features.EldritchInvocationsBuilder;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionFeatureSets;

namespace SolastaCommunityExpansion.Classes.Warlock.Features
{
    internal partial class WarlockFeatures
    {
        #region WarlockEldritchInvocationSetLevel2
        private static FeatureDefinitionFeatureSet warlockEldritchInvocationSetLevel2;
        public static FeatureDefinitionFeatureSet WarlockEldritchInvocationSetLevel2 => warlockEldritchInvocationSetLevel2 ??= FeatureDefinitionFeatureSetBuilder
            .Create(TerrainTypeAffinityRangerNaturalExplorerChoice, "ClassWarlockEldritchInvocationSetLevel2", CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
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
                DictionaryofEIAttributeModifers["GiftoftheEver-LivingOnes"],
                DictionaryofEIAttributeModifers["ImprovedPactWeapon"]
            )
            //ACEHIGH NOTE : Seems to be a bug with unique choices where it makes the list smaller but then selects from the wrong index
            //from the master list, using the index of the item in the smaller list.  My tests on higher levels would have
            //RepellingBlast chosen from the master list when choosing ThirstingBlade from the smaller unique list as an example.
            .SetUniqueChoices(false)
            .AddToDB();
        #endregion

        #region WarlockEldritchInvocationSetLevel5
        private static FeatureDefinitionFeatureSet warlockEldritchInvocationSetLevel5;
        public static FeatureDefinitionFeatureSet WarlockEldritchInvocationSetLevel5 => warlockEldritchInvocationSetLevel5 ??= FeatureDefinitionFeatureSetBuilder
            .Create(WarlockEldritchInvocationSetLevel2, "ClassWarlockEldritchInvocationSetLevel5", CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            /*
                EI that might need a bit more work
                Sign of Ill Omen - create a feature set that adds converted versions of the subspells
                Tomb of Levistus - add tempHP and then apply incapcitated or stunned status
                Undying Servitude - summon a a skeleton or zombie

            */
            .SetFeatureSet(
                DictionaryofEIPowers["MiretheMind"],
                DictionaryofEIAttributeModifers["EldritchSmite"],
                DictionaryofEIAttributeModifers["ThirstingBlade"]
            )
            .SetUniqueChoices(false)
            .AddToDB();
        #endregion
    }

    internal class WarlockEldritchInvocationSetBuilderLevel7 : FeatureDefinitionFeatureSetBuilder
    {
        private const string WarlockEldritchInvocationSetLevel7Name = "ClassWarlockEldritchInvocationSetLevel7";

        protected WarlockEldritchInvocationSetBuilderLevel7(string name) : base(WarlockFeatures.WarlockEldritchInvocationSetLevel5, name)
        {
            Definition.GuiPresentation.Title = "Feature/&ClassWarlockEldritchInvocationSetLevelTitle";
            Definition.GuiPresentation.Description = "Feature/&ClassWarlockEldritchInvocationSetLevelDescription";


            Definition.FeatureSet.Add(DictionaryofEIAttributeModifers["OneWithShadows"]);
            Definition.FeatureSet.Add(DictionaryofEIPowers["DreadfulWord"]);
            Definition.FeatureSet.Add(DictionaryofEIPowers["Trickster'sEscape"]);
            Definition.SetUniqueChoices(false);
        }

        public static FeatureDefinitionFeatureSet CreateAndAddToDB(string name)
        {
            return new WarlockEldritchInvocationSetBuilderLevel7(name).AddToDB();
        }

        public static readonly FeatureDefinitionFeatureSet WarlockEldritchInvocationSetLevel7 = CreateAndAddToDB(WarlockEldritchInvocationSetLevel7Name);
    }

    internal class WarlockEldritchInvocationSetBuilderLevel9 : FeatureDefinitionFeatureSetBuilder
    {
        private const string WarlockEldritchInvocationSetLevel9Name = "ClassWarlockEldritchInvocationSetLevel9";

        protected WarlockEldritchInvocationSetBuilderLevel9(string name) : base(WarlockFeatures.WarlockEldritchInvocationSetLevel5, name)
        {
            Definition.GuiPresentation.Title = "Feature/&ClassWarlockEldritchInvocationSetLevelTitle";
            Definition.GuiPresentation.Description = "Feature/&ClassWarlockEldritchInvocationSetLevelDescription";

            /*
                *EI that  might need a bit more work
                Minions of Chaos - cant convert subspells , would need to use a feature set of powers conjure elemental
            */

            Definition.FeatureSet.Add(DictionaryofEIPowers["AscendantStep"]);
            Definition.FeatureSet.Add(DictionaryofEIPowers["OtherworldlyLeap"]);
            Definition.FeatureSet.Add(DictionaryofEIAttributeModifers["GiftoftheProtectors"]);


            Definition.SetUniqueChoices(false);
        }

        public static FeatureDefinitionFeatureSet CreateAndAddToDB(string name)
        {
            return new WarlockEldritchInvocationSetBuilderLevel9(name).AddToDB();
        }

        public static readonly FeatureDefinitionFeatureSet WarlockEldritchInvocationSetLevel9 = CreateAndAddToDB(WarlockEldritchInvocationSetLevel9Name);
    }


    internal class WarlockEldritchInvocationSetBuilderLevel12 : FeatureDefinitionFeatureSetBuilder
    {
        private const string WarlockEldritchInvocationSetLevel12Name = "ClassWarlockEldritchInvocationSetLevel12";

        protected WarlockEldritchInvocationSetBuilderLevel12(string name) : base(WarlockFeatures.WarlockEldritchInvocationSetLevel5, name)
        {
            Definition.GuiPresentation.Title = "Feature/&ClassWarlockEldritchInvocationSetLevelTitle";
            Definition.GuiPresentation.Description = "Feature/&ClassWarlockEldritchInvocationSetLevelDescription";


            /*
                *EI that should be doable
                Lifedrinker - similar to AdditionalDamageDomainOblivionStrikeOblivion + damageValueDetermination = RuleDefinitions.AdditionalDamageValueDetermination.SpellcastingBonus;
            */


            Definition.FeatureSet.Add(DictionaryofEIAttributeModifers["BondoftheTalisman"]);

            Definition.SetUniqueChoices(false);
        }

        public static FeatureDefinitionFeatureSet CreateAndAddToDB(string name)
        {
            return new WarlockEldritchInvocationSetBuilderLevel12(name).AddToDB();
        }

        public static readonly FeatureDefinitionFeatureSet WarlockEldritchInvocationSetLevel12 = CreateAndAddToDB(WarlockEldritchInvocationSetLevel12Name);
    }


    internal class WarlockEldritchInvocationSetBuilderLevel15 : FeatureDefinitionFeatureSetBuilder
    {
        private const string WarlockEldritchInvocationSetLevel15Name = "ClassWarlockEldritchInvocationSetLevel15";

        protected WarlockEldritchInvocationSetBuilderLevel15(string name) : base(WarlockFeatures.WarlockEldritchInvocationSetLevel5, name)
        {
            Definition.GuiPresentation.Title = "Feature/&ClassWarlockEldritchInvocationSetLevelTitle";
            Definition.GuiPresentation.Description = "Feature/&ClassWarlockEldritchInvocationSetLevelDescription";

            /*
                *EI that  more work
                Master of Myriad Forms - would need to create the alter self spell then convert it

            */

            Definition.FeatureSet.Add(DictionaryofEIPowers["ChainsofCarceri"]);
            Definition.FeatureSet.Add(DictionaryofEIPowers["ShroudofShadow"]);
            Definition.FeatureSet.Add(DictionaryofEIAttributeModifers["WitchSight"]);

            Definition.SetUniqueChoices(false);
        }

        public static FeatureDefinitionFeatureSet CreateAndAddToDB(string name)
        {
            return new WarlockEldritchInvocationSetBuilderLevel15(name).AddToDB();
        }

        public static readonly FeatureDefinitionFeatureSet WarlockEldritchInvocationSetLevel15 = CreateAndAddToDB(WarlockEldritchInvocationSetLevel15Name);
    }


    internal class WarlockEldritchInvocationSetBuilderLevel18 : FeatureDefinitionFeatureSetBuilder
    {
        private const string WarlockEldritchInvocationSetLevel18Name = "ClassWarlockEldritchInvocationSetLevel18";

        protected WarlockEldritchInvocationSetBuilderLevel18(string name) : base(WarlockFeatures.WarlockEldritchInvocationSetLevel5, name)
        {
            Definition.GuiPresentation.Title = "Feature/&ClassWarlockEldritchInvocationSetLevelTitle";
            Definition.GuiPresentation.Description = "Feature/&ClassWarlockEldritchInvocationSetLevelDescription";


            Definition.SetUniqueChoices(false);
        }

        public static FeatureDefinitionFeatureSet CreateAndAddToDB(string name)
        {
            return new WarlockEldritchInvocationSetBuilderLevel18(name).AddToDB();
        }

        public static readonly FeatureDefinitionFeatureSet WarlockEldritchInvocationSetLevel18 = CreateAndAddToDB(WarlockEldritchInvocationSetLevel18Name);
    }
}
