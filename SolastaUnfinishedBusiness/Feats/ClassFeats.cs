using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Classes;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;

namespace SolastaUnfinishedBusiness.Feats;

internal static class ClassFeats
{
    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        var featCallForCharge = BuildCallForCharge();
        var featCunningEscape = BuildCunningEscape();
        var featExpandTheHunt = BuildExpandTheHunt();
        var featExploiter = BuildExploiter();
        var featPoisoner = BuildPoisoner();
        var featSlayTheEnemies = BuildSlayTheEnemies();
        var awakenTheBeastWithinGroup = BuildAwakenTheBeastWithin(feats);
        var blessedSoulGroup = BuildBlessedSoul(feats);
        var closeQuartersGroup = BuildCloseQuarters(feats);
        var hardyGroup = BuildHardy(feats);
        var potentSpellcasterGroup = BuildPotentSpellcaster(feats);
        var primalRageGroup = BuildPrimalRage(feats);

        feats.AddRange(
            featCallForCharge,
            featCunningEscape,
            featExpandTheHunt,
            featExploiter,
            featPoisoner,
            featSlayTheEnemies);

        GroupFeats.FeatGroupAgilityCombat.AddFeats(
            featCunningEscape);

        GroupFeats.FeatGroupSpellCombat.AddFeats(
            potentSpellcasterGroup);

        GroupFeats.FeatGroupSupportCombat.AddFeats(
            featCallForCharge,
            featPoisoner,
            hardyGroup);

        GroupFeats.FeatGroupTools.AddFeats(
            featPoisoner);

        GroupFeats.MakeGroup("FeatGroupClassBound", null,
            featCallForCharge,
            featCunningEscape,
            featExpandTheHunt,
            featExploiter,
            featSlayTheEnemies,
            awakenTheBeastWithinGroup,
            blessedSoulGroup,
            closeQuartersGroup,
            hardyGroup,
            potentSpellcasterGroup,
            primalRageGroup);
    }

    #region Call for Charge

    private static FeatDefinitionWithPrerequisites BuildCallForCharge()
    {
        const string NAME = "FeatCallForCharge";

        var condition = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(NAME, Category.Feat, ConditionDefinitions.ConditionBlessed)
            .SetSpecialInterruptions(ConditionInterruption.Attacks)
            .SetPossessive()
            .SetFeatures(
                FeatureDefinitionMovementAffinityBuilder
                    .Create($"MovementAffinity{NAME}")
                    .SetGuiPresentationNoContent(true)
                    .SetBaseSpeedAdditiveModifier(3)
                    .AddToDB(),
                FeatureDefinitionCombatAffinityBuilder
                    .Create($"CombatAffinity{NAME}")
                    .SetGuiPresentation(NAME, Category.Feat, Gui.NoLocalization)
                    .SetMyAttackAdvantage(AdvantageType.Advantage)
                    .AddToDB())
            .AddToDB();

        condition.GuiPresentation.description = Gui.EmptyContent;

        return FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatCallForCharge")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                FeatureDefinitionPowerBuilder
                    .Create($"Power{NAME}")
                    .SetGuiPresentation(Category.Feature,
                        Sprites.GetSprite("PowerCallForCharge", Resources.PowerCallForCharge, 256, 128))
                    .SetUsesAbilityBonus(ActivationTime.BonusAction, RechargeRate.LongRest,
                        AttributeDefinitions.Charisma)
                    .SetEffectDescription(
                        EffectDescriptionBuilder
                            .Create()
                            .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Sphere, 6)
                            .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                            .SetEffectForms(EffectFormBuilder.ConditionForm(condition))
                            .SetParticleEffectParameters(MagicWeapon)
                            .Build())
                    .AddToDB())
            .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
            .SetValidators(ValidatorsFeat.IsPaladinLevel1)
            .AddToDB();
    }

    #endregion

    #region Blessed Soul

    private static FeatDefinition BuildBlessedSoul(List<FeatDefinition> feats)
    {
        const string Name = "FeatBlessedSoul";

        var blessedSoulCleric = FeatDefinitionWithPrerequisitesBuilder
            .Create($"{Name}Cleric")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierClericChannelDivinityAdd,
                AttributeModifierCreed_Of_Maraike)
            .SetValidators(ValidatorsFeat.IsClericLevel4)
            .SetFeatFamily("BlessedSoul")
            .AddToDB();

        var blessedSoulPaladin = FeatDefinitionWithPrerequisitesBuilder
            .Create($"{Name}Paladin")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierClericChannelDivinityAdd,
                AttributeModifierCreed_Of_Solasta)
            .SetValidators(ValidatorsFeat.IsPaladinLevel4)
            .SetFeatFamily("BlessedSoul")
            .AddToDB();

        feats.AddRange(blessedSoulCleric, blessedSoulPaladin);

        return GroupFeats.MakeGroupWithPreRequisite(
            "FeatGroupBlessedSoul", "BlessedSoul", ValidatorsFeat.IsClericOrPaladinLevel4,
            blessedSoulCleric,
            blessedSoulPaladin);
    }

    #endregion

    #region Primal Rage

    private static FeatDefinition BuildPrimalRage(List<FeatDefinition> feats)
    {
        const string Name = "FeatPrimalRage";

        var primalRageStr = FeatDefinitionWithPrerequisitesBuilder
            .Create($"{Name}Str")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierBarbarianRagePointsAdd,
                AttributeModifierCreed_Of_Einar)
            .SetValidators(ValidatorsFeat.IsBarbarianLevel4)
            .SetFeatFamily(Name)
            .AddToDB();

        var primalRageCon = FeatDefinitionWithPrerequisitesBuilder
            .Create($"{Name}Con")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierBarbarianRagePointsAdd,
                AttributeModifierCreed_Of_Arun)
            .SetValidators(ValidatorsFeat.IsBarbarianLevel4)
            .SetFeatFamily(Name)
            .AddToDB();

        feats.AddRange(primalRageStr, primalRageCon);

        return GroupFeats.MakeGroupWithPreRequisite(
            "FeatGroupPrimalRage", Name, ValidatorsFeat.IsBarbarianLevel4, primalRageStr, primalRageCon);
    }

    #endregion

    #region Expand the Hunt

    private static FeatDefinitionWithPrerequisites BuildExpandTheHunt()
    {
        const string Name = "FeatExpandTheHunt";

        return FeatDefinitionWithPrerequisitesBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Maraike,
                FeatureDefinitionPointPools.PointPoolBackgroundLanguageChoice_one,
                RulesContext.InvocationPoolRangerPreferredEnemy,
                RulesContext.InvocationPoolRangerTerrainType)
            .SetValidators(ValidatorsFeat.IsRangerLevel4)
            .AddToDB();
    }

    #endregion

    #region Poisoner

    private static FeatDefinitionWithPrerequisites BuildPoisoner()
    {
        const string Name = "FeatPoisoner";

        return FeatDefinitionWithPrerequisitesBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                FeatureDefinitionProficiencyBuilder
                    .Create($"Proficiency{Name}")
                    .SetGuiPresentationNoContent(true)
                    .SetProficiencies(ProficiencyType.ToolOrExpertise, PoisonersKitType)
                    .AddToDB(),
                FeatureDefinitionActionAffinityBuilder
                    .Create($"ActionAffinity{Name}")
                    .SetGuiPresentationNoContent(true)
                    .AddCustomSubFeatures(
                        new ValidateDeviceFunctionUse((_, device, _) =>
                            device.UsableDeviceDescription.UsableDeviceTags.Contains("Poison")),
                        new ModifyDamageResistancePoisoner())
                    .SetAuthorizedActions(ActionDefinitions.Id.UseItemBonus)
                    .AddToDB())
            .AddToDB();
    }

    private sealed class ModifyDamageResistancePoisoner : IModifyDamageAffinity
    {
        public void ModifyDamageAffinity(RulesetActor attacker, RulesetActor defender, List<FeatureDefinition> features)
        {
            features.RemoveAll(x =>
                x is IDamageAffinityProvider
                {
                    DamageAffinityType: DamageAffinityType.Resistance,
                    DamageType: DamageTypePoison
                });
        }
    }

    #endregion

    #region Close Quarters

    // close quarters behavior is in CharacterContext Sneak Attack Additional Damage Form modifier

    private const string CloseQuartersName = "CloseQuarters";

    private static (bool result, string output) HasSneakAttack(FeatDefinition feat, RulesetCharacterHero hero)
    {
        var isRogue = hero.GetClassLevel(Rogue) > 0;
        var isSorrAkkath = hero.GetSubclassLevel(Sorcerer, "SorcerousSorrAkkath") > 0;
        var hasSneakAttack = isRogue || isSorrAkkath;

        var guiFormat = Gui.Format("Tooltip/&PreReqMustKnow", "Feature/&RogueSneakAttackTitle");

        return hasSneakAttack ? (true, guiFormat) : (false, Gui.Colorize(guiFormat, Gui.ColorFailure));
    }

    internal static readonly FeatDefinitionWithPrerequisites CloseQuartersDex = FeatDefinitionWithPrerequisitesBuilder
        .Create("FeatCloseQuartersDex")
        .SetGuiPresentation(Category.Feat)
        .SetFeatures(AttributeModifierCreed_Of_Misaye)
        .SetFeatFamily(CloseQuartersName)
        .SetValidators(HasSneakAttack)
        .AddToDB();

    internal static readonly FeatDefinitionWithPrerequisites CloseQuartersInt = FeatDefinitionWithPrerequisitesBuilder
        .Create("FeatCloseQuartersInt")
        .SetGuiPresentation(Category.Feat)
        .SetFeatures(AttributeModifierCreed_Of_Pakri)
        .SetFeatFamily(CloseQuartersName)
        .SetValidators(HasSneakAttack)
        .AddToDB();

    internal static void HandleCloseQuarters(
        GameLocationCharacter attacker,
        RulesetCharacterHero rulesetAttacker,
        GameLocationCharacter defender,
        ref DamageForm damageForm,
        string text = "Feedback/&ChangeSneakDiceDieType")
    {
        if (!attacker.IsWithinRange(defender, 1) ||
            (!rulesetAttacker.TrainedFeats.Contains(CloseQuartersDex) &&
             !rulesetAttacker.TrainedFeats.Contains(CloseQuartersInt)))
        {
            return;
        }

        var title = Gui.Format("Feature/&FeatureCloseQuartersTitle");
        var description = Gui.Format("Feature/&FeatureCloseQuartersDescription");

        rulesetAttacker.LogCharacterActivatesAbility(
            title, text,
            tooltipContent: description, indent: true,
            extra:
            [
                (ConsoleStyleDuplet.ParameterType.AbilityInfo, Gui.FormatDieTitle(DieType.D6)),
                (ConsoleStyleDuplet.ParameterType.AbilityInfo, Gui.FormatDieTitle(DieType.D8))
            ]);

        damageForm.DieType = DieType.D8;
    }

    private static FeatDefinition BuildCloseQuarters(List<FeatDefinition> feats)
    {
        feats.AddRange(CloseQuartersDex, CloseQuartersInt);

        return GroupFeats.MakeGroupWithPreRequisite(
            "FeatGroupCloseQuarters", CloseQuartersName, HasSneakAttack, CloseQuartersDex, CloseQuartersInt);
    }

    #endregion

    #region Exploiter

    private static FeatDefinitionWithPrerequisites BuildExploiter()
    {
        const string Name = "FeatExploiter";

        var featureExploiter = FeatureDefinitionBuilder
            .Create("FeatureExploiter")
            .SetGuiPresentation("FeatExploiter", Category.Feat)
            .AddCustomSubFeatures(new CustomBehaviorFeatExploiter())
            .AddToDB();

        return FeatDefinitionWithPrerequisitesBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Feat)
            .AddFeatures(featureExploiter)
            .SetValidators(ValidatorsFeat.IsRogueLevel4)
            .AddToDB();
    }

    private class CustomBehaviorFeatExploiter : IMagicEffectFinishedByMeOrAlly, IPhysicalAttackFinishedByMeOrAlly
    {
        public IEnumerator OnMagicEffectFinishedByMeOrAlly(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter helper,
            List<GameLocationCharacter> targets)
        {
            var effectDescription = action.actionParams.RulesetEffect.EffectDescription;

            if (effectDescription.RangeType is not (RangeType.MeleeHit or RangeType.RangeHit) ||
                effectDescription.TargetParameter != 1 ||
                action.Countered ||
                action is CharacterActionCastSpell { ExecutionFailed: true })
            {
                yield break;
            }

            var attackRollOutcome = action.AttackRollOutcome;

            yield return HandleReaction(battleManager, attackRollOutcome, attacker, helper, targets);
        }

        public IEnumerator OnPhysicalAttackFinishedByMeOrAlly(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            yield return HandleReaction(battleManager, rollOutcome, attacker, helper, [defender]);
        }

        private static IEnumerator HandleReaction(
            GameLocationBattleManager battleManager,
            RollOutcome attackRollOutcome,
            GameLocationCharacter attacker,
            GameLocationCharacter helper,
            // ReSharper disable once ParameterTypeCanBeEnumerable.Local
            List<GameLocationCharacter> targets)
        {
            if (attackRollOutcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess) ||
                attacker == helper ||
                helper.IsMyTurn())
            {
                yield break;
            }

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var defender in targets)
            {
                if (!helper.CanReact())
                {
                    yield break;
                }

                var (opportunityAttackMode, actionModifier) =
                    helper.GetFirstMeleeModeThatCanAttack(defender, battleManager);

                if (opportunityAttackMode == null)
                {
                    continue;
                }

                yield return helper.MyReactForOpportunityAttack(
                    defender,
                    attacker,
                    opportunityAttackMode,
                    actionModifier,
                    "Exploiter",
                    battleManager: battleManager);
            }
        }
    }

    #endregion

    #region Awaken The Beast Within

    private static FeatDefinition BuildAwakenTheBeastWithin([NotNull] List<FeatDefinition> feats)
    {
        const string NAME = "FeatAwakenTheBeastWithin";

        var awakenTheBeastWithinFeats = AttributeDefinitions.AbilityScoreNames
            .Select(abilityScore => new
            {
                abilityScore,
                attributeModifier = DatabaseRepository.GetDatabase<FeatureDefinitionAttributeModifier>()
                    .FirstOrDefault(x =>
                        x.Name.StartsWith("AttributeModifierCreed") && x.ModifiedAttribute == abilityScore)
            })
            .Select(t =>
                FeatDefinitionWithPrerequisitesBuilder
                    .Create($"{NAME}{t.abilityScore}")
                    .SetGuiPresentation(
                        Gui.Format($"Feat/&{NAME}Title",
                            CultureInfo.CurrentCulture.TextInfo.ToTitleCase(
                                Gui.Localize($"Attribute/&{t.abilityScore}Title").ToLower())),
                        Gui.Format($"Feat/&{NAME}Description", t.abilityScore))
                    .SetFeatures(t.attributeModifier)
                    .SetFeatFamily(NAME)
                    .SetValidators(ValidatorsFeat.IsDruidLevel4)
                    .AddCustomSubFeatures(new ActionFinishedByMeFeatAwakenTheBeastWithin())
                    .AddToDB())
            .Cast<FeatDefinition>()
            .ToArray();

        var awakenTheBeastWithinGroup = GroupFeats.MakeGroupWithPreRequisite(
            "FeatGroupAwakenTheBeastWithin", NAME, ValidatorsFeat.IsDruidLevel4, awakenTheBeastWithinFeats);

        feats.AddRange(awakenTheBeastWithinFeats);

        return awakenTheBeastWithinGroup;
    }

    internal sealed class ActionFinishedByMeFeatAwakenTheBeastWithin : IActionFinishedByMe
    {
        // A towel is just about the most massively useful thing an interstellar hitchhiker can carry
        private const ulong TemporaryHitPointsGuid = 42424242;

        public IEnumerator OnActionFinishedByMe(CharacterAction action)
        {
            var actingCharacter = action.ActingCharacter;

            if (action is not CharacterActionRevertShape ||
                actingCharacter.RulesetCharacter is not RulesetCharacterMonster rulesetCharacterMonster)
            {
                yield break;
            }

            var rulesetCondition =
                rulesetCharacterMonster.ConditionsByCategory
                    .SelectMany(x => x.Value)
                    .FirstOrDefault(x =>
                        x.SourceGuid == TemporaryHitPointsGuid);

            if (rulesetCondition != null)
            {
                rulesetCharacterMonster.RemoveCondition(rulesetCondition);
            }
        }

        // ReSharper disable once InconsistentNaming
        internal static void GrantTempHP(RulesetCharacterMonster __instance)
        {
            if (__instance.OriginalFormCharacter is not RulesetCharacterHero rulesetCharacterHero ||
                !rulesetCharacterHero.TrainedFeats.Exists(x => x.Name.StartsWith("FeatAwakenTheBeastWithin")))
            {
                return;
            }

            var classLevel = rulesetCharacterHero.GetClassLevel(Druid);

            __instance.ReceiveTemporaryHitPoints(
                2 * classLevel, DurationType.UntilAnyRest, 0, TurnOccurenceType.StartOfTurn, TemporaryHitPointsGuid);
        }
    }

    #endregion

    #region Cunning Escape

    private static FeatDefinitionWithPrerequisites BuildCunningEscape()
    {
        return FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatCunningEscape")
            .SetGuiPresentation(Category.Feat)
            .AddCustomSubFeatures(new ActionFinishedByMeFeatCunningEscape())
            .SetValidators(ValidatorsFeat.IsRogueLevel4)
            .AddToDB();
    }

    private class ActionFinishedByMeFeatCunningEscape : IActionFinishedByMe
    {
        public IEnumerator OnActionFinishedByMe(CharacterAction action)
        {
            if (action.ActionId != ActionDefinitions.Id.DashBonus)
            {
                yield break;
            }

            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;

            rulesetCharacter.InflictCondition(
                ConditionDisengaging,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                // all disengaging in game is set under TagCombat (why?)
                AttributeDefinitions.TagCombat,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                ConditionDisengaging,
                0,
                0,
                0);
        }
    }

    #endregion

    #region Hardy

    private static FeatDefinition BuildHardy(List<FeatDefinition> feats)
    {
        const string Name = "FeatHardy";

        var feature = FeatureDefinitionBuilder
            .Create($"Feature{Name}")
            .SetGuiPresentationNoContent(true)
            .AddCustomSubFeatures(new UsePowerFinishedByMeFeatHardy())
            .AddToDB();

        var hardyStr = FeatDefinitionWithPrerequisitesBuilder
            .Create($"{Name}Str")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                feature,
                AttributeModifierCreed_Of_Einar)
            .SetValidators(ValidatorsFeat.IsFighterLevel4)
            .SetFeatFamily(Name)
            .AddToDB();

        var hardyCon = FeatDefinitionWithPrerequisitesBuilder
            .Create($"{Name}Con")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                feature,
                AttributeModifierCreed_Of_Arun)
            .SetValidators(ValidatorsFeat.IsFighterLevel4)
            .SetFeatFamily(Name)
            .AddToDB();

        feats.AddRange(hardyStr, hardyCon);

        return GroupFeats.MakeGroupWithPreRequisite(
            "FeatGroupHardy", Name, ValidatorsFeat.IsFighterLevel4, hardyStr, hardyCon);
    }

    private sealed class UsePowerFinishedByMeFeatHardy : IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(
            CharacterAction action,
            GameLocationCharacter attacker,
            List<GameLocationCharacter> targets)
        {
            if (action.ActionParams.RulesetEffect.SourceDefinition != PowerFighterSecondWind)
            {
                yield break;
            }

            var rulesetCharacter = attacker.RulesetCharacter;
            var classLevel = rulesetCharacter.GetClassLevel(Fighter);
            var dieRoll =
                rulesetCharacter.RollDie(DieType.D10, RollContext.None, false, AdvantageType.None, out _, out _);
            var healingReceived = classLevel + dieRoll;

            rulesetCharacter.ReceiveTemporaryHitPoints(
                healingReceived, DurationType.UntilAnyRest, 0, TurnOccurenceType.StartOfTurn, rulesetCharacter.Guid);
        }
    }

    #endregion

    #region Potent Spellcaster

    private static FeatDefinition BuildPotentSpellcaster(List<FeatDefinition> feats)
    {
        const string Name = "FeatPotentSpellcaster";

        var classes = new List<CharacterClassDefinition>
        {
            Bard,
            Cleric,
            Druid,
            Sorcerer,
            Wizard,
            InventorClass.Class
        };

        var validators = new List<Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)>>
        {
            ValidatorsFeat.IsBardLevel4,
            ValidatorsFeat.IsClericLevel4,
            ValidatorsFeat.IsDruidLevel4,
            ValidatorsFeat.IsSorcererLevel4,
            ValidatorsFeat.IsWizardLevel4,
            ValidatorsFeat.IsInventorLevel4
        };

        var potentSpellcasterFeats = new List<FeatDefinition>();

        for (var i = 0; i < classes.Count; i++)
        {
            var className = classes[i].Name;
            var validator = validators[i];
            var classTitle = GetDefinition<CharacterClassDefinition>(className).FormatTitle();
            var featPotentSpellcaster = FeatDefinitionWithPrerequisitesBuilder
                .Create($"{Name}{className}")
                .SetGuiPresentation(
                    Gui.Format("Feat/&FeatPotentSpellcasterTitle", classTitle),
                    Gui.Format("Feat/&FeatPotentSpellcasterDescription", classTitle))
                .SetValidators(validator)
                .SetFeatFamily("PotentSpellcaster")
                .AddToDB();

            featPotentSpellcaster.AddCustomSubFeatures(
                new CustomBehaviorFeatPotentSpellcaster(featPotentSpellcaster, classes[i]));
            potentSpellcasterFeats.Add(featPotentSpellcaster);
        }

        var potentSpellcasterGroup = GroupFeats.MakeGroupWithPreRequisite(
            "FeatGroupPotentSpellcaster", "PotentSpellcaster", ValidatorsFeat.IsLevel4,
            [.. potentSpellcasterFeats]);

        feats.AddRange(potentSpellcasterFeats);

        return potentSpellcasterGroup;
    }

    internal sealed class CustomBehaviorFeatPotentSpellcaster(
        BaseDefinition baseDefinition,
        CharacterClassDefinition castingClass) : IModifyEffectDescription, IModifyWeaponAttackMode
    {
        public bool IsValid(
            BaseDefinition definition,
            RulesetCharacter character,
            EffectDescription effectDescription)
        {
            if (definition is not SpellDefinition { SpellLevel: 0 } spell) { return false; }

            var repertoires = new List<RulesetSpellRepertoire>();
            character.GetSpellRepertoireFromDefinition(spell, allMatchingRepertoires: repertoires);

            return repertoires.Any(r => r.GetCastingClass() == castingClass);
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            // this might not be correct if same spell is learned from different classes
            // if we follow other patches we should ideally identify all repertoires that can cast spell
            // and use the one with the highest attribute. will revisit if this ever becomes a thing
            if (definition is not SpellDefinition spell)
            {
                return effectDescription;
            }

            var spellRepertoire =
                character.SpellRepertoires.FirstOrDefault(x => x.HasKnowledgeOfSpell(spell));

            if (spellRepertoire == null)
            {
                if (SpellsContext.SpellsChildMaster.TryGetValue(spell, out var parentSpell))
                {
                    spellRepertoire =
                        character.SpellRepertoires.FirstOrDefault(x => x.HasKnowledgeOfSpell(parentSpell));
                }

                if (spellRepertoire == null)
                {
                    return effectDescription;
                }
            }

            var damage = effectDescription.FindFirstDamageForm();

            if (damage == null)
            {
                return effectDescription;
            }

            var attribute = spellRepertoire.SpellCastingAbility;
            var bonus = AttributeDefinitions.ComputeAbilityScoreModifier(character.TryGetAttributeValue(attribute));

            damage.BonusDamage += bonus;
            damage.DamageBonusTrends.Add(
                new TrendInfo(bonus, FeatureSourceType.CharacterFeature, baseDefinition.Name, baseDefinition));

            return effectDescription;
        }

        public void ModifyWeaponAttackMode(
            RulesetCharacter character,
            RulesetAttackMode attackMode,
            RulesetItem weapon,
            bool canAddAbilityDamageBonus)
        {
            if (attackMode.SourceDefinition != CustomWeaponsContext.ProducedFlameDart)
            {
                return;
            }

            var damage = attackMode.EffectDescription.FindFirstDamageForm();

            if (damage == null)
            {
                return;
            }

            var spellRepertoire =
                character.SpellRepertoires.FirstOrDefault(x => x.HasKnowledgeOfSpell(ProduceFlame));

            if (spellRepertoire == null)
            {
                return;
            }

            var attribute = spellRepertoire.SpellCastingAbility;
            var bonus = AttributeDefinitions.ComputeAbilityScoreModifier(character.TryGetAttributeValue(attribute));

            damage.BonusDamage += bonus;
            damage.DamageBonusTrends.Add(
                new TrendInfo(bonus, FeatureSourceType.CharacterFeature, baseDefinition.Name, baseDefinition));
        }
    }

    #endregion

    #region Slay the Enemies

    private static FeatDefinitionWithPrerequisites BuildSlayTheEnemies()
    {
        const string NAME = "FeatSlayTheEnemies";

        var powerPool = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}Pool")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerSlayTheEnemies", Resources.PowerSlayTheEnemies, 256, 128))
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .AddToDB();

        var powerPoolList = new List<FeatureDefinitionPower>();

        var additionalDamage = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{NAME}")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("SlayTheEnemy")
            .SetDamageValueDetermination(ExtraAdditionalDamageValueDetermination.FlatWithProgression)
            .SetIgnoreCriticalDoubleDice(true)
            .SetFlatDamageBonus(0)
            .SetAdvancement(ExtraAdditionalDamageAdvancement.ConditionAmount,
                DiceByRankBuilder.Build((1, 1), (2, 2), (3, 3)))
            .AddToDB();

        var advantageOnFavorite = FeatureDefinitionCombatAffinityBuilder
            .Create($"CombatAffinity{NAME}Favorite")
            .SetGuiPresentation(NAME, Category.Feat, Gui.NoLocalization)
            .SetMyAttackAdvantage(AdvantageType.Advantage)
            .SetSituationalContext(ExtraSituationalContext.TargetIsFavoriteEnemy)
            .AddToDB();

        var toHitOnRegular = FeatureDefinitionCombatAffinityBuilder
            .Create($"CombatAffinity{NAME}Regular")
            .SetGuiPresentation(NAME, Category.Feat, Gui.NoLocalization)
            .SetMyAttackModifier(ExtraCombatAffinityValueDetermination.ConditionAmountIfNotFavoriteEnemy)
            .AddToDB();

        for (var i = 3; i >= 1; i--)
        {
            // closure
            var a = i;

            var rounds = 2 + i;

            var powerGainSlot = FeatureDefinitionPowerSharedPoolBuilder
                .Create($"Power{NAME}{i}")
                .SetGuiPresentation(
                    Gui.Format($"Feature/&Power{NAME}Title", Gui.ToRoman(i)),
                    Gui.Format($"Feature/&Power{NAME}Description", i.ToString(), rounds.ToString()))
                .SetSharedPool(ActivationTime.BonusAction, powerPool)
                .SetEffectDescription(
                    EffectDescriptionBuilder
                        .Create()
                        .SetDurationData(DurationType.Round, rounds)
                        .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                        .SetEffectForms(EffectFormBuilder.ConditionForm(
                            ConditionDefinitionBuilder
                                .Create($"Condition{NAME}{i}")
                                .SetGuiPresentation(
                                    "Condition/&ConditionFeatSlayTheEnemiesTitle",
                                    Gui.Format("Condition/&ConditionFeatSlayTheEnemiesDescription",
                                        i.ToString()), ConditionDefinitions.ConditionTrueStrike)
                                .SetPossessive()
                                .SetFixedAmount(i)
                                .SetFeatures(additionalDamage, advantageOnFavorite, toHitOnRegular)
                                .AddToDB()))
                        .Build())
                .AddCustomSubFeatures(
                    new ValidatorsValidatePowerUse(
                        c =>
                        {
                            var remaining = 0;

                            c.GetClassSpellRepertoire(Ranger)?
                                .GetSlotsNumber(a, out remaining, out _);

                            var noCondition = !c.HasAnyConditionOfType(
                                "ConditionFeatSlayTheEnemies1",
                                "ConditionFeatSlayTheEnemies2",
                                "ConditionFeatSlayTheEnemies3");

                            return remaining > 0 && noCondition;
                        }))
                .AddToDB();

            powerPoolList.Add(powerGainSlot);
        }

        PowerBundle.RegisterPowerBundle(powerPool, false, powerPoolList);

        return FeatDefinitionWithPrerequisitesBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(powerPool)
            .SetValidators(ValidatorsFeat.IsRangerLevel1)
            .AddCustomSubFeatures(new ActionFinishedByMeFeatSlayTheEnemies())
            .AddToDB();
    }

    private sealed class ActionFinishedByMeFeatSlayTheEnemies : IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition power)
        {
            if (!power.Name.StartsWith("PowerFeatSlayTheEnemies"))
            {
                yield break;
            }

            var character = action.ActingCharacter.RulesetCharacter;
            var name = power.Name;
            var level = int.Parse(name.Substring(name.Length - 1, 1));
            var repertoire = character.GetClassSpellRepertoire(Ranger);

            repertoire?.SpendSpellSlot(level);
        }
    }

    #endregion
}
