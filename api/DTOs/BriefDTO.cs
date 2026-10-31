using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace BriefQuest.DTOs;

//A DTO is a (declarative) contract between the client and the _server_, since it represents
//a shared agreement about how data will be transferred and used.
public record BriefDTO( int ID, [Required][MaxLength(200)] string Title, [Required] string Text );

/// <summary>
/// The brief in its entirety.
/// </summary>
/// <param name="Title">The title of the brief.</param>
/// <param name="Text">The entire content of the brief.</param>
public record CreateBriefDTO ([Required] string Title, [Required] string Text );

public record BriefDetailDTO(int ID, string Title, string Text, List<OutcomeDetailDTO> Outcomes, List<QuestDTO> Quests);
