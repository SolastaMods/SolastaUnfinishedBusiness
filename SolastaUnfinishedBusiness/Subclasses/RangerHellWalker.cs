using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class RangerHellWalker : AbstractSubclass
{
    private const string Name = "RangerHellWalker";

    internal RangerHellWalker()
    {
        // LEVEL 03

        // Hellwalker Magic

        var autoPreparedSpells = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{Name}")
            .SetGuiPresentation(Category.Feature)
            .SetAutoTag("Ranger")
            .SetSpellcastingClass(CharacterClassDefinitions.Ranger)
            .SetPreparedSpellGroups(
                BuildSpellGroup(2, HellishRebuke),
                BuildSpellGroup(5, Invisibility),
                BuildSpellGroup(9, BestowCurse),
                BuildSpellGroup(13, WallOfFire),
                BuildSpellGroup(17, SpellsContext.FarStep))
            .AddToDB();

        var powerFirebolt = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Firebolt")
            .SetGuiPresentation(SpellsContext.EnduringSting.GuiPresentation)
            .SetUsesFixed(ActivationTime.Action)
            .SetEffectDescription(SpellsContext.EnduringSting.EffectDescription)
            .SetCustomSubFeatures(new ModifyMagicEffectFireBolt())
            .AddToDB();

        var featureSetFirebolt = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}Firebolt")
            .SetGuiPresentationNoContent(true)
            .AddFeatureSet(powerFirebolt)
            .AddToDB();

        // Damning Strike

        var conditionDammingStrike = ConditionDefinitionBuilder
            .Create($"Condition{Name}DammingStrike")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionOnFire)
            .SetConditionType(ConditionType.Detrimental)
            .SetPossessive()
            .CopyParticleReferences(ConditionDefinitions.ConditionOnFire)
            .SetSpecialDuration(DurationType.Minute, 1, TurnOccurenceType.StartOfTurn)
            .SetRecurrentEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(DamageTypeFire, 1, DieType.D6)
                    .Build())
            .AddToDB();

        var additionalDamageDammingStrike = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}DammingStrike")
            .SetGuiPresentation(Category.Feature)
            .SetRequiredProperty(RestrictedContextRequiredProperty.Weapon)
            .SetAttackModeOnly()
            .SetSavingThrowData()
            .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
            .SetIgnoreCriticalDoubleDice(true)
            .SetConditionOperations(
                new ConditionOperationDescription
                {
                    canSaveToCancel = true,
                    hasSavingThrow = true,
                    saveOccurence = TurnOccurenceType.StartOfTurn,
                    conditionDefinition = conditionDammingStrike,
                    operation = ConditionOperationDescription.ConditionOperation.Add,
                    saveAffinity = EffectSavingThrowType.Negates
                })
            .AddToDB();

        // Cursed Tongue

        var proficiencyCursedTongue = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}CursedTongue")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Language, "Language_Abyssal", "Language_Infernal")
            .AddToDB();

        // LEVEL 07

        // Burning Constitution

        var featureSetBurningConstitution = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}BurningConstitution")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                FeatureDefinitionDamageAffinitys.DamageAffinityFireResistance,
                FeatureDefinitionDamageAffinitys.DamageAffinityNecroticResistance)
            .AddToDB();

        // LEVEL 11

        // Mark of the Dammed

        var conditionMarkOfTheDammed = ConditionDefinitionBuilder
            .Create($"Condition{Name}MarkOfTheDammed")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionPoisoned)
            .SetConditionType(ConditionType.Detrimental)
            .SetPossessive()
            .SetParentCondition(ConditionDefinitions.ConditionFrightened)
            .CopyParticleReferences(ConditionDefinitions.ConditionOnFire)
            .SetFeatures(ConditionDefinitions.ConditionFrightened.Features)
            .SetRecurrentEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(DamageTypeNecrotic, 1, DieType.D6)
                    .Build())
            .AddToDB();

        var powerMarkOfTheDammed = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}MarkOfTheDammed")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("MarkOfTheDammed", Resources.PowerMarkOfTheDammed, 256, 128))
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    // .SetSavingThrowData(false, AttributeDefinitions.Constitution, true,
                    //     EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            //.HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.StartOfTurn, true)
                            .SetConditionForm(conditionMarkOfTheDammed, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        conditionDammingStrike.SetCustomSubFeatures(
            new NotifyConditionRemovalDammingStrike(conditionMarkOfTheDammed));

        powerMarkOfTheDammed.SetCustomSubFeatures(
            new CustomBehaviorMarkOfTheDammed(powerMarkOfTheDammed, conditionMarkOfTheDammed));

        // LEVEL 15

        // Fiendish Spawn

        var fiendMonsters = new List<MonsterDefinition>
        {
            MonsterDefinitions.Hezrou_MonsterDefinition, MonsterDefinitions.Marilith_MonsterDefinition
        };

        var powerFiendishSpawnPool = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}FiendishSpawn")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerFiendishSpawn", Resources.PowerFiendishSpawn, 256, 128))
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .AddToDB();

        var powerFiendishSpawnList = fiendMonsters
            .Select(monsterDefinition => new
            {
                monsterDefinition, monsterName = monsterDefinition.Name.Replace("_MonsterDefinition", string.Empty)
            })
            .Select(t => new
            {
                t,
                newMonsterDefinition = MonsterDefinitionBuilder
                    .Create(t.monsterDefinition, $"Monster{t.monsterName}")
                    .SetOrUpdateGuiPresentation(Category.Monster)
                    .SetDefaultFaction(FactionDefinitions.Party)
                    .SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.None)
                    .SetDungeonMakerPresence(MonsterDefinition.DungeonMaker.None)
                    .SetFullyControlledWhenAllied(true)
                    .SetDroppedLootDefinition(null)
                    .AddToDB()
            })
            .Select(t => FeatureDefinitionPowerSharedPoolBuilder
                .Create($"Power{Name}FiendishSpawn{t.t.monsterName}")
                .SetGuiPresentation(t.newMonsterDefinition.GuiPresentation)
                .SetSharedPool(ActivationTime.Action, powerFiendishSpawnPool)
                .SetEffectDescription(EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.Position)
                    .SetEffectForms(EffectFormBuilder.Create()
                        .SetSummonCreatureForm(1, t.newMonsterDefinition.Name)
                        .Build())
                    .Build())
                .AddToDB())
            .Cast<FeatureDefinitionPower>()
            .ToList();

        PowerBundle.RegisterPowerBundle(powerFiendishSpawnPool, true, powerFiendishSpawnList);

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.RangerHellWalker, 256))
            .AddFeaturesAtLevel(3,
                autoPreparedSpells,
                featureSetFirebolt,
                additionalDamageDammingStrike,
                proficiencyCursedTongue)
            .AddFeaturesAtLevel(7,
                featureSetBurningConstitution)
            .AddFeaturesAtLevel(11,
                powerMarkOfTheDammed)
            .AddFeaturesAtLevel(15,
                powerFiendishSpawnPool)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceRangerArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    //
    // FireBolt
    //

    private sealed class ModifyMagicEffectFireBolt : IModifyMagicEffect
    {
        public EffectDescription ModifyEffect(
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

            var levels = character.GetClassLevel(CharacterClassDefinitions.Ranger);
            var diceNumber = levels switch
            {
                >= 17 => 4,
                >= 11 => 3,
                >= 5 => 2,
                _ => 1
            };

            damageForm.diceNumber = diceNumber;

            return effectDescription;
        }
    }
    //
    // DammingStrike
    //

    private sealed class NotifyConditionRemovalDammingStrike : INotifyConditionRemoval
    {
        private readonly ConditionDefinition _conditionDefinition;

        public NotifyConditionRemovalDammingStrike(ConditionDefinition conditionDefinition)
        {
            _conditionDefinition = conditionDefinition;
        }

        public void AfterConditionRemoved(RulesetActor removedFrom, RulesetCondition rulesetCondition)
        {
            var otherRulesetCondition =
                removedFrom.AllConditions.FirstOrDefault(x => x.ConditionDefinition == _conditionDefinition);

            if (otherRulesetCondition != null)
            {
                removedFrom.RemoveCondition(otherRulesetCondition);
            }
        }

        public void BeforeDyingWithCondition(RulesetActor rulesetActor, RulesetCondition rulesetCondition)
        {
            // Empty
        }
    }

    //
    // Mark of the Dammed
    //

    private sealed class CustomBehaviorMarkOfTheDammed :
        IModifyDamageAffinity, IUsePowerFinishedByMe, IFilterTargetingMagicEffect
    {
        private readonly ConditionDefinition _conditionDefinition;
        private readonly FeatureDefinitionPower _featureDefinitionPower;

        public CustomBehaviorMarkOfTheDammed(
            FeatureDefinitionPower featureDefinitionPower,
            ConditionDefinition conditionDefinition)
        {
            _featureDefinitionPower = featureDefinitionPower;
            _conditionDefinition = conditionDefinition;
        }

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            if (__instance.actionParams.RulesetEffect is not RulesetEffectPower rulesetEffectPower ||
                rulesetEffectPower.PowerDefinition != _featureDefinitionPower)
            {
                return true;
            }

            if (target.RulesetCharacter == null)
            {
                return true;
            }

            var isValid = target.RulesetCharacter.HasConditionOfType("ConditionRangerHellWalkerDammingStrike");

            if (!isValid)
            {
                __instance.actionModifier.FailureFlags.Add("Tooltip/&MustHaveDammingStrikeCondition");
            }

            return isValid;
        }

        public void ModifyDamageAffinity(RulesetActor defender, RulesetActor attacker, List<FeatureDefinition> features)
        {
            if (!attacker.HasConditionOfType(_conditionDefinition.Name))
            {
                return;
            }

            features.RemoveAll(x =>
                x is IDamageAffinityProvider
                {
                    DamageAffinityType: DamageAffinityType.Immunity, DamageType: DamageTypeFire
                });
        }

        public IEnumerator OnUsePowerFinishedByMe(CharacterActionUsePower action, FeatureDefinitionPower power)
        {
            var battle = Gui.Battle;

            if (battle == null || power != _featureDefinitionPower)
            {
                yield break;
            }

            var gameLocationDefender = action.actionParams.targetCharacters[0];

            // remove this condition from all other enemies
            foreach (var gameLocationCharacter in battle.EnemyContenders
                         .Where(x => x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false })
                         .Where(x => x != gameLocationDefender)
                         .ToList()) // avoid changing enumerator
            {
                var rulesetDefender = gameLocationCharacter.RulesetCharacter;
                var rulesetCondition = rulesetDefender.AllConditions
                    .FirstOrDefault(x => x.ConditionDefinition == _conditionDefinition);

                if (rulesetCondition != null)
                {
                    rulesetDefender.RemoveCondition(rulesetCondition);
                }
            }
        }
    }
}
