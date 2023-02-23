using System.Linq;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;
using static SolastaUnfinishedBusiness.Models.SpellsContext;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class DomainDefiler : AbstractSubclass
{
    internal DomainDefiler()
    {
        const string NAME = "DomainDefiler";

        //
        // Level 1
        //

        var autoPreparedSpellsDomainDefiler = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{NAME}")
            .SetGuiPresentation("DomainSpells", Category.Feature)
            .SetPreparedSpellGroups(
                BuildSpellGroup(1, FalseLife, InflictWounds), //Ray of Sickness maybe later
                BuildSpellGroup(3, Blindness, RayOfEnfeeblement),
                BuildSpellGroup(5, BestowCurse, Fear),
                BuildSpellGroup(7, Blight, PhantasmalKiller),
                BuildSpellGroup(9, CloudKill, Contagion)) //Anti life shell maybe later
            .SetSpellcastingClass(CharacterClassDefinitions.Paladin)
            .AddToDB();

        var bonusCantripDomainDefiler = FeatureDefinitionBonusCantripsBuilder
            .Create($"BonusCantrip{NAME}")
            .SetGuiPresentation(Category.Feature)
            .SetBonusCantrips(Wrack, RayOfFrost) //Added custom cantrip
            .AddToDB();

        var conditionInsidiousDeathMagic = ConditionDefinitionBuilder
            .Create($"Condition{NAME}InsidiousDeathMagic")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionFrightenedFear)
            .SetConditionType(ConditionType.Detrimental)
            .SetFeatures(FeatureDefinitionHealingModifiers.HealingModifierChilledByTouch)
            .SetSpecialDuration(DurationType.Round, 5)
            .AddToDB();

        var effectInsidiousDeathMagic = EffectFormBuilder
            .Create()
            .SetConditionForm(conditionInsidiousDeathMagic, ConditionForm.ConditionOperation.Add)
            .Build();

        var featureInsidiousDeathMagic = FeatureDefinitionBuilder
            .Create($"Feature{NAME}InsidiousDeathMagic")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(
                new DeathMagicModifyMagic(effectInsidiousDeathMagic),
                new OnAttackEffectInsidiousMagic(conditionInsidiousDeathMagic))
            .AddToDB();

        //
        // Level 2 (5, 11, 17)
        //

        var powerDefileLife2 = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}DefileLife2")
            .SetGuiPresentation($"Power{NAME}DefileLife", Category.Feature,
                Sprites.GetSprite("PowerDefileLife", Resources.PowerDefileLife, 128, 64))
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ChannelDivinity)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetParticleEffectParameters(PowerWightLord_CircleOfDeath)
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 3)
                    .AddImmuneCreatureFamilies(CharacterFamilyDefinitions.Undead)
                    .SetSavingThrowData(
                        false,
                        AttributeDefinitions.Constitution,
                        true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypeNecrotic, 1, DieType.D6)
                            .Build())
                    .Build())
            .AddToDB();

        var powerDefileLife5 = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}DefileLife5")
            .SetGuiPresentation($"Power{NAME}DefileLife", Category.Feature,
                Sprites.GetSprite("PowerDefileLife", Resources.PowerDefileLife, 128, 64))
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ChannelDivinity)
            .SetOverriddenPower(powerDefileLife2)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetParticleEffectParameters(PowerWightLord_CircleOfDeath)
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 3)
                    .AddImmuneCreatureFamilies(CharacterFamilyDefinitions.Undead)
                    .SetSavingThrowData(
                        false,
                        AttributeDefinitions.Constitution,
                        true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypeNecrotic, 3, DieType.D6)
                            .Build())
                    .Build())
            .AddToDB();

        var powerDefileLife11 = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}DefileLife11")
            .SetGuiPresentation($"Power{NAME}DefileLife", Category.Feature,
                Sprites.GetSprite("PowerDefileLife", Resources.PowerDefileLife, 128, 64))
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ChannelDivinity)
            .SetOverriddenPower(powerDefileLife5)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetParticleEffectParameters(PowerWightLord_CircleOfDeath)
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 3)
                    .AddImmuneCreatureFamilies(CharacterFamilyDefinitions.Undead)
                    .SetSavingThrowData(
                        false,
                        AttributeDefinitions.Constitution,
                        true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypeNecrotic, 5, DieType.D6)
                            .Build())
                    .Build())
            .AddToDB();

        var powerDefileLife17 = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}DefileLife17")
            .SetGuiPresentation($"Power{NAME}DefileLife", Category.Feature,
                Sprites.GetSprite("PowerDefileLife", Resources.PowerDefileLife, 128, 64))
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ChannelDivinity)
            .SetOverriddenPower(powerDefileLife11)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetParticleEffectParameters(PowerWightLord_CircleOfDeath)
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 3)
                    .AddImmuneCreatureFamilies(CharacterFamilyDefinitions.Undead)
                    .SetSavingThrowData(
                        false,
                        AttributeDefinitions.Constitution,
                        true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypeNecrotic, 5, DieType.D6)
                            .Build())
                    .Build())
            .AddToDB();

        //
        // Level 6 (14)
        //

        var additionalDamageDivineStrike6 = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{NAME}DivineStrike")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag("DivineStrike")
            .SetSpecificDamageType(DamageTypeNecrotic)
            .SetDamageDice(DieType.D8, 1)
            .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 0, 1, 14)
            .SetFrequencyLimit(FeatureLimitedUsage.OnceInMyTurn)
            .SetAttackOnly()
            .AddToDB();

        var damageAffinityDivineResistance = FeatureDefinitionDamageAffinityBuilder
            .Create($"DamageAffinity{NAME}DivineResistance")
            .SetGuiPresentation(Category.Feature)
            .SetDamageAffinityType(DamageAffinityType.Resistance)
            .SetDamageType(DamageTypeNecrotic)
            .AddToDB();

        var additionalDamageDivineStrike14 = FeatureDefinitionBuilder
            .Create($"AdditionalDamage{NAME}DivineStrike14")
            .SetGuiPresentation($"AdditionalDamage{NAME}DivineStrike", Category.Feature)
            .AddToDB();
        
        var damageAffinityDivineImmunity = FeatureDefinitionDamageAffinityBuilder
            .Create($"DamageAffinity{NAME}DivineImmunity")
            .SetGuiPresentation(Category.Feature)
            .SetDamageAffinityType(DamageAffinityType.Immunity)
            .SetDamageType(DamageTypeNecrotic)
            .AddToDB();

        //
        // Level 8
        //

        var conditionMarkForDeath = ConditionDefinitionBuilder
            .Create($"Condition{NAME}MarkForDeath")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionDead)
            .SetConditionType(ConditionType.Detrimental)
            .CopyParticleReferences(ConditionDefinitions.ConditionChilled)
            .AddFeatures(
                FeatureDefinitionDamageAffinityBuilder
                    .Create("DamageAffinityNecroticVulnerability")
                    .SetDamageAffinityType(DamageAffinityType.Vulnerability)
                    .SetDamageType(DamageTypeNecrotic)
                    .AddToDB())
            .AddToDB();

        var powerMarkForDeath = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}MarkForDeath")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerMarkForDeath", Resources.PowerMarkForDeath, 128, 64))
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.ChannelDivinity)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.Individuals)
                    .SetSavingThrowData(
                        false,
                        AttributeDefinitions.Charisma,
                        true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .CanSaveToCancel(TurnOccurenceType.EndOfTurn)
                            .SetConditionForm(
                                conditionMarkForDeath,
                                ConditionForm.ConditionOperation.Add,
                                false,
                                false)
                            .Build())
                    .Build())
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Subclass, SorcerousHauntedSoul)
            .AddFeaturesAtLevel(1,
                autoPreparedSpellsDomainDefiler,
                featureInsidiousDeathMagic,
                bonusCantripDomainDefiler)
            .AddFeaturesAtLevel(2,
                powerDefileLife2)
            .AddFeaturesAtLevel(5,
                powerDefileLife5)
            .AddFeaturesAtLevel(6,
                additionalDamageDivineStrike6,
                damageAffinityDivineResistance)
            .AddFeaturesAtLevel(8,
                powerMarkForDeath)
            .AddFeaturesAtLevel(10,
                PowerClericDivineInterventionPaladin)
            .AddFeaturesAtLevel(11,
                powerDefileLife11)
            .AddFeaturesAtLevel(14,
                additionalDamageDivineStrike14,
                damageAffinityDivineImmunity)
            .AddFeaturesAtLevel(17,
                powerDefileLife17)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }
    internal override FeatureDefinitionSubclassChoice SubclassChoice { get; }
    internal override DeityDefinition DeityDefinition => DeityDefinitions.Maraike;

    private sealed class DeathMagicModifyMagic : IModifyMagicEffect
    {
        private readonly EffectForm _effectInsidiousDeathMagic;

        internal DeathMagicModifyMagic(EffectForm effectInsidiousDeathMagic)
        {
            _effectInsidiousDeathMagic = effectInsidiousDeathMagic;
        }

        public EffectDescription ModifyEffect(
            BaseDefinition definition,
            EffectDescription effect,
            RulesetCharacter caster)
        {
            if (definition is not SpellDefinition spellDefinition ||
                !SpellListDefinitions.SpellListCleric.SpellsByLevel.Any(x =>
                    x.Spells.Contains(spellDefinition) && spellDefinition.EffectDescription.HasDamageForm()))
            {
                return effect;
            }

            var damage = effect.FindFirstDamageForm();

            if (damage.DamageType == DamageTypeNecrotic)
            {
                effect.effectForms.Add(_effectInsidiousDeathMagic);
            }

            return effect;
        }
    }

    private sealed class OnAttackEffectInsidiousMagic : IAfterAttackEffect
    {
        private readonly ConditionDefinition _conditionInsidiousDeathMagic;

        internal OnAttackEffectInsidiousMagic(ConditionDefinition conditionInsidiousDeathMagic)
        {
            _conditionInsidiousDeathMagic = conditionInsidiousDeathMagic;
        }

        public void AfterOnAttackHit(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            if (attackMode == null)
            {
                return;
            }

            if (outcome is RollOutcome.Failure or RollOutcome.CriticalFailure)
            {
                return;
            }

            foreach (var rulesetCondition in attackMode.EffectDescription.effectForms
                         .Where(x => x.DamageForm.DamageType == DamageTypeNecrotic)
                         .Select(_ => RulesetCondition.CreateActiveCondition(
                             defender.RulesetCharacter.Guid,
                             _conditionInsidiousDeathMagic,
                             DurationType.Round,
                             5,
                             TurnOccurenceType.StartOfTurn,
                             attacker.RulesetCharacter.Guid,
                             attacker.RulesetCharacter.CurrentFaction.Name)))
            {
                attacker.RulesetCharacter.AddConditionOfCategory(AttributeDefinitions.TagCombat, rulesetCondition);
            }
        }
    }
}
