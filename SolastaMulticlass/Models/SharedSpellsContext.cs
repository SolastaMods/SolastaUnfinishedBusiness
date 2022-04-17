using System;
using System.Collections.Generic;
using System.Linq;
using static FeatureDefinitionCastSpell;
using static SolastaCommunityExpansion.Classes.Warlock.WarlockSpells;
using static SolastaCommunityExpansion.Level20.SpellsHelper;

namespace SolastaMulticlass.Models
{
    internal enum CasterType
    {
        None = 0,
        Full = 2,
        Half = 4,
        HalfRoundUp = 5,
        OneThird = 6
    }

    internal static class SharedSpellsContext
    {
        internal static Dictionary<string, BaseDefinition> RecoverySlots { get; } = new();

        internal static Dictionary<string, CasterType> ClassCasterType { get; } = new()
        {
            //{ IntegrationContext.CLASS_ALCHEMIST, CasterType.HalfRoundUp },
            //{ IntegrationContext.CLASS_BARD, CasterType.Full },
            { IntegrationContext.CLASS_TINKERER, CasterType.HalfRoundUp },
            { IntegrationContext.CLASS_WITCH, CasterType.Full },
            { RuleDefinitions.ClericClass, CasterType.Full },
            { RuleDefinitions.DruidClass, CasterType.Full },
            { RuleDefinitions.SorcererClass, CasterType.Full },
            { RuleDefinitions.WizardClass, CasterType.Full },
            { RuleDefinitions.PaladinClass, CasterType.Half },
            { RuleDefinitions.RangerClass, CasterType.Half }
        };

        internal static Dictionary<string, CasterType> SubclassCasterType { get; } = new()
        {
            //{ "BarbarianSubclassPrimalPathOfWarShaman", CasterType.OneThird }, // Holic
            //{ "MartialEldritchKnight", CasterType.OneThird }, // Holic
            { "MartialSpellblade", CasterType.OneThird },
            { "RoguishShadowCaster", CasterType.OneThird },
            { "RoguishConArtist", CasterType.OneThird }, // ChrisJohnDigital
            { "FighterSpellShield", CasterType.OneThird } // ChrisJohnDigital
        };

        internal class CasterLevelContext
        {
            private readonly Dictionary<CasterType, int> levels;

            internal CasterLevelContext()
            {
                levels = new Dictionary<CasterType, int>
                {
                    { CasterType.None, 0 },
                    { CasterType.Full, 0 },
                    { CasterType.Half, 0 },
                    { CasterType.HalfRoundUp, 0 },
                    { CasterType.OneThird, 0 },
                };
            }

            internal void IncrementCasterLevel(CasterType casterType, int increment) => levels[casterType] += increment;

            internal int GetCasterLevel()
            {
                var casterLevel = 0;

                // Full Casters
                casterLevel += levels[CasterType.Full];

                // Tinkerer / ...
                if (levels[CasterType.HalfRoundUp] == 1)
                {
                    casterLevel++;
                }
                // Half Casters
                else
                {
                    casterLevel += (int)Math.Floor(levels[CasterType.HalfRoundUp] / 2.0);
                }

                casterLevel += (int)Math.Floor(levels[CasterType.Half] / 2.0);

                // Con Artist / ...
                casterLevel += (int)Math.Floor(levels[CasterType.OneThird] / 3.0);

                return casterLevel;
            }

            internal int GetSpellLevel()
            {
                var casterLevel = GetCasterLevel();

                return casterLevel == 0 ? 0 : FullCastingSlots.First(x => x.Level == GetCasterLevel()).Slots.IndexOf(0);
            }
        }

        private static CasterType GetCasterTypeForClassOrSubclass(CharacterClassDefinition characterClassDefinition, CharacterSubclassDefinition characterSubclassDefinition)
        {
            if (characterClassDefinition != null && ClassCasterType.ContainsKey(characterClassDefinition.Name) && ClassCasterType[characterClassDefinition.Name] != CasterType.None)
            {
                return ClassCasterType[characterClassDefinition.Name];
            }

            if (characterSubclassDefinition != null && SubclassCasterType.ContainsKey(characterSubclassDefinition.Name))
            {
                return SubclassCasterType[characterSubclassDefinition.Name];
            }

            return CasterType.None;
        }

        internal static RulesetCharacterHero GetHero(string name)
        {
            // try to get hero from game campaign
            var gameCampaign = Gui.GameCampaign;

            if (gameCampaign != null)
            {
                var gameCampaignCharacter = gameCampaign.Party.CharactersList.Find(x => x.RulesetCharacter.Name == name);

                if (gameCampaignCharacter != null
                    && gameCampaignCharacter.RulesetCharacter is RulesetCharacterHero rulesetCharacterHero)
                {
                    return rulesetCharacterHero;
                }
            }

            // otherwise gets hero from level up context
            var hero = LevelUpContext.GetHero(name);

            if (hero != null)
            {
                return hero;
            }

            // finally falls back to inspection [when browsing hero in char pool]
            return InspectionPanelContext.SelectedHero;
        }

        // need the null check for companions who don't have repertoires
        internal static bool IsMulticaster(RulesetCharacterHero rulesetCharacterHero) => rulesetCharacterHero == null
            ? false
            : rulesetCharacterHero.SpellRepertoires
                .Count(sr => sr.SpellCastingFeature.SpellCastingOrigin != CastingOrigin.Race) > 1;

        // need the null check for companions who don't have repertoires
        internal static bool IsSharedcaster(RulesetCharacterHero rulesetCharacterHero) => rulesetCharacterHero == null
            ? false
            : rulesetCharacterHero.SpellRepertoires
                .Where(sr => sr.SpellCastingClass != IntegrationContext.WarlockClass)
                .Count(sr => sr.SpellCastingFeature.SpellCastingOrigin != CastingOrigin.Race) > 1;

        internal static bool IsWarlock(CharacterClassDefinition characterClassDefinition) =>
            characterClassDefinition == IntegrationContext.WarlockClass;

        // need the null check for companions who don't have repertoires
        internal static int GetWarlockLevel(RulesetCharacterHero rulesetCharacterHero)
        {
            if (rulesetCharacterHero == null)
            {
                return 0;
            }

            var warlockLevel = 0;
            var warlock = rulesetCharacterHero.ClassesAndLevels.Keys.FirstOrDefault(x => x == IntegrationContext.WarlockClass);

            if (warlock != null)
            {
                warlockLevel = rulesetCharacterHero.ClassesAndLevels[warlock];
            }

            return warlockLevel;
        }

        // need the null check for companions who don't have repertoires
        internal static int GetWarlockSpellLevel(RulesetCharacterHero rulesetCharacterHero)
        {
            if (rulesetCharacterHero == null)
            {
                return 0;
            }

            var warlockLevel = GetWarlockLevel(rulesetCharacterHero);

            if (warlockLevel > 0)
            {
                return WarlockCastingSlots[warlockLevel - 1].Slots.IndexOf(0);
            }

            return 0;
        }

        // need the null check for companions who don't have repertoires
        internal static int GetWarlockMaxSlots(RulesetCharacterHero rulesetCharacterHero)
        {
            if (rulesetCharacterHero == null)
            {
                return 0;
            }

            var warlockLevel = GetWarlockLevel(rulesetCharacterHero);

            if (warlockLevel > 0)
            {
                return WarlockCastingSlots[warlockLevel - 1].Slots[0];
            }

            return 0;
        }

        internal static RulesetSpellRepertoire GetWarlockSpellRepertoire(RulesetCharacterHero rulesetCharacterHero) =>
            rulesetCharacterHero.SpellRepertoires.FirstOrDefault(x => IsWarlock(x.SpellCastingClass));

        internal static int GetSharedCasterLevel(RulesetCharacterHero rulesetCharacterHero)
        {
            if (rulesetCharacterHero == null || rulesetCharacterHero.ClassesAndLevels == null)
            {
                return 0;
            }

            var casterLevelContext = new CasterLevelContext();

            foreach (var classAndLevel in rulesetCharacterHero.ClassesAndLevels)
            {
                var currentCharacterClassDefinition = classAndLevel.Key;

                rulesetCharacterHero.ClassesAndSubclasses.TryGetValue(currentCharacterClassDefinition, out var currentCharacterSubclassDefinition);

                var casterType = GetCasterTypeForClassOrSubclass(currentCharacterClassDefinition, currentCharacterSubclassDefinition);

                casterLevelContext.IncrementCasterLevel(casterType, classAndLevel.Value);
            }

            return casterLevelContext.GetCasterLevel();
        }

        internal static int GetSharedSpellLevel(RulesetCharacterHero rulesetCharacterHero)
        {
            var sharedCasterLevel = GetSharedCasterLevel(rulesetCharacterHero);

            if (sharedCasterLevel > 0)
            {
                return FullCastingSlots[GetSharedCasterLevel(rulesetCharacterHero) - 1].Slots.IndexOf(0);
            }

            return 0;
        }

        internal static int GetClassSpellLevel(
            RulesetCharacterHero rulesetCharacterHero,
            CharacterClassDefinition filterCharacterClassDefinition,
            CharacterSubclassDefinition filterCharacterSubclassDefinition = null)
        {
            int classSpellLevel;

            if (IsWarlock(filterCharacterClassDefinition))
            {
                classSpellLevel = GetWarlockSpellLevel(rulesetCharacterHero);
            }
            else if (!IsMulticaster(rulesetCharacterHero))
            {
                var casterRepertoire = rulesetCharacterHero.SpellRepertoires
                    .Where(sr => !IsWarlock(sr.SpellCastingClass))
                    .FirstOrDefault(sr => sr.SpellCastingFeature.SpellCastingOrigin != CastingOrigin.Race);

                if (casterRepertoire != null)
                {
                    return casterRepertoire.MaxSpellLevelOfSpellCastingLevel;
                }

                return 1;
            }
            else
            {
                var casterLevelContext = new CasterLevelContext();

                if (rulesetCharacterHero != null && rulesetCharacterHero.ClassesAndLevels != null)
                {
                    foreach (var classAndLevel in rulesetCharacterHero.ClassesAndLevels)
                    {
                        var currentCharacterClassDefinition = classAndLevel.Key;

                        rulesetCharacterHero.ClassesAndSubclasses.TryGetValue(currentCharacterClassDefinition, out var currentCharacterSubclassDefinition);

                        if (filterCharacterClassDefinition == currentCharacterClassDefinition
                            || (filterCharacterSubclassDefinition != null && filterCharacterSubclassDefinition == currentCharacterSubclassDefinition))
                        {
                            var casterType = GetCasterTypeForClassOrSubclass(currentCharacterClassDefinition, currentCharacterSubclassDefinition);

                            casterLevelContext.IncrementCasterLevel(casterType, classAndLevel.Value);
                        }
                    }
                }

                classSpellLevel = casterLevelContext.GetSpellLevel();
            }

            return classSpellLevel;
        }

        internal static int GetCombinedSpellLevel(RulesetCharacterHero rulesetCharacterHero)
            => Math.Max(GetWarlockSpellLevel(rulesetCharacterHero), GetSharedSpellLevel(rulesetCharacterHero));

        internal static void Load()
        {
            // init the recovery slots tab
            var dbCharacterClassDefinition = DatabaseRepository.GetDatabase<CharacterClassDefinition>();

            foreach (var element in dbCharacterClassDefinition)
            {
                var powers = element.FeatureUnlocks
                    .Select(x => x.FeatureDefinition)
                    .OfType<FeatureDefinitionPower>();

                foreach (var power in powers
                    .Where(x => x.ActivationTime == RuleDefinitions.ActivationTime.Rest)
                    .Where(x => x.RechargeRate != RuleDefinitions.RechargeRate.AtWill)
                    .Where(x => x.EffectDescription.EffectForms
                        .Where(x => x.FormType == EffectForm.EffectFormType.SpellSlots)
                        .Any(x => x.SpellSlotsForm.Type == SpellSlotsForm.EffectType.RecoverHalfLevelUp)))
                {
                    RecoverySlots.Add(power.Name, element);
                }
            }

            var dbCharacterSubclassDefinition = DatabaseRepository.GetDatabase<CharacterSubclassDefinition>();

            foreach (var element in dbCharacterSubclassDefinition)
            {
                var powers = element.FeatureUnlocks
                    .Select(x => x.FeatureDefinition)
                    .OfType<FeatureDefinitionPower>();

                foreach (var power in powers
                    .Where(x => x.ActivationTime == RuleDefinitions.ActivationTime.Rest)
                    .Where(x => x.RechargeRate != RuleDefinitions.RechargeRate.AtWill)
                    .Where(x => x.EffectDescription.EffectForms
                        .Where(x => x.FormType == EffectForm.EffectFormType.SpellSlots)
                        .Any(x => x.SpellSlotsForm.Type == SpellSlotsForm.EffectType.RecoverHalfLevelUp)))
                {
                    RecoverySlots.Add(power.Name, element);
                }
            }

            // Tinkerer special case
            RecoverySlots.Add("ArtificerInfusionSpellRefuelingRing", IntegrationContext.TinkererClass);
        }

        internal const int PACT_MAGIC_SLOT_TAB_INDEX = -1;
    }
}
