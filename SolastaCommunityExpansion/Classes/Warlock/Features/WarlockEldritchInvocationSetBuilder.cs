using System;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Classes.Warlock.Features
{
    internal class WarlockEldritchInvocationSetBuilderLevel2 : FeatureDefinitionFeatureSetBuilder
    {
        private const string WarlockEldritchInvocationSetLevel2Name = "ClassWarlockEldritchInvocationSetLevel2";
        private static readonly string WarlockEldritchInvocationSetLevel2Guid = GuidHelper.Create(new Guid(Settings.GUID), WarlockEldritchInvocationSetLevel2Name).ToString();

        protected WarlockEldritchInvocationSetBuilderLevel2(string name, string guid) : base(DatabaseHelper.FeatureDefinitionFeatureSets.TerrainTypeAffinityRangerNaturalExplorerChoice, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&ClassWarlockEldritchInvocationSetLevelTitle";
            Definition.GuiPresentation.Description = "Feature/&ClassWarlockEldritchInvocationSetLevelDescription";

            Definition.FeatureSet.Clear();


            /*
             *  EI that   might need a bit more work
            	Investment of the Chain Master - multiple featurs through summoning affinity
             *  Book of ancient secrets - similar to MagicAffinityWizardRitualCasting or ritual casting feat
            */


            Definition.FeatureSet.Add(DHEldritchInvocationsBuilder.AgonizingBlastFeatureSet);
            Definition.FeatureSet.Add(DHEldritchInvocationsBuilder.HinderingBlastFeatureSet);//DictionaryofEBInvocations["HinderingBlast"]);
            Definition.FeatureSet.Add(DHEldritchInvocationsBuilder.DictionaryofEBInvocations["RepellingBlast"]);
            Definition.FeatureSet.Add(DHEldritchInvocationsBuilder.DictionaryofEBInvocations["GraspingHand"]);


            Definition.FeatureSet.Add(DHEldritchInvocationsBuilder.DictionaryofEIPowers["ArmorofShadows"]);
            Definition.FeatureSet.Add(DHEldritchInvocationsBuilder.DictionaryofEIPowers["EldritchSight"]);
            Definition.FeatureSet.Add(DHEldritchInvocationsBuilder.DictionaryofEIPowers["FiendishVigor"]);
            Definition.FeatureSet.Add(DHEldritchInvocationsBuilder.DictionaryofEIPowers["ThiefofFiveFates"]);


            Definition.FeatureSet.Add(DHEldritchInvocationsBuilder.DictionaryofEIAttributeModifers["AspectoftheMoon"]);
            Definition.FeatureSet.Add(DHEldritchInvocationsBuilder.DictionaryofEIAttributeModifers["BeguilingInfluence"]);
            Definition.FeatureSet.Add(DHEldritchInvocationsBuilder.DictionaryofEIAttributeModifers["EldritchMind"]);
            Definition.FeatureSet.Add(DHEldritchInvocationsBuilder.DictionaryofEIAttributeModifers["EyesoftheRuneKeeper"]);
            Definition.FeatureSet.Add(DHEldritchInvocationsBuilder.DictionaryofEIAttributeModifers["GiftoftheEver-LivingOnes"]);
            Definition.FeatureSet.Add(DHEldritchInvocationsBuilder.DictionaryofEIAttributeModifers["ImprovedPactWeapon"]);


            Definition.SetUniqueChoices(false); //ACEHIGH NOTE : Seems to be a bug with unique choices where it makes the list smaller but then selects from the wrong index from the master list, using the index of the item in the smaller list.  My tests on higher levels would have RepellingBlast chosen from the master list when choosing ThirstingBlade from the smaller unique list as an example.
        }

        public static FeatureDefinitionFeatureSet CreateAndAddToDB(string name, string guid)
        {
            return new WarlockEldritchInvocationSetBuilderLevel2(name, guid).AddToDB();
        }

        public static FeatureDefinitionFeatureSet WarlockEldritchInvocationSetLevel2 = CreateAndAddToDB(WarlockEldritchInvocationSetLevel2Name, WarlockEldritchInvocationSetLevel2Guid);
    }



    internal class WarlockEldritchInvocationSetBuilderLevel5 : FeatureDefinitionFeatureSetBuilder
    {
        private const string WarlockEldritchInvocationSetLevel5Name = "ClassWarlockEldritchInvocationSetLevel5";
        private static readonly string WarlockEldritchInvocationSetLevel5Guid = GuidHelper.Create(new Guid(Settings.GUID), WarlockEldritchInvocationSetLevel5Name).ToString();

        protected WarlockEldritchInvocationSetBuilderLevel5(string name, string guid) : base(WarlockEldritchInvocationSetBuilderLevel2.WarlockEldritchInvocationSetLevel2, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&ClassWarlockEldritchInvocationSetLevelTitle";
            Definition.GuiPresentation.Description = "Feature/&ClassWarlockEldritchInvocationSetLevelDescription";

            /*
                * EI that   might need a bit more work
                One with Shadows - PowerSorakAssassinShadowMurder(copy but remove second effect) or covert invisibility spell effects (buffed because cant limit to dim light/darkness without patch)
                Sign of Ill Omen - create a feature set that adds converted versions of the subspells
                Tomb of Levistus - add tempHP and then apply incapcitated or stunned status
                Undying Servitude - summon a a skeleton or zombie

            */


            Definition.FeatureSet.Add(DHEldritchInvocationsBuilder.DictionaryofEIPowers["MiretheMind"]);
        //    Definition.FeatureSet.Add(DHEldritchInvocationsBuilder.DictionaryofEIPowers["SignofIllOmen"]);
            Definition.FeatureSet.Add(DHEldritchInvocationsBuilder.DictionaryofEIAttributeModifers["EldritchSmite"]);
            Definition.FeatureSet.Add(DHEldritchInvocationsBuilder.DictionaryofEIAttributeModifers["ThirstingBlade"]);

            Definition.SetUniqueChoices(false);
        }

        public static FeatureDefinitionFeatureSet CreateAndAddToDB(string name, string guid)
        {
            return new WarlockEldritchInvocationSetBuilderLevel5(name, guid).AddToDB();
        }

        public static FeatureDefinitionFeatureSet WarlockEldritchInvocationSetLevel5 = CreateAndAddToDB(WarlockEldritchInvocationSetLevel5Name, WarlockEldritchInvocationSetLevel5Guid);
    }


    internal class WarlockEldritchInvocationSetBuilderLevel7 : FeatureDefinitionFeatureSetBuilder
    {
        private const string WarlockEldritchInvocationSetLevel7Name = "ClassWarlockEldritchInvocationSetLevel7";
        private static readonly string WarlockEldritchInvocationSetLevel7Guid = GuidHelper.Create(new Guid(Settings.GUID), WarlockEldritchInvocationSetLevel7Name).ToString();

        protected WarlockEldritchInvocationSetBuilderLevel7(string name, string guid) : base(WarlockEldritchInvocationSetBuilderLevel5.WarlockEldritchInvocationSetLevel5, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&ClassWarlockEldritchInvocationSetLevelTitle";
            Definition.GuiPresentation.Description = "Feature/&ClassWarlockEldritchInvocationSetLevelDescription";

            Definition.FeatureSet.Add(DHEldritchInvocationsBuilder.DictionaryofEIPowers["DreadfulWord"]);
            Definition.FeatureSet.Add(DHEldritchInvocationsBuilder.DictionaryofEIPowers["Trickster'sEscape"]);
            Definition.SetUniqueChoices(false);
        }

        public static FeatureDefinitionFeatureSet CreateAndAddToDB(string name, string guid)
        {
            return new WarlockEldritchInvocationSetBuilderLevel7(name, guid).AddToDB();
        }

        public static FeatureDefinitionFeatureSet WarlockEldritchInvocationSetLevel7 = CreateAndAddToDB(WarlockEldritchInvocationSetLevel7Name, WarlockEldritchInvocationSetLevel7Guid);
    }


    internal class WarlockEldritchInvocationSetBuilderLevel9 : FeatureDefinitionFeatureSetBuilder
    {
        private const string WarlockEldritchInvocationSetLevel9Name = "ClassWarlockEldritchInvocationSetLevel9";
        private static readonly string WarlockEldritchInvocationSetLevel9Guid = GuidHelper.Create(new Guid(Settings.GUID), WarlockEldritchInvocationSetLevel9Name).ToString();

        protected WarlockEldritchInvocationSetBuilderLevel9(string name, string guid) : base(WarlockEldritchInvocationSetBuilderLevel5.WarlockEldritchInvocationSetLevel5, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&ClassWarlockEldritchInvocationSetLevelTitle";
            Definition.GuiPresentation.Description = "Feature/&ClassWarlockEldritchInvocationSetLevelDescription";

            /*
                *EI that  might need a bit more work
                Minions of Chaos - cant convert subspells , would need to use a feature set of powers conjure elemental
            */

            Definition.FeatureSet.Add(DHEldritchInvocationsBuilder.DictionaryofEIPowers["AscendantStep"]);
            Definition.FeatureSet.Add(DHEldritchInvocationsBuilder.DictionaryofEIPowers["OtherworldlyLeap"]);
            Definition.FeatureSet.Add(DHEldritchInvocationsBuilder.DictionaryofEIAttributeModifers["GiftoftheProtectors"]);


            Definition.SetUniqueChoices(false);
        }

        public static FeatureDefinitionFeatureSet CreateAndAddToDB(string name, string guid)
        {
            return new WarlockEldritchInvocationSetBuilderLevel9(name, guid).AddToDB();
        }

        public static FeatureDefinitionFeatureSet WarlockEldritchInvocationSetLevel9 = CreateAndAddToDB(WarlockEldritchInvocationSetLevel9Name, WarlockEldritchInvocationSetLevel9Guid);
    }


    internal class WarlockEldritchInvocationSetBuilderLevel12 : FeatureDefinitionFeatureSetBuilder
    {
        private const string WarlockEldritchInvocationSetLevel12Name = "ClassWarlockEldritchInvocationSetLevel12";
        private static readonly string WarlockEldritchInvocationSetLevel12Guid = GuidHelper.Create(new Guid(Settings.GUID), WarlockEldritchInvocationSetLevel12Name).ToString();

        protected WarlockEldritchInvocationSetBuilderLevel12(string name, string guid) : base(WarlockEldritchInvocationSetBuilderLevel5.WarlockEldritchInvocationSetLevel5, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&ClassWarlockEldritchInvocationSetLevelTitle";
            Definition.GuiPresentation.Description = "Feature/&ClassWarlockEldritchInvocationSetLevelDescription";


            /*
                *EI that should be doable
                Lifedrinker - similar to AdditionalDamageDomainOblivionStrikeOblivion + damageValueDetermination = RuleDefinitions.AdditionalDamageValueDetermination.SpellcastingBonus;
            */


            Definition.FeatureSet.Add(DHEldritchInvocationsBuilder.DictionaryofEIAttributeModifers["BondoftheTalisman"]);

            Definition.SetUniqueChoices(false);
        }

        public static FeatureDefinitionFeatureSet CreateAndAddToDB(string name, string guid)
        {
            return new WarlockEldritchInvocationSetBuilderLevel12(name, guid).AddToDB();
        }

        public static FeatureDefinitionFeatureSet WarlockEldritchInvocationSetLevel12 = CreateAndAddToDB(WarlockEldritchInvocationSetLevel12Name, WarlockEldritchInvocationSetLevel12Guid);
    }


    internal class WarlockEldritchInvocationSetBuilderLevel15 : FeatureDefinitionFeatureSetBuilder
    {
        private const string WarlockEldritchInvocationSetLevel15Name = "ClassWarlockEldritchInvocationSetLevel15";
        private static readonly string WarlockEldritchInvocationSetLevel15Guid = GuidHelper.Create(new Guid(Settings.GUID), WarlockEldritchInvocationSetLevel15Name).ToString();

        protected WarlockEldritchInvocationSetBuilderLevel15(string name, string guid) : base(WarlockEldritchInvocationSetBuilderLevel5.WarlockEldritchInvocationSetLevel5, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&ClassWarlockEldritchInvocationSetLevelTitle";
            Definition.GuiPresentation.Description = "Feature/&ClassWarlockEldritchInvocationSetLevelDescription";

            /*
                *EI that  more work
                Master of Myriad Forms - would need to create the alter self spell then convert it

            */

            Definition.FeatureSet.Add(DHEldritchInvocationsBuilder.DictionaryofEIPowers["ChainsofCarceri"]);
            Definition.FeatureSet.Add(DHEldritchInvocationsBuilder.DictionaryofEIPowers["ShroudofShadow"]);
            Definition.FeatureSet.Add(DHEldritchInvocationsBuilder.DictionaryofEIAttributeModifers["WitchSight"]);

            Definition.SetUniqueChoices(false);
        }

        public static FeatureDefinitionFeatureSet CreateAndAddToDB(string name, string guid)
        {
            return new WarlockEldritchInvocationSetBuilderLevel15(name, guid).AddToDB();
        }

        public static FeatureDefinitionFeatureSet WarlockEldritchInvocationSetLevel15 = CreateAndAddToDB(WarlockEldritchInvocationSetLevel15Name, WarlockEldritchInvocationSetLevel15Guid);
    }


    internal class WarlockEldritchInvocationSetBuilderLevel18 : FeatureDefinitionFeatureSetBuilder
    {
        private const string WarlockEldritchInvocationSetLevel18Name = "ClassWarlockEldritchInvocationSetLevel18";
        private static readonly string WarlockEldritchInvocationSetLevel18Guid = GuidHelper.Create(new Guid(Settings.GUID), WarlockEldritchInvocationSetLevel18Name).ToString();

        protected WarlockEldritchInvocationSetBuilderLevel18(string name, string guid) : base(WarlockEldritchInvocationSetBuilderLevel5.WarlockEldritchInvocationSetLevel5, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&ClassWarlockEldritchInvocationSetLevelTitle";
            Definition.GuiPresentation.Description = "Feature/&ClassWarlockEldritchInvocationSetLevelDescription";


            Definition.SetUniqueChoices(false);
        }

        public static FeatureDefinitionFeatureSet CreateAndAddToDB(string name, string guid)
        {
            return new WarlockEldritchInvocationSetBuilderLevel18(name, guid).AddToDB();
        }

        public static FeatureDefinitionFeatureSet WarlockEldritchInvocationSetLevel18 = CreateAndAddToDB(WarlockEldritchInvocationSetLevel18Name, WarlockEldritchInvocationSetLevel18Guid);
    }


}
