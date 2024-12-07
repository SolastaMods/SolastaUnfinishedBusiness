using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Interfaces;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;

namespace SolastaUnfinishedBusiness.Models;

internal static partial class Tabletop2024Context
{
    private static readonly List<(string, string)> GuidanceProficiencyPairs =
    [
        (AttributeDefinitions.Dexterity, SkillDefinitions.Acrobatics),
        (AttributeDefinitions.Wisdom, SkillDefinitions.AnimalHandling),
        (AttributeDefinitions.Intelligence, SkillDefinitions.Arcana),
        (AttributeDefinitions.Strength, SkillDefinitions.Athletics),
        (AttributeDefinitions.Charisma, SkillDefinitions.Deception),
        (AttributeDefinitions.Intelligence, SkillDefinitions.History),
        (AttributeDefinitions.Wisdom, SkillDefinitions.Insight),
        (AttributeDefinitions.Charisma, SkillDefinitions.Intimidation),
        (AttributeDefinitions.Intelligence, SkillDefinitions.Investigation),
        (AttributeDefinitions.Wisdom, SkillDefinitions.Medecine),
        (AttributeDefinitions.Intelligence, SkillDefinitions.Nature),
        (AttributeDefinitions.Wisdom, SkillDefinitions.Perception),
        (AttributeDefinitions.Charisma, SkillDefinitions.Performance),
        (AttributeDefinitions.Charisma, SkillDefinitions.Persuasion),
        (AttributeDefinitions.Intelligence, SkillDefinitions.Religion),
        (AttributeDefinitions.Dexterity, SkillDefinitions.SleightOfHand),
        (AttributeDefinitions.Dexterity, SkillDefinitions.Stealth),
        (AttributeDefinitions.Wisdom, SkillDefinitions.Survival)
    ];

    private static readonly List<SpellDefinition> GuidanceSubSpells = [];

    private static readonly ConditionDefinition ConditionTrueStrike2024 = ConditionDefinitionBuilder
        .Create("ConditionTrueStrike2024")
        .SetGuiPresentationNoContent(true)
        .SetSilent(Silent.WhenAddedOrRemoved)
        .SetSpecialDuration()
        .SetFeatures(
            FeatureDefinitionAdditionalDamageBuilder
                .Create("AdditionalDamageTrueStrike")
                .SetGuiPresentationNoContent(true)
                .SetNotificationTag("TrueStrike")
                .SetRequiredProperty(RestrictedContextRequiredProperty.Weapon)
                .SetDamageDice(DieType.D6, 0)
                .SetSpecificDamageType(DamageTypeRadiant)
                .SetAdvancement(
                    ExtraAdditionalDamageAdvancement.CharacterLevel,
                    DiceByRankBuilder.InterpolateDiceByRankTable(0, 20, (5, 1), (11, 2), (17, 3)))
                .SetImpactParticleReference(SacredFlame
                    .EffectDescription.EffectParticleParameters.effectParticleReference)
                .SetAttackModeOnly()
                .AddToDB())
        .SetSpecialInterruptions(ExtraConditionInterruption.AttacksWithWeaponOrUnarmed)
        .AddCustomSubFeatures(new ModifyAttackActionModifierTrueStrike())
        .AddToDB();

    private static readonly EffectForm EffectFormPowerWordStunStopped = EffectFormBuilder
        .Create()
        .SetFilterId(1)
        .SetConditionForm(
            ConditionDefinitionBuilder
                .Create(CustomConditionsContext.StopMovement, "ConditionPowerWordStunStopped")
                .SetSpecialDuration(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
                .AddToDB(),
            ConditionForm.ConditionOperation.Add)
        .Build();

    internal static void SwitchSpellRitualOnAllCasters()
    {
        var subclasses = SharedSpellsContext.SubclassCasterType.Keys.Select(GetDefinition<CharacterSubclassDefinition>);

        if (Main.Settings.EnableRitualOnAllCasters2024)
        {
            Paladin.FeatureUnlocks.Add(new FeatureUnlockByLevel(FeatureSetClericRitualCasting,
                Main.Settings.EnablePaladinSpellCastingAtLevel1 ? 1 : 2));
            Ranger.FeatureUnlocks.Add(new FeatureUnlockByLevel(FeatureSetClericRitualCasting,
                Main.Settings.EnableRangerSpellCastingAtLevel1 ? 1 : 2));
            Sorcerer.FeatureUnlocks.Add(new FeatureUnlockByLevel(FeatureSetClericRitualCasting, 1));
        }
        else
        {
            Paladin.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == FeatureSetClericRitualCasting);
            Ranger.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == FeatureSetClericRitualCasting);
            Sorcerer.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == FeatureSetClericRitualCasting);
        }

        Paladin.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
        Ranger.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
        Sorcerer.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);

        foreach (var subclass in subclasses
                     .Where(x => x.HasSubFeatureOfType<FeatureDefinitionCastSpell>()))
        {
            if (Main.Settings.EnableRitualOnAllCasters2024)
            {
                subclass.FeatureUnlocks.Add(new FeatureUnlockByLevel(FeatureSetClericRitualCasting, 3));
            }
            else
            {
                subclass.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == FeatureSetClericRitualCasting);
            }

            subclass.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
        }
    }

    internal static void SwitchOneDndSpellBarkskin()
    {
        if (Main.Settings.EnableOneDndBarkskinSpell)
        {
            Barkskin.requiresConcentration = false;
            Barkskin.castingTime = ActivationTime.BonusAction;
            AttributeModifierBarkskin.modifierValue = 17;
            Barkskin.GuiPresentation.description = "Spell/&BarkskinOneDndDescription";
            ConditionBarkskin.GuiPresentation.description = "Rules/&ConditionOneDndBarkskinDescription";
        }
        else
        {
            Barkskin.requiresConcentration = true;
            Barkskin.castingTime = ActivationTime.Action;
            AttributeModifierBarkskin.modifierValue = 16;
            Barkskin.GuiPresentation.description = "Spell/&BarkskinDescription";
            ConditionBarkskin.GuiPresentation.description = "Rules/&ConditionBarkskinDescription";
        }
    }

    private static void LoadOneDndSpellGuidanceSubspells()
    {
        foreach (var (attribute, skill) in GuidanceProficiencyPairs)
        {
            var proficiencyPair = (attribute, skill);
            var affinity = $"AbilityCheckAffinityGuidance{skill}";
            var condition = $"ConditionGuidance{skill}";

            GuidanceSubSpells.Add(
                SpellDefinitionBuilder
                    .Create($"Guidance{skill}")
                    .SetGuiPresentation(Category.Spell, Guidance.GuiPresentation.SpriteReference)
                    .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolDivination)
                    .SetSpellLevel(0)
                    .SetCastingTime(ActivationTime.Action)
                    .SetMaterialComponent(MaterialComponentType.None)
                    .SetVerboseComponent(true)
                    .SetSomaticComponent(true)
                    .SetRequiresConcentration(true)
                    .SetVocalSpellSameType(VocalSpellSemeType.Buff)
                    .SetEffectDescription(
                        EffectDescriptionBuilder
                            .Create()
                            .SetDurationData(DurationType.Minute, 1)
                            .SetTargetingData(Side.Ally, RangeType.Touch, 0, TargetType.IndividualsUnique)
                            .SetEffectForms(EffectFormBuilder.ConditionForm(
                                ConditionDefinitionBuilder
                                    .Create(ConditionGuided, condition)
                                    .SetGuiPresentation(Category.Condition, ConditionGuided)
                                    .SetSpecialInterruptions(ConditionInterruption.None)
                                    .SetFeatures(
                                        FeatureDefinitionAbilityCheckAffinityBuilder
                                            .Create(affinity)
                                            .SetGuiPresentationNoContent(true)
                                            .BuildAndSetAffinityGroups(CharacterAbilityCheckAffinity.None, DieType.D4,
                                                1, AbilityCheckGroupOperation.AddDie, proficiencyPair)
                                            .AddToDB())
                                    .AddToDB()))
                            .SetParticleEffectParameters(Guidance)
                            .Build())
                    .AddToDB());
        }
    }

    internal static void SwitchOneDndSpellDivineFavor()
    {
        DivineFavor.requiresConcentration = !Main.Settings.EnableOneDndDivineFavorSpell;
    }

    internal static void SwitchOneDndSpellLesserRestoration()
    {
        LesserRestoration.castingTime = Main.Settings.EnableOneDndLesserRestorationSpell
            ? ActivationTime.BonusAction
            : ActivationTime.Action;
    }

    private static void LoadOneDndSpellSpareTheDying()
    {
        SpareTheDying.AddCustomSubFeatures(new ModifyEffectDescriptionSpareTheDying());
    }

    internal static void SwitchOneDndSpellSpareTheDying()
    {
        SpareTheDying.GuiPresentation.description =
            Main.Settings.EnableOneDndSpareTheDyingSpell
                ? "Spell/&SpareTheDyingDescription"
                : "Spell/&SpareTheDyingExtendedDescription";
    }

    internal static void SwitchOneDndSpellSpiderClimb()
    {
        SpiderClimb.EffectDescription.EffectAdvancement.additionalTargetsPerIncrement =
            Main.Settings.EnableOneDndSpiderClimbSpell
                ? 1
                : 0;
        SpiderClimb.EffectDescription.EffectAdvancement.effectIncrementMethod =
            Main.Settings.EnableOneDndSpiderClimbSpell
                ? EffectIncrementMethod.PerAdditionalSlotLevel
                : EffectIncrementMethod.None;
    }

    internal static void SwitchOneDndSpellStoneSkin()
    {
        Stoneskin.GuiPresentation.description = "Spell/&StoneskinExtendedDescription";
        ConditionStoneskin.GuiPresentation.description = "Rules/&ConditionStoneskinExtendedDescription";
        DamageAffinityStoneskinBludgeoning.TagsIgnoringAffinity.Clear();
        DamageAffinityStoneskinPiercing.TagsIgnoringAffinity.Clear();
        DamageAffinityStoneskinSlashing.TagsIgnoringAffinity.Clear();

        if (Main.Settings.EnableOneDndStoneSkinSpell)
        {
            return;
        }

        Stoneskin.GuiPresentation.description = "Spell/&StoneskinDescription";
        ConditionStoneskin.GuiPresentation.description = "Rules/&ConditionStoneskinDescription";
        DamageAffinityStoneskinBludgeoning.TagsIgnoringAffinity.AddRange(
            TagsDefinitions.MagicalWeapon, TagsDefinitions.MagicalEffect);
        DamageAffinityStoneskinPiercing.TagsIgnoringAffinity.AddRange(
            TagsDefinitions.MagicalWeapon, TagsDefinitions.MagicalEffect);
        DamageAffinityStoneskinSlashing.TagsIgnoringAffinity.AddRange(
            TagsDefinitions.MagicalWeapon, TagsDefinitions.MagicalEffect);
    }

    internal static void SwitchOneDndSpellGuidance()
    {
        foreach (var spell in GuidanceSubSpells)
        {
            spell.implemented = false;
        }

        if (Main.Settings.EnableOneDndGuidanceSpell)
        {
            Guidance.spellsBundle = true;
            Guidance.SubspellsList.SetRange(GuidanceSubSpells);
            Guidance.compactSubspellsTooltip = true;
            Guidance.EffectDescription.EffectForms.Clear();
            Guidance.GuiPresentation.description = "Spell/&OneDndGuidanceDescription";
        }
        else
        {
            Guidance.spellsBundle = false;
            Guidance.SubspellsList.Clear();
            Guidance.EffectDescription.EffectForms.SetRange(EffectFormBuilder.ConditionForm(ConditionGuided));
            Guidance.GuiPresentation.description = "Spell/&GuidanceDescription";
        }
    }

    internal static void SwitchOneDndSpellHideousLaughter()
    {
        HideousLaughter.EffectDescription.EffectAdvancement.effectIncrementMethod =
            Main.Settings.EnableOneDndHideousLaughterSpell
                ? EffectIncrementMethod.PerAdditionalSlotLevel
                : EffectIncrementMethod.None;
    }

    internal static void SwitchOneDndSpellHuntersMark()
    {
        FeatureDefinitionAdditionalDamages.AdditionalDamageHuntersMark.specificDamageType = DamageTypeForce;
        FeatureDefinitionAdditionalDamages.AdditionalDamageHuntersMark.additionalDamageType =
            Main.Settings.EnableOneDndHuntersMarkSpell
                ? AdditionalDamageType.Specific
                : AdditionalDamageType.SameAsBaseDamage;
        HuntersMark.GuiPresentation.description =
            Main.Settings.EnableOneDndHuntersMarkSpell
                ? "Spell/&HuntersMarkExtendedDescription"
                : "Spell/&HuntersMarkDescription";
        ConditionMarkedByHunter.GuiPresentation.description =
            Main.Settings.EnableOneDndHuntersMarkSpell
                ? "Rules/&ConditionMarkedByHunterExtendedDescription"
                : "Rules/&ConditionMarkedByHunterDescription";
    }

    internal static void SwitchOneDndSpellMagicWeapon()
    {
        if (Main.Settings.EnableOneDndMagicWeaponSpell)
        {
            MagicWeapon.requiresConcentration = false;
            MagicWeapon.castingTime = ActivationTime.BonusAction;
            MagicWeapon.EffectDescription.EffectForms[0].ItemPropertyForm.FeatureBySlotLevel[1].level = 3;
        }
        else
        {
            MagicWeapon.requiresConcentration = true;
            MagicWeapon.castingTime = ActivationTime.Action;
            MagicWeapon.EffectDescription.EffectForms[0].ItemPropertyForm.FeatureBySlotLevel[1].level = 4;
        }
    }

    internal static void SwitchOneDndSpellPowerWordStun()
    {
        var effectForms = PowerWordStun.EffectDescription.EffectForms;

        if (effectForms.Count > 1)
        {
            effectForms.RemoveAt(1);
            PowerWordStun.EffectDescription.EffectFormFilters.RemoveAt(1);
        }

        PowerWordStun.GuiPresentation.description = "Spell/&PowerWordStunDescription";

        if (!Main.Settings.EnableOneDndPowerWordStunSpell)
        {
            return;
        }

        PowerWordStun.GuiPresentation.description = "Spell/&PowerWordStunExtendedDescription";
        PowerWordStun.EffectDescription.EffectFormFilters.Add(
            new EffectFormFilter { effectFormId = 1, minHitPoints = 151, maxHitPoints = 10000 });
        effectForms.Add(EffectFormPowerWordStunStopped);
    }

    internal static void SwitchOneDndPreparedSpellsTables()
    {
        if (Main.Settings.EnablePreparedSpellsTables2024)
        {
            FeatureDefinitionCastSpells.CastSpellBard.knownSpells =
                [4, 5, 6, 7, 9, 10, 11, 12, 14, 15, 16, 16, 17, 17, 18, 18, 19, 20, 21, 22];

            if (Main.Settings.EnableRangerSpellCastingAtLevel1)
            {
                FeatureDefinitionCastSpells.CastSpellRanger.knownSpells =
                    [2, 3, 4, 5, 6, 6, 7, 7, 9, 9, 10, 10, 11, 11, 12, 12, 14, 14, 15, 15];
            }
            else
            {
                FeatureDefinitionCastSpells.CastSpellRanger.knownSpells =
                    [0, 3, 4, 5, 6, 6, 7, 7, 9, 9, 10, 10, 11, 11, 12, 12, 14, 14, 15, 15];
            }

            FeatureDefinitionCastSpells.CastSpellSorcerer.knownSpells =
                [2, 4, 6, 7, 9, 10, 11, 12, 14, 15, 16, 16, 17, 17, 18, 18, 19, 20, 21, 22];
        }
        else
        {
            FeatureDefinitionCastSpells.CastSpellBard.knownSpells =
                [4, 5, 6, 7, 8, 9, 10, 11, 12, 14, 15, 15, 16, 18, 19, 19, 20, 22, 22, 22];

            if (Main.Settings.EnableRangerSpellCastingAtLevel1)
            {
                FeatureDefinitionCastSpells.CastSpellRanger.knownSpells =
                    [2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 10, 10, 11, 11];
            }
            else
            {
                FeatureDefinitionCastSpells.CastSpellRanger.knownSpells =
                    [0, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 10, 10, 11, 11];
            }

            FeatureDefinitionCastSpells.CastSpellSorcerer.knownSpells =
                [2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 12, 13, 13, 14, 14, 15, 15, 15, 15];
        }
    }

    private static void LoadOneDndTrueStrike()
    {
        if (!Main.Settings.EnableOneDndTrueStrikeCantrip)
        {
            return;
        }

        TrueStrike.AddCustomSubFeatures(FixesContext.NoTwinned.Mark, AttackAfterMagicEffect.MarkerAnyWeaponAttack);
        TrueStrike.GuiPresentation.description = "Spell/&TrueStrike2024Description";
        TrueStrike.requiresConcentration = false;
        TrueStrike.effectDescription = EffectDescriptionBuilder
            .Create()
            .SetDurationData(DurationType.Round)
            // 24 seems to be the max range on Solasta ranged weapons
            .SetTargetingData(Side.Enemy, RangeType.Distance, 24, TargetType.IndividualsUnique)
            .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, additionalDicePerIncrement: 1)
            .SetEffectForms(
                EffectFormBuilder.ConditionForm(ConditionTrueStrike2024, ConditionForm.ConditionOperation.Add, true))
            .SetParticleEffectParameters(SacredFlame)
            .SetImpactEffectParameters(new AssetReference())
            .Build();
    }

    internal static void SwitchOneDndHealingSpellsUpgrade()
    {
        var dice = Main.Settings.EnableOneDndHealingSpellsUpgrade ? 2 : 1;

        // Cure Wounds, Healing Word got buf on base damage and add dice
        CureWounds.EffectDescription.EffectForms[0].healingForm.diceNumber = dice;
        CureWounds.EffectDescription.effectAdvancement.additionalDicePerIncrement = dice;
        FalseLife.EffectDescription.EffectForms[0].temporaryHitPointsForm.diceNumber = dice;
        HealingWord.EffectDescription.EffectForms[0].healingForm.diceNumber = dice;
        HealingWord.EffectDescription.effectAdvancement.additionalDicePerIncrement = dice;

        // Mass Cure Wounds and Mass Healing Word only got buf on base damage
        MassHealingWord.EffectDescription.EffectForms[0].healingForm.diceNumber = dice;

        dice = Main.Settings.EnableOneDndHealingSpellsUpgrade ? 5 : 3;

        MassCureWounds.EffectDescription.EffectForms[0].healingForm.diceNumber = dice;

        var school = Main.Settings.EnableOneDndHealingSpellsUpgrade ? SchoolAbjuration : SchoolEvocation;
        SpellsContext.AuraOfVitality.schoolOfMagic = school;
        CureWounds.schoolOfMagic = school;
        Heal.schoolOfMagic = school;
        HealingWord.schoolOfMagic = school;
        MassCureWounds.schoolOfMagic = school;
        MassHealingWord.schoolOfMagic = school;
        PrayerOfHealing.schoolOfMagic = school;
    }

    internal static void SwitchOneDndDamagingSpellsUpgrade()
    {
        EffectProxyDefinitions.ProxyArcaneSword.AdditionalFeatures.Clear();

        if (Main.Settings.EnableOneDndDamagingSpellsUpgrade)
        {
            EffectProxyDefinitions.ProxyArcaneSword.damageDie = DieType.D12;
            EffectProxyDefinitions.ProxyArcaneSword.damageDieNum = 4;
            EffectProxyDefinitions.ProxyArcaneSword.addAbilityToDamage = true;
            EffectProxyDefinitions.ProxyArcaneSword.AdditionalFeatures.AddRange(
                FeatureDefinitionMoveModes.MoveModeFly2,
                FeatureDefinitionMoveModes.MoveModeMove6);
            CircleOfDeath.EffectDescription.EffectForms[0].DamageForm.dieType = DieType.D8;
            FlameStrike.EffectDescription.EffectForms[0].DamageForm.diceNumber = 5;
            FlameStrike.EffectDescription.EffectForms[1].DamageForm.diceNumber = 5;
            PrismaticSpray.EffectDescription.EffectForms
                .Where(x => x.FormType == EffectForm.EffectFormType.Damage)
                .Do(y => y.DamageForm.DiceNumber = 12);
            IceStorm.EffectDescription.EffectForms[0].DamageForm.dieType = DieType.D10;
            ViciousMockery.EffectDescription.EffectForms[0].DamageForm.dieType = DieType.D6;
        }
        else
        {
            EffectProxyDefinitions.ProxyArcaneSword.damageDie = DieType.D10;
            EffectProxyDefinitions.ProxyArcaneSword.damageDieNum = 3;
            EffectProxyDefinitions.ProxyArcaneSword.addAbilityToDamage = false;
            EffectProxyDefinitions.ProxyArcaneSword.AdditionalFeatures.AddRange(
                FeatureDefinitionMoveModes.MoveModeFly2,
                FeatureDefinitionMoveModes.MoveModeMove4);
            CircleOfDeath.EffectDescription.EffectForms[0].DamageForm.dieType = DieType.D6;
            FlameStrike.EffectDescription.EffectForms[0].DamageForm.diceNumber = 4;
            FlameStrike.EffectDescription.EffectForms[1].DamageForm.diceNumber = 4;
            PrismaticSpray.EffectDescription.EffectForms
                .Where(x => x.FormType == EffectForm.EffectFormType.Damage)
                .Do(y => y.DamageForm.DiceNumber = 10);
            IceStorm.EffectDescription.EffectForms[0].DamageForm.dieType = DieType.D8;
            ViciousMockery.EffectDescription.EffectForms[0].DamageForm.dieType = DieType.D4;
        }
    }

    private sealed class ModifyEffectDescriptionSpareTheDying : IModifyEffectDescription
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return Main.Settings.EnableOneDndSpareTheDyingSpell && definition == SpareTheDying;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            if (!Main.Settings.EnableOneDndSpareTheDyingSpell)
            {
                return effectDescription;
            }

            effectDescription.RangeType = RangeType.Distance;

            var level = character.TryGetAttributeValue(AttributeDefinitions.CharacterLevel);
            var power = level switch
            {
                >= 17 => 3,
                >= 11 => 2,
                >= 5 => 1,
                _ => 0
            };

            effectDescription.rangeParameter = 3 * (int)Math.Pow(2, power);

            return effectDescription;
        }
    }

    private sealed class ModifyAttackActionModifierTrueStrike : IModifyAttackActionModifier
    {
        public void OnAttackComputeModifier(
            RulesetCharacter attacker,
            RulesetCharacter defender,
            BattleDefinitions.AttackProximity attackProximity,
            RulesetAttackMode attackMode,
            string effectName,
            ref ActionModifier attackModifier)
        {
            if (attackMode == null ||
                attacker.SpellsCastByMe.Count == 0)
            {
                return;
            }

            var damageForm = attackMode.EffectDescription.FindFirstDamageForm();

            if (damageForm != null)
            {
                damageForm.damageType = DamageTypeRadiant;
            }

            var oldAttribute = attackMode.AbilityScore;
            var newAttribute = attacker.SpellsCastByMe[attacker.SpellsCastByMe.Count - 1].SourceAbility;

            CanUseAttribute.ChangeAttackModeAttributeIfBetter(
                attacker, attackMode, oldAttribute, newAttribute, true);
        }
    }
}
