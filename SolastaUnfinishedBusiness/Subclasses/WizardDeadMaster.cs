using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.MonsterDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterFamilyDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class WizardDeadMaster : AbstractSubclass
{
    private const string WizardDeadMasterName = "WizardDeadMaster";
    private const string CreateDeadTag = "DeadMasterMinion";

    internal WizardDeadMaster()
    {
        var autoPreparedSpellsDeadMaster = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsDeadMaster")
            .SetGuiPresentation(Category.Feature)
            .SetSpellcastingClass(CharacterClassDefinitions.Wizard)
            .SetPreparedSpellGroups(GetDeadSpellAutoPreparedGroups())
            .AddToDB();

        var targetReducedToZeroHpDeadMasterStarkHarvest = FeatureDefinitionBuilder
            .Create("TargetReducedToZeroHpDeadMasterStarkHarvest")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new TargetReducedToZeroHpDeadMasterStarkHarvest())
            .AddToDB();

        const string ChainsName = "SummoningAffinityDeadMasterUndeadChains";

        var hpBonus = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierDeadMasterUndeadChains")
            .SetGuiPresentation(ChainsName, Category.Feature)
            .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.AddConditionAmount,
                AttributeDefinitions.HitPoints)
            .AddToDB();
        var attackBonus = FeatureDefinitionAttackModifierBuilder
            .Create("AttackModifierDeadMasterUndeadChains")
            .SetGuiPresentation(ChainsName, Category.Feature)
            .SetAttackRollModifier(method: AttackModifierMethod.SourceConditionAmount)
            .AddToDB();

        var deadMasterUndeadChains = FeatureDefinitionSummoningAffinityBuilder
            .Create(ChainsName)
            .SetGuiPresentation(Category.Feature)
            .SetRequiredMonsterTag(CreateDeadTag)
            .SetAddedConditions(ConditionDefinitionBuilder
                    .Create("ConditionDeadMasterUndeadChainsProficiency")
                    .SetGuiPresentationNoContent(true)
                    .SetAmountOrigin(ExtraOriginOfAmount.SourceProficiencyBonus)
                    .SetFeatures(attackBonus)
                    .AddToDB(),
                ConditionDefinitionBuilder
                    .Create("ConditionDeadMasterUndeadChainsLevel")
                    .SetGuiPresentationNoContent(true)
                    .SetAmountOrigin(ExtraOriginOfAmount.SourceClassLevel, WizardClass)
                    .SetFeatures(hpBonus, hpBonus)
                    .AddToDB())
            .AddToDB();

        var damageAffinityDeadMasterHardenToNecrotic = FeatureDefinitionDamageAffinityBuilder
            .Create(DamageAffinityNecroticImmunity, "DamageAffinityDeadMasterHardenToNecrotic")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var powerDeadMasterCommandUndead = FeatureDefinitionPowerBuilder
            .Create("PowerDeadMasterCommandUndead")
            .SetGuiPresentation(Category.Feature)
            .SetUsesProficiencyBonus(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(DominateBeast.EffectDescription)
                    .SetEffectAdvancement(EffectIncrementMethod.None)
                    .SetRestrictedCreatureFamilies(Undead)
                    .SetSavingThrowData(
                        false,
                        AttributeDefinitions.Charisma,
                        false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                        AttributeDefinitions.Intelligence,
                        8,
                        true)
                    .Build())
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(WizardDeadMasterName)
            .SetGuiPresentation(Category.Subclass, DomainMischief)
            .AddFeaturesAtLevel(2,
                autoPreparedSpellsDeadMaster,
                deadMasterUndeadChains
            )
            .AddFeaturesAtLevel(6,
                targetReducedToZeroHpDeadMasterStarkHarvest)
            .AddFeaturesAtLevel(10,
                damageAffinityDeadMasterHardenToNecrotic)
            .AddFeaturesAtLevel(14,
                powerDeadMasterCommandUndead)
            .AddToDB();

        EnableCommandAllUndead();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceWizardArcaneTraditions;

    private static void EnableCommandAllUndead()
    {
        if (!Main.Settings.EnableCommandAllUndead)
        {
            return;
        }

        var monsterDefinitions = DatabaseRepository.GetDatabase<MonsterDefinition>();

        foreach (var monsterDefinition in monsterDefinitions
                     .Where(x => x.CharacterFamily == Undead.Name))
        {
            monsterDefinition.fullyControlledWhenAllied = true;
        }
    }

    [NotNull]
    private static FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup[] GetDeadSpellAutoPreparedGroups()
    {
        var createDeadSpellMonsters =
            new Dictionary<(int clazz, int spell),
                List<(MonsterDefinition monster, int number, AssetReferenceSprite icon)>>
            {
                {
                    (2, 1),
                    new()
                    {
                        (Skeleton, 2, Sprites.SpellRaiseSkeleton),
                        (Skeleton_Archer, 2, Sprites.SpellRaiseSkeletonArcher)
                    }
                }, //CR 0.25 x2
                { (3, 2), new() { (Ghoul, 1, Sprites.SpellRaiseGhoul) } }, //CR 1
                { (5, 3), new() { (Skeleton_Enforcer, 1, Sprites.SpellRaiseSkeletonEnforcer) } }, //CR 2
                {
                    (7, 4),
                    new()
                    {
                        (Skeleton_Knight, 1, Sprites.SpellRaiseSkeletonKnight),
                        (Skeleton_Marksman, 1, Sprites.SpellRaiseSkeletonMarksman)
                    }
                }, //CR 3
                { (9, 5), new() { (Ghost, 1, Sprites.SpellRaiseGhost) } }, //CR 4
                { (11, 6), new() { (Wight, 2, Sprites.SpellRaiseWight) } }, //CR 3 x2
                { (13, 7), new() { (WightLord, 1, Sprites.SpellRaiseWightLord) } } //CR 6
            };

        var result = new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>();

        foreach (var kvp in createDeadSpellMonsters)
        {
            var level = kvp.Key;
            var monsters = kvp.Value;
            var spells = new List<SpellDefinition>();

            for (var i = 0; i < monsters.Count; i++)
            {
                var monster = MakeSummonedMonster(monsters[i].monster);
                var count = monsters[i].number;
                var icon = monsters[i].icon;

                spells.Add(SpellDefinitionBuilder
                    .Create($"CreateDead{monster.name}")
                    .SetGuiPresentation(
                        Gui.Format("Spell/&SpellRaiseDeadFormatTitle", monster.FormatTitle()),
                        Gui.Format("Spell/&SpellRaiseDeadFormatDescription", monster.FormatTitle(),
                            monster.FormatDescription()),
                        icon)
                    .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolNecromancy)
                    .SetSpellLevel(level.spell)
                    .SetSomaticComponent(true)
                    .SetMaterialComponent(MaterialComponentType.Mundane)
                    .SetVerboseComponent(true)
                    .SetVocalSpellSameType(VocalSpellSemeType.Debuff)
                    .SetUniqueInstance()
                    .SetRequiresConcentration(false)
                    .SetCastingTime(ActivationTime.Action)
                    .SetEffectDescription(EffectDescriptionBuilder.Create()
                        .SetTargetingData(Side.All, RangeType.Distance, 4, TargetType.Position, count)
                        .SetDurationData(DurationType.UntilLongRest)
                        .SetParticleEffectParameters(VampiricTouch)
                        .SetEffectForms(EffectFormBuilder.Create()
                            .SetSummonCreatureForm(1, monster.Name)
                            .Build())
                        .Build())
                    .AddToDB());
            }

            result.Add(new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup
            {
                ClassLevel = level.clazz, SpellsList = spells
            });
        }

        return result.ToArray();
    }

    private static MonsterDefinition MakeSummonedMonster(MonsterDefinition monster)
    {
        return MonsterDefinitionBuilder
            .Create(monster, $"Risen{monster.Name}")
            .SetDefaultFaction(FactionDefinitions.Party)
            .SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.None)
            .SetDungeonMakerPresence(MonsterDefinition.DungeonMaker.None)
            .AddCreatureTags(CreateDeadTag)
            .SetFullyControlledWhenAllied(true)
            .SetDroppedLootDefinition(null)
            .AddToDB();
    }

    private sealed class TargetReducedToZeroHpDeadMasterStarkHarvest : ITargetReducedToZeroHp
    {
        public IEnumerator HandleCharacterReducedToZeroHp(
            GameLocationCharacter attacker,
            GameLocationCharacter downedCreature,
            RulesetAttackMode attackMode,
            RulesetEffect activeEffect)
        {
            if (Global.CurrentAction is not CharacterActionCastSpell actionCastSpell)
            {
                yield break;
            }

            var characterFamily = downedCreature.RulesetCharacter.CharacterFamily;

            if (characterFamily == Construct.Name || characterFamily == Undead.Name)
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;
            var spellLevel = actionCastSpell.ActiveSpell.SpellDefinition.SpellLevel;
            var isNecromancy = actionCastSpell.ActiveSpell.SpellDefinition.SchoolOfMagic == SchoolNecromancy;
            var healingReceived = (isNecromancy ? 3 : 2) * spellLevel;

            rulesetAttacker.ReceiveHealing(healingReceived, true, rulesetAttacker.Guid);
        }
    }
}
