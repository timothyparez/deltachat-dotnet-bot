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
             { "ask", HandleAskAsync }
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
                var messageDataAndChatId = new MessageDataAndChatId(new MessageData() { Text = $"❌ Sorry I do not understand the /{command} {args} command" }, chatMessage.ChatId);
                responseMessageReadySubject.OnNext(messageDataAndChatId);
                return;
            }
        }
        else
        {
            var messageDataAndChatId = new MessageDataAndChatId(new MessageData { Text = $"❌ Sorry I do not understand the command: {chatMessage.Text}", Viewtype = ViewType.TEXT }, chatMessage.ChatId);
            responseMessageReadySubject.OnNext(messageDataAndChatId);            
        }
    }

    private async Task HandleAskAsync(string arguments, ChatMessage chatMessage)
    {
        var index = arguments.IndexOf(' ');
        var question = arguments[index..];
        var response = LLMClient.Ask("Only provide an answer if you are 100% sure it is correct, otherwise let me know you don't know", question).Result;
        
        var messageDataAndChatId = new MessageDataAndChatId(new MessageData() { Text = response }, chatMessage.ChatId);
        responseMessageReadySubject.OnNext(messageDataAndChatId);
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
            var failedMessageDataAndChatId = new MessageDataAndChatId(new MessageData { Text = $"❌ Sorry I cannot roll a D{diceFaces}" }, chatMessage.ChatId);
            responseMessageReadySubject.OnNext(failedMessageDataAndChatId);
            return;
        }

        var messageDataAndChatId = new MessageDataAndChatId(new MessageData { Text = $"🎲 You rolled a D{diceFaces}: {random.Next(0, diceFaces + 1)}" }, chatMessage.ChatId);
        responseMessageReadySubject.OnNext(messageDataAndChatId);
        return;
    }

    private async Task HandleFlipCoinAsync(string _, ChatMessage chatMessage)
    {
        var flipResult = random.Next(int.MaxValue) % 2 == 0 ? "Heads" : "Tails";
        var messageDataAndChatId = new MessageDataAndChatId(new MessageData() { Text = $"🪙 You flipped a coin: {flipResult}" }, chatMessage.ChatId);
        responseMessageReadySubject.OnNext(messageDataAndChatId);
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
        var messageDataAndChatId = new MessageDataAndChatId(new MessageData() { Text = $"❓ {messageBuilder}" }, chatMessage.ChatId);
        responseMessageReadySubject.OnNext(messageDataAndChatId);
    }
}


