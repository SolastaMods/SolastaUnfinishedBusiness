using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Classes;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Displays;
#if DEBUG
using SolastaUnfinishedBusiness.DataMiner;
#endif

namespace SolastaUnfinishedBusiness.Models;

internal static class BootContext
{
    internal static void Startup()
    {
#if DEBUG
        ItemDefinitionVerification.Load();
        EffectFormVerification.Load();
#endif
        GameUiContext.ModifyActionMaps();

        // STEP 0: Cache TA definitions for diagnostics and export
        DiagnosticsContext.CacheTaDefinitions();

        // Load Portraits, Translations and Resources Locator after
        PortraitsContext.Load();
        TranslatorContext.Load();
        ResourceLocatorContext.Load();

        // Fixes spell slots and progressions early on
        FixesContext.Load();

        // Create our Content Pack for anything that gets further created
        CeContentPackContext.Load();
        CustomActionIdContext.Load();

        // Cache all Merchant definitions and what item types they sell
        MerchantTypeContext.Load();

        // Custom Conditions must load as early as possible
        CustomConditionsContext.Load();

        // AI Context
        AiContext.Load();

        //
        // custom stuff that can be loaded in any order
        //

        CustomReactionsContext.Load();
        CustomWeaponsContext.Load();
        CustomItemsContext.Load();
        PowerBundleContext.Load();

        //
        // other stuff that can be loaded in any order
        //

        ToolsContext.Load();
        CharacterExportContext.Load();
        DmProEditorContext.Load();
        GameUiContext.Load();

        // Fighting Styles must be loaded before feats to allow feats to generate corresponding fighting style ones.
        FightingStyleContext.Load();

        // Backgrounds may rely on spells and powers being in the DB before they can properly load.
        BackgroundsContext.Load();

        // Races may rely on spells and powers being in the DB before they can properly load.
        RacesContext.Load();

        // Classes may rely on spells and powers being in the DB before they can properly load.
        ClassesContext.Load();

        // Subclasses may rely on spells and powers being in the DB before they can properly load.
        SubclassesContext.Load();

        // Level 20 must always load after classes and subclasses
        Level20Context.Load();

        // Item Options must be loaded after Item Crafting
        ItemCraftingMerchantContext.Load();
        RecipeHelper.AddRecipeIcons();

        MerchantContext.Load();

        ServiceRepository.GetService<IRuntimeService>().RuntimeLoaded += _ =>
        {
            // There are feats that need all character classes loaded before they can properly be setup.
            FeatsContext.LateLoad();

            // Late initialized to allow feats and races from other mods
            CharacterContext.LateLoad();

            // Custom invocations
            InvocationsContext.LateLoad();

            // Custom metamagic
            MetamagicContext.LateLoad();

            // SRD rules switches
            SrdAndHouseRulesContext.LateLoad();

            // Action Switching
            ActionSwitching.LateLoad();

            // Vanilla Fixes
            FixesContext.LateLoad();

            // Level 20 - patching and final configs
            Level20Context.LateLoad();

            // Multiclass - patching and final configs
            MulticlassContext.LateLoad();

            // Spells context need Level 20 and Multiclass to properly register spells
            SpellsContext.LateLoad();

            // Shared Slots - patching and final configs
            SharedSpellsContext.LateLoad();

            // Set anything on subs that depends on spells and others
            SubclassesContext.LateLoad();
            InventorClass.LateLoadSpellStoringItem();
            LightingAndObscurementContext.LateLoad();

            // Save by location initialization depends on services to be ready
            SaveByLocationContext.LateLoad();

            // Recache all gui collections
            GuiWrapperContext.Recache();

            // Cache CE definitions for diagnostics and export
            DiagnosticsContext.CacheCeDefinitions();

            // Dump documentations to mod folder
            DocumentationContext.DumpDocumentation();
            ModUi.LoadTabletopDefinitions();

            AddExtraTooltipDefinitions();

            // Manages update or welcome messages
            UpdateContext.Load();

            // Log invalid user campaign
            LogMissingReferencesInUserCampaigns();

            // Fix condition UI
            DatabaseHelper.FeatureDefinitionCombatAffinitys.CombatAffinityForeknowledge.GuiPresentation.Description =
                Gui.NoLocalization;

            // Enable mod
            Main.Enable();
        };
    }

    private static void AddExtraTooltipDefinitions()
    {
        if (ServiceRepository.GetService<IGuiService>() is not GuiManager gui)
        {
            return;
        }

        var definition = gui.tooltipClassDefinitions[GuiFeatDefinition.tooltipClass];

        var index = definition.tooltipFeatures.FindIndex(f =>
            f.scope == TooltipDefinitions.Scope.All &&
            f.featurePrefab.GetComponent<TooltipFeature>() is TooltipFeaturePrerequisites);

        if (index >= 0)
        {
            var custom = GuiTooltipClassDefinitionBuilder
                .Create(gui.tooltipClassDefinitions["ItemDefinition"], CustomItemTooltipProvider.ItemWithPreReqsTooltip)
                .SetGuiPresentationNoContent()
                .AddTooltipFeature(definition.tooltipFeatures[index])
                //TODO: figure out why only background widens, but not content
                // .SetPanelWidth(400f) //items have 340f by default
                .AddToDB();

            gui.tooltipClassDefinitions.Add(custom.Name, custom);
        }

        //make condition description visible on both modes
        definition = gui.tooltipClassDefinitions[GuiActiveCondition.tooltipClass];
        index = definition.tooltipFeatures.FindIndex(f =>
            f.scope == TooltipDefinitions.Scope.Simplified &&
            f.featurePrefab.GetComponent<TooltipFeature>() is TooltipFeatureDescription);

        if (index < 0)
        {
            return;
        }

        //since FeatureInfo is a struct we get here a copy
        var info = definition.tooltipFeatures[index];
        //modify it
        info.scope = TooltipDefinitions.Scope.All;
        //and then put copy back
        definition.tooltipFeatures[index] = info;
    }

    private static void LogMissingReferencesInUserCampaigns()
    {
        if (!Main.Settings.EnableLoggingInvalidReferencesInUserCampaigns)
        {
            return;
        }

        var userCampaigns = Directory.GetFiles(TacticalAdventuresApplication.UserCampaignsDirectory);

        foreach (var userCampaign in userCampaigns)
        {
            try
            {
                var payload = File.ReadAllText(userCampaign);
                var infoJson = JsonConvert.DeserializeObject<JObject>(payload);

                foreach (var userItem in infoJson["userItems"]!)
                {
                    var referenceDefinition = userItem["referenceDefinition"]!.Value<string>();

                    if (DatabaseRepository.GetDatabase<ItemDefinition>().TryGetElement(referenceDefinition, out _))
                    {
                        continue;
                    }

                    Main.Error(
                        $"User campaign {Path.GetFileName(userCampaign)} has an invalid item reference: {referenceDefinition}");
                }

                foreach (var userMonster in infoJson["userMonsters"]!)
                {
                    var referenceDefinition = userMonster["referenceDefinition"]!.Value<string>();

                    if (DatabaseRepository.GetDatabase<MonsterDefinition>().TryGetElement(referenceDefinition, out _))
                    {
                        continue;
                    }

                    Main.Error(
                        $"User campaign {Path.GetFileName(userCampaign)} has an invalid monster reference: {referenceDefinition}");
                }
            }
            catch
            {
                Main.Error($"User campaign {Path.GetFileName(userCampaign)} is really messed up.");
            }
        }
    }
}
