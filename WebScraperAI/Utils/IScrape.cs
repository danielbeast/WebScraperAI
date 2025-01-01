using OpenQA.Selenium.Remote;

namespace WebScraperAI.Utils
{
    public interface IScrape
    {
        //private RemoteWebDriver CreateDriver();
        public Task<string> ScrapeWebsite(string website);
        public string ExtractBodyContent(string htmlContent);
        public string CleanBodyContent(string bodyContent);
        public List<string> SplitDomContent(string domContent, int maxLength = 6000);
    }
}
