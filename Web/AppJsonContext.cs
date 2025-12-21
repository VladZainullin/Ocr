using System.Text.Json.Serialization;
using Domain;

namespace Web;

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(ResponseModel))]
public partial class AppJsonContext : JsonSerializerContext;