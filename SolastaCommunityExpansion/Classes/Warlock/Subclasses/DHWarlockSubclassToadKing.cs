using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi;
using SolastaModApi.Extensions;
using static RuleDefinitions;
using static SolastaModApi.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaModApi.DatabaseHelper.ConditionDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionConditionAffinitys;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionMovementAffinitys;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionProficiencys;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;
using static SolastaModApi.DatabaseHelper.SpellListDefinitions;

namespace SolastaCommunityExpansion.Classes.Warlock.Subclasses
{
    public static class DHWarlockSubclassToadKingPatron
    {
        public static CharacterSubclassDefinition Build()
        {
            var ToadKingExpandedSpelllist = SpellListDefinitionBuilder
                .Create(SpellListPaladin, "ToadKingExpandedSpelllist", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation("ToadKingExpandedSpelllist", Category.Feature)
                .SetGuiPresentationNoContent()
                .ClearSpells()
                .SetSpellsAtLevel(1, DetectPoisonAndDisease, Longstrider)
                .SetSpellsAtLevel(2, AcidArrow, SpiderClimb)
                .SetSpellsAtLevel(3, Slow, StinkingCloud)
                .SetSpellsAtLevel(4, BlackTentacles, FreedomOfMovement)
                .SetSpellsAtLevel(5, Contagion, InsectPlague)
                .FinalizeSpells()
                .AddToDB();

            var ToadKingExpandedSpelllistAfinity = FeatureDefinitionMagicAffinityBuilder
                .Create("ToadKingExpandedSpelllistAfinity", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation("ToadKingExpandedSpelllistAfinity", Category.Feature)
                .SetExtendedSpellList(ToadKingExpandedSpelllist)
                .AddToDB();

            var ToadKingConditionAffinityPoisonImmunity = FeatureDefinitionConditionAffinityBuilder
                .Create(ConditionAffinityPoisonImmunity, "ToadKingConditionAffinityPoisonImmunity",
                    DefinitionBuilder.CENamespaceGuid)
                .AddToDB();
            ToadKingConditionAffinityPoisonImmunity.GuiPresentation.SetTitle(
                "Feature/&ToadKingPoisonConditionAffinityTitle");

            var ToadKingMovementAffinityJump = FeatureDefinitionMovementAffinityBuilder
                .Create(MovementAffinityJump, "ToadKingMovementAffinityJump", DefinitionBuilder.CENamespaceGuid)
                .AddToDB();
            ToadKingMovementAffinityJump.GuiPresentation.SetTitle("Feature/&ToadKingJumpTitle");

            var ToadKingMovementAffinitySpiderClimb = FeatureDefinitionMovementAffinityBuilder
                .Create(MovementAffinitySpiderClimb, "ToadKingMovementAffinitySpiderClimb",
                    DefinitionBuilder.CENamespaceGuid)
                .AddToDB();
            ToadKingMovementAffinitySpiderClimb.GuiPresentation.SetTitle("Feature/&ToadKingStickyFeetTitle");

            var ToadKingDamageAffinityPoisonImmunity = FeatureDefinitionDamageAffinityBuilder
                .Create(DamageAffinityPoisonImmunity, "ToadKingDamageAffinityPoisonImmunity",
                    DefinitionBuilder.CENamespaceGuid)
                .AddToDB();
            ToadKingDamageAffinityPoisonImmunity.GuiPresentation.SetTitle("Feature/&ToadKingPoisonDamageAffinityTitle");


            var Croak = FeatureDefinitionPowerBuilder
                .Create("ToadKingCroak", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Power)
                .Configure(
                    1,
                    UsesDetermination.ProficiencyBonus,
                    AttributeDefinitions.Charisma,
                    ActivationTime.Action,
                    1,
                    RechargeRate.AtWill,
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
                                    HealFromInflictedDamage.Never,
                                    new List<TrendInfo>())
                                .Build())
                        .SetTargetingData(
                            Side.All,
                            RangeType.Self,
                            1,
                            TargetType.CubeWithOffset,
                            3,
                            2)
                        .SetEffectAdvancement(
                            EffectIncrementMethod.CasterLevelTable,
                            5,
                            0,
                            1
                        )
                        .SetParticleEffectParameters(Thunderwave.EffectDescription.EffectParticleParameters)
                        .Build()
                    ,
                    true)
                .AddToDB();
            Croak.GuiPresentation.SetSpriteReference(PowerWindShelteringBreeze.GuiPresentation.SpriteReference);

            var hallucinogenicToxinPower = FeatureDefinitionPowerBuilder
                .Create("ToadKingHallucinogenicToxinPower", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Power)
                .Configure(
                    1,
                    UsesDetermination.ProficiencyBonus,
                    AttributeDefinitions.Charisma,
                    ActivationTime.Action,
                    1,
                    RechargeRate.AtWill,
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
                            EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                            DatabaseHelper.SmartAttributeDefinitions.Constitution.name,
                            20,
                            false,
                            new List<SaveAffinityBySenseDescription>())
                        .SetTargetingData(
                            Side.Enemy,
                            RangeType.Touch,
                            1,
                            TargetType.Individuals)
                        .Build()
                    ,
                    true)
                .AddToDB();

            var hallucinogenicToxinAffinity = FeatureDefinitionDamageAffinityBuilder
                .Create(DamageAffinityPoisonResistance, "ToadKingHallucinogenicToxinAffinty",
                    DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Feature)
                .SetDamageType(DamageTypePoison)
                .SetDamageAffinityType(DamageAffinityType.Resistance)
                .SetRetaliate(hallucinogenicToxinPower, 1)
                .AddToDB();


            var GraspingTongue = FeatureDefinitionPowerBuilder
                .Create("ToadKingGraspingTongue", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Power)
                .Configure(
                    1,
                    UsesDetermination.ProficiencyBonus,
                    AttributeDefinitions.Charisma,
                    ActivationTime.Action,
                    1,
                    RechargeRate.AtWill,
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
                            Side.All,
                            RangeType.Distance,
                            6,
                            TargetType.Individuals)
                        .SetSavingThrowData(
                            true,
                            false,
                            DatabaseHelper.SmartAttributeDefinitions.Strength.name,
                            true,
                            EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                            DatabaseHelper.SmartAttributeDefinitions.Strength.name,
                            20,
                            false,
                            new List<SaveAffinityBySenseDescription>())
                        .SetParticleEffectParameters(PowerShadowTamerRopeGrapple.EffectDescription
                            .EffectParticleParameters)
                        .Build()
                    ,
                    true)
                .AddToDB();

            GraspingTongue.GuiPresentation.SetSpriteReference(PowerShadowTamerRopeGrapple.GuiPresentation
                .SpriteReference);

            var SwallowingToadCondition = ConditionDefinitionBuilder
                .Create(ConditionSwallowingRemorhaz, "SwallowingToadCondition", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Condition)
                .SetSpecialInterruptions(ConditionInterruption.Damaged)
                .SetInterruptionDamageThreshold(15)
                .AddToDB();

            var SwallowedByToadCondition = ConditionDefinitionBuilder
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
                            HealFromInflictedDamage.Never,
                            new List<TrendInfo>())
                        .Build()
                )
                .AddToDB();

            var ApplyPoison = FeatureDefinitionPowerBuilder
                .Create(PowerFunctionApplyPoison_Basic, "ToadKingBasicToxin", DefinitionBuilder.CENamespaceGuid)
                .AddToDB();
            ApplyPoison.GuiPresentation.SetTitle("Feature/&ToadKingBasicToxinTitle");

            var Swallow = FeatureDefinitionPowerBuilder
                .Create("ToadKingSwallow", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Power)
                .Configure(
                    1,
                    UsesDetermination.ProficiencyBonus,
                    AttributeDefinitions.Charisma,
                    ActivationTime.Action,
                    1,
                    RechargeRate.AtWill,
                    false,
                    false,
                    AttributeDefinitions.Charisma,
                    new EffectDescriptionBuilder()
                        .SetTargetingData(
                            Side.All,
                            RangeType.MeleeHit,
                            1,
                            TargetType.Individuals)
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
                            EffectDifficultyClassComputation.AbilityScoreAndProficiency,
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
            Swallow.GuiPresentation.SetSpriteReference(DatabaseHelper.MonsterAttackDefinitions.Attack_TigerDrake_Bite
                .GuiPresentation.SpriteReference);

            return CharacterSubclassDefinitionBuilder
                .Create("ToadKing", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation("WarlockToadKing", Category.Subclass,
                    SorcerousDraconicBloodline.GuiPresentation.SpriteReference)
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
