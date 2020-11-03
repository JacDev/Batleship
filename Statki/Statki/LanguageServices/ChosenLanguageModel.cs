using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Battleship.LanguageServices
{
	public class ChosenLanguageModel
	{
		public string[] MenuOptions { get; set; }
		public string[] FileService { get; set; }
		public Dictionary<string, string> SignsMeaning { get; set; }

		public string[] TopMessages { get; set; }
		public string[] BottomMessages { get; set; }
		[JsonIgnore]
		public List<Tuple<string, string>> SignsMeaningList { get; set; } = null;
	}
}
