namespace RetailFiscalDriver.Shared.Contracts;

public interface IJsonSerializer
{
    string Serialize<T>(T value);
    T? Deserialize<T>(string json);
}
