using System.Collections.Generic;
using SolastaCommunityExpansion.CustomFeatureDefinitions;
using SolastaCommunityExpansion.Models;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.BuilderHelpers;
using SolastaModApi.Infrastructure;
using SolastaCommunityExpansion.Features;
using UnityEngine.AddressableAssets;

namespace SolastaCommunityExpansion.Spells
{
    internal class SrdSpells
    {
        internal static void Load()
        {
            // 7th level
            SpellsContext.RegisterSpell(BuildDivineWord(), isFromOtherMod: false, SpellsContext.NOT_IN_MIN_SET, "SpellListCleric");
            SpellsContext.RegisterSpell(BuildFingerOfDeath(), isFromOtherMod: false, SpellsContext.NOT_IN_MIN_SET, "SpellListWizard", "SpellListSorcerer");
            SpellsContext.RegisterSpell(BuildReverseGravity(), isFromOtherMod: false, SpellsContext.NOT_IN_MIN_SET, "SpellListDruid", "SpellListWizard", "SpellListSorcerer");
            SpellsContext.RegisterSpell(BuildConjureCelestial(), isFromOtherMod: false, SpellsContext.NOT_IN_MIN_SET, "SpellListCleric");

            // 8th level
            SpellsContext.RegisterSpell(BuildDominateMonster(), isFromOtherMod: false, SpellsContext.NOT_IN_MIN_SET, "SpellListWizard", "SpellListSorcerer", "WitchSpellList");
            SpellsContext.RegisterSpell(BuildFeeblemind(), isFromOtherMod: false, SpellsContext.NOT_IN_MIN_SET, "SpellListWizard", "SpellListDruid", "WitchSpellList");
            SpellsContext.RegisterSpell(BuildHolyAura(), isFromOtherMod: false, SpellsContext.NOT_IN_MIN_SET, "SpellListCleric");
            SpellsContext.RegisterSpell(BuildIncendiaryCloud(), isFromOtherMod: false, SpellsContext.NOT_IN_MIN_SET, "SpellListWizard", "SpellListSorcerer");
            SpellsContext.RegisterSpell(BuildMaze(), isFromOtherMod: false, SpellsContext.NOT_IN_MIN_SET, "SpellListWizard");
            SpellsContext.RegisterSpell(BuildMindBlank(), isFromOtherMod: false, SpellsContext.NOT_IN_MIN_SET, "SpellListWizard", "WitchSpellList");
            SpellsContext.RegisterSpell(BuildPowerWordStun(), isFromOtherMod: false, SpellsContext.NOT_IN_MIN_SET, "SpellListWizard", "SpellListSorcerer", "WitchSpellList");
            SpellsContext.RegisterSpell(BuildSunBurst(), isFromOtherMod: false, SpellsContext.NOT_IN_MIN_SET, "SpellListDruid", "SpellListWizard", "SpellListSorcerer");

            // 9th level
            SpellsContext.RegisterSpell(BuildForesight(), isFromOtherMod: false, SpellsContext.NOT_IN_MIN_SET, "SpellListDruid", "SpellListWizard", "WitchSpellList");
            SpellsContext.RegisterSpell(BuildMassHeal(), isFromOtherMod: false, SpellsContext.NOT_IN_MIN_SET, "SpellListCleric");
            SpellsContext.RegisterSpell(BuildMeteorSwarmSingleTarget(), isFromOtherMod: false, SpellsContext.NOT_IN_MIN_SET, "SpellListWizard", "SpellListSorcerer");
            SpellsContext.RegisterSpell(BuildPowerWordHeal(), isFromOtherMod: false, SpellsContext.NOT_IN_MIN_SET, "SpellListCleric");
            SpellsContext.RegisterSpell(BuildPowerWordKill(), isFromOtherMod: false, SpellsContext.NOT_IN_MIN_SET, "SpellListWizard", "SpellListSorcerer");
            SpellsContext.RegisterSpell(BuildTimeStop(), isFromOtherMod: false, SpellsContext.NOT_IN_MIN_SET, "SpellListWizard", "SpellListSorcerer");
            SpellsContext.RegisterSpell(BuildShapechange(), isFromOtherMod: false, SpellsContext.NOT_IN_MIN_SET, "SpellListDruid", "SpellListWizard");
            SpellsContext.RegisterSpell(BuildWeird(), isFromOtherMod: false, SpellsContext.NOT_IN_MIN_SET, "SpellListWizard", "WitchSpellList");
        }

        //
        // ChrisJohnDigital Spells
        //

        private static SpellDefinition BuildDivineWord()
        {
            SpellBuilder spellBuilder = new SpellBuilder("CJDivineWord","18ecba41-a8ac-4048-979e-2139e66934a7")

            .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolEvocation)
            .SetMaterialComponent(RuleDefinitions.MaterialComponentType.None)
            .SetCastingTime(RuleDefinitions.ActivationTime.BonusAction)
            .SetSomaticComponent(false)
            .SetVerboseComponent(true)
            .SetSpellLevel(7)
            .SetGuiPresentation(new GuiPresentationBuilder("Spell/&CJDivineWordDescription", "Spell/&CJDivineWordTitle")
                .Build()
                .SetSpriteReference(DatabaseHelper.SpellDefinitions.DivineFavor.GuiPresentation.SpriteReference))
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetTargetingData(
                    RuleDefinitions.Side.All, 
                    RuleDefinitions.RangeType.Distance, 
                    6, 
                    RuleDefinitions.TargetType.IndividualsUnique, 
                    20, 
                    1, 
                    ActionDefinitions.ItemSelectionType.None)
                .SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.MassHealingWord.EffectDescription.EffectParticleParameters)
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
                .Build());

            return spellBuilder.AddToDB();
        }

        private sealed class DivineWordEffectForm : CustomEffectForm
        {
            private readonly List<string> monsterFamilyPlaneshiftList = new List<string>
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
                    ApplyCondition(formsParams, DatabaseHelper.ConditionDefinitions.ConditionBanished, RuleDefinitions.DurationType.Day, 1);

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
                    ApplyCondition(formsParams, DatabaseHelper.ConditionDefinitions.ConditionDeafened, RuleDefinitions.DurationType.Hour, 1);
                    ApplyCondition(formsParams, DatabaseHelper.ConditionDefinitions.ConditionBlinded, RuleDefinitions.DurationType.Hour, 1);
                    ApplyCondition(formsParams, DatabaseHelper.ConditionDefinitions.ConditionStunned, RuleDefinitions.DurationType.Hour, 1);
                }
                else if (curHP <= 40)
                {
                    // deafened, blinded 10 minutes
                    ApplyCondition(formsParams, DatabaseHelper.ConditionDefinitions.ConditionDeafened, RuleDefinitions.DurationType.Minute, 10);
                    ApplyCondition(formsParams, DatabaseHelper.ConditionDefinitions.ConditionBlinded, RuleDefinitions.DurationType.Minute, 10);
                }
                else if (curHP <= 50)
                {
                    // deafened 1 minute
                    ApplyCondition(formsParams, DatabaseHelper.ConditionDefinitions.ConditionDeafened, RuleDefinitions.DurationType.Minute, 1);
                }
            }

            public override void FillTags(Dictionary<string, TagsDefinitions.Criticity> tagsMap)
            {
                DatabaseHelper.ConditionDefinitions.ConditionDeafened.FillTags(tagsMap);
                DatabaseHelper.ConditionDefinitions.ConditionBlinded.FillTags(tagsMap);
                DatabaseHelper.ConditionDefinitions.ConditionStunned.FillTags(tagsMap);
                DatabaseHelper.ConditionDefinitions.ConditionBanished.FillTags(tagsMap);
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

                int sourceAbilityBonus = formsParams.activeEffect != null ? formsParams.activeEffect.ComputeSourceAbilityBonus(formsParams.sourceCharacter) : 0;

                formsParams.targetCharacter.InflictCondition(condition.Name, durationType, durationParam, RuleDefinitions.TurnOccurenceType.EndOfTurn, "11Effect", sourceGuid, sourceFaction, formsParams.effectLevel, effectDefinitionName, 0, sourceAbilityBonus);
            }
        }

        //
        // DubbHerder SRD Spells
        //

        private const string DhBaseGuid = "05c1b1dbae144731b4505c1232fdc37e";

        private const string DhBaseString = "DH";

        private static string GetSpellTitleTerm(string text) => $"Spell/&{DhBaseString}{text}Title";

        private static string GetSpellDescriptionTerm(string text) => $"Spell/&{DhBaseString}{text}Description";

        private static SpellDefinition BuildFingerOfDeath()
        {
            string text = "FingerOfDeathSpell";

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

                //    effectDescription.AddEffectForm(new EffectFormBuilder()
                //        .SetSummonForm
                //        (
                //        SummonForm.Type.Creature,
                //        new ItemDefinition(),
                //        1,
                //        DatabaseHelper.MonsterDefinitions.Zombie.name,
                //        DatabaseHelper.ConditionDefinitions.ConditionMindDominatedByCaster,
                //        true,
                //        DatabaseHelper.DecisionPackageDefinitions.IdleGuard_Default,
                //        new EffectProxyDefinition()
                //        )
                //        .Build()
                //        );

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

                //  effectDescription.AddEffectForm(
                //  new EffectFormBuilder().SetConditionForm(
                //      SummonZombieConditionBuilder.SummonZombieCondition,
                //      ConditionForm.ConditionOperation.Add,
                //      true,
                //      true,
                //      new List<ConditionDefinition>()
                //      )
                //    //  .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.Negates)
                //  .Build());

                .SetSavingThrowData(
                    true,
                    false,
                    DatabaseHelper.SmartAttributeDefinitions.Constitution.name,
                    true,
                    RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                    DatabaseHelper.SmartAttributeDefinitions.Constitution.name,
                    20,
                    false,
                    new List<SaveAffinityBySenseDescription>());

            GuiPresentation guiPresentationSpell = new GuiPresentation()
                .SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f))
                .SetDescription(GetSpellDescriptionTerm(text))
                .SetTitle(GetSpellTitleTerm(text))
                .SetSpriteReference(DatabaseHelper.SpellDefinitions.RayOfEnfeeblement.GuiPresentation.SpriteReference)
                .SetSymbolChar("221E");

            SpellBuilder fingerOfDeathSpell = new SpellBuilder(DhBaseString + text, GuidHelper.Create(new System.Guid(DhBaseGuid), DhBaseString + text).ToString())
                .SetGuiPresentation(guiPresentationSpell)
                .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(7)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetVerboseComponent(true)
                .SetSomaticComponent(false)
                .SetEffectDescription(effectDescription.Build())
                .SetAiParameters(new SpellAIParameters());

            return fingerOfDeathSpell.AddToDB();
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
            string text = "ReverseGravitySpell";

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
                    DatabaseHelper.SmartAttributeDefinitions.Dexterity.name,
                    true,
                    RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                    DatabaseHelper.SmartAttributeDefinitions.Dexterity.name,
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
                .SetRecurrentEffect(DatabaseHelper.SpellDefinitions.Entangle.EffectDescription.RecurrentEffect);

            GuiPresentation guiPresentationSpell = new GuiPresentation()
                .SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f))
                .SetDescription(GetSpellDescriptionTerm(text))
                .SetTitle(GetSpellTitleTerm(text))
                .SetSpriteReference(DatabaseHelper.SpellDefinitions.Thunderwave.GuiPresentation.SpriteReference)
                .SetSymbolChar("221E");

            SpellBuilder reverseGravitySpell = new SpellBuilder(DhBaseString + text, GuidHelper.Create(new System.Guid(DhBaseGuid), DhBaseString + text).ToString())
                .SetGuiPresentation(guiPresentationSpell)
                .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(7)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetVerboseComponent(true)
                .SetSomaticComponent(true)
                .SetEffectDescription(effectDescription.Build())
                .SetAiParameters(new SpellAIParameters())
                .SetConcentration();

            return reverseGravitySpell.AddToDB();
        }

        internal class ReverseGravityConditionBuilder : BaseDefinitionBuilder<ConditionDefinition>
        {
            const string Name = "DHReverseGravitySpellcondition";
            const string Guid = "809f1cef-6bdc-4b5a-93bf-275af8ab0b36";

            const string TitleString = "Condition/&DHReverseGravitySpellTitle";
            const string DescriptionString = "Condition/&DHReverseGravitySpellDescription";

            protected ReverseGravityConditionBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionLevitate, name, guid)
            {
                Definition.GuiPresentation.Title = TitleString;
                Definition.GuiPresentation.Description = DescriptionString;
                Definition.SetConditionType(RuleDefinitions.ConditionType.Neutral);
                Definition.Features.SetRange
                (
                    DatabaseHelper.FeatureDefinitionMovementAffinitys.MovementAffinityConditionLevitate,
                    DatabaseHelper.FeatureDefinitionMoveModes.MoveModeFly2
                );
            }

            private static ConditionDefinition CreateAndAddToDB(string name, string guid)
                => new ReverseGravityConditionBuilder(name, guid).AddToDB();

            internal static readonly ConditionDefinition ReverseGravityCondition = CreateAndAddToDB(Name, Guid);
        }

        private static SpellDefinition BuildConjureCelestial()
        {
            SpellListDefinition CouatlSpellList = SpellListBuilder.CreateSpellList(
                 DhBaseString + "CouatlSpellList",
                 GuidHelper.Create(new System.Guid(DhBaseGuid), DhBaseString + "CouatlSpellList").ToString(),
                 "",
                 DatabaseHelper.SpellListDefinitions.SpellListCleric,
                 new List<SpellDefinition>(),
                 new List<SpellDefinition>
                 {
                     DatabaseHelper.SpellDefinitions.Bless,
                     DatabaseHelper.SpellDefinitions.CureWounds,
                     DatabaseHelper.SpellDefinitions.DetectEvilAndGood,
                     DatabaseHelper.SpellDefinitions.DetectMagic,
                     DatabaseHelper.SpellDefinitions.Shield
                 },
                 new List<SpellDefinition>
                 {
                     DatabaseHelper.SpellDefinitions.LesserRestoration,
                     DatabaseHelper.SpellDefinitions.ProtectionFromPoison
                 },
                 new List<SpellDefinition>(),
                 new List<SpellDefinition>(),
                 new List<SpellDefinition>
                 {
                     DatabaseHelper.SpellDefinitions.GreaterRestoration
                 });

            CouatlSpellList.SetHasCantrips(false);
            CouatlSpellList.SetMaxSpellLevel(5);

            FeatureDefinitionCastSpell castSpellCouatl =
                new CastSpellBuilder(DhBaseString + "CastSpellCouatl", GuidHelper.Create(new System.Guid(DhBaseGuid), DhBaseString + "CastSpellCouatl").ToString())
                .AddToDB()
                .SetGuiPresentation(new GuiPresentationBuilder("Feature/&NoContentTitle", "Feature/&NoContentTitle").Build())
                .SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Monster)
                .SetSpellcastingAbility(AttributeDefinitions.Charisma)
                .SetSpellcastingParametersComputation(RuleDefinitions.SpellcastingParametersComputation.Static)
                .SetStaticDCValue(14)
                .SetStaticToHitValue(8)
                .SetSpellListDefinition(CouatlSpellList)
                .SetSpellKnowledge(RuleDefinitions.SpellKnowledge.FixedList)
                .SetSlotsRecharge(RuleDefinitions.RechargeRate.ShortRest)
                .SetSpellCastingLevel(9)
                .SetSpellReadyness(RuleDefinitions.SpellReadyness.AllKnown)
                .SetFocusType(EquipmentDefinitions.FocusType.None);

            int[] castSpellCouatlKnownSpells = { 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20 };

            castSpellCouatl.RestrictedSchools.Clear();
            castSpellCouatl.KnownSpells.Clear();
            castSpellCouatl.KnownSpells.AddRange(castSpellCouatlKnownSpells);
            castSpellCouatl.SlotsPerLevels.AddRange(new List<FeatureDefinitionCastSpell.SlotsByLevelDuplet>
            {
                new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {13,6,1,1,1,0,0,0,0,0}, Level = 01 },
                new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {13,6,1,1,1,0,0,0,0,0}, Level = 02 },
                new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {13,6,1,1,1,0,0,0,0,0}, Level = 03 },
                new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {13,6,1,1,1,0,0,0,0,0}, Level = 04 },
                new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {13,6,1,1,1,0,0,0,0,0}, Level = 05 },
                new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {13,6,1,1,1,0,0,0,0,0}, Level = 06 },
                new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {13,6,1,1,1,0,0,0,0,0}, Level = 07 },
                new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {13,6,1,1,1,0,0,0,0,0}, Level = 08 },
                new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {13,6,1,1,1,0,0,0,0,0}, Level = 09 },
                new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {13,6,1,1,1,0,0,0,0,0}, Level = 10 },
                new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {13,6,1,1,1,0,0,0,0,0}, Level = 11 },
                new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {13,6,1,1,1,0,0,0,0,0}, Level = 12 },
                new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {13,6,1,1,1,0,0,0,0,0}, Level = 13 },
                new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {13,6,1,1,1,0,0,0,0,0}, Level = 14 },
                new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {13,6,1,1,1,0,0,0,0,0}, Level = 15 },
                new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {13,6,1,1,1,0,0,0,0,0}, Level = 16 },
                new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {13,6,1,1,1,0,0,0,0,0}, Level = 17 },
                new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {13,6,1,1,1,0,0,0,0,0}, Level = 18 },
                new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {13,6,1,1,1,0,0,0,0,0}, Level = 19 },
                new FeatureDefinitionCastSpell.SlotsByLevelDuplet() { Slots = new List<int> {13,6,1,1,1,0,0,0,0,0}, Level = 20 },
            });

            string text = "ConjureCelestialSpell";

            MonsterDefinition BaseTemplateName = DatabaseHelper.MonsterDefinitions.KindredSpiritViper;
            MonsterDefinition MonsterShaderReference = DatabaseHelper.MonsterDefinitions.KindredSpiritViper;

            string NewName = "CustomCouatl";
            string NewTitle = "CustomCouatlTitle";
            string NewDescription = "CustomCouatlDescription";
            CharacterSizeDefinition Size = DatabaseHelper.CharacterSizeDefinitions.Medium;
            string Alignment = "LawfulGood";
            int ArmorClass = 19;
            int HitDice = 13;
            RuleDefinitions.DieType HitDiceType = RuleDefinitions.DieType.D8;
            int HitPointsBonus = 39;
            int StandardHitPoints = 97;
            int AttributeStrength = 16;
            int AttributeDexterity = 20;
            int AttributeConstitution = 17;
            int AttributeIntelligence = 18;
            int AttributeWisdom = 20;
            int AttributeCharisma = 18;
            int SavingThrowStrength = 0;
            int SavingThrowDexterity = 0;
            int SavingThrowConstitution = 5;
            int SavingThrowIntelligence = 0;
            int SavingThrowWisdom = 7;
            int SavingThrowCharisma = 6;
            int CR = 4;
            bool LegendaryCreature = false;
            string Type = "Celestial";

            List<FeatureDefinition> Features = new List<FeatureDefinition>()
            {
                DatabaseHelper.FeatureDefinitionMoveModes.MoveModeFly12,
                DatabaseHelper.FeatureDefinitionSenses.SenseNormalVision,
                DatabaseHelper.FeatureDefinitionSenses.SenseSuperiorDarkvision,
                DatabaseHelper.FeatureDefinitionSenses.SenseTruesight16,
                // wildshape was meant to be a substitute for the couatl's shapechanging but the game didnt like it
                // (gave the couatl a second position in the intiative order)
                // DatabaseHelper.FeatureDefinitionPowers.PowerDruidWildShape,
                DatabaseHelper.FeatureDefinitionAttackModifiers.AttackModifierKindredSpiritMagicSpiritMagicAttack,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityRadiantResistance,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPsychicImmunity,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityMummyLord_PiercingImmunity,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityMummyLord_SlashingImmunity,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityMummyLord_BludgeoningImmunity,
                DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityPoisonImmunity,
                castSpellCouatl
            };

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
                DatabaseHelper.MonsterDefinitions.Tiger_Drake.AttackIterations[0]
                // CouatlBite_Attack
            };

            List<LegendaryActionDescription> LegendaryActionOptions = new List<LegendaryActionDescription>();

            bool GroupAttacks = false;

            bool PhantomDistortion = true;
            // AttachedParticlesReference = "0286006526f6f9c4fa61ed8ead4f72cc"
            AssetReference AttachedParticlesReference = DatabaseHelper.MonsterDefinitions.FeyBear.MonsterPresentation.GetField<UnityEngine.AddressableAssets.AssetReference>("attachedParticlesReference");
            AssetReferenceSprite SpriteReference = DatabaseHelper.MonsterDefinitions.KindredSpiritViper.GuiPresentation.SpriteReference;

            MonsterBuilder couatlBuilder = new MonsterBuilder(
                NewName,
                "fc38ed74-3e2f-4960-86cc-3120d638410b",
                "Monster/&" + DhBaseString + NewTitle,
                "Monster/&" + DhBaseString + NewDescription,
                BaseTemplateName);

            couatlBuilder.SetInDungeonEditor(false);
            couatlBuilder.SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.None);
            couatlBuilder.SetSizeDefinition(Size);
            couatlBuilder.SetChallengeRating(CR);
            couatlBuilder.SetAlignment(Alignment);
            couatlBuilder.SetCharacterFamily(Type);
            couatlBuilder.SetLegendaryCreature(LegendaryCreature);
            couatlBuilder.SetArmorClass(ArmorClass);
            couatlBuilder.SetHitDiceNumber(HitDice);
            couatlBuilder.SetHitDiceType(HitDiceType);
            couatlBuilder.SetHitPointsBonus(HitPointsBonus);
            couatlBuilder.SetStandardHitPoints(StandardHitPoints);
            couatlBuilder.ClearFeatures();
            couatlBuilder.AddFeatures(Features);
            couatlBuilder.ClearAbilityScores();
            couatlBuilder.SetAbilityScores(
                AttributeStrength,
                AttributeDexterity,
                AttributeConstitution,
                AttributeIntelligence,
                AttributeWisdom,
                AttributeCharisma);

            MonsterSavingThrowProficiency StrSave = new MonsterSavingThrowProficiency();
            StrSave.SetField("abilityScoreName", AttributeDefinitions.Strength);
            StrSave.SetField("bonus", SavingThrowStrength);

            MonsterSavingThrowProficiency DexSave = new MonsterSavingThrowProficiency();
            DexSave.SetField("abilityScoreName", AttributeDefinitions.Dexterity);
            DexSave.SetField("bonus", SavingThrowDexterity);

            MonsterSavingThrowProficiency ConSave = new MonsterSavingThrowProficiency();
            ConSave.SetField("abilityScoreName", AttributeDefinitions.Constitution);
            ConSave.SetField("bonus", SavingThrowConstitution);

            MonsterSavingThrowProficiency IntSave = new MonsterSavingThrowProficiency();
            IntSave.SetField("abilityScoreName", AttributeDefinitions.Intelligence);
            IntSave.SetField("bonus", SavingThrowIntelligence);

            MonsterSavingThrowProficiency WisSave = new MonsterSavingThrowProficiency();
            WisSave.SetField("abilityScoreName", AttributeDefinitions.Wisdom);
            WisSave.SetField("bonus", SavingThrowWisdom);

            MonsterSavingThrowProficiency ChaSave = new MonsterSavingThrowProficiency();
            ChaSave.SetField("abilityScoreName", AttributeDefinitions.Charisma);
            ChaSave.SetField("bonus", SavingThrowCharisma);

            couatlBuilder.ClearSavingThrowScores();
            couatlBuilder.AddSavingThrowScores(new List<MonsterSavingThrowProficiency>()
            {
                StrSave,
                DexSave,
                ConSave,
                IntSave,
                WisSave,
                ChaSave
            });

            couatlBuilder.ClearSkillScores();
            couatlBuilder.AddSkillScores(SkillScores);
            couatlBuilder.ClearAttackIterations();
            couatlBuilder.AddAttackIterations(AttackIterations);
            //  couatlBuilder.SetDefaultBattleDecisionPackage(DefaultBattleDecisionPackage);
            couatlBuilder.SetGroupAttacks(GroupAttacks);
            couatlBuilder.ClearLegendaryActionOptions();
            couatlBuilder.AddLegendaryActionOptions(LegendaryActionOptions);
            couatlBuilder.SetSpriteReference(SpriteReference);
            couatlBuilder.SetHasPhantomDistortion(PhantomDistortion);
            couatlBuilder.SetAttachedParticlesReference(AttachedParticlesReference);
            couatlBuilder.SetNoExperienceGain(false);
            couatlBuilder.SetHasMonsterPortraitBackground(true);
            couatlBuilder.SetCanGeneratePortrait(true);
            couatlBuilder.SetCustomShaderReference(MonsterShaderReference);

            MonsterDefinition Couatl = couatlBuilder.AddToDB();

            Couatl.CreatureTags.Clear();

            EffectDescriptionBuilder effectDescription = new EffectDescriptionBuilder()
                .SetDurationData(RuleDefinitions.DurationType.Minute, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn)
                .SetTargetingData(RuleDefinitions.Side.All, RuleDefinitions.RangeType.Distance, 18, RuleDefinitions.TargetType.Position, 1, 1, 0)
                .AddEffectForm(new EffectFormBuilder()
                    .SetSummonForm(
                        SummonForm.Type.Creature,
                        DatabaseHelper.ItemDefinitions.Dagger,
                        1,
                        NewName,
                        DatabaseHelper.ConditionDefinitions.ConditionMindDominatedByCaster,
                        false,
                        DatabaseHelper.DecisionPackageDefinitions.IdleGuard_Default,
                        DatabaseHelper.EffectProxyDefinitions.ProxyDancingLights)
                    .Build())
                .SetCreatedByCharacter()
                .SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.ConjureElementalAir.EffectDescription.EffectParticleParameters);

            GuiPresentation guiPresentationSpell = new GuiPresentation()
                .SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f))
                .SetDescription(GetSpellDescriptionTerm(text))
                .SetTitle(GetSpellTitleTerm(text))
                .SetSpriteReference(DatabaseHelper.SpellDefinitions.Bless.GuiPresentation.SpriteReference)
                .SetSymbolChar("221E");

            SpellBuilder conjureCelestialSpell = new SpellBuilder(DhBaseString + text, GuidHelper.Create(new System.Guid(DhBaseGuid), DhBaseString + text).ToString())
                .SetGuiPresentation(guiPresentationSpell)
                .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(7)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetVerboseComponent(true)
                .SetSomaticComponent(true)
                .SetEffectDescription(effectDescription.Build())
                .SetAiParameters(new SpellAIParameters())
                .SetConcentration();

            return conjureCelestialSpell.AddToDB().SetRequiresConcentration(true);
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
                    guiPresentationSpell.SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f));
                    guiPresentationSpell.SetDescription(GetSpellDescriptionTerm(text));
                    guiPresentationSpell.SetTitle(GetSpellTitleTerm(text));
                    guiPresentationSpell.SetSpriteReference(DatabaseHelper.SpellDefinitions.ConjureFey.GuiPresentation.SpriteReference);
                    guiPresentationSpell.SetSymbolChar("221E");

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
            string text = "DominateMonsterSpell";

            EffectDescription effectDescription = new EffectDescription();

            effectDescription.Copy(DatabaseHelper.SpellDefinitions.DominatePerson.EffectDescription);
            effectDescription.RestrictedCreatureFamilies.Clear();
            effectDescription.SetDurationType(RuleDefinitions.DurationType.Hour);

            GuiPresentation guiPresentationSpell = new GuiPresentation()
                .SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f))
                .SetDescription(GetSpellDescriptionTerm(text))
                .SetTitle(GetSpellTitleTerm(text))
                .SetSpriteReference(DatabaseHelper.SpellDefinitions.DominatePerson.GuiPresentation.SpriteReference)
                .SetSymbolChar("221E");

            SpellBuilder dominateMonsterSpell = new SpellBuilder(DhBaseString + text, GuidHelper.Create(new System.Guid(DhBaseGuid), DhBaseString + text).ToString())
                .SetGuiPresentation(guiPresentationSpell)
                .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(8)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetVerboseComponent(true)
                .SetSomaticComponent(true)
                .SetEffectDescription(effectDescription)
                .SetAiParameters(new SpellAIParameters());

            return dominateMonsterSpell.AddToDB();
        }

        private static SpellDefinition BuildFeeblemind()
        {
            string text = "FeeblemindSpell";

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
                DatabaseHelper.SmartAttributeDefinitions.Intelligence.name,
                true,
                RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                DatabaseHelper.SmartAttributeDefinitions.Intelligence.name,
                20,
                false,
                new List<SaveAffinityBySenseDescription> { }
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

            GuiPresentation guiPresentationSpell = new GuiPresentation()
                .SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f))
                .SetDescription(GetSpellDescriptionTerm(text))
                .SetTitle(GetSpellTitleTerm(text))
                .SetSpriteReference(DatabaseHelper.SpellDefinitions.BestowCurse.GuiPresentation.SpriteReference)
                .SetSymbolChar("221E");

            SpellBuilder feeblemindSpell = new SpellBuilder(DhBaseString + text, GuidHelper.Create(new System.Guid(DhBaseGuid), DhBaseString + text).ToString())
                .SetGuiPresentation(guiPresentationSpell)
                .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(8)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetVerboseComponent(true)
                .SetSomaticComponent(false)
                .SetEffectDescription(effectDescription.Build())
                .SetAiParameters(new SpellAIParameters());

            DatabaseHelper.SpellDefinitions.GreaterRestoration.EffectDescription.EffectForms[0].ConditionForm.DetrimentalConditions.Add(FeeblemindConditionBuilder.FeeblemindCondition);

            return feeblemindSpell.AddToDB();
        }

        internal class FeeblemindConditionBuilder : BaseDefinitionBuilder<ConditionDefinition>
        {
            const string Name = "DHFeeblemindSpellcondition";
            const string Guid = "965a09b2-cb22-452b-b93c-2bccdcda4871";

            const string TitleString = "Condition/&DHFeeblemindSpellTitle";
            const string DescriptionString = "Condition/&DHFeeblemindSpellDescription";

            protected FeeblemindConditionBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionBearsEndurance, name, guid)
            {
                Definition.GuiPresentation.Title = TitleString;
                Definition.GuiPresentation.Description = DescriptionString;
                Definition.SetConditionType(RuleDefinitions.ConditionType.Detrimental);
                Definition.Features.SetRange(
                    DatabaseHelper.FeatureDefinitionMagicAffinitys.MagicAffinitySilenced,
                    DatabaseHelper.FeatureDefinitionMagicAffinitys.MagicAffinityConditionRaging,
                    FeeblemindIntAttributeModifierBuilder.FeeblemindIntAttributeModifier,
                    FeeblemindCha_AttributeModifierBuilder.FeeblemindCha_AttributeModifier,
                    FeeblemindActionAffinityBuilder.FeeblemindActionAffinity);
            }

            internal static ConditionDefinition CreateAndAddToDB(string name, string guid)
                => new FeeblemindConditionBuilder(name, guid).AddToDB();

            internal static readonly ConditionDefinition FeeblemindCondition = CreateAndAddToDB(Name, Guid);
        }

        internal class FeeblemindIntAttributeModifierBuilder : BaseDefinitionBuilder<FeatureDefinitionAttributeModifier>
        {
            const string Name = "DHFeeblemindIntSpellAttributeModifier";
            const string Guid = "a2a16bda-e7b1-4a87-9f0e-3e4c21870fd8";

            const string TitleString = "AttributeModifier/&DHFeeblemindIntSpellTitle";
            const string DescriptionString = "AttributeModifier/&DHFeeblemindIntSpellDescription";

            protected FeeblemindIntAttributeModifierBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierHeadbandOfIntellect, name, guid)
            {
                Definition.GuiPresentation.Title = TitleString;
                Definition.GuiPresentation.Description = DescriptionString;
                Definition.SetModifiedAttribute(DatabaseHelper.SmartAttributeDefinitions.Intelligence.name);
                Definition.SetModifierAbilityScore(DatabaseHelper.SmartAttributeDefinitions.Intelligence.name);
                Definition.SetModifierType2(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Force);
                Definition.SetModifierValue(1);
                Definition.SetSituationalContext(RuleDefinitions.SituationalContext.None);

            }
            private static FeatureDefinitionAttributeModifier CreateAndAddToDB(string name, string guid)
                => new FeeblemindIntAttributeModifierBuilder(name, guid).AddToDB();

            internal static readonly FeatureDefinitionAttributeModifier FeeblemindIntAttributeModifier = CreateAndAddToDB(Name, Guid);
        }

        internal class FeeblemindCha_AttributeModifierBuilder : BaseDefinitionBuilder<FeatureDefinitionAttributeModifier>
        {
            const string Name = "DHFeeblemindChaSpellAttributeModifier";
            const string Guid = "6721abe1-19eb-4a8c-9702-2fdea2070464";

            const string TitleString = "AttributeModifier/&DHFeeblemindChaSpellTitle";
            const string DescriptionString = "AttributeModifier/&DHFeeblemindChaSpellDescription";

            protected FeeblemindCha_AttributeModifierBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierHeadbandOfIntellect, name, guid)
            {
                Definition.GuiPresentation.Title = TitleString;
                Definition.GuiPresentation.Description = DescriptionString;
                Definition.SetModifiedAttribute(DatabaseHelper.SmartAttributeDefinitions.Charisma.name);
                Definition.SetModifierAbilityScore(DatabaseHelper.SmartAttributeDefinitions.Charisma.name);
                Definition.SetModifierType2(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Force);
                Definition.SetModifierValue(1);
                Definition.SetSituationalContext(RuleDefinitions.SituationalContext.None);
            }

            private static FeatureDefinitionAttributeModifier CreateAndAddToDB(string name, string guid)
                => new FeeblemindCha_AttributeModifierBuilder(name, guid).AddToDB();

            internal static readonly FeatureDefinitionAttributeModifier FeeblemindCha_AttributeModifier = CreateAndAddToDB(Name, Guid);
        }

        internal class FeeblemindActionAffinityBuilder : BaseDefinitionBuilder<FeatureDefinitionActionAffinity>
        {
            const string Name = "DHFeeblemindSpellActionAffinity";
            const string Guid = "749a9572-07f6-4678-9458-904c04b9ab22";

            const string TitleString = "ActionAffinity/&DHFeeblemindSpellTitle";
            const string DescriptionString = "ActionAffinity/&DHFeeblemindSpellDescription";

            protected FeeblemindActionAffinityBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionActionAffinitys.ActionAffinityConditionRaging, name, guid)
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
                => new FeeblemindActionAffinityBuilder(name, guid).AddToDB();

            internal static readonly FeatureDefinitionActionAffinity FeeblemindActionAffinity = CreateAndAddToDB(Name, Guid);
        }

        private static SpellDefinition BuildHolyAura()
        {
            string text = "HolyAuraSpell";

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
                    DatabaseHelper.SpellDefinitions.BeaconOfHope.
                    EffectDescription.EffectParticleParameters)
                .AddEffectForm( new EffectFormBuilder()
                    .SetConditionForm(
                        HolyAuraConditionBuilder.HolyAuraCondition,
                        ConditionForm.ConditionOperation.Add,
                        false,
                        false,
                        new List<ConditionDefinition>())
                    .Build());

            GuiPresentation guiPresentationSpell = new GuiPresentation()
                .SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f))
                .SetDescription(GetSpellDescriptionTerm(text))
                .SetTitle(GetSpellTitleTerm(text))
                .SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerDomainLifePreserveLife.GuiPresentation.SpriteReference)
                .SetSymbolChar("221E");

            SpellBuilder holyAuraSpell = new SpellBuilder(DhBaseString + text, GuidHelper.Create(new System.Guid(DhBaseGuid), DhBaseString + text).ToString())
                .SetGuiPresentation(guiPresentationSpell)
                .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(8)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetVerboseComponent(true)
                .SetSomaticComponent(false)
                .SetEffectDescription(effectDescription.Build())
                .SetAiParameters(new SpellAIParameters())
                .SetConcentration();

            return holyAuraSpell.AddToDB();
        }


        internal class HolyAuraConditionBuilder : BaseDefinitionBuilder<ConditionDefinition>
        {
            const string Name = "DHHolyAuraSpellcondition";
            const string Guid = "1808ca4b-8f46-41bf-a59c-0bcbd4f60248";

            const string TitleString = "Condition/&DHHolyAuraSpellTitle";
            const string DescriptionString = "Condition/&DHHolyAuraSpellDescription";

            protected HolyAuraConditionBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionBearsEndurance, name, guid)
            {
                Definition.GuiPresentation.Title = TitleString;
                Definition.GuiPresentation.Description = DescriptionString;
                Definition.SetConditionType(RuleDefinitions.ConditionType.Beneficial);
                Definition.Features.SetRange
                (
                    DatabaseHelper.FeatureDefinitionCombatAffinitys.CombatAffinityDodging,
                    DatabaseHelper.FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityShelteringBreeze,
                    HolyAuraDamageAffinityBuilder.HolyAuraDamageAffinity
                );
            }

            private static ConditionDefinition CreateAndAddToDB(string name, string guid)
                => new HolyAuraConditionBuilder(name, guid).AddToDB();

            internal static readonly ConditionDefinition HolyAuraCondition = CreateAndAddToDB(Name, Guid);
        }

        internal class HolyAuraDamageAffinityBuilder : BaseDefinitionBuilder<FeatureDefinitionDamageAffinity>
        {
            const string Name = "DHHolyAuraSpellDamageAffinity";
            const string Guid = "c83aceae-e4c4-4a9c-a83d-58ffebe92007";

            const string TitleString = "DamageAffinity/&DHHolyAuraSpellTitle";
            const string DescriptionString = "DamageAffinity/&DHHolyAuraSpellDescription";

            protected HolyAuraDamageAffinityBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPoisonAdvantage, name, guid)
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
                => new HolyAuraDamageAffinityBuilder(name, guid).AddToDB();

            internal static readonly FeatureDefinitionDamageAffinity HolyAuraDamageAffinity = CreateAndAddToDB(Name, Guid);
        }

        internal class HolyAuraBlindingPowerBuilder : BaseDefinitionBuilder<FeatureDefinitionPower>
        {
            const string Name = "DHHolyAuraSpellBlindingPower";
            const string Guid = "40366ca2-00a0-471a-b370-8c81f6283ce1";

            const string TitleString = "Feature/&DHHolyAura_BlindingPower_Title";
            const string DescriptionString = "Feataure/&DHHolyAura_BlindingPower_Description";

            protected HolyAuraBlindingPowerBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionPowers.PowerOathOfMotherlandFieryPresence, name, guid)
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
                .AddRestrictedCreatureFamilies(DatabaseHelper.CharacterFamilyDefinitions.Fiend)
                .AddRestrictedCreatureFamilies(DatabaseHelper.CharacterFamilyDefinitions.Undead)
                .AddEffectForm(new EffectFormBuilder()
                    .SetConditionForm(
                        DatabaseHelper.ConditionDefinitions.ConditionBlinded,
                        ConditionForm.ConditionOperation.Add,
                        false,
                        false,
                        new List<ConditionDefinition>())
                     .Build());

                Definition.SetEffectDescription(effectDescription.Build());
            }

            private static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
                => new HolyAuraBlindingPowerBuilder(name, guid).AddToDB();

            internal static readonly FeatureDefinitionPower HolyAuraBlindingPower = CreateAndAddToDB(Name, Guid);
        }

        private static SpellDefinition BuildIncendiaryCloud()
        {

            string text = "IncendiaryCloudSpell";

            //
            // TODO: Why this effect description isn't used?
            //

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
                    4,
                    4,
                    ActionDefinitions.ItemSelectionType.None)
                .AddEffectForm(new EffectFormBuilder()
                    .SetSummonForm(
                        SummonForm.Type.EffectProxy,
                        DatabaseHelper.ItemDefinitions.Dart,
                        1,
                        DatabaseHelper.MonsterDefinitions.Adam_The_Twelth.name,
                        null,
                        false,
                        DatabaseHelper.DecisionPackageDefinitions.IdleGuard_Default,
                        DatabaseHelper.EffectProxyDefinitions.ProxyFogCloud)
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
                .SetRecurrentEffect(DatabaseHelper.SpellDefinitions.WallOfFireLine.EffectDescription.RecurrentEffect)
                .SetVelocity(2, RuleDefinitions.VelocityType.AwayFromSourceOriginalPosition)
                .SetSavingThrowData(
                        true,
                        false,
                        DatabaseHelper.SmartAttributeDefinitions.Dexterity.name,
                        false,
                        RuleDefinitions.EffectDifficultyClassComputation.FixedValue,
                        DatabaseHelper.SmartAttributeDefinitions.Dexterity.name,
                        20,
                        false,
                        new List<SaveAffinityBySenseDescription>());

            effectDescription.Build();

            EffectDescription neweffectDescription = new EffectDescription();

            neweffectDescription.Copy(DatabaseHelper.SpellDefinitions.FogCloud.EffectDescription);

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

            neweffectDescription.SetSavingThrowAbility(DatabaseHelper.SmartAttributeDefinitions.Dexterity.name);

            GuiPresentation guiPresentationSpell = new GuiPresentation()
                .SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f))
                .SetDescription(GetSpellDescriptionTerm(text))
                .SetTitle(GetSpellTitleTerm(text))
                .SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalFireBurst.GuiPresentation.SpriteReference)
                .SetSymbolChar("221E");

            SpellBuilder IncendiaryCloudSpell = new SpellBuilder(DhBaseString + text, GuidHelper.Create(new System.Guid(DhBaseGuid), DhBaseString + text).ToString())
                .SetGuiPresentation(guiPresentationSpell)
                .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(8)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetVerboseComponent(true)
                .SetSomaticComponent(false)
                //  IncendiaryCloudSpell.SetEffectDescription(effectDescription.Build());
                .SetEffectDescription(neweffectDescription)
                .SetAiParameters(new SpellAIParameters())
                //    IncendiaryCloudSpell
                .SetConcentration();

            return IncendiaryCloudSpell.AddToDB();
        }

        private static SpellDefinition BuildMaze()
        {
            string text = "MazeSpell";

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
                DatabaseHelper.SmartAttributeDefinitions.Intelligence.name,
                false,
                RuleDefinitions.EffectDifficultyClassComputation.FixedValue,
                DatabaseHelper.SmartAttributeDefinitions.Intelligence.name,
                20,
                false,
                new List<SaveAffinityBySenseDescription>())
            .AddEffectForm( new EffectFormBuilder()
                .SetConditionForm(
                    DatabaseHelper.ConditionDefinitions.ConditionBanished,
                    ConditionForm.ConditionOperation.Add,
                    false,
                    false,
                    new List<ConditionDefinition>())
                .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.Negates)
                .CanSaveToCancel(RuleDefinitions.TurnOccurenceType.EndOfTurn)
                .Build());

            GuiPresentation guiPresentationSpell = new GuiPresentation()
                .SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f))
                .SetDescription(GetSpellDescriptionTerm(text))
                .SetTitle(GetSpellTitleTerm(text))
                .SetSpriteReference(DatabaseHelper.SpellDefinitions.IdentifyCreatures.GuiPresentation.SpriteReference)
                .SetSymbolChar("221E");

            SpellBuilder mazeSpell = new SpellBuilder(DhBaseString + text, GuidHelper.Create(new System.Guid(DhBaseGuid), DhBaseString + text).ToString())
                .SetGuiPresentation(guiPresentationSpell)
                .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(8)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetVerboseComponent(true)
                .SetSomaticComponent(false)
                .SetEffectDescription(effectDescription.Build())
                .SetAiParameters(new SpellAIParameters())
                .SetConcentration();

            return mazeSpell.AddToDB();
        }

        private static SpellDefinition BuildMindBlank()
        {

            string text = "MindBlankSpell";

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

            effectDescription.Build();

            GuiPresentation guiPresentationSpell = new GuiPresentation()
                .SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f))
                .SetDescription(GetSpellDescriptionTerm(text))
                .SetTitle(GetSpellTitleTerm(text))
                .SetSpriteReference(DatabaseHelper.SpellDefinitions.MindTwist.GuiPresentation.SpriteReference)
                .SetSymbolChar("221E");

            SpellBuilder mindBlankSpell = new SpellBuilder(DhBaseString + text, GuidHelper.Create(new System.Guid(DhBaseGuid), DhBaseString + text).ToString())
                .SetGuiPresentation(guiPresentationSpell)
                .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(8)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetVerboseComponent(true)
                .SetSomaticComponent(false)
                .SetEffectDescription(effectDescription.Build())
                .SetAiParameters(new SpellAIParameters());

            return mindBlankSpell.AddToDB();
        }

        internal class MindBlankConditionBuilder : BaseDefinitionBuilder<ConditionDefinition>
        {
            const string Name = "DHMindBlankSpellcondition";
            const string Guid = "74f77a4c-b5cb-45d6-ac6d-d9fa2ebe3869";

            const string TitleString = "Condition/&DHMindBlankSpellTitle";
            const string DescriptionString = "Condition/&DHMindBlankSpellDescription";

            protected MindBlankConditionBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionBearsEndurance, name, guid)
            {
                Definition.GuiPresentation.Title = TitleString;
                Definition.GuiPresentation.Description = DescriptionString;
                Definition.Features.SetRange(
                    DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityCharmImmunity,
                    DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityCharmImmunityHypnoticPattern,
                    DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityCalmEmotionCharmedImmunity,
                    DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPsychicImmunity);
            }
            private static ConditionDefinition CreateAndAddToDB(string name, string guid)
                => new MindBlankConditionBuilder(name, guid).AddToDB();

            internal static readonly ConditionDefinition MindBlankCondition = CreateAndAddToDB(Name, Guid);
        }

        private static SpellDefinition BuildPowerWordStun()
        {
            string text = "PowerWordStunSpell";

            ConditionForm conditionForm = new ConditionForm()
                .SetApplyToSelf(false)
                .SetForceOnSelf(false)
                .SetOperation(ConditionForm.ConditionOperation.Add)
                .SetConditionDefinitionName(DatabaseHelper.ConditionDefinitions.ConditionStunned.Name)
                .SetConditionDefinition(DatabaseHelper.ConditionDefinitions.ConditionStunned);

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
                    DatabaseHelper.SmartAttributeDefinitions.Constitution.name,
                    false,
                    RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                    DatabaseHelper.SmartAttributeDefinitions.Constitution.name,
                    15,
                    false,
                    new List<SaveAffinityBySenseDescription>());

            GuiPresentation guiPresentationSpell = new GuiPresentation()
                .SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f))
                .SetDescription(GetSpellDescriptionTerm(text))
                .SetTitle(GetSpellTitleTerm(text))
                .SetSpriteReference(DatabaseHelper.SpellDefinitions.Contagion.GuiPresentation.SpriteReference)
                .SetSymbolChar("221E");

            SpellBuilder powerWordStunSpell = new SpellBuilder(DhBaseString + text, GuidHelper.Create(new System.Guid(DhBaseGuid), DhBaseString + text).ToString())
                .SetGuiPresentation(guiPresentationSpell)
                .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(8)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetVerboseComponent(true)
                .SetSomaticComponent(false)
                .SetEffectDescription(effectDescription.Build())
                .SetAiParameters(new SpellAIParameters());

            return powerWordStunSpell.AddToDB();
        }


        private static SpellDefinition BuildSunBurst()
        {
            string text = "SunBurstSpell";

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
                    DatabaseHelper.SmartAttributeDefinitions.Constitution.name,
                    false,
                    RuleDefinitions.EffectDifficultyClassComputation.FixedValue,
                    DatabaseHelper.SmartAttributeDefinitions.Constitution.name,
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
                        DatabaseHelper.ConditionDefinitions.ConditionBlinded,
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
                .SetFamily(DatabaseHelper.CharacterFamilyDefinitions.Ooze.name);

            SaveAffinityByFamilyDescription SaveAffinityByFamilyDescriptionUndead = new SaveAffinityByFamilyDescription()
                .SetAdvantageType(RuleDefinitions.AdvantageType.Disadvantage)
                .SetFamily(DatabaseHelper.CharacterFamilyDefinitions.Undead.name);

            effect.SavingThrowAffinitiesByFamily.AddRange(SaveAffinityByFamilyDescription, SaveAffinityByFamilyDescriptionUndead);

            GuiPresentation guiPresentationSpell = new GuiPresentation()
                .SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f))
                .SetDescription(GetSpellDescriptionTerm(text))
                .SetTitle(GetSpellTitleTerm(text))
                .SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerDomainSunIndomitableLight.GuiPresentation.SpriteReference)
                .SetSymbolChar("221E");

            SpellBuilder sunBurstSpell = new SpellBuilder(DhBaseString + text, GuidHelper.Create(new System.Guid(DhBaseGuid), DhBaseString + text).ToString())
                .SetGuiPresentation(guiPresentationSpell)
                .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(8)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetVerboseComponent(true)
                .SetSomaticComponent(false)
                .SetEffectDescription(effect)
                .SetAiParameters(new SpellAIParameters());

            return sunBurstSpell.AddToDB();
        }

        private static SpellDefinition BuildForesight()
        {
            string text = "ForesightSpell";

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

            GuiPresentation guiPresentationSpell = new GuiPresentation()
                .SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f))
                .SetDescription(GetSpellDescriptionTerm(text))
                .SetTitle(GetSpellTitleTerm(text))
                .SetSpriteReference(DatabaseHelper.SpellDefinitions.TrueSeeing.GuiPresentation.SpriteReference)
                .SetSymbolChar("221E");

            SpellBuilder foresightSpell = new SpellBuilder(DhBaseString + text, GuidHelper.Create(new System.Guid(DhBaseGuid), DhBaseString + text).ToString())
                .SetGuiPresentation(guiPresentationSpell)
                .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(9)
                .SetCastingTime(RuleDefinitions.ActivationTime.Minute1)
                .SetVerboseComponent(true)
                .SetSomaticComponent(false)
                .SetEffectDescription(effectDescription.Build())
                .SetAiParameters(new SpellAIParameters());

            return foresightSpell.AddToDB();
        }

        internal class ForesightConditionBuilder : BaseDefinitionBuilder<ConditionDefinition>
        {
            const string Name = "DHForesightSpellcondition";
            const string Guid = "4615c639-95f2-4c04-b904-e79f5b916b68";

            const string TitleString = "Condition/&DHForesightSpellTitle";
            const string DescriptionString = "Condition/&DHForesightSpellDescription";

            protected ForesightConditionBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionBearsEndurance, name, guid)
            {
                Definition.GuiPresentation.Title = TitleString;
                Definition.GuiPresentation.Description = DescriptionString;
                Definition.Features.SetRange
                (
                    DatabaseHelper.FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionBearsEndurance,
                    DatabaseHelper.FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionBullsStrength,
                    DatabaseHelper.FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionCatsGrace,
                    DatabaseHelper.FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionEaglesSplendor,
                    DatabaseHelper.FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionFoxsCunning,
                    DatabaseHelper.FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionOwlsWisdom,
                    DatabaseHelper.FeatureDefinitionCombatAffinitys.CombatAffinityStealthy,
                    DatabaseHelper.FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityShelteringBreeze
                );
            }

            private static ConditionDefinition CreateAndAddToDB(string name, string guid)
                => new ForesightConditionBuilder(name, guid).AddToDB();

            internal static readonly ConditionDefinition ForesightCondition = CreateAndAddToDB(Name, Guid);
        }

        private static SpellDefinition BuildMassHeal()
        {
            string text = "MassHealSpell";

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

            GuiPresentation guiPresentationSpell = new GuiPresentation()
                .SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f))
                .SetDescription(GetSpellDescriptionTerm(text))
                .SetTitle(GetSpellTitleTerm(text))
                .SetSpriteReference(DatabaseHelper.SpellDefinitions.Heal.GuiPresentation.SpriteReference)
                .SetSymbolChar("221E");

            SpellBuilder massHealSpell = new SpellBuilder(DhBaseString + text, GuidHelper.Create(new System.Guid(DhBaseGuid), DhBaseString + text).ToString())
                .SetGuiPresentation(guiPresentationSpell)
                .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(9)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetVerboseComponent(true)
                .SetSomaticComponent(false)
                .SetEffectDescription(effectDescription.Build())
                .SetAiParameters(new SpellAIParameters());

            return massHealSpell.AddToDB();
        }

        private static SpellDefinition BuildMeteorSwarmSingleTarget()
        {
            string text = "MeteorSwarmSingleTargetSpell";

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
                    DatabaseHelper.SmartAttributeDefinitions.Dexterity.name,
                    true,
                    RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                    DatabaseHelper.SmartAttributeDefinitions.Dexterity.name,
                    20,
                    false,
                    new List<SaveAffinityBySenseDescription>())
                .SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.FlameStrike.EffectDescription.EffectParticleParameters);

            GuiPresentation guiPresentationSpell = new GuiPresentation()
                .SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f))
                .SetDescription(GetSpellDescriptionTerm(text))
                .SetTitle(GetSpellTitleTerm(text))
                .SetSpriteReference(DatabaseHelper.SpellDefinitions.FlamingSphere.GuiPresentation.SpriteReference)
                .SetSymbolChar("221E");

            SpellBuilder meteorSwarmSingleTargetSpell = new SpellBuilder(DhBaseString + text, GuidHelper.Create(new System.Guid(DhBaseGuid), DhBaseString + text).ToString())
                .SetGuiPresentation(guiPresentationSpell)
                .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(9)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetVerboseComponent(true)
                .SetSomaticComponent(false)
                .SetEffectDescription(effectDescription.Build())
                .SetAiParameters(new SpellAIParameters());

            return meteorSwarmSingleTargetSpell.AddToDB();
        }

        private static SpellDefinition BuildPowerWordHeal()
        {
            string text = "PowerWordHealSpell";

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
                    DatabaseHelper.ConditionDefinitions.ConditionParalyzed,
                    ConditionForm.ConditionOperation.RemoveDetrimentalAll,
                    false,
                    false,
                    new List<ConditionDefinition>
                    {
                        DatabaseHelper.ConditionDefinitions.ConditionCharmed,
                        DatabaseHelper.ConditionDefinitions.ConditionCharmedByHypnoticPattern,
                        DatabaseHelper.ConditionDefinitions.ConditionFrightened,
                        DatabaseHelper.ConditionDefinitions.ConditionFrightenedFear,
                        DatabaseHelper.ConditionDefinitions.ConditionFrightenedPhantasmalKiller,
                        DatabaseHelper.ConditionDefinitions.ConditionParalyzed,
                        DatabaseHelper.ConditionDefinitions.ConditionParalyzed_CrimsonSpiderVenom,
                        DatabaseHelper.ConditionDefinitions.ConditionParalyzed_GhoulsCaress,
                        DatabaseHelper.ConditionDefinitions.ConditionStunned,
                        DatabaseHelper.ConditionDefinitions.ConditionStunned_MutantApeSlam,
                        DatabaseHelper.ConditionDefinitions.ConditionStunnedConjuredDeath,
                        DatabaseHelper.ConditionDefinitions.ConditionProne
                    })
                .Build());


            GuiPresentation guiPresentationSpell = new GuiPresentation()
                .SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f))
                .SetDescription(GetSpellDescriptionTerm(text))
                .SetTitle(GetSpellTitleTerm(text))
                .SetSpriteReference(DatabaseHelper.SpellDefinitions.HealingWord.GuiPresentation.SpriteReference)
                .SetSymbolChar("221E");

            SpellBuilder powerWordHealSpell = new SpellBuilder(DhBaseString + text, GuidHelper.Create(new System.Guid(DhBaseGuid), DhBaseString + text).ToString())
                .SetGuiPresentation(guiPresentationSpell)
                .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(9)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetVerboseComponent(true)
                .SetSomaticComponent(false)
                .SetEffectDescription(effectDescription.Build())
                .SetAiParameters(new SpellAIParameters());

            return powerWordHealSpell.AddToDB();
        }

        private static SpellDefinition BuildPowerWordKill()
        {
            string text = "PowerWordKillSpell";

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

            GuiPresentation guiPresentationSpell = new GuiPresentation()
                .SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f))
                .SetDescription(GetSpellDescriptionTerm(text))
                .SetTitle(GetSpellTitleTerm(text))
                .SetSpriteReference(DatabaseHelper.SpellDefinitions.Disintegrate.GuiPresentation.SpriteReference)
                .SetSymbolChar("221E");

            SpellBuilder powerWordKillSpell = new SpellBuilder(DhBaseString + text, GuidHelper.Create(new System.Guid(DhBaseGuid), DhBaseString + text).ToString())
                .SetGuiPresentation(guiPresentationSpell)
                .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(9)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetVerboseComponent(true)
                .SetSomaticComponent(false)
                .SetEffectDescription(effectDescription.Build())
                .SetAiParameters(new SpellAIParameters());

            return powerWordKillSpell.AddToDB();
        }

        private static SpellDefinition BuildShapechange()
        {
            string text = "ShapechangeSpell";

            ShapeChangeForm shapeChangeForm = new ShapeChangeForm()
                .SetKeepMentalAbilityScores(true)
                .SetShapeChangeType(ShapeChangeForm.Type.FreeListSelection)
                .SetSpecialSubstituteCondition(DatabaseHelper.ConditionDefinitions.ConditionWildShapeSubstituteForm);

            shapeChangeForm.ShapeOptions.AddRange
            (
                new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(DatabaseHelper.MonsterDefinitions.GoldDragon_AerElai),
                new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(DatabaseHelper.MonsterDefinitions.Divine_Avatar),
                new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(DatabaseHelper.MonsterDefinitions.Sorr_Akkath_Tshar_Boss),
                new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(DatabaseHelper.MonsterDefinitions.GreenDragon_MasterOfConjuration),
                new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(DatabaseHelper.MonsterDefinitions.BlackDragon_MasterOfNecromancy),
                new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(DatabaseHelper.MonsterDefinitions.Remorhaz),
                new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(DatabaseHelper.MonsterDefinitions.Emperor_Laethar),
                new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(DatabaseHelper.MonsterDefinitions.Giant_Ape),
                new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(DatabaseHelper.MonsterDefinitions.Spider_Queen),
                new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(DatabaseHelper.MonsterDefinitions.Sorr_Akkath_Shikkath)
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
                .SetParticleEffectParameters(DatabaseHelper.FeatureDefinitionPowers.PowerDruidWildShape.EffectDescription.EffectParticleParameters);

            GuiPresentation guiPresentationSpell = new GuiPresentation()
                .SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f))
                .SetDescription(GetSpellDescriptionTerm(text))
                .SetTitle(GetSpellTitleTerm(text))
                .SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerDruidWildShape.GuiPresentation.SpriteReference)
                .SetSymbolChar("221E");

            SpellBuilder shapechangeSpell = new SpellBuilder(DhBaseString + text, GuidHelper.Create(new System.Guid(DhBaseGuid), DhBaseString + text).ToString())
                .SetGuiPresentation(guiPresentationSpell)
                .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(9)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetVerboseComponent(true)
                .SetSomaticComponent(false)
                .SetEffectDescription(effectDescription.Build())
                .SetAiParameters(new SpellAIParameters())
                .SetConcentration();

            return shapechangeSpell.AddToDB().SetRequiresConcentration(true);
        }

        private static SpellDefinition BuildTimeStop()
        {
            string text = "TimeStopSpell";

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

            GuiPresentation guiPresentationSpell = new GuiPresentation()
                .SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f))
                .SetDescription(GetSpellDescriptionTerm(text))
                .SetTitle(GetSpellTitleTerm(text))
                .SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerDomainLawWordOfLaw.GuiPresentation.SpriteReference)
                .SetSymbolChar("221E");

            SpellBuilder timeStopSpell = new SpellBuilder(DhBaseString + text, GuidHelper.Create(new System.Guid(DhBaseGuid), DhBaseString + text).ToString())
                .SetGuiPresentation(guiPresentationSpell)
                .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(9)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetVerboseComponent(true)
                .SetSomaticComponent(false)
                .SetEffectDescription(effectDescription.Build())
                .SetAiParameters(new SpellAIParameters());

            return timeStopSpell.AddToDB();
        }

        internal class TimeStopConditionBuilder : BaseDefinitionBuilder<ConditionDefinition>
        {
            const string Name = "DHTimeStopSpellCondition";
            const string Guid = "f00e592f-61c3-4cbf-a800-97596e83028d";

            const string TitleString = "Condition/&DHTimeStopSpellTitle";
            const string DescriptionString = "Condition/&DHTimeStopSpellDescription";

            protected TimeStopConditionBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionIncapacitated, name, guid)
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
                => new TimeStopConditionBuilder(name, guid).AddToDB();

            internal static readonly ConditionDefinition TimeStopCondition = CreateAndAddToDB(Name, Guid);
        }

        private static SpellDefinition BuildWeird()
        {
            string text = "WeirdSpell";

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
                    DatabaseHelper.SmartAttributeDefinitions.Wisdom.name,
                    true,
                    RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                    DatabaseHelper.SmartAttributeDefinitions.Constitution.name,
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

            GuiPresentation guiPresentationSpell = new GuiPresentation()
                .SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f))
                .SetDescription(GetSpellDescriptionTerm(text))
                .SetTitle(GetSpellTitleTerm(text))
                .SetSpriteReference(DatabaseHelper.SpellDefinitions.PhantasmalKiller.GuiPresentation.SpriteReference)
                .SetSymbolChar("221E");

            SpellBuilder weirdSpell = new SpellBuilder(DhBaseString + text, GuidHelper.Create(new System.Guid(DhBaseGuid), DhBaseString + text).ToString())
                .SetGuiPresentation(guiPresentationSpell)
                .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(9)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetVerboseComponent(true)
                .SetSomaticComponent(false)
                .SetEffectDescription(effectDescription.Build())
                .SetAiParameters(new SpellAIParameters())
                .SetConcentration();

            return weirdSpell.AddToDB();
        }

        internal class WeirdConditionBuilder : BaseDefinitionBuilder<ConditionDefinition>
        {
            const string Name = "DHWeirdSpellCondition";
            const string Guid = "0f76e7e1-4490-4ee8-a13f-a4a967ba1c08";

            const string TitleString = "Condition/&DHWeirdSpellTitle";
            const string DescriptionString = "Condition/&DHWeirdSpellDescription";

            protected WeirdConditionBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionFrightenedPhantasmalKiller, name, guid)
            {
                Definition.GuiPresentation.Title = TitleString;
                Definition.GuiPresentation.Description = DescriptionString;

                // weird condition is the same as phantasma killer condition, just for more people
            }
            private static ConditionDefinition CreateAndAddToDB(string name, string guid)
                => new WeirdConditionBuilder(name, guid).AddToDB();

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
                    guiPresentationSpell.SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f));
                    guiPresentationSpell.SetDescription(GetSpellDescriptionTerm(text));
                    guiPresentationSpell.SetTitle(GetSpellTitleTerm(text));
                    guiPresentationSpell.SetSpriteReference(DatabaseHelper.SpellDefinitions.Banishment.GuiPresentation.SpriteReference);
                    guiPresentationSpell.SetSymbolChar("221E");

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
