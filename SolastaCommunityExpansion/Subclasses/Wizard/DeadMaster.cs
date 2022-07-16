using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.Models;
using static SolastaCommunityExpansion.Api.DatabaseHelper;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaCommunityExpansion.Api.DatabaseHelper.SpellDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.MonsterDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.SchoolOfMagicDefinitions;

namespace SolastaCommunityExpansion.Subclasses.Wizard;

internal sealed class DeadMaster : AbstractSubclass
{
    private static readonly Guid SubclassNamespace = new("58ce31d8-6a37-4d6e-ffea-b9b60658a3ef");

    private static readonly Dictionary<int, List<MonsterDefinition>> CreateDeadSpellMonsters = new()
    {
        {3, new List<MonsterDefinition> {Zombie, Skeleton, Skeleton_Archer}},
        // {5, new List<MonsterDefinition> {Ghoul, Skeleton_Enforcer}},
        // {7, new List<MonsterDefinition> {Skeleton_Knight, Skeleton_Marksman, Skeleton_Sorcerer}}
    };

    private static CharacterSubclassDefinition Subclass { get; set; }

    internal DeadMaster()
    {
        var autoPreparedSpellsWizardDeadMaster = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsWizardDeadMaster", SubclassNamespace)
            .SetGuiPresentation(Category.Feature)
            .SetCastingClass(CharacterClassDefinitions.Wizard)
            .SetPreparedSpellGroups(GetDeadSpellAutoPreparedGroups())
            .AddToDB();

        var featureStarkHarvest = FeatureDefinitionOnCharacterKillBuilder
            .Create("FeatureStarkHarvest", SubclassNamespace)
            .SetGuiPresentationNoContent()
            .SetOnCharacterKill(OnStarkHarvestKill)
            .AddToDB();

        for (var i = 2; i < 7; i++)
        {
            _ = FeatureDefinitionAttackModifierBuilder
                .Create($"FeatureUndeadChainsAttackModifier{i}", SubclassNamespace)
                .SetGuiPresentation("FeatureUndeadChainsAttackModifier", Category.Feature)
                .Configure(RuleDefinitions.AttackModifierMethod.FlatValue, i)
                .AddToDB();
        }

        var featureUndeadChains = FeatureDefinitionOnCharacterKillBuilder
            .Create("FeatureUndeadChains", SubclassNamespace)
            .SetGuiPresentationNoContent()
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("WizardDeadMaster", SubclassNamespace)
            .SetGuiPresentation(Category.Subclass, DomainMischief.GuiPresentation.SpriteReference)
            .AddFeatureAtLevel(featureStarkHarvest, 2)
            .AddFeatureAtLevel(autoPreparedSpellsWizardDeadMaster, 6)
            .AddFeatureAtLevel(featureUndeadChains, 6)
            .AddFeatureAtLevel(DamageAffinityNecroticImmunity, 10)
            .AddToDB();
    }

    internal static void OnMonsterCreated(RulesetCharacterMonster monster)
    {
        if (Global.CastedSpell == null || !Global.CastedSpell.Name.StartsWith("CreateDead"))
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
            .TryGetElement($"FeatureUndeadChainsAttackModifier{proficiencyBonus}", out var featureDefinitionAttackModifier))
        {
            monster.ActiveFeatures.Add(featureDefinitionAttackModifier);
        }
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

        if (characterFamily == CharacterFamilyDefinitions.Construct.Name ||
            characterFamily == CharacterFamilyDefinitions.Undead.Name)
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
        GetDeadSpellAutoPreparedGroups()
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
                    .SetGuiPresentation(Category.Spell)
                    .SetSchoolOfMagic(SchoolNecromancy)
                    .SetSpellLevel(level)
                    .SetCastingTime(RuleDefinitions.ActivationTime.Minute1)
                    .SetSubSpells()
                    .AddToDB();

                monster.fullyControlledWhenAllied = true;
                subSpell.EffectDescription.durationType = RuleDefinitions.DurationType.UntilLongRest;
                subSpell.EffectDescription.EffectAdvancement.additionalSummonsPerIncrement = 2;
                subSpell.EffectDescription.EffectForms[0].SummonForm.monsterDefinitionName = monster.name;
                spells.Add(subSpell);
            }

            var spell = SpellDefinitionBuilder
                .Create(ConjureFey, $"CreateDead{level}", SubclassNamespace)
                .SetGuiPresentation(Category.Spell)
                .SetSchoolOfMagic(SchoolNecromancy)
                .SetSpellLevel(level)
                .SetCastingTime(RuleDefinitions.ActivationTime.Minute1)
                .SetSubSpells(spells)
                .AddToDB();

            spell.EffectDescription.EffectAdvancement.additionalSummonsPerIncrement = 2;
            spell.EffectDescription.EffectAdvancement.effectIncrementMethod =
                RuleDefinitions.EffectIncrementMethod.PerAdditionalSlotLevel;

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
