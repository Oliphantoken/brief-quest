namespace BriefQuest.DTOs;

public record BriefDTO( int ID, string Title, string Text );

/// <summary>
/// The brief in its entirety.
/// </summary>
/// <param name="Title">The title of the brief.</param>
/// <param name="Text">The entire content of the brief.</param>
public record CreateBriefDTO ( string Title, string Text );
