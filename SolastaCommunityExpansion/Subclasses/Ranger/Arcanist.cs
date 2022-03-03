using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using UnityEngine.AddressableAssets;
using static SolastaModApi.DatabaseHelper;
using static SolastaModApi.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionAdditionalDamages;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionMagicAffinitys;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaCommunityExpansion.Subclasses.Ranger
{
    internal class Arcanist : AbstractSubclass
    {
        private CharacterSubclassDefinition Subclass;
        internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
        {
            return FeatureDefinitionSubclassChoices.SubclassChoiceRangerArchetypes;
        }
        internal override CharacterSubclassDefinition GetSubclass()
        {
            return Subclass ??= BuildAndAddSubclass();
        }

        private const string RangerArcanistRangerSubclassName = "RangerArcanistRangerSubclass";
        private const string RangerArcanistRangerSubclassGuid = "5ABD870D-9ABD-4953-A2EC-E2109324FAB9";

        public static readonly Guid RA_BASE_GUID = new(RangerArcanistRangerSubclassGuid);

        public static readonly FeatureDefinitionFeatureSet ranger_arcanist_magic = CreateRangerArcanistMagic();
        public static readonly FeatureDefinitionAdditionalDamage arcanist_mark = CreateArcanistMark();
        public static readonly FeatureDefinitionAdditionalDamage arcane_detonation = CreateArcaneDetonation();
        public static readonly FeatureDefinition arcane_detonation_upgrade = CreateArcaneDetonationUpgrade();
        public static readonly Dictionary<int, FeatureDefinitionPower> arcane_pulse_dict = CreateArcanePulseDict();

        public static CharacterSubclassDefinition BuildAndAddSubclass()
        {
            return CharacterSubclassDefinitionBuilder
                .Create(RangerArcanistRangerSubclassName, RangerArcanistRangerSubclassGuid)
                .SetGuiPresentation(Category.Subclass, RoguishShadowCaster.GuiPresentation.SpriteReference)
                .AddFeatureAtLevel(ranger_arcanist_magic, 3)
                .AddFeatureAtLevel(arcanist_mark, 3)
                .AddFeatureAtLevel(arcane_detonation, 3)
                .AddFeatureAtLevel(arcane_pulse_dict[7], 7)
                .AddFeatureAtLevel(arcane_detonation_upgrade, 11)
                .AddFeatureAtLevel(arcane_pulse_dict[15], 15)
                .AddToDB();
        }

        // Common helper: factor out
        private static DiceByRank BuildDiceByRank(int rank, int dice)
        {
            DiceByRank diceByRank = new DiceByRank();
            diceByRank.SetRank(rank);
            diceByRank.SetDiceNumber(dice);
            return diceByRank;
        }

        private static FeatureDefinitionFeatureSet CreateRangerArcanistMagic()
        {
            GuiPresentation blank = new GuiPresentationBuilder("Feature/&NoContentTitle", "Feature/&NoContentTitle").Build();

            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup arcanistSpells1 = new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup()
            {
                ClassLevel = 2,
                SpellsList = new List<SpellDefinition>() { SpellDefinitions.Shield, }
            };
            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup arcanistSpells2 = new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup()
            {
                ClassLevel = 5,
                SpellsList = new List<SpellDefinition>() { SpellDefinitions.MistyStep, }
            };
            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup arcanistSpells3 = new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup()
            {
                ClassLevel = 9,
                SpellsList = new List<SpellDefinition>() { SpellDefinitions.Haste, }
            };
            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup arcanistSpells4 = new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup()
            {
                ClassLevel = 13,
                SpellsList = new List<SpellDefinition>() { SpellDefinitions.DimensionDoor, }
            };
            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup arcanistSpells5 = new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup()
            {
                ClassLevel = 17,
                SpellsList = new List<SpellDefinition>() { SpellDefinitions.HoldMonster, }
            };

            FeatureDefinitionAutoPreparedSpells preparedSpells = new FeatureDefinitionAutoPreparedSpellsBuilder("ArcanistAutoPreparedSpells",
                GuidHelper.Create(RA_BASE_GUID, "ArcanistAutoPreparedSpells").ToString(),
               new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>() {
                    arcanistSpells1, arcanistSpells2, arcanistSpells3, arcanistSpells4, arcanistSpells5},
               CharacterClassDefinitions.Ranger, blank).AddToDB();

            var arcanist_affinity = FeatureDefinitionMagicAffinityBuilder
                .Create(MagicAffinityBattleMagic, "MagicAffinityRangerArcanist", RA_BASE_GUID)
                .SetGuiPresentationNoContent()
                .AddToDB();

            GuiPresentation arcanistMagicGui = new GuiPresentationBuilder("Feature/&RangerArcanistMagicTitle", "Feature/&RangerArcanistMagicDescription").Build();
            return new FeatureDefinitionFeatureSetBuilder("RangerArcanistMagic",
                GuidHelper.Create(RA_BASE_GUID, "RangerArcanistManaTouchedGuardian").ToString(), // Oops, will have to live with this name being off)
                new List<FeatureDefinition>() { preparedSpells, arcanist_affinity },
                FeatureDefinitionFeatureSet.FeatureSetMode.Union, arcanistMagicGui).AddToDB();
        }

        private sealed class FeatureDefinitionAutoPreparedSpellsBuilder : Builders.Features.FeatureDefinitionAutoPreparedSpellsBuilder
        {
            public FeatureDefinitionAutoPreparedSpellsBuilder(string name, string guid, List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup> autospelllists,
                CharacterClassDefinition characterclass, GuiPresentation guiPresentation) : base(name, guid)
            {
                Definition.AutoPreparedSpellsGroups.SetRange(autospelllists);
                Definition.SetSpellcastingClass(characterclass);
                Definition.SetGuiPresentation(guiPresentation);
            }
        }

        private sealed class FeatureDefinitionFeatureSetBuilder : Builders.Features.FeatureDefinitionFeatureSetBuilder
        {
            public FeatureDefinitionFeatureSetBuilder(string name, string guid, List<FeatureDefinition> features,
                FeatureDefinitionFeatureSet.FeatureSetMode mode, GuiPresentation guiPresentation) : base(name, guid)
            {
                Definition.FeatureSet.SetRange(features);
                Definition.SetMode(mode);
                Definition.SetGuiPresentation(guiPresentation);
                // enumerateInDescription and uniqueChoices default to false.
            }
        }

        private static FeatureDefinitionAdditionalDamage CreateArcanistMark()
        {
            var marked_condition = ConditionMarkedByArcanistBuilder.GetOrAdd();

            return FeatureDefinitionAdditionalDamageBuilder
                .Create(AdditionalDamageHuntersMark, "AdditionalDamageArcanistMark", RA_BASE_GUID)
                .SetGuiPresentation("ArcanistMark", Category.Feature)
                .SetSpecificDamageType("DamageForce")
                .SetDamageDice(RuleDefinitions.DieType.D6, 0)
                .SetNotificationTag("ArcanistMark")
                .SetTriggerCondition(RuleDefinitions.AdditionalDamageTriggerCondition.AlwaysActive)
                .SetNoSave()
                .SetNoAdvancement()
                .SetConditionOperations(
                    new ConditionOperationDescription
                    {
                        ConditionDefinition = marked_condition,
                        Operation = ConditionOperationDescription.ConditionOperation.Add
                    }
                )
                .AddToDB();
        }

        private static FeatureDefinitionAdditionalDamage CreateArcaneDetonation()
        {
            var marked_condition = ConditionMarkedByArcanistBuilder.GetOrAdd();

            var asset_reference = new AssetReference();
            asset_reference.SetField("m_AssetGUID", "9f1fe10e6ef8c9c43b6b2ef91b2ad38a");

            return FeatureDefinitionAdditionalDamageBuilder
                .Create(AdditionalDamageHuntersMark, "AdditionalDamageArcaneDetonation", RA_BASE_GUID)
                .SetGuiPresentation("ArcaneDetonation", Category.Feature)
                .SetSpecificDamageType("DamageForce")
                .SetDamageDice(RuleDefinitions.DieType.D6, 1)
                .SetNotificationTag("ArcanistMark")
                .SetTargetCondition(marked_condition, RuleDefinitions.AdditionalDamageTriggerCondition.TargetHasConditionCreatedByMe)
                .SetNoSave()
                .SetConditionOperations(
                    new ConditionOperationDescription
                    {
                        ConditionDefinition = marked_condition,
                        Operation = ConditionOperationDescription.ConditionOperation.Remove
                    }
                )
                .SetClassAdvancement(
                    BuildDiceByRank(1, 1),
                    BuildDiceByRank(2, 1),
                    BuildDiceByRank(3, 1),
                    BuildDiceByRank(4, 1),
                    BuildDiceByRank(5, 1),
                    BuildDiceByRank(6, 1),
                    BuildDiceByRank(7, 1),
                    BuildDiceByRank(8, 1),
                    BuildDiceByRank(9, 1),
                    BuildDiceByRank(10, 1),
                    BuildDiceByRank(11, 2),
                    BuildDiceByRank(12, 2),
                    BuildDiceByRank(13, 2),
                    BuildDiceByRank(14, 2),
                    BuildDiceByRank(15, 2),
                    BuildDiceByRank(16, 2),
                    BuildDiceByRank(17, 2),
                    BuildDiceByRank(18, 2),
                    BuildDiceByRank(19, 2),
                    BuildDiceByRank(20, 2))
                .SetFrequencyLimit(RuleDefinitions.FeatureLimitedUsage.None)
                .SetImpactParticleReference(asset_reference)
                .AddToDB();
        }

        private static FeatureDefinition CreateArcaneDetonationUpgrade()
        {
            // This is a blank feature. It does nothing except create a description for what happens at level 11.
            return new FeatureDefinitionBuilder("AdditionalDamageArcaneDetonationUpgrade",
                GuidHelper.Create(RA_BASE_GUID, "AdditionalDamageArcaneDetonationUpgrade").ToString(),
                new GuiPresentationBuilder("Feature/&ArcaneDetonationUpgradeTitle", "Feature/&ArcaneDetonationUpgradeDescription").Build()).AddToDB();
        }

        private sealed class FeatureDefinitionBuilder : Builders.Features.FeatureDefinitionBuilder
        {
            public FeatureDefinitionBuilder(string name, string guid, GuiPresentation guiPresentation) : base(name, guid)
            {
                Definition.SetGuiPresentation(guiPresentation);
            }
        }

        private static Dictionary<int, FeatureDefinitionPower> CreateArcanePulseDict()
        {
            var marked_effect = new EffectForm
            {
                ConditionForm = new ConditionForm(),
                FormType = EffectForm.EffectFormType.Condition
            };
            marked_effect.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
            marked_effect.ConditionForm.ConditionDefinition = ConditionMarkedByArcanistBuilder.GetOrAdd();

            var damage_effect = new EffectForm
            {
                DamageForm = new DamageForm
                {
                    DamageType = "DamageForce",
                    DieType = RuleDefinitions.DieType.D8,
                    DiceNumber = 4
                }
            };
            damage_effect.DamageForm.SetHealFromInflictedDamage(RuleDefinitions.HealFromInflictedDamage.Never);
            damage_effect.SavingThrowAffinity = RuleDefinitions.EffectSavingThrowType.None;

            var damage_upgrade_effect = new EffectForm
            {
                DamageForm = new DamageForm
                {
                    DamageType = "DamageForce",
                    DieType = RuleDefinitions.DieType.D8,
                    DiceNumber = 8
                }
            };
            damage_upgrade_effect.DamageForm.SetHealFromInflictedDamage(RuleDefinitions.HealFromInflictedDamage.Never);
            damage_upgrade_effect.SavingThrowAffinity = RuleDefinitions.EffectSavingThrowType.None;

            var arcane_pulse_action = CreateArcanePulse("ArcanePulse", "Feature/&ArcanePulseTitle", "Feature/&ArcanePulseDescription", marked_effect, damage_effect);

            var arcane_pulse_upgrade_action = CreateArcanePulse("ArcanePulseUpgrade", "Feature/&ArcanePulseTitle", "Feature/&ArcanePulseDescription", marked_effect, damage_upgrade_effect);
            arcane_pulse_upgrade_action.SetOverriddenPower(arcane_pulse_action);

            return new Dictionary<int, FeatureDefinitionPower>{
                {7, arcane_pulse_action},
                {15, arcane_pulse_upgrade_action}};
        }

        private static FeatureDefinitionPower CreateArcanePulse(string name, string title, string description, EffectForm marked_effect, EffectForm damage_effect)
        {
            var pulse_description = new EffectDescription();
            pulse_description.Copy(SpellDefinitions.MagicMissile.EffectDescription);
            pulse_description.SetCreatedByCharacter(true);
            pulse_description.SetTargetSide(RuleDefinitions.Side.Enemy);
            pulse_description.SetTargetType(RuleDefinitions.TargetType.Sphere);
            pulse_description.SetTargetParameter(3);
            pulse_description.SetRangeType(RuleDefinitions.RangeType.Distance);
            pulse_description.SetRangeParameter(30);

            pulse_description.EffectForms.Clear();
            pulse_description.EffectForms.AddRange(new List<EffectForm>
            {
                damage_effect,
                marked_effect
            });

            return FeatureDefinitionPowerBuilder
                .Create(name, RA_BASE_GUID)
                .SetGuiPresentation(title, description, PowerDomainElementalHeraldOfTheElementsThunder.GuiPresentation.SpriteReference)
                .SetUsesAbility(0, AttributeDefinitions.Wisdom)
                .SetShowCasting(true)
                .SetRechargeRate(RuleDefinitions.RechargeRate.LongRest)
                .SetActivation(RuleDefinitions.ActivationTime.Action, 1)
                .SetEffectDescription(pulse_description)
                .SetAbility(AttributeDefinitions.Wisdom)
                .SetShortTitle("Arcane Pulse")
                .AddToDB();
        }
    }

    // Creates a dedicated builder for the marked by arcanist condition. This helps with GUID wonkiness on the fact that separate features interact with it.
    internal sealed class ConditionMarkedByArcanistBuilder : ConditionDefinitionBuilder
    {
        private ConditionMarkedByArcanistBuilder(string name, string guid) : base(ConditionDefinitions.ConditionMarkedByBrandingSmite, name, guid)
        {
            Definition.GuiPresentation.Title = "Condition/&ConditionMarkedByArcanistTitle";
            Definition.GuiPresentation.Description = "Condition/&ConditionMarkedByArcanistDescription";

            Definition.SetAllowMultipleInstances(false);
            Definition.SetDurationParameter(1);
            Definition.SetDurationType(RuleDefinitions.DurationType.Permanent);
            Definition.SetTurnOccurence(RuleDefinitions.TurnOccurenceType.EndOfTurn);
            Definition.SetPossessive(true);
            Definition.SetSpecialDuration(true);
        }

        public static ConditionDefinition CreateAndAddToDB()
        {
            return new ConditionMarkedByArcanistBuilder("ConditionMarkedByArcanist", GuidHelper.Create(Arcanist.RA_BASE_GUID, "ConditionMarkedByArcanist").ToString()).AddToDB();
        }

        public static ConditionDefinition GetOrAdd()
        {
            var db = DatabaseRepository.GetDatabase<ConditionDefinition>();
            return db.TryGetElement("ConditionMarkedByArcanist", GuidHelper.Create(Arcanist.RA_BASE_GUID, "ConditionMarkedByArcanist").ToString()) ?? CreateAndAddToDB();
        }
    }
}
