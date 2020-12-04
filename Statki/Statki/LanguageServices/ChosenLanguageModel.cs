using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Battleship.LanguageServices
{
	public class ChosenLanguageModel
	{
		public string[] MenuOptions { get; set; }
		public string[] FileService { get; set; }
		public DuringAdding DuringAdding { get; set; }
		public DuringGame DuringGame { get; set; }
	}
	public abstract class MessagesBasicData
	{
		public Dictionary<string, string> SignsMeaning { get; set; }
		[JsonIgnore]
		public List<Tuple<string, string>> SignsMeaningList { get; set; } = null;
		public string[] TopMessages { get; set; }
		public string[] BottomMessages { get; set; }
	}

	public class DuringAdding : MessagesBasicData
	{
	}
	public class DuringGame : MessagesBasicData
	{
	}
}
