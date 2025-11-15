

using M = Spectre.Console.Markup;

public class EchoMessageProcessor() : IMessageProcessor
{    
    public MessageData? ProcessChatMessage(ChatMessage chatMessage)
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





