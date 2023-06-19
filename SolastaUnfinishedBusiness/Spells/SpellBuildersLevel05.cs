using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
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
            .SetCustomSubFeatures(AddUsablePowersFromCondition.Marker)
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
            .SetSomaticComponent(false)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetEffectDescription(EffectDescriptionBuilder.Create(CustomActionIdContext.FarStep)
                .SetDurationData(DurationType.Minute, 1)
                .AddEffectForms(EffectFormBuilder.Create()
                    .SetConditionForm(condition, ConditionForm.ConditionOperation.Add, true, true)
                    .Build())
                .Build())
            .AddToDB();
    }

    #endregion

    #region Mantle of Thorns

    internal static SpellDefinition BuildMantleOfThorns()
    {
        const string NAME = "MantleOfThorns";

        //Leaving this proxy in case someone already has spell in effect
        EffectProxyDefinitionBuilder
            .Create(EffectProxyDefinitions.ProxySpikeGrowth, $"EffectProxy{NAME}")
            .SetCanMove()
            .SetCanMoveOnCharacters()
            .AddToDB();

        var effectDescription = EffectDescriptionBuilder.Create()
            .SetParticleEffectParameters(SpikeGrowth)
            .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 3)
            .SetDurationData(DurationType.Minute, 1)
            .SetRecurrentEffect(RecurrentEffect.OnEnter | RecurrentEffect.OnMove | RecurrentEffect.OnTurnStart)
            .AddEffectForms(
                EffectFormBuilder.DamageForm(DamageTypePiercing, 2, DieType.D8),
                EffectFormBuilder.TopologyForm(TopologyForm.Type.DangerousZone, false),
                EffectFormBuilder.TopologyForm(TopologyForm.Type.DifficultThrough, false)
            )
            .Build();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell,
                Sprites.GetSprite("MantleOfThorns", Resources.MantleOfThorns, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(5)
            .SetCastingTime(ActivationTime.Action)
            .SetVerboseComponent(true)
            .SetRequiresConcentration(true)
            .SetEffectDescription(effectDescription)
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
            .SetTargetingData(Side.All, RangeType.Distance, 12, TargetType.Sphere, 3)
            .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
            .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, 2, additionalDicePerIncrement: 1)
            .SetParticleEffectParameters(Disintegrate)
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

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.SonicBoom, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(5)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetCastingTime(ActivationTime.Action)
            .SetEffectDescription(effectDescription)
            .SetCustomSubFeatures(PushesOrDragFromEffectPoint.Marker)
            .AddToDB();

        return spell;
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

        var additionalDamageBanishingSmite = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{NAME}")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag(NAME)
            .SetCustomSubFeatures(ValidatorsRestrictedContext.IsWeaponAttack)
            .SetDamageDice(DieType.D10, 5)
            .SetSpecificDamageType(DamageTypeForce)
            .SetCustomSubFeatures(new OnAttackHitEffectBanishingSmite(conditionBanishingSmiteEnemy))
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
            .SetVerboseComponent(true)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Minute, 1)
                .SetEffectForms(EffectFormBuilder.Create()
                    .SetConditionForm(conditionBanishingSmite, ConditionForm.ConditionOperation.Add)
                    .Build())
                .Build())
            .AddToDB();

        return spell;
    }

    private sealed class OnAttackHitEffectBanishingSmite : IAttackEffectAfterDamage
    {
        private readonly ConditionDefinition _conditionDefinition;

        public OnAttackHitEffectBanishingSmite(ConditionDefinition conditionDefinition)
        {
            _conditionDefinition = conditionDefinition;
        }

        public void OnAttackEffectAfterDamage(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetDefender = defender.RulesetCharacter;

            if (outcome is RollOutcome.Failure or RollOutcome.CriticalFailure ||
                rulesetAttacker == null ||
                rulesetDefender == null ||
                rulesetDefender.IsDeadOrDying ||
                rulesetDefender.CurrentHitPoints > 50)
            {
                return;
            }

            //TODO: ideally we need to banish extra planar creatures forever (kill them?)
            rulesetDefender.InflictCondition(
                _conditionDefinition.Name,
                DurationType.Minute,
                1,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagCombat,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                null,
                0,
                0,
                0);
        }
    }

    #endregion
}
