using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Spells;

internal static partial class SpellBuilders
{
    #region LEVEL 05

    internal static SpellDefinition BuildFarStep()
    {
        var condition = ConditionDefinitionBuilder
            .Create("ConditionFarStep")
            .SetGuiPresentation(Category.Condition, ConditionJump)
            .SetCustomSubFeatures(AddUsablePowersFromCondition.Marker)
            .SetSilent(Silent.None)
            .SetPossessive()
            .SetFeatures(CustomActionIdContext.FarStep)
            .AddToDB();

        return SpellDefinitionBuilder
            .Create("FarStep")
            .SetGuiPresentation(Category.Spell, Sprites.SpellFarStep)
            .SetSpellLevel(1)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
            .SetCastingTime(ActivationTime.BonusAction)
            .SetSomaticComponent(false)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetEffectDescription(EffectDescriptionBuilder.Create(CustomActionIdContext.FarStep)
                .SetDurationData(DurationType.Minute, 1)
                .AddEffectForms(EffectFormBuilder.Create()
                    .SetConditionForm(condition, ConditionForm.ConditionOperation.Add, true, true)
                    .Build())
                .Build())
            .AddToDB();
    }

    #endregion
}
