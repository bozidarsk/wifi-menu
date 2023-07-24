using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

/*

in another file: (example 'ProjectConfig.cs')
'public static partial class Config'
it must contain the following fields:
- 'private static readonly string DefaultConfig'
- 'private static readonly string DefaultCss'
- 'private static readonly string DefaultConfigDir' (must end in '/')

in config file:
- format is 'NAME=VALUE'
- each space is treated as part of NAME or VALUE depending on its position
- lines that start with '#' are comments and are skipped
- if a line starts with '!' environment variable expansion will not be used
- each environment variable (starting with '$') will be expanded to its corresponding value
- all directories must end with '/'

properties defined in config file must be defined in 'ProjectConfig.cs' as:
- 'public static TYPE NAME { private set; get; }'
- 'public static TYPE[] NAME { private set; get; }'

arrays are defined in config file as:
'NAME[]=ITEM0,ITEM1,ITEM2' (supports ony 1d arrays; again be carefull of spaces)

*/

public static partial class Config 
{
	public static void CreateDefaults() 
	{
		if (!Directory.Exists(Config.DefaultConfigDir)) { Directory.CreateDirectory(Config.DefaultConfigDir); }
		File.WriteAllText(Config.DefaultConfigDir + "/config", Config.DefaultConfig);
		File.WriteAllText(Config.DefaultConfigDir + "/style.css", Config.DefaultCss);
	}

	private static object ParseContent(string content, Type type) => type.IsEnum ? Enum.Parse(type, content) : Convert.ChangeType(content, type);

	public static void Initialize(string pathConfigDir) 
	{
		if (pathConfigDir == null && !Directory.Exists(Config.DefaultConfigDir)) { Config.CreateDefaults(); }

		string configPath = (pathConfigDir ?? Config.DefaultConfigDir) + "config";
		string cssPath = (pathConfigDir ?? Config.DefaultConfigDir) + "style.css";

		Config.Css = File.Exists(cssPath) ? File.ReadAllText(cssPath) : Config.DefaultCss;

		string[] config = (File.Exists(configPath) ? File.ReadAllText(configPath) : Config.DefaultConfig).Split('\n').Where(x => !string.IsNullOrEmpty(x) && x[0] != '#').ToArray();
		for (int i = 0; i < config.Length; i++) 
		{
			if (string.IsNullOrEmpty(config[i])) { continue; }
			int index = config[i].IndexOf("=");
			string name = config[i].Substring(0, index);
			string content = config[i].Remove(0, index + 1);
			bool isArray = name.EndsWith("[]");

			if (isArray) { name = name.Substring(0, name.Length - 2); }

			if (name[0] != '!') 
			{
				for (Match match = Regex.Match(content, "\\$[a-zA-Z0-9_\\-]+"); match.Success; match = Regex.Match(content, "\\$[a-zA-Z0-9_\\-]+")) 
				{ match.Captures.ToList().ForEach(x => content = content.Replace(x.Value, Environment.GetEnvironmentVariable(x.Value.Remove(0, 1)))); }
			} else { name = name.Remove(0, 1); }

			PropertyInfo property = typeof(Config).GetProperty(name);
			if (property == null) { Console.WriteLine("Property '" + name + "' was not found."); continue; }

			try 
			{
				if (isArray) 
				{
					string[] items = content.Split(',');
					Type itemType = Type.GetType(property.PropertyType.ToString().Replace("[]", ""));
					Array array = Array.CreateInstance(itemType, items.Length);

					for (int t = 0; t < array.Length; t++) { array.SetValue(ParseContent(items[t], itemType), t); }
					property.SetValue(null, array);
				} else { property.SetValue(null, ParseContent(content, property.PropertyType)); }
			}
			catch 
			{
				Console.WriteLine("Error parsing '" + content + "' for '" + name + "'.");
				Environment.Exit(1);
			}
		}
	}
}