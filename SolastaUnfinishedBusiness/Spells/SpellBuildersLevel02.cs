using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
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
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterFamilyDefinitions;
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
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(2)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(false)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
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
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
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
            .AddCustomSubFeatures(MirrorImageLogic.DuplicateProvider.Mark)
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
            .SetSpellLevel(2)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
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
            .SetSpellLevel(2)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Debuff)
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
            .SetSpellLevel(2)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Debuff)
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

    #region Noxious Spray

    internal static SpellDefinition BuildNoxiousSpray()
    {
        const string NAME = "NoxiousSpray";

        var actionAffinityNoxiousSpray = FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{NAME}")
            .SetGuiPresentationNoContent(true)
            .SetAllowedActionTypes(false, move: false)
            .AddToDB();

        var conditionNoxiousSpray = ConditionDefinitionBuilder
            .Create(ConditionPheromoned, $"Condition{NAME}")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionDiseased)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .SetFeatures(actionAffinityNoxiousSpray)
            .AddToDB();

        conditionNoxiousSpray.specialDuration = false;

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.NoxiousSpray, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(2)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Enemy, RangeType.RangeHit, 12, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Constitution, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel,
                        additionalTargetsPerIncrement: 1)
                    .AddImmuneCreatureFamilies(Construct, Elemental, Undead)
                    .SetEffectForms(
                        EffectFormBuilder.DamageForm(DamageTypePoison, 4, DieType.D6),
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(conditionNoxiousSpray, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .SetParticleEffectParameters(PowerDomainOblivionMarkOfFate)
                    .Build())
            .AddToDB();

        spell.EffectDescription.EffectParticleParameters.casterParticleReference =
            PoisonSpray.EffectDescription.EffectParticleParameters.casterParticleReference;

        return spell;
    }

    #endregion

#if false
//Spell/&CloudOfDaggersDescription=You fill the air with spinning daggers in a cube 5 feet on each side, centered on a point you choose within range. A creature takes 4d4 slashing damage when it enters the spell's area for the first time on a turn or starts its turn there. When you cast this spell using a spell slot of 3rd level or higher, the damage increases by 2d4 for each slot level above 2nd.
//Spell/&CloudOfDaggersTitle=Cloud of Daggers

    #region Cloud of Daggers

    internal static SpellDefinition BuildCloudOfDaggers()
    {
        const string Name = "CloudOfDaggers";

        var proxy = EffectProxyDefinitionBuilder
            .Create($"Proxy{Name}")
            .SetGuiPresentation(Name, Category.Spell)
            .AddToDB();

        proxy.prefabReference = EffectProxyDefinitions.ProxyDancingLights.prefabReference;

        var spell = SpellDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(Name, Resources.CloudOfDaggers, 128, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
            .SetSpellLevel(2)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Debuff)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.All, RangeType.Distance, 12, TargetType.Cube)
                    .SetEffectAdvancement(
                        EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 2)
                    .SetRecurrentEffect(RecurrentEffect.OnTurnStart | RecurrentEffect.OnEnter)
                    .SetEffectForms(
                        EffectFormBuilder.DamageForm(DamageTypeSlashing, 4, DieType.D4),
                        EffectFormBuilder
                            .Create()
                            .SetSummonEffectProxyForm(proxy)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetTopologyForm(TopologyForm.Type.DangerousZone, true)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetTopologyForm(TopologyForm.Type.SightImpaired, true)
                            .Build())
                    .SetParticleEffectParameters(ShadowDagger)
                    .Build())
            .AddToDB();

        return spell;
    }

    #endregion
#endif

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
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolIllusion)
            .SetSpellLevel(2)
            .SetCastingTime(ActivationTime.BonusAction)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
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

        var conditionShadowBlade = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        conditionShadowBlade.AddCustomSubFeatures(
            new ModifyAttackActionModifierShadowBlade(itemShadowBlade, conditionShadowBlade));

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
        private readonly BaseDefinition _featureAdvantage;
        private readonly ItemDefinition _itemShadowBlade;

        public ModifyAttackActionModifierShadowBlade(ItemDefinition itemShadowBlade, BaseDefinition featureAdvantage)
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
                new TrendInfo(1, FeatureSourceType.Condition, _featureAdvantage.Name, _featureAdvantage));
        }
    }

    #endregion

    #region Psychic Whip

    internal static SpellDefinition BuildPsychicWhip()
    {
        const string NAME = "PsychicWhip";

        var actionAffinityPsychicWhipNoBonus = FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{NAME}NoBonus")
            .SetGuiPresentationNoContent(true)
            .SetAllowedActionTypes(bonus: false)
            .AddToDB();

        var conditionPsychicWhipNoBonus = ConditionDefinitionBuilder
            .Create($"Condition{NAME}NoBonus")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialDuration()
            .SetFeatures(actionAffinityPsychicWhipNoBonus)
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddToDB();

        var actionAffinityPsychicWhipNoMove = FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{NAME}NoMove")
            .SetGuiPresentationNoContent(true)
            .SetAllowedActionTypes(move: false)
            .AddToDB();

        var conditionPsychicWhipNoMove = ConditionDefinitionBuilder
            .Create($"Condition{NAME}NoMove")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialDuration()
            .SetFeatures(actionAffinityPsychicWhipNoMove)
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddToDB();

        var actionAffinityPsychicWhipNoMain = FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{NAME}NoMain")
            .SetGuiPresentationNoContent(true)
            .SetAllowedActionTypes(false)
            .AddToDB();

        var conditionPsychicWhipNoMain = ConditionDefinitionBuilder
            .Create($"Condition{NAME}NoMain")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialDuration()
            .SetFeatures(actionAffinityPsychicWhipNoMain)
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddToDB();

        var actionAffinityPsychicWhipNoReaction = FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{NAME}NoReaction")
            .SetGuiPresentationNoContent(true)
            .SetAllowedActionTypes(reaction: false)
            .AddToDB();

        var conditionPsychicWhipNoReaction = ConditionDefinitionBuilder
            .Create(ConditionConfused, $"Condition{NAME}NoReaction")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .SetFeatures(actionAffinityPsychicWhipNoReaction)
            .AddToDB();

        conditionPsychicWhipNoReaction.AddCustomSubFeatures(new ActionFinishedByMePsychicWhip(
            conditionPsychicWhipNoBonus,
            conditionPsychicWhipNoMain,
            conditionPsychicWhipNoMove,
            conditionPsychicWhipNoReaction));

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.PsychicWhip, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEnchantment)
            .SetSpellLevel(2)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(false)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Defense)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 18, TargetType.IndividualsUnique)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel,
                        additionalTargetsPerIncrement: 1)
                    .SetSavingThrowData(false, AttributeDefinitions.Intelligence, true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypePsychic, 3, DieType.D6)
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionPsychicWhipNoReaction, ConditionForm.ConditionOperation.Add)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .SetParticleEffectParameters(GravitySlam)
                    .Build())
            .AddToDB();

        return spell;
    }

    private sealed class ActionFinishedByMePsychicWhip : IActionFinishedByMe
    {
        private readonly ConditionDefinition _conditionNoBonus;
        private readonly ConditionDefinition _conditionNoMain;
        private readonly ConditionDefinition _conditionNoMove;
        private readonly ConditionDefinition _conditionNoReaction;

        public ActionFinishedByMePsychicWhip(
            ConditionDefinition conditionNoBonus,
            ConditionDefinition conditionNoMain,
            ConditionDefinition conditionNoMove,
            ConditionDefinition conditionNoReaction)
        {
            _conditionNoBonus = conditionNoBonus;
            _conditionNoMain = conditionNoMain;
            _conditionNoMove = conditionNoMove;
            _conditionNoReaction = conditionNoReaction;
        }

        public IEnumerator OnActionFinishedByMe(CharacterAction characterAction)
        {
            var actionType = characterAction.ActionType;
            var conditions = new List<ConditionDefinition>();

            switch (actionType)
            {
                case ActionDefinitions.ActionType.Main:
                    conditions.Add(_conditionNoMove);
                    conditions.Add(_conditionNoBonus);
                    break;
                case ActionDefinitions.ActionType.Bonus:
                    conditions.Add(_conditionNoMain);
                    conditions.Add(_conditionNoMove);
                    break;
                case ActionDefinitions.ActionType.Move:
                    conditions.Add(_conditionNoBonus);
                    conditions.Add(_conditionNoMain);
                    break;
                case ActionDefinitions.ActionType.FreeOnce:
                case ActionDefinitions.ActionType.Reaction:
                case ActionDefinitions.ActionType.NoCost:
                case ActionDefinitions.ActionType.Max:
                case ActionDefinitions.ActionType.None:
                default:
                    break;
            }

            if (characterAction.ActingCharacter.RulesetCharacter is not
                { IsDeadOrDyingOrUnconscious: false } rulesetCharacter)
            {
                yield break;
            }

            if (!rulesetCharacter.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect,
                    _conditionNoReaction.Name,
                    out var activeCondition))
            {
                yield break;
            }

            var caster = EffectHelpers.GetCharacterByGuid(activeCondition.SourceGuid);

            if (caster is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            if (characterAction.ActingCharacter.RulesetCharacter is
                { IsDeadOrDyingOrUnconscious: false })
            {
                conditions.ForEach(condition =>
                    rulesetCharacter.InflictCondition(
                        condition.Name,
                        condition.DurationType,
                        condition.DurationParameter,
                        condition.TurnOccurence,
                        AttributeDefinitions.TagCombat,
                        caster.guid,
                        caster.CurrentFaction.Name,
                        0,
                        null,
                        0,
                        0,
                        0));
            }
        }
    }

    #endregion
}
