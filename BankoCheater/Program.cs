using System;
using System.Collections.Generic;
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

			Console.Write("Hvad er dit navn? ");
			string pladeNavn = Console.ReadLine();

			Console.Write("Hvor mange plader ønsker du? ");
			long pladeAntal = Convert.ToInt64(Console.ReadLine());

			// Opretter en instans af ChromeDriver uden for løkken
			IWebDriver banko = new ChromeDriver(driverPath);

			// Åbner websiden hvor inputfeltet er placeret
			banko.Navigate().GoToUrl(url);

			List<Plade> plader = new List<Plade>();

			for (int i = 0; i < pladeAntal; i++)
			{
				// Find inputfeltet ved hjælp af dens id
				IWebElement inputField = banko.FindElement(By.Id("tekstboks"));

				// Indtast navn i inputfeltet
				string name = pladeNavn + (i + 1); // Tilføj pladenummer til navnet
				inputField.SendKeys(name);

				// Find og klik på generér-knappen
				IWebElement generateButton = banko.FindElement(By.Id("knap"));
				generateButton.Click();

				// Vent på, at pladen genereres
				System.Threading.Thread.Sleep(0); // Vent i 1 sekund (juster efter behov)

				// Find pladeelementerne ved hjælp af deres id
				IWebElement pladeElement = banko.FindElement(By.Id("p11"));
				IWebElement pladeElement2 = banko.FindElement(By.Id("p12"));
				IWebElement pladeElement3 = banko.FindElement(By.Id("p13"));

				// Udskriv bankoplade på hjemmesiden
				Console.WriteLine("Bankoplade på hjemmesiden:");
				Console.WriteLine(pladeElement.Text);
				Console.WriteLine(pladeElement2.Text);
				Console.WriteLine(pladeElement3.Text);

				// Tilføj pladen til listen
				Plade plade = new Plade
				{
					Navn = name,
					Række1 = pladeElement.Text,
					Række2 = pladeElement2.Text,
					Række3 = pladeElement3.Text
				};
				plader.Add(plade);

				inputField.Clear();
			}

			// Gem pladerne i en JSON-fil
			GemPlader(plader);

			// Luk browseren uden for løkken, når du er færdig med at bruge den
			banko.Quit();
		}

		static void GemPlader(List<Plade> plader)
		{
			string sti = @"C:\Users\blsto\source\repos\OPGCoding\banko-snyd-OPGCoding\BankoCheater\plader.json";

			try
			{
				string json = JsonConvert.SerializeObject(plader, Formatting.Indented);
				System.IO.File.WriteAllText(sti, json);
				Console.WriteLine("Pladerne er gemt i plader.json filen.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Fejl ved gemning af pladerne: {ex.Message}");
			}
		}


		



	}

	class Plade
	{
		public string Navn { get; set; }
		public string Række1 { get; set; }
		public string Række2 { get; set; }
		public string Række3 { get; set; }
	}
}
