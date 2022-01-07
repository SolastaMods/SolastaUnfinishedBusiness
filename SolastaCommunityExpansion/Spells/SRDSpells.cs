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
        public static void Load()
        {

            // 7th Level
            SpellsContext.RegisterSpell(BuildDivineWord(), isFromOtherMod: false, "SpellListCleric");

            SpellsContext.RegisterSpell(BuildNew_FingerOfDeath_Spell(), isFromOtherMod: false, "SpellListWizard", "SpellListSorcerer");
            SpellsContext.RegisterSpell(BuildNew_ReverseGravity_Spell(), isFromOtherMod: false, "SpellListDruid", "SpellListWizard", "SpellListSorcerer");
            SpellsContext.RegisterSpell(BuildNew_ConjureCelestial_Spell(), isFromOtherMod: false, "SpellListCleric");

            // 8th lvl
            SpellsContext.RegisterSpell(BuildNew_DominateMonster_Spell(), isFromOtherMod: false, "SpellListWizard", "SpellListSorcerer");
            SpellsContext.RegisterSpell(BuildNew_Feeblemind_Spell(), isFromOtherMod: false, "SpellListWizard", "SpellListDruid");
            SpellsContext.RegisterSpell(BuildNew_HolyAura_Spell(), isFromOtherMod: false, "SpellListCleric");
            SpellsContext.RegisterSpell(BuildNew_IncendiaryCloud_Spell(), isFromOtherMod: false, "SpellListWizard", "SpellListSorcerer");
            SpellsContext.RegisterSpell(BuildNew_Maze_Spell(), isFromOtherMod: false, "SpellListWizard");
            SpellsContext.RegisterSpell(BuildNew_MindBlank_Spell(), isFromOtherMod: false, "SpellListWizard");
            SpellsContext.RegisterSpell(BuildNew_PowerWordStun_Spell(), isFromOtherMod: false, "SpellListWizard", "SpellListSorcerer");
            SpellsContext.RegisterSpell(BuildNew_SunBurst_Spell(), isFromOtherMod: false, "SpellListDruid", "SpellListWizard", "SpellListSorcerer");

            //9th Level
            SpellsContext.RegisterSpell(BuildNew_Foresight_Spell(), isFromOtherMod: false, "SpellListDruid", "SpellListWizard");
            SpellsContext.RegisterSpell(BuildNew_MassHeal_Spell(), isFromOtherMod: false, "SpellListCleric");
            SpellsContext.RegisterSpell(BuildNew_MeteorSwarm_SingleTarget_Spell(), isFromOtherMod: false, "SpellListWizard", "SpellListSorcerer");
            SpellsContext.RegisterSpell(BuildNew_PowerWordHeal_Spell(), isFromOtherMod: false, "SpellListCleric");
            SpellsContext.RegisterSpell(BuildNew_PowerWordKill_Spell(), isFromOtherMod: false, "SpellListWizard", "SpellListSorcerer");
            SpellsContext.RegisterSpell(BuildNew_TimeStop_Spell(), isFromOtherMod: false, "SpellListWizard", "SpellListSorcerer");
            SpellsContext.RegisterSpell(BuildNew_Shapechange_Spell(), isFromOtherMod: false, "SpellListDruid", "SpellListWizard");
            SpellsContext.RegisterSpell(BuildNew_Weird_Spell(), isFromOtherMod: false, "SpellListWizard");

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

        static public string BaseGuid = "05c1b1dbae144731b4505c1232fdc37e";
        static public string BaseString = "DH_Level20_";



        private static SpellDefinition BuildNew_FingerOfDeath_Spell()
        {

            string text = "FingerOfDeath_Spell";

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


            GuiPresentation guiPresentation_Spell = new GuiPresentation();
            guiPresentation_Spell.SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f));
            guiPresentation_Spell.SetDescription("Spell/&" + BaseString + text + "_Description");
            guiPresentation_Spell.SetTitle("Spell/&" + BaseString + text + "_Title");
            guiPresentation_Spell.SetSpriteReference(DatabaseHelper.SpellDefinitions.RayOfEnfeeblement.GuiPresentation.SpriteReference);
            guiPresentation_Spell.SetSymbolChar("221E");

            SpellBuilder FingerOfDeath_Spell = new SpellBuilder(BaseString + text, GuidHelper.Create(new System.Guid(BaseGuid), BaseString + text).ToString());
            FingerOfDeath_Spell.SetGuiPresentation(guiPresentation_Spell);
            FingerOfDeath_Spell.SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation);
            FingerOfDeath_Spell.SetSpellLevel(7);
            FingerOfDeath_Spell.SetCastingTime(RuleDefinitions.ActivationTime.Action);
            FingerOfDeath_Spell.SetVerboseComponent(true);
            FingerOfDeath_Spell.SetSomaticComponent(false);
            FingerOfDeath_Spell.SetEffectDescription(effectDescription.Build());
            FingerOfDeath_Spell.SetAiParameters(new SpellAIParameters());
            //    FingerOfDeath_Spell
            SpellDefinition FingerOfDeath = FingerOfDeath_Spell.AddToDB();

            return FingerOfDeath;

        }

        //   internal class SummonZombieCondition_Builder : BaseDefinitionBuilder<ConditionDefinition>
        //   {
        //       const string name = "DH_Level20_SummonZombie_Spellcondition";
        //       const string guid = "a4d94551-efd3-4987-918c-a35b24d607a6";
        //       string title_string = "Condition/&DH_Level20_SummonZombie_Spell_Title";
        //       string description_string = "Condition/&DH_Level20_SummonZombie_Spell_Description";
        //       protected SummonZombieCondition_Builder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionDummy, name, guid)
        //       {
        //           Definition.GuiPresentation.Title = title_string;
        //           Definition.GuiPresentation.Description = description_string;
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
        //       public static ConditionDefinition CreateAndAddToDB(string name, string guid)
        //           => new SummonZombieCondition_Builder(name, guid).AddToDB();
        //       public static ConditionDefinition SummonZombieCondition = CreateAndAddToDB(name, guid);
        //   }

        private static SpellDefinition BuildNew_ReverseGravity_Spell()
        {


            string text = "ReverseGravity_Spell";

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


            GuiPresentation guiPresentation_Spell = new GuiPresentation();
            guiPresentation_Spell.SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f));
            guiPresentation_Spell.SetDescription("Spell/&" + BaseString + text + "_Description");
            guiPresentation_Spell.SetTitle("Spell/&" + BaseString + text + "_Title");
            guiPresentation_Spell.SetSpriteReference(DatabaseHelper.SpellDefinitions.Thunderwave.GuiPresentation.SpriteReference);
            guiPresentation_Spell.SetSymbolChar("221E");

            SpellBuilder ReverseGravity_Spell = new SpellBuilder(BaseString + text, GuidHelper.Create(new System.Guid(BaseGuid), BaseString + text).ToString());

            ReverseGravity_Spell.SetGuiPresentation(guiPresentation_Spell);
            ReverseGravity_Spell.SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation);
            ReverseGravity_Spell.SetSpellLevel(7);
            ReverseGravity_Spell.SetCastingTime(RuleDefinitions.ActivationTime.Action);
            ReverseGravity_Spell.SetVerboseComponent(true);
            ReverseGravity_Spell.SetSomaticComponent(true);
            ReverseGravity_Spell.SetEffectDescription(effectDescription.Build());
            ReverseGravity_Spell.SetAiParameters(new SpellAIParameters());
            //  ReverseGravity_Spell
            ReverseGravity_Spell.SetConcentration();
            SpellDefinition ReverseGravity = ReverseGravity_Spell.AddToDB();

            return ReverseGravity;

        }

        internal class ReverseGravityCondition_Builder : BaseDefinitionBuilder<ConditionDefinition>
        {
            const string name = "DH_Level20_ReverseGravity_Spellcondition";
            const string guid = "809f1cef-6bdc-4b5a-93bf-275af8ab0b36";
            string title_string = "Condition/&DH_Level20_ReverseGravity_Spell_Title";
            string description_string = "Condition/&DH_Level20_ReverseGravity_Spell_Description";
            protected ReverseGravityCondition_Builder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionLevitate, name, guid)
            {
                Definition.GuiPresentation.Title = title_string;
                Definition.GuiPresentation.Description = description_string;

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
            public static ConditionDefinition CreateAndAddToDB(string name, string guid)
                => new ReverseGravityCondition_Builder(name, guid).AddToDB();
            public static ConditionDefinition ReverseGravityCondition = CreateAndAddToDB(name, guid);
        }


        private static SpellDefinition BuildNew_ConjureCelestial_Spell()
        {


            SpellListDefinition Couatl_SpellList = SpellListBuilder.CreateSpellList(
                 BaseString + "Couatl_SpellList",
                 GuidHelper.Create(new System.Guid(BaseGuid), BaseString + "Couatl_SpellList").ToString(),
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

            Couatl_SpellList.SetHasCantrips(false);
            Couatl_SpellList.SetMaxSpellLevel(5);





            FeatureDefinitionCastSpell CastSpell_Couatl = new CastSpellBuilder
                (
                BaseString + "CastSpell_Couatl",
                GuidHelper.Create(new System.Guid(BaseGuid), BaseString + "CastSpell_Couatl").ToString()
                 )
                .AddToDB();

            CastSpell_Couatl.SetGuiPresentation
                (
                    new GuiPresentationBuilder
                    (
                            "Feature/&NoContentTitle",
                            "Feature/&NoContentTitle"
                            )
                            .Build()
                 );



            CastSpell_Couatl.SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Monster);
            CastSpell_Couatl.SetSpellcastingAbility(AttributeDefinitions.Charisma);
            CastSpell_Couatl.SetSpellcastingParametersComputation(RuleDefinitions.SpellcastingParametersComputation.Static);
            CastSpell_Couatl.SetStaticDCValue(14);
            CastSpell_Couatl.SetStaticToHitValue(8);
            CastSpell_Couatl.SetSpellListDefinition(Couatl_SpellList);
            CastSpell_Couatl.RestrictedSchools.Clear();
            CastSpell_Couatl.SetSpellKnowledge(RuleDefinitions.SpellKnowledge.FixedList);
            CastSpell_Couatl.SetSlotsRecharge(RuleDefinitions.RechargeRate.ShortRest);
            CastSpell_Couatl.SetSpellCastingLevel(9);
            CastSpell_Couatl.SetSpellReadyness(RuleDefinitions.SpellReadyness.AllKnown);
            CastSpell_Couatl.SetFocusType(EquipmentDefinitions.FocusType.None);

            int[] arr = { 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20 };

            CastSpell_Couatl.KnownSpells.Clear();
            CastSpell_Couatl.KnownSpells.AddRange(arr);

            CastSpell_Couatl.SlotsPerLevels.Clear();
            CastSpell_Couatl.SlotsPerLevels.AddRange(new List<FeatureDefinitionCastSpell.SlotsByLevelDuplet>()
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


            string text = "ConjureCelestial_Spell";

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
                    DatabaseHelper.FeatureDefinitionMoveModes.MoveModeFly12 ,
                     DatabaseHelper.FeatureDefinitionSenses.SenseNormalVision   ,
                     DatabaseHelper.FeatureDefinitionSenses.SenseSuperiorDarkvision ,
                     DatabaseHelper.FeatureDefinitionSenses.SenseTruesight16 ,
                     // wildshape was meant to be a substitute for the couatl's shapechanging but the game didnt like it
                     // (gave the couatl a second position in the intiative order)
                    // DatabaseHelper.FeatureDefinitionPowers.PowerDruidWildShape,
                     DatabaseHelper.FeatureDefinitionAttackModifiers.AttackModifierKindredSpiritMagicSpiritMagicAttack ,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityRadiantResistance,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPsychicImmunity   ,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityMummyLord_PiercingImmunity,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityMummyLord_SlashingImmunity,
                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityMummyLord_BludgeoningImmunity  ,
                     DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityPoisonImmunity ,

                     CastSpell_Couatl
                };

            List<MonsterSkillProficiency> SkillScores = new List<MonsterSkillProficiency>()
            {

            };





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

            List<MonsterAttackIteration> AttackIterations = new List<MonsterAttackIteration>()
                {
                   DatabaseHelper.MonsterDefinitions.Tiger_Drake.AttackIterations[0]
                   // CouatlBite_Attack
                };
            List<LegendaryActionDescription> LegendaryActionOptions = new List<LegendaryActionDescription>()
            {

            };



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

            MonsterSavingThrowProficiency Str_save = new MonsterSavingThrowProficiency();
            Str_save.SetField("abilityScoreName", "Strength");
            Str_save.SetField("bonus", SavingThrowStrength);

            MonsterSavingThrowProficiency Dex_save = new MonsterSavingThrowProficiency();
            Dex_save.SetField("abilityScoreName", "Dexterity");
            Dex_save.SetField("bonus", SavingThrowDexterity);

            MonsterSavingThrowProficiency Con_save = new MonsterSavingThrowProficiency();
            Con_save.SetField("abilityScoreName", "Constitution");
            Con_save.SetField("bonus", SavingThrowConstitution);

            MonsterSavingThrowProficiency Int_save = new MonsterSavingThrowProficiency();
            Int_save.SetField("abilityScoreName", "Intelligence");
            Int_save.SetField("bonus", SavingThrowIntelligence);

            MonsterSavingThrowProficiency Wis_save = new MonsterSavingThrowProficiency();
            Wis_save.SetField("abilityScoreName", "Wisdom");
            Wis_save.SetField("bonus", SavingThrowWisdom);

            MonsterSavingThrowProficiency Cha_save = new MonsterSavingThrowProficiency();
            Cha_save.SetField("abilityScoreName", "Charisma");
            Cha_save.SetField("bonus", SavingThrowCharisma);

            CouatlBuilder.ClearSavingThrowScores();
            CouatlBuilder.AddSavingThrowScores(new List<MonsterSavingThrowProficiency>()
                {
                    Str_save,
                    Dex_save,
                    Con_save,
                    Int_save,
                    Wis_save,
                    Cha_save
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


            GuiPresentation guiPresentation_Spell = new GuiPresentation();
            guiPresentation_Spell.SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f));
            guiPresentation_Spell.SetDescription("Spell/&" + BaseString + text + "_Description");
            guiPresentation_Spell.SetTitle("Spell/&" + BaseString + text + "_Title");
            guiPresentation_Spell.SetSpriteReference(DatabaseHelper.SpellDefinitions.Bless.GuiPresentation.SpriteReference);
            guiPresentation_Spell.SetSymbolChar("221E");


            SpellBuilder ConjureCelestial_Spell = new SpellBuilder(BaseString + text, GuidHelper.Create(new System.Guid(BaseGuid), BaseString + text).ToString());
            ConjureCelestial_Spell.SetGuiPresentation(guiPresentation_Spell);
            ConjureCelestial_Spell.SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation);
            ConjureCelestial_Spell.SetSpellLevel(7);
            ConjureCelestial_Spell.SetCastingTime(RuleDefinitions.ActivationTime.Action);
            ConjureCelestial_Spell.SetVerboseComponent(true);
            ConjureCelestial_Spell.SetSomaticComponent(true);
            ConjureCelestial_Spell.SetEffectDescription(effectDescription.Build());
            ConjureCelestial_Spell.SetAiParameters(new SpellAIParameters());
            ConjureCelestial_Spell.SetConcentration();
            SpellDefinition ConjureCelestial = ConjureCelestial_Spell.AddToDB(true);
            SpellDefinitionExtensions.SetRequiresConcentration<SpellDefinition>(ConjureCelestial, true);


            return ConjureCelestial;



        }

/*
        private static SpellDefinition BuildNew_AnimalShapes_Spell()
        {

            string text = "AnimalShapes_Spell";

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


            GuiPresentation guiPresentation_Spell = new GuiPresentation();
            guiPresentation_Spell.SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f));
            guiPresentation_Spell.SetDescription("Spell/&" + BaseString + text + "_Description");
            guiPresentation_Spell.SetTitle("Spell/&" + BaseString + text + "_Title");
            guiPresentation_Spell.SetSpriteReference(DatabaseHelper.SpellDefinitions.ConjureFey.GuiPresentation.SpriteReference);
            guiPresentation_Spell.SetSymbolChar("221E");

            SpellBuilder AnimalShapes_Spell = new SpellBuilder(BaseString + text, GuidHelper.Create(new System.Guid(BaseGuid), BaseString + text).ToString());
            AnimalShapes_Spell.SetGuiPresentation(guiPresentation_Spell);
            AnimalShapes_Spell.SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation);
            AnimalShapes_Spell.SetSpellLevel(8);
            AnimalShapes_Spell.SetCastingTime(RuleDefinitions.ActivationTime.Action);
            AnimalShapes_Spell.SetVerboseComponent(true);
            AnimalShapes_Spell.SetSomaticComponent(false);
            AnimalShapes_Spell.SetEffectDescription(effectDescription.Build());
            AnimalShapes_Spell.SetAiParameters(new SpellAIParameters());
            AnimalShapes_Spell.SetConcentration();
            //  AnimalShapes_Spell
            SpellDefinition AnimalShapes = AnimalShapes_Spell.AddToDB();

            return AnimalShapes;
        }
*/
        private static SpellDefinition BuildNew_DominateMonster_Spell()
        {


            string text = "DominateMonster_Spell";


            EffectDescription effectDescription = new EffectDescription();
            effectDescription.Copy(DatabaseHelper.SpellDefinitions.DominatePerson.EffectDescription);
            effectDescription.RestrictedCreatureFamilies.Clear();
            effectDescription.SetDurationType(RuleDefinitions.DurationType.Hour);

            GuiPresentation guiPresentation_Spell = new GuiPresentation();
            guiPresentation_Spell.SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f));
            guiPresentation_Spell.SetDescription("Spell/&" + BaseString + text + "_Description");
            guiPresentation_Spell.SetTitle("Spell/&" + BaseString + text + "_Title");
            guiPresentation_Spell.SetSpriteReference(DatabaseHelper.SpellDefinitions.DominatePerson.GuiPresentation.SpriteReference);
            guiPresentation_Spell.SetSymbolChar("221E");

            SpellBuilder DominateMonster_Spell = new SpellBuilder(BaseString + text, GuidHelper.Create(new System.Guid(BaseGuid), BaseString + text).ToString());
            DominateMonster_Spell.SetGuiPresentation(guiPresentation_Spell);
            DominateMonster_Spell.SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation);
            DominateMonster_Spell.SetSpellLevel(8);
            DominateMonster_Spell.SetCastingTime(RuleDefinitions.ActivationTime.Action);
            DominateMonster_Spell.SetVerboseComponent(true);
            DominateMonster_Spell.SetSomaticComponent(true);
            DominateMonster_Spell.SetEffectDescription(effectDescription);
            DominateMonster_Spell.SetAiParameters(new SpellAIParameters());
            SpellDefinition DominateMonster= DominateMonster_Spell.AddToDB();


            return DominateMonster;

        }






        private static SpellDefinition BuildNew_Feeblemind_Spell()
        {

            string text = "Feeblemind_Spell";



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


            GuiPresentation guiPresentation_Spell = new GuiPresentation();
            guiPresentation_Spell.SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f));
            guiPresentation_Spell.SetDescription("Spell/&" + BaseString + text + "_Description");
            guiPresentation_Spell.SetTitle("Spell/&" + BaseString + text + "_Title");
            guiPresentation_Spell.SetSpriteReference(DatabaseHelper.SpellDefinitions.BestowCurse.GuiPresentation.SpriteReference);
            guiPresentation_Spell.SetSymbolChar("221E");

            SpellBuilder Feeblemind_Spell = new SpellBuilder(BaseString + text, GuidHelper.Create(new System.Guid(BaseGuid), BaseString + text).ToString());
            Feeblemind_Spell.SetGuiPresentation(guiPresentation_Spell);
            Feeblemind_Spell.SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation);
            Feeblemind_Spell.SetSpellLevel(8);
            Feeblemind_Spell.SetCastingTime(RuleDefinitions.ActivationTime.Action);
            Feeblemind_Spell.SetVerboseComponent(true);
            Feeblemind_Spell.SetSomaticComponent(false);
            Feeblemind_Spell.SetEffectDescription(effectDescription.Build());
            Feeblemind_Spell.SetAiParameters(new SpellAIParameters());
            //   Feeblemind_Spell
            SpellDefinition Feeblemind = Feeblemind_Spell.AddToDB();

            DatabaseHelper.SpellDefinitions.GreaterRestoration.EffectDescription.EffectForms[0].ConditionForm.DetrimentalConditions.Add(FeeblemindCondition_Builder.FeeblemindCondition);

            return Feeblemind;
        }

        internal class FeeblemindCondition_Builder : BaseDefinitionBuilder<ConditionDefinition>
        {
            const string name = "DH_Level20_Feeblemind_Spellcondition";
            const string guid = "965a09b2-cb22-452b-b93c-2bccdcda4871";
            string title_string = "Condition/&DH_Level20_Feeblemind_Spell_Title";
            string description_string = "Condition/&DH_Level20_Feeblemind_Spell_Description";
            protected FeeblemindCondition_Builder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionBearsEndurance, name, guid)
            {
                Definition.GuiPresentation.Title = title_string;
                Definition.GuiPresentation.Description = description_string;

                Definition.SetConditionType(RuleDefinitions.ConditionType.Detrimental);

                Definition.Features.Clear();
                Definition.Features.Empty();
                Definition.Features.AddRange
                    (
                    DatabaseHelper.FeatureDefinitionMagicAffinitys.MagicAffinitySilenced,
                    DatabaseHelper.FeatureDefinitionMagicAffinitys.MagicAffinityConditionRaging,
                    Feeblemind_Int_AttributeModifier_Builder.Feeblemind_Int_AttributeModifier,
                    Feeblemind_Cha_AttributeModifier_Builder.Feeblemind_Cha_AttributeModifier,
                    FeeblemindActionAffinity_Builder.FeeblemindActionAffinity
                    );
            }
            public static ConditionDefinition CreateAndAddToDB(string name, string guid)
                => new FeeblemindCondition_Builder(name, guid).AddToDB();
            public static ConditionDefinition FeeblemindCondition = CreateAndAddToDB(name, guid);
        }

        internal class Feeblemind_Int_AttributeModifier_Builder : BaseDefinitionBuilder<FeatureDefinitionAttributeModifier>
        {
            const string name = "DH_Level20_Feeblemind_Int_SpellAttributeModifier";
            const string guid = "a2a16bda-e7b1-4a87-9f0e-3e4c21870fd8";
            string title_string = "AttributeModifier/&DH_Level20_Feeblemind_Int_Spell_Title";
            string description_string = "AttributeModifier/&DH_Level20_Feeblemind_Int_Spell_Description";
            protected Feeblemind_Int_AttributeModifier_Builder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierHeadbandOfIntellect, name, guid)
            {
                Definition.GuiPresentation.Title = title_string;
                Definition.GuiPresentation.Description = description_string;

                Definition.SetModifiedAttribute(DatabaseHelper.SmartAttributeDefinitions.Intelligence.name);
                Definition.SetModifierAbilityScore(DatabaseHelper.SmartAttributeDefinitions.Intelligence.name);
                Definition.SetModifierType2(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Force);
                Definition.SetModifierValue(1);
                Definition.SetSituationalContext(RuleDefinitions.SituationalContext.None);

            }
            public static FeatureDefinitionAttributeModifier CreateAndAddToDB(string name, string guid)
                => new Feeblemind_Int_AttributeModifier_Builder(name, guid).AddToDB();
            public static FeatureDefinitionAttributeModifier Feeblemind_Int_AttributeModifier = CreateAndAddToDB(name, guid);
        }

        internal class Feeblemind_Cha_AttributeModifier_Builder : BaseDefinitionBuilder<FeatureDefinitionAttributeModifier>
        {
            const string name = "DH_Level20_Feeblemind_Cha_SpellAttributeModifier";
            const string guid = "6721abe1-19eb-4a8c-9702-2fdea2070464";
            string title_string = "AttributeModifier/&DH_Level20_Feeblemind_Cha_Spell_Title";
            string description_string = "AttributeModifier/&DH_Level20_Feeblemind_Cha_Spell_Description";
            protected Feeblemind_Cha_AttributeModifier_Builder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierHeadbandOfIntellect, name, guid)
            {
                Definition.GuiPresentation.Title = title_string;
                Definition.GuiPresentation.Description = description_string;

                Definition.SetModifiedAttribute(DatabaseHelper.SmartAttributeDefinitions.Charisma.name);
                Definition.SetModifierAbilityScore(DatabaseHelper.SmartAttributeDefinitions.Charisma.name);
                Definition.SetModifierType2(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Force);
                Definition.SetModifierValue(1);
                Definition.SetSituationalContext(RuleDefinitions.SituationalContext.None);

            }
            public static FeatureDefinitionAttributeModifier CreateAndAddToDB(string name, string guid)
                => new Feeblemind_Cha_AttributeModifier_Builder(name, guid).AddToDB();
            public static FeatureDefinitionAttributeModifier Feeblemind_Cha_AttributeModifier = CreateAndAddToDB(name, guid);
        }



        internal class FeeblemindActionAffinity_Builder : BaseDefinitionBuilder<FeatureDefinitionActionAffinity>
        {
            const string name = "DH_Level20_Feeblemind_SpellActionAffinity";
            const string guid = "749a9572-07f6-4678-9458-904c04b9ab22";
            string title_string = "ActionAffinity/&DH_Level20_Feeblemind_Spell_Title";
            string description_string = "ActionAffinity/&DH_Level20_Feeblemind_Spell_Description";
            protected FeeblemindActionAffinity_Builder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionActionAffinitys.ActionAffinityConditionRaging, name, guid)
            {
                Definition.GuiPresentation.Title = title_string;
                Definition.GuiPresentation.Description = description_string;


                Definition.ForbiddenActions.Clear();
                Definition.ForbiddenActions.Empty();
                Definition.ForbiddenActions.AddRange
                    (DatabaseHelper.ActionDefinitions.CastBonus.Id,
                    DatabaseHelper.ActionDefinitions.CastMain.Id,
                    DatabaseHelper.ActionDefinitions.CastNoCost.Id,
                    DatabaseHelper.ActionDefinitions.CastReaction.Id,
                    DatabaseHelper.ActionDefinitions.CastReadied.Id,
                    DatabaseHelper.ActionDefinitions.CastRitual.Id

                    );
            }
            public static FeatureDefinitionActionAffinity CreateAndAddToDB(string name, string guid)
                => new FeeblemindActionAffinity_Builder(name, guid).AddToDB();
            public static FeatureDefinitionActionAffinity FeeblemindActionAffinity = CreateAndAddToDB(name, guid);
        }


        private static SpellDefinition BuildNew_HolyAura_Spell()
        {

            string text = "HolyAura_Spell";



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


            GuiPresentation guiPresentation_Spell = new GuiPresentation();
            guiPresentation_Spell.SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f));
            guiPresentation_Spell.SetDescription("Spell/&" + BaseString + text + "_Description");
            guiPresentation_Spell.SetTitle("Spell/&" + BaseString + text + "_Title");
            guiPresentation_Spell.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerDomainLifePreserveLife.GuiPresentation.SpriteReference);
            guiPresentation_Spell.SetSymbolChar("221E");

            SpellBuilder HolyAura_Spell = new SpellBuilder(BaseString + text, GuidHelper.Create(new System.Guid(BaseGuid), BaseString + text).ToString());
            HolyAura_Spell.SetGuiPresentation(guiPresentation_Spell);
            HolyAura_Spell.SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation);
            HolyAura_Spell.SetSpellLevel(8);
            HolyAura_Spell.SetCastingTime(RuleDefinitions.ActivationTime.Action);
            HolyAura_Spell.SetVerboseComponent(true);
            HolyAura_Spell.SetSomaticComponent(false);
            HolyAura_Spell.SetEffectDescription(effectDescription.Build());
            HolyAura_Spell.SetAiParameters(new SpellAIParameters());
            //   HolyAura_Spell
            HolyAura_Spell.SetConcentration();
            SpellDefinition HolyAura = HolyAura_Spell.AddToDB();

            return HolyAura;

        }


        internal class HolyAuraCondition_Builder : BaseDefinitionBuilder<ConditionDefinition>
        {
            const string name = "DH_Level20_HolyAura_Spellcondition";
            const string guid = "1808ca4b-8f46-41bf-a59c-0bcbd4f60248";
            string title_string = "Condition/&DH_Level20_HolyAura_Spell_Title";
            string description_string = "Condition/&DH_Level20_HolyAura_Spell_Description";
            protected HolyAuraCondition_Builder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionBearsEndurance, name, guid)
            {
                Definition.GuiPresentation.Title = title_string;
                Definition.GuiPresentation.Description = description_string;

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
            public static ConditionDefinition CreateAndAddToDB(string name, string guid)
                => new HolyAuraCondition_Builder(name, guid).AddToDB();
            public static ConditionDefinition HolyAuraCondition = CreateAndAddToDB(name, guid);
        }

        internal class HolyAuraDamageAffinity_Builder : BaseDefinitionBuilder<FeatureDefinitionDamageAffinity>
        {
            const string name = "DH_Level20_HolyAura_SpellDamageAffinity";
            const string guid = "c83aceae-e4c4-4a9c-a83d-58ffebe92007";
            string title_string = "DamageAffinity/&DH_Level20_HolyAura_Spell_Title";
            string description_string = "DamageAffinity/&DH_Level20_HolyAura_Spell_Description";
            protected HolyAuraDamageAffinity_Builder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPoisonAdvantage, name, guid)
            {
                Definition.GuiPresentation.Title = title_string;
                Definition.GuiPresentation.Description = description_string;

                Definition.SetDamageAffinityType(RuleDefinitions.DamageAffinityType.None);
                Definition.SetSavingThrowAdvantageType(RuleDefinitions.AdvantageType.None);
                Definition.SetKnockOutAffinity(RuleDefinitions.KnockoutAffinity.None);

                Definition.SetRetaliateWhenHit(true);
                Definition.SetRetaliateProximity(RuleDefinitions.AttackProximity.Melee);
                Definition.SetRetaliateRangeCells(1);
                Definition.SetRetaliatePower(HolyAuraBlindingPower_Builder.HolyAuraBlindingPower);



            }
            public static FeatureDefinitionDamageAffinity CreateAndAddToDB(string name, string guid)
                => new HolyAuraDamageAffinity_Builder(name, guid).AddToDB();
            public static FeatureDefinitionDamageAffinity HolyAuraDamageAffinity = CreateAndAddToDB(name, guid);
        }


        internal class HolyAuraBlindingPower_Builder : BaseDefinitionBuilder<FeatureDefinitionPower>
        {
            const string name = "DH_Level20_HolyAura_SpellBlindingPower";
            const string guid = "40366ca2-00a0-471a-b370-8c81f6283ce1";
            string title_string = "Feature/&DH_Level20_HolyAura_BlindingPower_Title";
            string description_string = "Feataure/&DH_Level20_HolyAura_BlindingPower_Description";
            protected HolyAuraBlindingPower_Builder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionPowers.PowerOathOfMotherlandFieryPresence, name, guid)
            {
                Definition.GuiPresentation.Title = title_string;
                Definition.GuiPresentation.Description = description_string;

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
            public static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
                => new HolyAuraBlindingPower_Builder(name, guid).AddToDB();
            public static FeatureDefinitionPower HolyAuraBlindingPower = CreateAndAddToDB(name, guid);
        }

        private static SpellDefinition BuildNew_IncendiaryCloud_Spell()
        {

            string text = "IncendiaryCloud_Spell";



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

            GuiPresentation guiPresentation_Spell = new GuiPresentation();
            guiPresentation_Spell.SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f));
            guiPresentation_Spell.SetDescription("Spell/&" + BaseString + text + "_Description");
            guiPresentation_Spell.SetTitle("Spell/&" + BaseString + text + "_Title");
            guiPresentation_Spell.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalFireBurst.GuiPresentation.SpriteReference);
            guiPresentation_Spell.SetSymbolChar("221E");

            SpellBuilder IncendiaryCloud_Spell = new SpellBuilder(BaseString + text, GuidHelper.Create(new System.Guid(BaseGuid), BaseString + text).ToString());
            IncendiaryCloud_Spell.SetGuiPresentation(guiPresentation_Spell);
            IncendiaryCloud_Spell.SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation);
            IncendiaryCloud_Spell.SetSpellLevel(8);
            IncendiaryCloud_Spell.SetCastingTime(RuleDefinitions.ActivationTime.Action);
            IncendiaryCloud_Spell.SetVerboseComponent(true);
            IncendiaryCloud_Spell.SetSomaticComponent(false);
            //  IncendiaryCloud_Spell.SetEffectDescription(effectDescription.Build());
            IncendiaryCloud_Spell.SetEffectDescription(neweffectDescription);
            IncendiaryCloud_Spell.SetAiParameters(new SpellAIParameters());
            //    IncendiaryCloud_Spell
            IncendiaryCloud_Spell.SetConcentration();
            SpellDefinition IncendiaryCloud = IncendiaryCloud_Spell.AddToDB();

            return IncendiaryCloud ;

        }


        private static SpellDefinition BuildNew_Maze_Spell()
        {

            string text = "Maze_Spell";



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


            GuiPresentation guiPresentation_Spell = new GuiPresentation();
            guiPresentation_Spell.SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f));
            guiPresentation_Spell.SetDescription("Spell/&" + BaseString + text + "_Description");
            guiPresentation_Spell.SetTitle("Spell/&" + BaseString + text + "_Title");
            guiPresentation_Spell.SetSpriteReference(DatabaseHelper.SpellDefinitions.IdentifyCreatures.GuiPresentation.SpriteReference);
            guiPresentation_Spell.SetSymbolChar("221E");

            SpellBuilder Maze_Spell = new SpellBuilder(BaseString + text, GuidHelper.Create(new System.Guid(BaseGuid), BaseString + text).ToString());
            Maze_Spell.SetGuiPresentation(guiPresentation_Spell);
            Maze_Spell.SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation);
            Maze_Spell.SetSpellLevel(8);
            Maze_Spell.SetCastingTime(RuleDefinitions.ActivationTime.Action);
            Maze_Spell.SetVerboseComponent(true);
            Maze_Spell.SetSomaticComponent(false);
            Maze_Spell.SetEffectDescription(effectDescription.Build());
            Maze_Spell.SetAiParameters(new SpellAIParameters());
            //   Maze_Spell
            Maze_Spell.SetConcentration();
            SpellDefinition Maze = Maze_Spell.AddToDB();

            return Maze;

        }

        private static SpellDefinition BuildNew_MindBlank_Spell()
        {

            string text = "MindBlank_Spell";



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


            GuiPresentation guiPresentation_Spell = new GuiPresentation();
            guiPresentation_Spell.SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f));
            guiPresentation_Spell.SetDescription("Spell/&" + BaseString + text + "_Description");
            guiPresentation_Spell.SetTitle("Spell/&" + BaseString + text + "_Title");
            guiPresentation_Spell.SetSpriteReference(DatabaseHelper.SpellDefinitions.MindTwist.GuiPresentation.SpriteReference);
            guiPresentation_Spell.SetSymbolChar("221E");

            SpellBuilder MindBlank_Spell = new SpellBuilder(BaseString + text, GuidHelper.Create(new System.Guid(BaseGuid), BaseString + text).ToString());
            MindBlank_Spell.SetGuiPresentation(guiPresentation_Spell);
            MindBlank_Spell.SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation);
            MindBlank_Spell.SetSpellLevel(8);
            MindBlank_Spell.SetCastingTime(RuleDefinitions.ActivationTime.Action);
            MindBlank_Spell.SetVerboseComponent(true);
            MindBlank_Spell.SetSomaticComponent(false);
            MindBlank_Spell.SetEffectDescription(effectDescription.Build());
            MindBlank_Spell.SetAiParameters(new SpellAIParameters());
            //    MindBlank_Spell
            SpellDefinition MindBlank = MindBlank_Spell.AddToDB();

            return MindBlank;

        }

        internal class MindBlankCondition_Builder : BaseDefinitionBuilder<ConditionDefinition>
        {
            const string name = "DH_Level20_MindBlank_Spellcondition";
            const string guid = "74f77a4c-b5cb-45d6-ac6d-d9fa2ebe3869";
            string title_string = "Condition/&DH_Level20_MindBlank_Spell_Title";
            string description_string = "Condition/&DH_Level20_MindBlank_Spell_Description";
            protected MindBlankCondition_Builder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionBearsEndurance, name, guid)
            {
                Definition.GuiPresentation.Title = title_string;
                Definition.GuiPresentation.Description = description_string;

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
            public static ConditionDefinition CreateAndAddToDB(string name, string guid)
                => new MindBlankCondition_Builder(name, guid).AddToDB();
            public static ConditionDefinition MindBlankCondition = CreateAndAddToDB(name, guid);
        }


        private static SpellDefinition BuildNew_PowerWordStun_Spell()
        {

            string text = "PowerWordStun_Spell";


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


            GuiPresentation guiPresentation_Spell = new GuiPresentation();
            guiPresentation_Spell.SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f));
            guiPresentation_Spell.SetDescription("Spell/&" + BaseString + text + "_Description");
            guiPresentation_Spell.SetTitle("Spell/&" + BaseString + text + "_Title");
            guiPresentation_Spell.SetSpriteReference(DatabaseHelper.SpellDefinitions.Contagion.GuiPresentation.SpriteReference);
            guiPresentation_Spell.SetSymbolChar("221E");

            SpellBuilder PowerWordStun_Spell = new SpellBuilder(BaseString + text, GuidHelper.Create(new System.Guid(BaseGuid), BaseString + text).ToString());
            PowerWordStun_Spell.SetGuiPresentation(guiPresentation_Spell);
            PowerWordStun_Spell.SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation);
            PowerWordStun_Spell.SetSpellLevel(8);
            PowerWordStun_Spell.SetCastingTime(RuleDefinitions.ActivationTime.Action);
            PowerWordStun_Spell.SetVerboseComponent(true);
            PowerWordStun_Spell.SetSomaticComponent(false);
            PowerWordStun_Spell.SetEffectDescription(effectDescription.Build());
            PowerWordStun_Spell.SetAiParameters(new SpellAIParameters());
            //   PowerWordStun_Spell
            SpellDefinition PowerWordStun = PowerWordStun_Spell.AddToDB();

            return PowerWordStun;



        }


        private static SpellDefinition BuildNew_SunBurst_Spell()
        {

            string text = "SunBurst_Spell";



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

            GuiPresentation guiPresentation_Spell = new GuiPresentation();
            guiPresentation_Spell.SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f));
            guiPresentation_Spell.SetDescription("Spell/&" + BaseString + text + "_Description");
            guiPresentation_Spell.SetTitle("Spell/&" + BaseString + text + "_Title");
            guiPresentation_Spell.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerDomainSunIndomitableLight.GuiPresentation.SpriteReference);
            guiPresentation_Spell.SetSymbolChar("221E");

            SpellBuilder SunBurst_Spell = new SpellBuilder(BaseString + text, GuidHelper.Create(new System.Guid(BaseGuid), BaseString + text).ToString());
            SunBurst_Spell.SetGuiPresentation(guiPresentation_Spell);
            SunBurst_Spell.SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation);
            SunBurst_Spell.SetSpellLevel(8);
            SunBurst_Spell.SetCastingTime(RuleDefinitions.ActivationTime.Action);
            SunBurst_Spell.SetVerboseComponent(true);
            SunBurst_Spell.SetSomaticComponent(false);
            SunBurst_Spell.SetEffectDescription(effect);
            SunBurst_Spell.SetAiParameters(new SpellAIParameters());
            //   SunBurst_Spell
            SpellDefinition SunBurst = SunBurst_Spell.AddToDB();

            return SunBurst;

        }



        private static SpellDefinition BuildNew_Foresight_Spell()
        {

            string text = "Foresight_Spell";



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


            GuiPresentation guiPresentation_Spell = new GuiPresentation();
            guiPresentation_Spell.SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f));
            guiPresentation_Spell.SetDescription("Spell/&" + BaseString + text + "_Description");
            guiPresentation_Spell.SetTitle("Spell/&" + BaseString + text + "_Title");
            guiPresentation_Spell.SetSpriteReference(DatabaseHelper.SpellDefinitions.TrueSeeing.GuiPresentation.SpriteReference);
            guiPresentation_Spell.SetSymbolChar("221E");

            SpellBuilder Foresight_Spell = new SpellBuilder(BaseString + text, GuidHelper.Create(new System.Guid(BaseGuid), BaseString + text).ToString());
            Foresight_Spell.SetGuiPresentation(guiPresentation_Spell);
            Foresight_Spell.SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation);
            Foresight_Spell.SetSpellLevel(9);
            Foresight_Spell.SetCastingTime(RuleDefinitions.ActivationTime.Minute1);
            Foresight_Spell.SetVerboseComponent(true);
            Foresight_Spell.SetSomaticComponent(false);
            Foresight_Spell.SetEffectDescription(effectDescription.Build());
            Foresight_Spell.SetAiParameters(new SpellAIParameters());
            //    Foresight_Spell
            SpellDefinition  Foresight = Foresight_Spell.AddToDB();

            return Foresight;

        }

        internal class ForesightCondition_Builder : BaseDefinitionBuilder<ConditionDefinition>
        {
            const string name = "DH_Level20_Foresight_Spellcondition";
            const string guid = "4615c639-95f2-4c04-b904-e79f5b916b68";
            string title_string = "Condition/&DH_Level20_Foresight_Spell_Title";
            string description_string = "Condition/&DH_Level20_Foresight_Spell_Description";
            protected ForesightCondition_Builder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionBearsEndurance, name, guid)
            {
                Definition.GuiPresentation.Title = title_string;
                Definition.GuiPresentation.Description = description_string;

                Definition.Features.Clear();
                Definition.Features.Empty();
                Definition.Features.AddRange
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
            public static ConditionDefinition CreateAndAddToDB(string name, string guid)
                => new ForesightCondition_Builder(name, guid).AddToDB();
            public static ConditionDefinition ForesightCondition = CreateAndAddToDB(name, guid);
        }

        private static SpellDefinition BuildNew_MassHeal_Spell()
        {

            string text = "MassHeal_Spell";



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


            GuiPresentation guiPresentation_Spell = new GuiPresentation();
            guiPresentation_Spell.SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f));
            guiPresentation_Spell.SetDescription("Spell/&" + BaseString + text + "_Description");
            guiPresentation_Spell.SetTitle("Spell/&" + BaseString + text + "_Title");
            guiPresentation_Spell.SetSpriteReference(DatabaseHelper.SpellDefinitions.Heal.GuiPresentation.SpriteReference);
            guiPresentation_Spell.SetSymbolChar("221E");

            SpellBuilder MassHeal_Spell = new SpellBuilder(BaseString + text, GuidHelper.Create(new System.Guid(BaseGuid), BaseString + text).ToString());
            MassHeal_Spell.SetGuiPresentation(guiPresentation_Spell);
            MassHeal_Spell.SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation);
            MassHeal_Spell.SetSpellLevel(9);
            MassHeal_Spell.SetCastingTime(RuleDefinitions.ActivationTime.Action);
            MassHeal_Spell.SetVerboseComponent(true);
            MassHeal_Spell.SetSomaticComponent(false);
            MassHeal_Spell.SetEffectDescription(effectDescription.Build());
            MassHeal_Spell.SetAiParameters(new SpellAIParameters());
            //    MassHeal_Spell
            SpellDefinition MassHeal = MassHeal_Spell.AddToDB();

            return MassHeal;

        }


        private static SpellDefinition BuildNew_MeteorSwarm_SingleTarget_Spell()
        {

            string text = "MeteorSwarm_SingleTarget_Spell";

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


            GuiPresentation guiPresentation_Spell = new GuiPresentation();
            guiPresentation_Spell.SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f));
            guiPresentation_Spell.SetDescription("Spell/&" + BaseString + text + "_Description");
            guiPresentation_Spell.SetTitle("Spell/&" + BaseString + text + "_Title");
            guiPresentation_Spell.SetSpriteReference(DatabaseHelper.SpellDefinitions.FlamingSphere.GuiPresentation.SpriteReference);
            guiPresentation_Spell.SetSymbolChar("221E");

            SpellBuilder MeteorSwarm_SingleTarget_Spell = new SpellBuilder(BaseString + text, GuidHelper.Create(new System.Guid(BaseGuid), BaseString + text).ToString());
            MeteorSwarm_SingleTarget_Spell.SetGuiPresentation(guiPresentation_Spell);
            MeteorSwarm_SingleTarget_Spell.SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation);
            MeteorSwarm_SingleTarget_Spell.SetSpellLevel(9);
            MeteorSwarm_SingleTarget_Spell.SetCastingTime(RuleDefinitions.ActivationTime.Action);
            MeteorSwarm_SingleTarget_Spell.SetVerboseComponent(true);
            MeteorSwarm_SingleTarget_Spell.SetSomaticComponent(false);
            MeteorSwarm_SingleTarget_Spell.SetEffectDescription(effectDescription.Build());
            MeteorSwarm_SingleTarget_Spell.SetAiParameters(new SpellAIParameters());
            //            MeteorSwarm_SingleTarget_Spell
            SpellDefinition  MeteorSwarm_SingleTarget = MeteorSwarm_SingleTarget_Spell.AddToDB();

            return MeteorSwarm_SingleTarget;

        }


        private static SpellDefinition BuildNew_PowerWordHeal_Spell()
        {

            string text = "PowerWordHeal_Spell";



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


            GuiPresentation guiPresentation_Spell = new GuiPresentation();
            guiPresentation_Spell.SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f));
            guiPresentation_Spell.SetDescription("Spell/&" + BaseString + text + "_Description");
            guiPresentation_Spell.SetTitle("Spell/&" + BaseString + text + "_Title");
            guiPresentation_Spell.SetSpriteReference(DatabaseHelper.SpellDefinitions.HealingWord.GuiPresentation.SpriteReference);
            guiPresentation_Spell.SetSymbolChar("221E");

            SpellBuilder PowerWordHeal_Spell = new SpellBuilder(BaseString + text, GuidHelper.Create(new System.Guid(BaseGuid), BaseString + text).ToString());
            PowerWordHeal_Spell.SetGuiPresentation(guiPresentation_Spell);
            PowerWordHeal_Spell.SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation);
            PowerWordHeal_Spell.SetSpellLevel(9);
            PowerWordHeal_Spell.SetCastingTime(RuleDefinitions.ActivationTime.Action);
            PowerWordHeal_Spell.SetVerboseComponent(true);
            PowerWordHeal_Spell.SetSomaticComponent(false);
            PowerWordHeal_Spell.SetEffectDescription(effectDescription.Build());
            PowerWordHeal_Spell.SetAiParameters(new SpellAIParameters());
            //    PowerWordHeal_Spell
            SpellDefinition PowerWordHeal = PowerWordHeal_Spell.AddToDB();

            return PowerWordHeal;

        }


        private static SpellDefinition BuildNew_PowerWordKill_Spell()
        {

            string text = "PowerWordKill_Spell";


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


            GuiPresentation guiPresentation_Spell = new GuiPresentation();
            guiPresentation_Spell.SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f));
            guiPresentation_Spell.SetDescription("Spell/&" + BaseString + text + "_Description");
            guiPresentation_Spell.SetTitle("Spell/&" + BaseString + text + "_Title");
            guiPresentation_Spell.SetSpriteReference(DatabaseHelper.SpellDefinitions.Disintegrate.GuiPresentation.SpriteReference);
            guiPresentation_Spell.SetSymbolChar("221E");

            SpellBuilder PowerWordKill_Spell = new SpellBuilder(BaseString + text, GuidHelper.Create(new System.Guid(BaseGuid), BaseString + text).ToString());
            PowerWordKill_Spell.SetGuiPresentation(guiPresentation_Spell);
            PowerWordKill_Spell.SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation);
            PowerWordKill_Spell.SetSpellLevel(9);
            PowerWordKill_Spell.SetCastingTime(RuleDefinitions.ActivationTime.Action);
            PowerWordKill_Spell.SetVerboseComponent(true);
            PowerWordKill_Spell.SetSomaticComponent(false);
            PowerWordKill_Spell.SetEffectDescription(effectDescription.Build());
            PowerWordKill_Spell.SetAiParameters(new SpellAIParameters());
            //    PowerWordKill_Spell
            SpellDefinition PowerWordKill = PowerWordKill_Spell.AddToDB();

            return PowerWordKill;

        }



        private static SpellDefinition BuildNew_Shapechange_Spell()
        {

            string text = "Shapechange_Spell";

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
            shapeChangeForm.ShapeOptions.AddRange(

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


            GuiPresentation guiPresentation_Spell = new GuiPresentation();
            guiPresentation_Spell.SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f));
            guiPresentation_Spell.SetDescription("Spell/&" + BaseString + text + "_Description");
            guiPresentation_Spell.SetTitle("Spell/&" + BaseString + text + "_Title");
            guiPresentation_Spell.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerDruidWildShape.GuiPresentation.SpriteReference);
            guiPresentation_Spell.SetSymbolChar("221E");

            SpellBuilder Shapechange_Spell = new SpellBuilder(BaseString + text, GuidHelper.Create(new System.Guid(BaseGuid), BaseString + text).ToString());
            Shapechange_Spell.SetGuiPresentation(guiPresentation_Spell);
            Shapechange_Spell.SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation);
            Shapechange_Spell.SetSpellLevel(9);
            Shapechange_Spell.SetCastingTime(RuleDefinitions.ActivationTime.Action);
            Shapechange_Spell.SetVerboseComponent(true);
            Shapechange_Spell.SetSomaticComponent(false);
            Shapechange_Spell.SetEffectDescription(effectDescription.Build());
            Shapechange_Spell.SetAiParameters(new SpellAIParameters());
            //    Shapechange_Spell
            Shapechange_Spell.SetConcentration();
            SpellDefinition Shapechange = Shapechange_Spell.AddToDB();
            Shapechange.SetRequiresConcentration(true);

            return Shapechange;

        }


        private static SpellDefinition BuildNew_TimeStop_Spell()
        {

            string text = "TimeStop_Spell";

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

            GuiPresentation guiPresentation_Spell = new GuiPresentation();
            guiPresentation_Spell.SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f));
            guiPresentation_Spell.SetDescription("Spell/&" + BaseString + text + "_Description");
            guiPresentation_Spell.SetTitle("Spell/&" + BaseString + text + "_Title");
            guiPresentation_Spell.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerDomainLawWordOfLaw.GuiPresentation.SpriteReference);
            guiPresentation_Spell.SetSymbolChar("221E");

            SpellBuilder TimeStop_Spell = new SpellBuilder(BaseString + text, GuidHelper.Create(new System.Guid(BaseGuid), BaseString + text).ToString());
            TimeStop_Spell.SetGuiPresentation(guiPresentation_Spell);
            TimeStop_Spell.SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation);
            TimeStop_Spell.SetSpellLevel(9);
            TimeStop_Spell.SetCastingTime(RuleDefinitions.ActivationTime.Action);
            TimeStop_Spell.SetVerboseComponent(true);
            TimeStop_Spell.SetSomaticComponent(false);
            TimeStop_Spell.SetEffectDescription(effectDescription.Build());
            TimeStop_Spell.SetAiParameters(new SpellAIParameters());
            //    TimeStop_Spell
            SpellDefinition TimeStop = TimeStop_Spell.AddToDB();


            return TimeStop;

        }

        internal class TimeStopCondition_Builder : BaseDefinitionBuilder<ConditionDefinition>
        {
            const string name = "DH_Level20_TimeStop_Spellcondition";
            const string guid = "f00e592f-61c3-4cbf-a800-97596e83028d";
            string title_string = "Condition/&DH_Level20_TimeStop_Spell_Title";
            string description_string = "Condition/&DH_Level20_TimeStop_Spell_Description";
            protected TimeStopCondition_Builder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionIncapacitated, name, guid)
            {
                Definition.GuiPresentation.Title = title_string;
                Definition.GuiPresentation.Description = description_string;

                Definition.HasSpecialInterruptionOfType(RuleDefinitions.ConditionInterruption.Attacked);
                Definition.HasSpecialInterruptionOfType(RuleDefinitions.ConditionInterruption.Damaged);
                Definition.SpecialInterruptions.Add(RuleDefinitions.ConditionInterruption.Attacked);
                Definition.SpecialInterruptions.Add(RuleDefinitions.ConditionInterruption.Damaged);
                Definition.SetInterruptionDamageThreshold(1);
                Definition.SetInterruptionRequiresSavingThrow(false);
            }
            public static ConditionDefinition CreateAndAddToDB(string name, string guid)
                => new TimeStopCondition_Builder(name, guid).AddToDB();
            public static ConditionDefinition TimeStopCondition = CreateAndAddToDB(name, guid);
        }

        private static SpellDefinition BuildNew_Weird_Spell()
        {

            string text = "Weird_Spell";



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


            GuiPresentation guiPresentation_Spell = new GuiPresentation();
            guiPresentation_Spell.SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f));
            guiPresentation_Spell.SetDescription("Spell/&" + BaseString + text + "_Description");
            guiPresentation_Spell.SetTitle("Spell/&" + BaseString + text + "_Title");
            guiPresentation_Spell.SetSpriteReference(DatabaseHelper.SpellDefinitions.PhantasmalKiller.GuiPresentation.SpriteReference);
            guiPresentation_Spell.SetSymbolChar("221E");

            SpellBuilder Weird_Spell = new SpellBuilder(BaseString + text, GuidHelper.Create(new System.Guid(BaseGuid), BaseString + text).ToString());
            Weird_Spell.SetGuiPresentation(guiPresentation_Spell);
            Weird_Spell.SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation);
            Weird_Spell.SetSpellLevel(9);
            Weird_Spell.SetCastingTime(RuleDefinitions.ActivationTime.Action);
            Weird_Spell.SetVerboseComponent(true);
            Weird_Spell.SetSomaticComponent(false);
            Weird_Spell.SetEffectDescription(effectDescription.Build());
            Weird_Spell.SetAiParameters(new SpellAIParameters());
            //    Weird_Spell
            Weird_Spell.SetConcentration();
            SpellDefinition Weird = Weird_Spell.AddToDB();

            return Weird;


        }




        internal class WeirdCondition_Builder : BaseDefinitionBuilder<ConditionDefinition>
        {
            const string name = "DH_Level20_Weird_Spellcondition";
            const string guid = "0f76e7e1-4490-4ee8-a13f-a4a967ba1c08";
            string title_string = "Condition/&DH_Level20_Weird_Spell_Title";
            string description_string = "Condition/&DH_Level20_Weird_Spell_Description";
            protected WeirdCondition_Builder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionFrightenedPhantasmalKiller, name, guid)
            {
                Definition.GuiPresentation.Title = title_string;
                Definition.GuiPresentation.Description = description_string;

                // weird condition is the same as phantasma killer condition , just for more people
            }
            public static ConditionDefinition CreateAndAddToDB(string name, string guid)
                => new WeirdCondition_Builder(name, guid).AddToDB();
            public static ConditionDefinition WeirdCondition = CreateAndAddToDB(name, guid);
        }

/*
        private static SpellDefinition BuildNew_Wish_overall_Spell()
        {

            string text = "Wish_overall_Spell";



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


            GuiPresentation guiPresentation_Spell = new GuiPresentation();
            guiPresentation_Spell.SetColor(new UnityEngine.Color(1f, 1f, 1f, 1f));
            guiPresentation_Spell.SetDescription("Spell/&" + BaseString + text + "_Description");
            guiPresentation_Spell.SetTitle("Spell/&" + BaseString + text + "_Title");
            guiPresentation_Spell.SetSpriteReference(DatabaseHelper.SpellDefinitions.Banishment.GuiPresentation.SpriteReference);
            guiPresentation_Spell.SetSymbolChar("221E");

            SpellBuilder Wish_overall_Spell = new SpellBuilder(BaseString + text, GuidHelper.Create(new System.Guid(BaseGuid), BaseString + text).ToString());
            Wish_overall_Spell.SetGuiPresentation(guiPresentation_Spell);
            Wish_overall_Spell.SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation);
            Wish_overall_Spell.SetSpellLevel(9);
            Wish_overall_Spell.SetCastingTime(RuleDefinitions.ActivationTime.Action);
            Wish_overall_Spell.SetVerboseComponent(true);
            Wish_overall_Spell.SetSomaticComponent(false);
            Wish_overall_Spell.SetEffectDescription(effectDescription.Build());
            Wish_overall_Spell.SetAiParameters(new SpellAIParameters());
            //    Wish_overall_Spell
            Wish_overall_Spell.SetSubSpells(
                new List<SpellDefinition>
                {

                });
           SpellDefinition Wish_overall = Wish_overall_Spell.AddToDB();

            return Wish_overall;

        }
*/


    }
}
