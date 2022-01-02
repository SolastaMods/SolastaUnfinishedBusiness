using System;
using System.Collections.Generic;
using SolastaModApi;
using SolastaModApi.BuilderHelpers;
using SolastaModApi.Extensions;
using SolastaCommunityExpansion.Features;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Spells
{
    internal static class BazouSpells
    {
        public static readonly Guid BAZOU_SPELLS_BASE_GUID = new Guid("91384db5-6659-4384-bf2c-3a41160343f4");

        //
        // TODO: add suggested classes and subclasses names list here
        //
        public static void Load()
        {
            SpellsContext.RegisterSpell(BuildEldritchOrb(), "WarlockClassSpelllist", "WitchSpellList");
            SpellsContext.RegisterSpell(BuildFindFamiliar(), "SpellListWizard");
            SpellsContext.RegisterSpell(BuildFrenzy(), "BardClassSpelllist", "SpellListWizard", "WitchSpellList");
            SpellsContext.RegisterSpell(BuildMinorLifesteal(), "SpellListSorcerer", "SpellListWizard", "WitchSpellList");
            SpellsContext.RegisterSpell(BuildPetalStorm(), "SpellListDruid", "WitchSpellList");
            SpellsContext.RegisterSpell(BuildProtectThreshold(), "SpellListSorcerer", "SpellListWizard", "WitchSpellList");
        }

        private static SpellDefinition BuildEldritchOrb()
        {

            var spellBuilder = new SpellBuilder(
                    DatabaseHelper.SpellDefinitions.Fireball,
                    "EldritchOrb",
                    GuidHelper.Create(BAZOU_SPELLS_BASE_GUID, "EldritchOrb").ToString());

            spellBuilder.SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolEvocation);
            spellBuilder.SetMaterialComponent(RuleDefinitions.MaterialComponentType.None);
            spellBuilder.SetSomaticComponent(true);
            spellBuilder.SetVerboseComponent(true);
            spellBuilder.SetSpellLevel(0);
            spellBuilder.SetGuiPresentation(
                    new GuiPresentationBuilder(
                            "Spell/&EldritchOrbDescription",
                            "Spell/&EldritchOrbTitle").Build()
                            .SetSpriteReference(DatabaseHelper.SpellDefinitions.Shine.GuiPresentation.SpriteReference));

            var spell = spellBuilder.AddToDB();

            // Not sure if I prefer copying and editing existing effect description
            // or creating one from scratch through API
            spell.EffectDescription.SetRangeType(RuleDefinitions.RangeType.Distance);
            spell.EffectDescription.SetRangeParameter(12);
            spell.EffectDescription.SetDurationType(RuleDefinitions.DurationType.Instantaneous);
            spell.EffectDescription.SetTargetType(RuleDefinitions.TargetType.Sphere);
            spell.EffectDescription.SetTargetParameter(1);
            spell.EffectDescription.SetHasSavingThrow(false);
            spell.EffectDescription.SetSavingThrowAbility(AttributeDefinitions.Dexterity);
            spell.EffectDescription.SetCanBeDispersed(true);
            spell.EffectDescription.EffectAdvancement.SetAdditionalDicePerIncrement(1);
            spell.EffectDescription.EffectAdvancement.SetIncrementMultiplier(5);
            spell.EffectDescription.EffectAdvancement.SetEffectIncrementMethod(RuleDefinitions.EffectIncrementMethod.CasterLevelTable);

            // Changing to a single damage effect with d4, as I am unsure how to implement 2 different effectDescriptions within the same spell
            // First one should be single target attack roll, d8 damage
            // Second one should be adjacent aoe to first target, half of damage of first effect, no damage on saving throw negates
            spell.EffectDescription.EffectForms[0].SetHasSavingThrow(false);
            spell.EffectDescription.EffectForms[0].DamageForm.SetDiceNumber(1);
            spell.EffectDescription.EffectForms[0].DamageForm.SetDieType(RuleDefinitions.DieType.D4);
            spell.EffectDescription.EffectForms[0].DamageForm.SetDamageType(RuleDefinitions.DamageTypeForce);
            spell.EffectDescription.EffectForms[0].SetLevelMultiplier(1);
            spell.EffectDescription.EffectForms[0].AlterationForm.SetMaximumIncrease(2);
            spell.EffectDescription.EffectForms[0].AlterationForm.SetValueIncrease(2);

            // Not sure if I prefer copying and editing existing effect forms
            // or creating one from scratch through API
            //            var effectForm = new EffectFormBuilder().Build();

            //            effectForm.Copy(spell.EffectDescription.EffectForms[0]);
            //            effectForm.SetHasSavingThrow(true);
            //            effectForm.SetSavingThrowAffinity(RuleDefinitions.EffectSavingThrowType.Negates);
            //            effectForm.DamageForm.SetDieType(RuleDefinitions.DieType.D4);

            //            spell.EffectDescription.EffectForms.Add(effectForm);

            return spell;

        }
        private static SpellDefinition BuildFindFamiliar()
        {

            var familiarMonsterBuilder = new MonsterBuilder(
                    "Owl",
                    GuidHelper.Create(BAZOU_SPELLS_BASE_GUID, "Owl").ToString(),
                    "Owl",
                    "Owl",
                    DatabaseHelper.MonsterDefinitions.Eagle_Matriarch)
                    .ClearFeatures()
                    .AddFeatures(new List<FeatureDefinition>{
                            DatabaseHelper.FeatureDefinitionSenses.SenseNormalVision,
                            DatabaseHelper.FeatureDefinitionSenses.SenseDarkvision24,
                            DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove2,
                            DatabaseHelper.FeatureDefinitionMoveModes.MoveModeFly12,
                            DatabaseHelper.FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityKeenSight,
                            DatabaseHelper.FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityKeenHearing,
                            DatabaseHelper.FeatureDefinitionCombatAffinitys.CombatAffinityFlyby,
                            DatabaseHelper.FeatureDefinitionMovementAffinitys.MovementAffinityNoClimb,
                            DatabaseHelper.FeatureDefinitionMovementAffinitys.MovementAffinityNoSpecialMoves,
                            DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityProneImmunity,
                            })
                    .ClearAttackIterations()
                    .ClearSkillScores()
                    .AddSkillScores(new List<MonsterSkillProficiency>{
                            new MonsterSkillProficiency(DatabaseHelper.SkillDefinitions.Perception.Name, 3),
                            new MonsterSkillProficiency(DatabaseHelper.SkillDefinitions.Stealth.Name, 3)
                    })
                    .SetArmorClass(11)
                    .SetAbilityScores(3, 13, 8, 2, 12, 7)
                    .SetHitDiceNumber(1)
                    .SetHitDiceType(RuleDefinitions.DieType.D4)
                    .SetHitPointsBonus(-1)
                    .SetStandardHitPoints(1)
                    .SetSizeDefinition(DatabaseHelper.CharacterSizeDefinitions.Tiny)
                    .SetAlignment(DatabaseHelper.AlignmentDefinitions.Neutral.Name)
                    .SetCharacterFamily(DatabaseHelper.CharacterFamilyDefinitions.Fey.name)
                    .SetChallengeRating(0)
                    .SetDroppedLootDefinition(null)
                    .SetDefaultBattleDecisionPackage(DatabaseHelper.DecisionPackageDefinitions.DefaultSupportCasterWithBackupAttacksDecisions)
                    .SetFullyControlledWhenAllied(true)
                    .SetDefaultFaction("Party")
                    .SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.None);

            if (DatabaseRepository.GetDatabase<FeatureDefinition>().TryGetElement("HelpAction", out FeatureDefinition help))
            {
                familiarMonsterBuilder.AddFeatures(new List<FeatureDefinition> { help });
            }

            var familiarMonster = familiarMonsterBuilder.AddToDB();

            var spellBuilder = new SpellBuilder(
                    DatabaseHelper.SpellDefinitions.Fireball,
                    "FindFamiliar",
                    GuidHelper.Create(BAZOU_SPELLS_BASE_GUID, "FindFamiliar").ToString());

            spellBuilder.SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolConjuration);
            spellBuilder.SetMaterialComponent(RuleDefinitions.MaterialComponentType.Specific);
            spellBuilder.SetSomaticComponent(true);
            spellBuilder.SetVerboseComponent(true);
            spellBuilder.SetSpellLevel(1);
            spellBuilder.SetCastingTime(RuleDefinitions.ActivationTime.Hours1);
            // BUG: Unable to have 70 minutes ritual casting time... if set to 10 minutes, it really only takes 10 minutes, instead of 70
            spellBuilder.SetRitualCasting(RuleDefinitions.ActivationTime.Hours1);
            spellBuilder.SetGuiPresentation(
                    new GuiPresentationBuilder(
                            "Spell/&FindFamiliarDescription",
                            "Spell/&FindFamiliarTitle").Build()
                            .SetSpriteReference(DatabaseHelper.SpellDefinitions.AnimalFriendship.GuiPresentation.SpriteReference));

            var spell = spellBuilder.AddToDB();

            spell.SetUniqueInstance(true);

            spell.EffectDescription.Copy(DatabaseHelper.SpellDefinitions.ConjureAnimalsOneBeast.EffectDescription);
            spell.EffectDescription.SetRangeType(RuleDefinitions.RangeType.Distance);
            spell.EffectDescription.SetRangeParameter(2);
            spell.EffectDescription.SetDurationType(RuleDefinitions.DurationType.Permanent);
            spell.EffectDescription.SetTargetSide(RuleDefinitions.Side.Ally);
            spell.EffectDescription.EffectForms.Clear();

            var summonForm = new SummonForm();
            summonForm.SetMonsterDefinitionName(familiarMonster.name);
            summonForm.SetDecisionPackage(null);

            var effectForm = new EffectForm();
            effectForm.SetFormType(EffectForm.EffectFormType.Summon);
            effectForm.SetCreatedByCharacter(true);
            effectForm.SetSummonForm(summonForm);

            spell.EffectDescription.EffectForms.Add(effectForm);

            return spell;

        }
        private static SpellDefinition BuildFrenzy()
        {

            var spellBuilder = new SpellBuilder(
                    DatabaseHelper.SpellDefinitions.Confusion,
                    "Frenzy",
                    GuidHelper.Create(BAZOU_SPELLS_BASE_GUID, "Frenzy").ToString());

            spellBuilder.SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolEnchantment);
            spellBuilder.SetMaterialComponent(RuleDefinitions.MaterialComponentType.Mundane);
            spellBuilder.SetSomaticComponent(true);
            spellBuilder.SetVerboseComponent(true);
            spellBuilder.SetSpellLevel(6);
            spellBuilder.SetConcentration();
            spellBuilder.SetGuiPresentation(
                    new GuiPresentationBuilder(
                            "Spell/&FrenzyDescription",
                            "Spell/&FrenzyTitle").Build()
                            .SetSpriteReference(DatabaseHelper.SpellDefinitions.Confusion.GuiPresentation.SpriteReference));

            var spell = spellBuilder.AddToDB();

            // Not sure if I prefer copying and editing existing effect description
            // or creating one from scratch through API
            spell.EffectDescription.SetRangeType(RuleDefinitions.RangeType.Distance);
            spell.EffectDescription.SetRangeParameter(24);
            spell.EffectDescription.SetDurationType(RuleDefinitions.DurationType.Minute);
            spell.EffectDescription.SetDurationParameter(1);
            spell.EffectDescription.SetTargetType(RuleDefinitions.TargetType.Sphere);
            spell.EffectDescription.SetTargetParameter(4);
            spell.EffectDescription.SetHasSavingThrow(true);
            spell.EffectDescription.SetSavingThrowAbility(AttributeDefinitions.Wisdom);

            var conditionDefinition = new ConditionDefinitionBuilder<ConditionDefinition>(
                    DatabaseHelper.ConditionDefinitions.ConditionConfused,
                    "ConditionFrenzied",
                    GuidHelper.Create(BAZOU_SPELLS_BASE_GUID, "ConditionFrenzied").ToString(),
                    new GuiPresentationBuilder(
                            "Condition/&FrenziedDescription",
                            "Condition/&FrenziedTitle").Build()
                            .SetSpriteReference(DatabaseHelper.ConditionDefinitions.ConditionConfusedAttack.GuiPresentation.SpriteReference))
                    .AddToDB();

            conditionDefinition.Features.Clear();

            // Some methods are missing like SetField or Copy
            var actionAffinity = new FeatureDefinitionActionAffinityBuilder(
                    DatabaseHelper.FeatureDefinitionActionAffinitys.ActionAffinityConditionConfused,
                    "ActionAffinityConditionFrenzied",
                    GuidHelper.Create(BAZOU_SPELLS_BASE_GUID, "ActionAffinityConditionFrenzied").ToString())
                    .AddToDB();

            actionAffinity.RandomBehaviourOptions.Clear();

            var behaviorMode = new BehaviorModeDescription();
            behaviorMode.SetBehaviour(RuleDefinitions.RandomBehaviour.ConditionDuringTurn);
            // This condition seems to only attack a creature adjacent to where it is. 
            // It will not make the affected creature move towards another creature... :(
            behaviorMode.SetCondition(DatabaseHelper.ConditionDefinitions.ConditionConfusedAttack);
            behaviorMode.SetWeight(10);

            actionAffinity.RandomBehaviourOptions.Add(behaviorMode);
            conditionDefinition.Features.Add(actionAffinity);

            spell.EffectDescription.EffectForms[0].ConditionForm.SetConditionDefinition(conditionDefinition);

            return spell;

        }
        private static SpellDefinition BuildMinorLifesteal()
        {

            var spellBuilder = new SpellBuilder(
                    DatabaseHelper.SpellDefinitions.VampiricTouch,
                    "MinorLifesteal",
                    GuidHelper.Create(BAZOU_SPELLS_BASE_GUID, "MinorLifesteal").ToString());

            spellBuilder.SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolNecromancy);
            spellBuilder.SetMaterialComponent(RuleDefinitions.MaterialComponentType.None);
            spellBuilder.SetSomaticComponent(true);
            spellBuilder.SetVerboseComponent(false);
            spellBuilder.SetSpellLevel(0);
            spellBuilder.SetGuiPresentation(
                    new GuiPresentationBuilder(
                            "Spell/&MinorLifestealDescription",
                            "Spell/&MinorLifestealTitle").Build()
                            .SetSpriteReference(DatabaseHelper.SpellDefinitions.VampiricTouch.GuiPresentation.SpriteReference));

            var spell = spellBuilder.AddToDB();

            // Missing method in the API to set concentration to FALSE
            spell.SetRequiresConcentration(false);

            spell.EffectDescription.SetRangeType(RuleDefinitions.RangeType.Distance);
            spell.EffectDescription.SetRangeParameter(12);
            spell.EffectDescription.SetDurationType(RuleDefinitions.DurationType.Instantaneous);
            spell.EffectDescription.SetTargetType(RuleDefinitions.TargetType.Individuals);
            spell.EffectDescription.SetHasSavingThrow(true);
            spell.EffectDescription.SetHalfDamageOnAMiss(false);
            spell.EffectDescription.SetSavingThrowAbility(AttributeDefinitions.Constitution);
            spell.EffectDescription.EffectAdvancement.SetAdditionalDicePerIncrement(1);
            spell.EffectDescription.EffectAdvancement.SetIncrementMultiplier(5);
            spell.EffectDescription.EffectAdvancement.SetEffectIncrementMethod(RuleDefinitions.EffectIncrementMethod.CasterLevelTable);

            spell.EffectDescription.EffectForms[1].SetHasSavingThrow(true);
            spell.EffectDescription.EffectForms[1].SetSavingThrowAffinity(RuleDefinitions.EffectSavingThrowType.Negates);
            spell.EffectDescription.EffectForms[1].DamageForm.SetDiceNumber(1);
            spell.EffectDescription.EffectForms[1].DamageForm.SetDieType(RuleDefinitions.DieType.D4);
            spell.EffectDescription.EffectForms[1].DamageForm.SetDamageType(RuleDefinitions.DamageTypeNecrotic);
            spell.EffectDescription.EffectForms[1].DamageForm.SetHealFromInflictedDamage(RuleDefinitions.HealFromInflictedDamage.Full);
            spell.EffectDescription.EffectForms[1].SetLevelMultiplier(1);
            spell.EffectDescription.EffectForms[1].AlterationForm.SetMaximumIncrease(2);
            spell.EffectDescription.EffectForms[1].AlterationForm.SetValueIncrease(2);

            return spell;

        }
        private static SpellDefinition BuildPetalStorm()
        {

            var spellBuilder = new SpellBuilder(
                    DatabaseHelper.SpellDefinitions.InsectPlague,
                    "PetalStorm",
                    GuidHelper.Create(BAZOU_SPELLS_BASE_GUID, "PetalStorm").ToString());

            spellBuilder.SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolConjuration);
            spellBuilder.SetMaterialComponent(RuleDefinitions.MaterialComponentType.Mundane);
            spellBuilder.SetSomaticComponent(true);
            spellBuilder.SetVerboseComponent(true);
            spellBuilder.SetSpellLevel(2);
            spellBuilder.SetConcentration();
            spellBuilder.SetGuiPresentation(
                    new GuiPresentationBuilder(
                            "Spell/&PetalStormDescription",
                            "Spell/&PetalStormTitle").Build()
                            .SetSpriteReference(DatabaseHelper.SpellDefinitions.WindWall.GuiPresentation.SpriteReference));

            var spell = spellBuilder.AddToDB();

            // Not sure if I prefer copying and editing existing effect description
            // or creating one from scratch through API
            spell.EffectDescription.SetRangeType(RuleDefinitions.RangeType.Distance);
            spell.EffectDescription.SetRangeParameter(12);
            spell.EffectDescription.SetDurationType(RuleDefinitions.DurationType.Minute);
            spell.EffectDescription.SetDurationParameter(1);
            spell.EffectDescription.SetTargetType(RuleDefinitions.TargetType.Cube);
            spell.EffectDescription.SetTargetParameter(3);
            spell.EffectDescription.SetHasSavingThrow(true);
            spell.EffectDescription.SetSavingThrowAbility(AttributeDefinitions.Strength);
            spell.EffectDescription.SetRecurrentEffect((RuleDefinitions.RecurrentEffect)20);
            spell.EffectDescription.EffectAdvancement.SetAdditionalDicePerIncrement(2);
            spell.EffectDescription.EffectAdvancement.SetIncrementMultiplier(1);
            spell.EffectDescription.EffectAdvancement.SetEffectIncrementMethod(RuleDefinitions.EffectIncrementMethod.PerAdditionalSlotLevel);

            spell.EffectDescription.EffectForms[0].SetHasSavingThrow(true);
            spell.EffectDescription.EffectForms[0].SetSavingThrowAffinity(RuleDefinitions.EffectSavingThrowType.Negates);
            spell.EffectDescription.EffectForms[0].DamageForm.SetDiceNumber(3);
            spell.EffectDescription.EffectForms[0].DamageForm.SetDieType(RuleDefinitions.DieType.D4);
            spell.EffectDescription.EffectForms[0].DamageForm.SetDamageType(RuleDefinitions.DamageTypeSlashing);
            spell.EffectDescription.EffectForms[0].SetLevelMultiplier(1);
            spell.EffectDescription.EffectForms[0].AlterationForm.SetMaximumIncrease(2);
            spell.EffectDescription.EffectForms[0].AlterationForm.SetValueIncrease(2);

            var effectProxyDefinitionBuilder = new EffectProxyDefinitionBuilder(
                    DatabaseHelper.EffectProxyDefinitions.ProxyInsectPlague,
                    "ProxyPetalStorm",
                    GuidHelper.Create(BAZOU_SPELLS_BASE_GUID, "ProxyPetalStorm").ToString());

            effectProxyDefinitionBuilder.SetGuiPresentation(
                    new GuiPresentationBuilder(
                            "Spell/&PetalStormDescription",
                            "Spell/&PetalStormTitle").Build()
                            .SetSpriteReference(DatabaseHelper.SpellDefinitions.WindWall.GuiPresentation.SpriteReference));
            effectProxyDefinitionBuilder.SetCanMove();
            effectProxyDefinitionBuilder.SetPortrait(DatabaseHelper.SpellDefinitions.WindWall.GuiPresentation.SpriteReference);
            effectProxyDefinitionBuilder.AddAdditionalFeature(DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove6);

            var effectProxyDefinition = effectProxyDefinitionBuilder.AddToDB();

            effectProxyDefinition.SetActionId(ActionDefinitions.Id.ProxyFlamingSphere);
            effectProxyDefinition.SetAttackMethod(RuleDefinitions.ProxyAttackMethod.ReproduceDamageForms);
            effectProxyDefinition.SetCanMoveOnCharacters(true);
            effectProxyDefinition.SetIsEmptyPresentation(false);

            spell.EffectDescription.EffectForms[2].SummonForm.SetEffectProxyDefinitionName("ProxyPetalStorm");

            return spell;

        }
        private static SpellDefinition BuildProtectThreshold()
        {

            var spellBuilder = new SpellBuilder(
                    DatabaseHelper.SpellDefinitions.SpikeGrowth,
                    "ProtectThreshold",
                    GuidHelper.Create(BAZOU_SPELLS_BASE_GUID, "ProtectThreshold").ToString());

            spellBuilder.SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolAbjuration);
            spellBuilder.SetMaterialComponent(RuleDefinitions.MaterialComponentType.Mundane);
            spellBuilder.SetSomaticComponent(true);
            spellBuilder.SetVerboseComponent(true);
            spellBuilder.SetSpellLevel(2);
            spellBuilder.SetRitualCasting(RuleDefinitions.ActivationTime.Minute10);
            spellBuilder.SetGuiPresentation(
                    new GuiPresentationBuilder(
                            "Spell/&ProtectThresholdDescription",
                            "Spell/&ProtectThresholdTitle").Build()
                            .SetSpriteReference(DatabaseHelper.SpellDefinitions.Bane.GuiPresentation.SpriteReference));

            var spell = spellBuilder.AddToDB();
            spell.SetRequiresConcentration(false);

            // Not sure if I prefer copying and editing existing effect description
            // or creating one from scratch through API
            spell.EffectDescription.SetRangeType(RuleDefinitions.RangeType.Distance);
            spell.EffectDescription.SetRangeParameter(1);
            spell.EffectDescription.SetDurationType(RuleDefinitions.DurationType.Minute);
            spell.EffectDescription.SetDurationParameter(10);
            spell.EffectDescription.SetTargetType(RuleDefinitions.TargetType.Sphere);
            spell.EffectDescription.SetTargetParameter(0);
            spell.EffectDescription.SetHasSavingThrow(true);
            spell.EffectDescription.SetSavingThrowAbility(AttributeDefinitions.Wisdom);
            spell.EffectDescription.SetRecurrentEffect(RuleDefinitions.RecurrentEffect.OnEnter);

            spell.EffectDescription.EffectAdvancement.SetAdditionalDicePerIncrement(1);
            spell.EffectDescription.EffectAdvancement.SetIncrementMultiplier(1);
            spell.EffectDescription.EffectAdvancement.SetEffectIncrementMethod(RuleDefinitions.EffectIncrementMethod.PerAdditionalSlotLevel);

            spell.EffectDescription.EffectForms[1].SetHasSavingThrow(true);
            spell.EffectDescription.EffectForms[1].SetSavingThrowAffinity(RuleDefinitions.EffectSavingThrowType.HalfDamage);
            spell.EffectDescription.EffectForms[1].DamageForm.SetDiceNumber(4);
            spell.EffectDescription.EffectForms[1].DamageForm.SetDieType(RuleDefinitions.DieType.D6);
            spell.EffectDescription.EffectForms[1].DamageForm.SetDamageType(RuleDefinitions.DamageTypePsychic);
            spell.EffectDescription.EffectForms[1].SetLevelMultiplier(1);
            spell.EffectDescription.EffectForms[1].AlterationForm.SetMaximumIncrease(2);
            spell.EffectDescription.EffectForms[1].AlterationForm.SetValueIncrease(2);

            return spell;

        }

    }
}
