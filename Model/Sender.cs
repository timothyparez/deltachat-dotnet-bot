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
