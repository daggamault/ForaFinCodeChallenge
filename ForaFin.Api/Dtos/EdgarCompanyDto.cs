using Newtonsoft.Json;

namespace ForaFin.Api.Dtos;

[JsonObject(MemberSerialization.OptIn)]
public class EdgarCompanyDto
{
    [JsonProperty("cik")] public int? Cik { get; set; }

    [JsonProperty("entityName")] public string? EntityName { get; set; }

    [JsonProperty("facts")] public InfoFact? Facts { get; set; } = new();

    [JsonObject(MemberSerialization.OptIn)]
    public class InfoFact
    {
        [JsonProperty("us-gaap")] public InfoFactUsGaap? UsGaap { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class InfoFactUsGaap
    {
        [JsonProperty("netIncomeLoss")] public InfoFactUsGaapNetIncomeLoss? NetIncomeLoss { get; set; } = new();
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class InfoFactUsGaapNetIncomeLoss
    {
        [JsonProperty("units")] public InfoFactUsGaapIncomeLossUnits? Units { get; set; } = new();
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class InfoFactUsGaapIncomeLossUnits
    {
        [JsonProperty("usd")] public InfoFactUsGaapIncomeLossUnitsUsd[] Usd { get; set; } = [];
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class InfoFactUsGaapIncomeLossUnitsUsd
    {
        [JsonProperty("form")] public string? Form { get; set; }
        [JsonProperty("frame")] public string? Frame { get; set; }
        [JsonProperty("val")] public decimal? Val { get; set; }
    }
}