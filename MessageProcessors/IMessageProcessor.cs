using Microsoft.VisualBasic;

public interface IMessageProcessor
{
    public Task ProcessChatMessageAsync(ChatMessage chatMessage);
    public IObservable<MessageDataAndChatId> ResponseMessageReady { get; }
}


