using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

public sealed class FileSpool
{
    private readonly string _dir;
    public FileSpool(string dir) { _dir = dir; Directory.CreateDirectory(dir); }

    public string Enqueue(string json)
    {
        var name = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss_fff") + "_" +
                   RandomNumberGenerator.GetInt32(1000,9999) + ".json";
        File.WriteAllText(Path.Combine(_dir, name), json);
        return name;
    }

    public IEnumerable<string> DequeueBatch(int max = 20) =>
        Directory.EnumerateFiles(_dir, "*.json").OrderBy(f => f).Take(max);
}