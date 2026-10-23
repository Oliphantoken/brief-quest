using System.Text.Json.Serialization;

namespace BriefQuest;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Tier {
    Pass, Merit, Distinction
}
