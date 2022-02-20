using System;
using System.Collections.Generic;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaCommunityExpansion.Builders.Features;

namespace SolastaCommunityExpansion.Classes.Warlock.Features
{
    internal class WarlockMysticArcanumSetBuilderLevel11 : BaseDefinitionBuilder<FeatureDefinitionFeatureSet>
    {
        private const string WarlockMysticArcanumSetLevel11Name = "ClassWarlockMysticArcanumSetLevel11";
        private static readonly string WarlockMysticArcanumSetLevel11Guid = GuidHelper.Create(new Guid(Settings.GUID), WarlockMysticArcanumSetLevel11Name).ToString();

        [Obsolete]
        protected WarlockMysticArcanumSetBuilderLevel11(string name, string guid) : base(DatabaseHelper.FeatureDefinitionFeatureSets.TerrainTypeAffinityRangerNaturalExplorerChoice, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&ClassWarlockMysticArcanumSetTitle";
            Definition.GuiPresentation.Description = "Feature/&ClassWarlockMysticArcanumSetDescription";
            Definition.FeatureSet.Clear();

            var spelllist = new List<SpellDefinition>();
            spelllist.AddRange(WarlockSpellList.ClassWarlockSpellList.SpellsByLevel[6].Spells);

            foreach (SpellDefinition spell in spelllist)
            {
                var MysticArcanumBuilder = new FeatureDefinitionPowerBuilder(
                        "DH_MysticArcanum11_" + spell.name,
                        GuidHelper.Create(new Guid(Settings.GUID), "DH_MysticArcanum11_" + spell.name).ToString(),
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
                       spell.GuiPresentation,
                        true);
                FeatureDefinitionPower MysticArcanum = MysticArcanumBuilder.AddToDB();


                Definition.FeatureSet.Add(MysticArcanum);
            }
            Definition.SetUniqueChoices(true);
        }

        [Obsolete]
        public static FeatureDefinitionFeatureSet CreateAndAddToDB(string name, string guid)
        {
            return new WarlockMysticArcanumSetBuilderLevel11(name, guid).AddToDB();
        }

        [Obsolete]
        public static FeatureDefinitionFeatureSet WarlockMysticArcanumSetLevel11 = CreateAndAddToDB(WarlockMysticArcanumSetLevel11Name, WarlockMysticArcanumSetLevel11Guid);
    }

    internal class WarlockMysticArcanumSetBuilderLevel13 : BaseDefinitionBuilder<FeatureDefinitionFeatureSet>
    {
        private const string WarlockMysticArcanumSetLevel13Name = "ClassWarlockMysticArcanumSetLevel13";
        private static readonly string WarlockMysticArcanumSetLevel13Guid = GuidHelper.Create(new Guid(Settings.GUID), WarlockMysticArcanumSetLevel13Name).ToString();

        [Obsolete]
        protected WarlockMysticArcanumSetBuilderLevel13(string name, string guid) : base(DatabaseHelper.FeatureDefinitionFeatureSets.TerrainTypeAffinityRangerNaturalExplorerChoice, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&ClassWarlockMysticArcanumSetTitle";
            Definition.GuiPresentation.Description = "Feature/&ClassWarlockMysticArcanumSetDescription";
            Definition.FeatureSet.Clear();

            var spelllist = new List<SpellDefinition>();
            spelllist.AddRange(WarlockSpellList.ClassWarlockSpellList.SpellsByLevel[7].Spells);
            spelllist.AddRange(WarlockSpellList.ClassWarlockSpellList.SpellsByLevel[6].Spells);

            foreach (SpellDefinition spell in spelllist)
            {
                var MysticArcanumBuilder = new FeatureDefinitionPowerBuilder(
                        "DH_MysticArcanum13_" + spell.name,
                        GuidHelper.Create(new Guid(Settings.GUID), "DH_MysticArcanum13_" + spell.name).ToString(),
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
                       spell.GuiPresentation,
                        true);
                FeatureDefinitionPower MysticArcanum = MysticArcanumBuilder.AddToDB();


                Definition.FeatureSet.Add(MysticArcanum);
            }
            Definition.SetUniqueChoices(true);
        }

        [Obsolete]
        public static FeatureDefinitionFeatureSet CreateAndAddToDB(string name, string guid)
        {
            return new WarlockMysticArcanumSetBuilderLevel13(name, guid).AddToDB();
        }

        [Obsolete]
        public static FeatureDefinitionFeatureSet WarlockMysticArcanumSetLevel13 = CreateAndAddToDB(WarlockMysticArcanumSetLevel13Name, WarlockMysticArcanumSetLevel13Guid);
    }


    internal class WarlockMysticArcanumSetBuilderLevel15 : BaseDefinitionBuilder<FeatureDefinitionFeatureSet>
    {
        private const string WarlockMysticArcanumSetLevel15Name = "ClassWarlockMysticArcanumSetLevel15";
        private static readonly string WarlockMysticArcanumSetLevel15Guid = GuidHelper.Create(new Guid(Settings.GUID), WarlockMysticArcanumSetLevel15Name).ToString();

        [Obsolete]
        protected WarlockMysticArcanumSetBuilderLevel15(string name, string guid) : base(DatabaseHelper.FeatureDefinitionFeatureSets.TerrainTypeAffinityRangerNaturalExplorerChoice, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&ClassWarlockMysticArcanumSetTitle";
            Definition.GuiPresentation.Description = "Feature/&ClassWarlockMysticArcanumSetDescription";
            Definition.FeatureSet.Clear();

            var spelllist = new List<SpellDefinition>();
            spelllist.AddRange(WarlockSpellList.ClassWarlockSpellList.SpellsByLevel[8].Spells);
            spelllist.AddRange(WarlockSpellList.ClassWarlockSpellList.SpellsByLevel[7].Spells);
            spelllist.AddRange(WarlockSpellList.ClassWarlockSpellList.SpellsByLevel[6].Spells);

            foreach (SpellDefinition spell in spelllist)
            {
                var MysticArcanumBuilder = new FeatureDefinitionPowerBuilder(
                        "DH_MysticArcanum15_" + spell.name,
                        GuidHelper.Create(new Guid(Settings.GUID), "DH_MysticArcanum15_" + spell.name).ToString(),
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
                       spell.GuiPresentation,
                        true);
                FeatureDefinitionPower MysticArcanum = MysticArcanumBuilder.AddToDB();


                Definition.FeatureSet.Add(MysticArcanum);
            }
            Definition.SetUniqueChoices(true);
        }

        [Obsolete]
        public static FeatureDefinitionFeatureSet CreateAndAddToDB(string name, string guid)
        {
            return new WarlockMysticArcanumSetBuilderLevel15(name, guid).AddToDB();
        }

        [Obsolete]
        public static FeatureDefinitionFeatureSet WarlockMysticArcanumSetLevel15 = CreateAndAddToDB(WarlockMysticArcanumSetLevel15Name, WarlockMysticArcanumSetLevel15Guid);
    }

    internal class WarlockMysticArcanumSetBuilderLevel17 : BaseDefinitionBuilder<FeatureDefinitionFeatureSet>
    {
        private const string WarlockMysticArcanumSetLevel17Name = "ClassWarlockMysticArcanumSetLevel17";
        private static readonly string WarlockMysticArcanumSetLevel17Guid = GuidHelper.Create(new Guid(Settings.GUID), WarlockMysticArcanumSetLevel17Name).ToString();

        [Obsolete]
        protected WarlockMysticArcanumSetBuilderLevel17(string name, string guid) : base(DatabaseHelper.FeatureDefinitionFeatureSets.TerrainTypeAffinityRangerNaturalExplorerChoice, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&ClassWarlockMysticArcanumSetTitle";
            Definition.GuiPresentation.Description = "Feature/&ClassWarlockMysticArcanumSetDescription";
            Definition.FeatureSet.Clear();

            var spelllist =  new List<SpellDefinition>();
            spelllist.AddRange(WarlockSpellList.ClassWarlockSpellList.SpellsByLevel[9].Spells);
            spelllist.AddRange(WarlockSpellList.ClassWarlockSpellList.SpellsByLevel[8].Spells);
            spelllist.AddRange(WarlockSpellList.ClassWarlockSpellList.SpellsByLevel[7].Spells);
            spelllist.AddRange(WarlockSpellList.ClassWarlockSpellList.SpellsByLevel[6].Spells);

            foreach (SpellDefinition spell in spelllist)
            {
                var MysticArcanumBuilder = new FeatureDefinitionPowerBuilder(
                        "DH_MysticArcanum17_" + spell.name,
                        GuidHelper.Create(new Guid(Settings.GUID), "DH_MysticArcanum17_" + spell.name).ToString(),
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
                       spell.GuiPresentation,
                        true);
                FeatureDefinitionPower MysticArcanum = MysticArcanumBuilder.AddToDB();


                Definition.FeatureSet.Add(MysticArcanum);
            }
            Definition.SetUniqueChoices(true);
        }

        [Obsolete]
        public static FeatureDefinitionFeatureSet CreateAndAddToDB(string name, string guid)
        {
            return new WarlockMysticArcanumSetBuilderLevel17(name, guid).AddToDB();
        }

        [Obsolete]
        public static FeatureDefinitionFeatureSet WarlockMysticArcanumSetLevel17 = CreateAndAddToDB(WarlockMysticArcanumSetLevel17Name, WarlockMysticArcanumSetLevel17Guid);
    }
}
