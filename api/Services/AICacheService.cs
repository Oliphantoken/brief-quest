using System.Security.Cryptography;
using System.Text;
using BriefQuest.Data;
using BriefQuest.Entities;
using Microsoft.EntityFrameworkCore;

namespace BriefQuest.Services;

/// <summary>
/// A caching service to convert, get and save brief extractions to the DB
/// </summary>
/// <param name="dbContext"></param>
public class AICacheService(BriefQuestDbContext dbContext)
{
    //Convert text into a hash string
    public static string HashBrief(string text) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(text)));


    //Get the cached brief
    public async Task<string?> GetAsync(string userID, string briefHash) =>
    (await dbContext.AICacheEntries.AsNoTracking().FirstOrDefaultAsync(
        c => c.OwnerID == userID && c.BriefHash == briefHash)
    )?.ResponseJson;


    //Cache a brief
    public async Task SetAsync(string userID, string briefHash, string json)
    {
        dbContext.AICacheEntries.Add(
            new AICacheEntry {OwnerID = userID, BriefHash = briefHash, ResponseJson = json}
        );
        await dbContext.SaveChangesAsync();
    }

    
    

}
