using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Selenium_Practice
{
    public class Tests
    {
        [TestFixture]
        public class GoogleSearchE2ETests
        {
            private IWebDriver driver;
            private const string googleUrl = "https://www.google.com/?hl=en";


            [SetUp]
            public void SetUp()
            {
                var options = new ChromeOptions();
                options.AddArgument("--lang=en");
                options.AddUserProfilePreference("intl.accept_languages", "en");

                driver = new ChromeDriver(options);
                
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                driver.Manage().Window.Maximize();

            }

            [Test, TestCaseSource(nameof(SearchQueries))]
            public void GoogleSearch_ShouldDisplayResults_WhenSearchingForNUnit(string query)
            {
                driver.Navigate().GoToUrl(googleUrl);

                var searchBox = driver.FindElement(By.Name("q"));
                searchBox.SendKeys(query);
                searchBox.Submit();

                var results = driver.FindElements(By.CssSelector("h3"));
                Console.WriteLine($"we have {results.Count} serach resluts from {query}");
                Assert.IsTrue(results.Count > 0, "No search results were found.");
            }

            [Test, TestCaseSource(nameof(SearchQueries))]
            public void GoogleSearch_VerifyResultsContainQuery_AndPageTitle(string searchQuery)
            {
                driver.Navigate().GoToUrl(googleUrl);
                Console.WriteLine($"Navigated to {googleUrl}");

                AcceptCookiesIfPresent();

                PerformSearch(searchQuery);

                Thread.Sleep(500);

                Assert.IsTrue(driver.Title.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0,
     $"Expected page title to contain '{searchQuery}', but got '{driver.Title}'.");

                var results = driver.FindElements(By.CssSelector("h3")).ToList();
                Assert.IsTrue(results.Count > 0, "Expected to find at least one search result.");
                Thread.Sleep(500);

                bool queryInResults = results.Any(result => result.Text.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));
                Assert.IsTrue(queryInResults, $"None of the result titles contain the search query '{searchQuery}'.");

                Console.WriteLine($"Verified that results for '{searchQuery}' were found and contained the search query.");
            }


            [Test]
            public void GoogleSearch_VerifySearchResultNavigation()
            {
                string searchQuery = "NUnit documentation";

                driver.Navigate().GoToUrl(googleUrl);
                AcceptCookiesIfPresent();
                PerformSearch(searchQuery);

                var results = driver.FindElements(By.CssSelector("h3")).ToList();
                Assert.IsTrue(results.Count > 0, "No search results found.");

                var firstResult = results.First();
                string firstResultTitle = firstResult.Text;
                firstResult.Click();

                Thread.Sleep(500);  

                Assert.IsTrue(driver.Title.Contains("NUnit", StringComparison.OrdinalIgnoreCase),
                    $"Expected the page title to contain 'NUnit' after clicking a result. Actual title: '{driver.Title}'.");

                Console.WriteLine($"Navigated to '{driver.Url}' with title containing 'NUnit'.");
            }

            [TearDown]
            public void TearDown()
            {
                driver.Quit();
            }

            private void PerformSearch(string query)
            {
                var searchBox = driver.FindElement(By.Name("q"));
                searchBox.Clear();
                searchBox.SendKeys(query);
                searchBox.Submit();
                Console.WriteLine($"Performed search with query '{query}'.");
            }

            private void AcceptCookiesIfPresent()
            {
                try
                {
                    var acceptCookiesButton = driver.FindElement(By.XPath("//button[text()='I agree' or text()='Accept all']"));
                    acceptCookiesButton.Click();
                    Console.WriteLine("Accepted cookies.");
                }
                catch (NoSuchElementException)
                {
                    Console.WriteLine("No cookies prompt found.");
                }
            }

            private static IEnumerable<string> SearchQueries()
            {
                return new List<string> { "NUnit", "Selenium WebDriver", "FaceBook", "Instagram" , "iubenda" };
            }
        }
    }
}