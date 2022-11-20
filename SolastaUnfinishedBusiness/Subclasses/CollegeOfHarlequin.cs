using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Models;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class CollegeOfHarlequin : AbstractSubclass
{
    internal CollegeOfHarlequin()
    {
        var powerCombatInspiration = FeatureDefinitionPowerBuilder
            .Create("PowerCombatInspiration")
            .SetGuiPresentation("CombatInspiration", Category.Feature,
                SpellDefinitions.MagicWeapon.GuiPresentation.spriteReference)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.BardicInspiration)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self)
                    .SetEffectForms(
                        new CombatInspirationEffectForm(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                ConditionDefinitionBuilder
                                    .Create("ConditionFightingAbilityEnhanced")
                                    .SetGuiPresentation("ConditionFightingAbilityEnhanced", Category.Condition)
                                    .AddFeatures(
                                        FeatureDefinitionAttributeModifierBuilder
                                            .Create("CombatInspirationArmorClassEnhancement")
                                            .SetGuiPresentation("CombatInspirationArmorClassEnhancement",
                                                Category.Feature)
                                            .SetModifier(
                                                FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                                                AttributeDefinitions.ArmorClass)
                                            .AddToDB()
                                        ,
                                        FeatureDefinitionMovementAffinityBuilder
                                            .Create("CombatInspirationMovementEnhancement")
                                            .SetGuiPresentation("CombatInspirationMovementEnhancement",
                                                Category.Feature)
                                            .SetCustomSubFeatures(new UseBardicDieRollForSpeedModifier())
                                            .AddToDB(),
                                        FeatureDefinitionAttackModifierBuilder
                                            .Create("CombatInspirationAttackEnhancement")
                                            .SetGuiPresentation("CombatInspirationAttackEnhancement", Category.Feature)
                                            .SetCustomSubFeatures(new AddBardicDieRollToAttackAndDamage())
                                            .AddToDB()
                                    )
                                    .SetAmountOrigin(ExtraOriginOfAmount.SourceRollBardicDie)
                                    .AddToDB(),
                                ConditionForm.ConditionOperation.Add
                            )
                            .Build()
                        // roll bardic die and store to global cache
                    )
                    .Build()
            )
            /*.SetCustomSubFeatures(new RemoveBardicDieRollAtStartOfTurn())*/
            .AddToDB();

        var powerTerrificPerformance = FeatureDefinitionPowerBuilder
            .Create("PowerTerrificPerformance")
            .SetGuiPresentation("TerrificPerformance", Category.Feature)
            .SetUsesFixed(ActivationTime.OnReduceCreatureToZeroHPAuto)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Self, 6, TargetType.PerceivingWithinDistance)
                    .ExcludeCaster()
                    .SetDurationData(DurationType.Round, 1)
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetParticleEffectParameters(SpellDefinitions.Fear.effectDescription.effectParticleParameters)
                    .SetEffectForms(
                        new EffectForm()
                        {
                            ConditionForm = new ConditionForm()
                            {
                                ConditionDefinition = ConditionDefinitions.ConditionFrightened,
                                operation = ConditionForm.ConditionOperation.Add
                            }
                        },
                        new EffectForm()
                        {
                            ConditionForm = new ConditionForm()
                            {
                                ConditionDefinition =
                                    ConditionDefinitions.ConditionPatronHiveWeakeningPheromones,
                                operation = ConditionForm.ConditionOperation.Add
                            }
                        }
                    )
                    .Build()
            )
            .AddToDB();

        var powerImprovedCombatInspiration = FeatureDefinitionPowerBuilder
            .Create("PowerImprovedCombatInspiration")
            .SetGuiPresentation("ImproveCombatInspiration", Category.Feature)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetEffectForms(new ImprovedCombatInspirationEffectForm())
                    .SetParticleEffectParameters(FeatureDefinitionPowers.PowerFighterActionSurge.EffectDescription
                        .effectParticleParameters)
                    .Build()
            )
            .SetUsesFixed(ActivationTime.OnReduceCreatureToZeroHPAuto)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("CollegeOfHarlequin")
            .SetOrUpdateGuiPresentation("CollegeOfHarlequin", Category.Subclass, CharacterSubclassDefinitions.RoguishShadowCaster)
            .AddFeaturesAtLevel(3, powerCombatInspiration, powerTerrificPerformance,
                FeatureDefinitionProficiencys.ProficiencyFighterWeapon,
                FeatureDefinitionMagicAffinitys.MagicAffinityBattleMagic)
            .AddFeaturesAtLevel(6,
                FeatureDefinitionAttributeModifiers.AttributeModifierFighterExtraAttack,
                powerImprovedCombatInspiration)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceBardColleges;

    private sealed class CombatInspirationEffectForm : CustomEffectForm
    {
        public CombatInspirationEffectForm()
        {
        }

        internal override void ApplyForm(RulesetImplementationDefinitions.ApplyFormsParams formsParams,
            List<string> effectiveDamageTypes, bool retargeting, bool proxyOnly,
            bool forceSelfConditionOnly, EffectApplication effectApplication = EffectApplication.All,
            List<EffectFormFilter> filters = null)
        {
            Main.Log("DEBUG: CombatInspirationEffectForm Apply Form", true);
            
            ServiceRepository.GetService<IRulesetEntityService>()
                .TryGetEntityByGuid(formsParams.sourceCharacter.guid, out var rulesetEntity);
            if (rulesetEntity is not RulesetCharacterHero hero ||
                hero.GetBardicInspirationDieValue() == DieType.D1)
            {
                Main.Log("DEBUG: CombatInspirationEffectForm Apply Form Return", true);
                return;
            }

            var dieType = hero.GetBardicInspirationDieValue();
            var dieRoll = RollDie(dieType, AdvantageType.None, out int _, out int _);

            var console = Gui.Game.GameConsole;
            var entry = new GameConsoleEntry("Feedback/&BardicInspirationUsedToBoostCombatAbility",
                console.consoleTableDefinition);
            console.AddCharacterEntry(formsParams.sourceCharacter, entry);
            entry.AddParameter(ConsoleStyleDuplet.ParameterType.Positive, Gui.FormatDieTitle(dieType));
            entry.AddParameter(ConsoleStyleDuplet.ParameterType.Positive, dieRoll.ToString());
            console.AddEntry(entry);

            Global.SetBardicRoll(formsParams.sourceCharacter.guid, dieRoll);
        }
    }

    private sealed class ImprovedCombatInspirationEffectForm : CustomEffectForm
    {
        internal override void ApplyForm(RulesetImplementationDefinitions.ApplyFormsParams formsParams,
            List<string> effectiveDamageTypes, bool retargeting, bool proxyOnly,
            bool forceSelfConditionOnly, EffectApplication effectApplication = EffectApplication.All,
            List<EffectFormFilter> filters = null)
        {
            Main.Log("DEBUG@: ImprovedCombatInspirationEffectForm", true);
            ServiceRepository.GetService<IRulesetEntityService>()
                .TryGetEntityByGuid(formsParams.sourceCharacter.guid, out var rulesetEntity);
            if (rulesetEntity is not RulesetCharacterHero hero)
            {
                return;
            }

            if (hero.usedBardicInspiration > 0)
            {
                hero.usedBardicInspiration -= 1;
            }
        }
    }

    internal sealed class UseBardicDieRollForSpeedModifier
    {
    }

    private sealed class AddBardicDieRollToAttackAndDamage : IModifyAttackModeForWeapon
    {
        public void ModifyAttackMode(RulesetCharacter character, [CanBeNull] RulesetAttackMode attackMode)
        {
            var damage = attackMode?.EffectDescription?.FindFirstDamageForm();

            if (damage == null)
            {
                return;
            }

            var dieRoll = Global.GetBardicRoll(character.guid);

            attackMode.ToHitBonus += dieRoll;
            attackMode.ToHitBonusTrends.Add(new TrendInfo(dieRoll,
                FeatureSourceType.Condition, "CombatInspiration", null));

            damage.BonusDamage += dieRoll;
            damage.DamageBonusTrends.Add(new TrendInfo(dieRoll,
                FeatureSourceType.Condition, "CombatInspiration", null));
        }
    }

    private sealed class RemoveBardicDieRollAtStartOfTurn : ICharacterTurnStartListener
    {
        public void OnCharacterTurnStarted(GameLocationCharacter locationCharacter)
        {
            Global.RemoveBardicRoll(locationCharacter.Guid);
        }
    }
}
