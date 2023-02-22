using System.Linq;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
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
        // Level 2
        //

        var powerDefileLife = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}DefileLife")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerDefileLife", Resources.PowerDefileLife, 128, 64))
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ChannelDivinity)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetParticleEffectParameters(PowerWightLord_CircleOfDeath)
                    .SetTargetingData(Side.Enemy, RangeType.Self, 1, TargetType.Sphere, 3)
                    .AddImmuneCreatureFamilies(CharacterFamilyDefinitions.Undead)
                    .SetSavingThrowData(
                        false,
                        AttributeDefinitions.Constitution,
                        true,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                        AttributeDefinitions.Constitution)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypeNecrotic, 1, DieType.D6)
                            .SetDiceByRankTable(
                                EffectForm.LevelApplianceType.DiceNumberByLevelTable,
                                LevelSourceType.CharacterLevel,
                                1,
                                1,
                                2,
                                6,
                                5)
                            .Build())
                    .Build())
            .AddToDB();

        //
        // Level 6
        //

        var conditionMarkForDeath = ConditionDefinitionBuilder
            .Create($"Condition{NAME}MarkForDeath")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionDead)
            .CopyParticleReferences(ConditionDefinitions.ConditionChilled)
            .AddFeatures(
                DamageAffinityBludgeoningVulnerability,
                DamageAffinityPiercingVulnerability,
                DamageAffinitySlashingVulnerability)
            .AddToDB();

        var powerMarkForDeath = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}MarkForDeath")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerMarkForDeath", Resources.PowerMarkForDeath, 128, 64))
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ChannelDivinity)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.Individuals)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                conditionMarkForDeath,
                                ConditionForm.ConditionOperation.Add,
                                false,
                                false)
                            .Build())
                    .Build())
            .AddToDB();

        //
        // Level 8
        //

        var additionalDamageDivineStrikeDefiler = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{NAME}DivineStrike")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag("DivineStrike")
            .SetSpecificDamageType(DamageTypeNecrotic)
            .SetDamageDice(DieType.D8, 1)
            .SetFrequencyLimit(FeatureLimitedUsage.OnceInMyTurn)
            .SetAttackOnly()
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Subclass, SorcerousHauntedSoul)
            .AddFeaturesAtLevel(1, 
                autoPreparedSpellsDomainDefiler,
                featureInsidiousDeathMagic,
                bonusCantripDomainDefiler)
            .AddFeaturesAtLevel(2, powerDefileLife)
            .AddFeaturesAtLevel(6, powerMarkForDeath)
            .AddFeaturesAtLevel(8, additionalDamageDivineStrikeDefiler)
            .AddFeaturesAtLevel(10, PowerClericDivineInterventionPaladin)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }
    internal override FeatureDefinitionSubclassChoice SubclassChoice { get; }
    internal override DeityDefinition DeityDefinition { get; } = DeityDefinitions.Maraike;

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
                         .Select(x => RulesetCondition.CreateActiveCondition(
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
