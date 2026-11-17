namespace DynaDocs.Serialization;

using System.Text.Json.Serialization;
using DynaDocs.Models;

[JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(SafeDeleteManifest))]
internal partial class SafeDeleteJsonContext : JsonSerializerContext;
