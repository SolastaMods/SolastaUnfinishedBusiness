<Query Kind="Program" />

void Main()
{
	// Where you want the files to be written to
	const string outputPath = @"C:\Users\passp\Source\Repos\SolastaModApi\SolastaModApi\DatabaseHelper";

	var assets = File.ReadLines(@"C:\Users\passp\source\repos\SolastaModApi\Scripts\Assets.txt")
		.Skip(1)
		.Select(f => f.Split("\t"))
		.Select(f => new { Name = f[0], Type = f[1], Database = f[2], Guid = f[3] })
		.GroupBy(f => f.Database);
		
	var sb = new StringBuilder();

	sb.Append("namespace SolastaModApi.DatabaseHelpers");
	sb.AppendLine();
	sb.Append("{");
	sb.AppendLine();

	foreach(var db in assets.OrderBy(d => d.Key))
	{
		sb.Append($"    public static class {PluralizeNormalizeClassName(db.Key)}");
		sb.AppendLine();
		sb.Append("    {");
		sb.AppendLine();

		foreach(var asset in db.OrderBy(a => a.Name))
		{
			if(string.IsNullOrWhiteSpace(asset.Guid))
			{
				$"asset {asset.Name} has no guid".Dump();
				
				sb.Append($"       public static {asset.Type} {NormalizePropertyName(asset.Name)} => Repository.GetByName<{asset.Type}>(\"{asset.Name}\");");
			}
			else
			{
				sb.Append($"       public static {asset.Type} {NormalizePropertyName(asset.Name)} => Repository.GetByGuid<{asset.Type}>(\"{asset.Guid}\");");
			}

			sb.AppendLine();
		}

		sb.Append("    }");
		sb.AppendLine();
		sb.AppendLine();
	}
	
	sb.Append("}");
	
	File.WriteAllText(Path.Combine(outputPath, "DatabaseHelpers.cs"), sb.ToString());
	//sb.ToString().Dump();
}

string PluralizeNormalizeClassName(string name)
{
	if(name.StartsWith("TA.AI."))
		name = name.Replace("TA.AI.", "");
	
	return name + "Set";
}

string NormalizePropertyName(string name)
{
	var n1 = name
		.Replace("_", "")
		.Replace("-","")
		.Replace(".", "")
		.Replace(" ", "")
		.Replace("+", "Plus")
		.Replace("ABJURATION", "Abjuration")
		.Replace("ARISTOCRAT", "Aristocrat")
		;
	
	if(n1.Length > 1 && char.IsDigit(n1[0]))
	{
		n1 = "N" + n1;	
	}
	
	return n1;
}