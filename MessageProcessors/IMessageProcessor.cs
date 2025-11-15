using Microsoft.VisualBasic;

public interface IMessageProcessor
{
    public MessageData? ProcessChatMessage(ChatMessage chatMessage);
}


