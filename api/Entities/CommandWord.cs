using System;
using BriefQuest.Utils;
namespace BriefQuest.Entities;


/// <summary>
/// A class for keyowrds from Bloom's Taxonomy and what they mean/indicate for the student.
/// </summary>
public class CommandWord
{
    public required string Word { get; set; }
    public required string PlainMeaning { get; set; }
    public BloomLevel BloomLevel { get; set; }
    public string? Example { get; set; }
}
