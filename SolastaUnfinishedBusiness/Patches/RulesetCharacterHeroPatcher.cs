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
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Races;
using SolastaUnfinishedBusiness.Subclasses;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class RulesetCharacterHeroPatcher
{
    private static void EnumerateFeatureDefinitionSavingThrowAffinity(
        RulesetCharacter __instance,
        List<FeatureDefinition> featuresToBrowse,
        Dictionary<FeatureDefinition, RuleDefinitions.FeatureOrigin> featuresOrigin)
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
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: pass condition amount to `RefreshArmorClassInFeatures` - allows AC modification by Condition Amount for heroes
            //in vanilla this only works on monsters, but for heroes only default 0 is passed

            var codes = instructions.ToList();
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
                    //this means code has changed and we need to look at it - maybe this patch is not needed anymore in this case
                    break;
                }
            }

            if (!found)
            {
                Main.Error("Couldn't patch RulesetCharacterHero.RefreshArmorClass");
            }

            return codes;
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
            var classHolder = featureDefinition.GetFirstSubFeatureOfType<IClassHoldingFeature>()?.Class;

            if (classHolder == null)
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
            if (invocation.invocationDefinition.HasSubFeatureOfType<Hidden>())
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
                //PATCH: mark some invocation as disabled by default
                invocation.active = !invocation.invocationDefinition.HasSubFeatureOfType<InvocationDisabledByDefault>();

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
                case RuleDefinitions.AttackModifierMethod.SourceConditionAmount:
                    num = hero.FindFirstConditionHoldingFeature(provider as FeatureDefinition).Amount;
                    break;
                case RuleDefinitions.AttackModifierMethod.AddAbilityScoreBonus when
                    !string.IsNullOrEmpty(provider.AttackRollAbilityScore):
                    num += AttributeDefinitions.ComputeAbilityScoreModifier(
                        hero.TryGetAttributeValue(provider.AttackRollAbilityScore));
                    break;
                case RuleDefinitions.AttackModifierMethod.None:
                    break;
                case RuleDefinitions.AttackModifierMethod.FlatValue:
                    break;
                case RuleDefinitions.AttackModifierMethod.AddProficiencyBonus:
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
                case RuleDefinitions.AttackModifierMethod.SourceConditionAmount:
                    num = hero.FindFirstConditionHoldingFeature(provider as FeatureDefinition).Amount;
                    break;
                case RuleDefinitions.AttackModifierMethod.AddAbilityScoreBonus when
                    !string.IsNullOrEmpty(provider.DamageRollAbilityScore):
                    num += AttributeDefinitions.ComputeAbilityScoreModifier(
                        hero.TryGetAttributeValue(provider.DamageRollAbilityScore));
                    break;
                case RuleDefinitions.AttackModifierMethod.None:
                    break;
                case RuleDefinitions.AttackModifierMethod.FlatValue:
                    break;
                case RuleDefinitions.AttackModifierMethod.AddProficiencyBonus:
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
            List<IAttackModificationProvider> attackModifiers)
        {
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
            //PATCH: Allows changing what attribute is used for weapon's attack and damage rolls
            var modifiers = __instance.GetSubFeaturesByType<IModifyAttackAttributeForWeapon>();

            var mods = modifiers;

            if (__result.sourceObject is RulesetItem item)
            {
                mods = item.GetSubFeaturesByType<IModifyAttackAttributeForWeapon>();
                mods.AddRange(modifiers);
            }

            foreach (var modifier in mods)
            {
                modifier.ModifyAttribute(__instance, __result, weapon, canAddAbilityDamageBonus);
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
            //PATCH: Allows adding extra attack modes
            __instance.GetSubFeaturesByType<IAddExtraAttack>()
                .ForEach(provider => provider.TryAddExtraAttack(__instance));

            //PATCH: Allows changing damage and other stats of an attack mode
            var modifiers = __instance.GetSubFeaturesByType<IModifyAttackModeForWeapon>();

            foreach (var attackMode in __instance.AttackModes)
            {
                var mods = modifiers;

                if (attackMode.sourceObject is RulesetItem item)
                {
                    mods = item.GetSubFeaturesByType<IModifyAttackModeForWeapon>();
                    mods.AddRange(modifiers);
                }

                foreach (var modifier in mods)
                {
                    modifier.ModifyAttackMode(__instance, attackMode);
                }
            }

            //PATCH: remove invalid attacks
            //used to prevent hand crossbows use with no free hand
            __instance.AttackModes.RemoveAll(mode => SrdAndHouseRulesContext.IsAttackModeInvalid(__instance, mode));

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
        [UsedImplicitly]
        public static bool Prefix(RulesetCharacterHero __instance,
            RulesetAttackMode attackMode,
            IAttackModificationProvider attackModifier)
        {
            var feature = attackModifier as FeatureDefinition;
            if (feature == null)
            {
                return true;
            }

            var provider = feature.GetFirstSubFeatureOfType<IRankProvider>();
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
            ref RuleDefinitions.AttackProximity proximity)
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
                                   main.ItemDefinition.WeaponDescription.IsMonkWeaponOrUnarmed() ||
                                   __instance.IsMonkWeapon(main.ItemDefinition);

            var isOffMonkWeapon = off == null || !off.ItemDefinition.IsWeapon ||
                                  off.ItemDefinition.WeaponDescription.IsMonkWeaponOrUnarmed() ||
                                  __instance.IsMonkWeapon(off.ItemDefinition);

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
            if (!SharedSpellsContext.IsMulticaster(__instance))
            {
                return true;
            }

            var allRitualSpells = new List<SpellDefinition>();
            var magicAffinities = new List<FeatureDefinition>();

            ritualSpells.SetRange(allRitualSpells);

            __instance.EnumerateFeaturesToBrowse<FeatureDefinitionMagicAffinity>(magicAffinities);

            // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
            foreach (var featureDefinitionMagicAffinity in magicAffinities
                         .OfType<FeatureDefinitionMagicAffinity>())
            {
                if (featureDefinitionMagicAffinity.RitualCasting == RuleDefinitions.RitualCasting.None)
                {
                    continue;
                }

                foreach (var spellRepertoire in __instance.SpellRepertoires)
                {
                    // this is very similar to switch statement TA wrote but with spell loops outside
                    // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                    switch (featureDefinitionMagicAffinity.RitualCasting)
                    {
                        case RuleDefinitions.RitualCasting.PactTomeRitual:
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

                        case RuleDefinitions.RitualCasting.Selection:
                        {
                            var spells = spellRepertoire.KnownSpells
                                .Where(knownSpell =>
                                    knownSpell.Ritual && spellRepertoire.MaxSpellLevelOfSpellCastingLevel >=
                                    knownSpell.SpellLevel);

                            allRitualSpells.AddRange(spells);

                            break;
                        }

                        case RuleDefinitions.RitualCasting.Prepared
                            when spellRepertoire.SpellCastingFeature.SpellReadyness ==
                                 RuleDefinitions.SpellReadyness.Prepared &&
                                 spellRepertoire.SpellCastingFeature.SpellKnowledge ==
                                 RuleDefinitions.SpellKnowledge.WholeList:
                        {
                            var maxSpellLevel = SharedSpellsContext.MaxSpellLevelOfSpellCastingLevel(spellRepertoire);
                            var spells = spellRepertoire.PreparedSpells
                                .Where(s => s.Ritual)
                                .Where(s => maxSpellLevel >= s.SpellLevel);

                            allRitualSpells.AddRange(spells);

                            break;
                        }
                        case RuleDefinitions.RitualCasting.Spellbook
                            when spellRepertoire.SpellCastingFeature.SpellKnowledge ==
                                 RuleDefinitions.SpellKnowledge.Spellbook:
                        {
                            __instance.CharacterInventory.EnumerateAllItems(__instance.Items);

                            var maxSpellLevel = SharedSpellsContext.MaxSpellLevelOfSpellCastingLevel(spellRepertoire);
                            var spells = __instance.Items
                                .OfType<RulesetItemSpellbook>()
                                .SelectMany(x => x.ScribedSpells)
                                .ToList();

                            spells = spells
                                .Where(s => s.Ritual)
                                .Where(s => maxSpellLevel >= s.SpellLevel)
                                .ToList();

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
            var maxLevel = Gui.Game == null ? Level20Context.GameMaxLevel : Gui.Game.CampaignDefinition.LevelCap;
            var levelCap = Main.Settings.EnableLevel20 ? Level20Context.ModMaxLevel : maxLevel;
            var level = __instance.TryGetAttributeValue(AttributeDefinitions.CharacterLevel);
            var experience = __instance.TryGetAttributeValue(AttributeDefinitions.Experience);
            var nextLevelThreshold = RuleDefinitions.ComputeNextLevelThreshold(level + 1);

            __result = ((Main.Settings.NoExperienceOnLevelUp && Gui.GameLocation != null) ||
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
            var isLevelingUp = LevelUpContext.IsLevelingUp(__instance);

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

            if (providers.Empty())
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
                if (activity.functor != PowerBundleContext.UseCustomRestPowerFunctorName)
                {
                    return false;
                }

                if (!DatabaseHelper.TryGetDefinition<FeatureDefinitionPower>(activity.StringParameter, out var power))
                {
                    return false;
                }

                var p = activity.GetFirstSubFeatureOfType<RestActivityValidationParams>()
                        ?? new RestActivityValidationParams(true, true);

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
            //TODO: convert this to an interface
            WizardBladeDancer.OnItemEquipped(__instance);
            CollegeOfWarDancer.OnItemEquipped(__instance);
            FairyRaceBuilder.OnItemEquipped(__instance);
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
                currentAmmunitionSlot.EquipedItem.ItemDefinition == null ||
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
            var enumerate = new Action<
                RulesetCharacter,
                List<FeatureDefinition>,
                Dictionary<FeatureDefinition, RuleDefinitions.FeatureOrigin>
            >(EnumerateFeatureDefinitionSavingThrowAffinity).Method;

            //PATCH: make ISpellCastingAffinityProvider from dynamic item properties apply to repertoires
            return instructions.ReplaceEnumerateFeaturesToBrowse("FeatureDefinitionSavingThrowAffinity",
                -1, "RulesetCharacterHero.FindBestRegenerationFeature",
                new CodeInstruction(OpCodes.Call, enumerate));
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
            var enumerate = new Action<
                RulesetCharacter,
                List<FeatureDefinition>,
                Dictionary<FeatureDefinition, RuleDefinitions.FeatureOrigin>
            >(EnumerateFeatureDefinitionSavingThrowAffinity).Method;

            //PATCH: make ISpellCastingAffinityProvider from dynamic item properties apply to repertoires
            return instructions.ReplaceEnumerateFeaturesToBrowse("FeatureDefinitionSavingThrowAffinity",
                -1, "RulesetCharacterHero.ComputeBaseSavingThrowBonus",
                new CodeInstruction(OpCodes.Call, enumerate));
        }
    }
}
