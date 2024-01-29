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
            .Create($"Condition{Name}LunarRadianceEnemy")
            .SetGuiPresentation($"Power{Name}LunarRadiance", Category.Feature,
                ConditionDefinitions.ConditionLightSensitive)
            .SetConditionType(ConditionType.Detrimental)
            .SetPossessive()
            .SetFeatures(
                FeatureDefinitionAttributeModifierBuilder
                    .Create($"AttributeModifier{Name}LunarRadianceEnemy")
                    .SetGuiPresentation(Category.Feature)
                    .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, -1)
                    .AddToDB())
            .AddToDB();

        conditionLunarRadianceEnemy.GuiPresentation.description = Gui.NoLocalization;

        // Lunar Radiance No Cost

        var spriteLunarRadiance = Sprites.GetSprite("LunarRadiance", Resources.PowerFullMoon, 256, 128);

        var powerLunarRadianceNoCost = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}LunarRadianceNoCost")
            .SetGuiPresentation($"Power{Name}LunarRadiance", Category.Feature, spriteLunarRadiance)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetUseSpellAttack()
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.Enemy, RangeType.RangeHit, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder.DamageForm(DamageTypeRadiant, 1, DieType.D8),
                        EffectFormBuilder.ConditionForm(conditionLunarRadianceEnemy))
                    .SetParticleEffectParameters(FeatureDefinitionPowers.PowerTraditionLightBlindingFlash)
                    .Build())
            .AddToDB();

        var conditionFullMoonNoCost = ConditionDefinitionBuilder
            .Create($"Condition{Name}FullMoonNoCost")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(powerLunarRadianceNoCost)
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddCustomSubFeatures(new AddUsablePowersFromCondition())
            .AddToDB();

        powerLunarRadianceNoCost.AddCustomSubFeatures(
            ValidatorsValidatePowerUse.InCombat,
            new MagicEffectFinishedByMeNoCost(conditionFullMoonNoCost));

        // Lunar Radiance

        var powerLunarRadiance = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}LunarRadiance")
            .SetGuiPresentation(Category.Feature, spriteLunarRadiance)
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetUseSpellAttack()
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.Enemy, RangeType.RangeHit, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder.DamageForm(DamageTypeRadiant, 1, DieType.D8),
                        EffectFormBuilder.ConditionForm(conditionLunarRadianceEnemy))
                    .SetParticleEffectParameters(FeatureDefinitionPowers.PowerTraditionLightBlindingFlash)
                    .Build())
            .AddToDB();

        var conditionFullMoon = ConditionDefinitionBuilder
            .Create($"Condition{Name}FullMoon")
            .SetGuiPresentation($"Power{Name}FullMoon", Category.Feature,
                ConditionDefinitions.ConditionProtectedInsideMagicCircle)
            .SetPossessive()
            .SetFeatures(powerLunarRadiance)
            .AddCustomSubFeatures(new AddUsablePowersFromCondition())
            .AddToDB();

        conditionFullMoon.GuiPresentation.description = Gui.NoLocalization;

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
                    .Build())
            .AddToDB();

        // Lunar Chill Debuff

        var conditionLunarChillEnemy = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionHindered_By_Frost, $"Condition{Name}LunarChillEnemy")
            .SetOrUpdateGuiPresentation($"Power{Name}LunarChill", Category.Feature)
            .SetPossessive()
            .AddToDB();

        conditionLunarChillEnemy.GuiPresentation.description = Gui.NoLocalization;

        // Lunar Chill No Cost

        var spriteLunarChill = Sprites.GetSprite("LunarChill", Resources.PowerNewMoon, 256, 128);

        var powerLunarChillNoCost = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}LunarChillNoCost")
            .SetGuiPresentation($"Power{Name}LunarChill", Category.Feature, spriteLunarChill)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetUseSpellAttack()
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.Enemy, RangeType.RangeHit, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder.DamageForm(DamageTypeCold, 1, DieType.D8),
                        EffectFormBuilder.ConditionForm(conditionLunarChillEnemy))
                    .SetParticleEffectParameters(FeatureDefinitionPowers.PowerDomainElementalIceLance)
                    .Build())
            .AddToDB();

        var conditionNewMoonNoCost = ConditionDefinitionBuilder
            .Create($"Condition{Name}NewMoonNoCost")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(powerLunarChillNoCost)
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddCustomSubFeatures(new AddUsablePowersFromCondition())
            .AddToDB();

        powerLunarChillNoCost.AddCustomSubFeatures(
            ValidatorsValidatePowerUse.InCombat,
            new MagicEffectFinishedByMeNoCost(conditionNewMoonNoCost));

        // Lunar Chill

        var powerLunarChill = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}LunarChill")
            .SetGuiPresentation(Category.Feature, spriteLunarChill)
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetUseSpellAttack()
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.Enemy, RangeType.RangeHit, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder.DamageForm(DamageTypeCold, 1, DieType.D8),
                        EffectFormBuilder.ConditionForm(conditionLunarChillEnemy))
                    .SetParticleEffectParameters(FeatureDefinitionPowers.PowerDomainElementalIceLance)
                    .Build())
            .AddToDB();

        var conditionNewMoon = ConditionDefinitionBuilder
            .Create($"Condition{Name}NewMoon")
            .SetGuiPresentation($"Power{Name}NewMoon", Category.Feature,
                ConditionDefinitions.ConditionChildOfDarkness_DimLight)
            .SetPossessive()
            .SetFeatures(powerLunarChill)
            .AddCustomSubFeatures(new AddUsablePowersFromCondition(), new ForceLightingStateNewMoon())
            .AddToDB();

        conditionNewMoon.GuiPresentation.description = Gui.NoLocalization;

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
                    .Build())
            .AddToDB();

        var featureSetLunarCloak = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}LunarCloak")
            .SetGuiPresentation(Category.Feature)
            .SetFeatureSet(powerLunarCloak, powerFullMoon, powerNewMoon)
            .AddToDB();

        // LEVEL 06

        // Midnight's Blessing

        var conditionFullMoonMidnightBlessing =
            ConditionDefinitionBuilder
                .Create(conditionFullMoon, $"Condition{Name}FullMoonMidnightBlessing")
                .AddFeatures(FeatureDefinitionDamageAffinitys.DamageAffinityRadiantResistance)
                .AddCustomSubFeatures(new AddUsablePowersFromCondition())
                .AddToDB();

        var conditionNewMoonMidnightBlessing =
            ConditionDefinitionBuilder
                .Create(conditionNewMoon, $"Condition{Name}NewMoonMidnightBlessing")
                .AddFeatures(FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance)
                .AddCustomSubFeatures(new AddUsablePowersFromCondition(), new ForceLightingStateNewMoon())
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
                    .Build())
            .AddCustomSubFeatures(new MagicEffectFinishedByMeMidnightBlessing())
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

        var conditionFullMoonLunarEmbrace =
            ConditionDefinitionBuilder
                .Create(conditionFullMoonMidnightBlessing, $"Condition{Name}FullMoonLunarEmbrace")
                .AddFeatures(FeatureDefinitionMovementAffinitys.MovementAffinityConditionFlyingAdaptive)
                .AddCustomSubFeatures(new AddUsablePowersFromCondition())
                .AddToDB();

        var conditionNewMoonLunarEmbrace =
            ConditionDefinitionBuilder
                .Create(conditionNewMoonMidnightBlessing, $"Condition{Name}NewMoonLunarEmbrace")
                .AddFeatures(FeatureDefinitionMovementAffinitys.MovementAffinityConditionFlyingAdaptive)
                .AddCustomSubFeatures(new AddUsablePowersFromCondition(), new ForceLightingStateNewMoon())
                .AddToDB();

        var featureLunarEmbrace = FeatureDefinitionBuilder
            .Create($"Feature{Name}LunarEmbrace")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        // LEVEL 14

        var powerMoonlightGuise = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}MoonlightGuise")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Reaction, RechargeRate.ShortRest)
            .SetReactionContext(ExtraReactionContext.Custom)
            .AddToDB();

        powerMoonlightGuise.AddCustomSubFeatures(new CustomBehaviorMoonlightGuise(powerMoonlightGuise));

        // MAIN

        PowerBundle.RegisterPowerBundle(powerLunarCloak, false, powerFullMoon, powerNewMoon);
        ForceGlobalUniqueEffects.AddToGroup(
            ForceGlobalUniqueEffects.Group.MoonlitNewAndFullMoon, powerFullMoon, powerNewMoon);

        powerLunarRadianceNoCost.AddCustomSubFeatures(
            new ModifyEffectDescriptionLunarRadianceOrLunarChill(powerLunarRadianceNoCost));
        powerLunarRadiance.AddCustomSubFeatures(
            new ModifyEffectDescriptionLunarRadianceOrLunarChill(powerLunarRadiance));
        powerFullMoon.AddCustomSubFeatures(
            new ModifyEffectDescriptionMidnightBlessingOrLunarEmbrace(
                powerFullMoon, conditionFullMoon, conditionFullMoonMidnightBlessing, conditionFullMoonLunarEmbrace));
        powerLunarChillNoCost.AddCustomSubFeatures(
            new ModifyEffectDescriptionLunarRadianceOrLunarChill(powerLunarChillNoCost));
        powerLunarChill.AddCustomSubFeatures(
            new ModifyEffectDescriptionLunarRadianceOrLunarChill(powerLunarChill));
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
    private sealed class MagicEffectFinishedByMeNoCost(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionFree) : IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;

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
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionPower power,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
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

            if (effectForm != null)
            {
                effectForm.ConditionForm.conditionDefinition = levels < 10
                    ? conditionMidnightBlessing
                    : conditionLunarEmbrace;
            }

            return effectDescription;
        }
    }

    // extend dice number on Lunar Radiance or Lunar Chill at level 10 onwards
    private sealed class ModifyEffectDescriptionLunarRadianceOrLunarChill(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionPower power) : IModifyEffectDescription
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

            // lunar embrace
            if (levels < 10)
            {
                return effectDescription;
            }

            var damageForm = effectDescription.FindFirstDamageForm();

            if (damageForm == null)
            {
                return effectDescription;
            }

            damageForm.diceNumber = 2;

            return effectDescription;
        }
    }

    private sealed class MagicEffectFinishedByMeMidnightBlessing : IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
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
            
            rulesetCharacter.ReceiveTemporaryHitPoints(
                levels, DurationType.Minute, 1, TurnOccurenceType.StartOfTurn, rulesetCharacter.guid);

            var effectSpell = ServiceRepository.GetService<IRulesetImplementationService>()
                .InstantiateEffectSpell(rulesetCharacter, null, MoonBeam, slotLevel, false);

            var actionParams = new CharacterActionParams(actingCharacter, ActionDefinitions.Id.CastNoCost)
            {
                RulesetEffect = effectSpell, positions = action.ActionParams.Positions
            };

            rulesetCharacter.SpellsCastByMe.TryAdd(effectSpell);
            ServiceRepository.GetService<ICommandService>()?.ExecuteAction(actionParams, null, true);

            yield break;
        }
    }

    private sealed class CustomBehaviorMoonlightGuise(
        FeatureDefinitionPower powerMoonlightGuise)
        : IAttackBeforeHitConfirmedOnMe, IMagicalAttackBeforeHitConfirmedOnMe
    {
        public IEnumerator OnAttackBeforeHitConfirmedOnMe(
            GameLocationBattleManager battle,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            RulesetEffect rulesetEffect,
            bool firstTarget,
            bool criticalHit)
        {
            if (rulesetEffect == null)
            {
                yield return HandleReaction(defender);
            }
        }

        public IEnumerator OnMagicalAttackBeforeHitConfirmedOnMe(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier magicModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            yield return HandleReaction(defender);
        }

        private IEnumerator HandleReaction(GameLocationCharacter defender)
        {
            var gameLocationBattleManager =
                ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;
            var gameLocationActionManager =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (gameLocationBattleManager is not { IsBattleInProgress: true } || gameLocationActionManager == null)
            {
                yield break;
            }

            if (!defender.CanReact())
            {
                yield break;
            }

            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetDefender.GetRemainingPowerCharges(powerMoonlightGuise) <= 0)
            {
                yield break;
            }

            var reactionParams =
                new CharacterActionParams(defender, (ActionDefinitions.Id)ExtraActionId.DoNothingReaction)
                {
                    StringParameter = "MoonlightGuise"
                };

            var previousReactionCount = gameLocationActionManager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestSpendPower(reactionParams);

            gameLocationActionManager.AddInterruptRequest(reactionRequest);

            yield return gameLocationBattleManager.WaitForReactions(
                defender, gameLocationActionManager, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            rulesetDefender.UpdateUsageForPower(powerMoonlightGuise, powerMoonlightGuise.CostPerUse);
            rulesetDefender.InflictCondition(
                ConditionInvisible,
                DurationType.Round,
                1,
                TurnOccurenceType.EndOfSourceTurn,
                AttributeDefinitions.TagEffect,
                rulesetDefender.Guid,
                rulesetDefender.CurrentFaction.Name,
                1,
                ConditionInvisible,
                0,
                0,
                0);
        }
    }
}
