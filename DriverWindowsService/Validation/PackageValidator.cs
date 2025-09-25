using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System.Collections.Generic;

public static class PackageValidator
{
    private static readonly JSchema _schema =
        JSchema.Parse(File.ReadAllText("Validation/PackageSchema.json"));

    public static (bool ok, string error) Validate(string json)
    {
        var j = JToken.Parse(json);
        if (j.IsValid(_schema, out IList<string> errors)) return (true, null);
        return (false, string.Join("; ", errors));
    }
}