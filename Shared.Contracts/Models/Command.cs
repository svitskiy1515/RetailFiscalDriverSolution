using Newtonsoft.Json.Linq;

namespace RetailFiscalDriver.Shared.Contracts;

public sealed class Command
{
    public string Name { get; set; } = string.Empty;
    /// <summary>Произвольные аргументы команды (JSON-объект).</summary>
    public JObject Args { get; set; } = new JObject();
}
