<Query Kind="Program">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
  <Namespace>static Microsoft.CodeAnalysis.CSharp.SyntaxFactory</Namespace>
  <Namespace>System.Globalization</Namespace>
</Query>

const string pattern = @"^(\<(?<name>.*)\>k__BackingField)|(m_(?<name>.*))$";
static readonly Regex NameRegex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled);

// Where you want the files to be written to
const string outputPath = @"C:\Users\passp\source\repos\SolastaCommunityExpansion\SolastaCommunityExpansion\Api\Extensions";

// The path that contains Solasta_Data\Managed

// You can do this if you have configured the SolastaInstallDir environment variable
//readonly string installDir = Environment.GetEnvironmentVariable("SolastaInstallDir");

// Or point at specific version - e.g. 0.5.42
const string installDir = @"D:\Program Files (x86)\SteamLibrary\steamapps\common\Slasta_COTM";

void Main()
{
	var assemblyDir = Path.Combine(installDir, @"Solasta_Data\Managed");

	var assembly = Assembly.LoadFrom(Path.Combine(assemblyDir, @"Assembly-CSharp.dll"));

	var exclusions = new List<string>{
		"PrefabPool",
		"AnimatorSavedParameters",
		"FunctorParametersDescription",
		"TextFragmentStyleDescription",
		"TrianglePool",
		"FadingRendererParameters"};

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
		.Concat(GetTypesEndingIn(assembly, "Form", true))
		.Concat(GetTypesEndingIn(assembly, "TreasureOption", true))
		.Concat(GetTypesEndingIn(assembly, "HeroEquipmentOption", true))
		.Concat(GetTypesEndingIn(assembly, "HeroEquipmentRow", true))
		.Concat(GetTypesEndingIn(assembly, "HeroEquipmentColumn", true))
		.Concat(GetTypesEndingIn(assembly, "DiceByRank", true))
		.Concat(GetTypesEndingIn(assembly, "RulesetUsablePower", true))

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

	types.Count().Dump("Total types");

	//types.ForEach(t => t.FullName.Dump());

	// TODO: delete everything from output path?

	// set to true to create files, otherwise false for testing
	bool createTheFiles = true;

	foreach (var t in types)
	{
		CreateExtensions(t, createTheFiles);
	}
}

IEnumerable<(Type type, Type baseType)> GetDerivedTypes(Assembly assembly, string baseType)
{
	var types = assembly.GetTypes().ToList();

	var type = types.SingleOrDefault(t => t.Name == baseType);

	if (type == null)
	{
		return Enumerable.Empty<(Type type, Type baseType)>();
	}

	return Enumerable
		.Repeat((type, (Type)null), 1)
		.Concat(GetDerivedTypes(assembly, type));
}

IEnumerable<(Type type, Type baseType)> GetDerivedTypes(Assembly assembly, Type baseType)
{
	var types = assembly.GetTypes()
		.Where(t => t.BaseType?.Name == baseType.Name)
		.Select(t => (type: t, baseType))
		.OrderBy(t => t.type.Name)
		.ToList();

	return types.Concat(types.SelectMany(t => GetDerivedTypes(assembly, t.type)));
}

IEnumerable<Type> GetTypesEndingIn(Assembly assembly, string suffix, bool caseSensitive = true)
{
	var typesEndingIn = assembly.GetTypes()
		.Where(t => t.Name?.EndsWith(suffix, caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase) ?? false)
		.OrderBy(t => t.Name)
		.ToList();

	return typesEndingIn.Concat(typesEndingIn.SelectMany(t => GetDerivedTypes(assembly, t).Select(d => d.type)));
}

IEnumerable<Type> GetTypes(Assembly assembly, params string[] typeNames)
{
	var tn = typeNames.ToHashSet();

	return assembly.GetTypes()
		.Where(t => tn.Contains(t.Name))
		.OrderBy(t => t.Name)
		.ToList();
}

void CreateExtensions(Type t, bool createFiles = false)
{
	var sf = SyntaxFactory.CompilationUnit()
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
		.AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword), Token(SyntaxKind.PartialKeyword))
		.AddAttributeLists(GetALS(t.Name));
	//.WithLeadingTrivia(GetClassComment($"{t.Name}Extensions"));

	AttributeListSyntax GetALS(string typeName)
	{
		return SyntaxFactory.AttributeList(
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
										ParseExpression($"\"Community Expansion Extension Generator\""))
									)
								.Add(
									// probably best not to change the version often since that makes 
									// it impossible to detect changes to extensions
									AttributeArgument(
										ParseExpression($"\"1.0.0\"")) 
									)
								)
						)
					)
			);
	}

	var privateFields = t
		.GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic)
		.Select(f => new { f.Name, FieldInfo = f, f.FieldType, Type = SimplifyType(f.FieldType) });

	var writeablePublicProperties = t
		.GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public)
		.Where(pg => pg.CanWrite)
		.Select(pg => new { pg.Name, pg.PropertyType, Type = SimplifyType(pg.PropertyType), IsSetterProtected = pg.SetMethod.IsFamily });

	var readablePublicProperties = t
		.GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public)
		.Where(pg => pg.CanRead)
		.Select(pg => new { pg.Name, pg.PropertyType, Type = SimplifyType(pg.PropertyType) });

	var readablePublicPropertiesByName = readablePublicProperties
		.Select(pp => pp.Name)
		.ToHashSet(StringComparer.OrdinalIgnoreCase);

	var writeablePublicPropertiesByName = writeablePublicProperties
		.Select(pp => pp.Name)
		.ToHashSet(StringComparer.OrdinalIgnoreCase);

	var privateFieldsThatNeedWriter = privateFields
		.Where(f => !f.FieldType.IsGenericType && !writeablePublicPropertiesByName.Contains(f.Name));

	var genericPrivateFieldsThatNeedGetters = privateFields
		.Where(f => f.FieldType.IsGenericType && !writeablePublicPropertiesByName.Contains(GetPropertyNameForField(f.FieldInfo)) && !readablePublicPropertiesByName.Contains(GetPropertyNameForField(f.FieldInfo)));

	var genericReadablePublicProperties = readablePublicProperties
		.Where(f => f.PropertyType.IsGenericType);

	// ---------------------------------------------------------------------------
	var methods = privateFieldsThatNeedWriter
		.OrderBy(ftnw => ftnw.Name)
		.Select(f =>
			{
				if (t.IsSealed)
				{
					// Generic constraint not allowed with sealed type
					return MethodDeclaration(ParseTypeName($"{t.Name}"), $"Set{GetPropertyNameForField(f.FieldInfo)}")
					   .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
					   .AddParameterListParameters(
							Parameter(Identifier("entity"))
								.WithType(ParseTypeName($"{t.Name}"))
								.AddModifiers(Token(SyntaxKind.ThisKeyword)),
							Parameter(Identifier("value"))
								.WithType(ParseTypeName(SimplifyType(f.FieldType)))
						)
						.WithBody(Block(ParseStatement($"entity.SetField(\"{f.Name}\", value);"), ParseStatement("return entity;")));
				}
				else
				{
					return MethodDeclaration(ParseTypeName($"T"), $"Set{GetPropertyNameForField(f.FieldInfo)}")
					   .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
					   .AddTypeParameterListParameters(TypeParameter(Identifier("T")))
					   .AddParameterListParameters(
							Parameter(Identifier("entity")).WithType(ParseTypeName("T")).AddModifiers(Token(SyntaxKind.ThisKeyword)),
							Parameter(Identifier("value")).WithType(ParseTypeName(SimplifyType(f.FieldType)))
						)
						.AddConstraintClauses(TypeParameterConstraintClause("T").WithConstraints(GetSL(t.Name)))
						.WithBody(Block(ParseStatement($"entity.SetField(\"{f.Name}\", value);"), ParseStatement("return entity;")));
				}
			}
		);

	// ---------------------------------------------------------------------------
	var genericGeneratedGetters = genericPrivateFieldsThatNeedGetters
		.Select(f => MethodDeclaration(ParseTypeName($"{SimplifyType(f.FieldType)}"), $"Get{GetPropertyNameForField(f.FieldInfo)}")
			.AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
			.AddTypeParameterListParameters(TypeParameter(Identifier("T")))
			.AddParameterListParameters(Parameter(Identifier("entity")).WithType(ParseTypeName("T")).AddModifiers(Token(SyntaxKind.ThisKeyword)))
			.AddConstraintClauses(TypeParameterConstraintClause("T").WithConstraints(GetSL(t.Name)))
			.WithBody(Block(ParseStatement($"return entity.GetField<{SimplifyType(f.FieldType)}>(\"{f.Name}\");")))
		);

	methods = methods.Concat(genericGeneratedGetters);

	// ---------------------------------------------------------------------------
	var generatedSetters =
		privateFieldsThatNeedWriter.Select(f => GetPropertyNameForField(f.FieldInfo)).ToHashSet(StringComparer.OrdinalIgnoreCase);

	var writeablePublicPropertiesThatNeedWriters = writeablePublicProperties.Where(p => !generatedSetters.Contains(p.Name));

	methods = methods.Concat(writeablePublicPropertiesThatNeedWriters
		.OrderBy(p => p.Name)
		// TODO: handle generic parameters/property types
		.Where(p => !p.PropertyType.IsGenericType && !p.PropertyType.IsGenericParameter && !p.PropertyType.ContainsGenericParameters)
		.Select(p =>
			{
				if (t.IsSealed)
				{
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
							? Block(ParseStatement($"entity.SetProperty(\"{p.Name}\", value);"), ParseStatement("return entity;"))
							: Block(ParseStatement($"entity.{p.Name} = value;"), ParseStatement("return entity;")));
				}
				else
				{
					return MethodDeclaration(ParseTypeName($"T"), $"Set{p.Name}")
					   .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
					   .AddTypeParameterListParameters(TypeParameter(Identifier("T")))
					   .AddParameterListParameters(
						   Parameter(Identifier("entity")).WithType(ParseTypeName("T")).AddModifiers(Token(SyntaxKind.ThisKeyword)),
							Parameter(Identifier("value")).WithType(ParseTypeName(p.Type))
						)
						.AddConstraintClauses(TypeParameterConstraintClause("T").WithConstraints(GetSL(t.Name)))
						.WithBody(p.IsSetterProtected
							? Block(ParseStatement($"entity.SetProperty(\"{p.Name}\", value);"), ParseStatement("return entity;"))
							: Block(ParseStatement($"entity.{p.Name} = value;"), ParseStatement("return entity;")));
				}
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
				{
					// Generic constraint not allowed with sealed type
					return new[]{
						MethodDeclaration(ParseTypeName($"{t.Name}"), $"Clear{p.Name}")
						   .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
						   .AddParameterListParameters(
								Parameter(Identifier("entity"))
									.WithType(ParseTypeName($"{t.Name}"))
									.AddModifiers(Token(SyntaxKind.ThisKeyword))
							)
							.WithBody(Block(ParseStatement($"entity.{p.Name}.Clear();"), ParseStatement("return entity;"))),
						MethodDeclaration(ParseTypeName($"{t.Name}"), $"Add{p.Name}")
						   .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
						   .AddParameterListParameters(
								Parameter(Identifier("entity"))
									.WithType(ParseTypeName($"{t.Name}"))
									.AddModifiers(Token(SyntaxKind.ThisKeyword)),
								Parameter(Identifier("value"))
									.WithType(ParseTypeName($"IEnumerable<{string.Join(",", p.PropertyType.GenericTypeArguments.Select(a => SimplifyType(a)))}>"))
							)
							.WithBody(Block(ParseStatement($"entity.{p.Name}.AddRange(value);"), ParseStatement("return entity;"))),
						MethodDeclaration(ParseTypeName($"{t.Name}"), $"Add{p.Name}")
						   .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
						   .AddParameterListParameters(
								Parameter(Identifier("entity"))
									.WithType(ParseTypeName($"{t.Name}"))
									.AddModifiers(Token(SyntaxKind.ThisKeyword)),
								Parameter(Identifier("value"))
									.WithType(ParseTypeName($"params {string.Join(",", p.PropertyType.GenericTypeArguments.Select(a => SimplifyType(a)))}[]"))
							)
							.WithBody(Block(ParseStatement($"Add{p.Name}(entity, value.AsEnumerable());"), ParseStatement("return entity;"))),
						MethodDeclaration(ParseTypeName($"{t.Name}"), $"Set{p.Name}")
						   .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
						   .AddParameterListParameters(
								Parameter(Identifier("entity"))
									.WithType(ParseTypeName($"{t.Name}"))
									.AddModifiers(Token(SyntaxKind.ThisKeyword)),
								Parameter(Identifier("value"))
									.WithType(ParseTypeName($"IEnumerable<{string.Join(",", p.PropertyType.GenericTypeArguments.Select(a => SimplifyType(a)))}>"))
							)
							.WithBody(Block(ParseStatement($"entity.{p.Name}.SetRange(value);"), ParseStatement("return entity;"))),
						MethodDeclaration(ParseTypeName($"{t.Name}"), $"Set{p.Name}")
						   .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
						   .AddParameterListParameters(
								Parameter(Identifier("entity"))
									.WithType(ParseTypeName($"{t.Name}"))
									.AddModifiers(Token(SyntaxKind.ThisKeyword)),
								Parameter(Identifier("value"))
									.WithType(ParseTypeName($"params {string.Join(",", p.PropertyType.GenericTypeArguments.Select(a => SimplifyType(a)))}[]"))
							)
							.WithBody(Block(ParseStatement($"Set{p.Name}(entity, value.AsEnumerable());"), ParseStatement("return entity;")))
						};
				}
				else
				{
					return new[]{
						MethodDeclaration(ParseTypeName($"T"), $"Clear{p.Name}")
						   .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
						   .AddTypeParameterListParameters(TypeParameter(Identifier("T")))
						   .AddParameterListParameters(
								Parameter(Identifier("entity")).WithType(ParseTypeName("T")).AddModifiers(Token(SyntaxKind.ThisKeyword))
							)
							.AddConstraintClauses(TypeParameterConstraintClause("T").WithConstraints(GetSL(t.Name)))
							.WithBody(Block(ParseStatement($"entity.{p.Name}.Clear();"), ParseStatement("return entity;"))),
						MethodDeclaration(ParseTypeName($"T"), $"Add{p.Name}")
						   .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
						   .AddTypeParameterListParameters(TypeParameter(Identifier("T")))
						   .AddParameterListParameters(
								Parameter(Identifier("entity"))
									.WithType(ParseTypeName("T"))
									.AddModifiers(Token(SyntaxKind.ThisKeyword)),
								Parameter(Identifier("value"))
									.WithType(ParseTypeName($"params {string.Join(",", p.PropertyType.GenericTypeArguments.Select(a => SimplifyType(a)))}[]"))
							)
							.AddConstraintClauses(TypeParameterConstraintClause("T").WithConstraints(GetSL(t.Name)))
							.WithBody(Block(ParseStatement($"Add{p.Name}(entity, value.AsEnumerable());"), ParseStatement("return entity;"))),
						MethodDeclaration(ParseTypeName($"T"), $"Add{p.Name}")
						   .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
						   .AddTypeParameterListParameters(TypeParameter(Identifier("T")))
						   .AddParameterListParameters(
								Parameter(Identifier("entity"))
									.WithType(ParseTypeName("T"))
									.AddModifiers(Token(SyntaxKind.ThisKeyword)),
								Parameter(Identifier("value"))
									.WithType(ParseTypeName($"IEnumerable<{string.Join(",", p.PropertyType.GenericTypeArguments.Select(a => SimplifyType(a)))}>"))
							)
							.AddConstraintClauses(TypeParameterConstraintClause("T").WithConstraints(GetSL(t.Name)))
							.WithBody(Block(ParseStatement($"entity.{p.Name}.AddRange(value);"), ParseStatement("return entity;"))),
						MethodDeclaration(ParseTypeName($"T"), $"Set{p.Name}")
						   .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
						   .AddTypeParameterListParameters(TypeParameter(Identifier("T")))
						   .AddParameterListParameters(
								Parameter(Identifier("entity"))
									.WithType(ParseTypeName("T"))
									.AddModifiers(Token(SyntaxKind.ThisKeyword)),
								Parameter(Identifier("value"))
									.WithType(ParseTypeName($"params {string.Join(",", p.PropertyType.GenericTypeArguments.Select(a => SimplifyType(a)))}[]"))
							)
							.AddConstraintClauses(TypeParameterConstraintClause("T").WithConstraints(GetSL(t.Name)))
							.WithBody(Block(ParseStatement($"Set{p.Name}(entity, value.AsEnumerable());"), ParseStatement("return entity;"))),
						MethodDeclaration(ParseTypeName($"T"), $"Set{p.Name}")
						   .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
						   .AddTypeParameterListParameters(TypeParameter(Identifier("T")))
						   .AddParameterListParameters(
								Parameter(Identifier("entity"))
									.WithType(ParseTypeName("T"))
									.AddModifiers(Token(SyntaxKind.ThisKeyword)),
								Parameter(Identifier("value"))
									.WithType(ParseTypeName($"IEnumerable<{string.Join(",", p.PropertyType.GenericTypeArguments.Select(a => SimplifyType(a)))}>"))
							)
							.AddConstraintClauses(TypeParameterConstraintClause("T").WithConstraints(GetSL(t.Name)))
							.WithBody(Block(ParseStatement($"entity.{p.Name}.SetRange(value);"), ParseStatement("return entity;")))
						};
				}
			}
		);
	
	methods = methods.Concat(collectionHelpers);
	
	// ---------------------------------------------------------------------------
	// TODO: for all types add a .Create() method
	// TODO: for all types with a .Copy(..) method generate a Copy extension method
	
	if (methods.Any())
	{
		cd = cd.AddMembers(methods.OrderBy(m => m.Identifier.ToString()).ToArray());

		// namespace
		var ns = SyntaxFactory
			.NamespaceDeclaration(ParseName("SolastaModApi.Extensions"))
			.AddMembers(cd);

		var path = Path.Combine(outputPath, $"{t.Name}Extensions.cs");

		var code = sf.AddMembers(ns)
			.NormalizeWhitespace()
			.ToFullString();

		// hack until I work out how to do this with Roslyn - it's so much easier like this :)
		string withComment =
			"    /// <summary>" + Environment.NewLine +
			"    /// This helper extensions class was automatically generated." + Environment.NewLine +
			"    /// If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues." + Environment.NewLine +
			"    /// </summary>" + Environment.NewLine +
			"    [TargetType";

		code = code.Replace("    [TargetType", withComment);

		//code.Dump();

		if (createFiles)
		{
			File.WriteAllText(path, code);
		}
	}

	UsingDirectiveSyntax GetUsingSyntax(string name) { return UsingDirective(ParseName(name)); }

	SeparatedSyntaxList<TypeParameterConstraintSyntax> GetSL(string name)
	{
		return SeparatedList<TypeParameterConstraintSyntax>().Add(TypeConstraint(ParseTypeName($"{name}")));
	}

	string GetPropertyNameForField(FieldInfo f)
	{
		var match = NameRegex.Match(f.Name);

		if (match.Success)
		{
			//f.Name.Dump("regex match");

			return Capitalize(match.Groups["name"].Value);
		}
		else
		{
			var type = SimplifyType(f.FieldType);

			if (readablePublicPropertiesByName.Contains(type))
			{
				return Capitalize(type);
			}

			return Capitalize(f.Name);
		}
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
		{
			// This will fail on say Dictionary<string, int[]>
			
			return t1.ToString()
				.Replace("`1", "") // generic type position 1
				.Replace("`2", "") // generic type position 2
				.Replace("[", "<") // same as < 
				.Replace("]", ">") // same as >
				.Replace("+", "."); // nested class
		}
		else
		{
			return t1.ToString().Replace("+", ".");
		}
	}

	SyntaxTriviaList GetClassComment(string v)
	{
		//var t1 = SyntaxFactory.SyntaxTrivia(SyntaxKind.SingleLineDocumentationCommentTrivia, "This is a comment");

		var list = SyntaxFactory.List<XmlNodeSyntax>();

		list = list.Add(SyntaxFactory.XmlSummaryElement(SyntaxFactory.XmlText("This is a comment")));
		list = list.Add(SyntaxFactory.XmlNewLine(""));

		;
		var t2 = SyntaxFactory.DocumentationCommentTrivia(SyntaxKind.SingleLineDocumentationCommentTrivia, list);
		return TriviaList(DocumentationCommentExterior("exterior"));
	}
}