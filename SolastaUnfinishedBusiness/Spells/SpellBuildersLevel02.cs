using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Spells;

internal static partial class SpellBuilders
{
    #region LEVEL 02

    internal static SpellDefinition BuildPetalStorm()
    {
        const string ProxyPetalStormName = "ProxyPetalStorm";

        _ = EffectProxyDefinitionBuilder
            .Create(EffectProxyDefinitions.ProxyInsectPlague, ProxyPetalStormName)
            .SetGuiPresentation("PetalStorm", Category.Spell, WindWall)
            .SetCanMove()
            .SetIsEmptyPresentation(false)
            .SetCanMoveOnCharacters()
            .SetAttackMethod(ProxyAttackMethod.ReproduceDamageForms)
            .SetActionId(ActionDefinitions.Id.ProxyFlamingSphere)
            .SetPortrait(WindWall.GuiPresentation.SpriteReference)
            .AddAdditionalFeatures(FeatureDefinitionMoveModes.MoveModeMove6)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(InsectPlague, "PetalStorm")
            .SetGuiPresentation(Category.Spell, WindWall)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetSpellLevel(2)
            .AddToDB();

        //TODO: move this into a builder
        var effectDescription = spell.EffectDescription;

        effectDescription.rangeType = RangeType.Distance;
        effectDescription.rangeParameter = 12;
        effectDescription.durationType = DurationType.Minute;
        effectDescription.durationParameter = 1;
        effectDescription.targetType = TargetType.Cube;
        effectDescription.targetParameter = 3;
        effectDescription.hasSavingThrow = true;
        effectDescription.savingThrowAbility = AttributeDefinitions.Strength;
        effectDescription.recurrentEffect = (RecurrentEffect)20;

        effectDescription.EffectAdvancement.additionalDicePerIncrement = 2;
        effectDescription.EffectAdvancement.incrementMultiplier = 1;
        effectDescription.EffectAdvancement.effectIncrementMethod = EffectIncrementMethod.PerAdditionalSlotLevel;

        effectDescription.EffectForms[0].hasSavingThrow = true;
        effectDescription.EffectForms[0].savingThrowAffinity = EffectSavingThrowType.Negates;
        effectDescription.EffectForms[0].DamageForm.diceNumber = 3;
        effectDescription.EffectForms[0].DamageForm.dieType = DieType.D4;
        effectDescription.EffectForms[0].DamageForm.damageType = DamageTypeSlashing;
        effectDescription.EffectForms[0].levelMultiplier = 1;

        effectDescription.EffectForms[2].SummonForm.effectProxyDefinitionName = ProxyPetalStormName;

        return spell;
    }

    [NotNull]
    internal static SpellDefinition BuildProtectThreshold()
    {
        const string ProxyPetalStormName = "ProxyProtectThreshold";

        EffectProxyDefinitionBuilder
            .Create(EffectProxyDefinitions.ProxySpikeGrowth, ProxyPetalStormName)
            .SetOrUpdateGuiPresentation("ProtectThreshold", Category.Spell)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(SpikeGrowth, "ProtectThreshold")
            .SetGuiPresentation(Category.Spell, Bane)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolAbjuration)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVocalSpellSameType(VocalSpellSemeType.Defense)
            .SetSpellLevel(2)
            .SetRequiresConcentration(false)
            .SetRitualCasting(ActivationTime.Minute10)
            .AddToDB();

        //TODO: move this into a builder
        var effectDescription = spell.EffectDescription;

        effectDescription.difficultyClassComputation = EffectDifficultyClassComputation.SpellCastingFeature;
        effectDescription.durationParameter = 10;
        effectDescription.durationType = DurationType.Minute;
        effectDescription.hasSavingThrow = true;
        effectDescription.rangeParameter = 1;
        effectDescription.rangeType = RangeType.Distance;
        effectDescription.recurrentEffect = RecurrentEffect.OnEnter;
        effectDescription.savingThrowAbility = AttributeDefinitions.Wisdom;
        effectDescription.fixedSavingThrowDifficultyClass = 12;
        effectDescription.targetParameter = 0;
        effectDescription.targetType = TargetType.Sphere;

        effectDescription.EffectAdvancement.additionalDicePerIncrement = 1;
        effectDescription.EffectAdvancement.incrementMultiplier = 1;
        effectDescription.EffectAdvancement.effectIncrementMethod = EffectIncrementMethod.PerAdditionalSlotLevel;

        effectDescription.EffectForms[0].SummonForm.effectProxyDefinitionName = ProxyPetalStormName;

        effectDescription.EffectForms[1].hasSavingThrow = true;
        effectDescription.EffectForms[1].savingThrowAffinity = EffectSavingThrowType.HalfDamage;
        effectDescription.EffectForms[1].DamageForm.diceNumber = 4;
        effectDescription.EffectForms[1].DamageForm.dieType = DieType.D6;
        effectDescription.EffectForms[1].DamageForm.damageType = DamageTypePsychic;
        effectDescription.EffectForms[1].levelMultiplier = 1;

        return spell;
    }

    [NotNull]
    internal static SpellDefinition BuildMirrorImage()
    {
        //Use Condition directly, instead of ConditionName to guarantee it gets built
        var condition = ConditionDefinitionBuilder
            .Create("ConditionMirrorImageMark")
            .SetGuiPresentation(MirrorImageLogic.Condition.Name, Category.Condition)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .CopyParticleReferences(ConditionBlurred)
            .SetFeatures(FeatureDefinitionBuilder
                .Create("FeatureMirrorImage")
                .SetGuiPresentation(MirrorImageLogic.Condition.Name, Category.Condition)
                .SetCustomSubFeatures(MirrorImageLogic.DuplicateProvider.Mark)
                .AddToDB())
            .AddToDB();

        var spell = MirrorImage;

        spell.implemented = true;
        spell.uniqueInstance = true;
        spell.schoolOfMagic = SchoolIllusion;
        spell.verboseComponent = true;
        spell.somaticComponent = true;
        spell.vocalSpellSemeType = VocalSpellSemeType.Defense;
        spell.materialComponentType = MaterialComponentType.None;
        spell.castingTime = ActivationTime.Action;
        spell.effectDescription = EffectDescriptionBuilder.Create()
            .SetDurationData(DurationType.Minute, 1)
            .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
            .SetEffectForms(EffectFormBuilder.Create()
                .SetConditionForm(condition, ConditionForm.ConditionOperation.Add, true, false)
                .Build())
            .SetParticleEffectParameters(Blur)
            .Build();

        return spell;
    }

    #endregion
}
