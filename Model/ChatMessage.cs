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
