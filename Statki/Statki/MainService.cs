namespace Battleship
{
	public class MainService
	{
		public MainService()
		{
			LanguageOptions languageOptions = new LanguageOptions();
			Window appWindow = new Window();
			int option = appWindow.ShowMenu(languageOptions.AvailableLanguages.Languages);
			languageOptions.LoadOptions(languageOptions.AvailableLanguages.Languages[option]);
			new Game(languageOptions.ChosenLanguage, appWindow);
		}
	}
}
