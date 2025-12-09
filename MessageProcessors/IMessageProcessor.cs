using Microsoft.VisualBasic;

public interface IMessageProcessor
{
    public Task<MessageData?> ProcessChatMessageAsync(ChatMessage chatMessage);
}


