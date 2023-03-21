using System.Linq;
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
            .SetGuiPresentation("ExpandedSpells", Category.Feature)
            .SetAutoTag("Domain")
            .SetPreparedSpellGroups(
                BuildSpellGroup(1, FalseLife, InflictWounds), //Ray of Sickness maybe later
                BuildSpellGroup(3, Blindness, RayOfEnfeeblement),
                BuildSpellGroup(5, BestowCurse, Fear),
                BuildSpellGroup(7, Blight, PhantasmalKiller),
                BuildSpellGroup(9, CloudKill, Contagion)) //Anti life shell maybe later
            .SetSpellcastingClass(CharacterClassDefinitions.Cleric)
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

        const string DEFILE_LIFE_DESCRIPTION = "Feature/&PowerDomainDefilerDefileLifeDescription";

        var powerDefileSprite = Sprites.GetSprite("PowerDefileLife", Resources.PowerDefileLife, 128, 64);

        static string PowerDefileDescription(int x)
        {
            return Gui.Format(DEFILE_LIFE_DESCRIPTION, x.ToString());
        }

        var powerDefileLife2 = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}DefileLife2")
            .SetGuiPresentation($"Power{NAME}DefileLife", Category.Feature, PowerDefileDescription(1),
                powerDefileSprite)
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

        var featureSetDefileLife2 = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{NAME}DefileLife2")
            .SetGuiPresentation(
                Gui.Localize("Feature/&ClericChannelDivinityTitle") + ": " +
                Gui.Localize($"Feature/&Power{NAME}DefileLifeTitle"),
                PowerDefileDescription(1),
                powerDefileSprite)
            .AddFeatureSet(powerDefileLife2)
            .AddToDB();

        var powerDefileLife5 = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}DefileLife5")
            .SetGuiPresentation($"Power{NAME}DefileLife", Category.Feature, PowerDefileDescription(3),
                powerDefileSprite)
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

        var featureSetDefileLife5 = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{NAME}DefileLife5")
            .SetGuiPresentation(
                Gui.Localize("Feature/&ClericChannelDivinityTitle") + ": " +
                Gui.Localize($"Feature/&Power{NAME}DefileLifeTitle"),
                PowerDefileDescription(3),
                powerDefileSprite)
            .AddFeatureSet(powerDefileLife5)
            .AddToDB();

        var powerDefileLife11 = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}DefileLife11")
            .SetGuiPresentation($"Power{NAME}DefileLife", Category.Feature, PowerDefileDescription(5),
                powerDefileSprite)
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

        var featureSetDefileLife11 = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{NAME}DefileLife11")
            .SetGuiPresentation(
                Gui.Localize("Feature/&ClericChannelDivinityTitle") + ": " +
                Gui.Localize($"Feature/&Power{NAME}DefileLifeTitle"),
                PowerDefileDescription(5),
                powerDefileSprite)
            .AddFeatureSet(powerDefileLife11)
            .AddToDB();

        var powerDefileLife17 = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}DefileLife17")
            .SetGuiPresentation($"Power{NAME}DefileLife", Category.Feature, PowerDefileDescription(7),
                powerDefileSprite)
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
                            .SetDamageForm(DamageTypeNecrotic, 7, DieType.D6)
                            .Build())
                    .Build())
            .AddToDB();

        var featureSetDefileLife17 = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{NAME}DefileLife17")
            .SetGuiPresentation(
                Gui.Localize("Feature/&ClericChannelDivinityTitle") + ": " +
                Gui.Localize($"Feature/&Power{NAME}DefileLifeTitle"),
                PowerDefileDescription(7),
                powerDefileSprite)
            .AddFeatureSet(powerDefileLife17)
            .AddToDB();

        //
        // Level 6 (14)
        //

        const string DIVINE_STRIKE_DESCRIPTION = "Feature/&AdditionalDamageDomainDefilerDivineStrikeDescription";

        static string PowerDivineStrikeDescription(int x)
        {
            return Gui.Format(DIVINE_STRIKE_DESCRIPTION, x.ToString());
        }

        var additionalDamageDivineStrike6 = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{NAME}DivineStrike6")
            .SetGuiPresentation($"AdditionalDamage{NAME}DivineStrike", Category.Feature,
                PowerDivineStrikeDescription(1))
            .SetNotificationTag("DivineStrike")
            .SetSpecificDamageType(DamageTypeNecrotic)
            .SetDamageDice(DieType.D8, 1)
            .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 1, 8, 6)
            .SetFrequencyLimit(FeatureLimitedUsage.OnceInMyTurn)
            .SetAttackModeOnly()
            .AddToDB();

        var damageAffinityDivineResistance = FeatureDefinitionDamageAffinityBuilder
            .Create($"DamageAffinity{NAME}DivineResistance")
            .SetGuiPresentation(Category.Feature)
            .SetDamageAffinityType(DamageAffinityType.Resistance)
            .SetDamageType(DamageTypeNecrotic)
            .AddToDB();

        var additionalDamageDivineStrike14 = FeatureDefinitionBuilder
            .Create($"AdditionalDamage{NAME}DivineStrike14")
            .SetGuiPresentation($"AdditionalDamage{NAME}DivineStrike", Category.Feature,
                PowerDivineStrikeDescription(2))
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
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        var featureSetMarkForDeath = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{NAME}MarkForDeath")
            .SetGuiPresentation(
                Gui.Localize("Feature/&ClericChannelDivinityTitle") + ": " +
                Gui.Localize($"Feature/&Power{NAME}MarkForDeathTitle"),
                Gui.Localize($"Feature/&Power{NAME}MarkForDeathDescription"),
                powerDefileSprite)
            .AddFeatureSet(powerMarkForDeath)
            .AddToDB();

        /*
        Level 17 - Death's Usher
        Your banal distaste for life seems to help usher the dying to their end. Starting at 17th level, creatures (other than yourself) within 30 feet of you, make their death saving throws at disadvantage, and they begin their death saves with 1 marked failure.
        */

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite("DomainDefiler", Resources.DomainDefiler, 256))
            .AddFeaturesAtLevel(1,
                autoPreparedSpellsDomainDefiler,
                featureInsidiousDeathMagic,
                bonusCantripDomainDefiler)
            .AddFeaturesAtLevel(2,
                featureSetDefileLife2)
            .AddFeaturesAtLevel(5,
                featureSetDefileLife5)
            .AddFeaturesAtLevel(6,
                additionalDamageDivineStrike6,
                damageAffinityDivineResistance)
            .AddFeaturesAtLevel(8,
                featureSetMarkForDeath)
            .AddFeaturesAtLevel(10,
                PowerClericDivineInterventionPaladin)
            .AddFeaturesAtLevel(11,
                featureSetDefileLife11)
            .AddFeaturesAtLevel(14,
                additionalDamageDivineStrike14,
                damageAffinityDivineImmunity)
            .AddFeaturesAtLevel(17,
                featureSetDefileLife17)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    // ReSharper disable once UnassignedGetOnlyAutoProperty
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
            var damage = effect.FindFirstDamageForm();

            if (damage is not { DamageType: DamageTypeNecrotic })
            {
                return effect;
            }

            effect.effectForms.Add(_effectInsidiousDeathMagic);

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

            var hero = attacker.RulesetCharacter as RulesetCharacterHero ??
                       attacker.RulesetCharacter.OriginalFormCharacter as RulesetCharacterHero;

            if (hero == null)
            {
                return;
            }

            var levels = hero.GetClassLevel(CharacterClassDefinitions.Cleric);

            foreach (var rulesetCondition in attackMode.EffectDescription.effectForms
                         .Where(x => x.DamageForm.DamageType == DamageTypeNecrotic)
                         .Select(_ => RulesetCondition.CreateActiveCondition(
                             defender.RulesetCharacter.Guid,
                             _conditionInsidiousDeathMagic,
                             DurationType.Round,
                             levels,
                             TurnOccurenceType.StartOfTurn,
                             attacker.RulesetCharacter.Guid,
                             attacker.RulesetCharacter.CurrentFaction.Name)))
            {
                attacker.RulesetCharacter.AddConditionOfCategory(AttributeDefinitions.TagCombat, rulesetCondition);
            }
        }
    }
}
