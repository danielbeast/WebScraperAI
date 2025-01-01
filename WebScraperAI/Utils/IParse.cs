namespace WebScraperAI.Utils
{
    public interface IParse
    {
        public Task<string> ParseWithGemini(List<string> domChunks, string parseDescription);
    }
}
