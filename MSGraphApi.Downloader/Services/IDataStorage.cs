public interface IDataStorage
{
    Task StoreData(string fileName, string content);
}
