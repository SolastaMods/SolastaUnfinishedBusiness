using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.Utils;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static ConditionOperationDescription;
using static SolastaCommunityExpansion.Classes.Warlock.WarlockSpells;
using static SolastaCommunityExpansion.Models.SpellsContext;

namespace SolastaCommunityExpansion.Spells
{
    internal static class EWSpells
    {
        private static SpellDefinition _sunlightBlade;
        internal static SpellDefinition SunlightBlade => _sunlightBlade ??= BuildSunlightBlade();


        internal static void Register()
        {
            RegisterSpell(SunlightBlade, 1, WarlockSpellList);
        }

        private static SpellDefinition BuildSunlightBlade()
        {
            var highlight = new ConditionOperationDescription()
                .SetHasSavingThrow(false)
                .SetOperation(ConditionOperation.Add)
                .SetConditionDefinition(ConditionDefinitionBuilder
                    .Create(DatabaseHelper.ConditionDefinitions.ConditionHighlighted, "EWSunlightBladeHighlighted", DefinitionBuilder.CENamespaceGuid)
                    .SetSpecialInterruptions(RuleDefinitions.ConditionInterruption.Attacked)
                    .SetDuration(RuleDefinitions.DurationType.Round, 1)
                    .SetTurnOccurence(RuleDefinitions.TurnOccurenceType.StartOfTurn)
                    .SetSpecialDuration(true)
                    .AddToDB());
            
            var dimLight = new LightSourceForm()
                .SetBrightRange(0)
                .SetDimAdditionalRange(2)
                .SetLightSourceType(RuleDefinitions.LightSourceType.Basic)
                .SetColor(new Color(0.9f, 0.8f, 0.4f));

            dimLight.SetGraphicsPrefabReference(DatabaseHelper.FeatureDefinitionAdditionalDamages
                .AdditionalDamageBrandingSmite.LightSourceForm.GetField<AssetReference>("graphicsPrefabReference"));
            
            return SpellDefinitionBuilder
                .Create("EWSunlightBlade", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Spell,
                    CustomIcons.CreateAssetReferenceSprite("SunlightBlade", Properties.Resources.SunlightBlade, 128, 128))
                .SetSpellLevel(0)
                .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolEvocation)
                .SetSomaticComponent(true)
                .SetVerboseComponent(false)
                .SetCustomSubFeatures(
                    PerformAttackAfterMagicEffectUse.MeleeAttack,
                    CustomSpellEffectLevel.ByCasterLevel
                )
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetEffectDescription(new EffectDescriptionBuilder()
                    .SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.ScorchingRay)
                    .SetTargetingData(
                        RuleDefinitions.Side.Enemy,
                        RuleDefinitions.RangeType.Touch,
                        1,
                        RuleDefinitions.TargetType.Individuals
                    )
                    .SetSavingThrowData(
                        false,
                        false,
                        AttributeDefinitions.Dexterity,
                        true,
                        RuleDefinitions.EffectDifficultyClassComputation.SpellCastingFeature,
                        AttributeDefinitions.Charisma
                    )
                    .SetEffectAdvancement(
                        RuleDefinitions.EffectIncrementMethod.CasterLevelTable,
                        additionalDicePerIncrement: 1,
                        incrementMultiplier: 1
                    )
                    .SetDurationData(RuleDefinitions.DurationType.Round, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn)
                    .SetEffectForms(new EffectFormBuilder()
                        .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.None)
                        .SetConditionForm(ConditionDefinitionBuilder
                                .Create("EWSunlightBladeDamage", DefinitionBuilder.CENamespaceGuid)
                                .SetGuiPresentation(Category.Condition)
                                .SetSpecialInterruptions(RuleDefinitions.ConditionInterruption.Attacks)
                                .SetSilent(Silent.WhenAddedOrRemoved)
                                .SetTurnOccurence(RuleDefinitions.TurnOccurenceType.EndOfTurn)
                                .SetDuration(RuleDefinitions.DurationType.Round, 1)
                                .SetFeatures(FeatureDefinitionAdditionalDamageBuilder
                                    .Create("EWSunlightBladeDamageBonus", DefinitionBuilder.CENamespaceGuid)
                                    .Configure(
                                        "SunlightBlade",
                                        RuleDefinitions.FeatureLimitedUsage.None,
                                        RuleDefinitions.AdditionalDamageValueDetermination.Die,
                                        RuleDefinitions.AdditionalDamageTriggerCondition.AlwaysActive,
                                        RuleDefinitions.AdditionalDamageRequiredProperty.MeleeWeapon,
                                        true,
                                        RuleDefinitions.DieType.D8,
                                        1,
                                        RuleDefinitions.AdditionalDamageType.Specific,
                                        RuleDefinitions.DamageTypeRadiant,
                                        RuleDefinitions.AdditionalDamageAdvancement.SlotLevel,
                                        DiceByRankMaker.MakeBySteps(start: 0, step: 5, increment: 1)
                                    )
                                    .SetConditionOperations(highlight)
                                    .SetAddLightSource(true)
                                    .SetLightSourceForm(dimLight)
                                    .AddToDB()
                                )
                                .AddToDB(),
                            ConditionForm.ConditionOperation.Add,
                            true,
                            false
                        )
                        .Build()
                    )
                    .Build()
                )
                .AddToDB();
        }
    }
}
