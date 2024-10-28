using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace SignalRWhisper.Hubs;

public class WhisperHub : Hub
{
    public async Task SendMessage(byte[] message, [FromServices] IWhisperService dbService)
    {
        var sb = new StringBuilder();
        using (var ms = new MemoryStream(message))
        {
            await foreach (string text in dbService!.ProcessAsync(ms))
            {
                sb.Append(text);
            }
        }
        await Clients.Caller.SendAsync("ReceiveMessage", $"Echo:{sb.ToString()}");

    }
}
