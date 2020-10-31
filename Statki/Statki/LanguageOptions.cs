using System;
using System.Collections.Generic;
using System.Text;

namespace Statki
{
	public static class LanguageOptions
	{
		public static string[] Languages { get; set; } =
		{
			"Polski",
			"English"
		};
		public static string[] MenuOptions { get; set; } = {
	/*0*/"GRA Z KOMPUTEREM",
	/*1*/"GRA Z PRZECIWNIKIEM",
	/*2*/"WCZYTAJ GRE",
	/*3*/"WYJDZ Z GRY",
	/*4*/"ZMIEN JEZYK",
	/*5*/"USTAWIENIA",
	/*6*/"KONTYNUUJ GRE",
	/*7*/"ZAPISZ GRE"
		};
	}
}
