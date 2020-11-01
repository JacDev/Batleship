using Battleship.LanguageServices;
using System;
using System.IO;
using System.Text.Json;

namespace Battleship
{
	public  class LanguageOptions
	{
		public LanguageOptions()
		{
			try
			{
				string basePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\"));
				string filePath = Path.Combine(basePath, "LanguageData", "LanguageOptions.json");
				if (File.Exists(filePath))
				{
					string jsonString = File.ReadAllText(filePath);
					AvailableLanguages = JsonSerializer.Deserialize<AvailableLanguage>(jsonString);
				}
			}
			catch
			{

			}
		}
		public void LoadOptions(string language)
		{
			try
			{
				string basePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\"));
				string filePath = Path.Combine(basePath, "LanguageData", language + ".json");
				if (File.Exists(filePath))
				{
					string jsonString = File.ReadAllText(filePath);
					ChosenLanguage = JsonSerializer.Deserialize<ChosenLanguage>(jsonString);
				}
			}
			catch
			{

			}
		}
		public AvailableLanguage AvailableLanguages { get; set; }
		public ChosenLanguage ChosenLanguage { get; set; }

	}
}
