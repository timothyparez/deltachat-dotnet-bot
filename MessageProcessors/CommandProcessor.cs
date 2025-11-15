using System.Text.RegularExpressions;
public class CommandProcessor : IMessageProcessor
{
    private static Random random = new Random();
    private readonly Dictionary<string, Func<string, ChatMessage, MessageData?>> commands;
    private static Regex commandRegex = new Regex(@"/{1}(?<command>\S*)(\s*)-{0,1}(?<args>.*)", RegexOptions.Compiled);

    public CommandProcessor()
    {
        commands = new Dictionary<string, Func<string, ChatMessage, MessageData?>>(StringComparer.OrdinalIgnoreCase)
        {
            { "rolldice", HandleRollDice},
            { "flipcoin", HandleFlipCoin},
            { "help", HandleHelp}
        };
    }

    public MessageData? ProcessChatMessage(ChatMessage chatMessage)
    {
        if (chatMessage == null || string.IsNullOrWhiteSpace(chatMessage.Text))
            return null;        
        
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
                return handler(args, chatMessage);
            }
            else
            {
                return new MessageData() { Text = $"❌ Sorry I do not understand the /{command} {args} command" };
            }
        }
        else
        {
            return new MessageData
            {
                Text = $"❌ Sorry I do not undertand the command: {chatMessage.Text}",
                Viewtype = ViewType.TEXT
            };
        }
    }

    private MessageData? HandleRollDice(string arguments, ChatMessage chatMessage)
    {
        var diceFaces = 6;
        var args = arguments.Split(' ');
        if (args.Length > 0)
        {
            if (int.TryParse(args[0].Replace("d","", StringComparison.OrdinalIgnoreCase), out var faceCount))
            {
                diceFaces = faceCount;
            }            
        }

        if (diceFaces < 3)
        {
            return new MessageData { Text = $"❌ Sorry I cannot roll a D{diceFaces}" };
        }

        return new MessageData
        {
            Text = $"🎲 You rolled a D{diceFaces}: {random.Next(0, diceFaces + 1)}"
        };
    }

    private MessageData? HandleFlipCoin(string _, ChatMessage message)
    {
        var flipResult = random.Next(int.MaxValue) % 2 == 0 ? "Heads" : "Tails" ;
        return new MessageData() { Text = $"🪙 You flipped a coin: {flipResult}"};
    }

    private MessageData? HandleHelp(string _, ChatMessage message)
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
        return new MessageData() { Text = $"❓ {messageBuilder}"};
    }
}


