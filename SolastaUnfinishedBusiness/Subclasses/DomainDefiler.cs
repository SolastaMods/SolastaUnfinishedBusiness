using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;
using static SolastaUnfinishedBusiness.Models.SpellsContext;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class DomainDefiler : AbstractSubclass
{
    public DomainDefiler()
    {
        const string NAME = "DomainDefiler";

        var divinePowerPrefix = Gui.Localize("Feature/&ClericChannelDivinityTitle") + ": ";

        //
        // Level 1
        //

        var autoPreparedSpellsDomainDefiler = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{NAME}")
            .SetGuiPresentation("ExpandedSpells", Category.Feature)
            .SetAutoTag("Domain")
            .SetPreparedSpellGroups(
                BuildSpellGroup(1, FalseLife, InflictWounds),
                BuildSpellGroup(3, Blindness, RayOfEnfeeblement),
                BuildSpellGroup(5, BestowCurse, Fear),
                BuildSpellGroup(7, Blight, PhantasmalKiller),
                BuildSpellGroup(9, CloudKill, Contagion))
            .SetSpellcastingClass(CharacterClassDefinitions.Cleric)
            .AddToDB();

        var bonusCantripDomainDefiler = FeatureDefinitionBonusCantripsBuilder
            .Create($"BonusCantrip{NAME}")
            .SetGuiPresentation(Category.Feature)
            .SetBonusCantrips(Wrack, RayOfFrost)
            .AddToDB();

        var conditionInsidiousDeathMagic = ConditionDefinitionBuilder
            .Create($"Condition{NAME}InsidiousDeathMagic")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionFrightenedFear)
            .SetSpecialDuration(DurationType.Minute, 1)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .SetFeatures(FeatureDefinitionHealingModifiers.HealingModifierChilledByTouch)
            .AddToDB();

        var featureInsidiousDeathMagic = FeatureDefinitionBuilder
            .Create($"Feature{NAME}InsidiousDeathMagic")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new CustomBehaviorInsidiousDeathMagic(conditionInsidiousDeathMagic))
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
                            .SetDamageForm(DamageTypeNecrotic, 2, DieType.D6)
                            .Build())
                    .Build())
            .AddToDB();

        powerDefileLife.AddCustomSubFeatures(new ModifyEffectDescriptionDefileLife(powerDefileLife));

        var featureSetDefileLife = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{NAME}DefileLife")
            .SetGuiPresentation(
                divinePowerPrefix + powerDefileLife.FormatTitle(), powerDefileLife.FormatDescription())
            .AddFeatureSet(powerDefileLife)
            .AddToDB();

        //
        // LEVEL 6 - Mark for Death
        //

        var conditionMarkForDeath = ConditionDefinitionBuilder
            .Create($"Condition{NAME}MarkForDeath")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionDead)
            .SetConditionType(ConditionType.Detrimental)
            .CopyParticleReferences(ConditionDefinitions.ConditionChilled)
            .AddFeatures(
                FeatureDefinitionDamageAffinityBuilder
                    .Create("DamageAffinityNecroticVulnerability")
                    .SetGuiPresentationNoContent(true)
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
                    .SetParticleEffectParameters(Bane)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetSavingThrowData(
                        false,
                        AttributeDefinitions.Charisma,
                        true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.EndOfTurn, true)
                            .SetConditionForm(
                                conditionMarkForDeath,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        var featureSetMarkForDeath = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{NAME}MarkForDeath")
            .SetGuiPresentation(
                divinePowerPrefix + powerMarkForDeath.FormatTitle(), powerMarkForDeath.FormatDescription())
            .AddFeatureSet(powerMarkForDeath)
            .AddToDB();

        //
        // LEVEL 08
        //

        // Divine Strike

        var additionalDamageDivineStrike = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{NAME}DivineStrike")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag("DivineStrike")
            .SetDamageDice(DieType.D8, 1)
            .SetSpecificDamageType(DamageTypeNecrotic)
            .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 1, 8, 6)
            .SetFrequencyLimit(FeatureLimitedUsage.OnceInMyTurn)
            .SetAttackModeOnly()
            .SetConditionOperations(new ConditionOperationDescription
            {
                ConditionDefinition = conditionInsidiousDeathMagic,
                Operation = ConditionOperationDescription.ConditionOperation.Add
            })
            .AddToDB();

        // Divine Resistance

        var damageAffinityDivineResistance = FeatureDefinitionDamageAffinityBuilder
            .Create($"DamageAffinity{NAME}DivineResistance")
            .SetGuiPresentation(Category.Feature)
            .SetDamageAffinityType(DamageAffinityType.Resistance)
            .SetDamageType(DamageTypeNecrotic)
            .AddToDB();

        // LEVEL 14

        // Divine Immunity

        _ = FeatureDefinitionDamageAffinityBuilder
            .Create($"DamageAffinity{NAME}DivineImmunity")
            .SetGuiPresentation(Category.Feature)
            .SetDamageAffinityType(DamageAffinityType.Immunity)
            .SetDamageType(DamageTypeNecrotic)
            .AddToDB();

        // LEVEL 17

        // Dying Light

        var autoPreparedSpellsDyingLight = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{NAME}DyingLight")
            .SetGuiPresentation(Category.Feature)
            .SetAutoTag("Domain")
            .SetPreparedSpellGroups(
                BuildSpellGroup(17, CircleOfDeath, FingerOfDeath))
            .SetSpellcastingClass(CharacterClassDefinitions.Cleric)
            .AddCustomSubFeatures(new ModifyDamageResistanceDyingLight())
            .AddToDB();

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(NAME, Resources.DomainDefiler, 256))
            .AddFeaturesAtLevel(1,
                autoPreparedSpellsDomainDefiler,
                bonusCantripDomainDefiler,
                featureInsidiousDeathMagic)
            .AddFeaturesAtLevel(2,
                featureSetDefileLife)
            .AddFeaturesAtLevel(6,
                featureSetMarkForDeath)
            .AddFeaturesAtLevel(8,
                additionalDamageDivineStrike,
                damageAffinityDivineResistance)
            .AddFeaturesAtLevel(10,
                PowerClericDivineInterventionPaladin)
            .AddFeaturesAtLevel(17,
                autoPreparedSpellsDyingLight)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Cleric;

    internal override CharacterSubclassDefinition Subclass { get; }

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override FeatureDefinitionSubclassChoice SubclassChoice { get; }

    internal override DeityDefinition DeityDefinition => DeityDefinitions.Maraike;

    //
    // Insidious Death Magic
    //

    private sealed class CustomBehaviorInsidiousDeathMagic :
        IAttackBeforeHitConfirmedOnEnemy, IMagicalAttackBeforeHitConfirmedOnEnemy
    {
        private readonly ConditionDefinition _conditionInsidiousDeathMagic;

        internal CustomBehaviorInsidiousDeathMagic(ConditionDefinition conditionInsidiousDeathMagic)
        {
            _conditionInsidiousDeathMagic = conditionInsidiousDeathMagic;
        }

        public IEnumerator OnAttackBeforeHitConfirmedOnEnemy(
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
            if (rulesetEffect != null)
            {
                yield break;
            }

            yield return TryAddCondition(actualEffectForms, attacker, defender);
        }

        public IEnumerator OnMagicalAttackBeforeHitConfirmedOnEnemy(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier magicModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            yield return TryAddCondition(actualEffectForms, attacker, defender);
        }

        private IEnumerator TryAddCondition(
            IEnumerable<EffectForm> actualEffectForms,
            GameLocationCharacter attacker,
            GameLocationCharacter defender)
        {
            if (actualEffectForms.All(x =>
                    x.FormType == EffectForm.EffectFormType.Damage && x.DamageForm.DamageType != DamageTypeNecrotic))
            {
                yield break;
            }

            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetDefender is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;

            if (rulesetAttacker is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            var classLevel = rulesetAttacker.GetClassLevel(CharacterClassDefinitions.Cleric);
            var duration = (classLevel + 1) / 2;
            var rulesetCondition = RulesetCondition.CreateActiveCondition(
                defender.Guid,
                _conditionInsidiousDeathMagic,
                DurationType.Round,
                duration,
                TurnOccurenceType.StartOfTurn,
                attacker.Guid,
                attacker.RulesetCharacter.CurrentFaction.Name);

            rulesetDefender.AddConditionOfCategory(AttributeDefinitions.TagCombat, rulesetCondition);
        }
    }

    //
    // Defile Life
    //

    private sealed class ModifyEffectDescriptionDefileLife : IModifyEffectDescription
    {
        private readonly BaseDefinition _baseDefinition;

        internal ModifyEffectDescriptionDefileLife(BaseDefinition baseDefinition)
        {
            _baseDefinition = baseDefinition;
        }

        public bool IsValid(
            BaseDefinition definition,
            RulesetCharacter character,
            EffectDescription effectDescription)
        {
            return definition == _baseDefinition;
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

            var classLevel = character.GetClassLevel(CharacterClassDefinitions.Cleric);

            damageForm.bonusDamage = classLevel;

            return effectDescription;
        }
    }

    //
    // Dying Light
    //

    private sealed class ModifyDamageResistanceDyingLight : IModifyDamageAffinity
    {
        public void ModifyDamageAffinity(RulesetActor attacker, RulesetActor defender, List<FeatureDefinition> features)
        {
            features.RemoveAll(x =>
                x is IDamageAffinityProvider
                {
                    DamageAffinityType: DamageAffinityType.Resistance, DamageType: DamageTypeNecrotic
                });
        }
    }
}
