using BriefQuest.Utils;

namespace BriefQuest.DTOs;

public record CommandWordDTO(string Word, string PlainMeaning, BloomLevel BloomLevel, string? Example);
