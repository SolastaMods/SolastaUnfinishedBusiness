using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.Builders;
using TA.AI;
using UnityEngine;
using static SolastaCommunityExpansion.Spells.SrdSpells;

namespace SolastaCommunityExpansion.Monsters
{
    public static class NewMonsterAttributes
    {
        public static SpellListDefinition Archmage_spelllist { get; private set; } = ScriptableObject.CreateInstance<SpellListDefinition>();
        public static FeatureDefinitionCastSpell CastSpell_ArchMage { get; private set; } = ScriptableObject.CreateInstance<FeatureDefinitionCastSpell>();
        public static SpellListDefinition GuardianNaga_spelllist { get; private set; } = ScriptableObject.CreateInstance<SpellListDefinition>();
        public static FeatureDefinitionCastSpell CastSpell_GuardianNaga { get; private set; } = ScriptableObject.CreateInstance<FeatureDefinitionCastSpell>();
        public static SpellListDefinition Lich_spelllist { get; private set; } = ScriptableObject.CreateInstance<SpellListDefinition>();
        public static FeatureDefinitionCastSpell CastSpell_Lich { get; private set; } = ScriptableObject.CreateInstance<FeatureDefinitionCastSpell>();

        public static FeatureDefinitionDamageAffinity FireTitan_Retaliate_DamageAffinity { get; private set; } = ScriptableObject.CreateInstance<FeatureDefinitionDamageAffinity>();
        public static FeatureDefinitionDamageAffinity Balor_Retaliate_DamageAffinity { get; private set; } = ScriptableObject.CreateInstance<FeatureDefinitionDamageAffinity>();
        public static FeatureDefinitionMagicAffinity AirTitan_SleetStorm_Immunity { get; private set; } = ScriptableObject.CreateInstance<FeatureDefinitionMagicAffinity>();
        public static FeatureDefinitionMagicAffinity TarrasqueReflectiveCarapace { get; private set; } = ScriptableObject.CreateInstance<FeatureDefinitionMagicAffinity>();

        public static DecisionPackageDefinition AncientDragon_CombatDecisions { get; private set; } = ScriptableObject.CreateInstance<DecisionPackageDefinition>();
        public static DecisionPackageDefinition PitFiend_CombatDecisions { get; private set; } = ScriptableObject.CreateInstance<DecisionPackageDefinition>();
        public static DecisionPackageDefinition Balor_CombatDecisions { get; private set; } = ScriptableObject.CreateInstance<DecisionPackageDefinition>();
        public static DecisionPackageDefinition Nalfeshnee_CombatDecisions { get; private set; } = ScriptableObject.CreateInstance<DecisionPackageDefinition>();
        public static DecisionPackageDefinition Solar_CombatDecisions { get; private set; } = ScriptableObject.CreateInstance<DecisionPackageDefinition>();
        public static DecisionPackageDefinition HighLevelCaster_CombatDecisions { get; private set; } = ScriptableObject.CreateInstance<DecisionPackageDefinition>();
        public static DecisionPackageDefinition Naga_CombatDecisions { get; private set; } = ScriptableObject.CreateInstance<DecisionPackageDefinition>();
        public static DecisionPackageDefinition Vampire_CombatDecisions { get; private set; } = ScriptableObject.CreateInstance<DecisionPackageDefinition>();
        public static DecisionPackageDefinition Titan_CombatDecisions { get; private set; } = ScriptableObject.CreateInstance<DecisionPackageDefinition>();
        public static DecisionPackageDefinition ConstructTitan_CombatDecisions { get; private set; } = ScriptableObject.CreateInstance<DecisionPackageDefinition>();
        public static DecisionPackageDefinition Tarrasque_CombatDecisions { get; private set; } = ScriptableObject.CreateInstance<DecisionPackageDefinition>();
        public static DecisionDefinition TarrasqueSwallow_Decision { get; private set; } = ScriptableObject.CreateInstance<DecisionDefinition>();

        public static DecisionDefinition AtWillAOE_Magic_Decision { get; private set; } = ScriptableObject.CreateInstance<DecisionDefinition>();
        public static DecisionDefinition AtWillSelfBuff_Magic_Decision { get; private set; } = ScriptableObject.CreateInstance<DecisionDefinition>();
        public static DecisionDefinition LimitedPerDayTargetDebuff_Magic_Decision { get; private set; } = ScriptableObject.CreateInstance<DecisionDefinition>();
        public static DecisionDefinition LimitedPerDayAOE_Magic_Decision { get; private set; } = ScriptableObject.CreateInstance<DecisionDefinition>();
        public static DecisionDefinition SummonCreature_Magic_Decision { get; private set; } = ScriptableObject.CreateInstance<DecisionDefinition>();
        public static DecisionDefinition CastMagic_Stoneskin_Decision { get; private set; } = ScriptableObject.CreateInstance<DecisionDefinition>();
        public static Dictionary<string, string> Dictionaryof_Dragon_DamageAffinity { get; private set; } = new Dictionary<string, string>();

        //public static List<string> ListofDamageTypes_Dragon  { get; private set; } = new List<string> { "DamageAcid", "DamagePoison", "DamageFire", "DamageCold", "DamageLightning" };
        //public static List<string> ListofDamageTypes_Physical  { get; private set; } = new List<string> { "DamageSlashing", "DamageBludgeoning", "DamagePiercing" };
        //public static List<string> ListofDamageTypes_Other  { get; private set; } = new List<string> { "DamageThunder", "DamagePsychic", "DamageForce", "DamageNecrotic", "DamageRadiant" };

        internal static void Create()
        {
            Setdamageaffinity();
            BuildNewCastMagic_StoneSkin_Decision();
            BuildNewAtWillAOE_Magic_Decision();
            BuildNewAtWillSelfBuff_Magic_Decision();
            BuildNewLimitedPerDayTargetDebuffMagic_Decision();
            BuildNewLimitedPerDayAOE_Magic_Decision();
            BuildNewSummonCreature_Magic_Decision();
            BuildNewPitFiend_CombatDecisions();
            BuildNewBalor_CombatDecisions();
            BuildNewNalfeshnee_CombatDecisions();
            BuildNewSolar_CombatDecisions();
            BuildNewHighLevelCaster_CombatDecisions();
            BuildNewNaga_CombatDecisions();
            BuildNewVampire_CombatDecisions();
            BuildNewTitan_CombatDecisions();
            BuildNewConstructTitan_CombatDecisions();
            BuildNewLich_Spelllist();
            BuildNewCastSpell_Lich();
            BuildNewArchMage_Spelllist();
            BuildNewCastSpell_ArchMage();
            BuildNewGuardianNaga_Spelllist();
            BuildNewCastSpell_GuardianNaga();
            BuildNewFireTitan_Retaliate_DamageAffinity();
            BuildNewAncientDragon_CombatDecisions();
            BuildNewAirTitan_SleetStorm_Immunity_MagicAffinity();
            BuildNewBalor_Retaliate_DamageAffinity();
            BuildNewTarrasqueReflectiveCarapace();
            BuildNewTarrasqueSwallow_Decision();
            BuildNewTarrasque_CombatDecisions();
        }

        public static void Setdamageaffinity()
        {
            // correct damage type and dice numbers/type for ancient dragon Breath
            Dictionaryof_Dragon_DamageAffinity.Add("Ancient Black Dragon", "DamageAcid");
            Dictionaryof_Dragon_DamageAffinity.Add("Ancient Blue Dragon", "DamageLightning");
            Dictionaryof_Dragon_DamageAffinity.Add("Ancient Green Dragon", "DamagePoison");
            Dictionaryof_Dragon_DamageAffinity.Add("Ancient Red Dragon", "DamageFire");
            Dictionaryof_Dragon_DamageAffinity.Add("Ancient White Dragon", "DamageCold");
        }

        public static void BuildNewTarrasqueReflectiveCarapace()
        {
            string text = "TarrasqueReflectiveCarapaceImmunity";

            TarrasqueReflectiveCarapace = BuildNewMagicAffinity(
                   "DH" + text,
                   DatabaseHelper.FeatureDefinitionMagicAffinitys.MagicAffinityConditionImmuneToShine
                    );

            TarrasqueReflectiveCarapace.SpellImmunities.Clear();
            TarrasqueReflectiveCarapace.SpellImmunities.Add(DatabaseHelper.SpellDefinitions.MagicMissile.Name);

            SpellDefinition[] listofAllSpells = DatabaseRepository.GetDatabase<SpellDefinition>().GetAllElements();

            foreach (SpellDefinition spell in listofAllSpells)
            {
                if (spell.EffectDescription.TargetType == RuleDefinitions.TargetType.Line || spell.EffectDescription.RangeType == RuleDefinitions.RangeType.RangeHit)
                {
                    TarrasqueReflectiveCarapace.SpellImmunities.Add(spell.Name);
                }
            }

        }
        public static void BuildNewTarrasqueSwallow_Decision()
        {
            string text = "TarrasqueSwallowDecisions";

            TarrasqueSwallow_Decision = BuildNewDecisionDefinition(
                   "DH" + text,
                   DatabaseHelper.DecisionDefinitions.CastMagic_Fly_Self
                    );

            TarrasqueSwallow_Decision.Decision.SetStringParameter("TarrasqueSwallow");
        }

        public static void BuildNewTarrasque_CombatDecisions()
        {
            string text = "TarrasqueCombatDecisions";


            Tarrasque_CombatDecisions = BuildNewDecisionPackageDefinition(
                   "DH" + text,
                   DatabaseHelper.DecisionPackageDefinitions.RemorhazCombatDecisions
                    );

            Tarrasque_CombatDecisions.Package.WeightedDecisions.Clear();


            //
            WeightedDecisionDescription weightedDecisionDescription_0 = new WeightedDecisionDescription();
            weightedDecisionDescription_0.SetDecision(DatabaseHelper.DecisionDefinitions.LongRangePathToEnemy_Dash);
            weightedDecisionDescription_0.SetWeight(5f);

            //
            WeightedDecisionDescription weightedDecisionDescription_2 = new WeightedDecisionDescription();
            weightedDecisionDescription_2.SetDecision(DatabaseHelper.DecisionDefinitions.CastMagic_DPS_SingleTarget);
            weightedDecisionDescription_2.SetWeight(4.5f);

            //
            WeightedDecisionDescription weightedDecisionDescription_1 = new WeightedDecisionDescription();
            weightedDecisionDescription_1.SetDecision(DatabaseHelper.DecisionDefinitions.MeleeAttack_Default);
            weightedDecisionDescription_1.SetWeight(4f);

            //
            WeightedDecisionDescription weightedDecisionDescription_4 = new WeightedDecisionDescription();
            weightedDecisionDescription_4.SetDecision(TarrasqueSwallow_Decision);
            weightedDecisionDescription_4.SetWeight(4.5f);
            //
            WeightedDecisionDescription weightedDecisionDescription_3 = new WeightedDecisionDescription();
            weightedDecisionDescription_3.SetDecision(DatabaseHelper.DecisionDefinitions.CastMagic_FrightfulPresence_Dragon);
            weightedDecisionDescription_3.SetWeight(3f);


            //
            WeightedDecisionDescription weightedDecisionDescription_5 = new WeightedDecisionDescription();
            weightedDecisionDescription_5.SetDecision(DatabaseHelper.DecisionDefinitions.Move_Aggressive_Remorhaz);
            weightedDecisionDescription_5.SetWeight(2.0f);


            //
            WeightedDecisionDescription weightedDecisionDescription_7 = new WeightedDecisionDescription();
            weightedDecisionDescription_7.SetDecision(DatabaseHelper.DecisionDefinitions.Emote_Angry);
            weightedDecisionDescription_7.SetWeight(0.1f);



            // don't know if keeping descending weight order is important or not but it matches the preexisting format in
            // all other DecisionPackageDefinition so it might be necessary. can't hurt to order it

            Tarrasque_CombatDecisions.Package.WeightedDecisions.AddRange(
                                                                    weightedDecisionDescription_1,
                                                                    weightedDecisionDescription_2,
                                                                    weightedDecisionDescription_4,
                                                                    weightedDecisionDescription_3,
                                                                    weightedDecisionDescription_5,
                                                                    //       weightedDecisionDescription_6,
                                                                    weightedDecisionDescription_7
                                                                    );
        }

        public static void BuildNewBalor_Retaliate_DamageAffinity()
        {

            string Power_text = "BalorRetaliatePower";


            FeatureDefinitionPower Balor_Retaliate_Power = NewMonsterPowers.BuildNewPower(
                    "DH" + Power_text,
                    DatabaseHelper.FeatureDefinitionPowers.PowerRemorhazRetaliate
                     );

            Balor_Retaliate_Power.EffectDescription.SetTargetType(RuleDefinitions.TargetType.Sphere);
            Balor_Retaliate_Power.EffectDescription.SetTargetParameter(6);

            Balor_Retaliate_Power.EffectDescription.EffectForms[0].DamageForm.SetDiceNumber(3);
            Balor_Retaliate_Power.EffectDescription.EffectForms[0].DamageForm.SetDieType(RuleDefinitions.DieType.D6);
            Balor_Retaliate_Power.EffectDescription.EffectForms[0].DamageForm.SetDamageType(RuleDefinitions.DamageTypeFire);


            string text = "BalorRetaliateDamageAffinity";


            Balor_Retaliate_DamageAffinity = BuildNewDamageAffinity(
                   "DH" + text,
                   DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityFireImmunityRemorhaz
                    );

            Balor_Retaliate_DamageAffinity.SetRetaliatePower(Balor_Retaliate_Power);

        }

        public static void BuildNewLich_Spelllist()

        {
            /*
            string text = "Lich_Spelllist";
            Lich_spelllist = Helpers.SpelllistBuilder.create9LevelSpelllist(text, GuidHelper.Create(Settings.GUID, "DH" + text).ToString(), "",
                                                                    new List<SpellDefinition>
                                                                    {
                                                                                    DatabaseHelper.SpellDefinitions.FireBolt,
                                                                                    DatabaseHelper.SpellDefinitions.RayOfFrost,
                                                                                    DatabaseHelper.SpellDefinitions.ChillTouch

                                                                    },
                                                                    new List<SpellDefinition>
                                                                    {

                                                                                    DatabaseHelper.SpellDefinitions.DetectMagic,
                                                                                    DatabaseHelper.SpellDefinitions.MagicMissile,
                                                                                    DatabaseHelper.SpellDefinitions.Shield,
                                                                                    DatabaseHelper.SpellDefinitions.Thunderwave
                                                                     },
                                                                    new List<SpellDefinition>
                                                                    {
                                                                                    DatabaseHelper.SpellDefinitions.Invisibility,
                                                                                    DatabaseHelper.SpellDefinitions.AcidArrow,
                                                                                    DatabaseHelper.SpellDefinitions.Blur

                                                                    },
                                                                    new List<SpellDefinition>
                                                                    {
                                                                                    DatabaseHelper.SpellDefinitions.AnimateDead,
                                                                                    DatabaseHelper.SpellDefinitions.Counterspell,
                                                                                    DatabaseHelper.SpellDefinitions.DispelMagic,
                                                                                    DatabaseHelper.SpellDefinitions.Fireball

                                                                    },
                                                                    new List<SpellDefinition>
                                                                    {
                                                                                    DatabaseHelper.SpellDefinitions.Blight,
                                                                                    DatabaseHelper.SpellDefinitions.DimensionDoor

                                                                    },
                                                                    new List<SpellDefinition>
                                                                    {
                                                                                    DatabaseHelper.SpellDefinitions.CloudKill,
                                                                                    DatabaseHelper.SpellDefinitions.ConeOfCold
                                                                    },
                                                                    new List<SpellDefinition>
                                                                    {
                                                                                    DatabaseHelper.SpellDefinitions.Disintegrate,
                                                                                    DatabaseHelper.SpellDefinitions.GlobeOfInvulnerability,
                                                                    },
                                                                    new List<SpellDefinition>
                                                                    {
                                                                                    CreateAndAddNewSpells.ReverseGravity_Spell,
                                                                                    CreateAndAddNewSpells.PowerWordStun_Spell,
                                                                                    CreateAndAddNewSpells.PowerWordKill_Spell
                                                                    }
                                                                    );

            Lich_spelllist.SetMaxSpellLevel(9);
            Lich_spelllist.SetHasCantrips(true);

            */

            string text = "LichSpelllist";

            Lich_spelllist = BuildNewSpelllist(
                     "DH" + text,
                     DatabaseHelper.SpellListDefinitions.SpellListMage
                      );


            SpellListDefinition.SpellsByLevelDuplet LichSpell_Cantrips = new SpellListDefinition.SpellsByLevelDuplet
            {
                Spells = new List<SpellDefinition>()
            };
            LichSpell_Cantrips.Spells.Add(DatabaseHelper.SpellDefinitions.FireBolt);
            LichSpell_Cantrips.Spells.Add(DatabaseHelper.SpellDefinitions.RayOfFrost);
            // LichSpell_Cantrips.Spells.Add(DatabaseHelper.SpellDefinitions.ChillTouch);


            SpellListDefinition.SpellsByLevelDuplet LichSpell_level_1 = new SpellListDefinition.SpellsByLevelDuplet
            {
                Spells = new List<SpellDefinition>(),
                Level = 1
            };
            LichSpell_level_1.Spells.Add(DatabaseHelper.SpellDefinitions.DetectMagic);
            LichSpell_level_1.Spells.Add(DatabaseHelper.SpellDefinitions.MagicMissile);
            LichSpell_level_1.Spells.Add(DatabaseHelper.SpellDefinitions.Shield);
            LichSpell_level_1.Spells.Add(DatabaseHelper.SpellDefinitions.Thunderwave);

            SpellListDefinition.SpellsByLevelDuplet LichSpell_level_2 = new SpellListDefinition.SpellsByLevelDuplet
            {
                Spells = new List<SpellDefinition>(),
                Level = 2
            };
            LichSpell_level_2.Spells.Add(DatabaseHelper.SpellDefinitions.Invisibility);
            LichSpell_level_2.Spells.Add(DatabaseHelper.SpellDefinitions.AcidArrow);
            LichSpell_level_2.Spells.Add(DatabaseHelper.SpellDefinitions.Blur);


            SpellListDefinition.SpellsByLevelDuplet LichSpell_level_3 = new SpellListDefinition.SpellsByLevelDuplet
            {
                Spells = new List<SpellDefinition>(),
                Level = 3
            };
            //LichSpell_level_3.Spells.Add(DatabaseHelper.SpellDefinitions.AnimateDead);
            LichSpell_level_3.Spells.Add(DatabaseHelper.SpellDefinitions.Counterspell);
            LichSpell_level_3.Spells.Add(DatabaseHelper.SpellDefinitions.DispelMagic);
            LichSpell_level_3.Spells.Add(DatabaseHelper.SpellDefinitions.Fireball);


            SpellListDefinition.SpellsByLevelDuplet LichSpell_level_4 = new SpellListDefinition.SpellsByLevelDuplet
            {
                Spells = new List<SpellDefinition>(),
                Level = 4
            };
            LichSpell_level_4.Spells.Add(DatabaseHelper.SpellDefinitions.Blight);
            LichSpell_level_4.Spells.Add(DatabaseHelper.SpellDefinitions.DimensionDoor);

            SpellListDefinition.SpellsByLevelDuplet LichSpell_level_5 = new SpellListDefinition.SpellsByLevelDuplet
            {
                Spells = new List<SpellDefinition>(),
                Level = 5
            };
            LichSpell_level_5.Spells.Add(DatabaseHelper.SpellDefinitions.CloudKill);
            LichSpell_level_5.Spells.Add(DatabaseHelper.SpellDefinitions.ConeOfCold);

            SpellListDefinition.SpellsByLevelDuplet LichSpell_level_6 = new SpellListDefinition.SpellsByLevelDuplet
            {
                Spells = new List<SpellDefinition>(),
                Level = 6
            };
            LichSpell_level_6.Spells.Add(DatabaseHelper.SpellDefinitions.Disintegrate);
            LichSpell_level_6.Spells.Add(DatabaseHelper.SpellDefinitions.GlobeOfInvulnerability);

            SpellListDefinition.SpellsByLevelDuplet LichSpell_level_7 = new SpellListDefinition.SpellsByLevelDuplet
            {
                Spells = new List<SpellDefinition>(),
                Level = 7
            };
            LichSpell_level_7.Spells.Add(ReverseGravity);
            LichSpell_level_7.Spells.Add(PowerWordStun);
            LichSpell_level_7.Spells.Add(PowerWordKill);

            SpellListDefinition.SpellsByLevelDuplet LichSpell_level_8 = new SpellListDefinition.SpellsByLevelDuplet
            {
                Spells = new List<SpellDefinition>(),
                Level = 8
            };
            LichSpell_level_8.Spells.Add(PowerWordStun);

            SpellListDefinition.SpellsByLevelDuplet LichSpell_level_9 = new SpellListDefinition.SpellsByLevelDuplet
            {
                Spells = new List<SpellDefinition>(),
                Level = 9
            };
            LichSpell_level_9.Spells.Add(PowerWordKill);

            Lich_spelllist.SetContentCopyright(BaseDefinition.Copyright.UserContent);
            Lich_spelllist.SetMaxSpellLevel(7);
            Lich_spelllist.SetHasCantrips(true);
            Lich_spelllist.SpellsByLevel.Clear();
            Lich_spelllist.SpellsByLevel.AddRange(new List<SpellListDefinition.SpellsByLevelDuplet>
            {
                LichSpell_Cantrips,
                LichSpell_level_1,
                LichSpell_level_2,
                LichSpell_level_3,
                LichSpell_level_4,
                LichSpell_level_5,
                LichSpell_level_6,
                LichSpell_level_7,
             //   LichSpell_level_8,
             //   LichSpell_level_9
            });


        }
        public static void BuildNewCastSpell_Lich()
        {

            string text = "CastSpellLich";




            CastSpell_Lich = BuildNewCaster(
                     "DH" + text,
                     DatabaseHelper.FeatureDefinitionCastSpells.CastSpellMage
                      );



            CastSpell_Lich.SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Monster);
            CastSpell_Lich.SetSpellcastingAbility("Intelligence");
            CastSpell_Lich.SetSpellcastingParametersComputation(RuleDefinitions.SpellcastingParametersComputation.Static);
            CastSpell_Lich.SetStaticDCValue(20);
            CastSpell_Lich.SetStaticToHitValue(12);
            CastSpell_Lich.SetSpellListDefinition(Lich_spelllist);
            CastSpell_Lich.RestrictedSchools.Clear();
            CastSpell_Lich.SetSpellKnowledge(RuleDefinitions.SpellKnowledge.FixedList);
            CastSpell_Lich.SetSlotsRecharge(RuleDefinitions.RechargeRate.LongRest);
            CastSpell_Lich.SetSpellCastingLevel(18);
            CastSpell_Lich.SetSpellReadyness(RuleDefinitions.SpellReadyness.AllKnown);

            int[] cantrip_arr = { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 };
            CastSpell_Lich.KnownCantrips.AddRange(cantrip_arr);

            CastSpell_Lich.SlotsPerLevels.Clear();
            CastSpell_Lich.SlotsPerLevels.AddRange(new List<FeatureDefinitionCastSpell.SlotsByLevelDuplet>()
            {
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {2,0,0,0,0,0,0,0,0,0}, Level = 01 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {3,0,0,0,0,0,0,0,0,0}, Level = 02 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,2,0,0,0,0,0,0,0,0}, Level = 03 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,0,0,0,0,0,0,0,0}, Level = 04 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,2,0,0,0,0,0,0,0}, Level = 05 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,0,0,0,0,0,0,0}, Level = 06 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,1,0,0,0,0,0,0}, Level = 07 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,2,0,0,0,0,0,0}, Level = 08 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,1,0,0,0,0,0}, Level = 09 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,0,0,0,0,0}, Level = 10 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,1,0,0,0,0}, Level = 11 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,1,0,0,0,0}, Level = 12 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,1,1,0,0,0}, Level = 13 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,1,1,0,0,0}, Level = 14 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,1,1,1,0,0}, Level = 15 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,1,1,1,0,0}, Level = 16 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,1,1,1,1,0}, Level = 17 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,3,1,1,1,1,0}, Level = 18 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,3,2,1,1,1,0}, Level = 19 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,3,2,2,1,1,0}, Level = 20 },
            });



        }


        public static void BuildNewArchMage_Spelllist()

        {
            /*   string text = "ArchMage_Spelllist";
               ArchMage_spelllist = Helpers.SpelllistBuilder.create9LevelSpelllist(text, GuidHelper.Create(Settings.GUID, "DH" + text).ToString(), "",
                                                                       new List<SpellDefinition>
                                                                       {
                                                                                       DatabaseHelper.SpellDefinitions.FireBolt,
                                                                                       DatabaseHelper.SpellDefinitions.ShockingGrasp,

                                                                       },
                                                                       new List<SpellDefinition>
                                                                       {

                                                                                       DatabaseHelper.SpellDefinitions.MagicMissile,
                                                                                       DatabaseHelper.SpellDefinitions.Thunderwave,
                                                                       },
                                                                       new List<SpellDefinition>
                                                                       {
                                                                                       DatabaseHelper.SpellDefinitions.Blur,
                                                                                       DatabaseHelper.SpellDefinitions.MistyStep,
                                                                       },
                                                                       new List<SpellDefinition>
                                                                       {
                                                                                       DatabaseHelper.SpellDefinitions.Counterspell,
                                                                                       DatabaseHelper.SpellDefinitions.Fly,
                                                                                       DatabaseHelper.SpellDefinitions.LightningBolt,

                                                                       },
                                                                       new List<SpellDefinition>
                                                                       {
                                                                                       DatabaseHelper.SpellDefinitions.Banishment,
                                                                                       DatabaseHelper.SpellDefinitions.FireShield

                                                                       },
                                                                       new List<SpellDefinition>
                                                                       {
                                                                                       DatabaseHelper.SpellDefinitions.CloudKill,
                                                                                       DatabaseHelper.SpellDefinitions.WallOfForce,
                                                                                       DatabaseHelper.SpellDefinitions.ConeOfCold,
                                                                       },
                                                                       new List<SpellDefinition>
                                                                       {
                                                                                       DatabaseHelper.SpellDefinitions.GlobeOfInvulnerability,
                                                                       },
                                                                       new List<SpellDefinition>
                                                                       {
                                                                                       CreateAndAddNewSpells.ReverseGravity_Spell,
                                                                                       CreateAndAddNewSpells.PowerWordStun_Spell,
                                                                                       CreateAndAddNewSpells.TimeStop_Spell,
                                                                       }
                                                                       );

               ArchMage_spelllist.SetMaxSpellLevel(9);
               ArchMage_spelllist.SetHasCantrips(true);
            */

            string text = "ArchMageSpelllist";

            Archmage_spelllist = BuildNewSpelllist(
                     "DH" + text,
                     DatabaseHelper.SpellListDefinitions.SpellListMage
                      );


            SpellListDefinition.SpellsByLevelDuplet ArchmageSpell_Cantrips = new SpellListDefinition.SpellsByLevelDuplet
            {
                Spells = new List<SpellDefinition>()
            };
            ArchmageSpell_Cantrips.Spells.Add(DatabaseHelper.SpellDefinitions.FireBolt);
            ArchmageSpell_Cantrips.Spells.Add(DatabaseHelper.SpellDefinitions.ShockingGrasp);


            SpellListDefinition.SpellsByLevelDuplet ArchmageSpell_level_1 = new SpellListDefinition.SpellsByLevelDuplet
            {
                Spells = new List<SpellDefinition>(),
                Level = 1
            };
            ArchmageSpell_level_1.Spells.Add(DatabaseHelper.SpellDefinitions.MagicMissile);
            ArchmageSpell_level_1.Spells.Add(DatabaseHelper.SpellDefinitions.Thunderwave);

            SpellListDefinition.SpellsByLevelDuplet ArchmageSpell_level_2 = new SpellListDefinition.SpellsByLevelDuplet
            {
                Spells = new List<SpellDefinition>(),
                Level = 2
            };
            ArchmageSpell_level_2.Spells.Add(DatabaseHelper.SpellDefinitions.Blur);
            ArchmageSpell_level_2.Spells.Add(DatabaseHelper.SpellDefinitions.MistyStep);


            SpellListDefinition.SpellsByLevelDuplet ArchmageSpell_level_3 = new SpellListDefinition.SpellsByLevelDuplet
            {
                Spells = new List<SpellDefinition>(),
                Level = 3
            };
            ArchmageSpell_level_3.Spells.Add(DatabaseHelper.SpellDefinitions.Counterspell);
            ArchmageSpell_level_3.Spells.Add(DatabaseHelper.SpellDefinitions.Fly);
            ArchmageSpell_level_3.Spells.Add(DatabaseHelper.SpellDefinitions.LightningBolt);


            SpellListDefinition.SpellsByLevelDuplet ArchmageSpell_level_4 = new SpellListDefinition.SpellsByLevelDuplet
            {
                Spells = new List<SpellDefinition>(),
                Level = 4
            };
            ArchmageSpell_level_4.Spells.Add(DatabaseHelper.SpellDefinitions.Banishment);
            ArchmageSpell_level_4.Spells.Add(DatabaseHelper.SpellDefinitions.FireShield);

            SpellListDefinition.SpellsByLevelDuplet ArchmageSpell_level_5 = new SpellListDefinition.SpellsByLevelDuplet
            {
                Spells = new List<SpellDefinition>(),
                Level = 5
            };
            ArchmageSpell_level_5.Spells.Add(DatabaseHelper.SpellDefinitions.CloudKill);
            ArchmageSpell_level_5.Spells.Add(DatabaseHelper.SpellDefinitions.ConeOfCold);
            //ArchmageSpell_level_5.Spells.Add(DatabaseHelper.SpellDefinitions.WallOfForce);

            SpellListDefinition.SpellsByLevelDuplet ArchmageSpell_level_6 = new SpellListDefinition.SpellsByLevelDuplet
            {
                Spells = new List<SpellDefinition>(),
                Level = 6
            };
            ArchmageSpell_level_6.Spells.Add(DatabaseHelper.SpellDefinitions.GlobeOfInvulnerability);

            SpellListDefinition.SpellsByLevelDuplet ArchmageSpell_level_7 = new SpellListDefinition.SpellsByLevelDuplet
            {
                Spells = new List<SpellDefinition>(),
                Level = 7
            };
            ArchmageSpell_level_7.Spells.Add(ReverseGravity);
            ArchmageSpell_level_7.Spells.Add(PowerWordStun);
            ArchmageSpell_level_7.Spells.Add(TimeStop);

            SpellListDefinition.SpellsByLevelDuplet ArchmageSpell_level_8 = new SpellListDefinition.SpellsByLevelDuplet
            {
                Spells = new List<SpellDefinition>(),
                Level = 8
            };
            ArchmageSpell_level_8.Spells.Add(PowerWordStun);

            SpellListDefinition.SpellsByLevelDuplet ArchmageSpell_level_9 = new SpellListDefinition.SpellsByLevelDuplet
            {
                Spells = new List<SpellDefinition>(),
                Level = 9
            };
            ArchmageSpell_level_9.Spells.Add(TimeStop);

            Archmage_spelllist.SetContentCopyright(BaseDefinition.Copyright.UserContent);
            Archmage_spelllist.SetMaxSpellLevel(7);
            Archmage_spelllist.SetHasCantrips(true);
            Archmage_spelllist.SpellsByLevel.Clear();
            Archmage_spelllist.SpellsByLevel.AddRange(new List<SpellListDefinition.SpellsByLevelDuplet>
            {
                ArchmageSpell_Cantrips,
                ArchmageSpell_level_1,
                ArchmageSpell_level_2,
                ArchmageSpell_level_3,
                ArchmageSpell_level_4,
                ArchmageSpell_level_5,
                ArchmageSpell_level_6,
                ArchmageSpell_level_7,
                //ArchmageSpell_level_8,
               // ArchmageSpell_level_9
            });

        }

        public static void BuildNewCastSpell_ArchMage()
        {

            string text = "CastSpellArchMage";




            CastSpell_ArchMage = BuildNewCaster(
                     "DH" + text,
                     DatabaseHelper.FeatureDefinitionCastSpells.CastSpellMage
                      );



            CastSpell_ArchMage.SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Monster);
            CastSpell_ArchMage.SetSpellcastingAbility("Intelligence");
            CastSpell_ArchMage.SetSpellcastingParametersComputation(RuleDefinitions.SpellcastingParametersComputation.Static);
            CastSpell_ArchMage.SetStaticDCValue(17);
            CastSpell_ArchMage.SetStaticToHitValue(9);
            CastSpell_ArchMage.SetSpellListDefinition(Archmage_spelllist);
            CastSpell_ArchMage.RestrictedSchools.Clear();
            CastSpell_ArchMage.SetSpellKnowledge(RuleDefinitions.SpellKnowledge.FixedList);
            CastSpell_ArchMage.SetSlotsRecharge(RuleDefinitions.RechargeRate.LongRest);
            CastSpell_ArchMage.SetSpellCastingLevel(18);
            CastSpell_ArchMage.SetSpellReadyness(RuleDefinitions.SpellReadyness.AllKnown);

            int[] cantrip_arr = { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 };
            CastSpell_ArchMage.KnownCantrips.AddRange(cantrip_arr);

            CastSpell_ArchMage.SlotsPerLevels.Clear();
            CastSpell_ArchMage.SlotsPerLevels.AddRange(new List<FeatureDefinitionCastSpell.SlotsByLevelDuplet>()
           {
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {2,0,0,0,0,0,0,0,0,0}, Level = 01 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {3,0,0,0,0,0,0,0,0,0}, Level = 02 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,2,0,0,0,0,0,0,0,0}, Level = 03 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,0,0,0,0,0,0,0,0}, Level = 04 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,2,0,0,0,0,0,0,0}, Level = 05 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,0,0,0,0,0,0,0}, Level = 06 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,1,0,0,0,0,0,0}, Level = 07 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,2,0,0,0,0,0,0}, Level = 08 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,1,0,0,0,0,0}, Level = 09 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,0,0,0,0,0}, Level = 10 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,1,0,0,0,0}, Level = 11 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,1,0,0,0,0}, Level = 12 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,1,1,0,0,0}, Level = 13 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,1,1,0,0,0}, Level = 14 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,1,1,1,0,0}, Level = 15 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,1,1,1,0,0}, Level = 16 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,1,1,1,1,0}, Level = 17 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,3,1,1,1,1,0}, Level = 18 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,3,2,1,1,1,0}, Level = 19 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,3,2,2,1,1,0}, Level = 20 },
           });



        }

        public static void BuildNewGuardianNaga_Spelllist()

        {
            /*
                        string text = "GuardianNaga_Spelllist";
                        GuardianNaga_spelllist = Helpers.SpelllistBuilder.create9LevelSpelllist(text, GuidHelper.Create(Settings.GUID, "DH" + text).ToString(), "",
                                                                                new List<SpellDefinition>
                                                                                {
                                                                                                DatabaseHelper.SpellDefinitions.SacredFlame

                                                                                },
                                                                                new List<SpellDefinition>
                                                                                {
                                                                                                DatabaseHelper.SpellDefinitions.CureWounds,
                                                                                                DatabaseHelper.SpellDefinitions.Command,
                                                                                                DatabaseHelper.SpellDefinitions.ShieldOfFaith
                                                                                },
                                                                                new List<SpellDefinition>
                                                                                {
                                                                                                DatabaseHelper.SpellDefinitions.CalmEmotions,
                                                                                                DatabaseHelper.SpellDefinitions.HoldPerson

                                                                                },
                                                                                new List<SpellDefinition>
                                                                                {
                                                                                                DatabaseHelper.SpellDefinitions.BestowCurse

                                                                                },
                                                                                new List<SpellDefinition>
                                                                                {
                                                                                                DatabaseHelper.SpellDefinitions.Banishment,
                                                                                                DatabaseHelper.SpellDefinitions.FreedomOfMovement

                                                                                },
                                                                                new List<SpellDefinition>
                                                                                {
                                                                                                DatabaseHelper.SpellDefinitions.FlameStrike,
                                                                                                DatabaseHelper.SpellDefinitions.Contagion
                                                                                },
                                                                                new List<SpellDefinition>
                                                                                {

                                                                                                DatabaseHelper.SpellDefinitions.GlobeOfInvulnerability
                                                                                }

                                                                                );

                        */


            string text = "GuardianNagaSpelllist";

            GuardianNaga_spelllist = BuildNewSpelllist(
            "DH" + text,
            DatabaseHelper.SpellListDefinitions.SpellListHighPriest
            );


            SpellListDefinition.SpellsByLevelDuplet GuardianNagaSpell_Cantrips = new SpellListDefinition.SpellsByLevelDuplet
            {
                Spells = new List<SpellDefinition>()
            };
            GuardianNagaSpell_Cantrips.Spells.Add(DatabaseHelper.SpellDefinitions.SacredFlame);


            SpellListDefinition.SpellsByLevelDuplet GuardianNagaSpell_level_1 = new SpellListDefinition.SpellsByLevelDuplet
            {
                Spells = new List<SpellDefinition>(),
                Level = 1
            };
            GuardianNagaSpell_level_1.Spells.Add(DatabaseHelper.SpellDefinitions.CureWounds);
            //GuardianNagaSpell_level_1.Spells.Add(DatabaseHelper.SpellDefinitions.Command);
            GuardianNagaSpell_level_1.Spells.Add(DatabaseHelper.SpellDefinitions.ShieldOfFaith);

            SpellListDefinition.SpellsByLevelDuplet GuardianNagaSpell_level_2 = new SpellListDefinition.SpellsByLevelDuplet
            {
                Spells = new List<SpellDefinition>(),
                Level = 2
            };
            GuardianNagaSpell_level_2.Spells.Add(DatabaseHelper.SpellDefinitions.CalmEmotions);
            GuardianNagaSpell_level_2.Spells.Add(DatabaseHelper.SpellDefinitions.HoldPerson);


            SpellListDefinition.SpellsByLevelDuplet GuardianNagaSpell_level_3 = new SpellListDefinition.SpellsByLevelDuplet
            {
                Spells = new List<SpellDefinition>(),
                Level = 3
            };
            GuardianNagaSpell_level_3.Spells.Add(DatabaseHelper.SpellDefinitions.BestowCurse);


            SpellListDefinition.SpellsByLevelDuplet GuardianNagaSpell_level_4 = new SpellListDefinition.SpellsByLevelDuplet
            {
                Spells = new List<SpellDefinition>(),
                Level = 4
            };
            GuardianNagaSpell_level_4.Spells.Add(DatabaseHelper.SpellDefinitions.Banishment);
            GuardianNagaSpell_level_4.Spells.Add(DatabaseHelper.SpellDefinitions.FreedomOfMovement);

            SpellListDefinition.SpellsByLevelDuplet GuardianNagaSpell_level_5 = new SpellListDefinition.SpellsByLevelDuplet
            {
                Spells = new List<SpellDefinition>(),
                Level = 5
            };
            GuardianNagaSpell_level_5.Spells.Add(DatabaseHelper.SpellDefinitions.FlameStrike);
            GuardianNagaSpell_level_5.Spells.Add(DatabaseHelper.SpellDefinitions.Contagion);

            SpellListDefinition.SpellsByLevelDuplet GuardianNagaSpell_level_6 = new SpellListDefinition.SpellsByLevelDuplet
            {
                Spells = new List<SpellDefinition>(),
                Level = 6
            };
            GuardianNagaSpell_level_6.Spells.Add(DatabaseHelper.SpellDefinitions.GlobeOfInvulnerability);

            GuardianNaga_spelllist.SetContentCopyright(BaseDefinition.Copyright.UserContent);
            GuardianNaga_spelllist.SetMaxSpellLevel(6);
            GuardianNaga_spelllist.SetHasCantrips(true);
            GuardianNaga_spelllist.SpellsByLevel.Clear();
            GuardianNaga_spelllist.SpellsByLevel.AddRange(new List<SpellListDefinition.SpellsByLevelDuplet>
            {
                GuardianNagaSpell_Cantrips,
                GuardianNagaSpell_level_1,
                GuardianNagaSpell_level_2,
                GuardianNagaSpell_level_3,
                GuardianNagaSpell_level_4,
                GuardianNagaSpell_level_5,
                GuardianNagaSpell_level_6
            });
        }
        public static void BuildNewCastSpell_GuardianNaga()
        {

            string text = "CastSpellGuardianNaga";




            CastSpell_GuardianNaga = BuildNewCaster(
                     "DH" + text,
                     DatabaseHelper.FeatureDefinitionCastSpells.CastSpellDivineAvatar_Wizard
                      );



            CastSpell_GuardianNaga.SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Monster);
            CastSpell_GuardianNaga.SetSpellcastingAbility("Wisdom");
            CastSpell_GuardianNaga.SetSpellcastingParametersComputation(RuleDefinitions.SpellcastingParametersComputation.Static);
            CastSpell_GuardianNaga.SetStaticDCValue(16);
            CastSpell_GuardianNaga.SetStaticToHitValue(8);
            CastSpell_GuardianNaga.SetSpellListDefinition(GuardianNaga_spelllist);
            CastSpell_GuardianNaga.RestrictedSchools.Clear();
            CastSpell_GuardianNaga.SetSpellKnowledge(RuleDefinitions.SpellKnowledge.FixedList);
            CastSpell_GuardianNaga.SetSlotsRecharge(RuleDefinitions.RechargeRate.LongRest);
            CastSpell_GuardianNaga.SetSpellCastingLevel(11);
            //    CastSpell_GuardianNaga.SetSpellReadyness(RuleDefinitions.SpellReadyness.AllKnown);
            //    CastSpell_GuardianNaga.SetSpellPreparationCount(RuleDefinitions.SpellPreparationCount.AbilityBonusPlusLevel);

            int[] cantrip_arr = { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            CastSpell_GuardianNaga.KnownCantrips.AddRange(cantrip_arr);

            CastSpell_GuardianNaga.SlotsPerLevels.Clear();
            CastSpell_GuardianNaga.SlotsPerLevels.AddRange(new List<FeatureDefinitionCastSpell.SlotsByLevelDuplet>()
           {
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {2,0,0,0,0,0,0,0,0,0}, Level = 01 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {3,0,0,0,0,0,0,0,0,0}, Level = 02 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,2,0,0,0,0,0,0,0,0}, Level = 03 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,0,0,0,0,0,0,0,0}, Level = 04 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,2,0,0,0,0,0,0,0}, Level = 05 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,0,0,0,0,0,0,0}, Level = 06 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,1,0,0,0,0,0,0}, Level = 07 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,2,0,0,0,0,0,0}, Level = 08 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,1,0,0,0,0,0}, Level = 09 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,0,0,0,0,0}, Level = 10 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,1,0,0,0,0}, Level = 11 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,1,0,0,0,0}, Level = 12 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,1,1,0,0,0}, Level = 13 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,1,1,0,0,0}, Level = 14 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,1,1,1,0,0}, Level = 15 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,1,1,1,0,0}, Level = 16 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,1,1,1,1,0}, Level = 17 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,3,1,1,1,1,0}, Level = 18 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,3,2,1,1,1,0}, Level = 19 },
            new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,3,2,2,1,1,0}, Level = 20 },
           });


        }

        public static void BuildNewAirTitan_SleetStorm_Immunity_MagicAffinity()
        {
            string text = "SleetStormImmunity";


            AirTitan_SleetStorm_Immunity = BuildNewMagicAffinity(
                   "DH" + text,
                   DatabaseHelper.FeatureDefinitionMagicAffinitys.MagicAffinityConditionImmuneToShine
                    );


            AirTitan_SleetStorm_Immunity.SpellImmunities.Clear();
            AirTitan_SleetStorm_Immunity.SpellImmunities.Add(DatabaseHelper.SpellDefinitions.SleetStorm.Name);
        }
        public static void BuildNewFireTitan_Retaliate_DamageAffinity()
        {

            string Power_text = "FireTitanRetaliatePower";


            FeatureDefinitionPower FireTitan_Retaliate_Power = NewMonsterPowers.BuildNewPower(
                    "DH" + Power_text,
                    DatabaseHelper.FeatureDefinitionPowers.PowerRemorhazRetaliate
                     );


            FireTitan_Retaliate_Power.EffectDescription.SetTargetParameter(10);

            FireTitan_Retaliate_Power.EffectDescription.EffectForms[0].DamageForm.SetDiceNumber(10);
            FireTitan_Retaliate_Power.EffectDescription.EffectForms[0].DamageForm.SetDieType(RuleDefinitions.DieType.D6);
            FireTitan_Retaliate_Power.EffectDescription.EffectForms[0].DamageForm.SetDamageType(RuleDefinitions.DamageTypeFire);



            string text = "FireTitanRetaliateDamageAffinity";


            FireTitan_Retaliate_DamageAffinity = BuildNewDamageAffinity(
                   "DH" + text,
                   DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityFireImmunityRemorhaz
                    );

            FireTitan_Retaliate_DamageAffinity.SetRetaliatePower(FireTitan_Retaliate_Power);
        }


        public static void BuildNewAncientDragon_CombatDecisions()
        {
            string text = "AncientDragonCombatDecisions";


            AncientDragon_CombatDecisions = BuildNewDecisionPackageDefinition(
                   "DH" + text,
                   DatabaseHelper.DecisionPackageDefinitions.DragonCombatDecisions
                    );

            AncientDragon_CombatDecisions.Package.WeightedDecisions.Clear();


            WeightedDecisionDescription weightedDecisionDescription = new WeightedDecisionDescription();
            weightedDecisionDescription.SetDecision(DatabaseHelper.DecisionDefinitions.Move_SoftRetreat_Flying);
            weightedDecisionDescription.SetWeight(3);

            WeightedDecisionDescription weightedDecisionDescription_2 = new WeightedDecisionDescription();
            weightedDecisionDescription_2.SetDecision(DatabaseHelper.DecisionDefinitions.Move_Burrow_Aggressive_Small);
            weightedDecisionDescription_2.SetWeight(3);

            // don't know if keeping descending weight order is important or not but it matches the preexisting format in
            // all other DecisionPackageDefinition so it might be necessary. can't hurt to order it
            AncientDragon_CombatDecisions.Package.WeightedDecisions.AddRange
                (
                DatabaseHelper.DecisionPackageDefinitions.DragonCombatDecisions.Package.WeightedDecisions[0],
                DatabaseHelper.DecisionPackageDefinitions.DragonCombatDecisions.Package.WeightedDecisions[1],
                DatabaseHelper.DecisionPackageDefinitions.DragonCombatDecisions.Package.WeightedDecisions[2],
                DatabaseHelper.DecisionPackageDefinitions.DragonCombatDecisions.Package.WeightedDecisions[3],
                DatabaseHelper.DecisionPackageDefinitions.DragonCombatDecisions.Package.WeightedDecisions[4],
                weightedDecisionDescription,
                weightedDecisionDescription_2,
                DatabaseHelper.DecisionPackageDefinitions.DragonCombatDecisions.Package.WeightedDecisions[5],
                DatabaseHelper.DecisionPackageDefinitions.DragonCombatDecisions.Package.WeightedDecisions[6]
                );

        }
        public static void BuildNewSummonCreature_Magic_Decision()
        {
            string text = "SummonCreatureMagicDecisions";


            SummonCreature_Magic_Decision = BuildNewDecisionDefinition(
                   "DH" + text,
                   DatabaseHelper.DecisionDefinitions.CastMagic_Debuff_AoE
                    );

            SummonCreature_Magic_Decision.Decision.SetEnumParameter(((int)RuleDefinitions.MagicType.SummonsCreature));

            SummonCreature_Magic_Decision.Decision.SetStringParameter("SummonCreature");
        }
        public static void BuildNewAtWillAOE_Magic_Decision()
        {
            string text = "AtWillAOEMagicDecisions";


            AtWillAOE_Magic_Decision = BuildNewDecisionDefinition(
                   "DH" + text,
                   DatabaseHelper.DecisionDefinitions.CastMagic_DPS_AoE
                    );


            AtWillAOE_Magic_Decision.Decision.SetStringParameter("AtWillAOE");
        }
        public static void BuildNewCastMagic_StoneSkin_Decision()
        {
            string text = "CastMagicStoneSkinDecisions";


            CastMagic_Stoneskin_Decision = BuildNewDecisionDefinition(
                   "DH" + text,
                   DatabaseHelper.DecisionDefinitions.CastMagic_Fly_Self
                    );


            CastMagic_Stoneskin_Decision.Decision.SetStringParameter("Stoneskin");
        }
        public static void BuildNewLimitedPerDayAOE_Magic_Decision()
        {
            string text = "LimitedPerDayAOEMagicDecisions";


            LimitedPerDayAOE_Magic_Decision = BuildNewDecisionDefinition(
                   "DH" + text,
                   DatabaseHelper.DecisionDefinitions.CastMagic_DPS_AoE
                    );

            LimitedPerDayAOE_Magic_Decision.Decision.SetStringParameter("LimitedPerDayAOE");
        }
        public static void BuildNewAtWillSelfBuff_Magic_Decision()
        {
            string text = "AtWillSelfBuffMagicDecisions";


            AtWillSelfBuff_Magic_Decision = BuildNewDecisionDefinition(
                   "DH" + text,
                   DatabaseHelper.DecisionDefinitions.CastMagic_SelfBuff
                    );


            AtWillSelfBuff_Magic_Decision.Decision.SetStringParameter("AtWillSelfBuff");
        }
        public static void BuildNewLimitedPerDayTargetDebuffMagic_Decision()
        {
            string text = "LimitedPerDayTargetDebuffMagicDecisions";


            LimitedPerDayTargetDebuff_Magic_Decision = BuildNewDecisionDefinition(
                   "DH" + text,
                   DatabaseHelper.DecisionDefinitions.CastMagic_Debuff_SingleTarget
                    );

            LimitedPerDayTargetDebuff_Magic_Decision.Decision.SetStringParameter("LimitedPerDayTargetDebuff");
        }

        public static void BuildNewSolar_CombatDecisions()
        {
            string text = "SolarCombatDecisions";


            Solar_CombatDecisions = BuildNewDecisionPackageDefinition(
                   "DH" + text,
                   DatabaseHelper.DecisionPackageDefinitions.DefaultRangeWithBackupMeleeDecisions
                    );

            Solar_CombatDecisions.Package.WeightedDecisions.Clear();

            //
            WeightedDecisionDescription weightedDecisionDescription = new WeightedDecisionDescription();
            weightedDecisionDescription.SetDecision(DatabaseHelper.DecisionDefinitions.RangedAttack_Default);
            weightedDecisionDescription.SetWeight(4.5f);


            //
            WeightedDecisionDescription weightedDecisionDescription_0 = new WeightedDecisionDescription();
            weightedDecisionDescription_0.SetDecision(DatabaseHelper.DecisionDefinitions.MeleeAttack_Default);
            weightedDecisionDescription_0.SetWeight(1.5f);

            //
            WeightedDecisionDescription weightedDecisionDescription_1 = new WeightedDecisionDescription();
            weightedDecisionDescription_1.SetDecision(AtWillSelfBuff_Magic_Decision);
            weightedDecisionDescription_1.SetWeight(2.5f);

            //
            WeightedDecisionDescription weightedDecisionDescription_2 = new WeightedDecisionDescription();
            weightedDecisionDescription_2.SetDecision(DatabaseHelper.DecisionDefinitions.CastMagic_SelfBuff_WithRandom);
            weightedDecisionDescription_2.SetWeight(2.5f);

            //
            WeightedDecisionDescription weightedDecisionDescription_3 = new WeightedDecisionDescription();
            weightedDecisionDescription_3.SetDecision(DatabaseHelper.DecisionDefinitions.CastMagic_Debuff_AoE);
            weightedDecisionDescription_3.SetWeight(2.5f);

            // don't know if keeping descending weight order is important or not but it matches the preexisting format in
            // all other DecisionPackageDefinition so it might be necessary. can't hurt to order it
            Solar_CombatDecisions.Package.WeightedDecisions.AddRange
                (
                DatabaseHelper.DecisionPackageDefinitions.DefaultMeleeBeastCombatDecisions.Package.WeightedDecisions[2], // "weight": 5.0 : Move_RangedKite
                weightedDecisionDescription,
                weightedDecisionDescription_1,
                weightedDecisionDescription_2,
                weightedDecisionDescription_3,
                weightedDecisionDescription_0,
               DatabaseHelper.DecisionPackageDefinitions.DefaultMeleeBeastCombatDecisions.Package.WeightedDecisions[2] // "weight": 2.0 : LongRangePathToEnemyOnlyIfNoAction_Dash
                 );

        }

        public static void BuildNewHighLevelCaster_CombatDecisions()
        {
            string text = "HighLevelCasterCombatDecisions";


            HighLevelCaster_CombatDecisions = BuildNewDecisionPackageDefinition(
                   "DH" + text,
                   DatabaseHelper.DecisionPackageDefinitions.CubeOfLightCombatDecisions
                    );

            HighLevelCaster_CombatDecisions.Package.WeightedDecisions.Clear();

            //
            WeightedDecisionDescription weightedDecisionDescription_0 = new WeightedDecisionDescription();
            weightedDecisionDescription_0.SetDecision(CastMagic_Stoneskin_Decision);
            weightedDecisionDescription_0.SetWeight(4);

            //
            WeightedDecisionDescription weightedDecisionDescription_1 = new WeightedDecisionDescription();
            weightedDecisionDescription_1.SetDecision(DatabaseHelper.DecisionDefinitions.CastMagic_Fly_Self);
            weightedDecisionDescription_1.SetWeight(4);

            //
            WeightedDecisionDescription weightedDecisionDescription_2 = new WeightedDecisionDescription();
            weightedDecisionDescription_2.SetDecision(DatabaseHelper.DecisionDefinitions.CastMagic_SelfBuff_WithRandom);
            weightedDecisionDescription_2.SetWeight(4);

            //
            WeightedDecisionDescription weightedDecisionDescription_5 = new WeightedDecisionDescription();
            weightedDecisionDescription_5.SetDecision(DatabaseHelper.DecisionDefinitions.Move_Ranged);
            weightedDecisionDescription_5.SetWeight(3.0f);

            //
            WeightedDecisionDescription weightedDecisionDescription_8 = new WeightedDecisionDescription();
            weightedDecisionDescription_8.SetDecision(DatabaseHelper.DecisionDefinitions.Move_SoftRetreat_Flying);
            weightedDecisionDescription_8.SetWeight(1.5f);

            // don't know if keeping descending weight order is important or not but it matches the preexisting format in
            // all other DecisionPackageDefinition so it might be necessary. can't hurt to order it
            HighLevelCaster_CombatDecisions.Package.WeightedDecisions.Add(weightedDecisionDescription_0);
            HighLevelCaster_CombatDecisions.Package.WeightedDecisions.Add(weightedDecisionDescription_1);
            HighLevelCaster_CombatDecisions.Package.WeightedDecisions.Add(weightedDecisionDescription_2);
            HighLevelCaster_CombatDecisions.Package.WeightedDecisions.Add(weightedDecisionDescription_5);
            HighLevelCaster_CombatDecisions.Package.WeightedDecisions.AddRange(DatabaseHelper.DecisionPackageDefinitions.CubeOfLightCombatDecisions.Package.WeightedDecisions);

            HighLevelCaster_CombatDecisions.Package.WeightedDecisions.Add(weightedDecisionDescription_8);
        }
        public static void BuildNewVampire_CombatDecisions()
        {
            string text = "VampireCombatDecisions";


            Vampire_CombatDecisions = BuildNewDecisionPackageDefinition(
                   "DH" + text,
                   DatabaseHelper.DecisionPackageDefinitions.DefilerCombatDecisions
                    );

            Vampire_CombatDecisions.Package.WeightedDecisions.Clear();


            //
            WeightedDecisionDescription weightedDecisionDescription_0 = new WeightedDecisionDescription();
            weightedDecisionDescription_0.SetDecision(DatabaseHelper.DecisionDefinitions.LongRangePathToEnemy_DashAvoidLight);
            weightedDecisionDescription_0.SetWeight(5f);

            //
            WeightedDecisionDescription weightedDecisionDescription_1 = new WeightedDecisionDescription();
            weightedDecisionDescription_1.SetDecision(DatabaseHelper.DecisionDefinitions.MeleeAttack_Default);
            weightedDecisionDescription_1.SetWeight(4f);

            //
            WeightedDecisionDescription weightedDecisionDescription_2 = new WeightedDecisionDescription();
            weightedDecisionDescription_2.SetDecision(DatabaseHelper.DecisionDefinitions.CastMagic_Defiler_Darkness);
            weightedDecisionDescription_2.SetWeight(4f);
            weightedDecisionDescription_2.SetCooldown(3);


            //
            WeightedDecisionDescription weightedDecisionDescription_3 = new WeightedDecisionDescription();
            weightedDecisionDescription_3.SetDecision(SummonCreature_Magic_Decision);
            weightedDecisionDescription_3.SetWeight(4f);

            //
            WeightedDecisionDescription weightedDecisionDescription_4 = new WeightedDecisionDescription();
            weightedDecisionDescription_4.SetDecision(DatabaseHelper.DecisionDefinitions.CastMagic_Defiler_Teleport);
            weightedDecisionDescription_4.SetWeight(3.0f);

            //
            WeightedDecisionDescription weightedDecisionDescription_5 = new WeightedDecisionDescription();
            weightedDecisionDescription_5.SetDecision(DatabaseHelper.DecisionDefinitions.Move_AggressiveLightSensitive_Defiler);
            weightedDecisionDescription_5.SetWeight(2.0f);

            //
            WeightedDecisionDescription weightedDecisionDescription_6 = new WeightedDecisionDescription();
            weightedDecisionDescription_6.SetDecision(DatabaseHelper.DecisionDefinitions.RangedAttack_Default);
            weightedDecisionDescription_6.SetWeight(1.0f);

            //
            WeightedDecisionDescription weightedDecisionDescription_7 = new WeightedDecisionDescription();
            weightedDecisionDescription_7.SetDecision(DatabaseHelper.DecisionDefinitions.CastMagic_Defiler_Teleport_Fallback);
            weightedDecisionDescription_7.SetWeight(0.1f);



            // don't know if keeping descending weight order is important or not but it matches the preexisting format in
            // all other DecisionPackageDefinition so it might be necessary. can't hurt to order it

            Vampire_CombatDecisions.Package.WeightedDecisions.AddRange(
                                                                    weightedDecisionDescription_1,
                                                                    weightedDecisionDescription_2,
                                                                    weightedDecisionDescription_3,
                                                                    weightedDecisionDescription_4,
                                                                    weightedDecisionDescription_5,
                                                                    weightedDecisionDescription_6,
                                                                    weightedDecisionDescription_7
                                                                    );
        }

        public static void BuildNewTitan_CombatDecisions()
        {
            string text = "TitanCombatDecisions";


            Titan_CombatDecisions = BuildNewDecisionPackageDefinition(
                   "DH" + text,
                   DatabaseHelper.DecisionPackageDefinitions.DefaultMeleeBeastCombatDecisions
                    );

            Titan_CombatDecisions.Package.WeightedDecisions.Clear();

            //   AtWillAOE_Magic_Decision
            //   AtWillSelfBuff_Magic_Decision
            //   LimitedPerDayTargetDebuff_Magic_Decision
            //   LimitedPerDayAOE_Magic_Decision
            //   SummonCreature_Magic_Decision



            //
            WeightedDecisionDescription weightedDecisionDescription_0 = new WeightedDecisionDescription();
            weightedDecisionDescription_0.SetDecision(DatabaseHelper.DecisionDefinitions.LongRangePathToEnemy_Dash);
            weightedDecisionDescription_0.SetWeight(5f);

            //
            WeightedDecisionDescription weightedDecisionDescription_1 = new WeightedDecisionDescription();
            weightedDecisionDescription_1.SetDecision(LimitedPerDayAOE_Magic_Decision);
            weightedDecisionDescription_1.SetWeight(4f);

            //
            WeightedDecisionDescription weightedDecisionDescription_2 = new WeightedDecisionDescription();
            weightedDecisionDescription_2.SetDecision(AtWillAOE_Magic_Decision);
            weightedDecisionDescription_2.SetWeight(3.5f);



            //
            WeightedDecisionDescription weightedDecisionDescription_3 = new WeightedDecisionDescription();
            weightedDecisionDescription_3.SetDecision(SummonCreature_Magic_Decision);
            weightedDecisionDescription_3.SetWeight(3.5f);
            weightedDecisionDescription_3.SetDynamicCooldown(true);
            weightedDecisionDescription_3.SetCooldown(3);

            //
            WeightedDecisionDescription weightedDecisionDescription_4 = new WeightedDecisionDescription();
            weightedDecisionDescription_4.SetDecision(DatabaseHelper.DecisionDefinitions.MeleeAttack_Default);
            weightedDecisionDescription_4.SetWeight(3.0f);

            //
            WeightedDecisionDescription weightedDecisionDescription_5 = new WeightedDecisionDescription();
            weightedDecisionDescription_5.SetDecision(DatabaseHelper.DecisionDefinitions.Move_AggressiveSingleTargetAndSpread);
            weightedDecisionDescription_5.SetWeight(2.0f);

            //
            WeightedDecisionDescription weightedDecisionDescription_6 = new WeightedDecisionDescription();
            weightedDecisionDescription_6.SetDecision(DatabaseHelper.DecisionDefinitions.RangedAttack_Default);
            weightedDecisionDescription_6.SetWeight(1.0f);

            //
            WeightedDecisionDescription weightedDecisionDescription_7 = new WeightedDecisionDescription();
            weightedDecisionDescription_7.SetDecision(DatabaseHelper.DecisionDefinitions.Emote_Angry);
            weightedDecisionDescription_7.SetWeight(1.0f);



            // don't know if keeping descending weight order is important or not but it matches the preexisting format in
            // all other DecisionPackageDefinition so it might be necessary. can't hurt to order it

            Titan_CombatDecisions.Package.WeightedDecisions.AddRange(
                                                                    weightedDecisionDescription_1,
                                                                    weightedDecisionDescription_2,
                                                                    weightedDecisionDescription_3,
                                                                    weightedDecisionDescription_4,
                                                                    weightedDecisionDescription_5,
                                                                    weightedDecisionDescription_6,
                                                                    weightedDecisionDescription_7
                                                                    );
        }
        public static void BuildNewConstructTitan_CombatDecisions()
        {
            string text = "ConstructTitanCombatDecisions";


            ConstructTitan_CombatDecisions = BuildNewDecisionPackageDefinition(
                   "DH" + text,
                   DatabaseHelper.DecisionPackageDefinitions.DefaultMeleeBeastCombatDecisions
                    );

            ConstructTitan_CombatDecisions.Package.WeightedDecisions.Clear();

            //   AtWillAOE_Magic_Decision
            //   AtWillSelfBuff_Magic_Decision
            //   LimitedPerDayTargetDebuff_Magic_Decision
            //   LimitedPerDayAOE_Magic_Decision
            //   SummonCreature_Magic_Decision



            //
            WeightedDecisionDescription weightedDecisionDescription_0 = new WeightedDecisionDescription();
            weightedDecisionDescription_0.SetDecision(DatabaseHelper.DecisionDefinitions.LongRangePathToEnemy_Dash);
            weightedDecisionDescription_0.SetWeight(5f);

            //
            WeightedDecisionDescription weightedDecisionDescription_1 = new WeightedDecisionDescription();
            weightedDecisionDescription_1.SetDecision(DatabaseHelper.DecisionDefinitions.CastMagic_DPS_AoE_DragonBreath);
            weightedDecisionDescription_1.SetWeight(4f);

            //
            WeightedDecisionDescription weightedDecisionDescription_2 = new WeightedDecisionDescription();
            weightedDecisionDescription_2.SetDecision(DatabaseHelper.DecisionDefinitions.CastMagic_FrightfulPresence_Dragon);
            weightedDecisionDescription_2.SetWeight(3.5f);



            //
            WeightedDecisionDescription weightedDecisionDescription_3 = new WeightedDecisionDescription();
            weightedDecisionDescription_3.SetDecision(AtWillAOE_Magic_Decision);
            weightedDecisionDescription_3.SetWeight(3.5f);
            weightedDecisionDescription_3.SetDynamicCooldown(true);
            weightedDecisionDescription_3.SetCooldown(3);

            //
            WeightedDecisionDescription weightedDecisionDescription_4 = new WeightedDecisionDescription();
            weightedDecisionDescription_4.SetDecision(DatabaseHelper.DecisionDefinitions.MeleeAttack_Default);
            weightedDecisionDescription_4.SetWeight(3.0f);

            //
            WeightedDecisionDescription weightedDecisionDescription_5 = new WeightedDecisionDescription();
            weightedDecisionDescription_5.SetDecision(DatabaseHelper.DecisionDefinitions.Move_AggressiveSingleTargetAndSpread);
            weightedDecisionDescription_5.SetWeight(2.0f);

            //
            WeightedDecisionDescription weightedDecisionDescription_6 = new WeightedDecisionDescription();
            weightedDecisionDescription_6.SetDecision(DatabaseHelper.DecisionDefinitions.RangedAttack_Default);
            weightedDecisionDescription_6.SetWeight(3.0f);

            //
            WeightedDecisionDescription weightedDecisionDescription_7 = new WeightedDecisionDescription();
            weightedDecisionDescription_7.SetDecision(DatabaseHelper.DecisionDefinitions.Emote_Angry);
            weightedDecisionDescription_7.SetWeight(1.0f);



            // don't know if keeping descending weight order is important or not but it matches the preexisting format in
            // all other DecisionPackageDefinition so it might be necessary. can't hurt to order it

            ConstructTitan_CombatDecisions.Package.WeightedDecisions.AddRange(
                                                                    weightedDecisionDescription_1,
                                                                    weightedDecisionDescription_2,
                                                                    weightedDecisionDescription_3,
                                                                    weightedDecisionDescription_4,
                                                                    weightedDecisionDescription_5,
                                                                    weightedDecisionDescription_6,
                                                                    weightedDecisionDescription_7
                                                                    );
        }

        public static void BuildNewNaga_CombatDecisions()
        {
            string text = "NagaCombatDecisions";


            Naga_CombatDecisions = BuildNewDecisionPackageDefinition(
                   "DH" + text,
                   DatabaseHelper.DecisionPackageDefinitions.FlyingSnakeCombatDecisions
                    );

            Naga_CombatDecisions.Package.WeightedDecisions.Clear();


            //
            WeightedDecisionDescription weightedDecisionDescription_1 = new WeightedDecisionDescription();
            weightedDecisionDescription_1.SetDecision(DatabaseHelper.DecisionDefinitions.MeleeAttack_Default);
            weightedDecisionDescription_1.SetWeight(4.5f);

            //
            WeightedDecisionDescription weightedDecisionDescription_2 = new WeightedDecisionDescription();
            weightedDecisionDescription_2.SetDecision(DatabaseHelper.DecisionDefinitions.RangedAttack_Default);
            weightedDecisionDescription_2.SetWeight(4.5f);

            //
            WeightedDecisionDescription weightedDecisionDescription_4 = new WeightedDecisionDescription();
            weightedDecisionDescription_4.SetDecision(DatabaseHelper.DecisionDefinitions.CastMagic_SelfHeal);
            weightedDecisionDescription_4.SetWeight(4.5f);

            //
            WeightedDecisionDescription weightedDecisionDescription_3 = new WeightedDecisionDescription();
            weightedDecisionDescription_3.SetDecision(DatabaseHelper.DecisionDefinitions.CastMagic_SelfBuff);
            weightedDecisionDescription_3.SetWeight(3.5f);

            //
            WeightedDecisionDescription weightedDecisionDescription_5 = new WeightedDecisionDescription();
            weightedDecisionDescription_5.SetDecision(DatabaseHelper.DecisionDefinitions.Move_Ranged);
            weightedDecisionDescription_5.SetWeight(3.0f);

            //
            WeightedDecisionDescription weightedDecisionDescription_6 = new WeightedDecisionDescription();
            weightedDecisionDescription_6.SetDecision(DatabaseHelper.DecisionDefinitions.CastMagic_DPS_SingleTarget);
            weightedDecisionDescription_6.SetWeight(2.5f);

            //
            WeightedDecisionDescription weightedDecisionDescription_7 = new WeightedDecisionDescription();
            weightedDecisionDescription_7.SetDecision(DatabaseHelper.DecisionDefinitions.CastMagic_Debuff_SingleTarget);
            weightedDecisionDescription_7.SetWeight(2.5f);

            //
            WeightedDecisionDescription weightedDecisionDescription_8 = new WeightedDecisionDescription();
            weightedDecisionDescription_8.SetDecision(DatabaseHelper.DecisionDefinitions.Move_SoftRetreat_Flying);
            weightedDecisionDescription_8.SetWeight(1.5f);

            // don't know if keeping descending weight order is important or not but it matches the preexisting format in
            // all other DecisionPackageDefinition so it might be necessary. can't hurt to order it

            Naga_CombatDecisions.Package.WeightedDecisions.AddRange(
                                                                    weightedDecisionDescription_1,
                                                                    weightedDecisionDescription_2,
                                                                    weightedDecisionDescription_3,
                                                                    weightedDecisionDescription_4,
                                                                    weightedDecisionDescription_5,
                                                                    weightedDecisionDescription_6,
                                                                    weightedDecisionDescription_7,
                                                                    weightedDecisionDescription_8
                                                                    );
        }

        public static void BuildNewPitFiend_CombatDecisions()
        {
            string text = "PitFiendCombatDecisions";


            PitFiend_CombatDecisions = BuildNewDecisionPackageDefinition(
                   "DH" + text,
                   DatabaseHelper.DecisionPackageDefinitions.DefaultMeleeBeastCombatDecisions
                    );

            PitFiend_CombatDecisions.Package.WeightedDecisions.Clear();

            // summon creature 1/day
            WeightedDecisionDescription weightedDecisionDescription = new WeightedDecisionDescription();
            weightedDecisionDescription.SetDecision(SummonCreature_Magic_Decision);
            weightedDecisionDescription.SetWeight(4.5f);


            // frightful presence roll 5 or 6
            WeightedDecisionDescription weightedDecisionDescription_0 = new WeightedDecisionDescription();
            weightedDecisionDescription_0.SetDecision(DatabaseHelper.DecisionDefinitions.CastMagic_FrightfulPresence_Dragon);
            weightedDecisionDescription_0.SetWeight(4.5f);

            // wall of fire 3/day
            WeightedDecisionDescription weightedDecisionDescription_1 = new WeightedDecisionDescription();
            weightedDecisionDescription_1.SetDecision(LimitedPerDayTargetDebuff_Magic_Decision);
            weightedDecisionDescription_1.SetWeight(3.0f);

            // hold person 3/day
            WeightedDecisionDescription weightedDecisionDescription_2 = new WeightedDecisionDescription();
            weightedDecisionDescription_2.SetDecision(LimitedPerDayAOE_Magic_Decision);
            weightedDecisionDescription_2.SetWeight(3.0f);

            // at will fireballs
            WeightedDecisionDescription weightedDecisionDescription_3 = new WeightedDecisionDescription();
            weightedDecisionDescription_3.SetDecision(AtWillAOE_Magic_Decision);
            weightedDecisionDescription_3.SetWeight(3.0f);

            // don't know if keeping descending weight order is important or not but it matches the preexisting format in
            // all other DecisionPackageDefinition so it might be necessary. can't hurt to order it
            PitFiend_CombatDecisions.Package.WeightedDecisions.AddRange
                (

                DatabaseHelper.DecisionPackageDefinitions.DefaultMeleeBeastCombatDecisions.Package.WeightedDecisions[0], // "weight": 5.0
                weightedDecisionDescription,
                weightedDecisionDescription_0,
                DatabaseHelper.DecisionPackageDefinitions.DefaultMeleeBeastCombatDecisions.Package.WeightedDecisions[1], // "weight": 3.0
                weightedDecisionDescription_1,
                weightedDecisionDescription_2,
                weightedDecisionDescription_3,
                DatabaseHelper.DecisionPackageDefinitions.DefaultMeleeBeastCombatDecisions.Package.WeightedDecisions[2] // "weight": 2.0
                );

        }

        public static void BuildNewBalor_CombatDecisions()
        {
            string text = "BalorCombatDecisions";


            Balor_CombatDecisions = BuildNewDecisionPackageDefinition(
                   "DH" + text,
                   DatabaseHelper.DecisionPackageDefinitions.PhaseSpiderCombatDecisions
                    );

            Balor_CombatDecisions.Package.WeightedDecisions.Clear();

            // summon creature 1/day
            WeightedDecisionDescription weightedDecisionDescription = new WeightedDecisionDescription();
            weightedDecisionDescription.SetDecision(SummonCreature_Magic_Decision);
            weightedDecisionDescription.SetWeight(4.5f);




            // don't know if keeping descending weight order is important or not but it matches the preexisting format in
            // all other DecisionPackageDefinition so it might be necessary. can't hurt to order it
            Balor_CombatDecisions.Package.WeightedDecisions.AddRange
                (

                DatabaseHelper.DecisionPackageDefinitions.PhaseSpiderCombatDecisions.Package.WeightedDecisions[0], // "weight": 5.0
                weightedDecisionDescription,
                DatabaseHelper.DecisionPackageDefinitions.PhaseSpiderCombatDecisions.Package.WeightedDecisions[1], // "weight": 3.0

                DatabaseHelper.DecisionPackageDefinitions.PhaseSpiderCombatDecisions.Package.WeightedDecisions[2] // "weight": 2.0
                );

        }

        public static void BuildNewNalfeshnee_CombatDecisions()
        {
            string text = "NalfeshneeCombatDecisions";


            Nalfeshnee_CombatDecisions = BuildNewDecisionPackageDefinition(
                   "DH" + text,
                   DatabaseHelper.DecisionPackageDefinitions.PhaseSpiderCombatDecisions
                    );

            Nalfeshnee_CombatDecisions.Package.WeightedDecisions.Clear();

            // frightful presence roll 5 or 6
            WeightedDecisionDescription weightedDecisionDescription = new WeightedDecisionDescription();
            weightedDecisionDescription.SetDecision(DatabaseHelper.DecisionDefinitions.CastMagic_FrightfulPresence_Dragon);
            weightedDecisionDescription.SetWeight(4.5f);




            // don't know if keeping descending weight order is important or not but it matches the preexisting format in
            // all other DecisionPackageDefinition so it might be necessary. can't hurt to order it
            Nalfeshnee_CombatDecisions.Package.WeightedDecisions.AddRange
                (

                DatabaseHelper.DecisionPackageDefinitions.PhaseSpiderCombatDecisions.Package.WeightedDecisions[0], // "weight": 5.0
                weightedDecisionDescription,
                DatabaseHelper.DecisionPackageDefinitions.PhaseSpiderCombatDecisions.Package.WeightedDecisions[1], // "weight": 3.0

                DatabaseHelper.DecisionPackageDefinitions.PhaseSpiderCombatDecisions.Package.WeightedDecisions[2] // "weight": 2.0
                );

        }

        //
        //
        //

        public static FeatureDefinitionCastSpell BuildNewCaster(string name, FeatureDefinitionCastSpell baseCaster)
        {
            return FeatureDefinitionCastSpellBuilder
                .Create(baseCaster, name, DefinitionBuilder.CENamespaceGuid)
                .SetOrUpdateGuiPresentation(Category.Spell)
                .AddToDB();
        }

        public static FeatureDefinitionDamageAffinity BuildNewDamageAffinity(string name, FeatureDefinitionDamageAffinity baseDamageAffinity)
        {
            return FeatureDefinitionDamageAffinityBuilder
                .Create(baseDamageAffinity, name, DefinitionBuilder.CENamespaceGuid)
                .SetOrUpdateGuiPresentation(Category.MonsterPower)
                .AddToDB();
        }

        public static DecisionPackageDefinition BuildNewDecisionPackageDefinition(string name, DecisionPackageDefinition baseDecisionPackageDefinition)
        {
            return DecisionPackageDefinitionBuilder
                .Create(baseDecisionPackageDefinition, name, DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentationNoContent()
                .AddToDB();
        }

        public static DecisionDefinition BuildNewDecisionDefinition(string name, DecisionDefinition baseDecisionDefinition)
        {
            return DecisionDefinitionBuilder
                .Create(baseDecisionDefinition, name, DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentationNoContent()
                .AddToDB();
        }

        public static FeatureDefinitionMagicAffinity BuildNewMagicAffinity(string name, FeatureDefinitionMagicAffinity baseMagicAffinity)
        {
            return FeatureDefinitionMagicAffinityBuilder
                .Create(baseMagicAffinity, name, DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentationNoContent()
                .AddToDB();
        }

        public static SpellListDefinition BuildNewSpelllist(string name, SpellListDefinition baseSpellList)
        {
            return SpellListDefinitionBuilder
                .Create(baseSpellList, name, DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentationNoContent()
                .AddToDB();
        }
    }
}
