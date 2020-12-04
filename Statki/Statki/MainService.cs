using Battleship.Interfaces;
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

			ILoggerService logger = new LoggerService();
			IInputDevice inputDevice = new KeyboardReader();
			IOutputDevice appWindow = new Window(languageOptions, logger, inputDevice);


			int option = appWindow.ChoseLanguage();

			languageOptions.LoadOptions(languageOptions.AvailableLanguages.Languages[option]);

			new Game(languageOptions.ChosenLanguage, appWindow, inputDevice);
		}
	}
}
