using Battleship.Logger;
using System;

namespace Battleship
{
	public class MainService
	{
		public MainService()
		{
			LanguageOptions languageOptions = new LanguageOptions();
			languageOptions.LoadLanguages();
			Window appWindow = new Window(languageOptions);


			int option = appWindow.ChoseLanguage();
			languageOptions.LoadOptions(languageOptions.AvailableLanguages.Languages[option]);
			new Game(languageOptions.ChosenLanguage, appWindow);
		}
	}
}
