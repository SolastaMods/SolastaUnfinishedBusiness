using System;
using System.Collections.Generic;
using SolastaModApi;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi.Extensions;
using static RuleDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionConditionAffinitys;
using static SolastaModApi.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;
using static SolastaModApi.DatabaseHelper.ConditionDefinitions;
using static SolastaModApi.DatabaseHelper.SpellListDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionProficiencys;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionMovementAffinitys;


//******************************************************************************************
//  DO NOT REFACTOR OR CHANGE WITHOUT TESTING OR TAKING RESPOSBILITY FOR CODE GOING FORWARD
//******************************************************************************************

namespace SolastaCommunityExpansion.Classes.Warlock.Subclasses
{
    public static class DHWarlockSubclassToadKingPatron
    {
        public static CharacterSubclassDefinition Build()
        {

            SpellListDefinition ToadKingExpandedSpelllist = SpellListDefinitionBuilder
                .Create(SpellListPaladin, "ToadKingExpandedSpelllist", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation("ToadKingExpandedSpelllist", Category.Feature)
                .SetGuiPresentationNoContent()
                .ClearSpells()
               // .SetSpellsAtLevel(1, Grease, DetectPoisonAndDisease)
               // .SetSpellsAtLevel(2, AcidArrow, SpiderClimb)
               // .SetSpellsAtLevel(3, Slow, StinkingCloud)
               // .SetSpellsAtLevel(4, BlackTentacles,FreedomOfMovement)
               // .SetSpellsAtLevel(5, Contagion,InsectPlague)
                .SetMaxSpellLevel(5, false)
                .AddToDB();
            ToadKingExpandedSpelllist.ClearSpellsByLevel();
            ToadKingExpandedSpelllist.SpellsByLevel.AddRange(new List<SpellListDefinition.SpellsByLevelDuplet>()
             {
               //  new SpellListDefinition.SpellsByLevelDuplet
               //  {
               //      Level =0,
               //      Spells = new List<SpellDefinition>
               //      {
               //      }
               //  },
                 new SpellListDefinition.SpellsByLevelDuplet
                 {
                     Level =1,
                     Spells = new List<SpellDefinition>
                     {
                         DatabaseHelper.SpellDefinitions.DetectPoisonAndDisease,
                         DatabaseHelper.SpellDefinitions.Longstrider
                     }
                 },
                 new SpellListDefinition.SpellsByLevelDuplet
                 {
                     Level =2,
                     Spells = new List<SpellDefinition>
                     {
                         DatabaseHelper.SpellDefinitions.AcidArrow,
                         DatabaseHelper.SpellDefinitions.SpiderClimb
                     }
                 },
                 new SpellListDefinition.SpellsByLevelDuplet
                 {
                     Level =3,
                     Spells = new List<SpellDefinition>
                     {
                         DatabaseHelper.SpellDefinitions.StinkingCloud,
                         DatabaseHelper.SpellDefinitions.Slow
                     }
                 },
                 new SpellListDefinition.SpellsByLevelDuplet
                 {
                     Level =4,
                     Spells = new List<SpellDefinition>
                     {
                         DatabaseHelper.SpellDefinitions.FreedomOfMovement,
                         DatabaseHelper.SpellDefinitions.BlackTentacles
                     }
                 },
                 new SpellListDefinition.SpellsByLevelDuplet
                 {
                     Level =5,
                     Spells = new List<SpellDefinition>
                     {
                         DatabaseHelper.SpellDefinitions.Contagion,
                         DatabaseHelper.SpellDefinitions.InsectPlague
                     }
                 },

             });

            FeatureDefinitionMagicAffinity ToadKingExpandedSpelllistAfinity = FeatureDefinitionMagicAffinityBuilder
                .Create("ToadKingExpandedSpelllistAfinity", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation("ToadKingExpandedSpelllistAfinity", Category.Feature)
                .SetExtendedSpellList(ToadKingExpandedSpelllist)
                .AddToDB();

            FeatureDefinitionConditionAffinity ToadKingConditionAffinityPoisonImmunity = FeatureDefinitionConditionAffinityBuilder
                 .Create(ConditionAffinityPoisonImmunity, "ToadKingConditionAffinityPoisonImmunity", DefinitionBuilder.CENamespaceGuid)
                 .AddToDB();
            ToadKingConditionAffinityPoisonImmunity.GuiPresentation.SetTitle("Feature/&ToadKingPoisonConditionAffinityTitle");

            FeatureDefinitionMovementAffinity ToadKingMovementAffinityJump = FeatureDefinitionMovementAffinityBuilder
                .Create(MovementAffinityJump, "ToadKingMovementAffinityJump", DefinitionBuilder.CENamespaceGuid)
                .AddToDB();
            ToadKingMovementAffinityJump.GuiPresentation.SetTitle("Feature/&ToadKingJumpTitle");

            FeatureDefinitionMovementAffinity ToadKingMovementAffinitySpiderClimb = FeatureDefinitionMovementAffinityBuilder
                 .Create(MovementAffinitySpiderClimb, "ToadKingMovementAffinitySpiderClimb", DefinitionBuilder.CENamespaceGuid)
                 .AddToDB();
            ToadKingMovementAffinitySpiderClimb.GuiPresentation.SetTitle("Feature/&ToadKingStickyFeetTitle");

            FeatureDefinitionDamageAffinity ToadKingDamageAffinityPoisonImmunity = FeatureDefinitionDamageAffinityBuilder
                 .Create(DamageAffinityPoisonImmunity, "ToadKingDamageAffinityPoisonImmunity", DefinitionBuilder.CENamespaceGuid)
                 .AddToDB();
            ToadKingDamageAffinityPoisonImmunity.GuiPresentation.SetTitle("Feature/&ToadKingPoisonDamageAffinityTitle");


            FeatureDefinitionPower Croak = FeatureDefinitionPowerBuilder
                .Create("ToadKingCroak", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Power)
                .Configure(
                       1,
                       RuleDefinitions.UsesDetermination.ProficiencyBonus,
                       AttributeDefinitions.Charisma,
                       RuleDefinitions.ActivationTime.Action,
                       1,
                       RuleDefinitions.RechargeRate.AtWill,
                       false,
                       false,
                       AttributeDefinitions.Charisma,
                       new EffectDescriptionBuilder()
                            .AddEffectForm(
                                new EffectFormBuilder().SetDamageForm(
                                    false,
                                    DieType.D6,
                                    DamageTypeThunder,
                                    0,
                                    DieType.D6,
                                    1,
                                    RuleDefinitions.HealFromInflictedDamage.Never,
                                    new List<RuleDefinitions.TrendInfo>())
                                .Build())
                            .SetTargetingData(
                                    RuleDefinitions.Side.All,
                                    RuleDefinitions.RangeType.Self,
                                    1,
                                    RuleDefinitions.TargetType.CubeWithOffset,
                                    3,
                                    2,
                                    ActionDefinitions.ItemSelectionType.None)
                            .SetEffectAdvancement(
                                RuleDefinitions.EffectIncrementMethod.CasterLevelTable,
                                5,
                                0,
                                1,
                                0,
                                0,
                                0,
                                0,
                                0,
                                0,
                                RuleDefinitions.AdvancementDuration.None
                                )
                            .SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.Thunderwave.EffectDescription.EffectParticleParameters)
                            .Build()
                       ,
                       true)
                .AddToDB() ;
            Croak.GuiPresentation.SetSpriteReference(PowerWindShelteringBreeze.GuiPresentation.SpriteReference);

            FeatureDefinitionPower hallucinogenicToxinPower = FeatureDefinitionPowerBuilder
                .Create("ToadKingHallucinogenicToxinPower", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Power)
                .Configure(
                       1,
                       RuleDefinitions.UsesDetermination.ProficiencyBonus,
                       AttributeDefinitions.Charisma,
                       RuleDefinitions.ActivationTime.Action,
                       1,
                       RuleDefinitions.RechargeRate.AtWill,
                       false,
                       false,
                       AttributeDefinitions.Charisma,
                       new EffectDescriptionBuilder()
                            .AddEffectForm(
                                new EffectFormBuilder().SetConditionForm(
                                    ConditionCharmedByHypnoticPattern,
                                    ConditionForm.ConditionOperation.Add,
                                    false,
                                    false,
                                    new List<ConditionDefinition>()
                                    )
                                .Build()
                                .SetHasSavingThrow(true)
                                .SetSavingThrowAffinity(EffectSavingThrowType.Negates)
                                )
                            .SetSavingThrowData(
                                true,
                                false,
                                DatabaseHelper.SmartAttributeDefinitions.Constitution.name,
                                true,
                                RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                                DatabaseHelper.SmartAttributeDefinitions.Constitution.name,
                                20,
                                false,
                                new List<SaveAffinityBySenseDescription>())
                            .SetTargetingData(
                                    RuleDefinitions.Side.Enemy,
                                    RuleDefinitions.RangeType.Touch,
                                    1,
                                    RuleDefinitions.TargetType.Individuals,
                                    1,
                                    1,
                                    ActionDefinitions.ItemSelectionType.None)
                            .Build()
                       ,
                       true)
                .AddToDB();

            FeatureDefinitionDamageAffinity hallucinogenicToxinAffinity = FeatureDefinitionDamageAffinityBuilder
                .Create(DamageAffinityPoisonResistance, "ToadKingHallucinogenicToxinAffinty", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Feature)
                .SetDamageType(DamageTypePoison)
                .SetDamageAffinityType(DamageAffinityType.Resistance)
                .SetRetaliate(hallucinogenicToxinPower, 1)
                .AddToDB();


            FeatureDefinitionPower GraspingTongue = FeatureDefinitionPowerBuilder
                .Create("ToadKingGraspingTongue", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Power)
                .Configure(
                       1,
                       RuleDefinitions.UsesDetermination.ProficiencyBonus,
                       AttributeDefinitions.Charisma,
                       RuleDefinitions.ActivationTime.Action,
                       1,
                       RuleDefinitions.RechargeRate.AtWill,
                       false,
                       false,
                       AttributeDefinitions.Charisma,
                       new EffectDescriptionBuilder()
                            .AddEffectForm(
                                new EffectFormBuilder().SetMotionForm(
                                    MotionForm.MotionType.DragToOrigin,
                                    5)
                                .Build()
                                .SetHasSavingThrow(true)
                                .SetSavingThrowAffinity(EffectSavingThrowType.Negates)
                                )
                            .SetTargetingData(
                                    RuleDefinitions.Side.All,
                                    RuleDefinitions.RangeType.Distance,
                                    6,
                                    RuleDefinitions.TargetType.Individuals,
                                    1,
                                    1,
                                    ActionDefinitions.ItemSelectionType.None)
                            .SetSavingThrowData(
                                true,
                                false,
                                DatabaseHelper.SmartAttributeDefinitions.Strength.name,
                                true,
                                RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                                DatabaseHelper.SmartAttributeDefinitions.Strength.name,
                                20,
                                false,
                                new List<SaveAffinityBySenseDescription>())
                            .SetParticleEffectParameters(PowerShadowTamerRopeGrapple.EffectDescription.EffectParticleParameters)
                            .Build()
                       ,
                       true)
                .AddToDB();

            GraspingTongue.GuiPresentation.SetSpriteReference(PowerShadowTamerRopeGrapple.GuiPresentation.SpriteReference);

            ConditionDefinition SwallowingToadCondition =  ConditionDefinitionBuilder
                .Create(ConditionSwallowingRemorhaz, "SwallowingToadCondition", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Condition)
                .SetSpecialInterruptions(ConditionInterruption.Damaged)
                .SetInterruptionDamageThreshold(15)
                .AddToDB();

            ConditionDefinition SwallowedByToadCondition = ConditionDefinitionBuilder
                .Create(ConditionSwallowedRemorhaz, "SwallowedByToadCondition", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Condition)
                .ClearRecurrentEffectForms()
                
                .AddRecurrentEffectForm(
                        new EffectFormBuilder()
                            .SetDamageForm(
                                false,
                                DieType.D6,
                                DamageTypeAcid,
                                3,
                                DieType.D6,
                                1,
                                RuleDefinitions.HealFromInflictedDamage.Never,
                                new List<RuleDefinitions.TrendInfo>())
                            .Build()
                            )
                .AddToDB();

            FeatureDefinitionPower ApplyPoison = FeatureDefinitionPowerBuilder
                .Create(PowerFunctionApplyPoison_Basic, "ToadKingBasicToxin", DefinitionBuilder.CENamespaceGuid)
                .AddToDB();
            ApplyPoison.GuiPresentation.SetTitle("Feature/&ToadKingBasicToxinTitle");

            FeatureDefinitionPower Swallow = FeatureDefinitionPowerBuilder
                .Create("ToadKingSwallow", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Power)
                .Configure(
                       1,
                       RuleDefinitions.UsesDetermination.ProficiencyBonus,
                       AttributeDefinitions.Charisma,
                       RuleDefinitions.ActivationTime.Action,
                       1,
                       RuleDefinitions.RechargeRate.AtWill,
                       false,
                       false,
                       AttributeDefinitions.Charisma,
                       new EffectDescriptionBuilder()
                            .SetTargetingData(
                                    RuleDefinitions.Side.All,
                                    RuleDefinitions.RangeType.MeleeHit,
                                    1,
                                    RuleDefinitions.TargetType.Individuals,
                                    1,
                                    1,
                                    ActionDefinitions.ItemSelectionType.None)
                            .AddEffectForm(
                                new EffectFormBuilder().SetConditionForm(
                                    SwallowedByToadCondition,
                                    ConditionForm.ConditionOperation.Add,
                                    false,
                                    false,
                                    new List<ConditionDefinition>()
                                    )
                                .Build()
                                .SetHasSavingThrow(true)
                                .SetSavingThrowAffinity(EffectSavingThrowType.Negates)
                                )
                            .AddEffectForm(
                                new EffectFormBuilder().SetConditionForm(
                                    SwallowingToadCondition,
                                    ConditionForm.ConditionOperation.Add,
                                    true,
                                    true,
                                    new List<ConditionDefinition>()
                                    )
                                .Build()
                                .SetHasSavingThrow(true)
                                .SetSavingThrowAffinity(EffectSavingThrowType.Negates)
                                )
                            .SetSavingThrowData(
                                true,
                                false,
                                DatabaseHelper.SmartAttributeDefinitions.Strength.name,
                                true,
                                RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                                DatabaseHelper.SmartAttributeDefinitions.Strength.name,
                                20,
                                false,
                                new List<SaveAffinityBySenseDescription>())
                            .SetParticleEffectParameters(PowerRemorhazSwallow.EffectDescription.EffectParticleParameters)
                            
                            .Build()
                       ,
                       true)
                .AddToDB();
            Swallow.EffectDescription.AddRestrictedCharacterSizes(CreatureSize.Tiny);
            Swallow.EffectDescription.AddRestrictedCharacterSizes(CreatureSize.Small);
            Swallow.EffectDescription.AddRestrictedCharacterSizes(CreatureSize.Medium);
            Swallow.GuiPresentation.SetSpriteReference(DatabaseHelper.MonsterAttackDefinitions.Attack_TigerDrake_Bite.GuiPresentation.SpriteReference);

            return CharacterSubclassDefinitionBuilder
                .Create("ToadKing", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation("WarlockToadKing", Category.Subclass, SorcerousDraconicBloodline.GuiPresentation.SpriteReference)
                .AddFeatureAtLevel(ToadKingExpandedSpelllistAfinity, 1)
                .AddFeatureAtLevel(ProficiencyRoguishDarkweaver, 1)
                .AddFeatureAtLevel(ApplyPoison, 1)
                .AddFeatureAtLevel(Croak, 1)
                .AddFeatureAtLevel(hallucinogenicToxinAffinity, 6)
                .AddFeatureAtLevel(ToadKingConditionAffinityPoisonImmunity, 6)
                .AddFeatureAtLevel(ToadKingMovementAffinityJump, 6)
                .AddFeatureAtLevel(ToadKingMovementAffinitySpiderClimb, 10)
                .AddFeatureAtLevel(ToadKingDamageAffinityPoisonImmunity, 10)
                .AddFeatureAtLevel(GraspingTongue, 14)
                .AddFeatureAtLevel(Swallow, 14)
                .AddToDB();

        }
    }
}
