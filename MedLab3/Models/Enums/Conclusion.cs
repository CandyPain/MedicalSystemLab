﻿using System.Text.Json.Serialization;

namespace MedLab3.Models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Conclusion
    {
        Disease,
        Recovery,
        Death
    }
}