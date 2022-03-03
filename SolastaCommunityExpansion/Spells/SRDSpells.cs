using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomFeatureDefinitions;
using SolastaCommunityExpansion.Models;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using UnityEngine.AddressableAssets;
using static FeatureDefinitionCastSpell;
using static SolastaModApi.DatabaseHelper;
using static SolastaModApi.DatabaseHelper.ConditionDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaModApi.DatabaseHelper.MonsterDefinitions;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Spells
{
    internal static class SrdSpells
    {
        internal static readonly Guid DhBaseGuid = new("05c1b1dbae144731b4505c1232fdc37e");

        private static readonly SpellDefinition DivineWord = BuildDivineWord();
        private static readonly SpellDefinition FingerOfDeath = BuildFingerOfDeath();
        private static readonly SpellDefinition ReverseGravity = BuildReverseGravity();
        private static readonly SpellDefinition ConjureCelestial = BuildConjureCelestial();
        private static readonly SpellDefinition DominateMonster = BuildDominateMonster();
        private static readonly SpellDefinition Feeblemind = BuildFeeblemind();
        private static readonly SpellDefinition HolyAura = BuildHolyAura();
        private static readonly SpellDefinition IncendiaryCloud = BuildIncendiaryCloud();
        private static readonly SpellDefinition Maze = BuildMaze();
        private static readonly SpellDefinition MindBlank = BuildMindBlank();
        private static readonly SpellDefinition PowerWordStun = BuildPowerWordStun();
        private static readonly SpellDefinition SunBurst = BuildSunBurst();
        private static readonly SpellDefinition Foresight = BuildForesight();
        private static readonly SpellDefinition MassHeal = BuildMassHeal();
        private static readonly SpellDefinition MeteorSwarmSingleTarget = BuildMeteorSwarmSingleTarget();
        private static readonly SpellDefinition PowerWordHeal = BuildPowerWordHeal();
        private static readonly SpellDefinition PowerWordKill = BuildPowerWordKill();
        private static readonly SpellDefinition TimeStop = BuildTimeStop();
        private static readonly SpellDefinition Shapechange = BuildShapechange();
        private static readonly SpellDefinition Weird = BuildWeird();

        internal static void AddToDB()
        {
            _ = DivineWord;
            _ = FingerOfDeath;
            _ = ReverseGravity;
            _ = ConjureCelestial;
            _ = DominateMonster;
            _ = Feeblemind;
            _ = HolyAura;
            _ = IncendiaryCloud;
            _ = Maze;
            _ = MindBlank;
            _ = PowerWordStun;
            _ = SunBurst;
            _ = Foresight;
            _ = MassHeal;
            _ = MeteorSwarmSingleTarget;
            _ = PowerWordHeal;
            _ = PowerWordKill;
            _ = TimeStop;
            _ = Shapechange;
            _ = Weird;
        }

        internal static void Register()
        {
            // 7th level
            SpellsContext.RegisterSpell(DivineWord, isFromOtherMod: false, SpellsContext.NOT_IN_MIN_SET, "SpellListCleric");
            SpellsContext.RegisterSpell(FingerOfDeath, isFromOtherMod: false, SpellsContext.NOT_IN_MIN_SET, "SpellListWizard", "SpellListSorcerer");
            SpellsContext.RegisterSpell(ReverseGravity, isFromOtherMod: false, SpellsContext.NOT_IN_MIN_SET, "SpellListDruid", "SpellListWizard", "SpellListSorcerer");
            SpellsContext.RegisterSpell(ConjureCelestial, isFromOtherMod: false, SpellsContext.NOT_IN_MIN_SET, "SpellListCleric");

            // 8th level
            SpellsContext.RegisterSpell(DominateMonster, isFromOtherMod: false, SpellsContext.NOT_IN_MIN_SET, "SpellListWizard", "SpellListSorcerer", "WitchSpellList");
            SpellsContext.RegisterSpell(Feeblemind, isFromOtherMod: false, SpellsContext.NOT_IN_MIN_SET, "SpellListWizard", "SpellListDruid", "WitchSpellList");
            SpellsContext.RegisterSpell(HolyAura, isFromOtherMod: false, SpellsContext.NOT_IN_MIN_SET, "SpellListCleric");
            SpellsContext.RegisterSpell(IncendiaryCloud, isFromOtherMod: false, SpellsContext.NOT_IN_MIN_SET, "SpellListWizard", "SpellListSorcerer");
            SpellsContext.RegisterSpell(Maze, isFromOtherMod: false, SpellsContext.NOT_IN_MIN_SET, "SpellListWizard");
            SpellsContext.RegisterSpell(MindBlank, isFromOtherMod: false, SpellsContext.NOT_IN_MIN_SET, "SpellListWizard", "WitchSpellList");
            SpellsContext.RegisterSpell(PowerWordStun, isFromOtherMod: false, SpellsContext.NOT_IN_MIN_SET, "SpellListWizard", "SpellListSorcerer", "WitchSpellList");
            SpellsContext.RegisterSpell(SunBurst, isFromOtherMod: false, SpellsContext.NOT_IN_MIN_SET, "SpellListDruid", "SpellListWizard", "SpellListSorcerer");

            // 9th level
            SpellsContext.RegisterSpell(Foresight, isFromOtherMod: false, SpellsContext.NOT_IN_MIN_SET, "SpellListDruid", "SpellListWizard", "WitchSpellList");
            SpellsContext.RegisterSpell(MassHeal, isFromOtherMod: false, SpellsContext.NOT_IN_MIN_SET, "SpellListCleric");
            SpellsContext.RegisterSpell(MeteorSwarmSingleTarget, isFromOtherMod: false, SpellsContext.NOT_IN_MIN_SET, "SpellListWizard", "SpellListSorcerer");
            SpellsContext.RegisterSpell(PowerWordHeal, isFromOtherMod: false, SpellsContext.NOT_IN_MIN_SET, "SpellListCleric");
            SpellsContext.RegisterSpell(PowerWordKill, isFromOtherMod: false, SpellsContext.NOT_IN_MIN_SET, "SpellListWizard", "SpellListSorcerer");
            SpellsContext.RegisterSpell(TimeStop, isFromOtherMod: false, SpellsContext.NOT_IN_MIN_SET, "SpellListWizard", "SpellListSorcerer");
            SpellsContext.RegisterSpell(Shapechange, isFromOtherMod: false, SpellsContext.NOT_IN_MIN_SET, "SpellListDruid", "SpellListWizard");
            SpellsContext.RegisterSpell(Weird, isFromOtherMod: false, SpellsContext.NOT_IN_MIN_SET, "SpellListWizard", "WitchSpellList");
        }

        //
        // ChrisJohnDigital Spells
        //

        private static SpellDefinition BuildDivineWord()
        {
            return SpellDefinitionBuilder
                .Create("CJDivineWord", "18ecba41-a8ac-4048-979e-2139e66934a7")
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
                .SetMaterialComponent(RuleDefinitions.MaterialComponentType.None)
                .SetCastingTime(RuleDefinitions.ActivationTime.BonusAction)
                .SetSomaticComponent(false)
                .SetVerboseComponent(true)
                .SetSpellLevel(7)
                .SetGuiPresentation(Category.Spell, DivineFavor.GuiPresentation.SpriteReference)
                .SetEffectDescription(new EffectDescriptionBuilder()
                    .SetTargetingData(
                        RuleDefinitions.Side.All,
                        RuleDefinitions.RangeType.Distance,
                        6,
                        RuleDefinitions.TargetType.IndividualsUnique,
                        20,
                        1,
                        ActionDefinitions.ItemSelectionType.None)
                    .SetParticleEffectParameters(MassHealingWord.EffectDescription.EffectParticleParameters)
                    .SetDurationData(RuleDefinitions.DurationType.Instantaneous, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn)
                    .SetSavingThrowData(
                        true,
                        false,
                        AttributeDefinitions.Charisma,
                        true,
                        RuleDefinitions.EffectDifficultyClassComputation.SpellCastingFeature,
                        AttributeDefinitions.Wisdom,
                        15,
                        false,
                        new List<SaveAffinityBySenseDescription>())
                    .AddEffectForm(new DivineWordEffectForm())
                    .Build())
                .AddToDB();
        }

        private sealed class DivineWordEffectForm : CustomEffectForm
        {
            private readonly List<string> monsterFamilyPlaneshiftList = new()
            {
                "Celestial",
                "Elemental",
                "Fey",
                "Fiend",
            };

            public override void ApplyForm(RulesetImplementationDefinitions.ApplyFormsParams formsParams, bool retargeting, bool proxyOnly, bool forceSelfConditionOnly)
            {
                if (formsParams.saveOutcome == RuleDefinitions.RollOutcome.CriticalSuccess || formsParams.saveOutcome == RuleDefinitions.RollOutcome.Success)
                {
                    return;
                }

                // If the target is in one of the special families, banish it.
                if (formsParams.targetCharacter is RulesetCharacterMonster monster && monsterFamilyPlaneshiftList.Contains(monster.CharacterFamily))
                {
                    ApplyCondition(formsParams, ConditionBanished, RuleDefinitions.DurationType.Day, 1);

                    return;
                }

                int curHP = formsParams.targetCharacter.CurrentHitPoints;

                if (curHP <= 20)
                {
                    if (formsParams.targetCharacter.IsDead)
                    {
                        return;
                    }

                    ServiceRepository.GetService<IGameLocationActionService>().InstantKillCharacter(formsParams.targetCharacter as RulesetCharacter);
                }
                else if (curHP <= 30)
                {
                    // blind, deafened, stunned 1 hour
                    ApplyCondition(formsParams, ConditionDeafened, RuleDefinitions.DurationType.Hour, 1);
                    ApplyCondition(formsParams, ConditionBlinded, RuleDefinitions.DurationType.Hour, 1);
                    ApplyCondition(formsParams, ConditionStunned, RuleDefinitions.DurationType.Hour, 1);
                }
                else if (curHP <= 40)
                {
                    // deafened, blinded 10 minutes
                    ApplyCondition(formsParams, ConditionDeafened, RuleDefinitions.DurationType.Minute, 10);
                    ApplyCondition(formsParams, ConditionBlinded, RuleDefinitions.DurationType.Minute, 10);
                }
                else if (curHP <= 50)
                {
                    // deafened 1 minute
                    ApplyCondition(formsParams, ConditionDeafened, RuleDefinitions.DurationType.Minute, 1);
                }
            }

            public override void FillTags(Dictionary<string, TagsDefinitions.Criticity> tagsMap)
            {
                ConditionDeafened.FillTags(tagsMap);
                ConditionBlinded.FillTags(tagsMap);
                ConditionStunned.FillTags(tagsMap);
                ConditionBanished.FillTags(tagsMap);
            }

            private static void ApplyCondition(RulesetImplementationDefinitions.ApplyFormsParams formsParams, ConditionDefinition condition, RuleDefinitions.DurationType durationType, int durationParam)
            {
                // Prepare params for inflicting conditions
                ulong sourceGuid = formsParams.sourceCharacter != null ? formsParams.sourceCharacter.Guid : 0L;
                string sourceFaction = formsParams.sourceCharacter != null ? formsParams.sourceCharacter.CurrentFaction.Name : string.Empty;
                string effectDefinitionName = string.Empty;

                if (formsParams.attackMode != null)
                {
                    effectDefinitionName = formsParams.attackMode.SourceDefinition.Name;
                }
                else if (formsParams.activeEffect != null)
                {
                    effectDefinitionName = formsParams.activeEffect.SourceDefinition.Name;
                }

                int sourceAbilityBonus = (formsParams.activeEffect?.ComputeSourceAbilityBonus(formsParams.sourceCharacter)) ?? 0;

                formsParams.targetCharacter.InflictCondition(condition.Name, durationType, durationParam, RuleDefinitions.TurnOccurenceType.EndOfTurn, "11Effect", sourceGuid, sourceFaction, formsParams.effectLevel, effectDefinitionName, 0, sourceAbilityBonus);
            }
        }

        //
        // DubbHerder SRD Spells
        //

        private static SpellDefinition BuildFingerOfDeath()
        {
            const string text = "DHFingerOfDeathSpell";

            EffectDescriptionBuilder effectDescription = new EffectDescriptionBuilder()
                .SetDurationData(
                    RuleDefinitions.DurationType.Instantaneous,
                    1,
                    RuleDefinitions.TurnOccurenceType.EndOfTurn)
                .SetTargetingData(
                    RuleDefinitions.Side.Enemy,
                    RuleDefinitions.RangeType.Distance,
                    12,
                    RuleDefinitions.TargetType.Individuals,
                    1,
                    1,
                    ActionDefinitions.ItemSelectionType.None)

                .AddEffectForm(new EffectFormBuilder()
                    .SetDamageForm(
                        false,
                        RuleDefinitions.DieType.D8,
                        RuleDefinitions.DamageTypeNecrotic,
                        30,
                        RuleDefinitions.DieType.D8,
                        7,
                        RuleDefinitions.HealFromInflictedDamage.Never,
                        new List<RuleDefinitions.TrendInfo>())
                    .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.HalfDamage)
                    .Build())

                .SetSavingThrowData(
                    true,
                    false,
                    SmartAttributeDefinitions.Constitution.name,
                    true,
                    RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                    SmartAttributeDefinitions.Constitution.name,
                    20,
                    false,
                    new List<SaveAffinityBySenseDescription>());

            return SpellDefinitionBuilder
                .Create(text, DhBaseGuid)
                .SetGuiPresentation(Category.Spell, RayOfEnfeeblement.GuiPresentation.SpriteReference)
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(7)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetVerboseComponent(true)
                .SetSomaticComponent(false)
                .SetEffectDescription(effectDescription.Build())
                .SetAiParameters(new SpellAIParameters())
                .AddToDB();
        }

        //   internal class SummonZombieConditionBuilder : BaseDefinitionBuilder<ConditionDefinition>
        //   {
        //       const string Name = "DHSummonZombieSpellcondition";
        //       const string Guid = "a4d94551-efd3-4987-918c-a35b24d607a6";
        //       const string TitleString = "Condition/&DHSummonZombieSpellTitle";
        //       const string DescriptionString = "Condition/&DHSummonZombieSpellDescription";
        //       protected SummonZombieConditionBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionDummy, name, guid)
        //       {
        //           Definition.GuiPresentation.Title = TitleString;
        //           Definition.GuiPresentation.Description = DescriptionString;
        //
        //           Definition.SetSilentWhenAdded ( false);
        //           Definition.SetSilentWhenRemoved(false);
        //
        //           Definition.SetConditionType(RuleDefinitions.ConditionType.Detrimental);
        //           Definition.SetSpecialDuration(true);
        //           Definition.SetDurationType(RuleDefinitions.DurationType.Round);
        //           Definition.SetTurnOccurence(RuleDefinitions.TurnOccurenceType.EndOfTurn);
        //
        //
        //           Definition.Features.Clear();
        //           Definition.Features.Empty();
        //           //  Definition.Features.AddRange();
        //
        //
        //
        //           Definition.RecurrentEffectForms.Add
        //               (
        //                   new EffectFormBuilder()
        //                     .SetSummonForm
        //                     (
        //                     SummonForm.Type.Creature,
        //                     new ItemDefinition(),
        //                     1,
        //                     DatabaseHelper.MonsterDefinitions.Zombie.name,
        //                     DatabaseHelper.ConditionDefinitions.ConditionMindDominatedByCaster,
        //                     true,
        //                     DatabaseHelper.DecisionPackageDefinitions.IdleGuard_Default,
        //                     new EffectProxyDefinition()).Build()
        //               );
        //
        //
        //       }
        //       internal static ConditionDefinition CreateAndAddToDB(string name, string guid)
        //           => new SummonZombieConditionBuilder(name, guid).AddToDB();
        //       internal static ConditionDefinition SummonZombieCondition = CreateAndAddToDB(Name, Guid);
        //   }

        private static SpellDefinition BuildReverseGravity()
        {
            const string text = "DHReverseGravitySpell";

            EffectDescriptionBuilder effectDescription = new EffectDescriptionBuilder()
                .SetDurationData(
                     RuleDefinitions.DurationType.Minute,
                     1,
                     RuleDefinitions.TurnOccurenceType.EndOfTurn)
                .SetTargetingData(
                    RuleDefinitions.Side.All,
                    RuleDefinitions.RangeType.Distance,
                    12,
                    RuleDefinitions.TargetType.Cylinder,
                    10,
                    10,
                    ActionDefinitions.ItemSelectionType.None)
                .SetSavingThrowData(
                    true,
                    false,
                    SmartAttributeDefinitions.Dexterity.name,
                    true,
                    RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                    SmartAttributeDefinitions.Dexterity.name,
                    20,
                    false,
                    new List<SaveAffinityBySenseDescription>())
                .AddEffectForm(new EffectFormBuilder()
                    .SetConditionForm(
                        ReverseGravityConditionBuilder.ReverseGravityCondition,
                        ConditionForm.ConditionOperation.Add,
                        false,
                        false,
                        new List<ConditionDefinition>())
                        .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.Negates)
                    .Build())
                .AddEffectForm(new EffectFormBuilder()
                    .SetMotionForm(
                        MotionForm.MotionType.Levitate,
                        10)
                    .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.Negates)
                    .Build())
                .SetRecurrentEffect(Entangle.EffectDescription.RecurrentEffect);

            return SpellDefinitionBuilder
                .Create(text, DhBaseGuid)
                .SetGuiPresentation(Category.Spell, Thunderwave.GuiPresentation.SpriteReference)
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(7)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetVerboseComponent(true)
                .SetSomaticComponent(true)
                .SetEffectDescription(effectDescription.Build())
                .SetAiParameters(new SpellAIParameters())
                .SetRequiresConcentration(true)
                .AddToDB();
        }

        internal class ReverseGravityConditionBuilder : ConditionDefinitionBuilder
        {
            private const string Name = "DHReverseGravitySpellcondition";
            private const string Guid = "809f1cef-6bdc-4b5a-93bf-275af8ab0b36";

            protected ReverseGravityConditionBuilder(string name, string guid) : base(ConditionLevitate, name, guid)
            {
                Definition.SetConditionType(RuleDefinitions.ConditionType.Neutral);
                Definition.Features.SetRange
                (
                    FeatureDefinitionMovementAffinitys.MovementAffinityConditionLevitate,
                    FeatureDefinitionMoveModes.MoveModeFly2
                );
            }

            private static ConditionDefinition CreateAndAddToDB(string name, string guid)
            {
                return new ReverseGravityConditionBuilder(name, guid)
                    .SetGuiPresentation("DHReverseGravitySpell", Category.Condition)
                    .AddToDB();
            }

            internal static readonly ConditionDefinition ReverseGravityCondition = CreateAndAddToDB(Name, Guid);
        }

        private static SpellDefinition BuildConjureCelestial()
        {
            SpellListDefinition couatlSpellList = SpellListDefinitionBuilder
                .Create(SpellListDefinitions.SpellListCleric, "DHCouatlSpellList", DhBaseGuid)
                .SetGuiPresentationNoContent()
                .ClearSpells()
                .SetSpellsAtLevel(1, Bless, CureWounds, DetectEvilAndGood, DetectMagic, Shield)
                .SetSpellsAtLevel(2, LesserRestoration, ProtectionFromPoison)
                .SetSpellsAtLevel(5, GreaterRestoration)
                .SetMaxSpellLevel(5, false)
                .AddToDB();

            FeatureDefinitionCastSpell castSpellCouatl = FeatureDefinitionCastSpellBuilder
                .Create("DHCastSpellCouatl", DhBaseGuid)
                .SetGuiPresentationNoContent()
                .SetSpellCastingOrigin(CastingOrigin.Monster)
                .SetSpellCastingAbility(AttributeDefinitions.Charisma)
                .SetSpellKnowledge(RuleDefinitions.SpellKnowledge.FixedList)
                .SetSlotsRecharge(RuleDefinitions.RechargeRate.ShortRest)
                .SetSpellCastingLevel(9)
                .SetSpellReadyness(RuleDefinitions.SpellReadyness.AllKnown)
                .AddToDB()
                // TODO: Move these onto builder
                .SetFocusType(EquipmentDefinitions.FocusType.None)
                .SetSpellListDefinition(couatlSpellList)
                .SetStaticToHitValue(8)
                .SetStaticDCValue(14)
                .SetSpellcastingParametersComputation(RuleDefinitions.SpellcastingParametersComputation.Static);

            int[] castSpellCouatlKnownSpells = { 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20 };

            castSpellCouatl.RestrictedSchools.Clear();
            castSpellCouatl.KnownSpells.Clear();
            castSpellCouatl.KnownSpells.AddRange(castSpellCouatlKnownSpells);
            castSpellCouatl.SlotsPerLevels.AddRange(new List<SlotsByLevelDuplet>
            {
                new () { Slots = new List<int> {13,6,1,1,1,0,0,0,0,0}, Level = 01 },
                new () { Slots = new List<int> {13,6,1,1,1,0,0,0,0,0}, Level = 02 },
                new () { Slots = new List<int> {13,6,1,1,1,0,0,0,0,0}, Level = 03 },
                new () { Slots = new List<int> {13,6,1,1,1,0,0,0,0,0}, Level = 04 },
                new () { Slots = new List<int> {13,6,1,1,1,0,0,0,0,0}, Level = 05 },
                new () { Slots = new List<int> {13,6,1,1,1,0,0,0,0,0}, Level = 06 },
                new () { Slots = new List<int> {13,6,1,1,1,0,0,0,0,0}, Level = 07 },
                new () { Slots = new List<int> {13,6,1,1,1,0,0,0,0,0}, Level = 08 },
                new () { Slots = new List<int> {13,6,1,1,1,0,0,0,0,0}, Level = 09 },
                new () { Slots = new List<int> {13,6,1,1,1,0,0,0,0,0}, Level = 10 },
                new () { Slots = new List<int> {13,6,1,1,1,0,0,0,0,0}, Level = 11 },
                new () { Slots = new List<int> {13,6,1,1,1,0,0,0,0,0}, Level = 12 },
                new () { Slots = new List<int> {13,6,1,1,1,0,0,0,0,0}, Level = 13 },
                new () { Slots = new List<int> {13,6,1,1,1,0,0,0,0,0}, Level = 14 },
                new () { Slots = new List<int> {13,6,1,1,1,0,0,0,0,0}, Level = 15 },
                new () { Slots = new List<int> {13,6,1,1,1,0,0,0,0,0}, Level = 16 },
                new () { Slots = new List<int> {13,6,1,1,1,0,0,0,0,0}, Level = 17 },
                new () { Slots = new List<int> {13,6,1,1,1,0,0,0,0,0}, Level = 18 },
                new () { Slots = new List<int> {13,6,1,1,1,0,0,0,0,0}, Level = 19 },
                new () { Slots = new List<int> {13,6,1,1,1,0,0,0,0,0}, Level = 20 },
            });

            const string CustomCouatlName = "CustomCouatl";
            CharacterSizeDefinition Size = CharacterSizeDefinitions.Medium;
            const string Alignment = "LawfulGood";
            const int ArmorClass = 19;
            const int HitDice = 13;
            const RuleDefinitions.DieType HitDiceType = RuleDefinitions.DieType.D8;
            const int HitPointsBonus = 39;
            const int StandardHitPoints = 97;
            const int AttributeStrength = 16;
            const int AttributeDexterity = 20;
            const int AttributeConstitution = 17;
            const int AttributeIntelligence = 18;
            const int AttributeWisdom = 20;
            const int AttributeCharisma = 18;
            const int SavingThrowStrength = 0;
            const int SavingThrowDexterity = 0;
            const int SavingThrowConstitution = 5;
            const int SavingThrowIntelligence = 0;
            const int SavingThrowWisdom = 7;
            const int SavingThrowCharisma = 6;
            const int CR = 4;
            const bool LegendaryCreature = false;
            const string Type = "Celestial";

            List<MonsterSkillProficiency> SkillScores = new List<MonsterSkillProficiency>();

            /*waiting until MonsterAttackDefinitionBuilder is available to use

                        MonsterAttackDefinition CouatlBite_Attack = MonsterAttackDefinitionBuilder(
                                 "DH_Custom_" + text,
                                 DatabaseHelper.MonsterAttackDefinitions.Attack_TigerDrake_Bite,
                                 GuidHelper.Create(new System.Guid(DhBaseGuid), DhBaseString + "CouatlBite_Attack").ToString(),
                                "MonsterAttack/&DH_CouatlBite_Attack_Title",
                                 "MonsterAttack/&DH_CouatlBite_Attack_Description"
                                  );

                        CouatlBite_Attack.SetToHitBonus(7);
                        CouatlBite_Attack.EffectDescription.SetRangeParameter(1);
                        CouatlBite_Attack.EffectDescription.EffectForms[0].DamageForm.SetDiceNumber(1);
                        CouatlBite_Attack.EffectDescription.EffectForms[0].DamageForm.SetDieType(RuleDefinitions.DieType.D6);
                        CouatlBite_Attack.EffectDescription.EffectForms[0].DamageForm.SetBonusDamage(5);
                        CouatlBite_Attack.EffectDescription.EffectForms[0].DamageForm.SetDamageType(RuleDefinitions.DamageTypePiercing);


                        ConditionForm conditionForm = new ConditionForm();
                        conditionForm.SetConditionDefinition(DatabaseHelper.ConditionDefinitions.ConditionPoisoned);
                        conditionForm.SetConditionDefinitionName(DatabaseHelper.ConditionDefinitions.ConditionPoisoned.name);
                        conditionForm.SetOperation(ConditionForm.ConditionOperation.Add);

                        EffectForm extraPoisonEffect = new EffectForm();
                        extraPoisonEffect.SetApplyLevel(EffectForm.LevelApplianceType.No);
                        extraPoisonEffect.SetLevelMultiplier(1);
                        extraPoisonEffect.SetLevelType(RuleDefinitions.LevelSourceType.ClassLevel);
                        extraPoisonEffect.SetCreatedByCharacter(true);
                        extraPoisonEffect.FormType = EffectForm.EffectFormType.Condition;
                        extraPoisonEffect.SetConditionForm(conditionForm);
                        extraPoisonEffect.SetHasSavingThrow(true);
                        extraPoisonEffect.SetSavingThrowAffinity(RuleDefinitions.EffectSavingThrowType.Negates);

                        ConditionForm sleepForm = new ConditionForm();
                        sleepForm.SetConditionDefinition(DatabaseHelper.ConditionDefinitions.ConditionMagicallyAsleep);
                        sleepForm.SetConditionDefinitionName(DatabaseHelper.ConditionDefinitions.ConditionAsleep.name);
                        sleepForm.SetOperation(ConditionForm.ConditionOperation.Add);

                        EffectForm extraSleepEffect = new EffectForm();
                        extraSleepEffect.SetApplyLevel(EffectForm.LevelApplianceType.No);
                        extraSleepEffect.SetLevelMultiplier(1);
                        extraSleepEffect.SetLevelType(RuleDefinitions.LevelSourceType.ClassLevel);
                        extraSleepEffect.SetCreatedByCharacter(true);
                        extraSleepEffect.FormType = EffectForm.EffectFormType.Condition;
                        extraSleepEffect.SetConditionForm(sleepForm);
                        extraSleepEffect.SetHasSavingThrow(true);
                        extraSleepEffect.SetSavingThrowAffinity(RuleDefinitions.EffectSavingThrowType.Negates);


                        CouatlBite_Attack.EffectDescription.EffectForms.Add(extraSleepEffect);
                        CouatlBite_Attack.EffectDescription.EffectForms.Add(extraPoisonEffect);
                        CouatlBite_Attack.EffectDescription.SetSavingThrowAbility(DatabaseHelper.SmartAttributeDefinitions.Constitution.Name);
                        CouatlBite_Attack.EffectDescription.SetSavingThrowDifficultyAbility(DatabaseHelper.SmartAttributeDefinitions.Constitution.Name);
                        CouatlBite_Attack.EffectDescription.SetHasSavingThrow(true);
                        CouatlBite_Attack.EffectDescription.SetFixedSavingThrowDifficultyClass(13);
                        CouatlBite_Attack.EffectDescription.SetDurationParameter(24);
                        CouatlBite_Attack.EffectDescription.SetDurationType(RuleDefinitions.DurationType.Hour);

                        */

            List<MonsterAttackIteration> AttackIterations = new List<MonsterAttackIteration>
            {
                Tiger_Drake.AttackIterations[0]
                // CouatlBite_Attack
            };

            List<LegendaryActionDescription> LegendaryActionOptions = new List<LegendaryActionDescription>();

            const bool GroupAttacks = false;

            const bool PhantomDistortion = true;
            // AttachedParticlesReference = "0286006526f6f9c4fa61ed8ead4f72cc"
            AssetReference attachedParticlesReference = FeyBear.MonsterPresentation.GetField<AssetReference>("attachedParticlesReference");
            AssetReferenceSprite spriteReference = KindredSpiritViper.GuiPresentation.SpriteReference;

            MonsterDefinitionBuilder
                .Create(KindredSpiritViper, CustomCouatlName, "fc38ed74-3e2f-4960-86cc-3120d638410b")
                .SetGuiPresentation("DH" + CustomCouatlName, Category.Monster, spriteReference)
                .SetInDungeonEditor(false)
                .SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.None)
                .SetSizeDefinition(Size)
                .SetChallengeRating(CR)
                .SetAlignment(Alignment)
                .SetCharacterFamily(Type)
                .SetLegendaryCreature(LegendaryCreature)
                .SetArmorClass(ArmorClass)
                .SetHitDiceNumber(HitDice)
                .SetHitDiceType(HitDiceType)
                .SetHitPointsBonus(HitPointsBonus)
                .SetStandardHitPoints(StandardHitPoints)
                .SetFeatures(
                    FeatureDefinitionMoveModes.MoveModeFly12,
                    FeatureDefinitionSenses.SenseNormalVision,
                    FeatureDefinitionSenses.SenseSuperiorDarkvision,
                    FeatureDefinitionSenses.SenseTruesight16,
                    // wildshape was meant to be a substitute for the couatl's shapechanging but the game didnt like it
                    // (gave the couatl a second position in the intiative order)
                    // DatabaseHelper.FeatureDefinitionPowers.PowerDruidWildShape,
                    FeatureDefinitionAttackModifiers.AttackModifierKindredSpiritMagicSpiritMagicAttack,
                    FeatureDefinitionDamageAffinitys.DamageAffinityRadiantResistance,
                    FeatureDefinitionDamageAffinitys.DamageAffinityPsychicImmunity,
                    FeatureDefinitionDamageAffinitys.DamageAffinityMummyLord_PiercingImmunity,
                    FeatureDefinitionDamageAffinitys.DamageAffinityMummyLord_SlashingImmunity,
                    FeatureDefinitionDamageAffinitys.DamageAffinityMummyLord_BludgeoningImmunity,
                    FeatureDefinitionConditionAffinitys.ConditionAffinityPoisonImmunity,
                    castSpellCouatl
                )
                .SetAbilityScores(
                    AttributeStrength,
                    AttributeDexterity,
                    AttributeConstitution,
                    AttributeIntelligence,
                    AttributeWisdom,
                    AttributeCharisma)
                .SetSavingThrowScores(
                    (AttributeDefinitions.Strength, SavingThrowStrength),
                    (AttributeDefinitions.Dexterity, SavingThrowDexterity),
                    (AttributeDefinitions.Constitution, SavingThrowConstitution),
                    (AttributeDefinitions.Intelligence, SavingThrowIntelligence),
                    (AttributeDefinitions.Wisdom, SavingThrowWisdom),
                    (AttributeDefinitions.Charisma, SavingThrowCharisma))
                .SetSkillScores(SkillScores)
                .SetAttackIterations(AttackIterations)
                //  couatlBuilder.SetDefaultBattleDecisionPackage(DefaultBattleDecisionPackage);
                .SetGroupAttacks(GroupAttacks)
                .SetLegendaryActionOptions(LegendaryActionOptions)
                .SetHasPhantomDistortion(PhantomDistortion)
                .SetAttachedParticlesReference(attachedParticlesReference)
                .SetNoExperienceGain(false)
                .SetHasMonsterPortraitBackground(true)
                .SetCanGeneratePortrait(true)
                .SetCustomShaderReference(KindredSpiritViper.MonsterPresentation.CustomShaderReference)
                .ClearCreatureTags()
                .AddToDB();

            EffectDescriptionBuilder effectDescription = new EffectDescriptionBuilder()
                .SetDurationData(RuleDefinitions.DurationType.Minute, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn)
                .SetTargetingData(RuleDefinitions.Side.All, RuleDefinitions.RangeType.Distance, 18, RuleDefinitions.TargetType.Position, 1, 1, 0)
                .AddEffectForm(new EffectFormBuilder()
                    .SetSummonForm(
                        SummonForm.Type.Creature,
                        ItemDefinitions.Dagger,
                        1,
                        CustomCouatlName,
                        ConditionMindDominatedByCaster,
                        false,
                        DecisionPackageDefinitions.IdleGuard_Default,
                        EffectProxyDefinitions.ProxyDancingLights)
                    .Build())
                .SetCreatedByCharacter()
                .SetParticleEffectParameters(ConjureElementalAir.EffectDescription.EffectParticleParameters);

            return SpellDefinitionBuilder
                .Create("DHConjureCelestialSpell", DhBaseGuid)
                .SetGuiPresentation(Category.Spell, Bless.GuiPresentation.SpriteReference)
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(7)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetVerboseComponent(true)
                .SetSomaticComponent(true)
                .SetEffectDescription(effectDescription.Build())
                .SetAiParameters(new SpellAIParameters())
                .SetRequiresConcentration(true)
                .AddToDB();
        }

        /*
                private static SpellDefinition AnimalShapes()
                {

                    string text = "AnimalShapesSpell";

                    ShapeChangeForm shapeChangeForm = new ShapeChangeForm();
                    shapeChangeForm.SetKeepMentalAbilityScores(true);
                    shapeChangeForm.SetShapeChangeType(ShapeChangeForm.Type.FreeListSelection);
                    shapeChangeForm.SetSpecialSubstituteCondition(DatabaseHelper.ConditionDefinitions.ConditionWildShapeSubstituteForm);
                    shapeChangeForm.ShapeOptions.AddRange(
                       new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(DatabaseHelper.MonsterDefinitions.BadlandsBear),
                        new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(DatabaseHelper.MonsterDefinitions.DeepSpider),
                        new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(DatabaseHelper.MonsterDefinitions.Direwolf),
                        new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(DatabaseHelper.MonsterDefinitions.Giant_Beetle),
                        new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(DatabaseHelper.MonsterDefinitions.Giant_Eagle),
                        new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(DatabaseHelper.MonsterDefinitions.Tiger_Drake),
                        new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(DatabaseHelper.MonsterDefinitions.Ape_MonsterDefinition)

                        // no room on ui, monsters too strong or are weaker versions of above monsters
                        //        new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(DatabaseHelper.MonsterDefinitions.BadlandsSpider),
                        //        new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(DatabaseHelper.MonsterDefinitions.BlackBear),
                        //        new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(DatabaseHelper.MonsterDefinitions.BrownBear),
                        //       new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(DatabaseHelper.MonsterDefinitions.Eagle_Matriarch),
                        //        new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(DatabaseHelper.MonsterDefinitions.Flying_Snake),
                        //        new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(DatabaseHelper.MonsterDefinitions.Giant_Ape),
                        //        new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(DatabaseHelper.MonsterDefinitions.Giant_Crow),
                        //        new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(DatabaseHelper.MonsterDefinitions.Poisonous_Snake),
                        //        new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(DatabaseHelper.MonsterDefinitions.Small_Beetle),
                        //        new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(DatabaseHelper.MonsterDefinitions.Wolf),
                        //        new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(DatabaseHelper.MonsterDefinitions.Ape_Range_MonsterDefinition)
                        );

                    EffectForm effectForm = new EffectForm();
                    effectForm.SetAddBonusMode(RuleDefinitions.AddBonusMode.None);
                    effectForm.SetApplyLevel(EffectForm.LevelApplianceType.No);
                    effectForm.SetCanSaveToCancel(false);
                    effectForm.SetCreatedByCharacter(true);
                    effectForm.SetFormType(EffectForm.EffectFormType.ShapeChange);
                    effectForm.SetShapeChangeForm(shapeChangeForm);

                    EffectDescriptionBuilder effectDescription = new EffectDescriptionBuilder();
                    effectDescription.SetDurationData(
                        RuleDefinitions.DurationType.Hour,
                        24,
                        RuleDefinitions.TurnOccurenceType.EndOfTurn);
                    effectDescription.SetTargetingData(
                        RuleDefinitions.Side.Ally,
                        RuleDefinitions.RangeType.Distance,
                        12,
                        RuleDefinitions.TargetType.Individuals,
                        12,
                        1,
                        ActionDefinitions.ItemSelectionType.None);
                    effectDescription.AddEffectForm(effectForm);
                    effectDescription.SetParticleEffectParameters(DatabaseHelper.FeatureDefinitionPowers.PowerDruidWildShape.EffectDescription.EffectParticleParameters);
                    effectDescription.Build();


                    GuiPresentation guiPresentationSpell = new GuiPresentation();
                    guiPresentationSpell.SetDescription(GetSpellDescriptionTerm(text));
                    guiPresentationSpell.SetTitle(GetSpellTitleTerm(text));
                    guiPresentationSpell.SetSpriteReference(DatabaseHelper.SpellDefinitions.ConjureFey.GuiPresentation.SpriteReference);

                    SpellBuilder AnimalShapesSpell = new SpellBuilder(DhBaseString + text, GuidHelper.Create(new System.Guid(DhBaseGuid), DhBaseString + text).ToString());
                    AnimalShapesSpell.SetGuiPresentation(guiPresentationSpell);
                    AnimalShapesSpell.SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation);
                    AnimalShapesSpell.SetSpellLevel(8);
                    AnimalShapesSpell.SetCastingTime(RuleDefinitions.ActivationTime.Action);
                    AnimalShapesSpell.SetVerboseComponent(true);
                    AnimalShapesSpell.SetSomaticComponent(false);
                    AnimalShapesSpell.SetEffectDescription(effectDescription.Build());
                    AnimalShapesSpell.SetAiParameters(new SpellAIParameters());
                    AnimalShapesSpell.SetConcentration();
                    //  AnimalShapesSpell
                    SpellDefinition AnimalShapes = AnimalShapesSpell.AddToDB();

                    return AnimalShapes;
                }
        */
        private static SpellDefinition BuildDominateMonster()
        {
            EffectDescription effectDescription = new EffectDescription();

            effectDescription.Copy(DominatePerson.EffectDescription);
            effectDescription.RestrictedCreatureFamilies.Clear();
            effectDescription.SetDurationType(RuleDefinitions.DurationType.Hour);

            return SpellDefinitionBuilder
                .Create("DHDominateMonsterSpell", DhBaseGuid)
                .SetGuiPresentation(Category.Spell, DominatePerson.GuiPresentation.SpriteReference)
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(8)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetVerboseComponent(true)
                .SetSomaticComponent(true)
                .SetEffectDescription(effectDescription)
                .SetAiParameters(new SpellAIParameters())
                .AddToDB();
        }

        private static SpellDefinition BuildFeeblemind()
        {
            EffectDescriptionBuilder effectDescription = new EffectDescriptionBuilder()
            .SetDurationData(
                RuleDefinitions.DurationType.Dispelled,
                1,
                RuleDefinitions.TurnOccurenceType.EndOfTurn)
            .SetTargetingData(
                RuleDefinitions.Side.Enemy,
                RuleDefinitions.RangeType.Distance,
                30,
                RuleDefinitions.TargetType.Individuals,
                1,
                1,
                ActionDefinitions.ItemSelectionType.None)
            .SetSavingThrowData(
                true,
                false,
                SmartAttributeDefinitions.Intelligence.name,
                true,
                RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                SmartAttributeDefinitions.Intelligence.name,
                20,
                false,
                new List<SaveAffinityBySenseDescription>()
                )
            .AddEffectForm(new EffectFormBuilder()
                .SetConditionForm(
                    FeeblemindConditionBuilder.FeeblemindCondition,
                    ConditionForm.ConditionOperation.Add,
                    false,
                    false,
                    new List<ConditionDefinition>())
                .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.Negates)
                .Build())
            .AddEffectForm(new EffectFormBuilder()
                .SetDamageForm(
                    false,
                    RuleDefinitions.DieType.D6,
                    RuleDefinitions.DamageTypePsychic,
                    0,
                    RuleDefinitions.DieType.D6,
                    4,
                    RuleDefinitions.HealFromInflictedDamage.Never,
                    new List<RuleDefinitions.TrendInfo>()
                    )
                .Build());

            GreaterRestoration.EffectDescription.EffectForms[0].ConditionForm.ConditionsList.Add(FeeblemindConditionBuilder.FeeblemindCondition);

            return SpellDefinitionBuilder
                .Create("DHFeeblemindSpell", DhBaseGuid)
                .SetGuiPresentation(Category.Spell, BestowCurse.GuiPresentation.SpriteReference)
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(8)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetVerboseComponent(true)
                .SetSomaticComponent(false)
                .SetEffectDescription(effectDescription.Build())
                .SetAiParameters(new SpellAIParameters())
                .AddToDB();
        }

        internal class FeeblemindConditionBuilder : ConditionDefinitionBuilder
        {
            private const string Name = "DHFeeblemindSpellcondition";
            private const string Guid = "965a09b2-cb22-452b-b93c-2bccdcda4871";
            private const string TitleString = "Condition/&DHFeeblemindSpellTitle";
            private const string DescriptionString = "Condition/&DHFeeblemindSpellDescription";

            protected FeeblemindConditionBuilder(string name, string guid) : base(ConditionBearsEndurance, name, guid)
            {
                Definition.GuiPresentation.Title = TitleString;
                Definition.GuiPresentation.Description = DescriptionString;
                Definition.SetConditionType(RuleDefinitions.ConditionType.Detrimental);
                Definition.Features.SetRange(
                    FeatureDefinitionMagicAffinitys.MagicAffinitySilenced,
                    FeatureDefinitionMagicAffinitys.MagicAffinityConditionRaging,
                    FeeblemindIntAttributeModifierBuilder.FeeblemindIntAttributeModifier,
                    FeeblemindChaAttributeModifierBuilder.FeeblemindCha_AttributeModifier,
                    FeeblemindActionAffinityBuilder.FeeblemindActionAffinity);
            }

            internal static ConditionDefinition CreateAndAddToDB(string name, string guid)
            {
                return new FeeblemindConditionBuilder(name, guid).AddToDB();
            }

            internal static readonly ConditionDefinition FeeblemindCondition = CreateAndAddToDB(Name, Guid);
        }

        internal class FeeblemindIntAttributeModifierBuilder : FeatureDefinitionAttributeModifierBuilder
        {
            private const string Name = "DHFeeblemindIntSpellAttributeModifier";
            private const string Guid = "a2a16bda-e7b1-4a87-9f0e-3e4c21870fd8";
            private const string TitleString = "AttributeModifier/&DHFeeblemindIntSpellTitle";
            private const string DescriptionString = "AttributeModifier/&DHFeeblemindIntSpellDescription";

            protected FeeblemindIntAttributeModifierBuilder(string name, string guid) : base(FeatureDefinitionAttributeModifiers.AttributeModifierHeadbandOfIntellect, name, guid)
            {
                Definition.GuiPresentation.Title = TitleString;
                Definition.GuiPresentation.Description = DescriptionString;
                Definition.SetModifiedAttribute(SmartAttributeDefinitions.Intelligence.name);
                Definition.SetModifierAbilityScore(SmartAttributeDefinitions.Intelligence.name);
                Definition.SetModifierType2(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Force);
                Definition.SetModifierValue(1);
                Definition.SetSituationalContext(RuleDefinitions.SituationalContext.None);
            }
            private static FeatureDefinitionAttributeModifier CreateAndAddToDB(string name, string guid)
            {
                return new FeeblemindIntAttributeModifierBuilder(name, guid).AddToDB();
            }

            internal static readonly FeatureDefinitionAttributeModifier FeeblemindIntAttributeModifier = CreateAndAddToDB(Name, Guid);
        }

        internal class FeeblemindChaAttributeModifierBuilder : FeatureDefinitionAttributeModifierBuilder
        {
            private const string Name = "DHFeeblemindChaSpellAttributeModifier";
            private const string Guid = "6721abe1-19eb-4a8c-9702-2fdea2070464";
            private const string TitleString = "AttributeModifier/&DHFeeblemindChaSpellTitle";
            private const string DescriptionString = "AttributeModifier/&DHFeeblemindChaSpellDescription";

            protected FeeblemindChaAttributeModifierBuilder(string name, string guid) : base(FeatureDefinitionAttributeModifiers.AttributeModifierHeadbandOfIntellect, name, guid)
            {
                Definition.GuiPresentation.Title = TitleString;
                Definition.GuiPresentation.Description = DescriptionString;
                Definition.SetModifiedAttribute(SmartAttributeDefinitions.Charisma.name);
                Definition.SetModifierAbilityScore(SmartAttributeDefinitions.Charisma.name);
                Definition.SetModifierType2(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Force);
                Definition.SetModifierValue(1);
                Definition.SetSituationalContext(RuleDefinitions.SituationalContext.None);
            }

            private static FeatureDefinitionAttributeModifier CreateAndAddToDB(string name, string guid)
            {
                return new FeeblemindChaAttributeModifierBuilder(name, guid).AddToDB();
            }

            internal static readonly FeatureDefinitionAttributeModifier FeeblemindCha_AttributeModifier = CreateAndAddToDB(Name, Guid);
        }

        internal class FeeblemindActionAffinityBuilder : FeatureDefinitionActionAffinityBuilder
        {
            private const string Name = "DHFeeblemindSpellActionAffinity";
            private const string Guid = "749a9572-07f6-4678-9458-904c04b9ab22";
            private const string TitleString = "ActionAffinity/&DHFeeblemindSpellTitle";
            private const string DescriptionString = "ActionAffinity/&DHFeeblemindSpellDescription";

            protected FeeblemindActionAffinityBuilder(string name, string guid) : base(FeatureDefinitionActionAffinitys.ActionAffinityConditionRaging, name, guid)
            {
                Definition.GuiPresentation.Title = TitleString;
                Definition.GuiPresentation.Description = DescriptionString;
                Definition.ForbiddenActions.Clear();
                Definition.ForbiddenActions.Empty();
                Definition.ForbiddenActions.AddRange
                (
                    DatabaseHelper.ActionDefinitions.CastBonus.Id,
                    DatabaseHelper.ActionDefinitions.CastMain.Id,
                    DatabaseHelper.ActionDefinitions.CastNoCost.Id,
                    DatabaseHelper.ActionDefinitions.CastReaction.Id,
                    DatabaseHelper.ActionDefinitions.CastReadied.Id,
                    DatabaseHelper.ActionDefinitions.CastRitual.Id
                );
            }

            private static FeatureDefinitionActionAffinity CreateAndAddToDB(string name, string guid)
            {
                return new FeeblemindActionAffinityBuilder(name, guid).AddToDB();
            }

            internal static readonly FeatureDefinitionActionAffinity FeeblemindActionAffinity = CreateAndAddToDB(Name, Guid);
        }

        private static SpellDefinition BuildHolyAura()
        {
            EffectDescriptionBuilder effectDescription = new EffectDescriptionBuilder()
                .SetDurationData(
                    RuleDefinitions.DurationType.Minute,
                    1,
                    RuleDefinitions.TurnOccurenceType.EndOfTurn)
                .SetTargetingData(
                    RuleDefinitions.Side.Ally,
                    RuleDefinitions.RangeType.Self,
                    12,
                    RuleDefinitions.TargetType.Sphere,
                    6,
                    6,
                    ActionDefinitions.ItemSelectionType.None)
                .SetParticleEffectParameters(
                    BeaconOfHope.
                    EffectDescription.EffectParticleParameters)
                .AddEffectForm(new EffectFormBuilder()
                    .SetConditionForm(
                        HolyAuraConditionBuilder.HolyAuraCondition,
                        ConditionForm.ConditionOperation.Add,
                        false,
                        false,
                        new List<ConditionDefinition>())
                    .Build());

            return SpellDefinitionBuilder
                .Create("DHHolyAuraSpell", DhBaseGuid)
                .SetGuiPresentation(Category.Spell, PowerDomainLifePreserveLife.GuiPresentation.SpriteReference)
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(8)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetVerboseComponent(true)
                .SetSomaticComponent(false)
                .SetEffectDescription(effectDescription.Build())
                .SetAiParameters(new SpellAIParameters())
                .SetRequiresConcentration(true)
                .AddToDB();
        }

        internal class HolyAuraConditionBuilder : ConditionDefinitionBuilder
        {
            private const string Name = "DHHolyAuraSpellcondition";
            private const string Guid = "1808ca4b-8f46-41bf-a59c-0bcbd4f60248";
            private const string TitleString = "Condition/&DHHolyAuraSpellTitle";
            private const string DescriptionString = "Condition/&DHHolyAuraSpellDescription";

            protected HolyAuraConditionBuilder(string name, string guid) : base(ConditionBearsEndurance, name, guid)
            {
                Definition.GuiPresentation.Title = TitleString;
                Definition.GuiPresentation.Description = DescriptionString;
                Definition.SetConditionType(RuleDefinitions.ConditionType.Beneficial);
                Definition.Features.SetRange
                (
                    FeatureDefinitionCombatAffinitys.CombatAffinityDodging,
                    FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityShelteringBreeze,
                    HolyAuraDamageAffinityBuilder.HolyAuraDamageAffinity
                );
            }

            private static ConditionDefinition CreateAndAddToDB(string name, string guid)
            {
                return new HolyAuraConditionBuilder(name, guid).AddToDB();
            }

            internal static readonly ConditionDefinition HolyAuraCondition = CreateAndAddToDB(Name, Guid);
        }

        internal class HolyAuraDamageAffinityBuilder : FeatureDefinitionDamageAffinityBuilder
        {
            private const string Name = "DHHolyAuraSpellDamageAffinity";
            private const string Guid = "c83aceae-e4c4-4a9c-a83d-58ffebe92007";
            private const string TitleString = "DamageAffinity/&DHHolyAuraSpellTitle";
            private const string DescriptionString = "DamageAffinity/&DHHolyAuraSpellDescription";

            protected HolyAuraDamageAffinityBuilder(string name, string guid) : base(FeatureDefinitionDamageAffinitys.DamageAffinityPoisonAdvantage, name, guid)
            {
                Definition.GuiPresentation.Title = TitleString;
                Definition.GuiPresentation.Description = DescriptionString;
                Definition.SetDamageAffinityType(RuleDefinitions.DamageAffinityType.None);
                Definition.SetSavingThrowAdvantageType(RuleDefinitions.AdvantageType.None);
                Definition.SetKnockOutAffinity(RuleDefinitions.KnockoutAffinity.None);
                Definition.SetRetaliateWhenHit(true);
                Definition.SetRetaliateProximity(RuleDefinitions.AttackProximity.Melee);
                Definition.SetRetaliateRangeCells(1);
                Definition.SetRetaliatePower(HolyAuraBlindingPowerBuilder.HolyAuraBlindingPower);
            }

            internal static FeatureDefinitionDamageAffinity CreateAndAddToDB(string name, string guid)
            {
                return new HolyAuraDamageAffinityBuilder(name, guid).AddToDB();
            }

            internal static readonly FeatureDefinitionDamageAffinity HolyAuraDamageAffinity = CreateAndAddToDB(Name, Guid);
        }

        internal class HolyAuraBlindingPowerBuilder : FeatureDefinitionPowerBuilder
        {
            private const string Name = "DHHolyAuraSpellBlindingPower";
            private const string Guid = "40366ca2-00a0-471a-b370-8c81f6283ce1";
            private const string TitleString = "Feature/&DHHolyAuraBlindingPowerTitle";
            private const string DescriptionString = "Feature/&DHHolyAuraBlindingPowerDescription";

            protected HolyAuraBlindingPowerBuilder(string name, string guid) : base(PowerOathOfMotherlandFieryPresence, name, guid)
            {
                Definition.GuiPresentation.Title = TitleString;
                Definition.GuiPresentation.Description = DescriptionString;

                EffectDescriptionBuilder effectDescription = new EffectDescriptionBuilder()
                .SetDurationData(
                    RuleDefinitions.DurationType.Minute,
                    1,
                    RuleDefinitions.TurnOccurenceType.EndOfTurn)
                .SetTargetingData(
                    RuleDefinitions.Side.Enemy,
                    RuleDefinitions.RangeType.Distance,
                    12,
                    RuleDefinitions.TargetType.Individuals,
                    1,
                    1,
                    ActionDefinitions.ItemSelectionType.None)
                .AddRestrictedCreatureFamilies(CharacterFamilyDefinitions.Fiend)
                .AddRestrictedCreatureFamilies(CharacterFamilyDefinitions.Undead)
                .AddEffectForm(new EffectFormBuilder()
                    .SetConditionForm(
                        ConditionBlinded,
                        ConditionForm.ConditionOperation.Add,
                        false,
                        false,
                        new List<ConditionDefinition>())
                     .Build());

                Definition.SetEffectDescription(effectDescription.Build());
            }

            private static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
            {
                return new HolyAuraBlindingPowerBuilder(name, guid).AddToDB();
            }

            internal static readonly FeatureDefinitionPower HolyAuraBlindingPower = CreateAndAddToDB(Name, Guid);
        }

        private static SpellDefinition BuildIncendiaryCloud()
        {
            //
            // TODO: Why this effect description isn't used?
            //

            /*            EffectDescriptionBuilder effectDescription = new EffectDescriptionBuilder()
                            .SetDurationData(
                                RuleDefinitions.DurationType.Minute,
                                1,
                                RuleDefinitions.TurnOccurenceType.EndOfTurn)
                            .SetTargetingData(
                                RuleDefinitions.Side.All,
                                RuleDefinitions.RangeType.Distance,
                                30,
                                RuleDefinitions.TargetType.Sphere,
                                4,
                                4,
                                ActionDefinitions.ItemSelectionType.None)
                            .AddEffectForm(new EffectFormBuilder()
                                .SetSummonForm(
                                    SummonForm.Type.EffectProxy,
                                    ItemDefinitions.Dart,
                                    1,
                                    Adam_The_Twelth.name,
                                    null,
                                    false,
                                    DecisionPackageDefinitions.IdleGuard_Default,
                                    EffectProxyDefinitions.ProxyFogCloud)
                                .Build())
                            .AddEffectForm(new EffectFormBuilder()
                                .SetDamageForm(
                                    false,
                                    RuleDefinitions.DieType.D8,
                                    RuleDefinitions.DamageTypeFire,
                                    0,
                                    RuleDefinitions.DieType.D8,
                                    10,
                                    RuleDefinitions.HealFromInflictedDamage.Never,
                                    new List<RuleDefinitions.TrendInfo>())
                                .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.HalfDamage)
                                .Build())
                            .SetRecurrentEffect(WallOfFireLine.EffectDescription.RecurrentEffect)
                            .SetVelocity(2, RuleDefinitions.VelocityType.AwayFromSourceOriginalPosition)
                            .SetSavingThrowData(
                                    true,
                                    false,
                                    SmartAttributeDefinitions.Dexterity.name,
                                    false,
                                    RuleDefinitions.EffectDifficultyClassComputation.FixedValue,
                                    SmartAttributeDefinitions.Dexterity.name,
                                    20,
                                    false,
                                    new List<SaveAffinityBySenseDescription>());

                        effectDescription.Build();
            */
            EffectDescription neweffectDescription = new EffectDescription();

            neweffectDescription.Copy(FogCloud.EffectDescription);

            neweffectDescription.EffectForms.Add(new EffectFormBuilder()
                .SetDamageForm(
                    false,
                    RuleDefinitions.DieType.D8,
                    RuleDefinitions.DamageTypeFire,
                    0,
                    RuleDefinitions.DieType.D8,
                    10,
                    RuleDefinitions.HealFromInflictedDamage.Never,
                    new List<RuleDefinitions.TrendInfo>()
                )
                .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.HalfDamage)
                .Build());

            neweffectDescription.SetHasSavingThrow(true);
            neweffectDescription.SetSavingThrowAbility(SmartAttributeDefinitions.Dexterity.name);

            return SpellDefinitionBuilder
                .Create("DHIncendiaryCloudSpell", DhBaseGuid)
                .SetGuiPresentation(Category.Spell, PowerDomainElementalFireBurst.GuiPresentation.SpriteReference)
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(8)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetVerboseComponent(true)
                .SetSomaticComponent(false)
                .SetEffectDescription(neweffectDescription)
                .SetAiParameters(new SpellAIParameters())
                .SetRequiresConcentration(true)
                .AddToDB();
        }

        private static SpellDefinition BuildMaze()
        {
            EffectDescriptionBuilder effectDescription = new EffectDescriptionBuilder()
            .SetDurationData(
                RuleDefinitions.DurationType.Minute,
                10,
                RuleDefinitions.TurnOccurenceType.EndOfTurn
                )
            .SetTargetingData(
                RuleDefinitions.Side.Enemy,
                RuleDefinitions.RangeType.Distance,
                12,
                RuleDefinitions.TargetType.Individuals,
                1,
                1,
                ActionDefinitions.ItemSelectionType.None
                )
            .SetSavingThrowData(
                true,
                false,
                SmartAttributeDefinitions.Intelligence.name,
                false,
                RuleDefinitions.EffectDifficultyClassComputation.FixedValue,
                SmartAttributeDefinitions.Intelligence.name,
                20,
                false,
                new List<SaveAffinityBySenseDescription>())
            .AddEffectForm(new EffectFormBuilder()
                .SetConditionForm(
                    ConditionBanished,
                    ConditionForm.ConditionOperation.Add,
                    false,
                    false,
                    new List<ConditionDefinition>())
                .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.Negates)
                .CanSaveToCancel(RuleDefinitions.TurnOccurenceType.EndOfTurn)
                .Build());

            return SpellDefinitionBuilder
                .Create("DHMazeSpell", DhBaseGuid)
                .SetGuiPresentation(Category.Spell, IdentifyCreatures.GuiPresentation.SpriteReference)
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(8)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetVerboseComponent(true)
                .SetSomaticComponent(false)
                .SetEffectDescription(effectDescription.Build())
                .SetAiParameters(new SpellAIParameters())
                .SetRequiresConcentration(true)
                .AddToDB();
        }

        private static SpellDefinition BuildMindBlank()
        {
            EffectDescriptionBuilder effectDescription = new EffectDescriptionBuilder();
            effectDescription.SetDurationData(
                RuleDefinitions.DurationType.Hour,
                24,
                RuleDefinitions.TurnOccurenceType.EndOfTurn
                );
            effectDescription.SetTargetingData(
                RuleDefinitions.Side.Ally,
                RuleDefinitions.RangeType.Touch,
                1,
                RuleDefinitions.TargetType.Individuals,
                1,
                1,
                ActionDefinitions.ItemSelectionType.None
                );
            effectDescription.AddEffectForm(
                new EffectFormBuilder().SetConditionForm(
                    MindBlankConditionBuilder.MindBlankCondition,
                    ConditionForm.ConditionOperation.Add,
                    false,
                    false,
                    new List<ConditionDefinition>()
                    )
                .Build()
                );

            return SpellDefinitionBuilder
                .Create("DHMindBlankSpell", DhBaseGuid)
                .SetGuiPresentation(Category.Spell, MindTwist.GuiPresentation.SpriteReference)
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(8)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetVerboseComponent(true)
                .SetSomaticComponent(false)
                .SetEffectDescription(effectDescription.Build())
                .SetAiParameters(new SpellAIParameters())
                .AddToDB();
        }

        internal class MindBlankConditionBuilder : ConditionDefinitionBuilder
        {
            private const string Name = "DHMindBlankSpellcondition";
            private const string Guid = "74f77a4c-b5cb-45d6-ac6d-d9fa2ebe3869";
            private const string TitleString = "Condition/&DHMindBlankSpellTitle";
            private const string DescriptionString = "Condition/&DHMindBlankSpellDescription";

            protected MindBlankConditionBuilder(string name, string guid) : base(ConditionBearsEndurance, name, guid)
            {
                Definition.GuiPresentation.Title = TitleString;
                Definition.GuiPresentation.Description = DescriptionString;
                Definition.Features.SetRange(
                    FeatureDefinitionConditionAffinitys.ConditionAffinityCharmImmunity,
                    FeatureDefinitionConditionAffinitys.ConditionAffinityCharmImmunityHypnoticPattern,
                    FeatureDefinitionConditionAffinitys.ConditionAffinityCalmEmotionCharmedImmunity,
                    FeatureDefinitionDamageAffinitys.DamageAffinityPsychicImmunity);
            }
            private static ConditionDefinition CreateAndAddToDB(string name, string guid)
            {
                return new MindBlankConditionBuilder(name, guid).AddToDB();
            }

            internal static readonly ConditionDefinition MindBlankCondition = CreateAndAddToDB(Name, Guid);
        }

        private static SpellDefinition BuildPowerWordStun()
        {
            ConditionForm conditionForm = new ConditionForm()
                .SetApplyToSelf(false)
                .SetForceOnSelf(false)
                .SetOperation(ConditionForm.ConditionOperation.Add)
                .SetConditionDefinitionName(ConditionStunned.Name)
                .SetConditionDefinition(ConditionStunned);

            EffectForm effectForm = new EffectForm()
                .SetApplyLevel(EffectForm.LevelApplianceType.No)
                .SetLevelMultiplier(1)
                .SetLevelType(RuleDefinitions.LevelSourceType.ClassLevel)
                .SetCreatedByCharacter(true)
                .SetFormType(EffectForm.EffectFormType.Condition)
                .SetConditionForm(conditionForm)
                .SetCanSaveToCancel(true)
                .SetSaveOccurence(RuleDefinitions.TurnOccurenceType.EndOfTurn);

            EffectDescriptionBuilder effectDescription = new EffectDescriptionBuilder()
                .SetDurationData(
                    RuleDefinitions.DurationType.Dispelled,
                    1,
                    RuleDefinitions.TurnOccurenceType.EndOfTurn)
                .SetTargetingData(
                    RuleDefinitions.Side.Enemy,
                    RuleDefinitions.RangeType.Distance,
                    12,
                    RuleDefinitions.TargetType.Individuals,
                    1,
                    1,
                    ActionDefinitions.ItemSelectionType.None)
                .AddEffectForm(effectForm)
                .SetTargetFiltering(
                    RuleDefinitions.TargetFilteringMethod.CharacterIncreasingHitPointsFromPool,
                    RuleDefinitions.TargetFilteringTag.No,
                    150,
                    RuleDefinitions.DieType.D1)
                .SetSavingThrowData(
                    true,
                    false,
                    SmartAttributeDefinitions.Constitution.name,
                    false,
                    RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                    SmartAttributeDefinitions.Constitution.name,
                    15,
                    false,
                    new List<SaveAffinityBySenseDescription>());

            return SpellDefinitionBuilder
                .Create("DHPowerWordStunSpell", DhBaseGuid)
                .SetGuiPresentation(Category.Spell, Contagion.GuiPresentation.SpriteReference)
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(8)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetVerboseComponent(true)
                .SetSomaticComponent(false)
                .SetEffectDescription(effectDescription.Build())
                .SetAiParameters(new SpellAIParameters())
                .AddToDB();
        }

        private static SpellDefinition BuildSunBurst()
        {
            EffectDescriptionBuilder effectDescription = new EffectDescriptionBuilder()
                .SetDurationData(
                    RuleDefinitions.DurationType.Minute,
                    1,
                    RuleDefinitions.TurnOccurenceType.EndOfTurn)
                .SetTargetingData(
                    RuleDefinitions.Side.All,
                    RuleDefinitions.RangeType.Distance,
                    30,
                    RuleDefinitions.TargetType.Sphere,
                    12,
                    1,
                    ActionDefinitions.ItemSelectionType.None)
                .SetSavingThrowData(
                    true,
                    false,
                    SmartAttributeDefinitions.Constitution.name,
                    false,
                    RuleDefinitions.EffectDifficultyClassComputation.FixedValue,
                    SmartAttributeDefinitions.Constitution.name,
                    20,
                    false,
                    new List<SaveAffinityBySenseDescription>())
                .AddEffectForm(new EffectFormBuilder()
                    .SetDamageForm(
                        false,
                        RuleDefinitions.DieType.D6,
                        RuleDefinitions.DamageTypeRadiant,
                        0,
                        RuleDefinitions.DieType.D6,
                        12,
                        RuleDefinitions.HealFromInflictedDamage.Never,
                        new List<RuleDefinitions.TrendInfo>())
                    .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.HalfDamage)
                    .Build())
                .AddEffectForm(new EffectFormBuilder()
                    .SetConditionForm(
                        ConditionBlinded,
                        ConditionForm.ConditionOperation.Add,
                        false,
                        false,
                        new List<ConditionDefinition>())
                    .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.Negates)
                    .CanSaveToCancel(RuleDefinitions.TurnOccurenceType.EndOfTurn)
                    .Build());

            EffectDescription effect = effectDescription.Build();

            effect.EffectParticleParameters.SetImpactParticleReference(new AssetReference("96018e15e8eba4b40a9a5bd637d7ae91"));

            SaveAffinityByFamilyDescription SaveAffinityByFamilyDescription = new SaveAffinityByFamilyDescription()
                .SetAdvantageType(RuleDefinitions.AdvantageType.Disadvantage)
                .SetFamily(CharacterFamilyDefinitions.Ooze.name);

            SaveAffinityByFamilyDescription SaveAffinityByFamilyDescriptionUndead = new SaveAffinityByFamilyDescription()
                .SetAdvantageType(RuleDefinitions.AdvantageType.Disadvantage)
                .SetFamily(CharacterFamilyDefinitions.Undead.name);

            effect.SavingThrowAffinitiesByFamily.AddRange(SaveAffinityByFamilyDescription, SaveAffinityByFamilyDescriptionUndead);

            return SpellDefinitionBuilder
                .Create("DHSunBurstSpell", DhBaseGuid)
                .SetGuiPresentation(Category.Spell, PowerDomainSunIndomitableLight.GuiPresentation.SpriteReference)
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(8)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetVerboseComponent(true)
                .SetSomaticComponent(false)
                .SetEffectDescription(effect)
                .SetAiParameters(new SpellAIParameters())
                .AddToDB();
        }

        private static SpellDefinition BuildForesight()
        {
            EffectDescriptionBuilder effectDescription = new EffectDescriptionBuilder()
                .SetDurationData(
                    RuleDefinitions.DurationType.Hour,
                    8,
                    RuleDefinitions.TurnOccurenceType.EndOfTurn)
               .SetTargetingData(
                    RuleDefinitions.Side.Ally,
                    RuleDefinitions.RangeType.Touch,
                    1,
                    RuleDefinitions.TargetType.Individuals,
                    1,
                    1,
                    ActionDefinitions.ItemSelectionType.None)
                .AddEffectForm(new EffectFormBuilder()
                    .SetConditionForm(
                        ForesightConditionBuilder.ForesightCondition,
                        ConditionForm.ConditionOperation.Add,
                        false,
                        false,
                        new List<ConditionDefinition>())
                    .Build());

            return SpellDefinitionBuilder
                .Create("DHForesightSpell", DhBaseGuid)
                .SetGuiPresentation(Category.Spell, TrueSeeing.GuiPresentation.SpriteReference)
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(9)
                .SetCastingTime(RuleDefinitions.ActivationTime.Minute1)
                .SetVerboseComponent(true)
                .SetSomaticComponent(false)
                .SetEffectDescription(effectDescription.Build())
                .SetAiParameters(new SpellAIParameters())
                .AddToDB();
        }

        internal class ForesightConditionBuilder : ConditionDefinitionBuilder
        {
            private const string Name = "DHForesightSpellcondition";
            private const string Guid = "4615c639-95f2-4c04-b904-e79f5b916b68";
            private const string TitleString = "Condition/&DHForesightSpellTitle";
            private const string DescriptionString = "Condition/&DHForesightSpellDescription";

            protected ForesightConditionBuilder(string name, string guid) : base(ConditionBearsEndurance, name, guid)
            {
                Definition.GuiPresentation.Title = TitleString;
                Definition.GuiPresentation.Description = DescriptionString;
                Definition.Features.SetRange
                (
                    FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionBearsEndurance,
                    FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionBullsStrength,
                    FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionCatsGrace,
                    FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionEaglesSplendor,
                    FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionFoxsCunning,
                    FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionOwlsWisdom,
                    FeatureDefinitionCombatAffinitys.CombatAffinityStealthy,
                    FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityShelteringBreeze
                );
            }

            private static ConditionDefinition CreateAndAddToDB(string name, string guid)
            {
                return new ForesightConditionBuilder(name, guid).AddToDB();
            }

            internal static readonly ConditionDefinition ForesightCondition = CreateAndAddToDB(Name, Guid);
        }

        private static SpellDefinition BuildMassHeal()
        {
            EffectDescriptionBuilder effectDescription = new EffectDescriptionBuilder()
                .SetDurationData(
                    RuleDefinitions.DurationType.Instantaneous,
                    1,
                    RuleDefinitions.TurnOccurenceType.EndOfTurn)
                .SetTargetingData(
                    RuleDefinitions.Side.All,
                    RuleDefinitions.RangeType.Distance,
                    12,
                    RuleDefinitions.TargetType.Individuals,
                    14,
                    1,
                    ActionDefinitions.ItemSelectionType.None)
                .AddEffectForm(new EffectFormBuilder()
                    .SetHealingForm(
                        RuleDefinitions.HealingComputation.Dice,
                        50,
                        RuleDefinitions.DieType.D1,
                        0,
                        false,
                        RuleDefinitions.HealingCap.MaximumHitPoints)
                    .Build());

            return SpellDefinitionBuilder
                .Create("DHMassHealSpell", DhBaseGuid)
                .SetGuiPresentation(Category.Spell, Heal.GuiPresentation.SpriteReference)
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(9)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetVerboseComponent(true)
                .SetSomaticComponent(false)
                .SetEffectDescription(effectDescription.Build())
                .SetAiParameters(new SpellAIParameters())
                .AddToDB();
        }

        private static SpellDefinition BuildMeteorSwarmSingleTarget()
        {
            EffectDescriptionBuilder effectDescription = new EffectDescriptionBuilder()
                .SetDurationData(
                    RuleDefinitions.DurationType.Instantaneous,
                    1,
                    RuleDefinitions.TurnOccurenceType.EndOfTurn)
                .SetTargetingData(
                    RuleDefinitions.Side.All,
                    RuleDefinitions.RangeType.Distance,
                    200,
                    RuleDefinitions.TargetType.Sphere,
                    8,
                    8,
                    ActionDefinitions.ItemSelectionType.None)
                .AddEffectForm(
                     new EffectFormBuilder().SetDamageForm(
                         false,
                         RuleDefinitions.DieType.D6,
                         RuleDefinitions.DamageTypeFire,
                         0,
                         RuleDefinitions.DieType.D6,
                         20,                                          // 20 because hits dont stack even on single target
                         RuleDefinitions.HealFromInflictedDamage.Never,
                         new List<RuleDefinitions.TrendInfo>())
                     .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.HalfDamage)
                     .Build())
                .AddEffectForm(new EffectFormBuilder()
                    .SetDamageForm(
                        false,
                        RuleDefinitions.DieType.D6,
                        RuleDefinitions.DamageTypeBludgeoning,
                        0,
                        RuleDefinitions.DieType.D6,
                        20,                                          // 20 because hits dont stack even on single target
                        RuleDefinitions.HealFromInflictedDamage.Never,
                        new List<RuleDefinitions.TrendInfo>())
                    .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.HalfDamage)
                    .Build())
                .SetSavingThrowData(
                    true,
                    false,
                    SmartAttributeDefinitions.Dexterity.name,
                    true,
                    RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                    SmartAttributeDefinitions.Dexterity.name,
                    20,
                    false,
                    new List<SaveAffinityBySenseDescription>())
                .SetParticleEffectParameters(FlameStrike.EffectDescription.EffectParticleParameters);

            return SpellDefinitionBuilder
                .Create("DHMeteorSwarmSingleTargetSpell", DhBaseGuid)
                .SetGuiPresentation(Category.Spell, FlamingSphere.GuiPresentation.SpriteReference)
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(9)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetVerboseComponent(true)
                .SetSomaticComponent(false)
                .SetEffectDescription(effectDescription.Build())
                .SetAiParameters(new SpellAIParameters())
                .AddToDB();
        }

        private static SpellDefinition BuildPowerWordHeal()
        {
            EffectDescriptionBuilder effectDescription = new EffectDescriptionBuilder()
            .SetDurationData(
                RuleDefinitions.DurationType.Instantaneous,
                1,
                RuleDefinitions.TurnOccurenceType.EndOfTurn)
            .SetTargetingData(
                RuleDefinitions.Side.Ally,
                RuleDefinitions.RangeType.Distance,
                12,
                RuleDefinitions.TargetType.Individuals,
                1,
                1,
                ActionDefinitions.ItemSelectionType.None)
            .AddEffectForm(new EffectFormBuilder().SetHealingForm(
                RuleDefinitions.HealingComputation.Dice,
                700,
                RuleDefinitions.DieType.D1,
                0,
                false,
                RuleDefinitions.HealingCap.MaximumHitPoints)
                .Build())
            .AddEffectForm(new EffectFormBuilder()
                .SetConditionForm(
                    ConditionParalyzed,
                    ConditionForm.ConditionOperation.RemoveDetrimentalAll,
                    false,
                    false,
                    new List<ConditionDefinition>
                    {
                        ConditionCharmed,
                        ConditionCharmedByHypnoticPattern,
                        ConditionFrightened,
                        ConditionFrightenedFear,
                        ConditionFrightenedPhantasmalKiller,
                        ConditionParalyzed,
                        ConditionParalyzed_CrimsonSpiderVenom,
                        ConditionParalyzed_GhoulsCaress,
                        ConditionStunned,
                        ConditionStunned_MutantApeSlam,
                        ConditionStunnedConjuredDeath,
                        ConditionProne
                    })
                .Build());

            return SpellDefinitionBuilder
                .Create("DHPowerWordHealSpell", DhBaseGuid)
                .SetGuiPresentation(Category.Spell, HealingWord.GuiPresentation.SpriteReference)
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(9)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetVerboseComponent(true)
                .SetSomaticComponent(false)
                .SetEffectDescription(effectDescription.Build())
                .SetAiParameters(new SpellAIParameters())
                .AddToDB();
        }

        private static SpellDefinition BuildPowerWordKill()
        {
            KillForm killForm = new KillForm()
                .SetKillCondition(RuleDefinitions.KillCondition.UnderHitPoints)
                .SetHitPoints(100);

            EffectForm effectForm = new EffectForm()
                .SetApplyLevel(EffectForm.LevelApplianceType.No)
                .SetLevelMultiplier(1)
                .SetLevelType(RuleDefinitions.LevelSourceType.ClassLevel)
                .SetCreatedByCharacter(true)
                .SetFormType(EffectForm.EffectFormType.Kill)
                .SetKillForm(killForm);

            EffectDescriptionBuilder effectDescription = new EffectDescriptionBuilder()
            .SetDurationData(
                RuleDefinitions.DurationType.Instantaneous,
                1,
                RuleDefinitions.TurnOccurenceType.EndOfTurn)
            .SetTargetingData(
                RuleDefinitions.Side.Enemy,
                RuleDefinitions.RangeType.Distance,
                12,
                RuleDefinitions.TargetType.Individuals,
                1,
                1,
                ActionDefinitions.ItemSelectionType.None)
            .AddEffectForm(effectForm);

            return SpellDefinitionBuilder
                .Create("DHPowerWordKillSpell", DhBaseGuid)
                .SetGuiPresentation(Category.Spell, Disintegrate.GuiPresentation.SpriteReference)
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(9)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetVerboseComponent(true)
                .SetSomaticComponent(false)
                .SetEffectDescription(effectDescription.Build())
                .SetAiParameters(new SpellAIParameters())
                .AddToDB();
        }

        private static SpellDefinition BuildShapechange()
        {
            ShapeChangeForm shapeChangeForm = new ShapeChangeForm()
                .SetKeepMentalAbilityScores(true)
                .SetShapeChangeType(ShapeChangeForm.Type.FreeListSelection)
                .SetSpecialSubstituteCondition(ConditionWildShapeSubstituteForm);

            shapeChangeForm.ShapeOptions.AddRange
            (
                new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(GoldDragon_AerElai),
                new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(Divine_Avatar),
                new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(Sorr_Akkath_Tshar_Boss),
                new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(GreenDragon_MasterOfConjuration),
                new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(BlackDragon_MasterOfNecromancy),
                new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(Remorhaz),
                new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(Emperor_Laethar),
                new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(Giant_Ape),
                new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(Spider_Queen),
                new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(Sorr_Akkath_Shikkath)
            );

            EffectForm effectForm = new EffectForm()
                .SetAddBonusMode(RuleDefinitions.AddBonusMode.None)
                .SetApplyLevel(EffectForm.LevelApplianceType.No)
                .SetCanSaveToCancel(false)
                .SetCreatedByCharacter(true)
                .SetFormType(EffectForm.EffectFormType.ShapeChange)
                .SetShapeChangeForm(shapeChangeForm);

            EffectDescriptionBuilder effectDescription = new EffectDescriptionBuilder()
                .SetDurationData(
                    RuleDefinitions.DurationType.Hour,
                    1,
                    RuleDefinitions.TurnOccurenceType.EndOfTurn)
                .SetTargetingData(
                    RuleDefinitions.Side.Ally,
                    RuleDefinitions.RangeType.Distance,
                    12,
                    RuleDefinitions.TargetType.Self,
                    1,
                    1,
                    ActionDefinitions.ItemSelectionType.None)
                .AddEffectForm(effectForm)
                .SetCreatedByCharacter()
                .SetParticleEffectParameters(PowerDruidWildShape.EffectDescription.EffectParticleParameters);

            return SpellDefinitionBuilder
                .Create("DHShapechangeSpell", DhBaseGuid)
                .SetGuiPresentation(Category.Spell, PowerDruidWildShape.GuiPresentation.SpriteReference)
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(9)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetVerboseComponent(true)
                .SetSomaticComponent(false)
                .SetEffectDescription(effectDescription.Build())
                .SetAiParameters(new SpellAIParameters())
                .SetRequiresConcentration(true)
                .AddToDB();
        }

        private static SpellDefinition BuildTimeStop()
        {
            EffectDescriptionBuilder effectDescription = new EffectDescriptionBuilder()
            .SetDurationData(
                RuleDefinitions.DurationType.Round,
                3,
                RuleDefinitions.TurnOccurenceType.EndOfTurn)
            .SetTargetingData(
                RuleDefinitions.Side.All,
                RuleDefinitions.RangeType.Self,
                0,
                RuleDefinitions.TargetType.Cylinder,
                20,
                10,
                ActionDefinitions.ItemSelectionType.None)
            .AddEffectForm(new EffectFormBuilder()
                .SetConditionForm(
                    TimeStopConditionBuilder.TimeStopCondition,
                    ConditionForm.ConditionOperation.Add,
                    false,
                    false,
                    new List<ConditionDefinition>()).Build())
            .ExcludeCaster();

            return SpellDefinitionBuilder
                .Create("DHTimeStopSpell", DhBaseGuid)
                .SetGuiPresentation(Category.Spell, PowerDomainLawWordOfLaw.GuiPresentation.SpriteReference)
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(9)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetVerboseComponent(true)
                .SetSomaticComponent(false)
                .SetEffectDescription(effectDescription.Build())
                .SetAiParameters(new SpellAIParameters())
                .AddToDB();
        }

        internal class TimeStopConditionBuilder : ConditionDefinitionBuilder
        {
            private const string Name = "DHTimeStopSpellCondition";
            private const string Guid = "f00e592f-61c3-4cbf-a800-97596e83028d";
            private const string TitleString = "Condition/&DHTimeStopSpellTitle";
            private const string DescriptionString = "Condition/&DHTimeStopSpellDescription";

            protected TimeStopConditionBuilder(string name, string guid) : base(ConditionIncapacitated, name, guid)
            {
                Definition.GuiPresentation.Title = TitleString;
                Definition.GuiPresentation.Description = DescriptionString;
                Definition.HasSpecialInterruptionOfType(RuleDefinitions.ConditionInterruption.Attacked);
                Definition.HasSpecialInterruptionOfType(RuleDefinitions.ConditionInterruption.Damaged);
                Definition.SpecialInterruptions.Add(RuleDefinitions.ConditionInterruption.Attacked);
                Definition.SpecialInterruptions.Add(RuleDefinitions.ConditionInterruption.Damaged);
                Definition.SetInterruptionDamageThreshold(1);
                Definition.SetInterruptionRequiresSavingThrow(false);
            }

            private static ConditionDefinition CreateAndAddToDB(string name, string guid)
            {
                return new TimeStopConditionBuilder(name, guid).AddToDB();
            }

            internal static readonly ConditionDefinition TimeStopCondition = CreateAndAddToDB(Name, Guid);
        }

        private static SpellDefinition BuildWeird()
        {
            EffectDescriptionBuilder effectDescription = new EffectDescriptionBuilder()
                .SetDurationData(
                    RuleDefinitions.DurationType.Minute,
                    1,
                    RuleDefinitions.TurnOccurenceType.EndOfTurn)
                .SetTargetingData(
                    RuleDefinitions.Side.Enemy,
                    RuleDefinitions.RangeType.Distance,
                    12,
                    RuleDefinitions.TargetType.Sphere,
                    6,
                    6,
                    ActionDefinitions.ItemSelectionType.None)
                .SetSavingThrowData(
                    true,
                    false,
                    SmartAttributeDefinitions.Wisdom.name,
                    true,
                    RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                    SmartAttributeDefinitions.Constitution.name,
                    20,
                    false,
                    new List<SaveAffinityBySenseDescription>())
                .AddEffectForm(new EffectFormBuilder()
                    .SetConditionForm(
                        WeirdConditionBuilder.WeirdCondition,
                        ConditionForm.ConditionOperation.Add,
                        false,
                        false,
                        new List<ConditionDefinition>())
                    .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.Negates)
                    .CanSaveToCancel(RuleDefinitions.TurnOccurenceType.EndOfTurn)
                    .Build());

            return SpellDefinitionBuilder
                .Create("DHWeirdSpell", DhBaseGuid)
                .SetGuiPresentation(Category.Spell, PhantasmalKiller.GuiPresentation.SpriteReference)
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(9)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetVerboseComponent(true)
                .SetSomaticComponent(false)
                .SetEffectDescription(effectDescription.Build())
                .SetAiParameters(new SpellAIParameters())
                .SetRequiresConcentration(true)
                .AddToDB();
        }

        internal class WeirdConditionBuilder : ConditionDefinitionBuilder
        {
            private const string Name = "DHWeirdSpellCondition";
            private const string Guid = "0f76e7e1-4490-4ee8-a13f-a4a967ba1c08";
            private const string TitleString = "Condition/&DHWeirdSpellTitle";
            private const string DescriptionString = "Condition/&DHWeirdSpellDescription";

            protected WeirdConditionBuilder(string name, string guid) : base(ConditionFrightenedPhantasmalKiller, name, guid)
            {
                Definition.GuiPresentation.Title = TitleString;
                Definition.GuiPresentation.Description = DescriptionString;

                // weird condition is the same as phantasma killer condition, just for more people
            }
            private static ConditionDefinition CreateAndAddToDB(string name, string guid)
            {
                return new WeirdConditionBuilder(name, guid).AddToDB();
            }

            internal static readonly ConditionDefinition WeirdCondition = CreateAndAddToDB(Name, Guid);
        }

        /*
                private static SpellDefinition Wish_overall()
                {

                    string text = "WishOverallSpell";

                    EffectDescriptionBuilder effectDescription = new EffectDescriptionBuilder();
                    effectDescription.SetDurationData(
                        RuleDefinitions.DurationType.Instantaneous,
                        1,
                        RuleDefinitions.TurnOccurenceType.EndOfTurn);
                    effectDescription.SetTargetingData(
                        RuleDefinitions.Side.Enemy,
                        RuleDefinitions.RangeType.Distance,
                        12,
                        RuleDefinitions.TargetType.Individuals,
                        1,
                        1,
                        ActionDefinitions.ItemSelectionType.None);
                    effectDescription.Build();

                    GuiPresentation guiPresentationSpell = new GuiPresentation();
                    guiPresentationSpell.SetDescription(GetSpellDescriptionTerm(text));
                    guiPresentationSpell.SetTitle(GetSpellTitleTerm(text));
                    guiPresentationSpell.SetSpriteReference(DatabaseHelper.SpellDefinitions.Banishment.GuiPresentation.SpriteReference);

                    SpellBuilder Wish_overallSpell = new SpellBuilder(DhBaseString + text, GuidHelper.Create(new System.Guid(DhBaseGuid), DhBaseString + text).ToString());
                    Wish_overallSpell.SetGuiPresentation(guiPresentationSpell);
                    Wish_overallSpell.SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation);
                    Wish_overallSpell.SetSpellLevel(9);
                    Wish_overallSpell.SetCastingTime(RuleDefinitions.ActivationTime.Action);
                    Wish_overallSpell.SetVerboseComponent(true);
                    Wish_overallSpell.SetSomaticComponent(false);
                    Wish_overallSpell.SetEffectDescription(effectDescription.Build());
                    Wish_overallSpell.SetAiParameters(new SpellAIParameters());
                    //    Wish_overallSpell
                    Wish_overallSpell.SetSubSpells(
                        new List<SpellDefinition>
                        {

                        });
                   SpellDefinition Wish_overall = Wish_overallSpell.AddToDB();

                    return Wish_overall;
                }
        */
    }
}
