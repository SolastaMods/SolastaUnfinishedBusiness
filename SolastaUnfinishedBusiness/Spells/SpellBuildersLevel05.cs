using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using TA;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Spells;

internal static partial class SpellBuilders
{
    #region Far Step

    internal static SpellDefinition BuildFarStep()
    {
        var condition = ConditionDefinitionBuilder
            .Create("ConditionFarStep")
            .SetGuiPresentation(Category.Condition, ConditionJump)
            .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
            .SetSilent(Silent.None)
            .SetPossessive()
            .SetFeatures(CustomActionIdContext.FarStep)
            .AddToDB();

        return SpellDefinitionBuilder
            .Create("FarStep")
            .SetGuiPresentation(Category.Spell, Sprites.FarStep)
            .SetSpellLevel(5)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
            .SetCastingTime(ActivationTime.BonusAction)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(false)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.Position)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.TeleportToDestination)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(condition, ConditionForm.ConditionOperation.Add, true, true)
                            .Build())
                    .SetParticleEffectParameters(MistyStep)
                    .UseQuickAnimations()
                    .Build())
            .AddToDB();
    }

    #endregion

    #region Mantle of Thorns

    internal static SpellDefinition BuildMantleOfThorns()
    {
        const string NAME = "MantleOfThorns";

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.MantleOfThorns, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(5)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Debuff)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetParticleEffectParameters(SpikeGrowth)
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 3)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetRecurrentEffect(RecurrentEffect.OnEnter | RecurrentEffect.OnMove | RecurrentEffect.OnTurnStart)
                    .AddEffectForms(
                        EffectFormBuilder.DamageForm(DamageTypePiercing, 2, DieType.D8),
                        EffectFormBuilder.TopologyForm(TopologyForm.Type.DangerousZone, false),
                        EffectFormBuilder.TopologyForm(TopologyForm.Type.DifficultThrough, false))
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Synaptic Static

    internal static SpellDefinition BuildSynapticStatic()
    {
        const string NAME = "SynapticStatic";

        var conditionMuddled = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(Category.Condition, Gui.NoLocalization, ConditionDazzled)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .SetFeatures(
                FeatureDefinitionCombatAffinityBuilder
                    .Create($"CombatAffinity{NAME}")
                    .SetGuiPresentation($"Condition{NAME}", Category.Condition, Gui.NoLocalization)
                    .SetMyAttackModifierSign(AttackModifierSign.Substract)
                    .SetMyAttackModifierDieType(DieType.D6)
                    .AddToDB(),
                FeatureDefinitionAbilityCheckAffinityBuilder
                    .Create($"AbilityCheckAffinity{NAME}")
                    .SetGuiPresentation($"Condition{NAME}", Category.Condition, Gui.NoLocalization)
                    .BuildAndSetAffinityGroups(CharacterAbilityCheckAffinity.None, DieType.D6, 1,
                        AbilityCheckGroupOperation.SubstractDie,
                        [
                            (AttributeDefinitions.Strength, string.Empty),
                            (AttributeDefinitions.Dexterity, string.Empty),
                            (AttributeDefinitions.Constitution, string.Empty),
                            (AttributeDefinitions.Intelligence, string.Empty),
                            (AttributeDefinitions.Wisdom, string.Empty),
                            (AttributeDefinitions.Charisma, string.Empty)
                        ])
                    .AddToDB())
            .SetConditionParticleReference(ConditionFeebleMinded)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.SynapticStatic, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(5)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(false)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.All, RangeType.Distance, 24, TargetType.Sphere, 4)
                    .SetSavingThrowData(false, AttributeDefinitions.Intelligence, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypePsychic, 8, DieType.D6)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.EndOfTurn, true)
                            .SetConditionForm(conditionMuddled, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .SetParticleEffectParameters(Feeblemind)
                    .SetImpactEffectParameters(PowerSorakDreadLaughter)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Sonic Boom

    internal static SpellDefinition BuildSonicBoom()
    {
        const string NAME = "SonicBoom";

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetTargetingData(Side.All, RangeType.Distance, 12, TargetType.Sphere, 6)
            .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
            .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, 2, additionalDicePerIncrement: 1)
            .SetParticleEffectParameters(Thunderwave)
            .SetSavingThrowData(
                false, AttributeDefinitions.Strength, false, EffectDifficultyClassComputation.SpellCastingFeature)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(DamageTypeThunder, 6, DieType.D8)
                    .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                    .Build(),
                EffectFormBuilder
                    .Create()
                    .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 6)
                    .HasSavingThrow(EffectSavingThrowType.Negates)
                    .Build())
            .Build();

        effectDescription.EffectParticleParameters.zoneParticleReference =
            Shatter.EffectDescription.EffectParticleParameters.zoneParticleReference;

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.SonicBoom, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(5)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetCastingTime(ActivationTime.Action)
            .SetEffectDescription(effectDescription)
            .AddCustomSubFeatures(ForcePushOrDragFromEffectPoint.Marker)
            .AddToDB();

        return spell;
    }

    #endregion

    #region Incineration

    internal static SpellDefinition BuildIncineration()
    {
        const string NAME = "Incineration";

        var lightSourceForm = FaerieFire.EffectDescription
            .GetFirstFormOfType(EffectForm.EffectFormType.LightSource).LightSourceForm;

        var conditionIncineration = ConditionDefinitionBuilder
            .Create(ConditionOnFire, $"Condition{NAME}")
            .SetParentCondition(ConditionOnFire)
            .SetFeatures()
            .SetRecurrentEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(DamageTypeFire, 8, DieType.D6)
                    .SetCreatedBy()
                    .Build())
            .AddToDB();

        conditionIncineration.specialDuration = false;

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.Immolation, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(5)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 18, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Dexterity, true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypeFire, 8, DieType.D6)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(conditionIncineration, ConditionForm.ConditionOperation.Add)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetLightSourceForm(
                                LightSourceType.Basic, 6, 6, lightSourceForm.Color,
                                lightSourceForm.graphicsPrefabReference)
                            .Build())
                    .SetParticleEffectParameters(Fireball)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Divine Wrath

    internal static SpellDefinition BuildDivineWrath()
    {
        const string NAME = "DivineWrath";

        var spellRadiant = SpellDefinitionBuilder
            .Create($"{NAME}Radiant")
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.DivineWrath, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(5)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 6)
                    .SetSavingThrowData(false, AttributeDefinitions.Constitution, true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypeThunder, 5, DieType.D6)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypeRadiant, 5, DieType.D6)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetMotionForm(MotionForm.MotionType.FallProne)
                            .Build())
                    .SetParticleEffectParameters(HolyAura)
                    .SetImpactEffectParameters(Sunburst)
                    .Build())
            .AddToDB();

        var spellNecrotic = SpellDefinitionBuilder
            .Create($"{NAME}Necrotic")
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.DivineWrath, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(5)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 6)
                    .SetSavingThrowData(false, AttributeDefinitions.Constitution, true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypeThunder, 5, DieType.D6)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypeNecrotic, 5, DieType.D6)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetMotionForm(MotionForm.MotionType.FallProne)
                            .Build())
                    .SetParticleEffectParameters(HolyAura)
                    .Build())
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.DivineWrath, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(5)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetSubSpells(spellNecrotic, spellRadiant)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 6)
                    .SetSavingThrowData(false, AttributeDefinitions.Constitution, true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypeThunder, 5, DieType.D6)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetMotionForm(MotionForm.MotionType.FallProne)
                            .Build())
                    .SetParticleEffectParameters(HolyAura)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion

    #region Dawn

    internal static SpellDefinition BuildDawn()
    {
        const string NAME = "Dawn";

        var effectProxy = EffectProxyDefinitionBuilder
            .Create(EffectProxyDefinitions.ProxyDaylight, $"Proxy{NAME}")
            .SetOrUpdateGuiPresentation(Category.Proxy)
            .SetActionId(ExtraActionId.ProxyDawn)
            .SetCanMove()
            .SetAdditionalFeatures(FeatureDefinitionMoveModes.MoveModeMove12, FeatureDefinitionMoveModes.MoveModeFly12)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.Dawn, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(5)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.All, RangeType.Distance, 24, TargetType.Cylinder, 6, 8)
                    .SetSavingThrowData(false, AttributeDefinitions.Constitution, true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetRecurrentEffect(RecurrentEffect.OnActivation | RecurrentEffect.OnTurnEnd)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetSummonEffectProxyForm(effectProxy)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypeRadiant, 4, DieType.D10)
                            .Build())
                    .SetParticleEffectParameters(Daylight)
                    .SetImpactEffectParameters(Sunburst)
                    .Build())
            .AddToDB();

        spell.EffectDescription.effectParticleParameters.activeEffectImpactParticleReference =
            Sunburst.EffectDescription.EffectParticleParameters.impactParticleReference;

        return spell;
    }

    #endregion

    #region Empowered Knowledge

    internal static SpellDefinition BuildEmpoweredKnowledge()
    {
        const string NAME = "EmpoweredKnowledge";

        LimitEffectInstances limiter = new("EmpoweredKnowledge", _ => 1);

        var skillsDb = DatabaseRepository.GetDatabase<SkillDefinition>();
        var powers = new List<FeatureDefinitionPower>();
        var powerPool = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.NoCost)
            .AddToDB();

        foreach (var skill in skillsDb)
        {
            var power = FeatureDefinitionPowerSharedPoolBuilder
                .Create($"Power{NAME}{skill.Name}")
                .SetGuiPresentation(skill.GuiPresentation.Title, skill.GuiPresentation.Description)
                .SetSharedPool(ActivationTime.NoCost, powerPool)
                .SetEffectDescription(
                    EffectDescriptionBuilder
                        .Create()
                        .SetDurationData(DurationType.Hour, 1)
                        .SetTargetingData(Side.Ally, RangeType.Touch, 0, TargetType.IndividualsUnique)
                        .SetEffectForms(
                            EffectFormBuilder.ConditionForm(
                                ConditionDefinitionBuilder
                                    .Create($"Condition{NAME}{skill.Name}")
                                    .SetGuiPresentation(
                                        skill.GuiPresentation.Title, Gui.NoLocalization, ConditionBullsStrength)
                                    .SetPossessive()
                                    .SetFeatures(
                                        FeatureDefinitionProficiencyBuilder
                                            .Create($"Proficiency{NAME}{skill.Name}")
                                            .SetGuiPresentation(skill.GuiPresentation)
                                            .SetProficiencies(ProficiencyType.Expertise, skill.Name)
                                            .AddToDB())
                                    .AddCustomSubFeatures(new OnConditionAddedOrRemovedEmpoweredKnowledge(skill.Name))
                                    .AddToDB()))
                        .Build())
                .AddCustomSubFeatures(limiter)
                .AddToDB();

            power.GuiPresentation.hidden = true;

            powers.Add(power);
        }

        PowerBundle.RegisterPowerBundle(powerPool, false, powers);

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.EmpoweredKnowledge, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolDivination)
            .SetSpellLevel(5)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Hour, 1)
                    .SetTargetingData(Side.All, RangeType.Touch, 0, TargetType.IndividualsUnique)
                    .SetCasterEffectParameters(TrueSeeing)
                    .SetEffectEffectParameters(PowerPaladinCleansingTouch)
                    .Build())
            .AddCustomSubFeatures(new PowerOrSpellFinishedByMeEmpoweredKnowledge(powerPool, [.. powers]))
            .AddToDB();

        return spell;
    }

    private sealed class OnConditionAddedOrRemovedEmpoweredKnowledge(string skillName) : IOnConditionAddedOrRemoved
    {
        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // empty
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            var hero = target.GetOriginalHero();

            hero?.ExpertiseProficiencies.Remove(skillName);
        }
    }

    private sealed class PowerOrSpellFinishedByMeEmpoweredKnowledge(
        FeatureDefinitionPower powerPool,
        params FeatureDefinitionPower[] powers) : IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var actionManager =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
            var battleManager = ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (!actionManager || !battleManager || action.Countered)
            {
                yield break;
            }

            var actingCharacter = action.ActingCharacter;
            var rulesetCharacter = actingCharacter.RulesetCharacter;
            var target = action.ActionParams.TargetCharacters[0];
            var rulesetTarget = target.RulesetCharacter;
            var hero = rulesetTarget.GetOriginalHero();

            if (hero == null)
            {
                yield break;
            }

            var usablePowers = new List<RulesetUsablePower>();
            var skillsDb = DatabaseRepository.GetDatabase<SkillDefinition>();

            foreach (var power in powers)
            {
                var skillName = power.Name.Replace("PowerEmpoweredKnowledge", string.Empty);

                if (!skillsDb.TryGetElement(skillName, out var skill))
                {
                    continue;
                }

                var hasSkill =
                    hero.SkillProficiencies.Contains(skill.Name) ||
                    hero.TrainedSkills.Contains(skill) ||
                    hero.GetFeaturesByType<FeatureDefinitionProficiency>()
                        .Any(x =>
                            x.ProficiencyType == ProficiencyType.Skill &&
                            x.Proficiencies.Contains(skillName));

                var hasExpertise =
                    hero.TrainedExpertises.Contains(skill.name) ||
                    hero.ExpertiseProficiencies.Contains(skill.Name);

                if (!hasSkill || hasExpertise)
                {
                    continue;
                }

                var up = PowerProvider.Get(power, rulesetCharacter);

                usablePowers.Add(up);
                rulesetCharacter.UsablePowers.Add(up);
            }

            var implementationManager =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(powerPool, rulesetCharacter);
            var actionParams = new CharacterActionParams(actingCharacter, ActionDefinitions.Id.SpendPower)
            {
                StringParameter = "EmpoweredKnowledge",
                RulesetEffect = implementationManager
                    .MyInstantiateEffectPower(rulesetCharacter, usablePower, false),
                UsablePower = usablePower,
                TargetCharacters = { target }
            };
            var count = actionManager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestSpendBundlePower(actionParams);

            actionManager.AddInterruptRequest(reactionRequest);

            yield return battleManager.WaitForReactions(actingCharacter, actionManager, count);

            rulesetCharacter.UsablePowers.Remove(usablePower);
            usablePowers.ForEach(x => rulesetCharacter.UsablePowers.Remove(x));

            if (!actionParams.ReactionValidated)
            {
                yield break;
            }

            var selectedPower = powers[reactionRequest.SelectedSubOption];
            var locationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
            var contenders =
                locationCharacterService.PartyCharacters.Union(locationCharacterService.GuestCharacters)
                    .ToList();

            foreach (var contender in contenders)
            {
                var rulesetContender = contender.RulesetCharacter;

                foreach (var skill in skillsDb)
                {
                    var conditionName = $"ConditionEmpoweredKnowledge{skill.Name}";

                    if (rulesetContender.TryGetConditionOfCategoryAndType(
                            AttributeDefinitions.TagEffect, conditionName, out var activeCondition) &&
                        activeCondition.SourceGuid == rulesetCharacter.Guid &&
                        (activeCondition.TargetGuid != rulesetTarget.Guid || !selectedPower.Name.Contains(skill.Name)))
                    {
                        rulesetContender.RemoveCondition(activeCondition);
                    }
                }
            }
        }
    }

    #endregion

    #region Steel Whirlwind

    internal static SpellDefinition BuildSteelWhirlwind()
    {
        const string Name = "SteelWhirlwind";

        var powerTeleport = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Teleport")
            .SetGuiPresentation(Category.Feature, PowerMelekTeleport)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.None)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.Position)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.TeleportToDestination)
                            .Build())
                    .SetParticleEffectParameters(PowerMelekTeleport)
                    .Build())
            .AddCustomSubFeatures(new FilterTargetingPositionSteelWhirlwind())
            .AddToDB();

        var conditionTeleport = ConditionDefinitionBuilder
            .Create($"Condition{Name}Teleport")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialInterruptions(ConditionInterruption.UsedActionOrReaction, ConditionInterruption.Moved)
            .SetFeatures(powerTeleport)
            .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(Name, Resources.SteelWhirlwind, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
            .SetSpellLevel(5)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Specific)
            .SetSpecificMaterialComponent(TagsDefinitions.WeaponTagMelee, 0, false)
            .SetVerboseComponent(false)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Enemy, RangeType.MeleeHit, 6, TargetType.IndividualsUnique, 5)
                    .SetEffectForms(
                        EffectFormBuilder.DamageForm(DamageTypeForce, 6, DieType.D10),
                        EffectFormBuilder.ConditionForm(
                            conditionTeleport, ConditionForm.ConditionOperation.Add, true))
                    .SetParticleEffectParameters(GravitySlam)
                    .SetImpactEffectParameters(ArcaneSword)
                    .Build())
            .AddCustomSubFeatures(new PowerOrSpellFinishedByMeSteelWhirlwind())
            .AddToDB();

        return spell;
    }

    // keep a tab of all allowed positions for filtering using ContextualFormation collection
    // ContextualFormation is only used by the game when spawning new locations, so it's safe in this context
    private sealed class PowerOrSpellFinishedByMeSteelWhirlwind : IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var positioningService = ServiceRepository.GetService<IGameLocationPositioningService>();
            var visibilityService =
                ServiceRepository.GetService<IGameLocationVisibilityService>() as GameLocationVisibilityManager;

            var actingCharacter = action.ActingCharacter;

            actingCharacter.contextualFormation = [];

            foreach (var boxInt in action.ActionParams.TargetCharacters
                         .Select(targetCharacter => new BoxInt(
                             targetCharacter.LocationPosition, new int3(-1, -1, -1), new int3(1, 1, 1))))
            {
                foreach (var position in boxInt.EnumerateAllPositionsWithin())
                {
                    if (!visibilityService.MyIsCellPerceivedByCharacter(position, actingCharacter) ||
                        !positioningService.CanPlaceCharacter(
                            actingCharacter, position, CellHelpers.PlacementMode.Station) ||
                        !positioningService.CanCharacterStayAtPosition_Floor(
                            actingCharacter, position, onlyCheckCellsWithRealGround: true))
                    {
                        continue;
                    }

                    actingCharacter.ContextualFormation.Add(position);
                }
            }

            yield break;
        }
    }

    private sealed class FilterTargetingPositionSteelWhirlwind : IFilterTargetingPosition
    {
        public IEnumerator ComputeValidPositions(CursorLocationSelectPosition cursorLocationSelectPosition)
        {
            var source = cursorLocationSelectPosition.ActionParams.ActingCharacter;

            cursorLocationSelectPosition.validPositionsCache.SetRange(source.ContextualFormation);

            yield break;
        }
    }

    #endregion

    #region Banishing Smite

    internal static SpellDefinition BuildBanishingSmite()
    {
        const string NAME = "BanishingSmite";

        var conditionBanishingSmiteEnemy = ConditionDefinitionBuilder
            .Create(ConditionBanished, $"Condition{NAME}Enemy")
            .SetSpecialDuration(DurationType.Minute, 1)
            .AddToDB();

        conditionBanishingSmiteEnemy.permanentlyRemovedIfExtraPlanar = true;

        var additionalDamageBanishingSmite = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{NAME}")
            .SetGuiPresentation(NAME, Category.Spell)
            .SetNotificationTag(NAME)
            .SetAttackModeOnly()
            .AddCustomSubFeatures(new PhysicalAttackFinishedByMeBanishingSmite(conditionBanishingSmiteEnemy))
            .SetDamageDice(DieType.D10, 5)
            .SetSpecificDamageType(DamageTypeForce)
            // doesn't follow the standard impact particle reference
            .SetImpactParticleReference(Banishment.EffectDescription.EffectParticleParameters.effectParticleReference)
            .AddToDB();

        var conditionBanishingSmite = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(NAME, Category.Spell, ConditionBrandingSmite)
            .SetPossessive()
            .SetFeatures(additionalDamageBanishingSmite)
            .SetSpecialInterruptions(ConditionInterruption.AttacksAndDamages)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(BrandingSmite, NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.ThunderousSmite, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolAbjuration)
            .SetSpellLevel(5)
            .SetCastingTime(ActivationTime.BonusAction)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(false)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    // .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionBanishingSmite))
                    .SetParticleEffectParameters(Banishment)
                    .Build())
            .AddToDB();

        return spell;
    }

    private sealed class PhysicalAttackFinishedByMeBanishingSmite(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionDefinition)
        : IPhysicalAttackFinishedByMe
    {
        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetDefender = defender.RulesetActor;

            if (rollOutcome is RollOutcome.Failure or RollOutcome.CriticalFailure ||
                rulesetAttacker is not { IsDeadOrDyingOrUnconscious: false } ||
                rulesetDefender is not { IsDeadOrDyingOrUnconscious: false } ||
                rulesetDefender.CurrentHitPoints > 50)
            {
                yield break;
            }

            rulesetDefender.InflictCondition(
                conditionDefinition.Name,
                DurationType.Minute,
                1,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                conditionDefinition.Name,
                0,
                0,
                0);
        }
    }

    #endregion

    #region Circle of Magical Negation

    internal static SpellDefinition BuildCircleOfMagicalNegation()
    {
        const string NAME = "CircleOfMagicalNegation";

        var savingThrowAffinityCircleOfMagicalNegation = FeatureDefinitionSavingThrowAffinityBuilder
            .Create($"SavingThrowAffinity{NAME}")
            .SetGuiPresentation(NAME, Category.Spell)
            .SetAffinities(CharacterSavingThrowAffinity.Advantage, false,
                AttributeDefinitions.Strength,
                AttributeDefinitions.Dexterity,
                AttributeDefinitions.Constitution,
                AttributeDefinitions.Intelligence,
                AttributeDefinitions.Wisdom,
                AttributeDefinitions.Charisma)
            .AddToDB();

        var conditionCircleOfMagicalNegation = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(NAME, Category.Spell, ConditionShielded)
            .SetPossessive()
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(savingThrowAffinityCircleOfMagicalNegation)
            .AddToDB();

        conditionCircleOfMagicalNegation.GuiPresentation.Description = Gui.NoLocalization;

        conditionCircleOfMagicalNegation.AddCustomSubFeatures(
            new MagicEffectBeforeHitConfirmedOnMeCircleOfMagicalNegation(conditionCircleOfMagicalNegation));

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.CircleOfMagicalNegation, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolAbjuration)
            .SetSpellLevel(5)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(false)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 10)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Sphere, 6)
                    .SetRecurrentEffect(
                        RecurrentEffect.OnActivation | RecurrentEffect.OnEnter | RecurrentEffect.OnTurnStart)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionCircleOfMagicalNegation))
                    .SetParticleEffectParameters(DivineWord)
                    .Build())
            .AddToDB();

        return spell;
    }

    private sealed class MagicEffectBeforeHitConfirmedOnMeCircleOfMagicalNegation(
        ConditionDefinition conditionCircleOfMagicalNegation)
        : IMagicEffectBeforeHitConfirmedOnMe, IRollSavingThrowFinished
    {
        private const string CircleOfMagicalNegationSavedTag = "CircleOfMagicalNegationSaved";

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
            if (!defender.UsedSpecialFeatures.Remove(CircleOfMagicalNegationSavedTag))
            {
                yield break;
            }

            actualEffectForms.RemoveAll(x =>
                x.HasSavingThrow
                && x.FormType == EffectForm.EffectFormType.Damage
                && x.SavingThrowAffinity == EffectSavingThrowType.HalfDamage);

            defender.RulesetCharacter.LogCharacterAffectedByCondition(conditionCircleOfMagicalNegation);
        }

        public void OnSavingThrowFinished(
            RulesetCharacter caster,
            RulesetCharacter defender,
            int saveBonus,
            string abilityScoreName,
            BaseDefinition sourceDefinition,
            List<TrendInfo> modifierTrends,
            List<TrendInfo> advantageTrends,
            int rollModifier,
            int saveDC,
            bool hasHitVisual,
            ref RollOutcome outcome,
            ref int outcomeDelta,
            List<EffectForm> effectForms)
        {
            if (outcome == RollOutcome.Success)
            {
                GameLocationCharacter.GetFromActor(defender).UsedSpecialFeatures
                    .TryAdd(CircleOfMagicalNegationSavedTag, 0);
            }
        }
    }

    #endregion

    #region Telekinesis

    private const string ConditionTelekinesisRestrainedName = "ConditionTelekinesisRestrained";

    private const int TelekinesisRange = 12;

    internal static SpellDefinition BuildTelekinesis()
    {
        const string Name = "Telekinesis";

        _ = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionRestrained, ConditionTelekinesisRestrainedName)
            .SetParentCondition(ConditionDefinitions.ConditionRestrained)
            .SetFeatures()
            .AddToDB();

        var sprite = Sprites.GetSprite(Name, Resources.Telekinesis, 128);

        var powerTelekinesis = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}")
            .SetGuiPresentation(Name, Category.Spell, sprite)
            .SetUsesFixed(ActivationTime.Action)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.All, RangeType.Distance, TelekinesisRange, TargetType.IndividualsUnique)
                    .ExcludeCaster()
                    .SetParticleEffectParameters(MistyStep)
                    .Build())
            .AddToDB();

        var conditionTelekinesis = ConditionDefinitionBuilder
            .Create($"Condition{Name}")
            .SetGuiPresentation(Category.Condition, ConditionRevealedByDetectGoodOrEvil)
            .SetPossessive()
            .SetFeatures(powerTelekinesis)
            .AddCustomSubFeatures(
                AddUsablePowersFromCondition.Marker,
                OnConditionAddedOrRemovedTelekinesis.Marker)
            .CopyParticleReferences(SpiderClimb)
            .AddToDB();

        var powerTelekinesisNoCost = FeatureDefinitionPowerBuilder
            .Create(powerTelekinesis, $"Power{Name}NoCost")
            .SetUsesFixed(ActivationTime.NoCost)
            .AddToDB();

        var conditionTelekinesisNoCost = ConditionDefinitionBuilder
            .Create($"Condition{Name}NoCost")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(powerTelekinesisNoCost)
            .AddCustomSubFeatures(
                AddUsablePowersFromCondition.Marker,
                OnConditionAddedOrRemovedTelekinesis.Marker)
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Spell, sprite)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(5)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Debuff)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 10)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(conditionTelekinesisNoCost),
                        EffectFormBuilder.ConditionForm(conditionTelekinesis))
                    .SetParticleEffectParameters(MindTwist)
                    .Build())
            .AddToDB();

        spell.EffectDescription.EffectParticleParameters.conditionStartParticleReference = new AssetReference();
        spell.EffectDescription.EffectParticleParameters.conditionParticleReference = new AssetReference();
        spell.EffectDescription.EffectParticleParameters.conditionEndParticleReference = new AssetReference();

        var customBehavior = new CustomBehaviorTelekinesis(conditionTelekinesisNoCost, spell);

        powerTelekinesis.AddCustomSubFeatures(customBehavior);
        powerTelekinesisNoCost.AddCustomSubFeatures(customBehavior, ValidatorsValidatePowerUse.InCombat);

        return spell;
    }

    private static void RemoveExistingRestrainedInstances(
        // ReSharper disable once SuggestBaseTypeForParameter
        RulesetCharacter rulesetCaster)
    {
        if (Gui.Battle == null)
        {
            return;
        }

        foreach (var rulesetContender in Gui.Battle.EnemyContenders
                     .Select(locationContender => locationContender.RulesetCharacter))
        {
            if (!rulesetContender.TryGetConditionOfCategoryAndType(AttributeDefinitions.TagEffect,
                    ConditionTelekinesisRestrainedName, out var activeCondition) ||
                activeCondition.SourceGuid != rulesetCaster.Guid)
            {
                continue;
            }

            rulesetContender.RemoveCondition(activeCondition);

            break;
        }
    }

    internal sealed class OnConditionAddedOrRemovedTelekinesis : IOnConditionAddedOrRemoved
    {
        internal static readonly OnConditionAddedOrRemovedTelekinesis Marker = new();

        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // empty
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            RemoveExistingRestrainedInstances(target);
        }
    }

    internal sealed class CustomBehaviorTelekinesis(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionTelekinesisNoCost,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        SpellDefinition spellTelekinesis)
        : IFilterTargetingPosition, IPowerOrSpellFinishedByMe, ISelectPositionAfterCharacter
    {
        public IEnumerator ComputeValidPositions(CursorLocationSelectPosition cursorLocationSelectPosition)
        {
            var positioningService = ServiceRepository.GetService<IGameLocationPositioningService>();
            var visibilityService =
                ServiceRepository.GetService<IGameLocationVisibilityService>() as GameLocationVisibilityManager;

            var actingCharacter = cursorLocationSelectPosition.ActionParams?.ActingCharacter;
            var targetCharacter = cursorLocationSelectPosition.ActionParams?.TargetCharacters[0];

            if (actingCharacter == null || targetCharacter == null)
            {
                yield break;
            }

            const int RANGE = TelekinesisRange / 2;
            var boxInt = new BoxInt(targetCharacter.LocationPosition, int3.zero, int3.zero);

            boxInt.Inflate(RANGE);

            foreach (var position in boxInt.EnumerateAllPositionsWithin())
            {
                if (
                    // must use vanilla distance calculation here
                    int3.Distance(targetCharacter.LocationPosition, position) > RANGE ||
                    // must use vanilla distance calculation here
                    int3.Distance(actingCharacter.LocationPosition, position) > TelekinesisRange ||
                    !positioningService.CanPlaceCharacter(targetCharacter, position,
                        CellHelpers.PlacementMode.Station) ||
                    !positioningService.CanCharacterStayAtPosition_Floor(targetCharacter, position,
                        onlyCheckCellsWithRealGround: true) ||
                    !visibilityService.MyIsCellPerceivedByCharacter(position, actingCharacter))
                {
                    continue;
                }

                cursorLocationSelectPosition.validPositionsCache.Add(position);

                if (cursorLocationSelectPosition.stopwatch.Elapsed.TotalMilliseconds > 0.5)
                {
                    yield return null;
                }
            }
        }

        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            if (action.Countered)
            {
                yield break;
            }

            var actingCharacter = action.ActingCharacter;
            var actingRulesetCharacter = actingCharacter.RulesetCharacter;

            if (actingRulesetCharacter.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionTelekinesisNoCost.Name, out var activeTelekinesis))
            {
                actingRulesetCharacter.RemoveCondition(activeTelekinesis);
            }

            var rulesetSpell =
                actingRulesetCharacter.SpellsCastByMe.FirstOrDefault(x => x.SpellDefinition == spellTelekinesis);

            if (rulesetSpell == null)
            {
                yield break;
            }

            RemoveExistingRestrainedInstances(actingRulesetCharacter);

            var targetCharacter = action.ActionParams.TargetCharacters[0];

            RollAbilityCheckAndTryMoveApplyRestrained(
                actingCharacter,
                targetCharacter,
                rulesetSpell,
                action);
        }

        public int PositionRange => TelekinesisRange;

        public bool EnforcePositionSelection(CursorLocationSelectPosition cursorLocationSelectPosition)
        {
            var targetCharacter = cursorLocationSelectPosition.ActionParams.TargetCharacters[0];

            return targetCharacter.Side == Side.Ally;
        }

        private static bool ResolveRolls(
            GameLocationCharacter actor,
            GameLocationCharacter opponent,
            string spellCastingAbility,
            ActionDefinitions.Id actionId)
        {
            var actionModifier1 = new ActionModifier();
            var actionModifier2 = new ActionModifier();

            var abilityCheckBonus1 = actor.RulesetCharacter.ComputeBaseAbilityCheckBonus(spellCastingAbility,
                actionModifier1.AbilityCheckModifierTrends, string.Empty);
            var abilityCheckBonus2 = opponent.RulesetCharacter.ComputeBaseAbilityCheckBonus(
                AttributeDefinitions.Strength,
                actionModifier2.AbilityCheckModifierTrends, string.Empty);

            var contextField1 = 0;

            if (!actor.RulesetCharacter.IsWearingHeavyArmor())
            {
                contextField1 |= 64;
            }

            actor.ComputeAbilityCheckActionModifier(spellCastingAbility, string.Empty, actionModifier1, contextField1);

            var contextField2 = 1;

            if (!opponent.RulesetCharacter.IsWearingHeavyArmor())
            {
                contextField2 |= 64;
            }

            opponent.ComputeAbilityCheckActionModifier(AttributeDefinitions.Strength, string.Empty, actionModifier2,
                contextField2);

            actor.RulesetCharacter.EnumerateFeaturesToBrowse<IActionPerformanceProvider>(
                actor.RulesetCharacter.FeaturesToBrowse, actor.RulesetCharacter.FeaturesOrigin);

            foreach (var key in actor.RulesetCharacter.FeaturesToBrowse)
            {
                foreach (var executionModifier in (key as IActionPerformanceProvider)!.ActionExecutionModifiers)
                {
                    if (executionModifier.actionId != actionId ||
                        !actor.RulesetCharacter.IsMatchingEquipementCondition(executionModifier.equipmentContext) ||
                        executionModifier.advantageType == AdvantageType.None)
                    {
                        continue;
                    }

                    var num = executionModifier.advantageType == AdvantageType.Advantage ? 1 : -1;
                    var featureOrigin = actor.RulesetCharacter.FeaturesOrigin[key];
                    actionModifier1.AbilityCheckAdvantageTrends.Add(new TrendInfo(num, featureOrigin.sourceType,
                        featureOrigin.sourceName, featureOrigin.source));
                }
            }

            actor.RulesetCharacter.ResolveContestCheck(
                abilityCheckBonus1,
                actionModifier1.AbilityCheckModifier,
                spellCastingAbility,
                string.Empty,
                actionModifier1.AbilityCheckAdvantageTrends,
                actionModifier1.AbilityCheckModifierTrends,
                abilityCheckBonus2,
                actionModifier2.AbilityCheckModifier,
                AttributeDefinitions.Strength,
                string.Empty,
                actionModifier2.AbilityCheckAdvantageTrends,
                actionModifier2.AbilityCheckModifierTrends,
                opponent.RulesetCharacter,
                out var outcome);

            return outcome is RollOutcome.Success or RollOutcome.CriticalSuccess;
        }

        private static void RollAbilityCheckAndTryMoveApplyRestrained(
            GameLocationCharacter actingCharacter,
            // ReSharper disable once SuggestBaseTypeForParameter
            GameLocationCharacter targetCharacter,
            RulesetEffectSpell rulesetSpell,
            CharacterAction action)
        {
            var actingRulesetCharacter = actingCharacter.RulesetCharacter;
            var targetRulesetCharacter = targetCharacter.RulesetCharacter;
            var isEnemy = actingCharacter.IsOppositeSide(targetCharacter.Side);

            if (isEnemy)
            {
                var spellCastingAbility = actingRulesetCharacter.SpellsCastByMe
                    .FirstOrDefault(x => x.SpellDefinition == rulesetSpell.SpellDefinition)?.SpellRepertoire?
                    // assume Intelligence if no repertoire (ritual spell only used on Force Knight)
                    .SpellCastingAbility ?? AttributeDefinitions.Intelligence;

                var result = ResolveRolls(actingCharacter, targetCharacter, spellCastingAbility, action.ActionId);

                if (!result)
                {
                    return;
                }
            }

            if (action.ActionParams.Positions.Count > 0)
            {
                var actionParams = new CharacterActionParams(targetCharacter, ActionDefinitions.Id.SpiritRallyTeleport)
                {
                    Positions = { action.ActionParams.Positions[0] }
                };

                ServiceRepository.GetService<IGameLocationActionService>()?
                    .ExecuteAction(actionParams, null, true);
            }

            if (!isEnemy)
            {
                return;
            }

            targetRulesetCharacter.InflictCondition(
                ConditionTelekinesisRestrainedName,
                DurationType.Round,
                1,
                TurnOccurenceType.EndOfSourceTurn,
                AttributeDefinitions.TagEffect,
                actingRulesetCharacter.guid,
                actingRulesetCharacter.CurrentFaction.Name,
                1,
                ConditionTelekinesisRestrainedName,
                0,
                0,
                0);
        }
    }

    #endregion
}
