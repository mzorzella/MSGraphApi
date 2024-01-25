public class GraphApiDataStorageException : Exception
{
    public GraphApiDataStorageException(string message, Exception innerException)
        : base(message, innerException) { }
}
