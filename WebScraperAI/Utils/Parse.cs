using Mscc.GenerativeAI;
using System.Text.Json;

namespace WebScraperAI.Utils
{
    public class Parse : IParse
    {
        private GenerativeModel _model;
        private GoogleAI _googleAI;
        private readonly string _apiKey = "";

        public Parse()
        {
            _apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
            _googleAI = new GoogleAI(apiKey: _apiKey);
            _model = _googleAI.GenerativeModel(model: Model.Gemini15Pro);
        }

        private string CleanJsonResponse(string text)
        {
            // Remove markdown code blocks if present
            text = text.Replace("```json", "").Replace("```", "").Trim();

            try
            {
                // Try to find JSON content between curly braces
                int start = text.IndexOf('{');
                int end = text.LastIndexOf('}') + 1;
                string jsonStr = text.Substring(start, end - start);

                // Parse and re-format JSON
                var parsedJson = JsonSerializer.Deserialize<object>(jsonStr);
                return JsonSerializer.Serialize(parsedJson, new JsonSerializerOptions { WriteIndented = true });
            }
            catch (Exception)
            {
                return text;
            }
        }

        public async Task<string> ParseWithGemini(List<string> domChunks, string parseDescription)
        {
            const string promptTemplate = @"
        Extract information from the following text content and return it as a CLEAN JSON object.

        Text content: {0}

        Instructions:
        1. Extract information matching this description: {1}
        2. Return ONLY a valid JSON object, no other text or markdown
        3. If no information is found, return an empty JSON object {{}}
        4. Ensure the JSON is properly formatted and valid
        5. DO NOT include any explanatory text, code blocks, or markdown - ONLY the JSON object
        ";

            var parsedResults = new List<string>();

            for (int i = 0; i < domChunks.Count; i++)
            {
                try
                {
                    string prompt = string.Format(promptTemplate, domChunks[i], parseDescription);
                    var response = await _model.GenerateContent(prompt);
                    string result = CleanJsonResponse(response.Text.Trim());

                    if (!string.IsNullOrEmpty(result) && result != "{}")
                    {
                        parsedResults.Add(result);
                    }
                    Console.WriteLine($"Parsed batch: {i + 1} of {domChunks.Count}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing chunk {i + 1}: {ex.Message}");
                    continue;
                }
            }

            // Combine results if multiple chunks produced output
            if (parsedResults.Count > 1)
            {
                try
                {
                    // Parse all results into objects
                    var jsonObjects = parsedResults.Select(result =>
                        JsonSerializer.Deserialize<object>(result)).ToList();

                    // Check if all objects are dictionaries
                    if (jsonObjects.All(obj => obj is JsonElement element &&
                        element.ValueKind == JsonValueKind.Object))
                    {
                        // Merge dictionaries
                        var merged = new Dictionary<string, JsonElement>();
                        foreach (var obj in jsonObjects)
                        {
                            var element = (JsonElement)obj;
                            foreach (var prop in element.EnumerateObject())
                            {
                                merged[prop.Name] = prop.Value;
                            }
                        }
                        return JsonSerializer.Serialize(merged, new JsonSerializerOptions { WriteIndented = true });
                    }

                    // If they're lists or mixed, combine them
                    return JsonSerializer.Serialize(jsonObjects, new JsonSerializerOptions { WriteIndented = true });
                }
                catch (JsonException)
                {
                    // If merging fails, return the first valid result
                    return parsedResults[0];
                }
            }

            // Return the single result or empty JSON object
            return parsedResults.Count > 0 ? parsedResults[0] : "{}";
        }
    }
}
