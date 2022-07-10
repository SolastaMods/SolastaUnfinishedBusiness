using System;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using static SolastaCommunityExpansion.Models.SpellsContext;
using static SolastaCommunityExpansion.Api.DatabaseHelper;
using static SolastaCommunityExpansion.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.SpellDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.SpellListDefinitions;

namespace SolastaCommunityExpansion.Spells;

public static class SgSpells
{
    private static readonly Guid SgBaseGuid = new("56f97707-9af1-4761-987a-63b84ba18d46");

    private static readonly SpellDefinition IlluminatingSphere = BuildIlluminatingSphere();
    private static readonly SpellDefinition RadiantMotes = BuildRadiantMotes();
    private static readonly SpellDefinition Mule = BuildMule();

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

    [NotNull]
    private static SpellDefinition BuildIlluminatingSphere()
    {
        const string NAME = "IlluminatingSphere";

        var spell = SpellDefinitionBuilder
            .Create(Sparkle, NAME, SgBaseGuid)
            .SetGuiPresentation(Category.Spell, Shine.GuiPresentation.SpriteReference)
            .AddToDB();

        spell.EffectDescription.SetRangeType(RuleDefinitions.RangeType.Distance);
        spell.EffectDescription.SetRangeParameter(18);
        spell.EffectDescription.SetTargetType(RuleDefinitions.TargetType.Sphere);
        spell.EffectDescription.SetTargetParameter(5);
        spell.EffectDescription.SetEffectParticleParameters(
            SacredFlame_B.EffectDescription.EffectParticleParameters);

        return spell;
    }

    [NotNull]
    private static SpellDefinition BuildRadiantMotes()
    {
        const string NAME = "RadiantMotes";

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
                1, 2)
            .Build();

        var spell = SpellDefinitionBuilder
            .Create(MagicMissile, NAME, SgBaseGuid)
            .SetGuiPresentation(Category.Spell, Sparkle.GuiPresentation.SpriteReference)
            .SetEffectDescription(effectDescription)
            .AddToDB();

        spell.EffectDescription.EffectForms[0].DamageForm.dieType = RuleDefinitions.DieType.D1;
        spell.EffectDescription.EffectForms[0].DamageForm.damageType = RuleDefinitions.DamageTypeRadiant;

        return spell;
    }

    private static SpellDefinition BuildMule()
    {
        const string NAME = "Mule";

        var movementAffinity = FeatureDefinitionMovementAffinityBuilder
            .Create("MovementAffinityConditionMule", SgBaseGuid)
            .AddToDB();
        movementAffinity.heavyArmorImmunity = true;
        movementAffinity.encumbranceImmunity = true;

        var equipmentAffinity = FeatureDefinitionEquipmentAffinityBuilder
            .Create("EquipmentAffinityConditionMule", SgBaseGuid)
            .AddToDB();
        equipmentAffinity.additionalCarryingCapacity = 20;

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
                            .Create("ConditionMule", SgBaseGuid)
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
            .Create(NAME, SgBaseGuid)
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
