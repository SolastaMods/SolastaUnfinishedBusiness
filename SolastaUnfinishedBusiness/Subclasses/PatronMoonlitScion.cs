using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Validators;
using UnityEngine.AddressableAssets;
using static FeatureDefinitionAttributeModifier;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellListDefinitions;
using Resources = SolastaUnfinishedBusiness.Properties.Resources;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class PatronMoonlitScion : AbstractSubclass
{
    private const string Name = "MoonlitScion";

    public PatronMoonlitScion()
    {
        // LEVEL 01

        // Expanded Spell List

        var spellListMoonlit = SpellListDefinitionBuilder
            .Create(SpellListWizard, $"SpellList{Name}")
            .SetGuiPresentationNoContent(true)
            .ClearSpells()
            .SetSpellsAtLevel(1, FaerieFire, Sleep)
            .SetSpellsAtLevel(2, MoonBeam, SeeInvisibility)
            .SetSpellsAtLevel(3, Daylight, Slow)
            .SetSpellsAtLevel(4, GreaterInvisibility, GuardianOfFaith)
            .SetSpellsAtLevel(5, ConeOfCold, GreaterRestoration)
            .FinalizeSpells(true, 9)
            .AddToDB();

        var magicAffinityMoonlitExpandedSpells = FeatureDefinitionMagicAffinityBuilder
            .Create($"MagicAffinity{Name}ExpandedSpells")
            .SetGuiPresentation("MagicAffinityPatronExpandedSpells", Category.Feature)
            .SetExtendedSpellList(spellListMoonlit)
            .AddToDB();

        // Lunar Cloak

        var powerLunarCloak = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}LunarCloak")
            .SetGuiPresentation($"FeatureSet{Name}LunarCloak", Category.Feature,
                Sprites.GetSprite("LunarCloak", Resources.PowerLunarCloak, 256, 128))
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .Build())
            .AddToDB();

        // Lunar Radiance Debuff

        var conditionLunarRadianceEnemy = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionLuminousKi, $"Condition{Name}LunarRadianceEnemy")
            .SetGuiPresentation($"Power{Name}LunarRadiance", Category.Feature,
                ConditionDefinitions.ConditionLightSensitive)
            .SetConditionType(ConditionType.Detrimental)
            .SetPossessive()
            // need special duration because of SetUseSpellAttack
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
            .SetFeatures(
                FeatureDefinitionAttributeModifierBuilder
                    .Create($"AttributeModifier{Name}LunarRadianceEnemy")
                    .SetGuiPresentation($"Power{Name}LunarRadiance", Category.Feature, Gui.NoLocalization)
                    .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, -1)
                    .AddToDB())
            .AddToDB();

        conditionLunarRadianceEnemy.GuiPresentation.description = Gui.EmptyContent;

        // Lunar Radiance

        var powerLunarRadiance = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}LunarRadiance")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerFullMoon", Resources.PowerFullMoon, 256, 128))
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetUseSpellAttack()
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.Enemy, RangeType.MeleeHit, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 1, 0, 10, 10)
                            .SetDamageForm(DamageTypeRadiant, 1, DieType.D8)
                            .Build(),
                        EffectFormBuilder.ConditionForm(conditionLunarRadianceEnemy))
                    .SetParticleEffectParameters(FeatureDefinitionPowers.PowerTraditionLightBlindingFlash)
                    .SetEffectEffectParameters(new AssetReference())
                    .Build())
            // required as added through condition
            .AddCustomSubFeatures(ClassHolder.Warlock)
            .AddToDB();

        var conditionFullMoon = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionShine, $"Condition{Name}FullMoon")
            .SetGuiPresentation($"Power{Name}FullMoon", Category.Feature,
                ConditionDefinitions.ConditionLightSensitiveSorakSaboteur)
            .SetPossessive()
            .SetConditionType(ConditionType.Beneficial)
            .SetFeatures(powerLunarRadiance)
            .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
            .AddToDB();

        conditionFullMoon.GuiPresentation.description = Gui.EmptyContent;

        // Lunar Radiance No Cost

        var powerLunarRadianceNoCost = FeatureDefinitionPowerBuilder
            .Create(powerLunarRadiance, $"Power{Name}LunarRadianceNoCost")
            .SetUsesFixed(ActivationTime.NoCost)
            .AddToDB();

        var conditionFullMoonNoCost = ConditionDefinitionBuilder
            .Create($"Condition{Name}FullMoonNoCost")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(powerLunarRadianceNoCost)
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
            .AddToDB();

        powerLunarRadianceNoCost.AddCustomSubFeatures(
            ValidatorsValidatePowerUse.InCombat,
            new PowerOrSpellFinishedByMeNoCost(conditionFullMoonNoCost));

        // Full Moon

        var lightSourceForm =
            FaerieFire.EffectDescription.GetFirstFormOfType(EffectForm.EffectFormType.LightSource);

        var powerFullMoon = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}FullMoon")
            .SetGuiPresentation(Category.Feature, hidden: true)
            .SetSharedPool(ActivationTime.BonusAction, powerLunarCloak)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(conditionFullMoon),
                        EffectFormBuilder.ConditionForm(conditionFullMoonNoCost),
                        EffectFormBuilder
                            .Create()
                            .SetLightSourceForm(
                                LightSourceType.Basic, 3, 3,
                                lightSourceForm.lightSourceForm.color,
                                lightSourceForm.lightSourceForm.graphicsPrefabReference)
                            .Build())
                    .SetParticleEffectParameters(FeatureDefinitionPowers.PowerOathOfJugementWeightOfJustice)
                    .Build())
            .AddToDB();

        // Lunar Chill Debuff

        var conditionLunarChillEnemy = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionHindered, $"Condition{Name}LunarChillEnemy")
            .SetOrUpdateGuiPresentation($"Power{Name}LunarChill", Category.Feature)
            .SetParentCondition(ConditionDefinitions.ConditionHindered)
            .SetPossessive()
            // need special duration because of SetUseSpellAttack
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
            .SetFeatures()
            .CopyParticleReferences(FeatureDefinitionPowers.PowerDomainElementalHeraldOfTheElementsCold)
            .AddToDB();

        conditionLunarChillEnemy.GuiPresentation.description = Gui.EmptyContent;

        // Lunar Chill

        var powerLunarChill = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}LunarChill")
            .SetGuiPresentation(Category.Feature, Sprites.GetSprite("PowerNewMoon", Resources.PowerNewMoon, 256, 128))
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetUseSpellAttack()
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.Enemy, RangeType.MeleeHit, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 1, 0, 10, 10)
                            .SetDamageForm(DamageTypeCold, 1, DieType.D8)
                            .Build(),
                        EffectFormBuilder.ConditionForm(conditionLunarChillEnemy))
                    .SetParticleEffectParameters(FeatureDefinitionPowers.PowerDomainElementalIceLance)
                    .Build())
            // required as added through condition
            .AddCustomSubFeatures(ClassHolder.Warlock)
            .AddToDB();

        var conditionNewMoon = ConditionDefinitionBuilder
            .Create($"Condition{Name}NewMoon")
            .SetGuiPresentation($"Power{Name}NewMoon", Category.Feature,
                ConditionDefinitions.ConditionChildOfDarkness_DimLight)
            .SetPossessive()
            .SetFeatures(powerLunarChill, FeatureDefinitionSenses.SenseDarkvision)
            .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker, new ForceLightingStateNewMoon())
            .CopyParticleReferences(FeatureDefinitionPowers.PowerSorcererChildRiftDeflection)
            .AddToDB();

        conditionNewMoon.GuiPresentation.description = Gui.EmptyContent;

        // Lunar Chill No Cost

        var powerLunarChillNoCost = FeatureDefinitionPowerBuilder
            .Create(powerLunarChill, $"Power{Name}LunarChillNoCost")
            .SetUsesFixed(ActivationTime.NoCost)
            .AddToDB();

        var conditionNewMoonNoCost = ConditionDefinitionBuilder
            .Create($"Condition{Name}NewMoonNoCost")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(powerLunarChillNoCost)
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
            .AddToDB();

        powerLunarChillNoCost.AddCustomSubFeatures(
            ValidatorsValidatePowerUse.InCombat,
            new PowerOrSpellFinishedByMeNoCost(conditionNewMoonNoCost));

        // New Moon

        var powerNewMoon = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}NewMoon")
            .SetGuiPresentation(Category.Feature, hidden: true)
            .SetSharedPool(ActivationTime.BonusAction, powerLunarCloak)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(conditionNewMoon),
                        EffectFormBuilder.ConditionForm(conditionNewMoonNoCost))
                    .SetParticleEffectParameters(FeatureDefinitionPowers.PowerMagebaneSpellCrusher)
                    .Build())
            .AddToDB();

        var featureSetLunarCloak = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}LunarCloak")
            .SetGuiPresentation(Category.Feature)
            .SetFeatureSet(powerLunarCloak, powerFullMoon, powerNewMoon)
            .AddToDB();

        // LEVEL 06

        // Midnight's Blessing

        var conditionFullMoonMidnightBlessing = ConditionDefinitionBuilder
            .Create(conditionFullMoon, $"Condition{Name}FullMoonMidnightBlessing")
            .AddFeatures(FeatureDefinitionDamageAffinitys.DamageAffinityRadiantResistance)
            .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
            .AddToDB();

        var conditionNewMoonMidnightBlessing = ConditionDefinitionBuilder
            .Create(conditionNewMoon, $"Condition{Name}NewMoonMidnightBlessing")
            .AddFeatures(FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance)
            .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker, new ForceLightingStateNewMoon())
            .AddToDB();

        var conditionMidnightBlessing = ConditionDefinitionBuilder
            .Create($"Condition{Name}MidnightBlessing")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        var powerMidnightBlessing = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}MidnightBlessing")
            .SetGuiPresentation(Category.Feature, MoonBeam)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(MoonBeam)
                    .SetEffectForms()
                    .SetNoSavingThrow()
                    .Build())
            .AddCustomSubFeatures(new CustomBehaviorMidnightBlessing(conditionMidnightBlessing))
            .AddToDB();

        var autoPreparedSpells = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{Name}MidnightBlessing")
            .SetGuiPresentation("ExpandedSpells", Category.Feature)
            .SetAutoTag("Patron")
            .SetPreparedSpellGroups(AutoPreparedSpellsGroupBuilder.BuildSpellGroup(2, MoonBeam))
            .SetSpellcastingClass(CharacterClassDefinitions.Warlock)
            .AddToDB();

        var featureSetMidnightBlessing = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}MidnightBlessing")
            .SetGuiPresentation(Category.Feature)
            .SetFeatureSet(autoPreparedSpells, powerMidnightBlessing)
            .AddToDB();

        // LEVEL 10

        // Lunar Embrace

        var movementAffinityFullMoonLunarEmbrace = FeatureDefinitionMovementAffinityBuilder
            .Create(FeatureDefinitionMovementAffinitys.MovementAffinityConditionFlyingAdaptive,
                $"MovementAffinity{Name}FullMoonLunarEmbrace")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var conditionFullMoonLunarEmbrace =
            ConditionDefinitionBuilder
                .Create(conditionFullMoonMidnightBlessing, $"Condition{Name}FullMoonLunarEmbrace")
                .SetParentCondition(ConditionDefinitions.ConditionFlying)
                .AddFeatures(movementAffinityFullMoonLunarEmbrace)
                .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
                .AddToDB();

        // there is indeed a typo on tag
        // ReSharper disable once StringLiteralTypo
        conditionFullMoonLunarEmbrace.ConditionTags.Add("Verticality");

        var conditionNewMoonLunarEmbrace = ConditionDefinitionBuilder
            .Create(conditionNewMoonMidnightBlessing, $"Condition{Name}NewMoonLunarEmbrace")
            .SetParentCondition(ConditionDefinitions.ConditionFlying)
            .AddFeatures(movementAffinityFullMoonLunarEmbrace)
            .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker, new ForceLightingStateNewMoon())
            .AddToDB();

        // there is indeed a typo on tag
        // ReSharper disable once StringLiteralTypo
        conditionNewMoonLunarEmbrace.ConditionTags.Add("Verticality");

        var featureLunarEmbrace = FeatureDefinitionBuilder
            .Create($"Feature{Name}LunarEmbrace")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        // LEVEL 14

        var powerMoonlightGuise = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}MoonlightGuise")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.ShortRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(ConditionDefinitions.ConditionInvisible))
                    .SetParticleEffectParameters(Banishment)
                    .Build())
            .AddToDB();

        powerMoonlightGuise.AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden,
            new CustomBehaviorMoonlightGuise(powerMoonlightGuise));

        // MAIN

        PowerBundle.RegisterPowerBundle(powerLunarCloak, false, powerFullMoon, powerNewMoon);
        ForceGlobalUniqueEffects.AddToGroup(
            ForceGlobalUniqueEffects.Group.MoonlitNewAndFullMoon, powerFullMoon, powerNewMoon);

        powerFullMoon.AddCustomSubFeatures(
            new ModifyEffectDescriptionMidnightBlessingOrLunarEmbrace(
                powerFullMoon, conditionFullMoon, conditionFullMoonMidnightBlessing, conditionFullMoonLunarEmbrace));

        powerNewMoon.AddCustomSubFeatures(
            new ModifyEffectDescriptionMidnightBlessingOrLunarEmbrace(
                powerNewMoon, conditionNewMoon, conditionNewMoonMidnightBlessing, conditionNewMoonLunarEmbrace));

        Subclass = CharacterSubclassDefinitionBuilder
            .Create($"Patron{Name}")
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.PatronMoonlit, 256))
            .AddFeaturesAtLevel(1, magicAffinityMoonlitExpandedSpells, featureSetLunarCloak)
            .AddFeaturesAtLevel(6, featureSetMidnightBlessing)
            .AddFeaturesAtLevel(10, featureLunarEmbrace)
            .AddFeaturesAtLevel(14, powerMoonlightGuise)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Warlock;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice => FeatureDefinitionSubclassChoices
        .SubclassChoiceWarlockOtherworldlyPatrons;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    // force an unlit lighting state
    private sealed class ForceLightingStateNewMoon : IForceLightingState
    {
        public LocationDefinitions.LightingState GetLightingState(
            GameLocationCharacter gameLocationCharacter, LocationDefinitions.LightingState lightingState)
        {
            return lightingState is LocationDefinitions.LightingState.Bright or LocationDefinitions.LightingState.Dim
                ? LocationDefinitions.LightingState.Unlit
                : lightingState;
        }
    }

    // remove the No Cost condition if the no cost power is used
    private sealed class PowerOrSpellFinishedByMeNoCost(ConditionDefinition conditionFree) : IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;

            // no need to check for source here as these are all self conditions
            if (rulesetCharacter.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionFree.Name, out var activeCondition))
            {
                rulesetCharacter.RemoveCondition(activeCondition);
            }

            yield break;
        }
    }

    // replace lunar cloak conditions with midnight blessing or lunar embrace ones depending on hero level
    private sealed class ModifyEffectDescriptionMidnightBlessingOrLunarEmbrace(
        FeatureDefinitionPower power,
        ConditionDefinition conditionToReplace,
        ConditionDefinition conditionMidnightBlessing,
        ConditionDefinition conditionLunarEmbrace) : IModifyEffectDescription
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == power;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var levels = character.GetClassLevel(CharacterClassDefinitions.Warlock);

            if (levels < 6)
            {
                return effectDescription;
            }

            var effectForm = effectDescription.EffectForms.FirstOrDefault(x =>
                x.FormType == EffectForm.EffectFormType.Condition &&
                x.ConditionForm.ConditionDefinition == conditionToReplace);

            effectForm!.ConditionForm.conditionDefinition = levels < 10
                ? conditionMidnightBlessing
                : conditionLunarEmbrace;

            return effectDescription;
        }
    }

    private sealed class CustomBehaviorMidnightBlessing(ConditionDefinition conditionMidnightBlessing)
        : IPowerOrSpellFinishedByMe, IPreventRemoveConcentrationOnDamage
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var actingCharacter = action.ActingCharacter;
            var rulesetCharacter = actingCharacter.RulesetCharacter;
            var levels = rulesetCharacter.GetClassLevel(CharacterClassDefinitions.Warlock);
            var slotLevel = levels switch
            {
                < 7 => 3,
                < 9 => 4,
                < 11 => 5,
                < 13 => 6,
                < 15 => 7,
                < 17 => 8,
                _ => 9
            };

            rulesetCharacter.InflictCondition(
                conditionMidnightBlessing.Name,
                DurationType.Minute,
                1,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetCharacter.Guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                conditionMidnightBlessing.Name,
                0,
                0,
                0);

            rulesetCharacter.ReceiveTemporaryHitPoints(
                levels, DurationType.UntilAnyRest, 0, TurnOccurenceType.StartOfTurn, rulesetCharacter.guid);

            var spellRepertoire = rulesetCharacter.GetClassSpellRepertoire(CharacterClassDefinitions.Warlock);

            action.ActingCharacter.MyExecuteActionCastNoCost(MoonBeam, slotLevel, action.ActionParams, spellRepertoire);

            yield break;
        }

        public SpellDefinition[] SpellsThatShouldNotRollConcentrationCheckFromDamage(RulesetCharacter rulesetCharacter)
        {
            return rulesetCharacter.HasConditionOfType(conditionMidnightBlessing) ? [MoonBeam] : [];
        }
    }

    private sealed class CustomBehaviorMoonlightGuise(FeatureDefinitionPower powerMoonlightGuise)
        : ITryAlterOutcomeAttack, IMagicEffectBeforeHitConfirmedOnMe
    {
        public IEnumerator OnMagicEffectBeforeHitConfirmedOnMe(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            if (rulesetEffect.EffectDescription.RangeType is not (RangeType.MeleeHit or RangeType.RangeHit))
            {
                yield return HandleReaction(battleManager, attacker, defender, actualEffectForms);
            }
        }

        public int HandlerPriority => 40;

        public IEnumerator OnTryAlterOutcomeAttack(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            RulesetEffect rulesetEffect)
        {
            if (helper != defender)
            {
                yield break;
            }

            var actualEffectForms =
                attackMode?.EffectDescription.EffectForms ?? rulesetEffect?.EffectDescription.EffectForms ?? [];

            yield return HandleReaction(battleManager, attacker, defender, actualEffectForms);
        }

        private IEnumerator HandleReaction(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            List<EffectForm> actualEffectForms)
        {
            if (actualEffectForms.All(x => x.FormType != EffectForm.EffectFormType.Damage))
            {
                yield break;
            }

            var rulesetDefender = defender.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerMoonlightGuise, rulesetDefender);

            if (actualEffectForms.All(x => x.FormType != EffectForm.EffectFormType.Damage) ||
                !defender.CanReact() ||
                rulesetDefender.GetRemainingUsesOfPower(usablePower) == 0)
            {
                yield break;
            }

            yield return defender.MyReactToUsePower(
                ActionDefinitions.Id.PowerReaction,
                usablePower,
                [defender],
                attacker,
                "MoonlightGuise",
                battleManager: battleManager);
        }
    }
}
