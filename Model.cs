public record Void();

public record SystemInfoResult([JsonProperty("arch")] string? Architecture, [JsonProperty("deltachat_core_version")] string DeltaChatCoreVersion, [JsonProperty("level")] string Level, [JsonProperty("num_cpus")] int CpuCount, [JsonProperty("sqlite_version")] string SQLiteVersion);

public record Error([JsonProperty("code")] int Code, [JsonProperty("message")] string Message);

record Request(string jsonrpc = "2.0", string method = "", int id = 1, object[]? @params = default);