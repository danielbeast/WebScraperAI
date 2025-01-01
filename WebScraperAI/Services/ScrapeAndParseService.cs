using Microsoft.AspNetCore.Mvc;
using WebScraperAI.Models;
using WebScraperAI.Utils;

namespace WebScraperAI.Services
{
    public class ScrapeAndParseService : IScrapeAndParseService
    {
        private readonly IParse _parse;
        private readonly IScrape _scrape;

        public ScrapeAndParseService(IParse parse,
            IScrape scrape)
        {
            _parse = parse;
            _scrape = scrape;
        }

        [HttpPost]
        public async Task<PromptModel> Post(PromptModel model)
        {
            //Scrape the website
            var domContent = await _scrape.ScrapeWebsite(model.Url);
            var bodyContent = _scrape.ExtractBodyContent(domContent);
            var cleanedContent = _scrape.CleanBodyContent(bodyContent);

            //Parse the website
            var domChunks = _scrape.SplitDomContent(cleanedContent);
            var result = await _parse.ParseWithGemini(domChunks, model.ParseDescription);

            model.Data = result;
            return model;
        }
    }
}
