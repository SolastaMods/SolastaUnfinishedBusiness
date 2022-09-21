using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Utils;
using UnityEngine.AddressableAssets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.MonsterDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SchoolOfMagicDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class WizardDeadMaster : AbstractSubclass
{
    private const string CreateDeadSpellPrefix = "CreateDead";
    private const string AttackModifierDeadMasterUndeadChainsPrefix = "AttackModifierDeadMasterUndeadChains";

    private static readonly Dictionary<int, List<MonsterDefinition>> CreateDeadSpellMonsters = new()
    {
        { 3, new List<MonsterDefinition> { Zombie, Skeleton, Skeleton_Archer } },
        { 5, new List<MonsterDefinition> { Ghoul, Skeleton_Enforcer } },
        { 7, new List<MonsterDefinition> { Skeleton_Knight, Skeleton_Marksman, Skeleton_Sorcerer } }
    };

    internal WizardDeadMaster()
    {
        var spriteReference =
            CustomIcons.CreateAssetReferenceSprite("CreateDead", Resources.CreateDead, 128, 128);

        // var spellCommandUndead = SpellDefinitionBuilder
        //     .Create(DominateBeast, "CommandUndead", SubclassNamespace)
        //     .SetGuiPresentation(Category.Spell, spriteReference)
        //     .SetSchoolOfMagic(SchoolNecromancy)
        //     .SetSpellLevel(7);

        var autoPreparedSpellsDeadMaster = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsDeadMaster")
            .SetGuiPresentation(Category.Feature)
            .SetCastingClass(CharacterClassDefinitions.Wizard)
            .SetPreparedSpellGroups(GetDeadSpellAutoPreparedGroups(spriteReference))
            .AddToDB();

        var featureStarkHarvest = FeatureDefinitionOnCharacterKillBuilder
            .Create("OnCharacterKillDeadMasterStarkHarvest")
            .SetGuiPresentation(Category.Feature)
            .SetOnCharacterKill(OnStarkHarvestKill)
            .AddToDB();

        var featureUndeadChains = FeatureDefinitionOnCharacterKillBuilder
            .Create("OnCharacterKillDeadMasterUndeadChains")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        for (var i = 2; i < 7; i++)
        {
            _ = FeatureDefinitionAttackModifierBuilder
                .Create($"{AttackModifierDeadMasterUndeadChainsPrefix}{i}")
                .SetGuiPresentation("OnCharacterKillDeadMasterUndeadChains", Category.Feature)
                .Configure(RuleDefinitions.AttackModifierMethod.FlatValue, i)
                .AddToDB();
        }

        var featureHardenToNecrotic = FeatureDefinitionDamageAffinityBuilder
            .Create(DamageAffinityNecroticImmunity, "DamageAffinityDeadMasterHardenToNecrotic")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var powerDeadMasterCommandUndead = FeatureDefinitionPowerBuilder
            .Create("PowerDeadMasterCommandUndead")
            .SetGuiPresentation(Category.Feature)
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

        var commandUndeadEffect = powerDeadMasterCommandUndead.EffectDescription;

        commandUndeadEffect.restrictedCreatureFamilies = new List<string> { CharacterFamilyDefinitions.Undead.Name };
        commandUndeadEffect.EffectAdvancement.effectIncrementMethod = RuleDefinitions.EffectIncrementMethod.None;
        commandUndeadEffect.savingThrowAbility = AttributeDefinitions.Charisma;
        commandUndeadEffect.savingThrowDifficultyAbility = AttributeDefinitions.Intelligence;
        commandUndeadEffect.difficultyClassComputation =
            RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency;
        commandUndeadEffect.fixedSavingThrowDifficultyClass = 8;

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("WizardDeadMaster")
            .SetGuiPresentation(Category.Subclass, DomainMischief.GuiPresentation.SpriteReference)
            .AddFeatureAtLevel(autoPreparedSpellsDeadMaster, 2)
            .AddFeatureAtLevel(featureStarkHarvest, 2)
            .AddFeatureAtLevel(featureUndeadChains, 6)
            .AddFeatureAtLevel(featureHardenToNecrotic, 10)
            .AddFeatureAtLevel(powerDeadMasterCommandUndead, 14)
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
            .TryGetElement($"{AttackModifierDeadMasterUndeadChainsPrefix}{proficiencyBonus}",
                out var featureDefinitionAttackModifier))
        {
            monster.ActiveFeatures.Add(featureDefinitionAttackModifier);
        }

        var gameLoreService = ServiceRepository.GetService<IGameLoreService>();

        gameLoreService.LearnMonsterKnowledge(monster.MonsterDefinition, KnowledgeLevelDefinitions.Mastered4);
    }

    private static void OnStarkHarvestKill(GameLocationCharacter character)
    {
        if (Global.CurrentAction is not CharacterActionCastSpell actionCastSpell)
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
                    .Create(ConjureFey, $"CreateDead{monster.name}")
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

                spells.Add(subSpell);
            }

            var spell = SpellDefinitionBuilder
                .Create(ConjureFey, $"CreateDead{level}")
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

            var autoPreparedSpellsGroup =
                new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup
                {
                    ClassLevel = level, SpellsList = new List<SpellDefinition> { spell }
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
