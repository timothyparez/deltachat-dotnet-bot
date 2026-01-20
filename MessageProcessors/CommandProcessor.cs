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

        var imageBase64 = "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%";
        

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


