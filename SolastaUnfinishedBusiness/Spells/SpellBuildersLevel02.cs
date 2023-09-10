using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionMovementAffinitys;

namespace SolastaUnfinishedBusiness.Spells;

internal static partial class SpellBuilders
{
    #region Binding Ice

    internal static SpellDefinition BuildBindingIce()
    {
        const string NAME = "BindingIce";

        var spriteReference = Sprites.GetSprite("WinterBreath", Resources.WinterBreath, 128);

        var conditionGrappledRestrainedIceBound = ConditionDefinitionBuilder
            .Create(ConditionGrappledRestrainedRemorhaz, "ConditionGrappledRestrainedIceBound")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetFeatures(MovementAffinityConditionRestrained, ActionAffinityConditionRestrained, ActionAffinityGrappled)
            //.SetParentCondition(ConditionDefinitions.ConditionRestrained)
            .AddToDB();

        conditionGrappledRestrainedIceBound.specialDuration = false;
        conditionGrappledRestrainedIceBound.specialInterruptions.Clear();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, spriteReference)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Cone, 6)
                    .ExcludeCaster()
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                    .SetSavingThrowData(
                        false,
                        AttributeDefinitions.Constitution,
                        true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetParticleEffectParameters(ConeOfCold.EffectDescription.EffectParticleParameters)
                    .AddEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeCold, 3, DieType.D8)
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .Build())
                    .AddEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionGrappledRestrainedIceBound, ConditionForm.ConditionOperation.Add)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .Build())
            .SetCastingTime(ActivationTime.Action)
            .SetSpellLevel(2)
            .SetVerboseComponent(false)
            .SetSomaticComponent(true)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .AddToDB();

        spell.EffectDescription.EffectParticleParameters.conditionParticleReference =
            PowerDomainElementalHeraldOfTheElementsCold.EffectDescription.EffectParticleParameters
                .conditionParticleReference;

        spell.EffectDescription.EffectParticleParameters.conditionStartParticleReference =
            PowerDomainElementalHeraldOfTheElementsCold.EffectDescription.EffectParticleParameters
                .conditionStartParticleReference;

        spell.EffectDescription.EffectParticleParameters.conditionEndParticleReference =
            PowerDomainElementalHeraldOfTheElementsCold.EffectDescription.EffectParticleParameters
                .conditionEndParticleReference;

        return spell;
    }

    #endregion

    #region Color Burst

    internal static SpellDefinition BuildColorBurst()
    {
        const string NAME = "ColorBurst";

        var spell = SpellDefinitionBuilder
            .Create(ColorSpray, NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.ColorBurst, 128))
            .SetSpellLevel(2)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(ColorSpray)
                    .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Cube, 5)
                    .ExcludeCaster()
                    .SetParticleEffectParameters(HypnoticPattern)
                    .Build())
            .AddToDB();

        spell.EffectDescription.EffectParticleParameters.impactParticleReference =
            spell.EffectDescription.EffectParticleParameters.zoneParticleReference;
        spell.EffectDescription.EffectParticleParameters.zoneParticleReference = new AssetReference();

        return spell;
    }

    #endregion

    #region Mirror Image

    [NotNull]
    internal static SpellDefinition BuildMirrorImage()
    {
        //Use Condition directly, instead of ConditionName to guarantee it gets built
        var condition = ConditionDefinitionBuilder
            .Create("ConditionMirrorImageMark")
            .SetGuiPresentation(MirrorImageLogic.Condition.Name, Category.Condition)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .CopyParticleReferences(ConditionBlurred)
            .SetFeatures(
                FeatureDefinitionBuilder
                    .Create("FeatureMirrorImage")
                    .SetGuiPresentation(MirrorImageLogic.Condition.Name, Category.Condition)
                    .SetCustomSubFeatures(MirrorImageLogic.DuplicateProvider.Mark)
                    .AddToDB())
            .AddToDB();

        var spell = MirrorImage;

        spell.contentPack = CeContentPackContext.CeContentPack; // required otherwise it FUP spells UI
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
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetConditionForm(condition, ConditionForm.ConditionOperation.Add)
                    .Build())
            .SetParticleEffectParameters(Blur)
            .Build();

        return spell;
    }

    #endregion

    #region Petal Storm

    [NotNull]
    internal static SpellDefinition BuildPetalStorm()
    {
        const string NAME = "PetalStorm";
        const string ProxyPetalStormName = $"Proxy{NAME}";

        var sprite = Sprites.GetSprite(NAME, Resources.PetalStorm, 128);

        _ = EffectProxyDefinitionBuilder
            .Create(EffectProxyDefinitions.ProxyInsectPlague, ProxyPetalStormName)
            .SetGuiPresentation(NAME, Category.Spell, sprite)
            .SetCanMove()
            .SetIsEmptyPresentation(false)
            .SetCanMoveOnCharacters()
            .SetAttackMethod(ProxyAttackMethod.ReproduceDamageForms)
            .SetActionId(ActionDefinitions.Id.ProxyFlamingSphere)
            .SetPortrait(WindWall.GuiPresentation.SpriteReference)
            .AddAdditionalFeatures(FeatureDefinitionMoveModes.MoveModeMove6)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(InsectPlague, NAME)
            .SetGuiPresentation(Category.Spell, sprite)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetSpellLevel(2)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(InsectPlague.EffectDescription)
                    .SetTargetingData(Side.All, RangeType.Distance, 12, TargetType.Cube, 3)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 2)
                    .SetRecurrentEffect(RecurrentEffect.OnTurnStart | RecurrentEffect.OnEnter)
                    .SetSavingThrowData(
                        false,
                        AttributeDefinitions.Strength,
                        false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .Build())
            .AddToDB();

        //TODO: move this into a builder
        var effectDescription = spell.EffectDescription;

        effectDescription.EffectForms[0].hasSavingThrow = true;
        effectDescription.EffectForms[0].savingThrowAffinity = EffectSavingThrowType.Negates;
        effectDescription.EffectForms[0].DamageForm.diceNumber = 3;
        effectDescription.EffectForms[0].DamageForm.dieType = DieType.D4;
        effectDescription.EffectForms[0].DamageForm.damageType = DamageTypeSlashing;
        effectDescription.EffectForms[0].levelMultiplier = 1;
        effectDescription.EffectForms[2].SummonForm.effectProxyDefinitionName = ProxyPetalStormName;

        return spell;
    }

    #endregion

    #region Protect Threshold

    [NotNull]
    internal static SpellDefinition BuildProtectThreshold()
    {
        const string NAME = "ProtectThreshold";
        const string ProxyPetalStormName = "ProxyProtectThreshold";

        EffectProxyDefinitionBuilder
            .Create(EffectProxyDefinitions.ProxyGuardianOfFaith, ProxyPetalStormName)
            .SetOrUpdateGuiPresentation(NAME, Category.Spell)
            .AddToDB();

        var spriteReference = Sprites.GetSprite(NAME, Resources.ProtectThreshold, 128);

        var spell = SpellDefinitionBuilder
            .Create(GuardianOfFaith, "ProtectThreshold")
            .SetGuiPresentation(Category.Spell, spriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolAbjuration)
            .SetVocalSpellSameType(VocalSpellSemeType.Defense)
            .SetSpellLevel(2)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(SpikeGrowth.EffectDescription)
                    .SetTargetingData(Side.All, RangeType.Distance, 6, TargetType.Sphere, 3)
                    .SetDurationData(DurationType.Minute, 10)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                    .SetRecurrentEffect(RecurrentEffect.OnEnter)
                    .SetSavingThrowData(
                        false,
                        AttributeDefinitions.Wisdom,
                        false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .Build())
            .SetRequiresConcentration(false)
            .SetRitualCasting(ActivationTime.Minute10)
            .AddToDB();

        //TODO: move this into a builder
        var effectDescription = spell.EffectDescription;

        effectDescription.EffectForms[0].SummonForm.effectProxyDefinitionName = ProxyPetalStormName;
        effectDescription.EffectForms[1].hasSavingThrow = true;
        effectDescription.EffectForms[1].savingThrowAffinity = EffectSavingThrowType.HalfDamage;
        effectDescription.EffectForms[1].DamageForm.diceNumber = 4;
        effectDescription.EffectForms[1].DamageForm.dieType = DieType.D6;
        effectDescription.EffectForms[1].DamageForm.damageType = DamageTypePsychic;
        effectDescription.EffectForms[1].levelMultiplier = 1;

        return spell;
    }

    #endregion

    #region Web

    internal static SpellDefinition BuildWeb()
    {
        var conditionRestrainedBySpellWeb = ConditionDefinitionBuilder
            .Create(ConditionGrappledRestrainedRemorhaz, "ConditionGrappledRestrainedSpellWeb")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetParentCondition(ConditionRestrainedByWeb)
            //.SetSpecialDuration(DurationType.Round, 1)
            //.SetParentCondition(ConditionDefinitions.ConditionRestrained)
            .AddToDB();

        conditionRestrainedBySpellWeb.specialDuration = false;
        conditionRestrainedBySpellWeb.specialInterruptions.Clear();

        var conditionAffinityGrappledRestrainedSpellWebImmunity = FeatureDefinitionConditionAffinityBuilder
            .Create("ConditionAffinityGrappledRestrainedSpellWebImmunity")
            .SetGuiPresentationNoContent(true)
            .SetConditionType(conditionRestrainedBySpellWeb)
            .SetConditionAffinityType(ConditionAffinityType.Immunity)
            .AddToDB();

        foreach (var monsterDefinition in DatabaseRepository.GetDatabase<MonsterDefinition>()
                     .Where(x => x.Name.Contains("Spider") || x.Name.Contains("spider")))
        {
            monsterDefinition.Features.Add(conditionAffinityGrappledRestrainedSpellWebImmunity);
        }

        var spell = SpellDefinitionBuilder
            .Create("SpellWeb")
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite("SpellWeb", Resources.Web, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
            .SetVocalSpellSameType(VocalSpellSemeType.Debuff)
            .SetSpellLevel(2)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(Grease)
                    .SetTargetingData(Side.All, RangeType.Distance, 12, TargetType.Cube, 4, 1)
                    .SetDurationData(DurationType.Hour, 1)
                    .SetRecurrentEffect(RecurrentEffect.OnTurnStart | RecurrentEffect.OnEnter)
                    .SetSavingThrowData(
                        false,
                        AttributeDefinitions.Dexterity,
                        false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        Entangle.EffectDescription.EffectForms[0],
                        Entangle.EffectDescription.EffectForms[1],
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionRestrainedBySpellWeb, ConditionForm.ConditionOperation.Add)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .Build())
            .AddToDB();

        spell.EffectDescription.EffectParticleParameters.conditionParticleReference =
            Entangle.EffectDescription.EffectParticleParameters.conditionParticleReference;

        spell.EffectDescription.EffectParticleParameters.conditionStartParticleReference =
            Entangle.EffectDescription.EffectParticleParameters.conditionStartParticleReference;

        spell.EffectDescription.EffectParticleParameters.conditionEndParticleReference =
            Entangle.EffectDescription.EffectParticleParameters.conditionEndParticleReference;

        return spell;
    }

    #endregion

    #region Shadowblade

    [NotNull]
    internal static SpellDefinition BuildShadowBlade()
    {
        const string NAME = "ShadowBlade";

        var itemShadowBlade = ItemDefinitionBuilder
            .Create(ItemDefinitions.FlameBlade, $"Item{NAME}")
            .SetOrUpdateGuiPresentation(Category.Item, ItemDefinitions.Enchanted_Dagger_Souldrinker)
            .SetItemTags(TagsDefinitions.ItemTagConjured)
            .MakeMagical()
            .AddToDB();

        itemShadowBlade.activeTags.Clear();
        itemShadowBlade.isLightSourceItem = false;
        itemShadowBlade.itemPresentation.assetReference = ItemDefinitions.ScimitarPlus2.ItemPresentation.AssetReference;
        itemShadowBlade.weaponDefinition.EffectDescription.EffectParticleParameters.impactParticleReference =
            EffectProxyDefinitions.ProxyArcaneSword.attackImpactParticle;

        var weaponDescription = itemShadowBlade.WeaponDescription;

        weaponDescription.closeRange = 4;
        weaponDescription.maxRange = 12;
        weaponDescription.weaponType = WeaponTypeDefinitions.DaggerType.Name;
        weaponDescription.weaponTags.Add(TagsDefinitions.WeaponTagThrown);

        var damageForm = weaponDescription.EffectDescription.FindFirstDamageForm();

        damageForm.damageType = DamageTypePsychic;
        damageForm.dieType = DieType.D8;
        damageForm.diceNumber = 2;

        var spell = SpellDefinitionBuilder
            .Create(FlameBlade, NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.ShadeBlade, 128))
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolIllusion)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(FlameBlade)
                    .SetDurationData(DurationType.Minute, 1)
                    .Build())
            .AddToDB();

        var summonForm = spell.EffectDescription.EffectForms[0].SummonForm;
        summonForm.itemDefinition = itemShadowBlade;

        var itemPropertyForm = spell.EffectDescription.EffectForms[1].ItemPropertyForm;
        itemPropertyForm.featureBySlotLevel.Clear();
        itemPropertyForm.featureBySlotLevel.Add(BuildShadowBladeFeatureBySlotLevel(2, 0));
        itemPropertyForm.featureBySlotLevel.Add(BuildShadowBladeFeatureBySlotLevel(3, 1));
        itemPropertyForm.featureBySlotLevel.Add(BuildShadowBladeFeatureBySlotLevel(5, 2));
        itemPropertyForm.featureBySlotLevel.Add(BuildShadowBladeFeatureBySlotLevel(7, 3));

        var featureAdvantage = FeatureDefinitionBuilder
            .Create($"Feature{NAME}")
            .SetGuiPresentation(NAME, Category.Spell)
            .AddToDB();

        featureAdvantage.SetCustomSubFeatures(
            new ModifyAttackActionModifierShadowBlade(itemShadowBlade, featureAdvantage));

        var conditionShadowBlade = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(featureAdvantage)
            .AddToDB();

        spell.EffectDescription.EffectForms.Add(
            EffectFormBuilder
                .Create()
                .SetConditionForm(conditionShadowBlade, ConditionForm.ConditionOperation.Add, true)
                .Build());

        return spell;
    }

    private static FeatureUnlockByLevel BuildShadowBladeFeatureBySlotLevel(int level, int damageDice)
    {
        var attackModifierShadowBladeLevel = FeatureDefinitionAttackModifierBuilder
            .Create(FeatureDefinitionAttackModifiers.AttackModifierFlameBlade2, $"AttackModifierShadowBlade{level}")
            .AddToDB();

        attackModifierShadowBladeLevel.guiPresentation.description
            = damageDice > 0
                ? Gui.Format("Feature/&AttackModifierShadowBladeNDescription", damageDice.ToString())
                : "Feature/&AttackModifierShadowBlade0Description";
        attackModifierShadowBladeLevel.additionalDamageDice = damageDice;
        attackModifierShadowBladeLevel.impactParticleReference =
            ShadowDagger.EffectDescription.EffectParticleParameters.impactParticleReference;
        attackModifierShadowBladeLevel.abilityScoreReplacement = AbilityScoreReplacement.None;
        return new FeatureUnlockByLevel(attackModifierShadowBladeLevel, level);
    }

    private sealed class ModifyAttackActionModifierShadowBlade : IModifyAttackActionModifier
    {
        private readonly FeatureDefinition _featureAdvantage;
        private readonly ItemDefinition _itemShadowBlade;

        public ModifyAttackActionModifierShadowBlade(ItemDefinition itemShadowBlade, FeatureDefinition featureAdvantage)
        {
            _itemShadowBlade = itemShadowBlade;
            _featureAdvantage = featureAdvantage;
        }

        public void OnAttackComputeModifier(
            RulesetCharacter myself,
            RulesetCharacter defender,
            BattleDefinitions.AttackProximity attackProximity,
            RulesetAttackMode attackMode,
            ref ActionModifier attackModifier)
        {
            if (myself is not { IsDeadOrDyingOrUnconscious: false } ||
                defender is not { IsDeadOrDyingOrUnconscious: false })
            {
                return;
            }

            if (attackMode?.SourceDefinition != _itemShadowBlade)
            {
                return;
            }

            if (!ValidatorsCharacter.IsNotInBrightLight(defender))
            {
                return;
            }

            attackModifier.attackAdvantageTrends.Add(
                new TrendInfo(1, FeatureSourceType.CharacterFeature, _featureAdvantage.Name, _featureAdvantage));
        }
    }

    #endregion
}
