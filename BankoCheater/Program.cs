using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Newtonsoft.Json;

namespace BankoCheater
{
	internal class Program
	{
		static void Main(string[] args)
		{
			string url = "https://mags-template.github.io/Banko/";
			string driverPath = @"C:\Users\blsto\source\repos\OPGCoding\banko-snyd-OPGCoding\BankoCheater\chromedriver.exe";

			Console.WriteLine("Hvad er dit navn? ");
			string brugerNavn = Console.ReadLine();

			Console.WriteLine("Hvor mange plader vil du bruge?");
			int antalPlader = Convert.ToInt32(Console.ReadLine());

			List<Plade> plader = new List<Plade>();

			using (IWebDriver webdriver = new ChromeDriver(driverPath))
			{
				webdriver.Navigate().GoToUrl(url);

				for (int i = 0; i < antalPlader; i++)
				{
					IWebElement inputField = webdriver.FindElement(By.Id("tekstboks"));
					inputField.SendKeys(brugerNavn + i);

					IWebElement generateButton = webdriver.FindElement(By.Id("knap"));
					generateButton.Click();

					System.Threading.Thread.Sleep(0); // Vent til pladen er genereret

					Plade plade = new Plade
					{
						Navn = $"{brugerNavn}{i}",
						Række1 = ParseToIntList(webdriver.FindElement(By.Id("p11")).Text),
						Række2 = ParseToIntList(webdriver.FindElement(By.Id("p12")).Text),
						Række3 = ParseToIntList(webdriver.FindElement(By.Id("p13")).Text),
						BingoStatus = new bool[3]
					};

					plader.Add(plade);

					inputField.Clear();
				}

				GemPlader(plader);
			}

			søgITal(plader);
		}

		static List<int> ParseToIntList(string numbers)
		{
			return numbers.Split(' ')
						  .Select(n => int.TryParse(n, out int result) ? result : 0)
						  .Where(n => n != 0)
						  .ToList();
		}

		static void GemPlader(List<Plade> plader)
		{
			string sti = @"C:\Users\blsto\source\repos\OPGCoding\banko-snyd-OPGCoding\BankoCheater\plader.json";
			Console.Clear();
			try
			{
				string json = JsonConvert.SerializeObject(plader, Formatting.Indented);
				System.IO.File.WriteAllText(sti, json);
				Console.WriteLine("Pladerne er gemt korrekt!");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Fejl i forsøget på at gemme pladerne! {ex.Message}");
			}
		}

		static void søgITal(List<Plade> plader)
		{
			Console.WriteLine("-------------------------------------------------------------------");
			Console.WriteLine("Indtast de opråbte tal (eller skriv 'done' for at afslutte): ");
			List<int> indtastedeTal = new List<int>();
			string input;

			while ((input = Console.ReadLine()) != "done")
			{
				if (int.TryParse(input, out int tal))
				{
					indtastedeTal.Add(tal);
					Console.WriteLine($"Tilføjede {tal}. Indtast næste tal eller skriv 'done' for at afslutte.");

					foreach (var plade in plader)
					{
						UpdateBingoStatus(plade, indtastedeTal);

						int bingoCount = plade.BingoStatus.Count(b => b);
						if (bingoCount == 1)
						{
							Console.WriteLine($"\n\tBingo på ( 1 ) række! {plade.Navn} har bingo.\n");
						}
						else if (bingoCount == 2)
						{
							Console.WriteLine($"\n\tBingo på ( 2 ) rækker! {plade.Navn} har bingo på to rækker.\n");
						}
						else if (bingoCount == 3)
						{
							Console.WriteLine($"\n Bingo ( Fuld ) plade! {plade.Navn} har bingo på alle tre rækker.\n");
							return; // Afslutter, når en fuld plade er opnået
						}
					}
				}
				else
				{
					Console.WriteLine("Ugyldigt input, prøv igen.");
				}
			}
		}

		static void UpdateBingoStatus(Plade plade, List<int> indtastedeTal)
		{
			List<List<int>> rækker = new List<List<int>> { plade.Række1, plade.Række2, plade.Række3 };

			for (int i = 0; i < rækker.Count; i++)
			{
				if (!rækker[i].Except(indtastedeTal).Any() && !plade.BingoStatus[i])
				{
					plade.BingoStatus[i] = true; // Markerer rækken som havende bingo
				}
			}
		}
	}

	class Plade
	{
		public string Navn { get; set; }
		public List<int> Række1 { get; set; }
		public List<int> Række2 { get; set; }
		public List<int> Række3 { get; set; }
		public bool[] BingoStatus { get; set; } // Tilføjet for at holde styr på bingo-status for hver række
	}
}
