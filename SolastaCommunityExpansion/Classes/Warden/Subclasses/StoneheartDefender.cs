using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomFeatureDefinitions;
using SolastaModApi;
using SolastaModApi.Extensions;
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
        public static FeatureDefinitionPower FeatureDefinitionPowerMettle { get; private set; }
        public static FeatureDefinitionPower FeatureDefinitionPowerImmortalMountain { get; private set; }

        internal static CharacterSubclassDefinition GetSubclass(CharacterClassDefinition wardenClass)
        {
            return Subclass ??= BuildAndAddSubclass(wardenClass);
        }

        private sealed class RootsOfRockEffectForm : CustomEffectForm
        {
            public static ConditionDefinition conditionRestricted { get; private set; }
            public static ConditionDefinition conditionAC { get; private set; }
            public RootsOfRockEffectForm()
            {
                this.FormType = (EffectFormType)ExtraEffectFormType.Custom;

                var rootsOfRockMovementAffinity = new FeatureDefinitionBuilder<FeatureDefinitionMovementAffinity>(
                FeatureDefinitionMovementAffinitys.MovementAffinityConditionRestrained, "MovementRootsOfRock", Namespace)
                    .SetGuiPresentation("MovementRootsOfRock", Category.Modifier)
                    .AddToDB();

                var rootsOfRockRestrictedConditionDefinition = new ConditionDefinitionBuilder<ConditionDefinition>(
                    ConditionDefinitions.ConditionHeavilyEncumbered, "ConditionRootsOfRockRestricted", Namespace)
                        .SetGuiPresentation("RootsOfRockRestricted", Category.Condition, ConditionDefinitions.ConditionRestrained.GuiPresentation.SpriteReference)
                        .AddToDB()
                    .SetConditionType(RuleDefinitions.ConditionType.Detrimental)
                    .SetDurationParameter(1)
                    .SetDurationType(RuleDefinitions.DurationType.Round)
                    .SetTurnOccurence(RuleDefinitions.TurnOccurenceType.StartOfTurn);
                rootsOfRockRestrictedConditionDefinition.RecurrentEffectForms.Clear();
                rootsOfRockRestrictedConditionDefinition.Features.Clear();
                rootsOfRockRestrictedConditionDefinition.Features.Add(rootsOfRockMovementAffinity);

                var rootsOfRockAttributeModifier = FeatureDefinitionAttributeModifierBuilder
                    .Create("AttributeModifierRootsOfRock", Namespace)
                    .SetGuiPresentation(Category.Modifier)
                    .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, 2)
                    .AddToDB();

                var rootsOfRockACConditionDefinition = new ConditionDefinitionBuilder<ConditionDefinition>(
                    ConditionDefinitions.ConditionAcidArrowed, "ConditionRootsOfRockAC", Namespace)
                        .SetGuiPresentation("RootsOfRockAC", Category.Condition, ConditionDefinitions.ConditionShieldedByFaith.GuiPresentation.SpriteReference)
                        .AddToDB()
                    .SetConditionType(RuleDefinitions.ConditionType.Beneficial)
                    .SetDurationParameter(1)
                    .SetDurationType(RuleDefinitions.DurationType.Round)
                    .SetTurnOccurence(RuleDefinitions.TurnOccurenceType.StartOfTurn);
                rootsOfRockACConditionDefinition.RecurrentEffectForms.Clear();
                rootsOfRockACConditionDefinition.Features.Clear();
                rootsOfRockACConditionDefinition.Features.Add(rootsOfRockAttributeModifier);

                conditionRestricted = rootsOfRockRestrictedConditionDefinition;
                conditionAC = rootsOfRockACConditionDefinition;

            }
            public override void ApplyForm(RulesetImplementationDefinitions.ApplyFormsParams formsParams, bool retargeting, bool proxyOnly, bool forceSelfConditionOnly)
            {
                
                if (formsParams.targetCharacter.Side == RuleDefinitions.Side.Enemy || formsParams.targetCharacter == formsParams.sourceCharacter){
                    ApplyCondition(formsParams, conditionRestricted, RuleDefinitions.DurationType.Round, 1);
                    if (formsParams.targetCharacter == formsParams.sourceCharacter){
                        ApplyCondition(formsParams, conditionAC, RuleDefinitions.DurationType.Round, 1);
                    }
                }

            }

            public override void FillTags(Dictionary<string, TagsDefinitions.Criticity> tagsMap)
            {
                // Nothing
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

                formsParams.targetCharacter.InflictCondition(condition.Name, durationType, durationParam, RuleDefinitions.TurnOccurenceType.StartOfTurn, "11Effect", sourceGuid, sourceFaction, formsParams.effectLevel, effectDefinitionName, 0, sourceAbilityBonus);
            }
        }

        private static void BuildRootsOfRock()
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
                .SetRecharge(RuleDefinitions.RechargeRate.AtWill)
                .SetUsesFixed(1)
                .SetEffect(rootsOfRockEffectDescription)
                .AddToDB();
        }

        private static void BuildEarthshatter()
        {
            FeatureDefinitionPowerEarthshatter = FeatureDefinitionPowerBuilder
                .Create("Earthshatter", Namespace)
                .SetGuiPresentation(Category.Power, Thunderwave.GuiPresentation.SpriteReference)
                .AddToDB();
        }

        private static void BuildMettle()
        {
            FeatureDefinitionPowerMettle = FeatureDefinitionPowerBuilder
                .Create("Mettle", Namespace)
                .SetGuiPresentation(Category.Power, Resistance.GuiPresentation.SpriteReference)
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
                .AddFeatureAtLevel(FeatureDefinitionPowerRootsOfRock, 3);
//                .AddFeatureAtLevel(FeatureDefinitionPowerEarthshatter, 3)
  //              .AddFeatureAtLevel(FeatureDefinitionPowerMettle, 3)
    //            .AddFeatureAtLevel(FeatureDefinitionPowerImmortalMountain, 3);
        }

        public static CharacterSubclassDefinition BuildAndAddSubclass(CharacterClassDefinition wardenClass)
        {
            var subclassBuilder = CharacterSubclassDefinitionBuilder
                .Create("StoneheartDefender", Namespace)
                .SetGuiPresentation(Category.Subclass, MartialMountaineer.GuiPresentation.SpriteReference);

            BuildRootsOfRock();
            BuildEarthshatter();
            BuildMettle();
            BuildImmortalMountain();

            BuildProgression(subclassBuilder);

            return subclassBuilder.AddToDB();
        }
    }
}
