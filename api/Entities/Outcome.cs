namespace BriefQuest.Entities;

/// <summary>
/// The learning or assessment outcome to fulfil in order to 
/// sucessfully complete the brief
/// </summary>
/// <param name="Code">The LO or AO code</param>
/// <param name="Text">The definition of the LO or AO.</param>
public class Outcome {
    public int ID { get; set; }
    public int BriefID { get; set; }
    public required Brief Brief { get; set; } = null!;
    public required string Code { get; set; }
    public required string Text { get; set; }

    public required List<Criterion> Criterion;

}
