using System;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

public sealed class ServiceClient : IDisposable
{
    private readonly string _host;
    private readonly int _port;
    public ServiceClient(string host = "127.0.0.1", int port = 5055) { _host = host; _port = port; }

    public async Task<string> SendRawAsync(string json, CancellationToken ct = default)
    {
        using var client = new TcpClient();
        await client.ConnectAsync(_host, _port);
        using var ns = client.GetStream();
        using var sw = new StreamWriter(ns) { AutoFlush = true };
        using var sr = new StreamReader(ns);
        await sw.WriteLineAsync(json);
        return await sr.ReadLineAsync();
    }

    public void Dispose() { }
}