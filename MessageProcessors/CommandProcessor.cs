using System.Reactive.Linq;
using System.Reactive.Subjects;

public class CommandProcessor : IMessageProcessor
{
    private static Random random = new Random();
    private Subject<MessageDataAndChatId> responseMessageReadySubject = new Subject<MessageDataAndChatId>();
    private readonly Dictionary<string, Func<string, ChatMessage, Task>> commands;
    private static Regex commandRegex = new Regex(@"/{1}(?<command>\S*)(\s*)-{0,1}(?<args>.*)", RegexOptions.Compiled);

    public IObservable<MessageDataAndChatId> ResponseMessageReady => responseMessageReadySubject.AsObservable();

    public CommandProcessor()
    {
        commands = new Dictionary<string, Func<string, ChatMessage, Task>>(StringComparer.OrdinalIgnoreCase)
        {
             { "rolldice", HandleRollDiceAsync},
             { "flipcoin", HandleFlipCoinAsync},
             { "help", HandleHelpAsync},
             { "ask", HandleAskAsync },
             { "getimage", HandleGetImageAsync},
        };
    }

    public async Task ProcessChatMessageAsync(ChatMessage chatMessage)
    {
        if (chatMessage == null || string.IsNullOrWhiteSpace(chatMessage.Text) || !chatMessage.Text.StartsWith("/"))
            return;

        string text = chatMessage.Text.Replace('–', '-')
               .Replace('—', '-')
               .Replace('−', '-')
               .Replace('‒', '-')
               .Trim();

        var match = commandRegex.Match(text);
        if (match.Success)
        {
            var command = match.Groups["command"].Value;
            var args = match.Groups["args"].Value;

            MarkupLineInterpolated($"COMMAND: [yellow]{command}[/], ARGS: [green]{args}[/]");

            if (commands.TryGetValue(command, out var handler))
            {
                await handler(args, chatMessage);
            }
            else
            {
                responseMessageReadySubject.OnNext(CreateResponse($"❌ Sorry I do not understand the /{command} {args} command", chatMessage.ChatId));
                return;
            }
        }
        else
        {
            responseMessageReadySubject.OnNext(CreateResponse($"❌ Sorry I do not understand the command: {chatMessage.Text}", chatMessage.ChatId));
        }
    }

    private async Task HandleAskAsync(string arguments, ChatMessage chatMessage)
    {
        var index = arguments.IndexOf(' ');
        var question = arguments[index..];
        var llmResponse = LLMClient.Ask("Only provide an answer if you are 100% sure it is correct, otherwise let me know you don't know", question).Result;

        responseMessageReadySubject.OnNext(CreateResponse(llmResponse, chatMessage.ChatId));
    }

    private async Task HandleRollDiceAsync(string arguments, ChatMessage chatMessage)
    {
        var diceFaces = 6;
        var args = arguments.Split(' ');
        if (args.Length > 0)
        {
            if (int.TryParse(args[0].Replace("d", "", StringComparison.OrdinalIgnoreCase), out var faceCount))
            {
                diceFaces = faceCount;
            }
        }

        if (diceFaces < 3)
        {
            responseMessageReadySubject.OnNext(CreateResponse($"❌ Sorry I cannot roll a D{diceFaces}", chatMessage.ChatId));
            return;
        }

        responseMessageReadySubject.OnNext(CreateResponse($"🎲 You rolled a D{diceFaces}: {random.Next(0, diceFaces + 1)}", chatMessage.ChatId));
        return;
    }

    private async Task HandleFlipCoinAsync(string _, ChatMessage chatMessage)
    {
        var flipResult = random.Next(int.MaxValue) % 2 == 0 ? "Heads" : "Tails";
        responseMessageReadySubject.OnNext(CreateResponse($"🪙 You flipped a coin: {flipResult}", chatMessage.ChatId));
    }

    private async Task HandleGetImageAsync(string _, ChatMessage chatMessage)
    {
        /* This is obviously not the correct way to attach an image.
         * The error looks something like this:

         *      {"jsonrpc":"2.0","id":5125,"error":{"code":-1,"message":"Failed to create message: Copying new blobfile failed: No such file or directory (os error 2)"}}
         *      ERROR: Error { Code = -1, Message = Failed to create message: Copying new blobfile failed: No such file or directory (os error 2) }
         *
         * It seems like it wants to read a file from disk? But then what's the meaning of File and Filename?
         */

        var imageBase64 = "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";
        var imageData = Convert.FromBase64String(imageBase64);

        var messageData = new MessageData()
        {
            File = imageBase64,
            Filename = "delta.jpg",
            Text = "The delta.chat logo",
            Viewtype = ViewType.IMAGE
        };

        var messageAndChatId = new MessageDataAndChatId(messageData, chatMessage.ChatId);
        responseMessageReadySubject.OnNext(messageAndChatId);
    }


    private async Task HandleHelpAsync(string _, ChatMessage chatMessage)
    {
        var messageBuilder = new StringBuilder();
        messageBuilder.AppendLine("NG-Bot Usage");
        messageBuilder.AppendLine("\t /rolldice [-][d]<faces>\tRoll a dice");
        messageBuilder.AppendLine("\t /flipcoin\tFlip a coin");
        messageBuilder.AppendLine("\t /help\tPrint this help message");
        messageBuilder.AppendLine("");
        messageBuilder.AppendLine("Examples:");
        messageBuilder.AppendLine("\t/rolldice D20");
        messageBuilder.AppendLine("\t/rolldice D6");
        messageBuilder.AppendLine("\t/rolldice -D20");
        messageBuilder.AppendLine("\t/rolldice -D6");
        messageBuilder.AppendLine("\t/rolldice d100");
        messageBuilder.AppendLine("\t/rolldice -d100");
        
        responseMessageReadySubject.OnNext(CreateResponse($"❓ {messageBuilder}", chatMessage.ChatId));
    }

    private MessageDataAndChatId CreateResponse(string text, int chatId) => new MessageDataAndChatId(new MessageData() { Text = text, Viewtype = ViewType.TEXT }, chatId);
}


