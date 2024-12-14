using System.ClientModel;
using System.Text.Json;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;

namespace FCWhatUserWant;

internal class Program
{
    static async Task Main(string[] args)
    {
        var config = BuildAppSettings(Directory.GetCurrentDirectory());

        string endpoint = config["AzureOpenAI:Endpoint"] ?? throw new Exception("AzureOpenAI:Endpoint in appsettings.json is required.");
        string apiKey = config["AzureOpenAI:ApiKey"] ?? throw new Exception("AzureOpenAI:ApiKey in appsettings.json is required."); ;
        string depName = config["AzureOpenAI:DeploymentName"] ?? throw new Exception("AzureOpenAI:DeploymentName in appsettings.json is required."); ;

        try
        {
            var client = new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(apiKey));
            var chatClient = client.GetChatClient(depName);

            var chatMsgs = new List<ChatMessage>();

            //var sysMsgStr = "あなたはユーザーの入力した文章からユーザーの意図・目的を理解できるAIアシスタンスです。ユーザーの入力は「原因の調査」または「事例の照会」のどちらかです。";
            var sysMsgStr = "あなたはユーザーの入力した文章からユーザーの意図・目的を理解できるAIアシスタンスです。ユーザーの入力は「原因の調査」または「事例の照会」のどちらかの1つです。";

            var sysMsg = new SystemChatMessage(sysMsgStr);
            chatMsgs.Add(sysMsg);

            string[] usrMsgsStr =
            [
    "PC起動時にメモリが認識されない問題が発生しています。事例から原因を推察してください。",
    "PC起動時にメモリが認識されない問題が発生しています。類似の事例から原因を教えてください。",
    "PC起動時にメモリが認識されない不具合が発生しています。原因を調査してください。",
    "PC起動時にメモリが認識されない不具合の原因は？",
    "PC起動時にメモリが認識されません。何故ですか？",
    "PC起動時にメモリが認識されない不具合の事例を教えてください。",
    "PC起動時にメモリが認識されない不具合が発生しています。同様の過去トラブルを教えてください。",
    "PC起動時にメモリが認識されない過去トラブルはありますか？"
            ];

            foreach (string usrMsgStr in usrMsgsStr)
            {
                var usrMsg = new UserChatMessage(usrMsgStr);
                chatMsgs.Add(usrMsg);

                Console.WriteLine($"文章：{Environment.NewLine}{usrMsgStr}");
                var purpose = await CompleteChatToolCallAsync(chatClient, chatMsgs);
                Console.WriteLine("ユーザーの目的：");
                if (purpose == user_purpose_type.NONE)
                {
                    Console.WriteLine("意図の解析に失敗");
                }
                else
                {
                    if (purpose.HasFlag(user_purpose_type.INVESTIGATE_CAUSE))
                        Console.WriteLine(nameof(user_purpose_type.INVESTIGATE_CAUSE));
                    if (purpose.HasFlag(user_purpose_type.REFER_CASE))
                        Console.WriteLine(nameof(user_purpose_type.REFER_CASE));
                }
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return;
        }
        finally
        {
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
        return;
    }

    private void get_information_by_user_purpose(user_purpose_type user_purpose_type)
    {
    }

    private static ChatTool GetTool()
    {
        //        var jsonDef = @"{
        //  ""type"": ""object"",
        //  ""properties"": {
        //    ""user_purpose_type"": {
        //      ""type"": ""string"",
        //      ""description"": ""The possible values are either INVESTIGATE_CAUSE or REFER_CASE.""
        //      }
        //    },
        //    ""required"": [""user_purpose_type""]
        //}";

        var jsonDef = $@"{{
  ""type"": ""object"",
  ""properties"": {{
    ""{nameof(user_purpose_type)}"": {{
      ""type"": ""string"",
      ""description"": ""The possible values are either {nameof(user_purpose_type.INVESTIGATE_CAUSE)} or {nameof(user_purpose_type.REFER_CASE)}.""
      }}
    }},
    ""required"": [""{nameof(user_purpose_type)}""]
}}";

        ChatTool tool = ChatTool.CreateFunctionTool(
            nameof(get_information_by_user_purpose),
            "Retrieves information based on the user's purpose.",
            BinaryData.FromString(jsonDef));
        return tool;
    }


    public async static Task<user_purpose_type> CompleteChatToolCallAsync(ChatClient chatClient, List<ChatMessage> chatMsgs)
    {
        user_purpose_type results = user_purpose_type.NONE;

        var options = new ChatCompletionOptions() { Tools = { GetTool() }, };

        ChatCompletion completion = await chatClient.CompleteChatAsync(chatMsgs, options);

        if (completion.FinishReason == ChatFinishReason.ToolCalls)
        {
            foreach (var toolCall in completion.ToolCalls)
            {
                if (toolCall.FunctionName == nameof(get_information_by_user_purpose))
                {
                    using var argumentsDocument = JsonDocument.Parse(toolCall.FunctionArguments);

                    if (argumentsDocument.RootElement.TryGetProperty(nameof(user_purpose_type), out JsonElement functionArg))
                    {
                        if (Enum.TryParse(functionArg.ToString(), out user_purpose_type result))
                        {
                            results |= result;
                            break;
                        }
                    }
                }
            }
        }
        return results;
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

public enum user_purpose_type
{
    NONE = 0,
    INVESTIGATE_CAUSE = 1,
    REFER_CASE = 2
}
