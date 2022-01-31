using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.SrdAndHouseRules
{
    // fix twinned spells offering
    internal static class RulesetImplementationManagerLocationPatcher
    {
        [HarmonyPatch(typeof(RulesetImplementationManagerLocation), "IsMetamagicOptionAvailable")]
        [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
        internal static class RulesetImplementationManagerLocation_IsMetamagicOptionAvailable
        {
            private static readonly string[] AllowedSpellsIfHeroBelowLevel5 = new string[]
            {
                "EldritchBlastCantrip",
                "RepellingEldritchBlastCantrip",
                "WarlockRepellingBlastCantrip"
            };

            private static readonly string[] AllowedSpellsIfNotUpcast = new string[]
            {
                // level 1
                "CharmPerson",
                "Longstrider",

                // level 2
                "Blindness",
                "HoldPerson",
                "Invisibility",

                // level 3
                "Fly",

                // level 4
                "Banishment",

                // level 5
                "HoldMonster"
            };

            private static readonly string[] AlwaysAllowedSpells = new string[]
            {
                // cantrips
                "AnnoyingBee",
                "ChillTouch",
                "Dazzle",
                "FireBolt",
                "PoisonSpray",
                "RayOfFrost",
                "ShadowDagger",
                "ShockingGrasp",
                "Guidance",
                "Resistance",
                "SacredFlame",
                "SpareTheDying",
                "Shine",

                // level 1
                "HideousLaughter",
                "Jump",
                "MageArmor",
                "ProtectionFromEvilGood",
                "HuntersMark",
                "CureWounds",
                "Heroism",
                "ShieldOfFaith",
                "GuidingBolt",
                "HealingWord",
                "InflictWounds",
                "AnimalFriendship",
                "VulnerabilityHexSpell",

                // level 2
                "AcidArrow",
                "Darkvision",
                "Levitate",
                "RayOfEnfeeblement",
                "SpiderClimb",
                "Barkskin",
                "LesserRestoration",
                "ProtectionFromPoison",
                "EnhanceAbility",

                // level 3
                "BestowCurse",
                "DispelMagic",
                "Haste",
                "ProtectionFromEnergy",
                "RemoveCurse",
                "Tongues",
                "Revivify",

                // level 4
                "Blight",
                "GreaterInvisibility",
                "PhantasmalKiller",
                "Stoneskin",
                "FreedomOfMovement",
                "DeathWard",
                "DominateBeast",

                // level 5
                "DominatePerson",
                "GreaterRestoration",
                "RaiseDead"
            };

            internal static void Postfix(
                ref bool __result,
                RulesetEffectSpell rulesetEffectSpell,
                RulesetCharacter caster,
                MetamagicOptionDefinition metamagicOption,
                ref string failure)
            {
                if (!Main.Settings.FixSorcererTwinnedLogic || metamagicOption != SolastaModApi.DatabaseHelper.MetamagicOptionDefinitions.MetamagicTwinnedSpell)
                {
                    return;
                }

                var rulesetSpellRepertoire = rulesetEffectSpell.SpellRepertoire;
                var spellDefinition = rulesetEffectSpell.SpellDefinition;
                var spellLevel = spellDefinition?.SpellLevel;
                var slotLevel = rulesetEffectSpell.SlotLevel;
                int classLevel;

                if (spellDefinition != null && rulesetSpellRepertoire?.KnownCantrips.Contains(spellDefinition) == true)
                {
                    classLevel = (caster as RulesetCharacterHero).ClassesHistory.Count;
                }
                else
                {
                    classLevel = rulesetEffectSpell.GetClassLevel(caster);
                }

                var isAlwaysAllowedSpell = Array.Exists(AlwaysAllowedSpells, x => x == spellDefinition.Name);
                var isAllowedIfNotUpCastSpell = Array.Exists(AllowedSpellsIfNotUpcast, x => x == spellDefinition.Name);
                var isAllowedIfHeroBelowLevel5Spell = Array.Exists(AllowedSpellsIfHeroBelowLevel5, x => x == spellDefinition.Name);

                if (isAlwaysAllowedSpell || (isAllowedIfNotUpCastSpell && spellLevel == slotLevel) || (isAllowedIfHeroBelowLevel5Spell && classLevel < 5))
                {
                    return;
                }

                var postfix = "";

                if (!isAllowedIfHeroBelowLevel5Spell)
                {
                    postfix = " above level 4";
                }
                else if (!isAllowedIfNotUpCastSpell)
                {
                    postfix = " and upcasted";
                }

                failure = $"Cannot be twinned{postfix}";

                __result = false;
            }
        }
    }
}
