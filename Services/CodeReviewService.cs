using Azure;
using OpenAI.Chat;
using Azure.AI.OpenAI;
using CodeReviewAssistant.Models;

namespace CodeReviewAssistant.Services
{
    public interface ICodeReviewService
    {
        Task<string> PerformReviewAsync(ReviewRequest request);
    }

    public class CodeReviewService(IConfiguration config) : ICodeReviewService
    {
        public async Task<string> PerformReviewAsync(ReviewRequest request)
        {
            string endpoint = config["OpenAi:EndPoint"];
            string key = config["OpenAi:Key"];

            AzureOpenAIClient client = new(new Uri(endpoint), new AzureKeyCredential(key));
            ChatClient chatClient = client.GetChatClient(config["OpenAi:DeploymentName"]);

            var file = request.Files.First();

            var prompt = $"""
You are a senior C# engineer reviewing a pull request file. Analyze the following code diff:

{file.Diff}

Provide a brief, high-level summary of the changes focusing on key points only. 
Keep it short and to the point, consider clean code, clean naming, no hardcoded values, no magic strings,
follow all the OO principles, SOLID principles, and design patterns. you can suggest code example if needed.
""";

            var completion = await chatClient.CompleteChatAsync(
                [
                new SystemChatMessage("You are a senior code reviewer."),
                new UserChatMessage(prompt),

                ], 
                new ChatCompletionOptions()
                {
                    // Lower the value to get a more concise response
                    MaxOutputTokenCount = 250,
                    Temperature = 0.3f
                });

            var reviewText = completion.Value.Content[0].Text;

            return reviewText;
        }
    }
}
