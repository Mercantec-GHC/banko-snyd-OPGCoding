using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools.V120.Network;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;



// Vi bruger en namespace ved navn 'BankoCheater', Namespace bruger vi for at fortælle at alt indhold
// i vores namespace høre sammen. 
namespace BankoCheater
{
	// Klasse som har navnet 'Program', som er en form for blueprint.
	// (Dette eksempel er ikke det mest optimale)
	internal class Program
	{
		// Main funktionen er den metode som er vores hovedmetode det er altså der hvor
		// alt kode bliver kørt fra.
		static void Main(string[] args)
		{

			// her bliver hjemmesidens url angivet
			string url = "https://mags-template.github.io/Banko/";
			// Her bliver stien til mappen hvor den driver som åbner chrome angivet
			string driverPath = @"C:\Users\blsto\source\repos\OPGCoding\banko-snyd-OPGCoding\BankoCheater\chromedriver.exe";

			// Spørger efter navn, og lager input i variablen 'brugerNavn'
			Console.WriteLine("Hvad er dit navn? ");
			string brugerNavn = Console.ReadLine();

			// Spørger efter antal af plader, og lager input i variablen 'antalPlader',
			// og konventere input til int
			Console.WriteLine("Hvor mange plader vil du bruge?");
			long antalPlader = Convert.ToInt64(Console.ReadLine());

			// Opretter en liste som er navndøbt 'plader'
			List<Plade> plader = new List<Plade>();

			// Her bliver der oprettet en instans som vi bruger til at 
			// indstillet hvordan Chrome skal åbne
			ChromeOptions options = new ChromeOptions();
			options.PageLoadStrategy = PageLoadStrategy.None;

			// Her laver vi en instans ved navn 'webdriver'
			using (IWebDriver webdriver = new ChromeDriver(driverPath))
			{
				// webdriver åbner med chrome med urlen vi angiv tidligere
				webdriver.Navigate().GoToUrl(url);

				// Laver et for loop der køre indtil i er mindre end antalPlader
				for (int i = 0; i < antalPlader; i++)
				{
					// Her bruger vi IWebElement som er en instans i Selenium WebDriver bibliotek
					// til at indsætte vores input(brugerNavn) ind i tekstfeltet på hjemmesiden automatisk
					IWebElement inputField = webdriver.FindElement(By.Id("tekstboks"));
					((IJavaScriptExecutor)webdriver).ExecuteScript($"arguments[0].value='{brugerNavn}{i}';", inputField);

					// Her bruger vi IWebElement som er en instans i Selenium WebDriver bibliotek
					// til at trykke på knappen, som generer en ny plade
					IWebElement generateButton = webdriver.FindElement(By.Id("knap"));
					generateButton.Click();

					// Her bruger vi WebDriverWait som er en instans i Slenium WebDriverWait bibliotek
					// Der bliver bliver der brugt TimeSpan til at tilføje ventetid på exekveringen
					WebDriverWait wait = new WebDriverWait(webdriver, TimeSpan.FromSeconds(6));
					wait.Until(ExpectedConditions.ElementIsVisible(By.Id("p11"))); // Vent til elementet er synligt

					// Her opretter vi en instans, hvor variablerne får tildelt en værdi
					Plade plade = new Plade
					{
						Navn = $"{brugerNavn}{i}",
						Række1 = ParseToIntList(webdriver.FindElement(By.Id("p11")).Text),
						Række2 = ParseToIntList(webdriver.FindElement(By.Id("p12")).Text),
						Række3 = ParseToIntList(webdriver.FindElement(By.Id("p13")).Text),
						BingoStatus = new bool[3]
					};

					// Her der tilføjer den instansen Plade til listen plader
					plader.Add(plade);
					// Her der clear den input feltet på hjemmesiden
					inputField.Clear();
				}
				// Her der kører den metoden for listen plader
				GemPlader(plader);
			}
			// Her der kører den metoden søgITal for listen plader
			søgITal(plader);
		}

		// Her der laver vi en statisk liste med datatypen int, som tager en string numbers
		// som parameter
		static List<int> ParseToIntList(string numbers)
		{
			return numbers.Split(' ') // Deler stringen op ved mellemrum
						// Forsøger at konvertere hvert element til et heltal. Kun de succesfuldt konverterede heltal beholdes.
						  .Select(n => int.TryParse(n, out int result) ? result : 0)
						  .Where(n => n != 0) // Filtrer væk de mislykkede konverteringer (0'er)
						  .ToList(); // Konverter resultatet til en liste og returner
		}

		// Der bliver lavet en metode 'GemPlader' og bruger listen 'plader' som parameter
		static void GemPlader(List<Plade> plader)
		{
			// Gemmer stien til plader.json filen i en string variable
			string sti = @"C:\Users\blsto\source\repos\OPGCoding\banko-snyd-OPGCoding\BankoCheater\plader.json";
			
			// Her der bliver konsollen cleared
			Console.Clear();
			
			try
			{
				// Ændre på formateringen af json filen
				string json = JsonConvert.SerializeObject(plader, Formatting.Indented);
				// Indsætter pladerne ind i json filen med den format vi angiv i linjen inden
				System.IO.File.WriteAllText(sti, json);
				// Udskriver en bekræftelsesbesked til konsollen om, at pladerne er gemt korrekt
				Console.WriteLine("Pladerne er gemt korrekt!");
			}
			// hvis der sker en undtagelse (fejl) i try-blokken så kører catch-blokken
			catch (Exception ex)
			{
				// Hvis der opstår en fejl, udskrives en fejlmeddelelse med beskrivelsen af fejlen
				Console.WriteLine($"Fejl i forsøget på at gemme pladerne! {ex.Message}");
			}
		}

		// Metode til at søge efter tal i pladerne
		static void søgITal(List<Plade> plader)
		{
			// Udskriver en streg for at adskille fra tidligere output
			Console.WriteLine("-------------------------------------------------------------------");
			// Udskriver en besked til brugeren for at bede om indtastning af tal eller 'done' for at afslutte
			Console.WriteLine("Indtast de opråbte tal (eller skriv 'done' for at afslutte): ");

			// Initialiserer en liste til at gemme indtastede tal
			List<int> indtastedeTal = new List<int>();
			string input;
			string tidligereTal = "";

			// Kører så længe input = Console.ReadLine()) ikke er "done"
			while ((input = Console.ReadLine()) != "done")
			{
				// Forsøger at konvertere brugerens input til et heltal (int),
				// og gemmer resultatet i variablen 'tal'.
				// Hvis konverteringen lykkes, vil 'tal' indeholde det konverterede heltal,
				// ellers vil 'tal' være 0.
				if (int.TryParse(input, out int tal))
				{
					// Hvis tallet ikke allerede er tilføjet, tilføjes det til listen af indtastedeTal
					if (!indtastedeTal.Contains(tal)) 
					{

						indtastedeTal.Add(tal); // 'Tal' tilføjes til listen 'indtastedeTal'
						Console.Clear();
						// Udskriver de tidligere indtastede tal, hvis det indtastede tal allerede findes
						Console.WriteLine("Tidligere tal: " + string.Join(" - ", indtastedeTal));
					}
					else
					{
						// Udskriver de tidligere indtastede tal, hvis det indtastede tal allerede findes
						Console.WriteLine("Tidligere tal: " + string.Join(" - ", indtastedeTal));
						// Fortsætter til næste iteration af løkken
						continue;
					}

					Console.Clear();
					Console.WriteLine("Tidligere tal: " + string.Join(" - ", indtastedeTal));
					// printer tidligereTal ud og fjerner eventuelle mellemrum
					Console.WriteLine(tidligereTal.TrimEnd());

					// Gennemgår hver plade for at kontrollere bingo
					foreach (var plade in plader)
					{
						// Kontrollerer hver plade for bingo

						int bingoRow = CheckBingoRow(plade, indtastedeTal);

						// Hvis bingo findes på en række
						if (bingoRow != -1)
						{
							// Tilføj kun nye bingo rækker
							if (!plade.BingoRækker.Contains(bingoRow + 1))
							{
								plade.BingoRækker.Add(bingoRow + 1);
							}

							plade.BingoRækker.Sort(); // Sikrer, at rækkerne er sorteret

							if (plade.BingoStatus.All(status => status)) // Håndterer fuld plade specifikt
							{
								Console.Write($"Bingo ( FULD PLADE )! {plade.Navn}, Rækker: ");
								for (int i = 0; i < plade.Rækker.Count; i++)
								{
									Console.Write($"{i + 1}: {string.Join(", ", plade.Rækker[i])}" + (i < plade.Rækker.Count - 1 ? " | " : ""));
								}
							}
							else // Håndterer 1 eller 2 rækker med bingo
							{
								// Opretter en liste af strengrepræsentationer for hver bingo-række med dens tilhørende tal.
								// For hver række i plade.BingoRækker:
								//    - Sammensætter en streng, der indeholder rækkens nummer efterfulgt af dens tal, adskilt af kommaer.
								//    - Disse strengrepræsentationer gemmes i variablen bingoRækkerOutput.
								var bingoRækkerOutput = plade.BingoRækker.Select(rækkeNummer =>
									$"{rækkeNummer}: {string.Join(", ", plade.Rækker[rækkeNummer - 1])}");

								Console.WriteLine($"Bingo ( {plade.BingoRækkeCount} RÆKKE(R) )! {plade.Navn}, Rækker: {string.Join(" | ", bingoRækkerOutput)}");
							}
						}
					}
				}
				else
				{
					// Fejler ved forkert input
					Console.WriteLine("Ugyldigt input, prøv igen.");
				}
			}
		}



		// Metode til at kontrollere for bingo på en given plade
		static int CheckBingoRow(Plade plade, List<int> indtastedeTal)
		{
			// Gennemgår hver række på pladen
			for (int i = 0; i < plade.Rækker.Count; i++)
			{
				// Hvis bingo ikke allerede er markeret for rækken og alle tal i rækken
				// er indtastet
				if (!plade.BingoStatus[i] && plade.Rækker[i].All(t => indtastedeTal.Contains(t)))
				{
					// Markerer bingo for rækken
					plade.BingoStatus[i] = true;
					plade.BingoRækkeCount++; // Tilføjer 1 til bingo række count
					return i; // Returnerer rækkens index
				}
			}
			// Returnerer -1 hvis der ikke er bingo på nogen rækker
			return -1;
		}
	}


	// er en datamodel, der repræsenterer en bingo-plade.
	// Den indeholder forskellige egenskaber, der beskriver forskellige dele af pladen:
	class Plade
	{
		// Pladens egenskaber
		public string Navn { get; set; } // Navnet på pladen
		public List<int> Række1 { get; set; } // Tal i række 1 på pladen
		public List<int> Række2 { get; set; } // Tal i række 2 på pladen
		public List<int> Række3 { get; set; } // Tal i række 3 på pladen
		
		// Samling af alle pladens rækker
		public List<List<int>> Rækker => new List<List<int>> { Række1, Række2, Række3 };
		// Et array til at holde styr på bingo-status for hver række
		public bool[] BingoStatus { get; set; }
		// Antal af bingo-rækker på pladen
		public int BingoRækkeCount { get; set; } = 0;
		// Liste over bingo-rækker på pladen
		public List<int> BingoRækker { get; set; } = new List<int>();

	}
}










//using Newtonsoft.Json;
//using OpenQA.Selenium;
//using OpenQA.Selenium.Chrome;
//using OpenQA.Selenium.Support.UI;
//using SeleniumExtras.WaitHelpers;

//namespace BankoCheater
//{
//	internal class Program
//	{
//		static void Main(string[] args)
//		{
//			string url = "https://mags-template.github.io/Banko/";
//			string driverPath = @"C:\Users\blsto\source\repos\OPGCoding\banko-snyd-OPGCoding\BankoCheater\chromedriver.exe";

//			Console.WriteLine("Hvad er dit navn? ");
//			string brugerNavn = Console.ReadLine();

//			Console.WriteLine("Hvor mange plader vil du bruge?");
//			long antalPlader = Convert.ToInt64(Console.ReadLine());

//			List<Plade> plader = new List<Plade>();

//			ChromeOptions options = new ChromeOptions();
//			options.PageLoadStrategy = PageLoadStrategy.None;

//			using (IWebDriver webdriver = new ChromeDriver(driverPath))
//			{
//				webdriver.Navigate().GoToUrl(url);

//				for (int i = 0; i < antalPlader; i++)
//				{
//					IWebElement inputField = webdriver.FindElement(By.Id("tekstboks"));
//					((IJavaScriptExecutor)webdriver).ExecuteScript($"arguments[0].value='{brugerNavn}{i}';", inputField);


//					IWebElement generateButton = webdriver.FindElement(By.Id("knap"));
//					generateButton.Click();

//					WebDriverWait wait = new WebDriverWait(webdriver, TimeSpan.FromSeconds(6));
//					wait.Until(ExpectedConditions.ElementIsVisible(By.Id("p11"))); // Vent til elementet er synligt

//					Plade plade = new Plade
//					{
//						Navn = $"{brugerNavn}{i}",
//						Række1 = ParseToIntList(webdriver.FindElement(By.Id("p11")).Text),
//						Række2 = ParseToIntList(webdriver.FindElement(By.Id("p12")).Text),
//						Række3 = ParseToIntList(webdriver.FindElement(By.Id("p13")).Text),
//						BingoStatus = new bool[3]
//					};

//					plader.Add(plade);

//					inputField.Clear();
//				}

//				GemPlader(plader);
//			}

//			søgITal(plader);
//		}

//		static List<int> ParseToIntList(string numbers)
//		{
//			return numbers.Split(' ')
//						  .Select(n => int.TryParse(n, out int result) ? result : 0)
//						  .Where(n => n != 0)
//						  .ToList();
//		}

//		static void GemPlader(List<Plade> plader)
//		{
//			string sti = @"C:\Users\blsto\source\repos\OPGCoding\banko-snyd-OPGCoding\BankoCheater\plader.json";
//			Console.Clear();
//			try
//			{
//				string json = JsonConvert.SerializeObject(plader, Formatting.Indented);
//				System.IO.File.WriteAllText(sti, json);
//				Console.WriteLine("Pladerne er gemt korrekt!");
//			}
//			catch (Exception ex)
//			{
//				Console.WriteLine($"Fejl i forsøget på at gemme pladerne! {ex.Message}");
//			}
//		}

//		static void søgITal(List<Plade> plader)
//		{
//			Console.WriteLine("-------------------------------------------------------------------");
//			Console.WriteLine("Indtast de opråbte tal (eller skriv 'done' for at afslutte): ");
//			List<int> indtastedeTal = new List<int>();
//			string input;
//			string tidligereTal = "";

//			while ((input = Console.ReadLine()) != "done")
//			{
//				if (int.TryParse(input, out int tal))
//				{

//					if (!indtastedeTal.Contains(tal)) // Check if the list already contains the number
//					{
//						indtastedeTal.Add(tal); // Add the number if it's not already in the list
//						Console.Clear();
//						Console.WriteLine("Tidligere tal: " + string.Join(" - ", indtastedeTal)); // Directly use the list to show previous numbers
//					}
//					else
//					{
//						Console.WriteLine("Tidligere tal: " + string.Join(" - ", indtastedeTal)); // Directly use the list to show previous numbers
//						continue;
//					}

//					Console.Clear();
//					Console.WriteLine("Tidligere tal: " + string.Join(" - ", indtastedeTal)); // Directly use the list to show previous numbers
//					Console.WriteLine(tidligereTal.TrimEnd());

//					foreach (var plade in plader)
//					{
//						int bingoRow = CheckBingoRow(plade, indtastedeTal);

//						if (bingoRow != -1)
//						{
//							// Tilføj kun nye bingo rækker
//							if (!plade.BingoRækker.Contains(bingoRow + 1))
//							{
//								plade.BingoRækker.Add(bingoRow + 1);
//							}

//							plade.BingoRækker.Sort(); // Sikrer, at rækkerne er sorteret

//							if (plade.BingoStatus.All(status => status)) // Håndterer fuld plade specifikt
//							{
//								Console.Write($"Bingo (FULD PLADE)! {plade.Navn}, Rækker: ");
//								for (int i = 0; i < plade.Rækker.Count; i++)
//								{
//									Console.Write($"{i + 1}: {string.Join(", ", plade.Rækker[i])}" + (i < plade.Rækker.Count - 1 ? " | " : ""));
//								}
//							}
//							else // Håndterer 1 eller 2 rækker med bingo
//							{
//								var bingoRækkerOutput = plade.BingoRækker.Select(rækkeNummer =>
//									$"{rækkeNummer}: {string.Join(", ", plade.Rækker[rækkeNummer - 1])}");

//								Console.WriteLine($"Bingo ({plade.BingoRækkeCount} RÆKKE(R))! {plade.Navn}, Rækker: {string.Join(" | ", bingoRækkerOutput)}");
//							}
//						}
//					}
//				}
//				else
//				{
//					Console.WriteLine("Ugyldigt input, prøv igen.");
//				}
//			}
//		}




//		static int CheckBingoRow(Plade plade, List<int> indtastedeTal)
//		{
//			for (int i = 0; i < plade.Rækker.Count; i++)
//			{
//				if (!plade.BingoStatus[i] && plade.Rækker[i].All(t => indtastedeTal.Contains(t)))
//				{
//					plade.BingoStatus[i] = true;
//					plade.BingoRækkeCount++; // Inkrementerer bingo række count

//					return i;
//				}
//			}
//			return -1;
//		}
//	}




//	class Plade
//	{
//		public string Navn { get; set; }
//		public List<int> Række1 { get; set; }
//		public List<int> Række2 { get; set; }
//		public List<int> Række3 { get; set; }
//		public List<List<int>> Rækker => new List<List<int>> { Række1, Række2, Række3 };
//		public bool[] BingoStatus { get; set; } // Tilføjet for at holde styr på bingo-status for hver række
//		public int BingoRækkeCount { get; set; } = 0; // Ny egenskab til at tælle bingo rækker
//		public List<int> BingoRækker { get; set; } = new List<int>();

//	}
//}