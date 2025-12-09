

using M = Spectre.Console.Markup;

public class EchoMessageProcessor() : IMessageProcessor
{    
    public async Task<MessageData?> ProcessChatMessageAsync(ChatMessage chatMessage)
    {
        if (chatMessage.FromId != SpecialContactId.Self && !chatMessage.IsBot && !chatMessage.IsInfo)
        {
            MarkupLine(M.Escape(chatMessage.Text));
            var messageData = new MessageData() { Text = chatMessage.Text };
            return messageData;
        }
        else
        {            
            return null;
        }
    }

}
