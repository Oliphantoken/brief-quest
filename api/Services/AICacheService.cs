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
    /// <summary>
    /// Convert text into a hash string
    /// </summary>
    /// <param name="text">Brief text</param>
    /// <returns></returns>
    public static string HashBrief(string text) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(text)));


    /// <summary>
    /// Get an existing cached brief throuh 
    /// </summary>
    /// <param name="userID"></param>
    /// <param name="briefHash"></param>
    /// <returns></returns>
    public async Task<string?> GetAsync(string userID, string briefHash) =>
    (await dbContext.AICacheEntries.AsNoTracking().FirstOrDefaultAsync(
        c => c.OwnerID == userID && c.BriefHash == briefHash)
    )?.ResponseJson;


    /// <summary>
    /// Cache a brief and its Hash format
    /// </summary>
    /// <param name="userID">Owner ID</param>
    /// <param name="briefHash"></param>
    /// <param name="jsonText">The unwrapped reply (with outcomes and requirements etc) as json string</param>
    /// <returns></returns>
    public async Task SetAsync(string userID, string briefHash, string jsonText)
    {
        dbContext.AICacheEntries.Add(
            new AICacheEntry {OwnerID = userID, BriefHash = briefHash, ResponseJson = jsonText}
        );
        try
        {
            await dbContext.SaveChangesAsync();
        }catch(DbUpdateException ex) 
            when (ex.InnerException is Npgsql.PostgresException pg && pg.SqlState == "23505" )
        {
            //A cache row already exists for this user and brief hash.
            //It's harmless, so ignore the conflict
            dbContext.ChangeTracker.Clear();            
        }
    }

    
    

}
