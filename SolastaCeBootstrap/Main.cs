using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using HarmonyLib;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using UnityEngine;
using UnityModManagerNet;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SolastaCeBootstrap
{
    public static class Main
    {
        internal const string GAME_FOLDER = ".";

        internal const string ProjectEnvironmentVariable = "SolastaCEProjectDir";

        internal static string ProjectFolder =
            Environment.GetEnvironmentVariable(ProjectEnvironmentVariable, EnvironmentVariableTarget.Machine);

        internal static string ApiFolder = (ProjectFolder ?? GAME_FOLDER) + "/SolastaCommunityExpansion/Api";

        internal static UnityModManager.ModEntry.ModLogger Logger { get; private set; }

        internal static UnityModManager.ModEntry Mod { get; private set; }

        internal static void Log(string msg)
        {
            Logger.Log(msg);
        }

        internal static void Error(Exception ex)
        {
            Logger?.Error(ex.ToString());
        }

        internal static void Error(string msg)
        {
            Logger?.Error(msg);
        }

        internal static void Warning(string msg)
        {
            Logger?.Warning(msg);
        }

        internal static bool Load(UnityModManager.ModEntry modEntry)
        {
            try
            {
                var harmony = new Harmony(modEntry.Info.Id);
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                Logger = modEntry.Logger;
                Mod = modEntry;
                Mod.OnGUI = OnGUI;

                return true;
            }
            catch (Exception e)
            {
                Logger.Log(e + "\n" + e.StackTrace);
            }

            return true;
        }

        internal static void OnGUI(UnityModManager.ModEntry _)
        {
            var patches = Harmony.GetAllPatchedMethods().SelectMany(method =>
            {
                var patchInfo = Harmony.GetPatchInfo(method);
                return patchInfo.Prefixes.Concat(patchInfo.Transpilers).Concat(patchInfo.Postfixes);
            });

            var owners = patches.Select(patchInfo => patchInfo.owner).Distinct();

            GUILayout.Label("");
            GUILayout.Label(". You can set the environment variable " + ProjectEnvironmentVariable +
                            " to customize the output folder");

            if (ProjectFolder == null)
                GUILayout.Label(". The output folder is set to " + "your game folder");
            else
                GUILayout.Label(". The output folder is set to " + ApiFolder);

            GUILayout.Label("");

            if (owners.Count() > 3)
            {
                GUILayout.Label("CE bootstrap must be the only installed mod");
            }
            else
            {
                if (GUILayout.Button("dump database helpers"))
                {
                    DatabaseHelpersExporter.Dump();
                    //ExtensionsExporter.Dump();
                }
            }

            GUILayout.Label("");
        }
    }

    internal static class DatabaseHelpersExporter
    {
        private static readonly string OutputPath = Main.ApiFolder + @"\DatabaseHelper";

        internal struct Asset
        {
            public string Name;
            public string AssetType;
            public string DatabaseType;
            public string Guid;
        }

        internal static Dictionary<string, List<Asset>> GetAssets()
        {
            var ignore = new HashSet<string>()
            {
                "Action",
                "ActionType",
                "AdventureLog",
                "BanterEvent",
                "BestiaryStats",
                "BestiaryTable",
                "Biome",
                "BlueprintCategory",
                "Calendar",
                "CampaignNodeType",
                "CharacterInteraction",
                "CharacterTemplate",
                "ConsoleTable",
                "ContentPack",
                "CreditsGroup",
                "CreditsTable",
                "Currency",
                "DailyLog",
                "DatabaseIndex",
                "DateTime",
                "Deity",
                "DieStyle",
                "DieType",
                "DifficultyPreset",
                "DocumentTable",
                "Encounter",
                "EncounterTable",
                "Environment",
                "EnvironmentEffect",
                "GadgetDefinition",
                "HumanoidMonsterPresentation",
                "Inventory",
                "Location",
                "LocationType",
                "NarrativeTree",
                "MapWaypoint",
                "MoviePlayback",
                "MusicalState",
                "NamedPlace",
                "NarrativeEventTable",
                "Notification",
                "PropBlueprint",
                "QuestStatus",
                "QuestTree",
                "RoomBlueprint",
                "SoundBanks",
                "SubtitleTable",
                "TravelActivity",
                "TravelEvent",
                "TravelJournal",
                "TravelPace",
                "TutorialSection",
                "TutorialStep",
                "TutorialSubsection",
                "TutorialTable",
                "TutorialToc",
                
                "VisualMood",
                "Voice"
            };
            var definitions = new Dictionary<string, List<Asset>>();

            foreach (var db in (Dictionary<Type, object>) AccessTools.Field(typeof(DatabaseRepository), "databases")
                         .GetValue(null))
            {
                var dbName = db.Key.Name;

                if (ignore.Any(x => dbName.StartsWith(x)))
                {
                    continue;
                }

                foreach (var baseDefinition in ((IEnumerable) db.Value).Cast<BaseDefinition>())
                {
                    var assetType = baseDefinition.GetType().FullName;

                    if (!definitions.ContainsKey(assetType)) definitions.Add(assetType, new List<Asset>());

                    definitions[assetType].Add(new Asset
                    {
                        Name = baseDefinition.Name,
                        AssetType = assetType,
                        DatabaseType = db.Key.FullName,
                        Guid = baseDefinition.GUID
                    });
                }
            }

            return definitions.OrderBy(d => d.Key).ToDictionary(v => v.Key, v => v.Value);
        }

        internal static void WriteLine(StringBuilder stringBuilder, string line, int indentCount = 0)
        {
            if (indentCount > 0) stringBuilder.Append(new string(' ', indentCount * 4));

            stringBuilder.Append(line);
            stringBuilder.Append("\r\n");
        }

        internal static string MemberName(string memberName)
        {
            memberName = memberName.Replace("+", "Plus");
            memberName = memberName.Replace("-", "_");
            memberName = memberName.Replace(" ", "_");
            memberName = memberName.Replace(".", "_");
            memberName = memberName.Replace("&", "_");
            memberName = memberName.Replace("~", "_");

            if (Regex.IsMatch(memberName, @"^\d")) memberName = '_' + memberName;

            return memberName;
        }

        internal static void WriteGetter(StringBuilder stringBuilder, Asset asset, int indentCount = 0)
        {
            var name = MemberName(asset.Name);

            if (asset.AssetType == asset.DatabaseType)
                WriteLine(stringBuilder,
                    $"public static {asset.AssetType} {name} {{ get; }} = GetDefinition<{asset.DatabaseType}>(\"{asset.Name}\", \"{asset.Guid}\");",
                    indentCount);
        }

        internal static void WriteSubclass(StringBuilder stringBuilder, string subClass, List<Asset> assets,
            int indentCount = 0)
        {
            WriteLine(stringBuilder, $"public static class {subClass}", indentCount);
            WriteLine(stringBuilder, "{", indentCount);

            foreach (var asset in assets.OrderBy(a => a.Name)) WriteGetter(stringBuilder, asset, indentCount + 1);

            WriteLine(stringBuilder, "}", indentCount);
        }

        internal static void Dump()
        {
            var assets = GetAssets();
            var sb = new StringBuilder();

            if (!Directory.Exists(OutputPath)) Directory.CreateDirectory(OutputPath);

            WriteLine(sb, "// this file is automatically generated");
            sb.AppendLine();
            WriteLine(sb, "namespace SolastaCommunityExpansion.Api;");
            WriteLine(sb, "public static partial class DatabaseHelper");
            WriteLine(sb, "{");

            foreach (var kvp in assets)
            {
                var arr = kvp.Key.Split('.');
                var subClass = arr[arr.Length - 1] + 's';

                WriteSubclass(sb, subClass, kvp.Value, 1);
            }

            WriteLine(sb, "}");

            File.WriteAllText(Path.Combine(OutputPath, "DatabaseHelper.cs"), sb.ToString());
        }
    }
#if false
    internal static class ExtensionsExporter
    {
        private const string pattern = @"^(\<(?<name>.*)\>k__BackingField)|(m_(?<name>.*))$";

        private static readonly string OutputPath = Main.ApiFolder + @"\Extensions";

        private static readonly Regex NameRegex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled);

        internal static void Dump()
        {
            if (!Directory.Exists(OutputPath)) Directory.CreateDirectory(OutputPath);

            var assemblyDir = Path.Combine(Main.GAME_FOLDER, @"Solasta_Data\Managed");
            var assembly = Assembly.LoadFrom(Path.Combine(assemblyDir, @"Assembly-CSharp.dll"));

            var exclusions = new List<string>
            {
                "PrefabPool",
                "AnimatorSavedParameters",
                "FunctorParametersDescription",
                "TextFragmentStyleDescription",
                "TrianglePool",
                "FadingRendererParameters"
            };

            var types =
                    Enumerable.Empty<Type>()

                        // Get all types derived from and including BaseDefinition 
                        .Concat(GetDerivedTypes(assembly, "BaseDefinition").Select(a => a.type))
                        // Get all types derived from and including RulesetEntity 
                        .Concat(GetDerivedTypes(assembly, "RulesetEntity").Select(a => a.type))
                        // Get all types derived from and including GuiWrapper 
                        .Concat(GetDerivedTypes(assembly, "GuiWrapper").Select(a => a.type))
                        // Get all types ending in Description and all derived from types ending in Description
                        .Concat(GetTypesEndingIn(assembly, "Description"))
                        // Get all types ending in Parameters and all derived from types ending in Parameters
                        .Concat(GetTypesEndingIn(assembly, "Parameters"))
                        // Get all types ending in Bone and all derived from types ending in Bone
                        .Concat(GetTypesEndingIn(assembly, "Bone"))
                        // Get all types ending in Presentation and all derived from types ending in Presentation
                        .Concat(GetTypesEndingIn(assembly, "Presentation"))
                        .Concat(GetTypesEndingIn(assembly, "Occurence"))
                        .Concat(GetTypesEndingIn(assembly, "Pool"))
                        .Concat(GetTypesEndingIn(assembly, "Advancement"))
                        .Concat(GetTypesEndingIn(assembly, "ByTag"))
                        .Concat(GetTypesEndingIn(assembly, "Form"))
                        .Concat(GetTypesEndingIn(assembly, "TreasureOption"))
                        .Concat(GetTypesEndingIn(assembly, "HeroEquipmentOption"))
                        .Concat(GetTypesEndingIn(assembly, "HeroEquipmentRow"))
                        .Concat(GetTypesEndingIn(assembly, "HeroEquipmentColumn"))
                        .Concat(GetTypesEndingIn(assembly, "DiceByRank"))
                        .Concat(GetTypesEndingIn(assembly, "RulesetUsablePower"))

                        //.Concat(GetTypes(assembly, "EffectDescription"))
                        // Eliminate duplicates
                        .GroupBy(t => t.FullName)
                        .Select(g => g.First())
                        // Order by name
                        .OrderBy(g => g.FullName)
                        // Exclusions for now
                        .Where(g => !exclusions.Contains(g.Name))
                        .ToList()
                ;

            // TODO: delete everything from output path?

            // set to true to create files, otherwise false for testing
            var createTheFiles = true;

            foreach (var t in types) CreateExtensions(t, createTheFiles);
        }

        private static IEnumerable<(Type type, Type baseType)> GetDerivedTypes(Assembly assembly, string baseType)
        {
            var types = assembly.GetTypes().ToList();

            var type = types.SingleOrDefault(t => t.Name == baseType);

            if (type == null) return Enumerable.Empty<(Type type, Type baseType)>();

            return Enumerable
                .Repeat((type, (Type) null), 1)
                .Concat(GetDerivedTypes(assembly, type));
        }

        private static IEnumerable<(Type type, Type baseType)> GetDerivedTypes(Assembly assembly, Type baseType)
        {
            var types = assembly.GetTypes()
                .Where(t => t.BaseType?.Name == baseType.Name)
                .Select(t => (type: t, baseType))
                .OrderBy(t => t.type.Name)
                .ToList();

            return types.Concat(types.SelectMany(t => GetDerivedTypes(assembly, t.type)));
        }

        private static IEnumerable<Type> GetTypesEndingIn(Assembly assembly, string suffix, bool caseSensitive = true)
        {
            var typesEndingIn = assembly.GetTypes()
                .Where(t => t.Name?.EndsWith(suffix,
                    caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase) ?? false)
                .OrderBy(t => t.Name)
                .ToList();

            return typesEndingIn.Concat(typesEndingIn.SelectMany(t =>
                GetDerivedTypes(assembly, t).Select(d => d.type)));
        }

        private static IEnumerable<Type> GetTypes(Assembly assembly, params string[] typeNames)
        {
            var tn = typeNames.ToHashSet();

            return assembly.GetTypes()
                .Where(t => tn.Contains(t.Name))
                .OrderBy(t => t.Name)
                .ToList();
        }

        private static void CreateExtensions(Type t, bool createFiles = false)
        {
            var sf = CompilationUnit()
                .AddUsings(
                    GetUsingSyntax("SolastaModApi.Infrastructure"),
                    GetUsingSyntax("AK.Wwise"),
                    GetUsingSyntax("UnityEngine"),
                    GetUsingSyntax("UnityEngine.UI"),
                    GetUsingSyntax("UnityEngine.AddressableAssets"),
                    GetUsingSyntax("System"),
                    GetUsingSyntax("System.Linq"),
                    GetUsingSyntax("System.Text"),
                    GetUsingSyntax("System.CodeDom.Compiler"),
                    GetUsingSyntax("TA.AI"),
                    GetUsingSyntax("TA"),
                    GetUsingSyntax("System.Collections.Generic"),
                    GetUsingSyntax("UnityEngine.Rendering.PostProcessing"),
                    GetUsingSyntax("static ActionDefinitions"),
                    GetUsingSyntax("static TA.AI.DecisionPackageDefinition"),
                    GetUsingSyntax("static TA.AI.DecisionDefinition"),
                    GetUsingSyntax("static RuleDefinitions"),
                    GetUsingSyntax("static BanterDefinitions"),
                    GetUsingSyntax("static Gui"),
                    GetUsingSyntax("static GadgetDefinitions"),
                    GetUsingSyntax("static BestiaryDefinitions"),
                    GetUsingSyntax("static CursorDefinitions"),
                    GetUsingSyntax("static AnimationDefinitions"),
                    GetUsingSyntax("static FeatureDefinitionAutoPreparedSpells"),
                    GetUsingSyntax("static FeatureDefinitionCraftingAffinity"),
                    GetUsingSyntax("static CharacterClassDefinition"),
                    GetUsingSyntax("static CreditsGroupDefinition"),
                    GetUsingSyntax("static SoundbanksDefinition"),
                    GetUsingSyntax("static CampaignDefinition"),
                    GetUsingSyntax("static GraphicsCharacterDefinitions"),
                    GetUsingSyntax("static GameCampaignDefinitions"),
                    GetUsingSyntax("static FeatureDefinitionAbilityCheckAffinity"),
                    GetUsingSyntax("static TooltipDefinitions"),
                    GetUsingSyntax("static BaseBlueprint"),
                    GetUsingSyntax("static MorphotypeElementDefinition")
                );

            var cd = ClassDeclaration($"{t.Name}Extensions")
                .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword),
                    Token(SyntaxKind.PartialKeyword))
                .AddAttributeLists(GetALS(t.Name));
            //.WithLeadingTrivia(GetClassComment($"{t.Name}Extensions"));

            AttributeListSyntax GetALS(string typeName)
            {
                return AttributeList(
                    new SeparatedSyntaxList<AttributeSyntax>()
                        .Add(
                            Attribute(
                                IdentifierName("TargetType"),
                                AttributeArgumentList(
                                    new SeparatedSyntaxList<AttributeArgumentSyntax>().Add(
                                        AttributeArgument(
                                            ParseExpression($"typeof({typeName})"))
                                    )
                                )
                            )
                        )
                        .Add(
                            Attribute(
                                IdentifierName("GeneratedCode"),
                                AttributeArgumentList(
                                    new SeparatedSyntaxList<AttributeArgumentSyntax>()
                                        .Add(
                                            AttributeArgument(
                                                ParseExpression("\"Community Expansion Extension Generator\""))
                                        )
                                        .Add(
                                            // probably best not to change the version often since that makes 
                                            // it impossible to detect changes to extensions
                                            AttributeArgument(
                                                ParseExpression("\"1.0.0\""))
                                        )
                                )
                            )
                        )
                );
            }

            var privateFields = t
                .GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic)
                .Select(f => new {f.Name, FieldInfo = f, f.FieldType, Type = SimplifyType(f.FieldType)});

            var writeablePublicProperties = t
                .GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public)
                .Where(pg => pg.CanWrite && pg.GetCustomAttributes(typeof(ObsoleteAttribute), true).Length == 0)
                .Select(pg => new
                {
                    pg.Name, pg.PropertyType, Type = SimplifyType(pg.PropertyType),
                    IsSetterProtected = pg.SetMethod.IsFamily
                });

            var obsoleteWriteablePublicPropertiesByName = t
                .GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public)
                .Where(pg => pg.GetCustomAttributes(typeof(ObsoleteAttribute), true).Length != 0)
                .Select(pg => pg.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var readablePublicProperties = t
                .GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public)
                .Where(pg => pg.CanRead && pg.GetCustomAttributes(typeof(ObsoleteAttribute), true).Length == 0)
                .Select(pg => new {pg, pg.Name, pg.PropertyType, Type = SimplifyType(pg.PropertyType)});

            var readablePublicPropertiesByName = readablePublicProperties
                .Select(pp => pp.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var writeablePublicPropertiesByName = writeablePublicProperties
                .Select(pp => pp.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var privateFieldsThatNeedWriter = privateFields
                .Where(f => !f.FieldType.IsGenericType && !writeablePublicPropertiesByName.Contains(f.Name) &&
                            f.FieldInfo.GetCustomAttributes(typeof(ObsoleteAttribute), true).Length == 0);

            var genericPrivateFieldsThatNeedGetters = privateFields
                .Where(f => f.FieldType.IsGenericType)
                .Where(f => !writeablePublicPropertiesByName.Contains(GetPropertyNameForField(f.FieldInfo)))
                .Where(f => !readablePublicPropertiesByName.Contains(GetPropertyNameForField(f.FieldInfo)))
                .Where(f => f.FieldInfo.GetCustomAttributes(typeof(ObsoleteAttribute), true).Length == 0);

            var genericReadablePublicProperties = readablePublicProperties
                .Where(f => f.PropertyType.IsGenericType)
                .Where(f => !obsoleteWriteablePublicPropertiesByName.Contains(f.Name));

            // ---------------------------------------------------------------------------
            var methods = privateFieldsThatNeedWriter
                .OrderBy(ftnw => ftnw.Name)
                .Where(ftnw => !obsoleteWriteablePublicPropertiesByName.Contains(ftnw.Name))
                .Select(f =>
                    {
                        if (t.IsSealed)
                            // Generic constraint not allowed with sealed type
                            return MethodDeclaration(ParseTypeName($"{t.Name}"),
                                    $"Set{GetPropertyNameForField(f.FieldInfo)}")
                                .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
                                .AddParameterListParameters(
                                    Parameter(Identifier("entity"))
                                        .WithType(ParseTypeName($"{t.Name}"))
                                        .AddModifiers(Token(SyntaxKind.ThisKeyword)),
                                    Parameter(Identifier("value"))
                                        .WithType(ParseTypeName(SimplifyType(f.FieldType)))
                                )
                                .WithBody(Block(ParseStatement($"entity.SetField(\"{f.Name}\", value);"),
                                    ParseStatement("return entity;")));
                        return MethodDeclaration(ParseTypeName("T"), $"Set{GetPropertyNameForField(f.FieldInfo)}")
                            .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
                            .AddTypeParameterListParameters(TypeParameter(Identifier("T")))
                            .AddParameterListParameters(
                                Parameter(Identifier("entity")).WithType(ParseTypeName("T"))
                                    .AddModifiers(Token(SyntaxKind.ThisKeyword)),
                                Parameter(Identifier("value")).WithType(ParseTypeName(SimplifyType(f.FieldType)))
                            )
                            .AddConstraintClauses(TypeParameterConstraintClause("T").WithConstraints(GetSL(t.Name)))
                            .WithBody(Block(ParseStatement($"entity.SetField(\"{f.Name}\", value);"),
                                ParseStatement("return entity;")));
                    }
                );

            // ---------------------------------------------------------------------------
            var genericGeneratedGetters = genericPrivateFieldsThatNeedGetters
                .Select(f => MethodDeclaration(ParseTypeName($"{SimplifyType(f.FieldType)}"),
                        $"Get{GetPropertyNameForField(f.FieldInfo)}")
                    .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
                    .AddTypeParameterListParameters(TypeParameter(Identifier("T")))
                    .AddParameterListParameters(Parameter(Identifier("entity")).WithType(ParseTypeName("T"))
                        .AddModifiers(Token(SyntaxKind.ThisKeyword)))
                    .AddConstraintClauses(TypeParameterConstraintClause("T").WithConstraints(GetSL(t.Name)))
                    .WithBody(Block(
                        ParseStatement($"return entity.GetField<{SimplifyType(f.FieldType)}>(\"{f.Name}\");")))
                );

            methods = methods.Concat(genericGeneratedGetters);

            // ---------------------------------------------------------------------------
            var generatedSetters =
                privateFieldsThatNeedWriter.Select(f => GetPropertyNameForField(f.FieldInfo))
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var writeablePublicPropertiesThatNeedWriters =
                writeablePublicProperties.Where(p => !generatedSetters.Contains(p.Name));

            methods = methods.Concat(writeablePublicPropertiesThatNeedWriters
                .OrderBy(p => p.Name)
                // TODO: handle generic parameters/property types
                .Where(p => !p.PropertyType.IsGenericType && !p.PropertyType.IsGenericParameter &&
                            !p.PropertyType.ContainsGenericParameters)
                .Select(p =>
                    {
                        if (t.IsSealed)
                            // Generic constraint not allowed with sealed type
                            return MethodDeclaration(ParseTypeName($"{t.Name}"), $"Set{p.Name}")
                                .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
                                .AddParameterListParameters(
                                    Parameter(Identifier("entity"))
                                        .WithType(ParseTypeName($"{t.Name}"))
                                        .AddModifiers(Token(SyntaxKind.ThisKeyword)),
                                    Parameter(Identifier("value"))
                                        .WithType(ParseTypeName(p.Type))
                                )
                                .WithBody(p.IsSetterProtected
                                    ? Block(ParseStatement($"entity.SetProperty(\"{p.Name}\", value);"),
                                        ParseStatement("return entity;"))
                                    : Block(ParseStatement($"entity.{p.Name} = value;"),
                                        ParseStatement("return entity;")));
                        return MethodDeclaration(ParseTypeName("T"), $"Set{p.Name}")
                            .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
                            .AddTypeParameterListParameters(TypeParameter(Identifier("T")))
                            .AddParameterListParameters(
                                Parameter(Identifier("entity")).WithType(ParseTypeName("T"))
                                    .AddModifiers(Token(SyntaxKind.ThisKeyword)),
                                Parameter(Identifier("value")).WithType(ParseTypeName(p.Type))
                            )
                            .AddConstraintClauses(TypeParameterConstraintClause("T").WithConstraints(GetSL(t.Name)))
                            .WithBody(p.IsSetterProtected
                                ? Block(ParseStatement($"entity.SetProperty(\"{p.Name}\", value);"),
                                    ParseStatement("return entity;"))
                                : Block(ParseStatement($"entity.{p.Name} = value;"), ParseStatement("return entity;")));
                    }
                ));

            // ---------------------------------------------------------------------------
            // generate helper methods for collections Clear/Add/Set
            var collectionHelpers = genericReadablePublicProperties
                .OrderBy(p => p.Name)
                // Limit to 1 type param.  Dictionary support is harder.
                .Where(p => p.PropertyType.GenericTypeArguments.Length == 1)
                .SelectMany(p =>
                    {
                        if (t.IsSealed)
                            // Generic constraint not allowed with sealed type
                            return new[]
                            {
                                MethodDeclaration(ParseTypeName($"{t.Name}"), $"Clear{p.Name}")
                                    .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
                                    .AddParameterListParameters(
                                        Parameter(Identifier("entity"))
                                            .WithType(ParseTypeName($"{t.Name}"))
                                            .AddModifiers(Token(SyntaxKind.ThisKeyword))
                                    )
                                    .WithBody(Block(ParseStatement($"entity.{p.Name}.Clear();"),
                                        ParseStatement("return entity;"))),
                                MethodDeclaration(ParseTypeName($"{t.Name}"), $"Add{p.Name}")
                                    .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
                                    .AddParameterListParameters(
                                        Parameter(Identifier("entity"))
                                            .WithType(ParseTypeName($"{t.Name}"))
                                            .AddModifiers(Token(SyntaxKind.ThisKeyword)),
                                        Parameter(Identifier("value"))
                                            .WithType(ParseTypeName(
                                                $"IEnumerable<{string.Join(",", p.PropertyType.GenericTypeArguments.Select(a => SimplifyType(a)))}>"))
                                    )
                                    .WithBody(Block(ParseStatement($"entity.{p.Name}.AddRange(value);"),
                                        ParseStatement("return entity;"))),
                                MethodDeclaration(ParseTypeName($"{t.Name}"), $"Add{p.Name}")
                                    .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
                                    .AddParameterListParameters(
                                        Parameter(Identifier("entity"))
                                            .WithType(ParseTypeName($"{t.Name}"))
                                            .AddModifiers(Token(SyntaxKind.ThisKeyword)),
                                        Parameter(Identifier("value"))
                                            .WithType(ParseTypeName(
                                                $"params {string.Join(",", p.PropertyType.GenericTypeArguments.Select(a => SimplifyType(a)))}[]"))
                                    )
                                    .WithBody(Block(ParseStatement($"Add{p.Name}(entity, value.AsEnumerable());"),
                                        ParseStatement("return entity;"))),
                                MethodDeclaration(ParseTypeName($"{t.Name}"), $"Set{p.Name}")
                                    .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
                                    .AddParameterListParameters(
                                        Parameter(Identifier("entity"))
                                            .WithType(ParseTypeName($"{t.Name}"))
                                            .AddModifiers(Token(SyntaxKind.ThisKeyword)),
                                        Parameter(Identifier("value"))
                                            .WithType(ParseTypeName(
                                                $"IEnumerable<{string.Join(",", p.PropertyType.GenericTypeArguments.Select(a => SimplifyType(a)))}>"))
                                    )
                                    .WithBody(Block(ParseStatement($"entity.{p.Name}.SetRange(value);"),
                                        ParseStatement("return entity;"))),
                                MethodDeclaration(ParseTypeName($"{t.Name}"), $"Set{p.Name}")
                                    .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
                                    .AddParameterListParameters(
                                        Parameter(Identifier("entity"))
                                            .WithType(ParseTypeName($"{t.Name}"))
                                            .AddModifiers(Token(SyntaxKind.ThisKeyword)),
                                        Parameter(Identifier("value"))
                                            .WithType(ParseTypeName(
                                                $"params {string.Join(",", p.PropertyType.GenericTypeArguments.Select(a => SimplifyType(a)))}[]"))
                                    )
                                    .WithBody(Block(ParseStatement($"Set{p.Name}(entity, value.AsEnumerable());"),
                                        ParseStatement("return entity;")))
                            };
                        return new[]
                        {
                            MethodDeclaration(ParseTypeName("T"), $"Clear{p.Name}")
                                .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
                                .AddTypeParameterListParameters(TypeParameter(Identifier("T")))
                                .AddParameterListParameters(
                                    Parameter(Identifier("entity")).WithType(ParseTypeName("T"))
                                        .AddModifiers(Token(SyntaxKind.ThisKeyword))
                                )
                                .AddConstraintClauses(TypeParameterConstraintClause("T").WithConstraints(GetSL(t.Name)))
                                .WithBody(Block(ParseStatement($"entity.{p.Name}.Clear();"),
                                    ParseStatement("return entity;"))),
                            MethodDeclaration(ParseTypeName("T"), $"Add{p.Name}")
                                .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
                                .AddTypeParameterListParameters(TypeParameter(Identifier("T")))
                                .AddParameterListParameters(
                                    Parameter(Identifier("entity"))
                                        .WithType(ParseTypeName("T"))
                                        .AddModifiers(Token(SyntaxKind.ThisKeyword)),
                                    Parameter(Identifier("value"))
                                        .WithType(ParseTypeName(
                                            $"params {string.Join(",", p.PropertyType.GenericTypeArguments.Select(a => SimplifyType(a)))}[]"))
                                )
                                .AddConstraintClauses(TypeParameterConstraintClause("T").WithConstraints(GetSL(t.Name)))
                                .WithBody(Block(ParseStatement($"Add{p.Name}(entity, value.AsEnumerable());"),
                                    ParseStatement("return entity;"))),
                            MethodDeclaration(ParseTypeName("T"), $"Add{p.Name}")
                                .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
                                .AddTypeParameterListParameters(TypeParameter(Identifier("T")))
                                .AddParameterListParameters(
                                    Parameter(Identifier("entity"))
                                        .WithType(ParseTypeName("T"))
                                        .AddModifiers(Token(SyntaxKind.ThisKeyword)),
                                    Parameter(Identifier("value"))
                                        .WithType(ParseTypeName(
                                            $"IEnumerable<{string.Join(",", p.PropertyType.GenericTypeArguments.Select(a => SimplifyType(a)))}>"))
                                )
                                .AddConstraintClauses(TypeParameterConstraintClause("T").WithConstraints(GetSL(t.Name)))
                                .WithBody(Block(ParseStatement($"entity.{p.Name}.AddRange(value);"),
                                    ParseStatement("return entity;"))),
                            MethodDeclaration(ParseTypeName("T"), $"Set{p.Name}")
                                .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
                                .AddTypeParameterListParameters(TypeParameter(Identifier("T")))
                                .AddParameterListParameters(
                                    Parameter(Identifier("entity"))
                                        .WithType(ParseTypeName("T"))
                                        .AddModifiers(Token(SyntaxKind.ThisKeyword)),
                                    Parameter(Identifier("value"))
                                        .WithType(ParseTypeName(
                                            $"params {string.Join(",", p.PropertyType.GenericTypeArguments.Select(a => SimplifyType(a)))}[]"))
                                )
                                .AddConstraintClauses(TypeParameterConstraintClause("T").WithConstraints(GetSL(t.Name)))
                                .WithBody(Block(ParseStatement($"Set{p.Name}(entity, value.AsEnumerable());"),
                                    ParseStatement("return entity;"))),
                            MethodDeclaration(ParseTypeName("T"), $"Set{p.Name}")
                                .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
                                .AddTypeParameterListParameters(TypeParameter(Identifier("T")))
                                .AddParameterListParameters(
                                    Parameter(Identifier("entity"))
                                        .WithType(ParseTypeName("T"))
                                        .AddModifiers(Token(SyntaxKind.ThisKeyword)),
                                    Parameter(Identifier("value"))
                                        .WithType(ParseTypeName(
                                            $"IEnumerable<{string.Join(",", p.PropertyType.GenericTypeArguments.Select(a => SimplifyType(a)))}>"))
                                )
                                .AddConstraintClauses(TypeParameterConstraintClause("T").WithConstraints(GetSL(t.Name)))
                                .WithBody(Block(ParseStatement($"entity.{p.Name}.SetRange(value);"),
                                    ParseStatement("return entity;")))
                        };
                    }
                );

            methods = methods.Concat(collectionHelpers);

            // ---------------------------------------------------------------------------
            // Copy using Copy method
            var copyMethod = t.GetMethod("Copy", new[] {t});
            if (copyMethod != null)
                methods = methods.Concat(
                    new[]
                    {
                        MethodDeclaration(ParseTypeName($"{SimplifyType(t)}"), "Copy")
                            .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
                            .AddParameterListParameters(
                                Parameter(Identifier("entity"))
                                    .WithType(ParseTypeName($"{SimplifyType(t)}"))
                                    .AddModifiers(Token(SyntaxKind.ThisKeyword))
                            )
                            .WithBody(Block(ParseStatement($"var copy = new {SimplifyType(t)}();"),
                                ParseStatement("copy.Copy(entity);"), ParseStatement("return copy;")))
                    });

            // ---------------------------------------------------------------------------
            // Copy using Copy constructor
            if (copyMethod == null)
            {
                var copyMethod2 = t.GetConstructor(new[] {t});
                if (copyMethod2 != null)
                    methods = methods.Concat(
                        new[]
                        {
                            MethodDeclaration(ParseTypeName($"{SimplifyType(t)}"), "Copy")
                                .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
                                .AddParameterListParameters(
                                    Parameter(Identifier("entity"))
                                        .WithType(ParseTypeName($"{SimplifyType(t)}"))
                                        .AddModifiers(Token(SyntaxKind.ThisKeyword))
                                )
                                .WithBody(Block(ParseStatement($"return new {SimplifyType(t)}(entity);")))
                        });
            }

            // ---------------------------------------------------------------------------
            if (methods.Any())
            {
                cd = cd.AddMembers(methods.OrderBy(m => m.Identifier.ToString()).ToArray());

                // namespace
                var ns = NamespaceDeclaration(ParseName("SolastaModApi.Extensions"))
                    .AddMembers(cd);

                var path = Path.Combine(OutputPath, $"{t.Name}Extensions.cs");

                var code = sf.AddMembers(ns)
                    .NormalizeWhitespace()
                    .ToFullString();

                // hack until I work out how to do this with Roslyn - it's so much easier like this :)
                var withComment =
                    "    /// <summary>" + Environment.NewLine +
                    "    /// This helper extensions class was automatically generated." + Environment.NewLine +
                    "    /// If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues." +
                    Environment.NewLine +
                    "    /// </summary>" + Environment.NewLine +
                    "    [TargetType";

                code = code.Replace("    [TargetType", withComment);

                //code.Dump();

                if (createFiles) File.WriteAllText(path, code);
            }

            UsingDirectiveSyntax GetUsingSyntax(string name)
            {
                return UsingDirective(ParseName(name));
            }

            SeparatedSyntaxList<TypeParameterConstraintSyntax> GetSL(string name)
            {
                return SeparatedList<TypeParameterConstraintSyntax>().Add(TypeConstraint(ParseTypeName($"{name}")));
            }

            string GetPropertyNameForField(FieldInfo f)
            {
                var match = NameRegex.Match(f.Name);

                if (match.Success)
                    //f.Name.Dump("regex match");

                    return Capitalize(match.Groups["name"].Value);

                var type = SimplifyType(f.FieldType);

                if (readablePublicPropertiesByName.Contains(type)) return Capitalize(type);

                return Capitalize(f.Name);
            }

            string Capitalize(string name)
            {
                if (name.Length <= 1) return name;

                var sb = new StringBuilder(name);
                sb[0] = char.ToUpperInvariant(sb[0]);
                return sb.ToString();
            }

            string SimplifyType(Type t1)
            {
                if (t1.IsGenericType)
                    // This will fail on say Dictionary<string, int[]>

                    return t1.ToString()
                        .Replace("`1", "") // generic type position 1
                        .Replace("`2", "") // generic type position 2
                        .Replace("[", "<") // same as < 
                        .Replace("]", ">") // same as >
                        .Replace("+", "."); // nested class
                return t1.ToString().Replace("+", ".");
            }

            SyntaxTriviaList GetClassComment(string v)
            {
                var list = List<XmlNodeSyntax>();

                list = list.Add(XmlSummaryElement(XmlText("This is a comment")));
                list = list.Add(XmlNewLine(""));

                ;
                var t2 = DocumentationCommentTrivia(SyntaxKind.SingleLineDocumentationCommentTrivia, list);
                return TriviaList(DocumentationCommentExterior("exterior"));
            }
        }
    }
#endif
}