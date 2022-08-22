//
// TODO: looking for contributors to finish this class
//

#if false
using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaModApi;
using SolastaCommunityExpansion.Api.Extensions;
using static FeatureDefinitionAttributeModifier;
using static SolastaModApi.DatabaseHelper;
using static SolastaModApi.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Classes.Warden.Subclasses
{
    internal static class StoneheartDefender
    {
        private static readonly Guid Namespace = new("09b66b8b-a00e-4ccd-96ce-0aa64200f321");

        private static CharacterSubclassDefinition Subclass;
        public static FeatureDefinitionPower FeatureDefinitionPowerRootsOfRock { get; private set; }
        public static FeatureDefinitionPower FeatureDefinitionPowerEarthshatter { get; private set; }
        public static FeatureDefinitionSavingThrowAffinity FeatureDefinitionSavingThrowAffinityMettle { get; private set; }
        public static FeatureDefinitionPower FeatureDefinitionPowerImmortalMountain { get; private set; }

        internal static CharacterSubclassDefinition GetSubclass(CharacterClassDefinition wardenClass)
        {
            return Subclass ??= BuildAndAddSubclass(wardenClass);
        }

        private class RootsOfRockEffectForm : CustomEffectForm
        {
            public static ConditionDefinition conditionRestricted { get; private set; }
            public static ConditionDefinition conditionAC { get; private set; }
            public RootsOfRockEffectForm() : base()
            {
                if (conditionRestricted == null) {
                    var rootsOfRockMovementAffinity = FeatureDefinitionMovementAffinityBuilder
                        .Create(FeatureDefinitionMovementAffinitys.MovementAffinityConditionRestrained, "MovementRootsOfRock", Namespace)
                        .SetGuiPresentation("MovementRootsOfRock", Category.Modifier)
                        .AddToDB();

                    var rootsOfRockRestrictedConditionDefinition = ConditionDefinitionBuilder
                        .Create(ConditionDefinitions.ConditionHeavilyEncumbered, "ConditionRootsOfRockRestricted", Namespace)
                        .SetGuiPresentation("RootsOfRockRestricted", Category.Condition, ConditionDefinitions.ConditionRestrained.GuiPresentation.SpriteReference)
                        .AddToDB()
                    .SetConditionType(RuleDefinitions.ConditionType.Detrimental)
                    .SetDurationParameter(1)
                    .SetDurationType(RuleDefinitions.DurationType.Round)
                    .SetTurnOccurence(RuleDefinitions.TurnOccurenceType.StartOfTurn);
                    rootsOfRockRestrictedConditionDefinition.RecurrentEffectForms.Clear();
                    rootsOfRockRestrictedConditionDefinition.Features.Clear();
                    rootsOfRockRestrictedConditionDefinition.Features.Add(rootsOfRockMovementAffinity);

                    conditionRestricted = rootsOfRockRestrictedConditionDefinition;
                }

                if (conditionAC == null) {
                    var rootsOfRockAttributeModifier = FeatureDefinitionAttributeModifierBuilder
                        .Create("AttributeModifierRootsOfRock", Namespace)
                        .SetGuiPresentation(Category.Modifier)
                        .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, 2)
                        .AddToDB();

                    var rootsOfRockACConditionDefinition = ConditionDefinitionBuilder
                        .Create(ConditionDefinitions.ConditionShieldedByFaith, "ConditionRootsOfRockAC", Namespace)
                        .SetGuiPresentation("RootsOfRockAC", Category.Condition, ConditionDefinitions.ConditionShieldedByFaith.GuiPresentation.SpriteReference)
                        .AddToDB()
                    .SetConditionType(RuleDefinitions.ConditionType.Beneficial)
                    .SetDurationParameter(1)
                    .SetDurationType(RuleDefinitions.DurationType.Round)
                    .SetTurnOccurence(RuleDefinitions.TurnOccurenceType.StartOfTurn);
                    rootsOfRockACConditionDefinition.RecurrentEffectForms.Clear();
                    rootsOfRockACConditionDefinition.Features.Clear();
                    rootsOfRockACConditionDefinition.Features.Add(rootsOfRockAttributeModifier);

                    conditionAC = rootsOfRockACConditionDefinition;
                }
            }
            public override void ApplyForm(
                RulesetImplementationDefinitions.ApplyFormsParams formsParams, 
                bool retargeting, 
                bool proxyOnly, 
                bool forceSelfConditionOnly,
                RuleDefinitions.EffectApplication effectApplication = RuleDefinitions.EffectApplication.All,
                List<EffectFormFilter> filters = null)
            {
                if (formsParams.targetCharacter.Side == RuleDefinitions.Side.Enemy || formsParams.targetCharacter == formsParams.sourceCharacter){
                    ApplyCondition(formsParams, conditionRestricted, RuleDefinitions.DurationType.Round, 1);
                    if (formsParams.targetCharacter == formsParams.sourceCharacter){
                        ApplyCondition(formsParams, conditionAC, RuleDefinitions.DurationType.Round, 1);
                    }
                }
            }
            public override void FillTags(Dictionary<string, TagsDefinitions.Criticity> tagsMap){}
            private static void ApplyCondition(RulesetImplementationDefinitions.ApplyFormsParams formsParams, ConditionDefinition condition, RuleDefinitions.DurationType durationType, int durationParam)
            {
                // Prepare params for inflicting conditions
                ulong sourceGuid = formsParams.sourceCharacter != null ? formsParams.sourceCharacter.Guid : 0L;
                string sourceFaction =
 formsParams.sourceCharacter != null ? formsParams.sourceCharacter.CurrentFaction.Name : string.Empty;
                string effectDefinitionName = string.Empty;

                if (formsParams.attackMode != null)
                {
                    effectDefinitionName = formsParams.attackMode.SourceDefinition.Name;
                }
                else if (formsParams.activeEffect != null)
                {
                    effectDefinitionName = formsParams.activeEffect.SourceDefinition.Name;
                }

                int sourceAbilityBonus =
 (formsParams.activeEffect?.ComputeSourceAbilityBonus(formsParams.sourceCharacter)) ?? 0;

                formsParams.targetCharacter.InflictCondition(condition.Name, durationType, durationParam, RuleDefinitions.TurnOccurenceType.StartOfTurn, AttributeDefinitions.TagEffect, sourceGuid, sourceFaction, formsParams.effectLevel, effectDefinitionName, 0, sourceAbilityBonus);
            }
        }

        private static void BuildRootsOfRock(CharacterClassDefinition wardenClass)
        {
            var rootsOfRockEffectDescription = new EffectDescription();
            rootsOfRockEffectDescription.Copy(SpellDefinitions.Entangle.EffectDescription);
            rootsOfRockEffectDescription
                .SetCanBePlacedOnCharacter(true)
                .SetDurationParameter(1)
                .SetDurationType(RuleDefinitions.DurationType.Round)
                .SetEndOfEffect(RuleDefinitions.TurnOccurenceType.StartOfTurn)
                .SetHasSavingThrow(false)
                .SetRangeType(RuleDefinitions.RangeType.Self)
                .SetRecurrentEffect(RuleDefinitions.RecurrentEffect.No)
                .SetTargetExcludeCaster(false)
                .SetTargetParameter(3)
                .SetTargetSide(RuleDefinitions.Side.All)
                .SetTargetType(RuleDefinitions.TargetType.Cube);
            rootsOfRockEffectDescription.EffectParticleParameters.SetZoneParticleReference(null);
            rootsOfRockEffectDescription.RestrictedCharacterSizes.Add(RuleDefinitions.CreatureSize.Large);
            rootsOfRockEffectDescription.RestrictedCharacterSizes.Add(RuleDefinitions.CreatureSize.Medium);
            rootsOfRockEffectDescription.RestrictedCharacterSizes.Add(RuleDefinitions.CreatureSize.Small);
            rootsOfRockEffectDescription.RestrictedCharacterSizes.Add(RuleDefinitions.CreatureSize.Tiny);
            rootsOfRockEffectDescription.EffectForms.Clear();
            rootsOfRockEffectDescription.EffectForms.Add(new RootsOfRockEffectForm());

            FeatureDefinitionPowerRootsOfRock = FeatureDefinitionPowerBuilder
                .Create("RootsOfRock", Namespace)
                .SetGuiPresentation(Category.Power, Barkskin.GuiPresentation.SpriteReference)
                .SetActivation(RuleDefinitions.ActivationTime.BonusAction, 0)
                .SetRechargeRate(RuleDefinitions.RechargeRate.AtWill)
                .SetUsesFixed(1)
                .SetEffectDescription(rootsOfRockEffectDescription)
                .AddToDB();

            if (DatabaseRepository.GetDatabase<FeatureDefinitionPower>().TryGetElement("WardenGrasp", out var originalWardenGrasp))
            {
                FeatureDefinitionPowerRootsOfRock.SetOverriddenPower(originalWardenGrasp);
            }

        }

        private sealed class EarthshatterEffectForm : RootsOfRockEffectForm
        {
            public static ConditionDefinition conditionProne { get; private set; }
            public EarthshatterEffectForm() : base()
            {
                var earthshatterProneConditionDefinition = ConditionDefinitionBuilder
                    .Create(ConditionDefinitions.ConditionProne, "ConditionEarthshatterProne", Namespace)
                    .SetGuiPresentation("EarthshatterProne", Category.Condition, ConditionDefinitions.ConditionProne.GuiPresentation.SpriteReference)
                    .AddToDB()
                .SetConditionType(RuleDefinitions.ConditionType.Detrimental)
                .SetDurationParameter(1)
                .SetDurationType(RuleDefinitions.DurationType.Round)
                .SetTurnOccurence(RuleDefinitions.TurnOccurenceType.StartOfTurn);
                earthshatterProneConditionDefinition.RecurrentEffectForms.Clear();

                conditionProne = earthshatterProneConditionDefinition;

            }
            public override void ApplyForm(
                RulesetImplementationDefinitions.ApplyFormsParams formsParams, 
                bool retargeting, 
                bool proxyOnly, 
                bool forceSelfConditionOnly,
                RuleDefinitions.EffectApplication effectApplication = RuleDefinitions.EffectApplication.All,
                List<EffectFormFilter> filters = null)
            {
                
                if (formsParams.targetCharacter.Side == RuleDefinitions.Side.Enemy || formsParams.targetCharacter == formsParams.sourceCharacter){
                    ApplyCondition(formsParams, conditionRestricted, RuleDefinitions.DurationType.Round, 1);
                    if (formsParams.targetCharacter.Side == RuleDefinitions.Side.Enemy){
                        ApplyCondition(formsParams, conditionProne, RuleDefinitions.DurationType.Round, 1);
                    }
                    if (formsParams.targetCharacter == formsParams.sourceCharacter){
                        ApplyCondition(formsParams, conditionAC, RuleDefinitions.DurationType.Round, 1);
                    }
                }

            }

            public override void FillTags(Dictionary<string, TagsDefinitions.Criticity> tagsMap){}
            
            private static void ApplyCondition(RulesetImplementationDefinitions.ApplyFormsParams formsParams, ConditionDefinition condition, RuleDefinitions.DurationType durationType, int durationParam)
            {
                // Prepare params for inflicting conditions
                ulong sourceGuid = formsParams.sourceCharacter != null ? formsParams.sourceCharacter.Guid : 0L;
                string sourceFaction =
 formsParams.sourceCharacter != null ? formsParams.sourceCharacter.CurrentFaction.Name : string.Empty;
                string effectDefinitionName = string.Empty;

                if (formsParams.attackMode != null)
                {
                    effectDefinitionName = formsParams.attackMode.SourceDefinition.Name;
                }
                else if (formsParams.activeEffect != null)
                {
                    effectDefinitionName = formsParams.activeEffect.SourceDefinition.Name;
                }

                int sourceAbilityBonus =
 (formsParams.activeEffect?.ComputeSourceAbilityBonus(formsParams.sourceCharacter)) ?? 0;

                formsParams.targetCharacter.InflictCondition(condition.Name, durationType, durationParam, RuleDefinitions.TurnOccurenceType.StartOfTurn, AttributeDefinitions.TagEffect, sourceGuid, sourceFaction, formsParams.effectLevel, effectDefinitionName, 0, sourceAbilityBonus);
            }
        }

        private static void BuildEarthshatter()
        {
            var earthshatterEffectDescription = new EffectDescription();
            earthshatterEffectDescription.Copy(SpellDefinitions.Thunderwave.EffectDescription);
            earthshatterEffectDescription
                .SetCanBePlacedOnCharacter(true)
                .SetDurationParameter(1)
                .SetDurationType(RuleDefinitions.DurationType.Round)
                .SetEndOfEffect(RuleDefinitions.TurnOccurenceType.StartOfTurn)
                .SetHasSavingThrow(false)
                .SetRangeType(RuleDefinitions.RangeType.Self)
                .SetRecurrentEffect(RuleDefinitions.RecurrentEffect.No)
                .SetTargetExcludeCaster(false)
                .SetTargetParameter(3)
                .SetTargetSide(RuleDefinitions.Side.All)
                .SetTargetType(RuleDefinitions.TargetType.Cube);
            earthshatterEffectDescription.EffectParticleParameters.SetZoneParticleReference(null);
            earthshatterEffectDescription.RestrictedCharacterSizes.Add(RuleDefinitions.CreatureSize.Large);
            earthshatterEffectDescription.RestrictedCharacterSizes.Add(RuleDefinitions.CreatureSize.Medium);
            earthshatterEffectDescription.RestrictedCharacterSizes.Add(RuleDefinitions.CreatureSize.Small);
            earthshatterEffectDescription.RestrictedCharacterSizes.Add(RuleDefinitions.CreatureSize.Tiny);
            earthshatterEffectDescription.EffectForms.Clear();
            earthshatterEffectDescription.EffectForms.Add(new EarthshatterEffectForm());

            FeatureDefinitionPowerEarthshatter = FeatureDefinitionPowerBuilder
                .Create("Earthshatter", Namespace)
                .SetGuiPresentation(Category.Power, Thunderwave.GuiPresentation.SpriteReference)
                .SetActivation(RuleDefinitions.ActivationTime.Action, 1)
                .SetRechargeRate(RuleDefinitions.RechargeRate.LongRest)
                .SetUsesAbility(0, AttributeDefinitions.Strength)
                .SetEffectDescription(earthshatterEffectDescription)
                .AddToDB();
        }

        private static void BuildMettle()
        {
            FeatureDefinitionSavingThrowAffinityMettle = FeatureDefinitionSavingThrowAffinityBuilder
                .Create(FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityRogueEvasion, "Mettle", Namespace)
                .SetGuiPresentation(Category.Feature, FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityRogueEvasion.GuiPresentation.SpriteReference)
                .SetAffinities(RuleDefinitions.CharacterSavingThrowAffinity.SpecialHalfDamage, true, AttributeDefinitions.Constitution)
                .AddToDB();
        }

        private static void BuildImmortalMountain()
        {
            FeatureDefinitionPowerImmortalMountain = FeatureDefinitionPowerBuilder
                .Create("ImmortalMountain", Namespace)
                .SetGuiPresentation(Category.Power, Stoneskin.GuiPresentation.SpriteReference)
                .AddToDB();
        }

        private static void BuildProgression(CharacterSubclassDefinitionBuilder subclassBuilder)
        {
            subclassBuilder
                .AddFeatureAtLevel(FeatureDefinitionPowerRootsOfRock, 3)
                .AddFeatureAtLevel(FeatureDefinitionPowerEarthshatter, 3)
                .AddFeatureAtLevel(FeatureDefinitionSavingThrowAffinityMettle, 3);
    //            .AddFeatureAtLevel(FeatureDefinitionPowerImmortalMountain, 3);
        }

        public static CharacterSubclassDefinition BuildAndAddSubclass(CharacterClassDefinition wardenClass)
        {
            var subclassBuilder = CharacterSubclassDefinitionBuilder
                .Create("StoneheartDefender", Namespace)
                .SetGuiPresentation(Category.Subclass, MartialMountaineer.GuiPresentation.SpriteReference);

            BuildRootsOfRock(wardenClass);
            BuildEarthshatter();
            BuildMettle();
            BuildImmortalMountain();

            BuildProgression(subclassBuilder);

            return subclassBuilder.AddToDB();
        }
    }
}
#endif
