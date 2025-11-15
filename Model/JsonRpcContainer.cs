public struct JsonRpcContainer<T>
{
    [JsonProperty("jsonrpc")]
    public string JsonRpcVersion { get; set; }

    [JsonProperty("id")]
    public int MessageId { get; set; }

    [JsonProperty("result")]
    public T Result { get; set; }

    [JsonProperty("error")]
    public Error Error { get; set; }
}