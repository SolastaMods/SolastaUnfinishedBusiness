using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
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
            .SetEffectForms(EffectFormBuilder.Create()
                .SetConditionForm(condition, ConditionForm.ConditionOperation.Add)
                .Build())
            .SetParticleEffectParameters(Blur)
            .Build();

        return spell;
    }

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
            .SetEffectDescription(EffectDescriptionBuilder
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
            .SetEffectDescription(EffectDescriptionBuilder
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
                .SetConditionForm(conditionShadowBlade, ConditionForm.ConditionOperation.Add)
                .Build());

        var damageForm = weaponDescription.EffectDescription.FindFirstDamageForm();

        damageForm.damageType = DamageTypePsychic;
        damageForm.dieType = DieType.D8;
        damageForm.diceNumber = 2;

        var spell = SpellDefinitionBuilder
            .Create(FlameBlade, NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite("", Resources.ShadeBlade, 128))
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

    #if false
    #region Web

    internal static SpellDefinition BuildWeb()
    {
        var feature = FeatureDefinitionBuilder
            .Create("FeatureOnRoundStartWeb")
            .AddToDB();

        var condition = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionRestrained, "ConditionRestrainedBySpellWeb")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .AddFeatures(feature)
            .AddToDB();

        feature.SetCustomSubFeatures(new CharacterTurnStartListenerWeb(condition));

        var spell = SpellDefinitionBuilder
            .Create(Entangle, "SpellWeb")
            .SetGuiPresentation(Category.Spell)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
            .SetVocalSpellSameType(VocalSpellSemeType.Debuff)
            .SetSpellLevel(2)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create(Entangle.EffectDescription)
                .SetTargetingData(Side.All, RangeType.Distance, 12, TargetType.Cube, 2, 1)
                .SetDurationData(DurationType.Hour, 1)
                .SetRecurrentEffect(RecurrentEffect.OnActivation | RecurrentEffect.OnEnter)
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
                        .SetConditionForm(condition, ConditionForm.ConditionOperation.Add)
                        .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.EndOfTurn, true)
                        .Build())
                .Build())
            .SetRequiresConcentration(false)
            .AddToDB();

        return spell;
    }

    private sealed class CharacterTurnStartListenerWeb : ICharacterTurnStartListener
    {
        private readonly ConditionDefinition _conditionDefinition;

        public CharacterTurnStartListenerWeb(ConditionDefinition conditionDefinition)
        {
            _conditionDefinition = conditionDefinition;
        }

        public void OnCharacterTurnStarted(GameLocationCharacter locationCharacter)
        {
            var rulesetCharacter = locationCharacter.RulesetCharacter;
            var rulesetCondition =
                rulesetCharacter.AllConditions.FirstOrDefault(x => x.ConditionDefinition == _conditionDefinition);
            var actionTypeStatus = locationCharacter.GetActionTypeStatus(
                ActionDefinitions.ActionType.Main, ActionDefinitions.ActionScope.Battle, true);

            if (rulesetCondition == null || actionTypeStatus != ActionDefinitions.ActionStatus.Available)
            {
                return;
            }

            var gameLocationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
            var locationCharacterAttacker = gameLocationCharacterService.PartyCharacters
                .FirstOrDefault(x => x.Guid == rulesetCondition.SourceGuid);
            var checkDC = locationCharacterAttacker.RulesetCharacter.SpellRepertoires
                .Select(x => x.SaveDC)
                .Max();

            locationCharacter.RollAbilityCheck(
                AttributeDefinitions.Strength, string.Empty, checkDC, AdvantageType.None,
                new ActionModifier(), false, -1, out var outcome, out _, true);

            if (outcome is RollOutcome.Failure or RollOutcome.CriticalFailure)
            {
                return;
            }

            locationCharacter.currentActionRankByType.TryAdd(ActionDefinitions.ActionType.Main, 1);
            locationCharacterAttacker.ActionsRefreshed?.Invoke(locationCharacter);
        }
    }

    #endregion
#endif
}
