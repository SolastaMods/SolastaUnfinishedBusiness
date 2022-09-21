using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static SolastaUnfinishedBusiness.Models.SpellsContext;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellListDefinitions;

namespace SolastaUnfinishedBusiness.Spells;

public static class SgSpells
{
    internal static void Register()
    {
        //cantrip
        RegisterSpell(BuildIlluminatingSphere(), 0, SpellListWizard);

        //level 1
        RegisterSpell(BuildRadiantMotes(), 0, SpellListWizard);
        RegisterSpell(BuildMule(), 0, SpellListWizard);
    }

    [NotNull]
    private static SpellDefinition BuildIlluminatingSphere()
    {
        const string NAME = "IlluminatingSphere";

        var spell = SpellDefinitionBuilder
            .Create(Sparkle, NAME)
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
            .Create(MagicMissile, NAME)
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
            .Create("MovementAffinityConditionMule")
            .AddToDB();
        movementAffinity.heavyArmorImmunity = true;
        movementAffinity.encumbranceImmunity = true;

        var equipmentAffinity = FeatureDefinitionEquipmentAffinityBuilder
            .Create("EquipmentAffinityConditionMule")
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
                            .Create("ConditionMule")
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
            .Create(NAME)
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
