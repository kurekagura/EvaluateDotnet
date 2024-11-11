using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;
using System.ClientModel;

namespace BasicUsage;

internal class Program
{
    public static async Task Main(string[] args)
    {
        var config = BuildAppSettings(Directory.GetCurrentDirectory());

        string endpoint = config["AzureOpenAI:Endpoint"] ?? throw new Exception("AzureOpenAI:Endpoint in appsettings.json is required.");
        string apiKey = config["AzureOpenAI:ApiKey"] ?? throw new Exception("AzureOpenAI:ApiKey in appsettings.json is required."); ;
        string model = config["AzureOpenAI:Model"] ?? throw new Exception("AzureOpenAI:Model in appsettings.json is required."); ;

        try
        {
            var client = new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(apiKey));
            var chatClient = client.GetChatClient(model);

            string systemMessage = "あなたは.NETの専門家です。";
            string userMessage = ".NETの特徴を200字程度に要約してください";

            Console.WriteLine("-- None Stream API");
            await Query_CompleteChatAsync(chatClient, systemMessage, userMessage);

            Console.WriteLine("-- Stream API");
            await Query_CompleteChatStreamingAsync(chatClient, systemMessage, userMessage);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            Console.ReadKey();
            return;
        }

        Console.WriteLine(Environment.NewLine);
        Console.WriteLine("Press any key to exit.");
        Console.ReadKey();
        return;
    }

    public async static Task Query_CompleteChatStreamingAsync(ChatClient chatClient, string systemMessage, string userMessage)
    {
        var msgs = new List<ChatMessage>()
                {
                    new SystemChatMessage(systemMessage),
                    new UserChatMessage(userMessage)
                };
        Console.WriteLine($"System:{msgs[0].Content[0].Text}");
        Console.WriteLine($"User:{msgs[1].Content[0].Text}");
        AsyncCollectionResult<StreamingChatCompletionUpdate> collectionResult = chatClient.CompleteChatStreamingAsync(msgs);
        bool isFirst = true;
        await foreach (StreamingChatCompletionUpdate stChatCompUpd in collectionResult)
        {
            if (isFirst && stChatCompUpd.Role != null)
            {
                Console.WriteLine($"{stChatCompUpd.Role}:");
                isFirst = false;
            }

            foreach (var content in stChatCompUpd.ContentUpdate)
            {
                Console.Write(content.Text);
            }
        }
    }

    public async static Task Query_CompleteChatAsync(ChatClient chatClient, string systemMessage, string userMessage)
    {
        var sysMsg = new SystemChatMessage(systemMessage);
        var usrMsg = new UserChatMessage(userMessage);
        var msgs = new List<ChatMessage>();
        msgs.Add(sysMsg);
        msgs.Add(usrMsg);
        Console.WriteLine($"System:{sysMsg.Content[0].Text}");
        Console.WriteLine($"User:{usrMsg.Content[0].Text}");
        //var chatCompOpts = new ChatCompletionOptions { MaxOutputTokenCount = 500, Temperature = 0.7f, };
        var clientResult = await chatClient.CompleteChatAsync(msgs);

        ChatCompletion chatComple = clientResult.Value;
        var contentList = chatComple.Content;
        string text = contentList.Count > 0 ? contentList[0].Text : string.Empty;
        Console.WriteLine($"{clientResult.Value.Role}:{Environment.NewLine}{text}");
    }

    public static IConfiguration BuildAppSettings(string basePath)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();

        return configuration;
    }
}

