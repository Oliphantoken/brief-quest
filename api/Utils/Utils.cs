using System.Text.Json.Serialization;

namespace BriefQuest.Utils;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Tier { Pass, Merit, Distinction }

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum QuestStatus { Todo, Doing, Paused, Abandoned, Done }

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum BloomLevel { Remember, Understand, Apply, Analyse, Evaluate, Create }