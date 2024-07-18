using System.Collections;
using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Subclasses;
using SolastaUnfinishedBusiness.Validators;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static FeatureDefinitionAttributeModifier;

namespace SolastaUnfinishedBusiness.Spells;

internal static partial class SpellBuilders
{
    #region Mystical Cloak

    internal static SpellDefinition BuildMysticalCloak()
    {
        const string NAME = "MysticalCloak";

        var attributeModifierAC = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{NAME}")
            .SetGuiPresentation(NAME, Category.Spell)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, 2)
            .AddToDB();

        var conditionLowerPlane = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionFlyingAdaptive, $"Condition{NAME}LowerPlane")
            .SetGuiPresentation($"{NAME}LowerPlane", Category.Spell,
                ConditionDefinitions.ConditionMagicallyArmored)
            .SetPossessive()
            .SetParentCondition(ConditionDefinitions.ConditionFlying)
            .SetFeatures(
                attributeModifierAC,
                CommonBuilders.AttributeModifierCasterFightingExtraAttack,
                FeatureDefinitionMoveModes.MoveModeFly8,
                FeatureDefinitionDamageAffinitys.DamageAffinityFireImmunity,
                FeatureDefinitionDamageAffinitys.DamageAffinityPoisonImmunity,
                FeatureDefinitionConditionAffinitys.ConditionAffinityPoisonImmunity)
            .AddCustomSubFeatures(
                CanUseAttribute.SpellCastingAbility,
                new AddTagToWeaponWeaponAttack(
                    TagsDefinitions.MagicalWeapon, ValidatorsWeapon.AlwaysValid))
            .AddToDB();

        var lowerPlane = SpellDefinitionBuilder
            .Create($"{NAME}LowerPlane")
            .SetGuiPresentation(Category.Spell)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(6)
            .SetCastingTime(ActivationTime.BonusAction)
            .SetMaterialComponent(MaterialComponentType.Specific)
            .SetSpecificMaterialComponent(TagsDefinitions.ItemTagDiamond, 500, true)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionLowerPlane))
                    .SetCasterEffectParameters(MageArmor)
                    .Build())
            .AddToDB();

        var conditionHigherPlane = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionFlyingAdaptive, $"Condition{NAME}HigherPlane")
            .SetGuiPresentation($"{NAME}HigherPlane", Category.Spell,
                ConditionDefinitions.ConditionMagicallyArmored)
            .SetPossessive()
            .SetParentCondition(ConditionDefinitions.ConditionFlying)
            .SetFeatures(
                attributeModifierAC,
                CommonBuilders.AttributeModifierCasterFightingExtraAttack,
                FeatureDefinitionMoveModes.MoveModeFly8,
                FeatureDefinitionDamageAffinitys.DamageAffinityRadiantImmunity,
                FeatureDefinitionDamageAffinitys.DamageAffinityNecroticImmunity,
                FeatureDefinitionConditionAffinitys.ConditionAffinityCalmEmotionCharmedImmunity)
            .AddCustomSubFeatures(
                CanUseAttribute.SpellCastingAbility,
                new AddTagToWeaponWeaponAttack(TagsDefinitions.MagicalWeapon, ValidatorsWeapon.AlwaysValid))
            .AddToDB();

        var higherPlane = SpellDefinitionBuilder
            .Create($"{NAME}HigherPlane")
            .SetGuiPresentation(Category.Spell)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(6)
            .SetCastingTime(ActivationTime.BonusAction)
            .SetMaterialComponent(MaterialComponentType.Specific)
            .SetSpecificMaterialComponent(TagsDefinitions.ItemTagDiamond, 500, true)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionHigherPlane))
                    .SetCasterEffectParameters(MageArmor)
                    .Build())
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.MysticalCloak, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(6)
            .SetCastingTime(ActivationTime.BonusAction)
            .SetMaterialComponent(MaterialComponentType.Specific)
            .SetSpecificMaterialComponent(TagsDefinitions.ItemTagDiamond, 500, true)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetRequiresConcentration(true)
            .SetSubSpells(lowerPlane, higherPlane)
            .AddToDB();

        return spell;
    }

    #endregion

    #region Poison Wave

    internal static SpellDefinition BuildPoisonWave()
    {
        const string NAME = "PoisonWave";

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.PoisonWave, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(6)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Specific)
            .SetSpecificMaterialComponent(TagsDefinitions.ItemTagGlass, 50, false)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Sphere, 4)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                    .ExcludeCaster()
                    .SetSavingThrowData(false, AttributeDefinitions.Constitution, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypePoison, 6, DieType.D10)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.EndOfTurn, true)
                            .SetConditionForm(ConditionDefinitions.ConditionPoisoned,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .SetParticleEffectParameters(PoisonSpray)
                    .SetImpactEffectParameters(PowerDragonBreath_Poison)
                    .SetEffectEffectParameters(PowerVrockSpores)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Shelter From Energy

    private static readonly List<(string, IMagicEffect, AssetReference)> ShelterDamageTypes =
    [
        (DamageTypeAcid, AcidArrow,
            PowerDragonbornBreathWeaponBlack.EffectDescription.EffectParticleParameters.impactParticleReference),
        (DamageTypeCold, SleetStorm,
            PowerBulette_Snow_Leap.EffectDescription.EffectParticleParameters.impactParticleReference),
        (DamageTypeFire, HeatMetal,
            FireStorm.EffectDescription.EffectParticleParameters.impactParticleReference),
        (DamageTypeLightning, LightningBolt,
            Thunderstorm.EffectDescription.EffectParticleParameters.impactParticleReference),
        (DamageTypeNecrotic, FingerOfDeath,
            PowerPatronFiendDarkOnesOwnLuck.EffectDescription.EffectParticleParameters.effectParticleReference),
        (DamageTypeRadiant, GuardianOfFaith,
            PowerOathOfJugementPurgeCorruption.EffectDescription.EffectParticleParameters.effectParticleReference),
        (DamageTypeThunder, Thunderwave,
            Thunderwave.EffectDescription.EffectParticleParameters.impactParticleReference)
    ];

    internal static SpellDefinition BuildShelterFromEnergy()
    {
        const string NAME = "ShelterFromEnergy";

        var subSpells = new List<SpellDefinition>();

        foreach (var (damageType, casterEffect, impactEffect) in ShelterDamageTypes)
        {
            var title = Gui.Localize($"Tooltip/&Tag{damageType}Title");
            var description = Gui.Format($"Feedback/&{NAME}Description", title);

            subSpells.Add(
                SpellDefinitionBuilder
                    .Create($"Spell{NAME}{damageType}")
                    .SetGuiPresentation(title, description)
                    .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolAbjuration)
                    .SetSpellLevel(6)
                    .SetCastingTime(ActivationTime.Action)
                    .SetMaterialComponent(MaterialComponentType.Mundane)
                    .SetVerboseComponent(true)
                    .SetSomaticComponent(true)
                    .SetVocalSpellSameType(VocalSpellSemeType.Buff)
                    .SetEffectDescription(EffectDescriptionBuilder.Create()
                        .SetDurationData(DurationType.Hour, 1)
                        .SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.IndividualsUnique, 6)
                        .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel,
                            additionalTargetsPerIncrement: 1)
                        .SetEffectForms(EffectFormBuilder.ConditionForm(ConditionDefinitionBuilder
                            .Create($"Condition{NAME}{damageType}")
                            .SetGuiPresentation(
                                Gui.Format($"Condition/&Condition{NAME}Title", title),
                                Gui.NoLocalization,
                                ConditionAuraOfProtection)
                            .SetPossessive()
                            .SetFeatures(
                                FeatureDefinitionDamageAffinityBuilder
                                    .Create($"DamageAffinity{NAME}{damageType}")
                                    .SetGuiPresentation(NAME, Category.Spell, Gui.NoLocalization)
                                    .SetDamageType(damageType)
                                    .SetDamageAffinityType(DamageAffinityType.Resistance)
                                    .AddToDB())
                            .AddToDB()))
                        .SetCasterEffectParameters(casterEffect)
                        .SetImpactEffectParameters(impactEffect)
                        .Build())
                    .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
                    .AddToDB());
        }

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.ShelterFromEnergy, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolAbjuration)
            .SetSpellLevel(6)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Hour, 1)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.IndividualsUnique, 6)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel,
                        additionalTargetsPerIncrement: 1)
                    .Build())
            .SetSubSpells([.. subSpells])
            .AddToDB();

        return spell;
    }

    #endregion

    #region Fizban Platinum Shield

    internal static SpellDefinition BuildFizbanPlatinumShield()
    {
        const string NAME = "FizbanPlatinumShield";

        var conditionSelf = ConditionDefinitionBuilder
            .Create($"Condition{NAME}Self")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
            .AddToDB();

        var conditionMark = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(NAME, Category.Spell, ConditionDefinitions.ConditionMagicallyArmored)
            .SetPossessive()
            .SetFeatures(
                FeatureDefinitionDamageAffinitys.DamageAffinityAcidResistance,
                FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance,
                FeatureDefinitionDamageAffinitys.DamageAffinityFireResistance,
                FeatureDefinitionDamageAffinitys.DamageAffinityLightningResistance,
                FeatureDefinitionDamageAffinitys.DamageAffinityPoisonResistance,
                FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityRogueEvasion,
                FeatureDefinitionCombatAffinityBuilder
                    .Create($"CombatAffinity{NAME}")
                    .SetGuiPresentation(NAME, Category.Spell, "UI/&HasHalfCover")
                    .SetPermanentCover(CoverType.Half)
                    .AddToDB())
            .SetConditionParticleReference(WardingBond)
            .AddToDB();

        conditionMark.GuiPresentation.description = Gui.NoLocalization;

        var lightSourceForm = FaerieFire.EffectDescription.GetFirstFormOfType(EffectForm.EffectFormType.LightSource);

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.PrimordialWard, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolAbjuration)
            .SetSpellLevel(6)
            .SetCastingTime(ActivationTime.BonusAction)
            .SetMaterialComponent(MaterialComponentType.Specific)
            .SetSpecificMaterialComponent(TagsDefinitions.ItemTagDiamond, 500, false)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(conditionMark),
                        EffectFormBuilder
                            .Create()
                            .SetLightSourceForm(
                                LightSourceType.Basic, 6, 6,
                                lightSourceForm.lightSourceForm.color,
                                lightSourceForm.lightSourceForm.graphicsPrefabReference)
                            .Build(),
                        EffectFormBuilder.ConditionForm(
                            conditionSelf,
                            ConditionForm.ConditionOperation.Add, true))
                    .SetCasterEffectParameters(PrismaticSpray)
                    .Build())
            .AddToDB();

        var behavior = new PowerOrSpellFinishedByMeFizbanPlatinumShield(spell, conditionMark);

        var power = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}")
            .SetGuiPresentation(NAME, Category.Spell, spell)
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(spell)
                    .SetEffectForms()
                    .Build())
            .AddCustomSubFeatures(behavior, new FilterTargetingCharacter(conditionMark))
            .AddToDB();

        conditionSelf.Features.Add(power);
        spell.AddCustomSubFeatures(behavior);

        return spell;
    }

    private sealed class FilterTargetingCharacter(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionMark) : IFilterTargetingCharacter
    {
        public bool EnforceFullSelection => false;

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            if (target.RulesetCharacter == null)
            {
                return false;
            }

            var isValid = !target.RulesetCharacter.HasConditionOfCategoryAndType(
                AttributeDefinitions.TagEffect, conditionMark.Name);

            if (!isValid)
            {
                __instance.actionModifier.FailureFlags.Add("Tooltip/&MustNotHaveFizbanPlatinumShield");
            }

            return isValid;
        }
    }

    private sealed class PowerOrSpellFinishedByMeFizbanPlatinumShield(
        SpellDefinition spell,
        ConditionDefinition condition) : IPowerOrSpellFinishedByMe
    {
        private const string FizbanPlatinumShieldTag = "FizbanPlatinumShieldTag";

        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var actingCharacter = action.ActingCharacter;
            var rulesetCharacter = actingCharacter.RulesetCharacter;
            var rulesetSpell = rulesetCharacter.ConcentratedSpell;

            if (rulesetSpell == null ||
                rulesetSpell.SpellDefinition != spell)
            {
                yield break;
            }

            switch (action)
            {
                case CharacterActionUsePower:
                {
                    actingCharacter.UsedSpecialFeatures.TryAdd(FizbanPlatinumShieldTag, 0);
                    actingCharacter.UsedSpecialFeatures.TryAdd(FizbanPlatinumShieldTag, rulesetSpell.RemainingRounds);

                    var spellRepertoire = rulesetSpell.SpellRepertoire;
                    var actionService = ServiceRepository.GetService<IGameLocationActionService>();
                    var effectSpell = ServiceRepository.GetService<IRulesetImplementationService>()
                        .InstantiateEffectSpell(rulesetCharacter, spellRepertoire, spell, 6, false);

                    var actionParams = action.ActionParams.Clone();

                    actionParams.ActionDefinition = actionService.AllActionDefinitions[ActionDefinitions.Id.CastNoCost];
                    actionParams.RulesetEffect = effectSpell;

                    rulesetCharacter.SpellsCastByMe.TryAdd(effectSpell);
                    actionService.ExecuteAction(actionParams, null, true);
                    break;
                }
                case CharacterActionCastSpell
                    when actingCharacter.UsedSpecialFeatures.TryGetValue(FizbanPlatinumShieldTag, out var value) &&
                         value > 0:
                    rulesetSpell.RemainingRounds = value;

                    if (action.ActionParams.TargetCharacters[0].RulesetActor.TryGetConditionOfCategoryAndType(
                            AttributeDefinitions.TagEffect, condition.Name, out var activeCondition))
                    {
                        activeCondition.RemainingRounds = value;
                    }

                    actingCharacter.UsedSpecialFeatures.TryAdd(FizbanPlatinumShieldTag, 0);
                    break;
            }
        }
    }

    #endregion

    #region Flash Freeze

    internal static SpellDefinition BuildFlashFreeze()
    {
        const string NAME = "FlashFreeze";

        var conditionFlashFreeze = ConditionDefinitionBuilder
            .Create(ConditionGrappledRestrainedRemorhaz, $"Condition{NAME}")
            .SetGuiPresentation(
                RuleDefinitions.ConditionRestrained, Category.Rules, ConditionDefinitions.ConditionChilled)
            .SetPossessive()
            .SetParentCondition(ConditionRestrainedByWeb)
            .AddToDB();

        conditionFlashFreeze.specialDuration = false;
        conditionFlashFreeze.specialInterruptions.Clear();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.FLashFreeze, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(6)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 2)
                    .SetSavingThrowData(false, AttributeDefinitions.Dexterity, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypeCold, 10, DieType.D6)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(conditionFlashFreeze, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .SetParticleEffectParameters(PowerDomainElementalHeraldOfTheElementsCold)
                    .SetCasterEffectParameters(SleetStorm)
                    .Build())
            .AddCustomSubFeatures(new FilterTargetingCharacterFlashFreeze())
            .AddToDB();

        spell.EffectDescription.EffectParticleParameters.conditionStartParticleReference =
            ConditionDefinitions.ConditionRestrained.conditionStartParticleReference;
        spell.EffectDescription.EffectParticleParameters.conditionParticleReference =
            ConditionDefinitions.ConditionRestrained.conditionParticleReference;
        spell.EffectDescription.EffectParticleParameters.conditionEndParticleReference =
            ConditionDefinitions.ConditionRestrained.conditionEndParticleReference;

        return spell;
    }

// ReSharper disable once SuggestBaseTypeForParameterInConstructor
    private sealed class FilterTargetingCharacterFlashFreeze : IFilterTargetingCharacter
    {
        public bool EnforceFullSelection => false;

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            if (target.RulesetCharacter == null)
            {
                return false;
            }

            var rulesetCharacter = target.RulesetCharacter;
            var isValid = rulesetCharacter.SizeDefinition != CharacterSizeDefinitions.DragonSize
                          && rulesetCharacter.SizeDefinition != CharacterSizeDefinitions.Gargantuan
                          && rulesetCharacter.SizeDefinition != CharacterSizeDefinitions.Huge
                          && rulesetCharacter.SizeDefinition != CharacterSizeDefinitions.SpiderQueenSize;

            if (!isValid)
            {
                __instance.actionModifier.FailureFlags.Add("Tooltip/&MustBeLargeOrSmaller");
            }

            return isValid;
        }
    }

    #endregion

    #region Heroic Infusion

    internal static SpellDefinition BuildHeroicInfusion()
    {
        const string NAME = "HeroicInfusion";

        var attackModifierHeroicInfusion = FeatureDefinitionCombatAffinityBuilder
            .Create($"AttackModifier{NAME}")
            .SetGuiPresentation($"Condition{NAME}", Category.Condition, Gui.NoLocalization)
            .SetMyAttackAdvantage(AdvantageType.Advantage)
            .SetSituationalContext(ExtraSituationalContext.HasSimpleOrMartialWeaponInHands)
            .AddToDB();

        var additionalDamageHeroicInfusion = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{NAME}")
            .SetGuiPresentation($"Condition{NAME}", Category.Condition, Gui.NoLocalization)
            .SetNotificationTag(NAME)
            .SetDamageDice(DieType.D12, 2)
            .SetSpecificDamageType(DamageTypeForce)
            .AddToDB();

        var actionAffinityHeroicInfusion = FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{NAME}")
            .SetGuiPresentation($"Condition{NAME}", Category.Condition, Gui.NoLocalization)
            .SetAuthorizedActions()
            .SetForbiddenActions(
                ActionDefinitions.Id.CastBonus, ActionDefinitions.Id.CastInvocation,
                ActionDefinitions.Id.CastMain, ActionDefinitions.Id.CastReaction,
                ActionDefinitions.Id.CastReadied, ActionDefinitions.Id.CastRitual, ActionDefinitions.Id.CastNoCost)
            .AddToDB();

        var conditionExhausted = ConditionDefinitionBuilder
            .Create(ConditionExhausted, $"Condition{NAME}Exhausted")
            .SetOrUpdateGuiPresentation("ConditionExhausted", Category.Rules, ConditionLethargic)
            .AddToDB();

        var conditionHeroicInfusion = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(Category.Condition, ConditionHeroism)
            .SetPossessive()
            .SetSpecialDuration(DurationType.Minute, 10)
            .SetFeatures(
                attackModifierHeroicInfusion,
                additionalDamageHeroicInfusion,
                actionAffinityHeroicInfusion,
                CommonBuilders.AttributeModifierCasterFightingExtraAttack,
                FeatureDefinitionProficiencys.ProficiencyFighterArmor,
                FeatureDefinitionProficiencys.ProficiencyFighterSavingThrow,
                FeatureDefinitionProficiencys.ProficiencyFighterWeapon)
            .AddCustomSubFeatures(new OnConditionAddedOrRemovedHeroicInfusion(conditionExhausted))
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.HeroicInfusion, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(6)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.UntilAnyRest)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(conditionHeroicInfusion),
                        EffectFormBuilder
                            .Create()
                            .SetTempHpForm(50)
                            .Build())
                    .SetParticleEffectParameters(DivineFavor)
                    .Build())
            .AddToDB();

        return spell;
    }

    private sealed class OnConditionAddedOrRemovedHeroicInfusion(ConditionDefinition conditionExhausted)
        : IOnConditionAddedOrRemoved
    {
        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // empty
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            target.TemporaryHitPoints = 0;


            var modifierTrend = target.actionModifier.savingThrowModifierTrends;
            var advantageTrends = target.actionModifier.savingThrowAdvantageTrends;
            var conModifier = AttributeDefinitions.ComputeAbilityScoreModifier(
                target.TryGetAttributeValue(AttributeDefinitions.Constitution));

            target.RollSavingThrow(0, AttributeDefinitions.Constitution, null, modifierTrend,
                advantageTrends, conModifier, 15, false, out var savingOutcome, out _);

            if (savingOutcome is RollOutcome.Success)
            {
                return;
            }

            target.InflictCondition(
                conditionExhausted.Name,
                conditionExhausted.DurationType,
                conditionExhausted.DurationParameter,
                conditionExhausted.TurnOccurence,
                AttributeDefinitions.TagEffect,
                target.guid,
                target.CurrentFaction.Name,
                1,
                conditionExhausted.Name,
                0,
                0,
                0);
        }
    }

    #endregion

    #region Ring of Blades

    internal static SpellDefinition BuildRingOfBlades()
    {
        const string NAME = "RingOfBlades";

        var powerRingOfBlades = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}")
            .SetGuiPresentation(Category.Feature, Sprites.GetSprite($"Power{NAME}", Resources.PowerRingOfBlades, 128))
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.None, 1, 6)
            .SetUseSpellAttack()
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.RangeHit, 12, TargetType.IndividualsUnique)
                    .SetEffectForms(EffectFormBuilder.DamageForm(DamageTypeForce, 4, DieType.D10))
                    .SetParticleEffectParameters(ShadowDagger)
                    .SetCasterEffectParameters(PowerDomainLawWordOfLaw)
                    .Build())
            .AddToDB();

        var powerRingOfBladesFree = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}Free")
            .SetGuiPresentation($"Power{NAME}", Category.Feature,
                Sprites.GetSprite($"Power{NAME}", Resources.PowerRingOfBlades, 128))
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.None, 1, 6)
            .SetUseSpellAttack()
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.RangeHit, 12, TargetType.IndividualsUnique)
                    .SetEffectForms(EffectFormBuilder.DamageForm(DamageTypeForce, 4, DieType.D10))
                    .SetParticleEffectParameters(ShadowDagger)
                    .SetCasterEffectParameters(PowerDomainLawWordOfLaw)
                    .Build())
            .AddToDB();

        var conditionRingOfBlades = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation($"Power{NAME}", Category.Feature, ConditionGuided)
            .SetPossessive()
            .SetConditionType(ConditionType.Beneficial)
            .SetFeatures(powerRingOfBlades)
            .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
            .CopyParticleReferences(PowerSorcererChildRiftDeflection)
            .AddToDB();

        conditionRingOfBlades.GuiPresentation.description = Gui.NoLocalization;

        var conditionRingOfBladesFree = ConditionDefinitionBuilder
            .Create($"Condition{NAME}Free")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(powerRingOfBladesFree)
            .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddToDB();

        powerRingOfBladesFree.AddCustomSubFeatures(
            ValidatorsValidatePowerUse.InCombat,
            // it's indeed powerRingOfBlades here
            new PowerOrSpellFinishedByMeRingOfBladesFree(powerRingOfBlades, conditionRingOfBladesFree));

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.RingOfBlades, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
            .SetSpellLevel(6)
            .SetCastingTime(ActivationTime.BonusAction)
            .SetMaterialComponent(MaterialComponentType.Specific)
            .SetSpecificMaterialComponent(TagsDefinitions.WeaponTagMelee, 500, false)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 10)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(conditionRingOfBlades),
                        EffectFormBuilder.ConditionForm(conditionRingOfBladesFree))
                    .SetParticleEffectParameters(HypnoticPattern)
                    .SetEffectEffectParameters(PowerMagebaneSpellCrusher)
                    .Build())
            .AddCustomSubFeatures(new ModifyEffectDescriptionRingOfBlades(powerRingOfBlades, conditionRingOfBlades))
            .AddToDB();

        return spell;
    }

    private sealed class PowerOrSpellFinishedByMeRingOfBladesFree(
        FeatureDefinitionPower powerRingOfBlades,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionRingOfBladesFree) : IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerRingOfBlades, rulesetCharacter);

            rulesetCharacter.UsePower(usablePower);

            if (rulesetCharacter.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionRingOfBladesFree.Name, out var activeCondition))
            {
                rulesetCharacter.RemoveCondition(activeCondition);
            }

            yield break;
        }
    }

    private sealed class ModifyEffectDescriptionRingOfBlades(
        FeatureDefinitionPower powerRingOfBlades,
        ConditionDefinition conditionRingOfBlades) : IModifyEffectDescription
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == powerRingOfBlades;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var damageForm = effectDescription.FindFirstDamageForm();

            if (damageForm == null)
            {
                return effectDescription;
            }

            if (!character.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect,
                    conditionRingOfBlades.Name,
                    out var activeCondition))
            {
                return effectDescription;
            }

            damageForm.diceNumber = 4 + activeCondition.EffectLevel - 6;

            return effectDescription;
        }
    }

    #endregion

    #region Scatter

    internal static SpellDefinition BuildScatter()
    {
        const string NAME = "Scatter";

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite($"Power{NAME}", Resources.Scatter, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
            .SetSpellLevel(6)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Detection)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Distance, 24, TargetType.Position)
                    .InviteOptionalAlly()
                    .SetSavingThrowData(true, AttributeDefinitions.Wisdom, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetMotionForm(MotionForm.MotionType.TeleportToDestination, 6)
                            .Build())
                    .SetParticleEffectParameters(DimensionDoor)
                    .Build())
            .AddCustomSubFeatures(new ModifyTeleportEffectBehaviorScatter())
            .AddToDB();

        spell.EffectDescription.EffectParticleParameters.targetParticleReference = new AssetReference();

        return spell;
    }

    private sealed class ModifyTeleportEffectBehaviorScatter : IModifyTeleportEffectBehavior
    {
        public bool AllyOnly => false;

        public bool TeleportSelf => false;

        public int MaxTargets => 5;
    }

    #endregion
}
