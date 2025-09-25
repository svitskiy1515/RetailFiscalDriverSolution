using Newtonsoft.Json.Linq;

namespace RetailFiscalDriver.Shared.Contracts;

public sealed class CommandResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public JObject? Data { get; set; }

    public static CommandResponse Ok(JObject? data = null) => new() { Success = true, Data = data };
    public static CommandResponse Fail(string error) => new() { Success = false, Error = error };
}
