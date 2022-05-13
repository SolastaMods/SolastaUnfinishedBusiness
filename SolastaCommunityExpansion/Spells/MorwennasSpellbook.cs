using System;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi.Extensions;
using static SolastaCommunityExpansion.Models.SpellsContext;
using static SolastaModApi.DatabaseHelper;
using static SolastaModApi.DatabaseHelper.ConditionDefinitions;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;
using static SolastaModApi.DatabaseHelper.SpellListDefinitions;

namespace SolastaCommunityExpansion.Spells
{
    public static class MorwennasSpellbook
    {
        internal static readonly Guid MORWENNA_BASE_GUID = new("56f97707-9af1-4761-987a-63b84ba18d46");

        public static readonly SpellDefinition IlluminatingSphere = BuildIlluminatingSphere();
        public static readonly SpellDefinition RadiantMotes = BuildRadiantMotes();
        public static readonly SpellDefinition Mule = BuildMule();

        internal static void AddToDB()
        {
            _ = IlluminatingSphere;
            _ = RadiantMotes;
            _ = Mule;
        }

        internal static void Register()
        {
            //cantrip
            RegisterSpell(IlluminatingSphere, 0, SpellListWizard);

            //level 1
            RegisterSpell(RadiantMotes, 0, SpellListWizard);
            RegisterSpell(Mule, 0, SpellListWizard);

        }

        private static SpellDefinition BuildIlluminatingSphere()
        {
            const string name = "IlluminatingSphere";

            var spell = SpellDefinitionBuilder
                .Create(Sparkle, name, MORWENNA_BASE_GUID)
                .SetGuiPresentation(Category.Spell, Shine.GuiPresentation.SpriteReference)
                .AddToDB();

            spell.EffectDescription.SetRangeType(RuleDefinitions.RangeType.Distance);
            spell.EffectDescription.SetRangeParameter(18);
            spell.EffectDescription.SetTargetType(RuleDefinitions.TargetType.Sphere);
            spell.EffectDescription.SetTargetParameter(5);
            spell.EffectDescription.SetEffectParticleParameters(SacredFlame_B.EffectDescription.EffectParticleParameters);

            return spell;
        }

        private static SpellDefinition BuildRadiantMotes()
        {
            const string name = "RadiantMotes";

            var effectDescription = EffectDescriptionBuilder
                .Create(MagicMissile.EffectDescription)
                .SetTargetingData(
                    RuleDefinitions.Side.Enemy,
                    RuleDefinitions.RangeType.Distance,
                    18,
                    RuleDefinitions.TargetType.Individuals,
                    5,
                    2,
                    ActionDefinitions.ItemSelectionType.Equiped)
                .AddEffectForm(Shine.EffectDescription.EffectForms[0])
                .SetEffectAdvancement(
                    RuleDefinitions.EffectIncrementMethod.PerAdditionalSlotLevel,
                    1, 2, 0, 0, 0, 0, 0, 0, 0,
                    RuleDefinitions.AdvancementDuration.None)
                .Build();

            var spell = SpellDefinitionBuilder
                .Create(MagicMissile, name, MORWENNA_BASE_GUID)
                .SetGuiPresentation(Category.Spell, Sparkle.GuiPresentation.SpriteReference)
                .SetEffectDescription(effectDescription)
                .AddToDB();

            spell.EffectDescription.EffectForms[0].DamageForm.SetDieType(RuleDefinitions.DieType.D1);
            spell.EffectDescription.EffectForms[0].DamageForm.SetDamageType(RuleDefinitions.DamageTypeRadiant);

            return spell;
        }

        private static SpellDefinition BuildMule()
        {
            const string name = "Mule";

            var movementAffinity = FeatureDefinitionMovementAffinityBuilder
                .Create("MovementAffinityConditionMule", MORWENNA_BASE_GUID)
                .AddToDB();
            movementAffinity.SetHeavyArmorImmunity(true);
            movementAffinity.SetEncumbranceImmunity(true);

            var equipmentAffinity = FeatureDefinitionEquipmentAffinityBuilder
                .Create("EquipmentAffinityConditionMule", MORWENNA_BASE_GUID)
                .AddToDB();
            equipmentAffinity.SetAdditionalCarryingCapacity(20);

            var effectDescription = EffectDescriptionBuilder
                .Create()
                .SetTargetingData(
                    RuleDefinitions.Side.Ally,
                    RuleDefinitions.RangeType.Touch,
                    0,
                    RuleDefinitions.TargetType.Individuals,
                    1,
                    2, ActionDefinitions.ItemSelectionType.Equiped)
                .SetDurationData(
                    RuleDefinitions.DurationType.Hour,
                    8,
                    RuleDefinitions.TurnOccurenceType.EndOfTurn)
                .SetParticleEffectParameters(ExpeditiousRetreat.EffectDescription.EffectParticleParameters)
                .AddEffectForm(
                    EffectFormBuilder
                    .Create()
                    .SetConditionForm(
                        ConditionDefinitionBuilder
                            .Create("ConditionMule", MORWENNA_BASE_GUID)
                            .SetGuiPresentation(Category.Condition, Longstrider.GuiPresentation.SpriteReference)
                            .SetConditionType(RuleDefinitions.ConditionType.Beneficial)
                            .SetFeatures(movementAffinity, equipmentAffinity)
                            .AddToDB(),
                        ConditionForm.ConditionOperation.Add,
                        false,
                        false,
                        ConditionJump.AdditionalCondition)
                    .Build())
                .Build();

            var spell = SpellDefinitionBuilder
                .Create(name, MORWENNA_BASE_GUID)
                .SetGuiPresentation(Category.Spell, Longstrider.GuiPresentation.SpriteReference)
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
                .SetSpellLevel(1)
                .SetCastingTime(RuleDefinitions.ActivationTime.Action)
                .SetConcentrationAction(ActionDefinitions.ActionParameter.None)
                .SetMaterialComponent(RuleDefinitions.MaterialComponentType.Mundane)
                .SetRequiresConcentration(false)
                .SetSomaticComponent(true)
                .SetVerboseComponent(true)
                .SetEffectDescription(effectDescription)
                .AddToDB();

            return spell;
        }
    }
}
