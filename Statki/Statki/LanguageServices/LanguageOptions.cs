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
				string filePath = "LanguageData" + Path.DirectorySeparatorChar + "LanguageOptions.json";
				AvailableLanguages = ReadFromJson<AvailableLanguageModel>(filePath);
			}
			catch
			{

			}
		}
		public void LoadOptions(string language)
		{
			try
			{
				string filePath = "LanguageData" + Path.DirectorySeparatorChar + language + ".json";
				ChosenLanguage = ReadFromJson<ChosenLanguageModel>(filePath);
			}
			catch
			{

			}
		}
		private T ReadFromJson<T>(string fileName)
		{
			string basePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\"));
			string filePath = Path.Combine(basePath, fileName);
			if (File.Exists(filePath))
			{
				
				string jsonString = File.ReadAllText(filePath);
				return JsonSerializer.Deserialize<T>(jsonString);
			}
			return default;
		}
		public AvailableLanguageModel AvailableLanguages { get; set; }
		public ChosenLanguageModel ChosenLanguage { get; set; }

	}
}
