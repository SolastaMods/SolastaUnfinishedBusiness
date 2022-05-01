using System;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.Models;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using static SolastaCommunityExpansion.Classes.Warlock.WarlockSpells;
using static SolastaCommunityExpansion.Classes.Witch.Witch;
using static SolastaCommunityExpansion.Models.SpellsContext;
using static SolastaModApi.DatabaseHelper;
using static SolastaModApi.DatabaseHelper.ConditionDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaModApi.DatabaseHelper.MonsterDefinitions;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;
using static SolastaModApi.DatabaseHelper.SpellListDefinitions;

namespace SolastaCommunityExpansion.Spells
{
    internal static class BazouSpells
    {
        internal static readonly Guid BAZOU_SPELLS_BASE_GUID = new("91384db5-6659-4384-bf2c-3a41160343f4");

        private static SpellDefinition _eldritchOrb;
        internal static SpellDefinition EldritchOrb => _eldritchOrb ??= BuildEldritchOrb();

        private static SpellDefinition _findFamiliar;
        internal static SpellDefinition FindFamiliar => _findFamiliar ??= BuildFindFamiliar();

        private static SpellDefinition _frenzy;
        internal static SpellDefinition Frenzy => _frenzy ??= BuildFrenzy();

        private static SpellDefinition _minorLifesteal;
        internal static SpellDefinition MinorLifesteal => _minorLifesteal ??= BuildMinorLifesteal();

        private static SpellDefinition _petalStorm;
        internal static SpellDefinition PetalStorm => _petalStorm ??= BuildPetalStorm();

        private static SpellDefinition _protectThreshold;
        internal static SpellDefinition ProtectThreshold => _protectThreshold ??= BuildProtectThreshold();

        // don't need since spells are created when first referenced/used
/*        internal static void AddToDB()
        {
            _ = EldritchOrb;
            _ = FindFamiliar;
            _ = Frenzy;
            _ = MinorLifesteal;
            _ = PetalStorm;
            _ = ProtectThreshold;
        }
*/
        internal static void Register()
        {
            RegisterSpell(EldritchOrb, 1, WitchSpellList, WarlockSpellList);
            RegisterSpell(FindFamiliar, 1, WitchSpellList, SpellListWizard, WarlockSpellList);
            RegisterSpell(Frenzy, 1, WitchSpellList, WarlockSpellList, SpellListWizard, SpellListSorcerer);
            RegisterSpell(MinorLifesteal, 1, WitchSpellList, SpellListWizard);
            RegisterSpell(PetalStorm, 1, WitchSpellList, SpellListDruid);
            RegisterSpell(ProtectThreshold, 1, WitchSpellList, SpellListCleric, SpellListDruid, SpellListPaladin);
        }

        private static SpellDefinition BuildEldritchOrb()
        {
            var spell = SpellDefinitionBuilder
                .Create(Fireball, "EldritchOrb", BAZOU_SPELLS_BASE_GUID)
                .SetGuiPresentation(Category.Spell, Shine.GuiPresentation.SpriteReference)
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
                .SetMaterialComponent(RuleDefinitions.MaterialComponentType.None)
                .SetSomaticComponent(true)
                .SetVerboseComponent(true)
                .SetSpellLevel(0)
                .AddToDB();

            // Not sure if I prefer copying and editing existing effect description
            // or creating one from scratch through API
            spell.EffectDescription
                .SetRangeType(RuleDefinitions.RangeType.Distance)
                .SetRangeParameter(12)
                .SetDurationType(RuleDefinitions.DurationType.Instantaneous)
                .SetTargetType(RuleDefinitions.TargetType.Sphere)
                .SetTargetParameter(1)
                .SetHasSavingThrow(false)
                .SetSavingThrowAbility(AttributeDefinitions.Dexterity)
                .SetCanBeDispersed(true);

            spell.EffectDescription.EffectAdvancement
                .SetAdditionalDicePerIncrement(1)
                .SetIncrementMultiplier(5)
                .SetEffectIncrementMethod(RuleDefinitions.EffectIncrementMethod.CasterLevelTable);

            // Changing to a single damage effect with d4, as I am unsure how to implement 2 different effectDescriptions within the same spell
            // First one should be single target attack roll, d8 damage
            // Second one should be adjacent aoe to first target, half of damage of first effect, no damage on saving throw negates
            var effectForm = spell.EffectDescription.EffectForms[0];
            effectForm.SetHasSavingThrow(false).SetLevelMultiplier(1);
            // Bazou to rework - can't have DamageForm and AlterationForm on the same EffectForm
            //effectForm.AlterationForm.SetMaximumIncrease(2).SetValueIncrease(2);
            effectForm.DamageForm.SetDiceNumber(1).SetDieType(RuleDefinitions.DieType.D4).SetDamageType(RuleDefinitions.DamageTypeForce);

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
            var familiarMonsterBuilder = MonsterDefinitionBuilder
                .Create(Eagle_Matriarch, "Owl", BAZOU_SPELLS_BASE_GUID)
                .SetGuiPresentation("OwlFamiliar", Category.Monster, Eagle_Matriarch.GuiPresentation.SpriteReference)
                .SetFeatures(
                    FeatureDefinitionSenses.SenseNormalVision,
                    FeatureDefinitionSenses.SenseDarkvision24,
                    FeatureDefinitionMoveModes.MoveModeMove2,
                    FeatureDefinitionMoveModes.MoveModeFly12,
                    FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityKeenSight,
                    FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityKeenHearing,
                    FeatureDefinitionCombatAffinitys.CombatAffinityFlyby,
                    FeatureDefinitionMovementAffinitys.MovementAffinityNoClimb,
                    FeatureDefinitionMovementAffinitys.MovementAffinityNoSpecialMoves,
                    FeatureDefinitionConditionAffinitys.ConditionAffinityProneImmunity)
                .ClearAttackIterations()
                .SetSkillScores(
                    (DatabaseHelper.SkillDefinitions.Perception.Name, 3),
                    (DatabaseHelper.SkillDefinitions.Stealth.Name, 3)
                )
                .SetArmorClass(11)
                .SetAbilityScores(3, 13, 8, 2, 12, 7)
                .SetHitDiceNumber(1)
                .SetHitDiceType(RuleDefinitions.DieType.D4)
                .SetHitPointsBonus(-1)
                .SetStandardHitPoints(1)
                .SetSizeDefinition(CharacterSizeDefinitions.Tiny)
                .SetAlignment(AlignmentDefinitions.Neutral.Name)
                .SetCharacterFamily(CharacterFamilyDefinitions.Fey.name)
                .SetChallengeRating(0)
                .SetDroppedLootDefinition(null)
                .SetDefaultBattleDecisionPackage(DecisionPackageDefinitions.DefaultSupportCasterWithBackupAttacksDecisions)
                .SetFullyControlledWhenAllied(true)
                .SetDefaultFaction("Party")
                .SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.None);

            if (DatabaseRepository.GetDatabase<FeatureDefinition>().TryGetElement("HelpAction", out FeatureDefinition help))
            {
                familiarMonsterBuilder.AddFeatures(help);
            }

            var familiarMonster = familiarMonsterBuilder.AddToDB();

            var spell = SpellDefinitionBuilder.Create(Fireball, "FindFamiliar", BAZOU_SPELLS_BASE_GUID)
                .SetGuiPresentation(Category.Spell, AnimalFriendship.GuiPresentation.SpriteReference)
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
                .SetMaterialComponent(RuleDefinitions.MaterialComponentType.Specific)
                .SetSomaticComponent(true)
                .SetVerboseComponent(true)
                .SetSpellLevel(1)
                .SetCastingTime(RuleDefinitions.ActivationTime.Hours1)
                // BUG: Unable to have 70 minutes ritual casting time... if set to 10 minutes, it really only takes 10 minutes, instead of 70
                .SetRitualCasting(RuleDefinitions.ActivationTime.Minute10)
                .AddToDB();

            spell.SetUniqueInstance(true);

            spell.EffectDescription.Copy(ConjureAnimalsOneBeast.EffectDescription);
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

            GlobalUniqueEffects.AddToGroup(GlobalUniqueEffects.Group.Familiar, spell);
            
            return spell;
        }

        private static SpellDefinition BuildFrenzy()
        {
            var spell = SpellDefinitionBuilder
                .Create(Confusion, "Frenzy", BAZOU_SPELLS_BASE_GUID)
                .SetGuiPresentation(Category.Spell, Confusion.GuiPresentation.SpriteReference)
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEnchantment)
                .SetMaterialComponent(RuleDefinitions.MaterialComponentType.Mundane)
                .SetSomaticComponent(true)
                .SetVerboseComponent(true)
                .SetSpellLevel(6)
                .SetRequiresConcentration(true)
                .AddToDB();

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

            var conditionDefinition = ConditionDefinitionBuilder
                .Create(ConditionConfused, "ConditionFrenzied", BAZOU_SPELLS_BASE_GUID)
                .SetOrUpdateGuiPresentation("Frenzied", Category.Condition)
                .AddToDB();

            // Some methods are missing like SetField or Copy
            var actionAffinity = FeatureDefinitionActionAffinityBuilder
                .Create(ActionAffinityConditionConfused, "ActionAffinityConditionFrenzied", BAZOU_SPELLS_BASE_GUID)
                .AddToDB();

            actionAffinity.RandomBehaviourOptions.Clear();

            var behaviorMode = new BehaviorModeDescription();
            behaviorMode.SetBehaviour(RuleDefinitions.RandomBehaviour.ConditionDuringTurn);
            // This condition seems to only attack a creature adjacent to where it is. 
            // It will not make the affected creature move towards another creature... :(
            behaviorMode.SetCondition(ConditionConfusedAttack);
            behaviorMode.SetWeight(10);

            actionAffinity.RandomBehaviourOptions.Add(behaviorMode);
            conditionDefinition.Features.SetRange(actionAffinity);

            spell.EffectDescription.EffectForms[0].ConditionForm.SetConditionDefinition(conditionDefinition);

            return spell;
        }

        private static SpellDefinition BuildMinorLifesteal()
        {
            var spell = SpellDefinitionBuilder
                .Create(VampiricTouch, "MinorLifesteal", BAZOU_SPELLS_BASE_GUID)
                .SetGuiPresentation(Category.Spell, VampiricTouch.GuiPresentation.SpriteReference)
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolNecromancy)
                .SetMaterialComponent(RuleDefinitions.MaterialComponentType.None)
                .SetSomaticComponent(true)
                .SetVerboseComponent(false)
                .SetSpellLevel(0)
                .SetRequiresConcentration(false)
                .AddToDB();

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
            // Bazou to rework - can't have DamageForm and AlterationForm on the same EffectForm
            //spell.EffectDescription.EffectForms[1].AlterationForm.SetMaximumIncrease(2);
            //spell.EffectDescription.EffectForms[1].AlterationForm.SetValueIncrease(2);

            return spell;
        }

        private static SpellDefinition BuildPetalStorm()
        {
            var spell = SpellDefinitionBuilder
                .Create(InsectPlague, "PetalStorm", BAZOU_SPELLS_BASE_GUID)
                .SetGuiPresentation(Category.Spell, WindWall.GuiPresentation.SpriteReference)
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
                .SetMaterialComponent(RuleDefinitions.MaterialComponentType.Mundane)
                .SetSomaticComponent(true)
                .SetVerboseComponent(true)
                .SetSpellLevel(2)
                .SetRequiresConcentration(true)
                .AddToDB();

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
            // Bazou to rework - can't have DamageForm and AlterationForm on the same EffectForm
            //spell.EffectDescription.EffectForms[0].AlterationForm.SetMaximumIncrease(2);
            //spell.EffectDescription.EffectForms[0].AlterationForm.SetValueIncrease(2);

            var effectProxyDefinition = EffectProxyDefinitionBuilder
                .Create(EffectProxyDefinitions.ProxyInsectPlague, "ProxyPetalStorm", BAZOU_SPELLS_BASE_GUID)
                .SetGuiPresentation("PetalStorm", Category.Spell, WindWall.GuiPresentation.SpriteReference)
                .SetCanMove()
                .SetPortrait(WindWall.GuiPresentation.SpriteReference)
                .AddAdditionalFeatures(FeatureDefinitionMoveModes.MoveModeMove6)
                .AddToDB()
                // TODO: move into builder
                .SetActionId(ActionDefinitions.Id.ProxyFlamingSphere)
                .SetAttackMethod(RuleDefinitions.ProxyAttackMethod.ReproduceDamageForms)
                .SetCanMoveOnCharacters(true)
                .SetIsEmptyPresentation(false);

            spell.EffectDescription.EffectForms[2].SummonForm.SetEffectProxyDefinitionName(effectProxyDefinition.Name);

            return spell;
        }

        private static SpellDefinition BuildProtectThreshold()
        {
            var spell = SpellDefinitionBuilder
                .Create(SpikeGrowth, "ProtectThreshold", BAZOU_SPELLS_BASE_GUID)
                .SetGuiPresentation(Category.Spell, Bane.GuiPresentation.SpriteReference)
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolAbjuration)
                .SetMaterialComponent(RuleDefinitions.MaterialComponentType.Mundane)
                .SetSomaticComponent(true)
                .SetVerboseComponent(true)
                .SetSpellLevel(2)
                .SetRequiresConcentration(false)
                .SetRitualCasting(RuleDefinitions.ActivationTime.Minute10).AddToDB();

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
            spell.EffectDescription.SetDifficultyClassComputation(RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency);

            spell.EffectDescription.EffectAdvancement.SetAdditionalDicePerIncrement(1);
            spell.EffectDescription.EffectAdvancement.SetIncrementMultiplier(1);
            spell.EffectDescription.EffectAdvancement.SetEffectIncrementMethod(RuleDefinitions.EffectIncrementMethod.PerAdditionalSlotLevel);

            spell.EffectDescription.EffectForms[1].SetHasSavingThrow(true);
            spell.EffectDescription.EffectForms[1].SetSavingThrowAffinity(RuleDefinitions.EffectSavingThrowType.HalfDamage);
            spell.EffectDescription.EffectForms[1].DamageForm.SetDiceNumber(4);
            spell.EffectDescription.EffectForms[1].DamageForm.SetDieType(RuleDefinitions.DieType.D6);
            spell.EffectDescription.EffectForms[1].DamageForm.SetDamageType(RuleDefinitions.DamageTypePsychic);
            spell.EffectDescription.EffectForms[1].SetLevelMultiplier(1);
            // Bazou to rework - can't have DamageForm and AlterationForm on the same EffectForm
            //spell.EffectDescription.EffectForms[1].AlterationForm.SetMaximumIncrease(2);
            //spell.EffectDescription.EffectForms[1].AlterationForm.SetValueIncrease(2);

            const string proxyProtectThreshold = "ProxyProtectThreshold";

            EffectProxyDefinitionBuilder
                .Create(EffectProxyDefinitions.ProxySpikeGrowth, proxyProtectThreshold, BAZOU_SPELLS_BASE_GUID)
                .SetOrUpdateGuiPresentation("ProtectThreshold", Category.Spell)
                .AddToDB();

            spell.EffectDescription.EffectForms[0].SummonForm.SetEffectProxyDefinitionName(proxyProtectThreshold);

            return spell;
        }
    }
}
