using Newtonsoft.Json;


public static class AccountSetting
{
    public const string BOT = "bot";
    public const string MAIL_ADDRESS = "addr";
    public const string MAIL_PASSWORD = "mail_pw";
}

public class DeltaRpcClient
{
    private static Random random = new Random();
    private StreamWriter streamWriter;
    private StreamReader streamReader;

    public DeltaRpcClient(StreamWriter streamWriter, StreamReader streamReader)
    {
        this.streamWriter = streamWriter;
        this.streamReader = streamReader;
    }

    public SystemInfoResult GetSystemInfo() => SendAndReceive<SystemInfoResult>(DeltaChatMethodNames.METHOD_GET_SYSTEM_INFO);
    public int[] GetAllAccountIds() => SendAndReceive<int[]>("get_all_account_ids");
    public int AddAccount() => SendAndReceive<int>("add_account");
    public void ConfigureAccountSetting(int accountId, string key, string value) => SendAndReceive<Void>(DeltaChatMethodNames.METHOD_SET_CONFIG, accountId, key, value);
    public bool IsAccountConfigured(int accountId) => SendAndReceive<bool>(DeltaChatMethodNames.METHOD_IS_CONFIGURED, accountId);
    public object ConfigureAccount(int accountId) => SendAndReceive<object>(DeltaChatMethodNames.METHOD_CONFIGURE, accountId);
    public void StartIOForAccount(int accountId) => SendAndReceive<Void>(DeltaChatMethodNames.METHOD_START_IO, accountId);
    public int[] GetNextMessagesForAccount(int accountId) =>  SendAndReceive<int[]>(DeltaChatMethodNames.METHOD_GET_NEXT_MSGS, accountId);
    public object WaitForNextMessages(int accountId) => SendAndReceive<object>(DeltaChatMethodNames.METHOD_WAIT_NEXT_MSGS, accountId);

    public ChatMessage GetMessage(int accountId, int messageId) => SendAndReceive<ChatMessage>(DeltaChatMethodNames.METHOD_GET_MESSAGE, accountId, messageId);

    public void MarkMessagesAsSeen(int accountId, params int[] messageIds) => SendAndReceive<Void>(DeltaChatMethodNames.METHOD_MARKSEEN_MSGS, accountId, messageIds);

    public dynamic GetNextEvent() => SendAndReceive<dynamic>(DeltaChatMethodNames.METHOD_GET_NEXT_EVENT);
    
    public void ImportBackup(int accountId, string path, string passphrase) => SendAndReceive<Void>(DeltaChatMethodNames.METHOD_IMPORT_BACKUP, accountId, path, passphrase);

    public void SendMessage(int accountId, int chatId, MessageData messageData) => SendAndReceive<object>(DeltaChatMethodNames.METHOD_SEND_MSG, accountId, chatId, messageData);

    private T SendAndReceive<T>(string method, params object[] paremeters)
    {
        var request = new Request(method: method, id: random.Next(0, 10000), @params: paremeters);
        var requestJson = JsonConvert.SerializeObject(request);
        streamWriter.WriteLine(requestJson);        
        var responseJson = streamReader.ReadLine();
        if (String.IsNullOrWhiteSpace(responseJson))
        {
            throw new InvalidDataException("Did not receive a valid response");
        }

        var response = JsonConvert.DeserializeObject<JsonRpcContainer<T>>(responseJson);

        if (request.id != response.MessageId)
        {
            throw new InvalidDataException("The response id does not match the request id");
        }

        if (response.Error != null)
        {
            MarkupLineInterpolated($"[red]ERROR:[/] [yellow]{Spectre.Console.Markup.Escape(response.Error.ToString())}[/]");
            //throw new RpcResponseException(response.Error);
        }

        return response.Result;
    }
}
