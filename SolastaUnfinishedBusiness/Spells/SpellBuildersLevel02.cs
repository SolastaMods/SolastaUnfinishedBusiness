using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Spells;

internal static partial class SpellBuilders
{
    #region LEVEL 02

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

    [NotNull]
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
        const string NAME = "ProtectThreshold";
        const string ProxyPetalStormName = "ProxyProtectThreshold";

        EffectProxyDefinitionBuilder
            .Create(EffectProxyDefinitions.ProxySpikeGrowth, ProxyPetalStormName)
            .SetOrUpdateGuiPresentation(NAME, Category.Spell)
            .AddToDB();

        var spriteReference = Sprites.GetSprite(NAME, Resources.ProtectThreshold, 128);

        var spell = SpellDefinitionBuilder
            .Create(SpikeGrowth, "ProtectThreshold")
            .SetGuiPresentation(Category.Spell, spriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolAbjuration)
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
    internal static SpellDefinition BuildShadowBlade()
    {
        const string NAME = "ShadowBlade";

        var conditionShadowBlade = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(
                FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon,
                FeatureDefinitionCombatAffinityBuilder
                    .Create($"CombatAffinity{NAME}")
                    .SetGuiPresentation($"Item{NAME}", Category.Item)
                    .SetMyAttackAdvantage(AdvantageType.Advantage)
                    .AddToDB())
            .AddToDB();

        var itemShadowBlade = ItemDefinitionBuilder
            .Create(ItemDefinitions.FlameBlade, $"Item{NAME}")
            .SetOrUpdateGuiPresentation(Category.Item, ItemDefinitions.Enchanted_Dagger_Souldrinker)
            .AddToDB();

        itemShadowBlade.itemTags.SetRange(TagsDefinitions.ItemTagConjured);
        itemShadowBlade.activeTags.Clear();
        itemShadowBlade.isLightSourceItem = false;
        itemShadowBlade.itemPresentation.assetReference = ItemDefinitions.Scimitar.ItemPresentation.AssetReference;

        var weaponDescription = itemShadowBlade.WeaponDescription;

        weaponDescription.closeRange = 4;
        weaponDescription.reachRange = 12;
        weaponDescription.weaponType = WeaponTypeDefinitions.DaggerType.Name;
        weaponDescription.weaponTags.Add(TagsDefinitions.WeaponTagThrown);

        weaponDescription.EffectDescription.EffectForms.Add(
            EffectFormBuilder
                .Create()
                .SetConditionForm(
                    conditionShadowBlade,
                    ConditionForm.ConditionOperation.Add)
                .Build());

        var damageForm = weaponDescription.EffectDescription.FindFirstDamageForm();

        damageForm.damageType = DamageTypePsychic;
        damageForm.dieType = DieType.D8;
        damageForm.diceNumber = 2;

        var spell = SpellDefinitionBuilder
            .Create(FlameBlade, NAME)
            .SetOrUpdateGuiPresentation(Category.Spell, SpiritualWeapon)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolIllusion)
            .AddToDB();

        spell.EffectDescription.durationType = DurationType.Minute;
        spell.EffectDescription.durationParameter = 1;

        var summonForm = spell.EffectDescription.EffectForms[0].SummonForm;
        var itemPropertyForm = spell.EffectDescription.EffectForms[1].ItemPropertyForm;

        summonForm.itemDefinition = itemShadowBlade;
        summonForm.effectProxyDefinitionName = "ProxySpiritualWeapon";
        itemPropertyForm.FeatureBySlotLevel[1].level = 3;
        itemPropertyForm.FeatureBySlotLevel[2].level = 5;
        itemPropertyForm.FeatureBySlotLevel[3].level = 7;

        return spell;
    }

    #endregion
}
