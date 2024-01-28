using System.Collections;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellListDefinitions;

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
            .SetGuiPresentation(Category.Feature)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .AddToDB();

        var lightSourceForm =
            FaerieFire.EffectDescription.GetFirstFormOfType(EffectForm.EffectFormType.LightSource);

        var powerLunarRadianceNoCost = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}LunarRadianceNoCost")
            .SetGuiPresentation($"Power{Name}LunarRadiance", Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.RangeHit, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(EffectFormBuilder.DamageForm(DamageTypeRadiant, 1, DieType.D8))
                    .SetParticleEffectParameters(ShadowDagger)
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

        var powerLunarRadiance = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}LunarRadiance")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.RangeHit, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(EffectFormBuilder.DamageForm(DamageTypeRadiant, 1, DieType.D8))
                    .SetParticleEffectParameters(ShadowDagger)
                    .Build())
            .AddToDB();

        var conditionFullMoon = ConditionDefinitionBuilder
            .Create($"Condition{Name}FullMoon")
            .SetGuiPresentation(Category.Condition)
            .SetFeatures(powerLunarRadianceNoCost, powerLunarRadiance)
            .AddCustomSubFeatures(new AddUsablePowersFromCondition())
            .AddToDB();

        powerLunarRadianceNoCost.AddCustomSubFeatures(
            ValidatorsValidatePowerUse.InCombat,
            new MagicEffectFinishedByMeNoCost(powerLunarRadiance, conditionFullMoonNoCost));

        var powerFullMoon = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}FullMoon")
            .SetGuiPresentation(Category.Feature)
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

        var powerLunarChillNoCost = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}LunarChillNoCost")
            .SetGuiPresentation($"Power{Name}LunarChill", Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.Enemy, RangeType.RangeHit, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(EffectFormBuilder.DamageForm(DamageTypeCold, 1, DieType.D8))
                    .SetParticleEffectParameters(ShadowDagger)
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

        var powerLunarChill = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}LunarChill")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.Enemy, RangeType.RangeHit, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(EffectFormBuilder.DamageForm(DamageTypeCold, 1, DieType.D8))
                    .SetParticleEffectParameters(ShadowDagger)
                    .Build())
            .AddToDB();

        var conditionNewMoon = ConditionDefinitionBuilder
            .Create($"Condition{Name}NewMoon")
            .SetGuiPresentation(Category.Condition)
            .SetFeatures(powerLunarChill)
            .AddCustomSubFeatures(new AddUsablePowersFromCondition())
            .AddToDB();

        powerLunarChillNoCost.AddCustomSubFeatures(
            ValidatorsValidatePowerUse.InCombat,
            new MagicEffectFinishedByMeNoCost(powerLunarChill, conditionNewMoonNoCost));

        var powerNewMoon = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}NewMoon")
            .SetGuiPresentation(Category.Feature)
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

        var effectFormFullMoonMidnightBlessing = EffectFormBuilder.ConditionForm(
            ConditionDefinitionBuilder
                .Create($"Condition{Name}FullMoonMidnightBlessing")
                .SetGuiPresentationNoContent(true)
                .SetSilent(Silent.WhenAddedOrRemoved)
                .SetFeatures(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityRadiantResistance)
                .AddToDB(), ConditionForm.ConditionOperation.Add, true, true);
        
        var effectFormNewMoonMidnightBlessing = EffectFormBuilder.ConditionForm(
            ConditionDefinitionBuilder
                .Create($"Condition{Name}NewMoonMidnightBlessing")
                .SetGuiPresentationNoContent(true)
                .SetSilent(Silent.WhenAddedOrRemoved)
                .SetFeatures(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance)
                .AddToDB(), ConditionForm.ConditionOperation.Add, true, true);
        
        var powerMidnightBlessing = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}MidnightBlessing")
            .SetGuiPresentation(Category.Feature, MoonBeam)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(MoonBeam)
                    .AddEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetTempHpForm(6)
                            .Build())
                    .Build())
            .AddToDB();

        // LEVEL 10

        // Lunar Embrace

        var featureLunarEmbrace = FeatureDefinitionBuilder
            .Create($"Feature{Name}LunarEmbrace")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        // LEVEL 14

        var powerMoonlightGuise = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}MoonlightGuise")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Reaction)
            .SetReactionContext(ExtraReactionContext.Custom)
            .AddToDB();

        // MAIN
        
        PowerBundle.RegisterPowerBundle(powerLunarCloak, false, powerFullMoon, powerNewMoon);

        powerLunarRadianceNoCost.AddCustomSubFeatures(
            new ModifyEffectDescriptionMidnightBlessingAndLunarEmbrace(
                powerLunarRadianceNoCost, effectFormFullMoonMidnightBlessing));
        powerLunarRadiance.AddCustomSubFeatures(
            new ModifyEffectDescriptionMidnightBlessingAndLunarEmbrace(
                powerLunarRadiance, effectFormFullMoonMidnightBlessing));
        powerLunarChillNoCost.AddCustomSubFeatures(
            new ModifyEffectDescriptionMidnightBlessingAndLunarEmbrace(
                powerLunarChillNoCost, effectFormNewMoonMidnightBlessing));
        powerLunarChill.AddCustomSubFeatures(
            new ModifyEffectDescriptionMidnightBlessingAndLunarEmbrace(
                powerLunarChill, effectFormNewMoonMidnightBlessing));
        
        Subclass = CharacterSubclassDefinitionBuilder
            .Create($"Patron{Name}")
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.PatronMoonlit, 256))
            .AddFeaturesAtLevel(1, magicAffinityMoonlitExpandedSpells, featureSetLunarCloak)
            .AddFeaturesAtLevel(6, powerMidnightBlessing)
            .AddFeaturesAtLevel(10, featureLunarEmbrace)
            .AddFeaturesAtLevel(14, powerMoonlightGuise)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => DatabaseHelper.CharacterClassDefinitions.Warlock;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice => DatabaseHelper.FeatureDefinitionSubclassChoices
        .SubclassChoiceWarlockOtherworldlyPatrons;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private sealed class MagicEffectFinishedByMeNoCost(
        FeatureDefinitionPower powerBonusAction,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionFree) : IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerBonusAction, rulesetCharacter);

            rulesetCharacter.UsePower(usablePower);

            if (rulesetCharacter.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionFree.Name, out var activeCondition))
            {
                rulesetCharacter.RemoveCondition(activeCondition);
            }

            yield break;
        }
    }

    private sealed class ModifyEffectDescriptionMidnightBlessingAndLunarEmbrace(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionPower power,
        EffectForm midnightBlessing) : IModifyEffectDescription
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
            var levels = character.GetClassLevel(DatabaseHelper.CharacterClassDefinitions.Warlock);

            // midnight blessing
            if (levels < 6)
            {
                return effectDescription;
            }

            effectDescription.EffectForms.Add(midnightBlessing);

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
}
