using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Feats;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Subclasses;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAbilityCheckAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.WeaponTypeDefinitions;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class RulesetCharacterHeroPatcher
{
    private static void EnumerateFeatureDefinitionSavingThrowAffinity(
        RulesetCharacter __instance,
        List<FeatureDefinition> featuresToBrowse,
        Dictionary<FeatureDefinition, FeatureOrigin> featuresOrigin)
    {
        __instance.EnumerateFeaturesToBrowse<FeatureDefinitionSavingThrowAffinity>(featuresToBrowse, featuresOrigin);
        featuresToBrowse.RemoveAll(x =>
            !__instance.IsValid(x.GetAllSubFeaturesOfType<IsCharacterValidHandler>()));
    }

    [HarmonyPatch(typeof(RulesetCharacterHero), nameof(RulesetCharacterHero.RefreshArmorClass))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RefreshArmorClass_Patch
    {
        private static int MaxDexterityBonus(
            ArmorDescription armorDescription,
            RulesetCharacterHero rulesetCharacterHero)
        {
            return ArmorFeats.IsFeatMediumArmorMasterContextValid(armorDescription, rulesetCharacterHero)
                ? 3
                : armorDescription.MaxDexterityBonus;
        }

        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: supports Medium Armor Master Feat by allowing a max dexterity bonus of 3
            //TODO: make this an interface if we ever need similar behavior on other places
            var maxDexterityBonusMethod = typeof(ArmorDescription).GetMethod("get_MaxDexterityBonus");
            var myMaxDexterityBonusMethod =
                new Func<ArmorDescription, RulesetCharacterHero, int>(MaxDexterityBonus).Method;

            var codes = instructions.ReplaceCalls(
                    maxDexterityBonusMethod,
                    "RulesetCharacterHero.RefreshActiveItemFeatures",
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, myMaxDexterityBonusMethod))
                .ToList();

            //PATCH: pass condition amount to `RefreshArmorClassInFeatures` - allows AC modification by Condition Amount for heroes
            //in vanilla this only works on monsters, but for heroes only default 0 is passed

            object rulesetConditionVar = null;
            var found = false;

            var getAmount = typeof(RulesetCondition).GetMethod("get_Amount");

            for (var index = 0; index < codes.Count; index++)
            {
                var code = codes[index];

                if (found)
                {
                    continue;
                }

                if (rulesetConditionVar == null && code.opcode == OpCodes.Ldloc_S &&
                    $"{code.operand}".Contains("RulesetCondition"))
                {
                    rulesetConditionVar = code.operand;
                    continue;
                }

                if (rulesetConditionVar != null && code.opcode == OpCodes.Ldc_I4_0)
                {
                    codes[index] = new CodeInstruction(OpCodes.Ldloc_S, rulesetConditionVar);
                    codes.Insert(index + 1, new CodeInstruction(OpCodes.Callvirt, getAmount));
                    found = true;
                    continue;
                }

                if (rulesetConditionVar != null && $"{code.operand}".Contains("RefreshArmorClassInFeatures"))
                {
                    //abort if we reached refresh call after reaching RulesetCondition local var, but haven't found place of insertion
                    //this means code has changed, and we need to look at it - maybe this patch is not needed anymore in this case
                    break;
                }
            }

            if (!found)
            {
                Main.Error("Couldn't patch RulesetCharacterHero.RefreshArmorClass");
            }

            //PATCH: clear all modifiers tagged 09Health - fixes extra large AC that sometimes happens after respec
            //TODO: find out how/why 09Health modifiers are added to AC attribute
            var oldMethod = typeof(RulesetAttribute).GetProperty(nameof(RulesetAttribute.ValueTrends))!.GetGetMethod();
            var newMethod = new Func<RulesetAttribute, List<TrendInfo>>(ClearHealth).Method;

            return codes.ReplaceCall(oldMethod, 1, "RulesetCharacterHero.RefreshArmorClass.09Health",
                new CodeInstruction(OpCodes.Call, newMethod));
        }

        private static List<TrendInfo> ClearHealth(RulesetAttribute self)
        {
            self.RemoveModifiersByTags(AttributeDefinitions.TagHealth);
            return self.ValueTrends;
        }

        [UsedImplicitly]
        public static void Postfix(
            RulesetCharacterHero __instance,
            ref RulesetAttribute __result,
            bool callRefresh,
            bool dryRun,
            FeatureDefinition dryRunFeature)
        {
            foreach (var feature in __instance.GetSubFeaturesByType<IModifyAC>())
            {
                feature.ModifyAC(__instance, callRefresh, dryRun, dryRunFeature, __result);
            }

            RulesetAttributeModifier.SortAttributeModifiersList(__result.ActiveModifiers);
            __result.Refresh(true);
            __instance.SortArmorClassModifierTrends(__result);
            __result.Refresh();
            if (callRefresh && !dryRun && __instance.CharacterRefreshed != null)
            {
                __instance.CharacterRefreshed(__instance);
            }
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterHero), nameof(RulesetCharacterHero.ComputeAndApplyHitDieRoll))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ComputeAndApplyHitDieRoll_Patch
    {
        [UsedImplicitly]
        public static void Prefix(
            RulesetCharacterHero __instance,
            ref DieType die,
            ref int modifier,
            ref AdvantageType advantageType,
            ref bool healKindred,
            ref bool isBonus)
        {
            foreach (var modifyDiceRollHitDice in __instance.GetSubFeaturesByType<IModifyDiceRollHitDice>())
            {
                modifyDiceRollHitDice.BeforeRoll(
                    __instance, ref die, ref modifier, ref advantageType, ref healKindred, ref isBonus);
            }
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterHero), nameof(RulesetCharacterHero.ComputeBaseAbilityCheckBonus))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ComputeBaseAbilityCheckBonus_Patch
    {
        //BUGFIX: fix Bard jack of all trades, and Martial Champion remarkable athlete on checks with null or empty proficiency name  [VANILLA]
        [UsedImplicitly]
        public static bool Prefix(
            RulesetCharacterHero __instance,
            out int __result,
            string abilityScoreName,
            List<TrendInfo> modifierTrends,
            string proficiencyName = null,
            bool doubleProficiency = false,
            bool checkInventory = true,
            bool checkFeatures = true)
        {
            __result = ComputeBaseAbilityCheckBonus(__instance,
                abilityScoreName, modifierTrends, proficiencyName, doubleProficiency, checkInventory, checkFeatures);

            return false;
        }

        private static int HandleHalfProficiencyWhenNotProficient(
            RulesetCharacterHero __instance,
            string abilityScoreName,
            List<TrendInfo> modifierTrends)
        {
            __instance.EnumerateFeaturesToBrowse<FeatureDefinitionAbilityCheckAffinity>(
                __instance.FeaturesToBrowse, __instance.FeaturesOrigin);

            var result = 0;
            var pb = __instance.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);

            foreach (var featureDefinition in __instance.FeaturesToBrowse)
            {
                var key = (FeatureDefinitionAbilityCheckAffinity)featureDefinition;

                foreach (var add in key.AffinityGroups
                             .Where(x =>
                                 x.abilityScoreName == abilityScoreName &&
                                 x.affinity == CharacterAbilityCheckAffinity.HalfProficiencyWhenNotProficient)
                             .Select(_ => pb / 2))
                {
                    modifierTrends?.Add(new TrendInfo(
                        add,
                        __instance.FeaturesOrigin[key].sourceType,
                        __instance.FeaturesOrigin[key].sourceName,
                        __instance.FeaturesOrigin[key]));

                    result += add;
                }
            }

            return result;
        }

        // vanilla code from RulesetCharacter.ComputeBaseAbilityCheckBonus
        private static int BaseComputeBaseAbilityCheckBonus(
            RulesetCharacterHero __instance,
            string abilityScoreName,
            List<TrendInfo> modifierTrends)
        {
            var abilityScoreModifier =
                AttributeDefinitions.ComputeAbilityScoreModifier(__instance.TryGetAttributeValue(abilityScoreName));

            modifierTrends?.SetRange(
                new TrendInfo(abilityScoreModifier, FeatureSourceType.AbilityScore, abilityScoreName, null));

            return abilityScoreModifier;
        }

        // almost vanilla code except change on pb calc from half round up to half round down
        private static int ComputeBaseAbilityCheckBonus(
            RulesetCharacterHero __instance,
            string abilityScoreName,
            List<TrendInfo> modifierTrends,
            string proficiencyName = null,
            bool doubleProficiency = false,
            bool checkInventory = true,
            bool checkFeatures = true)
        {
            var abilityCheckBonus = 0;

            if (!string.IsNullOrEmpty(abilityScoreName))
            {
                abilityCheckBonus = BaseComputeBaseAbilityCheckBonus(__instance, abilityScoreName, modifierTrends);
            }
            else
            {
                modifierTrends?.Clear();
            }

            if (string.IsNullOrEmpty(proficiencyName))
            {
                if (checkFeatures &&
                    modifierTrends?.All(x => x.sourceType != FeatureSourceType.Proficiency) == true)
                {
                    abilityCheckBonus +=
                        HandleHalfProficiencyWhenNotProficient(__instance, abilityScoreName, modifierTrends);
                }

                return abilityCheckBonus;
            }

            var hasExpertise = __instance.expertiseProficiencies.Contains(proficiencyName);
            var modifier = 0;

            if (__instance.skillProficiencies.Contains(proficiencyName))
            {
                modifier = (hasExpertise | doubleProficiency ? 2 : 1) *
                           __instance.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
            }
            else if (__instance.toolTypeProficiencies.Contains(proficiencyName))
            {
                var element = DatabaseRepository.GetDatabase<ToolTypeDefinition>().GetElement(proficiencyName);

                if (!checkInventory || __instance.characterInventory.CountToolsOfType(element) > 0)
                {
                    modifier = (hasExpertise | doubleProficiency ? 2 : 1) *
                               __instance.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
                }
            }
            else if (proficiencyName == "ForcedProficiency")
            {
                modifier = __instance.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
            }

            if (modifier > 0)
            {
                modifierTrends?.Add(new TrendInfo(modifier, FeatureSourceType.Proficiency, string.Empty, null));
                abilityCheckBonus += modifier;
            }
            else if (checkFeatures)
            {
                abilityCheckBonus +=
                    HandleHalfProficiencyWhenNotProficient(__instance, abilityScoreName, modifierTrends);
            }

            return abilityCheckBonus;
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterHero), nameof(RulesetCharacterHero.FindClassHoldingFeature))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class FindClassHoldingFeature_Patch
    {
        [UsedImplicitly]
        public static void Postfix(
            RulesetCharacterHero __instance,
            FeatureDefinition featureDefinition,
            ref CharacterClassDefinition __result)
        {
            //PATCH: replaces feature holding class with one provided by custom interface
            //used for features that are not granted directly through class but need to scale with class levels
            var classHolder = featureDefinition.GetFirstSubFeatureOfType<ClassHolder>()?.Class;

            if (!classHolder)
            {
                return;
            }

            // Only override if the character actually has levels in the class, to prevent errors
            var levels = __instance.GetClassLevel(classHolder);

            if (levels > 0)
            {
                __result = classHolder;
            }
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterHero), nameof(RulesetCharacterHero.CanCastAnyInvocation))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class CanCastAnyInvocation_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(RulesetCharacterHero __instance, out bool __result)
        {
            //PATCH: Make sure availability of custom invocations doesn't affect default ones
            __result = __instance.Invocations
                .Where(x => x.InvocationDefinition is not InvocationDefinitionCustom)
                .Any(__instance.CanCastInvocation);

            return false;
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterHero), nameof(RulesetCharacterHero.CanCastInvocation))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class CanCastInvocation_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(RulesetCharacterHero __instance, ref bool __result, RulesetInvocation invocation)
        {
            //PATCH: make sure we can't cast hidden invocations, so they will be hidden
            var definition = invocation.InvocationDefinition;
            var isValid = definition
                .GetAllSubFeaturesOfType<IsInvocationValidHandler>()
                .All(v => v(__instance, definition));

            if (definition.HasSubFeatureOfType<ModifyInvocationVisibility>() || !isValid)
            {
                __result = false;

                return false;
            }

            //PATCH: report invocation as cast-able if this is a power we can use
            var power = invocation.invocationDefinition.GetPower();

            if (!power)
            {
                return true;
            }

            __result = __instance.CanUsePower(power);
            return false;
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterHero), nameof(RulesetCharacterHero.GrantInvocations))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class GrantInvocations_Patch
    {
        [UsedImplicitly]
        public static void Postfix(RulesetCharacterHero __instance)
        {
            foreach (var invocation in __instance.Invocations)
            {
                //PATCH: allow customized repertoire matching for invocation
                var matcher = invocation.InvocationDefinition
                    .GetFirstSubFeatureOfType<RepertoireValidForAutoPreparedFeature>();

                if (matcher == null)
                {
                    continue;
                }

                foreach (var repertoire in __instance.SpellRepertoires
                             .Where(repertoire => matcher(repertoire, __instance)))
                {
                    invocation.invocationRepertoire = repertoire;
                    invocation.spellCastingFeature = repertoire.spellCastingFeature;
                    break;
                }
            }
        }
    }

    //PATCH: supports attribute modifiers from feature sets
    [HarmonyPatch(typeof(RulesetCharacterHero), nameof(RulesetCharacterHero.RefreshAttributeModifiersFromInvocations))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RefreshAttributeModifiersFromInvocations_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(RulesetCharacterHero __instance)
        {
            RefreshAttributeModifiersFromInvocations(__instance);

            return false;
        }

        private static void RefreshAttributeModifiersFromInvocations(RulesetCharacterHero __instance)
        {
            __instance.FeaturesToBrowse.Clear();

            foreach (var attribute in __instance.Attributes)
            {
                attribute.Value.RemoveModifiersByTags(AttributeDefinitions.TagInvocation);
            }

            foreach (var trainedInvocation in __instance.trainedInvocations)
            {
                var grantedFeatures = new List<FeatureDefinitionAttributeModifier>();
                var rulesetInvocation = __instance.Invocations.FirstOrDefault(invocation =>
                    invocation.InvocationDefinition == trainedInvocation);

                if (rulesetInvocation == null || rulesetInvocation.Active)
                {
                    switch (trainedInvocation.GrantedFeature)
                    {
                        case FeatureDefinitionAttributeModifier attributeModifier:
                            grantedFeatures.Add(attributeModifier);
                            break;
                        // BEGIN PATCH: support attribute modifiers from feature sets
                        case FeatureDefinitionFeatureSet featureSet:
                            grantedFeatures.AddRange(featureSet.FeatureSet
                                .OfType<FeatureDefinitionAttributeModifier>());
                            break;
                        // END PATCH
                    }
                }

                //BEGIN PATCH: it wasn't a loop before
                foreach (var grantedFeature in grantedFeatures)
                {
                    var attribute = __instance.GetAttribute(grantedFeature.ModifiedAttribute);
                    var modifierValue = grantedFeature.ModifierValue;

                    // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                    switch (grantedFeature.ModifierOperation)
                    {
                        case FeatureDefinitionAttributeModifier.AttributeModifierOperation.AddAbilityScoreBonus:
                        {
                            modifierValue =
                                AttributeDefinitions.ComputeAbilityScoreModifier(
                                    __instance.TryGetAttributeValue(grantedFeature.ModifierAbilityScore));

                            if (grantedFeature.Minimum1 && modifierValue < 1)
                            {
                                modifierValue = 1;
                            }

                            break;
                        }
                        case FeatureDefinitionAttributeModifier.AttributeModifierOperation.AddProficiencyBonus
                            when grantedFeature.UseBonusFromCaster:
                        {
                            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                            switch (ServiceRepository.GetService<IGameService>()
                                        .TryFindControllerFromKindredSpirit(__instance, out var controller))
                            {
                                case TryFindControllerFromKindredSpiritErrorType.Success:
                                    modifierValue =
                                        controller.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
                                    break;
                                case TryFindControllerFromKindredSpiritErrorType.ConditionNotFound:
                                    modifierValue = 0;
                                    break;
                                default:
                                    Trace.LogError("Couldn't find caster for " + __instance.SystemName);
                                    modifierValue = 0;
                                    break;
                            }

                            break;
                        }
                        case FeatureDefinitionAttributeModifier.AttributeModifierOperation.AddProficiencyBonus:
                            modifierValue = __instance.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
                            break;
                        case FeatureDefinitionAttributeModifier.AttributeModifierOperation.AddHalfProficiencyBonus
                            when grantedFeature.UseBonusFromCaster:
                        {
                            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                            switch (ServiceRepository.GetService<IGameService>()
                                        .TryFindControllerFromKindredSpirit(__instance, out var controller))
                            {
                                case TryFindControllerFromKindredSpiritErrorType.Success:
                                    var attributeValue =
                                        controller.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
                                    modifierValue = (attributeValue / 2) + (attributeValue % 2 == 1 ? 1 : 0);
                                    break;
                                case TryFindControllerFromKindredSpiritErrorType.ConditionNotFound:
                                    modifierValue = 0;
                                    break;
                                default:
                                    Trace.LogError("Couldn't find caster for " + __instance.SystemName);
                                    modifierValue = 0;
                                    break;
                            }

                            break;
                        }
                        case FeatureDefinitionAttributeModifier.AttributeModifierOperation.AddHalfProficiencyBonus:
                        {
                            var attributeValue =
                                __instance.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);

                            modifierValue = (attributeValue / 2) + (attributeValue % 2 == 1 ? 1 : 0);
                            break;
                        }
                    }

                    attribute.AddModifier(
                        RulesetAttributeModifier.BuildAttributeModifier(
                            grantedFeature.ModifierOperation, modifierValue, AttributeDefinitions.TagInvocation));
                }
            }

            foreach (var attribute in __instance.Attributes)
            {
                RulesetAttributeModifier.SortAttributeModifiersList(attribute.Value.ActiveModifiers);
            }
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterHero), nameof(RulesetCharacterHero.RefreshAttackMode))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RefreshAttackMode_Patch
    {
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: enables `AttackRollModifierMethod` support for hero attack modification
            //default implementation just gets flat value and ignores other methods
            //replaces call to `AttackRollModifier` getter with custom method that returns proper values
            var attackRollMethod = typeof(IAttackModificationProvider).GetMethod("get_AttackRollModifier");
            var customAttackRoll =
                new Func<IAttackModificationProvider, RulesetCharacterHero, int>(GetAttackRollModifier)
                    .Method;

            //PATCH: enables `DamageRollModifierMethod` support for hero attack modification
            //default implementation just gets flat value and ignores other methods
            //replaces call to `DamageRollModifier` getter with custom method that returns proper values
            var damageRollMethod = typeof(IAttackModificationProvider).GetMethod("get_DamageRollModifier");
            var customDamageRoll =
                new Func<IAttackModificationProvider, RulesetCharacterHero, int>(GetDamageRollModifier)
                    .Method;

            //PATCH: support for AddTagToWeapon
            var weaponTags = typeof(WeaponDescription)
                .GetProperty(nameof(WeaponDescription.WeaponTags))
                ?.GetGetMethod();
            var customWeaponTags = new Func<
                WeaponDescription,
                RulesetCharacter,
                RulesetItem,
                List<string>
            >(AddTagToWeapon.GetCustomWeaponTags).Method;

            return instructions
                .ReplaceCalls(attackRollMethod,
                    "RulesetCharacterHero.RefreshAttackMode.AttackRollModifier",
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, customAttackRoll))
                .ReplaceCalls(damageRollMethod,
                    "RulesetCharacterHero.RefreshAttackMode.DamageRollModifier",
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, customDamageRoll))
                .ReplaceCalls(weaponTags,
                    "RulesetCharacterHero.RefreshAttackMode.WeaponTags",
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg, 9),
                    new CodeInstruction(OpCodes.Call, customWeaponTags));
        }

        private static int GetAttackRollModifier(IAttackModificationProvider provider, RulesetCharacterHero hero)
        {
            var num = provider.AttackRollModifier;

            switch (provider.AttackRollModifierMethod)
            {
                case AttackModifierMethod.SourceConditionAmount:
                    num = hero.FindFirstConditionHoldingFeature(provider as FeatureDefinition).Amount;
                    break;
                case AttackModifierMethod.AddAbilityScoreBonus when
                    !string.IsNullOrEmpty(provider.AttackRollAbilityScore):
                    num += AttributeDefinitions.ComputeAbilityScoreModifier(
                        hero.TryGetAttributeValue(provider.AttackRollAbilityScore));
                    break;
                case AttackModifierMethod.None:
                case AttackModifierMethod.FlatValue:
                    //These require no additional processing
                    break;
                case AttackModifierMethod.AddProficiencyBonus:
                    num += hero.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(provider));
            }

            return num;
        }

        private static int GetDamageRollModifier(IAttackModificationProvider provider, RulesetCharacterHero hero)
        {
            var num = provider.DamageRollModifier;

            switch (provider.DamageRollModifierMethod)
            {
                case AttackModifierMethod.SourceConditionAmount:
                case AttackModifierMethod.AddAbilityScoreBonus:
                    //These are processed by base method
                    break;
                case AttackModifierMethod.None:
                case AttackModifierMethod.FlatValue:
                    //These require no additional processing
                    break;
                case AttackModifierMethod.AddProficiencyBonus:
                    num += hero.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(provider));
            }

            return num;
        }

        [UsedImplicitly]
        public static void Prefix(
            RulesetCharacterHero __instance,
            ref ItemDefinition itemDefinition,
            ref WeaponDescription weaponDescription,
            List<IAttackModificationProvider> attackModifiers,
            ref RulesetItem weapon)
        {
            //PATCH: allow hand wraps to be put into gauntlet slot
            CustomWeaponsContext.ModifyUnarmedAttackWithGauntlet(
                __instance, ref itemDefinition, ref weaponDescription, ref weapon);

            //PATCH: validate damage features
            attackModifiers.RemoveAll(provider =>
                provider is BaseDefinition feature
                && !__instance.IsValid(feature.GetAllSubFeaturesOfType<IsCharacterValidHandler>()));
        }

        [UsedImplicitly]
        public static void Postfix(
            RulesetCharacterHero __instance,
            RulesetAttackMode __result,
            bool canAddAbilityDamageBonus,
            RulesetItem weapon)
        {
            //PATCH: Allows changing damage and other stats of an attack mode
            var weaponAttackModeModifiers = __instance.GetSubFeaturesByType<IModifyWeaponAttackMode>();

            if (__result.sourceObject is RulesetItem item)
            {
                weaponAttackModeModifiers.AddRange(item.GetSubFeaturesByType<IModifyWeaponAttackMode>());
            }

            foreach (var modifier in weaponAttackModeModifiers)
            {
                modifier.ModifyWeaponAttackMode(__instance, __result, weapon, canAddAbilityDamageBonus);
            }
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterHero), nameof(RulesetCharacterHero.RefreshAttackModes))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RefreshAttackModes_Patch
    {
        private static bool _callRefresh;

        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var weaponTagsMethod = typeof(WeaponDescription).GetMethod("get_WeaponTags");

            var myWeaponTagsMethod =
                new Func<WeaponDescription, List<string>>(WeaponTags).Method;

            //PATCH: make ISpellCastingAffinityProvider from dynamic item properties apply to repertoires
            return instructions.ReplaceCalls(weaponTagsMethod,
                "RulesetCharacterHero.RefreshAttackModes",
                new CodeInstruction(OpCodes.Call, myWeaponTagsMethod));
        }

        //PATCH: supports AccountForAllDiceOnFollowUpStrike
        private static List<string> WeaponTags(WeaponDescription weaponDescription)
        {
            return weaponDescription.WeaponTags.Where(x => x != "TwoHanded").ToList();
        }

        private static void HandleFollowUpStrike(RulesetCharacterHero rulesetCharacterHero)
        {
            var mainHandInventorySlot =
                rulesetCharacterHero.CharacterInventory.InventorySlotsByType[EquipmentDefinitions.SlotTypeMainHand][0];
            var equipedItem = mainHandInventorySlot.EquipedItem;
            var itemDefinition = mainHandInventorySlot.EquipedItem?.ItemDefinition;
            var weaponDescription = itemDefinition?.WeaponDescription;

            if (weaponDescription == null ||
                !weaponDescription.WeaponTags.Contains("TwoHanded") ||
                weaponDescription.WeaponTypeDefinition.WeaponProximity != AttackProximity.Melee ||
                rulesetCharacterHero.ExecutedAttacks == 0)
            {
                return;
            }

            foreach (var attackModifier in rulesetCharacterHero.attackModifiers)
            {
                if (!attackModifier.FollowUpStrike)
                {
                    continue;
                }

                var attackMode = rulesetCharacterHero.RefreshAttackMode(
                    ActionDefinitions.ActionType.Bonus,
                    itemDefinition,
                    weaponDescription,
                    false,
                    attackModifier.FollowUpAddAbilityBonus,
                    mainHandInventorySlot.Name,
                    rulesetCharacterHero.attackModifiers,
                    rulesetCharacterHero.FeaturesOrigin,
                    equipedItem);

                var effectDamageForms = attackMode.EffectDescription.EffectForms
                    .Where(x => x.FormType == EffectForm.EffectFormType.Damage)
                    .ToList();

                if (effectDamageForms.Count != 0)
                {
                    effectDamageForms[0] = EffectForm.GetCopy(effectDamageForms[0]);
                    effectDamageForms[0].DamageForm.DieType = attackModifier.FollowUpDamageDie;
                    effectDamageForms[0].DamageForm.DiceNumber = 1;
                }

                if (!Main.Settings.AccountForAllDiceOnFollowUpStrike &&
                    effectDamageForms.Count > 1)
                {
                    effectDamageForms.RemoveRange(1, effectDamageForms.Count - 1);
                }

                attackMode.EffectDescription.EffectForms.SetRange(effectDamageForms);
                rulesetCharacterHero.AttackModes.Add(attackMode);
            }
        }

        [UsedImplicitly]
        public static void Prefix(ref bool callRefresh)
        {
            //save refresh flag, so it can be used in postfix
            _callRefresh = callRefresh;
            //reset refresh flag, so default code won't do refresh before postfix
            callRefresh = false;
        }

        [UsedImplicitly]
        public static void Postfix(RulesetCharacterHero __instance)
        {
            //PATCH: supports AccountForAllDiceOnFollowUpStrike
            HandleFollowUpStrike(__instance);

            //PATCH: Allows adding extra attack modes
            __instance.GetSubFeaturesByType<IAddExtraAttack>()
                .OrderBy(provider => provider.Priority())
                .Do(provider => provider.TryAddExtraAttack(__instance));

            //PATCH: add Main Action gauntlet attacks if needed
            CustomWeaponsContext.TryAddMainActionUnarmedAttacks(__instance);

            //PATCH: remove invalid attacks to prevent hand crossbows use with no free hand
            __instance.AttackModes.RemoveAll(mode => CustomItemsContext.IsAttackModeInvalid(__instance, mode));

            //refresh character if needed after postfix
            if (_callRefresh && __instance.CharacterRefreshed != null)
            {
                __instance.CharacterRefreshed(__instance);
            }
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterHero))]
    [HarmonyPatch(nameof(RulesetCharacterHero.UpgradeAttackModeDieTypeWithAttackModifierByCharacterLevel))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class UpgradeAttackModeDieTypeWithAttackModifierByCharacterLevel_Patch
    {
        private static int TryGetAttributeValue(
            RulesetEntity __instance,
            string attribute,
            RulesetCharacterHero rulesetCharacterHero)
        {
            var monkLevels = rulesetCharacterHero.GetClassLevel(Monk);

            return monkLevels;
        }

        //PATCH: Monks should use class level on damage dice
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var tryGetAttributeValueMethod = typeof(RulesetEntity).GetMethod("TryGetAttributeValue");

            var myTryGetAttributeValueMethod =
                new Func<RulesetEntity, string, RulesetCharacterHero, int>(TryGetAttributeValue).Method;

            //PATCH: make ISpellCastingAffinityProvider from dynamic item properties apply to repertoires
            return instructions.ReplaceCalls(tryGetAttributeValueMethod,
                "RulesetCharacterHero.UpgradeAttackModeDieTypeWithAttackModifierByCharacterLevel",
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, myTryGetAttributeValueMethod));
        }

        [UsedImplicitly]
        public static bool Prefix(RulesetCharacterHero __instance,
            RulesetAttackMode attackMode,
            IAttackModificationProvider attackModifier)
        {
            var feature = attackModifier as FeatureDefinition;

            if (!feature)
            {
                return true;
            }

            var provider = feature.GetFirstSubFeatureOfType<IModifyProviderRank>();

            if (provider == null)
            {
                return true;
            }

            var rank = provider.GetRank(__instance);
            var dieTypeOfRank = attackModifier.GetDieTypeOfRank(rank);

            if (rank <= 0)
            {
                return false;
            }

            var damage = attackMode.EffectDescription.FindFirstDamageForm();

            if (dieTypeOfRank > damage.DieType)
            {
                damage.DieType = dieTypeOfRank;
            }

            if (dieTypeOfRank > damage.VersatileDieType)
            {
                damage.VersatileDieType = dieTypeOfRank;
            }


            return false;
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterHero), nameof(RulesetCharacterHero.RefreshAll))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RefreshAll_Patch
    {
        [UsedImplicitly]
        public static void Prefix(RulesetCharacterHero __instance)
        {
            //PATCH: clears cached customized spell effects
            PowerBundle.ClearSpellEffectCache(__instance);

#if false
            //PATCH: Support for `IHeroRefreshed`
            __instance.GetSubFeaturesByType<IHeroRefreshed>()
                .ForEach(listener => listener.OnHeroRefreshed(__instance));
#endif
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterHero), nameof(RulesetCharacterHero.RefreshActiveFightingStyles))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RefreshActiveFightingStyles_Patch
    {
        [UsedImplicitly]
        public static void Postfix(RulesetCharacterHero __instance)
        {
            //PATCH: enables some corner-case fighting styles (like archery for hand crossbows and dual wielding for shield expert)
            FightingStyleContext.RefreshFightingStylesPatch(__instance);
        }
    }

    //BUGFIX: allows gauntlet to be used by Druids
    //as vanilla put them on simple weapon categories and Druids are proficient with specic ones
    [HarmonyPatch(typeof(RulesetCharacterHero), nameof(RulesetCharacterHero.IsProficientWithItem))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class IsProficientWithItem_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(RulesetCharacterHero __instance, ref bool __result, ItemDefinition itemDefinition)
        {
            if (!itemDefinition.IsWeapon ||
                itemDefinition.WeaponDescription.WeaponTypeDefinition != UnarmedStrikeType)
            {
                return true;
            }

            __result = true;

            return false;
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterHero), nameof(RulesetCharacterHero.AcknowledgeAttackUse))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class AcknowledgeAttackUse_Patch
    {
        // ReSharper disable once RedundantAssignment
        [UsedImplicitly]
        public static void Prefix(
            RulesetCharacterHero __instance,
            RulesetAttackMode mode,
            ref AttackProximity proximity)
        {
            //PATCH: supports turning Produced Flame into a weapon
            //destroys Produced Flame after attacking with it
            CustomWeaponsContext.ProcessProducedFlameAttack(__instance, mode);

            //PATCH: Support for returning weapons
            //Sets proximity to `Melee` if this was ranged attack with thrown weapon that has returning sub-feature
            //this will skip removal of the weapon from hand and attempt to get new one from inventory
            proximity = ReturningWeapon.Process(__instance, mode, proximity);
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterHero), nameof(RulesetCharacterHero.ComputeCraftingDurationHours))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ComputeCraftingDurationHours_Patch
    {
        [UsedImplicitly]
        public static void Postfix(ref int __result)
        {
            //PATCH: reduces the total crafting time by a given percentage
            __result = (int)((100f - Main.Settings.TotalCraftingTimeModifier) / 100 * __result);
        }
    }

    //PATCH: allow Monk Specialized Weapon feature to work correctly
    [HarmonyPatch(typeof(RulesetCharacterHero), nameof(RulesetCharacterHero.IsWieldingMonkWeapon))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class IsWieldingMonkWeapon_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(RulesetCharacterHero __instance, out bool __result)
        {
            var main = __instance.GetMainWeapon();
            var off = __instance.GetOffhandWeapon();

            var isMainMonkWeapon = main == null || !main.ItemDefinition.IsWeapon ||
                                   __instance.IsMonkWeaponOrUnarmed(main.ItemDefinition);

            var isOffMonkWeapon = off == null || !off.ItemDefinition.IsWeapon ||
                                  __instance.IsMonkWeaponOrUnarmed(off.ItemDefinition);

            __result = isMainMonkWeapon && isOffMonkWeapon;

            return false;
        }
    }

    //PATCH: DisableAutoEquip
    [HarmonyPatch(typeof(RulesetCharacterHero), nameof(RulesetCharacterHero.GrantItem))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class GrantItem_Patch
    {
        [UsedImplicitly]
        public static void Prefix(RulesetCharacterHero __instance, ref bool tryToEquip)
        {
            if (!Main.Settings.DisableAutoEquip || !tryToEquip)
            {
                return;
            }

            tryToEquip = __instance.TryGetHeroBuildingData(out _);
        }
    }

    //PATCH: ensures ritual spells from all spell repertoires are made available (Multiclass)
    [HarmonyPatch(typeof(RulesetCharacterHero), nameof(RulesetCharacterHero.EnumerateUsableRitualSpells))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class EnumerateUsableRitualSpells_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(
            RulesetCharacterHero __instance,
            List<SpellDefinition> ritualSpells)
        {
            // originally it was supposed to only trigger with MC, but we now need for Plane Magic scenarios
            // if (!SharedSpellsContext.IsMulticaster(__instance))
            // {
            //     return true;
            // }

            var allRitualSpells = new List<SpellDefinition>();
            var magicAffinities = new List<FeatureDefinition>();

            ritualSpells.SetRange(allRitualSpells);

            __instance.EnumerateFeaturesToBrowse<FeatureDefinitionMagicAffinity>(magicAffinities);

            // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
            foreach (var featureDefinitionMagicAffinity in magicAffinities
                         .OfType<FeatureDefinitionMagicAffinity>())
            {
                if (featureDefinitionMagicAffinity.RitualCasting == RitualCasting.None)
                {
                    continue;
                }

                foreach (var spellRepertoire in __instance.SpellRepertoires)
                {
                    // this is very similar to switch statement TA wrote but with spell loops outside
                    // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                    switch (featureDefinitionMagicAffinity.RitualCasting)
                    {
                        case RitualCasting.PactTomeRitual:
                        {
                            var maxSpellLevel = SharedSpellsContext.MaxSpellLevelOfSpellCastingLevel(spellRepertoire);

                            foreach (var kvp in spellRepertoire.ExtraSpellsByTag.Where(kvp =>
                                         kvp.Key.Contains("PactTomeRitual")))
                            {
                                var spell = kvp.Value
                                    .Where(spellDefinition =>
                                        spellDefinition.Ritual && maxSpellLevel >= spellDefinition.SpellLevel);

                                allRitualSpells.AddRange(spell);
                            }

                            break;
                        }

                        case RitualCasting.Selection:
                        {
                            var spells = spellRepertoire.KnownSpells
                                .Where(knownSpell =>
                                    knownSpell.Ritual && spellRepertoire.MaxSpellLevelOfSpellCastingLevel >=
                                    knownSpell.SpellLevel);

                            allRitualSpells.AddRange(spells);

                            break;
                        }

                        case RitualCasting.Prepared
                            when spellRepertoire.SpellCastingFeature.SpellReadyness ==
                                 SpellReadyness.Prepared &&
                                 spellRepertoire.SpellCastingFeature.SpellKnowledge ==
                                 SpellKnowledge.WholeList:
                        {
                            var maxSpellLevel = SharedSpellsContext.MaxSpellLevelOfSpellCastingLevel(spellRepertoire);
                            var spells = spellRepertoire.PreparedSpells
                                .Where(s => s.Ritual)
                                .Where(s => maxSpellLevel >= s.SpellLevel);

                            allRitualSpells.AddRange(spells);

                            break;
                        }
                        case RitualCasting.Spellbook
                            when spellRepertoire.SpellCastingFeature.SpellKnowledge ==
                                 SpellKnowledge.Spellbook:
                        {
                            __instance.CharacterInventory.EnumerateAllItems(__instance.Items);

                            var maxSpellLevel = SharedSpellsContext.MaxSpellLevelOfSpellCastingLevel(spellRepertoire);
                            var spells = __instance.Items
                                .OfType<RulesetItemSpellbook>()
                                .SelectMany(x => x.ScribedSpells)
                                .Where(s => s.Ritual)
                                .Where(s => maxSpellLevel >= s.SpellLevel)
                                .ToArray();

                            __instance.Items.Clear();

                            allRitualSpells.AddRange(spells);

                            break;
                        }

#if false
                        // special case for Witch
                        case (RuleDefinitions.RitualCasting)ExtraRitualCasting.Known:
                        {
                            var maxSpellLevel = SharedSpellsContext.MaxSpellLevelOfSpellCastingLevel(spellRepertoire);
                            var spells = spellRepertoire.KnownSpells
                                .Where(s => s.Ritual)
                                .Where(s => maxSpellLevel >= s.SpellLevel);

                            allRitualSpells.AddRange(spells);

                            if (spellRepertoire.AutoPreparedSpells == null)
                            {
                                return true;
                            }

                            spells = spellRepertoire.AutoPreparedSpells
                                .Where(s => s.Ritual)
                                .Where(s => maxSpellLevel >= s.SpellLevel);

                            allRitualSpells.AddRange(spells);
                            break;
                        }
#endif
                    }
                }
            }

            ritualSpells.SetRange(allRitualSpells.Distinct());

            return false;
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterHero), nameof(RulesetCharacterHero.GrantExperience))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class GrantExperience_Patch
    {
        [UsedImplicitly]
        public static void Prefix(ref int experiencePoints)
        {
            if (Main.Settings.MultiplyTheExperienceGainedBy is 100 or <= 0)
            {
                return;
            }

            // ReSharper disable once RedundantAssignment
            experiencePoints =
                (int)Math.Round(experiencePoints * Main.Settings.MultiplyTheExperienceGainedBy / 100.0f,
                    MidpointRounding.AwayFromZero);
        }
    }

    //PATCH: enables the No Experience on Level up cheat (NoExperienceOnLevelUp)
    [HarmonyPatch(typeof(RulesetCharacterHero), nameof(RulesetCharacterHero.CanLevelUp), MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class CanLevelUp_Getter_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(RulesetCharacterHero __instance, out bool __result)
        {
            var maxLevel = !Gui.Game ? Level20Context.GameMaxLevel : Gui.Game.CampaignDefinition.LevelCap;
            var levelCap = Main.Settings.EnableLevel20 ? Level20Context.ModMaxLevel : maxLevel;
            var level = __instance.TryGetAttributeValue(AttributeDefinitions.CharacterLevel);
            var experience = __instance.TryGetAttributeValue(AttributeDefinitions.Experience);
            var nextLevelThreshold = ComputeNextLevelThreshold(level + 1);

            __result = ((Main.Settings.NoExperienceOnLevelUp && Gui.GameLocation) ||
                        (nextLevelThreshold > 0 && experience >= nextLevelThreshold)) &&
                       __instance.ClassesHistory.Count < levelCap;

            return false;
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterHero), nameof(RulesetCharacterHero.AddClassLevel))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class AddClassLevel_Patch
    {
        [UsedImplicitly]
        public static bool Prefix([NotNull] RulesetCharacterHero __instance, CharacterClassDefinition classDefinition)
        {
            var isLevelingUp = LevelUpHelper.IsLevelingUp(__instance);

            if (!isLevelingUp)
            {
                return true;
            }

            //PATCH: only adds the dice max value on level 1 (MULTICLASS)
            __instance.ClassesHistory.Add(classDefinition);
            __instance.ClassesAndLevels.TryAdd(classDefinition, 0);
            __instance.ClassesAndLevels[classDefinition]++;
            __instance.hitPointsGainHistory.Add(HeroDefinitions.RollHitPoints(classDefinition.HitDice));
            __instance.ComputeCharacterLevel();
            __instance.ComputeProficiencyBonus();

            return false;
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterHero), nameof(RulesetCharacterHero.EnumerateAvailableDevices))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class EnumerateAvailableDevices_Patch
    {
        [UsedImplicitly]
        public static void Postfix(
            RulesetCharacterHero __instance,
            ref IEnumerable<RulesetItemDevice> __result)
        {
            //PATCH: enabled `PowerPoolDevice` by adding fake device to hero's usable devices list
            if (__instance.UsableDeviceFromMenu != null)
            {
                return;
            }

            var providers = __instance.GetSubFeaturesByType<PowerPoolDevice>();

            if (providers.Count == 0)
            {
                return;
            }

            var tmp = __result.ToList();

            tmp.AddRange(providers.Select(provider => provider.GetDevice(__instance)));

            __result = tmp;
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterHero), nameof(RulesetCharacterHero.UseDeviceFunction))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class UseDeviceFunction_Patch
    {
        [UsedImplicitly]
        public static void Postfix(RulesetCharacterHero __instance,
            RulesetItemDevice usableDevice,
            RulesetDeviceFunction function,
            int additionalCharges)
        {
            //PATCH: enables `PowerPoolDevice` to consume usage for power pool
            var feature = PowerPoolDevice.GetFromRulesetItem(__instance, usableDevice);

            if (feature == null)
            {
                return;
            }

            var useAmount = function.DeviceFunctionDescription.UseAmount + additionalCharges;

            __instance.UpdateUsageForPower(feature.Pool, useAmount);
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterHero), nameof(RulesetCharacterHero.Unregister))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Unregister_Patch
    {
        [UsedImplicitly]
        public static void Postfix(RulesetCharacterHero __instance)
        {
            //PATCH: clears cached devices for a hero
            PowerPoolDevice.Clear(__instance);
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterHero), nameof(RulesetCharacterHero.EnumerateAfterRestActions))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class EnumerateAfterRestActions_Patch
    {
        [UsedImplicitly]
        public static void Postfix(RulesetCharacterHero __instance)
        {
            __instance.afterRestActions.RemoveAll(activity =>
            {
                if (!Tabletop2024Context.IsRestActivityMemorizeSpellAvailable(activity, __instance))
                {
                    return true;
                }

                if (!Level20Context.WizardSpellMastery.IsRestActivityAvailable(activity, __instance))
                {
                    return true;
                }

                if (!Level20Context.WizardSignatureSpells.IsRestActivityAvailable(activity, __instance))
                {
                    return true;
                }

                if (activity.functor != PowerBundleContext.UseCustomRestPowerFunctorName)
                {
                    return false;
                }

                if (!DatabaseHelper.TryGetDefinition<FeatureDefinitionPower>(activity.StringParameter, out var power))
                {
                    return false;
                }

                var p = activity.GetFirstSubFeatureOfType<ValidateRestActivity>()
                        ?? new ValidateRestActivity(true, true);

                return !__instance.CanUsePower(power, p.ConsiderUses, p.ConsiderHaving);
            });
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterHero), nameof(RulesetCharacterHero.ItemEquiped))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ItemEquiped_Patch
    {
        [UsedImplicitly]
        public static void Postfix(RulesetCharacterHero __instance)
        {
            __instance.GetSubFeaturesByType<IOnItemEquipped>()
                .ForEach(f => f.OnItemEquipped(__instance));

#if false
            //BUGFIX: fix a prepared spells exploit in vanilla
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var repertoire in __instance.SpellRepertoires)
            {
                var spellCastingFeature = repertoire.SpellCastingFeature;

                if (spellCastingFeature.SpellKnowledge is not (SpellKnowledge.Spellbook or SpellKnowledge.WholeList) ||
                    spellCastingFeature.SpellReadyness != SpellReadyness.Prepared)
                {
                    continue;
                }

                var preparedSpells = repertoire.PreparedSpells;

                __instance.EnumerateFeaturesToBrowse<ISpellCastingAffinityProvider>(__instance.FeaturesToBrowse);

                var maxPreparedSpells = __instance.ComputeMaxPreparedSpells(repertoire);

                repertoire.maxPreparedSpells = maxPreparedSpells;

                while (preparedSpells.Count > maxPreparedSpells)
                {
                    preparedSpells.RemoveAt(preparedSpells.Count - 1);
                }
            }
#endif
        }
    }

    //PATCH: support One DnD max prepared spells now calculated from a table
    [HarmonyPatch(typeof(RulesetCharacterHero), nameof(RulesetCharacterHero.ComputeMaxPreparedSpells))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ComputeMaxPreparedSpells_Patch
    {
        // these are One DnD max prepared spells table
        private static readonly List<int> ClericOrDruidOneDndPreparedSpellsTable =
            [4, 5, 6, 7, 9, 10, 11, 12, 14, 15, 16, 16, 17, 17, 18, 18, 19, 20, 21, 22];

        private static readonly List<int> PaladinOneDndPreparedSpellsTable =
            [2, 3, 4, 5, 6, 6, 7, 7, 9, 9, 10, 10, 11, 11, 12, 12, 14, 14, 15, 15];

        private static readonly List<int> WizardOneDndPreparedSpellsTable =
            [4, 5, 6, 7, 9, 10, 11, 12, 14, 15, 16, 16, 17, 18, 19, 21, 22, 23, 24, 25];

        private static readonly Dictionary<CharacterClassDefinition, List<int>> PreparedSpells = new()
        {
            { Cleric, ClericOrDruidOneDndPreparedSpellsTable },
            { Druid, ClericOrDruidOneDndPreparedSpellsTable },
            { Paladin, PaladinOneDndPreparedSpellsTable },
            { Wizard, WizardOneDndPreparedSpellsTable }
        };

        [UsedImplicitly]
        public static bool Prefix(
            RulesetCharacterHero __instance, ref int __result, RulesetSpellRepertoire spellRepertoire)
        {
            if (!spellRepertoire.SpellCastingClass)
            {
                return true;
            }

            if (!Main.Settings.EnablePreparedSpellsTables2024 ||
                !PreparedSpells.TryGetValue(spellRepertoire.SpellCastingClass, out var preparedSpells))
            {
                return true;
            }

            var levels = __instance.ClassesAndLevels[spellRepertoire.SpellCastingClass];

            __result = preparedSpells[levels - 1];

            foreach (var affinityProvider in __instance.FeaturesToBrowse.OfType<ISpellCastingAffinityProvider>())
            {
                // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                switch (affinityProvider.PreparedSpellModifier)
                {
                    case PreparedSpellsModifier.ProficiencyBonus:
                    {
                        __result += __instance.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
                        break;
                    }
                    case PreparedSpellsModifier.SpellcastingAbilityBonus:
                    {
                        var attribute = __instance.GetAttribute(spellRepertoire.SpellCastingAbility);

                        __result += AttributeDefinitions.ComputeAbilityScoreModifier(attribute.CurrentValue);
                        break;
                    }
                }
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterHero), nameof(RulesetCharacterHero.GetAmmunitionType))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class GetAmmunitionType_Patch
    {
        [UsedImplicitly]
        public static void Postfix(RulesetCharacterHero __instance, ref string __result, RulesetAttackMode mode)
        {
            var currentAmmunitionSlot = __instance.CharacterInventory.GetCurrentAmmunitionSlot(__result);

            // only standard ammunition
            if (currentAmmunitionSlot?.EquipedItem == null ||
                !currentAmmunitionSlot.EquipedItem.ItemDefinition ||
                currentAmmunitionSlot.EquipedItem.ItemDefinition.AmmunitionDescription?.EffectDescription == null ||
                currentAmmunitionSlot.EquipedItem.ItemDefinition.AmmunitionDescription.EffectDescription
                    .FindFirstDamageForm() != null)
            {
                return;
            }

            if (RepeatingShot.HasRepeatingShot(mode.sourceObject as RulesetItem))
            {
                __result = string.Empty;
            }
        }
    }

    //PATCH: allow FeatureDefinitionSavingThrowAffinity to be validated with IsCharacterValidHandler
    [HarmonyPatch(typeof(RulesetCharacterHero), nameof(RulesetCharacterHero.IsProficientWithSavingThrow))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class IsProficientWithSavingThrow_Patch
    {
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: make ISpellCastingAffinityProvider from dynamic item properties apply to repertoires
            return instructions.ReplaceEnumerateFeaturesToBrowse<FeatureDefinitionSavingThrowAffinity>(
                "RulesetCharacterHero.FindBestRegenerationFeature", EnumerateFeatureDefinitionSavingThrowAffinity);
        }
    }

    //PATCH: allow FeatureDefinitionSavingThrowAffinity to be validated with IsCharacterValidHandler
    [HarmonyPatch(typeof(RulesetCharacterHero), nameof(RulesetCharacterHero.ComputeBaseSavingThrowBonus))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ComputeBaseSavingThrowBonus_Patch
    {
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: make ISpellCastingAffinityProvider from dynamic item properties apply to repertoires
            return instructions.ReplaceEnumerateFeaturesToBrowse<FeatureDefinitionSavingThrowAffinity>(
                "RulesetCharacterHero.ComputeBaseSavingThrowBonus", EnumerateFeatureDefinitionSavingThrowAffinity);
        }
    }

    //PATCH: supports Medium Armor Master feat by removing disadvantage on medium armors stealth checks
    //TODO: make this an interface if we ever need similar behavior on other places
    [HarmonyPatch(typeof(RulesetCharacterHero), nameof(RulesetCharacterHero.RefreshActiveItemFeatures))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RefreshActiveItemFeatures_Patch
    {
        private static List<ItemPropertyDescription> StaticProperties(
            ItemDefinition itemDefinition,
            RulesetCharacterHero rulesetCharacterHero)
        {
            return ArmorFeats.IsFeatMediumArmorMasterContextValid(itemDefinition, rulesetCharacterHero)
                ? itemDefinition.StaticProperties
                    .Where(x => x.FeatureDefinition != AbilityCheckAffinityStealthDisadvantage)
                    .ToList()
                : itemDefinition.StaticProperties;
        }

        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var staticPropertiesMethod = typeof(ItemDefinition).GetMethod("get_StaticProperties");
            var myStaticPropertiesMethod =
                new Func<ItemDefinition, RulesetCharacterHero, List<ItemPropertyDescription>>(StaticProperties).Method;

            return instructions.ReplaceCalls(staticPropertiesMethod,
                "RulesetCharacterHero.RefreshActiveItemFeatures",
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, myStaticPropertiesMethod));
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterHero), nameof(RulesetCharacterHero.IsDualWieldingMeleeWeapons))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class GIsDualWieldingMeleeWeapons_Patch
    {
        [UsedImplicitly]
        public static void Postfix(RulesetCharacterHero __instance, ref bool __result)
        {
            //PATCH: allows using features that require dual-wielding melee if in Guardian mode with both hands empty
            if (__result)
            {
                return;
            }

            if (InnovationArmor.InGuardianMode(__instance))
            {
                __result = __instance.HasEmptyMainHand() && __instance.HasEmptyOffHand();
            }
        }
    }
}
