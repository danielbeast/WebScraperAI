using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace WebScraperAI.Utils
{
    public class Scrape : IScrape
    {
        private readonly string _sbrDriver;

        private ChromeOptions _options;

        public Scrape()
        {
            _sbrDriver = Environment.GetEnvironmentVariable("SBR_DRIVER");
            _options = new ChromeOptions();
            _options.AddArgument("--no-sandbox");
            _options.AddArgument("--headless");
            _options.AddArgument("--disable-dev-shm-usage");
        }

        private RemoteWebDriver CreateDriver()
        {
            return new RemoteWebDriver(new Uri(_sbrDriver), _options);
        }

        public async Task<string> ScrapeWebsite(string website)
        {
            int max_retries = 3;
            int retry_delay = 2;

            for (int i = 0; i <= max_retries; i++)
            {

                RemoteWebDriver driver = null;
                try
                {
                    Console.WriteLine("Attempt " + (i + 1) + " of " + max_retries);
                    Console.WriteLine("Connecting to Scraping Browser...");
                    driver = CreateDriver();
                    Console.WriteLine("Waiting for page to load...");
                    await driver.Navigate().GoToUrlAsync(website);                    
                    Console.WriteLine("Checking for captcha...");

                    try
                    {
                        var solveRes = ((IJavaScriptExecutor)driver).ExecuteAsyncScript(
                        "return await window.executeCdpCommand('Captcha.waitForSolve', " +
                        "{detectTimeout: 10000});"
                    );
                        Console.WriteLine($"Captcha solve status: {solveRes}");

                    }
                    catch (WebDriverException ex)
                    {
                        Console.WriteLine("No captcha detected or captcha handling failed:", ex);
                    }

                    Console.WriteLine("Scraping page content...");
                    var html = driver.PageSource;

                    if (!string.IsNullOrEmpty(html) && html.Length > 0)
                    {
                        Console.WriteLine("Scraping complete.");
                        return html;
                    }
                    else
                    {
                        throw new Exception("Empty page content received");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error during attempt {attempt + 1}: {str(e)}");
                    if (i < max_retries - 1)
                    {
                        Console.WriteLine("Retrying in {retry_delay} seconds...");
                        Thread.Sleep(retry_delay * 1000);
                    }
                    else
                    {
                        throw new Exception("Failed to scrape after {max_retries} attempts: " + ex);
                    }
                }
                finally
                {
                    driver?.Quit();
                }
            }
            return "";
        }

        public string ExtractBodyContent(string htmlContent)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);
            var body = htmlDoc.DocumentNode.SelectSingleNode("//body");
            return body != null ? body.OuterHtml : string.Empty;
        }

        public string CleanBodyContent(string bodyContent)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(bodyContent);

            foreach (var scriptOrStyle in htmlDoc.DocumentNode.SelectNodes("//script|//style") ?? Enumerable.Empty<HtmlNode>())
            {
                scriptOrStyle.Remove();
            }

            var cleanedContent = htmlDoc.DocumentNode.InnerText;
            return string.Join("\n", cleanedContent.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                                                     .Select(line => line.Trim()));
        }

        public List<string> SplitDomContent(string domContent, int maxLength = 6000)
        {
            List<string> result = new List<string>();
            for (int i = 0; i < domContent.Length; i += maxLength)
            {
                result.Add(domContent.Substring(i, Math.Min(maxLength, domContent.Length - i)));
            }
            return result;
        }
    }
}