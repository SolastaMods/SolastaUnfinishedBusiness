using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.Models;
using SolastaCommunityExpansion.Properties;
using SolastaCommunityExpansion.Utils;
using UnityEngine.AddressableAssets;
using static SolastaCommunityExpansion.Api.DatabaseHelper;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaCommunityExpansion.Api.DatabaseHelper.SpellDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.MonsterDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.SchoolOfMagicDefinitions;

namespace SolastaCommunityExpansion.Subclasses.Wizard;

internal sealed class DeadMaster : AbstractSubclass
{
    private const string CreateDeadSpellPrefix = "CreateDead";
    private const string FeatureUndeadChainsAttackModifierPrefix = "FeatureUndeadChainsAttackModifier";

    private static readonly Guid SubclassNamespace = new("58ce31d8-6a37-4d6e-ffea-b9b60658a3ef");

    private static readonly Dictionary<int, List<MonsterDefinition>> CreateDeadSpellMonsters = new()
    {
        {3, new List<MonsterDefinition> {Zombie, Skeleton, Skeleton_Archer}},
        {5, new List<MonsterDefinition> {Ghoul, Skeleton_Enforcer}},
        {7, new List<MonsterDefinition> {Skeleton_Knight, Skeleton_Marksman, Skeleton_Sorcerer}}
    };

    internal DeadMaster()
    {
        var spriteReference =
            CustomIcons.CreateAssetReferenceSprite("CreateDead", Resources.CreateDead, 128, 128);

        // var spellCommandUndead = SpellDefinitionBuilder
        //     .Create(DominateBeast, "CommandUndead", SubclassNamespace)
        //     .SetGuiPresentation(Category.Spell, spriteReference)
        //     .SetSchoolOfMagic(SchoolNecromancy)
        //     .SetSpellLevel(7);

        var autoPreparedSpellsWizardDeadMaster = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsWizardDeadMaster", SubclassNamespace)
            .SetGuiPresentation(Category.Feature)
            .SetCastingClass(CharacterClassDefinitions.Wizard)
            .SetPreparedSpellGroups(GetDeadSpellAutoPreparedGroups(spriteReference))
            .AddToDB();

        var featureStarkHarvest = FeatureDefinitionOnCharacterKillBuilder
            .Create("FeatureStarkHarvest", SubclassNamespace)
            .SetGuiPresentation(Category.Feature)
            .SetOnCharacterKill(OnStarkHarvestKill)
            .AddToDB();

        var featureUndeadChains = FeatureDefinitionOnCharacterKillBuilder
            .Create("FeatureUndeadChains", SubclassNamespace)
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        for (var i = 2; i < 7; i++)
        {
            _ = FeatureDefinitionAttackModifierBuilder
                .Create($"{FeatureUndeadChainsAttackModifierPrefix}{i}", SubclassNamespace)
                .SetGuiPresentation("FeatureUndeadChains", Category.Feature)
                .Configure(RuleDefinitions.AttackModifierMethod.FlatValue, i)
                .AddToDB();
        }

        var featureHardenToNecrotic = FeatureDefinitionDamageAffinityBuilder
            .Create(DamageAffinityNecroticImmunity, "FeatureHardenToNecrotic", SubclassNamespace)
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var powerCommandUndead = FeatureDefinitionPowerBuilder
            .Create("PowerCommandUndead", SubclassNamespace)
            .SetGuiPresentation(Category.Power)
            .Configure(
                0,
                RuleDefinitions.UsesDetermination.ProficiencyBonus,
                AttributeDefinitions.Intelligence,
                RuleDefinitions.ActivationTime.Action,
                1,
                RuleDefinitions.RechargeRate.LongRest,
                false,
                false,
                AttributeDefinitions.Intelligence,
                DominateBeast.EffectDescription)
            .SetAbilityScoreDetermination(RuleDefinitions.AbilityScoreDetermination.Explicit)
            .AddToDB();

        var commandUndeadEffect = powerCommandUndead.EffectDescription;

        commandUndeadEffect.restrictedCreatureFamilies = new List<string> {CharacterFamilyDefinitions.Undead.Name};
        commandUndeadEffect.EffectAdvancement.effectIncrementMethod = RuleDefinitions.EffectIncrementMethod.None;
        commandUndeadEffect.savingThrowAbility = AttributeDefinitions.Charisma;
        commandUndeadEffect.savingThrowDifficultyAbility = AttributeDefinitions.Intelligence;
        commandUndeadEffect.difficultyClassComputation = RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency;
        commandUndeadEffect.fixedSavingThrowDifficultyClass = 8;

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("WizardDeadMaster", SubclassNamespace)
            .SetGuiPresentation(Category.Subclass, DomainMischief.GuiPresentation.SpriteReference)
            .AddFeatureAtLevel(autoPreparedSpellsWizardDeadMaster, 2)
            .AddFeatureAtLevel(featureStarkHarvest, 2)
            .AddFeatureAtLevel(featureUndeadChains, 6)
            .AddFeatureAtLevel(featureHardenToNecrotic, 10)
            .AddFeatureAtLevel(powerCommandUndead, 14)
            .AddToDB();

        EnableCommandAllUndead();
    }

    private static CharacterSubclassDefinition Subclass { get; set; }

    private static void EnableCommandAllUndead()
    {
        var monsterDefinitions = DatabaseRepository.GetDatabase<MonsterDefinition>();

        foreach (var monsterDefinition in monsterDefinitions
                     .Where(x => x.CharacterFamily == CharacterFamilyDefinitions.Undead.Name))
        {
            monsterDefinition.fullyControlledWhenAllied = true;
        }
    }

    internal static void OnMonsterCreated(RulesetCharacterMonster monster)
    {
        if (Global.CastedSpell == null || !Global.CastedSpell.Name.StartsWith(CreateDeadSpellPrefix))
        {
            return;
        }

        var caster = Global.ActivePlayerCharacter.RulesetCharacter as RulesetCharacterHero
                     ?? Global.ActivePlayerCharacter.RulesetCharacter.OriginalFormCharacter as
                         RulesetCharacterHero;

        if (caster == null
            || !caster.ClassesAndSubclasses.TryGetValue(CharacterClassDefinitions.Wizard, out var subclassDefinition)
            || subclassDefinition != Subclass)
        {
            return;
        }

        var casterLevel = caster.ClassesAndLevels[CharacterClassDefinitions.Wizard];

        if (casterLevel < 6)
        {
            return;
        }

        var dbFeatureDefinitionAttackModifier = DatabaseRepository.GetDatabase<FeatureDefinitionAttackModifier>();
        var proficiencyBonus = caster.GetAttribute(AttributeDefinitions.ProficiencyBonus).CurrentValue;

        monster.GetAttribute(AttributeDefinitions.HitPoints).BaseValue += casterLevel;

        if (dbFeatureDefinitionAttackModifier
            .TryGetElement($"{FeatureUndeadChainsAttackModifierPrefix}{proficiencyBonus}",
                out var featureDefinitionAttackModifier))
        {
            monster.ActiveFeatures.Add(featureDefinitionAttackModifier);
        }

        var gameLoreService = ServiceRepository.GetService<IGameLoreService>();

        gameLoreService.LearnMonsterKnowledge(monster.MonsterDefinition, KnowledgeLevelDefinitions.Mastered4);
    }

    private static void OnStarkHarvestKill(
        GameLocationCharacter character,
        bool dropLoot,
        bool removeBody,
        bool forceRemove,
        bool considerDead,
        bool becomesDying)
    {
        if (!considerDead || Global.CurrentAction is not CharacterActionCastSpell actionCastSpell)
        {
            return;
        }

        var characterFamily = character.RulesetCharacter.CharacterFamily;

        if (characterFamily == CharacterFamilyDefinitions.Construct.Name
            || characterFamily == CharacterFamilyDefinitions.Undead.Name)
        {
            return;
        }

        var attacker = actionCastSpell.ActingCharacter.RulesetCharacter as RulesetCharacterHero
                       ?? actionCastSpell.ActingCharacter.RulesetCharacter.OriginalFormCharacter as
                           RulesetCharacterHero;

        if (attacker == null)
        {
            return;
        }

        var spellLevel = actionCastSpell.ActiveSpell.SpellDefinition.SpellLevel;
        var isNecromancy = actionCastSpell.ActiveSpell.SpellDefinition.SchoolOfMagic == SchoolNecromancy.Name;
        var healingReceived = (isNecromancy ? 3 : 2) * spellLevel;

        attacker.ReceiveHealing(healingReceived, true, attacker.Guid);
    }

    [NotNull]
    private static IEnumerable<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>
        GetDeadSpellAutoPreparedGroups(AssetReferenceSprite spriteReference)
    {
        var result = new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>();

        foreach (var kvp in CreateDeadSpellMonsters)
        {
            var level = kvp.Key;
            var monsters = kvp.Value;
            var spells = new List<SpellDefinition>();

            foreach (var monster in monsters)
            {
                var subSpell = SpellDefinitionBuilder
                    .Create(ConjureFey, $"CreateDead{monster.name}", SubclassNamespace)
                    .SetGuiPresentation(monster.GuiPresentation.Title, monster.GuiPresentation.Description,
                        spriteReference)
                    .SetSchoolOfMagic(SchoolNecromancy)
                    .SetSpellLevel(level)
                    .SetRequiresConcentration(false)
                    .SetCastingTime(RuleDefinitions.ActivationTime.Minute1)
                    .SetSubSpells()
                    .AddToDB();

                monster.fullyControlledWhenAllied = true;
                subSpell.EffectDescription.rangeParameter = 1;
                subSpell.EffectDescription.EffectForms[0].SummonForm.monsterDefinitionName = monster.name;
                subSpell.EffectDescription.EffectAdvancement.effectIncrementMethod =
                    RuleDefinitions.EffectIncrementMethod.None;
                subSpell.EffectDescription.durationType = RuleDefinitions.DurationType.UntilAnyRest;
                // subSpell.EffectDescription.EffectAdvancement.additionalSummonsPerIncrement = 1;
                // subSpell.EffectDescription.EffectAdvancement.effectIncrementMethod =
                //     RuleDefinitions.EffectIncrementMethod.PerAdditionalSlotLevel;

                spells.Add(subSpell);
            }

            var spell = SpellDefinitionBuilder
                .Create(ConjureFey, $"CreateDead{level}", SubclassNamespace)
                .SetGuiPresentation(Category.Spell, spriteReference)
                .SetSchoolOfMagic(SchoolNecromancy)
                .SetSpellLevel(level)
                .SetRequiresConcentration(false)
                .SetCastingTime(RuleDefinitions.ActivationTime.Minute1)
                .SetSubSpells(spells)
                .AddToDB();

            spell.EffectDescription.EffectForms.Clear();
            spell.EffectDescription.rangeParameter = 1;
            spell.EffectDescription.EffectAdvancement.effectIncrementMethod =
                RuleDefinitions.EffectIncrementMethod.None;
            spell.EffectDescription.durationType = RuleDefinitions.DurationType.UntilAnyRest;
            // spell.EffectDescription.EffectAdvancement.additionalSummonsPerIncrement = 1;
            // spell.EffectDescription.EffectAdvancement.effectIncrementMethod =
            //     RuleDefinitions.EffectIncrementMethod.PerAdditionalSlotLevel;

            var autoPreparedSpellsGroup =
                new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup
                {
                    ClassLevel = level, SpellsList = new List<SpellDefinition> {spell}
                };

            result.Add(autoPreparedSpellsGroup);
        }

        return result;
    }

    internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
    {
        return FeatureDefinitionSubclassChoices.SubclassChoiceWizardArcaneTraditions;
    }

    internal override CharacterSubclassDefinition GetSubclass()
    {
        return Subclass;
    }
}
