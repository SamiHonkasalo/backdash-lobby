using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Backdash.JsonConverters;

namespace LobbyServer;

public static class Extensions
{
    public static IPAddress? GetRemoteClientIP(this HttpContext context)
    {
        var headers = context.Request.Headers;
        IPAddress? result;

        if (headers.TryGetValue("X-Real-Ip", out var clientIPHeader)
            && IPAddress.TryParse(clientIPHeader, out var clientIP))
            result = clientIP;
        else
            result = context.Connection.RemoteIpAddress;

        return result?.MapToIPv4();
    }

    public static string NormalizeName(this string name) =>
        Regex.Replace(name.Trim().ToLower(), "[^a-zA-Z0-9]", "_");

    public static string WithPrefix(this string value, string prefix) => $"{prefix}::{value}";

    static readonly JsonConverter[] customJsonConverters =
    [
        new JsonStringEnumConverter(),
        new JsonIPAddressConverter(),
        new JsonIPEndPointConverter(),
    ];

    public static void AddCustomConverters(this JsonSerializerOptions options)
    {
        foreach (var converter in customJsonConverters)
            options.Converters.Add(converter);
    }
}
