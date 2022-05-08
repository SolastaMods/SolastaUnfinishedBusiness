using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.Utils;
using SolastaModApi;
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
            return SpellWithCustomFeaturesBuilder
                .Create("EWSunlightBlade", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Spell,
                    DatabaseHelper.SpellDefinitions.FlameBlade.GuiPresentation.SpriteReference) //TODO: change to proper icon
                .SetSpellLevel(0)
                .SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolEvocation)
                .SetSomaticComponent(true)
                .SetVerboseComponent(true)
                .AddCustomFeature(PerformAttackAfterMagicEffectUse.MeleeAttack)
                .AddCustomFeature(CustomSpellEffectLevel.ByCasterLevel)
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
                        true,
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
                                .Create("EWSunlightBladeHighlighted", DefinitionBuilder.CENamespaceGuid)
                                .SetGuiPresentation(Category.Condition,
                                    DatabaseHelper.SpellDefinitions.FlameBlade.GuiPresentation.SpriteReference)
                                .SetSpecialInterruptions(RuleDefinitions.ConditionInterruption.Attacks)
                                .SetSilent(Silent.None)
                                .SetTurnOccurence(RuleDefinitions.TurnOccurenceType.EndOfTurn)
                                .SetDuration(RuleDefinitions.DurationType.Round, 1)
                                .SetFeatures(FeatureDefinitionAdditionalDamageBuilder
                                    .Create("EWSunlightBladeHighlightedDamage", DefinitionBuilder.CENamespaceGuid)
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
                                        RuleDefinitions.DamageTypeFire,
                                        RuleDefinitions.AdditionalDamageAdvancement.SlotLevel,
                                        DiceByRankMaker.MakeBySteps(start: 0, step: 5, increment: 1)
                                    )
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
