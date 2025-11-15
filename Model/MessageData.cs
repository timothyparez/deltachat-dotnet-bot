public class MessageData
{
    [JsonProperty("file")]
    public string? File { get; set; } = null;

    [JsonProperty("filename")]
    public string? Filename { get; set; } = null;

    [JsonProperty("html")]
    public string? Html { get; set; } = null;

    [JsonProperty("location")]
    public Location? Location { get; set; } = null;

    [JsonProperty("overridesendername")]
    public string? OverrideSenderName { get; set; } = null;

    [JsonProperty("quotedmessageid")]
    public uint? QuotedMessageId { get; set; } = null;

    [JsonProperty("quotedtext")]
    public string? QuotedText { get; set; } = null;

    [JsonProperty("text")]
    public string? Text { get; set; } = null;

    [JsonProperty("viewtype")]
    public string? Viewtype { get; set; } = ViewType.TEXT;
}
