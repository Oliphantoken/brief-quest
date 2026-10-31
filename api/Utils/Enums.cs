using System.Text.Json.Serialization;

namespace BriefQuest.Utils;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Tier {
    Pass, Merit, Distinction
}

//[JsonConverter(typeof(JsonStringEnumConverter))]
public enum QuestStatus { TODO, DOING, PAUSED, ABANDONED, DONE }
