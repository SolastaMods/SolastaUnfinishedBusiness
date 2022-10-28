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
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class WizardDeadMaster : AbstractSubclass
{
    private const string WizardDeadMasterName = "WizardDeadMaster";
    private const string CreateDeadSpellPrefix = "CreateDead";
    private const string AttackModifierDeadMasterUndeadChainsPrefix = "AttackModifierDeadMasterUndeadChains";

    internal WizardDeadMaster()
    {
        var spriteReference = CustomIcons.GetSprite("CreateDead", Resources.CreateDead, 128, 128);

        var autoPreparedSpellsDeadMaster = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsDeadMaster")
            .SetGuiPresentation(Category.Feature)
            .SetSpellcastingClass(CharacterClassDefinitions.Wizard)
            .SetPreparedSpellGroups(GetDeadSpellAutoPreparedGroups(spriteReference))
            .AddToDB();

        var onCharacterKillDeadMasterStarkHarvest = FeatureDefinitionOnCharacterKillBuilder
            .Create("OnCharacterKillDeadMasterStarkHarvest")
            .SetGuiPresentation(Category.Feature)
            .SetOnCharacterKill(OnStarkHarvestKill)
            .AddToDB();

        var onCharacterKillDeadMasterUndeadChains = FeatureDefinitionOnCharacterKillBuilder
            .Create("OnCharacterKillDeadMasterUndeadChains")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        // all possible proficiency bonuses 
        for (var i = 2; i < 7; i++)
        {
            _ = FeatureDefinitionAttackModifierBuilder
                .Create($"{AttackModifierDeadMasterUndeadChainsPrefix}{i}")
                .SetGuiPresentation("OnCharacterKillDeadMasterUndeadChains", Category.Feature)
                .SetAttackRollModifier(i)
                .AddToDB();
        }

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
                    .SetRestrictedCreatureFamilies(CharacterFamilyDefinitions.Undead)
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
                onCharacterKillDeadMasterStarkHarvest)
            .AddFeaturesAtLevel(6,
                onCharacterKillDeadMasterUndeadChains)
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

        var caster = Global.ActionCharacter.RulesetCharacter as RulesetCharacterHero
                     ?? Global.ActionCharacter.RulesetCharacter.OriginalFormCharacter as
                         RulesetCharacterHero;

        if (caster == null
            || !caster.ClassesAndSubclasses.TryGetValue(CharacterClassDefinitions.Wizard, out var subclassDefinition)
            || subclassDefinition.Name != WizardDeadMasterName)
        {
            return;
        }

        var casterLevel = caster.ClassesAndLevels[CharacterClassDefinitions.Wizard];

        if (casterLevel < 6)
        {
            return;
        }

        var proficiencyBonus = caster.GetAttribute(AttributeDefinitions.ProficiencyBonus).CurrentValue;

        monster.GetAttribute(AttributeDefinitions.HitPoints).BaseValue += casterLevel;

        if (TryGetDefinition<FeatureDefinitionAttackModifier>(
                $"{AttackModifierDeadMasterUndeadChainsPrefix}{proficiencyBonus}",
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

        var attacker =
            actionCastSpell.ActingCharacter.RulesetCharacter as RulesetCharacterHero
            ?? actionCastSpell.ActingCharacter.RulesetCharacter.OriginalFormCharacter as RulesetCharacterHero;

        if (attacker == null)
        {
            return;
        }

        var spellLevel = actionCastSpell.ActiveSpell.SpellDefinition.SpellLevel;
        var isNecromancy = actionCastSpell.ActiveSpell.SpellDefinition.SchoolOfMagic == SchoolNecromancy;
        var healingReceived = (isNecromancy ? 3 : 2) * spellLevel;

        attacker.ReceiveHealing(healingReceived, true, attacker.Guid);
    }

    [NotNull]
    private static FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup[]
        GetDeadSpellAutoPreparedGroups(AssetReferenceSprite spriteReference)
    {
        var createDeadSpellMonsters = new Dictionary<int, List<MonsterDefinition>>
        {
            { 3, new List<MonsterDefinition> { Zombie, Skeleton, Skeleton_Archer } },
            { 5, new List<MonsterDefinition> { Ghoul, Skeleton_Enforcer } },
            { 7, new List<MonsterDefinition> { Skeleton_Knight, Skeleton_Marksman, Skeleton_Sorcerer } }
        };

        var result = new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup[createDeadSpellMonsters.Count];

        for (var j = 0; j < createDeadSpellMonsters.Count; j++)
        {
            var kvp = createDeadSpellMonsters.ElementAt(j);
            var level = kvp.Key;
            var monsters = kvp.Value;
            var spells = new SpellDefinition[monsters.Count];

            for (var i = 0; i < monsters.Count; i++)
            {
                var monster = monsters[i];
                var subSpell = SpellDefinitionBuilder
                    .Create(ConjureFey, $"CreateDead{monster.name}")
                    .SetGuiPresentation(monster.GuiPresentation.Title, monster.GuiPresentation.Description,
                        spriteReference)
                    .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolNecromancy)
                    .SetSpellLevel(level)
                    .SetRequiresConcentration(false)
                    .SetCastingTime(ActivationTime.Minute1)
                    .SetSubSpells()
                    .AddToDB();

                monster.fullyControlledWhenAllied = true;

                subSpell.EffectDescription.rangeParameter = 1;
                subSpell.EffectDescription.EffectForms[0].SummonForm.monsterDefinitionName = monster.name;
                subSpell.EffectDescription.EffectAdvancement.effectIncrementMethod = EffectIncrementMethod.None;
                subSpell.EffectDescription.durationType = DurationType.UntilAnyRest;

                spells[i] = subSpell;
            }

            var spell = SpellDefinitionBuilder
                .Create(ConjureFey, $"CreateDead{level}")
                .SetGuiPresentation(Category.Spell, spriteReference)
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolNecromancy)
                .SetSpellLevel(level)
                .SetRequiresConcentration(false)
                .SetCastingTime(ActivationTime.Minute1)
                .SetSubSpells(spells)
                .AddToDB();

            spell.EffectDescription.effectForms.Clear();
            spell.EffectDescription.rangeParameter = 1;
            spell.EffectDescription.effectAdvancement.effectIncrementMethod = EffectIncrementMethod.None;
            spell.EffectDescription.durationType = DurationType.UntilAnyRest;

            var autoPreparedSpellsGroup =
                new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup
                {
                    ClassLevel = level, SpellsList = new List<SpellDefinition> { spell }
                };

            result[j] = autoPreparedSpellsGroup;
        }

        return result;
    }
}
