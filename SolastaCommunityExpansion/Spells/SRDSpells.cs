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
            SpellsContext.RegisterSpell(BuildDivineWord(), isFromOtherMod: false, "SpellListCleric");
            SpellsContext.RegisterSpell(BuildFingerOfDeath(), isFromOtherMod: false, "SpellListWizard", "SpellListSorcerer");
            SpellsContext.RegisterSpell(BuildReverseGravity(), isFromOtherMod: false, "SpellListDruid", "SpellListWizard", "SpellListSorcerer");
            SpellsContext.RegisterSpell(BuildConjureCelestial(), isFromOtherMod: false, "SpellListCleric");

            // 8th level
            SpellsContext.RegisterSpell(BuildDominateMonster(), isFromOtherMod: false, "SpellListWizard", "SpellListSorcerer");
            SpellsContext.RegisterSpell(BuildFeeblemind(), isFromOtherMod: false, "SpellListWizard", "SpellListDruid");
            SpellsContext.RegisterSpell(BuildHolyAura(), isFromOtherMod: false, "SpellListCleric");
            SpellsContext.RegisterSpell(BuildIncendiaryCloud(), isFromOtherMod: false, "SpellListWizard", "SpellListSorcerer");
            SpellsContext.RegisterSpell(BuildMaze(), isFromOtherMod: false, "SpellListWizard");
            SpellsContext.RegisterSpell(BuildMindBlank(), isFromOtherMod: false, "SpellListWizard");
            SpellsContext.RegisterSpell(BuildPowerWordStun(), isFromOtherMod: false, "SpellListWizard", "SpellListSorcerer");
            SpellsContext.RegisterSpell(BuildSunBurst(), isFromOtherMod: false, "SpellListDruid", "SpellListWizard", "SpellListSorcerer");

            // 9th level
            SpellsContext.RegisterSpell(BuildForesight(), isFromOtherMod: false, "SpellListDruid", "SpellListWizard");
            SpellsContext.RegisterSpell(BuildMassHeal(), isFromOtherMod: false, "SpellListCleric");
            SpellsContext.RegisterSpell(BuildMeteorSwarmSingleTarget(), isFromOtherMod: false, "SpellListWizard", "SpellListSorcerer");
            SpellsContext.RegisterSpell(BuildPowerWordHeal(), isFromOtherMod: false, "SpellListCleric");
            SpellsContext.RegisterSpell(BuildPowerWordKill(), isFromOtherMod: false, "SpellListWizard", "SpellListSorcerer");
            SpellsContext.RegisterSpell(BuildTimeStop(), isFromOtherMod: false, "SpellListWizard", "SpellListSorcerer");
            SpellsContext.RegisterSpell(BuildShapechange(), isFromOtherMod: false, "SpellListDruid", "SpellListWizard");
            SpellsContext.RegisterSpell(BuildWeird(), isFromOtherMod: false, "SpellListWizard");
        }

        private static SpellDefinition BuildDivineWord()
        {
            SpellBuilder spellBuilder = new SpellBuilder(
                    "CJDivineWord",
                    "18ecba41-a8ac-4048-979e-2139e66934a7");

            spellBuilder.SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolEvocation);
            spellBuilder.SetMaterialComponent(RuleDefinitions.MaterialComponentType.None);
            spellBuilder.SetCastingTime(RuleDefinitions.ActivationTime.BonusAction);
            spellBuilder.SetSomaticComponent(false);
            spellBuilder.SetVerboseComponent(true);
            spellBuilder.SetSpellLevel(7);
            spellBuilder.SetGuiPresentation(
                    new GuiPresentationBuilder(
                            "Spell/&CJDivineWordDescription",
                            "Spell/&CJDivineWordTitle").Build()
                            .SetSpriteReference(DatabaseHelper.SpellDefinitions.DivineFavor.GuiPresentation.SpriteReference));

            spellBuilder.SetEffectDescription(new EffectDescriptionBuilder()
                .SetTargetingData(RuleDefinitions.Side.All, RuleDefinitions.RangeType.Distance, 6, RuleDefinitions.TargetType.IndividualsUnique, 20, 1, ActionDefinitions.ItemSelectionType.None)
                .SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.MassHealingWord.EffectDescription.EffectParticleParameters)
                .SetDurationData(RuleDefinitions.DurationType.Instantaneous, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn)
                .SetSavingThrowData(true, false, AttributeDefinitions.Charisma, true, RuleDefinitions.EffectDifficultyClassComputation.SpellCastingFeature, AttributeDefinitions.Wisdom, 15, false, new List<SaveAffinityBySenseDescription>())
                .AddEffectForm(new DivineWordEffectForm())
                .Build());

            SpellDefinition spell = spellBuilder.AddToDB();

            return spell;
        }

        private sealed class DivineWordEffectForm : CustomEffectForm
        {
            private readonly List<string> monsterFamilyPlaneshiftList = new List<string>()
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

        private const string BaseGuid = "05c1b1dbae144731b4505c1232fdc37e";
        private const string BaseString = "DH";

        private static string GetSpellTitleTerm(string text) => $"Spell/&{BaseString}{text}_Title";
        private static string GetSpellDescriptionTerm(string text) => $"Spell/&{BaseString}{text}_Description";

        private static SpellDefinition BuildFingerOfDeath()
        {
            string text = "FingerOfDeathSpell";

            EffectDescriptionBuilder effectDescription = new EffectDescriptionBuilder();
            effectDescription.SetDurationData
                (
                RuleDefinitions.DurationType.Instantaneous,
                1,
                RuleDefinitions.TurnOccurenceType.EndOfTurn
                );
            effectDescription.SetTargetingData
                (
                RuleDefinitions.Side.Enemy,
                RuleDefinitions.RangeType.Distance,
                12,
                RuleDefinitions.TargetType.Individuals,
                1,
                1,
                ActionDefinitions.ItemSelectionType.None
                );
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
            effectDescription.AddEffectForm(new EffectFormBuilder()
                .SetDamageForm
                (
                false,
                RuleDefinitions.DieType.D8,
                RuleDefinitions.DamageTypeNecrotic,
                30,
                RuleDefinitions.DieType.D8,
                7,
                RuleDefinitions.HealFromInflictedDamage.Never,
                new List<RuleDefinitions.TrendInfo>())
                .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.HalfDamage)
                .Build()
                );
            //  effectDescription.AddEffectForm(
            //  new EffectFormBuilder().SetConditionForm(
            //      SummonZombieCondition_Builder.SummonZombieCondition,
            //      ConditionForm.ConditionOperation.Add,
            //      true,
            //      true,
            //      new List<ConditionDefinition>()
            //      )
            //    //  .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.Negates)
            //  .Build());
            effectDescription.SetSavingThrowData
                (
                true,
                false,
                DatabaseHelper.SmartAttributeDefinitions.Constitution.name,
                true,
                RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                DatabaseHelper.SmartAttributeDefinitions.Constitution.name,
                20,
                false,
                new List<SaveAffinityBySenseDescription> { }
                );
            effectDescription.Build();

            GuiPresentation guiPresentationSpell = new GuiPresentation()
                .SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f))
                .SetDescription(GetSpellDescriptionTerm(text))
                .SetTitle(GetSpellTitleTerm(text))
                .SetSpriteReference(DatabaseHelper.SpellDefinitions.RayOfEnfeeblement.GuiPresentation.SpriteReference)
                .SetSymbolChar("221E");

            SpellBuilder FingerOfDeathSpell = new SpellBuilder(BaseString + text, GuidHelper.Create(new System.Guid(BaseGuid), BaseString + text).ToString())
                .SetGuiPresentation(guiPresentationSpell)
                .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(7)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetVerboseComponent(true)
                .SetSomaticComponent(false)
                .SetEffectDescription(effectDescription.Build())
                .SetAiParameters(new SpellAIParameters());

            return FingerOfDeathSpell.AddToDB();
        }

        //   internal class SummonZombieCondition_Builder : BaseDefinitionBuilder<ConditionDefinition>
        //   {
        //       const string Name = "DHSummonZombieSpellcondition";
        //       const string Guid = "a4d94551-efd3-4987-918c-a35b24d607a6";
        //       const string TitleString = "Condition/&DHSummonZombieSpell_Title";
        //       const string DescriptionString = "Condition/&DHSummonZombieSpell_Description";
        //       protected SummonZombieCondition_Builder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionDummy, name, guid)
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
        //           => new SummonZombieCondition_Builder(name, guid).AddToDB();
        //       internal static ConditionDefinition SummonZombieCondition = CreateAndAddToDB(Name, Guid);
        //   }

        private static SpellDefinition BuildReverseGravity()
        {
            string text = "ReverseGravitySpell";

            EffectDescriptionBuilder effectDescription = new EffectDescriptionBuilder();
            effectDescription.SetDurationData(
                 RuleDefinitions.DurationType.Minute,
                 1,
                 RuleDefinitions.TurnOccurenceType.EndOfTurn);
            effectDescription.SetTargetingData(
                RuleDefinitions.Side.All,
                RuleDefinitions.RangeType.Distance,
                12,
                RuleDefinitions.TargetType.Cylinder,
                10,
                10,
                ActionDefinitions.ItemSelectionType.None);

            effectDescription.SetSavingThrowData(
                true,
                false,
                DatabaseHelper.SmartAttributeDefinitions.Dexterity.name,
                true,
                RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                DatabaseHelper.SmartAttributeDefinitions.Dexterity.name,
                20,
                false,
                new List<SaveAffinityBySenseDescription> { }
                );
            effectDescription.AddEffectForm(
                new EffectFormBuilder().SetConditionForm(
                    ReverseGravityCondition_Builder.ReverseGravityCondition,
                    // DatabaseHelper.ConditionDefinitions.ConditionLevitate,
                    ConditionForm.ConditionOperation.Add,
                    false,
                    false,
                    new List<ConditionDefinition>()
                    )
                    .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.Negates)
                .Build());
            effectDescription.AddEffectForm(
                new EffectFormBuilder().SetMotionForm(
                    MotionForm.MotionType.Levitate,
                    10
                    )
                //     .CreatedByCharacter()
                .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.Negates)
                .Build());

            effectDescription.SetRecurrentEffect(DatabaseHelper.SpellDefinitions.Entangle.EffectDescription.RecurrentEffect);
            effectDescription.Build();


            GuiPresentation guiPresentationSpell = new GuiPresentation()
                .SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f))
                .SetDescription(GetSpellDescriptionTerm(text))
                .SetTitle(GetSpellTitleTerm(text))
                .SetSpriteReference(DatabaseHelper.SpellDefinitions.Thunderwave.GuiPresentation.SpriteReference)
                .SetSymbolChar("221E");

            SpellBuilder ReverseGravitySpell = new SpellBuilder(BaseString + text, GuidHelper.Create(new System.Guid(BaseGuid), BaseString + text).ToString())
                .SetGuiPresentation(guiPresentationSpell)
                .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(7)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetVerboseComponent(true)
                .SetSomaticComponent(true)
                .SetEffectDescription(effectDescription.Build())
                .SetAiParameters(new SpellAIParameters())
                .SetConcentration();

            return ReverseGravitySpell.AddToDB();
        }

        internal class ReverseGravityCondition_Builder : BaseDefinitionBuilder<ConditionDefinition>
        {
            const string Name = "DHReverseGravitySpellcondition";
            const string Guid = "809f1cef-6bdc-4b5a-93bf-275af8ab0b36";

            const string TitleString = "Condition/&DHReverseGravitySpell_Title";
            const string DescriptionString = "Condition/&DHReverseGravitySpell_Description";

            protected ReverseGravityCondition_Builder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionLevitate, name, guid)
            {
                Definition.GuiPresentation.Title = TitleString;
                Definition.GuiPresentation.Description = DescriptionString;

                Definition.SetConditionType(RuleDefinitions.ConditionType.Neutral);

                Definition.ConditionTags.Clear();
                Definition.ConditionTags.Empty();

                Definition.Features.Clear();
                Definition.Features.Empty();
                Definition.Features.AddRange
                (
                    DatabaseHelper.FeatureDefinitionMovementAffinitys.MovementAffinityConditionLevitate,
                    DatabaseHelper.FeatureDefinitionMoveModes.MoveModeFly2
                );
            }
            internal static ConditionDefinition CreateAndAddToDB(string name, string guid)
                => new ReverseGravityCondition_Builder(name, guid).AddToDB();

            internal static readonly ConditionDefinition ReverseGravityCondition = CreateAndAddToDB(Name, Guid);
        }

        private static SpellDefinition BuildConjureCelestial()
        {
            SpellListDefinition CouatlSpellList = SpellListBuilder.CreateSpellList(
                 BaseString + "CouatlSpellList",
                 GuidHelper.Create(new System.Guid(BaseGuid), BaseString + "CouatlSpellList").ToString(),
                 "",
                 DatabaseHelper.SpellListDefinitions.SpellListCleric,
                 new List<SpellDefinition>
                 {
                 },
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
                 new List<SpellDefinition>
                 {
                 },
                 new List<SpellDefinition>
                 {
                 },
                 new List<SpellDefinition>
                 {
                     DatabaseHelper.SpellDefinitions.GreaterRestoration
                 }
                 );

            CouatlSpellList.SetHasCantrips(false);
            CouatlSpellList.SetMaxSpellLevel(5);

            FeatureDefinitionCastSpell CastSpellCouatl = new CastSpellBuilder
                (
                BaseString + "CastSpellCouatl",
                GuidHelper.Create(new System.Guid(BaseGuid), BaseString + "CastSpellCouatl").ToString()
                 )
                .AddToDB();

            CastSpellCouatl.SetGuiPresentation(new GuiPresentationBuilder("Feature/&NoContentTitle", "Feature/&NoContentTitle").Build());
            CastSpellCouatl.SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Monster);
            CastSpellCouatl.SetSpellcastingAbility(AttributeDefinitions.Charisma);
            CastSpellCouatl.SetSpellcastingParametersComputation(RuleDefinitions.SpellcastingParametersComputation.Static);
            CastSpellCouatl.SetStaticDCValue(14);
            CastSpellCouatl.SetStaticToHitValue(8);
            CastSpellCouatl.SetSpellListDefinition(CouatlSpellList);
            CastSpellCouatl.RestrictedSchools.Clear();
            CastSpellCouatl.SetSpellKnowledge(RuleDefinitions.SpellKnowledge.FixedList);
            CastSpellCouatl.SetSlotsRecharge(RuleDefinitions.RechargeRate.ShortRest);
            CastSpellCouatl.SetSpellCastingLevel(9);
            CastSpellCouatl.SetSpellReadyness(RuleDefinitions.SpellReadyness.AllKnown);
            CastSpellCouatl.SetFocusType(EquipmentDefinitions.FocusType.None);

            int[] arr = { 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20 };

            CastSpellCouatl.KnownSpells.Clear();
            CastSpellCouatl.KnownSpells.AddRange(arr);

            CastSpellCouatl.SlotsPerLevels.Clear();
            CastSpellCouatl.SlotsPerLevels.AddRange(new List<FeatureDefinitionCastSpell.SlotsByLevelDuplet>()
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

            //string MonsterName = "Couatl";
            MonsterDefinition BaseTemplateName = DatabaseHelper.MonsterDefinitions.KindredSpiritViper;
            MonsterDefinition MonsterShaderReference = DatabaseHelper.MonsterDefinitions.KindredSpiritViper;
            string NewName = "Custom_Couatl";
            string NewTitle = "Custom_Couatl_Title";
            string NewDescription = "Custom_Couatl_Description";
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
                CastSpellCouatl
            };

            List<MonsterSkillProficiency> SkillScores = new List<MonsterSkillProficiency>();

            /*waiting until MonsterAttackDefinitionBuilder is available to use

                        MonsterAttackDefinition CouatlBite_Attack = MonsterAttackDefinitionBuilder(
                                 "DH_Custom_" + text,
                                 DatabaseHelper.MonsterAttackDefinitions.Attack_TigerDrake_Bite,
                                 GuidHelper.Create(new System.Guid(BaseGuid), BaseString + "CouatlBite_Attack").ToString(),
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

            MonsterBuilder CouatlBuilder = new MonsterBuilder(
                  NewName,
                  "fc38ed74-3e2f-4960-86cc-3120d638410b",
                  "Monster/&" + "DH_" + NewTitle,
                  "Monster/&" + "DH_" + NewDescription,
                  BaseTemplateName);

            CouatlBuilder.SetInDungeonEditor(false);
            CouatlBuilder.SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.None);
            CouatlBuilder.SetSizeDefinition(Size);
            CouatlBuilder.SetChallengeRating(CR);
            CouatlBuilder.SetAlignment(Alignment);
            CouatlBuilder.SetCharacterFamily(Type);
            CouatlBuilder.SetLegendaryCreature(LegendaryCreature);
            CouatlBuilder.SetArmorClass(ArmorClass);
            CouatlBuilder.SetHitDiceNumber(HitDice);
            CouatlBuilder.SetHitDiceType(HitDiceType);
            CouatlBuilder.SetHitPointsBonus(HitPointsBonus);
            CouatlBuilder.SetStandardHitPoints(StandardHitPoints);
            CouatlBuilder.ClearFeatures();
            CouatlBuilder.AddFeatures(Features);
            CouatlBuilder.ClearAbilityScores();
            CouatlBuilder.SetAbilityScores(
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

            CouatlBuilder.ClearSavingThrowScores();
            CouatlBuilder.AddSavingThrowScores(new List<MonsterSavingThrowProficiency>()
            {
                StrSave,
                DexSave,
                ConSave,
                IntSave,
                WisSave,
                ChaSave
            });

            CouatlBuilder.ClearSkillScores();
            CouatlBuilder.AddSkillScores(SkillScores);
            CouatlBuilder.ClearAttackIterations();
            CouatlBuilder.AddAttackIterations(AttackIterations);
            //  CouatlBuilder.SetDefaultBattleDecisionPackage(DefaultBattleDecisionPackage);
            CouatlBuilder.SetGroupAttacks(GroupAttacks);
            CouatlBuilder.ClearLegendaryActionOptions();
            CouatlBuilder.AddLegendaryActionOptions(LegendaryActionOptions);
            CouatlBuilder.SetSpriteReference(SpriteReference);
            CouatlBuilder.SetHasPhantomDistortion(PhantomDistortion);
            CouatlBuilder.SetAttachedParticlesReference(AttachedParticlesReference);
            CouatlBuilder.SetNoExperienceGain(false);
            CouatlBuilder.SetHasMonsterPortraitBackground(true);
            CouatlBuilder.SetCanGeneratePortrait(true);
            CouatlBuilder.SetCustomShaderReference(MonsterShaderReference);

            MonsterDefinition Couatl = CouatlBuilder.AddToDB();

            Couatl.CreatureTags.Clear();

            EffectDescriptionBuilder effectDescription = new EffectDescriptionBuilder();
            effectDescription.SetDurationData(RuleDefinitions.DurationType.Minute, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            effectDescription.SetTargetingData(RuleDefinitions.Side.All, RuleDefinitions.RangeType.Distance, 18, RuleDefinitions.TargetType.Position, 1, 1, 0);
            effectDescription.AddEffectForm(new EffectFormBuilder().SetSummonForm(
                SummonForm.Type.Creature,
                DatabaseHelper.ItemDefinitions.Dagger,
                1,
                NewName,
                DatabaseHelper.ConditionDefinitions.ConditionMindDominatedByCaster,
                false,
                DatabaseHelper.DecisionPackageDefinitions.IdleGuard_Default,
                DatabaseHelper.EffectProxyDefinitions.ProxyDancingLights
                ).Build()
                );
            effectDescription.SetCreatedByCharacter();
            effectDescription.SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.ConjureElementalAir.EffectDescription.EffectParticleParameters);
            effectDescription.Build();

            GuiPresentation guiPresentationSpell = new GuiPresentation();
            guiPresentationSpell.SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f));
            guiPresentationSpell.SetDescription(GetSpellDescriptionTerm(text));
            guiPresentationSpell.SetTitle(GetSpellTitleTerm(text));
            guiPresentationSpell.SetSpriteReference(DatabaseHelper.SpellDefinitions.Bless.GuiPresentation.SpriteReference);
            guiPresentationSpell.SetSymbolChar("221E");

            SpellBuilder ConjureCelestialSpell = new SpellBuilder(BaseString + text, GuidHelper.Create(new System.Guid(BaseGuid), BaseString + text).ToString());
            ConjureCelestialSpell.SetGuiPresentation(guiPresentationSpell);
            ConjureCelestialSpell.SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation);
            ConjureCelestialSpell.SetSpellLevel(7);
            ConjureCelestialSpell.SetCastingTime(RuleDefinitions.ActivationTime.Action);
            ConjureCelestialSpell.SetVerboseComponent(true);
            ConjureCelestialSpell.SetSomaticComponent(true);
            ConjureCelestialSpell.SetEffectDescription(effectDescription.Build());
            ConjureCelestialSpell.SetAiParameters(new SpellAIParameters());
            ConjureCelestialSpell.SetConcentration();
            SpellDefinition ConjureCelestial = ConjureCelestialSpell.AddToDB(true);
            SpellDefinitionExtensions.SetRequiresConcentration<SpellDefinition>(ConjureCelestial, true);

            return ConjureCelestial;
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

                    SpellBuilder AnimalShapesSpell = new SpellBuilder(BaseString + text, GuidHelper.Create(new System.Guid(BaseGuid), BaseString + text).ToString());
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

            SpellBuilder DominateMonsterSpell = new SpellBuilder(BaseString + text, GuidHelper.Create(new System.Guid(BaseGuid), BaseString + text).ToString())
                .SetGuiPresentation(guiPresentationSpell)
                .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(8)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetVerboseComponent(true)
                .SetSomaticComponent(true)
                .SetEffectDescription(effectDescription)
                .SetAiParameters(new SpellAIParameters());

            return DominateMonsterSpell.AddToDB();
        }

        private static SpellDefinition BuildFeeblemind()
        {
            string text = "FeeblemindSpell";

            EffectDescriptionBuilder effectDescription = new EffectDescriptionBuilder();
            effectDescription.SetDurationData(
                RuleDefinitions.DurationType.Dispelled,
                1,
                RuleDefinitions.TurnOccurenceType.EndOfTurn);
            effectDescription.SetTargetingData(
                RuleDefinitions.Side.Enemy,
                RuleDefinitions.RangeType.Distance,
                30,
                RuleDefinitions.TargetType.Individuals,
                1,
                1,
                ActionDefinitions.ItemSelectionType.None);
            effectDescription.SetSavingThrowData(
                true,
                false,
                DatabaseHelper.SmartAttributeDefinitions.Intelligence.name,
                true,
                RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                DatabaseHelper.SmartAttributeDefinitions.Intelligence.name,
                20,
                false,
                new List<SaveAffinityBySenseDescription> { }
                );
            effectDescription.AddEffectForm(
                new EffectFormBuilder().SetConditionForm(
                    FeeblemindCondition_Builder.FeeblemindCondition,
                    ConditionForm.ConditionOperation.Add,
                    false,
                    false,
                    new List<ConditionDefinition>()
                    )
                .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.Negates)
                .Build());
            effectDescription.AddEffectForm(
            new EffectFormBuilder().SetDamageForm(
                false,
                RuleDefinitions.DieType.D6,
                RuleDefinitions.DamageTypePsychic,
                0,
                RuleDefinitions.DieType.D6,
                4,
                RuleDefinitions.HealFromInflictedDamage.Never,
                new List<RuleDefinitions.TrendInfo>()
                ).Build());
            effectDescription.Build();

            GuiPresentation guiPresentationSpell = new GuiPresentation()
            .SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f))
            .SetDescription(GetSpellDescriptionTerm(text))
            .SetTitle(GetSpellTitleTerm(text))
            .SetSpriteReference(DatabaseHelper.SpellDefinitions.BestowCurse.GuiPresentation.SpriteReference)
            .SetSymbolChar("221E");

            SpellBuilder FeeblemindSpell = new SpellBuilder(BaseString + text, GuidHelper.Create(new System.Guid(BaseGuid), BaseString + text).ToString())
            .SetGuiPresentation(guiPresentationSpell)
            .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(8)
            .SetCastingTime(RuleDefinitions.ActivationTime.Action)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetEffectDescription(effectDescription.Build())
            .SetAiParameters(new SpellAIParameters());

            DatabaseHelper.SpellDefinitions.GreaterRestoration.EffectDescription.EffectForms[0].ConditionForm.ConditionsList.Add(FeeblemindCondition_Builder.FeeblemindCondition);

            return FeeblemindSpell.AddToDB();
        }

        internal class FeeblemindCondition_Builder : BaseDefinitionBuilder<ConditionDefinition>
        {
            const string Name = "DHFeeblemindSpellcondition";
            const string Guid = "965a09b2-cb22-452b-b93c-2bccdcda4871";

            const string TitleString = "Condition/&DHFeeblemindSpell_Title";
            const string DescriptionString = "Condition/&DHFeeblemindSpell_Description";

            protected FeeblemindCondition_Builder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionBearsEndurance, name, guid)
            {
                Definition.GuiPresentation.Title = TitleString;
                Definition.GuiPresentation.Description = DescriptionString;

                Definition.SetConditionType(RuleDefinitions.ConditionType.Detrimental);

                Definition.Features.Clear();
                Definition.Features.Empty();
                Definition.Features.AddRange
                (
                    DatabaseHelper.FeatureDefinitionMagicAffinitys.MagicAffinitySilenced,
                    DatabaseHelper.FeatureDefinitionMagicAffinitys.MagicAffinityConditionRaging,
                    FeeblemindIntAttributeModifier_Builder.FeeblemindIntAttributeModifier,
                    Feeblemind_Cha_AttributeModifier_Builder.Feeblemind_Cha_AttributeModifier,
                    FeeblemindActionAffinity_Builder.FeeblemindActionAffinity
                );
            }

            internal static ConditionDefinition CreateAndAddToDB(string name, string guid)
                => new FeeblemindCondition_Builder(name, guid).AddToDB();

            internal static readonly ConditionDefinition FeeblemindCondition = CreateAndAddToDB(Name, Guid);
        }

        internal class FeeblemindIntAttributeModifier_Builder : BaseDefinitionBuilder<FeatureDefinitionAttributeModifier>
        {
            const string Name = "DHFeeblemind_IntSpellAttributeModifier";
            const string Guid = "a2a16bda-e7b1-4a87-9f0e-3e4c21870fd8";

            const string TitleString = "AttributeModifier/&DHFeeblemind_IntSpell_Title";
            const string DescriptionString = "AttributeModifier/&DHFeeblemind_IntSpell_Description";

            protected FeeblemindIntAttributeModifier_Builder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierHeadbandOfIntellect, name, guid)
            {
                Definition.GuiPresentation.Title = TitleString;
                Definition.GuiPresentation.Description = DescriptionString;

                Definition.SetModifiedAttribute(DatabaseHelper.SmartAttributeDefinitions.Intelligence.name);
                Definition.SetModifierAbilityScore(DatabaseHelper.SmartAttributeDefinitions.Intelligence.name);
                Definition.SetModifierType2(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Force);
                Definition.SetModifierValue(1);
                Definition.SetSituationalContext(RuleDefinitions.SituationalContext.None);

            }
            internal static FeatureDefinitionAttributeModifier CreateAndAddToDB(string name, string guid)
                => new FeeblemindIntAttributeModifier_Builder(name, guid).AddToDB();

            internal static readonly FeatureDefinitionAttributeModifier FeeblemindIntAttributeModifier = CreateAndAddToDB(Name, Guid);
        }

        internal class Feeblemind_Cha_AttributeModifier_Builder : BaseDefinitionBuilder<FeatureDefinitionAttributeModifier>
        {
            const string Name = "DHFeeblemind_ChaSpellAttributeModifier";
            const string Guid = "6721abe1-19eb-4a8c-9702-2fdea2070464";

            const string TitleString = "AttributeModifier/&DHFeeblemind_ChaSpell_Title";
            const string DescriptionString = "AttributeModifier/&DHFeeblemind_ChaSpell_Description";

            protected Feeblemind_Cha_AttributeModifier_Builder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierHeadbandOfIntellect, name, guid)
            {
                Definition.GuiPresentation.Title = TitleString;
                Definition.GuiPresentation.Description = DescriptionString;

                Definition.SetModifiedAttribute(DatabaseHelper.SmartAttributeDefinitions.Charisma.name);
                Definition.SetModifierAbilityScore(DatabaseHelper.SmartAttributeDefinitions.Charisma.name);
                Definition.SetModifierType2(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Force);
                Definition.SetModifierValue(1);
                Definition.SetSituationalContext(RuleDefinitions.SituationalContext.None);

            }

            internal static FeatureDefinitionAttributeModifier CreateAndAddToDB(string name, string guid)
                => new Feeblemind_Cha_AttributeModifier_Builder(name, guid).AddToDB();

            internal static readonly FeatureDefinitionAttributeModifier Feeblemind_Cha_AttributeModifier = CreateAndAddToDB(Name, Guid);
        }

        internal class FeeblemindActionAffinity_Builder : BaseDefinitionBuilder<FeatureDefinitionActionAffinity>
        {
            const string Name = "DHFeeblemindSpellActionAffinity";
            const string Guid = "749a9572-07f6-4678-9458-904c04b9ab22";

            const string TitleString = "ActionAffinity/&DHFeeblemindSpell_Title";
            const string DescriptionString = "ActionAffinity/&DHFeeblemindSpell_Description";

            protected FeeblemindActionAffinity_Builder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionActionAffinitys.ActionAffinityConditionRaging, name, guid)
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

            internal static FeatureDefinitionActionAffinity CreateAndAddToDB(string name, string guid)
                => new FeeblemindActionAffinity_Builder(name, guid).AddToDB();

            internal static readonly FeatureDefinitionActionAffinity FeeblemindActionAffinity = CreateAndAddToDB(Name, Guid);
        }

        private static SpellDefinition BuildHolyAura()
        {
            string text = "HolyAuraSpell";

            EffectDescriptionBuilder effectDescription = new EffectDescriptionBuilder();
            effectDescription.SetDurationData(
                RuleDefinitions.DurationType.Minute,
                1,
                RuleDefinitions.TurnOccurenceType.EndOfTurn);
            effectDescription.SetTargetingData(
                RuleDefinitions.Side.Ally,
                RuleDefinitions.RangeType.Self,
                12,
                RuleDefinitions.TargetType.Sphere,
                6,
                6,
                ActionDefinitions.ItemSelectionType.None);
            effectDescription.AddEffectForm(
                new EffectFormBuilder().SetConditionForm(
                    HolyAuraCondition_Builder.HolyAuraCondition,
                    ConditionForm.ConditionOperation.Add,
                    false,
                    false,
                    new List<ConditionDefinition>()
                    )
                .Build());

            effectDescription.SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.BeaconOfHope.EffectDescription.EffectParticleParameters);
            effectDescription.Build();

            GuiPresentation guiPresentationSpell = new GuiPresentation()
                .SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f))
                .SetDescription(GetSpellDescriptionTerm(text))
                .SetTitle(GetSpellTitleTerm(text))
                .SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerDomainLifePreserveLife.GuiPresentation.SpriteReference)
                .SetSymbolChar("221E");

            SpellBuilder HolyAuraSpell = new SpellBuilder(BaseString + text, GuidHelper.Create(new System.Guid(BaseGuid), BaseString + text).ToString())
                .SetGuiPresentation(guiPresentationSpell)
                .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(8)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetVerboseComponent(true)
                .SetSomaticComponent(false)
                .SetEffectDescription(effectDescription.Build())
                .SetAiParameters(new SpellAIParameters())
                .SetConcentration();

            return HolyAuraSpell.AddToDB();
        }


        internal class HolyAuraCondition_Builder : BaseDefinitionBuilder<ConditionDefinition>
        {
            const string Name = "DHHolyAuraSpellcondition";
            const string Guid = "1808ca4b-8f46-41bf-a59c-0bcbd4f60248";

            const string TitleString = "Condition/&DHHolyAuraSpell_Title";
            const string DescriptionString = "Condition/&DHHolyAuraSpell_Description";

            protected HolyAuraCondition_Builder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionBearsEndurance, name, guid)
            {
                Definition.GuiPresentation.Title = TitleString;
                Definition.GuiPresentation.Description = DescriptionString;

                Definition.SetConditionType(RuleDefinitions.ConditionType.Beneficial);

                Definition.Features.Clear();
                Definition.Features.Empty();
                Definition.Features.AddRange
                (
                    DatabaseHelper.FeatureDefinitionCombatAffinitys.CombatAffinityDodging,
                    DatabaseHelper.FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityShelteringBreeze,
                    HolyAuraDamageAffinity_Builder.HolyAuraDamageAffinity
                );
            }
            internal static ConditionDefinition CreateAndAddToDB(string name, string guid)
                => new HolyAuraCondition_Builder(name, guid).AddToDB();

            internal static readonly ConditionDefinition HolyAuraCondition = CreateAndAddToDB(Name, Guid);
        }

        internal class HolyAuraDamageAffinity_Builder : BaseDefinitionBuilder<FeatureDefinitionDamageAffinity>
        {
            const string Name = "DHHolyAuraSpellDamageAffinity";
            const string Guid = "c83aceae-e4c4-4a9c-a83d-58ffebe92007";

            const string TitleString = "DamageAffinity/&DHHolyAuraSpell_Title";
            const string DescriptionString = "DamageAffinity/&DHHolyAuraSpell_Description";

            protected HolyAuraDamageAffinity_Builder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPoisonAdvantage, name, guid)
            {
                Definition.GuiPresentation.Title = TitleString;
                Definition.GuiPresentation.Description = DescriptionString;
                Definition.SetDamageAffinityType(RuleDefinitions.DamageAffinityType.None);
                Definition.SetSavingThrowAdvantageType(RuleDefinitions.AdvantageType.None);
                Definition.SetKnockOutAffinity(RuleDefinitions.KnockoutAffinity.None);
                Definition.SetRetaliateWhenHit(true);
                Definition.SetRetaliateProximity(RuleDefinitions.AttackProximity.Melee);
                Definition.SetRetaliateRangeCells(1);
                Definition.SetRetaliatePower(HolyAuraBlindingPower_Builder.HolyAuraBlindingPower);
            }

            internal static FeatureDefinitionDamageAffinity CreateAndAddToDB(string name, string guid)
                => new HolyAuraDamageAffinity_Builder(name, guid).AddToDB();

            internal static readonly FeatureDefinitionDamageAffinity HolyAuraDamageAffinity = CreateAndAddToDB(Name, Guid);
        }

        internal class HolyAuraBlindingPower_Builder : BaseDefinitionBuilder<FeatureDefinitionPower>
        {
            const string Name = "DHHolyAuraSpellBlindingPower";
            const string Guid = "40366ca2-00a0-471a-b370-8c81f6283ce1";

            const string TitleString = "Feature/&DHHolyAura_BlindingPower_Title";
            const string DescriptionString = "Feataure/&DHHolyAura_BlindingPower_Description";

            protected HolyAuraBlindingPower_Builder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionPowers.PowerOathOfMotherlandFieryPresence, name, guid)
            {
                Definition.GuiPresentation.Title = TitleString;
                Definition.GuiPresentation.Description = DescriptionString;

                EffectDescriptionBuilder effectDescription = new EffectDescriptionBuilder();
                effectDescription.SetDurationData(
                RuleDefinitions.DurationType.Minute,
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
                effectDescription.AddEffectForm(
                    new EffectFormBuilder().SetConditionForm
                        (
                            DatabaseHelper.ConditionDefinitions.ConditionBlinded,
                            ConditionForm.ConditionOperation.Add,
                            false,
                            false,
                            new List<ConditionDefinition>()
                        )
                        .Build()
                    );
                effectDescription.AddRestrictedCreatureFamilies(DatabaseHelper.CharacterFamilyDefinitions.Fiend);
                effectDescription.AddRestrictedCreatureFamilies(DatabaseHelper.CharacterFamilyDefinitions.Undead);

                Definition.SetEffectDescription(effectDescription.Build());
            }
            internal static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
                => new HolyAuraBlindingPower_Builder(name, guid).AddToDB();

            internal static readonly FeatureDefinitionPower HolyAuraBlindingPower = CreateAndAddToDB(Name, Guid);
        }

        private static SpellDefinition BuildIncendiaryCloud()
        {

            string text = "IncendiaryCloudSpell";

            EffectDescriptionBuilder effectDescription = new EffectDescriptionBuilder();
            effectDescription.SetDurationData(
                RuleDefinitions.DurationType.Minute,
                1,
                RuleDefinitions.TurnOccurenceType.EndOfTurn);
            effectDescription.SetTargetingData(
                RuleDefinitions.Side.All,
                RuleDefinitions.RangeType.Distance,
                30,
                RuleDefinitions.TargetType.Sphere,
                4,
                4,
                ActionDefinitions.ItemSelectionType.None);
            effectDescription.AddEffectForm(
              new EffectFormBuilder()
              .SetSummonForm
              (
                  SummonForm.Type.EffectProxy,
                  DatabaseHelper.ItemDefinitions.Dart,
                  1,
                  DatabaseHelper.MonsterDefinitions.Adam_The_Twelth.name,
                  null,
                  false,
                  DatabaseHelper.DecisionPackageDefinitions.IdleGuard_Default,
                  DatabaseHelper.EffectProxyDefinitions.ProxyFogCloud
              )
              .Build()
             );
            effectDescription.AddEffectForm
                (
                new EffectFormBuilder()
                    .SetDamageForm
                    (
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
                    .Build()
                );
            effectDescription.SetRecurrentEffect(DatabaseHelper.SpellDefinitions.WallOfFireLine.EffectDescription.RecurrentEffect);
            effectDescription.SetVelocity(2, RuleDefinitions.VelocityType.AwayFromSourceOriginalPosition);
            // effect description builder doesnt have can be dispersed
            // effectDescription.SetCanBeDispersed(true);
            effectDescription.SetSavingThrowData(
                    true,
                    false,
                    DatabaseHelper.SmartAttributeDefinitions.Dexterity.name,
                    false,
                    RuleDefinitions.EffectDifficultyClassComputation.FixedValue,
                    DatabaseHelper.SmartAttributeDefinitions.Dexterity.name,
                    20,
                    false,
                    new List<SaveAffinityBySenseDescription> { }
                    );

            effectDescription.Build();

            EffectDescription neweffectDescription = new EffectDescription();
            neweffectDescription.Copy(DatabaseHelper.SpellDefinitions.FogCloud.EffectDescription);
            neweffectDescription.EffectForms.Add(new EffectFormBuilder()
                    .SetDamageForm
                    (
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

            SpellBuilder IncendiaryCloudSpell = new SpellBuilder(BaseString + text, GuidHelper.Create(new System.Guid(BaseGuid), BaseString + text).ToString())
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

            EffectDescriptionBuilder effectDescription = new EffectDescriptionBuilder();
            effectDescription.SetDurationData(
                RuleDefinitions.DurationType.Minute,
                10,
                RuleDefinitions.TurnOccurenceType.EndOfTurn
                );
            effectDescription.SetTargetingData(
                RuleDefinitions.Side.Enemy,
                RuleDefinitions.RangeType.Distance,
                12,
                RuleDefinitions.TargetType.Individuals,
                1,
                1,
                ActionDefinitions.ItemSelectionType.None
                );
            effectDescription.SetSavingThrowData(
                true,
                false,
                DatabaseHelper.SmartAttributeDefinitions.Intelligence.name,
                false,
                RuleDefinitions.EffectDifficultyClassComputation.FixedValue,
                DatabaseHelper.SmartAttributeDefinitions.Intelligence.name,
                20,
                false,
                new List<SaveAffinityBySenseDescription> { }
                );
            effectDescription.AddEffectForm(
                new EffectFormBuilder().SetConditionForm(
                    DatabaseHelper.ConditionDefinitions.ConditionBanished,
                    ConditionForm.ConditionOperation.Add,
                    false,
                    false,
                    new List<ConditionDefinition>()
                    )
                .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.Negates)
                .CanSaveToCancel(RuleDefinitions.TurnOccurenceType.EndOfTurn)
                .Build()
                );

            effectDescription.Build();

            GuiPresentation guiPresentationSpell = new GuiPresentation()
                .SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f))
                .SetDescription(GetSpellDescriptionTerm(text))
                .SetTitle(GetSpellTitleTerm(text))
                .SetSpriteReference(DatabaseHelper.SpellDefinitions.IdentifyCreatures.GuiPresentation.SpriteReference)
                .SetSymbolChar("221E");

            SpellBuilder MazeSpell = new SpellBuilder(BaseString + text, GuidHelper.Create(new System.Guid(BaseGuid), BaseString + text).ToString())
                .SetGuiPresentation(guiPresentationSpell)
                .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(8)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetVerboseComponent(true)
                .SetSomaticComponent(false)
                .SetEffectDescription(effectDescription.Build())
                .SetAiParameters(new SpellAIParameters());

            MazeSpell.SetConcentration();

            return MazeSpell.AddToDB(); ;
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
                    MindBlankCondition_Builder.MindBlankCondition,
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

            SpellBuilder MindBlankSpell = new SpellBuilder(BaseString + text, GuidHelper.Create(new System.Guid(BaseGuid), BaseString + text).ToString())
            .SetGuiPresentation(guiPresentationSpell)
            .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(8)
            .SetCastingTime(RuleDefinitions.ActivationTime.Action)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetEffectDescription(effectDescription.Build())
            .SetAiParameters(new SpellAIParameters());

            return MindBlankSpell.AddToDB();

        }

        internal class MindBlankCondition_Builder : BaseDefinitionBuilder<ConditionDefinition>
        {
            const string Name = "DHMindBlankSpellcondition";
            const string Guid = "74f77a4c-b5cb-45d6-ac6d-d9fa2ebe3869";

            const string TitleString = "Condition/&DHMindBlankSpell_Title";
            const string DescriptionString = "Condition/&DHMindBlankSpell_Description";

            protected MindBlankCondition_Builder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionBearsEndurance, name, guid)
            {
                Definition.GuiPresentation.Title = TitleString;
                Definition.GuiPresentation.Description = DescriptionString;

                Definition.Features.Clear();
                Definition.Features.Empty();
                Definition.Features.AddRange
                    (
                    DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityCharmImmunity,
                    DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityCharmImmunityHypnoticPattern,
                    DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityCalmEmotionCharmedImmunity,
                    DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPsychicImmunity
                    );
            }
            internal static ConditionDefinition CreateAndAddToDB(string name, string guid)
                => new MindBlankCondition_Builder(name, guid).AddToDB();

            internal static readonly ConditionDefinition MindBlankCondition = CreateAndAddToDB(Name, Guid);
        }

        private static SpellDefinition BuildPowerWordStun()
        {

            string text = "PowerWordStunSpell";

            ConditionForm conditionForm = new ConditionForm();
            conditionForm.SetApplyToSelf(false);
            conditionForm.SetForceOnSelf(false);
            conditionForm.Operation = ConditionForm.ConditionOperation.Add;
            conditionForm.SetConditionDefinitionName(DatabaseHelper.ConditionDefinitions.ConditionStunned.Name);
            conditionForm.ConditionDefinition = DatabaseHelper.ConditionDefinitions.ConditionStunned;

            EffectForm effectForm = new EffectForm();
            effectForm.SetApplyLevel(EffectForm.LevelApplianceType.No);
            effectForm.SetLevelMultiplier(1);
            effectForm.SetLevelType(RuleDefinitions.LevelSourceType.ClassLevel);
            effectForm.SetCreatedByCharacter(true);
            effectForm.FormType = EffectForm.EffectFormType.Condition;
            effectForm.SetConditionForm(conditionForm);
            effectForm.SetCanSaveToCancel(true);
            effectForm.SetSaveOccurence(RuleDefinitions.TurnOccurenceType.EndOfTurn);

            EffectDescriptionBuilder effectDescription = new EffectDescriptionBuilder();
            effectDescription.SetDurationData(
                RuleDefinitions.DurationType.Dispelled,
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
            effectDescription.AddEffectForm(effectForm);
            effectDescription.SetTargetFiltering(
                RuleDefinitions.TargetFilteringMethod.CharacterIncreasingHitPointsFromPool,
                RuleDefinitions.TargetFilteringTag.No,
                150,
                RuleDefinitions.DieType.D1
                );
            effectDescription.SetSavingThrowData(
                true,
                false,
                DatabaseHelper.SmartAttributeDefinitions.Constitution.name,
                false,
                RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                DatabaseHelper.SmartAttributeDefinitions.Constitution.name,
                15,
                false,
                new List<SaveAffinityBySenseDescription> { }
                );
            effectDescription.Build();


            GuiPresentation guiPresentationSpell = new GuiPresentation();
            guiPresentationSpell.SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f));
            guiPresentationSpell.SetDescription(GetSpellDescriptionTerm(text));
            guiPresentationSpell.SetTitle(GetSpellTitleTerm(text));
            guiPresentationSpell.SetSpriteReference(DatabaseHelper.SpellDefinitions.Contagion.GuiPresentation.SpriteReference);
            guiPresentationSpell.SetSymbolChar("221E");

            SpellBuilder PowerWordStunSpell = new SpellBuilder(BaseString + text, GuidHelper.Create(new System.Guid(BaseGuid), BaseString + text).ToString());
            PowerWordStunSpell.SetGuiPresentation(guiPresentationSpell);
            PowerWordStunSpell.SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation);
            PowerWordStunSpell.SetSpellLevel(8);
            PowerWordStunSpell.SetCastingTime(RuleDefinitions.ActivationTime.Action);
            PowerWordStunSpell.SetVerboseComponent(true);
            PowerWordStunSpell.SetSomaticComponent(false);
            PowerWordStunSpell.SetEffectDescription(effectDescription.Build());
            PowerWordStunSpell.SetAiParameters(new SpellAIParameters());

            return PowerWordStunSpell.AddToDB();
        }


        private static SpellDefinition BuildSunBurst()
        {

            string text = "SunBurstSpell";

            EffectDescriptionBuilder effectDescription = new EffectDescriptionBuilder();
            effectDescription.SetDurationData(
                RuleDefinitions.DurationType.Minute,
                1,
                RuleDefinitions.TurnOccurenceType.EndOfTurn);
            effectDescription.SetTargetingData(
                RuleDefinitions.Side.All,
                RuleDefinitions.RangeType.Distance,
                30,
                RuleDefinitions.TargetType.Sphere,
                12,
                1,
                ActionDefinitions.ItemSelectionType.None);
            effectDescription.SetSavingThrowData(
                true,
                false,
                DatabaseHelper.SmartAttributeDefinitions.Constitution.name,
                false,
                RuleDefinitions.EffectDifficultyClassComputation.FixedValue,
                DatabaseHelper.SmartAttributeDefinitions.Constitution.name,
                20,
                false,
                new List<SaveAffinityBySenseDescription> { }
                );
            effectDescription.AddEffectForm
                (
                new EffectFormBuilder()
                    .SetDamageForm
                    (
                        false,
                        RuleDefinitions.DieType.D6,
                        RuleDefinitions.DamageTypeRadiant,
                        0,
                        RuleDefinitions.DieType.D6,
                        12,
                        RuleDefinitions.HealFromInflictedDamage.Never,
                        new List<RuleDefinitions.TrendInfo>()
                    )
                    .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.HalfDamage)

                   .Build());
            effectDescription.AddEffectForm(
              new EffectFormBuilder().SetConditionForm
                    (
                        DatabaseHelper.ConditionDefinitions.ConditionBlinded,
                        ConditionForm.ConditionOperation.Add,
                        false,
                        false,
                        new List<ConditionDefinition>()
                    )
                    .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.Negates)
                    .CanSaveToCancel(RuleDefinitions.TurnOccurenceType.EndOfTurn)
                    .Build()
                );


            EffectDescription effect = effectDescription.Build();

            effect.EffectParticleParameters.SetImpactParticleReference(new AssetReference("96018e15e8eba4b40a9a5bd637d7ae91"));

            SaveAffinityByFamilyDescription SaveAffinityByFamilyDescription = new SaveAffinityByFamilyDescription();
            SaveAffinityByFamilyDescription.SetAdvantageType(RuleDefinitions.AdvantageType.Disadvantage);
            SaveAffinityByFamilyDescription.SetFamily(DatabaseHelper.CharacterFamilyDefinitions.Ooze.name);
            SaveAffinityByFamilyDescription SaveAffinityByFamilyDescription_Undead = new SaveAffinityByFamilyDescription();
            SaveAffinityByFamilyDescription_Undead.SetAdvantageType(RuleDefinitions.AdvantageType.Disadvantage);
            SaveAffinityByFamilyDescription_Undead.SetFamily(DatabaseHelper.CharacterFamilyDefinitions.Undead.name);

            effect.SavingThrowAffinitiesByFamily.AddRange(SaveAffinityByFamilyDescription, SaveAffinityByFamilyDescription_Undead);

            GuiPresentation guiPresentationSpell = new GuiPresentation();
            guiPresentationSpell.SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f));
            guiPresentationSpell.SetDescription(GetSpellDescriptionTerm(text));
            guiPresentationSpell.SetTitle(GetSpellTitleTerm(text));
            guiPresentationSpell.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerDomainSunIndomitableLight.GuiPresentation.SpriteReference);
            guiPresentationSpell.SetSymbolChar("221E");

            SpellBuilder SunBurstSpell = new SpellBuilder(BaseString + text, GuidHelper.Create(new System.Guid(BaseGuid), BaseString + text).ToString());
            SunBurstSpell.SetGuiPresentation(guiPresentationSpell);
            SunBurstSpell.SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation);
            SunBurstSpell.SetSpellLevel(8);
            SunBurstSpell.SetCastingTime(RuleDefinitions.ActivationTime.Action);
            SunBurstSpell.SetVerboseComponent(true);
            SunBurstSpell.SetSomaticComponent(false);
            SunBurstSpell.SetEffectDescription(effect);
            SunBurstSpell.SetAiParameters(new SpellAIParameters());

            return SunBurstSpell.AddToDB();
        }

        private static SpellDefinition BuildForesight()
        {
            string text = "ForesightSpell";

            EffectDescriptionBuilder effectDescription = new EffectDescriptionBuilder();
            effectDescription.SetDurationData(
                RuleDefinitions.DurationType.Hour,
                8,
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
                    ForesightCondition_Builder.ForesightCondition,
                    ConditionForm.ConditionOperation.Add,
                    false,
                    false,
                    new List<ConditionDefinition>()
                    )
                .Build()
                );

            effectDescription.Build();

            GuiPresentation guiPresentationSpell = new GuiPresentation();
            guiPresentationSpell.SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f));
            guiPresentationSpell.SetDescription(GetSpellDescriptionTerm(text));
            guiPresentationSpell.SetTitle(GetSpellTitleTerm(text));
            guiPresentationSpell.SetSpriteReference(DatabaseHelper.SpellDefinitions.TrueSeeing.GuiPresentation.SpriteReference);
            guiPresentationSpell.SetSymbolChar("221E");

            SpellBuilder ForesightSpell = new SpellBuilder(BaseString + text, GuidHelper.Create(new System.Guid(BaseGuid), BaseString + text).ToString());
            ForesightSpell.SetGuiPresentation(guiPresentationSpell);
            ForesightSpell.SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation);
            ForesightSpell.SetSpellLevel(9);
            ForesightSpell.SetCastingTime(RuleDefinitions.ActivationTime.Minute1);
            ForesightSpell.SetVerboseComponent(true);
            ForesightSpell.SetSomaticComponent(false);
            ForesightSpell.SetEffectDescription(effectDescription.Build());
            ForesightSpell.SetAiParameters(new SpellAIParameters());

            return ForesightSpell.AddToDB();
        }

        internal class ForesightCondition_Builder : BaseDefinitionBuilder<ConditionDefinition>
        {
            const string Name = "DHForesightSpellcondition";
            const string Guid = "4615c639-95f2-4c04-b904-e79f5b916b68";

            const string TitleString = "Condition/&DHForesightSpell_Title";
            const string DescriptionString = "Condition/&DHForesightSpell_Description";

            protected ForesightCondition_Builder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionBearsEndurance, name, guid)
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

            internal static ConditionDefinition CreateAndAddToDB(string name, string guid)
                => new ForesightCondition_Builder(name, guid).AddToDB();

            internal static readonly ConditionDefinition ForesightCondition = CreateAndAddToDB(Name, Guid);
        }

        private static SpellDefinition BuildMassHeal()
        {

            string text = "MassHealSpell";

            EffectDescriptionBuilder effectDescription = new EffectDescriptionBuilder();
            effectDescription.SetDurationData(
                RuleDefinitions.DurationType.Instantaneous,
                1,
                RuleDefinitions.TurnOccurenceType.EndOfTurn);
            effectDescription.SetTargetingData(
                RuleDefinitions.Side.All,
                RuleDefinitions.RangeType.Distance,
                12,
                RuleDefinitions.TargetType.Individuals,
                14,
                1,
                ActionDefinitions.ItemSelectionType.None);
            effectDescription.AddEffectForm(new EffectFormBuilder().SetHealingForm(
                RuleDefinitions.HealingComputation.Dice,
                50,
                RuleDefinitions.DieType.D1,
                0,
                false,
                RuleDefinitions.HealingCap.MaximumHitPoints)
                .Build());
            effectDescription.Build();


            GuiPresentation guiPresentationSpell = new GuiPresentation();
            guiPresentationSpell.SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f));
            guiPresentationSpell.SetDescription(GetSpellDescriptionTerm(text));
            guiPresentationSpell.SetTitle(GetSpellTitleTerm(text));
            guiPresentationSpell.SetSpriteReference(DatabaseHelper.SpellDefinitions.Heal.GuiPresentation.SpriteReference);
            guiPresentationSpell.SetSymbolChar("221E");

            SpellBuilder MassHealSpell = new SpellBuilder(BaseString + text, GuidHelper.Create(new System.Guid(BaseGuid), BaseString + text).ToString());
            MassHealSpell.SetGuiPresentation(guiPresentationSpell);
            MassHealSpell.SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation);
            MassHealSpell.SetSpellLevel(9);
            MassHealSpell.SetCastingTime(RuleDefinitions.ActivationTime.Action);
            MassHealSpell.SetVerboseComponent(true);
            MassHealSpell.SetSomaticComponent(false);
            MassHealSpell.SetEffectDescription(effectDescription.Build());
            MassHealSpell.SetAiParameters(new SpellAIParameters());

            return MassHealSpell.AddToDB();
        }


        private static SpellDefinition BuildMeteorSwarmSingleTarget()
        {
            string text = "MeteorSwarmSingleTargetSpell";

            EffectDescriptionBuilder effectDescription = new EffectDescriptionBuilder();
            effectDescription.SetDurationData(
                RuleDefinitions.DurationType.Instantaneous,
                1,
                RuleDefinitions.TurnOccurenceType.EndOfTurn);
            effectDescription.SetTargetingData(
                RuleDefinitions.Side.All,
                RuleDefinitions.RangeType.Distance,
                200,
                RuleDefinitions.TargetType.Sphere,
                8,
                8,
                ActionDefinitions.ItemSelectionType.None);
            effectDescription.AddEffectForm(
                 new EffectFormBuilder().SetDamageForm(
                     false,
                     RuleDefinitions.DieType.D6,
                     RuleDefinitions.DamageTypeFire,
                     0,
                     RuleDefinitions.DieType.D6,
                     20,                                          // 20 because hits dont stack even on single target
                     RuleDefinitions.HealFromInflictedDamage.Never,
                     new List<RuleDefinitions.TrendInfo>()
                     )
                 .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.HalfDamage)
                 .Build());
            effectDescription.AddEffectForm(
                 new EffectFormBuilder().SetDamageForm(
                     false,
                     RuleDefinitions.DieType.D6,
                     RuleDefinitions.DamageTypeBludgeoning,
                     0,
                     RuleDefinitions.DieType.D6,
                     20,                                          // 20 because hits dont stack even on single target
                     RuleDefinitions.HealFromInflictedDamage.Never,
                     new List<RuleDefinitions.TrendInfo>()
                     )
                 .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.HalfDamage)
                 .Build());
            effectDescription.SetSavingThrowData(
                true,
                false,
                DatabaseHelper.SmartAttributeDefinitions.Dexterity.name,
                true,
                RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                DatabaseHelper.SmartAttributeDefinitions.Dexterity.name,
                20,
                false,
                new List<SaveAffinityBySenseDescription> { }
                );
            effectDescription.SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.FlameStrike.EffectDescription.EffectParticleParameters);
            effectDescription.Build();

            GuiPresentation guiPresentationSpell = new GuiPresentation();
            guiPresentationSpell.SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f));
            guiPresentationSpell.SetDescription(GetSpellDescriptionTerm(text));
            guiPresentationSpell.SetTitle(GetSpellTitleTerm(text));
            guiPresentationSpell.SetSpriteReference(DatabaseHelper.SpellDefinitions.FlamingSphere.GuiPresentation.SpriteReference);
            guiPresentationSpell.SetSymbolChar("221E");

            SpellBuilder MeteorSwarmSingleTargetSpell = new SpellBuilder(BaseString + text, GuidHelper.Create(new System.Guid(BaseGuid), BaseString + text).ToString());
            MeteorSwarmSingleTargetSpell.SetGuiPresentation(guiPresentationSpell);
            MeteorSwarmSingleTargetSpell.SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation);
            MeteorSwarmSingleTargetSpell.SetSpellLevel(9);
            MeteorSwarmSingleTargetSpell.SetCastingTime(RuleDefinitions.ActivationTime.Action);
            MeteorSwarmSingleTargetSpell.SetVerboseComponent(true);
            MeteorSwarmSingleTargetSpell.SetSomaticComponent(false);
            MeteorSwarmSingleTargetSpell.SetEffectDescription(effectDescription.Build());
            MeteorSwarmSingleTargetSpell.SetAiParameters(new SpellAIParameters());

            return MeteorSwarmSingleTargetSpell.AddToDB();
        }

        private static SpellDefinition BuildPowerWordHeal()
        {

            string text = "PowerWordHealSpell";

            EffectDescriptionBuilder effectDescription = new EffectDescriptionBuilder();
            effectDescription.SetDurationData(
                RuleDefinitions.DurationType.Instantaneous,
                1,
                RuleDefinitions.TurnOccurenceType.EndOfTurn);
            effectDescription.SetTargetingData(
                RuleDefinitions.Side.Ally,
                RuleDefinitions.RangeType.Distance,
                12,
                RuleDefinitions.TargetType.Individuals,
                1,
                1,
                ActionDefinitions.ItemSelectionType.None);
            effectDescription.AddEffectForm(new EffectFormBuilder().SetHealingForm(
                RuleDefinitions.HealingComputation.Dice,
                700,
                RuleDefinitions.DieType.D1,
                0,
                false,
                RuleDefinitions.HealingCap.MaximumHitPoints)
                .Build());
            effectDescription.AddEffectForm(new EffectFormBuilder().SetConditionForm(
                DatabaseHelper.ConditionDefinitions.ConditionParalyzed,
                ConditionForm.ConditionOperation.RemoveDetrimentalAll,
                false,
                false,
                new List<ConditionDefinition>()
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
                    }
                ).Build());
            effectDescription.Build();

            GuiPresentation guiPresentationSpell = new GuiPresentation();
            guiPresentationSpell.SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f));
            guiPresentationSpell.SetDescription(GetSpellDescriptionTerm(text));
            guiPresentationSpell.SetTitle(GetSpellTitleTerm(text));
            guiPresentationSpell.SetSpriteReference(DatabaseHelper.SpellDefinitions.HealingWord.GuiPresentation.SpriteReference);
            guiPresentationSpell.SetSymbolChar("221E");

            SpellBuilder PowerWordHealSpell = new SpellBuilder(BaseString + text, GuidHelper.Create(new System.Guid(BaseGuid), BaseString + text).ToString());
            PowerWordHealSpell.SetGuiPresentation(guiPresentationSpell);
            PowerWordHealSpell.SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation);
            PowerWordHealSpell.SetSpellLevel(9);
            PowerWordHealSpell.SetCastingTime(RuleDefinitions.ActivationTime.Action);
            PowerWordHealSpell.SetVerboseComponent(true);
            PowerWordHealSpell.SetSomaticComponent(false);
            PowerWordHealSpell.SetEffectDescription(effectDescription.Build());
            PowerWordHealSpell.SetAiParameters(new SpellAIParameters());

            return PowerWordHealSpell.AddToDB();
        }

        private static SpellDefinition BuildPowerWordKill()
        {

            string text = "PowerWordKillSpell";

            KillForm killForm = new KillForm();
            killForm.SetKillCondition(RuleDefinitions.KillCondition.UnderHitPoints);
            killForm.SetHitPoints(100);

            EffectForm effectForm = new EffectForm();
            effectForm.SetApplyLevel(EffectForm.LevelApplianceType.No);
            effectForm.SetLevelMultiplier(1);
            effectForm.SetLevelType(RuleDefinitions.LevelSourceType.ClassLevel);
            effectForm.SetCreatedByCharacter(true);
            effectForm.FormType = EffectForm.EffectFormType.Kill;
            effectForm.SetKillForm(killForm);


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
            effectDescription.AddEffectForm(effectForm);
            effectDescription.Build();


            GuiPresentation guiPresentationSpell = new GuiPresentation();
            guiPresentationSpell.SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f));
            guiPresentationSpell.SetDescription(GetSpellDescriptionTerm(text));
            guiPresentationSpell.SetTitle(GetSpellTitleTerm(text));
            guiPresentationSpell.SetSpriteReference(DatabaseHelper.SpellDefinitions.Disintegrate.GuiPresentation.SpriteReference);
            guiPresentationSpell.SetSymbolChar("221E");

            SpellBuilder PowerWordKillSpell = new SpellBuilder(BaseString + text, GuidHelper.Create(new System.Guid(BaseGuid), BaseString + text).ToString());
            PowerWordKillSpell.SetGuiPresentation(guiPresentationSpell);
            PowerWordKillSpell.SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation);
            PowerWordKillSpell.SetSpellLevel(9);
            PowerWordKillSpell.SetCastingTime(RuleDefinitions.ActivationTime.Action);
            PowerWordKillSpell.SetVerboseComponent(true);
            PowerWordKillSpell.SetSomaticComponent(false);
            PowerWordKillSpell.SetEffectDescription(effectDescription.Build());
            PowerWordKillSpell.SetAiParameters(new SpellAIParameters());

            return PowerWordKillSpell.AddToDB();
        }

        private static SpellDefinition BuildShapechange()
        {

            string text = "ShapechangeSpell";

            // GoldDragon_AerElai
            // Divine_Avatar
            // Sorr-Akkath_Tshar_Boss
            // GreenDragon_MasterOfConjuration
            // BlackDragon_MasterOfNecromancy
            // Remorhaz
            // -
            // Emperor_Laethar
            // Giant_Ape
            // Spider_Queen
            // Sorr-Akkath_Shikkath

            ShapeChangeForm shapeChangeForm = new ShapeChangeForm();
            shapeChangeForm.SetKeepMentalAbilityScores(true);
            shapeChangeForm.SetShapeChangeType(ShapeChangeForm.Type.FreeListSelection);
            shapeChangeForm.SetSpecialSubstituteCondition(DatabaseHelper.ConditionDefinitions.ConditionWildShapeSubstituteForm);
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
                1,
                RuleDefinitions.TurnOccurenceType.EndOfTurn);
            effectDescription.SetTargetingData(
                RuleDefinitions.Side.Ally,
                RuleDefinitions.RangeType.Distance,
                12,
                RuleDefinitions.TargetType.Self,
                1,
                1,
                ActionDefinitions.ItemSelectionType.None);
            effectDescription.AddEffectForm(effectForm);
            effectDescription.SetCreatedByCharacter();
            effectDescription.SetParticleEffectParameters(DatabaseHelper.FeatureDefinitionPowers.PowerDruidWildShape.EffectDescription.EffectParticleParameters);
            effectDescription.Build();

            GuiPresentation guiPresentationSpell = new GuiPresentation();
            guiPresentationSpell.SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f));
            guiPresentationSpell.SetDescription(GetSpellDescriptionTerm(text));
            guiPresentationSpell.SetTitle(GetSpellTitleTerm(text));
            guiPresentationSpell.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerDruidWildShape.GuiPresentation.SpriteReference);
            guiPresentationSpell.SetSymbolChar("221E");

            SpellBuilder ShapechangeSpell = new SpellBuilder(BaseString + text, GuidHelper.Create(new System.Guid(BaseGuid), BaseString + text).ToString());
            ShapechangeSpell.SetGuiPresentation(guiPresentationSpell);
            ShapechangeSpell.SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation);
            ShapechangeSpell.SetSpellLevel(9);
            ShapechangeSpell.SetCastingTime(RuleDefinitions.ActivationTime.Action);
            ShapechangeSpell.SetVerboseComponent(true);
            ShapechangeSpell.SetSomaticComponent(false);
            ShapechangeSpell.SetEffectDescription(effectDescription.Build());
            ShapechangeSpell.SetAiParameters(new SpellAIParameters());
            //    ShapechangeSpell
            ShapechangeSpell.SetConcentration();
            SpellDefinition Shapechange = ShapechangeSpell.AddToDB();
            Shapechange.SetRequiresConcentration(true);

            return Shapechange;
        }

        private static SpellDefinition BuildTimeStop()
        {

            string text = "TimeStopSpell";

            ConditionDefinition TimeStopped_Condition = TimeStopCondition_Builder.TimeStopCondition;

            EffectDescriptionBuilder effectDescription = new EffectDescriptionBuilder();
            effectDescription.SetDurationData(
                RuleDefinitions.DurationType.Round,
                3,
                RuleDefinitions.TurnOccurenceType.EndOfTurn);
            effectDescription.SetTargetingData(
                RuleDefinitions.Side.All,
                RuleDefinitions.RangeType.Self,
                0,
                RuleDefinitions.TargetType.Cylinder,
                20,
                10,
                ActionDefinitions.ItemSelectionType.None);
            effectDescription.AddEffectForm(
                new EffectFormBuilder().SetConditionForm(
                    TimeStopped_Condition,
                    ConditionForm.ConditionOperation.Add,
                    false,
                    false,
                    new List<ConditionDefinition>()).Build()
                    );
            effectDescription.ExcludeCaster();
            effectDescription.Build();

            GuiPresentation guiPresentationSpell = new GuiPresentation();
            guiPresentationSpell.SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f));
            guiPresentationSpell.SetDescription(GetSpellDescriptionTerm(text));
            guiPresentationSpell.SetTitle(GetSpellTitleTerm(text));
            guiPresentationSpell.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerDomainLawWordOfLaw.GuiPresentation.SpriteReference);
            guiPresentationSpell.SetSymbolChar("221E");

            SpellBuilder TimeStopSpell = new SpellBuilder(BaseString + text, GuidHelper.Create(new System.Guid(BaseGuid), BaseString + text).ToString());
            TimeStopSpell.SetGuiPresentation(guiPresentationSpell);
            TimeStopSpell.SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation);
            TimeStopSpell.SetSpellLevel(9);
            TimeStopSpell.SetCastingTime(RuleDefinitions.ActivationTime.Action);
            TimeStopSpell.SetVerboseComponent(true);
            TimeStopSpell.SetSomaticComponent(false);
            TimeStopSpell.SetEffectDescription(effectDescription.Build());
            TimeStopSpell.SetAiParameters(new SpellAIParameters());

            return TimeStopSpell.AddToDB();
        }

        internal class TimeStopCondition_Builder : BaseDefinitionBuilder<ConditionDefinition>
        {
            const string Name = "DHTimeStopSpellcondition";
            const string Guid = "f00e592f-61c3-4cbf-a800-97596e83028d";

            const string TitleString = "Condition/&DHTimeStopSpell_Title";
            const string DescriptionString = "Condition/&DHTimeStopSpell_Description";

            protected TimeStopCondition_Builder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionIncapacitated, name, guid)
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

            internal static ConditionDefinition CreateAndAddToDB(string name, string guid)
                => new TimeStopCondition_Builder(name, guid).AddToDB();

            internal static readonly ConditionDefinition TimeStopCondition = CreateAndAddToDB(Name, Guid);
        }

        private static SpellDefinition BuildWeird()
        {

            string text = "WeirdSpell";

            EffectDescriptionBuilder effectDescription = new EffectDescriptionBuilder();
            effectDescription.SetDurationData(
                RuleDefinitions.DurationType.Minute,
                1,
                RuleDefinitions.TurnOccurenceType.EndOfTurn
                );
            effectDescription.SetTargetingData(
                RuleDefinitions.Side.Enemy,
                RuleDefinitions.RangeType.Distance,
                12,
                RuleDefinitions.TargetType.Sphere,
                6,
                6,
                ActionDefinitions.ItemSelectionType.None
                );
            effectDescription.SetSavingThrowData
                (
                true,
                false,
                DatabaseHelper.SmartAttributeDefinitions.Wisdom.name,
                true,
                RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                DatabaseHelper.SmartAttributeDefinitions.Constitution.name,
                20,
                false,
                new List<SaveAffinityBySenseDescription> { }
                );
            effectDescription.AddEffectForm(
                new EffectFormBuilder().SetConditionForm(
                    //DatabaseHelper.ConditionDefinitions.ConditionFrightenedPhantasmalKiller,
                    WeirdCondition_Builder.WeirdCondition,
                    ConditionForm.ConditionOperation.Add,
                    false,
                    false,
                    new List<ConditionDefinition>()
                    )
                    .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.Negates)
                    .CanSaveToCancel(RuleDefinitions.TurnOccurenceType.EndOfTurn)
                    .Build()
                );

            effectDescription.Build();

            GuiPresentation guiPresentationSpell = new GuiPresentation();
            guiPresentationSpell.SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f));
            guiPresentationSpell.SetDescription(GetSpellDescriptionTerm(text));
            guiPresentationSpell.SetTitle(GetSpellTitleTerm(text));
            guiPresentationSpell.SetSpriteReference(DatabaseHelper.SpellDefinitions.PhantasmalKiller.GuiPresentation.SpriteReference);
            guiPresentationSpell.SetSymbolChar("221E");

            SpellBuilder WeirdSpell = new SpellBuilder(BaseString + text, GuidHelper.Create(new System.Guid(BaseGuid), BaseString + text).ToString());
            WeirdSpell.SetGuiPresentation(guiPresentationSpell);
            WeirdSpell.SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation);
            WeirdSpell.SetSpellLevel(9);
            WeirdSpell.SetCastingTime(RuleDefinitions.ActivationTime.Action);
            WeirdSpell.SetVerboseComponent(true);
            WeirdSpell.SetSomaticComponent(false);
            WeirdSpell.SetEffectDescription(effectDescription.Build());
            WeirdSpell.SetAiParameters(new SpellAIParameters());
            WeirdSpell.SetConcentration();

            return WeirdSpell.AddToDB();
        }




        internal class WeirdCondition_Builder : BaseDefinitionBuilder<ConditionDefinition>
        {
            const string Name = "DHWeirdSpellcondition";
            const string Guid = "0f76e7e1-4490-4ee8-a13f-a4a967ba1c08";

            const string TitleString = "Condition/&DHWeirdSpell_Title";
            const string DescriptionString = "Condition/&DHWeirdSpell_Description";

            protected WeirdCondition_Builder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionFrightenedPhantasmalKiller, name, guid)
            {
                Definition.GuiPresentation.Title = TitleString;
                Definition.GuiPresentation.Description = DescriptionString;

                // weird condition is the same as phantasma killer condition , just for more people
            }
            internal static ConditionDefinition CreateAndAddToDB(string name, string guid)
                => new WeirdCondition_Builder(name, guid).AddToDB();

            internal static readonly ConditionDefinition WeirdCondition = CreateAndAddToDB(Name, Guid);
        }

        /*
                private static SpellDefinition Wish_overall()
                {

                    string text = "Wish_overallSpell";



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

                    SpellBuilder Wish_overallSpell = new SpellBuilder(BaseString + text, GuidHelper.Create(new System.Guid(BaseGuid), BaseString + text).ToString());
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
