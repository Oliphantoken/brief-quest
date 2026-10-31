// namespace BriefQuest.DTOs;
// /// <summary>
// /// The learning or assessment outcome to fulfil in order to 
// /// sucessfully complete the brief
// /// </summary>
// /// <param name="Code">The LO or AO code</param>
// /// <param name="Definition">The definition of the LO or AO.</param>
// public record OutcomeDTO(int ID, string Code, string Definition);
using BriefQuest.DTOs;

public record OutcomeDetailDTO(int ID, string Code, string Text, List<CriterionDTO> Criteria);


// public record CreateOutcomeDTO(string Code, string Definition);

// public record UpdateOutcomeDTO(string Code, string Definition);
