using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.ComponentModel;

class LLMClient
{
    public static async Task<string> Ask(string systemPrompt, string userPrompt)
    {        
        string uri = "http://localhost:1234/v1/chat/completions";

        try
        {            
            using var client = new HttpClient();
            
            var json = new
            {
                model = "qwen/qwen3-1.7b",
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userPrompt }
                },
                temperature = 0.7,
                max_tokens = -1, 
                stream = false
            };

            var jsonString = JsonConvert.SerializeObject(json);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            
            var response = await client.PostAsync(uri, content);

            if (!response.IsSuccessStatusCode)
            {                                
                return "Invalid status code";
            }
            
            var responseContent = await response.Content.ReadAsStringAsync();  

            var responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(responseContent);
            if (responseObject?.Choices?.Count > 0)
            {
                if (!String.IsNullOrWhiteSpace(responseObject.Choices[0].Message?.Content))
                {
                    var responseText = responseObject.Choices[0]!.Message!.Content!;
                    var thinkEnd = responseText.IndexOf("</think>")+8;
                    responseText = responseText[thinkEnd..].Replace("**", "❞");
                    return responseText;   
                }            
                else
                {
                    return "❌ Sorry, the response was invalid";
                }
            }
            else
            {
                return "❌ Sorry, failed to generate a response";
            }            
        }
        catch (Exception ex)
        {
            return $"An error occurred: {ex.Message}";
        }
    }
}


public class RootObject
{
    [JsonProperty("id")]
    public string? Id { get; set; }

    [JsonProperty("object")]
    public string? ChatObject { get; set; }

    [JsonProperty("created")]
    public long Created { get; set; }

    [JsonProperty("model")]
    public string? Model { get; set; }

    [JsonProperty("choices")]
    public List<Choice>? Choices { get; set; }

    [JsonProperty("usage")]
    public Object? Usage { get; set; }

    [JsonProperty("stats")]
    public Dictionary<string, object>? Stats { get; set; }

    [JsonProperty("system_fingerprint")]
    public string? SystemFingerPrint { get; set; }
}

public class Choice
{
    [JsonProperty("index")]
    public int Index { get; set; }
    [JsonProperty("message")]
    public Message? Message { get; set; }
}

public class Message
{
    [JsonProperty("role")]
    public string? Role { get; set; }

    [JsonProperty("content")]
    public string? Content { get; set; }

    [JsonProperty("tool_calls")]
    public List<ToolCall>? tool_calls { get; set; }
}

public class ToolCall
{
    //Placeholder
}

