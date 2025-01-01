using WebScraperAI.Models;

namespace WebScraperAI.Services
{
    public interface IScrapeAndParseService
    {
        Task<PromptModel> Post(PromptModel model);
    }
}