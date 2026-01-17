

using System.Reactive.Linq;
using System.Reactive.Subjects;
using M = Spectre.Console.Markup;

public class EchoMessageProcessor() : IMessageProcessor
{
    private Subject<MessageDataAndChatId> responseMessageReadySubject = new Subject<MessageDataAndChatId>();
    
    public IObservable<MessageDataAndChatId> ResponseMessageReady => responseMessageReadySubject.AsObservable();

    public async Task ProcessChatMessageAsync(ChatMessage chatMessage)
    {
        if (chatMessage.FromId != SpecialContactId.Self && !chatMessage.IsBot && !chatMessage.IsInfo)
        {
            MarkupLine(M.Escape(chatMessage.Text));
            var messageData = new MessageData() { Text = chatMessage.Text };            
            responseMessageReadySubject.OnNext(new MessageDataAndChatId(messageData, chatMessage.ChatId));            
        }        
    }  
}
