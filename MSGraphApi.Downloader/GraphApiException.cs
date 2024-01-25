public class GraphApiException : Exception
{
    public GraphApiException(string message, Exception innerException)
        : base(message, innerException) { }
}
