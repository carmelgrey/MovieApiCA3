using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SeleniumTests
{
    [TestClass]
    public class PopularMoviesTests
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        [TestInitialize]
        public void SetUp()
        {
            driver = new ChromeDriver();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        [TestMethod]
        public void TestPopularMoviesPageHeader()
        {
            // navigate to the Popular Movies page
            driver.Navigate().GoToUrl("http://localhost:5196/popular-movies");

            // wait until the page header shows on the page
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("h1.text-center")));

            // find the page header and get the text
            var pageHeader = driver.FindElement(By.CssSelector("h1.text-center")).Text;

            // check that the page header matches the expected text
            Assert.AreEqual("Popular Movies", pageHeader, "Page header does not match.");
        }

        [TestMethod]
        public void TestMovieCardsLoad()
        {
            // navigate to the Popular Movies page
            driver.Navigate().GoToUrl("http://localhost:5196/popular-movies");

            // wait until at least one movie card is visible
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".movie-card")));

            // find all elements with the movie card class
            var movieCards = driver.FindElements(By.CssSelector(".movie-card"));

            // verify that there is at least one movie card on the page
            Assert.IsTrue(movieCards.Count > 0, "No movie cards are found.");
        }

        [TestMethod]
        public void TestSearchFunctionality()
        {
            // navigate to the Popular Movies page
            driver.Navigate().GoToUrl("http://localhost:5196/popular-movies");

            // wait until the search bar shows up
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[placeholder='Search movies...']")));

            // find the search input field and the search button
            var searchInput = driver.FindElement(By.CssSelector("input[placeholder='Search movies...']"));
            var searchButton = driver.FindElement(By.CssSelector("button.btn-primary"));

            // enter a search in the search bar
            searchInput.SendKeys("Moana");

            // click the search button
            searchButton.Click();

            // wait until movie cards become visible
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".movie-card")));

            // find all movie cards
            var filteredMovies = driver.FindElements(By.CssSelector(".movie-card"));

            // verify that there are filtered movie cards
            Assert.IsTrue(filteredMovies.Count > 0, "No movies found after the search.");
        }

        [TestMethod]
        public void TestPagination()
        {
            driver.Navigate().GoToUrl("http://localhost:5196/popular-movies");

            // wait until the pagination is visible
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".pagination-controls button")));

            // find the "Next" button in the pagination controls
            var nextButton = driver.FindElements(By.CssSelector(".pagination-controls button"))[1];

            // check if the "Next" button is enabled
            if (nextButton.Enabled)
            {
                // click the "Next" button
                nextButton.Click();

                // wait until new movie cards load on the next page
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".movie-card")));

                // find all movie cards
                var movieCardsAfterClick = driver.FindElements(By.CssSelector(".movie-card"));

                // verify that the new page has movie cards
                Assert.IsTrue(movieCardsAfterClick.Count > 0, "Pagination did not load new results.");
            }
            else
            {
                // if pagination is not available
                Assert.Inconclusive("Pagination is not available.");
            }
        }


        [TestMethod]
        public void TestSearchMoviesReturnsResults()
        {
            driver.Navigate().GoToUrl("http://localhost:5196/search-movies");

            // wait for the search bar to show
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".search-bar input")));

            // input a movie and search
            var searchInput = driver.FindElement(By.CssSelector(".search-bar input"));
            searchInput.SendKeys("Inception");

            var searchButton = driver.FindElement(By.CssSelector(".search-bar button"));
            searchButton.Click();

            // wait for movie cards to load
            wait.Until(d => d.FindElements(By.CssSelector(".movies-grid .movie-card")).Count > 0);

            var movieCards = driver.FindElements(By.CssSelector(".movies-grid .movie-card"));
            Assert.IsTrue(movieCards.Count > 0, "No movies were returned after a valid search.");
        }

        [TestMethod]
        public void TestNoResultsFound()
        {
            driver.Navigate().GoToUrl("http://localhost:5196/search-movies");

            // wait for the search bar to show
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".search-bar input")));

            // input a movie that isnt real
            var searchInput = driver.FindElement(By.CssSelector(".search-bar input"));
            searchInput.SendKeys("Moviethatdoesntexist");

            var searchButton = driver.FindElement(By.CssSelector(".search-bar button"));
            searchButton.Click();

            // wait for no results message to load up
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".no-results")));

            var noResults = driver.FindElement(By.CssSelector(".no-results"));
            Assert.IsTrue(noResults.Displayed, "Expected no results message, but it was not displayed.");
        }

        [TestMethod]
        public void TestFirstSearchResultNavigation()
        {
            driver.Navigate().GoToUrl("http://localhost:5196/search-movies");

            // wait for the search bar to show up
            wait.Until(d => d.FindElement(By.CssSelector(".search-bar input")).Displayed);

            // enter a movie and click search
            var searchInput = driver.FindElement(By.CssSelector(".search-bar input"));
            searchInput.SendKeys("Inception");

            var searchButton = driver.FindElement(By.CssSelector(".search-bar button"));
            searchButton.Click();

            // wait for search results to load
            wait.Until(d => d.FindElements(By.CssSelector(".movies-grid .movie-card")).Count > 0);

            // click the first search result
            var firstMovieCard = driver.FindElements(By.CssSelector(".movies-grid .movie-card"))[0];
            firstMovieCard.Click();

            // wait for navigation to the movie details page
            wait.Until(d => d.FindElement(By.CssSelector(".movie-details")).Displayed);

            // assert true if we're on the details page
            Assert.IsTrue(true, "Navigation to the movie details page successful.");
        }

        [TestCleanup]
        public void TearDown()
        {
            driver.Quit();
        }
    }

}