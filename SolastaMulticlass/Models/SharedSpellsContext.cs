using System;
using System.Collections.Generic;
using System.Linq;
using static FeatureDefinitionCastSpell;
using static SolastaCommunityExpansion.Classes.Warlock.WarlockSpells;
using static SolastaCommunityExpansion.Level20.SpellsHelper;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;
using static SolastaModApi.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaMulticlass.Models.IntegrationContext;

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

        internal static Dictionary<CharacterClassDefinition, CasterType> ClassCasterType { get; } = new()
        {
            { Cleric, CasterType.Full },
            { Druid, CasterType.Full },
            { Sorcerer, CasterType.Full },
            { Wizard, CasterType.Full },
            { Paladin, CasterType.Half },
            { Ranger, CasterType.Half }
            // these are added during load
            //{ TinkererClass, CasterType.HalfRoundUp },
            //{ WitchClass, CasterType.Full },
        };

        internal static Dictionary<CharacterSubclassDefinition, CasterType> SubclassCasterType { get; } = new()
        {
            { MartialSpellblade, CasterType.OneThird },
            { RoguishShadowCaster, CasterType.OneThird }
            // these are added during load
            //{ ConArtistSubclass, CasterType.OneThird }, // ChrisJohnDigital
            //{ SpellShieldSubclass, CasterType.OneThird } // ChrisJohnDigital
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
            if (characterClassDefinition != null && ClassCasterType.ContainsKey(characterClassDefinition) && ClassCasterType[characterClassDefinition] != CasterType.None)
            {
                return ClassCasterType[characterClassDefinition];
            }

            if (characterSubclassDefinition != null && SubclassCasterType.ContainsKey(characterSubclassDefinition))
            {
                return SubclassCasterType[characterSubclassDefinition];
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
        internal static bool IsMulticaster(RulesetCharacterHero rulesetCharacterHero) =>
            rulesetCharacterHero != null
            && rulesetCharacterHero.SpellRepertoires
                .Count(sr => sr.SpellCastingFeature.SpellCastingOrigin != CastingOrigin.Race) > 1;

        // need the null check for companions who don't have repertoires
        internal static bool IsSharedcaster(RulesetCharacterHero rulesetCharacterHero) =>
            rulesetCharacterHero != null
            && rulesetCharacterHero.SpellRepertoires
                .Where(sr => sr.SpellCastingClass != WarlockClass)
                .Count(sr => sr.SpellCastingFeature.SpellCastingOrigin != CastingOrigin.Race) > 1;

        internal static bool IsWarlock(CharacterClassDefinition characterClassDefinition) =>
            characterClassDefinition == WarlockClass;

        // need the null check for companions who don't have repertoires
        internal static int GetWarlockLevel(RulesetCharacterHero rulesetCharacterHero)
        {
            if (rulesetCharacterHero == null)
            {
                return 0;
            }

            var warlockLevel = 0;
            var warlock = rulesetCharacterHero.ClassesAndLevels.Keys.FirstOrDefault(x => x == WarlockClass);

            if (warlock != null)
            {
                warlockLevel = rulesetCharacterHero.ClassesAndLevels[warlock];
            }

            return warlockLevel;
        }

        internal static int GetWarlockSpellLevel(RulesetCharacterHero rulesetCharacterHero)
        {
            var warlockLevel = GetWarlockLevel(rulesetCharacterHero);

            if (warlockLevel > 0)
            {
                return WarlockCastingSlots[warlockLevel - 1].Slots.IndexOf(0);
            }

            return 0;
        }

        internal static int GetWarlockMaxSlots(RulesetCharacterHero rulesetCharacterHero)
        {
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
            if (rulesetCharacterHero == null)
            {
                return 0;
            }
            else if (IsWarlock(filterCharacterClassDefinition))
            {
                return GetWarlockSpellLevel(rulesetCharacterHero);
            }
            else if (!IsMulticaster(rulesetCharacterHero))
            {
                var casterRepertoire = rulesetCharacterHero.SpellRepertoires
                    .Where(sr => !IsWarlock(sr.SpellCastingClass))
                    .FirstOrDefault(sr => sr.SpellCastingFeature.SpellCastingOrigin != CastingOrigin.Race);

                return casterRepertoire == null ? 0 : casterRepertoire.MaxSpellLevelOfSpellCastingLevel;
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

                return casterLevelContext.GetSpellLevel();
            }
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
            RecoverySlots.Add("ArtificerInfusionSpellRefuelingRing", TinkererClass);

            // These need to be later added to avoid a null object in the collection
            ClassCasterType.Add(TinkererClass, CasterType.HalfRoundUp);
            ClassCasterType.Add(WitchClass, CasterType.Full);

            SubclassCasterType.Add(ConArtistSubclass, CasterType.OneThird);
            SubclassCasterType.Add(SpellShieldSubclass, CasterType.OneThird);
        }

        internal const int PACT_MAGIC_SLOT_TAB_INDEX = -1;
    }
}
