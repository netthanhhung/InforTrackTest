using InfoTrack_SEOWeb.Handler.Infrastructure;
using InfoTrack_SEOWeb.Handler.Queries;
using InfoTrack_SEOWeb.Models;
using InfoTrack_SEOWeb.Providers.Scrapper.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Xml;

namespace InfoTrack_SEOWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly IBroker broker;

        public HomeController(ILogger<HomeController> logger, IBroker broker)
        {
            this.logger = logger;
            this.broker = broker;
        }

        public IActionResult Index()
        {
            logger.LogInformation("info");
            return View(new SearchEngineScrapperQuery("", ""));
        }

        [HttpPost]
        public async Task<IActionResult> Index(SearchEngineScrapperQuery query)
        {
            if (!ModelState.IsValid)
            {
                return View(query);
            }
            try
            {
                ViewBag.ResultFound = await this.broker.QueryAsync(query);
            }
            catch (XmlException ex)
            {
                string message = $"Error while parsing html content.";
                this.logger.LogError(ex, message);
                ViewBag.ErrorMessage = message;
            }
            catch (ParentNodeNotFoundException ex)
            {
                string message = $"Error while extracting content in html page.";
                this.logger.LogError(ex, message);
                ViewBag.ErrorMessage = message;
            }
            catch (HttpRequestException ex)
            {
                string message = $"Error while getting content from google. Please try again after a while. {ex.Message}";
                this.logger.LogError(ex, message);
                ViewBag.ErrorMessage = message;

            }
            return View(query);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
