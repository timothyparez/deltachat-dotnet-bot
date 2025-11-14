[Serializable]
public class RpcResponseException : Exception
{
    public Error? Error { get; init; }

    public RpcResponseException(Error error) => Error = error;
}
