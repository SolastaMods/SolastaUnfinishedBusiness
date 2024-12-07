using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ItemDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.MonsterDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Subclasses.CommonBuilders;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class WizardDeadMaster : AbstractSubclass
{
    private const string Name = "WizardDeadMaster";
    private const string CreateDeadTag = "DeadMasterMinion";

    private static readonly FeatureDefinitionFeatureSet FeatureSetDeadMasterNecromancyBonusDc =
        FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetDeadMasterNecromancyBonusDC")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

    internal static readonly List<SpellDefinition> DeadMasterSpells = [];

    public WizardDeadMaster()
    {
        // LEVEL 02

        var autoPreparedSpellsDeadMaster = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsDeadMaster")
            .SetGuiPresentation(Category.Feature)
            .SetSpellcastingClass(CharacterClassDefinitions.Wizard)
            .SetAutoTag("College")
            .SetPreparedSpellGroups(GetDeadSpellAutoPreparedGroups())
            .AddToDB();

        var bypassSpellConcentrationDeadMaster = FeatureDefinitionBuilder
            .Create("BypassSpellConcentrationDeadMaster")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new ModifyConcentrationRequirementDeadMaster())
            .AddToDB();

        // LEVEL 06

        // Stark Harvest

        var targetReducedToZeroHpDeadMasterStarkHarvest = FeatureDefinitionBuilder
            .Create("TargetReducedToZeroHpDeadMasterStarkHarvest")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        targetReducedToZeroHpDeadMasterStarkHarvest.AddCustomSubFeatures(
            new OnReducedToZeroHpByMeStarkHarvest(targetReducedToZeroHpDeadMasterStarkHarvest));

        const string ChainsName = "SummoningAffinityDeadMasterUndeadChains";

        var hpBonus = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierDeadMasterUndeadChains")
            .SetGuiPresentationNoContent(true)
            .SetAddConditionAmount(AttributeDefinitions.HitPoints)
            .AddToDB();

        var attackBonus = FeatureDefinitionAttackModifierBuilder
            .Create("AttackModifierDeadMasterUndeadChains")
            .SetGuiPresentation(ChainsName, Category.Feature, Gui.NoLocalization)
            .SetAttackRollModifier(method: AttackModifierMethod.SourceConditionAmount)
            .AddToDB();

        var deadMasterUndeadChains = FeatureDefinitionSummoningAffinityBuilder
            .Create(ChainsName)
            .SetGuiPresentation(Category.Feature)
            .SetRequiredMonsterTag(CreateDeadTag)
            .SetAddedConditions(
                ConditionDefinitionBuilder
                    .Create("ConditionDeadMasterUndeadChainsProficiency")
                    .SetGuiPresentation(ChainsName, Category.Feature)
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetPossessive()
                    .SetAmountOrigin(ExtraOriginOfAmount.SourceProficiencyBonus)
                    .SetFeatures(attackBonus)
                    .AddToDB(),
                ConditionDefinitionBuilder
                    .Create("ConditionDeadMasterUndeadChainsLevel")
                    .SetGuiPresentation(ChainsName, Category.Feature)
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetPossessive()
                    .SetAmountOrigin(ExtraOriginOfAmount.SourceClassLevel, WizardClass)
                    .SetFeatures(hpBonus, hpBonus)
                    .AddToDB())
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.WizardDeadMaster, 256))
            .AddFeaturesAtLevel(2,
                bypassSpellConcentrationDeadMaster,
                FeatureSetDeadMasterNecromancyBonusDc,
                autoPreparedSpellsDeadMaster,
                deadMasterUndeadChains)
            .AddFeaturesAtLevel(6,
                targetReducedToZeroHpDeadMasterStarkHarvest)
            .AddFeaturesAtLevel(10,
                DamageAffinityGenericHardenToNecrotic)
            .AddFeaturesAtLevel(14,
                PowerCasterCommandUndead)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Wizard;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceWizardArcaneTraditions;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    internal static void LateLoad()
    {
        foreach (var spellDefinition in DatabaseRepository.GetDatabase<SpellDefinition>()
                     .Where(x => x.SchoolOfMagic == SchoolNecromancy))
        {
            FeatureSetDeadMasterNecromancyBonusDc.FeatureSet.Add(
                FeatureDefinitionMagicAffinityBuilder
                    .Create($"MagicAffinityDeadMaster{spellDefinition.Name}")
                    .SetGuiPresentationNoContent(true)
                    .SetSpellWithModifiedSaveDc(spellDefinition, 1)
                    .AddToDB());
        }
    }

    [NotNull]
    private static FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup[] GetDeadSpellAutoPreparedGroups()
    {
        var createDeadSpellMonsters =
            new Dictionary<(int clazz, int spell),
                List<(MonsterDefinition monster, int number, AssetReferenceSprite icon, BaseDefinition[] attackSprites
                    )>>
            {
                {
                    (3, 2), [
                        (Skeleton, 1, Sprites.GetSprite("SpellRaiseSkeleton", Resources.SpellRaiseSkeleton, 128),
                            [Scimitar]),

                        (Skeleton_Archer, 1,
                            Sprites.GetSprite("SpellRaiseSkeletonArcher", Resources.SpellRaiseSkeletonArcher, 128),
                            [Shortbow, Shortsword])
                    ]
                }, //CR 0.25 x2
                {
                    (5, 3), [
                        (Ghoul, 1, Sprites.GetSprite("SpellRaiseGhoul", Resources.SpellRaiseGhoul, 128),
                        [
                            MonsterAttackDefinitions.Attack_Wildshape_GiantEagle_Talons,
                            MonsterAttackDefinitions.Attack_Wildshape_Wolf_Bite
                        ])
                    ]
                }, //CR 1
                {
                    (7, 4), [
                        (Skeleton_Enforcer, 1,
                            Sprites.GetSprite("SpellRaiseSkeletonEnforcer", Resources.SpellRaiseSkeletonEnforcer, 128),
                            [Battleaxe, MonsterAttackDefinitions.Attack_Wildshape_Ape_Toss_Rock])
                    ]
                }, //CR 2
                {
                    (9, 5), [
                        (Skeleton_Knight, 1,
                            Sprites.GetSprite("SpellRaiseSkeletonKnight", Resources.SpellRaiseSkeletonKnight, 128),
                            [Longsword]),

                        (Skeleton_Marksman, 1,
                            Sprites.GetSprite("SpellRaiseSkeletonMarksman", Resources.SpellRaiseSkeletonMarksman, 128),
                            [Longbow, Shortsword])
                    ]
                }, //CR 3
                {
                    (11, 6), [
                        (Ghost, 1, Sprites.GetSprite("SpellRaiseGhost", Resources.SpellRaiseGhost, 128),
                            [Enchanted_Dagger_Souldrinker])
                    ]
                }, //CR 4
                {
                    (13, 7), [
                        (Wight, 1, Sprites.GetSprite("SpellRaiseWight", Resources.SpellRaiseWight, 128),
                            [LongswordPlus2, LongbowPlus1])
                    ]
                }, //CR 3 x2
                {
                    (15, 8), [
                        (WightLord, 1, Sprites.GetSprite("SpellRaiseWightLord", Resources.SpellRaiseWightLord, 128),
                            [Enchanted_Longsword_Frostburn, Enchanted_Shortbow_Medusa])
                    ]
                } //CR 6
            };

        var result = new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>();

        foreach (var kvp in createDeadSpellMonsters)
        {
            var (clazz, spell) = kvp.Key;
            var monsters = kvp.Value;
            var spells = new List<SpellDefinition>();

            foreach (var (monsterDefinition, count, icon, attackSprites) in monsters)
            {
                var monster = MakeSummonedMonster(monsterDefinition, attackSprites);
                var title = Gui.Format("Spell/&SpellRaiseDeadFormatTitle",
                    monster.FormatTitle());
                var description = Gui.Format("Spell/&SpellRaiseDeadFormatDescription",
                    monster.FormatTitle(),
                    monster.FormatDescription());

                // var duration = clazz switch
                // {
                //     >= 15 => 24 * 60,
                //     >= 13 => 8 * 60,
                //     >= 9 => 60,
                //     >= 5 => 10,
                //     _ => 1
                // };

                var createDeadSpell = SpellDefinitionBuilder
                    .Create($"CreateDead{monster.name}")
                    .SetGuiPresentation(title, description, icon)
                    .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolNecromancy)
                    .SetSpellLevel(spell)
                    .SetCastingTime(ActivationTime.Action)
                    .SetMaterialComponent(MaterialComponentType.Mundane)
                    .SetVerboseComponent(true)
                    .SetSomaticComponent(true)
                    .SetVocalSpellSameType(VocalSpellSemeType.Debuff)
                    .SetRequiresConcentration(true)
                    .SetEffectDescription(
                        EffectDescriptionBuilder
                            .Create()
                            .SetTargetingData(Side.All, RangeType.Distance, 6, TargetType.Position, count)
                            .SetDurationData(DurationType.Hour, 1)
                            .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, 2,
                                additionalSummonsPerIncrement: 1)
                            .SetParticleEffectParameters(VampiricTouch)
                            .SetEffectForms(
                                EffectFormBuilder
                                    .Create()
                                    .SetSummonCreatureForm(1, monster.Name)
                                    .Build())
                            .Build())
                    .AddToDB();

                spells.Add(createDeadSpell);
            }

            result.Add(new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup
            {
                ClassLevel = clazz, SpellsList = spells
            });
        }

        DeadMasterSpells.SetRange(result.SelectMany(x => x.SpellsList));
        FeatureDefinitionPowers.PowerWightLordRetaliate.rechargeRate = RechargeRate.ShortRest;
        FeatureDefinitionPowers.PowerWightLordRetaliate.activationTime = ActivationTime.BonusAction;

        return [.. result];
    }

    private static MonsterDefinition MakeSummonedMonster(
        MonsterDefinition monster,
        BaseDefinition[] attackSprites)
    {
        var modified = MonsterDefinitionBuilder
            .Create(monster, $"Risen{monster.Name}")
            .SetDefaultFaction(FactionDefinitions.Party)
            .SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.None)
            .SetDungeonMakerPresence(MonsterDefinition.DungeonMaker.None)
            .AddCreatureTags(CreateDeadTag)
            .SetFullyControlledWhenAllied(true)
            .SetDroppedLootDefinition(null)
            .AddToDB();

        if (attackSprites == null)
        {
            return modified;
        }

        for (var i = 0; i < attackSprites.Length; i++)
        {
            var attack = modified.AttackIterations.ElementAtOrDefault(i);

            if (attack != null)
            {
                attack.MonsterAttackDefinition.GuiPresentation.spriteReference =
                    attackSprites[i].GuiPresentation.SpriteReference;
            }
        }

        return modified;
    }

    private sealed class ModifyConcentrationRequirementDeadMaster : IModifyConcentrationRequirement
    {
        public bool RequiresConcentration(RulesetCharacter rulesetCharacter, RulesetEffectSpell rulesetEffectSpell)
        {
            var delta = rulesetEffectSpell.EffectLevel - rulesetEffectSpell.SpellDefinition.SpellLevel;

            return delta == 0 || !DeadMasterSpells.Contains(rulesetEffectSpell.SpellDefinition);
        }
    }

    private sealed class OnReducedToZeroHpByMeStarkHarvest(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinition feature) : IOnReducedToZeroHpByMe
    {
        public IEnumerator HandleReducedToZeroHpByMe(
            GameLocationCharacter attacker,
            GameLocationCharacter downedCreature,
            RulesetAttackMode attackMode,
            RulesetEffect activeEffect)
        {
            if (activeEffect is not RulesetEffectSpell spellEffect || spellEffect.SpellDefinition.SpellLevel == 0)
            {
                yield break;
            }

            if (downedCreature.RulesetCharacter is not { IsDeadOrDyingOrUnconscious: true })
            {
                yield break;
            }

            var rulesetDowned = downedCreature.RulesetCharacter;
            var characterFamily = rulesetDowned.CharacterFamily;

            if (characterFamily is "Construct" or "Undead")
            {
                yield break;
            }

            if (!attacker.OncePerTurnIsValid(feature.name))
            {
                yield break;
            }

            attacker.UsedSpecialFeatures.TryAdd(feature.Name, 1);

            var rulesetAttacker = attacker.RulesetCharacter;
            var spell = spellEffect.SpellDefinition;
            var isNecromancy = spell.SchoolOfMagic == SchoolNecromancy;
            var healingReceived = (isNecromancy ? 3 : 2) * spell.SpellLevel;

            rulesetAttacker.LogCharacterUsedFeature(feature, indent: true);

            if (rulesetAttacker.MissingHitPoints > 0)
            {
                rulesetAttacker.ReceiveHealing(healingReceived, true, rulesetAttacker.Guid);
            }
            else
            {
                rulesetAttacker.ReceiveTemporaryHitPoints(
                    healingReceived, DurationType.UntilAnyRest, 0, TurnOccurenceType.StartOfTurn, rulesetAttacker.Guid);
            }
        }
    }
}
