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
    public void StartIOForAccount(int accountId) => SendAndReceive<Void>(DeltaChatMethodNames.METHOD_START_IO, accountId);
    public int[] GetNextMessagesForAccount(int accountId) =>  SendAndReceive<int[]>(DeltaChatMethodNames.METHOD_GET_NEXT_MSGS, accountId);

    public ChatMessage GetMessage(int accountId, int messageId) => SendAndReceive<ChatMessage>(DeltaChatMethodNames.METHOD_GET_MESSAGE, accountId, messageId);

    public void MarkMessagesAsSeen(int accountId, params int[] messageIds) => SendAndReceive<Void>(DeltaChatMethodNames.METHOD_MARKSEEN_MSGS, accountId, messageIds);

    public dynamic GetNextEvent() => SendAndReceive<dynamic>(DeltaChatMethodNames.METHOD_GET_NEXT_EVENT);
    

    private T SendAndReceive<T>(string method, params object[] paremeters)
    {
        var request = new Request(method: method, id: random.Next(0, 1000), @params: paremeters);
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
            throw new RpcResponseException(response.Error);
        }

        return response.Result;
    }
}


//https://github.com/deltachat/deltachat-core-rust/blob/3b91815240a09ec30a84b0ccdc1e77e57be3f4cb/deltachat-rpc-client/src/deltachat_rpc_client/const.py#L25


public enum SpecialContactId
{
    //https://github.com/deltachat/deltachat-core-rust/blob/3b91815240a09ec30a84b0ccdc1e77e57be3f4cb/deltachat-rpc-client/src/deltachat_rpc_client/const.py#L18
    Self = 1,
    Info = 2,
    Device = 5,
    LastSpecial = 9
}

public static class EventType
{    
    //https://github.com/deltachat/deltachat-core-rust/blob/3b91815240a09ec30a84b0ccdc1e77e57be3f4cb/deltachat-rpc-client/src/deltachat_rpc_client/const.py#L25
    public const string INFO = "Info";
    public const string SMTP_CONNECTED = "SmtpConnected";
    public const string IMAP_CONNECTED = "ImapConnected";
    public const string SMTP_MESSAGE_SENT = "SmtpMessageSent";
    public const string IMAP_MESSAGE_DELETED = "ImapMessageDeleted";
    public const string IMAP_MESSAGE_MOVED = "ImapMessageMoved";
    public const string IMAP_INBOX_IDLE = "ImapInboxIdle";
    public const string NEW_BLOB_FILE = "NewBlobFile";
    public const string DELETED_BLOB_FILE = "DeletedBlobFile";
    public const string WARNING = "Warning";
    public const string ERROR = "Error";
    public const string ERROR_SELF_NOT_IN_GROUP = "ErrorSelfNotInGroup";
    public const string MSGS_CHANGED = "MsgsChanged";
    public const string REACTIONS_CHANGED = "ReactionsChanged";
    public const string INCOMING_MSG = "IncomingMsg";
    public const string INCOMING_MSG_BUNCH = "IncomingMsgBunch";
    public const string MSGS_NOTICED = "MsgsNoticed";
    public const string MSG_DELIVERED = "MsgDelivered";
    public const string MSG_FAILED = "MsgFailed";
    public const string MSG_READ = "MsgRead";
    public const string MSG_DELETED = "MsgDeleted";
    public const string CHAT_MODIFIED = "ChatModified";
    public const string CHAT_EPHEMERAL_TIMER_MODIFIED = "ChatEphemeralTimerModified";
    public const string CONTACTS_CHANGED = "ContactsChanged";
    public const string LOCATION_CHANGED = "LocationChanged";
    public const string CONFIGURE_PROGRESS = "ConfigureProgress";
    public const string IMEX_PROGRESS = "ImexProgress";
    public const string IMEX_FILE_WRITTEN = "ImexFileWritten";
    public const string SECUREJOIN_INVITER_PROGRESS = "SecurejoinInviterProgress";
    public const string SECUREJOIN_JOINER_PROGRESS = "SecurejoinJoinerProgress";
    public const string CONNECTIVITY_CHANGED = "ConnectivityChanged";
    public const string SELFAVATAR_CHANGED = "SelfavatarChanged";
    public const string WEBXDC_STATUS_UPDATE = "WebxdcStatusUpdate";
    public const string WEBXDC_INSTANCE_DELETED = "WebxdcInstanceDeleted";
    public const string CHATLIST_CHANGED = "ChatlistChanged";
    public const string CHATLIST_ITEM_CHANGED = "ChatlistItemChanged";
    public const string CONFIG_SYNCED = "ConfigSynced";
    public const string WEBXDC_REALTIME_DATA = "WebxdcRealtimeData";
}

public enum SpecialChatId
{
    Trash = 3,
    ArchivedLink = 6,
    AllDoneHind = 7,
    LastSpecial = 9
}

public enum ChatType 
{
    Undefined = 0,
    Single = 100,
    Group = 120,
    MailingList = 140,
    Broadcast = 160
}

public static class ChatVisibility
{
    public const string NORMAL = "Normal";
    public const string ARCHIVED = "Archived";
    public const string PINNED = "Pinned";
}

public static class DownloadState
{
    
    public const string DONE = "Done";
    public const string AVAILABLE = "Available";
    public const string FAILURE = "Failure";
    public const string IN_PROGRESS = "InProgress";
}

public static class ViewType
{
        public const string UNKNOWN = "Unknown";
        public const string TEXT = "Text";
        public const string IMAGE = "Image";
        public const string GIF = "Gif";
        public const string STICKER = "Sticker";
        public const string AUDIO = "Audio";
        public const string VOICE = "Voice";
        public const string VIDEO = "Video";
        public const string FILE = "File";
        public const string VIDEOCHAT_INVITATION = "VideochatInvitation";
        public const string WEBXDC = "Webxdc";
        public const string VCARD = "Vcard";
}

public static class SsytemMessageType
{
    public const string UNKNOWN = "Unknown";
    public const string GROUP_NAME_CHANGED = "GroupNameChanged";
    public const string GROUP_IMAGE_CHANGED = "GroupImageChanged";
    public const string MEMBER_ADDED_TO_GROUP = "MemberAddedToGroup";
    public const string MEMBER_REMOVED_FROM_GROUP = "MemberRemovedFromGroup";
    public const string AUTOCRYPT_SETUP_MESSAGE = "AutocryptSetupMessage";
    public const string SECUREJOIN_MESSAGE = "SecurejoinMessage";
    public const string LOCATION_STREAMING_ENABLED = "LocationStreamingEnabled";
    public const string LOCATION_ONLY = "LocationOnly";
    public const string CHAT_PROTECTION_ENABLED = "ChatProtectionEnabled";
    public const string CHAT_PROTECTION_DISABLED = "ChatProtectionDisabled";
    public const string WEBXDC_STATUS_UPDATE = "WebxdcStatusUpdate";
    public const string EPHEMERAL_TIMER_CHANGED = "EphemeralTimerChanged";
    public const string MULTI_DEVICE_SYNC = "MultiDeviceSync";
    public const string WEBXDC_INFO_MESSAGE = "WebxdcInfoMessage";
}

public enum MessageState
{
    Undefined = 0,
    InFresh = 10,
    InNoticed = 13,
    InSeen = 16,
    OutPreparing = 18,
    OutDraft = 19,
    OutPending = 20,
    OutFailed = 24,
    OutDelivered = 26,
    OutMdnReceived = 28 //What does MDN stand for?
}

public enum SpecialMessageId
{
    DayMarker = 9,
    LastSpecial = 9
}

public enum CertificateCheck
{
    Automatic = 0,
    Strict = 1,
    AcceptInvalidCertificated = 3,
}

public enum Connectivity 
{
    NotConnected = 1000,
    Connecting = 2000,
    Working = 3000,
    Connected = 4000,
}

public enum KeyGenType
{
    Default = 0,
    RSA2048 = 1,
    ED255519 = 2,
    RSA4096 = 3
}


public class ChatMessage
{
    [JsonProperty("chatId")]
    public int ChatId { get; set; }

    [JsonProperty("dimensionsHeight")]
    public int DimensionsHeight { get; set; }

    [JsonProperty("dimensionsWidth")]
    public int DimensionsWidth { get; set; }

    [JsonProperty("downloadState")]
    public string DownloadState { get; set; }

    [JsonProperty("duration")]
    public int Duration { get; set; }

    [JsonProperty("error")]
    public object Error { get; set; }

    [JsonProperty("file")]
    public object File { get; set; }

    [JsonProperty("fileBytes")]
    public int FileBytes { get; set; }

    [JsonProperty("fileMime")]
    public string FileMime { get; set; }

    [JsonProperty("fileName")]
    public string FileName { get; set; }

    [JsonProperty("fromId")]
    public SpecialContactId FromId { get; set; }

    [JsonProperty("hasDeviatingTimestamp")]
    public bool HasDeviatingTimestamp { get; set; }

    [JsonProperty("hasHtml")]
    public bool HasHtml { get; set; }

    [JsonProperty("hasLocation")]
    public bool HasLocation { get; set; }

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("isBot")]
    public bool IsBot { get; set; }

    [JsonProperty("isForwarded")]
    public bool IsForwarded { get; set; }

    [JsonProperty("isInfo")]
    public bool IsInfo { get; set; }

    [JsonProperty("isSetupmessage")]
    public bool IsSetupMessage { get; set; }

    [JsonProperty("overrideSenderName")]
    public object OverrideSenderName { get; set; }

    [JsonProperty("parentId")]
    public int ParentId { get; set; }

    [JsonProperty("quote")]
    public object Quote { get; set; }

    [JsonProperty("reactions")]
    public object Reactions { get; set; }

    [JsonProperty("receivedTimestamp")]
    public int ReceivedTimestamp { get; set; }

    [JsonProperty("sender")]
    public Sender Sender { get; set; }

    [JsonProperty("setupCodeBegin")]
    public object SetupCodeBegin { get; set; }

    [JsonProperty("showPadlock")]
    public bool ShowPadlock { get; set; }

    [JsonProperty("sortTimestamp")]
    public int SortTimestamp { get; set; }

    [JsonProperty("state")]
    public int State { get; set; }

    [JsonProperty("subject")]
    public string Subject { get; set; }

    [JsonProperty("systemMessageType")]
    public string SystemMessageType { get; set; }

    [JsonProperty("text")]
    public string Text { get; set; }

    [JsonProperty("timestamp")]
    public int Timestamp { get; set; }

    [JsonProperty("videochatType")]
    public object VideochatType { get; set; }

    [JsonProperty("videochatUrl")]
    public object VideochatUrl { get; set; }

    [JsonProperty("viewType")]
    public string ViewType { get; set; }

    [JsonProperty("webxdcInfo")]
    public object WebxdcInfo { get; set; }
}

public class Sender
{
    [JsonProperty("address")]
    public string Address { get; set; }

    [JsonProperty("authName")]
    public string AuthName { get; set; }

    [JsonProperty("color")]
    public string Color { get; set; }

    [JsonProperty("displayName")]
    public string DisplayName { get; set; }

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("isBlocked")]
    public bool IsBlocked { get; set; }

    [JsonProperty("isBot")]
    public bool IsBot { get; set; }

    [JsonProperty("isProfileVerified")]
    public bool IsProfileVerified { get; set; }

    [JsonProperty("isVerified")]
    public bool IsVerified { get; set; }

    [JsonProperty("lastSeen")]
    public int LastSeen { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("nameAndAddr")]
    public string NameAndAddr { get; set; }

    [JsonProperty("profileImage")]
    public object ProfileImage { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("verifierId")]
    public object VerifierId { get; set; }

    [JsonProperty("wasSeenRecently")]
    public bool WasSeenRecently { get; set; }
}
